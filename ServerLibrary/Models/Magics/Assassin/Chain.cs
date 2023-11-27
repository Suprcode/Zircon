using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Chain)]
    public class Chain : MagicObject
    {
        protected override Element Element => Element.None;

        private static readonly int LineDistance = 2;

        public Chain(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //https://www.youtube.com/watch?v=wKunzoVgrKU&t=676s
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target) || target.Race != ObjectType.Monster || ((MonsterObject)target).MonsterInfo.IsBoss)
            {
                response.Ob = null;
                return response;
            }

            if ((target.Poison & PoisonType.Chain) == PoisonType.Chain)
            {
                response.Ob = null;
                return response;
            }

            var chainedTargets = new HashSet<MonsterObject>();
            var possibleTargets = target.GetAllObjects(target.CurrentLocation, 2);

            while (chainedTargets.Count < 5 + (Magic.Level * 2))
            {
                if (possibleTargets.Count == 0) break;

                MapObject ob = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                possibleTargets.Remove(ob);

                if (target == ob) continue;

                if (!Player.CanAttackTarget(ob) || ob.Race != ObjectType.Monster || ((MonsterObject)ob).MonsterInfo.IsBoss) continue;

                if ((ob.Poison & PoisonType.Chain) == PoisonType.Chain) continue;

                if (ob.Level > Player.Level + 2) continue;

                chainedTargets.Add((MonsterObject)ob);
            }

            if (chainedTargets.Count == 0)
            {
                response.Ob = null;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(1000 + 700);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target, null, chainedTargets.ToList()));

            foreach (MapObject ob in chainedTargets)
            {
                response.Targets.Add(ob.ObjectID);
                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, ob, target, null));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MonsterObject target = (MonsterObject)data[1];
            MonsterObject leader = (MonsterObject)data[2];
            List<MonsterObject> followers = (List<MonsterObject>)data[3];

            if (target?.Node == null || target.Dead) return;

            Player.LevelMagic(Magic);

            var damage = 0;

            var chainOfFire = GetAugmentedSkill(MagicType.ChainOfFire);

            if (chainOfFire != null)
            {
                if (chainOfFire.Level >= 2)
                {
                    damage = Player.GetElementPower(target.Race, Stat.FireAttack) * 2;
                    damage -= damage * target.Stats[Stat.FireResistance] / 10;
                    damage = Math.Max(0, damage);
                }

                Player.LevelMagic(chainOfFire);
            }

            target.ApplyPoison(new Poison
            {
                Owner = Player,
                Type = PoisonType.Chain,
                TickCount = Magic.GetPower(),
                TickFrequency = TimeSpan.FromSeconds(1),
                Value = damage,
                Extra = leader,
                Extra1 = followers
            });
        }

        public static Cell CheckWalk(MapObject target, Cell cell, ref MirDirection direction)
        {
            if (target?.Node == null || target.Dead || !target.CanMove) return null;

            if (cell == null) return null;

            var p = target.PoisonList.FirstOrDefault(x => x.Type == PoisonType.Chain);

            if (p == null) return cell;

            if (p.Extra != null) //Follower with a leader
            {
                var leader = ((MapObject)p.Extra);

                var currentDistance = Functions.Distance(target.CurrentLocation, leader.CurrentLocation);
                var nextDistance = Functions.Distance(cell.Location, leader.CurrentLocation);

                if (nextDistance > LineDistance)
                {
                    if (currentDistance < nextDistance) return null;

                    var newDirection = Functions.DirectionFromPoint(cell.Location, leader.CurrentLocation);

                    cell = target.CurrentMap.GetCell(Functions.Move(target.CurrentLocation, newDirection));

                    if (cell == null) return null;

                    if (cell.IsBlocking(target, false)) return null;

                    direction = newDirection;
                }

                return cell;
            }
            
            if (p.Extra1 != null) //Leader with followers
            {
                var followers = ((List<MonsterObject>)p.Extra1);

                foreach (var follower in followers)
                {
                    if (follower?.Node == null || follower.Dead || !follower.CanMove) continue;

                    var dist = Functions.Distance(follower.CurrentLocation, cell.Location);

                    if (dist > LineDistance)
                    {
                        var dir = Functions.DirectionFromPoint(follower.CurrentLocation, cell.Location);

                        follower.Walk(dir);
                    }
                }
            }

            return cell;
        }

        public static int PoisonTick(MapObject target)
        {
            if (target?.Node == null || target.Dead) return 0;

            var p = target.PoisonList.FirstOrDefault(x => x.Type == PoisonType.Chain);

            if (p == null) return 0;

            if (p.Extra != null) //Follower with a leader
            {
                var leader = (MapObject)p.Extra;

                if (leader?.Node == null || leader.Dead)
                {
                    p.TickCount = 0; //remove chain as leader is dead
                }

                if ((p.TickCount % 4) == 0)
                {
                    return p.Value; //fire damage every 4 ticks
                }

                return 0;
            }

            if (p.Extra1 != null) //Leader with followers
            {
                var followers = (List<MonsterObject>)p.Extra1;

                if (followers.All(x => x.Node == null || x.Dead))
                {
                    p.TickCount = 0; //remove chain as no one else alive
                }
            }

            return 0;
        }

        public void SiphonDamage(MapObject target, int damage)
        {
            if (Player?.Node == null || Player.Dead) return;
            if (target?.Node == null || target.Dead || damage <= 0) return;

            var p = target.PoisonList.FirstOrDefault(x => x.Type == PoisonType.Chain);

            if (p == null) return;

            if (p.Extra1 != null) //Leader with followers
            {
                //If augment level 1+ - siphon damage from leader to followers, split it up evenly

                var chainOfFire = GetAugmentedSkill(MagicType.ChainOfFire);

                if (chainOfFire != null && chainOfFire.Level >= 1)
                {
                    var followers = (List<MonsterObject>)p.Extra1;

                    if (followers.Count < 1) return;

                    var dmg = damage / followers.Count;

                    foreach (var follower in followers)
                    {
                        if (follower?.Node == null || follower.Dead) continue;

                        follower.ChangeHP(-dmg);
                    }
                }
            }
        }

        public void Explode(MapObject target)
        {
            if (target?.Node == null || !target.Dead) return;

            var p = target.PoisonList.FirstOrDefault(x => x.Type == PoisonType.Chain);

            if (p == null) return;

            var chainOfFire = GetAugmentedSkill(MagicType.ChainOfFire);

            if (chainOfFire != null && chainOfFire.Level >= 3)
            {  
                if (p.Extra == null) //Leader
                {
                    var delay = SEnvir.Now.AddMilliseconds(800);

                    ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, MagicType.ChainOfFire, target));

                    target.Broadcast(new S.ObjectEffect { ObjectID = target.ObjectID, Effect = Effect.ChainOfFireExplode });

                    Player.LevelMagic(chainOfFire);
                }
            }
        }
    }
}

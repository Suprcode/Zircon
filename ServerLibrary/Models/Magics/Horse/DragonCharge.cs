using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DragonCharge)]
    public class DragonCharge : MagicObject
    {
        private const int RunDistance = 3;
        private const int DashRuns = 3;
        private const int DashDistance = RunDistance * DashRuns;
        private const int StepDelay = 300;

        protected override Element Element => Element.None;

        public DragonCharge(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target,
                Return = true
            };

            if (Player.Horse == HorseType.None || !Player.CanMove || (Player.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip || Player.Buffs.Exists(x => x.Type == BuffType.Dash))
            {
                Player.Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return response;
            }

            Player.Direction = direction;

            BuffInfo dash = Player.BuffAdd(BuffType.Dash, TimeSpan.FromMilliseconds(DashDistance * StepDelay), new Stats(), false, false, TimeSpan.Zero, true);

            ActionList.Add(new DelayedAction(SEnvir.Now, ActionType.DelayMagic, Type, CurrentMap, dash, 0));

            MagicConsume();
            MagicCooldown();

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Map map = (Map)data[1];
            BuffInfo dash = (BuffInfo)data[2];
            int travelled = (int)data[3];

            PoisonType movementPoison = PoisonType.Paralysis | PoisonType.WraithGrip | PoisonType.Containment | PoisonType.Binding;

            if (Player.CurrentMap != map || Player.Horse == HorseType.None || Player.Dead || !Player.Buffs.Contains(dash) ||
                (Player.Poison & movementPoison) != PoisonType.None || Player.Buffs.Exists(x => x.Type == BuffType.DragonRepulse))
            {
                CancelDash(dash);
                return;
            }

            int remaining = DashDistance - travelled;

            if (remaining <= 0)
            {
                CompleteDash(dash);
                return;
            }

            Cell destination = null;
            int distance = 0;
            int runDistance = Math.Min(RunDistance, remaining);

            for (int i = 1; i <= runDistance; i++)
            {
                Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, i));

                if (cell == null || cell.Movements?.Count > 0)
                {
                    break;
                }

                MapObject target = null;
                bool blocked = false;

                if (cell.Objects != null)
                {
                    for (int c = cell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (target != null || !CanPushTarget(ob))
                        {
                            blocked = true;
                            break;
                        }

                        target = ob;
                    }
                }

                if (blocked || target == null && cell.IsBlocking(Player, false))
                    break;

                if (target != null)
                {
                    if (target.Pushed(Direction, 1) != 1 || cell.IsBlocking(Player, false))
                        break;

                    ApplyStun(target);
                    Player.LevelMagic(Magic);

                    destination = cell;
                    distance = i;
                    continue;
                }

                destination = cell;
                distance = i;
            }

            if (distance == 0)
            {
                CompleteDash(dash);

                if (travelled == 0)
                    Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);

                return;
            }

            Player.CurrentCell = destination.GetMovement(Player);

            Player.RemoveAllObjects();
            Player.AddAllObjects();

            Player.Broadcast(new S.ObjectDash
            {
                ObjectID = Player.ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Distance = distance,
                Magic = Type,
                Continuous = true,
            });

            Player.ActionTime = SEnvir.Now + Globals.MoveTime;

            int nextTravelled = distance < runDistance ? DashDistance : travelled + distance;

            ActionList.Add(new DelayedAction(SEnvir.Now + Globals.MoveTime, ActionType.DelayMagic, Type, map, dash, nextTravelled));
        }

        private bool CanPushTarget(MapObject ob)
        {
            if (!Player.CanAttackTarget(ob)) return false;
            if (ob.Buffs.Exists(x => x.Type == BuffType.Endurance || x.Type == BuffType.DragonRepulse || x.Type == BuffType.ElementalHurricane)) return false;

            if (ob.Race == ObjectType.Monster)
            {
                MonsterObject monster = (MonsterObject)ob;
                if (monster.MonsterInfo.IsBoss || !monster.MonsterInfo.CanPush) return false;
            }

            return true;
        }

        private void ApplyStun(MapObject target)
        {
            int power = Magic.GetPower();

            target.ApplyPoison(new Poison
            {
                Type = PoisonType.Paralysis,
                TickCount = 1,
                TickFrequency = TimeSpan.FromMilliseconds(300 + power),
                Owner = Player,
            });

            target.ApplyPoison(new Poison
            {
                Type = PoisonType.Silenced,
                TickCount = 1,
                TickFrequency = TimeSpan.FromMilliseconds(300 + power * 2),
                Owner = Player,
            });
        }

        private void CompleteDash(BuffInfo dash)
        {
            CancelDash(dash);

            Player.Broadcast(new S.ObjectDash
            {
                ObjectID = Player.ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Distance = 0,
                Magic = Type,
                Continuous = true,
            });

            Player.ActionTime = SEnvir.Now.AddMilliseconds(StepDelay);
        }

        private void CancelDash(BuffInfo dash)
        {
            if (Player.Buffs.Contains(dash))
                Player.BuffRemove(dash);
        }
    }
}

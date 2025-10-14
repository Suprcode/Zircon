using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics.Warrior
{
    [MagicType(MagicType.HundredFist)]
    public class HundredFist : MagicObject
    {
        protected override Element Element => Element.None;

        public HundredFist(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Cast = true,
                Ob = target
            };

            if (!Player.CanAttackTarget(target))
            {
                response.Ob = null;
                return response;
            }

            if (!Functions.IsStraightEightDirection(Player.CurrentLocation, target.CurrentLocation))
            {
                Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                response.Ob = null;
                return response;
            }

            //TODO - change to CanMove check?
            if ((Player.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip)
            {
                return response;
            }

            var newDirection = Functions.DirectionFromPoint(Player.CurrentLocation, target.CurrentLocation);

            response.Direction = newDirection;

            CurrentMap.Broadcast(new S.MapEffect { Location = Player.CurrentLocation, Direction = newDirection, Effect = Effect.HundredFist });

            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayMagic, Type, target, newDirection));

            MagicConsume();
            MagicCooldown();

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];
            MirDirection dir = (MirDirection)data[2];

            if (target == null || target.Dead) return;

            var oppositeDirection = Functions.ShiftDirection(dir, 4);
            var nextToTarget = Functions.Move(target.CurrentLocation, oppositeDirection, 1);

            var travelled = Functions.Distance(Player.CurrentLocation, nextToTarget);

            Cell cell = CurrentMap.GetCell(nextToTarget);

            if (cell == null) return;

            Player.CurrentCell = cell.GetMovement(Player);
            Player.Direction = dir;

            Player.RemoveAllObjects();
            Player.AddAllObjects();

            Player.Broadcast(new S.ObjectTurn
            {
                ObjectID = Player.ObjectID,
                Direction = Player.Direction,
                Location = Player.CurrentLocation,
            });

            Cell nextCell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 1));

            if (nextCell == null) return;

            CurrentMap.Broadcast(new S.MapEffect { Location = nextCell.Location, Direction = oppositeDirection, Effect = Effect.HundredFistStruck });

            TargetPush(travelled, nextCell);

            Player.ActionTime = SEnvir.Now.AddMilliseconds(300);
        }

        private void TargetPush(int travelled, Cell nextCell)
        {
            var pushDistance = travelled * 2;

            if (pushDistance <= 0)
            {
                Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                return;
            }

            if (nextCell.Objects != null)
            {
                for (int c = nextCell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = nextCell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanPushTarget(Magic, ob))
                    {
                        break;
                    }

                    var pushed = TryPush(ob, pushDistance);

                    // Pushed against another object causes damage
                    if (pushed > 0 && pushed < pushDistance)
                    {
                        var damage = Player.MagicAttack(new List<MagicType> { Type }, ob, extra: pushed);

                        if (damage > 0)
                        {
                            Player.LevelMagic(Magic);
                        }
                    }
                }
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + (Player.GetDC() * extra);

            return power;
        }

        private bool CanPushTarget(UserMagic magic, MapObject ob)
        {
            if (!Player.CanAttackTarget(ob)) return false;
            if (ob.Level >= Player.Level) return false;
            if (SEnvir.Random.Next(Globals.MagicMaxLevel + 12) >= 6 + magic.Level * 3 + Player.Level - ob.Level) return false;
            if (ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return false;

            if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                return false;

            return true;
        }

        private int TryPush(MapObject ob, int distance)
        {
            return ob.Pushed(Direction, distance);
        }
    }
}

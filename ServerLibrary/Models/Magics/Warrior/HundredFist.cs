using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics.Warrior
{
    [MagicType(MagicType.HundredFist)]
    public class HundredFist : MagicObject
    {
        public override bool UpdateCombatTime => false;
        protected override Element Element => Element.None;

        public HundredFist(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Needs sound
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target,
                Return = true
            };

            //TODO - change to CanMove check?
            if ((Player.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip)
            {
                return response;
            }

            Player.Direction = direction;

            int distance = Magic.GetPower();

            var buffTime = TimeSpan.FromMilliseconds((distance + 1) * 300);

            Player.BuffAdd(BuffType.Dash, buffTime, new(), false, false, TimeSpan.Zero, true);

            ActionList.Add(new DelayedAction(SEnvir.Now, ActionType.DelayMagic, Type, distance, 0, false));

            MagicConsume();
            MagicCooldown();

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            int distance = (int)data[1];
            int travelled = (int)data[2];
            bool foundObject = (bool)data[3];

            if (!foundObject)
            {
                FreeRun(distance, travelled);
            }
            else
            {
                TargetPush(travelled);
            }
        }

        private void FreeRun(int distance, int travelled)
        {
            var remaining = distance - travelled;

            if (remaining < 0)
            {
                return;
            }

            if (!Player.Buffs.Any(x => x.Type == BuffType.Dash))
            {
                return;
            }

            Cell nextCell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 1));
            if (nextCell == null)
            {
                return;
            }

            if (nextCell.Objects == null)
            {
                if (remaining <= 0)
                {
                    return;
                }

                ++travelled;

                var delay = SEnvir.Now.AddMilliseconds(300);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, distance, travelled, false));

                FinalizeDash(true);
            }
            else
            {
                Player.BuffRemove(BuffType.Dash);

                ActionList.Add(new DelayedAction(SEnvir.Now, ActionType.DelayMagic, Type, distance, travelled, true));

                FinalizeDash(false);
            }
        }

        private void TargetPush(int travelled)
        {
            var pushDistance = travelled * 2;

            if (pushDistance <= 0)
            {
                Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                return;
            }

            Cell nextCell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 1));
            if (nextCell == null)
            {
                return;
            }

            int pushCount = 0;

            MapObject target = null;

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

                    if (TryPush(ob, pushDistance))
                    {
                        var newDir = Functions.ShiftDirection(Player.Direction, 4);
                        var newloc = Functions.Move(nextCell.Location, Direction, 1);

                        CurrentMap.Broadcast(nextCell.Location, new S.MapEffect { Location = newloc, Direction = newDir, Effect = Effect.HundredFistStruck });

                        target = ob;
                        pushCount++;
                        Player.LevelMagic(Magic);
                        continue;
                    }
                }

                if (pushCount == 0)
                {
                    //Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                }
            }
        }

        private bool CanPushTarget(UserMagic magic, MapObject ob)
        {
            if (!Player.CanAttackTarget(ob)) return false;
            if (ob.Level >= Player.Level) return false;
            if (SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Player.Level - ob.Level) return false;
            if (ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return false;

            if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                return false;

            return true;
        }

        private bool TryPush(MapObject ob, int distance)
        {
            return ob.Pushed(Direction, distance) != 0;
        }

        private void FinalizeDash(bool travel)
        {
            int distance = travel? 1 : 0;

            // Move to last travelled cell
            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, distance));
            Player.CurrentCell = cell.GetMovement(Player);

            Player.RemoveAllObjects();
            Player.AddAllObjects();

            Player.Broadcast(new S.ObjectDash
            {
                ObjectID = Player.ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Distance = distance,
                Magic = Magic.Info.Magic
            });

            Player.ActionTime = SEnvir.Now.AddMilliseconds(300);
        }
    }
}

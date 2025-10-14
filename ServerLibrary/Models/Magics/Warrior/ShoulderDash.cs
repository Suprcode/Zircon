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
    [MagicType(MagicType.ShoulderDash)]
    public class ShoulderDash : MagicObject
    {
        protected override Element Element => Element.None;

        public ShoulderDash(PlayerObject player, UserMagic magic) : base(player, magic)
        {

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

            if (Player.Buffs.Any(x => x.Type == BuffType.Dash))
            {
                return response;
            }

            Player.Direction = direction;

            int distance = Magic.GetPower();

            Player.BuffAdd(BuffType.Dash, TimeSpan.FromMilliseconds(distance * 300), new(), false, false, TimeSpan.Zero, true);

            ActionList.Add(new DelayedAction(SEnvir.Now, ActionType.DelayMagic, Type, distance, 0, null));

            MagicConsume();
            MagicCooldown();

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            int distance = (int)data[1];
            int travelled = (int)data[2];
            MagicType? augment = (MagicType?)data[3];

            MapObject target = null;

            int remainingDistance = distance - travelled;

            if (remainingDistance <= 0 || !Player.Buffs.Any(x => x.Type == BuffType.Dash))
            {
                FinalizeDash(false, Magic.Info.Magic);
                return;
            }

            Cell nextCell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 1));
            if (nextCell == null)
            {
                if (travelled == 0)
                {
                    FinalizeDash(false, Magic.Info.Magic);
                    Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                }

                Player.BuffRemove(BuffType.Dash);
                return;
            }

            bool blocked = false;
            bool stacked = false;
            MapObject stackedTarget = null;

            if (nextCell.Objects != null)
            {
                for (int c = nextCell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = nextCell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanPushTarget(Magic, ob))
                    {
                        blocked = true;
                        break;
                    }

                    if (TryPush(ob))
                    {
                        target = ob;
                        Player.LevelMagic(Magic);
                        continue;
                    }

                    // Could not push → stacking
                    stacked = true;
                    stackedTarget = ob;
                }
            }

            if (blocked)
            {
                if (travelled == 0)
                {
                    FinalizeDash(false, Magic.Info.Magic);
                    Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                }

                Player.BuffRemove(BuffType.Dash);
                return;
            }

            if (stacked)
            {
                if (Magic.Level < 3)
                {
                    Player.BuffRemove(BuffType.Dash);
                    return; // cannot push 2 mobs
                }

                Cell nextNextCell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 2));
                if (nextNextCell == null)
                {
                    Player.BuffRemove(BuffType.Dash);
                    return;
                }

                bool blockedSecond = false;

                if (nextNextCell.Objects != null)
                {
                    for (int c = nextNextCell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = nextNextCell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (!CanPushTarget(Magic, ob))
                        {
                            blockedSecond = true;
                            break;
                        }

                        if (TryPush(ob))
                        {
                            target = ob;
                            Player.LevelMagic(Magic);
                            continue;
                        }

                        blockedSecond = true;
                        break;
                    }
                }

                if (blockedSecond)
                {
                    if (travelled == 0)
                    {
                        FinalizeDash(false, Magic.Info.Magic);
                        Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
                    }

                    Player.BuffRemove(BuffType.Dash);
                    return;
                }

                // Push the stacked mob now that space is clear
                stackedTarget.Pushed(Direction, 1);
                Player.LevelMagic(Magic);
            }

            augment ??= ApplyAugment([target, stackedTarget]);

            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayMagic, Type, distance, travelled + 1, augment));

            FinalizeDash(true, augment ?? Magic.Info.Magic);
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

        private bool TryPush(MapObject ob)
        {
            return ob.Pushed(Direction, 1) == 1;
        }

        private MagicType? ApplyAugment(List<MapObject> targets)
        {
            MagicType? type = null;

            var assault = GetAugmentedSkill(MagicType.Assault);

            if (assault != null && SEnvir.Now >= assault.Cooldown)
            {
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Paralysis,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromMilliseconds(300 + assault.GetPower()),
                            Owner = Player,
                        });

                        target.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Silenced,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromMilliseconds(300 + assault.GetPower() * 2),
                            Owner = Player,
                        });

                        MagicCooldown(assault);
                        type = assault.Info.Magic;
                        Player.LevelMagic(assault);
                    }
                }
            }

            return type;
        }

        private void FinalizeDash(bool travel, MagicType type)
        {
            int distance = travel ? 1 : 0;

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
                Magic = type,
            });

            Player.ActionTime = SEnvir.Now.AddMilliseconds(300);
        }
    }
}

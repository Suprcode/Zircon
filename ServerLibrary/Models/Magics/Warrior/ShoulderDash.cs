using Library;
using Server.DBModels;
using Server.Envir;
using System;
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

            Player.Direction = direction;

            int count = ShoulderDashEnd(Magic);

            if (count == 0)
            {
                Player.Connection.ReceiveChatWithObservers(con => con.Language.DashFailed, MessageType.System);
            }

            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });
            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.ChangeMP(-Magic.Cost);

            return response;
        }

        private int ShoulderDashEnd(UserMagic magic)
        {
            int distance = magic.GetPower();

            int travelled = 0;
            Cell cell;
            MapObject target = null;

            for (int d = 1; d <= distance; d++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d));

                if (cell == null) break;

                if (cell.Objects == null)
                {
                    travelled++;
                    continue;
                }

                bool blocked = false;
                bool stacked = false;
                MapObject stackedMob = null;

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Player.Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        if (target == null) target = ob;

                        Player.LevelMagic(magic);
                        continue;
                    }

                    stacked = true;
                    stackedMob = ob;
                }

                if (blocked) break;


                if (!stacked)
                {
                    travelled++;
                    continue;
                }

                if (magic.Level < 3) break; // Cannot push 2 mobs

                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d + 1));

                if (cell == null) break; // Cannot push anymore as there is a wall or couldn't push

                //Failed to push first mob because of stacking AND its not a wall so must be mob in this cell
                if (cell.Objects != null) // Could have dashed someone through door.
                    for (int c = cell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Player.Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                        {
                            blocked = true;
                            break;
                        }

                        if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                        {
                            blocked = true;
                            continue;
                        }

                        if (ob.Pushed(Direction, 1) == 1)
                        {
                            Player.LevelMagic(magic);
                            continue;
                        }

                        blocked = true;
                        break;
                    }

                if (blocked) break; // Cannot push the two targets (either by level or wall)

                //pushed 2nd space, Now need to push the first mob
                //Should be 100% success to push stackedMob as it wasn't level nor is there a wall or mob in the way.
                stackedMob.Pushed(Direction, 1); //put this here to avoid the level / chance check
                Player.LevelMagic(magic);
                //need to check first cell again
                Point location = Functions.Move(CurrentLocation, Direction, d);
                cell = CurrentMap.Cells[location.X, location.Y];

                if (cell.Objects == null) //Might not be any more mobs on initial space after moving it
                {
                    travelled++;
                    continue;
                }

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!Player.CanAttackTarget(ob) || ob.Level >= Player.Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Player.Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        Player.LevelMagic(magic);
                        continue;
                    }

                    blocked = true;
                    break;
                }

                if (blocked) break;

                travelled++;
            }

            MagicType type = magic.Info.Magic;
            if (travelled > 0 && target != null)
            {
                var assault = GetAugmentedSkill(MagicType.Assault);

                if (assault != null && SEnvir.Now >= assault.Cooldown)
                {
                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Paralysis,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower()),
                        Owner = Player,
                    });

                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Silenced,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower() * 2),
                        Owner = Player,
                    });

                    assault.Cooldown = SEnvir.Now.AddMilliseconds(assault.Info.Delay);
                    Player.Enqueue(new S.MagicCooldown { InfoIndex = assault.Info.Index, Delay = assault.Info.Delay });
                    type = assault.Info.Magic;
                    Player.LevelMagic(assault);
                }
            }

            cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, travelled));

            Player.CurrentCell = cell.GetMovement(Player);

            Player.RemoveAllObjects();
            Player.AddAllObjects();

            Player.Broadcast(new S.ObjectDash { ObjectID = Player.ObjectID, Direction = Direction, Location = CurrentLocation, Distance = travelled, Magic = type });

            Player.ActionTime = SEnvir.Now.AddMilliseconds(300 * travelled);

            return travelled;
        }
    }
}

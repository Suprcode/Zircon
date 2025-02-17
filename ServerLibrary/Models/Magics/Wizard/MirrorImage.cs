using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;
using System.Linq;
using M = Server.Models.Monsters;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MirrorImage)]
    public class MirrorImage : MagicObject
    {
        protected override Element Element => Element.None;

        public MirrorImage(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //MagicEx - 2020
            //Magic - 2390?
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            response.Locations.Add(location);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, location));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Element element = Element.None;
            if (Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
            {
                Stats darkstoneStats = Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats;

                if (darkstoneStats.GetAffinityValue(Element.Fire) > 0)
                    element = Element.Fire;
                else if (darkstoneStats.GetAffinityValue(Element.Ice) > 0)
                    element = Element.Ice;
                else if (darkstoneStats.GetAffinityValue(Element.Lightning) > 0)
                    element = Element.Lightning;
                else if (darkstoneStats.GetAffinityValue(Element.Wind) > 0)
                    element = Element.Wind;
            }

            if (element == Element.None) return;

            if (Player.Pets.Any(x => x.MonsterInfo.Flag == MonsterFlag.MirrorImage))
                return;

            Point location = (Point)data[1];
            int count = 1;

            for (int i = 0; i < count; i++)
            {
                M.MirrorImage mob = new()
                {
                    MonsterInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MirrorImage),
                    Player = Player,
                    Direction = Direction,
                    Element = element,
                    Location = location,
                    ExplodeTime = SEnvir.Now.AddSeconds(Magic.Level > 0 ? Magic.Level * 5 : 5),
                    TameTime = SEnvir.Now.AddDays(365)
                };

                if (mob.Spawn(CurrentMap, location))
                {
                    mob.Broadcast(new S.ObjectEffect { Effect = Effect.MirrorImage, ObjectID = mob.ObjectID });

                    Player.Pets.Add(mob);
                    mob.PetOwner = Player;
                }
            }
        }
    }
}

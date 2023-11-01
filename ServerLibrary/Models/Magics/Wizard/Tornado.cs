using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Tornado)]
    public class Tornado : MagicObject
    {
        protected override Element Element => Element.Wind;

        public Tornado(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Finish skill anim and sound
            //Level 3 increases movement speed
            //Level 4 adds push back??
            //Mon-56 - 6000
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

            var info = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Tornado);

            var delay = SEnvir.Now.AddMilliseconds(400);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, location, info));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var location = (Point)data[1];
            var info = (MonsterInfo)data[2];

            Server.Models.Monsters.Tornado ob = MonsterObject.GetMonster(info) as Server.Models.Monsters.Tornado;

            if (ob == null) return;

            ob.VisibleTime = SEnvir.Now.AddSeconds(10);

            ob.Spawn(Player.CurrentMap, location);

            Player.LevelMagic(Magic);
        }
    }
}

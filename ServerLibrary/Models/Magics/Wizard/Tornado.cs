using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
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

            if (MonsterObject.GetMonster(info) is not Monsters.Tornado ob) return;

            ob.VisibleTime = SEnvir.Now.AddSeconds(10);

            int bonusScalingFactor = 10;

            Stats buffStatsRegen = new Stats
            {
                [Stat.MinDC] = Magic.Level + (Player.Stats[Stat.MinMC] / bonusScalingFactor) * Math.Max(1, Magic.Level) + Player.Stats[Stat.WindAttack],
                [Stat.MaxDC] = Magic.Level + (Player.Stats[Stat.MaxMC] / bonusScalingFactor) * Math.Max(1, Magic.Level) + Player.Stats[Stat.WindAttack],

                [Stat.Health] = (Magic.Level * 10) + (Player.Stats[Stat.Health] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.MinAC] = Magic.Level + (Player.Stats[Stat.MinAC] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.MaxAC] = Magic.Level + (Player.Stats[Stat.MaxAC] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.MinMR] = Magic.Level + (Player.Stats[Stat.MinMR] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.MaxMR] = Magic.Level + (Player.Stats[Stat.MaxMR] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.MaxMR] = Magic.Level + (Player.Stats[Stat.MaxMR] / bonusScalingFactor) * Math.Max(1, Magic.Level),
                [Stat.Agility] = 100,
                [Stat.Accuracy] = 100
            };

            ob.BuffAdd(BuffType.Tornado, TimeSpan.FromSeconds(10), buffStatsRegen, true, false, TimeSpan.Zero);

            ob.Spawn(Player.CurrentMap, location);

            Player.LevelMagic(Magic);
        }
    }
}

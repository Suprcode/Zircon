using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.StrengthOfFaith)]
    public class StrengthOfFaith : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public StrengthOfFaith(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Player.UseAmulet(5, 0))
            {
                response.Cast = false;
                return response;
            }

            response.Targets.Add(Player.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Stats buffStats = new()
            {
                [Stat.DCPercent] = (Magic.Level + 1) * -20,
                [Stat.PetDCPercent] = (Magic.Level + 1) * 30,
            };

            Player.BuffAdd(BuffType.StrengthOfFaith, TimeSpan.FromSeconds(Magic.GetPower()), buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}

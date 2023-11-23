using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.TheNewBeginning)]
    public class TheNewBeginning : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public TheNewBeginning(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            Player.BuffRemove(BuffType.Cloak);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = Player;

            if (ob?.Node == null || !Player.CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.TheNewBeginning] = Math.Min(Magic.Level + 2, Player.Stats[Stat.TheNewBeginning] + 1)
            };

            ob.BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), buffStats, false, false, TimeSpan.Zero);
        }

        public override void MagicFinalise()
        {
            
        }
    }
}

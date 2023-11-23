using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Transparency)]
    public class Transparency : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Transparency(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Player.UseAmulet(10, 0))
            {
                response.Cast = false;
                return response;
            }

            if (!Functions.InRange(CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, location));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            Point location = (Point)data[1];

            if (Player.Buffs.Any(x => x.Type == BuffType.Transparency)) return;

            //Player.Teleport(CurrentMap, location, false);

            int delay = Magic.Info.Delay;
            if (SEnvir.Now <= Player.PvPTime.AddSeconds(30))
                delay *= 10;

            Magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = delay });

            Stats buffStats = new()
            {
                [Stat.Transparency] = 1
            };

            Player.BuffAdd(BuffType.Transparency, TimeSpan.FromSeconds(Math.Min(SEnvir.Now <= Player.PvPTime.AddSeconds(30) ? 20 : 3600, Magic.GetPower() + Player.GetSC() / 2 + Player.Stats[Stat.PhantomAttack] * 2)), buffStats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }

        public override void MagicFinalise()
        {
            
        }
    }
}

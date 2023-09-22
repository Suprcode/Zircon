using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Endurance)]
    public class Endurance : MagicObject
    {
        protected override Element Element => Element.None;

        public Endurance(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
                Direction = MirDirection.Down
            };

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type));

            return response;
        }

        public override void Toggle(bool canUse)
        {
            if (Magic.Cost > Player.CurrentMP || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            Player.ChangeMP(-Magic.Cost);

            Player.BuffAdd(BuffType.Endurance, TimeSpan.FromSeconds(10 + Magic.Level * 5), null, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);

            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });
        }

        public override void MagicComplete(params object[] data)
        {
            //??
        }
    }
}

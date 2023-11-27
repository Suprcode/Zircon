using Library;
using Server.DBModels;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DefensiveBlow)]
    public class DefensiveBlow : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        public bool CanDefensiveBlow { get; private set; }
        public DateTime DefensiveBlowTime {  get; private set; }

        public DefensiveBlow(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (CanDefensiveBlow && SEnvir.Now >= DefensiveBlowTime)
            {
                CanDefensiveBlow = false;
                Player.Enqueue(new S.MagicToggle { Magic = MagicType.DefensiveBlow, CanUse = CanDefensiveBlow });

                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeExpire, Magic.Info.Name), MessageType.System);
                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Magic.Info.Name), MessageType.System);
            }
        }

        public override void Toggle(bool canUse)
        {
            if (Magic.Cost > Player.CurrentMP || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            Player.ChangeMP(-Magic.Cost);
            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });

            if (CanDefensiveBlow)
            {
                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeFail, Magic.Info.Name), MessageType.System);

                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeFail, Magic.Info.Name), MessageType.System);
            }
            else
            {
                DefensiveBlowTime = SEnvir.Now.AddSeconds(12);
                CanDefensiveBlow = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = CanDefensiveBlow });
            }
        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type || !CanDefensiveBlow)
                return response;

            CanDefensiveBlow = false;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            response.Cast = true;
            response.Magics.Add(Type);

            return response;
        }

        public override void AttackComplete(MapObject target)
        {
            var stats = new Stats
            {
                [Stat.MagicDefencePercent] = Magic.GetPower() * -1,
                [Stat.PhysicalDefencePercent] = Magic.GetPower() * -1
            };

            target.BuffAdd(BuffType.DefensiveBlow, TimeSpan.FromSeconds(10), stats, true, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }
    }
}
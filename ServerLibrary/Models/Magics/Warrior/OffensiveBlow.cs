using Library;
using Server.DBModels;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics.Warrior
{
    [MagicType(MagicType.OffensiveBlow)]
    public class OffensiveBlow : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        public bool CanOffensiveBlow { get; private set; }
        public DateTime OffensiveBlowTime { get; private set; }

        public OffensiveBlow(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (CanOffensiveBlow && SEnvir.Now >= OffensiveBlowTime)
            {
                CanOffensiveBlow = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = CanOffensiveBlow });

                Player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.ChargeExpire, Magic.Info.Name), MessageType.System);
            }
        }

        public override void Toggle(bool canUse)
        {
            if (!CheckCost() || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            MagicConsume();
            MagicCooldown();

            if (CanOffensiveBlow)
            {
                Player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.ChargeFail, Magic.Info.Name), MessageType.System);
            }
            else
            {
                OffensiveBlowTime = SEnvir.Now.AddSeconds(12);
                CanOffensiveBlow = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = CanOffensiveBlow });
            }
        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type || !CanOffensiveBlow)
                return response;

            CanOffensiveBlow = false;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            response.Cast = true;
            response.Magics.Add(Type);

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = power * Magic.GetPower() / 100;

            return power;
        }

        public override void AttackComplete(MapObject target)
        {
            if (target != null && TryPush(target, Magic.Level + 3))
            {
                target.ApplyPoison(new Poison
                {
                    Type = PoisonType.Paralysis,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    Owner = Player,
                });

                target.ApplyPoison(new Poison
                {
                    Type = PoisonType.Silenced,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    Owner = Player,
                });

                base.AttackComplete(target);
            }
        }

        private bool TryPush(MapObject ob, int distance)
        {
            return ob.Pushed(Direction, distance) != 0;
        }
    }
}

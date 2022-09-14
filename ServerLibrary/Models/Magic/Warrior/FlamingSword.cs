using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FlamingSword)]
    public class FlamingSword : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;

        public FlamingSword(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (Player.CanFlamingSword && SEnvir.Now >= Player.FlamingSwordTime)
            {
                Player.CanFlamingSword = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanFlamingSword });

                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeExpire, Player.Magics[Type].Info.Name), MessageType.System);
                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Player.Magics[Type].Info.Name), MessageType.System);
            }
        }

        public override void Toggle(bool canUse)
        {
            if (Magic.Cost > Player.CurrentMP || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            Player.ChangeMP(-Magic.Cost);
            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });

            if (Player.CanFlamingSword)
            {
                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeFail, Magic.Info.Name), MessageType.System);

                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeFail, Magic.Info.Name), MessageType.System);
            }
            else
            {
                Player.FlamingSwordTime = SEnvir.Now.AddSeconds(12);
                Player.CanFlamingSword = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanFlamingSword });
            }

            //Delay DragonRise
            if (Player.Magics.TryGetValue(MagicType.DragonRise, out UserMagic magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
            {
                magic.Cooldown = SEnvir.Now.AddSeconds(2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
            }

            //Delay BladeStorm
            if (Player.Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
            {
                magic.Cooldown = SEnvir.Now.AddSeconds(2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
            }
        }

        public override bool CanAttack(MagicType attackType)
        {
            if (attackType != Type || !Player.CanFlamingSword)
                return false;

            if (Player.Level < Magic.Info.NeedLevel1)
                return false;

            Player.CanFlamingSword = false;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            return true;
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob)
        {
            power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}

﻿using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.BladeStorm)]
    public class BladeStorm : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;

        public BladeStorm(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (Player.CanBladeStorm && SEnvir.Now >= Player.BladeStormTime)
            {
                Player.CanBladeStorm = false; ;
                Player.Enqueue(new S.MagicToggle { Magic = MagicType.BladeStorm, CanUse = Player.CanBladeStorm });

                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeExpire, Player.Magics[MagicType.BladeStorm].Info.Name), MessageType.System);
                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Player.Magics[MagicType.BladeStorm].Info.Name), MessageType.System);
            }
        }

        public override void Toggle(bool canUse)
        {
            if (Magic.Cost > Player.CurrentMP || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            Player.ChangeMP(-Magic.Cost);
            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });

            if (Player.CanBladeStorm)
            {
                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeFail, Magic.Info.Name), MessageType.System);

                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeFail, Magic.Info.Name), MessageType.System);
            }
            else
            {
                Player.BladeStormTime = SEnvir.Now.AddSeconds(12);
                Player.CanBladeStorm = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanBladeStorm });
            }
            if (Player.Magics.TryGetValue(MagicType.FlamingSword, out UserMagic magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
            {
                magic.Cooldown = SEnvir.Now.AddSeconds(2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
            }

            if (Player.Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
            {
                magic.Cooldown = SEnvir.Now.AddSeconds(2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
            }
        }

        public override bool CanAttack(MagicType attackType)
        {
            if (attackType != Type || !Player.CanBladeStorm)
                return false;

            if (Player.Level < Magic.Info.NeedLevel1)
                return false;

            Player.CanBladeStorm = false;
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
using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.DragonRise)]
    public class DragonRise : MagicObject
    {
        public override Element Element => Element.None;
        public override bool AttackSkill => true;

        public DragonRise(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void Process()
        {
            if (Player.CanDragonRise && SEnvir.Now >= Player.DragonRiseTime)
            {
                Player.CanDragonRise = false;
                Player.Enqueue(new S.MagicToggle { Magic = MagicType.DragonRise, CanUse = Player.CanDragonRise });

                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeExpire, Player.Magics[MagicType.DragonRise].Info.Name), MessageType.System);
                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Player.Magics[MagicType.DragonRise].Info.Name), MessageType.System);
            }
        }

        public override void Toggle(bool canUse)
        {
            if (Magic.Cost > Player.CurrentMP || SEnvir.Now < Magic.Cooldown || Player.Dead || (Player.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Player.Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            Player.ChangeMP(-Magic.Cost);
            Magic.Cooldown = SEnvir.Now.AddMilliseconds(Magic.Info.Delay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = Magic.Info.Delay });

            if (Player.CanDragonRise)
            {
                Player.Connection.ReceiveChat(string.Format(Player.Connection.Language.ChargeFail, Magic.Info.Name), MessageType.System);

                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeFail, Magic.Info.Name), MessageType.System);
            }
            else
            {
                Player.DragonRiseTime = SEnvir.Now.AddSeconds(12);
                Player.CanDragonRise = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = Player.CanDragonRise });
            }

            //Delay FlamingSword
            if (Player.Magics.TryGetValue(MagicType.FlamingSword, out UserMagic magic) && SEnvir.Now.AddSeconds(2) > Magic.Cooldown)
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

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type || !Player.CanDragonRise)
                return response;

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            Player.CanDragonRise = false;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            response.Cast = true;
            response.Magics.Add(Type);

            return response;
        }

        public override void AttackLocations(List<MagicType> magics)
        {
            Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, -1)), magics, false);
            Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 1)), magics, false);
            Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 2)), magics, false);
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}

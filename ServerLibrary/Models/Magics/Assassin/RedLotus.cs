using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.RedLotus)]
    public class RedLotus : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool IgnoreAccuracy => true;
        public override bool IgnorePhysicalDefense => true;
        public override int MaxLifeSteal => 1500;
        public override bool AttackSkill => true;

        public RedLotus(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type)
                return response;

            if (SEnvir.Now < Magic.Cooldown)
                return response;

            int cost = Magic.Cost;

            if (cost <= Player.CurrentMP)
            {
                Player.ChangeMP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                return response;
            }

            return response;
        }

        public override void AttackLocationSuccess(int attackDelay)
        {
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            if (Player.GetMagic(MagicType.FullBloom, out FullBloom fullBloom))
            {
                fullBloom.Magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = fullBloom.Magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            }
            if (Player.GetMagic(MagicType.WhiteLotus, out WhiteLotus whiteLotus))
            {
                whiteLotus.Magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = whiteLotus.Magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            }
            if (Player.GetMagic(MagicType.RedLotus, out RedLotus redLotus))
            {
                redLotus.Magic.Cooldown = SEnvir.Now.AddMilliseconds(redLotus.Magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = redLotus.Magic.Info.Index, Delay = redLotus.Magic.Info.Delay });
            }
            //if (Player.GetMagic(MagicType.SweetBrier, out SweetBrier sweetBrier))
            //{
            //    sweetBrier.Magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
            //    Player.Enqueue(new S.MagicCooldown { InfoIndex = sweetBrier.Magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            //}
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            bool hasStone = Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;

            int bonus = Player.GetLotusMana(ob.Race) * Magic.GetPower() / 1000;
            int res;

            power = Math.Max(0, power - ob.GetAC() + Player.GetDC()); //

            if (Player.Buffs.Any(x => x.Type == BuffType.WhiteLotus))
            {
                bonus *= 3;
                power += Math.Max(0, Player.Stats[Stat.MaxDC] - 100);
            }

            power += Math.Max(0, bonus - ob.GetMR());

            if (ob.Race == ObjectType.Player)
                res = ob.Stats.GetResistanceValue(hasStone ? Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
            else
                res = ob.Stats.GetResistanceValue(Element.None);

            if (res > 0)
                power -= power * res / 10;
            else if (res < 0)
                power -= power * res / 5;

            Player.BuffRemove(BuffType.WhiteLotus);
            Player.BuffAdd(BuffType.RedLotus, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
            ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.RedLotus });

            return power;
        }
    }
}

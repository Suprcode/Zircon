using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FullBloom)]
    public class FullBloom : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;

        public FullBloom(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CanAttack(MagicType attackType)
        {
            if (attackType != Type)
                return false;

            if (Player.Level < Magic.Info.NeedLevel1)
                return false;

            if (SEnvir.Now < Magic.Cooldown)
                return false;

            int cost = Magic.Cost;

            if (cost <= Player.CurrentMP)
            {
                Player.ChangeMP(-cost);
                return true;
            }

            return false;
        }

        public override void Cooldown(int attackDelay)
        {
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            if (Player.Magics.TryGetValue(MagicType.FullBloom, out UserMagic magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
            }
            if (Player.Magics.TryGetValue(MagicType.WhiteLotus, out magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            }
            if (Player.Magics.TryGetValue(MagicType.RedLotus, out magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            }
            if (Player.Magics.TryGetValue(MagicType.SweetBrier, out magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
            }
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob)
        {
            bool hasStone = Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;

            int bonus = Player.GetLotusMana(ob.Race) * Magic.GetPower() / 1000;
            int res;

            power = Math.Max(0, power - ob.GetAC() + Player.GetDC());

            power += Math.Max(0, bonus - ob.GetMR());

            if (ob.Race == ObjectType.Player)
                res = ob.Stats.GetResistanceValue(hasStone ? Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
            else
                res = ob.Stats.GetResistanceValue(Element.None);

            if (res > 0)
                power -= power * res / 10;
            else if (res < 0)
                power -= power * res / 5;

            Player.BuffAdd(BuffType.FullBloom, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
            ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FullBloom });

            return power;
        }
    }
}

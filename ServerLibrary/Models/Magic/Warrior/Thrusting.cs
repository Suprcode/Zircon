﻿using Library;
using Server.DBModels;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Thrusting)]
    public class Thrusting : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;
        public override bool HasAttackAnimation => false;

        public Thrusting(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void RefreshToggle()
        {
            if (Player.Character.CanThrusting && Player.Magics.ContainsKey(Type))
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
        }

        public override void Toggle(bool canUse)
        {
            Player.Character.CanThrusting = canUse;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = canUse });
        }

        public override bool CanAttack(MagicType attackType)
        {
            if (attackType != Type)
                return false;

            if (Player.Level < Magic.Info.NeedLevel1)
                return false;

            int cost = Magic.Cost;

            if (cost <= Player.CurrentMP)
            {
                Player.ChangeMP(-cost);
                return true;
            }

            return false;
        }

        public override int ModifyPower1(bool primary, int power)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}

﻿using Library;
using Server.DBModels;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.DestructiveSurge)]
    public class DestructiveSurge : MagicObject
    {
        public override Element Element => Element.None;
        public override bool PhysicalSkill => true;

        public DestructiveSurge(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void RefreshToggle()
        {
            if (Player.Character.CanDestructiveSurge && Player.Magics.ContainsKey(Type))
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
        }

        public override void Toggle(bool canUse)
        {
            Player.Character.CanDestructiveSurge = canUse;
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
                Player.DestructiveSurgeLifeSteal = 0;
                Player.ChangeMP(-cost);
                return true;
            }

            return false;
        }

        public override void Attack(List<MagicType> magics)
        {
            for (int i = 1; i < 8; i++)
                Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i)), magics, false);
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}
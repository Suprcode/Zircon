﻿using Library;
using Server.DBModels;
using System.Collections.Generic;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DestructiveSurge)]
    public class DestructiveSurge : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        public override bool HasDestructiveSurge(bool primary)
        {
            return !primary;
        }

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

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type)
                return response;

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            int cost = Magic.Cost;

            if (cost <= Player.CurrentMP)
            {
                Player.DestructiveSurgeLifeSteal = 0;
                Player.ChangeMP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                return response;
            }

            return response;
        }

        public override void AttackLocations(List<MagicType> magics)
        {
            for (int i = 1; i < 8; i++)
                Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i)), magics, false);
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}

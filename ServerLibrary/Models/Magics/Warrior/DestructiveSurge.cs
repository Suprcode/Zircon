using Library;
using Server.DBModels;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DestructiveSurge)]
    public class DestructiveSurge : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        public decimal DestructiveSurgeLifeSteal { get; private set; }

        public DestructiveSurge(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void RefreshToggle()
        {
            if (Player.Character.CanDestructiveSurge)
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

            if (attackType != Type || !Player.Character.CanDestructiveSurge)
                return response;

            int cost = Magic.Cost;

            if (cost <= Player.CurrentMP)
            {
                DestructiveSurgeLifeSteal = 0;
                Player.ChangeMP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                var augmentDestructiveSurge = GetAugmentedSkill(MagicType.AugmentDestructiveSurge);
                var hasAugmentDestructiveSurge = augmentDestructiveSurge != null && Magic.Level >= 3;

                if (hasAugmentDestructiveSurge)
                {
                    response.Magics.Add(augmentDestructiveSurge.Info.Magic);
                }

                return response;
            }

            return response;
        }

        public override void SecondaryAttackLocation(List<MagicType> magics)
        {
            for (int i = 1; i < 8; i++)
                Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i)), magics, false);

            var augmentDestructiveSurge = GetAugmentedSkill(MagicType.AugmentDestructiveSurge);
            var hasAugmentDestructiveSurge = augmentDestructiveSurge != null && Magic.Level >= 3;

            if (hasAugmentDestructiveSurge)
            {
                for (int i = 1; i < 8; i++)
                    Player.AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i), 2), magics, false);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            return power;
        }

        public override decimal LifeSteal(bool primary, decimal lifestealAmount)
        {
            if (!primary)
            {
                lifestealAmount = Math.Min(lifestealAmount, 750 - DestructiveSurgeLifeSteal);
                DestructiveSurgeLifeSteal += lifestealAmount;
            }

            return lifestealAmount;
        }
    }
}

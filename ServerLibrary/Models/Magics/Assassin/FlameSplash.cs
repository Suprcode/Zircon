using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FlameSplash)]
    public class FlameSplash : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;
        public decimal FlameSplashLifeSteal { get; private set; }

        public override bool HasFlameSplash(bool primary)
        {
            return primary;
        }

        public FlameSplash(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void RefreshToggle()
        {
            if (Player.Character.CanFlameSplash)
            {
                var canUse = true;

                if (!CanUseMagic())
                    canUse = false;

                Player.Enqueue(new S.MagicToggle { Magic = MagicType.FlameSplash, CanUse = canUse });
            }
        }

        public override void Toggle(bool canUse)
        {
            Player.Character.CanFlameSplash = canUse;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = canUse });
        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type || !Player.Character.CanFlameSplash)
                return response;

            int cost = Magic.Cost;

            var dragonWave = GetAugmentedSkill(MagicType.DragonWave);

            if (dragonWave != null)
            {
                cost -= dragonWave.Cost;
                response.Magics.Add(MagicType.DragonWave);
            }

            if (cost <= Player.CurrentMP)
            {
                FlameSplashLifeSteal = 0;
                Player.ChangeMP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                return response;
            }

            return response;
        }

        public override void SecondaryAttackLocation(List<MagicType> magics)
        {
            int count = 0;
            List<MirDirection> directions = new List<MirDirection>();

            for (int i = 0; i < 8; i++)
                directions.Add((MirDirection)i);

            directions.Remove(Direction);

            int targetCount = 4;

            var dragonWave = GetAugmentedSkill(MagicType.DragonWave);

            if (dragonWave != null && dragonWave.Level >= 3)
            {
                targetCount = 8;
            }

            while (count < targetCount)
            {
                MirDirection dir = directions[SEnvir.Random.Next(directions.Count)];

                if (Player.AttackLocation(Functions.Move(CurrentLocation, dir), magics, false))
                    count++;

                directions.Remove(dir);
                if (directions.Count == 0) break;
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
            lifestealAmount = Math.Min(lifestealAmount, 750 - FlameSplashLifeSteal);
            FlameSplashLifeSteal += lifestealAmount;

            return lifestealAmount;
        }
    }
}

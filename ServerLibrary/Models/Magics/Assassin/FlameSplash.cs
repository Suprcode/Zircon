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
        public override bool ToggleSkill => true;

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
            if (Player.Character.CanFlameSplash && Player.Magics.ContainsKey(MagicType.FlameSplash))
                Player.Enqueue(new S.MagicToggle { Magic = MagicType.FlameSplash, CanUse = true });
        }

        public override void Toggle(bool canUse)
        {
            Player.Character.CanFlameSplash = canUse;
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
                Player.FlameSplashLifeSteal = 0;
                Player.ChangeMP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                return response;
            }

            return response;
        }

        public override void AttackLocations(List<MagicType> magics)
        {
            int count = 0;
            List<MirDirection> directions = new List<MirDirection>();

            for (int i = 0; i < 8; i++)
                directions.Add((MirDirection)i);

            directions.Remove(Direction);

            while (count < 4)
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
            lifestealAmount = Math.Min(lifestealAmount, 2000 - FlameSplashLifeSteal);
            FlameSplashLifeSteal += lifestealAmount;

            return lifestealAmount;
        }
    }
}

﻿using Library;
using Library.Network.ClientPackets;
using Server.DBModels;

namespace Server.Models.Magics
{
    [MagicType(MagicType.AdvancedBloodyFlower)]
    public class AdvancedBloodyFlower : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public AdvancedBloodyFlower(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            response.Magics.Add(Type);

            return response;
        }

        public override Stats GetPassiveStats()
        {
            var stats = new Stats();

            stats[Stat.LifeSteal] += Magic.GetPower();

            return stats;
        }
    }
}

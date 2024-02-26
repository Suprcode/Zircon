using Library;
using Server.DBModels;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Thrusting)]
    public class Thrusting : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public Thrusting(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void RefreshToggle()
        {
            if (Player.Character.CanThrusting)
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
        }

        public override void Toggle(bool canUse)
        {
            Player.Character.CanThrusting = canUse;
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = canUse });
        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type || !Player.Character.CanThrusting)
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

        public override void SecondaryAttackLocation(List<MagicType> magics)
        {
            Player.AttackLocation(Functions.Move(Player.CurrentLocation, Player.Direction, 2), magics, false);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (!primary)
                power = power * Magic.GetPower() / 100;

            return power;
        }
    }
}

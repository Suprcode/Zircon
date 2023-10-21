using Library;
using Server.DBModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalHurricane)]
    public class ElementalHurricane : MagicObject
    {
        protected override Element Element => Element.None;

        public override Element GetElement(Element element)
        {
            bool hasStone = Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;

            return hasStone ? Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : base.GetElement(element);
        }

        public ElementalHurricane(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CheckCost()
        {
            int cost = Magic.Cost;
            if (Player.Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
                cost = 0;

            if (cost > Player.CurrentMP)
            {
                Player.Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return false;
            }

            return true;
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (Player.Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
            {
                Player.BuffRemove(BuffType.ElementalHurricane);
            }
            else
            {
                var buff = Player.BuffAdd(BuffType.ElementalHurricane, TimeSpan.MaxValue, null, true, false, TimeSpan.FromSeconds(1));
                buff.TickTime = TimeSpan.FromMilliseconds(500);
            }

            return response;
        }

        public override void MagicConsume()
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
                return;

            Player.ChangeMP(-(Player.Stats[Stat.Mana] * Magic.Cost / 1000));
        }

        public override void MagicComplete(params object[] data)
        {
            Cell cell = (Cell)data[1];
            bool primary = (bool)data[2];

            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                Player.MagicAttack(new List<MagicType> { Type }, ob, primary);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }

        public override int ModifyPowerMultiplier(bool primary, int power, Stats stats = null, int extra = 0)
        {
            if (!primary)
                power = (int)(power * 0.3M);

            return power;
        }
    }
}

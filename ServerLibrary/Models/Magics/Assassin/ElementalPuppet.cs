using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalPuppet)]
    public class ElementalPuppet : MagicObject
    {
        protected override Element Element => Element.None;

        public ElementalPuppet(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override Element GetElement(Element element)
        {
            bool hasStone = Player.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;

            return hasStone ? Player.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : base.GetElement(element);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            if (stats == null || stats.Count == 0) return power;

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.FireAffinity:
                    case Stat.IceAffinity:
                    case Stat.LightningAffinity:
                    case Stat.WindAffinity:
                    case Stat.HolyAffinity:
                    case Stat.DarkAffinity:
                    case Stat.PhantomAffinity:
                        power += s.Value;
                        break;
                }
            }

            return power;
        }

        public override int GetSlow(int slow, Stats stats = null)
        {
            if (stats == null || stats.Count == 0) return Slow;

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.IceAffinity:
                        return 2;
                }
            }

            return base.GetSlow(slow, stats);
        }

        public override int GetSlowLevel(int slowLevel, Stats stats = null)
        {
            if (stats == null || stats.Count == 0) return Slow;

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.IceAffinity:
                        return 3;
                }
            }

            return base.GetSlowLevel(slowLevel, stats);
        }

        public override int GetRepel(int repel, Stats stats = null)
        {
            if (stats == null || stats.Count == 0) return Repel;

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.WindAffinity:
                        return 2;
                }
            }

            return base.GetRepel(repel, stats);
        }

        public override int GetSilence(int silence, Stats stats = null)
        {
            if (stats == null || stats.Count == 0) return Silence;

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.WindAffinity:
                        return 4;
                }
            }

            return base.GetSilence(silence, stats);
        }
    }
}

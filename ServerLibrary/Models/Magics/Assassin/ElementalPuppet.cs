using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalPuppet)]
    public class ElementalPuppet : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AugmentedSkill => true;


        public ElementalPuppet(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override Element GetElement(Element currentElement, Stats stats = null)
        {
            if (stats == null || stats.Count == 0)
            {
                return currentElement;
            }

            foreach (KeyValuePair<Stat, int> s in stats.Values)
            {
                switch (s.Key)
                {
                    case Stat.FireAffinity:
                        break;
                    case Stat.IceAffinity:
                        currentElement = Element.Ice;
                        break;
                    case Stat.LightningAffinity:
                        currentElement = Element.Lightning;
                        break;
                    case Stat.WindAffinity:
                        currentElement = Element.Wind;
                        break;
                    case Stat.HolyAffinity:
                        currentElement = Element.Holy;
                        break;
                    case Stat.DarkAffinity:
                        currentElement = Element.Dark;
                        break;
                    case Stat.PhantomAffinity:
                        currentElement = Element.Phantom;
                        break;
                }
            }

            return currentElement;
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
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

            return Slow;
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

            return Slow;
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

            return Repel;
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

            return Silence;
        }
    }
}

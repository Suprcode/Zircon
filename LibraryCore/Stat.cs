using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Library.Network;

namespace Library
{
    public sealed class Stats
    {
        public SortedDictionary<Stat, int> Values { get; set; } = new SortedDictionary<Stat, int>();

        [IgnorePropertyPacket]
        public int Count => Values.Sum(pair => Math.Abs(pair.Value));

        [IgnorePropertyPacket]
        public int this[Stat stat]
        {
            get
            {
                int result;

                return !Values.TryGetValue(stat, out result) ? 0 : result;
            }
            set
            {
                if (value == 0)
                {
                    if (Values.ContainsKey(stat))
                        Values.Remove(stat);
                    return;
                }

                Values[stat] = value;
            }
        }

        public Stats()
        { }

        public Stats(Stats stats)
        {
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
                this[pair.Key] += pair.Value;
        }
        public Stats(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Values[(Stat) reader.ReadInt32()] = reader.ReadInt32();
        }
        public void Add(Stats stats, bool addElements = true)
        {
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
                switch (pair.Key)
                {
                    case Stat.FireAttack:
                    case Stat.LightningAttack:
                    case Stat.IceAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        if (addElements)
                            this[pair.Key] += pair.Value;
                        break;
                    case Stat.ItemReviveTime:
                        if (pair.Value == 0) continue;

                        if (this[pair.Key] == 0)
                            this[pair.Key] = pair.Value;
                        else
                            this[pair.Key] = Math.Min(this[pair.Key], pair.Value);
                        break;
                    default:
                        this[pair.Key] += pair.Value;
                        break;
                }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Values.Count);

            foreach (KeyValuePair<Stat, int> pair in Values)
            {
                writer.Write((int)pair.Key);
                writer.Write(pair.Value);
            }

        }

        public string GetDisplay(Stat stat)
        {
            Type type = stat.GetType();

            MemberInfo[] infos = type.GetMember(stat.ToString());

            StatDescription description = infos[0].GetCustomAttribute<StatDescription>();

            if (description == null) return null;

            List<Stat> list;
            string value;
            bool neecComma;
            switch (description.Mode)
            {
                case StatType.None:
                    return null;
                case StatType.Default:
                    return description.Title + ": " + string.Format(description.Format, this[stat]);
                case StatType.Min:
                    if (this[description.MaxStat] != 0) return null;

                    return description.Title + ": " + string.Format(description.Format, this[stat]);
                case StatType.Max:
                    return description.Title + ": " + string.Format(description.Format, this[description.MinStat], this[stat]);
                case StatType.Percent:
                    return description.Title + ": " + string.Format(description.Format, this[stat]/100D);
                case StatType.Text:
                    return description.Title;
                case StatType.Time:
                    if (this[stat] < 0)
                        return description.Title + ": Permanent";

                    return description.Title + ": " + Functions.ToString(TimeSpan.FromSeconds(this[stat]), true);
                case StatType.SpellPower:
                    if (description.MinStat == stat && this[description.MaxStat] != 0) return null;

                    if (this[Stat.MinMC] != this[Stat.MinSC] || this[Stat.MaxMC] != this[Stat.MaxSC])
                        return description.Title + ": " + string.Format(description.Format, this[description.MinStat], this[stat]);

                    if (stat != Stat.MaxSC) return null;


                    return "Spell Power: " + string.Format(description.Format, this[description.MinStat], this[stat]);
                case StatType.AttackElement:

                    list = new List<Stat>();
                    foreach (KeyValuePair<Stat, int> pair in Values)
                        if (type.GetMember(pair.Key.ToString())[0].GetCustomAttribute<StatDescription>().Mode == StatType.AttackElement) list.Add(pair.Key);

                    if (list.Count == 0 || list[0] != stat)
                        return null;

                    value = $"E. Atk: ";

                    neecComma = false;
                    foreach (Stat s in list)
                    {
                        description = type.GetMember(s.ToString())[0].GetCustomAttribute<StatDescription>();

                        if (neecComma)
                            value += $", ";

                        value += $"{description.Title} +" + this[s];
                        neecComma = true;
                    }
                    return value;
                case StatType.ElementResistance:


                    list = new List<Stat>();
                    foreach (KeyValuePair<Stat, int> pair in Values)
                    {
                        if (type.GetMember(pair.Key.ToString())[0].GetCustomAttribute<StatDescription>().Mode == StatType.ElementResistance) list.Add(pair.Key);
                    }

                    if (list.Count == 0)
                        return null;

                    bool ei;
                    bool hasAdv = false, hasDis = false;

                    foreach (Stat s in list)
                    {
                        if (this[s] > 0)
                            hasAdv = true;

                        if (this[s] < 0)
                            hasDis = true;
                    }

                    if (!hasAdv) // EV Online
                    {
                        ei = false;

                        if (list[0] != stat) return null;
                    }
                    else
                    {
                        if (!hasDis && list[0] != stat) return null;

                        ei = list[0] == stat;

                        if (!ei && list[1] != stat) return null; //Impossible to be false and have less than 2 stats.
                    }
                    

                    value = ei ? $"E. Adv: " : $"E. Dis: ";

                    neecComma = false;


                    foreach (Stat s in list)
                    {
                        description = type.GetMember(s.ToString())[0].GetCustomAttribute<StatDescription>();

                        if ((this[s] > 0) != ei) continue;

                        if (neecComma)
                            value += $", ";

                        value += $"{description.Title} x" + Math.Abs(this[s]);
                        neecComma = true;
                    }

                    return value;
                default: return null;
            }
        }


        public string GetFormat(Stat stat)
        {
            Type type = stat.GetType();

            MemberInfo[] infos = type.GetMember(stat.ToString());

            StatDescription description = infos[0].GetCustomAttribute<StatDescription>();

            if (description == null) return null;

            switch (description.Mode)
            {
                case StatType.Default:
                    return string.Format(description.Format, this[stat]);
                case StatType.Min:
                    return this[description.MaxStat] == 0 ? (string.Format(description.Format, this[stat])) : null;
                case StatType.Max:
                case StatType.SpellPower:
                    return string.Format(description.Format, this[description.MinStat], this[stat]);
                case StatType.Percent:
                    return string.Format(description.Format, this[stat] / 100D);
                case StatType.Text:
                    return description.Format;
                case StatType.Time:
                    if (this[stat] < 0)
                        return "Permanent";

                    return Functions.ToString(TimeSpan.FromSeconds(this[stat]), true);
                default: return null;
            }
        }

        public bool Compare(Stats s2)
        {
            if (Values.Count != s2.Values.Count) return false;

            foreach (KeyValuePair<Stat, int> value in Values)
                if (s2[value.Key] != value.Value) return false;

            return true;
        }

        public void Clear()
        {
            Values.Clear();
        }

        public bool HasElementalWeakness()
        {
            return 
                this[Stat.FireResistance] <= 0 && this[Stat.IceResistance] <= 0 && this[Stat.LightningResistance] <= 0 && this[Stat.WindResistance] <= 0 && 
                this[Stat.HolyResistance] <= 0 && this[Stat.DarkResistance] <= 0 &&
                this[Stat.PhantomResistance] <= 0 && this[Stat.PhysicalResistance] <= 0;

        }

        public Stat GetWeaponElement()
        {
            switch ((Element)this[Stat.WeaponElement])
            {
                case Element.Fire:
                    return Stat.FireAttack;
                case Element.Ice:
                    return Stat.IceAttack;
                case Element.Lightning:
                    return Stat.LightningAttack;
                case Element.Wind:
                    return Stat.WindAttack;
                case Element.Holy:
                    return Stat.HolyAttack;
                case Element.Dark:
                    return Stat.DarkAttack;
                case Element.Phantom:
                    return Stat.PhantomAttack;
            }

            foreach (KeyValuePair<Stat, int> pair in Values)
            {
                switch (pair.Key)
                {
                    case Stat.FireAttack:
                        return Stat.FireAttack;
                    case Stat.IceAttack:
                        return Stat.IceAttack;
                    case Stat.LightningAttack:
                        return Stat.LightningAttack;
                    case Stat.WindAttack:
                        return Stat.WindAttack;
                    case Stat.HolyAttack:
                        return Stat.HolyAttack;
                    case Stat.DarkAttack:
                        return Stat.DarkAttack;
                    case Stat.PhantomAttack:
                        return Stat.PhantomAttack;
                }
            }

            return Stat.None;
        }

        public int GetWeaponElementValue()
        {
            return this[Stat.FireAttack] + this[Stat.IceAttack] + this[Stat.LightningAttack] + this[Stat.WindAttack] + this[Stat.HolyAttack] + this[Stat.DarkAttack] + this[Stat.PhantomAttack];
        }


        public int GetElementValue(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireAttack];
                case Element.Ice:
                    return this[Stat.IceAttack];
                case Element.Lightning:
                    return this[Stat.LightningAttack];
                case Element.Wind:
                    return this[Stat.WindAttack];
                case Element.Holy:
                    return this[Stat.HolyAttack];
                case Element.Dark:
                    return this[Stat.DarkAttack];
                case Element.Phantom:
                    return this[Stat.PhantomAttack];
                default:
                    return 0;
            }

        }
        public int GetAffinityValue(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireAffinity];
                case Element.Ice:
                    return this[Stat.IceAffinity];
                case Element.Lightning:
                    return this[Stat.LightningAffinity];
                case Element.Wind:
                    return this[Stat.WindAffinity];
                case Element.Holy:
                    return this[Stat.HolyAffinity];
                case Element.Dark:
                    return this[Stat.DarkAffinity];
                case Element.Phantom:
                    return this[Stat.PhantomAffinity];
                default:
                    return 0;
            }

        }
        public int GetResistanceValue(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireResistance];
                case Element.Ice:
                    return this[Stat.IceResistance];
                case Element.Lightning:
                    return this[Stat.LightningResistance];
                case Element.Wind:
                    return this[Stat.WindResistance];
                case Element.Holy:
                    return this[Stat.HolyResistance];
                case Element.Dark:
                    return this[Stat.DarkResistance];
                case Element.Phantom:
                    return this[Stat.PhantomResistance];
                case Element.None:
                    return this[Stat.PhysicalResistance];
                default:
                    return 0;
            }

        }
        public Element GetAffinityElement()
        {
            List<Element> elements = new List<Element>();

            if (this[Stat.FireAffinity] > 0)
                elements.Add(Element.Fire);

            if (this[Stat.IceAffinity] > 0)
                elements.Add(Element.Ice);

            if (this[Stat.LightningAffinity] > 0)
                elements.Add(Element.Lightning);

            if (this[Stat.WindAffinity] > 0)
                elements.Add(Element.Wind);

            if (this[Stat.HolyAffinity] > 0)
                elements.Add(Element.Holy);

            if (this[Stat.DarkAffinity] > 0)
                elements.Add(Element.Dark);

            if (this[Stat.PhantomAffinity] > 0)
                elements.Add(Element.Phantom);

            if (elements.Count == 0) return Element.None;

            return elements[Globals.Random.Next(elements.Count)];
        }
    }

    public enum Stat
    {
        [StatDescription(Title = "Base Health", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        BaseHealth,
        [StatDescription(Title = "Base Mana", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        BaseMana,

        [StatDescription(Title = "Health", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Health,
        [StatDescription(Title = "Mana", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Mana,

        [StatDescription(Title = "AC", Format = "{0}-0", Mode = StatType.Min, MinStat = MinAC, MaxStat = MaxAC)]
        MinAC,
        [StatDescription(Title = "AC", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinAC, MaxStat = MaxAC)]
        MaxAC,
        [StatDescription(Title = "MR", Format = "{0}-0", Mode = StatType.Min, MinStat = MinMR, MaxStat = MaxMR)]
        MinMR,
        [StatDescription(Title = "MR", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinMR, MaxStat = MaxMR)]
        MaxMR,
        [StatDescription(Title = "DC", Format = "{0}-0", Mode = StatType.Min, MinStat = MinDC, MaxStat = MaxDC)]
        MinDC,
        [StatDescription(Title = "DC", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinDC, MaxStat = MaxDC)]
        MaxDC,
        [StatDescription(Title = "SP (Nature)", Format = "{0}-0", Mode = StatType.SpellPower, MinStat = MinMC, MaxStat = MaxMC)]
        MinMC,
        [StatDescription(Title = "SP (Nature)", Format = "{0}-{1}", Mode = StatType.SpellPower, MinStat = MinMC, MaxStat = MaxMC)]
        MaxMC,
        [StatDescription(Title = "SP (Spirit)", Format = "{0}-0", Mode = StatType.SpellPower, MinStat = MinSC, MaxStat = MaxSC)]
        MinSC,
        [StatDescription(Title = "SP (Spirit)", Format = "{0}-{1}", Mode = StatType.SpellPower, MinStat = MinSC, MaxStat = MaxSC)]
        MaxSC,

        [StatDescription(Title = "Accuracy", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Accuracy,
        [StatDescription(Title = "Agility", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Agility,
        [StatDescription(Title = "Attack Speed", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        AttackSpeed,

        [StatDescription(Title = "Light Radius", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Light,
        [StatDescription(Title = "Strength", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Strength, //Also known as Inten (Intensity)
        [StatDescription(Title = "Luck", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Luck, //does nothing at the moment

        [StatDescription(Title = "Fire", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        FireAttack,
        [StatDescription(Title = "Fire", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        FireResistance,

        [StatDescription(Title = "Ice", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        IceAttack,
        [StatDescription(Title = "Ice", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        IceResistance,

        [StatDescription(Title = "Lightning", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        LightningAttack,
        [StatDescription(Title = "Lightning", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        LightningResistance,
        
        [StatDescription(Title = "Wind", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        WindAttack,
        [StatDescription(Title = "Wind", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        WindResistance,
        
        [StatDescription(Title = "Holy", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        HolyAttack,
        [StatDescription(Title = "Holy", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        HolyResistance,

        [StatDescription(Title = "Dark", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        DarkAttack,
        [StatDescription(Title = "Dark", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        DarkResistance,

        [StatDescription(Title = "Phantom", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        PhantomAttack,
        [StatDescription(Title = "Phantom", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        PhantomResistance,

        [StatDescription(Title = "Comfort", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Comfort, //Regen Timer
        [StatDescription(Title = "Life Steal", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        LifeSteal,

        [StatDescription(Title = "Experience Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        ExperienceRate,
        [StatDescription(Title = "Drop Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        DropRate,
        [StatDescription(Title = "Blank Stat", Mode = StatType.None)]
        None,
        [StatDescription(Title = "Skill Rate", Format = "x{0}", Mode = StatType.Default)]
        SkillRate,

        [StatDescription(Title = "Pick Up Range", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        PickUpRadius,


        [StatDescription(Title = "Total Healing", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        Healing,
        [StatDescription(Title = "Max Heal per Tick", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        HealingCap,

        [StatDescription(Title = "Invisibility", Mode = StatType.Text)]
        Invisibility,

        [StatDescription(Title = "Affinity: Fire", Mode = StatType.Text)]
        FireAffinity,
        [StatDescription(Title = "Affinity: Ice", Mode = StatType.Text)]
        IceAffinity,
        [StatDescription(Title = "Affinity: Lightning", Mode = StatType.Text)]
        LightningAffinity,
        [StatDescription(Title = "Affinity: Wind", Mode = StatType.Text)]
        WindAffinity,
        [StatDescription(Title = "Affinity: Holy", Mode = StatType.Text)]
        HolyAffinity,
        [StatDescription(Title = "Affinity: Dark", Mode = StatType.Text)]
        DarkAffinity,
        [StatDescription(Title = "Affinity: Phantom", Mode = StatType.Text)]
        PhantomAffinity,

        [StatDescription(Title = "Reflect Damage", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        ReflectDamage,

        [StatDescription(Mode = StatType.None)]
        WeaponElement,
        [StatDescription(Title = "Temporary Innocence.", Mode = StatType.Text)]
        Redemption,
        [StatDescription(Title = "Health", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        HealthPercent,

        [StatDescription(Title = "Critical Chance", Format = "{0:+#0;-#0;#0}%", Mode = StatType.Default)]
        CriticalChance,

        [StatDescription(Title = "5% more profit when selling", Format = "{0} or more", Mode = StatType.Default)]
        SaleBonus5,
        [StatDescription(Title = "10% more profit when selling", Format = "{0} or more", Mode = StatType.Default)]
        SaleBonus10,
        [StatDescription(Title = "15% more profit when selling", Format = "{0} or more", Mode = StatType.Default)]
        SaleBonus15,
        [StatDescription(Title = "20% more profit when selling", Format = "{0} or more", Mode = StatType.Default)]
        SaleBonus20,

        [StatDescription(Title = "Magic Shield", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MagicShield,
        [StatDescription(Title = "Invisible", Mode = StatType.Text)]
        Cloak,
        [StatDescription(Title = "Cloak Damage", Format = "{0} per tick", Mode = StatType.Default)]
        CloakDamage,

        [StatDescription(Title = "New Beginning Charges", Format = "{0}", Mode = StatType.Default)]
        TheNewBeginning,

        [StatDescription(Title = "Brown, People can attack you freely", Mode = StatType.Text)]
        Brown,
        [StatDescription(Title = "PK Points:", Format = "{0}", Mode = StatType.Default)]
        PKPoint,


        [StatDescription(Title = "Global Shout no level restriction", Mode = StatType.Text)]
        GlobalShout,
        [StatDescription(Title = "MC", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MCPercent,

        [StatDescription(Title = "Chance of Judgement", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        JudgementOfHeaven,

        [StatDescription(Title = "Transparency", Mode = StatType.Text)]
        Transparency,

        [StatDescription(Title = "HP Recovery", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        CelestialLight,

        [StatDescription(Title = "MP Conversion", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        DarkConversion,

        [StatDescription(Title = "HP Recovery", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        RenounceHPLost,

        [StatDescription(Title = "Inventory Weight", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        BagWeight,
        [StatDescription(Title = "Wear Weight", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        WearWeight,
        [StatDescription(Title = "Hand Weight", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        HandWeight,

        [StatDescription(Title = "Gold Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        GoldRate,

        [StatDescription(Title = "OldDuration", Mode = StatType.Time)]
        OldDuration,
        [StatDescription(Title = "Available Hunt Gold", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        AvailableHuntGold,
        [StatDescription(Title = "Maximum Available Hunt Gold", Format = "{0:#0}", Mode = StatType.Default)]
        AvailableHuntGoldCap,
        [StatDescription(Title = "Revive Cool Down", Mode = StatType.Time)]
        ItemReviveTime,
        [StatDescription(Title = "Max Refine Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxRefineChance,

        [StatDescription(Title = "Companion Inventory Space", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        CompanionInventory,
        [StatDescription(Title = "Companion Inventory Weight", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        CompanionBagWeight,
        [StatDescription(Title = "DC", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        DCPercent,
        [StatDescription(Title = "SC", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        SCPercent,
        [StatDescription(Title = "Companion Hunger", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        CompanionHunger,

        [StatDescription(Title = "Pet's DC", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        PetDCPercent,

        [StatDescription(Title = "Locates Boss Monsters on the Map", Mode = StatType.Text)]
        BossTracker,
        [StatDescription(Title = "Locates Players on the Map", Mode = StatType.Text)]
        PlayerTracker,

        [StatDescription(Title = "Companion Rate", Format = "x{0}", Mode = StatType.Default)]
        CompanionRate,

        [StatDescription(Title = "Weight Rate", Format = "x{0}", Mode = StatType.Default)]
        WeightRate,
        [StatDescription(Title = "MinAC and MinMR have been greatly Increased.", Mode = StatType.Text)]
        Defiance,
        [StatDescription(Title = "You sacrfice your Defense for Offense.", Mode = StatType.Text)]
        Might,
        [StatDescription(Title = "Mana", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        ManaPercent,

        [StatDescription(Title = "Recall Command: @GroupRecall", Mode = StatType.Text)]
        RecallSet,

        [StatDescription(Title = "Regular Monster's Base Experience", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MonsterExperience,

        [StatDescription(Title = "Regular Monster's Base Gold", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MonsterGold,

        [StatDescription(Title = "Regular Monster's Base Drop Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MonsterDrop,

        [StatDescription(Title = "Regular Monster's Base Damage", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MonsterDamage,

        [StatDescription(Title = "Regular Monster's Base Health", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MonsterHealth,

        [StatDescription(Mode = StatType.None)]
        ItemIndex,

        [StatDescription(Title = "Improved Companion item collection.", Mode = StatType.Text)]
        CompanionCollection,
        [StatDescription(Title = "Protection Ring", Mode = StatType.Text)]
        ProtectionRing,
        [StatDescription(Mode = StatType.None)]
        ClearRing,
        [StatDescription(Mode = StatType.None)]
        TeleportRing,

        [StatDescription(Title = "Base Experience Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        BaseExperienceRate,

        [StatDescription(Title = "Base Gold Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        BaseGoldRate,

        [StatDescription(Title = "Base Drop Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        BaseDropRate,

        [StatDescription(Title = "Frost Bite Damage", Format = "{0}", Mode = StatType.Default)]
        FrostBiteDamage,

        [StatDescription(Title = "Max Regular Monster's Base Experience", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxMonsterExperience,

        [StatDescription(Title = "Max Regular Monster's Base Gold", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxMonsterGold,

        [StatDescription(Title = "Max Regular Monster's Base Drop Rate", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxMonsterDrop,

        [StatDescription(Title = "Max Regular Monster's Base Damage", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxMonsterDamage,

        [StatDescription(Title = "Max Regular Monster's Base Health", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        MaxMonsterHealth,

        [StatDescription(Title = "Critical Damage (PvE)", Format = "x{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        CriticalDamage,

        [StatDescription(Title = "Experience", Format = "{0}", Mode = StatType.Default)]
        Experience,

        [StatDescription(Title = "Death Drops Enabled.", Mode = StatType.Text)]
        DeathDrops,

        [StatDescription(Title = "Physical", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        PhysicalResistance,

        [StatDescription(Title = "Success Rate Per Fragment", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        FragmentRate,

        [StatDescription(Title = "Chance to summon map ", Mode = StatType.Text)]
        MapSummoning,

        [StatDescription(Title = "Max Frost Bite Damage", Format = "{0}", Mode = StatType.Default)]
        FrostBiteMaxDamage,

        [StatDescription(Title = "Paralysis Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        ParalysisChance,
        [StatDescription(Title = "Slow Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        SlowChance,
        [StatDescription(Title = "Silence Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        SilenceChance,
        [StatDescription(Title = "Block Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        BlockChance,
        [StatDescription(Title = "Evasion Chance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        EvasionChance,

        [StatDescription(Mode = StatType.None)]
        IgnoreStealth,
        [StatDescription(Mode = StatType.None)]
        FootballArmourAction,

        [StatDescription(Title = "Poison Resistance", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        PoisonResistance,

        [StatDescription(Title = "Rebirth ", Format = "{0}", Mode = StatType.Default)]
        Rebirth,



        [StatDescription(Title = "Duration", Mode = StatType.Time)]
        Duration = 10000,
    }

    public enum StatSource
    {
        None,
        Added,
        Refine,
        Enhancement, //Temporary Buff!?
        Other,
    }

    public enum StatType
    {
        None,
        Default,
        Min,
        Max,
        Percent,
        Text,
        AttackElement,
        ElementResistance,
        SpellPower,
        Time,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StatDescription : Attribute
    {
        public string Title { get; set; }
        public string Format { get; set; }
        public StatType Mode { get; set; }
        public Stat MinStat { get; set; }
        public Stat MaxStat { get; set; }
    }
}

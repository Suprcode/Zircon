using System;
using System.ComponentModel;

namespace Library
{
    public enum MirGender : byte
    {
        Male,
        Female
    }

    public enum MirClass : byte
    {
        Warrior,
        Wizard,
        Taoist,
        Assassin,
    }

    public enum AttackMode : byte
    {
        [Description("Attack: Peaceful")]
        Peace,
        [Description("Attack: Group")]
        Group,
        [Description("Attack: Guild")]
        Guild,
        [Description("Attack: War, Red, Brown")]
        WarRedBrown,
        [Description("Attack: All")]
        All
    }

    public enum PetMode : byte
    {
        [Description("Pet: Move, Attack")]
        Both,
        [Description("Pet: Move")]
        Move,
        [Description("Pet: Attack")]
        Attack,
        [Description("Pet: PvP")]
        PvP,
        [Description("Pet: None")]
        None,
    }

    public enum MirDirection : byte
    {
        Up = 0,
        [Description("Up Right")]
        UpRight = 1,
        Right = 2,
        [Description("Down Right")]
        DownRight = 3,
        Down = 4,
        [Description("Down Left")]
        DownLeft = 5,
        Left = 6,
        [Description("Up Left")]
        UpLeft = 7
    }

    [Flags]
    public enum RequiredClass : byte
    {
        None = 0,
        Warrior = 1,
        Wizard = 2,
        Taoist = 4,
        Assassin = 8,
        [Description("Warrior, Wizard, Taoist")]
        WarWizTao = Warrior | Wizard | Taoist,
        [Description("Wizard, Taoist")]
        WizTao = Wizard | Taoist,
        [Description("Warrior, Assassin")]
        AssWar = Warrior | Assassin,
        All = WarWizTao | Assassin
    }

    [Flags]
    public enum RequiredGender : byte
    {
        Male = 1,
        Female = 2,
        None = Male | Female
    }

    public enum EquipmentSlot
    {
        Weapon = 0,
        Armour = 1,
        Helmet = 2,
        Torch = 3,
        Necklace = 4,
        BraceletL = 5,
        BraceletR = 6,
        RingL = 7,
        RingR = 8,
        Shoes = 9,
        Poison = 10,
        Amulet = 11,
        Flower = 12,
        HorseArmour = 13,
        Emblem = 14,
        Shield = 15,
        Costume = 16,

        Hook = 17,
        Float = 18,
        Bait = 19,
        Finder = 20,
        Reel = 21
    }

    public enum CompanionSlot
    {
        Bag = 0,
        Head = 1,
        Back = 2,
        Food = 3,
    }

    [Flags]
    public enum DaysOfWeek
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Weekday = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekend = Saturday | Sunday
    }

    [Flags]
    public enum Weather
    {
        None = 0,
        Rain = 1,
        Snow = 2,
        Fog = 4,
        Lightning = 8,

        [Description("Snow, Fog")]
        SnowFog = 6,
        [Description("Rain, Lightning")]
        RainLightning = 9,
        [Description("Fog, Lightning")]
        FogLightning = 12,
        [Description("Rain, Fog, Lightning")]
        RainFogLightning = 13
    }

    public enum GridType
    {
        None,
        Inventory,
        Equipment,
        Belt,
        Repair,
        Storage,
        AutoPotion,
        RefineBlackIronOre,
        RefineAccessory,
        RefineSpecial,
        Inspect,
        Consign,
        SendMail,
        TradeUser,
        TradePlayer,
        GuildStorage,
        CompanionInventory,
        CompanionEquipment,
        WeddingRing,
        RefinementStoneIronOre,
        RefinementStoneSilverOre,
        RefinementStoneDiamond,
        RefinementStoneGoldOre,
        RefinementStoneCrystal,
        ItemFragment,
        AccessoryRefineUpgradeTarget,
        AccessoryRefineLevelTarget,
        AccessoryRefineLevelItems,
        MasterRefineFragment1,
        MasterRefineFragment2,
        MasterRefineFragment3,
        MasterRefineStone,
        MasterRefineSpecial,
        AccessoryReset,
        WeaponCraftTemplate,
        WeaponCraftYellow,
        WeaponCraftBlue,
        WeaponCraftRed,
        WeaponCraftPurple,
        WeaponCraftGreen,
        WeaponCraftGrey,
        RefineCorundumOre,
        AccessoryRefineCombTarget,
        AccessoryRefineCombItems,
        PartsStorage
    }

    public enum InventoryMode
    {
        Normal,
        Sell
    }

    public enum BuffType
    {
        None,

        Server = 1,
        HuntGold = 2,

        Observable = 3,
        Brown  = 4,
        PKPoint = 5,
        PvPCurse = 6,
        Redemption = 7,
        Companion = 8,

        Castle = 9,

        ItemBuff = 10,
        ItemBuffPermanent = 11,

        Ranking = 12,
        Developer = 13,
        Veteran = 14,

        MapEffect = 15,
        InstanceEffect = 16,
        Guild = 17,

        DeathDrops = 18,

        Fame = 19,

        //War
        Defiance = 100,
        Might = 101,
        Endurance = 102,
        ReflectDamage = 103,
        Invincibility = 104,
        DefensiveBlow = 105,

        //Wiz
        Renounce = 200,
        MagicShield = 201,
        JudgementOfHeaven = 202,
        ElementalHurricane = 203,
        SuperiorMagicShield = 204,
        FrostBite = 205,
        Tornado = 206,

        //Tao
        Heal = 300,
        Invisibility = 301,
        MagicResistance = 302,
        Resilience = 303,
        ElementalSuperiority = 304,
        BloodLust = 305,
        StrengthOfFaith = 306,
        CelestialLight = 307,
        Transparency = 308,
        LifeSteal = 309,
        Spiritualism = 310,
        SoulResonance = 311,

        //Ass
        PoisonousCloud = 400,
        FullBloom = 401, 
        WhiteLotus = 402,
        RedLotus = 403,
        Cloak = 404,
        GhostWalk = 405,
        TheNewBeginning = 406,
        DarkConversion = 407,
        DragonRepulse = 408,
        Evasion = 409,
        RagingWind = 410,
        LastStand = 411,
        Concentration = 412,

        MagicWeakness = 500,
    }

    public enum RequiredType : byte
    {
        Level,
        MaxLevel,
        AC,
        MR,
        DC,
        MC,
        SC,
        Health,
        Mana,
        Accuracy,
        Agility,
        CompanionLevel,
        MaxCompanionLevel,
        RebirthLevel,
        MaxRebirthLevel,
    }

    public enum Rarity : byte
    {
        Common,
        Superior,
        Elite,
    }

    public enum LightSetting : byte
    {
        Default,

        Light,
        Night,
        Twilight,
    }

    public enum TimeOfDay : byte
    {
        None,

        Dawn,
        Day,
        Dusk,
        Night
    }

    public enum FightSetting : byte
    {
        None,
        Safe,
        Fight,
    }

    public enum InstanceType : byte
    {
        Solo = 0,
        Group = 1,
        Guild = 2,
        Castle = 3
    }

    public enum RegionType : byte
    {
        None = 0,

        Area = 1,
        Connection = 2,
        Spawn = 3,
        Npc= 4
    }

    public enum ObjectType : byte
    {
        None, //Error

        Player,
        Item,
        NPC,
        Spell,
        Monster
    }

    public enum ItemType : byte
    {
        Nothing = 0,

        Consumable = 1,
        Weapon = 2,
        Armour = 3,
        Torch = 4,
        Helmet = 5,
        Necklace = 6,
        Bracelet = 7,
        Ring = 8,
        Shoes = 9,
        Poison = 10,
        Amulet = 11,
        Meat = 12,
        Ore = 13,
        Book = 14,
        Scroll = 15,
        [Description("Dark Stone")]
        DarkStone = 16,
        [Description("Refine Special")]
        RefineSpecial = 17,
        [Description("Horse Armour")]
        HorseArmour = 18,
        Flower = 19,
        [Description("Companion Food")]
        CompanionFood = 20,
        [Description("Companion Bag")]
        CompanionBag = 21,
        [Description("Companion Head")]
        CompanionHead = 22,
        [Description("Companion Back")]
        CompanionBack = 23,
        System = 24,
        [Description("Item Part")]
        ItemPart = 25,
        Emblem = 26,
        Shield = 27,
        Costume = 28,
        Hook = 29,
        Float = 30,
        Bait = 31,
        Finder = 32,
        Reel = 33,
        Currency = 34
    }

    public enum MirAction : byte
    {
        Standing,
        Moving,
        Pushed,
        Attack,
        RangeAttack,
        Spell,
        Harvest,
        Struck,
        Die,
        Dead,
        Show,
        Hide,
        Mount,
        Mining,
        Fishing
    }

    public enum MirAnimation : byte
    {
        Standing,
        Walking,
        CreepStanding,
        CreepWalkSlow,
        CreepWalkFast,
        Running,
        Pushed,
        Combat1,
        Combat2,
        Combat3,
        Combat4,
        Combat5,
        Combat6,
        Combat7,
        Combat8,
        Combat9,
        Combat10,
        Combat11,
        Combat12,
        Combat13,
        Combat14,
        Combat15,
        Harvest,
        Stance,
        Struck,
        Die,
        Dead,
        Skeleton,
        Show,
        Hide,

        HorseStanding,
        HorseWalking,
        HorseRunning,
        HorseStruck,

        StoneStanding,

        DragonRepulseStart,
        DragonRepulseMiddle,
        DragonRepulseEnd,

        ChannellingStart,
        ChannellingMiddle,
        ChannellingEnd,

        FishingCast,
        FishingWait,
        FishingReel
    }
    

    public enum MessageAction
    {
        None,
        Revive,
    }

    public enum MessageType
    {
        Normal,
        Shout,
        WhisperIn,
        GMWhisperIn,
        WhisperOut,
        Group,
        Global,
        Hint,
        System,
        Announcement,
        Combat,
        ObserverChat,
        Guild,
        Debug
    }

    public enum NPCDialogType
    {
        None,
        BuySell,
        Repair,
        Refine,
        RefineRetrieve,
        CompanionManage,
        WeddingRing,
        RefinementStone,
        MasterRefine,
        WeaponReset,
        ItemFragment,
        AccessoryRefineUpgrade,
        AccessoryRefineLevel,
        AccessoryReset,
        WeaponCraft,
        AccessoryRefine,

        RollDie,
        RollYut
    }

    public enum MagicSchool
    {
        None,

        Passive = 1,
        Active,
        Toggle,
        Fire,
        Ice,
        Lightning,
        Wind,
        Holy,
        Dark,
        Phantom,
        Physical,
        Atrocity,
        Kill,
        Assassination,

        Horse,

        Discipline = 20
    }
    
    public enum Element : byte
    {
        None,

        Fire,
        Ice,
        Lightning,
        Wind,
        Holy,
        Dark,
        Phantom,
    }

    public enum MagicType
    {
        None,

        Swordsmanship = 100,
        PotionMastery = 101,
        Slaying = 102,
        Thrusting = 103,
        HalfMoon = 104,
        ShoulderDash = 105,
        FlamingSword = 106,
        DragonRise = 107,
        BladeStorm = 108,
        DestructiveSurge = 109,
        Interchange = 110,
        Defiance = 111,
        Beckon = 112,
        Might = 113,
        SwiftBlade = 114,
        Assault = 115,
        Endurance = 116,
        ReflectDamage = 117,
        Fetter = 118,
        AugmentDestructiveSurge = 119,
        AugmentDefiance = 120,
        AugmentReflectDamage = 121,
        AdvancedPotionMastery = 122,
        MassBeckon = 123,
        SeismicSlam = 124,
        Invincibility = 125,
        CrushingWave = 126,
        DefensiveMastery = 127,
        PhysicalImmunity = 128,
        MagicImmunity = 129,
        DefensiveBlow = 130,
        ElementalSwords = 131,
        Shuriken = 132,

        FireBall = 201,
        LightningBall = 202,
        IceBolt = 203,
        GustBlast = 204,
        Repulsion = 205,
        ElectricShock = 206,
        Teleportation = 207,
        AdamantineFireBall = 208,
        ThunderBolt = 209,
        IceBlades = 210,
        Cyclone = 211,
        ScortchedEarth = 212,
        LightningBeam = 213,
        FrozenEarth = 214,
        BlowEarth = 215,
        FireWall = 216,
        ExpelUndead = 217,
        GeoManipulation = 218,
        MagicShield = 219,
        FireStorm = 220,
        LightningWave = 221,
        IceStorm = 222,
        DragonTornado = 223,
        GreaterFrozenEarth = 224,
        ChainLightning = 225,
        MeteorShower = 226,
        Renounce = 227,
        Tempest = 228,
        JudgementOfHeaven = 229,
        ThunderStrike = 230,
        FireBounce = 231,
        ElementalHurricane = 232,
        SuperiorMagicShield = 233,
        Burning = 234,
        Shocked = 235,
        LightningStrike = 236,
        MirrorImage = 237,//NOT CODED
        IceRain = 238,
        FrostBite = 239,
        Asteroid = 240,
        Storm = 241,//NOT CODED
        Tornado = 242,//NOT CODED

        Heal = 300,
        SpiritSword = 301,
        PoisonDust = 302,
        ExplosiveTalisman = 303,
        EvilSlayer = 304,
        Invisibility = 305,
        MagicResistance = 306,
        MassInvisibility = 307,
        GreaterEvilSlayer = 308,
        Resilience = 309,
        TrapOctagon = 310,
        CombatKick = 311,
        ElementalSuperiority = 312,
        MassHeal = 313,
        BloodLust = 314,
        Resurrection = 315,
        Purification = 316,
        Transparency = 317,
        CelestialLight = 318,
        EmpoweredHealing = 319,
        LifeSteal = 320,
        ImprovedExplosiveTalisman = 321,
        AugmentPoisonDust = 322,
        CursedDoll = 323,
        ThunderKick = 324,
        SoulResonance = 325,
        Parasite = 326,
        Spiritualism = 327,
        AugmentExplosiveTalisman = 328,
        AugmentEvilSlayer = 329,
        AugmentPurification = 330,
        AugmentResurrection = 331,
        SummonSkeleton = 332,
        SummonShinsu = 333,
        SummonJinSkeleton = 334,
        StrengthOfFaith = 335,
        SummonDemonicCreature = 336,
        DemonExplosion = 337,
        Infection = 338,
        DemonicRecovery = 339,
        Neutralize = 340,
        AugmentNeutralize = 341,
        DarkSoulPrison = 342,
        SearingLight = 343,
        AugmentCelestialLight = 344,
        CorpseExploder = 345,
        SummonDead = 346,

        WillowDance = 401,
        VineTreeDance = 402,
        Discipline = 403,
        PoisonousCloud = 404,
        FullBloom = 405,
        Cloak = 406,
        WhiteLotus = 407,
        CalamityOfFullMoon = 408,
        WraithGrip = 409,
        RedLotus = 410,
        HellFire = 411,
        PledgeOfBlood = 412,
        Rake = 413,
        SweetBrier = 414,
        SummonPuppet = 415,
        Karma = 416,
        TouchOfTheDeparted = 417,
        WaningMoon = 418,
        GhostWalk = 419,
        ElementalPuppet = 420,
        Rejuvenation = 421,
        Resolution = 422,
        ChangeOfSeasons = 423,
        Release = 424,
        FlameSplash = 425,
        BloodyFlower = 426,
        TheNewBeginning = 427,
        DanceOfSwallow = 428,
        DarkConversion = 429,
        DragonRepulse = 430,
        AdventOfDemon = 431,
        AdventOfDevil = 432,
        Abyss = 433,
        FlashOfLight = 434,
        Stealth = 435,
        Evasion = 436,
        RagingWind = 437,
        Unused = 438,//UNUSED
        Massacre = 439,
        ArtOfShadows = 440,
        DragonBlood = 441,
        FatalBlow = 442,
        LastStand = 443,
        MagicCombustion = 444,
        Vitality = 445,
        Chain = 446,
        Concentration = 447,
        DualWeaponSkills = 448,
        Containment = 449,
        DragonWave = 450,
        Hemorrhage = 451,
        BurningFire = 452,
        ChainOfFire = 453,

        MonsterScortchedEarth = 501,
        MonsterIceStorm = 502,
        MonsterDeathCloud = 503,
        MonsterThunderStorm = 504,

        SamaGuardianFire = 505,
        SamaGuardianIce = 506,
        SamaGuardianLightning = 507,
        SamaGuardianWind = 508,

        SamaPhoenixFire = 509,
        SamaBlackIce = 510,
        SamaBlueLightning = 511,
        SamaWhiteWind = 512,

        SamaProphetFire = 513,
        SamaProphetLightning = 514,
        SamaProphetWind = 515,

        DoomClawLeftPinch = 520,
        DoomClawLeftSwipe = 521,
        DoomClawRightPinch = 522,
        DoomClawRightSwipe = 523,
        DoomClawWave = 524,
        DoomClawSpit = 525,

        PinkFireBall = 530,
        GreenSludgeBall = 540,
    }

    public enum MagicProperty
    {
        None = 0,

        Active = 1,
        Passive = 2,
        Augmentation = 3,
        Toggle = 4,
        Charge = 5
    }

    //NF = No Frame
    public enum MonsterImage
    {
        None,

        //NF_StonePillar = 10,
        //NF_BlackPumpkinMan = 11,
        MutatedOctopus = 12,
        //NF_StoneBuilding13 = 13,
        StoneGolem = 14,
        NetherWorldGate = 15,
        LightArmedSoldier = 16,
        AntHealer = 17,
        ArmoredAnt = 18,
        Stomper = 19,

        ChaosKnight = 20,
        //NF_CrystalPillar = 21,
        CorpseStalker = 22,
        NumaMage = 23,
        AntSoldier = 24,
        //NF_StoneBuilding25 = 25,
        //NF_StoneBuilding26 = 26,
        NumaElite = 27,
        //NF_Phantom = 28,
        CrimsonNecromancer = 29,

        Chicken = 30,
        Deer = 31,
        //NF_Man1 = 32,
        Oma = 33,
        OmaHero = 34,
        SpittingSpider = 35,
        Guard = 36,
        OmaWarlord = 37,
        Scorpion = 38,
        CaveBat = 39,

        ForestYeti = 40,
        CarnivorousPlant = 41,
        Skeleton = 42,
        SkeletonAxeThrower = 43,
        SkeletonAxeMan = 44,
        SkeletonWarrior = 45,
        SkeletonLord = 46,
        CaveMaggot = 47,
        ClawCat = 48,
        //NF_KoreanFlag = 49,

        Scarecrow = 50,
        UmaInfidel = 51,
        BloodThirstyGhoul = 52,
        UmaFlameThrower = 53,
        UmaAnguisher = 54,
        UmaKing = 55,
        SpinedDarkLizard = 56,
        //NF_Dung = 57,
        GhostSorcerer = 58,
        GhostMage = 59,

        VoraciousGhost = 60,
        DevouringGhost = 61,
        CorpseRaisingGhost = 62,
        GhoulChampion = 63,
        RedSnake = 64,
        //NF_KatanaGuard = 65,
        WhiteBone = 66,
        TigerSnake = 67,
        Sheep = 68,
        SkyStinger = 69,

        ShellNipper = 70,
        VisceralWorm = 71,
        //NF_KingScorpion = 72,
        Beetle = 73,
        SpikedBeetle = 74,
        Wolf = 75,
        Centipede = 76,
        LordNiJae = 77,
        MutantMaggot = 78,
        Earwig = 79,

        IronLance = 80,
        WasHatchling = 81,
        ButterflyWorm = 82,
        WedgeMothLarva = 83,
        LesserWedgeMoth = 84,
        WedgeMoth = 85,
        RedBoar = 86,
        BlackBoar = 87,
        TuskLord = 88,
        ClawSerpent = 89,

        EvilSnake = 90,
        ViciousRat = 91,
        ZumaSharpShooter = 92,
        ZumaFanatic = 93,
        ZumaGuardian = 94,
        ZumaKing = 95,
        ArcherGuard = 96,
        //NF_DemonGuardMace = 97,
        //NF_DemonGuardSword = 98,
        Shinsu = 99, //Small

        Shinsu1 = 100, //Large
        //NF_UmaMaceInfidel = 101,
        AquaLizard = 102,
        CorrosivePoisonSpitter = 103,
        SandShark = 104,
        CursedCactus = 105,
        AntNeedler = 106,
        WindfurySorceress = 107,
        //NF_NumaMounted = 108,
        PhantomSoldier = 109,

        //NF_FoxWarrior = 110,
        SpiderBat = 111,
        //NF_FoxTaoist = 112,
        //NF_FoxWizard = 113,
        RedMoonTheFallen = 114,
        Larva = 115,
        ArachnidGazer = 116,
        RedMoonGuardian = 117,
        RedMoonProtector = 118,
        //NF_RedMoonRedProtector = 119,

        //NF_RedMoonGrayProtector = 120,
        VenomousArachnid = 121,
        DarkArachnid = 122,
        ForestGuard = 123,
        TownGuard = 124,
        SandGuard = 125,
        //NF_Blank126 = 126,
        //NF_Blank127 = 127,
        //NF_Blank128 = 128,
        Pig = 129,

        PachonTheChaosBringer = 130,
        Cow = 131,
        //NF_NumaAxeman = 132,
        //NF_Football = 133,
        //NF_HermitFemale = 134,
        //NF_HermitMale = 135,
        //NF_WhiteSnake = 136,
        ChestnutTree = 137,
        NumaGrunt = 138,
        NumaWarrior = 139,

        BanyaRightGuard = 140,
        BanyaLeftGuard = 141,
        DecayingGhoul = 142,
        FrostMinotaur = 143,
        ShockMinotaur = 144,
        FuryMinotaur = 145,
        FlameMinotaur = 146,
        Minotaur = 147,
        RottingGhoul = 148,
        EmperorSaWoo = 149,

        BoneCaptain = 150,
        ArchLichTaedu = 151,
        BoneSoldier = 152,
        BoneBladesman = 153,
        BoneArcher = 154,
        MutantFlea = 155,
        //NF_PurpleFlea = 156,
        BlasterMutantFlea = 157,
        //NF_BlueBlasterMutantFlea = 158,
        PoisonousMutantFlea = 159,

        RazorTusk = 160,
        //NF_Reindeer = 161,
        //NF_EvilScorpion = 162,
        //NF_ChristmasTree = 163,
        Monkey = 164,
        //NF_Santa = 165,
        CannibalFanatic = 166,
        EvilFanatic = 167,
        EvilElephant = 168,
        FlameGriffin = 169,

        StoneGriffin = 170,
        MutantCaptain = 171,
        PinkGoddess = 172,
        GreenGoddess = 173,
        JinchonDevil = 174,
        //NF_JungleAxeman = 175,
        //NF_JungleClubman = 176,
        //NF_Catapult177 = 177,
        //NF_Blank178 = 178,
        //NF_Catapult179 = 179,

        IcyGoddess = 180,
        WildBoar = 181,
        //NF_AngelGuardian = 182,
        //NF_Blank183 = 183,
        //NF_NumaElder = 184,
        //NF_Blank185 = 185,
        //NF_Blank186 = 186,
        //NF_NumaPriest = 187,
        //NF_Blank188 = 188,
        //NF_BonePile189 = 189,

        NumaCavalry = 190,
        NumaArmoredSoldier = 191,
        //NF_NumaAxeSoldier = 192,
        NumaStoneThrower = 193,
        NumaHighMage = 194,
        NumaRoyalGuard = 195,
        //NF_NumaWarlord = 196,
        BloodStone = 197,
        //NF_Chest = 198,
        //NF_BonePile199 = 199,

        //NF_Snowman = 200,
        RagingLizard = 201,
        SawToothLizard = 202,
        MutantLizard = 203,
        VenomSpitter = 204,
        SonicLizard = 205,
        GiantLizard = 206,
        TaintedTerror = 207,
        DeathLordJichon = 208,
        CrazedLizard = 209,

        IcyRanger = 210,
        FerociousIceTiger = 211,
        IcySpiritWarrior = 212,
        IcySpiritGeneral = 213,
        GhostKnight = 214,
        FrostLordHwa = 215,
        IcySpiritSpearman = 216,
        Werewolf = 217,
        Whitefang = 218,
        IcySpiritSolider = 219,

        EscortCommander = 220,
        QueenOfDawn = 221,
        FieryDancer = 222,
        EmeraldDancer = 223,
        //NF_Blank224 = 224,
        //NF_Blank225 = 225,
        //NF_Blank226 = 226,
        //NF_Blank227 = 227,
        //NF_Blank228 = 228,
        //NF_Blank229 = 229,

        ChiwooGeneral = 230,
        DragonLord = 231,
        DragonQueen = 232,
        OYoungBeast = 233,
        MaWarlord = 234,
        YumgonGeneral = 235,
        YumgonWitch = 236,
        JinhwanSpirit = 237,
        JinhwanGuardian = 238,
        JinamStoneGate = 239,

        //Mon24

        SamaFireGuardian = 250,
        SamaIceGuardian = 251,
        SamaLightningGuardian = 252,
        SamaWindGuardian = 253,
        Phoenix = 254,
        BlackTortoise = 255,
        BlueDragon = 256,
        WhiteTiger = 257,

        InfernalSoldier = 260,
        //NF_Blank261 = 261,
        //NF_Blank262 = 262,
        //NF_Blank263 = 263,
        //NF_Blank264 = 264,
        //NF_Blank265 = 265,
        //NF_Blank266 = 266,
        //NF_Blank267 = 267,
        //NF_Blank268 = 268,
        //NF_Blank269 = 269,

        SamaCursedBladesman = 270,
        SamaCursedSlave = 271,
        SamaCursedFlameMage = 272,
        SamaProphet = 273,
        SamaSorcerer = 274,
        EnshrinementBox = 275,
        //NF_AssassinMale = 276,
        //NF_AssassinFemale = 277,
        //NF_UmaMaceSoldier = 278,
        //NF_Blank279 = 279,

        Salamander = 280,
        SandGolem = 281,
        //NF_NumaLoneGuard = 282,
        //NF_SmallSpider = 283,
        OmaInfant = 284,
        Yob = 285,
        RakingCat = 286,
        //NF_UmaTrident = 287,
        GangSpider = 288,
        VenomSpider = 289,

        SDMob4 = 290,
        SDMob5 = 291,
        SDMob6 = 292,
        //NF_SpiritSpider = 293,
        //NF_DarkMage = 294,
        //NF_Lizard = 295,
        //NF_DarkDevil = 296,
        //NF_NumaSoldier = 297,
        SDMob7 = 298,
        OmaMage = 299,

        WildMonkey = 300,
        FrostYeti = 301,
        //NF_SnakeTower = 302,
        //NF_Duck = 303,
        //NF_Rabbit = 304,
        //NF_BonePile305 = 305,
        //NF_BigFootball = 306,
        //NF_BluePumpkinMan = 307,
        //NF_RedPumpkinMan = 308,
        //NF_Blank309 = 309,

        //Mon31

        SDMob8 = 320,
        SDMob9 = 321,
        //NF_BlueMouseWithTail = 322,
        //NF_VampireDagger = 323,
        //NF_VampireSpear = 324,
        SDMob10 = 325,
        SDMob11 = 326,
        SDMob12 = 327,
        SDMob13 = 328,
        SDMob14 = 329,

        //Mon33

        Companion_Pig = 340,
        Companion_TuskLord = 341,
        Companion_SkeletonLord = 342,
        Companion_Griffin = 343,
        Companion_Dragon = 344,
        Companion_Donkey = 345,
        Companion_Sheep = 346,
        Companion_BanyoLordGuzak = 347,
        Companion_Panda = 348,
        Companion_Rabbit = 349,

        OrangeTiger = 350,
        RegularTiger = 351,
        RedTiger = 352,
        SnowTiger = 353,
        BlackTiger = 354,
        BigBlackTiger = 355,
        BigWhiteTiger = 356,
        OrangeBossTiger = 357,
        BigBossTiger = 358,
        //NF_Blank359 = 359,

        //Mon36

        //NF_YurinMon0 = 370,
        //NF_YurinMon1 = 371,
        //NF_WhiteBeardedTiger = 372,
        //NF_BlackBeardedTiger = 373,
        //NF_HardenedRhino = 374,
        //NF_Mammoth = 375,
        //NF_CursedSlave1 = 376,
        //NF_CursedSlave2 = 377,
        //NF_CursedSlave3 = 378,
        //NF_PoisonousGolem = 379,

        //NF_GardenSoldier = 380,
        //NF_GardenDefender = 381,
        //NF_RedBlossom = 382,
        //NF_BlueBlossom = 383,
        //NF_FireBird = 384,
        //NF_BlueGorilla = 385,
        //NF_RedGorilla = 386,
        //NF_Blossom = 387,
        //NF_BlueBird = 388,
        //NF_Blank389 = 389,

        //NF_Nameless390 = 390,
        //NF_Nameless391 = 391,
        //NF_Nameless392 = 392,
        //NF_Nameless393 = 393,
        //NF_Nameless394 = 394,
        //NF_Nameless395 = 395,
        //NF_Nameless396 = 396,
        //NF_Nameless397 = 397,
        //NF_Nameless398 = 398,
        //NF_Nameless399 = 399,

        CrystalGolem = 400,
        //NF_Nameless401 = 401,
        //NF_Nameless402 = 402,
        //NF_Nameless403 = 403,
        //NF_Nameless404 = 404,
        //NF_Nameless405 = 405,
        //NF_Nameless406 = 406,
        //NF_Nameless407 = 407,
        //NF_Nameless408 = 408,
        //NF_Nameless409 = 409,

        //NF_Nameless410 = 410,
        DustDevil = 411,
        TwinTailScorpion = 412,
        BloodyMole = 413,
        //NF_Nameless414 = 414,
        //NF_Nameless415 = 415,
        //NF_Nameless416 = 416,
        //NF_Nameless417 = 417,
        //NF_Blank418 = 418,
        //NF_Blank419 = 419,

        //NF_HellVampire = 420,
        //NF_HellSmelterer = 421,
        //NF_HellPuddle = 422,
        //NF_CrystalPillar2 = 423,
        Terracotta1 = 424,
        Terracotta2 = 425,
        Terracotta3 = 426,
        Terracotta4 = 427,
        TerracottaSub = 428,
        TerracottaBoss = 429,

        //Mon43

        //NF_Nameless440 = 440,
        //NF_Nameless441 = 441,
        //NF_Nameless442 = 442,
        SDMob19 = 443,
        SDMob20 = 444,
        SDMob21 = 445,
        SDMob22 = 446,
        SDMob23 = 447,
        SDMob24 = 448,
        SDMob25 = 449,

        SDMob26 = 450,
        LobsterLord = 453,

        //Mon46

        NewMob1 = 470,
        NewMob2 = 471,
        NewMob3 = 472,
        NewMob4 = 473,
        NewMob5 = 474,
        NewMob6 = 475,
        NewMob7 = 476,
        NewMob8 = 477,
        NewMob9 = 478,
        NewMob10 = 479,

        //Mon48

        MonasteryMon0 = 490,
        MonasteryMon1 = 491,
        MonasteryMon2 = 492,
        MonasteryMon3 = 493,
        MonasteryMon4 = 494,
        MonasteryMon5 = 495,
        MonasteryMon6 = 496,
        //NF_Blank497 = 497,
        //NF_Blank498 = 498,
        //NF_Blank499 = 499,

        //Mon50

        //Mon51

        //Mon52

        //MonMagicEx25
        SeaHorseCavalry = 530,
        Seamancer = 531,
        CoralStoneDuin = 532,
        Brachiopod = 533,
        GiantClam = 534,
        BlueMassif = 535,
        Mollusk = 536,
        Kraken = 537,
        KrakenLeg = 538,
        GiantClam1 = 539,

        //Mon54
        SabukGateSouth = 540,
        SabukGateNorth = 541,
        SabukGateEast = 542,
        SabukGateWest = 543,
        //NF_NorthBarrier = 544,
        //NF_SouthBarrier = 545,
        //NF_EastBarrier = 546,
        //NF_WestBarrier = 547,
        //NF_TaoSungDoor = 548,
        //NF_Blank_549 = 549,

        //Mon55

        //Mon56
        Tornado = 566,

        //Mon57

        //Mon58

        //Mon59


        //Flag
        CastleFlag = 1000
    }

   
    public enum MapIcon
    {
        None,

        Cave = 1,
        Exit = 2,
        Down = 3,
        Up = 4,
        Province = 5,
        Building = 6,

        BichonCity = 100,
        Castle,
        BugCaves,
        CaveUpDown,
        SmallManInTriangle,
        Dunes,
        MineUpDown,
        GinkoTree,
        Forest,
        InsectCaveBubble,
        AntCave,
        JinchonTemple,
        MiningCave,
        Mudwall,
        BorderTown,
        Oasis,
        UnknownPalace,
        Pointer,
        Serpent,
        Shrine,
        SkullCave,
        SkullBonesCave,
        StairDown,
        StairUp,
        UnknownTemple,
        Walkway,
        StoneTemple,
        WoomaTemple,
        ZumaTemple,
        IslandShores,
        DuneWalkway,
        DuneWalkway2,
        ForestWalkway,
        ForestWalkway2,
        ForestWalkway3,
        Star,
        Lock
    }

    public enum Effect
    {
        TeleportOut,
        TeleportIn,

        FullBloom,
        WhiteLotus,
        RedLotus,
        SweetBrier,
        Karma,

        MirrorImage,

        Puppet,
        PuppetFire,
        PuppetIce,
        PuppetLightning,
        PuppetWind,

        SummonSkeleton,
        SummonShinsu,
        CursedDoll,
        UndeadSoul,

        ThunderBolt,
        FrostBiteEnd,

        DanceOfSwallow,
        FlashOfLight,
        ChainOfFireExplode,

        DemonExplosion,
        ParasiteExplode,
        BurningFireExplode
    }

    [Flags]
    public enum PoisonType
    {
        None = 0,

        Green = 1 << 0,         //Tick damage, displays green
        Red = 1 << 1,           //Increases damage received by 20%, displays red
        Slow = 1 << 2,          //Reduces attackTime, actionTime, 100ms per value, displays blue
        Paralysis = 1 << 3,     //Stops movement, physical and magic attacks (all races), displays grey
        WraithGrip = 1 << 4,    //Stops shoulderdash, movement, displays effect (needs code revisiting)
        HellFire = 1 << 5,      //Tick damage, no colour
        Silenced = 1 << 6,      //Stops movement (all races), physical and magic attacks (monster), displays effect
        Abyss = 1 << 7,         //Reduces monster viewrange, displays blinding effect (player)
        Parasite = 1 << 8,      //Tick damage, explosion, ignores transparency (monster), displays effect
        Neutralize = 1 << 9,    //Stops attackTime, slows actionTime, displays effect (needs code revisiting)
        Fear = 1 << 10,         //Stops attack (monster), forces runaway (monster), displays effect
        Burn = 1 << 11,         //Tick damage, displays effect
        Containment = 1 << 12,  //Tick damage, stops movement, displays effect
        Chain = 1 << 13,        //Tick damage, limits movement, displays effect
        Hemorrhage = 1 << 14,   //Tick damage, stops recovery, displays effect
    }

    public enum SpellEffect
    {
        None,

        SafeZone,

        FireWall,
        Tempest,

        TrapOctagon,
        DarkSoulPrison,

        PoisonousCloud,
        BurningFire,

        Rubble,

        MonsterDeathCloud,
    }


    public enum MagicEffect
    {
        ReflectDamage,
        Assault,
        DefensiveBlow,

        MagicShield,
        MagicShieldStruck,
        SuperiorMagicShield,
        SuperiorMagicShieldStruck,
        ElementalHurricane,
        FrostBite,
        Burn,

        CelestialLight,
        CelestialLightStruck,
        Parasite,
        Neutralize,

        WraithGrip,
        LifeSteal,
        Silence,
        Blind,
        Fear,
        Abyss,
        DragonRepulse,
        Containment,
        Chain,
        Hemorrhage,

        Ranking,
        Developer,
    }

    public enum MarketPlaceSort
    {
        Newest,
        Oldest,
        [Description("Highest Price")]
        HighestPrice,
        [Description("Lowest Price")]
        LowestPrice,
    }

    public enum MarketPlaceStoreSort
    {
        Alphabetical,
        [Description("Highest Price")]
        HighestPrice,
        [Description("Lowest Price")]
        LowestPrice,
        Favourite
    }

    public enum DungeonFinderSort
    {
        Name,
        Level,
        [Description("Player Count")]
        PlayerCount,
    }

    public enum RefineType : byte
    {
        None,

        Durability,
        DC,
        SpellPower,
        Fire,
        Ice,
        Lightning,
        Wind,
        Holy,
        Dark,
        Phantom,
        Reset,
        Health,
        Mana,
        AC,
        MR,
        Accuracy,
        Agility,
        DCPercent,
        SPPercent,
        HealthPercent,
        ManaPercent,
    }

    public enum RefineQuality : byte
    {
        Rush = 0,
        Quick = 1,
        Standard = 2,
        Careful = 3,
        Precise = 4,
    }

    public enum ExteriorEffect : byte
    {
        None = 0,

        //EquipEffect_Part [1~99] 
        A_WhiteAura = 1,
        A_FlameAura = 2,
        A_BlueAura = 3,

        A_FlameAura2 = 9,
        A_GreenWings = 10,
        A_FlameWings = 11,
        A_BlueWings = 12,
        A_RedSinWings = 13,

        A_DiamondFireWings = 14,
        A_PurpleTentacles2 = 15,
        A_PhoenixWings = 16,
        A_IceKingWings = 17,
        A_BlueButterflyWings = 18,


        S_WarThurible = 50,
        S_PenanceThurible = 51,
        S_CensorshipThurible = 52,
        S_PetrichorThurible = 53,
        
        //EquipEffect_Full [100~119]
        A_FireDragonWings = 100,
        A_SmallYellowWings = 101,
        A_GreenFeatherWings = 102,
        A_RedFeatherWings = 103,
        A_BlueFeatherWings = 104,
        A_WhiteFeatherWings = 105,
        A_PurpleTentacles = 106,

        W_ChaoticHeavenBlade = 110,
        W_JanitorsScimitar = 111,
        W_JanitorsDualBlade = 112,

        //EquipEffect_FullEx1 [120~139] 
        A_LionWings = 120,
        A_AngelicWings = 121,

        //EquipEffect_FullEx2 [140~159] 
        A_BlueDragonWings = 140,

        //EquipEffect_FullEx3 [160~179]
        A_RedWings2 = 160,

        //EquipEffect_Item [180~199]
        //Reserved

        //MonMagicEx26 [200~250] 
        E_RedEyeRing = 200,
        E_BlueEyeRing = 201,
        E_GreenSpiralRing = 202,
        E_Fireworks = 203
    }

    public enum ItemEffect : byte
    {
        None,

        //Gold = 1,
        Experience = 2,
        CompanionTicket = 3,
        BasicCompanionBag = 4,
        PickAxe = 5,
        UmaKingHorn = 6,
        ItemPart = 7,
        Carrot = 8,

        DestructionElixir = 10,
        HasteElixir = 11,
        LifeElixir = 12,
        ManaElixir = 13,
        NatureElixir = 14,
        SpiritElixir = 15,

        BlackIronOre = 20,
        GoldOre = 21,
        Diamond = 22,
        SilverOre = 23,
        IronOre = 24,
        Corundum = 25,

        ElixirOfPurification = 30,
        PillOfReincarnation = 31,

        Crystal = 40,
        RefinementStone = 41,
        Fragment1 = 42,
        Fragment2 = 43,
        Fragment3 = 44,

        GenderChange = 50,
        HairChange = 51,
        ArmourDye = 52,
        NameChange = 53,
        FortuneChecker = 54,
        Caption = 55,

        WeaponTemplate = 60,
        WarriorWeapon = 61,
        WizardWeapon = 63,
        TaoistWeapon = 64,
        AssassinWeapon = 65,

        YellowSlot = 70,
        BlueSlot = 71,
        RedSlot = 72,
        PurpleSlot = 73,
        GreenSlot = 74,
        GreySlot = 75,

        FootballArmour = 80,
        FootBallWhistle = 81,

        FishingRod = 82,
        FishingRobe = 83,

        StatExtractor = 90,
        SpiritBlade = 91,
        RefineExtractor = 92,

        DualWield = 100
    }

    public enum CurrencyType
    {
        Gold = 0,
        GameGold = 1,
        HuntGold = 2,
        Other = 3,
        FP = 4,
        CP = 5
    }

    public enum CurrencyCategory
    {
        Basic = 0,
        Player = 1,
        Event = 2,
        Map = 3,
        Other = 4
    }

    [Flags]
    public enum UserItemFlags
    {
        None = 0,
        
        Locked = 1,
        Bound = 2,
        Worthless = 4,
        Refinable = 8,
        Expirable = 16,
        QuestItem = 32,
        GameMaster = 64,
        Marriage = 128,
        NonRefinable = 256
    }
    
    public enum HorseType : byte
    {
        None = 0,
        Brown = 1,
        White = 2,
        Red = 3,
        Black = 4,
        WhiteUnicorn = 5,
        RedUnicorn = 6
    }
    
    public enum OnlineState : byte
    {
        Online,
        Busy,
        Away,
        Offline
    }

    [Flags]
    public enum GuildPermission
    {
        None = 0,

        Leader = -1,

        EditNotice = 1,
        AddMember = 2,
        RemoveMember = 4,
        Storage = 8,
        FundsRepair = 16,
        FundsMerchant = 32,
        FundsMarket = 64,
        StartWar = 128,
    }

    public enum NPCRequirementType
    {
        MinLevel = 0,
        MaxLevel = 1,
        Accepted = 2,
        NotAccepted = 3,
        HaveCompleted = 4,
        HaveNotCompleted = 5,
        Class = 6,
        DaysOfWeek = 7,
    }

    public enum QuestType
    {
        General = 0,
        Daily = 1,
        Weekly = 2,
        Repeatable = 3,
        Story = 4,
        Account = 5
    }

    public enum QuestIcon
    {
        None = 0,

        New = 1,
        Incomplete = 2,
        Complete = 3,
    }

    public enum QuestRequirementType
    {
        MinLevel = 0,
        MaxLevel = 1,
        NotAccepted = 2,
        HaveCompleted = 3,
        HaveNotCompleted = 4,
        Class = 5,
    }

    public enum QuestTaskType
    {
        KillMonster = 0,
        GainItem = 1,
        Region = 2
    }

    public enum MovementEffect
    {
        None = 0,

        SpecialRepair = 1,
    }

    public enum SpellKey : byte
    {
        None,

        [Description("Key\n1")]
        Spell01,
        [Description("Key\n2")]
        Spell02,
        [Description("Key\n3")]
        Spell03,
        [Description("Key\n4")]
        Spell04,
        [Description("Key\n5")]
        Spell05,
        [Description("Key\n6")]
        Spell06,
        [Description("Key\n7")]
        Spell07,
        [Description("Key\n8")]
        Spell08,
        [Description("Key\n9")]
        Spell09,
        [Description("Key\n10")]
        Spell10,
        [Description("Key\n11")]
        Spell11,
        [Description("Key\n12")]
        Spell12,

        [Description("Key\n13")]
        Spell13,
        [Description("Key\n14")]
        Spell14,
        [Description("Key\n15")]
        Spell15,
        [Description("Key\n16")]
        Spell16,
        [Description("Key\n17")]
        Spell17,
        [Description("Key\n18")]
        Spell18,
        [Description("Key\n19")]
        Spell19,
        [Description("Key\n20")]
        Spell20,
        [Description("Key\n21")]
        Spell21,
        [Description("Key\n22")]
        Spell22,
        [Description("Key\n23")]
        Spell23,
        [Description("Key\n24")]
        Spell24,
    }

    public enum MonsterFlag
    {
        None = 0,

        Skeleton = 1,
        JinSkeleton = 2,
        Shinsu = 3,
        InfernalSoldier = 4,
        CursedDoll = 5,

        SummonPuppet = 6,

        MirrorImage = 7,
        Tornado = 8,

        UndeadSoul = 9,

        CastleObjective = 10,
        CastleDefense = 11,

        Blocker = 20,

        Larva = 100,

        LesserWedgeMoth = 110,

        ZumaArcherMonster = 120,
        ZumaGuardianMonster = 121,
        ZumaFanaticMonster = 122,
        ZumaKeeperMonster = 123,

        BoneArcher = 130,
        BoneCaptain = 131,
        BoneBladesman = 132,
        BoneSoldier = 133,
        SkeletonEnforcer = 134,

        MatureEarwig = 140,
        GoldenArmouredBeetle = 141,
        Millipede = 142,

        FerociousFlameDemon = 150,
        FlameDemon = 151,

        GoruSpearman = 160,
        GoruArcher = 161,
        GoruGeneral = 162,

        DragonLord = 170,
        OYoungBeast = 171,
        YumgonWitch = 172,
        MaWarden = 173,
        MaWarlord = 174,
        JinhwanSpirit = 175,
        JinhwanGuardian = 176,
        OyoungGeneral = 177,
        YumgonGeneral = 178,

        BanyoCaptain = 180,

        SamaSorcerer = 190,
        BloodStone = 191,

        QuartzPinkBat = 200,
        QuartzBlueBat = 201,
        QuartzBlueCrystal = 202,
        QuartzRedHood = 203,
        QuartzMiniTurtle = 204,
        QuartzTurtleSub = 205,

        Sacrifice = 210
    }

    public enum FishingState : byte
    {
        None,
        Cast,
        Reel,
        Cancel
    }

    public enum HintPosition : byte
    {
        TopLeft,
        BottomLeft,

        FixedY,

        Fluid
    }

    #region Packet Enums

    public enum NewAccountResult : byte
    {
        Disabled,
        BadEMail,
        BadPassword,
        BadRealName,
        AlreadyExists,
        BadReferral,
        ReferralNotFound,
        ReferralNotActivated,
        Success
    }

    public enum ChangePasswordResult : byte
    {
        Disabled,
        BadEMail,
        BadCurrentPassword,
        BadNewPassword,
        AccountNotFound,
        AccountNotActivated,
        WrongPassword,
        Banned,
        Success
    }
    public enum RequestPasswordResetResult : byte
    {
        Disabled,
        BadEMail,
        AccountNotFound,
        AccountNotActivated,
        ResetDelay,
        Banned,
        Success
    }
    public enum ResetPasswordResult : byte
    {
        Disabled,
        AccountNotFound,
        BadNewPassword,
        KeyExpired,
        Success
    }
    

    public enum ActivationResult : byte
    {
        Disabled,
        AccountNotFound,
        Success,
    }

    public enum RequestActivationKeyResult : byte
    {
        Disabled,
        BadEMail,
        AccountNotFound,
        AlreadyActivated,
        RequestDelay,
        Success,
    }

    public enum LoginResult : byte
    {
        Disabled,
        BadEMail,
        BadPassword,
        AccountNotExists,
        AccountNotActivated,
        WrongPassword,
        Banned,
        AlreadyLoggedIn,
        AlreadyLoggedInPassword,
        AlreadyLoggedInAdmin,
        Success
    }

    public enum NewCharacterResult : byte
    {
        Disabled,
        BadCharacterName,
        BadGender,
        BadClass,
        BadHairType,
        BadHairColour,
        BadArmourColour,
        ClassDisabled,
        MaxCharacters,
        AlreadyExists,
        Success
    }

    public enum DeleteCharacterResult : byte
    {
        Disabled,
        AlreadyDeleted,
        NotFound,
        Success
    }

    public enum StartGameResult : byte
    {
        Disabled,
        Deleted,
        Delayed,
        UnableToSpawn,
        NotFound,
        Success
    }

    public enum DisconnectReason : byte
    {
        Unknown,
        TimedOut,
        WrongVersion,
        ServerClosing,
        AnotherUser,
        AnotherUserPassword,
        AnotherUserAdmin,
        Banned,
        Kicked,
        Crashed
    }

    public enum InstanceResult : byte
    {
        Invalid,
        InsufficientLevel,
        SafeZoneOnly,
        NotInGroup,
        NotInGuild,
        NotInCastle,
        TooFewInGroup,
        TooManyInGroup,
        ConnectRegionNotSet,
        NoSlots,
        NoRejoin,
        NotGroupLeader,
        UserCooldown,
        GuildCooldown,
        MissingItem,
        NoMap,
        Success
    }

    #endregion

    #region Sound

    public enum SoundIndex
    {
        None,
        LoginScene,
        SelectScene,

        #region Province Music
        B000 = 3,
        B2,
        B8,
        B009D,
        B009N,
        B0014D,
        B0014N,
        B100,
        B122,
        B300,
        B400,
        B14001,
        BD00,
        BD01,
        BD02,
        BD041,
        BD042,
        BD50,
        BD60,
        BD70,
        BD99,
        BD100,
        BD101,
        BD210,
        BD211,
        BDUnderseaCave,
        BDUnderseaCaveBoss,
        D3101,
        D3102,
        D3400,
        Dungeon_1,
        Dungeon_2,
        ID1_001,
        ID1_002,
        ID1_003,
        TS001,
        TS002,
        TS003,
        #endregion

        LoginScene2,
        LoginScene3,

        ButtonA = 100,
        ButtonB,
        ButtonC,

        SelectWarriorMale,
        SelectWarriorFemale,
        SelectWizardMale,
        SelectWizardFemale,
        SelectTaoistMale,
        SelectTaoistFemale,
        SelectAssassinMale,
        SelectAssassinFemale,

        TeleportOut,
        TeleportIn,

        ItemPotion,
        ItemWeapon,
        ItemArmour,
        ItemRing,
        ItemBracelet,
        ItemNecklace,
        ItemHelmet,
        ItemShoes,
        ItemDefault,

        GoldPickUp,
        GoldGained,

        RollDice,
        RollYut,

        DaggerSwing,
        WoodSwing,
        IronSwordSwing,
        ShortSwordSwing,
        AxeSwing,
        ClubSwing,
        WandSwing,
        FistSwing,
        GlaiveAttack,
        ClawAttack,

        GenericStruckPlayer,
        GenericStruckMonster,

        Foot1,
        Foot2,
        Foot3,
        Foot4,

        FishingCast,
        FishingBob,
        FishingReel,

        HorseWalk1,
        HorseWalk2,
        HorseRun,

        MaleStruck,
        FemaleStruck,

        MaleDie,
        FemaleDie,

        QuestTake,
        QuestComplete,

        #region Magics

        SlayingMale,
        SlayingFemale,

        EnergyBlast,

        HalfMoon,

        FlamingSword,

        DragonRise,

        BladeStorm,

        DefensiveBlow,

        DestructiveSurge,

        DefianceStart,

        ReflectDamageStart,

        InvincibilityStart,

        AssaultStart,

        SwiftBladeEnd,

        ElementalSwordStart,
        ElementalSwordEnd,

        FireBallStart,
        FireBallTravel,
        FireBallEnd,

        ThunderBoltStart,
        ThunderBoltTravel,
        ThunderBoltEnd,

        IceBoltStart,
        IceBoltTravel,
        IceBoltEnd,

        GustBlastStart,
        GustBlastTravel,
        GustBlastEnd,

        RepulsionEnd,

        ElectricShockStart,
        ElectricShockEnd,

        GreaterFireBallStart,
        GreaterFireBallTravel,
        GreaterFireBallEnd,

        LightningStrikeStart,
        LightningStrikeEnd,

        GreaterIceBoltStart,
        GreaterIceBoltTravel,
        GreaterIceBoltEnd,

        CycloneStart,
        CycloneEnd,

        TeleportationStart,

        LavaStrikeStart,

        LightningBeamEnd,

        FrozenEarthStart,
        FrozenEarthEnd,

        BlowEarthStart,
        BlowEarthEnd,
        BlowEarthTravel,

        FireWallStart,
        FireWallEnd,

        ExpelUndeadStart,
        ExpelUndeadEnd,

        MagicShieldStart,

        FireStormStart,
        FireStormEnd,

        LightningWaveStart,
        LightningWaveEnd,

        IceStormStart,
        IceStormEnd,

        DragonTornadoStart,
        DragonTornadoEnd,

        GreaterFrozenEarthStart,
        GreaterFrozenEarthEnd,

        ChainLightningStart,
        ChainLightningEnd,

        ParasiteTravel,
        ParasiteExplode,

        FrostBiteStart,

        ElementalHurricane,

        TornadoStart,

        HealStart,
        HealEnd,

        PoisonDustStart,
        PoisonDustEnd,

        ExplosiveTalismanStart,
        ExplosiveTalismanTravel,
        ExplosiveTalismanEnd,

        HolyStrikeStart,
        HolyStrikeTravel,
        HolyStrikeEnd,

        ImprovedHolyStrikeStart,
        ImprovedHolyStrikeTravel,
        ImprovedHolyStrikeEnd,

        MagicResistanceTravel,
        MagicResistanceEnd,

        ResilienceTravel,
        ResilienceEnd,

        ShacklingTalismanStart,
        ShacklingTalismanEnd,

        SummonSkeletonStart,
        SummonSkeletonEnd,

        CursedDollEnd,

        InvisibilityEnd,

        MassInvisibilityTravel,
        MassInvisibilityEnd,

        TaoistCombatKickStart,

        MassHealStart,
        MassHealEnd,

        BloodLustTravel,
        BloodLustEnd,

        ResurrectionStart,

        PurificationStart,
        PurificationEnd,

        SummonShinsuStart,
        SummonShinsuEnd,

        StrengthOfFaithStart,
        StrengthOfFaithEnd,

        NeutralizeTravel,
        NeutralizeEnd,

        DarkSoulPrison,

        CorpseExploderEnd,

        SummonDeadEnd,

        PoisonousCloudStart,

        CloakStart,

        WraithGripStart,
        WraithGripEnd,

        HellFireStart,

        FullBloom,
        WhiteLotus,
        RedLotus,
        SweetBrier,
        SweetBrierMale,
        SweetBrierFemale,

        WaningMoon,

        CalamityOfFullMoon,

        RakeStart,

        Karma,

        TheNewBeginning,
        Concentration,

        SummonPuppet,

        DanceOfSwallowsEnd,
        DragonRepulseStart,
        AbyssStart,
        FlashOfLightEnd,
        EvasionStart,
        RagingWindStart,

        ChainofFireExplode,
        #endregion

        #region Monsters

        ChickenAttack,
        ChickenStruck,
        ChickenDie,

        PigAttack,
        PigStruck,
        PigDie,

        DeerAttack,
        DeerStruck,
        DeerDie,

        CowAttack,
        CowStruck,
        CowDie,

        SheepAttack,
        SheepStruck,
        SheepDie,

        SkyStingerAttack,
        SkyStingerStruck,
        SkyStingerDie,

        ClawCatAttack,
        ClawCatStruck,
        ClawCatDie,

        WolfAttack,
        WolfStruck,
        WolfDie,

        ForestYetiAttack,
        ForestYetiStruck,
        ForestYetiDie,

        CarnivorousPlantAttack,
        CarnivorousPlantStruck,
        CarnivorousPlantDie,

        YobAttack,
        YobStruck,
        YobDie,

        OmaAttack,
        OmaStruck,
        OmaDie,

        TigerSnakeAttack,
        TigerSnakeStruck,
        TigerSnakeDie,

        SpittingSpiderAttack,
        SpittingSpiderStruck,
        SpittingSpiderDie,

        ScarecrowAttack,
        ScarecrowStruck,
        ScarecrowDie,

        OmaHeroAttack,
        OmaHeroStruck,
        OmaHeroDie,

        CaveBatAttack,
        CaveBatStruck,
        CaveBatDie,

        ScorpionAttack,
        ScorpionStruck,
        ScorpionDie,

        SkeletonAttack,
        SkeletonStruck,
        SkeletonDie,

        SkeletonAxeManAttack,
        SkeletonAxeManStruck,
        SkeletonAxeManDie,

        SkeletonAxeThrowerAttack,
        SkeletonAxeThrowerStruck,
        SkeletonAxeThrowerDie,

        SkeletonWarriorAttack,
        SkeletonWarriorStruck,
        SkeletonWarriorDie,

        SkeletonLordAttack,
        SkeletonLordStruck,
        SkeletonLordDie,

        CaveMaggotAttack,
        CaveMaggotStruck,
        CaveMaggotDie,

        GhostSorcererAttack,
        GhostSorcererStruck,
        GhostSorcererDie,

        GhostMageAppear,
        GhostMageAttack,
        GhostMageStruck,
        GhostMageDie,

        VoraciousGhostAttack,
        VoraciousGhostStruck,
        VoraciousGhostDie,

        GhoulChampionAttack,
        GhoulChampionStruck,
        GhoulChampionDie,

        ArmoredAntAttack,
        ArmoredAntStruck,
        ArmoredAntDie,

        AntNeedlerAttack,
        AntNeedlerStruck,
        AntNeedlerDie,

        KeratoidAttack,
        KeratoidStruck,
        KeratoidDie,

        ShellNipperAttack,
        ShellNipperStruck,
        ShellNipperDie,

        VisceralWormAttack,
        VisceralWormStruck,
        VisceralWormDie,

        MutantFleaAttack,
        MutantFleaStruck,
        MutantFleaDie,

        PoisonousMutantFleaAttack,
        PoisonousMutantFleaStruck,
        PoisonousMutantFleaDie,

        BlasterMutantFleaAttack,
        BlasterMutantFleaStruck,
        BlasterMutantFleaDie,

        WasHatchlingAttack,
        WasHatchlingStruck,
        WasHatchlingDie,

        CentipedeAttack,
        CentipedeStruck,
        CentipedeDie,

        ButterflyWormAttack,
        ButterflyWormStruck,
        ButterflyWormDie,

        MutantMaggotAttack,
        MutantMaggotStruck,
        MutantMaggotDie,

        EarwigAttack,
        EarwigStruck,
        EarwigDie,

        IronLanceAttack,
        IronLanceStruck,
        IronLanceDie,

        LordNiJaeAttack,
        LordNiJaeStruck,
        LordNiJaeDie,

        RottingGhoulAttack,
        RottingGhoulStruck,
        RottingGhoulDie,

        DecayingGhoulAttack,
        DecayingGhoulStruck,
        DecayingGhoulDie,

        BloodThirstyGhoulAttack,
        BloodThirstyGhoulStruck,
        BloodThirstyGhoulDie,


        SpinedDarkLizardAttack,
        SpinedDarkLizardStruck,
        SpinedDarkLizardDie,

        UmaInfidelAttack,
        UmaInfidelStruck,
        UmaInfidelDie,

        UmaFlameThrowerAttack,
        UmaFlameThrowerStruck,
        UmaFlameThrowerDie,

        UmaAnguisherAttack,
        UmaAnguisherStruck,
        UmaAnguisherDie,

        UmaKingAttack,
        UmaKingStruck,
        UmaKingDie,

        SpiderBatAttack,
        SpiderBatStruck,
        SpiderBatDie,

        ArachnidGazerStruck,
        ArachnidGazerDie,

        LarvaAttack,
        LarvaStruck,

        RedMoonGuardianAttack,
        RedMoonGuardianStruck,
        RedMoonGuardianDie,

        RedMoonProtectorAttack,
        RedMoonProtectorStruck,
        RedMoonProtectorDie,

        VenomousArachnidAttack,
        VenomousArachnidStruck,
        VenomousArachnidDie,

        DarkArachnidAttack,
        DarkArachnidStruck,
        DarkArachnidDie,

        RedMoonTheFallenAttack,
        RedMoonTheFallenStruck,
        RedMoonTheFallenDie,


        ViciousRatAttack,
        ViciousRatStruck,
        ViciousRatDie,

        ZumaSharpShooterAttack,
        ZumaSharpShooterStruck,
        ZumaSharpShooterDie,

        ZumaFanaticAttack,
        ZumaFanaticStruck,
        ZumaFanaticDie,

        ZumaGuardianAttack,
        ZumaGuardianStruck,
        ZumaGuardianDie,

        ZumaKingAppear,
        ZumaKingAttack,
        ZumaKingStruck,
        ZumaKingDie,

        EvilFanaticAttack,
        EvilFanaticStruck,
        EvilFanaticDie,

        MonkeyAttack,
        MonkeyStruck,
        MonkeyDie,

        EvilElephantAttack,
        EvilElephantStruck,
        EvilElephantDie,

        CannibalFanaticAttack,
        CannibalFanaticStruck,
        CannibalFanaticDie,

        SpikedBeetleAttack,
        SpikedBeetleStruck,
        SpikedBeetleDie,

        NumaGruntAttack,
        NumaGruntStruck,
        NumaGruntDie,

        NumaMageAttack,
        NumaMageStruck,
        NumaMageDie,

        NumaEliteAttack,
        NumaEliteStruck,
        NumaEliteDie,

        SandSharkAttack,
        SandSharkStruck,
        SandSharkDie,

        StoneGolemAppear,
        StoneGolemAttack,
        StoneGolemStruck,
        StoneGolemDie,

        WindfurySorceressAttack,
        WindfurySorceressStruck,
        WindfurySorceressDie,

        CursedCactusAttack,
        CursedCactusStruck,
        CursedCactusDie,

        RagingLizardAttack,
        RagingLizardStruck,
        RagingLizardDie,

        SawToothLizardAttack,
        SawToothLizardStruck,
        SawToothLizardDie,

        MutantLizardAttack,
        MutantLizardStruck,
        MutantLizardDie,

        VenomSpitterAttack,
        VenomSpitterStruck,
        VenomSpitterDie,

        SonicLizardAttack,
        SonicLizardStruck,
        SonicLizardDie,

        GiantLizardAttack,
        GiantLizardStruck,
        GiantLizardDie,

        CrazedLizardAttack,
        CrazedLizardStruck,
        CrazedLizardDie,

        TaintedTerrorAttack,
        TaintedTerrorStruck,
        TaintedTerrorDie,
        TaintedTerrorAttack2,

        DeathLordJichonAttack,
        DeathLordJichonStruck,
        DeathLordJichonDie,
        DeathLordJichonAttack2,
        DeathLordJichonAttack3,

        MinotaurAttack,
        MinotaurStruck,
        MinotaurDie,

        FrostMinotaurAttack,
        FrostMinotaurStruck,
        FrostMinotaurDie,

        BanyaLeftGuardAttack,
        BanyaLeftGuardStruck,
        BanyaLeftGuardDie,

        EmperorSaWooAttack,
        EmperorSaWooStruck,
        EmperorSaWooDie,

        BoneArcherAttack,
        BoneArcherStruck,
        BoneArcherDie,

        BoneCaptainAttack,
        BoneCaptainStruck,
        BoneCaptainDie,

        ArchLichTaeduAttack,
        ArchLichTaeduStruck,
        ArchLichTaeduDie,

        WedgeMothLarvaAttack,
        WedgeMothLarvaStruck,
        WedgeMothLarvaDie,

        LesserWedgeMothAttack,
        LesserWedgeMothStruck,
        LesserWedgeMothDie,

        WedgeMothAttack,
        WedgeMothStruck,
        WedgeMothDie,

        RedBoarAttack,
        RedBoarStruck,
        RedBoarDie,

        ClawSerpentAttack,
        ClawSerpentStruck,
        ClawSerpentDie,

        BlackBoarAttack,
        BlackBoarStruck,
        BlackBoarDie,

        TuskLordAttack,
        TuskLordStruck,
        TuskLordDie,

        RazorTuskAttack,
        RazorTuskStruck,
        RazorTuskDie,

        PinkGoddessAttack,
        PinkGoddessStruck,
        PinkGoddessDie,

        GreenGoddessAttack,
        GreenGoddessStruck,
        GreenGoddessDie,

        MutantCaptainAttack,
        MutantCaptainStruck,
        MutantCaptainDie,

        StoneGriffinAttack,
        StoneGriffinStruck,
        StoneGriffinDie,

        FlameGriffinAttack,
        FlameGriffinStruck,
        FlameGriffinDie,

        WhiteBoneAttack,
        WhiteBoneStruck,
        WhiteBoneDie,

        ShinsuSmallStruck,
        ShinsuSmallDie,

        ShinsuBigAttack,
        ShinsuBigStruck,
        ShinsuBigDie,

        ShinsuShow,

        CorpseStalkerAttack,
        CorpseStalkerStruck,
        CorpseStalkerDie,

        LightArmedSoldierAttack,
        LightArmedSoldierStruck,
        LightArmedSoldierDie,

        CorrosivePoisonSpitterAttack,
        CorrosivePoisonSpitterStruck,
        CorrosivePoisonSpitterDie,

        PhantomSoldierAttack,
        PhantomSoldierStruck,
        PhantomSoldierDie,

        MutatedOctopusAttack,
        MutatedOctopusStruck,
        MutatedOctopusDie,

        AquaLizardAttack,
        AquaLizardStruck,
        AquaLizardDie,

        CrimsonNecromancerAttack,
        CrimsonNecromancerStruck,
        CrimsonNecromancerDie,

        ChaosKnightAttack,
        ChaosKnightStruck,
        ChaosKnightDie,

        PachontheChaosbringerAttack,
        PachontheChaosbringerStruck,
        PachontheChaosbringerDie,


        NumaCavalryAttack,
        NumaCavalryStruck,
        NumaCavalryDie,

        NumaHighMageAttack,
        NumaHighMageStruck,
        NumaHighMageDie,

        NumaStoneThrowerAttack,
        NumaStoneThrowerStruck,
        NumaStoneThrowerDie,

        NumaRoyalGuardAttack,
        NumaRoyalGuardStruck,
        NumaRoyalGuardDie,

        NumaArmoredSoldierAttack,
        NumaArmoredSoldierStruck,
        NumaArmoredSoldierDie,

        IcyRangerAttack,
        IcyRangerStruck,
        IcyRangerDie,

        IcyGoddessAttack,
        IcyGoddessStruck,
        IcyGoddessDie,

        IcySpiritWarriorAttack,
        IcySpiritWarriorStruck,
        IcySpiritWarriorDie,

        IcySpiritGeneralAttack,
        IcySpiritGeneralStruck,
        IcySpiritGeneralDie,

        GhostKnightAttack,
        GhostKnightStruck,
        GhostKnightDie,

        IcySpiritSpearmanAttack,
        IcySpiritSpearmanStruck,
        IcySpiritSpearmanDie,

        WerewolfAttack,
        WerewolfStruck,
        WerewolfDie,

        WhitefangAttack,
        WhitefangStruck,
        WhitefangDie,

        IcySpiritSoliderAttack,
        IcySpiritSoliderStruck,
        IcySpiritSoliderDie,

        WildBoarAttack,
        WildBoarStruck,
        WildBoarDie,

        FrostLordHwaAttack,
        FrostLordHwaStruck,
        FrostLordHwaDie,

        JinchonDevilAttack,
        JinchonDevilAttack2,
        JinchonDevilAttack3,
        JinchonDevilStruck,
        JinchonDevilDie,

        EscortCommanderAttack,
        EscortCommanderStruck,
        EscortCommanderDie,

        FieryDancerAttack,
        FieryDancerStruck,
        FieryDancerDie,

        EmeraldDancerAttack,
        EmeraldDancerStruck,
        EmeraldDancerDie,

        QueenOfDawnAttack,
        QueenOfDawnStruck,
        QueenOfDawnDie,

        OYoungBeastAttack,
        OYoungBeastStruck,
        OYoungBeastDie,

        YumgonWitchAttack,
        YumgonWitchStruck,
        YumgonWitchDie,

        MaWarlordAttack,
        MaWarlordStruck,
        MaWarlordDie,

        JinhwanSpiritAttack,
        JinhwanSpiritStruck,
        JinhwanSpiritDie,

        JinhwanGuardianAttack,
        JinhwanGuardianStruck,
        JinhwanGuardianDie,

        YumgonGeneralAttack,
        YumgonGeneralStruck,
        YumgonGeneralDie,

        ChiwooGeneralAttack,
        ChiwooGeneralStruck,
        ChiwooGeneralDie,

        DragonQueenAttack,
        DragonQueenStruck,
        DragonQueenDie,

        DragonLordAttack,
        DragonLordStruck,
        DragonLordDie,

        FerociousIceTigerAttack,
        FerociousIceTigerStruck,
        FerociousIceTigerDie,

        SamaFireGuardianAttack,
        SamaFireGuardianStruck,
        SamaFireGuardianDie,

        SamaIceGuardianAttack,
        SamaIceGuardianStruck,
        SamaIceGuardianDie,

        SamaLightningGuardianAttack,
        SamaLightningGuardianStruck,
        SamaLightningGuardianDie,

        SamaWindGuardianAttack,
        SamaWindGuardianStruck,
        SamaWindGuardianDie,

        PhoenixAttack,
        PhoenixStruck,
        PhoenixDie,

        BlackTortoiseAttack,
        BlackTortoiseStruck,
        BlackTortoiseDie,

        BlueDragonAttack,
        BlueDragonStruck,
        BlueDragonDie,

        WhiteTigerAttack,//TODO - missing sound
        WhiteTigerStruck,//TODO - missing sound
        WhiteTigerDie,//TODO - missing sound

        Terracotta1Attack,
        Terracotta1Struck,
        Terracotta1Die,

        Terracotta2Attack,
        Terracotta2Struck,
        Terracotta2Die,

        Terracotta3Attack,
        Terracotta3Struck,
        Terracotta3Die,

        Terracotta4Attack,
        Terracotta4Struck,
        Terracotta4Die,

        TerracottaSubAttack,
        TerracottaSubAttack2,
        TerracottaSubStruck,
        TerracottaSubDie,

        TerracottaBossAttack,
        TerracottaBossAttack2,
        TerracottaBossStruck,
        TerracottaBossDie,

        #endregion
    }

    #endregion

}
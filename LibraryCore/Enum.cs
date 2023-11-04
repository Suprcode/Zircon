using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public enum GridSelect
    {
        Single,
        Multi
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
        FrostBite = 310,

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
        Solo,
        Group,
        Guild
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
        Nothing,
        Consumable,
        Weapon,
        Armour,
        Torch,
        Helmet,
        Necklace,
        Bracelet,
        Ring,
        Shoes,
        Poison,
        Amulet,
        Meat,
        Ore,
        Book,
        Scroll,
        [Description("Dark Stone")]
        DarkStone,
        [Description("Refine Special")]
        RefineSpecial,
        [Description("Horse Armour")]
        HorseArmour,
        Flower,
        [Description("Companion Food")]
        CompanionFood,
        [Description("Companion Bag")]
        CompanionBag,
        [Description("Companion Head")]
        CompanionHead,
        [Description("Companion Back")]
        CompanionBack,
        System,
        [Description("Item Part")]
        ItemPart,
        Emblem,
        Shield,
        Costume,
        Hook,
        Float,
        Bait,
        Finder,
        Reel
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
        WeaponSkills,
        Neutral,
        Fire,
        Ice,
        Lightning,
        Wind,
        Holy,
        Dark,
        Phantom,
        Combat,
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
        ElementalSwords = 131,//NOT CODED

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
        FireBounce = 231,//Custom
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
        SoulResonance = 325,//NOT CODED
        Parasite = 326,
        Seance = 327,//NOT CODED
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
        Mindfulness = 343,//NOT CODED
        AugmentCelestialLight = 344,
        CorpseExploder = 345,
        Necromancy = 346,//NOT CODED

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
        Resolution = 422,//NOT CODED
        ChangeOfSeasons = 423,//Removed From Official
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
        Stealth = 435,//NOT CODED
        Evasion = 436,
        RagingWind = 437,
        Unused = 438,//UNUSED
        Massacre = 439,//Custom
        ArtOfShadows = 440,//Custom
        FatalBlow = 441,//TODO
        LastStand = 442,//TODO
        Vitality = 443,//TODO
        Chain = 444,//TODO
        Concentration = 445,//TODO
        DualWeaponSkills = 446,//TODO
        Containment = 447,//TODO
        Unknown = 448,//TODO
        ChainOfFire = 449,//TODO
        DragonBlood = 450,//TODO
        DragonWave = 451,//TODO
        MagicCombustion = 452,//TODO
        BurningFire = 453,//TODO

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

    public enum MonsterImage
    {
        None,

        Guard,

        Chicken,
        Pig,
        Deer,
        Cow,
        Sheep,
        ClawCat,
        Wolf,
        ForestYeti,
        ChestnutTree,
        CarnivorousPlant,
        Oma,
        TigerSnake,
        SpittingSpider,
        Scarecrow,
        OmaHero,

        CaveBat,
        Scorpion,
        Skeleton,
        SkeletonAxeMan,
        SkeletonAxeThrower,
        SkeletonWarrior,
        SkeletonLord,

        CaveMaggot,
        GhostSorcerer,
        GhostMage,
        VoraciousGhost,
        DevouringGhost,
        CorpseRaisingGhost,
        GhoulChampion,

        ArmoredAnt,
        AntSoldier,
        AntHealer,
        AntNeedler,

        ShellNipper,
        Beetle,
        VisceralWorm,

        MutantFlea,
        PoisonousMutantFlea,
        BlasterMutantFlea,

        WasHatchling,
        Centipede,
        ButterflyWorm,
        MutantMaggot,
        Earwig,
        IronLance,
        LordNiJae,

        RottingGhoul,
        DecayingGhoul,
        BloodThirstyGhoul,

        SpinedDarkLizard,
        UmaInfidel,
        UmaFlameThrower,
        UmaAnguisher,
        UmaKing,

        SpiderBat,
        ArachnidGazer,
        Larva,
        RedMoonGuardian,
        RedMoonProtector,
        VenomousArachnid,
        DarkArachnid,
        RedMoonTheFallen,

        ZumaSharpShooter,
        ZumaFanatic,
        ZumaGuardian,
        ViciousRat,
        ZumaKing,

        EvilFanatic,
        Monkey,
        EvilElephant,
        CannibalFanatic,

        SpikedBeetle,
        NumaGrunt,
        NumaMage,
        NumaElite,
        SandShark,
        StoneGolem,
        WindfurySorceress,
        CursedCactus,
        NetherWorldGate,

        RagingLizard,
        SawToothLizard,
        MutantLizard,
        VenomSpitter,
        SonicLizard,
        GiantLizard,
        CrazedLizard,
        TaintedTerror,
        DeathLordJichon,

        Minotaur,
        FrostMinotaur,
        ShockMinotaur,
        FlameMinotaur,
        FuryMinotaur,
        BanyaLeftGuard,
        BanyaRightGuard,
        EmperorSaWoo,

        BoneArcher,
        BoneBladesman,
        BoneCaptain,
        BoneSoldier,
        ArchLichTaedu,

        WedgeMothLarva,
        LesserWedgeMoth,
        WedgeMoth,
        RedBoar,
        ClawSerpent,
        BlackBoar,
        TuskLord,
        RazorTusk,

        PinkGoddess,
        GreenGoddess,
        MutantCaptain,
        StoneGriffin,
        FlameGriffin,

        WhiteBone,
        Shinsu,
        InfernalSoldier,
        InfernalGuardian,
        InfernalWarrior,

        CorpseStalker,
        LightArmedSoldier,
        CorrosivePoisonSpitter,
        PhantomSoldier,
        MutatedOctopus,
        AquaLizard,
        Stomper,
        CrimsonNecromancer,
        ChaosKnight,
        PachonTheChaosBringer,

        NumaCavalry,
        NumaHighMage,
        NumaStoneThrower,
        NumaRoyalGuard,
        NumaArmoredSoldier,

        IcyRanger,
        IcyGoddess,
        IcySpiritWarrior,
        IcySpiritGeneral,
        GhostKnight,
        IcySpiritSpearman,
        Werewolf,
        Whitefang,
        IcySpiritSolider,
        WildBoar,
        JinamStoneGate,
        FrostLordHwa,

        Companion_Pig,
        Companion_TuskLord,
        Companion_SkeletonLord,
        Companion_Griffin,
        Companion_Dragon,
        Companion_Donkey,
        Companion_Sheep,
        Companion_BanyoLordGuzak,
        Companion_Panda,
        Companion_Rabbit,

        JinchonDevil,
        OmaWarlord,

        EscortCommander,
        FieryDancer,
        EmeraldDancer,
        QueenOfDawn,

        OYoungBeast,
        YumgonWitch,
        MaWarlord,
        JinhwanSpirit,
        JinhwanGuardian,
        YumgonGeneral,
        ChiwooGeneral,
        DragonQueen,
        DragonLord,

        FerociousIceTiger,

        SamaFireGuardian,
        SamaIceGuardian,
        SamaLightningGuardian,
        SamaWindGuardian,
        Phoenix,
        BlackTortoise,
        BlueDragon,
        WhiteTiger,
        SamaCursedBladesman,
        SamaCursedSlave,
        SamaCursedFlameMage,
        SamaProphet,
        SamaSorcerer,
        EnshrinementBox,
        BloodStone,

        OrangeTiger,
        RegularTiger,
        RedTiger,
        SnowTiger,
        BlackTiger,
        BigBlackTiger,
        BigWhiteTiger,
        OrangeBossTiger,
        BigBossTiger,
        WildMonkey,
        FrostYeti,

        EvilSnake,
        Salamander,
        SandGolem,
        SDMob4,
        SDMob5,
        SDMob6,
        SDMob7,
        OmaMage,
        SDMob9,
        SDMob10,
        SDMob11,
        SDMob12,
        SDMob13,
        SDMob14,
        CrystalGolem,
        DustDevil,
        TwinTailScorpion,
        BloodyMole,
        SDMob19,
        SDMob20,
        SDMob21,
        SDMob22,
        SDMob23,
        SDMob24,
        SDMob25,
        GangSpider,
        VenomSpider,
        SDMob26,

        LobsterLord,
        LobsterSpawn,

        NewMob1,
        NewMob2,
        NewMob3,
        NewMob4,
        NewMob5,
        NewMob6,
        NewMob7,
        NewMob8,
        NewMob9,
        NewMob10,

        MonasteryMon0,
        MonasteryMon1,
        MonasteryMon2,
        MonasteryMon3,
        MonasteryMon4,
        MonasteryMon5,
        MonasteryMon6,

        CastleFlag,

        Tornado
    }

    
    public enum MapIcon
    {
        None,
        Cave,
        Exit,
        Down,
        Up,
        Province,
        Building
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

        Puppet,
        PuppetFire,
        PuppetIce,
        PuppetLightning,
        PuppetWind,

        SummonSkeleton,
        SummonShinsu,
        CursedDoll,

        ThunderBolt,
        FrostBiteEnd,

        DanceOfSwallow,
        FlashOfLight,

        DemonExplosion,
        ParasiteExplode
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
        Burn = 1 << 10,         //Tick damage, displays effect
    }

    public enum SpellEffect
    {
        None,

        SafeZone,

        FireWall,
        Tempest,

        TrapOctagon,

        PoisonousCloud,
        DarkSoulPrison,

        Rubble,

        MonsterDeathCloud,
    }


    public enum MagicEffect
    {
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
        Abyss,
        DragonRepulse,

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
        None,

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
    }

    public enum CurrencyType
    {
        Gold = 0,
        GameGold = 1,
        HuntGold = 2,
        Other = 3
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

        CastleObjective = 10,

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
        Crashed
    }

    public enum InstanceResult : byte
    {
        Invalid,
        InsufficientLevel,
        SafeZoneOnly,
        NotInGroup,
        NotInGuild,
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

        AssaultStart,

        SwiftBladeEnd,

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

        RakeStart,

        Karma,

        TheNewBeginning,

        SummonPuppet,

        DanceOfSwallowsEnd,
        DragonRepulseStart,
        AbyssStart,
        FlashOfLightEnd,
        EvasionStart,
        RagingWindStart,

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

        WhiteTigerAttack,
        WhiteTigerStruck,
        WhiteTigerDie,

        #endregion
    }

    #endregion

}
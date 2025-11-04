using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MemoryPack;

namespace Library.Network.ServerPackets
{
    [MemoryPackable]
    public sealed partial class NewAccount : Packet
    {
        public NewAccountResult Result { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ChangePassword : Packet
    {
        public ChangePasswordResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    [MemoryPackable]
    public sealed partial class Login : Packet
    {
        public LoginResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }

        public List<SelectInfo> Characters { get; set; }
        public List<ClientUserItem> Items { get; set; }

        public List<ClientBlockInfo> BlockList { get; set; }

        public string Address { get; set; }

        public bool TestServer { get; set; }
    }
    [MemoryPackable]
    public sealed partial class RequestPasswordReset : Packet
    {
        public RequestPasswordResetResult Result { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ResetPassword : Packet
    {
        public ResetPasswordResult Result { get; set; }
    }
    [MemoryPackable]
    public sealed partial class Activation : Packet
    {
        public ActivationResult Result { get; set; }
    }
    [MemoryPackable]
    public sealed partial class RequestActivationKey : Packet
    {
        public RequestActivationKeyResult Result { get; set; }
        public TimeSpan Duration { get; set; }
    }
    [MemoryPackable]
    public sealed partial class SelectLogout : Packet
    {
    }
    [MemoryPackable]
    public sealed partial class GameLogout : Packet
    {
        public List<SelectInfo> Characters { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NewCharacter : Packet
    {
        public NewCharacterResult Result { get; set; }

        public SelectInfo Character { get; set; }
    }
    [MemoryPackable]
    public sealed partial class DeleteCharacter : Packet
    {
        public DeleteCharacterResult Result { get; set; }

        public int DeletedIndex { get; set; }
    }
    [MemoryPackable]
    public sealed partial class StartGame : Packet
    {
        public StartGameResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }

        public StartInformation StartInformation { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MapChanged : Packet
    {
        public int MapIndex { get; set; }
        public int InstanceIndex { get; set; }
    }
    [MemoryPackable]
    public sealed partial class UserLocation : Packet
    {
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectRemove : Packet
    {
        public uint ObjectID { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectTurn : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectHarvest : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectMount : Packet
    {
        public uint ObjectID { get; set; }
        public HorseType Horse { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectFishing : Packet
    {
        public uint ObjectID { get; set; }
        public FishingState State { get; set; }
        public MirDirection Direction { get; set; }
        public Point FloatLocation { get; set; }
        public bool FishFound { get; set; }
    }
    [MemoryPackable]
    public sealed partial class FishingStats : Packet
    {
        public bool CanAutoCast { get; set; }
        public int CurrentPoints { get; set; }

        public int ThrowQuality { get; set; } //1 time
        public int RequiredPoints { get; set; } //1 time
        public int MovementSpeed { get; set; } //1 time
        public int RequiredAccuracy { get; set; } //1 time
    }
    [MemoryPackable]
    public sealed partial class ObjectMove : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public TimeSpan Slow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectDash : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public MagicType Magic { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectPushed : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObjectIdle : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Type { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObjectAttack : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public uint TargetID { get; set; }

        public TimeSpan Slow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectRangeAttack : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public List<uint> Targets { get; set; } = new List<uint>();
    }
    [MemoryPackable]
    public sealed partial class ObjectMagic : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public MagicType Type { get; set; }
        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
        public bool Cast { get; set; }
        public Element AttackElement { get; set; }

        public TimeSpan Slow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectProjectile : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public MagicType Type { get; set; }
        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
    }

    [MemoryPackable]
    public sealed partial class ObjectMining : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public TimeSpan Slow { get; set; }
        public bool Effect { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectPetOwnerChanged : Packet
    {
        public uint ObjectID { get; set; }
        public string PetOwner { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectShow : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectHide : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectEffect : Packet
    {
        public uint ObjectID { get; set; }

        public Effect Effect { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MapEffect : Packet
    {
        public Point Location { get; set; }
        public Effect Effect { get; set; }
        public MirDirection Direction { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectBuffAdd : Packet
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
        public int Extra { get;set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectBuffRemove : Packet
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectPoison : Packet
    {
        public uint ObjectID { get; set; }
        public PoisonType Poison { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectPlayer : Packet
    {
        public int Index { get; set; }

        public uint ObjectID { get; set; }
        public string Name { get; set; }

        public string Caption { get; set; }
        [MemoryPackAllowSerialize]
        public Color NameColour { get; set; }
        public string GuildName { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }

        public int HairType { get; set; }
        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
        public int Weapon { get; set; }
        public int Shield { get; set; }
        public int Armour { get; set; }
        public int Costume { get; set; }
        [MemoryPackAllowSerialize]
        public Color ArmourColour { get; set; }
        public ExteriorEffect ArmourEffect { get; set; }
        public ExteriorEffect EmblemEffect { get; set; }
        public ExteriorEffect WeaponEffect { get; set; }
        public ExteriorEffect ShieldEffect { get; set; }

        public int Light { get; set; }

        public bool Dead { get; set; }
        public PoisonType Poison { get; set; }

        public Dictionary<BuffType, int> Buffs { get; set; }

        public HorseType Horse { get; set; }

        public int Helmet { get; set; }

        public int HorseShape { get; set; }

        public string FiltersClass;
        public string FiltersRarity;
        public string FiltersItemType;

        public bool HideHead;
    }
    [MemoryPackable]
    public sealed partial class ObjectMonster : Packet
    {
        public uint ObjectID { get; set; }
        public int MonsterIndex { get; set; }
        public string CustomName { get; set; }
        [MemoryPackAllowSerialize]
        public Color NameColour { get; set; }
        public string PetOwner { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public bool Dead { get; set; }
        public bool Skeleton { get; set; }

        public PoisonType Poison { get; set; }

        public bool EasterEvent { get; set; }
        public bool HalloweenEvent { get; set; }
        public bool ChristmasEvent { get; set; }

        public Dictionary<BuffType, int> Buffs { get; set; }
        public bool Extra { get; set; }

        public int Extra1 { get; set; }
        [MemoryPackAllowSerialize]
        public Color Colour { get; set; }

        public ClientCompanionObject CompanionObject { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObjectNPC : Packet
    {
        public uint ObjectID { get; set; }

        public int NPCIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public MirDirection Direction { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectItem : Packet
    {
        public uint ObjectID { get; set; }

        public ClientUserItem Item { get; set; }

        public Point Location { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectSpell : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public SpellEffect Effect { get; set; }
        public int Power { get; set; }

    }
    [MemoryPackable]
    public sealed partial class ObjectSpellChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Power { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectNameColour : Packet
    {
        public uint ObjectID { get; set; }
        [MemoryPackAllowSerialize]
        public Color Colour { get; set; }
    }

    [MemoryPackable]
    public sealed partial class PlayerUpdate : Packet
    {
        public uint ObjectID { get; set; }
        public int Weapon { get; set; }
        public int Shield { get; set; }
        public int Armour { get; set; }
        public int Costume { get; set; }
        [MemoryPackAllowSerialize]
        public Color ArmourColour { get; set; }
        public ExteriorEffect ArmourEffect { get; set; }
        public ExteriorEffect EmblemEffect { get; set; }
        public ExteriorEffect WeaponEffect { get; set; }
        public ExteriorEffect ShieldEffect { get; set; }

        public int HorseArmour { get; set; }
        public int Helmet { get; set; }
        public int Light { get; set; }

        public bool HideHead { get; set; }
    }


    [MemoryPackable]
    public sealed partial class MagicToggle : Packet
    {
        public MagicType Magic { get; set; }
        public bool CanUse { get; set; }
    }


    [MemoryPackable]
    public sealed partial class DayChanged : Packet
    {
        public float DayTime { get; set; }
    }

    [MemoryPackable]
    public sealed partial class InformMaxExperience : Packet
    {
        public decimal MaxExperience { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LevelChanged : Packet
    {
        public int Level { get; set; }
        public decimal Experience { get; set; }
        public decimal MaxExperience { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectLeveled : Packet
    {
        public uint ObjectID { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectRevive : Packet
    {
        public uint ObjectID { get; set; }
        public Point Location { get; set; }
        public bool Effect { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GainedExperience : Packet
    {
        public decimal Amount { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NewMagic : Packet
    {
        public ClientUserMagic Magic { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MagicLeveled : Packet
    {
        public int InfoIndex { get; set; }
        [MemoryPackIgnore]
        public MagicInfo Info;
        public int Level { get; set; }
        public long Experience { get; set; }

        [MemoryPackOnDeserialized]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }
    [MemoryPackable]
    public sealed partial class MagicCooldown : Packet
    {
        public int InfoIndex { get; set; }
        public int Delay { get; set; }
        [MemoryPackIgnore]
        public MagicInfo Info;

        [MemoryPackOnDeserialized]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }

    [MemoryPackable]
    public sealed partial class StatsUpdate : Packet
    {
        public Stats Stats { get; set; }
        public Stats HermitStats { get; set; }
        public int HermitPoints { get; set; }
    }
    [MemoryPackable]
    public sealed partial class HealthChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
        public bool Miss { get; set; }
        public bool Block { get; set; }
        public bool Critical { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectStats : Packet
    {
        public uint ObjectID { get; set; }
        public Stats Stats { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ManaChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
    }

    [MemoryPackable]
    public sealed partial class FocusChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObjectStruck : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public uint AttackerID { get; set; }
        public Element Element { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectDied : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObjectHarvested : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }



    [MemoryPackable]
    public sealed partial class ItemsGained : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ItemMove : Packet
    {
        public GridType FromGrid { get; set; }
        public GridType ToGrid { get; set; }
        public int FromSlot { get; set; }
        public int ToSlot { get; set; }
        public bool MergeItem { get; set; }

        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemSort : Packet
    {
        public GridType Grid { get; set; }
        public List<ClientUserItem> Items { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemSplit : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
        public int NewSlot { get; set; }

        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemDelete : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemLock : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public bool Locked { get; set; }

    }

    [MemoryPackable]
    public sealed partial class ItemUseDelay : Packet
    {
        public TimeSpan Delay { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ItemChanged : Packet
    {
        public CellLinkInfo Link { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemStatsChanged : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ItemStatsRefreshed : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ItemDurability : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public int CurrentDurability { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ItemExperience : Packet
    {
        public CellLinkInfo Target { get; set; }
        public decimal Experience { get; set; }
        public int Level { get; set; }
        public UserItemFlags Flags { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Chat : Packet
    {
        public uint ObjectID { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public List<ClientUserItem> LinkedItems { get; set; }
        public bool OverheadOnly { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCResponse : Packet
    {
        public uint ObjectID { get; set; }
        public int Index { get; set; }
        public List<ClientNPCValues> Values { get; set; }

        [MemoryPackIgnore]
        public NPCPage Page;

        [MemoryPackOnDeserialized]
        public void Complete()
        {
            Page = Globals.NPCPageList.Binding.FirstOrDefault(x => x.Index == Index);
        }
    }
    [MemoryPackable]
    public sealed partial class ItemsChanged : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Success { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCRepair : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool Success { get; set; }
        public TimeSpan SpecialRepairDelay { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCRefinementStone : Packet
    {
        public List<CellLinkInfo> IronOres { get; set; }
        public List<CellLinkInfo> SilverOres { get; set; }
        public List<CellLinkInfo> DiamondOres { get; set; }
        public List<CellLinkInfo> GoldOres { get; set; }
        public List<CellLinkInfo> Crystal { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public RefineQuality RefineQuality { get; set; }
        public List<CellLinkInfo> Ores { get; set; }
        public List<CellLinkInfo> Items { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
        public bool Success { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCMasterRefine : Packet
    {
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }

        public bool Success { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCClose : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class NPCAccessoryLevelUp : Packet
    {
        public CellLinkInfo Target { get; set; }
        public List<CellLinkInfo> Links { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCAccessoryUpgrade : Packet
    {
        public CellLinkInfo Target { get; set; }
        public RefineType RefineType { get; set; }
        public bool Success { get; set; }
    }


    [MemoryPackable]
    public sealed partial class NPCRefineRetrieve : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class RefineList : Packet
    {
        public List<ClientRefineInfo> List { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GroupSwitch : Packet
    {
        public bool Allow { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GroupMember : Packet
    {
        public uint ObjectID { get; set; }
        public string Name { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GroupRemove : Packet
    {
        public uint ObjectID { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GroupInvite : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BuffAdd : Packet
    {
        public ClientBuffInfo Buff { get; set; }
    }
    [MemoryPackable]
    public sealed partial class BuffRemove : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class BuffChanged : Packet
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }
    }
    [MemoryPackable]
    public sealed partial class BuffTime : Packet
    {
        public int Index { get; set; }
        public TimeSpan Time { get; set; }
    }
    [MemoryPackable]
    public sealed partial class BuffPaused : Packet
    {
        public int Index { get; set; }
        public bool Paused { get; set; }
    }
    [MemoryPackable]
    public sealed partial class SafeZoneChanged : Packet
    {
        public bool InSafeZone { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CombatTime : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class Inspect : Packet
    {
        public string Name { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }
        public int GuildFlag { get; set; } = -1;
        [MemoryPackAllowSerialize]
        public Color GuildColour { get; set; }
        public string Partner { get; set; }
        public MirClass Class { get; set; }
        public int Level { get; set; }
        public MirGender Gender { get; set; }
        //public Stats Stats { get; set; }
        //public Stats HermitStats { get; set; }
        //public int HermitPoints { get; set; }
        public List<ClientUserItem> Items { get; set; }
        public int Hair { get; set; }
        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
        public int Fame { get; set; }

        //public int WearWeight { get; set; }
        //public int HandWeight { get; set; }

        public bool Ranking { get; set; }
    }
    [MemoryPackable]
    public sealed partial class Rankings : Packet
    {
        public bool OnlineOnly { get; set; }
        public RequiredClass Class { get; set; }
        public int StartIndex { get; set; }
        public int Total { get; set; }
        public bool AllowObservation { get; set; }

        public List<RankInfo> Ranks { get; set; }
    }
    [MemoryPackable]
    public sealed partial class RankSearch : Packet
    {
        public RankInfo Rank { get; set; }
    }

    [MemoryPackable]
    public sealed partial class StartObserver : Packet
    {
        public StartInformation StartInformation { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ObservableSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceHistory : Packet
    {
        public int Index { get; set; }
        public long SaleCount { get; set; }
        public long LastPrice { get; set; }
        public long AveragePrice { get; set; }
        public int Display { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceConsign : Packet
    {
        public List<ClientMarketPlaceInfo> Consignments { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceSearch : Packet
    {
        public int Count { get; set; }
        public List<ClientMarketPlaceInfo> Results { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceSearchCount : Packet
    {
        public int Count { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceSearchIndex : Packet
    {
        public int Index { get; set; }
        public ClientMarketPlaceInfo Result { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceBuy : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
        public bool Success { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceStoreBuy : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class MarketPlaceConsignChanged : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }


    [MemoryPackable]
    public sealed partial class MailList : Packet
    {
        public List<ClientMailInfo> Mail { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailNew : Packet
    {
        public ClientMailInfo Mail { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailDelete : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailItemDelete : Packet
    {
        public int Index { get; set; }
        public int Slot { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailSend : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class ChangeAttackMode : Packet
    {
        public AttackMode Mode { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ChangePetMode : Packet
    {
        public PetMode Mode { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CurrencyChanged : Packet
    {
        public int CurrencyIndex { get; set; }
        public long Amount { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MountFailed : Packet
    {
        public HorseType Horse { get; set; }
    }

    [MemoryPackable]
    public sealed partial class WeightUpdate : Packet
    {
        public int BagWeight { get; set; }
        public int WearWeight { get; set; }
        public int HandWeight { get; set; }
    }


    [MemoryPackable]
    public sealed partial class TradeRequest : Packet
    {
        public string Name { get; set; }
    }
    [MemoryPackable]
    public sealed partial class TradeOpen : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TradeClose : Packet { }

    [MemoryPackable]
    public sealed partial class TradeAddItem : Packet
    {
        public CellLinkInfo Cell { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TradeAddGold : Packet
    {
        public long Gold { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TradeItemAdded : Packet
    {
        public ClientUserItem Item { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TradeGoldAdded : Packet
    {
        public long Gold { get; set; }
    }
    [MemoryPackable]
    public sealed partial class TradeUnlock : Packet { }


    [MemoryPackable]
    public sealed partial class GuildCreate : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildInfo : Packet
    {
        public ClientGuildInfo Guild { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildNoticeChanged : Packet
    {
        public string Notice { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildNewItem : Packet
    {
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
        //public int Count { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildGetItem : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildUpdate : Packet
    {
        public int MemberLimit { get; set; }
        public int StorageLimit { get; set; }

        public long GuildFunds { get; set; }
        public long DailyGrowth { get; set; }

        public int GuildLevel { get; set; }
        public int Tax { get; set; }

        public long TotalContribution { get; set; }
        public long DailyContribution { get; set; }

        public string DefaultRank { get; set; }
        public GuildPermission DefaultPermission { get; set; }

        [MemoryPackAllowSerialize]
        public Color Colour { get; set; }
        public int Flag { get; set; }

        public List<ClientGuildMemberInfo> Members { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildKick : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildTax : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildIncreaseMember : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildIncreaseStorage : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildInviteMember : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildInvite : Packet
    {
        public string Name { get; set; }
        public string GuildName { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildStats : Packet
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }

    }

    [MemoryPackable]
    public sealed partial class GuildMemberOffline : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildMemberOnline : Packet
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public uint ObjectID { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildMemberContribution : Packet
    {
        public int Index { get; set; }

        public long Contribution { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildDayReset : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class GuildFundsChanged : Packet
    {
        public long Change { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildChanged : Packet
    {
        public uint ObjectID { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildWarFinished : Packet
    {
        public string GuildName { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildWar : Packet
    {
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildWarStarted : Packet
    {
        public string GuildName { get; set; }
        public TimeSpan Duration { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildConquestDate : Packet
    {
        public int Index { get; set; }
        public TimeSpan WarTime { get; set; }

        [MemoryPackIgnore]
        public DateTime WarDate;

        [MemoryPackOnDeserialized]
        public void Update()
        {
            if (WarTime == TimeSpan.MinValue)
                WarDate = DateTime.MinValue;
            else
                WarDate = Time.Now + WarTime;
        }
    }
    [MemoryPackable]
    public sealed partial class GuildCastleInfo : Packet
    {
        public int Index { get; set; }
        public string Owner { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildConquestStarted : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildConquestFinished : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ReviveTimers : Packet
    {
        public TimeSpan ItemReviveTime { get; set; }
        public TimeSpan ReincarnationPillTime { get; set; }
    }

    [MemoryPackable]
    public sealed partial class QuestChanged : Packet
    {
        public ClientUserQuest Quest { get; set; }
    }

    [MemoryPackable]
    public sealed partial class QuestCancelled : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CompanionUnlock : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionAdopt : Packet
    {
        public ClientUserCompanion UserCompanion { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionRetrieve : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CompanionRelease : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CompanionStore : Packet
    {
    }
    [MemoryPackable]
    public sealed partial class CompanionWeightUpdate : Packet
    {
        public int BagWeight { get; set; }
        public int MaxBagWeight { get; set; }
        public int InventorySize { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionShapeUpdate : Packet
    {
        public uint ObjectID { get; set; }
        public int HeadShape { get; set; }
        public int BackShape { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionItemsGained : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionUpdate : Packet
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Hunger { get; set; }
    }
    [MemoryPackable]
    public sealed partial class CompanionSkillUpdate : Packet
    {
        public Stats Level3 { get; set; }
        public Stats Level5 { get; set; }
        public Stats Level7 { get; set; }
        public Stats Level10 { get; set; }
        public Stats Level11 { get; set; }
        public Stats Level13 { get; set; }
        public Stats Level15 { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarriageInvite : Packet
    {
        public string Name { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarriageInfo : Packet
    {
        public ClientPlayerInfo Partner { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarriageRemoveRing : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class MarriageMakeRing : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class MarriageOnlineChanged : Packet
    {
        public uint ObjectID { get; set; }
    }

    [MemoryPackable]
    public sealed partial class DataObjectRemove : Packet
    {
        public uint ObjectID { get; set; }
    }
    [MemoryPackable]
    public sealed partial class DataObjectPlayer : Packet
    {
        public uint ObjectID { get; set; }
        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public string Name { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Dead { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
    }
    [MemoryPackable]
    public sealed partial class DataObjectMonster : Packet
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        [MemoryPackIgnore]
        public MonsterInfo MonsterInfo;
        public int MonsterIndex { get; set; }
        public string PetOwner { get; set; }

        public int Health { get; set; }
        public Stats Stats { get; set; }
        public bool Dead { get; set; }

        [MemoryPackOnDeserialized]
        public void OnComplete()
        {
            MonsterInfo = Globals.MonsterInfoList.Binding.First(x => x.Index == MonsterIndex);
        }
    }
    [MemoryPackable]
    public sealed partial class DataObjectItem : Packet
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        [MemoryPackIgnore]
        public ItemInfo ItemInfo;
        public int ItemIndex { get; set; }

        [MemoryPackOnDeserialized]
        public void OnComplete()
        {
            ItemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == ItemIndex);
        }
    }
    [MemoryPackable]
    public sealed partial class DataObjectLocation : Packet
    {
        public uint ObjectID { get; set; }
        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }
    }
    [MemoryPackable]
    public sealed partial class DataObjectHealthMana : Packet
    {
        public uint ObjectID { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Dead { get; set; }
    }
    [MemoryPackable]
    public sealed partial class DataObjectMaxHealthMana : Packet
    {
        public uint ObjectID { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public Stats Stats { get; set; }
    }
    [MemoryPackable]
    public sealed partial class BlockAdd : Packet
    {
        public ClientBlockInfo Info { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BlockRemove : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class HelmetToggle : Packet
    {
        public bool HideHelmet { get; set; }
    }

    [MemoryPackable]
    public sealed partial class StorageSize : Packet
    {
        public int Size { get; set; }
    }

    [MemoryPackable]
    public sealed partial class PlayerChangeUpdate : Packet
    {

        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public MirGender Gender { get; set; }
        public int HairType { get; set; }

        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
        [MemoryPackAllowSerialize]
        public Color ArmourColour { get; set; }

    }

    [MemoryPackable]
    public sealed partial class FortuneUpdate : Packet
    {
        public List<ClientFortuneInfo> Fortunes { get; set; }

    }
    [MemoryPackable]
    public sealed partial class NPCWeaponCraft : Packet
    {
        public CellLinkInfo Template { get; set; }
        public CellLinkInfo Yellow { get; set; }
        public CellLinkInfo Blue { get; set; }
        public CellLinkInfo Red { get; set; }
        public CellLinkInfo Purple { get; set; }
        public CellLinkInfo Green { get; set; }
        public CellLinkInfo Grey { get; set; }

        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCAccessoryRefine : Packet
    {
        public CellLinkInfo Target { get; set; }
        public CellLinkInfo OreTarget { get; set; }
        public List<CellLinkInfo> Links { get; set; }
        public RefineType RefineType { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemAcessoryRefined : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }

    [MemoryPackable]
    public sealed partial class JoinInstance : Packet
    {
        public InstanceResult Result { get; set; }
        public bool Success { get; set; }
    }

    [MemoryPackable]
    public sealed partial class SendCompanionFilters : Packet
    {
        public List<MirClass> FilterClass { get; set; }
        public List<Rarity> FilterRarity { get; set; }
        public List<ItemType> FilterItemType { get; set; }
    }

    [MemoryPackable]
    public sealed partial class FriendUpdate : Packet
    {
        public ClientFriendInfo Info { get; set; }
    }

    [MemoryPackable]
    public sealed partial class FriendAdd : Packet
    {
        public ClientFriendInfo Info { get; set; }
    }
    [MemoryPackable]
    public sealed partial class FriendRemove : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class DisciplineUpdate : Packet
    {
        public ClientUserDiscipline Discipline { get; set; }
    }

    [MemoryPackable]
    public sealed partial class DisciplineExperienceChanged : Packet
    {
        public long Experience { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCRoll : Packet
    {
        public int Type { get; set; }
        public int Result { get; set; }
    }

    [MemoryPackable]
    public sealed partial class SetTimer : Packet
    {
        public string Key { get; set; }
        public byte Type { get; set; }
        public int Seconds { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxOpen : Packet
    {
        public int Slot { get; set; }
        public List<ClientLootBoxItemInfo> Items { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxClose : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class BundleOpen : Packet
    {
        public int Slot { get; set; }
        public List<ClientBundleItemInfo> Items { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BundleClose : Packet
    {

    }
}


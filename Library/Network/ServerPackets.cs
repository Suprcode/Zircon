using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Library.SystemModels;

namespace Library.Network.ServerPackets
{
    public sealed class NewAccount : Packet
    {
        public NewAccountResult Result { get; set; }
    }
    public sealed class ChangePassword : Packet
    {
        public ChangePasswordResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public sealed class Login : Packet
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
    public sealed class RequestPasswordReset : Packet
    {
        public RequestPasswordResetResult Result { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public sealed class ResetPassword : Packet
    {
        public ResetPasswordResult Result { get; set; }
    }
    public sealed class Activation : Packet
    {
        public ActivationResult Result { get; set; }
    }
    public sealed class RequestActivationKey : Packet
    {
        public RequestActivationKeyResult Result { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public sealed class SelectLogout : Packet
    {
    }
    public sealed class GameLogout : Packet
    {
        public List<SelectInfo> Characters { get; set; }
    }

    public sealed class NewCharacter : Packet
    {
        public NewCharacterResult Result { get; set; }

        public SelectInfo Character { get; set; }
    }

    public sealed class DeleteCharacter : Packet
    {
        public DeleteCharacterResult Result { get; set; }

        public int DeletedIndex { get; set; }
    }
    public sealed class StartGame : Packet
    {
        public StartGameResult Result { get; set; }

        public string Message { get; set; }
        public TimeSpan Duration { get; set; }

        public StartInformation StartInformation { get; set; }
    }


    public sealed class MapChanged : Packet
    {
        public int MapIndex { get; set; }
    }
    public sealed class UserLocation : Packet
    {
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectRemove : Packet
    {
        public uint ObjectID { get; set; }
    }

    public sealed class ObjectTurn : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectHarvest : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public TimeSpan Slow { get; set; }
    }

    public sealed class ObjectMount : Packet
    {
        public uint ObjectID { get; set; }
        public HorseType Horse { get; set; }
    }
    public sealed class ObjectMove : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectDash : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public int Distance { get; set; }
        public MagicType Magic { get; set; }
    }

    public sealed class ObjectPushed : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }

    public sealed class ObjectAttack : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public uint TargetID { get; set; }

        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectRangeAttack : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MagicType AttackMagic { get; set; }
        public Element AttackElement { get; set; }

        public List<uint> Targets { get; set; } = new List<uint>();
    }
    public sealed class ObjectMagic : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point CurrentLocation { get; set; }

        public MagicType Type { get; set; }
        public List<uint> Targets { get; set; } = new List<uint>();
        public List<Point> Locations { get; set; } = new List<Point>();
        public bool Cast { get; set; }

        public TimeSpan Slow { get; set; }
    }
    public sealed class ObjectMining : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public TimeSpan Slow { get; set; }
        public bool Effect { get; set; }
    }

    public sealed class ObjectPetOwnerChanged : Packet
    {
        public uint ObjectID { get; set; }
        public string PetOwner { get; set; }
    }

    public sealed class ObjectShow : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectHide : Packet
    {
        public uint ObjectID { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectEffect : Packet
    {
        public uint ObjectID { get; set; }

        public Effect Effect { get; set; }
    }
    public sealed class MapEffect : Packet
    {
        public Point Location { get; set; }
        public Effect Effect { get; set; }
        public MirDirection Direction { get; set; }
    }

    public sealed class ObjectBuffAdd : Packet
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
    }
    public sealed class ObjectBuffRemove : Packet
    {
        public uint ObjectID { get; set; }
        public BuffType Type { get; set; }
    }
    public sealed class ObjectPoison : Packet
    {
        public uint ObjectID { get; set; }
        public PoisonType Poison { get; set; }
    }
    public sealed class ObjectPlayer : Packet
    {
        public int Index { get; set; }

        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public Color NameColour { get; set; }
        public string GuildName { get; set; }

        public MirDirection Direction { get; set; }
        public Point Location { get; set; }

        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }

        public int HairType { get; set; }
        public Color HairColour { get; set; }
        public int Weapon { get; set; }
        public int Shield { get; set; }
        public int Armour { get; set; }
        public Color ArmourColour { get; set; }
        public int ArmourImage { get; set; }
        public int EmblemShape { get; set; }
        public int Wings { get; set; }

        public int Light { get; set; }

        public bool Dead { get; set; }
        public PoisonType Poison { get; set; }

        public List<BuffType> Buffs { get; set; }

        public HorseType Horse { get; set; }

        public int Helmet { get; set; }

        public int HorseShape { get; set; }
    }
    public sealed class ObjectMonster : Packet
    {
        public uint ObjectID { get; set; }
        public int MonsterIndex { get; set; }
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

        public List<BuffType> Buffs { get; set; }
        public bool Extra { get; set; }

        public ClientCompanionObject CompanionObject { get; set; }

        //public bool Extra { get; set; }
        //public int ExtraInt { get; set; }

    }
    public sealed class ObjectNPC : Packet
    {
        public uint ObjectID { get; set; }

        public int NPCIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public MirDirection Direction { get; set; }
    }
    public sealed class ObjectItem : Packet
    {
        public uint ObjectID { get; set; }

        public ClientUserItem Item { get; set; }

        public Point Location { get; set; }
    }
    public sealed class ObjectSpell : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public SpellEffect Effect { get; set; }
        public int Power { get; set; }

    }
    public sealed class ObjectSpellChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Power { get; set; }
    }
    public sealed class ObjectNameColour : Packet
    {
        public uint ObjectID { get; set; }
        public Color Colour { get; set; }
    }

    public sealed class PlayerUpdate : Packet
    {
        public uint ObjectID { get; set; }
        public int Weapon { get; set; }
        public int Shield { get; set; }
        public int Armour { get; set; }
        public Color ArmourColour { get; set; }
        public int ArmourImage { get; set; }
        public int EmblemShape { get; set; }

        public int HorseArmour { get; set; }
        public int Helmet { get; set; }
        public int WingsShape { get; set; }
        public int Light { get; set; }
    }


    public sealed class MagicToggle : Packet
    {
        public MagicType Magic { get; set; }
        public bool CanUse { get; set; }
    }


    public sealed class DayChanged : Packet
    {
        public float DayTime { get; set; }
    }

    public sealed class InformMaxExperience : Packet
    {
        public decimal MaxExperience { get; set; }
    }

    public sealed class LevelChanged : Packet
    {
        public int Level { get; set; }
        public decimal Experience { get; set; }
        public decimal MaxExperience { get; set; }
    }
    public sealed class ObjectLeveled : Packet
    {
        public uint ObjectID { get; set; }
    }
    public sealed class ObjectRevive : Packet
    {
        public uint ObjectID { get; set; }
        public Point Location { get; set; }
        public bool Effect { get; set; }
    }
    public sealed class GainedExperience : Packet
    {
        public decimal Amount { get; set; }
    }

    public sealed class NewMagic : Packet
    {
        public ClientUserMagic Magic { get; set; }
    }
    public sealed class MagicLeveled : Packet
    {
        public int InfoIndex { get; set; }
        public MagicInfo Info;
        public int Level { get; set; }
        public long Experience { get; set; }

        [CompleteObject]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }
    public sealed class MagicCooldown : Packet
    {
        public int InfoIndex { get; set; }
        public int Delay { get; set; }
        public MagicInfo Info;

        [CompleteObject]
        public void Complete()
        {
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }

    public sealed class StatsUpdate : Packet
    {
        public Stats Stats { get; set; }
        public Stats HermitStats { get; set; }
        public int HermitPoints { get; set; }
    }
    public sealed class HealthChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
        public bool Miss { get; set; }
        public bool Block { get; set; }
        public bool Critical { get; set; }
    }
    public sealed class ObjectStats : Packet
    {
        public uint ObjectID { get; set; }
        public Stats Stats { get; set; }
    }

    public sealed class ManaChanged : Packet
    {
        public uint ObjectID { get; set; }
        public int Change { get; set; }
    }

    public sealed class ObjectStruck : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
        public uint AttackerID { get; set; }
        public Element Element { get; set; }
    }
    public sealed class ObjectDied : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }
    public sealed class ObjectHarvested : Packet
    {
        public uint ObjectID { get; set; }
        public MirDirection Direction { get; set; }
        public Point Location { get; set; }
    }



    public sealed class ItemsGained : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class ItemMove : Packet
    {
        public GridType FromGrid { get; set; }
        public GridType ToGrid { get; set; }
        public int FromSlot { get; set; }
        public int ToSlot { get; set; }
        public bool MergeItem { get; set; }

        public bool Success { get; set; }
    }

    public sealed class ItemSplit : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
        public int NewSlot { get; set; }

        public bool Success { get; set; }
    }

    public sealed class ItemLock : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public bool Locked { get; set; }

    }

    public sealed class ItemUseDelay : Packet
    {
        public TimeSpan Delay { get; set; }
    }
    public sealed class ItemChanged : Packet
    {
        public CellLinkInfo Link { get; set; }
        public bool Success { get; set; }
    }

    public sealed class ItemStatsChanged : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }
    public sealed class ItemStatsRefreshed : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }
    public sealed class ItemDurability : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public int CurrentDurability { get; set; }
    }
    public sealed class GoldChanged : Packet
    {
        public long Gold { get; set; }
    }
    public sealed class ItemExperience : Packet
    {
        public CellLinkInfo Target { get; set; }
        public decimal Experience { get; set; }
        public int Level { get; set; }
        public UserItemFlags Flags { get; set; }
    }

    public sealed class Chat : Packet
    {
        public uint ObjectID { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }

    public sealed class NPCResponse : Packet
    {
        public uint ObjectID { get; set; }
        public int Index { get; set; }
        public List<ClientRefineInfo> Extra { get; set; }

        public NPCPage Page;

        [CompleteObject]
        public void Complete()
        {
            Page = Globals.NPCPageList.Binding.FirstOrDefault(x => x.Index == Index);
        }
    }
    public sealed class ItemsChanged : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Success { get; set; }
    }
    public sealed class NPCRepair : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool Success { get; set; }
        public TimeSpan SpecialRepairDelay { get; set; }
    }
    public sealed class NPCRefinementStone : Packet
    {
        public List<CellLinkInfo> IronOres { get; set; }
        public List<CellLinkInfo> SilverOres { get; set; }
        public List<CellLinkInfo> DiamondOres { get; set; }
        public List<CellLinkInfo> GoldOres { get; set; }
        public List<CellLinkInfo> Crystal { get; set; }
    }
    public sealed class NPCRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public RefineQuality RefineQuality { get; set; }
        public List<CellLinkInfo> Ores { get; set; }
        public List<CellLinkInfo> Items { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
        public bool Success { get; set; }
    }
    public sealed class NPCMasterRefine : Packet
    {
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }

        public bool Success { get; set; }
    }
    public sealed class NPCClose : Packet
    {
    }

    public sealed class NPCAccessoryLevelUp : Packet
    {
        public CellLinkInfo Target { get; set; }
        public List<CellLinkInfo> Links { get; set; }
    }

    public sealed class NPCAccessoryUpgrade : Packet
    {
        public CellLinkInfo Target { get; set; }
        public RefineType RefineType { get; set; }
        public bool Success { get; set; }
    }


    public sealed class NPCRefineRetrieve : Packet
    {
        public int Index { get; set; }
    }
    public sealed class RefineList : Packet
    {
        public List<ClientRefineInfo> List { get; set; }
    }

    public sealed class GroupSwitch : Packet
    {
        public bool Allow { get; set; }
    }
    public sealed class GroupMember : Packet
    {
        public uint ObjectID { get; set; }
        public string Name { get; set; }
    }
    public sealed class GroupRemove : Packet
    {
        public uint ObjectID { get; set; }
    }
    public sealed class GroupInvite : Packet
    {
        public string Name { get; set; }
    }

    public sealed class BuffAdd : Packet
    {
        public ClientBuffInfo Buff { get; set; }
    }
    public sealed class BuffRemove : Packet
    {
        public int Index { get; set; }
    }
    public sealed class BuffChanged : Packet
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }
    }
    public sealed class BuffTime : Packet
    {
        public int Index { get; set; }
        public TimeSpan Time { get; set; }
    }
    public sealed class BuffPaused : Packet
    {
        public int Index { get; set; }
        public bool Paused { get; set; }
    }
    public sealed class SafeZoneChanged : Packet
    {
        public bool InSafeZone { get; set; }
    }
    public sealed class CombatTime : Packet
    {

    }
    public sealed class Inspect : Packet
    {
        public string Name { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }
        public string Partner { get; set; }
        public MirClass Class { get; set; }
        public int Level { get; set; }
        public MirGender Gender { get; set; }
        public Stats Stats { get; set; }
        public Stats HermitStats { get; set; }
        public int HermitPoints { get; set; }
        public List<ClientUserItem> Items { get; set; }
        public int Hair { get; set; }
        public Color HairColour { get; set; }

        public int WearWeight { get; set; }
        public int HandWeight { get; set; }
    }
    public sealed class Rankings : Packet
    {
        public bool OnlineOnly { get; set; }
        public RequiredClass Class { get; set; }
        public int StartIndex { get; set; }
        public int Total { get; set; }

        public List<RankInfo> Ranks { get; set; }
    }

    public sealed class StartObserver : Packet
    {
        public StartInformation StartInformation { get; set; }
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class ObservableSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    public sealed class MarketPlaceHistory : Packet
    {
        public int Index { get; set; }
        public long SaleCount { get; set; }
        public long LastPrice { get; set; }
        public long AveragePrice { get; set; }
        public int Display { get; set; }
    }

    public sealed class MarketPlaceConsign : Packet
    {
        public List<ClientMarketPlaceInfo> Consignments { get; set; }
    }

    public sealed class MarketPlaceSearch : Packet
    {
        public int Count { get; set; }
        public List<ClientMarketPlaceInfo> Results { get; set; }
    }
    public sealed class MarketPlaceSearchCount : Packet
    {
        public int Count { get; set; }
    }

    public sealed class MarketPlaceSearchIndex : Packet
    {
        public int Index { get; set; }
        public ClientMarketPlaceInfo Result { get; set; }
    }

    public sealed class MarketPlaceBuy : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
        public bool Success { get; set; }
    }
    public sealed class MarketPlaceStoreBuy : Packet
    {
    }

    public sealed class MarketPlaceConsignChanged : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }


    public sealed class MailList : Packet
    {
        public List<ClientMailInfo> Mail { get; set; }
    }
    public sealed class MailNew : Packet
    {
        public ClientMailInfo Mail { get; set; }
    }
    public sealed class MailDelete : Packet
    {
        public int Index { get; set; }
    }
    public sealed class MailItemDelete : Packet
    {
        public int Index { get; set; }
        public int Slot { get; set; }
    }
    public sealed class MailSend : Packet
    {
    }

    public sealed class ChangeAttackMode : Packet
    {
        public AttackMode Mode { get; set; }
    }
    public sealed class ChangePetMode : Packet
    {
        public PetMode Mode { get; set; }
    }

    public sealed class GameGoldChanged : Packet
    {
        public int GameGold { get; set; }
    }

    public sealed class MountFailed : Packet
    {
        public HorseType Horse { get; set; }
    }

    public sealed class WeightUpdate : Packet
    {
        public int BagWeight { get; set; }
        public int WearWeight { get; set; }
        public int HandWeight { get; set; }
    }

    public sealed class HuntGoldChanged : Packet
    {
        public int HuntGold { get; set; }
    }


    public sealed class TradeRequest : Packet
    {
        public string Name { get; set; }
    }
    public sealed class TradeOpen : Packet
    {
        public string Name { get; set; }
    }

    public sealed class TradeClose : Packet { }

    public sealed class TradeAddItem : Packet
    {
        public CellLinkInfo Cell { get; set; }
        public bool Success { get; set; }
    }

    public sealed class TradeAddGold : Packet
    {
        public long Gold { get; set; }
    }

    public sealed class TradeItemAdded : Packet
    {
        public ClientUserItem Item { get; set; }
    }

    public sealed class TradeGoldAdded : Packet
    {
        public long Gold { get; set; }
    }
    public sealed class TradeUnlock : Packet { }


    public sealed class GuildCreate : Packet
    {

    }
    public sealed class GuildInfo : Packet
    {
        public ClientGuildInfo Guild { get; set; }
    }
    public sealed class GuildNoticeChanged : Packet
    {
        public string Notice { get; set; }
    }
    public sealed class GuildNewItem : Packet
    {
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
        //public int Count { get; set; }
    }
    public sealed class GuildGetItem : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public ClientUserItem Item { get; set; }
    }
    public sealed class GuildUpdate : Packet
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

        public List<ClientGuildMemberInfo> Members { get; set; }
    }
    public sealed class GuildKick : Packet
    {
        public int Index { get; set; }
    }
    public sealed class GuildTax : Packet
    {

    }
    public sealed class GuildIncreaseMember : Packet
    {

    }
    public sealed class GuildIncreaseStorage : Packet
    {

    }
    public sealed class GuildInviteMember : Packet
    {

    }
    public sealed class GuildInvite : Packet
    {
        public string Name { get; set; }
        public string GuildName { get; set; }
    }
    public sealed class GuildStats : Packet
    {
        public int Index { get; set; }
        public Stats Stats { get; set; }

    }

    public sealed class GuildMemberOffline : Packet
    {
        public int Index { get; set; }
    }
    public sealed class GuildMemberOnline : Packet
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public uint ObjectID { get; set; }
    }
    public sealed class GuildMemberContribution : Packet
    {
        public int Index { get; set; }

        public long Contribution { get; set; }
    }
    public sealed class GuildDayReset : Packet
    {

    }
    public sealed class GuildFundsChanged : Packet
    {
        public long Change { get; set; }
    }
    public sealed class GuildChanged : Packet
    {
        public uint ObjectID { get; set; }
        public string GuildName { get; set; }
        public string GuildRank { get; set; }
    }

    public sealed class GuildWarFinished : Packet
    {
        public string GuildName { get; set; }
    }

    public sealed class GuildWar : Packet
    {
        public bool Success { get; set; }
    }

    public sealed class GuildWarStarted : Packet
    {
        public string GuildName { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public sealed class GuildConquestDate : Packet
    {
        public int Index { get; set; }
        public TimeSpan WarTime { get; set; }

        public DateTime WarDate;

        [CompleteObject]
        public void Update()
        {
            if (WarTime == TimeSpan.MinValue)
                WarDate = DateTime.MinValue;
            else
                WarDate = Time.Now + WarTime;
        }
    }
    public sealed class GuildCastleInfo : Packet
    {
        public int Index { get; set; }
        public string Owner { get; set; }
    }

    public sealed class GuildConquestStarted : Packet
    {
        public int Index { get; set; }
    }

    public sealed class GuildConquestFinished : Packet
    {
        public int Index { get; set; }
    }

    public sealed class ReviveTimers : Packet
    {
        public TimeSpan ItemReviveTime { get; set; }
        public TimeSpan ReincarnationPillTime { get; set; }
    }

    public sealed class QuestChanged : Packet
    {
        public ClientUserQuest Quest { get; set; }
    }

    public sealed class CompanionUnlock : Packet
    {
        public int Index { get; set; }
    }
    public sealed class CompanionAdopt : Packet
    {
        public ClientUserCompanion UserCompanion { get; set; }
    }
    public sealed class CompanionRetrieve : Packet
    {
        public int Index { get; set; }
    }
    public sealed class CompanionStore : Packet
    {
    }
    public sealed class CompanionWeightUpdate : Packet
    {
        public int BagWeight { get; set; }
        public int MaxBagWeight { get; set; }
        public int InventorySize { get; set; }
    }
    public sealed class CompanionShapeUpdate : Packet
    {
        public uint ObjectID { get; set; }
        public int HeadShape { get; set; }
        public int BackShape { get; set; }
    }
    public sealed class CompanionItemsGained : Packet
    {
        public List<ClientUserItem> Items { get; set; }
    }
    public sealed class CompanionUpdate : Packet
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Hunger { get; set; }
    }
    public sealed class CompanionSkillUpdate : Packet
    {
        public Stats Level3 { get; set; }
        public Stats Level5 { get; set; }
        public Stats Level7 { get; set; }
        public Stats Level10 { get; set; }
        public Stats Level11 { get; set; }
        public Stats Level13 { get; set; }
        public Stats Level15 { get; set; }
    }


    public sealed class MarriageInvite : Packet
    {
        public string Name { get; set; }
    }
    public sealed class MarriageInfo : Packet
    {
        public ClientPlayerInfo Partner { get; set; }
    }
    public sealed class MarriageRemoveRing : Packet
    {

    }
    public sealed class MarriageMakeRing : Packet
    {

    }

    public sealed class MarriageOnlineChanged : Packet
    {
        public uint ObjectID { get; set; }
    }

    public sealed class DataObjectRemove : Packet
    {
        public uint ObjectID { get; set; }
    }
    public sealed class DataObjectPlayer : Packet
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
    public sealed class DataObjectMonster : Packet
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public MonsterInfo MonsterInfo;
        public int MonsterIndex { get; set; }
        public string PetOwner { get; set; }

        public int Health { get; set; }
        public Stats Stats { get; set; }
        public bool Dead { get; set; }

        [CompleteObject]
        public void OnComplete()
        {
            MonsterInfo = Globals.MonsterInfoList.Binding.First(x => x.Index == MonsterIndex);
        }
    }
    public sealed class DataObjectItem : Packet
    {
        public uint ObjectID { get; set; }

        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }

        public ItemInfo ItemInfo;
        public int ItemIndex { get; set; }

        [CompleteObject]
        public void OnComplete()
        {
            ItemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == ItemIndex);
        }
    }
    public sealed class DataObjectLocation : Packet
    {
        public uint ObjectID { get; set; }
        public int MapIndex { get; set; }
        public Point CurrentLocation { get; set; }
    }
    public sealed class DataObjectHealthMana : Packet
    {
        public uint ObjectID { get; set; }

        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Dead { get; set; }
    }
    public sealed class DataObjectMaxHealthMana : Packet
    {
        public uint ObjectID { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }
        public Stats Stats { get; set; }
    }
    public sealed class BlockAdd : Packet
    {
        public ClientBlockInfo Info { get; set; }
    }

    public sealed class BlockRemove : Packet
    {
        public int Index { get; set; }
    }

    public sealed class HelmetToggle : Packet
    {
        public bool HideHelmet { get; set; }
    }

    public sealed class StorageSize : Packet
    {
        public int Size { get; set; }
    }

    public sealed class PlayerChangeUpdate : Packet
    {

        public uint ObjectID { get; set; }
        public string Name { get; set; }
        public MirGender Gender { get; set; }
        public int HairType { get; set; }

        public Color HairColour { get; set; }
        public Color ArmourColour { get; set; }

    }

    public sealed class FortuneUpdate : Packet
    {
        public List<ClientFortuneInfo> Fortunes { get; set; }

    }
    public sealed class NPCWeaponCraft : Packet
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

    public sealed class NPCAccessoryRefine : Packet
    {
        public CellLinkInfo Target { get; set; }
        public CellLinkInfo OreTarget { get; set; }
        public List<CellLinkInfo> Links { get; set; }
        public RefineType RefineType { get; set; }
        public bool Success { get; set; }
    }

    public sealed class ItemAcessoryRefined : Packet
    {
        public GridType GridType { get; set; }
        public int Slot { get; set; }
        public Stats NewStats { get; set; }
    }
}


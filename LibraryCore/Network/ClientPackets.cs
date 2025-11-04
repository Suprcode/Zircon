using System;
using System.Collections.Generic;
using System.Drawing;
using MemoryPack;

namespace Library.Network.ClientPackets
{
    [MemoryPackable]
    public sealed partial class NewAccount : Packet
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string RealName { get; set; }
        public string Referral { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ChangePassword : Packet
    {
        public string EMailAddress { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class RequestPasswordReset : Packet
    {
        public string EMailAddress { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ResetPassword : Packet
    {
        public string ResetKey { get; set; }
        public string NewPassword { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Activation : Packet
    {
        public string ActivationKey { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class RequestActivationKey : Packet
    {
        public string EMailAddress { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class SelectLanguage : Packet
    {
        public string Language { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Login : Packet
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Logout : Packet { }


    [MemoryPackable]
    public sealed partial class NewCharacter : Packet
    {
        public string CharacterName { get; set; }
        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }
        public int HairType { get; set; }
        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
        [MemoryPackAllowSerialize]
        public Color ArmourColour { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class DeleteCharacter : Packet
    {
        public int CharacterIndex { get; set; }
        public string CheckSum { get; set; }
    }

    [MemoryPackable]
    public sealed partial class StartGame : Packet
    {
        public int CharacterIndex { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TownRevive : Packet { }

    [MemoryPackable]
    public sealed partial class Turn : Packet
    {
        public MirDirection Direction { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Harvest : Packet
    {
        public MirDirection Direction { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Move : Packet
    {
        public MirDirection Direction { get; set; }
        public int Distance { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Mount : Packet { }

    [MemoryPackable]
    public sealed partial class FishingCast : Packet
    {
        public FishingState State { get; set; }
        public MirDirection Direction { get; set; }
        public Point FloatLocation { get; set; }
        public bool CaughtFish { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Attack : Packet
    {
        public MirDirection Direction { get; set; }
        public MirAction Action { get; set; }
        public MagicType AttackMagic { get; set; }
    }

    [MemoryPackable]
    public sealed partial class RangeAttack : Packet
    {
        public MirDirection Direction { get; set; }
        public uint Target { get; set; }
        public int DelayedTime { get; set; }

    }
    [MemoryPackable]
    public sealed partial class Mining : Packet
    {
        public MirDirection Direction { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Magic : Packet
    {
        public MirDirection Direction { get; set; }
        public MirAction Action { get; set; }
        public MagicType Type { get; set; }
        public uint Target { get; set; }
        public Point Location { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemMove : Packet
    {
        public GridType FromGrid { get; set; }
        public GridType ToGrid { get; set; }
        public int FromSlot { get; set; }
        public int ToSlot { get; set; }
        public bool MergeItem { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemSort : Packet
    {
        public GridType Grid { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemDelete : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemSplit : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemDrop : Packet
    {
        public CellLinkInfo Link { get; set; }
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CurrencyDrop : Packet
    {
        public int CurrencyIndex { get; set; }
        public long Amount { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemUse : Packet
    {
        public CellLinkInfo Link { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ItemLock : Packet
    {
        public GridType GridType { get; set; }
        public int SlotIndex { get; set; }
        public bool Locked { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BeltLinkChanged : Packet
    {
        public int Slot { get; set; }
        public int LinkIndex { get; set; }
        public int LinkItemIndex { get; set; }
    }

    [MemoryPackable]
    public sealed partial class AutoPotionLinkChanged : Packet
    {
        public int Slot { get; set; }
        public int LinkIndex { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Enabled { get; set; }
    }

    [MemoryPackable]
    public sealed partial class PickUp : Packet { }

    [MemoryPackable]
    public sealed partial class Chat : Packet
    {
        public string Text { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCCall : Packet
    {
        public uint ObjectID { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCButton : Packet
    {
        public int ButtonID { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCRoll : Packet
    {
        public int Type { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCRollResult : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class NPCBuy : Packet
    {
        public int Index { get; set; }
        public long Amount { get; set; }
        public bool GuildFunds { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCSell : Packet
    {
        public List<CellLinkInfo> Links { get; set; }

    }

    [MemoryPackable]
    public sealed partial class NPCFragment : Packet
    {
        public List<CellLinkInfo> Links { get; set; }

    }

    [MemoryPackable]
    public sealed partial class NPCRepair : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool GuildFunds { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public RefineQuality RefineQuality { get; set; }
        public List<CellLinkInfo> Ores { get; set; }
        public List<CellLinkInfo> Items { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCMasterRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCMasterRefineEvaluate : Packet
    {
        public RefineType RefineType { get; set; }
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCRefinementStone : Packet
    {
        public List<CellLinkInfo> IronOres { get; set; }
        public List<CellLinkInfo> SilverOres { get; set; }
        public List<CellLinkInfo> DiamondOres { get; set; }
        public List<CellLinkInfo> GoldOres { get; set; }
        public List<CellLinkInfo> Crystal { get; set; }
        public long Gold { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NPCClose : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class NPCRefineRetrieve : Packet
    {
        public int Index { get; set; }
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
    }

    [MemoryPackable]
    public sealed partial class MagicKey : Packet
    {
        public MagicType Magic { get; set; }

        public SpellKey Set1Key { get; set; }
        public SpellKey Set2Key { get; set; }
        public SpellKey Set3Key { get; set; }
        public SpellKey Set4Key { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MagicToggle : Packet
    {
        public MagicType Magic { get; set; }
        public bool CanUse { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GroupSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GroupInvite : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GroupRemove : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GroupResponse : Packet
    {
        public bool Accept { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Inspect : Packet
    {
        public int Index { get; set; }
        public bool Ranking { get; set; }
    }

    [MemoryPackable]
    public sealed partial class RankRequest : Packet
    {
        public RequiredClass Class { get; set; }
        public bool OnlineOnly { get; set; }
        public int StartIndex { get; set; }
    }

    [MemoryPackable]
    public sealed partial class RankSearch : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObserverRequest : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ObservableSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    [MemoryPackable]
    public sealed partial class Hermit : Packet
    {
        public Stat Stat { get; set; }
    }


    [MemoryPackable]
    public sealed partial class MarketPlaceHistory : Packet
    {
        public int Index { get; set; }
        public int Display { get; set; }
        public int PartIndex { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceConsign : Packet
    {
        public CellLinkInfo Link { get; set; }

        public int Price { get; set; }

        public string Message { get; set; }
        public bool GuildFunds { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceSearch : Packet
    {
        public string Name { get; set; }

        public bool ItemTypeFilter { get; set; }
        public ItemType ItemType { get; set; }

        public MarketPlaceSort Sort { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceSearchIndex : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceCancelConsign : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceBuy : Packet
    {
        public long Index { get; set; }
        public long Count { get; set; }
        public bool GuildFunds { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarketPlaceStoreBuy : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
        public bool UseHuntGold { get; set; }
    }


    [MemoryPackable]
    public sealed partial class MailOpened : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailGetItem : Packet
    {
        public int Index { get; set; }
        public int Slot { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailDelete : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MailSend : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public long Gold { get; set; }
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
    public sealed partial class GameGoldRecharge : Packet
    { }

    [MemoryPackable]
    public sealed partial class TradeRequest : Packet
    {
    }
    [MemoryPackable]
    public sealed partial class TradeRequestResponse : Packet
    {
        public bool Accept { get; set; }
    }
    [MemoryPackable]
    public sealed partial class TradeClose : Packet
    {

    }
    [MemoryPackable]
    public sealed partial class TradeAddGold : Packet
    {
        public long Gold { get; set; }
    }
    [MemoryPackable]
    public sealed partial class TradeAddItem : Packet
    {
        public CellLinkInfo Cell { get; set; }
    }
    [MemoryPackable]
    public sealed partial class TradeConfirm : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class GuildCreate : Packet
    {
        public string Name { get; set; }
        public bool UseGold { get; set; }
        public int Members { get; set; }
        public int Storage { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildEditNotice : Packet
    {
        public string Notice { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildEditMember : Packet
    {
        public int Index { get; set; }
        public string Rank { get; set; }
        public GuildPermission Permission { get; set; }

    }
    [MemoryPackable]
    public sealed partial class GuildInviteMember : Packet
    {
        public string Name { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildKickMember : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class GuildTax : Packet
    {
        public long Tax { get; set; }
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
    public sealed partial class GuildResponse : Packet
    {
        public bool Accept { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildWar : Packet
    {
        public string GuildName { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildRequestConquest : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildColour : Packet
    {
        [MemoryPackAllowSerialize]
        public Color Colour { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildFlag : Packet
    {
        public int Flag { get; set; }
    }

    [MemoryPackable]
    public sealed partial class GuildToggleCastleGates : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class GuildRepairCastleGates : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class GuildRepairCastleGuards : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class QuestAccept : Packet
    {
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class QuestComplete : Packet
    {
        public int Index { get; set; }

        public int ChoiceIndex { get; set; }
    }
    [MemoryPackable]
    public sealed partial class QuestTrack : Packet
    {
        public int Index { get; set; }

        public bool Track { get; set; }
    }
    [MemoryPackable]
    public sealed partial class QuestAbandon : Packet
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
        public int Index { get; set; }
        public string Name { get; set; }
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
        public int Index { get; set; }
    }
    [MemoryPackable]
    public sealed partial class MarriageResponse : Packet
    {
        public bool Accept { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarriageMakeRing : Packet
    {
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class MarriageTeleport : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class BlockAdd : Packet
    {
        public string Name { get; set; }
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
    public sealed partial class GenderChange : Packet
    {
        public MirGender Gender { get; set; }
        public int HairType { get; set; }
        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
    }

    [MemoryPackable]
    public sealed partial class HairChange : Packet
    {
        public int HairType { get; set; }
        [MemoryPackAllowSerialize]
        public Color HairColour { get; set; }
    }

    [MemoryPackable]
    public sealed partial class ArmourDye : Packet
    {
        [MemoryPackAllowSerialize]
        public Color ArmourColour { get; set; }
    }

    [MemoryPackable]
    public sealed partial class NameChange : Packet
    {
        public string Name { get; set; }
    }

    [MemoryPackable]
    public sealed partial class CaptionChange : Packet
    {
        public string Caption { get; set; }
    }

    [MemoryPackable]
    public sealed partial class FortuneCheck : Packet
    {
        public int ItemIndex { get; set; }
    }

    [MemoryPackable]
    public sealed partial class TeleportRing : Packet
    {
        public Point Location { get; set; }
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class JoinStarterGuild : Packet
    {

    }

    [MemoryPackable]
    public sealed partial class NPCAccessoryReset : Packet
    {
        public CellLinkInfo Cell { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCWeaponCraft : Packet
    {
        public RequiredClass Class { get; set; }
        public CellLinkInfo Template { get; set; }
        public CellLinkInfo Yellow { get; set; }
        public CellLinkInfo Blue { get; set; }
        public CellLinkInfo Red { get; set; }
        public CellLinkInfo Purple { get; set; }
        public CellLinkInfo Green { get; set; }
        public CellLinkInfo Grey { get; set; }
    }
    [MemoryPackable]
    public sealed partial class NPCAccessoryRefine : Packet
    {
        public CellLinkInfo Target { get; set; }
        public CellLinkInfo OreTarget { get; set; }
        public List<CellLinkInfo> Links { get; set; }
        public RefineType RefineType { get; set; }
    }
    [MemoryPackable]
    public sealed partial class JoinInstance : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class SendCompanionFilters : Packet
    {
        public List<MirClass> FilterClass { get; set; }
        public List<Rarity> FilterRarity { get; set; }
        public List<ItemType> FilterItemType { get; set; }
    }
    [MemoryPackable]
    public sealed partial class ChangeOnlineState : Packet
    {
        public OnlineState State { get; set; }
    }

    [MemoryPackable]
    public sealed partial class FriendAdd : Packet
    {
        public string Name { get; set; }
    }
    [MemoryPackable]
    public sealed partial class FriendRemove : Packet
    {
        public int Index { get; set; }
    }

    [MemoryPackable]
    public sealed partial class IncreaseDiscipline : Packet
    {
    }

    [MemoryPackable]
    public sealed partial class LootBoxOpen : Packet
    {
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxReroll : Packet
    {
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxConfirmSelection : Packet
    {
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxReveal : Packet
    {
        public int Slot { get; set; }
        public int Choice { get; set; }
    }

    [MemoryPackable]
    public sealed partial class LootBoxTakeItems : Packet
    {
        public int Slot { get; set; }
        public int Choice { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BundleOpen : Packet
    {
        public int Slot { get; set; }
    }

    [MemoryPackable]
    public sealed partial class BundleConfirm : Packet
    {
        public int Slot { get; set; }
        public int Choice { get; set; }
    }
}

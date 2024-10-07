using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Network.ClientPackets
{
    public sealed class NewAccount : Packet
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string RealName { get; set; }
        public string Referral { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class ChangePassword : Packet
    {
        public string EMailAddress { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class RequestPasswordReset : Packet
    {
        public string EMailAddress { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class ResetPassword : Packet
    {
        public string ResetKey { get; set; }
        public string NewPassword { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class Activation : Packet
    {
        public string ActivationKey { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class RequestActivationKey : Packet
    {
        public string EMailAddress { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class SelectLanguage : Packet
    {
        public string Language { get; set; }
    }

    public sealed class Login : Packet
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class Logout : Packet {}


    public sealed class NewCharacter : Packet
    {
        public string CharacterName { get; set; }
        public MirClass Class { get; set; }
        public MirGender Gender { get; set; }
        public int HairType { get; set; }
        public Color HairColour { get; set; }
        public Color ArmourColour { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class DeleteCharacter : Packet
    {
        public int CharacterIndex { get; set; }
        public string CheckSum { get; set; }
    }

    public sealed class StartGame : Packet
    {
        public int CharacterIndex { get; set; }
    }

    public sealed class TownRevive : Packet {}

    public sealed class Turn : Packet
    {
        public MirDirection Direction { get; set; }
    }

    public sealed class Harvest : Packet
    {
        public MirDirection Direction { get; set; }
    }

    public sealed class Move : Packet
    {
        public MirDirection Direction { get; set; }
        public int Distance { get; set; }
    }

    public sealed class Mount : Packet {}

    public sealed class FishingCast : Packet 
    { 
        public FishingState State { get; set; }
        public MirDirection Direction { get; set; }
        public Point FloatLocation { get; set; }
        public bool CaughtFish { get; set; }
    }

    public sealed class Attack : Packet
    {
        public MirDirection Direction { get; set; }
        public MirAction Action { get; set; }
        public MagicType AttackMagic { get; set; }
    }

    public sealed class RangeAttack : Packet
    {
        public MirDirection Direction { get; set; }
        public uint Target { get; set; }
        public int DelayedTime { get; set; }

    }
    public sealed class Mining : Packet
    {
        public MirDirection Direction { get; set; }
    }
    
    public sealed class Magic : Packet
    {
        public MirDirection Direction { get; set; }
        public MirAction Action { get; set; }
        public MagicType Type { get; set; }
        public uint Target { get; set; }
        public Point Location { get; set; }
    }

    public sealed class ItemMove : Packet
    {
        public GridType FromGrid { get; set; }
        public GridType ToGrid { get; set; }
        public int FromSlot { get; set; }
        public int ToSlot { get; set; }
        public bool MergeItem { get; set; }
    }

    public sealed class ItemSort : Packet
    {
        public GridType Grid { get; set; }
    }

    public sealed class ItemDelete : Packet
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
    }

    public sealed class ItemSplit : Packet 
    {
        public GridType Grid { get; set; }
        public int Slot { get; set; }
        public long Count { get; set; }
    }

    public sealed class ItemDrop : Packet
    {
        public CellLinkInfo Link { get; set; }
        public int Slot { get; set; }
    }

    public sealed class CurrencyDrop : Packet
    {
        public int CurrencyIndex { get; set; }
        public long Amount { get; set; }
    }

    public sealed class ItemUse : Packet
    {
        public CellLinkInfo Link { get; set; }
    }

    public sealed class ItemLock : Packet
    {
        public GridType GridType { get; set; }
        public int SlotIndex { get; set; }
        public bool Locked { get; set; }
    }

    public sealed class BeltLinkChanged : Packet
    {
        public int Slot { get; set; }
        public int LinkIndex { get; set; }
        public int LinkItemIndex { get; set; }
    }

    public sealed class AutoPotionLinkChanged : Packet
    {
        public int Slot { get; set; }
        public int LinkIndex { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public bool Enabled { get; set; }
    }

    public sealed class PickUp : Packet {}

    public sealed class Chat : Packet
    {
        public string Text { get; set; }
    }

    public sealed class NPCCall : Packet
    {
        public uint ObjectID { get; set; }
    }

    public sealed class NPCButton : Packet
    {
        public int ButtonID { get; set; }
    }

    public sealed class NPCRoll : Packet
    {
        public int Type { get; set; }
    }

    public sealed class NPCRollResult : Packet
    {
    }

    public sealed class NPCBuy : Packet
    {
        public int Index { get; set; }
        public long Amount { get; set; }
        public bool GuildFunds { get; set; }
    }

    public sealed class NPCSell : Packet
    {
        public List<CellLinkInfo> Links { get; set; }

    }

    public sealed class NPCFragment : Packet
    {
        public List<CellLinkInfo> Links { get; set; }

    }

    public sealed class NPCRepair : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public bool Special { get; set; }
        public bool GuildFunds { get; set; }
    }

    public sealed class NPCRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public RefineQuality RefineQuality { get; set; }
        public List<CellLinkInfo> Ores { get; set; }
        public List<CellLinkInfo> Items { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    public sealed class NPCMasterRefine : Packet
    {
        public RefineType RefineType { get; set; }
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    public sealed class NPCMasterRefineEvaluate : Packet
    {
        public RefineType RefineType { get; set; }
        public List<CellLinkInfo> Fragment1s { get; set; }
        public List<CellLinkInfo> Fragment2s { get; set; }
        public List<CellLinkInfo> Fragment3s { get; set; }
        public List<CellLinkInfo> Stones { get; set; }
        public List<CellLinkInfo> Specials { get; set; }
    }
    public sealed class NPCRefinementStone : Packet
    {
        public List<CellLinkInfo> IronOres { get; set; }
        public List<CellLinkInfo> SilverOres { get; set; }
        public List<CellLinkInfo> DiamondOres { get; set; }
        public List<CellLinkInfo> GoldOres { get; set; }
        public List<CellLinkInfo> Crystal { get; set; }
        public long Gold { get; set; }
    }

    public sealed class NPCClose : Packet
    {
    }

    public sealed class NPCRefineRetrieve : Packet
    {
        public int Index { get; set; }
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
    }

    public sealed class MagicKey : Packet
    {
        public MagicType Magic { get; set; }

        public SpellKey Set1Key { get; set; }
        public SpellKey Set2Key { get; set; }
        public SpellKey Set3Key { get; set; }
        public SpellKey Set4Key { get; set; }
    }

    public sealed class MagicToggle : Packet
    {
        public MagicType Magic { get; set; }
        public bool CanUse { get; set; }
    }

    public sealed class GroupSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    public sealed class GroupInvite : Packet
    {
        public string Name { get; set; }
    }

    public sealed class GroupRemove : Packet
    {
        public string Name { get; set; }
    }

    public sealed class GroupResponse : Packet
    {
        public bool Accept { get; set; }
    }

    public sealed class Inspect : Packet
    {
        public int Index { get; set; }
        public bool Ranking { get; set; }
    }

    public sealed class RankRequest : Packet
    {
        public RequiredClass Class { get; set; }
        public bool OnlineOnly { get; set; }
        public int StartIndex { get; set; }
    }

    public sealed class RankSearch : Packet
    {
        public string Name { get; set; }
    }

    public sealed class ObserverRequest : Packet
    {
        public string Name { get; set; }
    }

    public sealed class ObservableSwitch : Packet
    {
        public bool Allow { get; set; }
    }

    public sealed class Hermit : Packet
    {
        public Stat Stat { get; set; }
    }


    public sealed class MarketPlaceHistory : Packet
    {
        public int Index { get; set; }
        public int Display { get; set; }
        public int PartIndex { get; set; }
    }
    public sealed class MarketPlaceConsign : Packet
    {
        public CellLinkInfo Link { get; set; }

        public int Price { get; set; }

        public string Message { get; set; }
        public bool GuildFunds { get; set; }
    }
    public sealed class MarketPlaceSearch : Packet
    {
        public string Name { get; set; }

        public bool ItemTypeFilter { get; set; }
        public ItemType ItemType { get; set; }

        public MarketPlaceSort Sort { get; set; }
    }
    public sealed class MarketPlaceSearchIndex : Packet
    {
        public int Index { get; set; }
    }
    public sealed class MarketPlaceCancelConsign : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
    }
    public sealed class MarketPlaceBuy : Packet
    {
        public long Index { get; set; }
        public long Count { get; set; }
        public bool GuildFunds { get; set; }
    }
    public sealed class MarketPlaceStoreBuy : Packet
    {
        public int Index { get; set; }
        public long Count { get; set; }
        public bool UseHuntGold { get; set; }
    }


    public sealed class MailOpened : Packet
    {
        public int Index { get; set; }
    }
    public sealed class MailGetItem : Packet
    {
        public int Index { get; set; }
        public int Slot { get; set; }
    }
    public sealed class MailDelete : Packet
    {
        public int Index { get; set; }
    }
    public sealed class MailSend : Packet
    {
        public List<CellLinkInfo> Links { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public long Gold { get; set; }
    }


    public sealed class ChangeAttackMode : Packet
    {
        public AttackMode Mode { get; set; }
    }
    public sealed class ChangePetMode : Packet
    {
        public PetMode Mode { get; set; }
    }

    public sealed class GameGoldRecharge : Packet
    { }

    public sealed class TradeRequest : Packet
    {
    }
    public sealed class TradeRequestResponse : Packet
    {
        public bool Accept { get; set; }
    }
    public sealed class TradeClose : Packet
    {

    }
    public sealed class TradeAddGold : Packet
    {
        public long Gold { get; set; }
    }
    public sealed class TradeAddItem : Packet
    {
        public CellLinkInfo Cell { get; set; }
    }
    public sealed class TradeConfirm : Packet
    {

    }

    public sealed class GuildCreate : Packet
    {
        public string Name { get; set; }
        public bool UseGold { get; set; }
        public int Members { get; set; }
        public int Storage { get; set; }
    }
    public sealed class GuildEditNotice : Packet
    {
        public string Notice { get; set; }
    }
    public sealed class GuildEditMember : Packet
    {
        public int Index { get; set; }
        public string Rank { get; set; }
        public GuildPermission Permission { get; set; }

    }
    public sealed class GuildInviteMember : Packet
    {
        public string Name { get; set; }
    }
    public sealed class GuildKickMember : Packet
    {
        public int Index { get; set; }
    }
    public sealed class GuildTax : Packet
    {
        public long Tax { get; set; }
    }
    public sealed class GuildIncreaseMember : Packet
    {

    }
    public sealed class GuildIncreaseStorage : Packet
    {

    }
    public sealed class GuildResponse : Packet
    {
        public bool Accept { get; set; }
    }

    public sealed class GuildWar : Packet
    {
        public string GuildName { get; set; }
    }

    public sealed class GuildRequestConquest : Packet
    {
        public int Index { get; set; }
    }

    public sealed class GuildColour : Packet
    {
        public Color Colour { get; set; }
    }

    public sealed class GuildFlag : Packet
    {
        public int Flag { get; set; }
    }

    public sealed class GuildToggleCastleGates : Packet
    {

    }

    public sealed class GuildRepairCastleGates : Packet
    {

    }

    public sealed class GuildRepairCastleGuards : Packet
    {

    }

    public sealed class QuestAccept : Packet
    {
        public int Index { get; set; }
    }
    public sealed class QuestComplete : Packet
    {
        public int Index { get; set; }

        public int ChoiceIndex { get; set; }
    }
    public sealed class QuestTrack : Packet
    {
        public int Index { get; set; }

        public bool Track { get; set; }
    }
    public sealed class QuestAbandon : Packet
    {
        public int Index { get; set; }
    }

    public sealed class CompanionUnlock : Packet
    {
        public int Index { get; set; }
    }
    public sealed class CompanionAdopt : Packet
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public sealed class CompanionRetrieve : Packet
    {
        public int Index { get; set; }
    }
    public sealed class CompanionStore : Packet
    {
        public int Index { get; set; }
    }
    public sealed class MarriageResponse : Packet
    {
        public bool Accept { get; set; }
    }

    public sealed class MarriageMakeRing : Packet
    {
        public int Slot { get; set; }
    }

    public sealed class MarriageTeleport : Packet
    {
        
    }

    public sealed class BlockAdd : Packet
    {
        public string Name { get; set; }
    }
    public sealed class BlockRemove : Packet
    {
        public int Index { get; set; }
    }

    public sealed class HelmetToggle : Packet
    {
        public bool HideHelmet { get; set; }
    }

    public sealed class GenderChange : Packet
    {
        public MirGender Gender { get; set; }
        public int HairType { get; set; }
        public Color HairColour { get; set; }
    }

    public sealed class HairChange : Packet
    {
        public int HairType { get; set; }
        public Color HairColour { get; set; }
    }

    public sealed class ArmourDye : Packet
    {
        public Color ArmourColour { get; set; }
    }

    public sealed class NameChange : Packet
    {
        public string Name { get; set; }
    }

    public sealed class CaptionChange : Packet
    {
        public string Caption { get; set; }
    }

    public sealed class FortuneCheck : Packet
    {
        public int ItemIndex { get; set; }
    }

    public sealed class TeleportRing : Packet
    {
        public Point Location { get; set; }
        public int Index { get; set; }
    }

    public sealed class JoinStarterGuild : Packet
    {
        
    }

    public sealed class NPCAccessoryReset : Packet
    {
        public CellLinkInfo Cell { get; set; }
    }
    public sealed class NPCWeaponCraft : Packet
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
    public sealed class NPCAccessoryRefine : Packet
    {
        public CellLinkInfo Target { get; set; }
        public CellLinkInfo OreTarget { get; set; }
        public List<CellLinkInfo> Links { get; set; }
        public RefineType RefineType { get; set; }
    }
    public sealed class JoinInstance : Packet
    {
        public int Index { get; set; }
    }

    public sealed class SendCompanionFilters : Packet
    {
        public List<MirClass> FilterClass { get; set; }
        public List<Rarity> FilterRarity { get; set; }
        public List<ItemType> FilterItemType { get; set; }
    }
    public sealed class ChangeOnlineState : Packet
    {
        public OnlineState State { get; set; }
    }

    public sealed class FriendAdd : Packet
    {
        public string Name { get; set; }
    }
    public sealed class FriendRemove : Packet
    {
        public int Index { get; set; }
    }

    public sealed class IncreaseDiscipline : Packet
    {
    }
}

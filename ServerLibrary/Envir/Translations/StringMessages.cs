using Library;

namespace Server.Envir
{
    public abstract class StringMessages
    {
        [ConfigSection("Account")]
        public abstract string BannedWrongPassword { get; set; }

        [ConfigSection("Payment")]
        public abstract string PaymentComplete { get; set; }
        public abstract string PaymentFailed { get; set; }
        public abstract string ReferralPaymentComplete { get; set; }
        public abstract string ReferralPaymentFailed { get; set; }
        public abstract string GameGoldLost { get; set; }
        public abstract string GameGoldRefund { get; set; }
        public abstract string HuntGoldRefund { get; set; }

        [ConfigSection("System")]
        public abstract string Welcome { get; set; }
        public abstract string WelcomeObserver { get; set; }
        public abstract string ObserverChangeFail { get; set; }
        public abstract string OnlineCount { get; set; }
        public abstract string ObserverCount { get; set; }
        public abstract string CannotFindPlayer { get; set; }
        public abstract string AlreadyBlocked { get; set; }
        public abstract string BlockingWhisper { get; set; }
        public abstract string PlayerBlockingWhisper { get; set; }
        public abstract string GlobalDelay { get; set; }
        public abstract string GlobalLevel { get; set; }
        public abstract string ShoutDelay { get; set; }
        public abstract string ShoutLevel { get; set; }
        public abstract string DiceRoll { get; set; }
        public abstract string TradingEnabled { get; set; }
        public abstract string TradingDisabled { get; set; }
        public abstract string WhisperEnabled { get; set; }
        public abstract string WhisperDisabled { get; set; }
        public abstract string GuildInviteEnabled { get; set; }
        public abstract string GuildInviteDisabled { get; set; }
        public abstract string ObserverNotLoggedIn { get; set; }
        public abstract string Poisoned { get; set; }
        public abstract string MurderedBy { get; set; }
        public abstract string Curse { get; set; }
        public abstract string Murdered { get; set; }
        public abstract string Protected { get; set; }
        public abstract string Killed { get; set; }
        public abstract string Died { get; set; }
        public abstract string GroupRecallEnabled { get; set; }
        public abstract string GroupRecallDisabled { get; set; }
        public abstract string AlreadyFriended { get; set; }
        public abstract string FriendStateChanged { get; set; }

        [ConfigSection("Movement")]
        public abstract string NeedLevel { get; set; }
        public abstract string NeedMaxLevel { get; set; }
        public abstract string NeedItem { get; set; }
        public abstract string NeedMonster { get; set; }
        public abstract string NeedClass { get; set; }

        [ConfigSection("Conquest")]
        public abstract string ConquestStarted { get; set; }
        public abstract string ConquestFinished { get; set; }
        public abstract string ConquestCapture { get; set; }
        public abstract string ConquestOwner { get; set; }
        public abstract string ConquestLost { get; set; }
        public abstract string ConquestTakingFlag { get; set; }
        public abstract string ConquestPreventingFlag { get; set; }
        public abstract string ConquestNotTakingFlag { get; set; }

        [ConfigSection("Monster")]
        public abstract string BossSpawn { get; set; }
        public abstract string HarvestRare { get; set; }
        public abstract string NetherGateOpen { get; set; }
        public abstract string NetherGateClosed { get; set; }
        public abstract string HarvestNothing { get; set; }
        public abstract string HarvestCarry { get; set; }
        public abstract string HarvestOwner { get; set; }
        public abstract string LairGateOpen { get; set; }
        public abstract string LairGateClosed { get; set; }

        [ConfigSection("Items")]
        public abstract string Expired { get; set; }
        public abstract string CannotTownTeleport { get; set; }
        public abstract string CannotRandomTeleport { get; set; }
        public abstract string ConnotResetCompanionSkill { get; set; }
        public abstract string MagicMaxLevelReached { get; set; }
        public abstract string LearnBookFailed { get; set; }
        public abstract string LearnBookSuccess { get; set; }
        public abstract string LearnBook4Failed { get; set; }
        public abstract string LearnBook4Success { get; set; }
        public abstract string StorageSafeZone { get; set; }
        public abstract string GuildStoragePermission { get; set; }
        public abstract string GuildStorageSafeZone { get; set; }
        public abstract string CompanionNoRoom { get; set; }
        public abstract string StorageLimit { get; set; }

        [ConfigSection("Marriage")]
        public abstract string MarryAlreadyMarried { get; set; }
        public abstract string MarryNeedLevel { get; set; }
        public abstract string MarryNeedGold { get; set; }
        public abstract string MarryNotFacing { get; set; }
        public abstract string MarryTargetAlreadyMarried { get; set; }
        public abstract string MarryTargetHasProposal { get; set; }
        public abstract string MarryTargetNeedLevel { get; set; }
        public abstract string MarryTargetNeedGold { get; set; }
        public abstract string MarryDead { get; set; }
        public abstract string MarryComplete { get; set; }
        public abstract string MarryDivorce { get; set; }
        public abstract string MarryDivorced { get; set; }
        public abstract string MarryTeleportDead { get; set; }
        public abstract string MarryTeleportPK { get; set; }
        public abstract string MarryTeleportDelay { get; set; }
        public abstract string MarryTeleportOffline { get; set; }
        public abstract string MarryTeleportPartnerDead { get; set; }
        public abstract string MarryTeleportMap { get; set; }
        public abstract string MarryTeleportMapEscape { get; set; }

        [ConfigSection("Companion")]
        public abstract string CompanionAppearanceAlready { get; set; }
        public abstract string CompanionNeedTicket { get; set; }
        public abstract string CompanionSkillEnabled { get; set; }
        public abstract string CompanionSkillDisabled { get; set; }
        public abstract string CompanionAppearanceLocked { get; set; }
        public abstract string CompanionNeedGold { get; set; }
        public abstract string CompanionBadName { get; set; }
        public abstract string CompanionRetrieveFailed { get; set; }

        [ConfigSection("Quest")]
        public abstract string QuestSelectReward { get; set; }
        public abstract string QuestNeedSpace { get; set; }

        [ConfigSection("Mail")]
        public abstract string MailSafeZone { get; set; }
        public abstract string MailNeedSpace { get; set; }
        public abstract string MailHasItems { get; set; }
        public abstract string MailNotFound { get; set; }
        public abstract string MailSelfMail { get; set; }
        public abstract string MailStorageFull { get; set; }
        public abstract string MailMailCost { get; set; }
        public abstract string MailSendSafeZone { get; set; }

        [ConfigSection("Market Place")]
        public abstract string ConsignSafeZone { get; set; }
        public abstract string ConsignLimit { get; set; }
        public abstract string ConsignGuildFundsGuild { get; set; }
        public abstract string ConsignGuildFundsPermission { get; set; }
        public abstract string ConsignGuildFundsCost { get; set; }
        public abstract string ConsignGuildFundsUsed { get; set; }
        public abstract string ConsignCost { get; set; }
        public abstract string ConsignComplete { get; set; }
        public abstract string ConsignAlreadySold { get; set; }
        public abstract string ConsignNotEnough { get; set; }
        public abstract string ConsignBuyOwnItem { get; set; }
        public abstract string ConsignBuyGuildFundsGuild { get; set; }
        public abstract string ConsignBuyGuildFundsPermission { get; set; }
        public abstract string ConsignBuyGuildFundsCost { get; set; }
        public abstract string ConsignBuyGuildFundsUsed { get; set; }
        public abstract string ConsignBuyCost { get; set; }
        public abstract string StoreNotAvailable { get; set; }
        public abstract string StoreNeedSpace { get; set; }
        public abstract string StoreCost { get; set; }

        [ConfigSection("Guild")]
        public abstract string GuildNeedHorn { get; set; }
        public abstract string GuildNeedGold { get; set; }
        public abstract string GuildBadName { get; set; }
        public abstract string GuildNameTaken { get; set; }
        public abstract string GuildNoticePermission { get; set; }
        public abstract string GuildEditMemberPermission { get; set; }
        public abstract string GuildMemberLength { get; set; }
        public abstract string GuildMemberNotFound { get; set; }
        public abstract string GuildKickPermission { get; set; }
        public abstract string GuildKickSelf { get; set; }
        public abstract string GuildMemberKicked { get; set; }
        public abstract string GuildKicked { get; set; }
        public abstract string GuildManagePermission { get; set; }
        public abstract string GuildCastleRepairPermission { get; set; }
        public abstract string GuildMemberLimit { get; set; }
        public abstract string GuildMemberCost { get; set; }
        public abstract string GuildStorageLimit { get; set; }
        public abstract string GuildStorageCost { get; set; }
        public abstract string GuildInvitePermission { get; set; }
        public abstract string GuildInviteGuild { get; set; }
        public abstract string GuildInviteInvited { get; set; }
        public abstract string GuildInviteNotAllowed { get; set; }
        public abstract string GuildInvitedNotAllowed { get; set; }
        public abstract string GuildInviteRoom { get; set; }
        public abstract string GuildNoGuild { get; set; }
        public abstract string GuildWarPermission { get; set; }
        public abstract string GuildNotFoundGuild { get; set; }
        public abstract string GuildWarOwnGuild { get; set; }
        public abstract string GuildAlreadyWar { get; set; }
        public abstract string GuildWarCost { get; set; }
        public abstract string GuildWarFunds { get; set; }
        public abstract string GuildConquestCastle { get; set; }
        public abstract string GuildConquestExists { get; set; }
        public abstract string GuildConquestBadCastle { get; set; }
        public abstract string GuildConquestProgress { get; set; }
        public abstract string GuildConquestNeedItem { get; set; }
        public abstract string GuildConquestSuccess { get; set; }
        public abstract string GuildConquestDate { get; set; }
        public abstract string GuildJoinGuild { get; set; }
        public abstract string GuildJoinTime { get; set; }
        public abstract string GuildJoinNoGuild { get; set; }
        public abstract string GuildJoinPermission { get; set; }
        public abstract string GuildJoinNoRoom { get; set; }
        public abstract string GuildJoinWelcome { get; set; }
        public abstract string GuildMemberJoined { get; set; }
        public abstract string GuildLeaveFailed { get; set; }
        public abstract string GuildLeave { get; set; }
        public abstract string GuildMemberLeave { get; set; }
        public abstract string GuildWarDeath { get; set; }
        public abstract string GuildRepairCastleGatesCost { get; set; }
        public abstract string GuildRepairCastleGuardsCost { get; set; }

        [ConfigSection("Group")]
        public abstract string GroupNoGroup { get; set; }
        public abstract string GroupNotLeader { get; set; }
        public abstract string GroupMemberNotFound { get; set; }
        public abstract string GroupAlreadyGrouped { get; set; }
        public abstract string GroupAlreadyInvited { get; set; }
        public abstract string GroupInviteNotAllowed { get; set; }
        public abstract string GroupSelf { get; set; }
        public abstract string GroupMemberLimit { get; set; }
        public abstract string GroupRecallDelay { get; set; }
        public abstract string GroupRecallMap { get; set; }
        public abstract string GroupRecallNotAllowed { get; set; }
        public abstract string GroupRecallMemberNotAllowed { get; set; }
        public abstract string GroupRecallFromMap { get; set; }
        public abstract string GroupRecallMemberFromMap { get; set; }

        [ConfigSection("Trade")]
        public abstract string TradeAlreadyTrading { get; set; }
        public abstract string TradeAlreadyHaveRequest { get; set; }
        public abstract string TradeNeedFace { get; set; }
        public abstract string TradeTargetNotAllowed { get; set; }
        public abstract string TradeTargetAlreadyTrading { get; set; }
        public abstract string TradeTargetAlreadyHaveRequest { get; set; }
        public abstract string TradeNotAllowed { get; set; }
        public abstract string TradeTargetDead { get; set; }
        public abstract string TradeRequested { get; set; }
        public abstract string TradeWaiting { get; set; }
        public abstract string TradePartnerWaiting { get; set; }
        public abstract string TradeNoGold { get; set; }
        public abstract string TradePartnerNoGold { get; set; }
        public abstract string TradeTooMuchGold { get; set; }
        public abstract string TradePartnerTooMuchGold { get; set; }
        public abstract string TradeFailedItemsChanged { get; set; }
        public abstract string TradeFailedPartnerItemsChanged { get; set; }
        public abstract string TradeNotEnoughSpace { get; set; }
        public abstract string TradeComplete { get; set; }

        [ConfigSection("NPC")]
        public abstract string NPCFundsGuild { get; set; }
        public abstract string NPCFundsPermission { get; set; }
        public abstract string NPCFundsCost { get; set; }
        public abstract string NPCFundsCurrency { get; set; }
        public abstract string NPCCost { get; set; }
        public abstract string NPCNoRoom { get; set; }
        public abstract string NPCFundsBuy { get; set; }
        public abstract string NPCSellWorthless { get; set; }
        public abstract string NPCSellTooMuchGold { get; set; }
        public abstract string NPCSellResult { get; set; }
        public abstract string FragmentCost { get; set; }
        public abstract string FragmentSpace { get; set; }
        public abstract string FragmentResult { get; set; }
        public abstract string AccessoryLevelCost { get; set; }
        public abstract string AccessoryLeveled { get; set; }
        public abstract string RepairFail { get; set; }
        public abstract string RepairFailRepaired { get; set; }
        public abstract string RepairFailLocation { get; set; }
        public abstract string RepairFailCooldown { get; set; }
        public abstract string NPCRepairGuild { get; set; }
        public abstract string NPCRepairPermission { get; set; }
        public abstract string NPCRepairGuildCost { get; set; }
        public abstract string NPCRepairCost { get; set; }
        public abstract string NPCRepairResult { get; set; }
        public abstract string NPCRepairSpecialResult { get; set; }
        public abstract string NPCRepairGuildResult { get; set; }
        public abstract string NPCRefinementGold { get; set; }
        public abstract string NPCRefinementStoneFailedRoom { get; set; }
        public abstract string NPCRefinementStoneFailed { get; set; }
        public abstract string NPCRefineNotReady { get; set; }
        public abstract string NPCRefineNoRoom { get; set; }
        public abstract string NPCRefineSuccess { get; set; }
        public abstract string NPCRefineFailed { get; set; }
        public abstract string NPCMasterRefineGold { get; set; }
        public abstract string NPCMasterRefineChance { get; set; }
        public abstract string AccessoryRefineSuccess { get; set; }
        public abstract string AccessoryRefineFailed { get; set; }

        [ConfigSection("Skills")]
        public abstract string ChargeExpire { get; set; }
        public abstract string ChargeFail { get; set; }
        public abstract string CloakCombat { get; set; }
        public abstract string DashFailed { get; set; }
        public abstract string WraithLevel { get; set; }
        public abstract string AbyssLevel { get; set; }
        public abstract string SkillEffort { get; set; }
        public abstract string SkillBadMap { get; set; }

        [ConfigSection("Horse")]
        public abstract string HorseDead { get; set; }
        public abstract string HorseOwner { get; set; }
        public abstract string HorseMap { get; set; }

        [ConfigSection("Instance")]
        public abstract string InstanceNoAction { get; set; }
        public abstract string InstanceInvalid { get; set; }
        public abstract string InstanceInsufficientLevel { get; set; }
        public abstract string InstanceSafeZoneOnly { get; set; }
        public abstract string InstanceNotInGroup { get; set; }
        public abstract string InstanceNotInGuild { get; set; }
        public abstract string InstanceNotInCastle { get; set; }
        public abstract string InstanceTooFewInGroup { get; set; }
        public abstract string InstanceTooManyInGroup { get; set; }
        public abstract string InstanceConnectRegionNotSet { get; set; }
        public abstract string InstanceUserCooldown { get; set; }
        public abstract string InstanceGuildCooldown { get; set; }
        public abstract string InstanceNoSlots { get; set; }
        public abstract string InstanceNoRejoin { get; set; }
        public abstract string InstanceMissingItem { get; set; }
        public abstract string InstanceNotGroupLeader { get; set; }
        public abstract string InstanceNoMap { get; set; }

        [ConfigSection("Discipline")]
        public abstract string DisciplineMaxLevel { get; set; }
        public abstract string DisciplineRequiredLevel { get; set; }
        public abstract string DisciplineRequiredGold { get; set; }
        public abstract string DisciplineRequiredExp { get; set; }

        [ConfigSection("Fame")]
        public abstract string FameNeedSpace { get; set; }
    }
}

﻿using Library;

namespace Server.Envir.Translations
{
    [ConfigPath(@".\Translations\ChineseMessages.ini")]
    public class ChineseMessages : StringMessages
    {
        public override string BannedWrongPassword { get; set; } = "Too many wrong password login attempts.";


        public override string PaymentComplete { get; set; } = "Your payment for {0} Game Gold was successful.";
        public override string PaymentFailed { get; set; } = "You have been deduceted {0} Game Gold.";
        public override string ReferralPaymentComplete { get; set; } = "One of your referral's has purchased some game gold, You got a bonus of {0} Hunt Gold.";
        public override string ReferralPaymentFailed { get; set; } = "One of your referal's purchase has failed, You lost your bonus of {0} Hunt Gold.";
        public override string GameGoldLost { get; set; } = "Your {0} Game Gold was removed.";
        public override string GameGoldRefund { get; set; } = "Your {0} Game Gold was refunded.";
        public override string HuntGoldRefund { get; set; } = "Your {0} Hunt Gold was refunded.";


        public override string Welcome { get; set; } = "Welcome to Zircon Server.";
        public override string WelcomeObserver { get; set; } = "You are now Observing {0}, to stop, please logout.";
        public override string ObserverChangeFail { get; set; } = "You cannot change observable mode unless you are in SafeZone";
        public override string OnlineCount { get; set; } = "Users Online: {0}, Observers Online: {1}";
        public override string ObserverCount { get; set; } = "You currently have {0} observers.";
        public override string CannotFindPlayer { get; set; } = "Unable to find the player: {0}";
        public override string AlreadyBlocked { get; set; } = "{0} is already on your block list.";
        public override string BlockingWhisper { get; set; } = "You are blocking Whispers.";
        public override string PlayerBlockingWhisper { get; set; } = "Player: {0} is blocking Whispers.";
        public override string GlobalDelay { get; set; } = "You cannot global for another {0} seconds.";
        public override string GlobalLevel { get; set; } = "You need to be level 33 before you can global shout.";
        public override string ShoutDelay { get; set; } = "You cannot shout for another {0} seconds.";
        public override string ShoutLevel { get; set; } = "You need to be level 2 before you can shout.";
        public override string DiceRoll { get; set; } = "[ROLL] - {0} has rolled {1} on a {2} sided dice.";
        public override string TradingEnabled { get; set; } = "Trading Enabled.";
        public override string TradingDisabled { get; set; } = "Trading Disabled.";
        public override string WhisperEnabled { get; set; } = "Whisper Enabled.";
        public override string WhisperDisabled { get; set; } = "Whisper Disabled.";
        public override string GuildInviteEnabled { get; set; } = "Guild Invites Enabled.";
        public override string GuildInviteDisabled { get; set; } = "Guild Invites Disabled.";
        public override string ObserverNotLoggedIn { get; set; } = "You need to be logged in before you can chat";
        public override string Poisoned { get; set; } = "You have been poisoned.";
        public override string MurderedBy { get; set; } = "You have been murdered by {0}.";
        public override string Curse { get; set; } = "You have murdered {0}, Bad luck follows you around...";
        public override string Murdered { get; set; } = "You have murdered {0}.";
        public override string Protected { get; set; } = "You have been protected by the law of self defence.";
        public override string Killed { get; set; } = "You have been killed by {0} in self defence.";
        public override string Died { get; set; } = "You have died in combat.";
        public override string GroupRecallEnabled { get; set; } = "Group Recall Enabled.";
        public override string GroupRecallDisabled { get; set; } = "Group Recall Disabled.";


        public override string NeedLevel { get; set; } = "You need to be level {0} to proceed.";
        public override string NeedMaxLevel { get; set; } = "You need to be level {0} or lower to proceed.";
        public override string NeedItem { get; set; } = "You require a '{0}' to proceed.";
        public override string NeedMonster { get; set; } = "The way is blocked...";


        public override string ConquestStarted { get; set; } = "{0} Conquest has started.";
        public override string ConquestFinished { get; set; } = "{0} Conquest has finished.";
        public override string ConquestCapture { get; set; } = "{0} has Captured {1}.";
        public override string ConquestOwner { get; set; } = "{0} are the now the owners of {1}.";
        public override string ConquestLost { get; set; } = "{0} have lost {1}.";


        public override string BossSpawn { get; set; } = "An evil lurks within {0}.";
        public override string HarvestRare { get; set; } = "Something valuable is hidden inside the {0}.";
        public override string NetherGateOpen { get; set; } = "The gate to the netherworld has opened, {0}, {1}";
        public override string NetherGateClosed { get; set; } = "The gate to the netherworld has closed";
        public override string HarvestNothing { get; set; } = "Nothing was found.";
        public override string HarvestCarry { get; set; } = "Cannot carry any more.";
        public override string HarvestOwner { get; set; } = "You do not own any nearby carcasses.";
        public override string LairGateOpen { get; set; } = "The gate to the underworld has opened, {0}, {1}";
        public override string LairGateClosed { get; set; } = "The gate to the underworld has closed";


        public override string Expired { get; set; } = "Your {0} has expired.";
        public override string CannotTownTeleport { get; set; } = "Unable to Town Teleport on this Map.";
        public override string CannotRandomTeleport { get; set; } = "Unable to Random Teleport on this Map.";
        public override string ConnotResetCompanionSkill { get; set; } = "To use {0} please type '@EnableLevel{1}'";
        public override string LearnBookFailed { get; set; } = "Failed to learn skill, not enough pages";
        public override string LearnBookSuccess { get; set; } = "Congratulations, You have successfully learned {0}.";
        public override string LearnBook4Failed { get; set; } = "Failed to learn level {0} skill.";
        public override string LearnBook4Success { get; set; } = "Congratulations, You have successfully learned level {1} {0}.";
        public override string StorageSafeZone { get; set; } = "You cannot access storage outside of SafeZone.";
        public override string GuildStoragePermission { get; set; } = "You do no have the permissions to take from the guild storage";
        public override string GuildStorageSafeZone { get; set; } = "You cannot use guild storage unless you are in a safe zone";
        public override string CompanionNoRoom { get; set; } = "Your companion cannot carry this many items";
        public override string StorageLimit { get; set; } = "You cannot expand your storage anymore.";


        public override string MarryAlreadyMarried { get; set; } = "You are already married.";
        public override string MarryNeedLevel { get; set; } = "You need to be atleast level 22 to get married.";
        public override string MarryNeedGold { get; set; } = "You do not have the 500,000 Gold required to pay for this service.";
        public override string MarryNotFacing { get; set; } = "You need to be facing another player to propose.";
        public override string MarryTargetAlreadyMarried { get; set; } = "{0} is already married.";
        public override string MarryTargetHasProposal { get; set; } = "{0} already has a marriage proposal.";
        public override string MarryTargetNeedLevel { get; set; } = "{0} needs to be atleast level 22 to get married.";
        public override string MarryTargetNeedGold { get; set; } = "{0} cannot afford to get married to you.";
        public override string MarryDead { get; set; } = "You cannot marry a dead person.";
        public override string MarryComplete { get; set; } = "Congratulations, you're now married to {0}.";
        public override string MarryDivorce { get; set; } = "You have divorced {0}";
        public override string MarryDivorced { get; set; } = "{0} has divorced you.";
        public override string MarryTeleportDead { get; set; } = "You cannot teleport to your partner you are dead.";
        public override string MarryTeleportPK { get; set; } = "You cannot teleport to your partner you are Red.";
        public override string MarryTeleportDelay { get; set; } = "You cannot teleport to your partner for another {0}.";
        public override string MarryTeleportOffline { get; set; } = "You cannot teleport to your partner whilst they are offline.";
        public override string MarryTeleportPartnerDead { get; set; } = "You cannot teleport to your partner whilst they are dead.";
        public override string MarryTeleportMap { get; set; } = "You cannot teleport to your partner on that map.";
        public override string MarryTeleportMapEscape { get; set; } = "You cannot use marraige teleport on this map.";


        public override string CompanionAppearanceAlready { get; set; } = "The {0} appreanace is already available.";
        public override string CompanionNeedTicket { get; set; } = "You need to have a Companion ticket to unlock a new appearance.";
        public override string CompanionSkillEnabled { get; set; } = "Companion Skill level {0} Enabled.";
        public override string CompanionSkillDisabled { get; set; } = "Companion Skill level {0} Disabled.";
        public override string CompanionAppearanceLocked { get; set; } = "The {0} appreanace is not available to you.";
        public override string CompanionNeedGold { get; set; } = "You cannot afford to adopt this companion.";
        public override string CompanionBadName { get; set; } = "The name given for your new companion is not acceptable.";
        public override string CompanionRetrieveFailed { get; set; } = "Able able to retrieve {0} because it is currently with {1}.";
        public override string QuestSelectReward { get; set; } = "You must select a reward";
        public override string QuestNeedSpace { get; set; } = "Unable to complete quest, Not enough space in your inventory.";


        public override string MailSafeZone { get; set; } = "Unable to get item from mail, you are not in a safe zone.";
        public override string MailNeedSpace { get; set; } = "Unable to get item from mail, not enough space.";
        public override string MailHasItems { get; set; } = "Unable to delete mail that contains items.";
        public override string MailNotFound { get; set; } = "{0} does not exist.";
        public override string MailSelfMail { get; set; } = "You cannot send mail to yourself.";
        public override string MailMailCost { get; set; } = "You cannot afford to send this mail.";
        public override string MailSendSafeZone { get; set; } = "You cannot send items from your inventory if you are not in SafeZone";


        public override string ConsignSafeZone { get; set; } = "You cannot Consign items from your inventory outside of safezone";
        public override string ConsignLimit { get; set; } = "You have reached the maximum number of Consignments";
        public override string ConsignGuildFundsGuild { get; set; } = "You cannot use Guild Funds to buy from the market place because you are not in a guild.";
        public override string ConsignGuildFundsPermission { get; set; } = "You cannot use Guild Funds to buy from the market place because you do not have permission.";
        public override string ConsignGuildFundsCost { get; set; } = "Your Guild cannot afford to buy this many items.";
        public override string ConsignGuildFundsUsed { get; set; } = "{0} used {1:#,##0} gold of guild funds to consign {2} x{3} for {4} each.";
        public override string ConsignCost { get; set; } = "You cannot afford to buy this many items.";
        public override string ConsignComplete { get; set; } = "Item registered successfully.";
        public override string ConsignAlreadySold { get; set; } = "This item has already sold.";
        public override string ConsignNotEnough { get; set; } = "There is not enough of this item for sale.";
        public override string ConsignBuyOwnItem { get; set; } = "You cannot buy your own item.";
        public override string ConsignBuyGuildFundsGuild { get; set; } = "You cannot use Guild Funds to buy from a merchant because you are not in a guild.";
        public override string ConsignBuyGuildFundsPermission { get; set; } = "You cannot use Guild Funds to buy from the market place because you do not have permission.";
        public override string ConsignBuyGuildFundsCost { get; set; } = "Your Guild cannot afford to buy this many items.";
        public override string ConsignBuyGuildFundsUsed { get; set; } = "{0} used {1:#,##0} gold of guild funds to buy {2} x{3}.";
        public override string ConsignBuyCost { get; set; } = "You cannot afford to buy this many items.";


        public override string StoreNotAvailable { get; set; } = "You cannot buy this item, It is not currently available for purchase.";
        public override string StoreNeedSpace { get; set; } = "You cannot carry this many items, please make room in your inventory.";
        public override string StoreCost { get; set; } = "You cannot afford to buy this many items.";


        public override string GuildNeedHorn { get; set; } = "Failed to create guild, You do not have the Uma King's Horn.";
        public override string GuildNeedGold { get; set; } = "Failed to create guild, You do not have enough gold.";
        public override string GuildBadName { get; set; } = "Failed to create guild, guild name is not acceptable.";
        public override string GuildNameTaken { get; set; } = "Failed to create guild, guild name already in use.";
        public override string GuildNoticePermission { get; set; } = "You do not have permission to change the guild notice.";
        public override string GuildEditMemberPermission { get; set; } = "You do not have permission to change guild member information.";
        public override string GuildMemberLength { get; set; } = "Failed to change guild rank, Rank Name was too long.";
        public override string GuildMemberNotFound { get; set; } = "Unable to find guild member.";
        public override string GuildKickPermission { get; set; } = "You do not have permission to kick a member.";
        public override string GuildKickSelf { get; set; } = "Unable kick yourself from the guild.";
        public override string GuildMemberKicked { get; set; } = "{0} has been kicked from the guild by {1}.";
        public override string GuildKicked { get; set; } = "You have been kicked form the guild by {0}.";
        public override string GuildManagePermission { get; set; } = "You do not have permission to Manage the guild.";
        public override string GuildMemberLimit { get; set; } = "Guild has already reached the Maxmimum Member Limit.";
        public override string GuildMemberCost { get; set; } = "Guild does not have enough funds to increase member limit.";
        public override string GuildStorageLimit { get; set; } = "Guild has already reached the Maxmimum Storage Size.";
        public override string GuildStorageCost { get; set; } = "Guild does not have enough funds to increase storage limit.";
        public override string GuildInvitePermission { get; set; } = "You do not have permission to invite new members";
        public override string GuildInviteGuild { get; set; } = "Player: {0}, is already in another guild.";
        public override string GuildInviteInvited { get; set; } = "Player: {0}, is currently being invited to another Guild.";
        public override string GuildInviteNotAllowed { get; set; } = "Player: {0}, is not allowing guild invites.";
        public override string GuildInvitedNotAllowed { get; set; } = "{0} wishes to invite you to the guild {1}, but you are not allowing Invites. @AllowGuild";
        public override string GuildInviteRoom { get; set; } = "Your guild already has reached it's member limit.";
        public override string GuildNoGuild { get; set; } = "You are not in a guild.";
        public override string GuildWarPermission { get; set; } = "You do not have the permission to start a guild war.";
        public override string GuildNotFoundGuild { get; set; } = "Could not find the guild {0}.";
        public override string GuildWarOwnGuild { get; set; } = "You cannot war your own guild.";
        public override string GuildAlreadyWar { get; set; } = "You are already at war with {0}.";
        public override string GuildWarCost { get; set; } = "Your guild cannot afford to start a guild war.";
        public override string GuildWarFunds { get; set; } = "{0} used {1:#,##0} gold of guild funds to start a war with {2}.";
        public override string GuildConquestCastle { get; set; } = "You already own a castle, You cannot submit a conquest.";
        public override string GuildConquestExists { get; set; } = "You already have a scheduled conquest.";
        public override string GuildConquestBadCastle { get; set; } = "Invalid Castle.";
        public override string GuildConquestProgress { get; set; } = "You cannot submit whilst a conquest is in process.";
        public override string GuildConquestNeedItem { get; set; } = "You need {0} to request a {1} conquest.";
        public override string GuildConquestSuccess { get; set; } = "A guild has submitted a conquest war for your castle.";
        public override string GuildConquestDate { get; set; } = "Your guild has submitted a conquest war for {0}.";
        public override string GuildJoinGuild { get; set; } = "You are already in a guild.";
        public override string GuildJoinTime { get; set; } = "You cannot join a guild a for another {0}";
        public override string GuildJoinNoGuild { get; set; } = "Player: {0}, is no longer in a guild.";
        public override string GuildJoinPermission { get; set; } = "Player: {0}, does not have permission to add you to the guild.";
        public override string GuildJoinNoRoom { get; set; } = "{0}'s group has already reached the maximum size.";
        public override string GuildJoinWelcome { get; set; } = "Welcome to the guild: {0}.";
        public override string GuildMemberJoined { get; set; } = "{0} has invited {1} to the guild.";
        public override string GuildLeaveFailed { get; set; } = "Failed to leave guild, You cannot leave other guild members without a leader.";
        public override string GuildLeave { get; set; } = "You have left the guild.";
        public override string GuildMemberLeave { get; set; } = "{0} has left the guild.";
        public override string GuildWarDeath { get; set; } = "{0} from {1} was killed by {2} from the guild {3}.";


        public override string GroupNoGroup { get; set; } = "You are not in a group.";
        public override string GroupNotLeader { get; set; } = "You are not the leader of your group";
        public override string GroupMemberNotFound { get; set; } = "Could not find Group Membmer {0} in your group.";
        public override string GroupAlreadyGrouped { get; set; } = "Player: {0}, is already in another group.";
        public override string GroupAlreadyInvited { get; set; } = "Player: {0}, is currently being invited to another group.";
        public override string GroupInviteNotAllowed { get; set; } = "Player: {0}, is not allowing group invites.";
        public override string GroupSelf { get; set; } = "You can not group with yourself.";
        public override string GroupMemberLimit { get; set; } = "{0}'s group has already reached the maximum size.";
        public override string GroupRecallDelay { get; set; } = "You cannot group recall for another {0}";
        public override string GroupRecallMap { get; set; } = "You cannot group recall on this map";
        public override string GroupRecallNotAllowed { get; set; } = "You are not allowing group recall";
        public override string GroupRecallMemberNotAllowed { get; set; } = "{0} is now allowing group recall";
        public override string GroupRecallFromMap { get; set; } = "You cannot be recalled from this map.";
        public override string GroupRecallMemberFromMap { get; set; } = "{0} cannot be recalled from this map.";


        public override string TradeAlreadyTrading { get; set; } = "You are already Trading with Someone.";
        public override string TradeAlreadyHaveRequest { get; set; } = "You already have a request to trade with Someone.";
        public override string TradeNeedFace { get; set; } = "You need to be facing a player to request a Trade.";
        public override string TradeTargetNotAllowed { get; set; } = "{0} isn't allowing trade requests.";
        public override string TradeTargetAlreadyTrading { get; set; } = "{0} is already Trading.";
        public override string TradeTargetAlreadyHaveRequest { get; set; } = "{0} already has a trade reqeust.";
        public override string TradeNotAllowed { get; set; } = "{0} wishes to trade with you, but you are not allowing trades. @AllowTrade";
        public override string TradeTargetDead { get; set; } = "You cannot trade a dead person.";
        public override string TradeRequested { get; set; } = "You have send a trade request to {0}...";
        public override string TradeWaiting { get; set; } = "Waiting for Partner to Accept Trade...";
        public override string TradePartnerWaiting { get; set; } = "Your Partner is waiting for you to Accept Trade...";
        public override string TradeNoGold { get; set; } = "You do not have enough gold To Trade....";
        public override string TradePartnerNoGold { get; set; } = "Your partner dose not have enough gold To Trade.";
        public override string TradeTooMuchGold { get; set; } = "You cannot carry this much gold.";
        public override string TradePartnerTooMuchGold { get; set; } = "Your Partner cannot carry this much gold...";
        public override string TradeFailedItemsChanged { get; set; } = "Your Items were changed, Trade Failed.";
        public override string TradeFailedPartnerItemsChanged { get; set; } = "{0}'s Items were changed, Trade Failed.";
        public override string TradeNotEnoughSpace { get; set; } = "You can not Carry this many items, Please make space in your inventory and try again.";
        public override string TradeComplete { get; set; } = "Trade Complete..";


        public override string NPCFundsGuild { get; set; } = "You cannot use Guild Funds to buy from a merchant because you are not in a guild.";
        public override string NPCFundsPermission { get; set; } = "You cannot use Guild Funds to buy from a merchant because you do not have permission.";
        public override string NPCFundsCost { get; set; } = "Unable to buy items, Your Guild needs another {0:#,##0} Gold.";
        public override string NPCCost { get; set; } = "Unable to buy items, You need another {0:#,##0} Gold.";
        public override string NPCNoRoom { get; set; } = "You can not carry this many items.";
        public override string NPCFundsBuy { get; set; } = "{0} used {1:#,##0} gold of guild funds to buy {2} x{3}.";
        public override string NPCSellWorthless { get; set; } = "Unable to sell items that are worthless";
        public override string NPCSellTooMuchGold { get; set; } = "Unable to sell items, You would be carrying too much gold";
        public override string NPCSellResult { get; set; } = "You sold {0} item(s) for {1:#,##0} Gold.";
        public override string FragmentCost { get; set; } = "Unable to Fragment these items, You need another {0:#,##0} Gold.";
        public override string FragmentSpace { get; set; } = "Unable to Fragment these items, You need do not have enough inventory space.";
        public override string FragmentResult { get; set; } = "You fragmented {0} item(s) costing {1:#,##0}.";
        public override string AccessoryLevelCost { get; set; } = "You cannot afford to level up your item any more.";
        public override string AccessoryLeveled { get; set; } = "Congratulations your {0} has leveled up and is now ready for upgrade.";
        public override string RepairFail { get; set; } = "You cannot repair {0}.";
        public override string RepairFailRepaired { get; set; } = "You cannot repair {0}, it is already fully repaired.";
        public override string RepairFailLocation { get; set; } = "You cannot repair {0} here.";
        public override string RepairFailCooldown { get; set; } = "You cannot special repair {0} for another {1}.";
        public override string NPCRepairGuild { get; set; } = "You cannot use Guild Funds to repair because you are not in a guild.";
        public override string NPCRepairPermission { get; set; } = "You cannot use Guild Funds to repair because you do not have permission.";
        public override string NPCRepairGuildCost { get; set; } = "Unable to repair items, Your Guild needs another {0:#,##0} Gold.";
        public override string NPCRepairCost { get; set; } = "Unable to repair items, You need another {0:#,##0} Gold.";
        public override string NPCRepairResult { get; set; } = "You normal repaired {0} item(s) for {1:#,##0} Gold.";
        public override string NPCRepairSpecialResult { get; set; } = "You special repaired {0} item(s) for {1:#,##0} Gold.";
        public override string NPCRepairGuildResult { get; set; } = "{0} used {1:#,##0} gold of guild funds to repair {2} item(s).";
        public override string NPCRefinementGold { get; set; } = "You do not have enough gold.";
        public override string NPCRefinementStoneFailedRoom { get; set; } = "Failed to create Refinement Stone, Unable to gain this item";
        public override string NPCRefinementStoneFailed { get; set; } = "Failed to synthesize a Refinement stone.";
        public override string NPCRefineNotReady { get; set; } = "Unable to get refine back, it is not ready.";
        public override string NPCRefineNoRoom { get; set; } = "Unable to get refine back, not enough space in your inventory.";
        public override string NPCRefineSuccess { get; set; } = "Congratulations, Your refine was successful";
        public override string NPCRefineFailed { get; set; } = "Unfortunately, Your refine was not successful";
        public override string NPCMasterRefineGold { get; set; } = "You do not have enough gold to request a master refine evaluation, Cost: {0:#,##0}.";
        public override string NPCMasterRefineChance { get; set; } = "Your chance of success is: {0}%";
        public override string AccessoryRefineSuccess { get; set; } = "Congratulations your {0} has been refined with {1} + {2}.";
        public override string AccessoryRefineFailed { get; set; } = "The refine failed and your {0}'s have been destroyed";


        public override string ChargeExpire { get; set; } = "The energy for {0} has left your weapon.";
        public override string ChargeFail { get; set; } = "Failed to gether the energy to charge {0}.";
        public override string CloakCombat { get; set; } = "You cannot cast Cloak during Combat";
        public override string DashFailed { get; set; } = "You were not strong enough to move what is infront of you.";
        public override string WraithLevel { get; set; } = "{0} is too high of a level to be effected by your wraith grip.";
        public override string AbyssLevel { get; set; } = "{0} is too high of a level to be effected by your Abyss.";
        public override string SkillEffort { get; set; } = "Using {0} on this map takes more effort than normal, You cannot use items for a {1}.";
        public override string SkillBadMap { get; set; } = "You are unable to use {0} on this map.";


        public override string HorseDead { get; set; } = "You cannot ride your horse when dead.";
        public override string HorseOwner { get; set; } = "You do not own a horse to ride.";
        public override string HorseMap { get; set; } = "You cannot ride your horse on this map.";
    }
}

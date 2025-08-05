using Library;
using Library.Network;
using Library.SystemModels;
using MirDB;
using Server.DBModels;
using Server.Envir.Commands;
using Server.Envir.Commands.Handler;
using Server.Envir.Events;
using Server.Envir.Events.Triggers;
using Server.Infrastructure.Logging;
using Server.Infrastructure.Logging.Formatter;
using Server.Infrastructure.Network.Http;
using Server.Infrastructure.Network.Smtp;
using Server.Infrastructure.Network.Tcp;
using Server.Infrastructure.Network.Tcp.ListenerHandler;
using Server.Infrastructure.Scheduler;
using Server.Infrastructure.Service;
using Server.Infrastructure.Service.Connection;
using Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using C = Library.Network.ClientPackets;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;

namespace Server.Envir
{
    public static class SEnvir
    {
        //TODO: EVERYTHING static in here should be using DI ideally.

        public static readonly IpAddressService IpManager = new IpAddressService();
        public static readonly UserConnectionService UserConnectionService = new UserConnectionService(
            new UserConnectionFactory(() => UserConnectionService.RemoveConnection),
            IpManager
        );
        public static readonly UserBroadcastService BroadcastService = new UserBroadcastService(UserConnectionService);

        #region Logging
        //TODO: these things should not exist here - need to move to DI then we can push this up outside of SEnvir
        public static bool UseLogConsole = false;
        public static readonly ConcurrentQueue<string> ServerAppLogs = []; 
        public static readonly ConcurrentQueue<string> ChatAppLogs = [];
        //TODO-END

        private static SingleThreadScheduler LogWritingScheduler = new SingleThreadScheduler();
        private static readonly ILogFormatter LogFormatter = new StdLogFormatter();
        public static readonly ILogger ServerLogger = new CompositeLogger(
            UseLogConsole ? [new SystemConsoleLogger(LogFormatter)] : [new SystemAppLogger(ServerAppLogs, LogFormatter), new SystemFileLogger(LogWritingScheduler, LogFormatter)]
        );
        public static readonly ILogger UserChatLogger = new CompositeLogger(
            UseLogConsole ? [new UserChatFileLogger(LogWritingScheduler,LogFormatter)] : [new UserChatAppLogger(ChatAppLogs, LogFormatter), new UserChatFileLogger(LogWritingScheduler, LogFormatter)]
        );
        #endregion

        #region TcpServer
        private static readonly TcpServer UserTcpServer = new TcpServer(
            new UserConnectionListenerHandler(UserConnectionService), 
            Config.IPAddress, 
            Config.Port
        );

        private static readonly TcpServer ActiveUserCountTcpServer = new TcpServer(
            new ActiveUserCountListenerHandler(() => UserConnectionService.ActiveConnections.Count), 
            Config.IPAddress, 
            Config.UserCountPort
        );
        #endregion

        public static bool Started //TODO: not sure what purpose of this was really.. it was 100% controlled by UserTcpServer
        { 
            get => UserTcpServer.Started;
            set 
            {
                if (UserTcpServer.Started == value) return;
                if (value) UserTcpServer.Start();
                else UserTcpServer.Stop();
            } 
        }
        public static bool Saving { get; private set; }

        public static Thread EnvirThread { get; private set; }

        public static DateTime Now, StartTime, LastWarTime;

        public static int ProcessObjectCount, LoopCount;

        public static ICommandHandler CommandHandler = new ErrorHandlingCommandHandler(
            new PlayerCommandHandler(),
            new AdminCommandHandler()
        );

        public static bool ServerBuffChanged;

        public static EventInfoHandler EventHandler = new EventInfoHandler();

        #region Database

        public static Session Session;

        public static DBCollection<MapInfo> MapInfoList;
        public static DBCollection<InstanceInfo> InstanceInfoList;
        public static DBCollection<InstanceMapInfo> InstanceMapInfoList;
        public static DBCollection<SafeZoneInfo> SafeZoneInfoList;
        public static DBCollection<ItemInfo> ItemInfoList;
        public static DBCollection<RespawnInfo> RespawnInfoList;
        public static DBCollection<MagicInfo> MagicInfoList;
        public static DBCollection<CurrencyInfo> CurrencyInfoList;

        public static DBCollection<AccountInfo> AccountInfoList;
        public static DBCollection<CharacterInfo> CharacterInfoList;
        public static DBCollection<CharacterBeltLink> BeltLinkList;
        public static DBCollection<AutoPotionLink> AutoPotionLinkList;
        public static DBCollection<UserItem> UserItemList;
        public static DBCollection<UserCurrency> UserCurrencyList;
        public static DBCollection<RefineInfo> RefineInfoList;
        public static DBCollection<UserItemStat> UserItemStatsList;
        public static DBCollection<UserMagic> UserMagicList;
        public static DBCollection<BuffInfo> BuffInfoList;
        public static DBCollection<MonsterInfo> MonsterInfoList;
        public static DBCollection<FishingInfo> FishingInfoList;
        public static DBCollection<DisciplineInfo> DisciplineInfoList;
        public static DBCollection<FameInfo> FameInfoList;
        public static DBCollection<SetInfo> SetInfoList;
        public static DBCollection<AuctionInfo> AuctionInfoList;
        public static DBCollection<MailInfo> MailInfoList;
        public static DBCollection<QuestInfo> QuestInfoList;
        public static DBCollection<AuctionHistoryInfo> AuctionHistoryInfoList;
        public static DBCollection<UserDrop> UserDropList;
        public static DBCollection<StoreInfo> StoreInfoList;
        public static DBCollection<BaseStat> BaseStatList;
        public static DBCollection<MovementInfo> MovementInfoList;
        public static DBCollection<NPCInfo> NPCInfoList;
        public static DBCollection<MapRegion> MapRegionList;
        public static DBCollection<GuildInfo> GuildInfoList;
        public static DBCollection<GuildMemberInfo> GuildMemberInfoList;
        public static DBCollection<UserQuest> UserQuestList;
        public static DBCollection<UserQuestTask> UserQuestTaskList;
        public static DBCollection<CompanionInfo> CompanionInfoList;
        public static DBCollection<CompanionLevelInfo> CompanionLevelInfoList;
        public static DBCollection<UserCompanion> UserCompanionList;
        public static DBCollection<CompanionFilters> CompanionFiltersList;
        public static DBCollection<UserCompanionUnlock> UserCompanionUnlockList;
        public static DBCollection<CompanionSkillInfo> CompanionSkillInfoList;
        public static DBCollection<BlockInfo> BlockInfoList;
        public static DBCollection<FriendInfo> FriendInfoList;
        public static DBCollection<CastleInfo> CastleInfoList;
        public static DBCollection<UserConquest> UserConquestList;
        public static DBCollection<GameGoldPayment> GameGoldPaymentList;
        public static DBCollection<GameStoreSale> GameStoreSaleList;
        public static DBCollection<GameNPCList> GameNPCList;
        public static DBCollection<GuildWarInfo> GuildWarInfoList;
        public static DBCollection<UserConquestStats> UserConquestStatsList;
        public static DBCollection<UserFortuneInfo> UserFortuneInfoList;
        public static DBCollection<WeaponCraftStatInfo> WeaponCraftStatInfoList;
        public static DBCollection<UserDiscipline> UserDisciplineList;
        public static DBCollection<BundleInfo> BundleInfoList;
        public static DBCollection<LootBoxInfo> LootBoxInfoList;

        public static DBCollection<WorldEventTrigger> WorldEventInfoTriggerList;
        public static DBCollection<PlayerEventTrigger> PlayerEventInfoTriggerList;

        public static ItemInfo GoldInfo, RefinementStoneInfo, FragmentInfo, Fragment2Info, Fragment3Info, FortuneCheckerInfo, ItemPartInfo;

        public static GuildInfo StarterGuild;

        public static MapRegion MysteryShipMapRegion, LairMapRegion;

        public static List<MonsterInfo> BossList = [];

        public static List<Type> MagicTypes = [];

        #endregion

        #region Game Variables

        public static Random Random;

        public static Dictionary<MapInfo, Map> Maps = [];
        public static Dictionary<InstanceInfo, Dictionary<MapInfo, Map>[]> Instances = [];

        private static long _ObjectID;
        public static uint ObjectID => (uint)Interlocked.Increment(ref _ObjectID);

        public static LinkedList<MapObject> Objects = [];
        public static List<MapObject> ActiveObjects = [];

        public static List<PlayerObject> Players = [];
        public static List<ConquestWar> ConquestWars = [];

        public static List<SpawnInfo> Spawns = [];

        public static List<EventLog> EventLogs = [];

        private static TimeOfDay _TimeOfDay;
        public static TimeOfDay TimeOfDay
        {
            get { return _TimeOfDay; }
            set
            {
                if (_TimeOfDay == value) return;

                _TimeOfDay = value;
            }
        }

        public static float PreviousDayTime { get; private set; }
        private static float _DayTime;
        public static float DayTime
        {
            get { return _DayTime; }
            set
            {
                if (_DayTime == value) return;

                PreviousDayTime = _DayTime;
                _DayTime = value;

                Broadcast(new S.DayChanged { DayTime = DayTime });
            }
        }

        public static byte[] CryptoKey { get; set; }

        public static LinkedList<CharacterInfo> Rankings;
        public static HashSet<CharacterInfo> TopRankings;
        public static DateTime NextRankChangeReset;

        public static long ConDelay, SaveDelay;
        #endregion

        public static void StartServer()
        {
            if (Started || EnvirThread != null) return;

            EnvirThread = new Thread(() => EnvirLoop()) { IsBackground = true };
            EnvirThread.Start();
        }

        public static void LoadExperienceList()
        {
            string path = @".\Config\ExperienceList.txt";
            if (!File.Exists(path))
            {
                if (!Directory.Exists(@".\Config")) Directory.CreateDirectory(@".\Config");
                using (StreamWriter file = new StreamWriter(path))
                {
                    for (int i = 0; i < Globals.ExperienceList.Count; i++)
                    {
                        file.WriteLine(i == 0 ? "//needed for lvl0" : (Globals.ExperienceList[i].ToString(CultureInfo.InvariantCulture) + " // level " + i));
                    }
                }
            }
            else
            {
                var lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == string.Empty) continue; //ignore empty line
                    if (lines[i].TrimStart().StartsWith("//")) continue; //ignore comment
                    try
                    {
                        decimal exp = decimal.Parse(lines[i].Split('/')[0].Trim()); //remove comment
                        if (Globals.ExperienceList.Count > i)
                            Globals.ExperienceList.Add(exp);
                        else
                            Globals.ExperienceList[i] = exp;
                    }
                    catch (Exception)
                    {
                        ServerLogger.Log(string.Format("ExperienceList: Error parsing line {0} - {1}", i, lines[i]));
                    }
                }
            }
            ServerLogger.Log("Experience List Loaded.");
        }

        private static void LoadDatabase()
        {
            Random = new Random();

            Session = new Session(SessionMode.Users)
            {
                BackUpDelay = 60,
            };

            Session.Initialize(
                Assembly.GetAssembly(typeof(ItemInfo)), // returns assembly LibraryCore
                Assembly.GetAssembly(typeof(AccountInfo)) // returns assembly ServerLibrary
            );

            MapInfoList = Session.GetCollection<MapInfo>();
            InstanceInfoList = Session.GetCollection<InstanceInfo>();
            SafeZoneInfoList = Session.GetCollection<SafeZoneInfo>();
            ItemInfoList = Session.GetCollection<ItemInfo>();
            MonsterInfoList = Session.GetCollection<MonsterInfo>();
            FishingInfoList = Session.GetCollection<FishingInfo>();
            DisciplineInfoList = Session.GetCollection<DisciplineInfo>();
            RespawnInfoList = Session.GetCollection<RespawnInfo>();
            MagicInfoList = Session.GetCollection<MagicInfo>();
            CurrencyInfoList = Session.GetCollection<CurrencyInfo>();
            FameInfoList = Session.GetCollection<FameInfo>();

            AccountInfoList = Session.GetCollection<AccountInfo>();
            CharacterInfoList = Session.GetCollection<CharacterInfo>();
            BeltLinkList = Session.GetCollection<CharacterBeltLink>();
            AutoPotionLinkList = Session.GetCollection<AutoPotionLink>();
            UserItemList = Session.GetCollection<UserItem>();
            UserCurrencyList = Session.GetCollection<UserCurrency>();
            UserItemStatsList = Session.GetCollection<UserItemStat>();
            RefineInfoList = Session.GetCollection<RefineInfo>();
            UserMagicList = Session.GetCollection<UserMagic>();
            BuffInfoList = Session.GetCollection<BuffInfo>();
            SetInfoList = Session.GetCollection<SetInfo>();
            AuctionInfoList = Session.GetCollection<AuctionInfo>();
            MailInfoList = Session.GetCollection<MailInfo>();
            QuestInfoList = Session.GetCollection<QuestInfo>();
            AuctionHistoryInfoList = Session.GetCollection<AuctionHistoryInfo>();
            UserDropList = Session.GetCollection<UserDrop>();
            StoreInfoList = Session.GetCollection<StoreInfo>();
            BaseStatList = Session.GetCollection<BaseStat>();
            MovementInfoList = Session.GetCollection<MovementInfo>();
            NPCInfoList = Session.GetCollection<NPCInfo>();
            MapRegionList = Session.GetCollection<MapRegion>();
            GuildInfoList = Session.GetCollection<GuildInfo>();
            GuildMemberInfoList = Session.GetCollection<GuildMemberInfo>();
            UserQuestList = Session.GetCollection<UserQuest>();
            UserQuestTaskList = Session.GetCollection<UserQuestTask>();
            CompanionSkillInfoList = Session.GetCollection<CompanionSkillInfo>();

            CompanionInfoList = Session.GetCollection<CompanionInfo>();
            CompanionLevelInfoList = Session.GetCollection<CompanionLevelInfo>();
            UserCompanionList = Session.GetCollection<UserCompanion>();
            CompanionFiltersList = Session.GetCollection<CompanionFilters>();
            UserCompanionUnlockList = Session.GetCollection<UserCompanionUnlock>();
            BlockInfoList = Session.GetCollection<BlockInfo>();
            FriendInfoList = Session.GetCollection<FriendInfo>();
            CastleInfoList = Session.GetCollection<CastleInfo>();
            UserConquestList = Session.GetCollection<UserConquest>();
            GameGoldPaymentList = Session.GetCollection<GameGoldPayment>();
            GameStoreSaleList = Session.GetCollection<GameStoreSale>();
            GameNPCList = Session.GetCollection<GameNPCList>();
            GuildWarInfoList = Session.GetCollection<GuildWarInfo>();
            UserConquestStatsList = Session.GetCollection<UserConquestStats>();
            UserFortuneInfoList = Session.GetCollection<UserFortuneInfo>();
            WeaponCraftStatInfoList = Session.GetCollection<WeaponCraftStatInfo>();
            UserDisciplineList = Session.GetCollection<UserDiscipline>();
            BundleInfoList = Session.GetCollection<BundleInfo>();
            LootBoxInfoList = Session.GetCollection<LootBoxInfo>();

            WorldEventInfoTriggerList = Session.GetCollection<WorldEventTrigger>();
            PlayerEventInfoTriggerList = Session.GetCollection<PlayerEventTrigger>();

            GoldInfo = CurrencyInfoList.Binding.First(x => x.Type == CurrencyType.Gold).DropItem;

            RefinementStoneInfo = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.RefinementStone);
            FragmentInfo = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.Fragment1);
            Fragment2Info = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.Fragment2);
            Fragment3Info = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.Fragment3);

            ItemPartInfo = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.ItemPart);
            FortuneCheckerInfo = ItemInfoList.Binding.FirstOrDefault(x => x.ItemEffect == ItemEffect.FortuneChecker);

            MysteryShipMapRegion = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.MysteryShipRegionIndex);
            LairMapRegion = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.LairRegionIndex);
            StarterGuild = GuildInfoList.Binding.FirstOrDefault(x => x.StarterGuild);

            if (StarterGuild == null)
            {
                StarterGuild = GuildInfoList.CreateNewObject();
                StarterGuild.StarterGuild = true;
            }

            StarterGuild.GuildName = Config.StarterGuildName;

            #region Create Ranks
            Rankings = new LinkedList<CharacterInfo>();
            TopRankings = new HashSet<CharacterInfo>();
            foreach (CharacterInfo info in CharacterInfoList.Binding)
            {
                if (info.Deleted) continue;

                info.RankingNode = Rankings.AddLast(info);
                RankingSort(info, false, true);
            }
            UpdateLead();
            #endregion

            for (int i = UserQuestList.Count - 1; i >= 0; i--)
                if (UserQuestList[i].QuestInfo == null)
                    UserQuestList[i].Delete();

            for (int i = UserQuestTaskList.Count - 1; i >= 0; i--)
                if (UserQuestTaskList[i].Task == null)
                    UserQuestTaskList[i].Delete();

            foreach (MonsterInfo monster in MonsterInfoList.Binding)
            {
                if (!monster.IsBoss) continue;
                if (monster.Drops.Count == 0) continue;

                BossList.Add(monster);
            }

            CreateMagic();
        }

        public static void RankingSort(CharacterInfo character, bool updateLead = true, bool initialSetup = false)
        {
            //Only works on Increasing EXP, still need to do Rebirth or loss of exp ranking update.
            bool changed = false;

            LinkedListNode<CharacterInfo> node;

            while ((node = character.RankingNode.Previous) != null)
            {
                if (node.Value.Level > character.Level) break;
                if (node.Value.Level == character.Level && node.Value.Experience >= character.Experience) break;

                if (!initialSetup)
                {
                    SwapRankPosition(character.RankChange, node.Value.RankChange, RequiredClass.All);

                    if (character.Class == node.Value.Class)
                    {
                        switch (character.Class)
                        {
                            case MirClass.Warrior:
                                SwapRankPosition(character.RankChange, node.Value.RankChange, RequiredClass.Warrior);
                                break;
                            case MirClass.Wizard:
                                SwapRankPosition(character.RankChange, node.Value.RankChange, RequiredClass.Wizard);
                                break;
                            case MirClass.Taoist:
                                SwapRankPosition(character.RankChange, node.Value.RankChange, RequiredClass.Taoist);
                                break;
                            case MirClass.Assassin:
                                SwapRankPosition(character.RankChange, node.Value.RankChange, RequiredClass.Assassin);
                                break;
                        }
                    }
                }

                changed = true;

                Rankings.Remove(character.RankingNode);
                Rankings.AddBefore(node, character.RankingNode);
            }

            if (!updateLead || (TopRankings.Count >= 20 && !changed)) return; //5 * 4

            UpdateLead();
        }

        private static void SwapRankPosition(Dictionary<RequiredClass, int> rankA, Dictionary<RequiredClass, int> rankB, RequiredClass cls)
        {
            if (!rankA.ContainsKey(cls))
                rankA[cls] = 0;

            if (!rankB.ContainsKey(cls))
                rankB[cls] = 0;

            rankA[cls] = rankA[cls] + 1;
            rankB[cls] = rankB[cls] - 1;
        }

        public static void UpdateLead()
        {
            HashSet<CharacterInfo> newTopRankings = new HashSet<CharacterInfo>();

            int war = 5, wiz = 5, tao = 5, ass = 5;

            foreach (CharacterInfo cInfo in Rankings)
            {
                if (cInfo.Account.Admin) continue;

                switch (cInfo.Class)
                {
                    case MirClass.Warrior:
                        if (war == 0) continue;
                        war--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Wizard:
                        if (wiz == 0) continue;
                        wiz--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Taoist:
                        if (tao == 0) continue;
                        tao--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Assassin:
                        if (ass == 0) continue;
                        ass--;
                        newTopRankings.Add(cInfo);
                        break;
                }

                if (war == 0 && wiz == 0 && tao == 0 && ass == 0) break;
            }

            foreach (CharacterInfo info in TopRankings)
            {
                if (newTopRankings.Contains(info)) continue;

                info.Player?.BuffRemove(BuffType.Ranking);
            }

            foreach (CharacterInfo info in newTopRankings)
            {
                if (TopRankings.Contains(info)) continue;

                info.Player?.BuffAdd(BuffType.Ranking, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
            }

            TopRankings = newTopRankings;
        }

        private static void StartEnvir()
        {
            LoadDatabase();
            LoadExperienceList();

            #region Load Files
            for (int i = 0; i < MapInfoList.Count; i++)
            {
                Maps[MapInfoList[i]] = new Map(MapInfoList[i]);
            }

            for (int i = 0; i < InstanceInfoList.Count; i++)
            {
                int count = InstanceInfoList[i].MaxInstances > 0 ? InstanceInfoList[i].MaxInstances : byte.MaxValue;

                Instances[InstanceInfoList[i]] = new Dictionary<MapInfo, Map>[count];
            }

            Parallel.ForEach(Maps, x => x.Value.Load());

            #endregion

            foreach (Map map in Maps.Values)
                map.Setup();

            Parallel.ForEach(MapRegionList.Binding, x =>
            {
                Map map = GetMap(x.Map);

                if (map == null) return;

                x.CreatePoints(map.Width);
            });

            CreateSafeZones();

            CreateMovements();

            CreateNPCs();

            CreateSpawns();

            CreateQuestRegions();
        }

        private static void CreateMovements(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            foreach (MovementInfo movement in MovementInfoList.Binding)
            {
                if (movement.SourceRegion == null && movement.DestinationRegion == null)
                {
                    ServerLogger.Log($"[Movement] No Source or Destination Region, Index: {movement.Index}");
                    continue;
                }

                if (movement.SourceRegion == null)
                {
                    ServerLogger.Log($"[Movement] No Source Region, Destination: {movement.DestinationRegion.ServerDescription}");
                    continue;
                }

                Map sourceMap = GetMap(movement.SourceRegion.Map, instance, instanceSequence);

                if (sourceMap == null)
                {
                    if (instance == null)
                    {
                        ServerLogger.Log($"[Movement] Bad Source Map, Source: {movement.SourceRegion.ServerDescription}");
                    }

                    continue;
                }

                if (movement.DestinationRegion == null)
                {
                    ServerLogger.Log($"[Movement] No Destination Region, Source: {movement.SourceRegion.ServerDescription}");
                    continue;
                }

                if (movement.DestinationRegion.PointList.Count == 0)
                {
                    ServerLogger.Log($"[Movement] Bad Destination, Dest: {movement.DestinationRegion.ServerDescription}, No Points");
                    continue;
                }

                Map destMap = GetMap(movement.DestinationRegion.Map, instance, instanceSequence);

                if (destMap == null)
                {
                    if (instance == null)
                    {
                        ServerLogger.Log($"[Movement] Bad Destination Map, Destination: {movement.DestinationRegion.ServerDescription}");
                    }

                    if (movement.NeedInstance == null || movement.NeedInstance != instance)
                    {
                        continue;
                    }
                }

                foreach (Point sPoint in movement.SourceRegion.PointRegion)
                {
                    Cell source = sourceMap.GetCell(sPoint);

                    if (source == null)
                    {
                        ServerLogger.Log($"[Movement] Bad Origin, Source: {movement.SourceRegion.ServerDescription}, X:{sPoint.X}, Y:{sPoint.Y}");
                        continue;
                    }

                    if (source.Movements == null)
                        source.Movements = new List<MovementInfo>();

                    source.Movements.Add(movement);
                }
            }
        }

        private static void CreateNPCs(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            foreach (NPCInfo info in NPCInfoList.Binding)
            {
                if (info.Region == null) continue;

                Map map = GetMap(info.Region.Map, instance, instanceSequence);

                if (map == null)
                {
                    if (instance == null)
                    {
                        ServerLogger.Log(string.Format("[NPC] Bad Map, NPC: {0}, Map: {1}", info.NPCName, info.Region.ServerDescription));
                    }

                    continue;
                }

                NPCObject ob = new NPCObject
                {
                    NPCInfo = info,
                };

                if (!ob.Spawn(info.Region, instance, instanceSequence))
                    ServerLogger.Log($"[NPC] Failed to spawn NPC, Region: {info.Region.ServerDescription}, NPC: {info.NPCName}");
            }
        }

        private static void CreateQuestRegions(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            foreach (QuestInfo quest in QuestInfoList.Binding)
            {
                foreach (QuestTask task in quest.Tasks)
                {
                    if (task.Task != QuestTaskType.Region) continue;
                    if (task.RegionParameter == null) continue;

                    var sourceMap = GetMap(task.RegionParameter.Map, instance, instanceSequence);

                    if (sourceMap == null)
                    {
                        if (instance == null)
                        {
                            ServerLogger.Log($"[Quest Region] Bad Map, Map: {task.RegionParameter.ServerDescription}");
                        }

                        continue;
                    }

                    foreach (Point sPoint in task.RegionParameter.PointList)
                    {
                        Cell source = sourceMap.GetCell(sPoint);

                        if (source == null)
                        {
                            ServerLogger.Log($"[Quest Region] Bad Quest Region, Source: {task.RegionParameter.ServerDescription}, X:{sPoint.X}, Y:{sPoint.Y}");
                            continue;
                        }

                        if (source.QuestTasks == null)
                            source.QuestTasks = new List<QuestTask>();

                        if (source.QuestTasks.Contains(task)) continue;

                        source.QuestTasks.Add(task);
                    }
                }
            }
        }

        private static void CreateSafeZones(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            foreach (SafeZoneInfo info in SafeZoneInfoList.Binding)
            {
                if (info.Region == null) continue;

                Map map = GetMap(info.Region.Map, instance, instanceSequence);

                if (map == null)
                {
                    if (instance == null)
                    {
                        ServerLogger.Log($"[Safe Zone] Bad Map, Map: {info.Region.ServerDescription}");
                    }

                    continue;
                }

                map.HasSafeZone = true;
   
                HashSet<Point> edges = new HashSet<Point>();

                foreach (Point point in info.Region.PointList)
                {
                    Cell cell = map.GetCell(point);

                    if (cell == null)
                    {
                        ServerLogger.Log($"[Safe Zone] Bad Location, Region: {info.Region.ServerDescription}, X: {point.X}, Y: {point.Y}.");

                        continue;
                    }

                    cell.SafeZone = info;

                    if (info.Border)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Point test = Functions.Move(point, (MirDirection)i);

                            if (info.Region.PointList.Contains(test)) continue;

                            if (map.GetCell(test) == null) continue;

                            edges.Add(test);
                        }
                    }
                }

                foreach (Point point in edges)
                {
                    SpellObject ob = new SpellObject
                    {
                        Visible = true,
                        DisplayLocation = point,
                        TickCount = 10,
                        TickFrequency = TimeSpan.FromDays(365),
                        Effect = SpellEffect.SafeZone
                    };

                    ob.Spawn(map, point);
                }

                if (info.BindRegion == null || instance != null) continue;

                map = GetMap(info.BindRegion.Map);

                if (map == null)
                {
                    ServerLogger.Log($"[Safe Zone] Bad Bind Map, Map: {info.Region.ServerDescription}");

                    continue;
                }

                foreach (Point point in info.BindRegion.PointList)
                {
                    Cell cell = map.GetCell(point);

                    if (cell == null)
                    {
                        ServerLogger.Log($"[Safe Zone] Bad Location, Region: {info.BindRegion.ServerDescription}, X: {point.X}, Y: {point.Y}.");
                        continue;
                    }

                    info.ValidBindPoints.Add(point);
                }
            }
        }

        private static void CreateSpawns(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            foreach (RespawnInfo info in RespawnInfoList.Binding)
            {
                if (info.Monster == null) continue;
                if (info.Region == null) continue;

                Map map = GetMap(info.Region.Map, instance, instanceSequence);

                if (map == null)
                {
                    if (instance == null)
                    {
                        ServerLogger.Log(string.Format("[Respawn] Bad Map, Map: {0}", info.Region.ServerDescription));
                    }

                    continue;
                }

                Spawns.Add(new SpawnInfo(info, instance, instanceSequence));
            }
        }

        private static void RemoveSpawns(InstanceInfo instance = null, byte instanceSequence = 0)
        {
            Spawns.RemoveAll(x => x.CurrentMap.Instance == instance && x.CurrentMap.InstanceSequence == instanceSequence);
        }

        private static void CreateMagic()
        {
            MagicTypes.Clear();

            var types = typeof(MagicObject).Assembly.GetTypes().Where(type =>
                type.BaseType != null &&
                !type.IsAbstract &&
                type.BaseType == typeof(MagicObject) &&
                type.IsDefined(typeof(MagicTypeAttribute))).ToList();

            MagicTypes.AddRange(types);
        }

        private static void StopEnvir()
        {
            Now = DateTime.MinValue;

            Session = null;

            MapInfoList = null;
            InstanceInfoList = null;
            SafeZoneInfoList = null;
            AccountInfoList = null;
            CharacterInfoList = null;
            CurrencyInfoList = null;

            MapInfoList = null;
            SafeZoneInfoList = null;
            ItemInfoList = null;
            MonsterInfoList = null;
            RespawnInfoList = null;
            MagicInfoList = null;
            FishingInfoList = null;
            DisciplineInfoList = null;
            FameInfoList = null;

            BeltLinkList = null;
            UserItemList = null;
            UserCurrencyList = null;
            UserItemStatsList = null;
            UserMagicList = null;
            BuffInfoList = null;
            SetInfoList = null;
            UserDisciplineList = null;

            WorldEventInfoTriggerList = null;
            PlayerEventInfoTriggerList = null;

            Rankings = null;
            Random = null;


            Maps.Clear();
            Instances.Clear();
            Objects.Clear();
            ActiveObjects.Clear();
            Players.Clear();

            Spawns.Clear();

            _ObjectID = 0;


            EnvirThread = null;
        }


        public static void EnvirLoop()
        {
            Now = Time.Now;
            DateTime DBTime = Now + Config.DBSaveDelay;

            StartEnvir();
            UserTcpServer.Start();
            ServerLogger.Log($"Network Started. Listen: {Config.IPAddress}:{Config.Port}"); //TODO: maybe pass this into Start() method as a PostStartupHook param
            ActiveUserCountTcpServer.Start();
            HttpWebServer.StartWebServer();

            int count = 0, loopCount = 0;
            DateTime nextCount = Now.AddSeconds(1), UserCountTime = Now.AddMinutes(5), EventTimerTime = Now.AddMinutes(1), saveTime;
            int lastindex = 0;
            long conDelay = 0;

            LastWarTime = Now;

            ServerLogger.Log($"Loading Time: {Functions.ToString(Time.Now - Now, true)}");

            while (Started)
            {
                Now = Time.Now;
                loopCount++;

                try
                {
                    UserConnectionService.Process();

                    long delay = (Time.Now - Now).Ticks / TimeSpan.TicksPerMillisecond;
                    if (delay > conDelay)
                        conDelay = delay;

                    for (int i = Players.Count - 1; i >= 0; i--)
                        Players[i].StartProcess();

                    if (ServerBuffChanged)
                    {
                        for (int i = Players.Count - 1; i >= 0; i--)
                            Players[i].ApplyServerBuff();

                        ServerBuffChanged = false;
                    }

                    DateTime loopTime = Time.Now.AddMilliseconds(1);

                    if (lastindex < 0) lastindex = ActiveObjects.Count;

                    while (Time.Now <= loopTime)
                    {
                        lastindex--;

                        if (lastindex >= ActiveObjects.Count) continue;

                        if (lastindex < 0) break;

                        MapObject ob = ActiveObjects[lastindex];

                        if (ob.Race == ObjectType.Player) continue;

                        try
                        {
                            ob.StartProcess();
                            count++;
                        }
                        catch (Exception ex)
                        {
                            ActiveObjects.Remove(ob);
                            ob.Activated = false;

                            ServerLogger.Log(ex.Message);
                            ServerLogger.Log(ex.StackTrace);
                            File.AppendAllText(@".\Errors.txt", ex.StackTrace + Environment.NewLine);
                        }
                    }

                    if (Now >= nextCount)
                    {
                        if (Now >= DBTime && !Saving)
                        {
                            DBTime = Time.Now + Config.DBSaveDelay;
                            saveTime = Time.Now;

                            Save();

                            SaveDelay = (Time.Now - saveTime).Ticks / TimeSpan.TicksPerMillisecond;
                        }

                        ProcessObjectCount = count;
                        LoopCount = loopCount;
                        ConDelay = conDelay;

                        count = 0;
                        loopCount = 0;
                        conDelay = 0;

                        if (Now >= UserCountTime)
                        {
                            UserCountTime = Now.AddMinutes(5);
                            BroadcastService.BroadcastOnlineCount();
                        }

                        if (Now >= EventTimerTime)
                        {
                            EventTimerTime = Now.AddMinutes(1);

                            foreach(var timer in EventTimer.Timers)
                            {
                                if (!timer.Started) continue;

                                EventHandler.Process(timer.Player, "TIMERMINUTE");
                            }
                        }

                        CalculateLights();

                        CheckGuildWars();

                        foreach (KeyValuePair<MapInfo, Map> pair in Maps)
                            pair.Value.Process();

                        foreach (var instance in Instances)
                        {
                            for (byte instanceSequence = 0; instanceSequence < instance.Value.Length; instanceSequence++)
                            {
                                bool expired = false;

                                if (instance.Value[instanceSequence] == null) continue;

                                foreach (KeyValuePair<MapInfo, Map> pair in instance.Value[instanceSequence]) 
                                {
                                    pair.Value.Process();

                                    if (pair.Value.InstanceExpiryDateTime < SEnvir.Now)
                                    {
                                        expired = true;
                                    }
                                }

                                if (expired || instance.Value[instanceSequence].Values.All(x => x.LastPlayer.AddMinutes(Globals.InstanceUnloadTimeInMinutes) < DateTime.UtcNow))
                                {
                                    UnloadInstance(instance.Key, instanceSequence);
                                    break;
                                }
                            }
                        }

                        foreach (SpawnInfo spawn in Spawns)
                            spawn.DoSpawn(false);

                        for (int i = ConquestWars.Count - 1; i >= 0; i--)
                            ConquestWars[i].Process();

                        if (Config.EnableWebServer)
                        {
                            HttpWebServer.Process();
                        }

                        if (Config.ProcessGameGold)
                            ProcessGameGold();

                        nextCount = Now.AddSeconds(1);

                        if (nextCount.Day != Now.Day)
                        {
                            foreach (GuildInfo guild in GuildInfoList.Binding)
                            {
                                guild.DailyContribution = 0;
                                guild.DailyGrowth = 0;

                                foreach (GuildMemberInfo member in guild.Members)
                                {
                                    member.DailyContribution = 0;
                                    if (member.Account.Connection?.Player == null) continue;

                                    member.Account.Connection.Enqueue(new S.GuildDayReset { ObserverPacket = false });
                                }
                            }

                            GC.Collect(2, GCCollectionMode.Forced);
                        }

                        foreach (CastleInfo info in CastleInfoList.Binding)
                        {
                            if (nextCount.TimeOfDay < info.StartTime) continue;
                            if (Now.TimeOfDay > info.StartTime) continue;

                            StartConquest(info, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session = null;

                    ServerLogger.Log(ex.Message);
                    ServerLogger.Log(ex.StackTrace);
                    File.AppendAllText(@".\Errors.txt", ex.StackTrace + Environment.NewLine);

                    UserConnectionService.Broadcast(new G.Disconnect { Reason = DisconnectReason.Crashed });

                    Thread.Sleep(3000);
                    break;
                }
            }

            HttpWebServer.StopWebServer();
            UserTcpServer.Stop();
            ActiveUserCountTcpServer.Stop();
            SEnvir.ServerLogger.Log("Network Stopped.");

            while (Saving) Thread.Sleep(1);
            if (Session != null)
                Session.BackUpDelay = 0;
            Save();
            while (Saving) Thread.Sleep(1);

            StopEnvir();
        }

        public static void ProcessGameGold()
        {
            while (!HttpWebServer.Messages.IsEmpty)
            {
                IPNMessage message;

                if (!HttpWebServer.Messages.TryDequeue(out message) || message == null) return;

                HttpWebServer.PaymentList.Add(message);

                if (!message.Verified)
                {
                    SEnvir.ServerLogger.Log("INVALID PAYPAL TRANSACTION " + message.Message);
                    continue;
                }

                //Add message to another list for file moving

                string[] data = message.Message.Split('&');

                Dictionary<string, string> values = new Dictionary<string, string>();

                for (int i = 0; i < data.Length; i++)
                {
                    string[] keypair = data[i].Split('=');

                    values[keypair[0]] = keypair.Length > 1 ? keypair[1] : null;
                }

                bool error = false;
                string tempString, paymentStatus, transactionID;
                decimal tempDecimal;
                long tempInt;

                if (!values.TryGetValue("payment_status", out paymentStatus))
                    error = true;

                if (!values.TryGetValue("txn_id", out transactionID))
                    error = true;


                //Check that Txn_id has not been used
                for (int i = 0; i < SEnvir.GameGoldPaymentList.Count; i++)
                {
                    if (SEnvir.GameGoldPaymentList[i].TransactionID != transactionID) continue;
                    if (SEnvir.GameGoldPaymentList[i].Status != paymentStatus) continue;


                    SEnvir.ServerLogger.Log(string.Format("[Duplicated Transaction] ID:{0} Status:{1}.", transactionID, paymentStatus));
                    message.Duplicate = true;
                    return;
                }

                GameGoldPayment payment = SEnvir.GameGoldPaymentList.CreateNewObject();
                payment.RawMessage = message.Message;
                payment.Error = error;

                if (values.TryGetValue("payment_date", out tempString))
                    payment.PaymentDate = HttpUtility.UrlDecode(tempString);

                if (values.TryGetValue("receiver_email", out tempString))
                    payment.Receiver_EMail = HttpUtility.UrlDecode(tempString);
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_fee", out tempString) && decimal.TryParse(tempString, out tempDecimal))
                    payment.Fee = tempDecimal;
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_gross", out tempString) && decimal.TryParse(tempString, out tempDecimal))
                    payment.Price = tempDecimal;
                else
                    payment.Error = true;

                if (values.TryGetValue("custom", out tempString))
                    payment.CharacterName = tempString;
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_currency", out tempString))
                    payment.Currency = tempString;
                else
                    payment.Error = true;

                if (values.TryGetValue("txn_type", out tempString))
                    payment.TransactionType = tempString;
                else
                    payment.Error = true;

                if (values.TryGetValue("payer_email", out tempString))
                    payment.Payer_EMail = HttpUtility.UrlDecode(tempString);

                if (values.TryGetValue("payer_id", out tempString))
                    payment.Payer_ID = tempString;

                payment.Status = paymentStatus;
                payment.TransactionID = transactionID;
                //Check if Paymentstats == completed
                switch (payment.Status)
                {
                    case "Completed":
                        break;
                }
                if (payment.Status != HttpWebServer.Completed) continue;

                //check that receiver_email is my primary paypal email
                if (string.Compare(payment.Receiver_EMail, Config.ReceiverEMail, StringComparison.OrdinalIgnoreCase) != 0)
                    payment.Error = true;

                //check that paymentamount/current are correct
                if (payment.Currency != HttpWebServer.Currency)
                    payment.Error = true;

                if (HttpWebServer.GoldTable.TryGetValue(payment.Price, out tempInt))
                    payment.GameGoldAmount = tempInt;
                else
                    payment.Error = true;

                CharacterInfo character = SEnvir.GetCharacter(payment.CharacterName);

                if (character == null || payment.Error)
                {
                    SEnvir.ServerLogger.Log($"[Transaction Error] ID:{transactionID} Status:{paymentStatus}, Amount{payment.Price}.");
                    continue;
                }

                payment.Account = character.Account;
                character.Account.GameGold.Amount += payment.GameGoldAmount;
                character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.PaymentComplete, payment.GameGoldAmount), MessageType.System);
                character.Player?.GameGoldChanged();

                AccountInfo referral = payment.Account.Referral;

                if (referral != null)
                {
                    referral.HuntGold.Amount += payment.GameGoldAmount / 10;

                    if (referral.Connection != null)
                    {
                        referral.Connection.ReceiveChat(string.Format(referral.Connection.Language.ReferralPaymentComplete, payment.GameGoldAmount / 10), MessageType.System);
                        referral.Connection.Player?.HuntGoldChanged();
                    }
                }

                SEnvir.ServerLogger.Log($"[Game Gold Purchase] Character: {character.CharacterName}, Amount: {payment.GameGoldAmount}.");
            }
        }

        private static void Save()
        {
            if (Session == null) return;

            Saving = true;
            Session.Save(false);

            HttpWebServer.Save();

            Thread saveThread = new Thread(CommitChanges) { IsBackground = true };
            saveThread.Start(Session);
        }
        private static void CommitChanges(object data)
        {
            Session session = (Session)data;
            session?.Commit();

            HttpWebServer.CommitChanges(data);

            Saving = false;
        }
        public static void CheckGuildWars()
        {
            TimeSpan change = Now - LastWarTime;
            LastWarTime = Now;

            for (int i = GuildWarInfoList.Count - 1; i >= 0; i--)
            {
                GuildWarInfo warInfo = GuildWarInfoList[i];

                warInfo.Duration -= change;

                if (warInfo.Duration > TimeSpan.Zero) continue;

                foreach (GuildMemberInfo member in warInfo.Guild1.Members)
                    member.Account.Connection?.Player?.Enqueue(new S.GuildWarFinished { GuildName = warInfo.Guild2.GuildName });

                foreach (GuildMemberInfo member in warInfo.Guild2.Members)
                    member.Account.Connection?.Player?.Enqueue(new S.GuildWarFinished { GuildName = warInfo.Guild1.GuildName });

                warInfo.Guild1 = null;
                warInfo.Guild2 = null;

                warInfo.Delete();
            }

        }

        public static void CalculateLights()
        {
            DayTime = Math.Max(0.05F, Math.Abs((float)Math.Round(((Now.TimeOfDay.TotalMinutes * Config.DayCycleCount) % 1440) / 1440F * 2 - 1, 2))); //12 hour rotation

            var previousTimeOfDay = TimeOfDay;

            if (DayTime <= 0.35F)
            {
                TimeOfDay = TimeOfDay.Night;
            }
            else if (DayTime > 0.65F)
            {
                TimeOfDay = TimeOfDay.Day;
            }
            else
            {
                if (DayTime > PreviousDayTime)
                {
                    TimeOfDay = TimeOfDay.Dawn;
                }
                else
                {
                    TimeOfDay = TimeOfDay.Dusk;
                }
            }

            if (previousTimeOfDay != TimeOfDay)
            {
                SEnvir.EventHandler.Process("TIMEOFDAY");
            }
        }

        public static void StartConquest(CastleInfo info, bool forced)
        {
            List<GuildInfo> participants = new List<GuildInfo>();

            if (!forced)
            {
                for (int i = UserConquestList.Binding.Count - 1; i >= 0; i--)
                {
                    var conquest = UserConquestList.Binding[i];
                    if (conquest.Guild == null)
                    {
                        conquest.Delete();
                        continue;
                    }

                    if (conquest.Castle != info) continue;
                    if (conquest.WarDate > Now.Date) continue;

                    participants.Add(conquest.Guild);
                }

                if (participants.Count == 0) return;

                foreach (GuildInfo guild in GuildInfoList.Binding)
                {
                    if (guild.Castle != info) continue;

                    participants.Add(guild);
                }
            }

            ConquestWar War = new ConquestWar
            {
                Castle = info,
                Participants = participants,
                EndTime = Now + info.Duration,
                StartTime = Now.Date + info.StartTime,
            };

            War.StartWar();
        }

        public static UserItem CreateFreshItem(UserItem item)
        {
            UserItem freshItem = UserItemList.CreateNewObject();

            freshItem.Colour = item.Colour;

            freshItem.Info = item.Info;
            freshItem.CurrentDurability = item.CurrentDurability;
            freshItem.MaxDurability = item.MaxDurability;

            freshItem.Flags = item.Flags;

            freshItem.ExpireTime = item.ExpireTime;

            foreach (UserItemStat stat in item.AddedStats)
                freshItem.AddStat(stat.Stat, stat.Amount, stat.StatSource);
            freshItem.StatsChanged();

            return freshItem;
        }
        public static UserItem CreateFreshItem(ItemCheck check)
        {
            UserItem item = check.Item != null ? CreateFreshItem(check.Item) : CreateFreshItem(check.Info);

            item.Flags = check.Flags;
            item.ExpireTime = check.ExpireTime;

            if (IsCurrencyItem(item.Info) || item.Info.ItemEffect == ItemEffect.Experience)
                item.Count = check.Count;
            else
                item.Count = Math.Min(check.Info.StackSize, check.Count);

            check.Count -= item.Count;

            return item;
        }
        public static UserItem CreateFreshItem(ItemInfo info)
        {
            UserItem item = UserItemList.CreateNewObject();

            item.Colour = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));

            item.Info = info;
            item.CurrentDurability = info.Durability;
            item.MaxDurability = info.Durability;

            return item;
        }
        public static UserItem CreateDropItem(ItemCheck check, int chance = 15)
        {
            UserItem item = CreateDropItem(check.Info, chance);

            item.Flags = check.Flags;
            item.ExpireTime = check.ExpireTime;

            if (IsCurrencyItem(item.Info) || item.Info.ItemEffect == ItemEffect.Experience)
                item.Count = check.Count;
            else
                item.Count = Math.Min(check.Info.StackSize, check.Count);

            check.Count -= item.Count;

            return item;
        }
        public static UserItem CreateDropItem(ItemInfo info, int chance = 15)
        {
            UserItem item = UserItemList.CreateNewObject();

            item.Info = info;
            item.MaxDurability = info.Durability;

            item.Colour = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));

            if (item.Info.Rarity != Rarity.Common)
                chance *= 2;

            if (Random.Next(chance) == 0)
            {
                switch (info.ItemType)
                {
                    case ItemType.Weapon:
                        UpgradeWeapon(item);
                        break;
                    case ItemType.Shield:
                        UpgradeShield(item);
                        break;
                    case ItemType.Armour:
                        UpgradeArmour(item);
                        break;
                    case ItemType.Helmet:
                        UpgradeHelmet(item);
                        break;
                    case ItemType.Necklace:
                        UpgradeNecklace(item);
                        break;
                    case ItemType.Bracelet:
                        UpgradeBracelet(item);
                        break;
                    case ItemType.Ring:
                        UpgradeRing(item);
                        break;
                    case ItemType.Shoes:
                        UpgradeShoes(item);
                        break;
                    case ItemType.Bundle:
                        UpgradeBundle(item);
                        break;
                    case ItemType.LootBox:
                        UpgradeLootBox(item);
                        break;
                }
                item.StatsChanged();
            }

            switch (info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Shield:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                    item.CurrentDurability = Math.Min(Random.Next(info.Durability) + 1000, item.MaxDurability);
                    break;
                case ItemType.Meat:
                    item.CurrentDurability = Random.Next(info.Durability * 2) + 2000;
                    break;
                case ItemType.Ore:
                    item.CurrentDurability = Random.Next(info.Durability * 3) + 3000;
                    break;
                case ItemType.Book:
                    item.CurrentDurability = Random.Next(96) + 5; //0~95 + 5
                    break;
                default:
                    item.CurrentDurability = info.Durability;
                    break;
            }


            return item;
        }
        public static ItemInfo GetItemInfo(string name)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
                if (string.Compare(ItemInfoList[i].ItemName.Replace(" ", ""), name, StringComparison.OrdinalIgnoreCase) == 0)
                    return ItemInfoList[i];

            return null;
        }

        public static MagicInfo GetMagicInfo(int index)
        {
            return MagicInfoList.Binding.FirstOrDefault(magic => magic.Index == index);
        }

        public static MonsterInfo GetMonsterInfo(string name)
        {
            return MonsterInfoList.Binding.FirstOrDefault
            (monster => string.Compare(monster.MonsterName.Replace(" ", ""), name,
                            StringComparison.OrdinalIgnoreCase) == 0);
        }


        public static MonsterInfo GetMonsterInfo(Dictionary<MonsterInfo, int> list)
        {
            int total = 0;

            foreach (KeyValuePair<MonsterInfo, int> pair in list)
                total += pair.Value;

            int value = Random.Next(total);

            foreach (KeyValuePair<MonsterInfo, int> pair in list)
            {
                value -= pair.Value;

                if (value >= 0) continue;

                return pair.Key;
            }


            return null;
        }

        public static bool IsCurrencyItem(ItemInfo info)
        {
            return CurrencyInfoList.Binding.FirstOrDefault(x => x.DropItem == info) != null;
        }

        public static bool IsUndroppableCurrencyItem(ItemInfo info)
        {
            return CurrencyInfoList.Binding.FirstOrDefault(x => x.DropItem == info && !x.DropItem.CanDrop) != null;
        }

        public static void UpgradeWeapon(UserItem item)
        {
            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);

            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(250) == 0)
                    value += 1;

                if (Random.Next(1250) == 0)
                    value += 1;

                item.AddStat(Stat.Accuracy, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };


            if (Random.Next(3) == 0)
            {
                int value = 1;

                if (Random.Next(5) == 0)
                    value += 1;

                if (Random.Next(25) == 0)
                    value += 1;

                item.AddStat(Elements[Random.Next(Elements.Count)], value, StatSource.Added);
            }
        }
        public static void UpgradeShield(UserItem item)
        {
            if (Random.Next(10) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.DCPercent, value, StatSource.Added);
            }

            if (Random.Next(10) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MCPercent, value, StatSource.Added);
                item.AddStat(Stat.SCPercent, value, StatSource.Added);

            }

            if (Random.Next(10) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.BlockChance, value, StatSource.Added);
            }

            if (Random.Next(10) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.EvasionChance, value, StatSource.Added);
            }

            if (Random.Next(10) == 0)
            {
                int value = 1;

                if (Random.Next(50) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.PoisonResistance, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>
            {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
            };

            if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, 2, StatSource.Added);

                if (Random.Next(2) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -2, StatSource.Added);
                }

                if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, 2, StatSource.Added);

                    if (Random.Next(2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -2, StatSource.Added);
                    }

                    if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, 2, StatSource.Added);

                        if (Random.Next(2) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -2, StatSource.Added);
                        }

                    }
                    else if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -2, StatSource.Added);
                    }
                }
                else if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -2, StatSource.Added);
                }
            }
            else if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, -2, StatSource.Added);
            }
        }
        public static void UpgradeArmour(UserItem item)
        {
            if (Random.Next(2) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(2) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>
            {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
            };

            if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, 2, StatSource.Added);

                if (Random.Next(2) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -2, StatSource.Added);
                }

                if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, 2, StatSource.Added);

                    if (Random.Next(2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -2, StatSource.Added);
                    }

                    if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, 2, StatSource.Added);

                        if (Random.Next(2) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -2, StatSource.Added);
                        }

                    }
                    else if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -2, StatSource.Added);
                    }
                }
                else if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -2, StatSource.Added);
                }
            }
            else if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, -2, StatSource.Added);
            }
        }
        public static void UpgradeHelmet(UserItem item)
        {
            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }


            List<Stat> Elements = new List<Stat>
            {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
            };
            if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, 1, StatSource.Added);

                if (Random.Next(2) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }

                if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, 1, StatSource.Added);

                    if (Random.Next(2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }

                    if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, 1, StatSource.Added);

                        if (Random.Next(2) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -1, StatSource.Added);
                        }

                    }
                    else if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }
                }
                else if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }
            }
            else if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, -1, StatSource.Added);
            }
        }
        public static void UpgradeNecklace(UserItem item)
        {
            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }


            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }


            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;


                item.AddStat(Stat.Accuracy, value, StatSource.Added);
            }


            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.Agility, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };


            if (Random.Next(3) == 0)
            {
                item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);

                if (Random.Next(5) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);

                if (Random.Next(25) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);
            }
        }
        public static void UpgradeBracelet(UserItem item)
        {
            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }


            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.Accuracy, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.Agility, value, StatSource.Added);
            }


            List<Stat> Elements = new List<Stat>
            {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
            };

            if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, 1, StatSource.Added);

                if (Random.Next(2) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }

                if (Random.Next(30) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, 1, StatSource.Added);

                    if (Random.Next(2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }

                    if (Random.Next(40) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, 1, StatSource.Added);

                        if (Random.Next(2) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -1, StatSource.Added);
                        }

                    }
                    else if (Random.Next(40) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }
                }
                else if (Random.Next(30) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }
            }
            else if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, -1, StatSource.Added);
            }
        }
        public static void UpgradeRing(UserItem item)
        {


            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            if (Random.Next(3) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.PickUpRadius, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };


            if (Random.Next(3) == 0)
            {
                item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);

                if (Random.Next(5) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);

                if (Random.Next(25) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], 1, StatSource.Added);
            }
        }
        public static void UpgradeShoes(UserItem item)
        {
            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            if (Random.Next(5) == 0)
            {
                int value = 1;

                if (Random.Next(25) == 0)
                    value += 1;

                if (Random.Next(250) == 0)
                    value += 1;

                item.AddStat(Stat.Comfort, value, StatSource.Added);
            }


            List<Stat> Elements = new List<Stat>
            {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
            };
            if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, 1, StatSource.Added);

                if (Random.Next(2) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }

                if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, 1, StatSource.Added);

                    if (Random.Next(2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }

                    if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, 1, StatSource.Added);

                        if (Random.Next(2) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -1, StatSource.Added);
                        }

                    }
                    else if (Random.Next(60) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -1, StatSource.Added);
                    }
                }
                else if (Random.Next(45) == 0)
                {
                    element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -1, StatSource.Added);
                }
            }
            else if (Random.Next(10) == 0)
            {
                Stat element = Elements[Random.Next(Elements.Count)];

                Elements.Remove(element);

                item.AddStat(element, -1, StatSource.Added);
            }
        }

        public static void UpgradeBundle(UserItem item)
        {
            item.AddStat(Stat.Random1, Random.Next(byte.MaxValue), StatSource.Added); // Full randomise
        }

        public static void UpgradeLootBox(UserItem item)
        {
            var lootBoxInfo = SEnvir.LootBoxInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);

            if (lootBoxInfo == null) return;

            item.AddStat(Stat.Random1, Random.Next(byte.MaxValue), StatSource.Added); // Full randomise
            item.AddStat(Stat.Random2, Random.Next(byte.MaxValue), StatSource.Added); // Loot Box grid randomise

            item.AddStat(Stat.Counter1, Globals.LootBoxRerollCount, StatSource.Added);

            item.AddStat(Stat.Counter2, lootBoxInfo.Contents.Count <= 15 ? 2 : 1, StatSource.Added); // Step 1 = Randomise, 2 = Selection
        }

        public static void Login(C.Login p, UserConnection con)
        {
            AccountInfo account = null;
            bool admin = false;
            if (!Globals.EMailRegex.IsMatch(p.EMailAddress) && p.Password == Config.MasterPassword)
            {
                account = GetCharacter(p.EMailAddress)?.Account;
                admin = true;
                ServerLogger.Log($"[Admin Attempted] Character: {p.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
            }
            else
            {
                if (!Config.AllowLogin)
                {
                    con.Enqueue(new S.Login { Result = 0 });
                    return;
                }

                if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
                {
                    con.Enqueue(new S.Login { Result = LoginResult.BadEMail });
                    return;
                }

                if (!Globals.PasswordRegex.IsMatch(p.Password))
                {
                    con.Enqueue(new S.Login { Result = LoginResult.BadPassword });
                    return;
                }

                for (int i = 0; i < AccountInfoList.Count; i++)
                    if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        account = AccountInfoList[i];
                        break;
                    }
            }


            if (account == null)
            {
                con.Enqueue(new S.Login { Result = LoginResult.AccountNotExists });
                return;
            }

            if (!account.Activated)
            {
                con.Enqueue(new S.Login { Result = LoginResult.AccountNotActivated });
                return;
            }

            if (!admin && account.Banned)
            {
                if (account.BanExpiry > Now)
                {
                    con.Enqueue(new S.Login { Result = LoginResult.Banned, Message = account.BanReason, Duration = account.BanExpiry - Now });
                    return;
                }

                account.Banned = false;
                account.BanReason = string.Empty;
                account.BanExpiry = DateTime.MinValue;
            }

            if (!admin && !PasswordMatch(p.Password, account.Password))
            {
                ServerLogger.Log($"[Wrong Password] IP Address: {con.IPAddress}, Account: {account.EMailAddress}, Security: {p.CheckSum}");

                if (account.WrongPasswordCount++ >= 5)
                {
                    account.Banned = true;
                    account.BanReason = con.Language.BannedWrongPassword;
                    account.BanExpiry = Now.AddMinutes(1);

                    con.Enqueue(new S.Login { Result = LoginResult.Banned, Message = account.BanReason, Duration = account.BanExpiry - Now });
                    return;
                }

                con.Enqueue(new S.Login { Result = LoginResult.WrongPassword });
                return;
            }

            account.WrongPasswordCount = 0;


            //LockAccount ??
            if (account.Connection != null)
            {
                if (admin)
                {
                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedIn });
                    account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUser });
                    return;
                    //  account.Connection.SendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUserAdmin });
                }

                ServerLogger.Log($"[Account in Use] Account: {account.EMailAddress}, Current IP: {account.LastIP}, New IP: {con.IPAddress}, Security: {p.CheckSum}");

                if (account.TempAdmin)
                {
                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedInAdmin });
                    return;
                }

                if (account.LastIP != con.IPAddress && account.LastSum != p.CheckSum)
                {
                    account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUserPassword });
                    string password = Functions.RandomString(Random, 10);

                    account.Password = CreateHash(password);
                    account.ResetKey = string.Empty;
                    account.WrongPasswordCount = 0;

                    EmailService.SendResetPasswordEmail(account, password);

                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedInPassword });
                    return;
                }

                con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedIn });
                account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUser });
                return;
            }


            account.Connection = con;
            account.TempAdmin = admin;

            con.Account = account;
            con.Stage = GameStage.Select;

            account.Key = Functions.RandomString(Random, 20);


            con.Enqueue(new S.Login
            {
                Result = LoginResult.Success,
                Characters = account.GetSelectInfo(),

                Items = account.Items.Select(x => x.ToClientInfo()).ToList(),
                BlockList = account.BlockingList.Select(x => x.ToClientInfo()).ToList(),

                Address = $"{Config.BuyAddress}?Key={account.Key}&Character=",

                TestServer = Config.TestServer,
            });

            account.LastLogin = Now;

            if (!admin)
            {
                account.LastIP = con.IPAddress;
                account.LastSum = p.CheckSum;
            }

            ServerLogger.Log($"[Account Logon] Admin: {admin}, Account: {account.EMailAddress}, IP Address: {account.LastIP}, Security: {p.CheckSum}");
        }
        public static void NewAccount(C.NewAccount p, UserConnection con)
        {
            if (!Config.AllowNewAccount)
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadEMail });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.Password))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadPassword });
                return;
            }

            if ((Globals.RealNameRequired || !string.IsNullOrEmpty(p.RealName)) && (p.RealName.Length < Globals.MinRealNameLength || p.RealName.Length > Globals.MaxRealNameLength))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadRealName });
                return;
            }

            var list = AccountInfoList.Binding.Where(e => e.CreationIP == con.IPAddress).ToList();
            int nowcount = 0;
            int todaycount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                AccountInfo info = list[i];
                if (info == null) continue;

                if (info.CreationDate.AddSeconds(1) > Now)
                {
                    nowcount++;
                    if (nowcount > 2)
                        break;
                }
                if (info.CreationDate.AddDays(1) > Now)
                {
                    todaycount++;
                    if (todaycount > 5)
                        break;
                }
            }

            if (nowcount > 2 || todaycount > 5)
            {
                IpManager.Timeout(con, TimeSpan.FromDays(7));
                ServerLogger.Log($"{con.IPAddress} Disconnected and banned for trying too many accounts");
                return;
            }

            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.AlreadyExists });
                    return;
                }

            AccountInfo refferal = null;

            if (!string.IsNullOrEmpty(p.Referral))
            {
                if (!Globals.EMailRegex.IsMatch(p.Referral))
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadReferral });
                    return;
                }

                for (int i = 0; i < AccountInfoList.Count; i++)
                    if (string.Compare(AccountInfoList[i].EMailAddress, p.Referral, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        refferal = AccountInfoList[i];
                        break;
                    }
                if (refferal == null)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.ReferralNotFound });
                    return;
                }
                if (!refferal.Activated)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.ReferralNotActivated });
                    return;
                }
            }

            AccountInfo account = AccountInfoList.CreateNewObject();

            account.EMailAddress = p.EMailAddress;
            account.Password = CreateHash(p.Password);
            account.RealName = p.RealName;
            account.BirthDate = p.BirthDate;
            account.Referral = refferal;
            account.CreationIP = con.IPAddress;
            account.CreationDate = Now;

            if (refferal != null)
            {
                int maxLevel = refferal.HighestLevel();

                if (maxLevel >= 50) account.HuntGold.Amount = 500;
                else if (maxLevel >= 40) account.HuntGold.Amount = 300;
                else if (maxLevel >= 30) account.HuntGold.Amount = 200;
                else if (maxLevel >= 20) account.HuntGold.Amount = 100;
                else if (maxLevel >= 10) account.HuntGold.Amount = 50;
            }



            EmailService.SendActivationEmail(account);

            con.Enqueue(new S.NewAccount { Result = NewAccountResult.Success });

            ServerLogger.Log($"[Account Created] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void ChangePassword(C.ChangePassword p, UserConnection con)
        {
            if (!Config.AllowChangePassword)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadEMail });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.CurrentPassword))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadCurrentPassword });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.NewPassword))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadNewPassword });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }


            if (account == null)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.AccountNotFound });
                return;
            }
            if (!account.Activated)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.AccountNotActivated });
                return;
            }

            if (account.Banned)
            {
                if (account.BanExpiry > Now)
                {
                    con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Banned, Message = account.BanReason, Duration = account.BanExpiry - Now });
                    return;
                }

                account.Banned = false;
                account.BanReason = string.Empty;
                account.BanExpiry = DateTime.MinValue;
            }

            if (!PasswordMatch(p.CurrentPassword, account.Password))
            {
                ServerLogger.Log($"[Wrong Password] IP Address: {con.IPAddress}, Account: {account.EMailAddress}, Security: {p.CheckSum}");

                if (account.WrongPasswordCount++ >= 5)
                {
                    account.Banned = true;
                    account.BanReason = con.Language.BannedWrongPassword;
                    account.BanExpiry = Now.AddMinutes(1);

                    con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Banned, Message = account.BanReason, Duration = account.BanExpiry - Now });
                    return;
                }

                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.WrongPassword });
                return;
            }

            account.Password = CreateHash(p.NewPassword);
            EmailService.SendChangePasswordEmail(account, con.IPAddress);
            con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Success });

            ServerLogger.Log($"[Password Changed] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void RequestPasswordReset(C.RequestPasswordReset p, UserConnection con)
        {
            if (!Config.AllowRequestPasswordReset)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.BadEMail });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.AccountNotFound });
                return;
            }

            if (!account.Activated)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.AccountNotActivated });
                return;
            }

            if (Now < account.ResetTime)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.ResetDelay, Duration = account.ResetTime - Now });
                return;
            }

            EmailService.SendResetPasswordRequestEmail(account, con.IPAddress);
            con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.Success });

            ServerLogger.Log($"[Request Password] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void ResetPassword(C.ResetPassword p, UserConnection con)
        {
            if (!Config.AllowManualResetPassword)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.Disabled });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.NewPassword))
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.BadNewPassword });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].ResetKey, p.ResetKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.AccountNotFound });
                return;
            }

            if (account.ResetTime.AddMinutes(25) < Now)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.KeyExpired });
                return;
            }

            account.ResetKey = string.Empty;
            account.Password = CreateHash(p.NewPassword);
            account.WrongPasswordCount = 0;

            EmailService.SendChangePasswordEmail(account, con.IPAddress);
            con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.Success });

            ServerLogger.Log($"[Reset Password] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void Activation(C.Activation p, UserConnection con)
        {
            if (!Config.AllowManualActivation)
            {
                con.Enqueue(new S.Activation { Result = ActivationResult.Disabled });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].ActivationKey, p.ActivationKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.Activation { Result = ActivationResult.AccountNotFound });
                return;
            }

            account.ActivationKey = null;
            account.Activated = true;

            con.Enqueue(new S.Activation { Result = ActivationResult.Success });

            ServerLogger.Log($"[Activation] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void RequestActivationKey(C.RequestActivationKey p, UserConnection con)
        {
            if (!Config.AllowRequestActivation)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.BadEMail });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.AccountNotFound });
                return;
            }

            if (account.Activated)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.AlreadyActivated });
                return;
            }

            if (Now < account.ActivationTime)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.RequestDelay, Duration = account.ActivationTime - Now });
                return;
            }
            EmailService.ResendActivationEmail(account);
            con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.Success });
            ServerLogger.Log($"[Request Activation] Account: {account.EMailAddress}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }

        public static void NewCharacter(C.NewCharacter p, UserConnection con)
        {
            if (!Config.AllowNewCharacter)
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.Disabled });
                return;
            }

            if (!Globals.CharacterReg.IsMatch(p.CharacterName))
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadCharacterName });
                return;
            }

            switch (p.Gender)
            {
                case MirGender.Male:
                case MirGender.Female:
                    break;
                default:
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadGender });
                    return;
            }

            if (p.HairType < 0)
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                return;
            }

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255))
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairColour });
                return;
            }


            switch (p.Class)
            {
                case MirClass.Warrior:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowWarrior) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });

                    return;
                case MirClass.Wizard:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowWizard) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                case MirClass.Taoist:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowTaoist) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                case MirClass.Assassin:

                    if (p.HairType > 5)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.ToArgb() != 0)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }

                    if (Config.AllowAssassin) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                default:
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadClass });
                    return;
            }



            int count = 0;

            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Deleted) continue;

                if (++count < Globals.MaxCharacterCount) continue;

                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.MaxCharacters });
                return;
            }


            for (int i = 0; i < CharacterInfoList.Count; i++)
                if (string.Compare(CharacterInfoList[i].CharacterName, p.CharacterName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (CharacterInfoList[i].Account == con.Account) continue;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.AlreadyExists });
                    return;
                }

            CharacterInfo cInfo = CharacterInfoList.CreateNewObject();

            cInfo.CharacterName = p.CharacterName;
            cInfo.Account = con.Account;
            cInfo.Class = p.Class;
            cInfo.Gender = p.Gender;
            cInfo.HairType = p.HairType;
            cInfo.HairColour = p.HairColour;
            cInfo.ArmourColour = p.ArmourColour;
            cInfo.CreationIP = con.IPAddress;
            cInfo.CreationDate = Now;

            cInfo.RankingNode = Rankings.AddLast(cInfo);

            con.Enqueue(new S.NewCharacter
            {
                Result = NewCharacterResult.Success,
                Character = cInfo.ToSelectInfo(),
            });

            ServerLogger.Log($"[Character Created] Character: {p.CharacterName}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
        }
        public static void DeleteCharacter(C.DeleteCharacter p, UserConnection con)
        {
            if (!Config.AllowDeleteCharacter)
            {
                con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.Disabled });
                return;
            }

            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Index != p.CharacterIndex) continue;

                if (character.Deleted)
                {
                    con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.AlreadyDeleted });
                    return;
                }

                character.Deleted = true;
                con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.Success, DeletedIndex = character.Index });

                ServerLogger.Log($"[Character Deleted] Character: {character.CharacterName}, IP Address: {con.IPAddress}, Security: {p.CheckSum}");
                return;
            }

            con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.NotFound });
        }
        public static void StartGame(C.StartGame p, UserConnection con)
        {
            if (!Config.AllowStartGame)
            {
                con.Enqueue(new S.StartGame { Result = StartGameResult.Disabled });
                return;
            }

            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Index != p.CharacterIndex) continue;

                if (character.Deleted)
                {
                    con.Enqueue(new S.StartGame { Result = StartGameResult.Deleted });
                    return;
                }

                TimeSpan duration = Now - character.LastLogin;

                if (duration < Config.RelogDelay)
                {
                    con.Enqueue(new S.StartGame { Result = StartGameResult.Delayed, Duration = Config.RelogDelay - duration });
                    return;
                }

                PlayerObject player = new PlayerObject(character, con);
                player.StartGame();
                return;
            }

            con.Enqueue(new S.StartGame { Result = StartGameResult.NotFound });
        }

        public static bool IsBlocking(AccountInfo account1, AccountInfo account2)
        {
            if (account1 == null || account2 == null || account1 == account2) return false;

            if (account1.TempAdmin || account2.TempAdmin) return false;

            foreach (BlockInfo blockInfo in account1.BlockingList)
                if (blockInfo.BlockedAccount == account2) return true;

            foreach (BlockInfo blockInfo in account2.BlockingList)
                if (blockInfo.BlockedAccount == account1) return true;

            return false;
        }



        #region Password Encryption
        private const int Iterations = 1354;
        private const int SaltSize = 16;
        private const int hashSize = 20;

        public static byte[] CreateHash(string password)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);

                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hash = rfc.GetBytes(hashSize);

                    byte[] totalHash = new byte[SaltSize + hashSize];

                    Buffer.BlockCopy(salt, 0, totalHash, 0, SaltSize);
                    Buffer.BlockCopy(hash, 0, totalHash, SaltSize, hashSize);

                    return totalHash;
                }
            }
        }
        private static bool PasswordMatch(string password, byte[] totalHash)
        {
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(totalHash, 0, salt, 0, SaltSize);

            using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = rfc.GetBytes(hashSize);

                return Functions.IsMatch(totalHash, hash, SaltSize);
            }
        }
        #endregion


        public static int ErrorCount;
        private static string LastError;
        public static void SaveError(string ex)
        {
            try
            {
                if (++ErrorCount > 200 || String.Compare(ex, LastError, StringComparison.OrdinalIgnoreCase) == 0) return;

                const string LogPath = @".\Errors\";

                LastError = ex;

                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                File.AppendAllText($"{LogPath}{Now.Year}-{Now.Month}-{Now.Day}.txt", LastError + Environment.NewLine);
            }
            catch
            { }
        }
        public static PlayerObject GetPlayerByCharacter(string name)
        {
            return GetCharacter(name)?.Account.Connection?.Player;
        }
        public static UserConnection GetConnectionByCharacter(string name)
        {
            return GetCharacter(name)?.Account.Connection;
        }

        public static CharacterInfo GetCharacter(string name)
        {
            for (int i = 0; i < CharacterInfoList.Count; i++)
                if (string.Compare(CharacterInfoList[i].CharacterName, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return CharacterInfoList[i];

            return null;
        }

        public static CharacterInfo GetCharacter(int index)
        {
            for (int i = 0; i < CharacterInfoList.Count; i++)
                if (CharacterInfoList[i].Index == index)
                    return CharacterInfoList[i];

            return null;
        }

        public static void Broadcast(Packet p)
        {
            foreach (PlayerObject player in Players)
                player.Enqueue(p);
        }
        public static S.Rankings GetRanks(C.RankRequest p, bool isGM)
        {
            S.Rankings result = new S.Rankings
            {
                AllowObservation = Config.AllowObservation,
                OnlineOnly = p.OnlineOnly,
                StartIndex = p.StartIndex,
                Class = p.Class,
                Ranks = new List<RankInfo>(),
                ObserverPacket = false,
            };

            int total = 0;
            int rank = 0;
            bool reset = false;

            if (Now > NextRankChangeReset)
            {
                reset = true;
                NextRankChangeReset = Now + Config.RankChangeResetDelay;
            }

            foreach (CharacterInfo info in Rankings)
            {
                if (info.Deleted) continue;

                if (reset)
                {
                    info.RankChange = new();
                }

                switch (info.Class)
                {
                    case MirClass.Warrior:
                        if ((p.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((p.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((p.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((p.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                rank++;

                info.CurrentRank[p.Class] = rank;

                if (!info.RankChange.ContainsKey(p.Class))
                {
                    info.RankChange[p.Class] = 0;
                }

                if (p.OnlineOnly && info.Player == null) continue;

                if (total++ < p.StartIndex || result.Ranks.Count > 20) continue;


                result.Ranks.Add(new RankInfo
                {
                    Rank = rank,
                    Index = info.Index,
                    Class = info.Class,
                    Experience = info.Experience,
                    MaxExperience = info.Level >= Globals.ExperienceList.Count ? 0 : Globals.ExperienceList[info.Level],
                    Level = info.Level,
                    Name = info.CharacterName,
                    Online = info.Player != null,
                    Observable = info.Observable || isGM,
                    Rebirth = info.Rebirth,
                    RankChange = info.RankChange[p.Class]
                });
            }

            result.Total = total;

            return result;
        }

        public static Map GetMap(MapInfo info, InstanceInfo instance = null, byte instanceSequence = 0)
        {
            if (instance == null)
            {
                return info != null && Maps.ContainsKey(info) ? Maps[info] : null;
            }

            var instanceMaps = Instances[instance];

            if (instanceSequence >= instanceMaps.Length || instanceMaps[instanceSequence] == null)
            {
                return null;
            }

            return instanceMaps != null && instanceMaps[instanceSequence].ContainsKey(info) ? instanceMaps[instanceSequence][info] : null;
        }

        public static byte? LoadInstance(InstanceInfo instance, byte instanceSequence)
        {
            var mapInstance = Instances[instance];

            mapInstance[instanceSequence] = new Dictionary<MapInfo, Map>();

            for (int i = 0; i < instance.Maps.Count; i++)
            {
                mapInstance[instanceSequence][instance.Maps[i].Map] = new Map(instance.Maps[i].Map, instance, instanceSequence, instance.Maps[i].RespawnIndex);
            }

            Parallel.ForEach(mapInstance[instanceSequence], x => x.Value.Load());

            foreach (Map map in mapInstance[instanceSequence].Values)
            {
                map.Setup();
            }

            CreateSafeZones(instance, instanceSequence);

            CreateMovements(instance, instanceSequence);

            CreateNPCs(instance, instanceSequence);

            CreateSpawns(instance, instanceSequence);

            CreateQuestRegions(instance, instanceSequence);

            ServerLogger.Log($"Loaded Instance {instance.Name} at index {instanceSequence}");

            return instanceSequence;
        }

        public static void UnloadInstance(InstanceInfo instance, byte instanceSequence)
        {
            if (Instances[instance][instanceSequence] == null) return;

            foreach (KeyValuePair<MapInfo, Map> pair in Instances[instance][instanceSequence])
            {
                var map = pair.Value;

                for (int i = map.Players.Count - 1; i >= 0; i--)
                {
                    if (instance.ReconnectRegion != null && map.Players[i].Teleport(instance.ReconnectRegion, null, 0))
                    {
                        continue;
                    }

                    if (map.Info.ReconnectMap != null)
                    {
                        var reconnectMap = GetMap(map.Info.ReconnectMap);
                        if (map.Players[i].Teleport(reconnectMap, reconnectMap.GetRandomLocation()))
                        {
                            continue;
                        }
                    }

                    if (map.Players[i].Teleport(map.Players[i].Character.BindPoint.BindRegion, null, 0))
                    {
                        continue;
                    }
                }
            }

            RemoveSpawns(instance, instanceSequence);

            EventLogs.RemoveAll(x => x.InstanceInfo == instance && x.InstanceSequence == instanceSequence);

            Instances[instance][instanceSequence] = null;

            var users = new List<string>();

            foreach (var pair in instance.UserRecord)
            {
                if (pair.Value == instanceSequence)
                    users.Add(pair.Key);
            }

            foreach (var user in users)
            {
                instance.UserRecord.Remove(user);

                if (instance.CooldownTimeInMinutes > 0)
                {
                    var cooldown = SEnvir.Now.AddMinutes(instance.CooldownTimeInMinutes);

                    switch (instance.Type)
                    {
                        case InstanceType.Guild:
                            var character = GetCharacter(user);
                            var guildName = character.Account?.GuildMember?.Guild.GuildName;
                            if (guildName != null && !instance.GuildCooldown.ContainsKey(guildName))
                                instance.GuildCooldown.Add(guildName, cooldown);
                            break;
                        default:
                            if (!instance.UserCooldown.ContainsKey(user))
                                instance.UserCooldown.Add(user, cooldown);
                            break;
                    }
                }
            }

            ServerLogger.Log($"Unloaded Instance {instance.Name} at index {instanceSequence} and removed {users.Count} user records");
        }

        public static UserConquestStats GetConquestStats(PlayerObject player)
        {
            foreach (ConquestWar war in ConquestWars)
            {
                if (war.Map != player.CurrentMap) continue;

                return war.GetStat(player.Character);
            }

            return null;
        }
    }

    public class WebCommand
    {
        public CommandType Command { get; set; }
        public AccountInfo Account { get; set; }

        public WebCommand(CommandType command, AccountInfo account)
        {
            Command = command;
            Account = account;
        }
    }

    public enum CommandType
    {
        None,

        Activation,
        PasswordReset,
        AccountDelete
    }

    public sealed class IPNMessage
    {
        public string Message { get; set; }
        public bool Verified { get; set; } //Ensures Paypal sent it
        public string FileName { get; set; }
        public bool Duplicate { get; set; }
    }
}

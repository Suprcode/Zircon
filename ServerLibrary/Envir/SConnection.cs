using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir.Translations;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using C = Library.Network.ClientPackets;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;

namespace Server.Envir
{
    public sealed class SConnection : BaseConnection
    {
        private static int SessionCount;

        protected override TimeSpan TimeOutDelay => Config.TimeOut;

        private DateTime PingTime;
        private bool PingSent;
        public int Ping { get; private set; }

        public GameStage Stage { get; set; }
        public AccountInfo Account { get; set; }
        public PlayerObject Player { get; set; }
        public string IPAddress { get; }
        public int SessionID { get; }

        public SConnection Observed;
        public List<SConnection> Observers = new List<SConnection>();

        public List<AuctionInfo> MPSearchResults = new List<AuctionInfo>();
        public HashSet<AuctionInfo> VisibleResults = new HashSet<AuctionInfo>();

        public StringMessages Language;

        public SConnection(TcpClient client) : base(client)
        {
            IPAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];
            SessionID = ++SessionCount;

            Language = (StringMessages)ConfigReader.ConfigObjects[typeof(EnglishMessages)];

            OnException += (o, e) =>
            {
                SEnvir.Log(string.Format("Crashed: Account: {0}, Character: {1}.", Account?.EMailAddress, Player?.Name));
                SEnvir.Log(e.ToString());
                SEnvir.Log(e.StackTrace.ToString());
                File.AppendAllText(@".\Errors.txt", e.StackTrace + Environment.NewLine);
            };

            SEnvir.Log(string.Format("[Connection] IP Address:{0}", IPAddress));

            UpdateTimeOut();
            BeginReceive();

            Enqueue(new G.Connected());
        }

        public override void Disconnect()
        {
            if (!Connected) return;

            base.Disconnect();

            CleanUp();

            if (!SEnvir.Connections.Contains(this))
                throw new InvalidOperationException("Connection was not found in list");

            SEnvir.Connections.Remove(this);
            SEnvir.IPCount[IPAddress]--;
            SEnvir.DBytesSent += TotalBytesSent;
            SEnvir.DBytesReceived += TotalBytesReceived;
        }

        public override void SendDisconnect(Packet p)
        {
            base.SendDisconnect(p);

            CleanUp();
        }
        public override void TryDisconnect()
        {
            if (Stage == GameStage.Game)
            {
                if (SEnvir.Now >= Player.CombatTime.AddSeconds(10))
                {
                    Disconnect();
                    return;
                }

                if (!Disconnecting)
                {
                    Disconnecting = true;
                    TimeOutTime = Time.Now.AddSeconds(10);
                }

                if (SEnvir.Now <= TimeOutTime) return;
            }

            Disconnect();
        }
        public override void TrySendDisconnect(Packet p)
        {
            if (Stage == GameStage.Game)
            {
                if (SEnvir.Now >= Player.CombatTime.AddSeconds(10))
                {
                    Disconnect();
                    return;
                }

                if (!Disconnecting)
                {
                    base.SendDisconnect(p);

                    TimeOutTime = Time.Now.AddSeconds(10);
                }

                if (SEnvir.Now <= TimeOutTime) return;

            }

            SendDisconnect(p);
        }

        public void EndObservation()
        {
            Observed.Observers.Remove(this);
            Observed = null;

            if (Account != null)
            {
                Stage = GameStage.Select;
                Enqueue(new S.GameLogout { Characters = Account.GetSelectInfo() });
            }
            else
            {
                Stage = GameStage.Login;
                Enqueue(new S.SelectLogout());
            }
        }
        public void CleanUp()
        {
            Stage = GameStage.Disconnected;

            if (Account != null && Account.Connection == this)
            {
                Account.TempAdmin = false;
                Account.Connection = null;
            }

            Account = null;
            Player?.StopGame();
            Player = null;

            Observed?.Observers.Remove(this);
            Observed = null;

            //   ItemList.Clear();
            //    MagicList.Clear();
        }

        public override void Process()
        {
            if (SEnvir.Now >= PingTime && !PingSent && Stage != GameStage.None)
            {
                PingTime = SEnvir.Now;
                PingSent = true;
                Enqueue(new G.Ping { ObserverPacket = false });
            }

            if (TotalPacketsProcessed == 0 && TotalBytesReceived > 1024)
            {
                TryDisconnect();
                SEnvir.IPBlocks[IPAddress] = SEnvir.Now.Add(Config.PacketBanTime);

                for (int i = SEnvir.Connections.Count - 1; i >= 0; i--)
                    if (SEnvir.Connections[i].IPAddress == IPAddress)
                        SEnvir.Connections[i].TryDisconnect();

                SEnvir.Log($"{IPAddress} Disconnected, Large Packet");
                return;
            }
            
            if (ReceiveList.Count > Config.MaxPacket)
            {
                TryDisconnect();
                SEnvir.IPBlocks[IPAddress] = SEnvir.Now.Add(Config.PacketBanTime);

                for (int i = SEnvir.Connections.Count - 1; i >= 0; i--)
                    if (SEnvir.Connections[i].IPAddress == IPAddress)
                        SEnvir.Connections[i].TryDisconnect();

                SEnvir.Log($"{IPAddress} Disconnected, Large amount of Packets");
                return;
            }

            base.Process();
        }

        public override void Enqueue(Packet p)
        {
            base.Enqueue(p);

            if (p == null || !p.ObserverPacket) return;

            foreach (SConnection observer in Observers)
                observer.Enqueue(p);
        }

        public void ReceiveChat(string text, MessageType type, List<ClientUserItem> linkedItems = null, uint objectID = 0)
        {
            switch (Stage)
            {
                case GameStage.Game:
                case GameStage.Observer:
                    Enqueue(new S.Chat
                    {
                        Text = text,
                        Type = type,
                        ObjectID = objectID, // && type != guild

                        LinkedItems = linkedItems,
                        ObserverPacket = false,
                    });
                    break;
                default:
                    return;
            }
        }

        public void ReceiveChatWithObservers(Func<SConnection, string> messageFunc, MessageType messageType, List<ClientUserItem> linkedItems = null, uint objectID = 0)
        {
            ReceiveChat(messageFunc(this), messageType, linkedItems, objectID);

            foreach (SConnection observer in Observers)
                observer.ReceiveChat(messageFunc(observer), messageType, linkedItems, objectID);
        }

        public void Process(C.SelectLanguage p)
        {
            switch (p.Language.ToUpper())
            {
                case "ENGLISH":
                    Language = (StringMessages)ConfigReader.ConfigObjects[typeof(EnglishMessages)];
                    break;
                case "CHINESE":
                    Language = (StringMessages)ConfigReader.ConfigObjects[typeof(ChineseMessages)];
                    break;
            }

        }
        public void Process(G.Disconnect p)
        {
            Disconnecting = true;
        }
        public void Process(G.Connected p)
        {
            if (Config.CheckVersion)
            {
                Enqueue(new G.CheckVersion());
                return;
            }

            Stage = GameStage.Login;
            Enqueue(new G.GoodVersion() { DatabaseKey = Config.EncryptionEnabled ? SEnvir.CryptoKey : null });
        }
        public void Process(G.Version p)
        {
            if (Stage != GameStage.None) return;

            if (!Functions.IsMatch(Config.ClientHash, p.ClientHash))
            {
                SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongVersion });
                return;
            }

            Stage = GameStage.Login;
            Enqueue(new G.GoodVersion() { DatabaseKey = Config.EncryptionEnabled ? SEnvir.CryptoKey : null });
        }
        public void Process(G.Ping p)
        {
            if (Stage == GameStage.None) return;

            int ping = (int)(SEnvir.Now - PingTime).TotalMilliseconds / 2;
            PingSent = false;
            PingTime = SEnvir.Now + Config.PingDelay;

            Ping = ping;
            Enqueue(new G.PingResponse { Ping = Ping, ObserverPacket = false });
        }

        public void Process(C.NewAccount p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.NewAccount(p, this);
        }
        public void Process(C.ChangePassword p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.ChangePassword(p, this);
        }
        public void Process(C.RequestPasswordReset p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.RequestPasswordReset(p, this);
        }
        public void Process(C.ResetPassword p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.ResetPassword(p, this);
        }
        public void Process(C.Activation p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.Activation(p, this);
        }
        public void Process(C.RequestActivationKey p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.RequestActivationKey(p, this);
        }
        public void Process(C.Login p)
        {
            if (Stage != GameStage.Login) return;

            SEnvir.Login(p, this);
        }
        public void Process(C.Logout p)
        {

            switch (Stage)
            {
                case GameStage.Select:
                    Stage = GameStage.Login;
                    Account.Connection = null;
                    Account = null;

                    Enqueue(new S.SelectLogout());
                    break;
                case GameStage.Game:

                    if (SEnvir.Now < Player.CombatTime.AddSeconds(10)) return;

                    Player.StopGame();

                    Stage = GameStage.Select;

                    Enqueue(new S.GameLogout { Characters = Account.GetSelectInfo() });
                    break;
                case GameStage.Observer:
                    EndObservation();
                    break;
            }
            ;

        }

        public void Process(C.NewCharacter p)
        {
            if (Stage != GameStage.Select) return;

            SEnvir.NewCharacter(p, this);
        }
        public void Process(C.DeleteCharacter p)
        {
            if (Stage != GameStage.Select) return;

            SEnvir.DeleteCharacter(p, this);
        }
        public void Process(C.StartGame p)
        {
            if (Stage != GameStage.Select) return;


            SEnvir.StartGame(p, this);
        }
        public void Process(C.TownRevive p)
        {
            if (Stage != GameStage.Game) return;

            Player.TownRevive();
        }
        public void Process(C.Turn p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Turn(p.Direction);
        }
        public void Process(C.Harvest p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Harvest(p.Direction);
        }
        public void Process(C.Move p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            /*  if (p.Distance > 1 && (Player.BagWeight > Player.Stats[Stat.BagWeight] || Player.WearWeight > Player.Stats[Stat.WearWeight]))
              {
                  Enqueue(new S.UserLocation { Direction = Player.Direction, Location = Player.CurrentLocation });
                  return;
              }*/

            Player.Move(p.Direction, p.Distance);
        }
        public void Process(C.Mount p)
        {
            if (Stage != GameStage.Game) return;

            Player.Mount();
        }
        public void Process(C.FishingCast p)
        {
            if (Stage != GameStage.Game) return;

            Player.FishingCast(p.State, p.Direction, p.FloatLocation, p.CaughtFish);
        }

        public void Process(C.Attack p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Attack(p.Direction, p.AttackMagic);
        }

        public void Process(C.RangeAttack p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.RangeAttack(p.Direction, p.DelayedTime, p.Target);
        }
        public void Process(C.Magic p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Magic(p);
        }
        public void Process(C.MagicToggle p)
        {
            if (Stage != GameStage.Game) return;

            Player.MagicToggle(p);
        }
        public void Process(C.Mining p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Mining(p.Direction);
        }

        public void Process(C.ItemMove p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemMove(p);
        }
        public void Process(C.ItemSort p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemSort(p);
        }
        public void Process(C.ItemDelete p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemDelete(p);
        }
        public void Process(C.ItemDrop p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemDrop(p);
        }
        public void Process(C.PickUp p)
        {
            if (Stage != GameStage.Game) return;

            Player.PickUp();
        }
        public void Process(C.CurrencyDrop p)
        {
            if (Stage != GameStage.Game) return;

            Player.CurrencyDrop(p);
        }
        public void Process(C.ItemUse p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemUse(p.Link);
        }

        public void Process(C.BeltLinkChanged p)
        {
            if (Stage != GameStage.Game) return;

            Player.BeltLinkChanged(p);
        }
        public void Process(C.AutoPotionLinkChanged p)
        {
            if (Stage != GameStage.Game) return;

            Player.AutoPotionLinkChanged(p);
        }

        public void Process(C.Chat p)
        {
            if (p.Text.Length > Globals.MaxChatLength) return;

            if (Stage == GameStage.Game)
                Player.Chat(p.Text);

            if (Stage == GameStage.Observer)
                Observed.Player.ObserverChat(this, p.Text);
        }

        public void Process(C.NPCCall p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCCall(p.ObjectID);
        }

        public void Process(C.NPCButton p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCButton(p.ButtonID);
        }

        public void Process(C.NPCRoll p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRoll(p.Type);
        }
        public void Process(C.NPCRollResult p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRollResult();
        }

        public void Process(C.NPCBuy p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCBuy(p);
        }
        public void Process(C.NPCSell p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCSell(p.Links);
        }
        public void Process(C.NPCRepair p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRepair(p);
        }
        public void Process(C.NPCRefinementStone p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefinementStone(p);
        }
        public void Process(C.NPCRefine p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefine(p);
        }
        public void Process(C.NPCRefineRetrieve p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefineRetrieve(p.Index);
        }
        public void Process(C.NPCMasterRefine p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCMasterRefine(p);
        }
        public void Process(C.NPCMasterRefineEvaluate p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCMasterRefineEvaluate(p);
        }
        public void Process(C.NPCClose p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPC = null;
            Player.NPCPage = null;

            foreach (SConnection con in Observers)
            {
                con.Enqueue(new S.NPCClose());
            }
        }
        public void Process(C.NPCFragment p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCFragment(p.Links);
        }
        public void Process(C.NPCAccessoryLevelUp p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryLevelUp(p);
        }
        public void Process(C.NPCAccessoryUpgrade p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryUpgrade(p);
        }

        public void Process(C.MagicKey p)
        {
            if (Stage != GameStage.Game) return;

            foreach (var type in Player.MagicObjects.Keys)
            {
                var magicObject = Player.MagicObjects[type];

                if (magicObject.Magic.Set1Key == p.Set1Key)
                    magicObject.Magic.Set1Key = SpellKey.None;

                if (magicObject.Magic.Set2Key == p.Set2Key)
                    magicObject.Magic.Set2Key = SpellKey.None;

                if (magicObject.Magic.Set3Key == p.Set3Key)
                    magicObject.Magic.Set3Key = SpellKey.None;

                if (magicObject.Magic.Set4Key == p.Set4Key)
                    magicObject.Magic.Set4Key = SpellKey.None;
            }

            if (!Player.GetMagic(p.Magic, out MagicObject magic)) return;

            magic.Magic.Set1Key = p.Set1Key;
            magic.Magic.Set2Key = p.Set2Key;
            magic.Magic.Set3Key = p.Set3Key;
            magic.Magic.Set4Key = p.Set4Key;
        }

        public void Process(C.GroupSwitch p)
        {
            if (Stage != GameStage.Game) return;

            Player.GroupSwitch(p.Allow);
        }
        public void Process(C.GroupInvite p)
        {
            if (Stage != GameStage.Game) return;

            Player.GroupInvite(p.Name);
        }
        public void Process(C.GroupRemove p)
        {
            if (Stage != GameStage.Game) return;

            Player.GroupRemove(p.Name);
        }
        public void Process(C.GroupResponse p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.GroupJoin();

            Player.GroupInvitation = null;
        }

        public void Process(C.Inspect p)
        {
            if (Stage == GameStage.Game)
                Player.Inspect(p.Index, p.Ranking, this);

            if (Stage == GameStage.Observer)
                Observed.Player.Inspect(p.Index, p.Ranking, this);
        }
        public void Process(C.RankRequest p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer && Stage != GameStage.Login) return;

            Enqueue(SEnvir.GetRanks(p, Account != null && (Account.TempAdmin || Account.Observer)));
        }

        public void Process(C.RankSearch p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer && Stage != GameStage.Login) return;

            bool isGM = Account != null && (Account.TempAdmin || Account.Observer);

            RankInfo rank = null;

            CharacterInfo info = SEnvir.GetCharacter(p.Name);

            if (info != null)
            {
                info.CurrentRank.TryGetValue(RequiredClass.All, out int currentRank);
                info.RankChange.TryGetValue(RequiredClass.All, out int positionChange);

                rank = new RankInfo
                {
                    Rank = 0,//currentRank,
                    Index = info.Index,
                    Class = info.Class,
                    Experience = info.Experience,
                    MaxExperience = info.Level >= Globals.ExperienceList.Count ? 0 : Globals.ExperienceList[info.Level],
                    Level = info.Level,
                    Name = info.CharacterName,
                    Online = info.Player != null,
                    Observable = info.Observable || isGM,
                    Rebirth = info.Rebirth,
                    RankChange = positionChange
                };
            }

            Enqueue(new S.RankSearch { Rank = rank });
        }

        public void Process(C.ObserverRequest p)
        {
            if (!Config.AllowObservation && (Account == null || (!Account.TempAdmin && !Account.Observer))) return;

            PlayerObject player = SEnvir.GetPlayerByCharacter(p.Name);

            if (player == null || player == Player) return;

            if (!player.Character.Observable && (Account == null || (!Account.TempAdmin && !Account.Observer))) return;

            if (Stage == GameStage.Game)
                Player.StopGame();

            if (Stage == GameStage.Observer)
            {
                Observed.Observers.Remove(this);
                Observed = null;
            }

            player.SetUpObserver(this);
        }
        public void Process(C.ObservableSwitch p)
        {
            if (Stage != GameStage.Game) return;

            Player.ObservableSwitch(p.Allow);
        }

        public void Process(C.Hermit p)
        {
            if (Stage != GameStage.Game) return;

            Player.AssignHermit(p.Stat);
        }

        public void Process(C.MarketPlaceHistory p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;


            S.MarketPlaceHistory result = new S.MarketPlaceHistory { Index = p.Index, Display = p.Display, ObserverPacket = false };
            Enqueue(result);

            AuctionHistoryInfo info = SEnvir.AuctionHistoryInfoList.Binding.FirstOrDefault(x => x.Info == p.Index && x.PartIndex == p.PartIndex);

            if (info == null) return;

            result.SaleCount = info.SaleCount;
            result.LastPrice = info.LastPrice;

            long average = 0;
            int count = 0;

            foreach (int value in info.Average)
            {
                if (value == 0) break;

                average += value;
                count++;
            }

            if (count == 0) return;
            result.AveragePrice = average / count;
        }
        public void Process(C.MarketPlaceConsign p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceConsign(p);
        }

        public void Process(C.MarketPlaceSearch p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            MPSearchResults.Clear();
            VisibleResults.Clear();

            HashSet<int> matches = new HashSet<int>();

            foreach (ItemInfo info in SEnvir.ItemInfoList.Binding)
            {
                try
                {
                    if (!string.IsNullOrEmpty(p.Name) && info.ItemName.IndexOf(p.Name, StringComparison.OrdinalIgnoreCase) < 0) continue;

                    matches.Add(info.Index);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            foreach (AuctionInfo info in SEnvir.AuctionInfoList.Binding)
            {
                if (info.Item == null) continue;

                if (p.ItemTypeFilter && info.Item.Info.ItemType != p.ItemType) continue;

                switch (info.Item.Info.ItemEffect)
                {
                    case ItemEffect.ItemPart:
                        if (!matches.Contains(info.Item.Stats[Stat.ItemIndex])) continue;
                        break;
                    default:
                        if (!matches.Contains(info.Item.Info.Index)) continue;
                        break;
                }

                MPSearchResults.Add(info);
            }

            switch (p.Sort)
            {
                case MarketPlaceSort.Newest:
                    MPSearchResults.Sort((x1, x2) => x2.Index.CompareTo(x1.Index));
                    break;
                case MarketPlaceSort.Oldest:
                    MPSearchResults.Sort((x1, x2) => x1.Index.CompareTo(x2.Index));
                    break;
                case MarketPlaceSort.HighestPrice:
                    MPSearchResults.Sort((x1, x2) => x2.Price.CompareTo(x1.Price));
                    break;
                case MarketPlaceSort.LowestPrice:
                    MPSearchResults.Sort((x1, x2) => x1.Price.CompareTo(x2.Price));
                    break;
            }

            //Send Rows 1 ~ 9
            List<ClientMarketPlaceInfo> results = new List<ClientMarketPlaceInfo>();

            foreach (AuctionInfo info in MPSearchResults)
            {
                if (results.Count >= 9) break;

                results.Add(info.ToClientInfo(Account));
                VisibleResults.Add(info);
            }


            Enqueue(new S.MarketPlaceSearch { Count = MPSearchResults.Count, Results = results, ObserverPacket = false });
        }
        public void Process(C.MarketPlaceSearchIndex p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            if (p.Index < 0 || p.Index >= MPSearchResults.Count) return;


            AuctionInfo info = MPSearchResults[p.Index];

            if (VisibleResults.Contains(info)) return;

            VisibleResults.Add(info);

            Enqueue(new S.MarketPlaceSearchIndex { Index = p.Index, Result = info.ToClientInfo(Account), ObserverPacket = false });
        }
        public void Process(C.MarketPlaceCancelConsign p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceCancelConsign(p);
        }
        public void Process(C.MarketPlaceBuy p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceBuy(p);
        }
        public void Process(C.MarketPlaceStoreBuy p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceStoreBuy(p);
        }

        public void Process(C.MailOpened p)
        {
            if (Stage != GameStage.Game) return;

            MailInfo mail = Account.Mail.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            mail.Opened = true;
        }
        public void Process(C.MailGetItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.MailGetItem(p);
        }
        public void Process(C.MailDelete p)
        {
            if (Stage != GameStage.Game) return;

            Player.MailDelete(p.Index);
        }
        public void Process(C.MailSend p)
        {
            if (Stage != GameStage.Game) return;

            Player.MailSend(p);
        }

        public void Process(C.ChangeAttackMode p)
        {
            if (Stage != GameStage.Game) return;

            switch (p.Mode)
            {
                case AttackMode.Peace:
                case AttackMode.Group:
                case AttackMode.Guild:
                case AttackMode.WarRedBrown:
                case AttackMode.All:
                    Player.AttackMode = p.Mode;
                    Enqueue(new S.ChangeAttackMode { Mode = p.Mode });
                    break;
            }
        }
        public void Process(C.ChangePetMode p)
        {
            if (Stage != GameStage.Game) return;

            switch (p.Mode)
            {
                case PetMode.Both:
                case PetMode.Move:
                case PetMode.Attack:
                case PetMode.PvP:
                case PetMode.None:
                    Player.PetMode = p.Mode;
                    Enqueue(new S.ChangePetMode { Mode = p.Mode });
                    break;
            }
        }

        public void Process(C.ItemSplit p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemSplit(p);
        }
        public void Process(C.ItemLock p)
        {
            if (Stage != GameStage.Game) return;

            Player.ItemLock(p);
        }


        public void Process(C.TradeRequest p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeRequest();
        }
        public void Process(C.TradeRequestResponse p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.TradeAccept();

            Player.TradePartnerRequest = null;
        }
        public void Process(C.TradeClose p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeClose();
        }
        public void Process(C.TradeAddItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeAddItem(p.Cell);
        }
        public void Process(C.TradeAddGold p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeAddGold(p.Gold);
        }
        public void Process(C.TradeConfirm p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeConfirm();
        }

        public void Process(C.GuildCreate p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildCreate(p);
        }
        public void Process(C.GuildEditNotice p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildEditNotice(p);
        }
        public void Process(C.GuildEditMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildEditMember(p);
        }
        public void Process(C.GuildTax p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildTax(p);
        }
        public void Process(C.GuildIncreaseMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildIncreaseMember(p);
        }
        public void Process(C.GuildIncreaseStorage p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildIncreaseStorage(p);
        }
        public void Process(C.GuildInviteMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildInviteMember(p);
        }
        public void Process(C.GuildKickMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildKickMember(p);
        }
        public void Process(C.GuildResponse p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.GuildJoin();

            Player.GuildInvitation = null;
        }
        public void Process(C.GuildWar p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildWar(p.GuildName);
        }
        public void Process(C.GuildRequestConquest p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildConquest(p.Index);
        }

        public void Process(C.GuildColour p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildColour(p.Colour);
        }
        public void Process(C.GuildFlag p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildFlag(p.Flag);
        }
        public void Process(C.GuildToggleCastleGates p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildToggleCastleGates();
        }
        public void Process(C.GuildRepairCastleGates p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildRepairCastleGates();
        }
        public void Process(C.GuildRepairCastleGuards p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildRepairCastleGuards();
        }

        public void Process(C.QuestAccept p)
        {
            if (Stage != GameStage.Game) return;

            Player.QuestAccept(p.Index);
        }
        public void Process(C.QuestComplete p)
        {
            if (Stage != GameStage.Game) return;

            Player.QuestComplete(p);
        }
        public void Process(C.QuestTrack p)
        {
            if (Stage != GameStage.Game) return;

            Player.QuestTrack(p);
        }
        public void Process(C.QuestAbandon p)
        {
            if (Stage != GameStage.Game) return;

            Player.QuestAbandon(p);
        }

        public void Process(C.CompanionUnlock p)
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionUnlock(p.Index);
        }
        public void Process(C.CompanionAdopt p)
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionAdopt(p);
        }
        public void Process(C.CompanionRetrieve p)
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionRetrieve(p.Index);
        }

        public void Process(C.CompanionStore p)
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionStore(p.Index);
        }

        public void Process(C.MarriageResponse p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.MarriageJoin();

            Player.MarriageInvitation = null;
        }
        public void Process(C.MarriageMakeRing p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageMakeRing(p.Slot);

        }
        public void Process(C.MarriageTeleport p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageTeleport();
        }

        public void Process(C.BlockAdd p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            CharacterInfo info = SEnvir.GetCharacter(p.Name);

            if (info == null)
            {
                ReceiveChat(string.Format(Language.CannotFindPlayer, p.Name), MessageType.System);
                return;
            }

            foreach (BlockInfo blockInfo in Account.BlockingList)
            {
                if (blockInfo.BlockedAccount == info.Account)
                {
                    ReceiveChat(string.Format(Language.AlreadyBlocked, p.Name), MessageType.System);
                    return;
                }
            }

            BlockInfo block = SEnvir.BlockInfoList.CreateNewObject();

            block.Account = Account;
            block.BlockedAccount = info.Account;
            block.BlockedName = info.CharacterName;

            Enqueue(new S.BlockAdd { Info = block.ToClientInfo(), ObserverPacket = false });
        }
        public void Process(C.BlockRemove p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            BlockInfo block = Account?.BlockingList.FirstOrDefault(x => x.Index == p.Index);

            if (block == null) return;

            block.Delete();

            Enqueue(new S.BlockRemove { Index = p.Index, ObserverPacket = false });
        }

        public void Process(C.HelmetToggle p)
        {
            if (Stage != GameStage.Game) return;

            Player.HelmetToggle(p.HideHelmet);
        }

        public void Process(C.GenderChange p)
        {
            if (Stage != GameStage.Game) return;

            Player.GenderChange(p);
        }

        public void Process(C.HairChange p)
        {
            if (Stage != GameStage.Game) return;

            Player.HairChange(p);
        }

        public void Process(C.ArmourDye p)
        {
            if (Stage != GameStage.Game) return;

            Player.ArmourDye(p.ArmourColour);
        }
        public void Process(C.NameChange p)
        {
            if (Stage != GameStage.Game) return;

            Player.NameChange(p.Name);
        }

        public void Process(C.CaptionChange p)
        {
            if (Stage != GameStage.Game) return;

            Player.CaptionChange(p.Caption);
        }

        public void Process(C.FortuneCheck p)
        {
            if (Stage != GameStage.Game) return;

            Player.FortuneCheck(p.ItemIndex);
        }

        public void Process(C.TeleportRing p)
        {
            if (Stage != GameStage.Game) return;

            Player.TeleportRing(p.Location, p.Index);
        }

        public void Process(C.JoinStarterGuild p)
        {
            if (Stage != GameStage.Game) return;

            Player.JoinStarterGuild();
        }
        public void Process(C.NPCAccessoryReset p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryReset(p);
        }

        public void Process(C.NPCWeaponCraft p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCWeaponCraft(p);
        }

        public void Process(C.NPCAccessoryRefine p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryRefine(p);
        }

        public void Process(C.JoinInstance p)
        {
            if (Stage != GameStage.Game) return;

            Player.JoinInstance(p);
        }

        public void Process(C.SendCompanionFilters p)
        {
            if (Stage != GameStage.Game) return;

            Player.SetFilters(p);
        }

        public void Process(C.ChangeOnlineState p)
        {
            if (Stage != GameStage.Game) return;

            Player.OnlineState = p.State;

            Player.UpdateOnlineState();
        }

        public void Process(C.FriendAdd p)
        {
            if (Stage != GameStage.Game) return;

            CharacterInfo info = SEnvir.GetCharacter(p.Name);

            if (info == null)
            {
                ReceiveChat(string.Format(Language.CannotFindPlayer, p.Name), MessageType.System);
                return;
            }

            foreach (FriendInfo friendInfo in Player.Character.Friends)
            {
                if (friendInfo.FriendedCharacter == info)
                {
                    ReceiveChat(string.Format(Language.AlreadyFriended, p.Name), MessageType.System);
                    return;
                }
            }

            FriendInfo friend = SEnvir.FriendInfoList.CreateNewObject();

            friend.Character = Player.Character;
            friend.FriendedCharacter = info;
            friend.FriendName = info.CharacterName;

            Enqueue(new S.FriendAdd { Info = friend.ToClientInfo(), ObserverPacket = false });
        }

        public void Process(C.FriendRemove p)
        {
            if (Stage != GameStage.Game) return;

            FriendInfo friend = Player.Character.Friends.FirstOrDefault(x => x.Index == p.Index);

            if (friend == null) return;

            friend.Delete();

            Enqueue(new S.FriendRemove { Index = p.Index, ObserverPacket = false });
        }

        public void Process(C.IncreaseDiscipline p)
        {
            if (Stage != GameStage.Game) return;

            Player.IncreaseDiscipline();
        }
    }

    public enum GameStage
    {
        None,
        Login,
        Select,
        Game,
        Observer,
        Disconnected,
    }
}

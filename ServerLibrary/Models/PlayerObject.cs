﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using S = Library.Network.ServerPackets;
using C = Library.Network.ClientPackets;

namespace Server.Models
{
    public sealed class PlayerObject : MapObject
    {
        public override ObjectType Race => ObjectType.Player;

        public CharacterInfo Character;
        public SConnection Connection;

        public override string Name
        {
            get { return Character.CharacterName; }
            set { Character.CharacterName = value; }
        }
        public override int Level
        {
            get { return Character.Level; }
            set { Character.Level = value; }
        }
        public override Point CurrentLocation
        {
            get { return Character.CurrentLocation; }
            set { Character.CurrentLocation = value; }
        }

        public MirGender Gender => Character.Gender;
        public MirClass Class => Character.Class;

        public override int CurrentHP
        {
            get { return Character.CurrentHP; }
            set { Character.CurrentHP = value; }
        }
        public override int CurrentMP
        {
            get { return Character.CurrentMP; }
            set { Character.CurrentMP = value; }
        }

        public AttackMode AttackMode
        {
            get { return Character.AttackMode; }
            set { Character.AttackMode = value; }
        }

        public PetMode PetMode
        {
            get { return Character.PetMode; }
            set { Character.PetMode = value; }
        }

        public long Gold
        {
            get { return Character.Account.Gold; }
            set { Character.Account.Gold = value; }
        }

        public decimal Experience
        {
            get { return Character.Experience; }
            set { Character.Experience = value; }
        }

        public int BagWeight, WearWeight, HandWeight;

        public int HairType
        {
            get { return Character.HairType; }
            set { Character.HairType = value; }
        }
        public Color HairColour
        {
            get { return Character.HairColour; }
            set { Character.HairColour = value; }
        }

        public override MirDirection Direction
        {
            get { return Character.Direction; }
            set { Character.Direction = value; }
        }

        public DateTime ShoutTime, UseItemTime, TorchTime, CombatTime, PvPTime, SentCombatTime, AutoPotionTime, AutoPotionCheckTime, ItemTime, FlamingSwordTime, DragonRiseTime, BladeStormTime, RevivalTime, TeleportTime;
        public bool PacketWaiting;

        public bool CanPowerAttack, GameMaster, Observer;

        public override bool Blocking => base.Blocking && !Observer;

        public NPCObject NPC;
        public NPCPage NPCPage;

        public HorseType Horse;

        public bool BlockWhisper;
        public bool CompanionLevelLock3, CompanionLevelLock5, CompanionLevelLock7, CompanionLevelLock10, CompanionLevelLock11, CompanionLevelLock13, CompanionLevelLock15;
        public bool ExtractorLock;

        public override bool CanAttack => base.CanAttack && Horse == HorseType.None;
        public override bool CanCast => base.CanCast && Horse == HorseType.None;


        public List<MonsterObject> Pets = new List<MonsterObject>();

        public HashSet<MapObject> VisibleObjects = new HashSet<MapObject>();
        public HashSet<MapObject> VisibleDataObjects = new HashSet<MapObject>();
        public HashSet<MonsterObject> TaggedMonsters = new HashSet<MonsterObject>();
        public HashSet<MapObject> NearByObjects = new HashSet<MapObject>();

        public UserItem[] Inventory = new UserItem[Globals.InventorySize],
            Equipment = new UserItem[Globals.EquipmentSize],
            Storage = new UserItem[1000],
            PartsStorage = new UserItem[1000];

        public Companion Companion;

        public MapObject LastHitter;

        public PlayerObject GroupInvitation,
            GuildInvitation, MarriageInvitation;

        public PlayerObject TradePartner, TradePartnerRequest;
        public Dictionary<UserItem, CellLinkInfo> TradeItems = new Dictionary<UserItem, CellLinkInfo>();
        public bool TradeConfirmed;
        public long TradeGold;

        public Dictionary<MagicType, UserMagic> Magics = new Dictionary<MagicType, UserMagic>();

        public List<AutoPotionLink> AutoPotions = new List<AutoPotionLink>();
        public CellLinkInfo DelayItemUse;
        public decimal MaxExperience;

        public bool CanFlamingSword, CanDragonRise, CanBladeStorm;

        public decimal SwiftBladeLifeSteal, FlameSplashLifeSteal, DestructiveSurgeLifeSteal;

        public string FiltersClass;
        public string FiltersRarity;
        public string FiltersItemType;

        public PlayerObject(CharacterInfo info, SConnection con)
        {
            Character = info;
            Connection = con;

            DisplayMP = CurrentMP;
            DisplayHP = CurrentHP;

            Character.LastStats = Stats = new Stats();

            foreach (UserItem item in Character.Account.Items)
                if (item.Slot >= 2000)
                {
                    PartsStorage[item.Slot - Globals.PartsStorageOffset] = item;
                }
                else
                    Storage[item.Slot] = item;

            foreach (UserItem item in Character.Items)
            {
                if (item.Slot >= Globals.EquipmentOffSet)
                {
                    Equipment[item.Slot - Globals.EquipmentOffSet] = item;
                    continue;
                }

                Inventory[item.Slot] = item;
            }

            ItemReviveTime = info.ItemReviveTime;
            ItemTime = SEnvir.Now;

            foreach (UserMagic magic in Character.Magics)
                Magics[magic.Info.Magic] = magic;

            Buffs.AddRange(Character.Account.Buffs);
            Buffs.AddRange(Character.Buffs);

            AutoPotions.AddRange(Character.AutoPotionLinks);

            AutoPotions.Sort((x1, x2) => x1.Slot.CompareTo(x2.Slot));

            if (Character.Account.Admin || Character.Account.TempAdmin)
            {
                GameMaster = true;
                Observer = true;
            }

            FiltersClass = Character.FiltersClass;
            FiltersItemType = Character.FiltersItemType;
            FiltersRarity = Character.FiltersRarity;
        }

        public override void Process()
        {
            base.Process();

            // if (LastHitter != null && LastHitter.Node == null) LastHitter = null;
            if (GroupInvitation != null && GroupInvitation.Node == null) GroupInvitation = null;
            if (GuildInvitation != null && GuildInvitation.Node == null) GuildInvitation = null;
            if (MarriageInvitation != null && MarriageInvitation.Node == null) MarriageInvitation = null;

            if (CombatTime != SentCombatTime)
            {
                SentCombatTime = CombatTime;
                Enqueue(new S.CombatTime());
            }

            ProcessRegen();

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>();

            foreach (MonsterObject ob in TaggedMonsters)
            {
                if (SEnvir.Now < ob.EXPOwnerTime) continue;
                clearList.Add(ob);
            }

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;

            if (CanFlamingSword && SEnvir.Now >= FlamingSwordTime)
            {
                CanFlamingSword = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.FlamingSword, CanUse = CanFlamingSword });

                Connection.ReceiveChat(string.Format(Connection.Language.ChargeExpire, Magics[MagicType.FlamingSword].Info.Name), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Magics[MagicType.FlamingSword].Info.Name), MessageType.System);
            }
            if (CanDragonRise && SEnvir.Now >= DragonRiseTime)
            {
                CanDragonRise = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.DragonRise, CanUse = CanDragonRise });

                Connection.ReceiveChat(string.Format(Connection.Language.ChargeExpire, Magics[MagicType.DragonRise].Info.Name), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Magics[MagicType.DragonRise].Info.Name), MessageType.System);
            }
            if (CanBladeStorm && SEnvir.Now >= BladeStormTime)
            {
                CanBladeStorm = false; ;
                Enqueue(new S.MagicToggle { Magic = MagicType.BladeStorm, CanUse = CanBladeStorm });

                Connection.ReceiveChat(string.Format(Connection.Language.ChargeExpire, Magics[MagicType.BladeStorm].Info.Name), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.ChargeExpire, Magics[MagicType.BladeStorm].Info.Name), MessageType.System);
            }

            if (Dead && SEnvir.Now >= RevivalTime)
                TownRevive();


            ProcessTorch();

            ProcessAutoPotion();

            ProcessItemExpire();
        }
        public override void ProcessAction(DelayedAction action)
        {
            MapObject ob;
            switch (action.Type)
            {
                case ActionType.Turn:
                    PacketWaiting = false;
                    Turn((MirDirection)action.Data[0]);
                    return;
                case ActionType.Harvest:
                    PacketWaiting = false;
                    Harvest((MirDirection)action.Data[0]);
                    return;
                case ActionType.Move:
                    PacketWaiting = false;
                    Move((MirDirection)action.Data[0], (int)action.Data[1]);
                    return;
                case ActionType.Magic:
                    PacketWaiting = false;
                    Magic((C.Magic)action.Data[0]);
                    return;
                case ActionType.Mining:
                    PacketWaiting = false;
                    Mining((MirDirection)action.Data[0]);
                    return;
                case ActionType.Attack:
                    PacketWaiting = false;
                    Attack((MirDirection)action.Data[0], (MagicType)action.Data[1]);
                    return;
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0], (List<UserMagic>)action.Data[1], (bool)action.Data[2], (int)action.Data[3]);
                    return;
                case ActionType.DelayMagic:
                    CompleteMagic(action.Data);
                    return;
                case ActionType.DelayedAttackDamage:
                    ob = (MapObject)action.Data[0];

                    if (!CanAttackTarget(ob)) return;

                    ob.Attacked(this, (int)action.Data[1], (Element)action.Data[2], (bool)action.Data[3], (bool)action.Data[4], (bool)action.Data[5], (bool)action.Data[6]);
                    return;
                case ActionType.DelayedMagicDamage:
                    ob = (MapObject)action.Data[1];

                    if (!CanAttackTarget(ob)) return;

                    MagicAttack((List<UserMagic>)action.Data[0], ob, (bool)action.Data[2], (Stats)action.Data[3], (int)action.Data[4]);
                    return;
                case ActionType.Mount:
                    PacketWaiting = false;
                    Mount();
                    break;


            }

            base.ProcessAction(action);
        }

        public void ProcessTorch()
        {
            if (SEnvir.Now <= TorchTime || InSafeZone) return;

            TorchTime = SEnvir.Now.AddSeconds(10);

            DamageItem(GridType.Equipment, (int)EquipmentSlot.Torch, Config.TorchRate);

            UserItem torch = Equipment[(int)EquipmentSlot.Torch];
            if (torch == null || torch.CurrentDurability != 0 || torch.Info.Durability <= 0) return;

            RemoveItem(torch);
            Equipment[(int)EquipmentSlot.Torch] = null;
            torch.Delete();

            RefreshWeight();

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Torch },
                Success = true,
            });
        }
        public void ProcessRegen()
        {
            if (Dead || SEnvir.Now < RegenTime) return;

            RegenTime = SEnvir.Now + RegenDelay;

            float rate = 2; //2%

            if (Class == MirClass.Wizard) rate += 1;

            UserMagic magic;
            if (Magics.TryGetValue(MagicType.Rejuvenation, out magic) && Level >= magic.Info.NeedLevel1)
                rate += 0.5F + magic.Level * 0.5F;

            rate /= 100F;

            if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                LevelMagic(magic);



            if (CurrentHP < Stats[Stat.Health])
            {
                int regen = (int)Math.Max(1, Stats[Stat.Health] * rate);

                ChangeHP(regen);
            }

            if (CurrentMP < Stats[Stat.Mana])
            {
                int regen = (int)Math.Max(1, Stats[Stat.Mana] * rate);

                ChangeMP(regen);
            }
        }
        public void ProcessAutoPotion()
        {
            if (SEnvir.Now < UseItemTime || Buffs.Any(x => x.Type == BuffType.Cloak || x.Type == BuffType.Transparency || x.Type == BuffType.DragonRepulse)) return; //Can't auto Pot


            if (DelayItemUse != null)
            {
                ItemUse(DelayItemUse);
                DelayItemUse = null;
                return;
            }

            if (Dead) return;

            foreach (AutoPotionLink link in AutoPotions)
            {
                if (!link.Enabled) continue;

                if (CurrentHP > link.Health && link.Health > 0) continue;
                if (CurrentMP > link.Mana && link.Mana > 0) continue;

                if (link.Health == 0 && link.Mana == 0) continue;

                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] == null || Inventory[i].Info.Index != link.LinkInfoIndex) continue;

                    if ((Inventory[i].Info.Stats[Stat.Health] == 0 || CurrentHP == Stats[Stat.Health]) &&
                        (Inventory[i].Info.Stats[Stat.Mana] == 0 || CurrentMP == Stats[Stat.Mana])) continue;

                    if (SEnvir.Now < AutoPotionCheckTime) return;

                    ItemUse(new CellLinkInfo { GridType = GridType.Inventory, Count = 1, Slot = i });
                    AutoPotionTime = UseItemTime;
                    AutoPotionCheckTime = UseItemTime;
                    return;
                }

                if (Companion == null) continue;

                for (int i = 0; i < Companion.Inventory.Length; i++)
                {
                    if (i >= Companion.Stats[Stat.CompanionInventory]) break;

                    if (Companion.Inventory[i] == null || Companion.Inventory[i].Info.Index != link.LinkInfoIndex) continue;

                    if ((Companion.Inventory[i].Info.Stats[Stat.Health] == 0 || CurrentHP == Stats[Stat.Health]) &&
                        (Companion.Inventory[i].Info.Stats[Stat.Mana] == 0 || CurrentMP == Stats[Stat.Mana])) continue;

                    if (SEnvir.Now < AutoPotionCheckTime) return;

                    ItemUse(new CellLinkInfo { GridType = GridType.CompanionInventory, Count = 1, Slot = i });
                    AutoPotionTime = UseItemTime;
                    AutoPotionCheckTime = UseItemTime;
                    return;
                }

            }

            AutoPotionCheckTime = SEnvir.Now.AddMilliseconds(200);
        }

        public void ProcessItemExpire()
        {
            if (ItemTime.AddSeconds(1) > SEnvir.Now) return;

            TimeSpan ticks = SEnvir.Now - ItemTime;
            ItemTime = SEnvir.Now;

            if (InSafeZone) return;
            bool refresh = false;

            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem item = Equipment[i];
                if (item == null) continue;
                if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                item.ExpireTime -= ticks;

                if (item.ExpireTime > TimeSpan.Zero) continue;

                Connection.ReceiveChat(string.Format(Connection.Language.Expired, item.Info.ItemName), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.Expired, item.Info.ItemName), MessageType.System);

                RemoveItem(item);
                Equipment[i] = null;
                item.Delete();

                refresh = true;

                Enqueue(new S.ItemChanged
                {
                    Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = i },
                    Success = true,
                });
            }


            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];
                if (item == null) continue;
                if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;


                item.ExpireTime -= ticks;

                if (item.ExpireTime > TimeSpan.Zero) continue;

                Connection.ReceiveChat(string.Format(Connection.Language.Expired, item.Info.ItemName), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.Expired, item.Info.ItemName), MessageType.System);

                RemoveItem(item);
                Inventory[i] = null;
                item.Delete();

                refresh = true;

                Enqueue(new S.ItemChanged
                {
                    Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i },
                    Success = true,
                });
            }

            if (Companion != null)
            {
                for (int i = 0; i < Companion.Inventory.Length; i++)
                {
                    UserItem item = Companion.Inventory[i];
                    if (item == null) continue;
                    if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;


                    item.ExpireTime -= ticks;

                    if (item.ExpireTime > TimeSpan.Zero) continue;

                    Connection.ReceiveChat(string.Format(Connection.Language.Expired, item.Info.ItemName), MessageType.System);
                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.Expired, item.Info.ItemName), MessageType.System);

                    RemoveItem(item);
                    Companion.Inventory[i] = null;
                    item.Delete();

                    refresh = true;

                    Enqueue(new S.ItemChanged
                    {
                        Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i },
                        Success = true,
                    });
                }
                for (int i = 0; i < Companion.Equipment.Length; i++)
                {
                    UserItem item = Companion.Equipment[i];
                    if (item == null) continue;
                    if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;


                    item.ExpireTime -= ticks;

                    if (item.ExpireTime > TimeSpan.Zero) continue;

                    Connection.ReceiveChat(string.Format(Connection.Language.Expired, item.Info.ItemName), MessageType.System);
                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.Expired, item.Info.ItemName), MessageType.System);

                    RemoveItem(item);
                    Companion.Equipment[i] = null;
                    item.Delete();

                    refresh = true;

                    Enqueue(new S.ItemChanged
                    {
                        Link = new CellLinkInfo { GridType = GridType.CompanionEquipment, Slot = i },
                        Success = true,
                    });
                }
            }


            if (refresh)
                RefreshStats();
        }

        public override void ProcessNameColour()
        {
            NameColour = Color.White;

            if (Stats[Stat.Rebirth] > 0)
                NameColour = Color.DeepPink;


            if (Stats[Stat.PKPoint] >= Config.RedPoint)
                NameColour = Globals.RedNameColour;
            else if (Stats[Stat.Brown] > 0)
                NameColour = Globals.BrownNameColour;
            else if (Stats[Stat.PKPoint] >= 50)
                NameColour = Color.Yellow;
        }

        public void StartGame()
        {
            if (!SetBindPoint())
            {
                SEnvir.Log($"[Failed to spawn Character] Index: {Character.Index}, Name: {Character.CharacterName}, Failed to reset bind point.");
                Enqueue(new S.StartGame { Result = StartGameResult.UnableToSpawn });
                Connection = null;
                Character = null;
                return;
            }

            if (Character.CurrentInstance != null && Spawn(Character.CurrentInstance.ReconnectRegion, null, 0))
            {
                return;
            }

            if (!Spawn(Character.CurrentMap, null, 0, CurrentLocation) && !Spawn(Character.BindPoint.BindRegion, null, 0))
            {
                SEnvir.Log($"[Failed to spawn Character] Index: {Character.Index}, Name: {Character.CharacterName}");
                Enqueue(new S.StartGame { Result = StartGameResult.UnableToSpawn });
                Connection = null;
                Character = null;
                return;
            }
        }

        public void StopGame()
        {
            Character.LastLogin = SEnvir.Now;

            if (Character.Account.GuildMember != null)
            {
                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member == Character.Account.GuildMember || member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildMemberOffline { Index = Character.Account.GuildMember.Index, ObserverPacket = false });
                }
            }

            TradeClose();

            BuffRemove(BuffType.DragonRepulse);
            BuffRemove(BuffType.Developer);
            BuffRemove(BuffType.Ranking);
            BuffRemove(BuffType.Castle);
            BuffRemove(BuffType.Veteran);

            if (GroupMembers != null) GroupLeave();

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>(TaggedMonsters);

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;

            TaggedMonsters.Clear();

            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
            SpellList.Clear();

            for (int i = Pets.Count - 1; i >= 0; i--)
                Pets[i].Despawn();
            Pets.Clear();

            for (int i = Connection.Observers.Count - 1; i >= 0; i--)
                Connection.Observers[i].EndObservation();
            Connection.Observers.Clear();

            CompanionDespawn();

            if (Character.Partner?.Player != null)
                Character.Partner.Player.Enqueue(new S.MarriageOnlineChanged());


            Despawn();


            Connection.Player = null;
            Character.Player = null;
            Connection = null;
            Character = null;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            Character.LastLogin = SEnvir.Now;

            SEnvir.Players.Add(this);

            Character.Account.LastCharacter = Character;

            Character.Player = this;
            Connection.Player = this;
            Connection.Stage = GameStage.Game;

            ShoutTime = SEnvir.Now.AddSeconds(10);

            //Broadcast Appearance(?)

            Enqueue(new S.StartGame { Result = StartGameResult.Success, StartInformation = GetStartInformation() });
            //Send Items

            Connection.ReceiveChat(Connection.Language.Welcome, MessageType.Announcement);

            SendGuildInfo();

            if (Level > 0)
            {
                RefreshStats();

                if (CurrentHP <= 0)
                {
                    Dead = true;
                    TownRevive();
                }
                Enqueue(new S.InformMaxExperience { MaxExperience = MaxExperience });
            }

            if (Character.Account.GuildMember != null)
            {
                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member == Character.Account.GuildMember || member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildMemberOnline
                    {
                        Index = Character.Account.GuildMember.Index,
                        Name = Name,
                        ObjectID = ObjectID,
                        ObserverPacket = false
                    });
                }
            }

            AddAllObjects();


            if (Level == 0)
                NewCharacter();

            if (Character.CanThrusting && Magics.ContainsKey(MagicType.Thrusting))
                Enqueue(new S.MagicToggle { Magic = MagicType.Thrusting, CanUse = true });

            if (Character.CanHalfMoon && Magics.ContainsKey(MagicType.HalfMoon))
                Enqueue(new S.MagicToggle { Magic = MagicType.HalfMoon, CanUse = true });

            if (Character.CanDestructiveSurge && Magics.ContainsKey(MagicType.DestructiveSurge))
                Enqueue(new S.MagicToggle { Magic = MagicType.DestructiveSurge, CanUse = true });

            if (Character.CanFlameSplash && Magics.ContainsKey(MagicType.FlameSplash))
                Enqueue(new S.MagicToggle { Magic = MagicType.FlameSplash, CanUse = true });

            List<ClientRefineInfo> refines = new List<ClientRefineInfo>();

            foreach (RefineInfo info in Character.Refines)
                refines.Add(info.ToClientInfo());

            if (refines.Count > 0)
                Enqueue(new S.RefineList { List = refines });

            Enqueue(new S.MarketPlaceConsign { Consignments = Character.Account.Auctions.Select(x => x.ToClientInfo(Character.Account)).ToList(), ObserverPacket = false });

            Enqueue(new S.MailList { Mail = Character.Account.Mail.Select(x => x.ToClientInfo()).ToList() });


            if (Character.Account.Characters.Max(x => x.Level) > Level && Character.Rebirth == 0)
                BuffAdd(BuffType.Veteran, TimeSpan.MaxValue, new Stats { [Stat.ExperienceRate] = 50 }, false, false, TimeSpan.Zero);


            Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold, ObserverPacket = false });

            Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });

            Map map = SEnvir.GetMap(CurrentMap.Info.ReconnectMap);

            if (map != null && !InSafeZone)
                Teleport(map, map.GetRandomLocation());

            UpdateReviveTimers(Connection);

            CompanionSpawn();

            Enqueue(GetMarriageInfo());

            if (Character.Partner?.Player != null)
                Character.Partner.Player.Enqueue(new S.MarriageOnlineChanged { ObjectID = ObjectID });

            ApplyMapBuff();
            ApplyServerBuff();
            ApplyCastleBuff();
            ApplyGuildBuff();
            ApplyObserverBuff();

            PauseBuffs();


            if (SEnvir.TopRankings.Contains(Character))
                BuffAdd(BuffType.Ranking, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);

            if (Character.Account.Admin)
                BuffAdd(BuffType.Developer, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);

            Enqueue(new S.HelmetToggle { HideHelmet = Character.HideHelmet });

            //Send War Date to guild.
            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)
            {
                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

                Enqueue(new S.GuildCastleInfo { Index = castle.Index, Owner = ownerGuild?.GuildName ?? String.Empty, ObserverPacket = false });
            }

            foreach (ConquestWar conquest in SEnvir.ConquestWars)
                Enqueue(new S.GuildConquestStarted { Index = conquest.Castle.Index });


            Enqueue(new S.FortuneUpdate { Fortunes = Character.Account.Fortunes.Select(x => x.ToClientInfo()).ToList() });
        }
        public void SetUpObserver(SConnection con)
        {
            con.Stage = GameStage.Observer;
            con.Observed = Connection;
            Connection.Observers.Add(con);

            con.Enqueue(new S.StartObserver
            {
                StartInformation = GetStartInformation(),

                Items = Character.Account.Items.Select(x => x.ToClientInfo()).ToList(),
            });
            //Send Items

            foreach (MapObject ob in VisibleObjects)
            {
                if (ob == this) continue;

                con.Enqueue(ob.GetInfoPacket(this));
            }


            List<ClientRefineInfo> refines = new List<ClientRefineInfo>();

            foreach (RefineInfo info in Character.Refines)
                refines.Add(info.ToClientInfo());

            if (refines.Count > 0)
                con.Enqueue(new S.RefineList { List = refines });


            con.Enqueue(new S.StatsUpdate { Stats = Stats, HermitStats = Character.HermitStats, HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints) });

            con.Enqueue(new S.WeightUpdate { BagWeight = BagWeight, WearWeight = WearWeight, HandWeight = HandWeight });

            Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });

            if (TradePartner != null)
            {
                con.Enqueue(new S.TradeOpen { Name = TradePartner.Name });

                if (TradeGold > 0)
                    con.Enqueue(new S.TradeAddGold { Gold = TradeGold });

                foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
                    con.Enqueue(new S.TradeAddItem { Cell = pair.Value, Success = true });


                if (TradePartner.TradeGold > 0)
                    con.Enqueue(new S.TradeGoldAdded { Gold = TradePartner.TradeGold });

                foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
                {
                    S.TradeItemAdded packet = new S.TradeItemAdded
                    {
                        Item = pair.Key.ToClientInfo()
                    };
                    packet.Item.Count = pair.Value.Count;
                    con.Enqueue(packet);
                }
            }

            if (NPCPage != null)
                con.Enqueue(new S.NPCResponse { ObjectID = NPC.ObjectID, Index = NPCPage.Index });


            UpdateReviveTimers(con);

            if (Companion != null)
                con.Enqueue(new S.CompanionWeightUpdate { BagWeight = Companion.BagWeight, MaxBagWeight = Companion.Stats[Stat.CompanionBagWeight], InventorySize = Companion.Stats[Stat.CompanionInventory] });

            con.Enqueue(GetMarriageInfo());

            foreach (MapObject ob in VisibleDataObjects)
            {
                // if (ob.Race == ObjectType.Player) continue;

                con.Enqueue(ob.GetDataPacket(this));
            }

            if (GroupMembers != null)
                foreach (PlayerObject ob in GroupMembers)
                    con.Enqueue(new S.GroupMember { ObjectID = ob.ObjectID, Name = ob.Name });

            con.ReceiveChat(string.Format(con.Language.WelcomeObserver, Name), MessageType.Announcement);

            if (Character.Account.GuildMember != null)
                foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
                {
                    if (warInfo.Guild1 == Character.Account.GuildMember.Guild)
                        con.Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild2.GuildName, Duration = warInfo.Duration });

                    if (warInfo.Guild2 == Character.Account.GuildMember.Guild)
                        con.Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild1.GuildName, Duration = warInfo.Duration });
                }

            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)
            {
                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

                con.Enqueue(new S.GuildCastleInfo { Index = castle.Index, Owner = ownerGuild?.GuildName ?? String.Empty });
            }

            foreach (ConquestWar conquest in SEnvir.ConquestWars)
                Enqueue(new S.GuildConquestStarted { Index = conquest.Castle.Index });

            con.Enqueue(new S.FortuneUpdate { Fortunes = Character.Account.Fortunes.Select(x => x.ToClientInfo()).ToList() });
        }

        public void ObservableSwitch(bool allow)
        {
            if (allow == Character.Observable) return;

            if (!InSafeZone)
            {
                Connection.ReceiveChat(Connection.Language.ObserverChangeFail, MessageType.System);
                return;
            }

            Character.Observable = allow;
            Enqueue(new S.ObservableSwitch { Allow = Character.Observable, ObserverPacket = false });

            for (int i = Connection.Observers.Count - 1; i >= 0; i--)
            {
                if (Connection.Observers[i].Account != null && (Connection.Observers[i].Account.Observer || Connection.Observers[i].Account.TempAdmin)) continue;

                Connection.Observers[i].EndObservation();
            }

            ApplyObserverBuff();

        }

        private void NewCharacter()
        {
            Level = 1;
            LevelUp();

            foreach (ItemInfo info in SEnvir.ItemInfoList.Binding)
            {
                if (!info.StartItem) continue;

                if (!CanStartWith(info)) continue;

                ItemCheck check = new ItemCheck(info, 1, UserItemFlags.Bound | UserItemFlags.Worthless, TimeSpan.Zero);

                if (CanGainItems(false, check))
                {
                    UserItem item = SEnvir.CreateFreshItem(check);

                    if (info.ItemType == ItemType.Armour)
                        item.Colour = Character.ArmourColour;

                    GainItem(item);
                }
            }

            RefreshStats();

            SetHP(Stats[Stat.Health]);
            SetMP(Stats[Stat.Mana]);

            Direction = MirDirection.Down;
        }
        private bool SetBindPoint()
        {
            if (Character.BindPoint != null && Character.BindPoint.ValidBindPoints.Count > 0)
                return true;

            List<SafeZoneInfo> spawnPoints = new List<SafeZoneInfo>();

            foreach (SafeZoneInfo info in SEnvir.SafeZoneInfoList.Binding)
            {
                if (info.ValidBindPoints.Count == 0) continue;

                switch (Class)
                {
                    case MirClass.Warrior:
                        if ((info.StartClass & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((info.StartClass & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((info.StartClass & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((info.StartClass & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                spawnPoints.Add(info);
            }

            if (spawnPoints.Count > 0)
                Character.BindPoint = spawnPoints[SEnvir.Random.Next(spawnPoints.Count)];

            return Character.BindPoint != null;
        }
        public void TownRevive()
        {
            if (!Dead) return;

            Cell cell = SEnvir.Maps[Character.BindPoint.BindRegion.Map].GetCell(Character.BindPoint.ValidBindPoints[SEnvir.Random.Next(Character.BindPoint.ValidBindPoints.Count)]);

            CurrentCell = cell.GetMovement(this);

            RemoveAllObjects();

            AddAllObjects();

            Dead = false;
            SetHP(Stats[Stat.Health]);
            SetMP(Stats[Stat.Mana]);

            Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = true });
        }

        protected override void OnMapChanged()
        {
            base.OnMapChanged();

            if (CurrentMap == null) return;

            Character.CurrentMap = CurrentMap.Info;
            Character.CurrentInstance = CurrentMap.Instance;

            if (!Spawned) return;

            for (int i = SpellList.Count - 1; i >= 0; i--)
                if (SpellList[i].CurrentMap != CurrentMap)
                    SpellList[i].Despawn();

            Enqueue(new S.MapChanged
            {
                MapIndex = CurrentMap.Info.Index,
                InstanceIndex = CurrentMap.Instance?.Index ?? -1
            });

            if (!CurrentMap.Info.CanHorse)
                RemoveMount();


            ApplyMapBuff();
        }
        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();

            TradeClose();

            if (CurrentCell == null) return;

            if (Companion != null)
                Companion.SearchTime = DateTime.MinValue;

            for (int i = SpellList.Count - 1; i >= 0; i--)
                if (SpellList[i].CurrentMap != CurrentMap || !Functions.InRange(SpellList[i].DisplayLocation, CurrentLocation, Config.MaxViewRange))
                    SpellList[i].Despawn();

            if (CurrentCell.SafeZone != null && CurrentCell.SafeZone.ValidBindPoints.Count > 0 && Stats[Stat.PKPoint] < Config.RedPoint)
                Character.BindPoint = CurrentCell.SafeZone;

            if (InSafeZone != (CurrentCell.SafeZone != null))
            {
                InSafeZone = CurrentCell.SafeZone != null;

                if (!Spawned) return;

                Enqueue(new S.SafeZoneChanged { InSafeZone = InSafeZone });
                PauseBuffs();
            }
            else if (Spawned && CurrentMap.Info.CanMine)
                PauseBuffs();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            SEnvir.Players.Remove(this);
        }
        public override void OnSafeDespawn()
        {
            throw new NotImplementedException();
        }

        public override void CleanUp()
        {
            base.CleanUp();

            NPC = null;
            NPCPage = null;

            Pets?.Clear();

            VisibleObjects?.Clear();

            VisibleDataObjects?.Clear();

            TaggedMonsters?.Clear();

            NearByObjects?.Clear();

            Inventory = null;
            Equipment = null;
            PartsStorage = null;
            Storage = null;

            Companion = null;

            LastHitter = null;

            GroupInvitation = null;

            GuildInvitation = null;

            MarriageInvitation = null;

            TradePartner = null;

            TradePartnerRequest = null;

            TradeItems?.Clear();

            Magics?.Clear();

            AutoPotions?.Clear();
        }

        public void RemoveMount()
        {
            if (Horse == HorseType.None) return;

            Horse = HorseType.None;
            Broadcast(new S.ObjectMount { ObjectID = ObjectID, Horse = Horse });
        }

        public void Chat(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            SEnvir.LogChat($"{Name}: {text}");

            //Item Links

            string[] parts;

            if (text.StartsWith("/"))
            {
                //Private Message
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                SConnection con = SEnvir.GetConnectionByCharacter(parts[0]);

                if (con == null || (con.Stage != GameStage.Observer && con.Stage != GameStage.Game) || SEnvir.IsBlocking(Character.Account, con.Account))
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.CannotFindPlayer, parts[0]), MessageType.System);
                    return;
                }

                if (!Character.Account.TempAdmin)
                {
                    if (BlockWhisper)
                    {
                        Connection.ReceiveChat(Connection.Language.BlockingWhisper, MessageType.System);
                        return;
                    }

                    if (con.Player != null && con.Player.BlockWhisper)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.PlayerBlockingWhisper, parts[0]), MessageType.System);
                        return;
                    }
                }

                Connection.ReceiveChat($"/{text}", MessageType.WhisperOut);

                if (SEnvir.Now < Character.Account.ChatBanExpiry) return;

                con.ReceiveChat($"{Name}=> {text.Remove(0, parts[0].Length)}", Character.Account.TempAdmin ? MessageType.GMWhisperIn : MessageType.WhisperIn);
            }
            else if (text.StartsWith("!!"))
            {
                if (GroupMembers == null) return;

                text = $"{Name}: {text.Remove(0, 2)}";

                foreach (PlayerObject member in GroupMembers)
                {
                    if (SEnvir.IsBlocking(Character.Account, member.Character.Account)) continue;

                    if (member != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    member.Connection.ReceiveChat(text, MessageType.Group);
                }
            }
            else if (text.StartsWith("!~"))
            {
                if (Character.Account.GuildMember == null) return;

                text = $"{Name}: {text.Remove(0, 2)}";

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member.Account.Connection == null) continue;
                    if (member.Account.Connection.Stage != GameStage.Game && member.Account.Connection.Stage != GameStage.Observer) continue;
                    if (SEnvir.IsBlocking(Character.Account, member.Account)) continue;

                    member.Account.Connection.ReceiveChat(text, MessageType.Guild);
                }
            }
            else if (text.StartsWith("!@"))
            {
                if (!Character.Account.TempAdmin)
                {
                    if (SEnvir.Now < Character.Account.GlobalTime)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.GlobalDelay, Math.Ceiling((Character.Account.GlobalTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }
                    if (Level < 33 && Stats[Stat.GlobalShout] == 0)
                    {
                        Connection.ReceiveChat(Connection.Language.GlobalLevel, MessageType.System);
                        return;
                    }

                    Character.Account.GlobalTime = SEnvir.Now.AddSeconds(30);
                }

                text = string.Format("(!@){0}: {1}", Name, text.Remove(0, 2));

                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            if (SEnvir.IsBlocking(Character.Account, con.Account)) continue;

                            con.ReceiveChat(text, MessageType.Global);
                            break;
                        default: continue;
                    }
                }
            }
            else if (text.StartsWith("!"))
            {
                //Shout
                if (!Character.Account.TempAdmin)
                {
                    if (SEnvir.Now < ShoutTime)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.ShoutDelay, Math.Ceiling((ShoutTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }
                    if (Level < 2)
                    {
                        Connection.ReceiveChat(Connection.Language.ShoutLevel, MessageType.System);
                        return;
                    }
                }

                text = string.Format("(!){0}: {1}", Name, text.Remove(0, 1));
                ShoutTime = SEnvir.Now + Config.ShoutDelay;

                foreach (PlayerObject player in CurrentMap.Players)
                {
                    if (player != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    if (!SEnvir.IsBlocking(Character.Account, player.Character.Account))
                        player.Connection.ReceiveChat(text, MessageType.Shout);

                    foreach (SConnection observer in player.Connection.Observers)
                    {
                        if (SEnvir.IsBlocking(Character.Account, observer.Account)) continue;

                        observer.ReceiveChat(text, MessageType.Shout);
                    }
                }
            }
            else if (text.StartsWith("@!"))
            {
                if (!Character.Account.TempAdmin) return;

                text = string.Format("{0}: {1}", Name, text.Remove(0, 2));

                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            con.ReceiveChat(text, MessageType.Announcement);
                            break;
                        default: continue;
                    }
                }
            }
            else if (text.StartsWith("@"))
            {
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                int level;
                int count;
                int value;
                PlayerObject player;

                switch (parts[0].ToUpper())
                {
                    case "ROLL":
                        if (GroupMembers == null) return;

                        if (parts.Length < 2 || !int.TryParse(parts[1], out count) || count < 0)
                            count = 6;

                        int result = SEnvir.Random.Next(count) + 1;


                        foreach (PlayerObject member in GroupMembers)
                            member.Connection.ReceiveChat(string.Format(member.Connection.Language.DiceRoll, Name, result, count), MessageType.Group);
                        break;
                    case "EXTRACTORLOCK":
                        ExtractorLock = !ExtractorLock;

                        Connection.ReceiveChat(ExtractorLock ? "Extraction Enabled" : "Extraction Locked", MessageType.System);
                        break;
                    case "ENABLELEVEL3":
                        CompanionLevelLock3 = !CompanionLevelLock3;

                        Connection.ReceiveChat(string.Format(CompanionLevelLock3 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 3), MessageType.System);
                        break;
                    case "ENABLELEVEL5":
                        CompanionLevelLock5 = !CompanionLevelLock5;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock5 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 5), MessageType.System);
                        break;
                    case "ENABLELEVEL7":
                        CompanionLevelLock7 = !CompanionLevelLock7;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock7 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 7), MessageType.System);
                        break;
                    case "ENABLELEVEL10":
                        CompanionLevelLock10 = !CompanionLevelLock10;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock10 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 10), MessageType.System);
                        break;
                    case "ENABLELEVEL11":
                        CompanionLevelLock11 = !CompanionLevelLock11;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock11 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 11), MessageType.System);
                        break;
                    case "ENABLELEVEL13":
                        CompanionLevelLock13 = !CompanionLevelLock13;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock13 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 13), MessageType.System);
                        break;
                    case "ENABLELEVEL15":
                        CompanionLevelLock15 = !CompanionLevelLock15;
                        Connection.ReceiveChat(string.Format(CompanionLevelLock15 ? Connection.Language.CompanionSkillEnabled : Connection.Language.CompanionSkillDisabled, 15), MessageType.System);
                        break;
                    case "ALLOWTRADE":
                        Character.Account.AllowTrade = !Character.Account.AllowTrade;
                        Connection.ReceiveChat(Character.Account.AllowTrade ? Connection.Language.TradingEnabled : Connection.Language.TradingDisabled, MessageType.System);
                        break;
                    case "BLOCKWHISPER":
                        BlockWhisper = !BlockWhisper;
                        Connection.ReceiveChat(BlockWhisper ? Connection.Language.WhisperDisabled : Connection.Language.WhisperEnabled, MessageType.System);
                        break;
                    case "ALLOWGUILD":
                        Character.Account.AllowGuild = !Character.Account.AllowGuild;
                        Connection.ReceiveChat(Character.Account.AllowGuild ? Connection.Language.GuildInviteEnabled : Connection.Language.GuildInviteDisabled, MessageType.System);
                        break;
                    case "LEAVEGUILD":
                        GuildLeave();
                        break;
                    case "RECALL":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        player = SEnvir.GetPlayerByCharacter(parts[1]);

                        player?.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction));
                        break;
                    case "ALLOWRECALL":
                        Character.Account.AllowGroupRecall = !Character.Account.AllowGroupRecall;
                        Connection.ReceiveChat(Character.Account.AllowGroupRecall ? Connection.Language.GroupRecallEnabled : Connection.Language.GroupRecallDisabled, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(Character.Account.AllowGroupRecall ? con.Language.GroupRecallEnabled : con.Language.GroupRecallDisabled, MessageType.System);
                        break;
                    case "GROUPRECALL":
                        if (Stats[Stat.RecallSet] <= 0) return;


                        if (GroupMembers == null)
                        {
                            Connection.ReceiveChat(Connection.Language.GroupNoGroup, MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(con.Language.GroupNoGroup, MessageType.System);
                            return;
                        }

                        if (GroupMembers[0] != this)
                        {
                            Connection.ReceiveChat(Connection.Language.GroupNotLeader, MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(con.Language.GroupNotLeader, MessageType.System);
                            return;
                        }

                        if (!CurrentMap.Info.AllowTT || !CurrentMap.Info.AllowRT || CurrentMap.Info.SkillDelay > 0)
                        {
                            Connection.ReceiveChat(Connection.Language.GroupRecallMap, MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(con.Language.GroupRecallMap, MessageType.System);
                            return;
                        }

                        if (SEnvir.Now < Character.GroupRecallTime)
                        {
                            Connection.ReceiveChat(string.Format(Connection.Language.GroupRecallDelay, Functions.ToString(Character.GroupRecallTime - SEnvir.Now, true)), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.GroupRecallDelay, Functions.ToString(Character.GroupRecallTime - SEnvir.Now, true)), MessageType.System);
                            return;
                        }

                        foreach (PlayerObject member in GroupMembers)
                        {
                            if (member.Dead || member == this) continue;

                            if (!member.CurrentMap.Info.AllowTT)
                            {
                                member.Connection.ReceiveChat(member.Connection.Language.GroupRecallFromMap, MessageType.System);

                                foreach (SConnection con in member.Connection.Observers)
                                    con.ReceiveChat(con.Language.GroupRecallFromMap, MessageType.System);

                                Connection.ReceiveChat(string.Format(Connection.Language.GroupRecallMemberFromMap, member.Name), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(string.Format(con.Language.GroupRecallMemberFromMap, member.Name), MessageType.System);
                                continue;
                            }

                            if (!member.Character.Account.AllowGroupRecall)
                            {
                                member.Connection.ReceiveChat(member.Connection.Language.GroupRecallNotAllowed, MessageType.System);

                                foreach (SConnection con in member.Connection.Observers)
                                    con.ReceiveChat(con.Language.GroupRecallNotAllowed, MessageType.System);

                                Connection.ReceiveChat(string.Format(member.Connection.Language.GroupRecallMemberNotAllowed, member.Name), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(string.Format(con.Language.GroupRecallMemberNotAllowed, member.Name), MessageType.System);
                                continue;
                            }


                            member.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 10));
                        }

                        Character.GroupRecallTime = SEnvir.Now.AddMinutes(3);
                        break;
                    case "OBSERVER":
                        if (!Character.Account.TempAdmin) return;
                        Observer = !Observer;

                        AddAllObjects();
                        RemoveAllObjects();
                        break;
                    case "GAMEMASTER":
                        if (!Character.Account.TempAdmin) return;
                        GameMaster = !GameMaster;
                        break;
                    case "GOLDBOT":
                        if (!Character.Account.TempAdmin) return;

                        if (parts.Length < 2) return;

                        CharacterInfo target = SEnvir.GetCharacter(parts[1]);

                        if (target == null) return;

                        target.Account.GoldBot = !target.Account.GoldBot;
                        Connection.ReceiveChat($"Gold Bot [{target.CharacterName}] - [{target.Account.GoldBot}]", MessageType.System);
                        break;
                    case "ITEMBOT":
                        if (!Character.Account.TempAdmin) return;

                        if (parts.Length < 2) return;

                        target = SEnvir.GetCharacter(parts[1]);

                        if (target == null) return;

                        target.Account.ItemBot = !target.Account.ItemBot;
                        Connection.ReceiveChat($"Item Bot [{target.CharacterName}] - [{target.Account.ItemBot}]", MessageType.System);
                        break;
                    case "LEVEL":
                        if (!Character.Account.TempAdmin) return;

                        if (parts.Length < 3)
                        {
                            if (parts.Length < 2) return;

                            if (!int.TryParse(parts[1], out value) || value < 0) return;

                            player = this;
                        }
                        else
                        {
                            if (!int.TryParse(parts[2], out value) || value < 0) return;

                            player = SEnvir.GetPlayerByCharacter(parts[1]);
                        }

                        if (player == null) return;

                        player.Level = value;
                        player.LevelUp();
                        break;
                    case "GOTO":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        player = SEnvir.GetPlayerByCharacter(parts[1]);

                        if (player == null) return;

                        Teleport(player.CurrentMap, player.CurrentLocation);
                        break;
                    case "GIVESKILLS":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        player = SEnvir.GetPlayerByCharacter(parts[1]);

                        if (player == null) return;

                        UserMagic uMagic;
                        foreach (MagicInfo mInfo in SEnvir.MagicInfoList.Binding)
                        {
                            if (mInfo.NeedLevel1 > player.Level || mInfo.Class != player.Class || mInfo.School == MagicSchool.None) continue;

                            if (!player.Magics.TryGetValue(mInfo.Magic, out uMagic))
                            {
                                uMagic = SEnvir.UserMagicList.CreateNewObject();
                                uMagic.Character = player.Character;
                                uMagic.Info = mInfo;
                                player.Magics[mInfo.Magic] = uMagic;

                                player.Enqueue(new S.NewMagic { Magic = uMagic.ToClientInfo() });
                            }

                            level = 1;

                            if (player.Level >= mInfo.NeedLevel2)
                                level = 2;

                            if (player.Level >= mInfo.NeedLevel3)
                                level = 3;

                            uMagic.Level = level;

                            player.Enqueue(new S.MagicLeveled { InfoIndex = uMagic.Info.Index, Level = uMagic.Level, Experience = uMagic.Experience });
                        }

                        player.RefreshStats();

                        break;
                    case "SETCOMPANIONVALUE":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        Stat stat;
                        if (!int.TryParse(parts[1], out level)) return;
                        if (!Enum.TryParse(parts[2], out stat)) return;
                        if (!int.TryParse(parts[3], out value)) return;

                        if (Companion == null) return;

                        switch (level)
                        {
                            case 3:
                                Companion.UserCompanion.Level3 = new Stats { [stat] = value };
                                break;
                            case 5:
                                Companion.UserCompanion.Level5 = new Stats { [stat] = value };
                                break;
                            case 7:
                                Companion.UserCompanion.Level7 = new Stats { [stat] = value };
                                break;
                            case 10:
                                Companion.UserCompanion.Level10 = new Stats { [stat] = value };
                                break;
                            case 11:
                                Companion.UserCompanion.Level11 = new Stats { [stat] = value };
                                break;
                            case 13:
                                Companion.UserCompanion.Level13 = new Stats { [stat] = value };
                                break;
                            case 15:
                                Companion.UserCompanion.Level15 = new Stats { [stat] = value };
                                break;
                        }

                        CompanionRefreshBuff();

                        Enqueue(new S.CompanionSkillUpdate
                        {
                            Level3 = Companion.UserCompanion.Level3,
                            Level5 = Companion.UserCompanion.Level5,
                            Level7 = Companion.UserCompanion.Level7,
                            Level10 = Companion.UserCompanion.Level10,
                            Level11 = Companion.UserCompanion.Level11,
                            Level13 = Companion.UserCompanion.Level13,
                            Level15 = Companion.UserCompanion.Level15
                        });
                        break;
                    case "MONSTER":
                        if (!Character.Account.TempAdmin)
                            return;

                        if (parts.Length < 2)
                            return;

                        var monsterInfo = SEnvir.GetMonsterInfo(parts[1]);

                        if (monsterInfo == null)
                            return;

                        if (parts.Length < 3 || !int.TryParse(parts[2], out value) || value == 0)
                            value = 1;

                        while (value > 0)
                        {
                            var monster = MonsterObject.GetMonster(monsterInfo);

                            monster.Spawn(CurrentMap, Functions.Move(CurrentLocation, Direction));
                            value -= 1;
                        }
                        break;
                    case "MAKE":
                        if (!Character.Account.TempAdmin) return;

                        if (parts.Length < 2) return;

                        ItemInfo item = SEnvir.GetItemInfo(parts[1]);

                        if (item == null) return;

                        if (parts.Length < 3 || !int.TryParse(parts[2], out value) || value == 0)
                            value = 1;

                        while (value > 0)
                        {
                            count = Math.Min(value, item.StackSize);

                            if (!CanGainItems(false, new ItemCheck(item, count, UserItemFlags.None, TimeSpan.Zero))) break;

                            UserItem userItem = SEnvir.CreateDropItem(item, 0);

                            userItem.Count = count;
                            userItem.Flags = UserItemFlags.None;

                            value -= count;


                            GainItem(userItem);
                        }

                        break;
                    case "GCCOLLECT":
                        if (!Character.Account.TempAdmin) return;

                        DateTime time = Time.Now;

                        GC.Collect(2, GCCollectionMode.Forced);

                        Connection.ReceiveChat($"[GC COLLECT] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
                        break;
                    case "CLEARIPBLOCKS":
                        if (!Character.Account.TempAdmin) return;

                        SEnvir.IPBlocks.Clear();
                        break;
                    case "REBOOT":
                        if (!Character.Account.TempAdmin) return;

                        time = Time.Now;

                        MarketPlaceCancelSuperior();

                        Connection.ReceiveChat($"[Reboot Command] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
                        break;
                    case "GIVEGAMEGOLD":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (!int.TryParse(parts[2], out count)) return;

                        character.Account.GameGold += count;
                        character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.PaymentComplete, count), MessageType.System);
                        character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                        if (character.Account.Referral != null)
                        {
                            character.Account.Referral.HuntGold += count / 10;

                            if (character.Account.Referral.Connection != null)
                            {
                                character.Account.Referral.Connection.ReceiveChat(string.Format(character.Account.Referral.Connection.Language.ReferralPaymentComplete, count / 10), MessageType.System, 0);

                                if (character.Account.Referral.Connection.Stage == GameStage.Game)
                                    character.Account.Referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.Referral.HuntGold });
                            }
                        }

                        Connection.ReceiveChat(string.Format("[GIVE GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);

                        break;
                    case "REMOVEGAMEGOLD":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (!int.TryParse(parts[2], out count)) return;

                        character.Account.GameGold -= count;
                        character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.PaymentFailed, count), MessageType.System);
                        character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                        if (character.Account.Referral != null)
                        {
                            character.Account.Referral.HuntGold -= count / 10;

                            if (character.Account.Referral.Connection != null)
                            {
                                character.Account.Referral.Connection.ReceiveChat(string.Format(character.Account.Referral.Connection.Language.ReferralPaymentFailed, count / 10), MessageType.System, 0);

                                if (character.Account.Referral.Connection.Stage == GameStage.Game)
                                    character.Account.Referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.Referral.HuntGold });
                            }
                        }

                        Connection.ReceiveChat(string.Format("[REMOVE GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
                        break;
                    case "TAKEGAMEGOLD":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (!int.TryParse(parts[2], out count)) return;

                        character.Account.GameGold -= count;
                        character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.GameGoldLost, count), MessageType.System);
                        character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                        Connection.ReceiveChat(string.Format("[TAKE GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
                        break;
                    case "REFUNDGAMEGOLD":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (!int.TryParse(parts[2], out count)) return;

                        character.Account.GameGold += count;
                        character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.GameGoldRefund, count), MessageType.System);
                        character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                        Connection.ReceiveChat(string.Format("[REFUND GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
                        break;
                    case "REFUNDHUNTGOLD":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (!int.TryParse(parts[2], out count)) return;

                        character.Account.HuntGold += count;
                        character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.HuntGoldRefund, count), MessageType.System);
                        character.Player?.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.HuntGold });

                        Connection.ReceiveChat(string.Format("[REFUND HUNT GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
                        break;
                    case "CHATBAN":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (parts.Length < 3 || !int.TryParse(parts[2], out count))
                            count = 1440 * 365 * 10;

                        character.Account.ChatBanExpiry = SEnvir.Now.AddMinutes(count);
                        break;
                    case "GLOBALBAN":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        character = SEnvir.GetCharacter(parts[1]);

                        if (character == null) return;

                        if (parts.Length < 3 || !int.TryParse(parts[2], out count))
                            count = 1440 * 365 * 10;

                        character.Account.GlobalTime = SEnvir.Now.AddMinutes(count);
                        break;
                    case "MOVE":
                        //If Is GM or Teleport Ring
                        break;
                    case "MAP":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        MapInfo info = SEnvir.MapInfoList.Binding.FirstOrDefault(x => string.Compare(x.FileName, parts[1], StringComparison.OrdinalIgnoreCase) == 0);

                        InstanceInfo instance = null;
                        byte? index = null;

                        //Get chosen index
                        if (parts.Length > 3)
                        {
                            if (byte.TryParse(parts[3], out byte tempIndex))
                            {
                                index = tempIndex;
                            }
                        }

                        if (parts.Length > 2)
                        {
                            var instanceName = parts[2];

                            instance = SEnvir.InstanceInfoList.Binding.FirstOrDefault(x => x.Name == instanceName);

                            //Load new index
                            if (instance != null && index == null)
                            {
                                index = SEnvir.LoadInstance(instance);
                            }

                            if (index == null) return;
                        }

                        Map map = SEnvir.GetMap(info, instance, index ?? 0);

                        if (map == null) return;

                        Teleport(map, map.GetRandomLocation());
                        break;
                    case "CLEARBELT":
                        for (int i = Character.BeltLinks.Count - 1; i >= 0; i--)
                            Character.BeltLinks[i].Delete();
                        break;
                    case "FORCEWAR":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        if (!int.TryParse(parts[1], out value)) return;

                        CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == value);

                        if (castle == null) return;

                        if (SEnvir.ConquestWars.Any(x => x.Castle == castle)) return;

                        SEnvir.StartConquest(castle, true);
                        break;
                    case "FORCEENDWAR":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        if (!int.TryParse(parts[1], out value)) return;

                        castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == value);

                        if (castle == null) return;

                        ConquestWar war = SEnvir.ConquestWars.FirstOrDefault(x => x.Castle == castle);

                        if (war == null) return;

                        war.EndTime = DateTime.MinValue;
                        break;
                    case "REBIRTH":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 3) return;

                        player = SEnvir.GetPlayerByCharacter(parts[1]);
                        if (player == null) return;

                        if (!int.TryParse(parts[2], out value)) return;
                        player.AdjustRebirth(value);

                        break;
                    case "TAKECASTLE":
                        if (!Character.Account.TempAdmin) return;
                        if (parts.Length < 2) return;

                        if (!int.TryParse(parts[1], out value)) return;

                        castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == value);

                        if (castle == null) return;

                        if (Character.Account.GuildMember?.Guild == null)
                        {
                            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

                            if (ownerGuild == null) return;

                            ownerGuild.Castle = null;

                            foreach (SConnection con in SEnvir.Connections)
                            {
                                switch (con.Stage)
                                {
                                    case GameStage.Game:
                                    case GameStage.Observer:
                                        con.ReceiveChat(string.Format(con.Language.ConquestLost, ownerGuild.GuildName, castle.Name), MessageType.System);
                                        break;
                                    default: continue;
                                }
                            }

                            SEnvir.Broadcast(new S.GuildCastleInfo { Index = castle.Index, Owner = string.Empty });

                            foreach (PlayerObject user in SEnvir.Players)
                                user.ApplyCastleBuff();

                            return;
                        }

                        Character.Account.GuildMember.Guild.Castle = castle;

                        foreach (SConnection con in SEnvir.Connections)
                        {
                            switch (con.Stage)
                            {
                                case GameStage.Game:
                                case GameStage.Observer:
                                    con.ReceiveChat(string.Format(con.Language.ConquestCapture, Character.Account.GuildMember.Guild.GuildName, castle.Name), MessageType.System);
                                    break;
                                default: continue;
                            }
                        }

                        SEnvir.Broadcast(new S.GuildCastleInfo { Index = castle.Index, Owner = Character.Account.GuildMember.Guild.GuildName });

                        foreach (PlayerObject user in SEnvir.Players)
                            user.ApplyCastleBuff();

                        break;
                    case "CREATEGUILD":
                        if (!Character.Account.TempAdmin)
                            return;

                        if (parts.Length < 2)
                            return;

                        character = parts.Length < 3 ? Character : SEnvir.GetCharacter(parts[1]);

                        if (character == null)
                            return;

                        if (Character.Account.GuildMember != null)
                            return;

                        var guildName = parts.Length < 3 ? parts[1] : parts[2];

                        if (!Globals.GuildNameRegex.IsMatch(guildName))
                            return;

                        var guildInfo = SEnvir.GuildInfoList.Binding.FirstOrDefault(
                            x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

                        if (guildInfo != null)
                            return;

                        guildInfo = SEnvir.GuildInfoList.CreateNewObject();

                        guildInfo.GuildName = guildName;
                        guildInfo.MemberLimit = 10;
                        guildInfo.StorageSize = 10;
                        guildInfo.GuildLevel = 1;

                        var memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

                        memberInfo.Account = character.Account;
                        memberInfo.Guild = guildInfo;
                        memberInfo.Rank = "Guild Leader";
                        memberInfo.JoinDate = SEnvir.Now;
                        memberInfo.Permission = GuildPermission.Leader;

                        SendGuildInfo();
                        break;

                }

            }
            else if (text.StartsWith("#"))
            {
                text = string.Format("(#){0}: {1}", Name, text.Remove(0, 1));

                Connection.ReceiveChat(text, MessageType.ObserverChat);

                foreach (SConnection target in Connection.Observers)
                {
                    if (SEnvir.IsBlocking(Character.Account, target.Account)) continue;

                    target.ReceiveChat(text, MessageType.ObserverChat);
                }
            }
            else
            {
                text = string.Format("{0}: {1}", Name, text);
                foreach (PlayerObject player in SeenByPlayers)
                {
                    if (!Functions.InRange(CurrentLocation, player.CurrentLocation, Config.MaxViewRange)) continue;

                    if (player != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    if (!SEnvir.IsBlocking(Character.Account, player.Character.Account))
                        player.Connection.ReceiveChat(text, MessageType.Normal, ObjectID);

                    foreach (SConnection observer in player.Connection.Observers)
                    {
                        if (SEnvir.IsBlocking(Character.Account, observer.Account)) continue;

                        observer.ReceiveChat(text, MessageType.Normal, ObjectID);
                    }
                }
            }
        }
        public void ObserverChat(SConnection con, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (con.Account?.LastCharacter == null)
            {
                con.ReceiveChat(con.Language.ObserverNotLoggedIn, MessageType.System);
                return;
            }
            SEnvir.LogChat($"{con.Account.LastCharacter.CharacterName}: {text}");

            string[] parts;

            if (text.StartsWith("/"))
            {
                //Private Message
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                SConnection target = SEnvir.GetConnectionByCharacter(parts[0]);

                if (target == null || (target.Stage != GameStage.Observer && target.Stage != GameStage.Game) || SEnvir.IsBlocking(con.Account, target.Account))
                {
                    con.ReceiveChat(string.Format(con.Language.CannotFindPlayer, parts[0]), MessageType.System);
                    return;
                }

                if (!con.Account.TempAdmin)
                {
                    if (target.Player != null && target.Player.BlockWhisper)
                    {
                        con.ReceiveChat(string.Format(con.Language.PlayerBlockingWhisper, parts[0]), MessageType.System);
                        return;
                    }
                }

                con.ReceiveChat($"/{text}", MessageType.WhisperOut);

                if (SEnvir.Now < con.Account.LastCharacter.Account.ChatBanExpiry) return;

                target.ReceiveChat($"{con.Account.LastCharacter.CharacterName}=> {text.Remove(0, parts[0].Length)}", Character.Account.TempAdmin ? MessageType.GMWhisperIn : MessageType.WhisperIn);
            }
            else if (text.StartsWith("!~"))
            {
                if (con.Account.GuildMember == null) return;

                text = string.Format("{0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));

                foreach (GuildMemberInfo member in con.Account.GuildMember.Guild.Members)
                {
                    if (member.Account.Connection == null) continue;
                    if (member.Account.Connection.Stage != GameStage.Game && member.Account.Connection.Stage != GameStage.Observer) continue;

                    if (SEnvir.IsBlocking(con.Account, member.Account)) continue;

                    member.Account.Connection.ReceiveChat(text, MessageType.Guild);
                }
            }
            else if (text.StartsWith("!@"))
            {
                if (!con.Account.LastCharacter.Account.TempAdmin)
                {
                    if (SEnvir.Now < con.Account.LastCharacter.Account.GlobalTime)
                    {
                        con.ReceiveChat(string.Format(con.Language.GlobalDelay, Math.Ceiling((con.Account.GlobalTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }

                    if (con.Account.LastCharacter.Level < 33 && con.Account.LastCharacter.LastStats[Stat.GlobalShout] == 0)
                    {
                        con.ReceiveChat(con.Language.GlobalLevel, MessageType.System);
                        return;
                    }

                    con.Account.LastCharacter.Account.GlobalTime = SEnvir.Now.AddSeconds(30);
                }

                text = string.Format("(!@){0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));

                foreach (SConnection target in SEnvir.Connections)
                {
                    switch (target.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            if (SEnvir.IsBlocking(con.Account, target.Account)) continue;

                            target.ReceiveChat(text, MessageType.Global);
                            break;
                        default: continue;
                    }
                }
            }
            else if (text.StartsWith("@!"))
            {
                if (!con.Account.LastCharacter.Account.TempAdmin) return;

                text = string.Format("{0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));

                foreach (SConnection target in SEnvir.Connections)
                {
                    switch (target.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            target.ReceiveChat(text, MessageType.Announcement);
                            break;
                        default: continue;
                    }
                }
            }
            else
            {
                if (SEnvir.IsBlocking(con.Account, Character.Account)) return;

                text = string.Format("(#){0}: {1}", con.Account.LastCharacter.CharacterName, text);

                Connection.ReceiveChat(text, MessageType.ObserverChat);

                foreach (SConnection target in Connection.Observers)
                {
                    if (SEnvir.IsBlocking(con.Account, target.Account)) continue;

                    target.ReceiveChat(text, MessageType.ObserverChat);
                }
            }
        }

        public void Inspect(int index, SConnection con)
        {
            if (index == Character.Index) return;

            CharacterInfo target = SEnvir.GetCharacter(index);

            if (target == null) return;

            S.Inspect packet = new S.Inspect
            {
                Name = target.CharacterName,
                Partner = target.Partner?.CharacterName,
                Class = target.Class,
                Gender = target.Gender,
                Stats = target.LastStats,
                HermitStats = target.HermitStats,
                HermitPoints = Math.Max(0, target.Level - 39 - target.SpentPoints),
                Level = target.Level,

                Hair = target.HairType,
                HairColour = target.HairColour,
                Items = new List<ClientUserItem>(),
                ObserverPacket = false,
            };

            if (target.Account.GuildMember != null)
            {
                packet.GuildName = target.Account.GuildMember.Guild.GuildName;
                packet.GuildRank = target.Account.GuildMember.Rank;
            }

            if (target.Player != null)
            {
                packet.WearWeight = target.Player.WearWeight;
                packet.HandWeight = target.Player.HandWeight;
            }


            foreach (UserItem item in target.Items)
            {
                if (item == null || item.Slot < 0 || item.Slot < Globals.EquipmentOffSet) continue;

                ClientUserItem clientItem = item.ToClientInfo();
                clientItem.Slot -= Globals.EquipmentOffSet;

                packet.Items.Add(clientItem);
            }

            con.Enqueue(packet);
        }

        public override void CelestialLightActivate()
        {
            base.CelestialLightActivate();

            UserMagic magic;

            if (!Magics.TryGetValue(MagicType.CelestialLight, out magic)) return;

            magic.Cooldown = SEnvir.Now.AddSeconds(6);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 6000 });
        }
        public override void ItemRevive()
        {
            base.ItemRevive();

            Character.ItemReviveTime = ItemReviveTime;

            UpdateReviveTimers(Connection);
        }

        public void UpdateReviveTimers(SConnection con)
        {
            con.Enqueue(new S.ReviveTimers
            {
                ItemReviveTime = ItemReviveTime > SEnvir.Now ? ItemReviveTime - SEnvir.Now : TimeSpan.Zero,
                ReincarnationPillTime = Character.ReincarnationPillTime > SEnvir.Now ? Character.ReincarnationPillTime - SEnvir.Now : TimeSpan.Zero,
            });
        }


        public void GainExperience(decimal amount, bool huntGold, int gainLevel = Int32.MaxValue, bool rateEffected = true)
        {
            if (rateEffected)
            {
                amount *= 1M + Stats[Stat.ExperienceRate] / 100M;

                amount *= 1M + Stats[Stat.BaseExperienceRate] / 100M;

                for (int i = 0; i < Character.Rebirth; i++)
                    amount *= 0.5M;
            }

            /*
            if (Level >= 60)
            {
    
                if (Level > gainLevel)
                    amount -= Math.Min(amount, amount * Math.Min(0.9M, (Level - gainLevel) * 0.10M));
            }
            else
            {
                if (Level > gainLevel)
                    amount -= Math.Min(amount, amount * Math.Min(0.3M, (Level - gainLevel) * 0.06M));
            }
            */

            if (amount == 0) return;

            Experience += amount;
            Enqueue(new S.GainedExperience { Amount = amount });

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon != null && weapon.Info.Effect != ItemEffect.PickAxe && (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable && (weapon.Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && weapon.Level < Globals.WeaponExperienceList.Count && rateEffected)
            {
                weapon.Experience += amount / 10;

                if (weapon.Experience >= Globals.WeaponExperienceList[weapon.Level])
                {
                    weapon.Experience = 0;
                    weapon.Level++;

                    if (weapon.Level < Globals.WeaponExperienceList.Count)
                        weapon.Flags |= UserItemFlags.Refinable;
                }
            }


            if (huntGold)
            {
                BuffInfo buff = Buffs.First(x => x.Type == BuffType.HuntGold);

                if (buff.Stats[Stat.AvailableHuntGold] > 0)
                {
                    buff.Stats[Stat.AvailableHuntGold]--;
                    Character.Account.HuntGold++;
                    Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });
                    Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                }
            }

            if (Level >= Config.MaxLevel || Experience < MaxExperience)
            {
                SEnvir.RankingSort(Character);
                return;
            }

            Experience -= MaxExperience;


            Level++;
            LevelUp();
        }
        public void LevelUp()
        {
            RefreshStats();

            SetHP(Stats[Stat.Health]);
            SetMP(Stats[Stat.Mana]);

            Enqueue(new S.LevelChanged { Level = Level, Experience = Experience, MaxExperience = MaxExperience });
            Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });

            SEnvir.RankingSort(Character);

            if (Character.Account.Characters.Max(x => x.Level) <= Level)
                BuffRemove(BuffType.Veteran);

            ApplyGuildBuff();
        }

        public void RefreshWeight()
        {
            BagWeight = 0;

            foreach (UserItem item in Inventory)
            {
                if (item == null) continue;

                BagWeight += item.Weight;
            }

            WearWeight = 0;
            HandWeight = 0;

            foreach (UserItem item in Equipment)
            {
                if (item == null) continue;

                switch (item.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Torch:
                        HandWeight += item.Weight;
                        break;
                    default:
                        WearWeight += item.Weight;
                        break;
                }
            }

            Enqueue(new S.WeightUpdate { BagWeight = BagWeight, WearWeight = WearWeight, HandWeight = HandWeight });
        }
        public override void RefreshStats()
        {
            int tracking = Stats[Stat.BossTracker] + Stats[Stat.PlayerTracker];

            Stats.Clear();

            AddBaseStats();

            switch (Character.Account.Horse)
            {
                case HorseType.Brown:
                    Stats[Stat.BagWeight] += 50;
                    break;
                case HorseType.White:
                    Stats[Stat.Comfort] += 2;
                    Stats[Stat.BagWeight] += 100;
                    Stats[Stat.MaxAC] += 5;
                    Stats[Stat.MaxMR] += 5;
                    Stats[Stat.MaxDC] += 5;
                    Stats[Stat.MaxMC] += 5;
                    Stats[Stat.MaxSC] += 5;
                    break;
                case HorseType.Red:
                    Stats[Stat.Comfort] += 5;
                    Stats[Stat.BagWeight] += 150;
                    Stats[Stat.MaxAC] += 12;
                    Stats[Stat.MaxMR] += 12;
                    Stats[Stat.MaxDC] += 12;
                    Stats[Stat.MaxMC] += 12;
                    Stats[Stat.MaxSC] += 12;
                    break;
                case HorseType.Black:
                    Stats[Stat.Comfort] += 7;
                    Stats[Stat.BagWeight] += 200;
                    Stats[Stat.MaxAC] += 25;
                    Stats[Stat.MaxMR] += 25;
                    Stats[Stat.MaxDC] += 25;
                    Stats[Stat.MaxMC] += 25;
                    Stats[Stat.MaxSC] += 25;
                    break;
            }

            Dictionary<SetInfo, List<ItemInfo>> sets = new Dictionary<SetInfo, List<ItemInfo>>();

            foreach (UserItem item in Equipment)
            {
                if (item == null || (item.CurrentDurability == 0 && item.Info.Durability > 0)) continue;

                if (item.Info.Set != null)
                {
                    List<ItemInfo> items;
                    if (!sets.TryGetValue(item.Info.Set, out items))
                        sets[item.Info.Set] = items = new List<ItemInfo>();

                    if (!items.Contains(item.Info))
                        items.Add(item.Info);
                }

                if (item.Info.ItemType == ItemType.HorseArmour && Character.Account.Horse == HorseType.None) continue;

                Stats.Add(item.Info.Stats, item.Info.ItemType != ItemType.Weapon);
                Stats.Add(item.Stats, item.Info.ItemType != ItemType.Weapon);

                if (item.Info.ItemType == ItemType.Weapon)
                {
                    Stat ele = item.Stats.GetWeaponElement();

                    if (ele == Stat.None)
                        ele = item.Info.Stats.GetWeaponElement();

                    if (ele != Stat.None)
                        Stats[ele] += item.Stats.GetWeaponElementValue() + item.Info.Stats.GetWeaponElementValue();
                }
            }


            if (GroupMembers != null && GroupMembers.Count >= 8)
            {
                int warrior = 0, wizard = 0, taoist = 0, assassin = 0;

                foreach (PlayerObject ob in GroupMembers)
                {
                    switch (ob.Class)
                    {
                        case MirClass.Warrior:
                            warrior++;
                            break;
                        case MirClass.Wizard:
                            wizard++;
                            break;
                        case MirClass.Taoist:
                            taoist++;
                            break;
                        case MirClass.Assassin:
                            assassin++;
                            break;
                    }
                }

                if (warrior >= 2 && wizard >= 2 && taoist >= 2 && assassin >= 2)
                {
                    Stats[Stat.Health] += Stats[Stat.BaseHealth] / 10;
                    Stats[Stat.Mana] += Stats[Stat.BaseMana] / 10;
                }
            }


            foreach (KeyValuePair<MagicType, UserMagic> pair in Magics)
            {
                if (Level < pair.Value.Info.NeedLevel1) continue;

                switch (pair.Key)
                {
                    case MagicType.Swordsmanship:
                        Stats[Stat.Accuracy] += pair.Value.GetPower();
                        break;
                    case MagicType.SpiritSword:
                        Stats[Stat.Accuracy] += pair.Value.GetPower();
                        break;
                    case MagicType.Slaying:
                        Stats[Stat.Accuracy] += pair.Value.Level * 2;
                        Stats[Stat.MinDC] += pair.Value.Level * 2;
                        Stats[Stat.MaxDC] += pair.Value.Level * 2;
                        break;
                    case MagicType.WillowDance:
                        Stats[Stat.Agility] += pair.Value.GetPower();
                        break;
                    case MagicType.VineTreeDance:
                        Stats[Stat.Accuracy] += pair.Value.GetPower();
                        break;
                    case MagicType.Discipline:
                        Stats[Stat.Accuracy] += pair.Value.GetPower() / 3;
                        Stats[Stat.MinDC] += pair.Value.GetPower();
                        break;
                    case MagicType.AdventOfDemon:
                        Stats[Stat.MaxAC] += pair.Value.GetPower();
                        break;
                    case MagicType.AdventOfDevil:
                        Stats[Stat.MaxMR] += pair.Value.GetPower();
                        break;
                    case MagicType.BloodyFlower:
                    case MagicType.AdvancedBloodyFlower:
                        Stats[Stat.LifeSteal] += pair.Value.GetPower();
                        break;
                    case MagicType.AdvancedRenounce:
                        Stats[Stat.MCPercent] += (1 + pair.Value.Level) * 10;
                        break;
                }
            }

            foreach (BuffInfo buff in Buffs)
            {
                if (buff.Pause) continue;

                if (buff.Type == BuffType.ItemBuff)
                {
                    Stats.Add(SEnvir.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).Stats);
                    continue;
                }

                if (buff.Stats == null) continue;

                Stats.Add(buff.Stats);
            }

            foreach (KeyValuePair<SetInfo, List<ItemInfo>> pair in sets)
            {
                if (pair.Key.Items.Count != pair.Value.Count) continue;


                foreach (SetInfoStat stat in pair.Key.SetStats)
                {
                    if (Level < stat.Level) continue;

                    switch (Class)
                    {
                        case MirClass.Warrior:
                            if ((stat.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                            break;
                        case MirClass.Wizard:
                            if ((stat.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                            break;
                        case MirClass.Taoist:
                            if ((stat.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                            break;
                        case MirClass.Assassin:
                            if ((stat.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                            break;
                    }

                    Stats[stat.Stat] += stat.Amount;
                }
            }

            UserMagic magic;
            if (Buffs.Any(x => x.Type == BuffType.RagingWind) && Magics.TryGetValue(MagicType.RagingWind, out magic))
            {
                int power = Stats[Stat.MinAC] + Stats[Stat.MaxAC] + 4 + magic.Level * 6;

                Stats[Stat.MinAC] = power * 3 / 10;
                Stats[Stat.MaxAC] = power - Stats[Stat.MinAC];

                power = Stats[Stat.MinMR] + Stats[Stat.MaxMR] + 4 + magic.Level * 6;

                Stats[Stat.MinMR] = power * 3 / 10;
                Stats[Stat.MaxMR] = power - Stats[Stat.MinMR];
            }

            Stats[Stat.AttackSpeed] += Math.Min(3, Level / 15);

            Stats[Stat.FireResistance] = Math.Min(5, Stats[Stat.FireResistance]);
            Stats[Stat.IceResistance] = Math.Min(5, Stats[Stat.IceResistance]);
            Stats[Stat.LightningResistance] = Math.Min(5, Stats[Stat.LightningResistance]);
            Stats[Stat.WindResistance] = Math.Min(5, Stats[Stat.WindResistance]);
            Stats[Stat.HolyResistance] = Math.Min(5, Stats[Stat.HolyResistance]);
            Stats[Stat.DarkResistance] = Math.Min(5, Stats[Stat.DarkResistance]);
            Stats[Stat.PhantomResistance] = Math.Min(5, Stats[Stat.PhantomResistance]);
            Stats[Stat.PhysicalResistance] = Math.Min(5, Stats[Stat.PhysicalResistance]);


            Stats[Stat.Comfort] = Math.Min(20, Stats[Stat.Comfort]);
            Stats[Stat.AttackSpeed] = Math.Min(15, Stats[Stat.AttackSpeed]);

            RegenDelay = TimeSpan.FromMilliseconds(15000 - Stats[Stat.Comfort] * 650);

            Stats[Stat.Health] += (Stats[Stat.Health] * Stats[Stat.HealthPercent]) / 100;
            Stats[Stat.Mana] += (Stats[Stat.Mana] * Stats[Stat.ManaPercent]) / 100;

            Stats[Stat.MinDC] += (Stats[Stat.MinDC] * Stats[Stat.DCPercent]) / 100;
            Stats[Stat.MaxDC] += (Stats[Stat.MaxDC] * Stats[Stat.DCPercent]) / 100;

            Stats[Stat.MinMC] += (Stats[Stat.MinMC] * Stats[Stat.MCPercent]) / 100;
            Stats[Stat.MaxMC] += (Stats[Stat.MaxMC] * Stats[Stat.MCPercent]) / 100;

            Stats[Stat.MinSC] += (Stats[Stat.MinSC] * Stats[Stat.SCPercent]) / 100;
            Stats[Stat.MaxSC] += (Stats[Stat.MaxSC] * Stats[Stat.SCPercent]) / 100;

            Stats[Stat.Health] = Math.Max(10, Stats[Stat.Health]);
            Stats[Stat.Mana] = Math.Max(10, Stats[Stat.Mana]);

            if (Stats[Stat.Defiance] > 0)
            {
                Stats[Stat.MinAC] = Stats[Stat.MaxAC];
                Stats[Stat.MinMR] = Stats[Stat.MaxMR];
            }

            if (Buffs.Any(x => x.Type == BuffType.MagicWeakness))
            {
                Stats[Stat.MinMR] = 0;
                Stats[Stat.MaxMR] = 0;
            }



            Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC]);
            Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC]);
            Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR]);
            Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR]);
            Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC]);
            Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC]);
            Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC]);
            Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC]);

            Stats[Stat.MinDC] = Math.Min(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Min(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Min(Stats[Stat.MinSC], Stats[Stat.MaxSC]);

            Stats[Stat.HandWeight] += Stats[Stat.HandWeight] * Stats[Stat.WeightRate];
            Stats[Stat.WearWeight] += Stats[Stat.WearWeight] * Stats[Stat.WeightRate];
            Stats[Stat.BagWeight] += Stats[Stat.BagWeight] * Stats[Stat.WeightRate];

            Stats[Stat.Rebirth] = Character.Rebirth;

            Stats[Stat.DropRate] += 20 * Stats[Stat.Rebirth];
            Stats[Stat.GoldRate] += 20 * Stats[Stat.Rebirth];

            Enqueue(new S.StatsUpdate { Stats = Stats, HermitStats = Character.HermitStats, HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints) });

            S.DataObjectMaxHealthMana p = new S.DataObjectMaxHealthMana { ObjectID = ObjectID, MaxHealth = Stats[Stat.Health], MaxMana = Stats[Stat.Mana] };

            foreach (PlayerObject player in DataSeenByPlayers)
                player.Enqueue(p);

            RefreshWeight();

            if (CurrentHP > Stats[Stat.Health]) SetHP(Stats[Stat.Health]);
            if (CurrentMP > Stats[Stat.Mana]) SetMP(Stats[Stat.Mana]);

            if (Spawned && tracking != Stats[Stat.PlayerTracker] + Stats[Stat.BossTracker])
            {
                RemoveAllObjects();
                AddAllObjects();
            }
        }
        public void AddBaseStats()
        {
            MaxExperience = Level < Globals.ExperienceList.Count ? Globals.ExperienceList[Level] : 0;

            BaseStat stat = null;

            //Get best possible match.
            foreach (BaseStat bStat in SEnvir.BaseStatList.Binding)
            {
                if (bStat.Class != Class) continue;
                if (bStat.Level > Level) continue;
                if (stat != null && bStat.Level < stat.Level) continue;

                stat = bStat;

                if (bStat.Level == Level) break;
            }

            if (stat == null) return;

            Stats[Stat.Health] = stat.Health;
            Stats[Stat.Mana] = stat.Mana;

            Stats[Stat.BagWeight] = stat.BagWeight;
            Stats[Stat.WearWeight] = stat.WearWeight;
            Stats[Stat.HandWeight] = stat.HandWeight;

            Stats[Stat.Accuracy] = stat.Accuracy;

            Stats[Stat.Agility] = stat.Agility;

            Stats[Stat.MinAC] = stat.MinAC;
            Stats[Stat.MaxAC] = stat.MaxAC;

            Stats[Stat.MinMR] = stat.MinMR;
            Stats[Stat.MaxMR] = stat.MaxMR;

            Stats[Stat.MinDC] = stat.MinDC;
            Stats[Stat.MaxDC] = stat.MaxDC;

            Stats[Stat.MinMC] = stat.MinMC;
            Stats[Stat.MaxMC] = stat.MaxMC;

            Stats[Stat.MinSC] = stat.MinSC;
            Stats[Stat.MaxSC] = stat.MaxSC;


            Stats[Stat.PickUpRadius] = 1;
            Stats[Stat.SkillRate] = 1;
            Stats[Stat.CriticalChance] = 1;

            Stats.Add(Character.HermitStats);

            Stats[Stat.BaseHealth] = Stats[Stat.Health];
            Stats[Stat.BaseMana] = Stats[Stat.Mana];
        }
        public void AssignHermit(Stat stat)
        {
            if (Level - 39 - Character.SpentPoints <= 0) return;

            switch (stat)
            {
                case Stat.MaxDC:
                case Stat.MaxMC:
                case Stat.MaxSC:
                    Character.HermitStats[stat] += 2 + Character.SpentPoints / 10;
                    break;
                case Stat.MaxAC:
                    Character.HermitStats[Stat.MinAC] += 2;
                    Character.HermitStats[Stat.MaxAC] += 2;
                    break;
                case Stat.MaxMR:
                    Character.HermitStats[Stat.MinMR] += 2;
                    Character.HermitStats[Stat.MaxMR] += 2;
                    break;
                case Stat.Health:
                    Character.HermitStats[stat] += 10 + (Character.SpentPoints / 10) * 10;
                    break;
                case Stat.Mana:
                    Character.HermitStats[stat] += 15 + (Character.SpentPoints / 10) * 15;
                    break;
                case Stat.WeaponElement:

                    if (Character.SpentPoints >= 20) return;

                    int count = 2 + Character.SpentPoints / 10;

                    List<Stat> Elements = new List<Stat>();

                    if (Stats[Stat.FireAttack] > 0) Elements.Add(Stat.FireAttack);
                    if (Stats[Stat.IceAttack] > 0) Elements.Add(Stat.IceAttack);
                    if (Stats[Stat.LightningAttack] > 0) Elements.Add(Stat.LightningAttack);
                    if (Stats[Stat.WindAttack] > 0) Elements.Add(Stat.WindAttack);
                    if (Stats[Stat.HolyAttack] > 0) Elements.Add(Stat.HolyAttack);
                    if (Stats[Stat.DarkAttack] > 0) Elements.Add(Stat.DarkAttack);
                    if (Stats[Stat.PhantomAttack] > 0) Elements.Add(Stat.PhantomAttack);

                    if (Elements.Count == 0)
                        Elements.AddRange(new[]
                        {
                            Stat.FireAttack,
                            Stat.IceAttack,
                            Stat.LightningAttack,
                            Stat.WindAttack,
                            Stat.HolyAttack,
                            Stat.DarkAttack,
                            Stat.PhantomAttack,
                        });

                    for (int i = 0; i < count; i++)
                        Character.HermitStats[Elements[SEnvir.Random.Next(Elements.Count)]]++;
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "Attempted to Exploit hermit.";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            Character.SpentPoints++;
            RefreshStats();
        }

        public override void DeActivate()
        {
            return;
        }

        #region Objects View
        public override void AddAllObjects()
        {
            base.AddAllObjects();

            int minX = Math.Max(0, CurrentLocation.X - Config.MaxViewRange);
            int maxX = Math.Min(CurrentMap.Width - 1, CurrentLocation.X + Config.MaxViewRange);

            for (int i = minX; i <= maxX; i++)
                foreach (MapObject ob in CurrentMap.OrderedObjects[i])
                {
                    if (ob.IsNearBy(this))
                        AddNearBy(ob);
                }

            foreach (MapObject ob in NearByObjects)
            {
                if (ob.CanBeSeenBy(this))
                    AddObject(ob);

                if (ob.CanDataBeSeenBy(this))
                    AddDataObject(ob);
            }

            if (Stats[Stat.BossTracker] > 0)
            {
                foreach (MonsterObject ob in CurrentMap.Bosses)
                {
                    if (ob.CanDataBeSeenBy(this))
                        AddDataObject(ob);
                }
            }


            foreach (PlayerObject ob in SEnvir.Players)
            {
                if (ob.CanDataBeSeenBy(this))
                    AddDataObject(ob);
            }
        }
        public override void RemoveAllObjects()
        {
            base.RemoveAllObjects();

            HashSet<MapObject> templist = new HashSet<MapObject>();

            foreach (MapObject ob in VisibleObjects)
            {
                if (ob.CanBeSeenBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveObject(ob);


            templist = new HashSet<MapObject>();
            foreach (MapObject ob in VisibleDataObjects)
            {
                if (ob.CanDataBeSeenBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveDataObject(ob);

            templist = new HashSet<MapObject>();
            foreach (MapObject ob in NearByObjects)
            {
                if (ob.IsNearBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveNearBy(ob);
        }

        public void AddObject(MapObject ob)
        {
            if (ob.SeenByPlayers.Contains(this)) return;

            ob.SeenByPlayers.Add(this);
            VisibleObjects.Add(ob);

            Enqueue(ob.GetInfoPacket(this));
        }
        public void AddNearBy(MapObject ob)
        {
            if (ob.NearByPlayers.Contains(this)) return;

            NearByObjects.Add(ob);
            ob.NearByPlayers.Add(this);

            ob.Activate();
        }
        public void AddDataObject(MapObject ob)
        {
            if (ob.DataSeenByPlayers.Contains(this)) return;

            ob.DataSeenByPlayers.Add(this);
            VisibleDataObjects.Add(ob);

            Enqueue(ob.GetDataPacket(this));
        }
        public void RemoveObject(MapObject ob)
        {
            if (!ob.SeenByPlayers.Contains(this)) return;

            ob.SeenByPlayers.Remove(this);
            VisibleObjects.Remove(ob);

            if (ob == NPC)
            {
                NPC = null;
                NPCPage = null;
            }

            if (ob.Race == ObjectType.Spell)
            {
                SpellObject spell = (SpellObject)ob;

                if (spell.Effect == SpellEffect.Rubble)
                    PauseBuffs();
            }

            if (ob.Race == ObjectType.Monster)
                foreach (MonsterObject mob in TaggedMonsters)
                {
                    if (mob != ob) continue;

                    mob.EXPOwner = null;
                    break;
                }

            Enqueue(new S.ObjectRemove { ObjectID = ob.ObjectID });
        }
        public void RemoveNearBy(MapObject ob)
        {
            if (!ob.NearByPlayers.Contains(this)) return;

            ob.NearByPlayers.Remove(this);
            NearByObjects.Remove(ob);
        }
        public void RemoveDataObject(MapObject ob)
        {
            if (!ob.DataSeenByPlayers.Contains(this)) return;

            ob.DataSeenByPlayers.Remove(this);
            VisibleDataObjects.Remove(ob);

            Enqueue(new S.DataObjectRemove { ObjectID = ob.ObjectID });
        }
        #endregion

        public override bool Teleport(Map map, Point location, bool leaveEffect = true)
        {
            bool res = base.Teleport(map, location, leaveEffect);

            if (res)
            {
                BuffRemove(BuffType.Cloak);
                BuffRemove(BuffType.Transparency);
                Companion?.Recall();
            }

            return res;
        }

        public void TeleportRing(Point location, int MapIndex)
        {
            MapInfo destInfo = SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == MapIndex);

            if (destInfo == null) return;

            if (!Character.Account.TempAdmin)
            {
                if (!Config.TestServer && Stats[Stat.TeleportRing] == 0) return;

                if (!CurrentMap.Info.AllowRT || !CurrentMap.Info.AllowTT) return;

                if (!destInfo.AllowRT || !destInfo.AllowTT) return;

                if (SEnvir.Now < TeleportTime) return;

                TeleportTime = SEnvir.Now.AddSeconds(1);
            }

            Map destMap = SEnvir.GetMap(destInfo, CurrentMap.Instance, CurrentMap.InstanceIndex);

            if (destMap == null)
            {
                return;
            }

            if (!Teleport(destMap, destMap.GetRandomLocation(location, 10, 25))) return;

            TeleportTime = SEnvir.Now.AddMinutes(5);
        }

        public override void Dodged()
        {
            base.Dodged();

            UserMagic magic;

            if (Magics.TryGetValue(MagicType.WillowDance, out magic) && Level >= magic.Info.NeedLevel1)
                LevelMagic(magic);

            //Todo Poison Cloud
        }

        #region Marriage

        public void MarriageRequest()
        {
            if (Character.Partner != null)
            {
                Connection.ReceiveChat(Connection.Language.MarryAlreadyMarried, MessageType.System);
                return;
            }

            if (Level < 22)
            {
                Connection.ReceiveChat(Connection.Language.MarryNeedLevel, MessageType.System);
                return;
            }

            if (Gold < 500000)
            {
                Connection.ReceiveChat(Connection.Language.MarryNeedGold, MessageType.System);
                return;
            }

            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction));

            if (cell?.Objects == null)
            {
                Connection.ReceiveChat(Connection.Language.MarryNotFacing, MessageType.System);
                return;
            }

            PlayerObject player = null;
            foreach (MapObject ob in cell.Objects)
            {
                if (ob.Race != ObjectType.Player) continue;
                player = (PlayerObject)ob;
                break;
            }

            if (player == null || player.Direction != Functions.ShiftDirection(Direction, 4))
            {
                Connection.ReceiveChat(Connection.Language.MarryNotFacing, MessageType.System);
                return;
            }

            if (player.Character.Partner != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTargetAlreadyMarried, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.MarriageInvitation != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTargetHasProposal, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.Level < 22)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTargetNeedLevel, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat(player.Connection.Language.MarryNeedLevel, MessageType.System);
                return;
            }

            if (player.Gold < 500000)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTargetNeedGold, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat(player.Connection.Language.MarryNeedGold, MessageType.System);
                return;
            }
            if (player.Dead || Dead)
            {
                Connection.ReceiveChat(Connection.Language.MarryDead, MessageType.System);
                player.Connection.ReceiveChat(player.Connection.Language.MarryDead, MessageType.System);
                return;
            }

            player.MarriageInvitation = this;
            player.Enqueue(new S.MarriageInvite { Name = Name });
        }
        public void MarriageJoin()
        {
            if (MarriageInvitation != null && MarriageInvitation.Node == null) MarriageInvitation = null;

            if (MarriageInvitation == null || Character.Partner != null || MarriageInvitation.Character.Partner != null) return;

            const int cost = 500000;

            if (Gold < cost)
            {
                Connection.ReceiveChat(Connection.Language.MarryNeedGold, MessageType.System);
                MarriageInvitation.Connection.ReceiveChat(string.Format(MarriageInvitation.Connection.Language.MarryTargetNeedGold, Character.CharacterName), MessageType.System);
                return;
            }

            if (MarriageInvitation.Gold < cost)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTargetNeedGold, MarriageInvitation.Character.CharacterName), MessageType.System);
                MarriageInvitation.Connection.ReceiveChat(MarriageInvitation.Connection.Language.MarryNeedGold, MessageType.System);
                return;
            }

            Character.Partner = MarriageInvitation.Character;

            Connection.ReceiveChat(string.Format(Connection.Language.MarryComplete, MarriageInvitation.Character.CharacterName), MessageType.System);
            MarriageInvitation.Connection.ReceiveChat(string.Format(MarriageInvitation.Connection.Language.MarryComplete, Character.CharacterName), MessageType.System);

            Gold -= cost;
            MarriageInvitation.Gold -= cost;

            GoldChanged();
            MarriageInvitation.GoldChanged();

            AddAllObjects();

            Enqueue(GetMarriageInfo());
            MarriageInvitation.Enqueue(MarriageInvitation.GetMarriageInfo());
        }
        public void MarriageLeave()
        {
            if (Character.Partner == null) return;

            CharacterInfo partner = Character.Partner;

            Character.Partner = null;

            MarriageRemoveRing();
            Connection.ReceiveChat(string.Format(Connection.Language.MarryDivorce, partner.CharacterName), MessageType.System);

            Enqueue(GetMarriageInfo());


            if (partner.Player != null)
            {
                partner.Player.MarriageRemoveRing();
                partner.Player.Connection.ReceiveChat(string.Format(partner.Player.Connection.Language.MarryDivorce, Character.CharacterName), MessageType.System);
                partner.Player.Enqueue(partner.Player.GetMarriageInfo());
            }
            else
                foreach (UserItem item in partner.Items)
                {
                    if (item.Slot != Globals.EquipmentOffSet + (int)EquipmentSlot.RingL) continue;

                    item.Flags &= ~UserItemFlags.Marriage;
                }
        }
        public void MarriageMakeRing(int index)
        {
            if (Character.Partner == null) return; // Not Married

            if (Equipment[(int)EquipmentSlot.RingL] != null && (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (index < 0 || index >= Inventory.Length) return;

            UserItem ring = Inventory[index];

            if (ring == null || ring.Info.ItemType != ItemType.Ring) return;

            ring.Flags |= UserItemFlags.Marriage;

            Inventory[index] = Equipment[(int)EquipmentSlot.RingL];

            if (Inventory[index] != null)
                Inventory[index].Slot = index;

            Equipment[(int)EquipmentSlot.RingL] = ring;
            ring.Slot = Globals.EquipmentOffSet + (int)EquipmentSlot.RingL;

            Enqueue(new S.ItemMove { FromGrid = GridType.Inventory, FromSlot = index, ToGrid = GridType.Equipment, ToSlot = (int)EquipmentSlot.RingL, Success = true });
            Enqueue(new S.MarriageMakeRing());
            RefreshStats();
            Enqueue(new S.NPCClose());
        }
        public void MarriageTeleport()
        {
            if (Character.Partner == null)
            {
                Connection.ReceiveChat("You are not married", MessageType.System);
                return;
            }

            if (Equipment[(int)EquipmentSlot.RingL] == null || (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage)
            {
                Connection.ReceiveChat("Your ring is not married ring", MessageType.System);
                return;
            }

            if (Dead)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportDead, MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportDead, MessageType.System);
                return;
            }

            if (Stats[Stat.PKPoint] >= Config.RedPoint)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportPK, MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportPK, MessageType.System);
                return;
            }

            if (SEnvir.Now < Character.MarriageTeleportTime)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MarryTeleportDelay, Functions.ToString(Character.MarriageTeleportTime - SEnvir.Now, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.MarryTeleportDelay, Functions.ToString(Character.MarriageTeleportTime - SEnvir.Now, true)), MessageType.System);
                return;
            }

            if (Character.Partner.Player == null)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportOffline, MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportOffline, MessageType.System);
                return;
            }
            if (Character.Partner.Player.Dead)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportPartnerDead, MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportPartnerDead, MessageType.System);
                return;
            }

            if (!Character.Partner.Player.CurrentMap.Info.CanMarriageRecall)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportMap, MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportMap, MessageType.System);
                return;
            }

            if (!CurrentMap.Info.AllowTT)
            {
                Connection.ReceiveChat(Connection.Language.MarryTeleportMapEscape, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.MarryTeleportMapEscape, MessageType.System);
                return;
            }


            if (Teleport(Character.Partner.Player.CurrentMap, Character.Partner.Player.CurrentMap.GetRandomLocation(Character.Partner.Player.CurrentLocation, 10)))
                Character.MarriageTeleportTime = SEnvir.Now.AddSeconds(120);
        }

        public void MarriageRemoveRing()
        {
            if (Equipment[(int)EquipmentSlot.RingL] == null || (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage) return;

            Equipment[(int)EquipmentSlot.RingL].Flags &= ~UserItemFlags.Marriage;
            Enqueue(new S.MarriageRemoveRing());
        }
        public S.MarriageInfo GetMarriageInfo()
        {
            return new S.MarriageInfo
            {
                Partner = new ClientPlayerInfo { Name = Character.Partner?.CharacterName, ObjectID = Character.Partner?.Player != null ? Character.Partner.Player.ObjectID : 0 }
            };
        }
        #endregion

        #region Companions

        public void CompanionUnlock(int index)
        {
            S.CompanionUnlock result = new S.CompanionUnlock();
            Enqueue(result);

            CompanionInfo info = SEnvir.CompanionInfoList.Binding.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (info.Available || Character.Account.CompanionUnlocks.Any(x => x.CompanionInfo == info))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.CompanionAppearanceAlready, info.MonsterInfo.MonsterName), MessageType.System);
                return;
            }

            UserItem item = null;
            int slot = 0;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.CompanionTicket) continue;


                item = Inventory[i];
                slot = i;
                break;
            }

            if (item == null)
            {
                Connection.ReceiveChat(Connection.Language.CompanionNeedTicket, MessageType.System);
                return;
            }

            S.ItemChanged changed = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = slot },

                Success = true
            };
            Enqueue(changed);
            if (item.Count > 1)
            {
                item.Count--;
                changed.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[slot] = null;
                item.Delete();
            }

            RefreshWeight();

            result.Index = info.Index;

            UserCompanionUnlock unlock = SEnvir.UserCompanionUnlockList.CreateNewObject();
            unlock.Account = Character.Account;
            unlock.CompanionInfo = info;
        }
        public void CompanionAdopt(C.CompanionAdopt p)
        {
            S.CompanionAdopt result = new S.CompanionAdopt();
            Enqueue(result);

            if (Dead || NPC == null || NPCPage == null) return;

            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;


            CompanionInfo info = SEnvir.CompanionInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);

            if (info == null) return;

            if (!info.Available && Character.Account.CompanionUnlocks.All(x => x.CompanionInfo != info))
            {
                Connection.ReceiveChat(Connection.Language.CompanionAppearanceAlready, MessageType.System);
                return;
            }

            if (info.Price > Gold)
            {
                Connection.ReceiveChat(Connection.Language.CompanionNeedGold, MessageType.System);
                return;
            }

            if (!Globals.GuildNameRegex.IsMatch(p.Name))
            {
                Connection.ReceiveChat(Connection.Language.CompanionBadName, MessageType.System);
                return;
            }

            Gold -= info.Price;
            GoldChanged();

            UserCompanion companion = SEnvir.UserCompanionList.CreateNewObject();

            companion.Account = Character.Account;
            companion.Info = info;
            companion.Level = 1;
            companion.Hunger = 100;
            companion.Name = p.Name;

            result.UserCompanion = companion.ToClientInfo();
        }
        public void CompanionRetrieve(int index)
        {
            if (Dead || NPC == null || NPCPage == null) return;

            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;


            UserCompanion info = Character.Account.Companions.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (info.Character != null)
            {
                if (info.Character != Character)
                    Connection.ReceiveChat(string.Format(Connection.Language.CompanionRetrieveFailed, info.Name, info.Character.CharacterName), MessageType.System);
                return;
            }

            info.Character = Character;

            Enqueue(new S.CompanionStore());
            Enqueue(new S.CompanionRetrieve { Index = index });

            CompanionDespawn();
            CompanionSpawn();

        }
        public void CompanionStore(int index)
        {
            if (Dead || NPC == null || NPCPage == null) return;

            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;

            if (Character.Companion == null) return;

            Character.Companion = null;

            Enqueue(new S.CompanionStore());

            CompanionDespawn();
        }

        public void CompanionSpawn()
        {
            if (Companion != null) return;

            if (Character.Companion == null) return;

            Companion tempCompanion = new Companion(Character.Companion)
            {
                CompanionOwner = this,
            };


            if (tempCompanion.Spawn(CurrentMap, CurrentLocation))
            {
                Companion = tempCompanion;
                CompanionApplyBuff();
            }
        }
        public void CompanionApplyBuff()
        {
            if (Companion.UserCompanion.Hunger <= 0) return;

            Stats buffStats = new Stats();

            if (Companion.UserCompanion.Level >= 3)
                buffStats.Add(Companion.UserCompanion.Level3);

            if (Companion.UserCompanion.Level >= 5)
                buffStats.Add(Companion.UserCompanion.Level5);

            if (Companion.UserCompanion.Level >= 7)
                buffStats.Add(Companion.UserCompanion.Level7);

            if (Companion.UserCompanion.Level >= 10)
                buffStats.Add(Companion.UserCompanion.Level10);

            if (Companion.UserCompanion.Level >= 11)
                buffStats.Add(Companion.UserCompanion.Level11);

            if (Companion.UserCompanion.Level >= 13)
                buffStats.Add(Companion.UserCompanion.Level13);

            if (Companion.UserCompanion.Level >= 15)
                buffStats.Add(Companion.UserCompanion.Level15);

            BuffInfo buff = BuffAdd(BuffType.Companion, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.FromMinutes(1));
            buff.TickTime = TimeSpan.FromMinutes(1); //set to Full Minute
        }
        public void CompanionDespawn()
        {
            if (Companion == null) return;

            BuffRemove(BuffType.Companion);

            Companion.CompanionOwner = null;
            Companion.Despawn();
            Companion = null;
        }
        public void CompanionRefreshBuff()
        {
            if (Companion.UserCompanion.Hunger <= 0) return;

            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.Companion);

            if (buff == null) return;

            Stats buffStats = new Stats();

            if (Companion.UserCompanion.Level >= 3)
                buffStats.Add(Companion.UserCompanion.Level3);

            if (Companion.UserCompanion.Level >= 5)
                buffStats.Add(Companion.UserCompanion.Level5);

            if (Companion.UserCompanion.Level >= 7)
                buffStats.Add(Companion.UserCompanion.Level7);

            if (Companion.UserCompanion.Level >= 10)
                buffStats.Add(Companion.UserCompanion.Level10);

            if (Companion.UserCompanion.Level >= 11)
                buffStats.Add(Companion.UserCompanion.Level11);

            if (Companion.UserCompanion.Level >= 13)
                buffStats.Add(Companion.UserCompanion.Level13);

            if (Companion.UserCompanion.Level >= 15)
                buffStats.Add(Companion.UserCompanion.Level15);


            buff.Stats = buffStats;
            RefreshStats();

            Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buffStats });
        }

        #endregion

        #region Quests

        public void QuestAccept(int index)
        {
            if (Dead || NPC == null) return;

            foreach (QuestInfo quest in NPC.NPCInfo.StartQuests)
            {
                if (quest.Index != index) continue;

                if (!QuestCanAccept(quest)) return;

                UserQuest userQuest = SEnvir.UserQuestList.CreateNewObject();

                userQuest.QuestInfo = quest;
                userQuest.Character = Character;

                Enqueue(new S.QuestChanged { Quest = userQuest.ToClientInfo() });
                break;
            }
        }
        public bool QuestCanAccept(QuestInfo quest)
        {
            if (Character.Quests.Any(x => x.QuestInfo == quest)) return false;

            foreach (QuestRequirement requirement in quest.Requirements)
            {
                switch (requirement.Requirement)
                {
                    case QuestRequirementType.MinLevel:
                        if (Level < requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.MaxLevel:
                        if (Level > requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.NotAccepted:
                        if (Character.Quests.Any(x => x.QuestInfo == requirement.QuestParameter)) return false;

                        break;
                    case QuestRequirementType.HaveCompleted:
                        if (Character.Quests.Any(x => x.QuestInfo == requirement.QuestParameter && x.Completed)) break;

                        return false;
                    case QuestRequirementType.HaveNotCompleted:
                        if (Character.Quests.Any(x => x.QuestInfo == requirement.QuestParameter && x.Completed)) return false;

                        break;
                    case QuestRequirementType.Class:
                        switch (Class)
                        {
                            case MirClass.Warrior:
                                if ((requirement.Class & RequiredClass.Warrior) != RequiredClass.Warrior) return false;

                                break;
                            case MirClass.Wizard:
                                if ((requirement.Class & RequiredClass.Wizard) != RequiredClass.Wizard) return false;
                                break;
                            case MirClass.Taoist:
                                if ((requirement.Class & RequiredClass.Taoist) != RequiredClass.Taoist) return false;
                                break;
                            case MirClass.Assassin:
                                if ((requirement.Class & RequiredClass.Assassin) != RequiredClass.Assassin) return false;
                                break;
                        }
                        break;
                }

            }
            return true;
        }

        public void QuestComplete(C.QuestComplete p)
        {
            if (Dead) return;
            if (Dead || NPC == null) return;

            foreach (QuestInfo quest in NPC.NPCInfo.FinishQuests)
            {
                if (quest.Index != p.Index) continue;

                UserQuest userQuest = Character.Quests.FirstOrDefault(x => x.QuestInfo == quest);

                if (userQuest == null || userQuest.Completed || !userQuest.IsComplete) return;

                List<ItemCheck> checks = new List<ItemCheck>();

                bool hasChoice = false;
                bool hasChosen = false;

                foreach (QuestReward reward in quest.Rewards)
                {
                    switch (Class)
                    {
                        case MirClass.Warrior:
                            if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                            break;
                        case MirClass.Wizard:
                            if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                            break;
                        case MirClass.Taoist:
                            if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                            break;
                        case MirClass.Assassin:
                            if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                            break;
                    }

                    if (reward.Choice)
                    {
                        hasChoice = true;
                        if (reward.Index != p.ChoiceIndex) continue;

                        hasChosen = true;
                    }

                    UserItemFlags flags = UserItemFlags.None;
                    TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                    if (reward.Bound)
                        flags |= UserItemFlags.Bound;

                    if (duration != TimeSpan.Zero)
                        flags |= UserItemFlags.Expirable;

                    ItemCheck check = new ItemCheck(reward.Item, reward.Amount, flags, duration);

                    checks.Add(check);
                }

                if (hasChoice && !hasChosen)
                {
                    Connection.ReceiveChat(Connection.Language.QuestSelectReward, MessageType.System);
                    return;
                }

                if (!CanGainItems(false, checks.ToArray()))
                {
                    Connection.ReceiveChat(Connection.Language.QuestNeedSpace, MessageType.System);
                    return;
                }

                foreach (ItemCheck check in checks)
                {
                    while (check.Count > 0)
                        GainItem(SEnvir.CreateFreshItem(check));
                }

                userQuest.Track = false;
                userQuest.Completed = true;
                if (hasChosen)
                    userQuest.SelectedReward = p.ChoiceIndex;

                Enqueue(new S.QuestChanged { Quest = userQuest.ToClientInfo() });
                break;
            }
        }

        public void QuestTrack(C.QuestTrack p)
        {
            UserQuest quest = Character.Quests.FirstOrDefault(x => x.Index == p.Index);

            if (quest == null || quest.Completed) return;

            quest.Track = p.Track;

        }
        #endregion

        #region Mail

        public void MailGetItem(C.MailGetItem p)
        {
            MailInfo mail = Character.Account.Mail.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null)
            {
                Enqueue(new S.MailDelete { Index = p.Index, ObserverPacket = true });
                return;
            }

            UserItem item = mail.Items.FirstOrDefault(x => x.Slot == p.Slot);

            if (item == null)
            {
                Enqueue(new S.MailItemDelete { Index = p.Index, Slot = p.Slot, ObserverPacket = true });
                return;
            }

            if (!InSafeZone && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat(Connection.Language.MailSafeZone, MessageType.System);
                return;
            }

            if (!CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                Connection.ReceiveChat(Connection.Language.MailNeedSpace, MessageType.System);
                return;
            }

            item.Mail = null;
            GainItem(item);

            Enqueue(new S.MailItemDelete { Index = p.Index, Slot = p.Slot, ObserverPacket = true });
        }
        public void MailDelete(int index)
        {
            MailInfo mail = Character.Account.Mail.FirstOrDefault(x => x.Index == index);

            if (mail == null)
            {
                Enqueue(new S.MailDelete { Index = index, ObserverPacket = true });
                return;
            };

            if (mail.Items.Count > 0)
            {
                Connection.ReceiveChat(Connection.Language.MailHasItems, MessageType.System);
                return;
            }

            mail.Delete();

            Enqueue(new S.MailDelete { Index = index, ObserverPacket = true });
        }
        public void MailSend(C.MailSend p)
        {
            Enqueue(new S.MailSend { ObserverPacket = false });

            S.ItemsChanged result = new S.ItemsChanged { Links = p.Links };

            Enqueue(result);

            if (!ParseLinks(p.Links, 0, 6)) return;

            AccountInfo account = SEnvir.GetCharacter(p.Recipient)?.Account;

            if (account == null || SEnvir.IsBlocking(Character.Account, account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.MailNotFound, p.Recipient), MessageType.System);
                return;
            }

            if (account == Character.Account && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat(Connection.Language.MailSelfMail, MessageType.System);
                return;
            }
            if (p.Gold < 0 || p.Gold > Gold)
            {
                Connection.ReceiveChat(Connection.Language.MailMailCost, MessageType.System);
                return;
            }

            UserItem item;
            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] fromArray;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        if (!InSafeZone && !Character.Account.TempAdmin)
                        {
                            Connection.ReceiveChat(Connection.Language.MailSendSafeZone, MessageType.System);
                            return;
                        }
                        fromArray = Inventory;
                        break;
                    case GridType.PartsStorage:
                        fromArray = PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        if (!InSafeZone && !Character.Account.TempAdmin)
                        {
                            Connection.ReceiveChat(Connection.Language.MailSendSafeZone, MessageType.System);
                            return;
                        }

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;

                item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count) return;
                if (((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound || !item.Info.CanTrade) && !account.Admin && !Character.Account.Admin) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
                //Success
            }

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            mail.Account = account;
            mail.Sender = Name;
            mail.Subject = p.Subject;
            mail.Message = p.Message;

            result.Success = true;

            if (p.Gold > 0)
            {
                Gold -= p.Gold;
                GoldChanged();

                item = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                item.Count = p.Gold;
                item.Slot = mail.Items.Count;
                item.Mail = mail;
            }

            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.PartsStorage:
                        fromArray = PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        fromArray = Companion.Inventory;
                        break;
                }

                item = fromArray[link.Slot];

                if (link.Count == item.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                }
                else
                {
                    item.Count -= link.Count;

                    item = SEnvir.CreateFreshItem(item);
                    item.Count = link.Count;
                }

                item.Slot = mail.Items.Count;
                item.Mail = mail;
            }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            mail.HasItem = mail.Items.Count > 0;

            if (account.Connection?.Player != null)
                account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });
        }

        #endregion

        #region MarketPlace

        public void MarketPlaceConsign(C.MarketPlaceConsign p)
        {
            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link,
            };
            Enqueue(result);

            if (!ParseLinks(p.Link)) return;

            if (p.Message.Length > 150) return;

            UserItem[] array;
            switch (p.Link.GridType)
            {
                case GridType.Inventory:
                    array = Inventory;
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.ConsignSafeZone, MessageType.System);
                        return;
                    }
                    break;
                case GridType.PartsStorage:
                    array = PartsStorage;
                    break;
                case GridType.Storage:
                    array = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    array = Companion.Inventory;
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.ConsignSafeZone, MessageType.System);
                        return;
                    }
                    break;
                default:
                    return;
            }

            if (p.Link.Slot < 0 || p.Link.Slot >= array.Length) return;
            UserItem item = array[p.Link.Slot];

            if (item == null || p.Link.Count > item.Count) return; //trying to sell more than owned.

            if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;
            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
            if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;


            if (p.Price <= 0) return; // Buy Out Less than 1

            int cost = 0;//(int) Math.Min(int.MaxValue, p.Price*Globals.MarketPlaceTax*p.Link.Count + Globals.MarketPlaceFee);

            if (Character.Account.Auctions.Count >= Character.Account.HightestLevel() * 3 + Character.Account.StorageSize - Globals.StorageSize)
            {
                Connection.ReceiveChat(Connection.Language.ConsignLimit, MessageType.System);
                return;
            }

            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignGuildFundsGuild, MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMarket) != GuildPermission.FundsMarket)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignGuildFundsPermission, MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignGuildFundsCost, MessageType.System);
                    return;
                }

                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat(string.Format(Connection.Language.ConsignGuildFundsUsed, Name, cost, item.Info.ItemName, result.Link.Count, p.Price), MessageType.System);
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignCost, MessageType.System);
                    return;
                }

                Gold -= cost;
                GoldChanged();
            }

            UserItem auctionItem;

            if (p.Link.Count == item.Count)
            {
                auctionItem = item;
                RemoveItem(item);
                array[p.Link.Slot] = null;

                result.Link.Count = 0;
            }
            else
            {
                auctionItem = SEnvir.CreateFreshItem(item);
                auctionItem.Count = p.Link.Count;
                item.Count -= p.Link.Count;

                result.Link.Count = item.Count;
            }

            RefreshWeight();
            Companion?.RefreshWeight();

            AuctionInfo auction = SEnvir.AuctionInfoList.CreateNewObject();

            auction.Account = Character.Account;

            auction.Price = p.Price;

            auction.Item = auctionItem;
            auction.Character = Character;
            auction.Message = p.Message;

            result.Success = true;

            Enqueue(new S.MarketPlaceConsign { Consignments = new List<ClientMarketPlaceInfo> { auction.ToClientInfo(Character.Account) }, ObserverPacket = false });
            Connection.ReceiveChat(Connection.Language.ConsignComplete, MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(con.Language.ConsignComplete, MessageType.System);
        }
        public void MarketPlaceCancelConsign(C.MarketPlaceCancelConsign p)
        {
            if (p.Count <= 0) return;

            AuctionInfo info = Character.Account.Auctions?.FirstOrDefault(x => x.Index == p.Index);

            if (info == null) return;

            if (info.Item == null)
            {
                Connection.ReceiveChat(Connection.Language.ConsignAlreadySold, MessageType.System);
                return;
            }

            if (info.Item.Count < p.Count)
            {
                Connection.ReceiveChat(Connection.Language.ConsignNotEnough, MessageType.System);
                return;
            }

            UserItem item = info.Item;

            if (info.Item.Count > p.Count)
            {
                info.Item.Count -= p.Count;

                item = SEnvir.CreateFreshItem(info.Item);
                item.Count = p.Count;
            }
            else
                info.Item = null;

            if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = Character.Account;
                mail.Subject = "Listing Cancelled";
                mail.Message = string.Format("You canceled your sale of '{0}{1}' and was not able to collect the item.", item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count);
                mail.Sender = "Market Place";
                item.Mail = mail;
                item.Slot = 0;
                mail.HasItem = true;

                Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });
            }
            else
            {
                GainItem(item);
            }


            if (info.Item == null)
                info.Delete();

            Enqueue(new S.MarketPlaceConsignChanged { Index = info.Index, Count = info.Item?.Count ?? 0, ObserverPacket = false, });
        }

        public void MarketPlaceBuy(C.MarketPlaceBuy p)
        {
            if (p.Count <= 0) return;

            S.MarketPlaceBuy result = new S.MarketPlaceBuy
            {
                ObserverPacket = false,
            };

            Enqueue(result);

            AuctionInfo info = Connection.MPSearchResults.FirstOrDefault(x => x.Index == p.Index);

            if (info == null) return;

            if (info.Item == null)
            {
                Connection.ReceiveChat(Connection.Language.ConsignAlreadySold, MessageType.System);
                return;
            }

            if (info.Account == Character.Account && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat(Connection.Language.ConsignBuyOwnItem, MessageType.System);
                return;
            }

            if (info.Item.Count < p.Count)
            {
                Connection.ReceiveChat(Connection.Language.ConsignNotEnough, MessageType.System);
                return;
            }


            long cost = p.Count;

            cost *= info.Price;


            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignBuyGuildFundsGuild, MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMarket) != GuildPermission.FundsMarket)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignBuyGuildFundsPermission, MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignBuyGuildFundsCost, MessageType.System);
                    return;
                }

                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat(string.Format(member.Account.Connection.Language.ConsignBuyGuildFundsUsed, Name, cost, info.Item.Info.ItemName, p.Count, info.Price), MessageType.System);
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat(Connection.Language.ConsignBuyCost, MessageType.System);
                    return;
                }

                Gold -= cost;
                GoldChanged();
            }


            UserItem item = info.Item;

            if (info.Item.Count > p.Count)
            {
                info.Item.Count -= p.Count;

                item = SEnvir.CreateFreshItem(info.Item);
                item.Count = p.Count;
            }
            else
                info.Item = null;

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            mail.Account = info.Account;

            long tax = (long)(cost * Globals.MarketPlaceTax);

            mail.Subject = "Listing Sale";
            mail.Sender = "Market Place";

            ItemInfo itemInfo = item.Info;
            int partIndex = item.Stats[Stat.ItemIndex];

            string itemName;

            if (item.Info.Effect == ItemEffect.ItemPart)
                itemName = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex).ItemName + " - [Part]";
            else
                itemName = item.Info.ItemName;

            mail.Message = $"You have sold an item\n\n" +
                           string.Format("Buyer: {0}\n", Name) +
                           string.Format("Item: {0} x{1}\n", itemName, p.Count) +
                           string.Format("Price: {0:#,##0} each\n", info.Price) +
                           string.Format("Sub Total: {0:#,##0}\n\n", cost) +
                           string.Format("Tax: {0:#,##0} ({1:p0})\n\n", tax, Globals.MarketPlaceTax) +
                           string.Format("Total: {0:#,##0}", cost - tax);

            UserItem gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
            gold.Count = (long)(cost - tax);

            gold.Mail = mail;
            gold.Slot = 0;
            mail.HasItem = true;


            if (info.Account.Connection?.Player != null)
                info.Account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });


            item.Flags |= UserItemFlags.Locked;

            if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = Character.Account;

                mail.Subject = "Item Purchase";
                mail.Sender = "Market Place";
                mail.Message = string.Format("You purshased '{0}{1}' and was not able to collect the item.", itemName, item.Count == 1 ? "" : "x" + item.Count);

                item.Mail = mail;
                item.Slot = 0;
                mail.HasItem = true;

                Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });
            }
            else
            {
                GainItem(item);
            }

            result.Index = info.Index;
            result.Count = info.Item?.Count ?? 0;
            result.Success = true;

            AuctionHistoryInfo history = SEnvir.AuctionHistoryInfoList.Binding.FirstOrDefault(x => x.Info == itemInfo.Index && x.PartIndex == partIndex) ?? SEnvir.AuctionHistoryInfoList.CreateNewObject();

            history.Info = itemInfo.Index;
            history.PartIndex = partIndex;
            history.SaleCount += p.Count;
            history.LastPrice = info.Price;

            for (int i = history.Average.Length - 2; i >= 0; i--)
                history.Average[i + 1] = history.Average[i];

            history.Average[0] = info.Price; //Only care about the price per transaction


            if (info.Account.Connection?.Player != null)
                info.Account.Connection.Enqueue(new S.MarketPlaceConsignChanged { Index = info.Index, Count = info.Item?.Count ?? 0, ObserverPacket = false, });

            if (info.Item == null)
                info.Delete();
        }

        public void MarketPlaceStoreBuy(C.MarketPlaceStoreBuy p)
        {
            if (p.Count <= 0) return;

            S.MarketPlaceStoreBuy result = new S.MarketPlaceStoreBuy
            {
                ObserverPacket = false,
            };

            Enqueue(result);

            StoreInfo info = SEnvir.StoreInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);

            if (info?.Item == null) return;

            if (!info.Available)
            {
                Connection.ReceiveChat(Connection.Language.StoreNotAvailable, MessageType.System);
                return;
            }

            p.Count = Math.Min(p.Count, info.Item.StackSize);

            long cost = p.Count;

            int price = p.UseHuntGold ? (info.HuntGoldPrice == 0 ? info.Price : info.HuntGoldPrice) : info.Price;

            cost *= price;


            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(info.Duration);

            if (p.UseHuntGold || Character.Account.HightestLevel() < 40)
                flags |= UserItemFlags.Bound;

            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;

            flags |= UserItemFlags.Locked;

            ItemCheck check = new ItemCheck(info.Item, p.Count, flags, duration);

            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat(Connection.Language.StoreNeedSpace, MessageType.System);
                return;
            }

            if (!Config.TestServer)
            {
                if (p.UseHuntGold)
                {
                    if (cost > Character.Account.HuntGold)
                    {
                        Connection.ReceiveChat(Connection.Language.StoreCost, MessageType.System);
                        return;
                    }

                    Character.Account.HuntGold -= (int)cost;
                    Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });
                }
                else
                {
                    if (cost > Character.Account.GameGold)
                    {
                        Connection.ReceiveChat(Connection.Language.StoreCost, MessageType.System);
                        return;
                    }

                    Character.Account.GameGold -= (int)cost;
                    Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold, ObserverPacket = false });
                }
            }


            UserItem item = SEnvir.CreateFreshItem(check);

            GainItem(item);



            GameStoreSale sale = SEnvir.GameStoreSaleList.CreateNewObject();

            sale.Item = info.Item;
            sale.Account = Character.Account;
            sale.Count = p.Count;
            sale.Price = price;
            sale.HuntGold = p.UseHuntGold;
        }

        public void MarketPlaceCancelSuperior()
        {
            for (int i = SEnvir.AuctionInfoList.Count - 1; i >= 0; i--)
            {
                AuctionInfo info = SEnvir.AuctionInfoList[i];

                if (info.Item == null) continue;

                if ((info.Item.Info.ItemType != ItemType.ItemPart) &&
                   (info.Item.Info.RequiredType != RequiredType.Level || info.Item.Info.RequiredAmount < 40 || info.Item.Info.RequiredAmount > 56)) continue;

                UserItem item = info.Item;

                info.Item = null;

                MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = info.Account;
                mail.Subject = "Listing Cancelled";
                mail.Message = "Your listing was canceled because of Item change(s).";
                mail.Sender = "System";
                item.Mail = mail;
                item.Slot = 0;
                mail.HasItem = true;

                info.Account.Connection?.Player?.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });

                if (info.Item == null)
                    info.Delete();
            }
        }

        #endregion

        #region Guild

        public void GuildCreate(C.GuildCreate p)
        {
            Enqueue(new S.GuildCreate { ObserverPacket = false });

            if (Character.Account.GuildMember != null) return;

            if (p.Members < 0 || p.Members > 100) return;
            if (p.Storage < 0 || p.Storage > 500) return;

            long cost = p.Members * Globals.GuildMemberCost + p.Storage * Globals.GuildStorageCost;

            if (p.UseGold)
                cost += Globals.GuildCreationCost;
            else
            {
                bool result = false;
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.UmaKingHorn) continue;

                    result = true;
                    break;
                }

                if (!result)
                {
                    Connection.ReceiveChat(Connection.Language.GuildNeedHorn, MessageType.System);
                    return;
                }
            }

            if (cost > Gold)
            {
                Connection.ReceiveChat(Connection.Language.GuildNeedGold, MessageType.System);
                return;
            }


            if (!Globals.GuildNameRegex.IsMatch(p.Name))
            {
                Connection.ReceiveChat(Connection.Language.GuildBadName, MessageType.System);
                return;
            }


            GuildInfo info = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);

            if (info != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildNameTaken, MessageType.System);
                return;
            }

            info = SEnvir.GuildInfoList.CreateNewObject();

            info.GuildName = p.Name;
            info.MemberLimit = 10 + p.Members;
            info.StorageSize = 10 + p.Storage;
            //info.GuildFunds = Globals.GuildCreationCost;
            info.GuildLevel = 1;

            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = info;
            memberInfo.Rank = "Guild Leader";
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = GuildPermission.Leader;

            if (!p.UseGold)
            {
                for (int i = 0; i < Inventory.Length; i++)
                {
                    UserItem item = Inventory[i];
                    if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.UmaKingHorn) continue;

                    if (item.Count > 1)
                    {
                        item.Count -= 1;

                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                        break;
                    }

                    RemoveItem(item);
                    Inventory[i] = null;
                    item.Delete();

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });
                    break;
                }
            }

            Gold -= cost;
            GoldChanged();

            SendGuildInfo();
        }
        public void GuildEditNotice(C.GuildEditNotice p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.EditNotice) != GuildPermission.EditNotice)
            {
                Connection.ReceiveChat(Connection.Language.GuildNoticePermission, MessageType.System);
                return;
            }

            if (p.Notice.Length > Globals.MaxGuildNoticeLength) return;


            Character.Account.GuildMember.Guild.GuildNotice = p.Notice;

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildNoticeChanged { Notice = p.Notice, ObserverPacket = false });
            }
        }
        public void GuildEditMember(C.GuildEditMember p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildEditMemberPermission, MessageType.System);
                return;
            }

            if (p.Rank.Length > Globals.MaxCharacterNameLength)
            {
                Connection.ReceiveChat(Connection.Language.GuildMemberLength, MessageType.System);
                return;
            }


            if (p.Index > 0)
            {
                GuildMemberInfo info = Character.Account.GuildMember.Guild.Members.FirstOrDefault(x => x.Index == p.Index);

                if (info == null)
                {
                    Connection.ReceiveChat(Connection.Language.GuildMemberNotFound, MessageType.System);
                    return;
                }

                if (info != Character.Account.GuildMember) //Don't Change ones own permission.
                    info.Permission = p.Permission;
                info.Rank = p.Rank;

                S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

                update.Members.Add(info.ToClientInfo());

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    member.Account.Connection?.Player?.Enqueue(update);

                info.Account.Connection?.Player?.Broadcast(new S.GuildChanged { ObjectID = info.Account.Connection.Player.ObjectID, GuildName = info.Guild.GuildName, GuildRank = info.Rank });
            }
            else
            {
                Character.Account.GuildMember.Guild.DefaultRank = p.Rank;
                Character.Account.GuildMember.Guild.DefaultPermission = p.Permission;

                S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    member.Account.Connection?.Player?.Enqueue(update);
            }
        }
        public void GuildKickMember(C.GuildKickMember p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildKickPermission, MessageType.System);
                return;
            }

            GuildMemberInfo info = Character.Account.GuildMember.Guild.Members.FirstOrDefault(x => x.Index == p.Index);

            if (info == null)
            {
                Connection.ReceiveChat(Connection.Language.GuildMemberNotFound, MessageType.System);
                return;
            }

            if (info == Character.Account.GuildMember)
            {
                Connection.ReceiveChat(Connection.Language.GuildKickSelf, MessageType.System);
                return;
            }

            GuildInfo guild = info.Guild;
            PlayerObject player = info.Account.Connection?.Player;
            string memberName = info.Account.LastCharacter.CharacterName;

            info.Account.GuildTime = SEnvir.Now.AddDays(1);

            info.Guild = null;
            info.Account = null;
            info.Delete();


            if (player != null)
            {
                player.Connection.ReceiveChat(string.Format(player.Connection.Language.GuildKicked, Name), MessageType.System);
                player.Enqueue(new S.GuildInfo { ObserverPacket = false });
                player.Broadcast(new S.GuildChanged { ObjectID = player.ObjectID });
                player.RemoveAllObjects();
                player.ApplyGuildBuff();
            }

            foreach (GuildMemberInfo member in guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildMemberKicked, memberName, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(new S.GuildKick { Index = info.Index, ObserverPacket = false });
                member.Account.Connection.Player.RemoveAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

        }
        public void GuildTax(C.GuildTax p)
        {
            Enqueue(new S.GuildTax { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildManagePermission, MessageType.System);
                return;
            }

            if (p.Tax < 0 || p.Tax > 100) return;

            Character.Account.GuildMember.Guild.GuildTax = p.Tax / 100M;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        public void GuildIncreaseMember(C.GuildIncreaseMember p)
        {
            Enqueue(new S.GuildIncreaseMember { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            GuildInfo guild = Character.Account.GuildMember.Guild;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildManagePermission, MessageType.System);
                return;
            }

            if (guild.MemberLimit >= 100)
            {
                Connection.ReceiveChat(Connection.Language.GuildMemberLimit, MessageType.System);
                return;
            }

            if (guild.GuildFunds < Globals.GuildMemberCost)
            {
                Connection.ReceiveChat(Connection.Language.GuildMemberCost, MessageType.System);
                return;
            }

            guild.GuildFunds -= Globals.GuildMemberCost;
            guild.DailyGrowth -= Globals.GuildMemberCost;

            Character.Account.GuildMember.Guild.MemberLimit++;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        public void GuildIncreaseStorage(C.GuildIncreaseStorage p)
        {
            Enqueue(new S.GuildIncreaseStorage { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildManagePermission, MessageType.System);
                return;
            }

            GuildInfo guild = Character.Account.GuildMember.Guild;
            if (guild.StorageSize >= 500)
            {
                Connection.ReceiveChat(Connection.Language.GuildStorageLimit, MessageType.System);
                return;
            }

            if (guild.GuildFunds < Globals.GuildStorageCost)
            {
                Connection.ReceiveChat(Connection.Language.GuildStorageCost, MessageType.System);
                return;
            }

            guild.GuildFunds -= Globals.GuildStorageCost;
            guild.DailyGrowth -= Globals.GuildStorageCost;
            Character.Account.GuildMember.Guild.StorageSize++;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        public void GuildInviteMember(C.GuildInviteMember p)
        {
            Enqueue(new S.GuildInviteMember { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember)
            {
                Connection.ReceiveChat(Connection.Language.GuildInvitePermission, MessageType.System);
                return;
            }

            PlayerObject player = SEnvir.GetPlayerByCharacter(p.Name);

            if (player == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.CannotFindPlayer, p.Name), MessageType.System);
                return;
            }

            if (player.Character.Account.GuildMember != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildInviteGuild, MessageType.System);
                return;
            }

            if (player.GuildInvitation != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildInviteInvited, MessageType.System);
                return;
            }

            if (SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildInviteNotAllowed, player.Name), MessageType.System);
                return;
            }
            if (!player.Character.Account.AllowGuild)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildInviteNotAllowed, player.Name), MessageType.System);
                player.Connection.ReceiveChat(string.Format(player.Connection.Language.GuildInvitedNotAllowed, Character.CharacterName, Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                return;
            }


            if (Character.Account.GuildMember.Guild.Members.Count >= Character.Account.GuildMember.Guild.MemberLimit)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildInviteRoom, player.Name), MessageType.System);
                return;
            }

            player.GuildInvitation = this;
            player.Enqueue(new S.GuildInvite { Name = Name, GuildName = Character.Account.GuildMember.Guild.GuildName, ObserverPacket = false });
        }
        public void GuildWar(string guildName)
        {
            S.GuildWar result = new S.GuildWar { ObserverPacket = false };
            Enqueue(result);

            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat(Connection.Language.GuildNoGuild, MessageType.System);
                return;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.StartWar) != GuildPermission.StartWar)
            {
                Connection.ReceiveChat(Connection.Language.GuildWarPermission, MessageType.System);
                return;
            }

            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

            if (guild == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildNotFoundGuild, guildName), MessageType.System);
                return;
            }

            if (guild == Character.Account.GuildMember.Guild)
            {
                Connection.ReceiveChat(Connection.Language.GuildWarOwnGuild, MessageType.System);
                result.Success = true;
                return;
            }

            if (SEnvir.GuildWarInfoList.Binding.Any(x => (x.Guild1 == guild && x.Guild2 == Character.Account.GuildMember.Guild) ||
                                                         (x.Guild2 == guild && x.Guild1 == Character.Account.GuildMember.Guild)))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildAlreadyWar, guild.GuildName), MessageType.System);
                return;
            }

            if (Globals.GuildWarCost > Character.Account.GuildMember.Guild.GuildFunds)
            {
                Connection.ReceiveChat(Connection.Language.GuildWarCost, MessageType.System);
                return;
            }

            result.Success = true;

            Character.Account.GuildMember.Guild.GuildFunds -= Globals.GuildWarCost;
            Character.Account.GuildMember.Guild.DailyGrowth -= Globals.GuildWarCost;

            GuildWarInfo warInfo = SEnvir.GuildWarInfoList.CreateNewObject();

            warInfo.Guild1 = Character.Account.GuildMember.Guild;
            warInfo.Guild2 = guild;
            warInfo.Duration = TimeSpan.FromHours(2);

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -Globals.GuildWarCost, ObserverPacket = false });
                member.Account.Connection?.Player?.Enqueue(new S.GuildWarStarted { GuildName = guild.GuildName, Duration = warInfo.Duration });
            }

            foreach (GuildMemberInfo member in guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildWarStarted { GuildName = Character.Account.GuildMember.Guild.GuildName, Duration = warInfo.Duration });
            }
        }
        public void GuildConquest(int index)
        {
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat(Connection.Language.GuildNoGuild, MessageType.System);
                return;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat(Connection.Language.GuildWarPermission, MessageType.System);
                return;
            }

            if (Character.Account.GuildMember.Guild.Castle != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildConquestCastle, MessageType.System);
                return;
            }

            if (Character.Account.GuildMember.Guild.Conquest != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildConquestExists, MessageType.System);
                return;
            }


            CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == index);

            if (castle == null)
            {
                Connection.ReceiveChat(Connection.Language.GuildConquestBadCastle, MessageType.System);
                return;
            }

            if (SEnvir.ConquestWars.Count > 0)
            {
                Connection.ReceiveChat(Connection.Language.GuildConquestProgress, MessageType.System);
                return;
            }

            if (castle.Item != null)
            {
                if (GetItemCount(castle.Item) == 0)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.GuildConquestNeedItem, castle.Item.ItemName, castle.Name), MessageType.System);
                    return;
                }

                TakeItem(castle.Item, 1);
            }


            DateTime now = SEnvir.Now;
            DateTime date = new DateTime(now.Ticks - now.TimeOfDay.Ticks + TimeSpan.TicksPerDay * 2);

            if (now.TimeOfDay.Ticks >= castle.StartTime.Ticks)
                date = date.AddTicks(TimeSpan.TicksPerDay);

            UserConquest conquest = SEnvir.UserConquestList.CreateNewObject();
            conquest.Guild = Character.Account.GuildMember.Guild;
            conquest.Castle = castle;
            conquest.WarDate = date;

            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

            if (ownerGuild != null)
            {
                foreach (GuildMemberInfo member in ownerGuild.Members)
                {
                    if (member.Account.Connection?.Player == null) continue; //Offline

                    member.Account.Connection.ReceiveChat(member.Account.Connection.Language.GuildConquestSuccess, MessageType.System);
                    member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = (date + castle.StartTime) - SEnvir.Now, ObserverPacket = false });
                }
            }

            //Send War Date to guild.
            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue; //Offline

                member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildConquestDate, castle.Name), MessageType.System);
                member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = (date + castle.StartTime) - SEnvir.Now, ObserverPacket = false });
            }
        }
        public void GuildJoin()
        {
            if (GuildInvitation != null && GuildInvitation.Node == null) GuildInvitation = null;

            if (GuildInvitation == null) return;

            if (Character.Account.GuildMember != null)
            {
                Connection.ReceiveChat(Connection.Language.GuildJoinGuild, MessageType.System);
                return;
            }
            if (Character.Account.GuildTime > SEnvir.Now)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinTime, Functions.ToString(Character.Account.GuildTime - SEnvir.Now, true)), MessageType.System);
                return;
            }
            if (GuildInvitation.Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinGuild, GuildInvitation.Name), MessageType.System);
                return;
            }

            if ((GuildInvitation.Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinPermission, GuildInvitation.Name), MessageType.System);
                return;
            }

            if (GuildInvitation.Character.Account.GuildMember.Guild.Members.Count >= GuildInvitation.Character.Account.GuildMember.Guild.MemberLimit)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinNoRoom, GuildInvitation.Name), MessageType.System);
                return;
            }


            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = GuildInvitation.Character.Account.GuildMember.Guild;
            memberInfo.Rank = GuildInvitation.Character.Account.GuildMember.Guild.DefaultRank;
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = GuildInvitation.Character.Account.GuildMember.Guild.DefaultPermission;


            SendGuildInfo();
            Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinWelcome, GuildInvitation.Name), MessageType.System);

            Broadcast(new S.GuildChanged { ObjectID = ObjectID, GuildName = memberInfo.Guild.GuildName, GuildRank = memberInfo.Rank });
            AddAllObjects();

            S.GuildUpdate update = memberInfo.Guild.GetUpdatePacket();

            update.Members.Add(memberInfo.ToClientInfo());

            foreach (GuildMemberInfo member in memberInfo.Guild.Members)
            {
                if (member == memberInfo || member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildMemberJoined, GuildInvitation.Name, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(update);

                member.Account.Connection.Player.AddAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

            ApplyCastleBuff();
            ApplyGuildBuff();
        }
        public void GuildLeave()
        {
            if (Character.Account.GuildMember == null) return;

            GuildMemberInfo info = Character.Account.GuildMember;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) == GuildPermission.Leader && info.Guild.Members.Count > 1 && info.Guild.Members.FirstOrDefault(x => x.Index != info.Index && (x.Permission & GuildPermission.Leader) == GuildPermission.Leader) == null)
            {
                Connection.ReceiveChat(Connection.Language.GuildLeaveFailed, MessageType.System);
                return;
            }

            GuildInfo guild = info.Guild;
            int index = info.Index;

            info.Guild = null;
            info.Account = null;
            info.Delete();

            if (!guild.StarterGuild)
                Character.Account.GuildTime = SEnvir.Now.AddDays(1);

            Connection.ReceiveChat(Connection.Language.GuildLeave, MessageType.System);
            Enqueue(new S.GuildInfo { ObserverPacket = false });

            Broadcast(new S.GuildChanged { ObjectID = ObjectID });
            RemoveAllObjects();



            foreach (GuildMemberInfo member in guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.Player.Enqueue(new S.GuildKick { Index = index, ObserverPacket = false });
                member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildMemberLeave, Name), MessageType.System);
                member.Account.Connection.Player.RemoveAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

            ApplyCastleBuff();
            ApplyGuildBuff();
        }

        public bool AtWar(PlayerObject player)
        {
            foreach (ConquestWar conquest in SEnvir.ConquestWars)
            {
                if (conquest.Map != CurrentMap) continue;

                if (Character.Account.GuildMember == null || player.Character.Account.GuildMember == null) return true;

                return Character.Account.GuildMember.Guild != player.Character.Account.GuildMember.Guild;
            }

            if (player.Character.Account.GuildMember == null) return false;
            if (Character.Account.GuildMember == null) return false;


            foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
            {
                if (warInfo.Guild1 == Character.Account.GuildMember.Guild && warInfo.Guild2 == player.Character.Account.GuildMember.Guild) return true;
                if (warInfo.Guild2 == Character.Account.GuildMember.Guild && warInfo.Guild1 == player.Character.Account.GuildMember.Guild) return true;
            }

            return false;
        }
        public void SendGuildInfo()
        {
            if (Character.Account.GuildMember == null) return;

            S.GuildInfo result = new S.GuildInfo
            {
                Guild = Character.Account.GuildMember.Guild.ToClientInfo(),
                ObserverPacket = false,
            };

            result.Guild.UserIndex = Character.Account.GuildMember.Index;

            Enqueue(result);

            foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
            {
                if (warInfo.Guild1 == Character.Account.GuildMember.Guild)
                    Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild2.GuildName, Duration = warInfo.Duration });

                if (warInfo.Guild2 == Character.Account.GuildMember.Guild)
                    Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild1.GuildName, Duration = warInfo.Duration });
            }

            //Send War Date to guild.
            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)
            {
                UserConquest conquest = SEnvir.UserConquestList.Binding.FirstOrDefault(x => x.Castle == castle && (x.Guild == Character.Account.GuildMember.Guild || x.Castle == Character.Account.GuildMember.Guild.Castle));

                TimeSpan warTime = TimeSpan.MinValue;
                if (conquest != null)
                    warTime = (conquest.WarDate + conquest.Castle.StartTime) - SEnvir.Now;

                Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = warTime, ObserverPacket = false });
            }
        }

        public void JoinStarterGuild()
        {
            if (Character.Account.GuildMember != null) return;

            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = SEnvir.StarterGuild;
            memberInfo.Rank = SEnvir.StarterGuild.DefaultRank;
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = SEnvir.StarterGuild.DefaultPermission;

            SendGuildInfo();

            Connection.ReceiveChat(string.Format(Connection.Language.GuildJoinWelcome, memberInfo.Guild.GuildName), MessageType.System);

            Broadcast(new S.GuildChanged { ObjectID = ObjectID, GuildName = memberInfo.Guild.GuildName, GuildRank = memberInfo.Rank });
            AddAllObjects();

            S.GuildUpdate update = memberInfo.Guild.GetUpdatePacket();

            update.Members.Add(memberInfo.ToClientInfo());

            foreach (GuildMemberInfo member in memberInfo.Guild.Members)
            {
                if (member == memberInfo || member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildMemberJoined, SEnvir.StarterGuild, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(update);

                member.Account.Connection.Player.AddAllObjects();
            }

            ApplyCastleBuff();
            ApplyGuildBuff();
        }

        #endregion

        #region Group
        public void GroupSwitch(bool allowGroup)
        {
            if (Character.Account.AllowGroup == allowGroup) return;

            if (GroupMembers.Any(x => x.CurrentMap.Instance != null))
            {
                Connection.ReceiveChat(Connection.Language.NoActionOnInstance, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NoActionOnInstance, MessageType.System);
                return;
            }

            Character.Account.AllowGroup = allowGroup;

            Enqueue(new S.GroupSwitch { Allow = Character.Account.AllowGroup });

            if (GroupMembers != null)
                GroupLeave();
        }

        public void GroupRemove(string name)
        {
            if (GroupMembers == null)
            {
                Connection.ReceiveChat(Connection.Language.GroupNoGroup, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNoGroup, MessageType.System);
                return;
            }

            if (GroupMembers[0] != this)
            {
                Connection.ReceiveChat(Connection.Language.GroupNotLeader, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNotLeader, MessageType.System);
                return;
            }

            if (GroupMembers.Any(x => x.CurrentMap.Instance != null))
            {
                Connection.ReceiveChat(Connection.Language.NoActionOnInstance, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NoActionOnInstance, MessageType.System);
                return;
            }

            foreach (PlayerObject member in GroupMembers)
            {
                if (string.Compare(member.Name, name, StringComparison.OrdinalIgnoreCase) != 0) continue;

                member.GroupLeave();
                return;
            }

            Connection.ReceiveChat(string.Format(Connection.Language.GroupMemberNotFound, name), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(con.Language.GroupMemberNotFound, name), MessageType.System);

        }
        public void GroupInvite(string name)
        {
            if (GroupMembers != null && GroupMembers[0] != this)
            {
                Connection.ReceiveChat(Connection.Language.GroupNotLeader, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupNotLeader, MessageType.System);
                return;
            }

            PlayerObject player = SEnvir.GetPlayerByCharacter(name);

            if (player == null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.CannotFindPlayer, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.CannotFindPlayer, name), MessageType.System);
                return;
            }

            if (player.GroupMembers != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyGrouped, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyGrouped, name), MessageType.System);
                return;
            }

            if (player.GroupInvitation != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyInvited, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyInvited, name), MessageType.System);
                return;
            }

            if (!player.Character.Account.AllowGroup || SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupInviteNotAllowed, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupInviteNotAllowed, name), MessageType.System);
                return;
            }

            if (player == this)
            {
                Connection.ReceiveChat(Connection.Language.GroupSelf, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.GroupSelf, MessageType.System);
                return;
            }

            if (GroupMembers.Any(x => x.CurrentMap.Instance != null))
            {
                Connection.ReceiveChat(Connection.Language.NoActionOnInstance, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NoActionOnInstance, MessageType.System);
                return;
            }

            player.GroupInvitation = this;
            player.Enqueue(new S.GroupInvite { Name = Name, ObserverPacket = false });
        }

        public void GroupJoin()
        {
            if (GroupInvitation != null && GroupInvitation.Node == null) GroupInvitation = null;

            if (GroupInvitation == null || GroupMembers != null) return;

            if (GroupInvitation.GroupMembers == null)
            {
                GroupInvitation.GroupSwitch(true);
                GroupInvitation.GroupMembers = new List<PlayerObject> { GroupInvitation };
                GroupInvitation.Enqueue(new S.GroupMember { ObjectID = GroupInvitation.ObjectID, Name = GroupInvitation.Name }); //<-- Setting group leader?
            }
            else if (GroupInvitation.GroupMembers[0] != GroupInvitation)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupAlreadyGrouped, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupAlreadyGrouped, GroupInvitation.Name), MessageType.System);
                return;
            }
            else if (GroupInvitation.GroupMembers.Count >= Globals.GroupLimit)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.GroupMemberLimit, GroupInvitation.Name), MessageType.System);
                return;
            }

            if (CurrentMap.Instance != null)
            {
                Connection.ReceiveChat(Connection.Language.NoActionOnInstance, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NoActionOnInstance, MessageType.System);
                return;
            }

            GroupMembers = GroupInvitation.GroupMembers;
            GroupMembers.Add(this);

            foreach (PlayerObject ob in GroupMembers)
            {
                if (ob == this) continue;

                ob.Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
                Enqueue(new S.GroupMember { ObjectID = ob.ObjectID, Name = ob.Name });

                ob.AddAllObjects();
                ob.RefreshStats();
                ob.ApplyGuildBuff();
            }

            AddAllObjects();
            ApplyGuildBuff();

            RefreshStats();
            Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
        }
        public void GroupLeave()
        {
            Packet p = new S.GroupRemove { ObjectID = ObjectID };

            GroupMembers.Remove(this);
            List<PlayerObject> oldGroup = GroupMembers;
            GroupMembers = null;

            foreach (PlayerObject ob in oldGroup)
            {
                ob.Enqueue(p);
                ob.RemoveAllObjects();
                ob.RefreshStats();
                ob.ApplyGuildBuff();
            }

            if (oldGroup.Count == 1) oldGroup[0].GroupLeave();

            GroupMembers = null;
            Enqueue(p);
            RemoveAllObjects();
            RefreshStats();
            ApplyGuildBuff();
        }
        #endregion

        #region Items

        public bool ParseLinks(List<CellLinkInfo> links, int minCount, int maxCount)
        {
            if (links == null || links.Count < minCount || links.Count > maxCount) return false;


            List<CellLinkInfo> tempLinks = new List<CellLinkInfo>();


            foreach (CellLinkInfo link in links)
            {
                if (link == null || link.Count <= 0) return false;

                CellLinkInfo tempLink = tempLinks.FirstOrDefault(x => x.GridType == link.GridType && x.Slot == link.Slot);

                if (tempLink == null)
                {
                    tempLinks.Add(link);
                    continue;
                }


                tempLink.Count += link.Count;
            }


            links.Clear();
            links.AddRange(tempLinks);

            return true;
        }
        public bool ParseLinks(CellLinkInfo link)
        {
            return link != null && link.Count > 0;
        }

        public void RemoveItem(UserItem item)
        {
            /*  foreach (BeltLink link in Character.BeltLinks)
              {
                  if (link.LinkItemIndex != item) continue;
  
                  link.LinkSlot = -1;
              }*/

            item.Slot = -1;
            item.Character = null;
            item.Account = null;
            item.Mail = null;
            item.Auction = null;
            item.Companion = null;
            item.Guild = null;

            /*item.GuildInfo = null;
            item.SaleInfo = null;*/


            //   item.Flags &= ~UserItemFlags.Locked;
        }

        public bool CanGainItems(bool checkWeight, params ItemCheck[] checks)
        {
            int index = 0;
            foreach (ItemCheck check in checks)
            {
                if ((check.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                long count = check.Count;

                if (check.Info.Effect == ItemEffect.Gold)
                {
                    long gold = Gold;

                    gold += count;

                    continue;
                }
                if (check.Info.Effect == ItemEffect.Experience) continue;

                if (checkWeight)
                {
                    switch (check.Info.ItemType)
                    {
                        case ItemType.Amulet:
                        case ItemType.Poison:
                            if (BagWeight + check.Info.Weight > Stats[Stat.BagWeight]) return false;
                            break;
                        default:
                            if (BagWeight + check.Info.Weight * count > Stats[Stat.BagWeight]) return false;
                            break;
                    }
                }

                if (check.Info.StackSize > 1 && (check.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (UserItem oldItem in Inventory)
                    {
                        if (oldItem == null) continue;

                        if (oldItem.Info != check.Info || oldItem.Count >= check.Info.StackSize) continue;

                        if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((oldItem.Flags & UserItemFlags.Bound) != (check.Flags & UserItemFlags.Bound)) continue;
                        if ((oldItem.Flags & UserItemFlags.Worthless) != (check.Flags & UserItemFlags.Worthless)) continue;
                        if ((oldItem.Flags & UserItemFlags.NonRefinable) != (check.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!oldItem.Stats.Compare(check.Stats)) continue;

                        count -= check.Info.StackSize - oldItem.Count;

                        if (count <= 0) break;
                    }

                    if (count <= 0) break;
                }

                //Start Index
                for (int i = index; i < Inventory.Length; i++)
                {
                    index++;
                    UserItem item = Inventory[i];
                    if (item == null)
                    {
                        count -= check.Info.StackSize;

                        if (count <= 0) break;
                    }
                }

                if (count > 0) return false;
            }

            return true;
        }
        public void GainItem(params UserItem[] items)
        {
            Enqueue(new S.ItemsGained { Items = items.Where(x => x.Info.Effect != ItemEffect.Experience).Select(x => x.ToClientInfo()).ToList() });

            HashSet<UserQuest> changedQuests = new HashSet<UserQuest>();

            foreach (UserItem item in items)
            {
                if (item.UserTask != null)
                {
                    if (item.UserTask.Completed) continue;

                    item.UserTask.Amount = Math.Min(item.UserTask.Task.Amount, item.UserTask.Amount + item.Count);

                    changedQuests.Add(item.UserTask.Quest);

                    if (item.UserTask.Completed)
                    {
                        for (int i = item.UserTask.Objects.Count - 1; i >= 0; i--)
                            item.UserTask.Objects[i].Despawn();
                    }

                    item.UserTask = null;
                    item.Flags &= ~UserItemFlags.QuestItem;


                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                if (item.Info.Effect == ItemEffect.Gold)
                {
                    Gold += item.Count;
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                if (item.Info.Effect == ItemEffect.Experience)
                {
                    GainExperience(item.Count, false);
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                bool handled = false;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (UserItem oldItem in Inventory)
                    {
                        if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;

                        if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!oldItem.Stats.Compare(item.Stats)) continue;


                        if (oldItem.Count + item.Count <= item.Info.StackSize)
                        {
                            oldItem.Count += item.Count;
                            item.IsTemporary = true;
                            item.Delete();
                            handled = true;
                            break;
                        }

                        item.Count -= item.Info.StackSize - oldItem.Count;
                        oldItem.Count = item.Info.StackSize;
                    }
                    if (handled) continue;
                }

                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] != null) continue;

                    Inventory[i] = item;
                    item.Slot = i;
                    item.Character = Character;
                    item.IsTemporary = false;
                    break;
                }
            }

            foreach (UserQuest quest in changedQuests)
                Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });


            RefreshWeight();
        }

        public void ItemUse(CellLinkInfo link)
        {
            if (!ParseLinks(link)) return;

            UserItem[] fromArray;
            switch (link.GridType)
            {
                case GridType.Inventory:
                    fromArray = Inventory;
                    break;
                case GridType.PartsStorage:
                    fromArray = PartsStorage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                case GridType.CompanionEquipment:
                    if (Companion == null) return;

                    fromArray = Companion.Equipment;
                    break;
                default:
                    return;
            }

            if (link.Slot < 0 || link.Slot >= fromArray.Length) return;

            UserItem item = fromArray[link.Slot];

            if (item == null) return;

            if (SEnvir.Now < AutoPotionTime && item.Info.Effect != ItemEffect.ElixirOfPurification)
            {
                if (DelayItemUse != null)
                    Enqueue(new S.ItemChanged
                    {
                        Link = DelayItemUse
                    });

                DelayItemUse = link;
                return;
            }

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = link.GridType, Slot = link.Slot }
            };
            Enqueue(result);


            if (Buffs.Any(x => x.Type == BuffType.DragonRepulse)) return;

            if (!CanUseItem(item)) return;

            if (Dead && item.Info.Effect != ItemEffect.PillOfReincarnation) return;

            int useCount = 1;

            UserMagic magic;
            BuffInfo buff;
            UserItem gainItem = null;
            switch (item.Info.ItemType)
            {
                case ItemType.Consumable:
                    if ((SEnvir.Now < UseItemTime && item.Info.Effect != ItemEffect.ElixirOfPurification) || Horse != HorseType.None) return;

                    bool work;
                    bool hasSpace;
                    UserItem weapon;
                    ItemInfo extractorInfo;
                    switch (item.Info.Shape)
                    {
                        case 0: //Potion

                            int health = item.Info.Stats[Stat.Health];
                            int mana = item.Info.Stats[Stat.Mana];

                            if (Magics.TryGetValue(MagicType.PotionMastery, out magic) && Level >= magic.Info.NeedLevel1)
                            {
                                health += health * magic.GetPower() / 100;
                                mana += mana * magic.GetPower() / 100;

                                if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                                    LevelMagic(magic);
                            }

                            if (Magics.TryGetValue(MagicType.AdvancedPotionMastery, out magic) && Level >= magic.Info.NeedLevel1)
                            {
                                health += health * magic.GetPower() / 100;
                                mana += mana * magic.GetPower() / 100;

                                if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                                    LevelMagic(magic);
                            }

                            ChangeHP(health);
                            ChangeMP(mana);

                            if (item.Info.Stats[Stat.Experience] > 0) GainExperience(item.Info.Stats[Stat.Experience], false);
                            break;
                        case 1:
                            if (!ItemBuffAdd(item.Info)) return;
                            break;
                        case 2: //Town Teleport
                            if (!CurrentMap.Info.AllowTT || CurrentMap.Instance != null)
                            {
                                Connection.ReceiveChat(Connection.Language.CannotTownTeleport, MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(con.Language.CannotTownTeleport, MessageType.System);
                                return;
                            }

                            if (!Teleport(SEnvir.Maps[Character.BindPoint.BindRegion.Map], Character.BindPoint.ValidBindPoints[SEnvir.Random.Next(Character.BindPoint.ValidBindPoints.Count)]))
                                return;
                            break;
                        case 3: //Random Teleport

                            if (!CurrentMap.Info.AllowRT)
                            {
                                Connection.ReceiveChat(Connection.Language.CannotRandomTeleport, MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(con.Language.CannotRandomTeleport, MessageType.System);
                                return;
                            }

                            if (!Teleport(CurrentMap, CurrentMap.GetRandomLocation()))
                                return;
                            break;
                        case 4: //Benediction
                            if (!UseOilOfBenediction()) return;
                            RefreshStats();
                            break;
                        case 5: //Conservation
                            if (!UseOilOfConservation()) return;
                            RefreshStats();
                            break;
                        case 6: //WarGod
                            work = SpecialRepair(EquipmentSlot.Weapon);

                            work = SpecialRepair(EquipmentSlot.Shield) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 7: //Potion of Forgetfulness
                            if (Character.SpentPoints == 0) return;

                            Character.SpentPoints = 0;
                            Character.HermitStats.Clear();

                            decimal loss = Math.Min(Experience, 1000000);

                            if (loss != 0)
                            {
                                Experience -= loss;
                                Enqueue(new S.GainedExperience { Amount = -loss });
                            }

                            RefreshStats();
                            break;
                        case 8: //Potion of Repentence

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);
                            if (buff == null) return;

                            buff.Stats[Stat.PKPoint] = Math.Max(0, buff.Stats[Stat.PKPoint] + item.Info.Stats[Stat.PKPoint]);

                            if (buff.Stats[Stat.PKPoint] == 0)
                                BuffRemove(buff);
                            else
                            {
                                Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                                RefreshStats();
                            }

                            break;
                        case 9: //Redemption Key Stone

                            TimeSpan duration = TimeSpan.FromSeconds(item.Info.Stats[Stat.Duration]);

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.Redemption);
                            if (buff != null)
                                duration += buff.RemainingTime;

                            Stats stats = new Stats(item.Info.Stats) { [Stat.Duration] = 0 };

                            BuffAdd(BuffType.Redemption, duration, stats, false, false, TimeSpan.Zero);

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PvPCurse);

                            if (buff != null)
                            {
                                buff.RemainingTime = TimeSpan.FromTicks(buff.RemainingTime.Ticks / 2);
                                Enqueue(new S.BuffTime { Index = buff.Index, Time = buff.RemainingTime });
                            }

                            break;
                        case 10: //Potion of Oblivion
                            if (Character.SpentPoints == 0) return;

                            Character.SpentPoints = 0;
                            Character.HermitStats.Clear();

                            RefreshStats();
                            break;
                        case 11: //Superior Repair Oil

                            work = SpecialRepair(EquipmentSlot.Weapon);
                            work = SpecialRepair(EquipmentSlot.Shield);

                            work = SpecialRepair(EquipmentSlot.Helmet) || work;
                            work = SpecialRepair(EquipmentSlot.Armour) || work;
                            work = SpecialRepair(EquipmentSlot.Necklace) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletL) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletR) || work;
                            work = SpecialRepair(EquipmentSlot.RingL) || work;
                            work = SpecialRepair(EquipmentSlot.RingR) || work;
                            work = SpecialRepair(EquipmentSlot.Shoes) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 12: //Accessory Repair Oil
                            work = SpecialRepair(EquipmentSlot.Necklace);
                            work = SpecialRepair(EquipmentSlot.BraceletL) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletR) || work;
                            work = SpecialRepair(EquipmentSlot.RingL) || work;
                            work = SpecialRepair(EquipmentSlot.RingR) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 13: //Armour Repair Oil

                            work = SpecialRepair(EquipmentSlot.Helmet);
                            work = SpecialRepair(EquipmentSlot.Armour) || work;
                            work = SpecialRepair(EquipmentSlot.Shoes) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 14: //ElixirOfPurification
                            work = false;

                            for (int i = PoisonList.Count - 1; i >= 0; i--)
                            {
                                Poison pois = PoisonList[i];

                                switch (pois.Type)
                                {
                                    case PoisonType.Green:
                                    case PoisonType.Red:
                                    case PoisonType.Slow:
                                    case PoisonType.Paralysis:
                                    case PoisonType.HellFire:
                                    case PoisonType.Silenced:
                                        work = true;
                                        PoisonList.Remove(pois);
                                        break;
                                    case PoisonType.Abyss:
                                        work = true;
                                        PoisonList.Remove(pois);
                                        break;
                                    default:
                                        continue;
                                }
                            }

                            if (!work)
                            {
                                if (SEnvir.Now.AddSeconds(3) > UseItemTime)
                                    UseItemTime = UseItemTime.AddMilliseconds(item.Info.Durability);

                                AutoPotionCheckTime = UseItemTime.AddMilliseconds(500);
                                return;
                            }

                            break;
                        case 15: //ElixirOfPurification

                            if (!Dead || SEnvir.Now < Character.ReincarnationPillTime) return;

                            Dead = false;
                            SetHP(Stats[Stat.Health]);
                            SetMP(Stats[Stat.Mana]);

                            Character.ReincarnationPillTime = SEnvir.Now.AddSeconds(item.Info.Stats[Stat.ItemReviveTime]);

                            UpdateReviveTimers(Connection);
                            Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = true });
                            break;
                        case 16: //Elixir of Regret
                            if (Companion == null) return;

                            switch (item.Info.RequiredAmount)
                            {
                                case 3:
                                    if (Companion.UserCompanion.Level3 == null) return;

                                    if (!CompanionLevelLock3)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 3), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 3), MessageType.System);
                                        return;
                                    }

                                    Stats current = new Stats(Companion.UserCompanion.Level3);

                                    while (current.Compare(Companion.UserCompanion.Level3))
                                    {
                                        Companion.UserCompanion.Level3 = null;

                                        Companion.CheckSkills();
                                    }

                                    break;
                                case 5:
                                    if (Companion.UserCompanion.Level5 == null) return;

                                    if (!CompanionLevelLock5)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 5), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 5), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level5);

                                    while (current.Compare(Companion.UserCompanion.Level5))
                                    {
                                        Companion.UserCompanion.Level5 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 7:
                                    if (Companion.UserCompanion.Level7 == null) return;

                                    if (!CompanionLevelLock7)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 7), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 7), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level7);

                                    while (current.Compare(Companion.UserCompanion.Level7))
                                    {
                                        Companion.UserCompanion.Level7 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 10:
                                    if (Companion.UserCompanion.Level10 == null) return;

                                    if (!CompanionLevelLock10)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 10), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 10), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level10);

                                    while (current.Compare(Companion.UserCompanion.Level10))
                                    {
                                        Companion.UserCompanion.Level10 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 11:
                                    if (Companion.UserCompanion.Level11 == null) return;

                                    if (!CompanionLevelLock11)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 11), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 11), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level11);

                                    while (current.Compare(Companion.UserCompanion.Level11))
                                    {
                                        Companion.UserCompanion.Level11 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 13:
                                    if (Companion.UserCompanion.Level13 == null) return;

                                    if (!CompanionLevelLock13)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 13), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 13), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level13);

                                    while (current.Compare(Companion.UserCompanion.Level13))
                                    {
                                        Companion.UserCompanion.Level13 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 15:
                                    if (Companion.UserCompanion.Level15 == null) return;

                                    if (!CompanionLevelLock15)
                                    {
                                        Connection.ReceiveChat(string.Format(Connection.Language.ConnotResetCompanionSkill, item.Info.ItemName, 15), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat(string.Format(con.Language.ConnotResetCompanionSkill, item.Info.ItemName, 15), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level15);

                                    while (current.Compare(Companion.UserCompanion.Level15))
                                    {
                                        Companion.UserCompanion.Level15 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                default:
                                    return;
                            }
                            break;
                        case 17:

                            int size = Character.Account.StorageSize + 10;

                            if (size >= Storage.Length)
                            {
                                Connection.ReceiveChat(Connection.Language.StorageLimit, MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(con.Language.StorageLimit, MessageType.System);
                                return;
                            }

                            Character.Account.StorageSize = size;
                            Enqueue(new S.StorageSize { Size = Character.Account.StorageSize });
                            break;
                        case 18:
                            if (item.Info.Stats[Stat.MapSummoning] > 0 && CurrentMap.HasSafeZone)
                            {
                                Connection.ReceiveChat($"You cannot use [{item.Info.ItemName}] with maps that have a SafeZone.", MessageType.System);
                                return;
                            }



                            if (item.Info.Stats[Stat.Experience] > 0) GainExperience(item.Info.Stats[Stat.Experience], false);

                            IncreasePKPoints(item.Info.Stats[Stat.PKPoint]);


                            if (item.Info.Stats[Stat.FootballArmourAction] > 0 && SEnvir.Random.Next(item.Info.Stats[Stat.FootballArmourAction]) == 0)
                            {
                                hasSpace = false;

                                foreach (UserItem slot in Inventory)
                                {
                                    if (slot != null) continue;

                                    hasSpace = true;
                                    break;
                                }

                                if (!hasSpace)
                                {
                                    Connection.ReceiveChat("You do not have any empty inventory slot", MessageType.System);
                                    return;
                                }

                                //Give armour
                                ItemInfo armourInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.FootballArmour && CanStartWith(x));

                                if (armourInfo != null)
                                {
                                    gainItem = SEnvir.CreateDropItem(armourInfo, 2);
                                    gainItem.CurrentDurability = gainItem.MaxDurability;
                                }
                            }

                            if (item.Info.Stats[Stat.MapSummoning] > 0)
                            {
                                MonsterInfo boss;
                                while (true)
                                {
                                    boss = SEnvir.BossList[SEnvir.Random.Next(SEnvir.BossList.Count)];

                                    if (boss.Level >= 300) continue;

                                    break;
                                }

                                MonsterObject mob = MonsterObject.GetMonster(boss);

                                if (mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 2)))
                                {
                                    if (SEnvir.Random.Next(item.Info.Stats[Stat.MapSummoning]) == 0)
                                    {
                                        for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                                        {
                                            mob = CurrentMap.Objects[i] as MonsterObject;

                                            if (mob == null) continue;

                                            if (mob.PetOwner != null) continue;

                                            if (mob is Guard) continue;

                                            if (mob.Dead || mob.MoveDelay == 0 || !mob.CanMove) continue;

                                            if (mob.Target != null) continue;

                                            if (mob.Level >= 300) continue;

                                            mob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 30));
                                        }


                                        string text = $"A [{item.Info.ItemName}] has been used in {CurrentMap.Info.Description}";

                                        foreach (SConnection con in SEnvir.Connections)
                                        {
                                            switch (con.Stage)
                                            {
                                                case GameStage.Game:
                                                case GameStage.Observer:
                                                    con.ReceiveChat(text, MessageType.System);
                                                    break;
                                                default: continue;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 19:
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("You are not holding a weapon.", MessageType.System);
                                return;
                            }


                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"You can not extract a {weapon.Info.ItemName}.", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("Your weapon is not the max level.", MessageType.System);
                                return;
                            }

                            if (weapon.AddedStats.Count == 0)
                            {
                                Connection.ReceiveChat("Your weapon does not have any added stats.", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("You do not have any empty inventory slot", MessageType.System);
                                return;
                            }

                            //Give armour
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.StatExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, Count = 0 }, Success = true });
                            RemoveItem(weapon);
                            Equipment[(int)EquipmentSlot.Weapon] = null;
                            weapon.Delete();
                            RefreshStats();

                            break;
                        case 20:
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("You are not holding a weapon.", MessageType.System);
                                return;
                            }
                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"You can not apply to a {weapon.Info.ItemName}.", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("Your weapon is not the max level.", MessageType.System);
                                return;
                            }

                            weapon.Flags &= ~UserItemFlags.Refinable;

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = weapon.AddedStats[i];
                                stat.Delete();
                            }

                            weapon.StatsChanged();

                            //Give armour
                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment, NewStats = new Stats(weapon.Stats) });
                            RefreshStats();
                            break;
                        case 21:
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("You are not holding a weapon.", MessageType.System);
                                return;
                            }
                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("Your weapon is not the max level.", MessageType.System);
                                return;
                            }

                            bool hasRefine = false;

                            foreach (UserItemStat stat in weapon.AddedStats)
                            {
                                if (stat.StatSource != StatSource.Refine) continue;

                                hasRefine = true;
                                break;
                            }

                            if (!hasRefine)
                            {
                                Connection.ReceiveChat("Your weapon does not have any refine stats.", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("You do not have any empty inventory slot", MessageType.System);
                                return;
                            }

                            //Give armour
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.RefineExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                if (weapon.AddedStats[i].StatSource != StatSource.Refine) continue;

                                weapon.AddedStats[i].Item = gainItem;
                            }

                            gainItem.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats) });

                            RefreshStats();

                            break;
                        case 22:
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("You are not holding a weapon.", MessageType.System);
                                return;
                            }
                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Extraction functions are locked, please type @ExtractorLock and try again", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("Your weapon is not the max level.", MessageType.System);
                                return;
                            }

                            weapon.Flags &= ~UserItemFlags.Refinable;

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = weapon.AddedStats[i];
                                if (stat.StatSource != StatSource.Refine) continue;
                                stat.Delete();
                            }

                            weapon.StatsChanged();

                            //Give armour
                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            weapon.StatsChanged();
                            weapon.ResetCoolDown = SEnvir.Now.AddDays(14);

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment, NewStats = new Stats(weapon.Stats) });
                            RefreshStats();
                            break;
                    }

                    if (item.Info.Effect != ItemEffect.ElixirOfPurification || UseItemTime < SEnvir.Now)
                        UseItemTime = SEnvir.Now.AddMilliseconds(item.Info.Durability);
                    else
                        UseItemTime = UseItemTime.AddMilliseconds(item.Info.Durability);

                    AutoPotionCheckTime = UseItemTime.AddMilliseconds(500);
                    break;
                case ItemType.CompanionFood:
                    if (Companion == null) return;
                    if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                    if (Companion.UserCompanion.Hunger >= Companion.LevelInfo.MaxHunger) return;

                    Companion.UserCompanion.Hunger = Math.Min(Companion.LevelInfo.MaxHunger, Companion.UserCompanion.Hunger + item.Info.Stats[Stat.CompanionHunger]);

                    if (Buffs.All(x => x.Type != BuffType.Companion))
                        CompanionApplyBuff();

                    Companion.RefreshStats();

                    Enqueue(new S.CompanionUpdate
                    {
                        Level = Companion.UserCompanion.Level,
                        Experience = Companion.UserCompanion.Experience,
                        Hunger = Companion.UserCompanion.Hunger,
                    });
                    break;
                case ItemType.Book:

                    if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                    if (SEnvir.Random.Next(100) >= item.CurrentDurability)
                    {
                        Connection.ReceiveChat(Connection.Language.LearnBookFailed, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.LearnBookFailed, MessageType.System);

                        break;
                    }



                    MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Index == item.Info.Shape);

                    if (Magics.TryGetValue(info.Magic, out magic))
                    {
                        int rate = (magic.Level - 2) * 500;

                        magic.Experience++;

                        if (magic.Experience >= rate || (magic.Level == 3 && SEnvir.Random.Next(rate) == 0))
                        {
                            magic.Level++;
                            magic.Experience = 0;

                            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });

                            Connection.ReceiveChat(string.Format(Connection.Language.LearnBook4Success, magic.Info.Name, magic.Level), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.LearnBook4Success, magic.Info.Name, magic.Level), MessageType.System);

                            RefreshStats();
                        }
                        else
                        {
                            Connection.ReceiveChat(string.Format(Connection.Language.LearnBook4Failed, magic.Level + 1), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.LearnBook4Failed, magic.Level + 1), MessageType.System);

                            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });
                        }
                    }
                    else
                    {
                        magic = SEnvir.UserMagicList.CreateNewObject();
                        magic.Character = Character;
                        magic.Info = info;
                        Magics[info.Magic] = magic;

                        Enqueue(new S.NewMagic { Magic = magic.ToClientInfo() });

                        Connection.ReceiveChat(string.Format(Connection.Language.LearnBookSuccess, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.LearnBookSuccess, magic.Info.Name), MessageType.System);

                        RefreshStats();
                    }

                    break;
                case ItemType.ItemPart:
                    ItemInfo partInfo = SEnvir.ItemInfoList.Binding.First(x => x.Index == item.Stats[Stat.ItemIndex]);

                    if (partInfo.PartCount < 1 || partInfo.PartCount > item.Count) return;

                    if (!CanGainItems(false, new ItemCheck(partInfo, 1, UserItemFlags.None, TimeSpan.Zero))) return;

                    useCount = partInfo.PartCount;

                    gainItem = SEnvir.CreateDropItem(partInfo, 2);

                    break;
                default:
                    return;
            }

            result.Success = true;

            BuffRemove(BuffType.Cloak);
            BuffRemove(BuffType.Transparency);

            if (item.Count > useCount)
            {
                item.Count -= useCount;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                fromArray[link.Slot] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            if (gainItem != null)
                GainItem(gainItem);

            Companion?.RefreshWeight();
            RefreshWeight();
        }
        public bool CanStartWith(ItemInfo info)
        {
            switch (Gender)
            {
                case MirGender.Male:
                    if ((info.RequiredGender & RequiredGender.Male) != RequiredGender.Male)
                        return false;
                    break;
                case MirGender.Female:
                    if ((info.RequiredGender & RequiredGender.Female) != RequiredGender.Female)
                        return false;
                    break;
            }

            switch (Class)
            {
                case MirClass.Warrior:
                    if ((info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                        return false;
                    break;
                case MirClass.Wizard:
                    if ((info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                        return false;
                    break;
                case MirClass.Taoist:
                    if ((info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                        return false;
                    break;
                case MirClass.Assassin:
                    if ((info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                        return false;
                    break;
            }

            return true;
        }
        public bool CanUseItem(UserItem item)
        {
            switch (Gender)
            {
                case MirGender.Male:
                    if ((item.Info.RequiredGender & RequiredGender.Male) != RequiredGender.Male)
                        return false;
                    break;
                case MirGender.Female:
                    if ((item.Info.RequiredGender & RequiredGender.Female) != RequiredGender.Female)
                        return false;
                    break;
            }

            switch (Class)
            {
                case MirClass.Warrior:
                    if ((item.Info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                        return false;
                    break;
                case MirClass.Wizard:
                    if ((item.Info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                        return false;
                    break;
                case MirClass.Taoist:
                    if ((item.Info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                        return false;
                    break;
                case MirClass.Assassin:
                    if ((item.Info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                        return false;
                    break;
            }


            switch (item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (Level < item.Info.RequiredAmount && Stats[Stat.Rebirth] == 0) return false;
                    break;
                case RequiredType.MaxLevel:
                    if (Level > item.Info.RequiredAmount || Stats[Stat.Rebirth] > 0) return false;
                    break;
                case RequiredType.CompanionLevel:
                    if (Companion == null) return false;

                    if (Companion.UserCompanion.Level < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (Companion == null) return false;

                    if (Companion.UserCompanion.Level > item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.AC:
                    if (Stats[Stat.MaxAC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MR:
                    if (Stats[Stat.MaxMR] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.DC:
                    if (Stats[Stat.MaxDC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MC:
                    if (Stats[Stat.MaxMC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.SC:
                    if (Stats[Stat.MaxSC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Health:
                    if (Stats[Stat.Health] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Mana:
                    if (Stats[Stat.Mana] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Accuracy:
                    if (Stats[Stat.Accuracy] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Agility:
                    if (Stats[Stat.Agility] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.RebirthLevel:
                    if (Stats[Stat.Rebirth] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxRebirthLevel:
                    if (Stats[Stat.Rebirth] > item.Info.RequiredAmount) return false;
                    break;
            }


            switch (item.Info.ItemType)
            {
                case ItemType.Book:
                    MagicInfo magic = SEnvir.MagicInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);
                    if (magic == null) return false;
                    if (Magics.ContainsKey(magic.Magic) && (Magics[magic.Magic].Level < 3 || (item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)) return false;
                    return true;
                case ItemType.Consumable:
                    switch (item.Info.Shape)
                    {
                        case 1: //Item Buffs
                            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == item.Info.Index);

                            if (buff != null && buff.RemainingTime == TimeSpan.MaxValue) return false;
                            break;
                    }
                    break;
            }

            return true;
        }
        public void ItemMove(C.ItemMove p)
        {
            S.ItemMove result = new S.ItemMove
            {
                FromGrid = p.FromGrid,
                FromSlot = p.FromSlot,
                ToGrid = p.ToGrid,
                ToSlot = p.ToSlot,
                MergeItem = p.MergeItem,

                ObserverPacket = p.ToGrid != GridType.GuildStorage && p.FromGrid != GridType.GuildStorage,
            };

            Enqueue(result);


            if (Dead || (p.FromGrid == p.ToGrid && p.FromSlot == p.ToSlot)) return;

            UserItem[] fromArray, toArray;

            switch (p.FromGrid)
            {
                case GridType.Inventory:
                    fromArray = Inventory;
                    break;
                case GridType.Equipment:
                    fromArray = Equipment;
                    break;
                case GridType.PartsStorage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }

                    fromArray = PartsStorage;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }

                    fromArray = Storage;

                    if (p.FromSlot >= Character.Account.StorageSize) return;
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat(Connection.Language.GuildStoragePermission, MessageType.System);
                        return;
                    }

                    if (!InSafeZone && (p.ToGrid != GridType.Storage || p.ToGrid != GridType.PartsStorage))
                    {
                        Connection.ReceiveChat(Connection.Language.GuildStorageSafeZone, MessageType.System);
                        return;
                    }

                    fromArray = Character.Account.GuildMember.Guild.Storage;

                    if (p.FromSlot >= Character.Account.GuildMember.Guild.StorageSize) return;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    if (p.FromSlot >= Companion.Stats[Stat.CompanionInventory]) return;
                    break;
                case GridType.CompanionEquipment:
                    if (Companion == null) return;

                    fromArray = Companion.Equipment;
                    break;
                default:
                    return;
            }

            if (p.FromSlot < 0 || p.FromSlot >= fromArray.Length) return;

            UserItem fromItem = fromArray[p.FromSlot];

            if (fromItem == null) return;
            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;


            switch (p.ToGrid)
            {
                case GridType.Inventory:
                    toArray = Inventory;
                    break;
                case GridType.Equipment:
                    toArray = Equipment;
                    break;
                case GridType.PartsStorage:

                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }

                    toArray = PartsStorage;

                    break;
                case GridType.Storage:

                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }

                    toArray = Storage;

                    if (p.ToSlot >= Character.Account.StorageSize) return;
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat(Connection.Language.GuildStoragePermission, MessageType.System);
                        return;
                    }

                    if (!InSafeZone && p.FromGrid != GridType.Storage)
                    {
                        Connection.ReceiveChat(Connection.Language.GuildStorageSafeZone, MessageType.System);
                        return;
                    }

                    toArray = Character.Account.GuildMember.Guild.Storage;

                    if (p.ToSlot >= Character.Account.GuildMember.Guild.StorageSize) return;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    toArray = Companion.Inventory;

                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory]) return;
                    break;
                case GridType.CompanionEquipment:
                    if (Companion == null) return;

                    toArray = Companion.Equipment;
                    break;
                default:
                    return;
            }

            if (p.ToSlot < 0 || p.ToSlot >= toArray.Length) return;

            UserItem toItem = toArray[p.ToSlot];

            if (toItem != null && (toItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (p.FromGrid == GridType.Equipment)
            {
                //CanRemove Item
                if (p.ToGrid == GridType.Equipment) return;

                if (!p.MergeItem && toItem != null) return;
            }

            if (p.FromGrid == GridType.CompanionEquipment)
            {
                //CanRemove Item
                if (p.ToGrid == GridType.CompanionEquipment) return;

                if (!p.MergeItem && toItem != null) return;

                if (p.ToGrid == GridType.CompanionInventory)
                {
                    int space = fromItem.Stats[Stat.CompanionInventory] + fromItem.Info.Stats[Stat.CompanionInventory];

                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory] - space) return;
                }
            }

            if (p.ToGrid == GridType.Equipment)
            {
                if (!CanWearItem(fromItem, (EquipmentSlot)p.ToSlot)) return;
            }

            if (p.ToGrid == GridType.CompanionEquipment)
            {
                if (!Companion.CanWearItem(fromItem, (CompanionSlot)p.ToSlot)) return;

                if (p.FromGrid == GridType.CompanionInventory && toItem != null)
                {
                    int space = fromItem.Stats[Stat.CompanionInventory] + fromItem.Info.Stats[Stat.CompanionInventory]
                                - toItem.Stats[Stat.CompanionInventory] - toItem.Info.Stats[Stat.CompanionInventory];

                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory] + space) return;
                }
            }

            if (p.ToGrid == GridType.CompanionInventory && p.FromGrid != GridType.CompanionInventory)
            {
                int weight = 0;

                switch (fromItem.Info.ItemType)
                {

                    case ItemType.Poison:
                    case ItemType.Amulet:
                        if (p.MergeItem) break;
                        weight = fromItem.Weight;

                        if (toItem != null)
                            weight -= toItem.Weight;

                        break;
                    default:
                        if (p.MergeItem)
                        {
                            if (toItem != null && toItem.Count < toItem.Info.StackSize)
                                weight = (int)(Math.Min(fromItem.Count, toItem.Info.StackSize - toItem.Count) * fromItem.Info.Weight);
                        }
                        else
                        {
                            weight = fromItem.Weight;

                            if (toItem != null)
                                weight -= toItem.Weight;
                        }
                        break;
                }

                if (Companion.BagWeight + weight > Companion.Stats[Stat.CompanionBagWeight])
                {
                    Connection.ReceiveChat(Connection.Language.CompanionNoRoom, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.CompanionNoRoom, MessageType.System);
                    return;
                }
            }
            if (p.FromGrid == GridType.CompanionInventory && p.ToGrid != GridType.CompanionInventory && toItem != null && !p.MergeItem)
            {
                int weight = toItem.Weight;

                weight -= fromItem.Weight;

                if (Companion.BagWeight + weight > Companion.Stats[Stat.CompanionBagWeight])
                {
                    Connection.ReceiveChat(Connection.Language.CompanionNoRoom, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.CompanionNoRoom, MessageType.System);
                    return;
                }
            }



            Packet guildpacket;
            if (p.MergeItem)
            {
                if (toItem == null || toItem.Info != fromItem.Info || toItem.Count >= toItem.Info.StackSize || toItem.ExpireTime != fromItem.ExpireTime) return;

                if ((toItem.Flags & UserItemFlags.Bound) != (fromItem.Flags & UserItemFlags.Bound)) return;
                if ((toItem.Flags & UserItemFlags.Worthless) != (fromItem.Flags & UserItemFlags.Worthless)) return;
                if ((toItem.Flags & UserItemFlags.Expirable) != (fromItem.Flags & UserItemFlags.Expirable)) return;
                if ((toItem.Flags & UserItemFlags.NonRefinable) != (fromItem.Flags & UserItemFlags.NonRefinable)) return;
                if (!toItem.Stats.Compare(fromItem.Stats)) return;

                long fromCount, toCount;
                if (toItem.Count + fromItem.Count <= toItem.Info.StackSize)
                {
                    toItem.Count += fromItem.Count;

                    fromArray[p.FromSlot] = null;
                    fromItem.Delete();

                    toCount = toItem.Count;
                    fromCount = 0;

                }
                else
                {
                    fromItem.Count -= fromItem.Info.StackSize - toItem.Count;
                    toItem.Count = toItem.Info.StackSize;

                    toCount = toItem.Count;
                    fromCount = fromItem.Count;
                }

                result.Success = true;
                RefreshWeight();
                Companion?.RefreshWeight();

                if (p.ToGrid == GridType.GuildStorage || p.FromGrid == GridType.GuildStorage)
                {
                    if (p.ToGrid == GridType.GuildStorage)
                    {
                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = p.ToGrid, Slot = p.ToSlot, Count = toCount, },
                            Success = true,

                            ObserverPacket = false
                        };

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(guildpacket);

                        }
                    }
                    else
                    {
                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = p.ToGrid, Slot = p.ToSlot, Count = toCount },
                                Success = true,
                            });
                        }
                    }


                    if (p.FromGrid == GridType.GuildStorage)
                    {
                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = p.FromGrid, Slot = p.FromSlot, Count = fromCount, },
                            Success = true,

                            ObserverPacket = false
                        };

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(guildpacket);

                        }
                    }
                    else
                    {
                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = p.FromGrid, Slot = p.FromSlot, Count = fromCount },
                                Success = true,
                            });
                        }
                    }
                }
                return;
            }

            if (p.ToGrid == GridType.GuildStorage)
            {
                if (toItem != null && p.FromGrid != GridType.GuildStorage) //This should force us to me merging stacks OR empty item?
                    return;

                if (!fromItem.Info.CanTrade || (fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;
            }

            if (p.FromGrid == GridType.GuildStorage)
            {
                if (toItem != null && p.ToGrid != GridType.GuildStorage) //This should force us to me merging stacks OR empty item?
                    return;
            }




            fromArray[p.FromSlot] = toItem;
            toArray[p.ToSlot] = fromItem;
            bool sendShape = false, sendCompanionShape = false;

            switch (p.FromGrid)
            {
                case GridType.Inventory:
                    if (toItem == null) break;

                    toItem.Slot = p.FromSlot;
                    toItem.Character = Character;
                    break;
                case GridType.Equipment:
                    sendShape = true;
                    if (toItem == null) break;
                    throw new Exception("Shitty Move Item Logic");
                case GridType.CompanionInventory:
                    if (toItem == null) break;

                    toItem.Slot = p.FromSlot;
                    toItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.CompanionEquipment:
                    sendCompanionShape = true;
                    if (toItem == null) break;
                    throw new Exception("Shitty Move Item Logic");
                case GridType.PartsStorage:
                    if (toItem == null) break;

                    toItem.Slot = p.FromSlot;
                    toItem.Account = Character.Account;
                    break;
                case GridType.Storage:
                    if (toItem == null) break;

                    toItem.Slot = p.FromSlot;
                    toItem.Account = Character.Account;
                    break;
                case GridType.GuildStorage:
                    if (p.ToGrid == GridType.GuildStorage)
                    {
                        //GuildStore -> GuildStore send Update to other players
                        guildpacket = new S.ItemMove
                        {
                            FromGrid = p.FromGrid,
                            FromSlot = p.FromSlot,
                            ToGrid = p.ToGrid,
                            ToSlot = p.ToSlot,
                            MergeItem = p.MergeItem,
                            Success = true,
                            ObserverPacket = false,
                        };
                    }
                    else
                    {
                        //Sendto MY observers I got item from guild store and what slot?

                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.GuildGetItem
                            {
                                Grid = p.ToGrid,
                                Slot = p.ToSlot,
                                Item = fromItem.ToClientInfo(), //To Destination IS to be empty, so Not merging
                            });
                        }

                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = p.FromGrid, Slot = p.FromSlot, },
                            Success = true,

                            ObserverPacket = false
                        };
                    }

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject player = member.Account.Connection?.Player;

                        if (player == null || player == this) continue;

                        //Send Removal Command
                        player.Enqueue(guildpacket);
                    }

                    if (toItem == null) break; //CAN ONLY BE GS -> GS

                    toItem.Slot = p.FromSlot;
                    break;
            }


            switch (p.ToGrid)
            {
                case GridType.Inventory:
                    fromItem.Slot = p.ToSlot;
                    fromItem.Character = Character;
                    break;
                case GridType.Equipment:
                    sendShape = true;
                    fromItem.Slot = p.ToSlot + Globals.EquipmentOffSet;
                    fromItem.Character = Character;
                    break;
                case GridType.CompanionInventory:
                    fromItem.Slot = p.ToSlot;
                    fromItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.CompanionEquipment:
                    sendCompanionShape = true;
                    fromItem.Slot = p.ToSlot + Globals.EquipmentOffSet;
                    fromItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.PartsStorage:
                    fromItem.Slot = p.ToSlot + Globals.PartsStorageOffset;
                    fromItem.Account = Character.Account;
                    break;
                case GridType.Storage:
                    fromItem.Slot = p.ToSlot;
                    fromItem.Account = Character.Account;
                    break;
                case GridType.GuildStorage:
                    fromItem.Slot = p.ToSlot;
                    fromItem.Guild = Character.Account.GuildMember.Guild;

                    if (p.FromGrid == GridType.GuildStorage) break; //Already Handled

                    //Must be removing from player to GuildStorage, Update Observer's bag
                    foreach (SConnection con in Connection.Observers)
                    {
                        con.Enqueue(new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = p.FromGrid, Slot = p.FromSlot }, //Should Always be Count = 0;
                            Success = true,
                        });
                    }

                    guildpacket = new S.GuildNewItem
                    {
                        Slot = p.ToSlot,
                        Item = fromItem.ToClientInfo(),
                        ObserverPacket = false
                    };

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject player = member.Account.Connection?.Player;

                        if (player == null || player == this) continue;

                        player.Enqueue(guildpacket);
                    }

                    break;
            }


            result.Success = true;

            RefreshStats();

            if (sendShape) SendShapeUpdate();
            if (sendCompanionShape) Companion?.SendShapeUpdate();

            if (p.ToGrid == GridType.CompanionEquipment || p.ToGrid == GridType.CompanionInventory || p.FromGrid == GridType.CompanionEquipment || p.FromGrid == GridType.CompanionInventory)
            {
                Companion.SearchTime = DateTime.MinValue;
                Companion.RefreshStats();
            }
        }

        public long GetItemCount(ItemInfo info)
        {
            long count = 0;
            foreach (UserItem item in Inventory)
            {
                if (item == null || item.Info != info) continue;

                count += item.Count;
            }

            if (Companion != null)
            {
                foreach (UserItem item in Companion.Inventory)
                {
                    if (item == null || item.Info != info) continue;

                    count += item.Count;
                }
            }

            return count;
        }
        public void TakeItem(ItemInfo info, long count)
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];

                if (item == null || item.Info != info) continue;

                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item);
                Inventory[i] = null;
                item.Delete();

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });

                if (count == 0) return;
            }

            for (int i = 0; i < Companion.Inventory.Length; i++)
            {
                UserItem item = Companion.Inventory[i];

                if (item == null || item.Info != info) continue;

                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item);
                Companion.Inventory[i] = null;
                item.Delete();

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i }, Success = true });

                if (count == 0) return;
            }

            throw new Exception(string.Format("Unable to Take {0}x{1} from {2}", info.ItemName, count, Name));
        }

        public void ItemLock(C.ItemLock p)
        {
            UserItem[] itemArray;

            switch (p.GridType)
            {
                case GridType.Inventory:
                    itemArray = Inventory;
                    break;
                case GridType.Equipment:
                    itemArray = Equipment;
                    break;
                case GridType.PartsStorage:
                    itemArray = PartsStorage;
                    break;
                case GridType.Storage:
                    itemArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    itemArray = Companion.Inventory;
                    break;
                case GridType.CompanionEquipment:
                    if (Companion == null) return;

                    itemArray = Companion.Equipment;
                    break;
                default:
                    return;
            }

            if (p.SlotIndex < 0 || p.SlotIndex >= itemArray.Length) return;


            UserItem fromItem = itemArray[p.SlotIndex];

            if (fromItem == null) return;

            if (p.Locked)
                fromItem.Flags |= UserItemFlags.Locked;
            else
                fromItem.Flags &= ~UserItemFlags.Locked;

            S.ItemLock result = new S.ItemLock
            {
                Grid = p.GridType,
                Slot = p.SlotIndex,
                Locked = p.Locked,
            };

            Enqueue(result);

        }
        public void ItemSplit(C.ItemSplit p)
        {
            S.ItemSplit result = new S.ItemSplit
            {
                Grid = p.Grid,
                Slot = p.Slot,
                Count = p.Count,
                ObserverPacket = p.Grid != GridType.GuildStorage,
            };

            Enqueue(result);

            if (Dead || p.Count <= 0) return;

            UserItem[] array;

            switch (p.Grid)
            {
                case GridType.Inventory:
                    array = Inventory;
                    break;
                case GridType.PartsStorage:
                    array = PartsStorage;
                    break;
                case GridType.Storage:
                    array = Storage;
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return;

                    array = Character.Account.GuildMember.Guild.Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    array = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Slot < 0 || p.Slot >= array.Length) return;

            UserItem item = array[p.Slot];

            if (item == null || item.Count <= p.Count || item.Info.StackSize < p.Count) return;

            int length = array.Length;
            if (p.Grid == GridType.CompanionInventory)
                length = Math.Min(array.Length, Companion.Stats[Stat.CompanionInventory]);

            if (p.Grid == GridType.Storage)
                length = Math.Min(array.Length, Character.Account.StorageSize);

            for (int i = 0; i < length; i++)
            {
                if (array[i] != null) continue;

                if (p.Grid == GridType.GuildStorage && i >= Character.Account.GuildMember.Guild.StorageSize) break;


                result.Success = true;
                result.NewSlot = i;

                item.Count -= p.Count;

                UserItem newItem = SEnvir.CreateFreshItem(item);
                newItem.Count = p.Count;

                array[i] = newItem;
                newItem.Slot = i;

                switch (p.Grid)
                {
                    case GridType.Inventory:
                        newItem.Character = Character;
                        break;
                    case GridType.PartsStorage:
                        newItem.Account = Character.Account;
                        break;
                    case GridType.Storage:
                        newItem.Account = Character.Account;
                        break;
                    case GridType.CompanionInventory:
                        newItem.Companion = Companion.UserCompanion;
                        break;
                    case GridType.GuildStorage:
                        newItem.Guild = Character.Account.GuildMember.Guild;

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = p.Grid, Slot = p.Slot, Count = item.Count },
                                Success = true,

                                ObserverPacket = false
                            });

                            player.Enqueue(new S.GuildNewItem
                            {
                                Slot = newItem.Slot,
                                Item = newItem.ToClientInfo(),

                                ObserverPacket = false
                            });
                        }
                        break;
                }

                return;
            }
        }

        public void GoldChanged()
        {
            Enqueue(new S.GoldChanged { Gold = Gold });
        }

        public void ItemDrop(C.ItemDrop p)
        {
            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link
            };
            Enqueue(result);

            if (Dead || !ParseLinks(p.Link))
                return;


            UserItem[] fromArray;

            switch (p.Link.GridType)
            {
                case GridType.Inventory:
                    fromArray = Inventory;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Link.Slot < 0 || p.Link.Slot >= fromArray.Length) return;

            UserItem fromItem = fromArray[p.Link.Slot];

            if (fromItem == null || p.Link.Count > fromItem.Count || !fromItem.Info.CanDrop || (fromItem.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;

            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
            Cell cell = GetDropLocation(1, null);

            if (cell == null) return;

            result.Success = true;

            UserItem dropItem;

            if (p.Link.Count == fromItem.Count)
            {
                dropItem = fromItem;
                RemoveItem(fromItem);
                fromArray[p.Link.Slot] = null;

                result.Link.Count = 0;
            }
            else
            {
                dropItem = SEnvir.CreateFreshItem(fromItem);
                dropItem.Count = p.Link.Count;
                fromItem.Count -= p.Link.Count;

                result.Link.Count = fromItem.Count;
            }

            RefreshWeight();
            Companion?.RefreshWeight();
            dropItem.IsTemporary = true;

            ItemObject ob = new ItemObject
            {
                Item = dropItem,
            };

            if ((fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
                ob.Account = Character.Account;

            ob.Spawn(CurrentMap, cell.Location);
        }
        public void GoldDrop(C.GoldDrop p)
        {
            if (Dead || p.Amount <= 0 || p.Amount > Gold) return;


            Cell cell = GetDropLocation(Config.DropDistance, null);

            if (cell == null) return;

            Gold -= p.Amount;
            GoldChanged();

            UserItem dropItem = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
            dropItem.Count = p.Amount;
            dropItem.IsTemporary = true;

            ItemObject ob = new ItemObject
            {
                Item = dropItem,
            };

            ob.Spawn(CurrentMap, cell.Location);
        }
        public void BeltLinkChanged(C.BeltLinkChanged p)
        {
            if (p.Slot < 0 || p.Slot >= Globals.MaxBeltCount) return;
            if (p.LinkIndex > 0 && p.LinkItemIndex > 0) return;
            if (p.Slot >= Inventory.Length) return;

            ItemInfo info = null;
            UserItem item = null;

            if (p.LinkIndex > 0)
                info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == p.LinkIndex);
            else if (p.Slot > 0)
                item = Inventory.FirstOrDefault(x => x?.Index == p.LinkItemIndex);

            foreach (CharacterBeltLink link in Character.BeltLinks)
            {
                if (link.Slot != p.Slot && (link.LinkInfoIndex != -1 || link.LinkItemIndex != -1)) continue;

                link.Slot = p.Slot;
                link.LinkInfoIndex = info?.Index ?? -1;
                link.LinkItemIndex = item?.Index ?? -1;
                return;
            }

            if (info == null && item == null) return;

            CharacterBeltLink bLink = SEnvir.BeltLinkList.CreateNewObject();

            bLink.Character = Character;
            bLink.Slot = p.Slot;
            bLink.LinkInfoIndex = p.LinkIndex;
            bLink.LinkItemIndex = p.LinkItemIndex;

        }
        public void AutoPotionLinkChanged(C.AutoPotionLinkChanged p)
        {
            if (p.Slot < 0 || p.Slot >= Globals.MaxAutoPotionCount) return;
            if (p.Slot >= Inventory.Length) return;

            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == p.LinkIndex);

            foreach (AutoPotionLink link in Character.AutoPotionLinks)
            {
                if (link.Slot != p.Slot) continue;

                link.Slot = p.Slot;
                link.LinkInfoIndex = info?.Index ?? -1;
                link.Health = p.Health;
                link.Mana = p.Mana;
                link.Enabled = p.Enabled;
                return;
            }

            AutoPotionLink aLink = SEnvir.AutoPotionLinkList.CreateNewObject();

            aLink.Character = Character;
            aLink.Slot = p.Slot;
            aLink.LinkInfoIndex = info?.Index ?? -1;
            aLink.Health = p.Health;
            aLink.Mana = p.Mana;
            aLink.Enabled = p.Enabled;

            AutoPotions.Add(aLink);
            AutoPotions.Sort((x1, x2) => x1.Slot.CompareTo(x2.Slot));
        }
        public void PickUp()
        {
            if (Dead) return;

            int range = Stats[Stat.PickUpRadius];

            for (int d = 0; d <= range; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y]; //Direct Access we've checked the boudaries.

                        if (cell?.Objects == null) continue;

                        foreach (MapObject cellObject in cell.Objects)
                        {
                            if (cellObject.Race != ObjectType.Item) continue;

                            ItemObject item = (ItemObject)cellObject;

                            if (item.PickUpItem(this)) return;
                        }

                    }
                }
            }
        }

        public bool CanWearItem(UserItem item, EquipmentSlot slot)
        {
            if (!Functions.CorrectSlot(item.Info.ItemType, slot) || !CanUseItem(item))
                return false;

            switch (item.Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Torch:
                case ItemType.Shield:
                    if (HandWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > Stats[Stat.HandWeight]) return false;
                    break;
                default:
                    if (WearWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > Stats[Stat.WearWeight]) return false;
                    break;

            }
            return true;
        }

        public bool DamageItem(GridType grid, int slot, int rate = 1, bool delayStats = false)
        {
            UserItem item;
            switch (grid)
            {
                case GridType.Inventory:
                    item = Inventory[slot];
                    break;
                case GridType.Equipment:
                    item = Equipment[slot];
                    break;
                default:
                    return false;
            }

            if (item == null || item.Info.Durability == 0 || item.CurrentDurability == 0) return false;

            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;

            switch (item.Info.ItemType)
            {
                case ItemType.Nothing:
                case ItemType.Consumable:
                case ItemType.Poison:
                case ItemType.Amulet:
                case ItemType.Scroll:
                    return false;
                case ItemType.Weapon:
                    if (SEnvir.Random.Next(Stats[Stat.Strength]) > 0) return false;
                    break;
                default:
                    if (SEnvir.Random.Next(3) == 0 && SEnvir.Random.Next(Stats[Stat.Strength]) > 0) return false;
                    break;
            }

            item.CurrentDurability = Math.Max(0, item.CurrentDurability - rate);

            Enqueue(new S.ItemDurability
            {
                GridType = grid,
                Slot = slot,
                CurrentDurability = item.CurrentDurability,
            });

            if (item.CurrentDurability == 0)
            {
                SendShapeUpdate();
                RefreshStats();
                return true;
            }
            return false;
        }
        public void DamageDarkStone(int rate = 1)
        {
            DamageItem(GridType.Equipment, (int)EquipmentSlot.Amulet, rate);

            UserItem stone = Equipment[(int)EquipmentSlot.Amulet];

            if (stone == null || stone.CurrentDurability != 0 || stone.Info.Durability <= 0) return;

            RemoveItem(stone);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            stone.Delete();

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet },
                Success = true,
            });
        }

        public bool UsePoison(int count, out int shape)
        {
            shape = 0;

            UserItem poison = Equipment[(int)EquipmentSlot.Poison];

            if (poison == null || poison.Info.ItemType != ItemType.Poison || poison.Count < count) return false;

            shape = poison.Info.Shape;

            poison.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Poison, Count = poison.Count },
                Success = true
            });


            if (poison.Count != 0) return true;

            RemoveItem(poison);
            Equipment[(int)EquipmentSlot.Poison] = null;
            poison.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }
        public bool UseAmulet(int count, int shape)
        {
            UserItem amulet = Equipment[(int)EquipmentSlot.Amulet];

            if (amulet == null || amulet.Info.ItemType != ItemType.Amulet || amulet.Count < count || amulet.Info.Shape != shape) return false;

            amulet.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet, Count = amulet.Count },
                Success = true
            });




            if (amulet.Count != 0) return true;

            RemoveItem(amulet);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            amulet.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }
        public bool UseAmulet(int count, int shape, out Stats stats)
        {
            stats = null;
            UserItem amulet = Equipment[(int)EquipmentSlot.Amulet];

            if (amulet == null || amulet.Info.ItemType != ItemType.Amulet || amulet.Count < count || amulet.Info.Shape != shape) return false;

            amulet.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet, Count = amulet.Count },
                Success = true
            });

            stats = new Stats(amulet.Info.Stats);

            if (amulet.Count != 0) return true;

            RemoveItem(amulet);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            amulet.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }

        public bool UseOilOfBenediction()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return false;

            int luck = 0;

            foreach (UserItemStat stat in weapon.AddedStats)
            {
                if (stat.Stat != Stat.Luck) continue;
                if (stat.StatSource != StatSource.Enhancement) continue;

                luck += stat.Amount;
            }

            if (luck >= Config.MaxLuck) return false;

            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
            Enqueue(result);

            if (luck > Config.MaxCurse && SEnvir.Random.Next(Config.CurseRate) == 0)
            {
                weapon.AddStat(Stat.Luck, -1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Luck]--;

                Stats[Stat.Luck]--;
            }
            else if (luck <= 0 || SEnvir.Random.Next(luck * Config.LuckRate) == 0)
            {
                weapon.AddStat(Stat.Luck, 1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Luck]++;

                Stats[Stat.Luck]++;
            }

            return true;
        }
        public bool UseOilOfConservation()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            int strength = 0;

            if (weapon == null) return false;

            foreach (UserItemStat stat in weapon.AddedStats)
            {
                if (stat.Stat != Stat.Strength) continue;
                if (stat.StatSource != StatSource.Enhancement) continue;

                strength += stat.Amount;
            }

            if (strength >= Config.MaxLuck) return false;



            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
            Enqueue(result);

            if (strength > 0 && SEnvir.Random.Next(Config.StrengthLossRate) == 0)
            {
                weapon.AddStat(Stat.Strength, -1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Strength]--;
            }
            else if (strength <= 0 || SEnvir.Random.Next(strength * Config.StrengthAddRate) == 0)
            {
                weapon.AddStat(Stat.Strength, 1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Strength]++;
            }

            return true;
        }

        public bool SpecialRepair(EquipmentSlot slot)
        {
            UserItem item = Equipment[(int)slot];

            if (item == null) return false;

            if (item.CurrentDurability >= item.MaxDurability || !item.Info.CanRepair) return false;

            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;

            item.CurrentDurability = item.MaxDurability;

            Enqueue(new S.NPCRepair { Links = new List<CellLinkInfo> { new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)slot, Count = 1 } }, Special = true, Success = true, SpecialRepairDelay = TimeSpan.Zero });

            return true;
        }

        public void HelmetToggle(bool value)
        {
            if (Character.HideHelmet == value) return;

            Character.HideHelmet = value;
            SendShapeUpdate();
            Enqueue(new S.HelmetToggle { HideHelmet = Character.HideHelmet });
        }
        #endregion

        #region Change

        public void GenderChange(C.GenderChange p)
        {
            switch (p.Gender)
            {
                case MirGender.Male:
                    if (Gender == MirGender.Male) return;
                    break;
                case MirGender.Female:
                    if (Gender == MirGender.Female) return;
                    break;
            }

            if (p.HairType < 0) return;

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255)) return;

            if (Equipment[(int)EquipmentSlot.Armour] != null) return;

            switch (Class)
            {
                case MirClass.Warrior:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Wizard:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Taoist:
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Assassin:
                    if (p.HairType > 5) return;
                    break;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.GenderChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }


            Character.Gender = p.Gender;
            Character.HairType = p.HairType;
            Character.HairColour = p.HairColour;

            SendChangeUpdate();
        }
        public void HairChange(C.HairChange p)
        {
            if (p.HairType < 0) return;

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255)) return;

            switch (Class)
            {
                case MirClass.Warrior:
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Wizard:
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Taoist:
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Assassin:
                    if (p.HairType > 5) return;
                    break;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.HairChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }


            Character.HairType = p.HairType;
            Character.HairColour = p.HairColour;

            SendChangeUpdate();
        }
        public void ArmourDye(Color colour)
        {
            if (Equipment[(int)EquipmentSlot.Armour] == null) return;

            switch (Class)
            {
                case MirClass.Warrior:
                case MirClass.Wizard:
                case MirClass.Taoist:
                    if (colour.A != 255) return;
                    break;
                case MirClass.Assassin:
                    if (colour.ToArgb() != 0) return;
                    return;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.ArmourDye) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            Equipment[(int)EquipmentSlot.Armour].Colour = colour;


            SendChangeUpdate();
        }
        public void NameChange(string newName)
        {
            if (!Globals.CharacterReg.IsMatch(newName))
            {
                Connection.ReceiveChat("Unacceptable character name.", MessageType.System);
                return;
            }

            if (newName == Name)
            {
                Connection.ReceiveChat($"Your name is already {newName}.", MessageType.System);
                return;
            }

            for (int i = 0; i < SEnvir.CharacterInfoList.Count; i++)
                if (string.Compare(SEnvir.CharacterInfoList[i].CharacterName, newName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (SEnvir.CharacterInfoList[i].Account == Character.Account) continue;

                    Connection.ReceiveChat("This name is already in use.", MessageType.System);
                    return;
                }


            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.NameChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            SEnvir.Log($"[NAME CHANGED] Old: {Name}, New: {newName}.", true);
            Name = newName;

            SendChangeUpdate();
        }
        public void FortuneCheck(int index)
        {
            if (SEnvir.FortuneCheckerInfo == null) return;


            long count = GetItemCount(SEnvir.FortuneCheckerInfo);

            if (count == 0)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.NeedItem, SEnvir.FortuneCheckerInfo.ItemName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.NeedItem, SEnvir.FortuneCheckerInfo.ItemName), MessageType.System);
                return;
            }

            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == index);

            if (info == null || info.Drops.Count == 0) return;

            if (Config.TestServer && info.Effect != ItemEffect.Gold) return;

            UserFortuneInfo savedFortune = null;

            foreach (UserFortuneInfo fortune in Character.Account.Fortunes)
            {
                if (fortune.Item != info) continue;

                savedFortune = fortune;
                break;
            }

            TakeItem(SEnvir.FortuneCheckerInfo, 1);

            if (savedFortune == null)
            {
                savedFortune = SEnvir.UserFortuneInfoList.CreateNewObject();
                savedFortune.Account = Character.Account;
                savedFortune.Item = info;
            }

            UserDrop drop = Character.Account.UserDrops.FirstOrDefault(x => x.Item == info);

            savedFortune.CheckTime = SEnvir.Now;


            if (drop != null)
            {
                savedFortune.DropCount = drop.DropCount;
                savedFortune.DropProgress = drop.Progress;
            }

            Enqueue(new S.FortuneUpdate { Fortunes = new List<ClientFortuneInfo> { savedFortune.ToClientInfo() } });
        }


        #endregion

        #region Buffs

        public void ApplyMapBuff()
        {
            BuffRemove(BuffType.MapEffect);

            if (CurrentMap == null) return;

            Stats stats = new Stats();

            stats[Stat.MonsterHealth] = CurrentMap.Info.MonsterHealth;
            stats[Stat.MonsterDamage] = CurrentMap.Info.MonsterDamage;
            stats[Stat.MonsterExperience] = CurrentMap.Info.ExperienceRate;
            stats[Stat.MonsterDrop] = CurrentMap.Info.DropRate;
            stats[Stat.MonsterGold] = CurrentMap.Info.GoldRate;

            stats[Stat.MaxMonsterHealth] = CurrentMap.Info.MaxMonsterHealth;
            stats[Stat.MaxMonsterDamage] = CurrentMap.Info.MaxMonsterDamage;
            stats[Stat.MaxMonsterExperience] = CurrentMap.Info.MaxExperienceRate;
            stats[Stat.MaxMonsterDrop] = CurrentMap.Info.MaxDropRate;
            stats[Stat.MaxMonsterGold] = CurrentMap.Info.MaxGoldRate;

            if (stats.Count == 0) return;

            BuffAdd(BuffType.MapEffect, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        public void ApplyServerBuff()
        {
            BuffRemove(BuffType.Server);

            Stats stats = new Stats();

            stats[Stat.BaseExperienceRate] += Config.ExperienceRate;
            stats[Stat.BaseDropRate] += Config.DropRate;
            stats[Stat.BaseGoldRate] += Config.GoldRate;
            stats[Stat.SkillRate] = Config.SkillRate;
            stats[Stat.CompanionRate] = Config.CompanionRate;


            if (stats.Count == 0) return;

            BuffAdd(BuffType.Server, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        public void ApplyObserverBuff()
        {
            BuffRemove(BuffType.Observable);

            if (!Character.Observable) return;

            Stats stats = new Stats();

            stats[Stat.ExperienceRate] += 15;
            stats[Stat.DropRate] += 15;
            stats[Stat.GoldRate] += 15;

            BuffAdd(BuffType.Observable, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        public void ApplyCastleBuff()
        {
            BuffRemove(BuffType.Castle);

            if (Character.Account.GuildMember?.Guild.Castle == null) return;

            Stats stats = new Stats();

            stats[Stat.ExperienceRate] += 10;
            stats[Stat.DropRate] += 10;
            stats[Stat.GoldRate] += 10;

            BuffAdd(BuffType.Castle, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        public void ApplyGuildBuff()
        {
            BuffRemove(BuffType.Guild);

            if (Character.Account.GuildMember == null) return;

            Stats stats = new Stats();

            if (Character.Account.GuildMember.Guild.StarterGuild)
            {
                if (Level < 50)
                {
                    stats[Stat.ExperienceRate] += 50;
                    stats[Stat.DropRate] += 50;
                    stats[Stat.GoldRate] += 50;
                }
                else
                {
                    stats[Stat.ExperienceRate] -= 50;
                    stats[Stat.DropRate] -= 50;
                    stats[Stat.GoldRate] -= 50;
                }
            }
            else if (Character.Account.GuildMember.Guild.Members.Count <= 15)
            {
                stats[Stat.ExperienceRate] += 30;
                stats[Stat.DropRate] += 30;
                stats[Stat.GoldRate] += 30;
            }
            else if (Character.Account.GuildMember.Guild.Members.Count <= 30)
            {
                stats[Stat.ExperienceRate] += 23;
                stats[Stat.DropRate] += 23;
                stats[Stat.GoldRate] += 23;
            }
            else if (Character.Account.GuildMember.Guild.Members.Count <= 45)
            {
                stats[Stat.ExperienceRate] += 18;
                stats[Stat.DropRate] += 18;
                stats[Stat.GoldRate] += 18;
            }
            else
            {
                stats[Stat.ExperienceRate] += 13;
                stats[Stat.DropRate] += 13;
                stats[Stat.GoldRate] += 13;
            }

            if (stats.Count == 0) return;

            if (!Character.Account.GuildMember.Guild.StarterGuild && GroupMembers != null)
            {
                foreach (PlayerObject member in GroupMembers)
                {
                    if (member.Character.Account.GuildMember != null && member.Character.Account.GuildMember.Guild.StarterGuild) continue;

                    if (member.Character.Account.GuildMember?.Guild != Character.Account.GuildMember.Guild) return;
                }
            }

            BuffAdd(BuffType.Guild, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }


        public bool ItemBuffAdd(ItemInfo info)
        {
            switch (info.Effect)
            {
                case ItemEffect.DestructionElixir:
                case ItemEffect.HasteElixir:
                case ItemEffect.LifeElixir:
                case ItemEffect.ManaElixir:
                case ItemEffect.NatureElixir:
                case ItemEffect.SpiritElixir:

                    for (int i = Buffs.Count - 1; i >= 0; i--)
                    {
                        BuffInfo buff = Buffs[i];
                        if (buff.Type != BuffType.ItemBuff || info.Index == buff.ItemIndex) continue; //Same Item don't remove, extend instead 

                        ItemInfo buffItemInfo = SEnvir.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex);

                        if (buffItemInfo.Effect == info.Effect)
                            BuffRemove(buff);
                    }
                    break;
            }

            BuffInfo currentBuff = Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == info.Index);

            if (currentBuff != null) //Extend buff
            {
                if (info.Stats[Stat.Duration] >= 0)
                {
                    TimeSpan duration = TimeSpan.FromSeconds(info.Stats[Stat.Duration]);

                    long ticks = currentBuff.RemainingTime.Ticks - long.MaxValue + duration.Ticks; //Check for Overflow (Probably never going to happen) 403x MaxValue durations refreshes.

                    if (ticks >= 0)
                        currentBuff.RemainingTime = TimeSpan.MaxValue;
                    else
                        currentBuff.RemainingTime += duration;
                }
                else
                    currentBuff.RemainingTime = TimeSpan.MaxValue;

                Enqueue(new S.BuffTime { Index = currentBuff.Index, Time = currentBuff.RemainingTime });
                return true;
            }

            currentBuff = SEnvir.BuffInfoList.CreateNewObject();

            currentBuff.Type = BuffType.ItemBuff;
            currentBuff.ItemIndex = info.Index;
            currentBuff.RemainingTime = info.Stats[Stat.Duration] > 0 ? TimeSpan.FromSeconds(info.Stats[Stat.Duration]) : TimeSpan.MaxValue;

            if (info.RequiredAmount == 0 && info.RequiredClass == RequiredClass.All)
                currentBuff.Account = Character.Account;
            else
                currentBuff.Character = Character;

            currentBuff.Pause = InSafeZone;
            Buffs.Add(currentBuff);
            Enqueue(new S.BuffAdd { Buff = currentBuff.ToClientInfo() });

            RefreshStats();
            AddAllObjects();

            return true;
        }

        public override BuffInfo BuffAdd(BuffType type, TimeSpan remainingTicks, Stats stats, bool visible, bool pause, TimeSpan tickRate)
        {
            BuffInfo info = base.BuffAdd(type, remainingTicks, stats, visible, pause, tickRate);

            info.Character = Character;

            switch (type)
            {
                case BuffType.ItemBuff:
                    info.Pause = InSafeZone;
                    break;
            }

            Enqueue(new S.BuffAdd { Buff = info.ToClientInfo() });

            switch (type)
            {
                case BuffType.StrengthOfFaith:
                    foreach (MonsterObject pet in Pets)
                    {
                        pet.RefreshStats();
                        pet.Magics.Add(Magics[MagicType.StrengthOfFaith]);
                    }
                    break;
                case BuffType.DragonRepulse:
                case BuffType.Companion:
                case BuffType.Server:
                case BuffType.MapEffect:
                case BuffType.Guild:
                case BuffType.Ranking:
                case BuffType.Developer:
                case BuffType.Castle:
                    info.IsTemporary = true;
                    break;
            }

            return info;
        }

        public override void BuffRemove(BuffInfo info)
        {
            int oldHealth = Stats[Stat.Health];

            base.BuffRemove(info);

            Enqueue(new S.BuffRemove { Index = info.Index });


            switch (info.Type)
            {
                case BuffType.StrengthOfFaith:
                    foreach (MonsterObject pet in Pets)
                    {
                        pet.Magics.Remove(Magics[MagicType.StrengthOfFaith]);
                        pet.RefreshStats();
                    }
                    break;
                case BuffType.Renounce:
                    if (Dead) return;
                    ChangeHP(info.Stats[Stat.RenounceHPLost]);
                    break;

                case BuffType.ItemBuff:
                    RefreshStats();
                    RemoveAllObjects();
                    break;
            }
        }

        public void PauseBuffs()
        {
            if (CurrentCell == null) return;

            bool change = false;

            bool pause = InSafeZone;

            foreach (MapObject ob in CurrentCell.Objects)
            {
                if (ob.Race != ObjectType.Spell) continue;

                SpellObject spell = (SpellObject)ob;

                if (spell.Effect != SpellEffect.Rubble) continue;

                pause = true;
                break;
            }

            foreach (BuffInfo buff in Buffs)
            {
                bool buffPause = pause;

                switch (buff.Type)
                {
                    case BuffType.ItemBuff:
                        buffPause = buff.RemainingTime != TimeSpan.MaxValue && pause;
                        break;
                    case BuffType.HuntGold:
                        break;
                    default:
                        continue;
                }

                if (buff.Pause == buffPause) continue;

                buff.Pause = buffPause;
                change = true;

                Enqueue(new S.BuffPaused { Index = buff.Index, Paused = buffPause });
            }

            if (change)
                RefreshStats();
        }
        #endregion

        #region Trade

        public void TradeClose()
        {
            if (TradePartner == null) return;

            Enqueue(new S.TradeClose());

            if (TradePartner?.Node != null)
                TradePartner.Enqueue(new S.TradeClose());

            TradePartner.TradePartner = null;
            TradePartner.TradeItems.Clear();
            TradePartner.TradeGold = 0;
            TradePartner.TradeConfirmed = false;

            TradePartner = null;
            TradeItems.Clear();
            TradeGold = 0;
            TradeConfirmed = false;
        }

        public void TradeRequest()
        {
            if (TradePartner != null)
            {
                Connection.ReceiveChat(Connection.Language.TradeAlreadyTrading, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeAlreadyTrading, MessageType.System);
                return;
            }
            if (TradePartnerRequest != null)
            {
                Connection.ReceiveChat(Connection.Language.TradeAlreadyHaveRequest, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeAlreadyHaveRequest, MessageType.System);
                return;
            }

            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction));

            if (cell?.Objects == null) return;

            PlayerObject player = null;
            foreach (MapObject ob in cell.Objects)
            {
                if (ob.Race != ObjectType.Player) continue;
                player = (PlayerObject)ob;
                break;
            }

            if (player == null || player.Direction != Functions.ShiftDirection(Direction, 4))
            {
                Connection.ReceiveChat(Connection.Language.TradeNeedFace, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeNeedFace, MessageType.System);
                return;
            }

            if (SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat(string.Format(Connection.Language.TradeTargetNotAllowed, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.TradeTargetNotAllowed, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.TradePartner != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.TradeTargetAlreadyTrading, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.TradeTargetAlreadyTrading, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.TradePartnerRequest != null)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.TradeTargetAlreadyHaveRequest, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.TradeTargetAlreadyHaveRequest, player.Character.CharacterName), MessageType.System);
                return;
            }


            if (!player.Character.Account.AllowTrade)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.TradeTargetNotAllowed, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat(string.Format(player.Connection.Language.TradeNotAllowed, Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.TradeTargetNotAllowed, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in player.Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.TradeNotAllowed, Character.CharacterName), MessageType.System);
                return;
            }

            if (player.Dead || Dead)
            {
                Connection.ReceiveChat(Connection.Language.TradeTargetDead, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeTargetDead, MessageType.System);
                return;
            }


            player.TradePartnerRequest = this;
            player.Enqueue(new S.TradeRequest { Name = Name, ObserverPacket = false });

            Connection.ReceiveChat(string.Format(Connection.Language.TradeRequested, player.Character.CharacterName), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(con.Language.TradeRequested, player.Character.CharacterName), MessageType.System);
        }
        public void TradeAccept()
        {
            if (TradePartnerRequest?.Node == null || TradePartnerRequest.TradePartner != null || TradePartnerRequest.Dead ||
                Functions.Distance(CurrentLocation, TradePartnerRequest.CurrentLocation) != 1 || TradePartnerRequest.Direction != Functions.ShiftDirection(Direction, 4))
                return;

            TradePartner = TradePartnerRequest;
            TradePartnerRequest.TradePartner = this;

            TradePartner.Enqueue(new S.TradeOpen { Name = Name });
            Enqueue(new S.TradeOpen { Name = TradePartner.Name });
        }

        public void TradeAddItem(CellLinkInfo cell)
        {
            S.TradeAddItem result = new S.TradeAddItem
            {
                Cell = cell,
            };

            Enqueue(result);

            if (!ParseLinks(cell) || TradePartner == null || TradeItems.Count >= 15) return;

            UserItem[] fromArray;

            switch (cell.GridType)
            {
                case GridType.Inventory:
                    fromArray = Inventory;
                    break;
                case GridType.Equipment:
                    fromArray = Equipment;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                case GridType.PartsStorage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);

                        return;
                    }

                    fromArray = PartsStorage;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);

                        return;
                    }

                    fromArray = Storage;
                    break; /*
                case GridType.GuildStorage:
                    if (Character.GuildMemberInfo == null) return;

                    if (!Character.GuildMemberInfo.Permissions.HasFlag(GuildPermissions.GetItem))
                    {
                        ReceiveChat("You do no have the permissions to take from the guild storage", ChatType.System);
                        return;
                    }

                    if (!CurrentCell.IsSafeZone)
                    {
                        ReceiveChat("You cannot use guild storage unless you are in a safe zone", ChatType.Hint);
                        return;
                    }

                    fromArray = Character.GuildMemberInfo.GuildInfo.StorageArray;
                    break;*/
                default:
                    return;
            }

            if (cell.Slot < 0 || cell.Slot >= fromArray.Length) return;

            UserItem fromItem = fromArray[cell.Slot];

            if (fromItem == null || cell.Count > fromItem.Count || (!TradePartner.Character.Account.Admin && !Character.Account.Admin && ((fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound || !fromItem.Info.CanTrade))) return;
            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (TradeItems.ContainsKey(fromItem)) return;

            //All is Well
            result.Success = true;
            TradeItems[fromItem] = cell;
            S.TradeItemAdded packet = new S.TradeItemAdded
            {
                Item = fromItem.ToClientInfo()
            };
            packet.Item.Count = cell.Count;
            TradePartner.Enqueue(packet);
        }
        public void TradeAddGold(long gold)
        {
            S.TradeAddGold p = new S.TradeAddGold
            {
                Gold = TradeGold,
            };
            Enqueue(p);

            if (TradePartner == null || TradeGold >= gold) return;

            if (gold <= 0 || gold > Gold) return;

            TradeGold = gold;
            p.Gold = TradeGold;

            //All is Well
            S.TradeGoldAdded packet = new S.TradeGoldAdded
            {
                Gold = TradeGold,
            };

            TradePartner.Enqueue(packet);
        }

        public void TradeConfirm()
        {
            if (TradePartner == null) return;

            TradeConfirmed = true;

            if (!TradePartner.TradeConfirmed)
            {
                Connection.ReceiveChat(Connection.Language.TradeWaiting, MessageType.System);
                TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradePartnerWaiting, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeWaiting, MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat(con.Language.TradePartnerWaiting, MessageType.System);

                return;
            }

            long gold = Gold;
            gold += TradePartner.TradeGold - TradeGold;

            if (gold < 0)
            {
                Connection.ReceiveChat(Connection.Language.TradeNoGold, MessageType.System);
                TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradePartnerNoGold, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeNoGold, MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat(con.Language.TradePartnerNoGold, MessageType.System);
                TradeClose();
                return;
            }


            gold = TradePartner.Gold;
            gold += TradeGold - TradePartner.TradeGold;

            if (gold < 0)
            {
                Connection.ReceiveChat(Connection.Language.TradePartnerNoGold, MessageType.System);
                TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeNoGold, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradePartnerNoGold, MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat(con.Language.TradeNoGold, MessageType.System);

                TradeClose();
                return;
            }

            List<ItemCheck> checks = new List<ItemCheck>();

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
            {
                UserItem[] fromArray;
                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.Equipment:
                        fromArray = Equipment;
                        break;
                    case GridType.PartsStorage:
                        fromArray = PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null)
                        {
                            Connection.ReceiveChat(Connection.Language.TradeFailedItemsChanged, MessageType.System);
                            TradePartner.Connection.ReceiveChat(string.Format(TradePartner.Connection.Language.TradeFailedPartnerItemsChanged, Name), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(con.Language.TradeFailedItemsChanged, MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.TradeFailedPartnerItemsChanged, Name), MessageType.System);

                            TradeClose();
                            return;
                        }

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        //MAJOR LOGIC FAILURE 
                        return;
                }

                if (fromArray[pair.Value.Slot] != pair.Key || pair.Key.Count < pair.Value.Count)
                {
                    Connection.ReceiveChat(Connection.Language.TradeFailedItemsChanged, MessageType.System);
                    TradePartner.Connection.ReceiveChat(string.Format(TradePartner.Connection.Language.TradeFailedPartnerItemsChanged, Name), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.TradeFailedItemsChanged, MessageType.System);

                    foreach (SConnection con in TradePartner.Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.TradeFailedPartnerItemsChanged, Name), MessageType.System);

                    TradeClose();
                    return;
                }

                UserItem item = fromArray[pair.Value.Slot];

                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                bool handled = false;

                foreach (ItemCheck check in checks)
                {
                    if (check.Info != item.Info) continue;
                    if ((check.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((check.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                    if ((check.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                    if ((check.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;


                    check.Count += pair.Value.Count;
                    handled = true;
                    break;
                }

                if (handled) continue;

                checks.Add(new ItemCheck(item, pair.Value.Count, item.Flags, item.ExpireTime));
            }

            if (!TradePartner.CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat(Connection.Language.TradeWaiting, MessageType.System);
                TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeNotEnoughSpace, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeWaiting, MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat(con.Language.TradeNotEnoughSpace, MessageType.System);

                TradePartner.TradeConfirmed = false;
                TradePartner.Enqueue(new S.TradeUnlock());
                return;
            }

            checks.Clear();

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
            {
                UserItem[] fromArray;
                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:
                        fromArray = TradePartner.Inventory;
                        break;
                    case GridType.Equipment:
                        fromArray = TradePartner.Equipment;
                        break;
                    case GridType.PartsStorage:
                        fromArray = TradePartner.PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = TradePartner.Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (TradePartner.Companion == null)
                        {
                            Connection.ReceiveChat(string.Format(Connection.Language.TradeFailedPartnerItemsChanged, TradePartner.Name), MessageType.System);
                            TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeFailedItemsChanged, MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.TradeFailedPartnerItemsChanged, TradePartner.Name), MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat(con.Language.TradeFailedItemsChanged, MessageType.System);
                            TradeClose();
                            return;
                        }


                        fromArray = TradePartner.Companion.Inventory;
                        break;
                    default:
                        //MAJOR LOGIC FAILURE 
                        return;
                }

                if (fromArray[pair.Value.Slot] != pair.Key || pair.Key.Count < pair.Value.Count)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.TradeFailedPartnerItemsChanged, TradePartner.Name), MessageType.System);
                    TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeFailedItemsChanged, MessageType.System);


                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.TradeFailedPartnerItemsChanged, TradePartner.Name), MessageType.System);

                    foreach (SConnection con in TradePartner.Connection.Observers)
                        con.ReceiveChat(con.Language.TradeFailedItemsChanged, MessageType.System);

                    TradeClose();
                    return;
                }

                UserItem item = fromArray[pair.Value.Slot];
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                bool handled = false;

                foreach (ItemCheck check in checks)
                {
                    if (check.Info != item.Info) continue;
                    if ((check.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((check.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                    if ((check.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                    if ((check.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;

                    check.Count += pair.Value.Count;
                    handled = true;
                    break;
                }

                if (handled) continue;

                checks.Add(new ItemCheck(item, pair.Value.Count, item.Flags, item.ExpireTime));
            }

            if (!CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat(Connection.Language.TradeNotEnoughSpace, MessageType.System);
                TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeWaiting, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.TradeNotEnoughSpace, MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat(con.Language.TradeWaiting, MessageType.System);

                TradeConfirmed = false;
                Enqueue(new S.TradeUnlock());
                return;
            }

            Enqueue(new S.ItemsChanged { Links = TradeItems.Values.ToList(), Success = true });

            //Deal Successful, Both can accept items without issues so send away
            UserItem tempItem;

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
            {

                if (pair.Key.Count > pair.Value.Count)
                {
                    pair.Key.Count -= pair.Value.Count;

                    tempItem = SEnvir.CreateFreshItem(pair.Key);
                    tempItem.Count = pair.Value.Count;
                    TradePartner.GainItem(tempItem);
                    continue;
                }


                UserItem[] fromArray;

                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.Equipment:
                        fromArray = Equipment;
                        break;
                    case GridType.PartsStorage:
                        fromArray = PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                fromArray[pair.Value.Slot] = null;
                RemoveItem(pair.Key);
                TradePartner.GainItem(pair.Key);
            }
            TradePartner.Enqueue(new S.ItemsChanged { Links = TradePartner.TradeItems.Values.ToList(), Success = true });

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
            {
                if (pair.Key.Count > pair.Value.Count)
                {
                    pair.Key.Count -= pair.Value.Count;

                    tempItem = SEnvir.CreateFreshItem(pair.Key);
                    tempItem.Count = pair.Value.Count;
                    GainItem(tempItem);
                    continue;
                }


                UserItem[] fromArray;

                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:
                        fromArray = TradePartner.Inventory;
                        break;
                    case GridType.Equipment:
                        fromArray = TradePartner.Equipment;
                        break;
                    case GridType.PartsStorage:
                        fromArray = TradePartner.PartsStorage;
                        break;
                    case GridType.Storage:
                        fromArray = TradePartner.Storage;
                        break;
                    case GridType.CompanionInventory:
                        fromArray = TradePartner.Companion.Inventory;
                        break;
                    default:
                        return;
                }

                fromArray[pair.Value.Slot] = null;
                TradePartner.RemoveItem(pair.Key);
                GainItem(pair.Key);
            }

            RefreshStats();
            SendShapeUpdate();
            TradePartner.RefreshStats();
            TradePartner.SendShapeUpdate();

            Gold += TradePartner.TradeGold - TradeGold;
            GoldChanged();

            TradePartner.Gold += TradeGold - TradePartner.TradeGold;
            TradePartner.GoldChanged();


            Connection.ReceiveChat(Connection.Language.TradeComplete, MessageType.System);
            TradePartner.Connection.ReceiveChat(TradePartner.Connection.Language.TradeComplete, MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(con.Language.TradeComplete, MessageType.System);

            foreach (SConnection con in TradePartner.Connection.Observers)
                con.ReceiveChat(con.Language.TradeComplete, MessageType.System);

            TradeClose();
        }

        #endregion

        #region NPCs
        public void NPCCall(uint objectID)
        {
            if (Dead) return;

            NPC = null;
            NPCPage = null;

            foreach (NPCObject ob in CurrentMap.NPCs)
            {
                if (ob.ObjectID != objectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Config.MaxViewRange)) return;

                ob.NPCCall(this, ob.NPCInfo.EntryPage);
                return;
            }
        }
        public void NPCButton(int ButtonID)
        {
            if (Dead || NPC == null || NPCPage == null) return;


            foreach (NPCButton button in NPCPage.Buttons)
            {
                if (button.ButtonID != ButtonID || button.DestinationPage == null) continue;

                NPC.NPCCall(this, button.DestinationPage);
                return;
            }


        }
        public void NPCBuy(C.NPCBuy p)
        {
            if (Dead || NPC == null || NPCPage == null || p.Amount <= 0) return;

            foreach (NPCGood good in NPCPage.Goods)
            {
                if (good.Index != p.Index || good.Item == null) continue;

                if (p.Amount > good.Item.StackSize) return;

                long cost = (long)(good.Rate * good.Item.Price * p.Amount);

                if (p.GuildFunds)
                {
                    if (Character.Account.GuildMember == null)
                    {
                        Connection.ReceiveChat(Connection.Language.NPCFundsGuild, MessageType.System);
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMerchant) != GuildPermission.FundsMerchant)
                    {
                        Connection.ReceiveChat(Connection.Language.NPCFundsPermission, MessageType.System);
                        return;
                    }

                    if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.NPCFundsCost, Character.Account.GuildMember.Guild.GuildFunds - cost), MessageType.System);
                        return;
                    }
                }
                else
                {
                    if (cost > Gold)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.NPCCost, Gold - cost), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.NPCCost, Gold - cost), MessageType.System);
                        return;
                    }
                }
                UserItemFlags flags = UserItemFlags.Locked;

                switch (good.Item.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Book:
                        flags |= UserItemFlags.NonRefinable;
                        break;
                }

                ItemCheck check = new ItemCheck(good.Item, p.Amount, flags, TimeSpan.Zero);

                if (!CanGainItems(true, check))
                {
                    Connection.ReceiveChat(Connection.Language.NPCNoRoom, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.NPCNoRoom, MessageType.System);
                    return;
                }


                UserItem item = SEnvir.CreateFreshItem(check);

                if (p.GuildFunds)
                {
                    Character.Account.GuildMember.Guild.GuildFunds -= cost;
                    Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                        member.Account.Connection?.ReceiveChat(string.Format(member.Account.Connection.Language.NPCFundsBuy, Name, cost, item.Info.ItemName, item.Count), MessageType.System);
                    }
                }
                else
                {
                    Gold -= cost;
                    GoldChanged();
                }

                GainItem(item);

            }
        }
        public void NPCSell(List<CellLinkInfo> links)
        {
            S.ItemsChanged p = new S.ItemsChanged { Links = links };
            Enqueue(p);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.BuySell) return;

            if (!ParseLinks(p.Links, 0, 100)) return;

            long gold = 0;
            long count = 0;


            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || !item.Info.CanSell || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
                if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return;

                count += link.Count;
                gold += item.Price(link.Count);
            }


            if (gold < 0)
            {
                Connection.ReceiveChat(Connection.Language.NPCSellWorthless, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCSellWorthless, MessageType.System);
                return;
            }

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = fromArray[link.Slot];


                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            Connection.ReceiveChat(string.Format(Connection.Language.NPCSellResult, count, gold), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(con.Language.NPCSellResult, count, gold), MessageType.System);

            p.Success = true;
            Gold += gold;

            GoldChanged();
        }
        public void NPCFragment(List<CellLinkInfo> links)
        {
            S.ItemsChanged p = new S.ItemsChanged { Links = links };
            Enqueue(p);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.ItemFragment) return;

            if (!ParseLinks(p.Links, 0, 100)) return;


            long cost = 0;
            int fragmentCount = 0;
            int fragment2Count = 0;
            int itemCount = 0;


            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return; //No harm in checking
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
                if (!item.CanFragment()) return;

                cost += item.FragmentCost();
                itemCount++;

                if (item.Info.Rarity == Rarity.Common)
                    fragmentCount += item.FragmentCount();
                else
                    fragment2Count += item.FragmentCount();
            }


            if (cost > Gold)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.FragmentCost, Gold - cost), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.FragmentCost, Gold - cost), MessageType.System);
                return;
            }

            List<ItemCheck> checks = new List<ItemCheck>();

            if (fragmentCount > 0)
                checks.Add(new ItemCheck(SEnvir.FragmentInfo, fragmentCount, UserItemFlags.None, TimeSpan.Zero));

            if (fragment2Count > 0)
                checks.Add(new ItemCheck(SEnvir.Fragment2Info, fragment2Count, UserItemFlags.None, TimeSpan.Zero));


            if (!CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat(Connection.Language.FragmentSpace, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.FragmentSpace, MessageType.System);
                return;
            }

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = fromArray[link.Slot];


                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (ItemCheck check in checks)
                while (check.Count > 0)
                    GainItem(SEnvir.CreateFreshItem(check));

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            Connection.ReceiveChat(string.Format(Connection.Language.FragmentResult, itemCount, cost), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(con.Language.FragmentResult, itemCount, cost), MessageType.System);

            p.Success = true;
            Gold -= cost;

            GoldChanged();
        }
        public void NPCAccessoryLevelUp(C.NPCAccessoryLevelUp p)
        {
            Enqueue(new S.NPCAccessoryLevelUp { Target = p.Target, Links = p.Links });

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryRefineLevel) return;

            if (!ParseLinks(p.Links, 0, 100) || !ParseLinks(p.Target)) return;


            UserItem[] targetArray = null;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:
                    targetArray = Equipment;
                    break;
                case GridType.Storage:
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Target.Slot < 0 || p.Target.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Target.Slot];

            if (targetItem == null || p.Target.Count > targetItem.Count) return; //Already Leveled.
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return; //No harm in checking

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return;
            }

            if (targetItem.Level >= Globals.AccessoryExperienceList.Count) return;

            bool changed = false;

            S.ItemsChanged result = new S.ItemsChanged { Links = new List<CellLinkInfo>(), Success = true };
            Enqueue(result);

            foreach (CellLinkInfo link in p.Links)
            {
                if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) break;


                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) continue;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        continue;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) continue;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) continue;
                if (item.Info != targetItem.Info) continue;
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (targetItem.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) continue;

                long cost = Globals.AccessoryLevelCost * link.Count;


                if (Gold < cost)
                {
                    Connection.ReceiveChat(Connection.Language.AccessoryLevelCost, MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(con.Language.AccessoryLevelCost, MessageType.System);
                    continue;
                }

                result.Links.Add(link);


                if (targetItem.Info.Rarity != Rarity.Common || targetItem.Level == 1)
                    targetItem.Experience += link.Count * 5;
                else
                    targetItem.Experience += link.Count;

                if (item.Level > 1 && targetItem.Info.Rarity == Rarity.Common)
                    targetItem.Experience -= 4;


                while (item.Level > 1)
                {
                    targetItem.Experience += Globals.AccessoryExperienceList[item.Level - 1];
                    item.Level--;
                }

                targetItem.Experience += item.Experience;

                Gold -= cost;

                if (targetItem.Experience >= Globals.AccessoryExperienceList[targetItem.Level])
                {
                    targetItem.Experience -= Globals.AccessoryExperienceList[targetItem.Level];
                    targetItem.Level++;

                    targetItem.Flags |= UserItemFlags.Refinable;
                }

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;

                changed = true;
            }


            if (changed)
            {
                if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.AccessoryLeveled, targetItem.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.AccessoryLeveled, targetItem.Info.ItemName), MessageType.System);
                }

                Companion?.RefreshWeight();
                RefreshWeight();
                GoldChanged();

                Enqueue(new S.ItemExperience { Target = p.Target, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });
            }
        }
        public void NPCAccessoryUpgrade(C.NPCAccessoryUpgrade p)
        {
            Enqueue(new S.ItemChanged { Link = p.Target }); //Unlock Item

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryRefineUpgrade) return;

            if (!ParseLinks(p.Target)) return;


            UserItem[] targetArray = null;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:
                    targetArray = Equipment;
                    break;
                case GridType.Storage:
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Target.Slot < 0 || p.Target.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Target.Slot];

            if (targetItem == null || p.Target.Count > targetItem.Count) return; //Already Leveled.
            if ((targetItem.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return;
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return;
            }

            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = p.Target.GridType, Slot = p.Target.Slot, NewStats = new Stats() };
            Enqueue(result);

            switch (p.RefineType)
            {
                case RefineType.DC:
                    targetItem.AddStat(Stat.MaxDC, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxDC] = 1;
                    break;
                case RefineType.SpellPower:
                    if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                    {
                        targetItem.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxMC] = 1;

                        targetItem.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxSC] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                    {
                        targetItem.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxMC] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                    {
                        targetItem.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxSC] = 1;
                    }
                    break;
                case RefineType.Health:
                    targetItem.AddStat(Stat.Health, 10, StatSource.Refine);
                    result.NewStats[Stat.Health] = 10;
                    break;
                case RefineType.Mana:
                    targetItem.AddStat(Stat.Mana, 10, StatSource.Refine);
                    result.NewStats[Stat.Mana] = 10;
                    break;
                case RefineType.DCPercent:
                    targetItem.AddStat(Stat.DCPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.DCPercent] = 1;
                    break;
                case RefineType.SPPercent:
                    if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                    {
                        targetItem.AddStat(Stat.MCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.MCPercent] = 1;

                        targetItem.AddStat(Stat.SCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.SCPercent] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                    {
                        targetItem.AddStat(Stat.MCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.MCPercent] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                    {
                        targetItem.AddStat(Stat.SCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.SCPercent] = 1;
                    }
                    break;
                case RefineType.HealthPercent:
                    targetItem.AddStat(Stat.HealthPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.HealthPercent] = 1;
                    break;
                case RefineType.ManaPercent:
                    targetItem.AddStat(Stat.ManaPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.ManaPercent] = 1;
                    break;
                case RefineType.Fire:
                    targetItem.AddStat(Stat.FireAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.FireAttack] = 1;
                    break;
                case RefineType.Ice:
                    targetItem.AddStat(Stat.IceAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.IceAttack] = 1;
                    break;
                case RefineType.Lightning:
                    targetItem.AddStat(Stat.LightningAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.LightningAttack] = 1;
                    break;
                case RefineType.Wind:
                    targetItem.AddStat(Stat.WindAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.WindAttack] = 1;
                    break;
                case RefineType.Holy:
                    targetItem.AddStat(Stat.HolyAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.HolyAttack] = 1;
                    break;
                case RefineType.Dark:
                    targetItem.AddStat(Stat.DarkAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.DarkAttack] = 1;
                    break;
                case RefineType.Phantom:
                    targetItem.AddStat(Stat.PhantomAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.PhantomAttack] = 1;
                    break;
                case RefineType.AC:
                    targetItem.AddStat(Stat.MinAC, 1, StatSource.Refine);
                    result.NewStats[Stat.MinAC] = 1;
                    targetItem.AddStat(Stat.MaxAC, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxAC] = 1;
                    break;
                case RefineType.MR:
                    targetItem.AddStat(Stat.MinMR, 1, StatSource.Refine);
                    result.NewStats[Stat.MinMR] = 1;
                    targetItem.AddStat(Stat.MaxMR, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxMR] = 1;
                    break;
                case RefineType.Accuracy:
                    targetItem.AddStat(Stat.Accuracy, 1, StatSource.Refine);
                    result.NewStats[Stat.Accuracy] = 1;
                    break;
                case RefineType.Agility:
                    targetItem.AddStat(Stat.Agility, 1, StatSource.Refine);
                    result.NewStats[Stat.Agility] = 1;
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "Attempted to Exploit refine, Accessory Refine Type.";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            targetItem.Flags &= ~UserItemFlags.Refinable;
            targetItem.StatsChanged();

            RefreshStats();

            if (targetItem.Experience >= Globals.AccessoryExperienceList[targetItem.Level])
            {
                targetItem.Experience -= Globals.AccessoryExperienceList[targetItem.Level];
                targetItem.Level++;

                targetItem.Flags |= UserItemFlags.Refinable;
            }

            Enqueue(new S.ItemExperience { Target = p.Target, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });
        }
        public void NPCAccessoryReset(C.NPCAccessoryReset p)
        {
            Enqueue(new S.ItemChanged { Link = p.Cell }); //Unlock Item

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryReset) return;

            if (!ParseLinks(p.Cell)) return;


            UserItem[] targetArray = null;

            switch (p.Cell.GridType)
            {
                case GridType.Inventory:
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:
                    targetArray = Equipment;
                    break;
                case GridType.Storage:
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (Globals.AccessoryResetCost > Gold)
            {
                Connection.ReceiveChat(Connection.Language.NPCRefinementGold, MessageType.System);
                return;
            }

            if (p.Cell.Slot < 0 || p.Cell.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Cell.Slot];

            if (targetItem == null || p.Cell.Count > targetItem.Count) return; //Already Leveled.
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;


            switch (targetItem.Level)
            {
                case 1:
                    return;
                case 2:
                    if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return; //Not Refuned.
                    break;
                default:
                    break;
            }

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return;
            }

            S.ItemStatsRefreshed result = new S.ItemStatsRefreshed { GridType = p.Cell.GridType, Slot = p.Cell.Slot };
            Enqueue(result);

            for (int i = targetItem.AddedStats.Count - 1; i >= 0; i--)
            {
                if (targetItem.AddedStats[i].StatSource != StatSource.Refine) continue;

                targetItem.AddedStats[i].Delete();
            }

            targetItem.StatsChanged();

            result.NewStats = new Stats(targetItem.Stats);

            RefreshStats();

            Gold -= Globals.AccessoryResetCost;
            GoldChanged();

            while (targetItem.Level > 1)
            {
                targetItem.Experience += Globals.AccessoryExperienceList[targetItem.Level - 1];
                targetItem.Level--;
            }

            if (targetItem.Experience >= Globals.AccessoryExperienceList[targetItem.Level])
            {
                targetItem.Experience -= Globals.AccessoryExperienceList[targetItem.Level];
                targetItem.Level++;

                targetItem.Flags |= UserItemFlags.Refinable;
            }

            Enqueue(new S.ItemExperience { Target = p.Cell, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });
        }

        public void NPCRepair(C.NPCRepair p)
        {
            S.NPCRepair result = new S.NPCRepair { Links = p.Links, Special = p.Special, SpecialRepairDelay = Config.SpecialRepairDelay };
            Enqueue(result);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.Repair) return;

            if (!ParseLinks(result.Links, 0, 100)) return;

            long cost = 0;
            int count = 0;

            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Equipment:
                        array = Equipment;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.GuildStorage:
                        if (Character.Account.GuildMember == null) return;
                        if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return;

                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || !item.Info.CanRepair || item.Info.Durability == 0) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                switch (item.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Armour:
                    case ItemType.Helmet:
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                    case ItemType.Shoes:
                    case ItemType.Shield:
                        break;
                    default:
                        Connection.ReceiveChat(string.Format(Connection.Language.RepairFail, item.Info.ItemName), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.RepairFail, item.Info.ItemName), MessageType.System);
                        return;
                }

                if (item.CurrentDurability >= item.MaxDurability)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.RepairFailRepaired, item.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.RepairFailRepaired, item.Info.ItemName), MessageType.System);
                    return;
                }
                if (NPCPage.Types.FirstOrDefault(x => x.ItemType == item.Info.ItemType) == null)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.RepairFailLocation, item.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.RepairFailLocation, item.Info.ItemName), MessageType.System);
                    return;
                }
                if (p.Special && SEnvir.Now < item.SpecialRepairCoolDown)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.RepairFailCooldown, item.Info.ItemName, Functions.ToString(item.SpecialRepairCoolDown - SEnvir.Now, false)), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.RepairFailCooldown, item.Info.ItemName, Functions.ToString(item.SpecialRepairCoolDown - SEnvir.Now, false)), MessageType.System);
                    return;
                }


                count++;
                cost += array[link.Slot].RepairCost(p.Special);
            }


            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat(Connection.Language.NPCRepairGuild, MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsRepair) != GuildPermission.FundsRepair)
                {
                    Connection.ReceiveChat(Connection.Language.NPCRepairPermission, MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.NPCRepairGuildCost, Character.Account.GuildMember.Guild.GuildFunds - cost), MessageType.System);
                    return;
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat(string.Format(Connection.Language.NPCRepairCost, Gold - cost), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.NPCRepairCost, Gold - cost), MessageType.System);
                    return;
                }
            }

            bool refresh = false;
            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Equipment:
                        array = Equipment;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.GuildStorage:
                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                }

                UserItem item = array[link.Slot];

                if (item.CurrentDurability == 0 && link.GridType == GridType.Equipment)
                    refresh = true;

                if (p.Special)
                {
                    item.CurrentDurability = item.MaxDurability;

                    if (item.Info.ItemType != ItemType.Weapon)
                        item.SpecialRepairCoolDown = SEnvir.Now + Config.SpecialRepairDelay;
                }
                else
                {
                    item.MaxDurability = Math.Max(0, item.MaxDurability - (item.MaxDurability - item.CurrentDurability) / Globals.DuraLossRate);
                    item.CurrentDurability = item.MaxDurability;
                }
            }

            Connection.ReceiveChat(string.Format(p.Special ? Connection.Language.NPCRepairSpecialResult : Connection.Language.NPCRepairResult, count, cost), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(string.Format(p.Special ? con.Language.NPCRepairSpecialResult : con.Language.NPCRepairResult, count, cost), MessageType.System);

            result.Success = true;

            if (p.GuildFunds)
            {
                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat(string.Format(member.Account.Connection.Language.NPCRepairGuildResult, Name, cost, count), MessageType.System);
                }
            }
            else
            {
                Gold -= cost;
                GoldChanged();
            }

            if (refresh)
                RefreshStats();
        }
        public void NPCRefinementStone(C.NPCRefinementStone p)
        {
            S.ItemsChanged result = new S.ItemsChanged
            {
                Links = new List<CellLinkInfo>()
            };
            Enqueue(result);

            if (p.IronOres != null) result.Links.AddRange(p.IronOres);
            if (p.SilverOres != null) result.Links.AddRange(p.SilverOres);
            if (p.DiamondOres != null) result.Links.AddRange(p.DiamondOres);
            if (p.GoldOres != null) result.Links.AddRange(p.GoldOres);
            if (p.Crystal != null) result.Links.AddRange(p.Crystal);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.RefinementStone) return;

            if (SEnvir.RefinementStoneInfo == null)
            {
                return;
            }

            if (!ParseLinks(p.IronOres, 4, 4)) return;
            if (!ParseLinks(p.SilverOres, 4, 4)) return;
            if (!ParseLinks(p.DiamondOres, 4, 4)) return;
            if (!ParseLinks(p.GoldOres, 2, 2)) return;
            if (!ParseLinks(p.Crystal, 1, 1)) return;

            if (p.Gold < 0) return;

            if (p.Gold > Gold)
            {
                Connection.ReceiveChat(Connection.Language.NPCRefinementGold, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefinementGold, MessageType.System);
                return;
            }

            ItemCheck check = new ItemCheck(SEnvir.RefinementStoneInfo, 1, UserItemFlags.None, TimeSpan.Zero);
            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat(Connection.Language.NPCRefinementStoneFailedRoom, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefinementStoneFailedRoom, MessageType.System);
                return;
            }

            int ironPurity = 0;
            int silverPurity = 0;
            int diamondPurity = 0;
            int goldPurity = 0;

            foreach (CellLinkInfo link in p.IronOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.IronOre) return;

                ironPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.SilverOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.SilverOre) return;

                silverPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.DiamondOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Diamond) return;

                diamondPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.GoldOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.GoldOre) return;

                goldPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.Crystal)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Crystal) return;
            }

            long chance = p.Gold / 25000; // 250k / 10%, 2,500,000 for 100%

            chance += Math.Min(23, ironPurity / 4350); // Need 100 Purity
            chance += Math.Min(23, silverPurity / 3475); // Need 80 Purity
            chance += Math.Min(23, diamondPurity / 2600); //Need 60 Purity
            chance += Math.Min(31, goldPurity / 1600); //Need 50 Purity

            foreach (CellLinkInfo link in p.IronOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.SilverOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.DiamondOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.GoldOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.Crystal)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            Gold -= p.Gold;
            GoldChanged();
            result.Success = true;

            if (SEnvir.Random.Next(100) >= chance)
            {
                Connection.ReceiveChat(Connection.Language.NPCRefinementStoneFailed, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefinementStoneFailed, MessageType.System);
                return;
            }


            UserItem stone = SEnvir.CreateFreshItem(check);
            GainItem(stone);
        }
        public void NPCRefine(C.NPCRefine p)
        {
            S.NPCRefine result = new S.NPCRefine
            {
                RefineQuality = p.RefineQuality,
                RefineType = p.RefineType,
                Ores = p.Ores,
                Items = p.Items,
                Specials = p.Specials,
            };
            Enqueue(result);

            switch (p.RefineQuality)
            {
                case RefineQuality.Rush:
                case RefineQuality.Quick:
                case RefineQuality.Standard:
                case RefineQuality.Careful:
                case RefineQuality.Precise:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "Attempted to Exploit refine, Weapon Refine Quality.";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            switch (p.RefineType)
            {
                case RefineType.Durability:
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "Attempted to Exploit refine, Weapon Refine Type.";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }



            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.Refine) return;

            if (!ParseLinks(p.Ores, 0, 5)) return;
            if (!ParseLinks(p.Items, 0, 3)) return;
            if (!ParseLinks(p.Specials, 0, 1)) return;

            int RefineCost = 50000;

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null || (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (Gold < RefineCost)
            {
                Connection.ReceiveChat(Connection.Language.NPCRefinementGold, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefinementGold, MessageType.System);
                return;
            }

            int ore = 0;
            int items = 0;
            int quality = 0;
            int special = 0;
            //Check Ores

            foreach (CellLinkInfo link in p.Ores)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.BlackIronOre) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                ore += item.CurrentDurability;
            }

            foreach (CellLinkInfo link in p.Items)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                switch (item.Info.ItemType)
                {
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                        break;
                    default:
                        return;
                }

                items += item.Info.RequiredAmount;

                if (item.Info.Rarity != Rarity.Common)
                    quality++;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 1) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                link.Count = 1;

                special += item.Info.Stats[Stat.MaxRefineChance];
            }


            /*
             * BaseChance  90% - Weapon Level
             * Max Chance  -5% | 0% | +5% | +10% | +20% = (Rush | Quick | Standard | Careful | Precise)  
             * 5 Ore 1% per 2 Dura Max
             * Items 1% per 6 Item Levels, 5% for Quality
             * Base Chance = 60% -Weapon Level  * 5%
             */

            int maxChance = 90 - weapon.Level + special;
            int chance = 60 - weapon.Level * 5;

            switch (p.RefineQuality)
            {
                case RefineQuality.Rush:
                    maxChance -= 5;
                    break;
                case RefineQuality.Quick:
                    break;
                case RefineQuality.Standard:
                    maxChance += 5;
                    break;
                case RefineQuality.Careful:
                    maxChance += 10;
                    break;
                case RefineQuality.Precise:
                    maxChance += 20;
                    break;
                default:
                    return;
            }

            //Special + Max Chance

            chance += ore / 2000;
            chance += items / 6;
            chance += quality * 25;

            maxChance = Math.Min(100, maxChance);
            chance = Math.Min(maxChance, chance);

            foreach (CellLinkInfo link in p.Ores)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Items)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            RemoveItem(weapon);
            Equipment[(int)EquipmentSlot.Weapon] = null;

            Gold -= RefineCost;
            GoldChanged();

            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();

            info.Character = Character;
            info.Weapon = weapon;
            info.Chance = chance;
            info.MaxChance = maxChance;
            info.Quality = p.RefineQuality;
            info.Type = p.RefineType;
            info.RetrieveTime = SEnvir.Now + Globals.RefineTimes[p.RefineQuality];

            result.Success = true;
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() } });
        }
        public void NPCRefineRetrieve(int index)
        {
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.RefineRetrieve) return;

            RefineInfo info = Character.Refines.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (SEnvir.Now < info.RetrieveTime && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat(Connection.Language.NPCRefineNotReady, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefineNotReady, MessageType.System);
                return;
            }

            ItemCheck check = new ItemCheck(info.Weapon, info.Weapon.Count, info.Weapon.Flags, info.Weapon.ExpireTime);

            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat(Connection.Language.NPCRefineNoRoom, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefineNoRoom, MessageType.System);
                return;
            }

            UserItem weapon = info.Weapon;

            if (SEnvir.Random.Next(100) < info.Chance)
            {
                switch (info.Type)
                {
                    case RefineType.Durability:
                        weapon.MaxDurability += 2000;
                        break;
                    case RefineType.DC:
                        weapon.AddStat(Stat.MaxDC, 1, StatSource.Refine);
                        break;
                    case RefineType.SpellPower:
                        if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                        {
                            weapon.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                            weapon.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        }

                        if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                            weapon.AddStat(Stat.MaxMC, 1, StatSource.Refine);

                        if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                            weapon.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        break;
                    case RefineType.Fire:
                        weapon.AddStat(Stat.FireAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Ice:
                        weapon.AddStat(Stat.IceAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 2 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Lightning:
                        weapon.AddStat(Stat.LightningAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 3 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Wind:
                        weapon.AddStat(Stat.WindAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 4 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Holy:
                        weapon.AddStat(Stat.HolyAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 5 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Dark:
                        weapon.AddStat(Stat.DarkAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 6 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Phantom:
                        weapon.AddStat(Stat.PhantomAttack, 1, StatSource.Refine);
                        weapon.AddStat(Stat.WeaponElement, 7 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        break;
                    case RefineType.Reset:
                        weapon.Level = 1;
                        weapon.ResetCoolDown = SEnvir.Now.AddDays(14);

                        weapon.MergeRefineElements(out Stat element);

                        for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                        {
                            UserItemStat stat = weapon.AddedStats[i];
                            if (stat.StatSource != StatSource.Refine || stat.Stat == Stat.WeaponElement) continue;

                            int amount = stat.Amount / 5;

                            stat.Delete();
                            weapon.AddStat(stat.Stat, amount, StatSource.Enhancement);
                        }

                        for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                        {
                            UserItemStat stat = weapon.AddedStats[i];
                            if (stat.StatSource != StatSource.Enhancement) continue;

                            switch (stat.Stat)
                            {
                                case Stat.MaxDC:
                                case Stat.MaxMC:
                                case Stat.MaxSC:
                                    stat.Amount = Math.Min(stat.Amount, 200);
                                    break;
                                case Stat.FireAttack:
                                case Stat.LightningAttack:
                                case Stat.IceAttack:
                                case Stat.WindAttack:
                                case Stat.DarkAttack:
                                case Stat.HolyAttack:
                                case Stat.PhantomAttack:
                                    stat.Amount = Math.Min(stat.Amount, 200);
                                    break;
                                case Stat.EvasionChance:
                                case Stat.BlockChance:
                                    stat.Amount = Math.Min(stat.Amount, 10);
                                    break;
                            }

                        }

                        break;
                }
                weapon.StatsChanged();

                Connection.ReceiveChat(Connection.Language.NPCRefineSuccess, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefineSuccess, MessageType.System);
            }
            else
            {
                Connection.ReceiveChat(Connection.Language.NPCRefineFailed, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.NPCRefineFailed, MessageType.System);
            }

            weapon.Flags &= ~UserItemFlags.Refinable;

            weapon.Flags |= UserItemFlags.Locked;

            Enqueue(new S.NPCRefineRetrieve { Index = info.Index });
            info.Weapon = null;
            info.Character = null;
            info.Delete();


            GainItem(weapon);
        }
        public void NPCResetWeapon()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];
            RemoveItem(weapon);
            Equipment[(int)EquipmentSlot.Weapon] = null;
            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment },
                Success = true
            });

            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();

            info.Character = Character;
            info.Weapon = weapon;
            info.Chance = 100;
            info.MaxChance = 100;
            info.Quality = RefineQuality.Precise;
            info.Type = RefineType.Reset;
            info.RetrieveTime = SEnvir.Now + Globals.RefineTimes[RefineQuality.Precise];

            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() } });
        }

        public void NPCRebirth()
        {
            AdjustRebirth(Character.Rebirth + 1);
        }

        public void AdjustRebirth(int rebirth)
        {
            Level = 1;
            Experience = Experience / 200;

            Enqueue(new S.LevelChanged { Level = Level, Experience = Experience, MaxExperience = MaxExperience });
            Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });

            Character.Rebirth = rebirth;
            Character.SpentPoints = 0;
            Character.HermitStats.Clear();

            RefreshStats();
        }

        public void NPCMasterRefine(C.NPCMasterRefine p)
        {
            S.NPCMasterRefine result = new S.NPCMasterRefine
            {
                Fragment1s = p.Fragment1s,
                Fragment2s = p.Fragment2s,
                Fragment3s = p.Fragment3s,
                Stones = p.Stones,
                Specials = p.Specials,
            };
            Enqueue(result);


            switch (p.RefineType)
            {
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "Attempted to Exploit Master refine, Weapon Refine Type.";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.MasterRefine) return;

            if (!ParseLinks(p.Fragment1s, 1, 1)) return;
            if (!ParseLinks(p.Fragment2s, 1, 1)) return;
            if (!ParseLinks(p.Fragment3s, 1, 1)) return;
            if (!ParseLinks(p.Stones, 1, 1)) return;
            if (!ParseLinks(p.Specials, 0, 1)) return;

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (weapon.Level != 17) return;

            long fragmentCount = 0;
            int special = 0;
            int fragmentRate = 2;
            //Check Ores

            foreach (CellLinkInfo link in p.Fragment1s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment1) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment2s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment2) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment3s)
            {
                if (link.Count < 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment3) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                fragmentCount += link.Count;
            }

            foreach (CellLinkInfo link in p.Stones)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.RefinementStone) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Specials)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 5) return;
                if (item.Count < link.Count) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;


                special += item.Info.Stats[Stat.MaxRefineChance];
                fragmentRate += item.Info.Stats[Stat.FragmentRate];
            }


            int maxChance = 80 + special;
            int statValue = 0;
            bool sucess = false;

            switch (p.RefineType)
            {
                case RefineType.DC:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.Stat != Stat.MaxDC || stat.StatSource != StatSource.Refine) continue;

                        statValue = stat.Amount;
                        break;
                    }

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, 80 - statValue * 4 + fragmentCount * fragmentRate);

                    if (sucess)
                        weapon.AddStat(Stat.MaxDC, 5, StatSource.Refine);
                    else
                        weapon.AddStat(Stat.MaxDC, -Math.Min(statValue, 5), StatSource.Refine);

                    break;
                case RefineType.SpellPower:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.StatSource != StatSource.Refine) continue;

                        if (stat.Stat != Stat.MaxMC && stat.Stat != Stat.MaxSC) continue;

                        statValue = Math.Max(statValue, stat.Amount);
                    }

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, 80 - statValue * 4 + fragmentCount * fragmentRate);

                    if (sucess)
                    {
                        if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                        {
                            weapon.AddStat(Stat.MaxMC, 5, StatSource.Refine);
                            weapon.AddStat(Stat.MaxSC, 5, StatSource.Refine);
                        }

                        if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                            weapon.AddStat(Stat.MaxMC, 5, StatSource.Refine);

                        if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                            weapon.AddStat(Stat.MaxSC, 5, StatSource.Refine);
                    }
                    else
                    {
                        if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                        {
                            weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, 5), StatSource.Refine);
                            weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, 5), StatSource.Refine);
                        }

                        if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                            weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, 5), StatSource.Refine);

                        if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                            weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, 5), StatSource.Refine);
                    }
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    statValue = weapon.MergeRefineElements(out Stat element);

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, 80 - statValue * 4 + fragmentCount * fragmentRate);

                    if (element == Stat.None)
                        element = Stat.FireAttack; //Could be any

                    if (sucess)
                    {

                        weapon.AddStat(element, 5, StatSource.Refine);
                        switch (p.RefineType)
                        {
                            case RefineType.Fire:
                                weapon.AddStat(Stat.WeaponElement, 1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Ice:
                                weapon.AddStat(Stat.WeaponElement, 2 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Lightning:
                                weapon.AddStat(Stat.WeaponElement, 3 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Wind:
                                weapon.AddStat(Stat.WeaponElement, 4 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Holy:
                                weapon.AddStat(Stat.WeaponElement, 5 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Dark:
                                weapon.AddStat(Stat.WeaponElement, 6 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Phantom:
                                weapon.AddStat(Stat.WeaponElement, 7 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                        }
                    }
                    else
                        weapon.AddStat(element, -Math.Min(statValue, 5), StatSource.Refine);
                    break;
            }


            foreach (CellLinkInfo link in p.Fragment1s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Fragment2s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Fragment3s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Stones)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            result.Success = true;

            Connection.ReceiveChat(sucess ? Connection.Language.NPCRefineSuccess : Connection.Language.NPCRefineFailed, MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(sucess ? con.Language.NPCRefineSuccess : con.Language.NPCRefineFailed, MessageType.System);

            weapon.StatsChanged();
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats) });
        }

        public void NPCSpecialRefine(Stat stat, int amount)
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if (weapon.Level != 17) return;

            weapon.AddStat(stat, amount, StatSource.Refine);


            Connection.ReceiveChat(Connection.Language.NPCRefineSuccess, MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat(con.Language.NPCRefineSuccess, MessageType.System);

            weapon.StatsChanged();
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats) });
        }

        public void NPCMasterRefineEvaluate(C.NPCMasterRefineEvaluate p)
        {
            switch (p.RefineType)
            {
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    return;
            }

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.MasterRefine) return;

            if (!ParseLinks(p.Fragment1s, 1, 1)) return;
            if (!ParseLinks(p.Fragment2s, 1, 1)) return;
            if (!ParseLinks(p.Fragment3s, 1, 1)) return;
            if (!ParseLinks(p.Stones, 1, 1)) return;
            if (!ParseLinks(p.Specials, 0, 1)) return;

            if (Gold < Globals.MasterRefineEvaluateCost)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.NPCMasterRefineGold, Globals.MasterRefineEvaluateCost), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.NPCMasterRefineGold, Globals.MasterRefineEvaluateCost), MessageType.System);
                return;
            }

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (weapon.Level != 17) return;

            long fragmentCount = 0;
            int special = 0;
            int fragmentRate = 2;
            //Check Ores

            foreach (CellLinkInfo link in p.Fragment1s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment1) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment2s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment2) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment3s)
            {
                if (link.Count < 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment3) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                fragmentCount += link.Count;
            }
            foreach (CellLinkInfo link in p.Stones)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.RefinementStone) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Specials)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 5) return;
                if (item.Count < link.Count) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;


                special += item.Info.Stats[Stat.MaxRefineChance];
                fragmentRate += item.Info.Stats[Stat.FragmentRate];
            }


            int maxChance = 80 + special;
            int statValue = 0;
            bool sucess = false;

            switch (p.RefineType)
            {
                case RefineType.DC:

                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.Stat != Stat.MaxDC || stat.StatSource != StatSource.Refine) continue;

                        statValue = stat.Amount;
                        break;
                    }

                    Connection.ReceiveChat(string.Format(Connection.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
                case RefineType.SpellPower:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.StatSource != StatSource.Refine) continue;

                        if (stat.Stat != Stat.MaxMC && stat.Stat != Stat.MaxSC) continue;

                        statValue = Math.Max(statValue, stat.Amount);
                    }
                    Connection.ReceiveChat(string.Format(Connection.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    statValue = weapon.MergeRefineElements(out Stat element);
                    weapon.StatsChanged();

                    sucess = SEnvir.Random.Next(100) >= Math.Min(maxChance, 80 - statValue * 4 + fragmentCount * 2);
                    Connection.ReceiveChat(string.Format(Connection.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat(string.Format(con.Language.NPCMasterRefineChance, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
            }

            Gold -= Globals.MasterRefineEvaluateCost;
            GoldChanged();
        }
        public void NPCWeaponCraft(C.NPCWeaponCraft p)
        {
            S.NPCWeaponCraft result = new S.NPCWeaponCraft
            {
                Template = p.Template,
                Yellow = p.Yellow,
                Blue = p.Blue,
                Red = p.Red,
                Purple = p.Purple,
                Green = p.Green,
                Grey = p.Grey,
            };
            Enqueue(result);


            int statCount = 0;

            bool isTemplate = false;

            #region Tempate Check

            if (p.Template == null) return;

            if (p.Template.GridType != GridType.Inventory) return;

            if (p.Template.Slot < 0 || p.Template.Slot >= Inventory.Length) return;

            if (p.Template.Count != 1) return;

            if (Inventory[p.Template.Slot] == null) return;

            if (Inventory[p.Template.Slot].Info.Effect == ItemEffect.WeaponTemplate)
            {
                isTemplate = true;
            }
            else if (Inventory[p.Template.Slot].Info.ItemType != ItemType.Weapon || Inventory[p.Template.Slot].Info.Effect == ItemEffect.SpiritBlade) return;

            #endregion

            long cost = Globals.CraftWeaponPercentCost;

            if (!isTemplate)
            {
                switch (Inventory[p.Template.Slot].Info.Rarity)
                {
                    case Rarity.Common:
                        cost = Globals.CommonCraftWeaponPercentCost;
                        break;
                    case Rarity.Superior:
                        cost = Globals.SuperiorCraftWeaponPercentCost;
                        break;
                    case Rarity.Elite:
                        cost = Globals.EliteCraftWeaponPercentCost;
                        break;
                }
            }


            #region Yellow Check

            if (p.Yellow != null)
            {
                if (p.Yellow.GridType != GridType.Inventory) return;

                if (p.Yellow.Slot < 0 || p.Yellow.Slot >= Inventory.Length) return;

                if (p.Yellow.Count != 1) return;

                if (Inventory[p.Yellow.Slot] == null || Inventory[p.Yellow.Slot].Info.Effect != ItemEffect.YellowSlot) return;

                statCount += Inventory[p.Yellow.Slot].Info.Shape;
            }

            #endregion

            #region Blue Check

            if (p.Blue != null)
            {
                if (p.Blue.GridType != GridType.Inventory) return;

                if (p.Blue.Slot < 0 || p.Blue.Slot >= Inventory.Length) return;

                if (p.Blue.Count != 1) return;

                if (Inventory[p.Blue.Slot] == null || Inventory[p.Blue.Slot].Info.Effect != ItemEffect.BlueSlot) return;

                statCount += Inventory[p.Blue.Slot].Info.Shape;
            }

            #endregion

            #region Red Check

            if (p.Red != null)
            {
                if (p.Red.GridType != GridType.Inventory) return;

                if (p.Red.Slot < 0 || p.Red.Slot >= Inventory.Length) return;

                if (p.Red.Count != 1) return;

                if (Inventory[p.Red.Slot] == null || Inventory[p.Red.Slot].Info.Effect != ItemEffect.RedSlot) return;

                statCount += Inventory[p.Red.Slot].Info.Shape;
            }

            #endregion

            #region Purple Check

            if (p.Purple != null)
            {
                if (p.Purple.GridType != GridType.Inventory) return;

                if (p.Purple.Slot < 0 || p.Purple.Slot >= Inventory.Length) return;

                if (p.Purple.Count != 1) return;

                if (Inventory[p.Purple.Slot] == null || Inventory[p.Purple.Slot].Info.Effect != ItemEffect.PurpleSlot) return;

                statCount += Inventory[p.Purple.Slot].Info.Shape;
            }

            #endregion

            #region Green Check

            if (p.Green != null)
            {
                if (p.Green.GridType != GridType.Inventory) return;

                if (p.Green.Slot < 0 || p.Green.Slot >= Inventory.Length) return;

                if (p.Green.Count != 1) return;

                if (Inventory[p.Green.Slot] == null || Inventory[p.Green.Slot].Info.Effect != ItemEffect.GreenSlot) return;

                statCount += Inventory[p.Green.Slot].Info.Shape;
            }

            #endregion

            #region Grey Check

            if (p.Grey != null)
            {
                if (p.Grey.GridType != GridType.Inventory) return;

                if (p.Grey.Slot < 0 || p.Grey.Slot >= Inventory.Length) return;

                if (p.Grey.Count != 1) return;

                if (Inventory[p.Grey.Slot] == null || Inventory[p.Grey.Slot].Info.Effect != ItemEffect.GreySlot) return;

                statCount += Inventory[p.Grey.Slot].Info.Shape;
            }

            #endregion

            ItemInfo weap = null;

            if (isTemplate)
            {

                switch (p.Class)
                {
                    case RequiredClass.Warrior:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.WarriorWeapon);
                        break;
                    case RequiredClass.Wizard:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.WizardWeapon);
                        break;
                    case RequiredClass.Taoist:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.TaoistWeapon);
                        break;
                    case RequiredClass.Assassin:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.AssassinWeapon);
                        break;
                    default:
                        return;
                }

                if (!CanGainItems(false, new ItemCheck(weap, 1, UserItemFlags.None, TimeSpan.Zero)))
                {
                    Connection.ReceiveChat("Not enough bag space available.", MessageType.System);
                    return;
                }
            }

            result.Success = true;

            UserItem item;

            #region Tempate

            if (isTemplate)
            {
                item = Inventory[p.Template.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Template.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }
            #endregion

            #region Yellow

            if (p.Yellow != null)
            {
                item = Inventory[p.Yellow.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Yellow.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Blue

            if (p.Blue != null)
            {
                item = Inventory[p.Blue.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Blue.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Red

            if (p.Red != null)
            {
                item = Inventory[p.Red.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Red.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Purple

            if (p.Purple != null)
            {
                item = Inventory[p.Purple.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Purple.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Green

            if (p.Green != null)
            {
                item = Inventory[p.Green.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Green.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Grey

            if (p.Grey != null)
            {
                item = Inventory[p.Grey.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Grey.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            Gold -= cost;
            GoldChanged();

            int total = 0;

            foreach (WeaponCraftStatInfo stat in SEnvir.WeaponCraftStatInfoList.Binding)
            {
                if ((stat.RequiredClass & p.Class) != p.Class) continue;

                total += stat.Weight;
            }

            if (isTemplate)
            {
                item = SEnvir.CreateFreshItem(weap);
            }
            else
            {
                item = Inventory[p.Template.Slot];

                RemoveItem(item);
                Inventory[p.Template.Slot] = null;

                item.Level = 1;
                item.Flags &= ~UserItemFlags.Refinable;

                for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                {
                    UserItemStat stat = item.AddedStats[i];
                    if (stat.StatSource == StatSource.Enhancement) continue;

                    stat.Delete();
                }

                item.StatsChanged();
            }

            for (int i = 0; i < statCount; i++)
            {
                int value = SEnvir.Random.Next(total);

                foreach (WeaponCraftStatInfo stat in SEnvir.WeaponCraftStatInfoList.Binding)
                {
                    if ((stat.RequiredClass & p.Class) != p.Class) continue;

                    value -= stat.Weight;

                    if (value >= 0) continue;

                    item.AddStat(stat.Stat, SEnvir.Random.Next(stat.MinValue, stat.MaxValue + 1), StatSource.Added);
                    break;
                }
            }

            item.StatsChanged();

            GainItem(item);
        }
        #endregion

        #region Packet Actions
        public void Turn(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Turn, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (direction != Direction)
                TradeClose();

            Direction = direction;


            ActionTime = SEnvir.Now + Globals.TurnTime;

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow });
        }
        public void Harvest(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Harvest, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove || Horse != HorseType.None)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            Direction = direction;
            ActionTime = SEnvir.Now + Globals.HarvestTime;

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectHarvest { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow });


            Point front = Functions.Move(CurrentLocation, Direction, 1);
            int range = Stats[Stat.PickUpRadius];
            bool send = false;

            for (int d = 0; d <= range; d++)
            {
                for (int y = front.Y - d; y <= front.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = front.X - d; x <= front.X + d; x += Math.Abs(y - front.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y]; //Direct Access we've checked the boudaries.

                        if (cell?.Objects == null) continue;

                        foreach (MapObject cellObject in cell.Objects)
                        {
                            if (cellObject.Race != ObjectType.Monster) continue;

                            MonsterObject ob = (MonsterObject)cellObject;

                            if (ob.Drops == null) continue;

                            List<UserItem> items;

                            if (!ob.Drops.TryGetValue(Character.Account, out items))
                            {
                                send = true;
                                continue;
                            }

                            if (ob.HarvestCount > 0)
                            {
                                ob.HarvestCount--;
                                continue;
                            }

                            if (items != null)
                            {
                                for (int i = items.Count - 1; i >= 0; i--)
                                {
                                    UserItem item = items[i];
                                    if (item.UserTask == null) continue;

                                    if (item.UserTask.Quest.Character == Character && !item.UserTask.Completed) continue;

                                    items.Remove(item);
                                    item.Delete();
                                }

                                if (items.Count == 0) items = null;
                            }



                            if (items == null)
                            {
                                ob.Drops.Remove(Character.Account);

                                if (ob.Drops.Count == 0) ob.Drops = null;
                                ob.HarvestChanged();
                                Connection.ReceiveChat(Connection.Language.HarvestNothing, MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat(con.Language.HarvestNothing, MessageType.System);
                                continue;
                            }

                            for (int i = items.Count - 1; i >= 0; i--)
                            {
                                UserItem item = items[i];

                                ItemCheck check = new ItemCheck(item, item.Count, item.Flags, item.ExpireTime);

                                if (!CanGainItems(false, check)) continue;

                                GainItem(item);
                                items.Remove(item);
                            }

                            if (items.Count == 0)
                            {
                                ob.Drops.Remove(Character.Account);

                                if (ob.Drops.Count == 0) ob.Drops = null;
                                ob.HarvestChanged();

                                continue;
                            }

                            Connection.ReceiveChat(Connection.Language.HarvestCarry, MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat(con.Language.HarvestCarry, MessageType.System);
                            continue;
                        }
                    }
                }
            }

            if (send)
            {
                Connection.ReceiveChat(Connection.Language.HarvestOwner, MessageType.System);


                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.HarvestOwner, MessageType.System);
            }
        }
        public void Mount()
        {
            if (SEnvir.Now < ActionTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Mount));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (Dead)
            {
                Connection.ReceiveChat(Connection.Language.HorseDead, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.HorseDead, MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            if (Character.Account.Horse == HorseType.None)
            {
                Connection.ReceiveChat(Connection.Language.HorseOwner, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.HorseOwner, MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            if (!CurrentMap.Info.CanHorse)
            {
                Connection.ReceiveChat(Connection.Language.HorseMap, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.HorseMap, MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            ActionTime = SEnvir.Now + Globals.TurnTime;

            if (Horse == HorseType.None)
                Horse = Character.Account.Horse;
            else
                Horse = HorseType.None;

            Broadcast(new S.ObjectMount { ObjectID = ObjectID, Horse = Horse });
        }
        public void Move(MirDirection direction, int distance)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Move, direction, distance));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (distance <= 0 || distance > 3)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (distance == 3 && Horse == HorseType.None)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }


            Cell cell = null;

            for (int i = 1; i <= distance; i++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction, i));
                if (cell == null)
                {
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
                }

                if (cell.IsBlocking(this, true))
                {
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
                }
            }

            BuffRemove(BuffType.Invisibility);
            BuffRemove(BuffType.Transparency);

            if (distance > 1)
            {
                if (Stats[Stat.Comfort] < 12)
                    RegenTime = SEnvir.Now + RegenDelay;
                BuffRemove(BuffType.Cloak);
            }


            Direction = direction;

            ActionTime = SEnvir.Now + Globals.MoveTime;
            MoveTime = SEnvir.Now + Globals.MoveTime;

            PreventSpellCheck = true;
            CurrentCell = cell.GetMovement(this);
            PreventSpellCheck = false;

            RemoveAllObjects();
            AddAllObjects();

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectMove { ObjectID = ObjectID, Direction = direction, Location = CurrentLocation, Slow = slow, Distance = distance });
            CheckSpellObjects();
        }
        public void Attack(MirDirection direction, MagicType attackMagic)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < AttackTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Attack, direction, attackMagic));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanAttack)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            CombatTime = SEnvir.Now;

            if (Stats[Stat.Comfort] < 15)
                RegenTime = SEnvir.Now + RegenDelay;
            Direction = direction;
            ActionTime = SEnvir.Now + Globals.AttackTime;

            int aspeed = Stats[Stat.AttackSpeed];
            int attackDelay = Globals.AttackDelay - aspeed * Globals.ASpeedRate;
            attackDelay = Math.Max(800, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            if (BagWeight > Stats[Stat.BagWeight])
                AttackTime += TimeSpan.FromMilliseconds(attackDelay);


            MagicType validMagic = MagicType.None;
            List<UserMagic> magics = new List<UserMagic>();

            UserMagic magic;

            #region Warrior

            if (Magics.TryGetValue(MagicType.Swordsmanship, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (Magics.TryGetValue(MagicType.Slaying, out magic) && Level >= magic.Info.NeedLevel1)
            {
                if (CanPowerAttack && attackMagic == MagicType.Slaying)
                {
                    magics.Add(magic);
                    validMagic = MagicType.Slaying;
                    Enqueue(new S.MagicToggle { Magic = MagicType.Slaying, CanUse = CanPowerAttack = false });
                }

                if (!CanPowerAttack && SEnvir.Random.Next(5) == 0)
                    Enqueue(new S.MagicToggle { Magic = MagicType.Slaying, CanUse = CanPowerAttack = true });
            }

            if (attackMagic == MagicType.Thrusting && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = MagicType.Thrusting;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }

            }

            if (attackMagic == MagicType.HalfMoon && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = MagicType.HalfMoon;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.DestructiveSurge && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    DestructiveSurgeLifeSteal = 0;
                    validMagic = MagicType.DestructiveSurge;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (CanFlamingSword && attackMagic == MagicType.FlamingSword && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                validMagic = MagicType.FlamingSword;
                magics.Add(magic);
                CanFlamingSword = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.FlamingSword, CanUse = false });
            }


            if (CanDragonRise && attackMagic == MagicType.DragonRise && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                validMagic = MagicType.DragonRise;
                magics.Add(magic);
                CanDragonRise = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.DragonRise, CanUse = false });
            }

            if (CanBladeStorm && attackMagic == MagicType.BladeStorm && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                validMagic = MagicType.BladeStorm;
                magics.Add(magic);
                CanBladeStorm = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.BladeStorm, CanUse = false });
            }

            #endregion

            #region Taoist

            if (Magics.TryGetValue(MagicType.SpiritSword, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            #endregion

            #region Assassin

            if (Magics.TryGetValue(MagicType.VineTreeDance, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (Magics.TryGetValue(MagicType.Discipline, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (Magics.TryGetValue(MagicType.BloodyFlower, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (Magics.TryGetValue(MagicType.AdvancedBloodyFlower, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);


            if (SEnvir.Random.Next(2) == 0 && Magics.TryGetValue(MagicType.CalamityOfFullMoon, out magic) && Level >= magic.Info.NeedLevel1) //LOTUS Phase
                magics.Add(magic);

            if (attackMagic == MagicType.FullBloom && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.WhiteLotus && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.RedLotus && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.SweetBrier && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.Karma && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown && Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                int cost = Stats[Stat.Health] * magic.Cost / 100;

                UserMagic augMagic;
                if (Magics.TryGetValue(MagicType.Release, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                {
                    cost -= cost * augMagic.GetPower() / 100;
                    magics.Add(augMagic);
                }

                if (cost < CurrentHP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeHP(-cost);
                }
            }

            if (validMagic == MagicType.None && SEnvir.Random.Next(2) == 0 && Magics.TryGetValue(MagicType.WaningMoon, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (attackMagic == MagicType.FlameSplash && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    FlameSplashLifeSteal = 0;
                    validMagic = MagicType.FlameSplash;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            #endregion


            Element element = Functions.GetElement(Stats);

            if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
            {
                foreach (KeyValuePair<Stat, int> stats in Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                {
                    switch (stats.Key)
                    {
                        case Stat.FireAffinity:
                            element = Element.Fire;
                            break;
                        case Stat.IceAffinity:
                            element = Element.Ice;
                            break;
                        case Stat.LightningAffinity:
                            element = Element.Lightning;
                            break;
                        case Stat.WindAffinity:
                            element = Element.Wind;
                            break;
                        case Stat.HolyAffinity:
                            element = Element.Holy;
                            break;
                        case Stat.DarkAffinity:
                            element = Element.Dark;
                            break;
                        case Stat.PhantomAffinity:
                            element = Element.Phantom;
                            break;
                    }
                }
            }

            if (AttackLocation(Functions.Move(CurrentLocation, Direction), magics, true))
            {
                switch (attackMagic)
                {
                    case MagicType.FullBloom:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });


                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.WhiteLotus:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.RedLotus:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.SweetBrier:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }

                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        break;
                    case MagicType.Karma:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        UseItemTime = SEnvir.Now.AddSeconds(10);

                        if (Magics.TryGetValue(MagicType.Karma, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }

                        if (Magics.TryGetValue(MagicType.SummonPuppet, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        break;
                    default:

                        break;
                }
            }

            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow, AttackMagic = validMagic, AttackElement = element });

            switch (validMagic)
            {
                case MagicType.Thrusting:
                    AttackLocation(Functions.Move(CurrentLocation, Direction, 2), magics, false);
                    break;
                case MagicType.HalfMoon:
                case MagicType.DragonRise:
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, -1)), magics, false);
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 1)), magics, false);
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 2)), magics, false);
                    break;
                case MagicType.DestructiveSurge:
                    for (int i = 1; i < 8; i++)
                        AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i)), magics, false);
                    break;
                case MagicType.FlameSplash:
                    int count = 0;
                    List<MirDirection> directions = new List<MirDirection>();

                    for (int i = 0; i < 8; i++)
                        directions.Add((MirDirection)i);

                    directions.Remove(Direction);

                    while (count < 4)
                    {
                        MirDirection dir = directions[SEnvir.Random.Next(directions.Count)];

                        if (AttackLocation(Functions.Move(CurrentLocation, dir), magics, false))
                            count++;

                        directions.Remove(dir);
                        if (directions.Count == 0) break;
                    }


                    break;
            }
        }
        public void Magic(C.Magic p)
        {
            UserMagic magic;

            if (!Magics.TryGetValue(p.Type, out magic) || Level < magic.Info.NeedLevel1)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (SEnvir.Now < ActionTime || SEnvir.Now < MagicTime || SEnvir.Now < magic.Cooldown)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Magic, p));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanCast)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            switch (p.Type)
            {
                case MagicType.ShoulderDash:
                case MagicType.Interchange:
                case MagicType.Defiance:
                case MagicType.Beckon:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Fetter:
                case MagicType.SwiftBlade:
                case MagicType.Endurance:
                case MagicType.Assault:
                case MagicType.SeismicSlam:


                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.AdamantineFireBall:
                case MagicType.ThunderBolt:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.FrozenEarth:
                case MagicType.BlowEarth:
                case MagicType.FireWall:
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.ExpelUndead:
                case MagicType.GeoManipulation:
                case MagicType.Transparency:
                case MagicType.MagicShield:
                case MagicType.FrostBite:
                case MagicType.IceStorm:
                case MagicType.DragonTornado:
                case MagicType.GreaterFrozenEarth:
                case MagicType.ChainLightning:
                case MagicType.MeteorShower:
                case MagicType.Renounce:
                case MagicType.Tempest:
                case MagicType.JudgementOfHeaven:
                case MagicType.ThunderStrike:
                case MagicType.MirrorImage:
                case MagicType.Teleportation:
                case MagicType.Asteroid:

                case MagicType.Heal:
                case MagicType.PoisonDust:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.MagicResistance:
                case MagicType.Resilience:
                //case MagicType.ShacklingTalisman:
                case MagicType.Invisibility:
                case MagicType.MassInvisibility:
                case MagicType.ThunderKick:
                case MagicType.StrengthOfFaith:
                case MagicType.CelestialLight:
                case MagicType.GreaterPoisonDust:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.Scarecrow:
                case MagicType.LifeSteal:
                case MagicType.ImprovedExplosiveTalisman:


                case MagicType.TrapOctagon:
                case MagicType.TaoistCombatKick:
                case MagicType.ElementalSuperiority:
                case MagicType.MassHeal:
                case MagicType.BloodLust:
                case MagicType.Resurrection:
                case MagicType.Purification:
                case MagicType.SummonSkeleton:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonShinsu:
                //case MagicType.Transparency:

                case MagicType.PoisonousCloud:
                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.TheNewBeginning:
                case MagicType.SummonPuppet:
                case MagicType.Abyss:
                case MagicType.FlashOfLight:
                case MagicType.DanceOfSwallow:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.MassBeckon:
                case MagicType.Infection:
                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.DarkConversion:
                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion)) break;

                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.DragonRepulse:
                    if (Stats[Stat.Health] * magic.Cost / 1000 >= CurrentHP || CurrentHP < Stats[Stat.Health] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    if (Stats[Stat.Mana] * magic.Cost / 1000 >= CurrentMP || CurrentMP < Stats[Stat.Mana] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.Cloak:
                    if (Buffs.Any(x => x.Type == BuffType.Cloak)) break;

                    if (SEnvir.Now < CombatTime.AddSeconds(10))
                    {
                        Connection.ReceiveChat(Connection.Language.CloakCombat, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.CloakCombat, MessageType.System);
                        break;
                    }

                    if (Stats[Stat.Health] * magic.Cost / 1000 >= CurrentHP || CurrentHP < Stats[Stat.Health] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                default:
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
            }

            //todo get cost
            //Combat time

            MapObject ob = VisibleObjects.FirstOrDefault(x => x.ObjectID == p.Target);

            if (ob != null && !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.MagicRange))
                ob = null;

            bool cast = true;



            List<uint> targets = new List<uint>();
            List<Point> locations = new List<Point>();

            List<Cell> cells;
            Stats stats;
            int power;
            UserMagic augMagic;
            HashSet<MapObject> realTargets;
            List<UserMagic> magics;
            int count;
            List<MapObject> possibleTargets;
            Point location;
            BuffInfo buff;
            switch (p.Type)
            {
                #region Warrior

                case MagicType.ShoulderDash:
                    if ((Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) break;

                    Direction = p.Direction;
                    count = ShoulderDashEnd(magic);

                    if (count == 0)
                    {
                        Connection.ReceiveChat(Connection.Language.DashFailed, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.DashFailed, MessageType.System);
                    }

                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    ChangeMP(-magic.Cost);
                    return;
                case MagicType.Interchange:
                case MagicType.Beckon:
                    if (ob == null) break;

                    if (!CanAttackTarget(ob))
                    {
                        ob = null;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(300),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Defiance:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Endurance:
                    ob = null;
                    p.Direction = MirDirection.Down;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Fetter:
                    ob = null;
                    p.Direction = MirDirection.Down;

                    cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.SwiftBlade:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);

                    cells = CurrentMap.GetCells(p.Location, 0, 3);
                    SwiftBladeLifeSteal = 0;

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(900),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.SeismicSlam:
                    ob = null;


                    cells = CurrentMap.GetCells(Functions.Move(CurrentLocation, p.Direction, 3), 0, 3);
                    SwiftBladeLifeSteal = 0;

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(600),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.MassBeckon:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;

                #endregion

                #region Wizard

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.AdamantineFireBall:
                case MagicType.IceBlades:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 48),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.ThunderBolt:
                case MagicType.Cyclone:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }
                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.ElectricShock:
                    if (!CanAttackTarget(ob) || ob.Race != ObjectType.Monster)
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }
                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.ExpelUndead:
                    if (!CanAttackTarget(ob) || ob.Race != ObjectType.Monster || !((MonsterObject)ob).MonsterInfo.Undead)
                    {
                        ob = null;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Repulsion:
                    ob = null;

                    for (MirDirection d = MirDirection.Up; d <= MirDirection.UpLeft; d++)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            CurrentMap.GetCell(Functions.Move(CurrentLocation, d)),
                            d));
                    }
                    break;
                case MagicType.Teleportation:
                    ob = null;
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.ScortchedEarth:
                case MagicType.FrozenEarth:
                    ob = null;

                    for (int i = 1; i <= 8; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;
                        locations.Add(cell.Location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(800),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));

                        switch (p.Direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2))),
                                    false));
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1))),
                                    false));
                                break;
                        }
                    }
                    break;
                case MagicType.LightningBeam:
                    ob = null;

                    locations.Add(Functions.Move(CurrentLocation, p.Direction));

                    for (int i = 1; i <= 8; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));


                        switch (p.Direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2))),
                                    false));
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(500),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1))),
                                    false));
                                break;
                        }
                    }
                    break;
                case MagicType.BlowEarth:
                    ob = null;

                    Point lastLocation = CurrentLocation;

                    for (int i = 1; i <= 8; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;

                        lastLocation = location;

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(800),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));


                        switch (p.Direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2))),
                                    false));
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1))),
                                    false));
                                break;
                        }
                    }

                    locations.Add(lastLocation);

                    if (lastLocation == CurrentLocation)
                        cast = false;
                    break;
                case MagicType.FireWall:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    foreach (ConquestWar war in SEnvir.ConquestWars)
                    {
                        if (war.Map != CurrentMap) continue;

                        for (int i = SpellList.Count - 1; i >= 0; i--)
                        {
                            if (SpellList[i].Effect != SpellEffect.FireWall) continue;

                            SpellList[i].Despawn();
                        }

                        break;
                    }
                    power = (magic.Level + 2) * 5;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Up)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Down)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(p.Location),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Left)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Right)),
                        power));
                    break;
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.IceStorm:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 1);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.Asteroid:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(1200),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }


                    if (Magics.TryGetValue(MagicType.FireWall, out augMagic) && augMagic.Info.NeedLevel1 > Level)
                        augMagic = null;


                    if (augMagic != null)
                    {
                        foreach (ConquestWar war in SEnvir.ConquestWars)
                        {
                            if (war.Map != CurrentMap) continue;

                            for (int i = SpellList.Count - 1; i >= 0; i--)
                            {
                                if (SpellList[i].Effect != SpellEffect.FireWall) continue;

                                SpellList[i].Despawn();
                            }

                            break;
                        }

                        power = (magic.Level + 2) * 5;

                        foreach (Cell cell in cells)
                        {

                            if (Math.Abs(cell.Location.X - p.Location.X) + Math.Abs(cell.Location.Y - p.Location.Y) >= 3) continue;


                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(2250),
                                ActionType.DelayMagic,
                                new List<UserMagic> { augMagic },
                                cell,
                                power));
                        }
                    }



                    break;
                case MagicType.DragonTornado:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 1);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(1200),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.MagicShield:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1100),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Renounce:
                case MagicType.JudgementOfHeaven:
                case MagicType.FrostBite:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.GeoManipulation:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location));
                    break;
                case MagicType.GreaterFrozenEarth:
                    ob = null;

                    for (int d = -1; d <= 1; d++)
                        for (int i = 1; i <= 8; i++)
                        {
                            MirDirection direction = Functions.ShiftDirection(p.Direction, d);

                            location = Functions.Move(CurrentLocation, direction, i);
                            Cell cell = CurrentMap.GetCell(location);

                            if (cell == null) continue;
                            locations.Add(cell.Location);

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(800),
                                ActionType.DelayMagic,
                                new List<UserMagic> { magic },
                                cell,
                                true));

                            switch (direction)
                            {
                                case MirDirection.Up:
                                case MirDirection.Right:
                                case MirDirection.Down:
                                case MirDirection.Left:
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -2))),
                                        false));
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 2))),
                                        false));
                                    break;
                                case MirDirection.UpRight:
                                case MirDirection.DownRight:
                                case MirDirection.DownLeft:
                                case MirDirection.UpLeft:
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 1))),
                                        false));
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -1))),
                                        false));
                                    break;
                            }
                        }
                    break;
                case MagicType.ChainLightning:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 4);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            Functions.Distance(cell.Location, p.Location))); //Central Point
                    }
                    break;
                case MagicType.MeteorShower:

                    ob = null;

                    magics = new List<UserMagic> { magic };

                    realTargets = new HashSet<MapObject>();

                    possibleTargets = GetTargets(CurrentMap, p.Location, 3);

                    while (realTargets.Count < 6 + magic.Level)
                    {
                        if (possibleTargets.Count == 0) break;

                        MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                        possibleTargets.Remove(target);

                        if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                        realTargets.Add(target);
                    }

                    foreach (MapObject target in realTargets)
                    {
                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }

                    break;
                case MagicType.Tempest:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    power = (magic.Level + 2) * 5;

                    foreach (ConquestWar war in SEnvir.ConquestWars)
                    {
                        if (war.Map != CurrentMap) continue;

                        for (int i = SpellList.Count - 1; i >= 0; i--)
                        {
                            if (SpellList[i].Effect != SpellEffect.Tempest) continue;

                            SpellList[i].Despawn();
                        }

                        break;
                    }

                    cells = CurrentMap.GetCells(p.Location, 0, 1);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            power));
                    }
                    break;
                case MagicType.ThunderStrike:

                    ob = null;

                    cells = CurrentMap.GetCells(CurrentLocation, 0, 6);
                    foreach (Cell cell in cells)
                    {
                        if (cell.Objects == null)
                        {
                            if (SEnvir.Random.Next(40) == 0)
                                locations.Add(cell.Location);

                            continue;
                        }

                        foreach (MapObject target in cell.Objects)
                        {
                            if (SEnvir.Random.Next(2) > 0) continue;
                            if (!CanAttackTarget(target)) continue;

                            targets.Add(target.ObjectID);

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(500),
                                ActionType.DelayMagic,
                                new List<UserMagic> { magic },
                                target));
                        }
                    }
                    break;

                #endregion

                #region Taoist

                case MagicType.Heal:
                    if (!CanHelpTarget(ob))
                        ob = this;

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.PoisonDust:

                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.GreaterPoisonDust, out augMagic);

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;
                        possibleTargets = GetTargets(CurrentMap, p.Location, 3);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            if (target is PlayerObject)
                            {
                                var player = (PlayerObject)target;
                                if ((InGroup(player) || InGuild(player)) && target.Dead)
                                {
                                    realTargets.Add(target);
                                }
                            }

                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        int shape;

                        if (!UsePoison(1, out shape))
                            break;


                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            shape == 0 ? PoisonType.Green : PoisonType.Red));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }
                    if (ob == null)
                        locations.Add(p.Location);

                    break;
                case MagicType.ExplosiveTalisman:
                case MagicType.ImprovedExplosiveTalisman:

                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.AugmentExplosiveTalisman, out augMagic);

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;
                        possibleTargets = GetTargets(CurrentMap, p.Location, 2);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        if (!UseAmulet(1, 0, out stats))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            target == ob,
                            stats));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    if (ob == null)
                        locations.Add(p.Location);

                    break;
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.AugmentEvilSlayer, out augMagic);

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);


                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetTargets(CurrentMap, p.Location, 2);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        if (Equipment[(int)EquipmentSlot.Amulet]?.Info.Stats[Stat.HolyAffinity] > 0)
                            UseAmulet(1, 0, out stats);
                        else
                            stats = null;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            target == ob,
                            stats));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    if (ob == null)
                        locations.Add(p.Location);

                    break;

                case MagicType.MagicResistance:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(1, 0, out stats))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            stats));
                    }
                    break;
                case MagicType.ElementalSuperiority:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(1, 0, out stats))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            stats));
                    }
                    break;
                case MagicType.Resilience:
                case MagicType.BloodLust:
                case MagicType.LifeSteal:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, 0))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.TrapOctagon:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, 0))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        p.Location));
                    break;
                case MagicType.SummonSkeleton:
                    ob = null;

                    if (!UseAmulet(1, 0))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction, -1),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Skeleton)));
                    break;
                case MagicType.SummonJinSkeleton:
                    ob = null;

                    if (!UseAmulet(2, 0))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction, -1),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinSkeleton)));
                    break;
                case MagicType.SummonShinsu:
                    ob = null;

                    if (!UseAmulet(5, 0))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction, -1),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Shinsu)));
                    break;
                case MagicType.SummonDemonicCreature:
                    ob = null;

                    if (!UseAmulet(25, 0))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction, -1),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.InfernalSoldier)));
                    break;
                case MagicType.Invisibility:
                    ob = null;
                    if (!UseAmulet(2, 0))
                    {
                        cast = false;
                        break;
                    }


                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.StrengthOfFaith:
                    ob = null;
                    if (!UseAmulet(5, 0))
                    {
                        cast = false;
                        break;
                    }
                    targets.Add(ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Transparency:
                    ob = null;
                    if (!UseAmulet(10, 0))
                    {
                        cast = false;
                        break;
                    }

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location));
                    break;
                case MagicType.CelestialLight:
                    if (Buffs.Any(x => x.Type == BuffType.CelestialLight)) break;
                    ob = null;
                    if (!UseAmulet(20, 0))
                    {
                        cast = false;
                        break;
                    }

                    targets.Add(ObjectID);


                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.MassInvisibility:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, 0))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.MassHeal:

                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.TaoistCombatKick:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(CurrentLocation, p.Direction)),
                        p.Direction));
                    break;
                case MagicType.Purification:

                    magics = new List<UserMagic> { magic };

                    Magics.TryGetValue(MagicType.AugmentPurification, out augMagic);

                    realTargets = new HashSet<MapObject>();

                    if (ob != null && (CanAttackTarget(ob) || CanHelpTarget(ob)))
                        realTargets.Add(ob);
                    else
                    {
                        realTargets.Add(this);
                        ob = null;
                    }


                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetAllObjects(p.Location, 3);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;


                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            if (!CanAttackTarget(target) && CanHelpTarget(target))
                                realTargets.Add(target);

                        }
                    }

                    count = -1;

                    foreach (MapObject target in realTargets)
                    {
                        if (!UseAmulet(2, 0))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    break;
                case MagicType.Resurrection:
                    magics = new List<UserMagic> { magic };

                    Magics.TryGetValue(MagicType.OathOfThePerished, out augMagic);

                    realTargets = new HashSet<MapObject>();

                    if ((InGroup(ob as PlayerObject) || InGuild(ob as PlayerObject)) && ob.Dead)
                        realTargets.Add(ob);
                    else
                        ob = null;

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetAllObjects(p.Location, 6);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;


                            PlayerObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)] as PlayerObject;

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            if ((InGroup(target as PlayerObject) || InGuild(target as PlayerObject)) && target.Dead)
                                realTargets.Add(target);

                        }
                    }

                    count = -1;

                    foreach (MapObject target in realTargets)
                    {
                        if (!UseAmulet(1, 1))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    break;
                case MagicType.DemonExplosion:
                    ob = null;
                    if (Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead) || !UseAmulet(20, 0, out stats))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        stats));
                    break;
                case MagicType.Infection:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 48),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;


                #endregion

                #region Assassin

                case MagicType.PoisonousCloud:

                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Cloak:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.Cloak)) break;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.WraithGrip:

                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }

                    if (ob.Race == ObjectType.Player ? ob.Level >= Level : ob.Level > Level + 15)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.WraithLevel, ob.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.WraithLevel, ob.Name), MessageType.System);

                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Abyss:

                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }


                    if ((ob.Race == ObjectType.Player && ob.Level >= Level) || (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.IsBoss))
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.AbyssLevel, ob.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.AbyssLevel, ob.Name), MessageType.System);

                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Rake:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction))));

                    break;
                case MagicType.HellFire:

                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1200),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;

                case MagicType.TheNewBeginning:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.SummonPuppet:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.DanceOfSwallow:

                    DanceOfSwallowEnd(magic, ob);

                    ChangeMP(-magic.Cost);
                    return;

                case MagicType.DarkConversion:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion)) break;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));

                    break;
                case MagicType.DragonRepulse:
                    ob = null;

                    buff = BuffAdd(BuffType.DragonRepulse, TimeSpan.FromSeconds(6), null, true, false, TimeSpan.FromSeconds(1));
                    buff.TickTime = TimeSpan.FromMilliseconds(500);
                    break;
                case MagicType.FlashOfLight:
                    ob = null;

                    magics = new List<UserMagic> { magic };
                    /*   buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);

                        if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                        {
                            BuffRemove(buff);
                            magics.Add(augMagic);
                            if (buff.Stats[Stat.TheNewBeginning] > 1)
                                BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                        }*/

                    for (int i = 1; i <= 2; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;
                        locations.Add(cell.Location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(400),
                            ActionType.DelayMagic,
                            magics,
                            cell));
                    }
                    break;


                #endregion

                default:
                    Connection.ReceiveChat("Spell Not Implemented", MessageType.System);
                    break;
            }


            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                    if (Buffs.Any(x => x.Type == BuffType.Cloak))
                    {
                        BuffRemove(BuffType.Cloak);
                        break;
                    }
                    ChangeHP(-(Stats[Stat.Health] * magic.Cost / 1000));
                    break;
                case MagicType.DragonRepulse:
                    ChangeHP(-(Stats[Stat.Health] * magic.Cost / 1000));
                    ChangeMP(-(Stats[Stat.Mana] * magic.Cost / 1000));
                    break;
                case MagicType.DarkConversion:
                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion))
                    {
                        BuffRemove(BuffType.DarkConversion);
                        break;
                    }
                    ChangeMP(-magic.Cost);
                    break;
                default:
                    ChangeMP(-magic.Cost);
                    break;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.DarkConversion:
                case MagicType.ChangeOfSeasons:
                case MagicType.TheNewBeginning:
                case MagicType.Transparency:
                    break;
                default:
                    BuffRemove(BuffType.Cloak);
                    BuffRemove(BuffType.Transparency);
                    break;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.ChangeOfSeasons:
                case MagicType.TheNewBeginning:
                case MagicType.SummonPuppet:
                case MagicType.SummonSkeleton:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonDemonicCreature:
                    break;
                case MagicType.Defiance:
                case MagicType.Might:
                case MagicType.ReflectDamage:

                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.Teleportation:
                case MagicType.GeoManipulation:
                case MagicType.MagicShield:
                case MagicType.FrostBite:
                case MagicType.Renounce:
                case MagicType.JudgementOfHeaven:
                case MagicType.MirrorImage:

                case MagicType.Heal:
                case MagicType.Invisibility:
                case MagicType.MagicResistance:
                case MagicType.MassInvisibility:
                case MagicType.Resilience:
                case MagicType.ElementalSuperiority:
                case MagicType.MassHeal:
                case MagicType.BloodLust:
                case MagicType.Resurrection:
                case MagicType.Transparency:
                case MagicType.CelestialLight:
                case MagicType.LifeSteal:
                case MagicType.SummonShinsu:
                case MagicType.StrengthOfFaith:

                case MagicType.PoisonousCloud:
                case MagicType.DarkConversion:
                    break;
                default:
                    CombatTime = SEnvir.Now;
                    break;
            }


            if (cast)
            {
                Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
            }


            Direction = ob == null || ob == this ? p.Direction : Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);

            if (Stats[Stat.Comfort] < 15)
                RegenTime = SEnvir.Now + RegenDelay;
            ActionTime = SEnvir.Now + Globals.CastTime;
            MagicTime = SEnvir.Now + Globals.MagicDelay;

            if (BagWeight > Stats[Stat.BagWeight])
                MagicTime += Globals.MagicDelay;

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }


            Broadcast(new S.ObjectMagic
            {
                ObjectID = ObjectID,
                Direction = Direction,
                CurrentLocation = CurrentLocation,
                Type = p.Type,
                Targets = targets,
                Locations = locations,
                Cast = cast,
                Slow = slow
            });
        }
        public void MagicToggle(C.MagicToggle p)
        {
            UserMagic magic;

            if (!Magics.TryGetValue(p.Magic, out magic) || Level < magic.Info.NeedLevel1 || Horse != HorseType.None) return;

            switch (p.Magic)
            {
                case MagicType.Thrusting:
                    Character.CanThrusting = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.HalfMoon:
                    Character.CanHalfMoon = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.DestructiveSurge:
                    Character.CanDestructiveSurge = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.FlameSplash:
                    Character.CanFlameSplash = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.DemonicRecovery:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    if (Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead))
                        return;

                    ChangeMP(-magic.Cost);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    DemonicRecoveryEnd(magic);
                    break;
                case MagicType.FlamingSword:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanFlamingSword)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.ChargeFail, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.ChargeFail, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        FlamingSwordTime = SEnvir.Now.AddSeconds(12);
                        CanFlamingSword = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanFlamingSword });
                    }

                    if (Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }

                    if (Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }

                    break;
                case MagicType.DragonRise:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanDragonRise)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.ChargeFail, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.ChargeFail, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        DragonRiseTime = SEnvir.Now.AddSeconds(12);
                        CanDragonRise = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanDragonRise });
                    }

                    if (Magics.TryGetValue(MagicType.FlamingSword, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }

                    if (Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }
                    break;
                case MagicType.BladeStorm:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanBladeStorm)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.ChargeFail, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.ChargeFail, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        BladeStormTime = SEnvir.Now.AddSeconds(12);
                        CanBladeStorm = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanBladeStorm });
                    }

                    if (Magics.TryGetValue(MagicType.FlamingSword, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }

                    if (Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(2) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(2);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 2000 });
                    }
                    break;
                case MagicType.Endurance:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    EnduranceEnd(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                    break;
            }
        }
        public void Mining(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < AttackTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Mining, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanAttack)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }



            CombatTime = SEnvir.Now;

            if (Stats[Stat.Comfort] < 15)
                RegenTime = SEnvir.Now + RegenDelay;
            Direction = direction;
            ActionTime = SEnvir.Now + Globals.AttackTime;

            int aspeed = Stats[Stat.AttackSpeed];
            int attackDelay = Globals.AttackDelay - aspeed * Globals.ASpeedRate;
            attackDelay = Math.Max(800, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            if (BagWeight > Stats[Stat.BagWeight])
                AttackTime += TimeSpan.FromMilliseconds(attackDelay);

            bool result = false;
            if (CurrentMap.Info.CanMine && CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction)) == null)
            {
                UserItem weap = Equipment[(int)EquipmentSlot.Weapon];

                if (weap != null && weap.Info.Effect == ItemEffect.PickAxe && (weap.CurrentDurability > 0 || weap.Info.Durability > 0))
                {
                    DamageItem(GridType.Equipment, (int)EquipmentSlot.Weapon, 4);

                    foreach (MineInfo info in CurrentMap.Info.Mining)
                    {
                        if (SEnvir.Random.Next(info.Chance) > 0) continue;

                        ItemCheck check = new ItemCheck(info.Item, 1, UserItemFlags.Bound, TimeSpan.Zero);

                        if (!CanGainItems(false, check)) continue;

                        UserItem item = SEnvir.CreateDropItem(check);
                        GainItem(item);
                    }


                    bool hasRubble = false;

                    foreach (MapObject ob in CurrentCell.Objects)
                    {
                        if (ob.Race != ObjectType.Spell) continue;

                        SpellObject rubble = (SpellObject)ob;
                        if (rubble.Effect != SpellEffect.Rubble) continue;

                        hasRubble = true;

                        rubble.Power++;
                        rubble.Broadcast(new S.ObjectSpellChanged { ObjectID = ob.ObjectID, Power = rubble.Power });
                        rubble.TickTime = SEnvir.Now.AddMinutes(1);
                        break;
                    }

                    if (!hasRubble)
                    {
                        SpellObject ob = new SpellObject
                        {
                            DisplayLocation = CurrentLocation,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromMinutes(1),
                            TickTime = SEnvir.Now.AddMinutes(1),
                            Owner = this,
                            Effect = SpellEffect.Rubble,
                        };

                        ob.Spawn(CurrentMap, CurrentLocation);

                        PauseBuffs();
                    }



                    result = true;
                }
            }


            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);
            Broadcast(new S.ObjectMining { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow, Effect = result });
        }
        #endregion

        #region Combat
        public bool AttackLocation(Point location, List<UserMagic> magics, bool primary)
        {
            Cell cell = CurrentMap.GetCell(location);

            if (cell?.Objects == null) return false;

            bool result = false;

            foreach (MapObject ob in cell.Objects)
            {
                if (!CanAttackTarget(ob)) continue;

                int delay = 300;
                foreach (UserMagic magic in magics)
                {
                    if (magic.Info.Magic == MagicType.DragonRise)
                        delay = 600;
                }

                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(delay), ActionType.DelayAttack,
                    ob,
                    magics,
                    primary,
                    0));


                result = true;
            }
            return result;
        }

        public void Attack(MapObject ob, List<UserMagic> magics, bool primary, int extra)
        {
            if (ob?.Node == null || ob.Dead) return;

            for (int i = Pets.Count - 1; i >= 0; i--)
                if (Pets[i].Target == null)
                    Pets[i].Target = ob;

            int power = GetDC();
            int karmaDamage = 0;
            bool ignoreAccuracy = false, hasFlameSplash = false, hasLotus = false, hasDestructiveSurge = false;
            bool hasBladeStorm = false, hasDanceOfSallows = false;
            bool hasMassacre = false;
            bool hasSwiftBlade = false, hasSeismicSlam = false;

            UserMagic magic;
            foreach (UserMagic mag in magics)
            {
                switch (mag.Info.Magic)
                {
                    case MagicType.FullBloom:
                    case MagicType.WhiteLotus:
                    case MagicType.RedLotus:
                    case MagicType.SweetBrier:
                        ignoreAccuracy = true;
                        hasLotus = true;
                        break;
                    case MagicType.SwiftBlade:
                    case MagicType.SeismicSlam:
                        ignoreAccuracy = true;
                        hasSwiftBlade = true;
                        break;
                    case MagicType.FlameSplash:
                        hasFlameSplash = !primary;
                        break;
                    case MagicType.DanceOfSwallow:
                        hasDanceOfSallows = true;
                        break;
                    case MagicType.DestructiveSurge:
                        hasDestructiveSurge = !primary;
                        break;


                }
            }

            int accuracy = Stats[Stat.Accuracy];

            int res;

            if (!ignoreAccuracy && SEnvir.Random.Next(ob.Stats[Stat.Agility]) > accuracy)
            {
                ob.Dodged();
                return;
            }

            bool hasStone = Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;

            for (int i = magics.Count - 1; i >= 0; i--)
            {
                magic = magics[i];
                int bonus;
                switch (magic.Info.Magic)
                {
                    case MagicType.Slaying:
                    case MagicType.CalamityOfFullMoon: // Lotus only
                        power += magic.GetPower();
                        break;
                    case MagicType.FlamingSword:
                        power = power * magic.GetPower() / 100;
                        break;
                    case MagicType.DragonRise:
                        power = power * magic.GetPower() / 100;
                        break;
                    case MagicType.BladeStorm:
                        power = power * magic.GetPower() / 100;
                        hasBladeStorm = true;
                        break;
                    case MagicType.Thrusting:
                    case MagicType.HalfMoon:
                    case MagicType.DestructiveSurge:
                        if (!primary)
                            power = power * magic.GetPower() / 100;
                        break;
                    case MagicType.SwiftBlade:
                        power = power * magic.GetPower() / 100;

                        if (ob.Race == ObjectType.Player)
                            power /= 2;
                        break;
                    case MagicType.SeismicSlam:
                        power = power * magic.GetPower() / 100;

                        if (ob.Race == ObjectType.Player)
                            power /= 2;

                        hasSeismicSlam = true;

                        break;
                    case MagicType.FullBloom:
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        power = Math.Max(0, power - ob.GetAC() + GetDC());

                        power += Math.Max(0, bonus - ob.GetMR());

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / 10;
                        else if (res < 0)
                            power -= power * res / 5;

                        BuffAdd(BuffType.FullBloom, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FullBloom });
                        break;
                    case MagicType.WhiteLotus:
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        power = Math.Max(0, power - ob.GetAC() + GetDC());

                        if (Buffs.Any(x => x.Type == BuffType.FullBloom))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }

                        power += Math.Max(0, bonus - ob.GetMR());

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / 10;
                        else if (res < 0)
                            power -= power * res / 5;

                        BuffRemove(BuffType.FullBloom);
                        BuffAdd(BuffType.WhiteLotus, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.WhiteLotus });
                        break;
                    case MagicType.RedLotus:
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        power = Math.Max(0, power - ob.GetAC() + GetDC()); //

                        if (Buffs.Any(x => x.Type == BuffType.WhiteLotus))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }

                        power += Math.Max(0, bonus - ob.GetMR());

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / 10;
                        else if (res < 0)
                            power -= power * res / 5;

                        BuffRemove(BuffType.WhiteLotus);
                        BuffAdd(BuffType.RedLotus, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.RedLotus });
                        break;
                    case MagicType.SweetBrier:

                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        power = Math.Max(0, power - ob.GetAC() + GetDC()); //

                        if (Buffs.Any(x => x.Type == BuffType.RedLotus))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }


                        power += Math.Max(0, bonus - ob.GetMR());

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / 10;
                        else if (res < 0)
                            power -= power * res / 5;

                        BuffRemove(BuffType.RedLotus);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.SweetBrier });
                        break;
                    case MagicType.Karma:
                        power += GetDC();

                        karmaDamage = ob.CurrentHP * magic.GetPower() / 100;

                        if (ob.Race == ObjectType.Monster)
                        {
                            if (((MonsterObject)ob).MonsterInfo.IsBoss)
                                karmaDamage = magic.GetPower() * 20;
                            else
                                karmaDamage /= 4;
                        }

                        /*   buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);
                           if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                           {
                               power += power * augMagic.GetPower() / 100;
                               magics.Add(augMagic);
                               BuffRemove(buff);
                               if (buff.Stats[Stat.TheNewBeginning] > 1)
                                   BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                           }
                           */
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.Karma });
                        break;
                    case MagicType.FlameSplash:
                        if (!primary)
                            power = power * magic.GetPower() / 100;

                        break;
                    case MagicType.DanceOfSwallow:
                        power += GetDC();
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.DanceOfSwallow });
                        break;
                    case MagicType.Massacre:
                        hasMassacre = true;
                        break;
                }
            }


            Element element = Element.None;

            if (!hasMassacre)
            {
                if (!hasLotus)
                {
                    power -= ob.GetAC();

                    if (ob.Race == ObjectType.Player)
                        res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                    else
                        res = ob.Stats.GetResistanceValue(Element.None);

                    if (res > 0)
                        power -= power * res / 10;
                    else if (res < 0)
                        power -= power * res / 5;
                }


                if (power < 0) power = 0;

                for (Element ele = Element.Fire; ele <= Element.Phantom; ele++)
                {
                    if (hasFlameSplash && ele > Element.Fire) break;

                    int value = Stats.GetElementValue(ele);

                    if (hasStone)
                    {
                        value += Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityValue(ele);
                        element = ele;
                    }

                    power += value;

                    res = ob.Stats.GetResistanceValue(ele);

                    if (res <= 0)
                        power -= value * res * 3 / 10;
                    else
                        power -= value * res * 2 / 10;
                }

                if (hasStone && (!hasFlameSplash || element == Element.Fire))
                    DamageDarkStone();

                if (hasFlameSplash)
                    element = Element.Fire;
            }
            else
            {
                power = extra;

                if (ob.Race == ObjectType.Player)
                    res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                else
                    res = ob.Stats.GetResistanceValue(Element.None);

                if (res > 0)
                    power -= power * res / 10;
                else if (res < 0)
                    power -= power * res / 5;
            }

            if (power <= 0)
            {
                ob.Blocked();
                return;
            }

            int damage = 0;
            if (hasBladeStorm)
            {
                power /= 2;
                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayedAttackDamage, ob, power, element, true, true, ob.Stats[Stat.MagicShield] == 0, true));
            }

            if (karmaDamage > 0)
                damage += ob.Attacked(this, karmaDamage, Element.None, false, true, false);

            damage += ob.Attacked(this, power, element, true, false, !hasMassacre);

            if (damage <= 0) return;


            CheckBrown(ob);

            DamageItem(GridType.Equipment, (int)EquipmentSlot.Weapon, SEnvir.Random.Next(2) + 1);
            if (hasDanceOfSallows && ob.Level < Level)
            {
                magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.DanceOfSwallow);

                ob.ApplyPoison(new Poison
                {
                    Type = PoisonType.Silenced,
                    TickCount = 1,
                    Owner = this,
                    TickFrequency = TimeSpan.FromSeconds(magic.GetPower() + 1),
                });

                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(1),
                    TickCount = 1,
                });
            }

            if (Buffs.Any(x => x.Type == BuffType.Might) && Magics.TryGetValue(MagicType.Might, out magic))
                LevelMagic(magic);

            decimal lifestealAmount = damage * Stats[Stat.LifeSteal] / 100M;


            if (hasSwiftBlade)
            {
                lifestealAmount = Math.Min(lifestealAmount, 2000 - SwiftBladeLifeSteal);
                SwiftBladeLifeSteal += lifestealAmount;
            }

            if (hasFlameSplash)
            {
                lifestealAmount = Math.Min(lifestealAmount, 750 - FlameSplashLifeSteal);
                FlameSplashLifeSteal += lifestealAmount;
            }
            if (hasDestructiveSurge)
            {
                lifestealAmount = Math.Min(lifestealAmount, 750 - DestructiveSurgeLifeSteal);
                DestructiveSurgeLifeSteal += lifestealAmount;
            }

            if (primary || Class == MirClass.Warrior || hasFlameSplash)
                LifeSteal += lifestealAmount;

            if (LifeSteal > 1)
            {
                int heal = (int)Math.Floor(LifeSteal);
                LifeSteal -= heal;
                ChangeHP(Math.Min((hasLotus ? 1500 : 750), heal));
            }

            //  if (primary)

            int psnRate = 200;

            if (ob.Level >= 250)
                psnRate = 2000;

            if (SEnvir.Random.Next(psnRate) < Stats[Stat.ParalysisChance] || hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    TickCount = 1,
                });
            }

            if (hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Type = PoisonType.WraithGrip,
                    Owner = this,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromMilliseconds(1500),
                });
            }

            if (ob.Race != ObjectType.Player && SEnvir.Random.Next(psnRate) < Stats[Stat.SlowChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Slow,
                    Value = 20,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Stats[Stat.SilenceChance] || hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Silenced,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            foreach (UserMagic mag in magics)
                LevelMagic(mag);

            if (ob.Dead && ob.Race == ObjectType.Monster && ob.CurrentHP < 0)
            {
                if (Magics.TryGetValue(MagicType.Massacre, out magic) && Level >= magic.Info.NeedLevel1)
                    magics.Add(magic);

                if (magic != null)
                {
                    power = Math.Abs(ob.CurrentHP) * magic.GetPower() / 100;


                    foreach (MapObject target in GetTargets(CurrentMap, ob.CurrentLocation, 2))
                    {
                        if (target.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)target;

                        if (mob.MonsterInfo.IsBoss) continue;

                        ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(600), ActionType.DelayAttack,
                            target,
                            magics,
                            false,
                            power));
                    }
                }
            }
        }

        public int MagicAttack(List<UserMagic> magics, MapObject ob, bool primary, Stats stats = null, int extra = 0)
        {
            if (ob?.Node == null || ob.Dead) return 0;

            if (PetMode == PetMode.PvP)
            {
                for (int i = Pets.Count - 1; i >= 0; i--)
                    if (Pets[i].CanAttackTarget(ob))
                        Pets[i].Target = ob;
            }
            else
                for (int i = Pets.Count - 1; i >= 0; i--)
                    if (Pets[i].Target == null)
                        Pets[i].Target = ob;

            Element element = Element.None;
            int slow = 0, slowLevel = 0, repel = 0, silence = 0;

            bool canStuck = true;

            int power = 0;
            UserMagic asteroid = null;

            foreach (UserMagic magic in magics)
            {
                switch (magic.Info.Magic)
                {
                    case MagicType.FireBall:
                    case MagicType.ScortchedEarth:
                    case MagicType.FireStorm:
                    case MagicType.AdamantineFireBall:
                    case MagicType.MeteorShower:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC();
                        break;
                    case MagicType.Asteroid:
                        element = Element.Fire;
                        asteroid = magic;
                        canStuck = false;
                        break;
                    case MagicType.FireWall:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC();
                        canStuck = false;
                        break;
                    case MagicType.HellFire:
                        element = Element.Fire;
                        power += magic.GetPower() + GetDC();
                        break;
                    case MagicType.IceBolt:
                    case MagicType.FrozenEarth:
                        slowLevel = 3;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC();
                        slow = 10;
                        break;
                    case MagicType.IceBlades:
                    case MagicType.GreaterFrozenEarth:
                    case MagicType.IceStorm:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC();
                        slow = 5;
                        break;
                    case MagicType.FrostBite:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += Math.Min(stats[Stat.FrostBiteDamage], stats[Stat.FrostBiteMaxDamage]) - Stats[Stat.IceAttack] * 2;
                        slow = 5;
                        break;
                    case MagicType.LightningBall:
                    case MagicType.ThunderBolt:
                    case MagicType.LightningWave:
                    case MagicType.LightningBeam:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC();
                        break;
                    case MagicType.ThunderStrike:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC();
                        power += power / 2;
                        break;
                    case MagicType.ChainLightning:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC();

                        power = power * 5 / (extra + 5);
                        break;
                    case MagicType.GustBlast:
                    case MagicType.BlowEarth:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC();
                        repel = 10;
                        break;
                    case MagicType.Cyclone:
                    case MagicType.DragonTornado:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC();
                        repel = 5;
                        break;
                    case MagicType.Tempest:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC();
                        repel = 5;
                        canStuck = false;
                        break;

                    case MagicType.ExplosiveTalisman:
                        element = Element.Dark;
                        power += magic.GetPower() + GetSC();
                        break;
                    case MagicType.ImprovedExplosiveTalisman:
                        element = Element.Dark;
                        power += magic.GetPower() + GetSC();
                        //power += power;
                        break;
                    case MagicType.EvilSlayer:
                    case MagicType.GreaterEvilSlayer:
                        element = Element.Holy;
                        power += magic.GetPower() + GetSC();
                        break;
                    case MagicType.SummonPuppet:
                        element = Element.Fire;
                        power += GetDC() * magic.GetPower() / 100;
                        break;
                    case MagicType.Rake:
                        element = Element.Ice;
                        power += GetDC() * magic.GetPower() / 100;
                        slow = 1;
                        slowLevel = 10;
                        break;
                    case MagicType.DragonRepulse:
                        element = Element.Lightning;
                        power = GetDC() * magic.GetPower() / 100 + Level;

                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }

                        break;
                    case MagicType.FlashOfLight:
                        element = Element.None;
                        power = GetDC() * magic.GetPower() / 100;

                        /* 
                           BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);
                           UserMagic augMagic;

                           if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                           {
                               power *= 2;
                               LevelMagic(augMagic);
                               BuffRemove(buff);
                               if (buff.Stats[Stat.TheNewBeginning] > 1)
                                   BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                           }*/

                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FlashOfLight });
                        break;
                    /*case MagicType.TheNewBeginning:
                        power += 2;
                        break;*/
                    case MagicType.ElementalPuppet:
                        if (stats.Count == 0) break;

                        foreach (KeyValuePair<Stat, int> s in stats.Values)
                        {
                            switch (s.Key)
                            {
                                case Stat.FireAffinity:
                                    element = Element.Fire;
                                    power += s.Value;
                                    break;
                                case Stat.IceAffinity:
                                    element = Element.Ice;
                                    power += s.Value;
                                    slow = 2;
                                    slowLevel = 3;
                                    break;
                                case Stat.LightningAffinity:
                                    element = Element.Lightning;
                                    power += s.Value;
                                    break;
                                case Stat.WindAffinity:
                                    element = Element.Wind;
                                    power += s.Value;
                                    repel = 2;
                                    silence = 4;
                                    break;
                                case Stat.HolyAffinity:
                                    element = Element.Holy;
                                    power += s.Value;
                                    break;
                                case Stat.DarkAffinity:
                                    element = Element.Dark;
                                    power += s.Value;
                                    break;
                                case Stat.PhantomAffinity:
                                    element = Element.Phantom;
                                    power += s.Value;
                                    break;
                            }
                        }

                        break;
                    case MagicType.DemonExplosion:
                        power = extra;
                        element = Element.Phantom;
                        break;
                }
            }


            foreach (UserMagic magic in magics)
            {
                switch (magic.Info.Magic)
                {
                    case MagicType.ScortchedEarth:
                    case MagicType.LightningBeam:
                    case MagicType.BlowEarth:
                    case MagicType.FrozenEarth:
                    case MagicType.GreaterFrozenEarth:
                        if (!primary)
                            power = (int)(power * 0.3F);
                        break;
                    case MagicType.FireWall:
                        power = (int)(power * 0.60F);
                        break;
                    case MagicType.Tempest:
                        power = (int)(power * 0.80F);
                        break;

                    case MagicType.ExplosiveTalisman:
                        if (stats != null && stats[Stat.DarkAffinity] >= 1)
                            power += (int)(power * 0.3F);

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }

                        break;
                    case MagicType.ImprovedExplosiveTalisman:
                        if (stats != null && stats[Stat.DarkAffinity] >= 1)
                            power += (int)(power * 0.6F);

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }

                        break;

                    case MagicType.EvilSlayer:
                        if (stats != null && stats[Stat.HolyAffinity] >= 1)
                            power += (int)(power * 0.3F);

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }

                        break;

                    case MagicType.GreaterEvilSlayer:
                        if (stats != null && stats[Stat.HolyAffinity] >= 1)
                            power += (int)(power * 0.6F);

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }

                        break;
                }
            }

            power -= ob.GetMR();


            /* if (Buffs.Any(x => x.Type == BuffType.Renounce))
             {
                 if (ob.Race == ObjectType.Player)
                     power += ob.Stats[Stat.Health] * (1 + (Math.Min(4000, ob.Stats[Stat.Health]) / 2000)) / 100;
             }*/

            switch (element)
            {
                case Element.None:
                    power -= power * ob.Stats[Stat.PhysicalResistance] / 10;
                    break;
                case Element.Fire:
                    power += GetElementPower(ob.Race, Stat.FireAttack) * 2;
                    power -= power * ob.Stats[Stat.FireResistance] / 10;
                    break;
                case Element.Ice:
                    power += GetElementPower(ob.Race, Stat.IceAttack) * 2;
                    power -= power * ob.Stats[Stat.IceResistance] / 10;
                    break;
                case Element.Lightning:
                    power += GetElementPower(ob.Race, Stat.LightningAttack) * 2;
                    power -= power * ob.Stats[Stat.LightningResistance] / 10;
                    break;
                case Element.Wind:
                    power += GetElementPower(ob.Race, Stat.WindAttack) * 2;
                    power -= power * ob.Stats[Stat.WindResistance] / 10;
                    break;
                case Element.Holy:
                    power += GetElementPower(ob.Race, Stat.HolyAttack) * 2;
                    power -= power * ob.Stats[Stat.HolyResistance] / 10;
                    break;
                case Element.Dark:
                    power += GetElementPower(ob.Race, Stat.DarkAttack) * 2;
                    power -= power * ob.Stats[Stat.DarkResistance] / 10;
                    break;
                case Element.Phantom:
                    power += GetElementPower(ob.Race, Stat.PhantomAttack) * 2;
                    power -= power * ob.Stats[Stat.PhantomResistance] / 10;
                    break;
            }

            if (asteroid != null)
                power += asteroid.GetPower() + GetMC();

            if (power <= 0)
            {
                ob.Blocked();
                return 0;
            }


            int damage = ob.Attacked(this, power, element, false, false, true, canStuck);

            if (damage <= 0) return damage;

            int psnRate = 100;

            if (ob.Level >= 250)
                psnRate = 1000;
            if (SEnvir.Random.Next(psnRate) < Stats[Stat.ParalysisChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 1,
                });
            }

            if (ob.Race != ObjectType.Player && SEnvir.Random.Next(psnRate) < Stats[Stat.SlowChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Slow,
                    Value = 20,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Stats[Stat.SilenceChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Silenced,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    /*   if (slow > 0 && SEnvir.Random.Next(slow) == 0 && Level > ob.Level)
                       {
                           TimeSpan duration = TimeSpan.FromSeconds(3 + SEnvir.Random.Next(3));
                           if (ob.Race == ObjectType.Monster)
                           {
                               slowLevel *= 2;
                               duration += duration;
                           }


                           ob.ApplyPoison(new Poison
                           {
                               Type = PoisonType.Slow,
                               Value = slowLevel,
                               TickCount = 1,
                               TickFrequency = duration,
                               Owner = this,
                           });*/
                    /*

                    if (repel > 0 && ob.CurrentMap == CurrentMap && Level  > ob.Level && SEnvir.Random.Next(repel) == 0)
                    {
                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }
                    }*/
                    break;
                case ObjectType.Monster:
                    if (slow > 0 && SEnvir.Random.Next(slow) == 0 && !((MonsterObject)ob).MonsterInfo.IsBoss)
                    {
                        TimeSpan duration = TimeSpan.FromSeconds(3 + SEnvir.Random.Next(3));

                        slowLevel *= 2;
                        duration += duration;

                        ob.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Slow,
                            Value = slowLevel,
                            TickCount = 1,
                            TickFrequency = duration,
                            Owner = this,
                        });
                    }

                    if (repel > 0 && ob.CurrentMap == CurrentMap && Level > ob.Level && SEnvir.Random.Next(repel) == 0)
                    {
                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }
                    }

                    if (silence > 0 && !((MonsterObject)ob).MonsterInfo.IsBoss)
                    {
                        ob.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Silenced,
                            Value = slowLevel,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromSeconds(silence),
                            Owner = this,
                        });
                    }

                    break;
            }

            CheckBrown(ob);

            foreach (UserMagic magic in magics)
                LevelMagic(magic);

            UserMagic temp;
            if (Buffs.Any(x => x.Type == BuffType.Renounce) && Magics.TryGetValue(MagicType.Renounce, out temp))
            {
                LevelMagic(temp);
            }

            if (Magics.TryGetValue(MagicType.AdvancedRenounce, out temp))
                LevelMagic(temp);


            return damage;
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker?.Node == null || power == 0 || Dead || attacker.CurrentMap != CurrentMap || !Functions.InRange(attacker.CurrentLocation, CurrentLocation, Config.MaxViewRange)) return 0;

            UserMagic magic;
            if (element != Element.None)
            {
                if (SEnvir.Random.Next(attacker.Race == ObjectType.Player ? 200 : 100) <= Stats[Stat.EvasionChance])// 4 + magic.Level * 2)
                {
                    if (Buffs.Any(x => x.Type == BuffType.Evasion) && Magics.TryGetValue(MagicType.Evasion, out magic))
                        LevelMagic(magic);

                    DisplayMiss = true;
                    return 0;
                }
            }
            else
            {
                if (SEnvir.Random.Next(attacker.Race == ObjectType.Player ? 200 : 100) <= Stats[Stat.BlockChance])
                {
                    DisplayMiss = true;
                    return 0;
                }
            }



            CombatTime = SEnvir.Now;

            if (attacker.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)attacker).PvPTime = SEnvir.Now;
            }


            if (Stats[Stat.Comfort] < 20)
                RegenTime = SEnvir.Now + RegenDelay;

            if ((Poison & PoisonType.Red) == PoisonType.Red)
                power = (int)(power * 1.2F);

            for (int i = 0; i < attacker.Stats[Stat.Rebirth]; i++)
                power = (int)(power * 1.2F);

            if (SEnvir.Random.Next(100) < attacker.Stats[Stat.CriticalChance] && canCrit)
            {
                if (!canReflect)
                    power = (int)(power * 1.2F);
                else if (attacker.Race == ObjectType.Player)
                    power = (int)(power * 1.3F);
                else
                    power += power;

                Critical();
            }

            BuffInfo buff;

            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.FrostBite);

            if (buff != null)
            {
                buff.Stats[Stat.FrostBiteDamage] += power;
                Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });
                return 0;
            }

            if (attacker.Race == ObjectType.Monster && SEnvir.Now < FrostBiteImmunity) return 0;

            if (!ignoreShield)
            {
                if (Buffs.Any(x => x.Type == BuffType.Cloak))
                    power -= power / 2;

                buff = Buffs.FirstOrDefault(x => x.Type == BuffType.MagicShield);

                if (buff != null)
                {
                    buff.RemainingTime -= TimeSpan.FromMilliseconds(power * 25);
                    Enqueue(new S.BuffTime { Index = buff.Index, Time = buff.RemainingTime });
                }

                power -= power * Stats[Stat.MagicShield] / 100;
            }



            //STRUCKDONE
            if (StruckTime != DateTime.MaxValue && SEnvir.Now > StruckTime.AddMilliseconds(500) && canStruck) //&&!Buffs.Any(x => x.Type == BuffType.DragonRepulse)) 
            {
                StruckTime = SEnvir.Now;

                //if (StruckTime.AddMilliseconds(300) > ActionTime) ActionTime = StruckTime.AddMilliseconds(300);
                Broadcast(new S.ObjectStruck { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, AttackerID = attacker.ObjectID, Element = element });

                bool update = false;
                for (int i = 0; i < Equipment.Length; i++)
                {
                    switch ((EquipmentSlot)i)
                    {
                        case EquipmentSlot.Amulet:
                        case EquipmentSlot.Poison:
                        case EquipmentSlot.Torch:
                            continue;
                    }

                    update = DamageItem(GridType.Equipment, i, SEnvir.Random.Next(2) + 1, true) || update;
                }

                if (update)
                {
                    SendShapeUpdate();
                    RefreshStats();
                }
            }


            #region Conquest Stats

            UserConquestStats conquest = SEnvir.GetConquestStats(this);

            if (conquest != null)
            {
                switch (attacker.Race)
                {
                    case ObjectType.Player:
                        conquest.PvPDamageTaken += power;

                        conquest = SEnvir.GetConquestStats((PlayerObject)attacker);

                        if (conquest != null)
                            conquest.PvPDamageDealt += power;

                        break;
                    case ObjectType.Monster:
                        MonsterObject mob = (MonsterObject)attacker;

                        if (mob is CastleLord)
                            conquest.BossDamageTaken += power;
                        else if (mob.PetOwner != null)
                        {
                            conquest.PvPDamageTaken += power;

                            conquest = SEnvir.GetConquestStats(mob.PetOwner);

                            if (conquest != null)
                                conquest.PvPDamageDealt += power;
                        }
                        break;
                }
            }

            #endregion


            LastHitter = attacker;
            ChangeHP(-power);
            LastHitter = null;

            if (canReflect && CanAttackTarget(attacker) && attacker.Race != ObjectType.Player)
            {
                attacker.Attacked(this, power * Stats[Stat.ReflectDamage] / 100, Element.None, false);

                if (Buffs.Any(x => x.Type == BuffType.ReflectDamage) && Magics.TryGetValue(MagicType.ReflectDamage, out magic))
                    LevelMagic(magic);
            }



            if (canReflect && CanAttackTarget(attacker) && SEnvir.Random.Next(100) < Stats[Stat.JudgementOfHeaven] && !(attacker is CastleLord))
            {
                int damagePvE = GetMC() / 5 + GetElementPower(ObjectType.Monster, Stat.LightningAttack) * 2;
                int damagePvP = Math.Min(50, GetMC() / 5 + GetElementPower(ObjectType.Monster, Stat.LightningAttack) / 2);

                Broadcast(new S.ObjectEffect { ObjectID = attacker.ObjectID, Effect = Effect.ThunderBolt });
                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayedAttackDamage, attacker, attacker.Race == ObjectType.Player ? damagePvP : damagePvE, Element.Lightning, false, false, true, true));

                if (Buffs.Any(x => x.Type == BuffType.JudgementOfHeaven) && Magics.TryGetValue(MagicType.JudgementOfHeaven, out magic))
                    LevelMagic(magic);
            }

            if (Buffs.Any(x => x.Type == BuffType.Defiance) && Magics.TryGetValue(MagicType.Defiance, out magic))
                LevelMagic(magic);

            if (Buffs.Any(x => x.Type == BuffType.RagingWind) && Magics.TryGetValue(MagicType.RagingWind, out magic))
                LevelMagic(magic);

            if (Magics.TryGetValue(MagicType.AdventOfDemon, out magic) && element == Element.None)
                LevelMagic(magic);

            if (Magics.TryGetValue(MagicType.AdventOfDevil, out magic) && element != Element.None)
                LevelMagic(magic);

            return power;
        }

        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob?.Node == null || ob == this || ob.Dead || !ob.Visible || ob is Guard) return false;

            switch (ob.Race)
            {
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == null) return true; //Wild Monster

                    // Player vs Pet
                    if (mob.PetOwner == this)
                        return AttackMode == AttackMode.All || mob is Puppet;

                    if (mob is Puppet) return false; //Don't hit other person's puppet

                    if (mob.InSafeZone || InSafeZone) return false;

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (InGroup(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (InGuild(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(mob.PetOwner))
                                return false;
                            break;
                    }

                    return true;
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    if (player.GameMaster) return false;

                    if (InSafeZone || player.InSafeZone) return false; //Login Time?

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (InGroup(player))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (InGuild(player))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(player))
                                return false;
                            break;
                    }

                    return true;
                default:
                    return false;
            }
        }
        public override bool CanHelpTarget(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;
            if (ob == this) return true;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (InGroup(player))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (InGuild(player))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(player))
                                return true;
                            break;
                    }

                    return false;

                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == this) return true;
                    if (mob.PetOwner == null) return false;

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (InGroup(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (InGuild(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(mob.PetOwner))
                                return true;
                            break;
                    }

                    return false;

                default:
                    return false;
            }
        }

        public void CompleteMagic(params object[] data)
        {
            List<UserMagic> magics = (List<UserMagic>)data[0];
            foreach (UserMagic magic in magics)
            {

                switch (magic.Info.Magic)
                {

                    #region Warrior
                    case MagicType.Interchange:
                        InterchangeEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.Beckon:
                        BeckonEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.MassBeckon:
                        MassBeckonEnd(magic);
                        break;
                    case MagicType.Defiance:
                        DefianceEnd(magic);
                        break;
                    case MagicType.Might:
                        MightEnd(magic);
                        break;
                    case MagicType.ReflectDamage:
                        ReflectDamageEnd(magic);
                        break;
                    case MagicType.Fetter:
                        FetterEnd(magic, (Cell)data[1]);
                        break;
                    case MagicType.SwiftBlade:
                    case MagicType.SeismicSlam:
                        Cell cell = (Cell)data[1];
                        if (cell == null || cell.Objects == null) continue;

                        for (int i = cell.Objects.Count - 1; i >= 0; i--)
                        {
                            if (!CanAttackTarget(cell.Objects[i])) continue;
                            Attack(cell.Objects[i], magics, true, 0);
                        }

                        break;

                    #endregion

                    #region Wizard

                    case MagicType.FireBall:
                    case MagicType.IceBolt:
                    case MagicType.LightningBall:
                    case MagicType.ThunderBolt:
                    case MagicType.GustBlast:
                    case MagicType.AdamantineFireBall:
                    case MagicType.IceBlades:
                    case MagicType.Cyclone:
                    case MagicType.MeteorShower:
                    case MagicType.ThunderStrike:
                    case MagicType.DragonRepulse:
                        MagicAttack(magics, (MapObject)data[1], true);
                        break;
                    case MagicType.Repulsion:
                        RepulsionEnd(magic, (Cell)data[1], (MirDirection)data[2]);
                        break;
                    case MagicType.ScortchedEarth:
                    case MagicType.LightningBeam:
                    case MagicType.FrozenEarth:
                    case MagicType.BlowEarth:
                    case MagicType.GreaterFrozenEarth:
                        AttackCell(magics, (Cell)data[1], (bool)data[2]);
                        break;
                    case MagicType.FireStorm:
                    case MagicType.LightningWave:
                    case MagicType.IceStorm:
                    case MagicType.DragonTornado:
                    case MagicType.Asteroid:
                        AttackCell(magics, (Cell)data[1], true);
                        break;
                    case MagicType.ChainLightning:
                        ChainLightningEnd(magics, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.Teleportation:
                        TeleportationEnd(magic);
                        break;
                    case MagicType.ElectricShock:
                        ElectricShockEnd(magic, (MonsterObject)data[1]);
                        break;
                    case MagicType.ExpelUndead:
                        ExpelUndeadEnd(magic, (MonsterObject)data[1]);
                        break;
                    case MagicType.FireWall:
                        FireWallEnd(magic, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.MagicShield:
                        MagicShieldEnd(magic);
                        break;
                    case MagicType.FrostBite:
                        FrostBiteEnd(magic);
                        break;
                    case MagicType.GeoManipulation:
                        GeoManipulationEnd(magic, (Point)data[1]);
                        break;
                    case MagicType.Renounce:
                        RenounceEnd(magic);
                        break;
                    case MagicType.Tempest:
                        TempestEnd(magic, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.JudgementOfHeaven:
                        JudgementOfHeavenEnd(magic);
                        break;


                    #endregion

                    #region Taoist

                    case MagicType.Heal:
                        HealEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.PoisonDust:
                        PoisonDustEnd(magics, (MapObject)data[1], (PoisonType)data[2]);
                        break;
                    case MagicType.ExplosiveTalisman:
                    case MagicType.EvilSlayer:
                    case MagicType.GreaterEvilSlayer:
                    case MagicType.ImprovedExplosiveTalisman:
                        MagicAttack(magics, (MapObject)data[1], (bool)data[2], (Stats)data[3]);
                        break;
                    case MagicType.Invisibility:
                        InvisibilityEnd(magic, this);
                        break;
                    case MagicType.StrengthOfFaith:
                        StrengthOfFaithEnd(magic);
                        break;
                    case MagicType.Transparency:
                        TransparencyEnd(magic, this, (Point)data[1]);
                        break;
                    case MagicType.CelestialLight:
                        CelestialLightEnd(magic);
                        break;
                    case MagicType.DemonExplosion:
                        DemonExplosionEnd(magic, (Stats)data[1]);
                        break;
                    case MagicType.MagicResistance:
                    case MagicType.ElementalSuperiority:
                        BuffCell(magic, (Cell)data[1], (Stats)data[2]);
                        break;
                    case MagicType.SummonSkeleton:
                    case MagicType.SummonJinSkeleton:
                    case MagicType.SummonShinsu:
                    case MagicType.SummonDemonicCreature:
                        SummonEnd(magic, (Map)data[1], (Point)data[2], (MonsterInfo)data[3]);
                        break;
                    case MagicType.TrapOctagon:
                        TrapOctagonEnd(magic, (Map)data[1], (Point)data[2]);
                        break;
                    case MagicType.Resilience:
                    case MagicType.MassInvisibility:
                    case MagicType.BloodLust:
                    case MagicType.MassHeal:
                    case MagicType.LifeSteal:
                        BuffCell(magic, (Cell)data[1], null);
                        break;
                    case MagicType.TaoistCombatKick:
                        TaoistCombatKick(magic, (Cell)data[1], (MirDirection)data[2]);
                        break;
                    case MagicType.Purification:
                        PurificationEnd(magics, (MapObject)data[1]);
                        break;
                    case MagicType.Resurrection:
                        ResurrectionEnd(magic, (PlayerObject)data[1]);
                        break;
                    case MagicType.Infection:
                        InfectionEnd(magics, (MapObject)data[1]);
                        break;

                    #endregion

                    #region Assassin

                    case MagicType.PoisonousCloud:
                        PoisonousCloudEnd(magic);
                        break;
                    case MagicType.Cloak:
                        CloakEnd(magic, this, false);
                        break;
                    case MagicType.WraithGrip:
                        WraithGripEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.Abyss:
                        AbyssEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.HellFire:
                        HellFireEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.TheNewBeginning:
                        TheNewBeginningEnd(magic, this);
                        break;
                    case MagicType.SummonPuppet:
                        SummonPuppetEnd(magic, this);
                        break;
                    case MagicType.DarkConversion:
                        DarkConversionEnd(magic, this);
                        break;
                    case MagicType.Evasion:
                        EvasionEnd(magic, this);
                        break;
                    case MagicType.RagingWind:
                        RagingWindEnd(magic, this);
                        break;

                    case MagicType.Rake:
                        RakeEnd(magic, (Cell)data[1]);
                        break;
                    case MagicType.FlashOfLight:
                        AttackCell(magics, (Cell)data[1], true);
                        break;

                        #endregion
                }
            }
        }
        public void LevelMagic(UserMagic magic)
        {
            if (magic == null) return;

            int experience = SEnvir.Random.Next(Config.SkillExp) + 1;

            experience *= Stats[Stat.SkillRate];

            int maxExperience;
            switch (magic.Level)
            {
                case 0:
                    if (Level < magic.Info.NeedLevel1) return;

                    maxExperience = magic.Info.Experience1;
                    break;
                case 1:
                    if (Level < magic.Info.NeedLevel2) return;

                    maxExperience = magic.Info.Experience2;
                    break;
                case 2:
                    if (Level < magic.Info.NeedLevel3) return;

                    maxExperience = magic.Info.Experience3;
                    break;
                default:
                    return;
            }

            magic.Experience += experience;

            if (magic.Experience >= maxExperience)
            {
                magic.Experience -= maxExperience;
                magic.Level++;
                RefreshStats();

                for (int i = Pets.Count - 1; i >= 0; i--)
                    Pets[i].RefreshStats();
            }

            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });
        }

        public override int Pushed(MirDirection direction, int distance)
        {
            UserMagic magic;
            if (Buffs.Any(x => x.Type == BuffType.Endurance) && Magics.TryGetValue(MagicType.Endurance, out magic))
                LevelMagic(magic);

            RemoveMount();

            return base.Pushed(direction, distance);
        }

        public override bool ApplyPoison(Poison p)
        {
            if (p.Owner != null && p.Owner.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)p.Owner).PvPTime = SEnvir.Now;
            }

            if (Buffs.Any(x => x.Type == BuffType.Endurance))
            {
                UserMagic magic;
                if (Magics.TryGetValue(MagicType.Endurance, out magic))
                    LevelMagic(magic);

                return false;
            }

            bool res = base.ApplyPoison(p);

            if (res)
            {
                Connection.ReceiveChat(Connection.Language.Poisoned, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.Poisoned, MessageType.System);


                if (p.Owner != null && p.Owner.Race == ObjectType.Player)
                    ((PlayerObject)p.Owner).CheckBrown(this);
            }

            return res;
        }
        private void AttackCell(List<UserMagic> magics, Cell cell, bool primary)
        {
            if (cell?.Objects == null) return;
            if (cell.Objects.Count == 0) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob)) continue;

                MagicAttack(magics, ob, primary);
            }
        }
        private void BuffCell(UserMagic magic, Cell cell, Stats stats)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanHelpTarget(ob)) continue;

                switch (magic.Info.Magic)
                {
                    case MagicType.MagicResistance:
                        MagicResistanceEnd(magic, ob, stats);
                        break;
                    case MagicType.Resilience:
                        ResilienceEnd(magic, ob);
                        break;
                    case MagicType.MassInvisibility:
                        InvisibilityEnd(magic, ob);
                        break;
                    case MagicType.ElementalSuperiority:
                        ElementalSuperiorityEnd(magic, ob, stats);
                        break;
                    case MagicType.BloodLust:
                        BloodLustEnd(magic, ob);
                        break;
                    case MagicType.MassHeal:
                        HealEnd(magic, ob);
                        break;
                    case MagicType.LifeSteal:
                        LifeStealEnd(magic, ob);
                        break;
                }
            }
        }

        public override void Die()
        {
            RevivalTime = SEnvir.Now + Config.AutoReviveDelay;

            RemoveMount();

            TradeClose();

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>(TaggedMonsters);

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;

            TaggedMonsters.Clear();

            base.Die();

            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();

            for (int i = Pets.Count - 1; i >= 0; i--)
                Pets[i].Die();
            Pets.Clear();

            #region Conquest Stats

            UserConquestStats conquest = SEnvir.GetConquestStats(this);

            if (conquest != null && LastHitter != null)
            {
                switch (LastHitter.Race)
                {
                    case ObjectType.Player:
                        conquest.PvPDeathCount++;

                        conquest = SEnvir.GetConquestStats((PlayerObject)LastHitter);

                        if (conquest != null)
                            conquest.PvPKillCount++;
                        break;
                    case ObjectType.Monster:
                        MonsterObject mob = (MonsterObject)LastHitter;

                        if (mob is CastleLord)
                            conquest.BossDeathCount++;
                        else if (mob.PetOwner != null)
                        {
                            conquest.PvPDeathCount++;

                            conquest = SEnvir.GetConquestStats(mob.PetOwner);
                            if (conquest != null)
                                conquest.PvPKillCount++;
                        }
                        break;
                }
            }

            #endregion

            switch (CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                    return;
            }

            if (InSafeZone) return;

            PlayerObject attacker = null;

            if (LastHitter != null)
            {
                switch (LastHitter.Race)
                {
                    case ObjectType.Player:
                        attacker = (PlayerObject)LastHitter;
                        break;
                    case ObjectType.Monster:
                        attacker = ((MonsterObject)LastHitter).PetOwner;
                        break;
                }
            }

            if (Stats[Stat.Rebirth] > 0 && (LastHitter == null || LastHitter.Race != ObjectType.Player))
            {
                //Level = Math.Max(Level - Stats[Stat.Rebirth] * 3, 1);
                decimal expbonus = Experience;
                Enqueue(new S.GainedExperience { Amount = -expbonus });
                Experience = 0;

                if (expbonus > 0)
                {
                    List<PlayerObject> targets = new List<PlayerObject>();

                    foreach (PlayerObject player in SEnvir.Players)
                    {
                        if (player.Character.Rebirth > 0 || player.Character.Level >= 86) continue;

                        targets.Add(player);
                    }

                    PlayerObject target = null;
                    if (targets.Count > 0)
                    {
                        target = targets[SEnvir.Random.Next(targets.Count)];

                        target.GainExperience(expbonus, false, int.MaxValue, false);
                    }

                    SEnvir.Broadcast(new S.Chat { Text = $"{Name} has died and lost {expbonus:##,##0} Experience, {target?.Name ?? "No one"} has won the experience.", Type = MessageType.System });
                }

                // Enqueue(new S.LevelChanged { Level = Level, Experience = Experience });
                // Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });
            }


            BuffInfo buff;
            int rate;
            TimeSpan time;

            if (attacker != null)
            {
                if (AtWar(attacker))
                {
                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        if (member.Account.Connection == null) continue;

                        member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildWarDeath, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);

                        foreach (SConnection con in member.Account.Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.GuildWarDeath, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);

                    }
                    foreach (GuildMemberInfo member in attacker.Character.Account.GuildMember.Guild.Members)
                    {
                        if (member.Account.Connection == null) continue;

                        member.Account.Connection.ReceiveChat(string.Format(member.Account.Connection.Language.GuildWarDeath, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);

                        foreach (SConnection con in member.Account.Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.GuildWarDeath, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                    }
                }
                else
                {
                    if (Stats[Stat.PKPoint] < Config.RedPoint && Stats[Stat.Brown] == 0)
                    {
                        Connection.ReceiveChat(string.Format(Connection.Language.MurderedBy, attacker.Name), MessageType.System);
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.MurderedBy, attacker.Name), MessageType.System);


                        //PvP death

                        if (attacker.Stats[Stat.PKPoint] >= Config.RedPoint && SEnvir.Random.Next(Config.PvPCurseRate) == 0)
                        {
                            rate = -1;
                            time = Config.PvPCurseDuration;
                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PvPCurse);

                            if (buff != null)
                            {
                                rate += buff.Stats[Stat.Luck];
                                time += buff.RemainingTime;
                            }

                            attacker.BuffAdd(BuffType.PvPCurse, time, new Stats { [Stat.Luck] = rate }, false, false, TimeSpan.Zero);

                            attacker.Connection.ReceiveChat(string.Format(attacker.Connection.Language.Curse, Name), MessageType.System);
                            foreach (SConnection con in attacker.Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.Murdered, Name), MessageType.System);
                        }
                        else
                        {
                            attacker.Connection.ReceiveChat(string.Format(attacker.Connection.Language.Murdered, Name), MessageType.System);
                            foreach (SConnection con in attacker.Connection.Observers)
                                con.ReceiveChat(string.Format(con.Language.Murdered, Name), MessageType.System);
                        }

                        attacker.IncreasePKPoints(Config.PKPointRate);
                    }
                    else
                    {
                        attacker.Connection.ReceiveChat(attacker.Connection.Language.Protected, MessageType.System);
                        foreach (SConnection con in attacker.Connection.Observers)
                            con.ReceiveChat(con.Language.Protected, MessageType.System);

                        Connection.ReceiveChat(string.Format(Connection.Language.Killed, attacker.Name), MessageType.System);
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(string.Format(con.Language.Killed, attacker.Name), MessageType.System);
                    }
                }
            }
            else
            {
                if (Stats[Stat.PKPoint] >= Config.RedPoint)
                {
                    bool update = false;
                    for (int i = 0; i < Equipment.Length; i++)
                    {
                        UserItem item = Equipment[i];
                        if (item == null) continue;

                        update = DamageItem(GridType.Equipment, i, item.Info.Durability / 10) || update;
                    }

                    if (update)
                    {
                        SendShapeUpdate();
                        RefreshStats();
                    }
                }

                Connection.ReceiveChat(Connection.Language.Died, MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(con.Language.Died, MessageType.System);
            }

            if (Stats[Stat.DeathDrops] > 0)
                DeathDrop();
        }
        public void DeathDrop()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];

                if (item == null) continue;
                if (!item.Info.CanDeathDrop) continue;
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;
                if (SEnvir.Random.Next(10) > 0) continue;

                Cell cell = GetDropLocation(4, null);

                if (cell == null) break;

                long count;

                count = 1 + SEnvir.Random.Next((int)item.Count);

                UserItem dropItem;
                if (count == item.Count)
                {
                    dropItem = item;
                    RemoveItem(item);
                    Inventory[i] = null;
                    count = 0;
                }
                else
                {
                    dropItem = SEnvir.CreateFreshItem(item);
                    dropItem.Count = count;
                    item.Count -= count;

                    count = item.Count;
                }

                Companion?.RefreshWeight();
                dropItem.IsTemporary = true;

                ItemObject ob = new ItemObject
                {
                    Item = dropItem,
                };

                ob.Spawn(CurrentMap, cell.Location);

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = count }, Success = true });
            }


            if (Companion != null)
            {
                for (int i = 0; i < Companion.Inventory.Length; i++)
                {
                    UserItem item = Companion.Inventory[i];

                    if (item == null) continue;
                    if (!item.Info.CanDeathDrop) continue;
                    if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                    if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;
                    if (SEnvir.Random.Next(7) > 0) continue;

                    Cell cell = GetDropLocation(4, null);

                    if (cell == null) break;

                    long count;

                    count = 1 + SEnvir.Random.Next((int)item.Count);

                    UserItem dropItem;
                    if (count == item.Count)
                    {
                        dropItem = item;
                        RemoveItem(item);
                        Companion.Inventory[i] = null;
                        count = 0;
                    }
                    else
                    {
                        dropItem = SEnvir.CreateFreshItem(item);
                        dropItem.Count = count;
                        item.Count -= count;

                        count = item.Count;
                    }

                    Companion?.RefreshWeight();
                    dropItem.IsTemporary = true;

                    ItemObject ob = new ItemObject
                    {
                        Item = dropItem,
                    };

                    ob.Spawn(CurrentMap, cell.Location);

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i, Count = count }, Success = true });
                }
            }

            bool botter = Character.Account.ItemBot || Character.Account.GoldBot;

            if (SEnvir.Random.Next((botter ? 10 : 100)) == 0)
            {
                List<int> dropList = new List<int>();

                for (int i = 0; i < Equipment.Length; i++)
                {
                    UserItem item = Equipment[i];

                    if (item == null) continue;
                    if (!item.Info.CanDeathDrop) continue;
                    if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                    if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                    if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;


                    dropList.Add(i);

                    if (botter && dropList.Count > 0) break;

                }

                if (dropList.Count > 0)
                {
                    int index = dropList[SEnvir.Random.Next(dropList.Count)];

                    UserItem item = Equipment[index];

                    Cell cell = GetDropLocation(4, null);

                    if (cell != null)
                    {
                        UserItem dropItem;

                        dropItem = item;
                        RemoveItem(item);
                        Equipment[index] = null;

                        dropItem.IsTemporary = true;

                        ItemObject ob = new ItemObject
                        {
                            Item = dropItem,
                        };

                        ob.Spawn(CurrentMap, cell.Location);

                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = index, Count = 0 }, Success = true });

                    }
                }
            }

            RefreshWeight();
            RefreshStats();
        }

        public bool InGuild(PlayerObject player)
        {
            if (player == null) return false;

            if (Character.Account.GuildMember == null || player.Character.Account.GuildMember == null) return false;

            return Character.Account.GuildMember.Guild == player.Character.Account.GuildMember.Guild;
        }

        public void CheckBrown(MapObject ob)
        {
            //if in fight map
            PlayerObject player;
            switch (ob.Race)
            {
                case ObjectType.Player:
                    player = (PlayerObject)ob;
                    break;
                case ObjectType.Monster:
                    player = ((MonsterObject)ob).PetOwner;
                    break;
                default:
                    return;
            }

            if (player == null || player == this) return;

            switch (CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                    return;
            }


            if (InSafeZone || player.InSafeZone) return;

            switch (player.CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                    return;
            }

            if (player.Stats[Stat.Brown] > 0 || player.Stats[Stat.PKPoint] >= Config.RedPoint) return;

            if (AtWar(player)) return;

            BuffAdd(BuffType.Brown, Config.BrownDuration, new Stats { [Stat.Brown] = 1 }, false, false, TimeSpan.Zero);
        }


        public void IncreasePKPoints(int count)
        {
            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);

            if (buff != null)
                count += buff.Stats[Stat.PKPoint];

            if (count >= Config.RedPoint && !Character.BindPoint.RedZone)
            {
                SafeZoneInfo info = SEnvir.SafeZoneInfoList.Binding.FirstOrDefault(x => x.RedZone && x.ValidBindPoints.Count > 0);

                if (info != null)
                    Character.BindPoint = info;
            }


            BuffAdd(BuffType.PKPoint, TimeSpan.MaxValue, new Stats { [Stat.PKPoint] = count }, false, false, Config.PKPointTickRate);
        }

        public int GetLotusMana(ObjectType race)
        {
            if (race != ObjectType.Player) return Stats[Stat.Mana];

            int min = 0;
            int max = Stats[Stat.Mana];

            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        public int GetElementPower(ObjectType race, Stat element)
        {
            if (race != ObjectType.Player) return Stats[element];

            int min = 0;
            int max = Stats[element];

            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        #endregion

        #region Warrior Magic

        public int ShoulderDashEnd(UserMagic magic)
        {
            int distance = magic.GetPower();

            int travelled = 0;
            Cell cell;
            MapObject target = null;

            for (int d = 1; d <= distance; d++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d));

                if (cell == null) break;

                if (cell.Objects == null)
                {
                    travelled++;
                    continue;
                }

                bool blocked = false;
                bool stacked = false;
                MapObject stackedMob = null;

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        if (target == null) target = ob;

                        LevelMagic(magic);
                        continue;
                    }

                    stacked = true;
                    stackedMob = ob;
                }

                if (blocked) break;


                if (!stacked)
                {
                    travelled++;
                    continue;
                }

                if (magic.Level < 3) break; // Cannot push 2 mobs

                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d + 1));

                if (cell == null) break; // Cannot push anymore as there is a wall or couldn't push

                //Failed to push first mob because of stacking AND its not a wall so must be mob in this cell
                if (cell.Objects != null) // Could have dashed someone through door.
                    for (int c = cell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                        {
                            blocked = true;
                            break;
                        }

                        if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                        {
                            blocked = true;
                            continue;
                        }

                        if (ob.Pushed(Direction, 1) == 1)
                        {
                            LevelMagic(magic);
                            continue;
                        }

                        blocked = true;
                        break;
                    }

                if (blocked) break; // Cannot push the two targets (either by level or wall)

                //pushed 2nd space, Now need to push the first mob
                //Should be 100% success to push stackedMob as it wasn't level nor is there a wall or mob in the way.
                stackedMob.Pushed(Direction, 1); //put this here to avoid the level / chance check
                LevelMagic(magic);
                //need to check first cell again
                Point location = Functions.Move(CurrentLocation, Direction, d);
                cell = CurrentMap.Cells[location.X, location.Y];

                if (cell.Objects == null) //Might not be any more mobs on initial space after moving it
                {
                    travelled++;
                    continue;
                }

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }


                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        LevelMagic(magic);
                        continue;
                    }

                    blocked = true;
                    break;
                }

                if (blocked) break;

                travelled++;
            }

            MagicType type = magic.Info.Magic;
            if (travelled > 0 && target != null)
            {
                UserMagic assault;

                if (Magics.TryGetValue(MagicType.Assault, out assault) && Level >= assault.Info.NeedLevel1 && SEnvir.Now >= assault.Cooldown)
                {
                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Paralysis,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower()),
                        Owner = this,
                    });

                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Silenced,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower() * 2),
                        Owner = this,
                    });

                    assault.Cooldown = SEnvir.Now.AddMilliseconds(assault.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = assault.Info.Index, Delay = assault.Info.Delay });
                    type = assault.Info.Magic;
                    LevelMagic(assault);
                }
            }

            cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, travelled));

            CurrentCell = cell.GetMovement(this);

            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectDash { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Distance = travelled, Magic = type });

            ActionTime = SEnvir.Now.AddMilliseconds(300 * travelled);

            return travelled;
        }

        public void InterchangeEnd(UserMagic magic, MapObject ob)
        {
            /*  if (CurrentMap.Info.SkillDelay > 0)
              {
                  Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                  return;
              }*/

            if (ob == null || ob.CurrentMap != CurrentMap) return;


            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return;
                    break;
                case ObjectType.Monster:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= Level || !((MonsterObject)ob).MonsterInfo.CanPush) return;
                    break;
                case ObjectType.Item:
                    break;
                default:
                    return;
            }

            if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;

            Point current = CurrentLocation;

            /*  if (CurrentMap.Info.SkillDelay > 0) return;
              {
                  TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                  Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                  Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
              }*/

            Teleport(CurrentMap, ob.CurrentLocation);
            ob.Teleport(CurrentMap, current);

            if (ob.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)ob).PvPTime = SEnvir.Now;
            }


            int delay = magic.Info.Delay;
            if (SEnvir.Now <= PvPTime.AddSeconds(30))
                delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            LevelMagic(magic);
        }

        public void BeckonEnd(UserMagic magic, MapObject ob)
        {
            if (ob == null || ob.CurrentMap != CurrentMap) return;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return;

                    /* if (CurrentMap.Info.SkillDelay > 0)
                     {
                         Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                         foreach (SConnection con in Connection.Observers)
                             con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                         return;
                     }*/

                    if (SEnvir.Random.Next(10) > 4 + magic.Level) return;

                    break;
                case ObjectType.Monster:
                    if (!CanAttackTarget(ob)) return;

                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss || !mob.MonsterInfo.CanPush) return;

                    if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;
                    break;
                case ObjectType.Item:
                    if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;
                    break;
                default:
                    return;
            }

            if (!ob.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction))) return;

            /*   if (CurrentMap.Info.SkillDelay > 0)
               {
                   TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                   Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                   foreach (SConnection con in Connection.Observers)
                       con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                   UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                   Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
               }*/


            if (ob.Race != ObjectType.Item)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(ob.Race == ObjectType.Monster ? (1 + magic.Level) : 1),
                    TickCount = 1,
                });
            }

            int delay = magic.Info.Delay;
            if (SEnvir.Now <= PvPTime.AddSeconds(30))
                delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            LevelMagic(magic);
        }
        public void MassBeckonEnd(UserMagic magic)
        {
            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 9);

            foreach (MapObject ob in targets)
            {
                if (ob.Race != ObjectType.Monster) continue;

                if (!CanAttackTarget(ob)) continue;
                if (ob.Level - 10 > Level || !((MonsterObject)ob).MonsterInfo.CanPush) continue;

                if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) continue;



                if (!ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 3))) continue;

                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(1 + magic.Level),
                    TickCount = 1,
                });

                LevelMagic(magic);
            }
        }

        public void DefianceEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.Might))
            {
                BuffRemove(BuffType.Might);
                ChangeHP(-(CurrentHP / 2));
            }

            Stats buffStats = new Stats
            {
                [Stat.Defiance] = 1,
            };

            BuffAdd(BuffType.Defiance, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        public void MightEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.Defiance))
            {
                BuffRemove(BuffType.Defiance);
                ChangeHP(-(CurrentHP / 2));
            }
            int value = 4 + magic.Level * 6;

            Stats buffStats = new Stats
            {
                [Stat.MinDC] = value,
                [Stat.MaxDC] = value,
            };

            BuffAdd(BuffType.Might, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        public void EnduranceEnd(UserMagic magic)
        {
            BuffAdd(BuffType.Endurance, TimeSpan.FromSeconds(10 + magic.Level * 5), null, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }


        public void ReflectDamageEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.ReflectDamage] = 5 + magic.Level * 3,
            };

            BuffAdd(BuffType.ReflectDamage, TimeSpan.FromSeconds(15 + magic.Level * 10), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        public void FetterEnd(UserMagic magic, Cell cell)
        {
            if (cell == null || cell.Map != CurrentMap) return;

            if (cell.Objects == null) return;

            foreach (MapObject ob in cell.Objects)
            {
                if (!CanAttackTarget(ob)) continue;

                switch (ob.Race)
                {
                    case ObjectType.Monster:
                        if (ob.Level > Level + 15) continue;

                        ob.ApplyPoison(new Poison
                        {
                            Owner = this,
                            Value = (3 + magic.Level) * 2,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromSeconds(5 + magic.Level * 3),
                            Type = PoisonType.Slow,
                        });
                        break;
                }

                LevelMagic(magic);

            }

        }

        #endregion

        #region Wizard Magic

        private void RepulsionEnd(UserMagic magic, Cell cell, MirDirection direction)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level) continue;

                //CanPush check ?

                if (ob.Pushed(direction, magic.GetPower()) <= 0) continue;

                LevelMagic(magic);
                break;
            }
        }
        private void ElectricShockEnd(UserMagic magic, MonsterObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            if (ob.MonsterInfo.IsBoss) return;

            if (SEnvir.Random.Next(4 - magic.Level) > 0)
            {

                if (SEnvir.Random.Next(2) == 0) LevelMagic(magic);
                return;
            }

            LevelMagic(magic);

            if (ob.PetOwner == this)
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(magic.Level * 5 + 10);
                ob.Target = null;
                return;
            }

            if (SEnvir.Random.Next(2) > 0)
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(magic.Level * 5 + 10);
                ob.Target = null;
                return;
            }

            if (ob.Level > Level + 2 || !ob.MonsterInfo.CanTame) return;

            if (SEnvir.Random.Next(Level + 20 + magic.Level * 5) <= ob.Level + 10)
            {
                if (SEnvir.Random.Next(5) > 0 && ob.PetOwner == null)
                {
                    ob.RageTime = SEnvir.Now.AddSeconds(SEnvir.Random.Next(20) + 10);
                    ob.Target = null;
                }
                return;
            }
            if (Pets.Count >= 3) return;

            if (SEnvir.Random.Next(4) > 0) return;

            if (SEnvir.Random.Next(20) == 0)
            {
                if (ob.EXPOwner == null && ob.PetOwner == null)
                    ob.EXPOwner = this;

                ob.Die();
                return;
            }

            if (ob.PetOwner != null)
            {
                int hp = Math.Max(1, ob.Stats[Stat.Health] / 10);

                if (hp < ob.CurrentHP) ob.SetHP(hp);

                ob.PetOwner.Pets.Remove(ob);
                ob.PetOwner = null;
                ob.Magics.Clear();
            }
            else if (ob.SpawnInfo != null)
            {
                ob.SpawnInfo.AliveCount--;
                ob.SpawnInfo = null;
            }

            ob.PetOwner = this;
            Pets.Add(ob);

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;

            ob.TameTime = SEnvir.Now.AddHours(magic.Level + 1);
            ob.Target = null;
            ob.RageTime = DateTime.MinValue;
            ob.ShockTime = DateTime.MinValue;
            ob.Magics.Add(magic);
            ob.SummonLevel = magic.Level;
            ob.RefreshStats();

            ob.Broadcast(new S.ObjectPetOwnerChanged { ObjectID = ob.ObjectID, PetOwner = Name });
        }
        private void ExpelUndeadEnd(UserMagic magic, MonsterObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || ob.MonsterInfo.IsBoss || ob.Level >= 70) return;

            if (ob.Target == null && ob.CanAttackTarget(this))
                ob.Target = this;

            if (ob.Level >= Level - 1 + SEnvir.Random.Next(4)) return;

            if (SEnvir.Random.Next(100) >= 35 + magic.Level * 9 + (Level - ob.Level) * 5 + Stats[Stat.PhantomAttack] / 2) return;

            if (ob.EXPOwner == null && ob.Master == null)
                ob.EXPOwner = this;

            ob.SetHP(0);

            LevelMagic(magic);
        }
        private void TeleportationEnd(UserMagic magic)
        {
            if (CurrentMap.Info.SkillDelay > 0)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                return;
            }

            if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;
            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay * 3);

                Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Teleport(CurrentMap, CurrentMap.GetRandomLocation());
            LevelMagic(magic);

        }
        private void FireWallEnd(UserMagic magic, Cell cell, int power)
        {
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = this,
                Effect = SpellEffect.FireWall,
                Magic = magic,
            };

            ob.Spawn(cell.Map, cell.Location);

            LevelMagic(magic);
        }

        public void GeoManipulationEnd(UserMagic magic, Point location)
        {
            /* if (CurrentMap.Info.SkillDelay > 0)
             {
                 Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                 foreach (SConnection con in Connection.Observers)
                     con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                 return;
             }*/

            if (location == CurrentLocation) return;

            if (SEnvir.Random.Next(100) > 25 + magic.Level * 25) return;

            if (!Teleport(CurrentMap, location, false)) return;
            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            LevelMagic(magic);

            int delay = magic.Info.Delay;
            if (SEnvir.Now <= PvPTime.AddSeconds(30))
                delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });
        }

        public void MagicShieldEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.MagicShield)) return;

            Stats buffStats = new Stats
            {
                [Stat.MagicShield] = 50
            };

            BuffAdd(BuffType.MagicShield, TimeSpan.FromSeconds(30 + magic.Level * 20 + GetMC() / 2 + Stats[Stat.PhantomAttack] * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void FrostBiteEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.FrostBite)) return;

            Stats buffStats = new Stats
            {
                [Stat.FrostBiteDamage] = GetMC() + Stats[Stat.IceAttack] * 2 + magic.GetPower(),
                [Stat.FrostBiteMaxDamage] = Stats[Stat.MaxMC] * 50 + Stats[Stat.IceAttack] * 70,
            };

            BuffAdd(BuffType.FrostBite, TimeSpan.FromSeconds(3 + magic.Level * 3), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        public void RenounceEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.HealthPercent] = -(1 + magic.Level) * 10,
                [Stat.MCPercent] = (1 + magic.Level) * 10,
            };

            int health = CurrentHP;

            BuffInfo buff = BuffAdd(BuffType.Renounce, TimeSpan.FromSeconds(30 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);


            buff.Stats[Stat.RenounceHPLost] = health - CurrentHP;
            Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });


            LevelMagic(magic);
        }
        public void JudgementOfHeavenEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.JudgementOfHeaven] = (2 + magic.Level) * 20,
            };

            BuffAdd(BuffType.JudgementOfHeaven, TimeSpan.FromSeconds(30 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        private void ChainLightningEnd(List<UserMagic> magics, Cell cell, int extra)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob)) continue;

                MagicAttack(magics, ob, true, null, extra);
            }
        }

        private void TempestEnd(UserMagic magic, Cell cell, int power)
        {
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = this,
                Effect = SpellEffect.Tempest,
                Magic = magic,
            };

            ob.Spawn(cell.Map, cell.Location);

            LevelMagic(magic);
        }

        #endregion

        #region Taoist Magic

        public void HealEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.CurrentHP >= ob.Stats[Stat.Health] || ob.Buffs.Any(x => x.Type == BuffType.Heal)) return;

            UserMagic empowered;
            int bonus = 0;
            int cap = 30;

            if (Magics.TryGetValue(MagicType.EmpoweredHealing, out empowered) && Level >= empowered.Info.NeedLevel1)
            {
                bonus = empowered.GetPower();
                cap += (1 + empowered.Level) * 30;

                LevelMagic(empowered);
            }

            Stats buffStats = new Stats
            {
                [Stat.Healing] = magic.GetPower() + GetSC() + Stats[Stat.HolyAttack] * 2 + bonus,
                [Stat.HealingCap] = cap, // empowered healing
            };

            ob.BuffAdd(BuffType.Heal, TimeSpan.FromSeconds(buffStats[Stat.Healing] / buffStats[Stat.HealingCap]), buffStats, false, false, TimeSpan.FromSeconds(1));
            LevelMagic(magic);
        }
        public void PoisonDustEnd(List<UserMagic> magics, MapObject ob, PoisonType type)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            UserMagic magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.PoisonDust);
            if (magic == null) return;

            for (int i = Pets.Count - 1; i >= 0; i--)
                if (Pets[i].Target == null)
                    Pets[i].Target = ob;


            int duration = magic.GetPower() + GetSC() + Stats[Stat.DarkAttack] * 2;

            ob.ApplyPoison(new Poison
            {
                Value = magic.Level + 1 + Level / 14,
                Type = type,
                Owner = this,
                TickCount = duration / 2,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            foreach (UserMagic mag in magics)
                LevelMagic(mag);
        }
        public void InvisibilityEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Invisibility)) return;

            Stats buffStats = new Stats
            {
                [Stat.Invisibility] = 1
            };

            ob.BuffAdd(BuffType.Invisibility, TimeSpan.FromSeconds((magic.GetPower() + GetSC() + Stats[Stat.PhantomAttack] * 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void LifeStealEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.LifeSteal] = 4 + (magic.Level) * 2
            };

            ob.BuffAdd(BuffType.LifeSteal, TimeSpan.FromSeconds(magic.GetPower() + GetSC() + Stats[Stat.DarkAttack] * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void StrengthOfFaithEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.DCPercent] = (magic.Level + 1) * -20,
                [Stat.PetDCPercent] = (magic.Level + 1) * 30,
            };

            BuffAdd(BuffType.StrengthOfFaith, TimeSpan.FromSeconds(magic.GetPower()), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void TransparencyEnd(UserMagic magic, MapObject ob, Point location)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Transparency)) return;

            Teleport(CurrentMap, location, false);

            int delay = magic.Info.Delay;
            if (SEnvir.Now <= PvPTime.AddSeconds(30))
                delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });


            Stats buffStats = new Stats
            {
                [Stat.Transparency] = 1
            };

            ob.BuffAdd(BuffType.Transparency, TimeSpan.FromSeconds(Math.Min(SEnvir.Now <= PvPTime.AddSeconds(30) ? 20 : 3600, magic.GetPower() + GetSC() / 2 + Stats[Stat.PhantomAttack] * 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void CelestialLightEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.CelestialLight)) return;

            Stats buffStats = new Stats
            {
                [Stat.CelestialLight] = (magic.Level + 1) * 10,
            };

            BuffAdd(BuffType.CelestialLight, TimeSpan.FromSeconds(magic.GetPower()), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void MagicResistanceEnd(UserMagic magic, MapObject ob, Stats stats)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;


            Stats buffStats = new Stats
            {
                [Stat.MaxMR] = 5 + magic.Level
            };

            if (stats[Stat.FireAffinity] > 0)
            {
                buffStats[Stat.FireResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.IceAffinity] > 0)
            {
                buffStats[Stat.IceResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.LightningAffinity] > 0)
            {
                buffStats[Stat.LightningResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.WindAffinity] > 0)
            {
                buffStats[Stat.WindResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.HolyAffinity] > 0)
            {
                buffStats[Stat.HolyResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.DarkAffinity] > 0)
            {
                buffStats[Stat.DarkResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.PhantomAffinity] > 0)
            {
                buffStats[Stat.PhantomResistance] = 1;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }


            ob.BuffAdd(BuffType.MagicResistance, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void ResilienceEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.MaxAC] = 5 + magic.Level,
                [Stat.PhysicalResistance] = 1,
            };

            ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds((magic.GetPower() + GetSC() * 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void ElementalSuperiorityEnd(UserMagic magic, MapObject ob, Stats stats)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;


            Stats buffStats = new Stats
            {
                [Stat.MaxMC] = 5 + magic.Level,
                [Stat.MaxSC] = 5 + magic.Level
            };

            if (stats[Stat.FireAffinity] > 0)
            {
                buffStats[Stat.FireAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.IceAffinity] > 0)
            {
                buffStats[Stat.IceAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.LightningAffinity] > 0)
            {
                buffStats[Stat.LightningAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.WindAffinity] > 0)
            {
                buffStats[Stat.WindAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.HolyAffinity] > 0)
            {
                buffStats[Stat.HolyAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.DarkAffinity] > 0)
            {
                buffStats[Stat.DarkAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.PhantomAffinity] > 0)
            {
                buffStats[Stat.PhantomAttack] = 5 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }


            ob.BuffAdd(BuffType.ElementalSuperiority, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void BloodLustEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.MaxDC] = 5 + magic.Level
            };

            ob.BuffAdd(BuffType.BloodLust, TimeSpan.FromSeconds((magic.GetPower() + GetSC() * 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        public void PurificationEnd(List<UserMagic> magics, MapObject ob)
        {
            if (ob?.Node == null || magics.Count == 0) return;

            UserMagic magic = magics[0];
            if (SEnvir.Random.Next(100) > 40 + magic.Level * 20) return;

            int result = Purify(ob);

            for (int i = 0; i < result; i++)
                foreach (UserMagic m in magics)
                    LevelMagic(m);
        }
        public void DemonExplosionEnd(UserMagic magic, Stats stats)
        {
            MonsterObject pet = Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;


            int damage = pet.Stats[Stat.Health];
            pet.Broadcast(new S.ObjectEffect { Effect = Effect.DemonExplosion, ObjectID = pet.ObjectID });

            List<MapObject> targets = GetTargets(pet.CurrentMap, pet.CurrentLocation, 2);

            pet.ChangeHP(-damage * 75 / 100);

            int damagePvE = damage * magic.GetPower() / 100 + GetSC() * 3;
            int damagePvP = damage * magic.GetPower() / 100 + GetSC() * 3;

            if (stats != null && stats.GetAffinityValue(Element.Phantom) > 0)
            {
                damagePvE += GetElementPower(ObjectType.Monster, Stat.PhantomAttack) * 8;
                damagePvP += GetElementPower(ObjectType.Player, Stat.PhantomAttack) * 8;
            }


            foreach (MapObject target in targets)
            {

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(800),
                    ActionType.DelayedMagicDamage,
                    new List<UserMagic> { magic },
                    target,
                    true,
                    null,
                    target.Race == ObjectType.Player ? damagePvP : damagePvE));
            }
        }
        public void DemonicRecoveryEnd(UserMagic magic)
        {
            MonsterObject pet = Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;

            int health = pet.Stats[Stat.Health] * magic.GetPower() / 100;

            pet.ChangeHP(health);

            LevelMagic(magic);
        }

        public void ResurrectionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !ob.Dead) return;

            if (SEnvir.Random.Next(100) > 25 + magic.Level * 25) return;

            int power = magic.GetPower();

            ob.Dead = false;
            ob.SetHP(ob.Stats[Stat.Health] * power / 100);
            ob.SetMP(ob.Stats[Stat.Mana] * power / 100);

            Broadcast(new S.ObjectRevive { ObjectID = ob.ObjectID, Location = ob.CurrentLocation, Effect = false });

            LevelMagic(magic);

            magic.Cooldown = SEnvir.Now.AddSeconds(20);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 20000 });
        }

        public void InfectionEnd(List<UserMagic> magics, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || (ob.Poison & PoisonType.Infection) == PoisonType.Infection) return;

            UserMagic magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.Infection);
            if (magic == null) return;

            ob.ApplyPoison(new Poison
            {
                Value = GetSC() + Stats[Stat.CriticalChance] + Stats[Stat.CriticalDamage],
                Type = PoisonType.Infection,
                Owner = this,
                TickCount = 10 + magic.Level * 10,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            foreach (UserMagic mag in magics)
                LevelMagic(mag);
        }
        public void TrapOctagonEnd(UserMagic magic, Map map, Point location)
        {
            if (map != CurrentMap) return;

            List<MapObject> targets = GetTargets(CurrentMap, location, 1);

            List<MapObject> trappedMonsters = new List<MapObject>();

            foreach (MapObject target in targets)
            {
                if (target.Race != ObjectType.Monster || target.Level >= Level + SEnvir.Random.Next(3)) continue;

                trappedMonsters.Add((MonsterObject)target);
            }

            if (trappedMonsters.Count == 0) return;

            int duration = GetSC() + magic.GetPower();

            List<Point> locationList = new List<Point>
            {
                new Point(location.X - 1, location.Y - 2),
                new Point(location.X - 1, location.Y + 2),
                new Point(location.X + 1, location.Y - 2),
                new Point(location.X + 1, location.Y + 2),

                new Point(location.X - 2, location.Y - 1),
                new Point(location.X - 2, location.Y + 1),
                new Point(location.X + 2, location.Y - 1),
                new Point(location.X + 2, location.Y + 1)
            };


            foreach (Point point in locationList)
            {
                SpellObject ob = new SpellObject
                {
                    DisplayLocation = point,
                    TickCount = duration * 4, //Checking every 1/4 of a second to see if all monsters were disturbed.
                    TickFrequency = TimeSpan.FromMilliseconds(250),
                    Owner = this,
                    Effect = SpellEffect.TrapOctagon,
                    Magic = magic,
                    Targets = trappedMonsters,
                };

                ob.Spawn(map, point);
            }

            DateTime shockTime = SEnvir.Now.AddSeconds(duration);
            foreach (MonsterObject monster in trappedMonsters)
            {
                if (shockTime <= monster.ShockTime) continue;

                monster.ShockTime = SEnvir.Now.AddSeconds(duration);
                LevelMagic(magic);
            }
        }

        private void TaoistCombatKick(UserMagic magic, Cell cell, MirDirection direction)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level) continue;

                //CanPush check ?

                if (ob.Pushed(direction, magic.GetPower()) <= 0) continue;

                Attack(ob, new List<UserMagic> { magic }, true, 0);
                LevelMagic(magic);
                break;
            }
        }

        public void SummonEnd(UserMagic magic, Map map, Point location, MonsterInfo info)
        {
            if (info == null) return;

            MonsterObject ob = Pets.FirstOrDefault(x => x.MonsterInfo == info);

            if (ob != null)
            {
                ob.PetRecall();
                return;
            }

            if (Pets.Count >= 2) return;

            ob = MonsterObject.GetMonster(info);

            ob.PetOwner = this;
            Pets.Add(ob);

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;
            ob.Magics.Add(magic);
            ob.SummonLevel = magic.Level * 2;
            ob.TameTime = SEnvir.Now.AddDays(365);

            if (Buffs.Any(x => x.Type == BuffType.StrengthOfFaith))
                ob.Magics.Add(Magics[MagicType.StrengthOfFaith]);


            if (magic.Info.Magic == MagicType.SummonDemonicCreature && Magics.TryGetValue(MagicType.DemonicRecovery, out UserMagic demonRecovery))
                ob.Magics.Add(demonRecovery);


            Cell cell = map.GetCell(location);

            if (cell == null || cell.Movements != null || !ob.Spawn(map, location))
                ob.Spawn(CurrentMap, CurrentLocation);

            ob.SetHP(ob.Stats[Stat.Health]);

            LevelMagic(magic);
        }
        #endregion

        #region Assassin Magic

        public void PoisonousCloudEnd(UserMagic magic)
        {
            if (CurrentCell.Objects.FirstOrDefault(x => x.Race == ObjectType.Spell && ((SpellObject)x).Effect == SpellEffect.PoisonousCloud) != null) return;

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

            int duration = magic.GetPower();
            foreach (Cell cell in cells)
            {
                SpellObject ob = new SpellObject
                {
                    Visible = cell == CurrentCell,
                    DisplayLocation = CurrentLocation,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(duration),
                    Owner = this,
                    Effect = SpellEffect.PoisonousCloud,
                    Power = 5,
                };

                ob.Spawn(CurrentMap, cell.Location);
            }

            LevelMagic(magic);

        }

        public void CloakEnd(UserMagic magic, MapObject ob, bool forceGhost)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Cloak)) return;

            UserMagic pledgeofBlood = Magics.ContainsKey(MagicType.PledgeOfBlood) ? Magics[MagicType.PledgeOfBlood] : null;


            int value = 0;
            if (pledgeofBlood != null && Level >= pledgeofBlood.Info.NeedLevel1)
                value = pledgeofBlood.GetPower();

            Stats buffStats = new Stats
            {
                [Stat.Cloak] = 1,
                [Stat.CloakDamage] = Stats[Stat.Health] * (20 - magic.Level - value) / 1000,
            };

            ob.BuffAdd(BuffType.Cloak, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.FromSeconds(2));


            LevelMagic(magic);
            LevelMagic(pledgeofBlood);
            if (!forceGhost)
            {
                UserMagic ghostWalk = Magics.ContainsKey(MagicType.GhostWalk) ? Magics[MagicType.GhostWalk] : null;
                if (ghostWalk == null || Level < ghostWalk.Info.NeedLevel1) return;

                int rate = (ghostWalk.Level + 1) * 3;

                if (SEnvir.Random.Next(2 + rate) >= rate) return;

                LevelMagic(ghostWalk);
            }
            ob.BuffAdd(BuffType.GhostWalk, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);

        }

        public void RakeEnd(UserMagic magic, Cell cell)
        {
            if (cell?.Objects == null) return;


            foreach (MapObject ob in cell.Objects)
                if (MagicAttack(new List<UserMagic> { magic }, ob, true) > 0) break;
        }
        public void WraithGripEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            int power = GetSP();

            int duration = magic.GetPower();

            UserMagic touch = null;

            if (Magics.TryGetValue(MagicType.TouchOfTheDeparted, out touch) && Level < magic.Info.NeedLevel1)
                touch = null;

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.WraithGrip,
                Owner = this,
                TickCount = ob.Race == ObjectType.Player ? duration * 7 / 10 : duration,
                TickFrequency = TimeSpan.FromSeconds(1),
                Extra = touch,
            });

            if (touch != null)
                ob.ApplyPoison(new Poison
                {
                    Value = power,
                    Type = PoisonType.Paralysis,

                    Owner = this,
                    TickCount = ob.Race == ObjectType.Player ? duration * 3 / 10 : duration,
                    TickFrequency = TimeSpan.FromSeconds(1),
                });

            LevelMagic(magic);
            LevelMagic(touch);
        }
        public void AbyssEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || (ob.Poison & PoisonType.Abyss) == PoisonType.Abyss) return;

            int power = GetSP();

            int duration = (magic.Level + 3) * 2;

            if (ob.Race == ObjectType.Monster)
                duration *= 2;

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.Abyss,
                Owner = this,
                TickCount = duration,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            if (ob.Race == ObjectType.Monster)
                ((MonsterObject)ob).Target = null;

            LevelMagic(magic);
        }

        public void HellFireEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            if (MagicAttack(new List<UserMagic> { magic }, ob, true) <= 0) return;


            int power = Math.Min(GetSC(), GetMC()) / 2;

            int duration = magic.GetPower();

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.HellFire,
                Owner = this,
                TickCount = duration / 2,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            LevelMagic(magic);
        }

        public void TheNewBeginningEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.TheNewBeginning] = Math.Min(magic.Level + 2, Stats[Stat.TheNewBeginning] + 1)
            };

            ob.BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), buffStats, false, false, TimeSpan.Zero);
        }

        public void SummonPuppetEnd(UserMagic magic, MapObject ob)
        {
            /*  if (CurrentMap.Info.SkillDelay > 0)
              {
                  Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                  return;
              }*/

            if (ob?.Node == null || !CanHelpTarget(ob)) return;


            List<UserMagic> magics = new List<UserMagic> { magic };

            UserMagic augMagic;

            //Summon Puppets.

            int count = magic.Level + 1;

            if (Magics.TryGetValue(MagicType.ElementalPuppet, out augMagic) && Level < augMagic.Info.NeedLevel1)
                augMagic = null;


            Stats darkstoneStats = new Stats();
            if (augMagic != null)
            {
                if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
                    darkstoneStats = Equipment[(int)EquipmentSlot.Amulet].Info.Stats;

                DamageDarkStone(10);

                magics.Add(augMagic);
            }

            if (Magics.TryGetValue(MagicType.ArtOfShadows, out augMagic) && Level < augMagic.Info.NeedLevel1)
                augMagic = null;

            int range = 1;
            if (augMagic != null)
            {
                count += augMagic.GetPower();
                range = 3;

                magics.Add(augMagic);
            }

            for (int i = 0; i < count; i++)
            {
                Puppet mob = new Puppet
                {
                    MonsterInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.SummonPuppet),
                    Player = this,
                    DarkStoneStats = darkstoneStats,
                    Direction = Direction,
                    TameTime = SEnvir.Now.AddDays(365)
                };

                foreach (UserMagic m in magics)
                    mob.Magics.Add(m);

                if (mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, range)))
                {
                    Pets.Add(mob);
                    mob.PetOwner = this;
                }
            }

            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Cell cell = CurrentMap.GetCell(CurrentMap.GetRandomLocation(CurrentLocation, 4));

            if (cell != null) CurrentCell = cell;

            CloakEnd(magic, ob, true);

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public void DanceOfSwallowEnd(UserMagic magic, MapObject ob)
        {
            /*if (CurrentMap.Info.SkillDelay > 0)
            {
                Connection.ReceiveChat(string.Format(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                return;
            }
            */
            if (!CanAttackTarget(ob))
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
            Cell cell = null;
            for (int i = 0; i < 8; i++)
            {
                cell = CurrentMap.GetCell(Functions.Move(ob.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                if (cell == null || cell.IsBlocking(this, false) || cell.Movements != null)
                {
                    cell = null;
                    continue;
                }
                break;
            }

            if (cell == null)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            /* if (CurrentMap.Info.SkillDelay > 0)
             {
                 TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                 Connection.ReceiveChat(string.Format(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                 foreach (SConnection con in Connection.Observers)
                     con.ReceiveChat(string.Format(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                 UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                 Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
             }*/

            PreventSpellCheck = true;
            CurrentCell = cell;
            PreventSpellCheck = false;


            Direction = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);

            CombatTime = SEnvir.Now;

            if (Stats[Stat.Comfort] < 15)
                RegenTime = SEnvir.Now + RegenDelay;
            ActionTime = SEnvir.Now + Globals.AttackTime;

            int aspeed = Stats[Stat.AttackSpeed];
            int attackDelay = Globals.AttackDelay - aspeed * Globals.ASpeedRate;
            attackDelay = Math.Max(800, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);


            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, AttackMagic = magic.Info.Magic });

            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(400), ActionType.DelayAttack,
                ob,
                new List<UserMagic> { magic },
                true,
                0));

            int delay = magic.Info.Delay;
            if (SEnvir.Now <= PvPTime.AddSeconds(30))
                delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });
        }

        public void DarkConversionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.DarkConversion)) return;

            Stats buffStats = new Stats
            {
                [Stat.DarkConversion] = magic.GetPower(),
            };

            ob.BuffAdd(BuffType.DarkConversion, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.FromSeconds(2));
        }

        public void EvasionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.EvasionChance] = 4 + magic.Level * 2,
            };

            ob.BuffAdd(BuffType.Evasion, TimeSpan.FromSeconds(magic.GetPower()), buffStats, false, false, TimeSpan.Zero);
        }
        public void RagingWindEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            ob.BuffAdd(BuffType.RagingWind, TimeSpan.FromSeconds(magic.GetPower()), null, false, false, TimeSpan.Zero);
        }


        #endregion




        public void Enqueue(Packet p) => Connection.Enqueue(p);
        private StartInformation GetStartInformation()
        {
            List<ClientBeltLink> blinks = new List<ClientBeltLink>();

            foreach (CharacterBeltLink link in Character.BeltLinks)
            {
                if (link.LinkItemIndex > 0 && Inventory.FirstOrDefault(x => x?.Index == link.LinkItemIndex) == null)
                    link.LinkItemIndex = -1;

                blinks.Add(link.ToClientInfo());
            }

            List<ClientAutoPotionLink> alinks = new List<ClientAutoPotionLink>();

            foreach (AutoPotionLink link in Character.AutoPotionLinks)
                alinks.Add(link.ToClientInfo());

            return new StartInformation
            {
                Index = Character.Index,
                ObjectID = ObjectID,
                Name = Name,
                GuildName = Character.Account.GuildMember?.Guild.GuildName,
                GuildRank = Character.Account.GuildMember?.Rank,
                NameColour = NameColour,

                Level = Level,
                Class = Class,
                Gender = Gender,
                Location = CurrentLocation,
                Direction = Direction,

                MapIndex = CurrentMap.Info.Index,
                InstanceIndex = CurrentMap.Instance?.Index ?? -1,

                Gold = Gold,
                GameGold = 0,

                HairType = HairType,
                HairColour = HairColour,

                Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info.Shape ?? -1,

                Shield = Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? -1,

                Armour = Equipment[(int)EquipmentSlot.Armour]?.Info.Shape ?? 0,
                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty,
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info.Image ?? 0,

                EmblemShape = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? 0,
                WingsShape = Equipment[(int)EquipmentSlot.Wings]?.Info.Shape ?? 0,

                Experience = Experience,

                DayTime = SEnvir.DayTime,
                AllowGroup = Character.Account.AllowGroup,

                CurrentHP = DisplayHP,
                CurrentMP = DisplayMP,

                AttackMode = AttackMode,
                PetMode = PetMode,

                Items = Character.Items.Select(x => x.ToClientInfo()).ToList(),
                BeltLinks = blinks,
                AutoPotionLinks = alinks,
                Magics = Character.Magics.Select(X => X.ToClientInfo()).ToList(),
                Buffs = Buffs.Select(X => X.ToClientInfo()).ToList(),

                Poison = Poison,

                InSafeZone = InSafeZone,

                Observable = Character.Observable,
                HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints),

                Dead = Dead,

                Horse = Horse,

                HelmetShape = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,

                HorseShape = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,

                Quests = Character.Quests.Select(x => x.ToClientInfo()).ToList(),

                CompanionUnlocks = Character.Account.CompanionUnlocks.Select(x => x.CompanionInfo.Index).ToList(),

                Companions = Character.Account.Companions.Select(x => x.ToClientInfo()).ToList(),

                Companion = Character.Companion?.Index ?? 0,

                StorageSize = Character.Account.StorageSize,

                FiltersClass = Character.FiltersClass,
                FiltersRarity = Character.FiltersRarity,
                FiltersItemType = Character.FiltersItemType,
            };
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            if (ob == this) return null;

            return new S.ObjectPlayer
            {
                Index = Character.Index,

                ObjectID = ObjectID,
                Name = Name,
                GuildName = Character.Account.GuildMember?.Guild.GuildName,
                NameColour = NameColour,
                Location = CurrentLocation,
                Direction = Direction,

                Light = Stats[Stat.Light],
                Dead = Dead,

                Class = Class,
                Gender = Gender,
                HairType = HairType,
                HairColour = HairColour,

                //TODO HElmet
                Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info.Shape ?? -1,

                Shield = Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? -1,

                Armour = Equipment[(int)EquipmentSlot.Armour]?.Info.Shape ?? 0,
                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty,
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info.Image ?? 0,
                EmblemShape = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? 0,
                Wings = Equipment[(int)EquipmentSlot.Wings]?.Info.Shape ?? 0,

                Poison = Poison,

                Buffs = Character.Buffs.Where(x => x.Visible).Select(x => x.Type).ToList(),

                Horse = Horse,

                Helmet = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,

                HorseShape = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,

                FiltersClass = Character.FiltersClass,
                FiltersItemType = Character.FiltersItemType,
                FiltersRarity = Character.FiltersRarity,
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)
        {
            return new S.DataObjectPlayer
            {
                ObjectID = ObjectID,

                MapIndex = CurrentMap.Info.Index,
                CurrentLocation = CurrentLocation,

                Name = Name,

                Health = DisplayHP,
                MaxHealth = Stats[Stat.Health],
                Dead = Dead,

                Mana = DisplayMP,
                MaxMana = Stats[Stat.Mana]
            };
        }

        public void SendShapeUpdate()
        {
            S.PlayerUpdate p = new S.PlayerUpdate
            {
                ObjectID = ObjectID,

                Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info.Shape ?? -1,

                Shield = Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? -1,

                Armour = Equipment[(int)EquipmentSlot.Armour]?.Info.Shape ?? 0,
                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty,
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info.Image ?? 0,
                EmblemShape = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? 0,
                WingsShape = Equipment[(int)EquipmentSlot.Wings]?.Info.Shape ?? 0,

                Helmet = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,

                HorseArmour = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,
                //Todo Helmet

                Light = Stats[Stat.Light]
            };

            Broadcast(p);
        }

        public void SendChangeUpdate()
        {
            S.PlayerChangeUpdate p = new S.PlayerChangeUpdate
            {
                ObjectID = ObjectID,

                Name = Name,

                Gender = Gender,
                HairType = HairType,
                HairColour = HairColour,
                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty,
            };

            Broadcast(p);
        }
        public void NPCAccessoryRefine(C.NPCAccessoryRefine p)
        {
            Enqueue(new S.NPCAccessoryRefine { Target = p.Target, OreTarget = p.OreTarget, Links = p.Links });

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryRefine) return;

            if (!ParseLinks(p.Target)) return;

            if (Gold < 50000) return;

            UserItem[] targetArray = null;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:
                    targetArray = Equipment;
                    break;
                case GridType.Storage:
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Target.Slot < 0 || p.Target.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Target.Slot];

            if (targetItem == null || p.Target.Count > targetItem.Count) return; //Already Leveled.
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            if (targetItem.Level > 1) return; //No refine on levelled items

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return; //only refined accessories
            }

            UserItem[] targetOreArray = null;

            switch (p.OreTarget.GridType)
            {
                case GridType.Inventory:
                    targetOreArray = Inventory;
                    break;
                case GridType.Equipment:
                    targetOreArray = Equipment;
                    break;
                case GridType.Storage:
                    targetOreArray = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return;

                    targetOreArray = Companion.Inventory;
                    break;
                default:
                    return;
            }
            if (p.OreTarget.Slot < 0 || p.OreTarget.Slot >= targetArray.Length) return;
            UserItem oretargetItem = targetOreArray[p.OreTarget.Slot];

            S.ItemsChanged result = new S.ItemsChanged { Links = new List<CellLinkInfo>(), Success = true };
            Enqueue(result);

            foreach (CellLinkInfo link in p.Links) //loop through to check level and added stats on each material vs target
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) continue;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        continue;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem refItem = fromArray[link.Slot];

                if (refItem.Level > 1) return; //if material is levelled dont refine
                if (targetItem.AddedStats.Count != refItem.AddedStats.Count) return; //if material has different amount of added stats to target dont refine

                if (targetItem.AddedStats.Count > 1) //if target has added stats loop through to check material has same stats
                {
                    int count = 0;
                    foreach (UserItemStat addStat in targetItem.AddedStats)
                    {
                        foreach (UserItemStat raddStat in refItem.AddedStats)
                        {
                            if (addStat.Stat == raddStat.Stat && addStat.StatSource == raddStat.StatSource && addStat.Amount == raddStat.Amount)
                            {
                                count++;
                            }
                        }
                    }
                    if (count != targetItem.AddedStats.Count) return;

                }

            }

            foreach (CellLinkInfo link in p.Links) //now loop through to remove materials
            {

                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.Storage:
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) continue;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        continue;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) continue;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) continue;
                if (item.Info != targetItem.Info) continue;
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (targetItem.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) continue;

                result.Links.Add(link);

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;

            }

            int chance = 100 - (oretargetItem.CurrentDurability / 1000);
            int success = 30;
            if (targetItem.Info.Rarity != Rarity.Common)
            {
                success = 40;
            }
            if (SEnvir.Random.Next(chance) < success)
            #region refineworked
            {

                S.ItemAcessoryRefined presult = new S.ItemAcessoryRefined { GridType = p.Target.GridType, Slot = p.Target.Slot, NewStats = new Stats() };
                Enqueue(presult);
                int amount = 1;
                if (SEnvir.Random.Next(chance) == 0)
                {
                    amount = 2;
                }
                switch (p.RefineType)
                {

                    case RefineType.DC:
                        targetItem.AddStat(Stat.MaxDC, amount, StatSource.Added);
                        presult.NewStats[Stat.MaxDC] = amount;
                        break;
                    case RefineType.SpellPower:
                        if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                        {
                            targetItem.AddStat(Stat.MaxMC, amount, StatSource.Added);
                            presult.NewStats[Stat.MaxMC] = amount;

                            targetItem.AddStat(Stat.MaxSC, amount, StatSource.Added);
                            presult.NewStats[Stat.MaxSC] = amount;
                        }

                        if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                        {
                            targetItem.AddStat(Stat.MaxMC, amount, StatSource.Added);
                            presult.NewStats[Stat.MaxMC] = amount;
                        }

                        if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                        {
                            targetItem.AddStat(Stat.MaxSC, amount, StatSource.Added);
                            presult.NewStats[Stat.MaxSC] = amount;
                        }
                        break;
                    case RefineType.Health:
                        amount *= 10;
                        targetItem.AddStat(Stat.Health, amount, StatSource.Added);
                        presult.NewStats[Stat.Health] = amount;
                        break;
                    case RefineType.Mana:
                        amount *= 10;
                        targetItem.AddStat(Stat.Mana, amount, StatSource.Added);
                        presult.NewStats[Stat.Mana] = amount;
                        break;
                    case RefineType.DCPercent:
                        targetItem.AddStat(Stat.DCPercent, amount, StatSource.Added);
                        presult.NewStats[Stat.DCPercent] = amount;
                        break;
                    case RefineType.SPPercent:
                        if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                        {
                            targetItem.AddStat(Stat.MCPercent, amount, StatSource.Added);
                            presult.NewStats[Stat.MCPercent] = amount;

                            targetItem.AddStat(Stat.SCPercent, amount, StatSource.Added);
                            presult.NewStats[Stat.SCPercent] = amount;
                        }

                        if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                        {
                            targetItem.AddStat(Stat.MCPercent, amount, StatSource.Added);
                            presult.NewStats[Stat.MCPercent] = amount;
                        }

                        if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                        {
                            targetItem.AddStat(Stat.SCPercent, amount, StatSource.Added);
                            presult.NewStats[Stat.SCPercent] = amount;
                        }
                        break;
                    case RefineType.HealthPercent:
                        targetItem.AddStat(Stat.HealthPercent, amount, StatSource.Added);
                        presult.NewStats[Stat.HealthPercent] = amount;
                        break;
                    case RefineType.ManaPercent:
                        targetItem.AddStat(Stat.ManaPercent, amount, StatSource.Added);
                        presult.NewStats[Stat.ManaPercent] = amount;
                        break;
                    case RefineType.Fire:
                        targetItem.AddStat(Stat.FireAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.FireAttack] = amount;
                        break;
                    case RefineType.Ice:
                        targetItem.AddStat(Stat.IceAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.IceAttack] = amount;
                        break;
                    case RefineType.Lightning:
                        targetItem.AddStat(Stat.LightningAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.LightningAttack] = amount;
                        break;
                    case RefineType.Wind:
                        targetItem.AddStat(Stat.WindAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.WindAttack] = amount;
                        break;
                    case RefineType.Holy:
                        targetItem.AddStat(Stat.HolyAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.HolyAttack] = amount;
                        break;
                    case RefineType.Dark:
                        targetItem.AddStat(Stat.DarkAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.DarkAttack] = amount;
                        break;
                    case RefineType.Phantom:
                        targetItem.AddStat(Stat.PhantomAttack, amount, StatSource.Added);
                        presult.NewStats[Stat.PhantomAttack] = amount;
                        break;
                    case RefineType.AC:
                        targetItem.AddStat(Stat.MaxAC, amount, StatSource.Added);
                        presult.NewStats[Stat.MaxAC] = amount;
                        break;
                    case RefineType.MR:
                        targetItem.AddStat(Stat.MaxMR, amount, StatSource.Added);
                        presult.NewStats[Stat.MaxMR] = amount;
                        break;
                    case RefineType.Accuracy:
                        targetItem.AddStat(Stat.Accuracy, amount, StatSource.Added);
                        presult.NewStats[Stat.Accuracy] = amount;
                        break;
                    case RefineType.Agility:
                        targetItem.AddStat(Stat.Agility, amount, StatSource.Added);
                        presult.NewStats[Stat.Agility] = amount;
                        break;
                    default:
                        Character.Account.Banned = true;
                        Character.Account.BanReason = "Attempted to Exploit refine, Accessory Refine Type.";
                        Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                        return;
                }
                targetItem.StatsChanged();
                Connection.ReceiveChat(string.Format(Connection.Language.AccessoryRefineSuccess, targetItem.Info.ItemName, p.RefineType, amount), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.AccessoryRefineSuccess, targetItem.Info.ItemName, p.RefineType, amount), MessageType.System);
                RefreshStats();
            }
            #endregion
            else
            {
                Connection.ReceiveChat(string.Format(Connection.Language.AccessoryRefineFailed, targetItem.Info.ItemName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat(string.Format(con.Language.AccessoryRefineFailed, targetItem.Info.ItemName), MessageType.System);
                targetArray[targetItem.Slot] = null;
                result.Links.Add(p.Target);
                RemoveItem(targetItem);
                targetItem.Delete();

            }

            Gold -= 50000;
            GoldChanged();
            targetOreArray[oretargetItem.Slot] = null;
            result.Links.Add(p.OreTarget);
            RemoveItem(oretargetItem);
            oretargetItem.Delete();
            Companion?.RefreshWeight();
            RefreshStats();
        }


        #region Instance / Dungeon Finder

        public void JoinInstance(C.JoinInstance p)
        {
            var instance = SEnvir.InstanceInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);

            if (instance == null)
            {
                return;
            }

            if (CurrentMap.Instance != null)
            {
                //Cannot move from one instance to another
                return;
            }

            S.JoinInstance result = new S.JoinInstance { Success = false };

            if (instance.MinPlayerLevel > 0 && Level < instance.MinPlayerLevel || instance.MaxPlayerLevel > 0 && Level > instance.MaxPlayerLevel)
            {
                result.Result = InstanceResult.InsufficientLevel;
                Enqueue(result);
                return;
            }
            
            if (instance.MinPlayerCount > 1 && (GroupMembers == null || GroupMembers.Count < instance.MinPlayerCount))
            {
                result.Result = InstanceResult.TooFewInGroup;
                Enqueue(result);
                return;
            }
            
            if (instance.MaxPlayerCount > 1 && (GroupMembers != null && GroupMembers.Count > instance.MaxPlayerCount))
            {
                result.Result = InstanceResult.TooManyInGroup;
                Enqueue(result);
                return;
            }

            //Load up instance
            var index = SEnvir.LoadInstance(instance);

            if (index == null)
            {
                result.Result = InstanceResult.NoSlots;
                Enqueue(result);
                return;
            }

            //TODO - Prompt first before taking everyone to the instance??
            //Offer teleport now or 5 minute delay.

            if (!Teleport(instance.ConnectRegion, instance, index.Value))
            {
                result.Result = InstanceResult.NoMap;
                Enqueue(result);
                return;
            }

            if (GroupMembers != null)
            {
                foreach (var member in GroupMembers)
                {
                    if (member == this) continue;

                    member.Teleport(instance.ConnectRegion, instance, index.Value);
                }
            }

            result.Success = true;
            result.Result = InstanceResult.Success;

            Enqueue(result);
        }

        public void SetFilters(C.SendCompanionFilters p)
        {
            Character.FiltersClass = String.Join(",", p.FilterClass);
            Character.FiltersRarity = String.Join(",", p.FilterRarity);
            Character.FiltersItemType = String.Join(",", p.FilterItemType);
            FiltersClass = Character.FiltersClass;
            FiltersItemType = Character.FiltersItemType;
            FiltersRarity = Character.FiltersRarity;
            Enqueue(new S.SendCompanionFilters { FilterClass = p.FilterClass, FilterRarity = p.FilterRarity, FilterItemType = p.FilterItemType });
            Connection.ReceiveChat("Companion filters have been updated", MessageType.System);
        }

        #endregion
    }

}

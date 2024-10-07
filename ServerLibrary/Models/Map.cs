using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public sealed class Map
    {
        public MapInfo Info { get; }
        public InstanceInfo Instance { get; }
        public byte InstanceSequence { get; }
        public int RespawnIndex { get; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool HasSafeZone { get; set; }

        public Cell[,] Cells { get; private set; }
        public List<Cell> ValidCells { get; } = new List<Cell>();

        public List<MapObject> Objects { get; } = new List<MapObject>();
        public List<PlayerObject> Players { get; } = new List<PlayerObject>();
        public List<MonsterObject> Bosses { get; } = new List<MonsterObject>();
        public List<CastleFlag> CastleFlags { get; } = new List<CastleFlag>();
        public List<CastleGate> CastleGates { get; } = new List<CastleGate>();
        public List<CastleGuard> CastleGuards { get; } = new List<CastleGuard>();
        public List<NPCObject> NPCs { get; } = new List<NPCObject>();
        public HashSet<MapObject>[] OrderedObjects;

        public DateTime LastProcess, LastPlayer, InstanceExpiryDateTime;

        public DateTime HalloweenEventTime, ChristmasEventTime;

        public Map(MapInfo info, InstanceInfo instance = null, byte instanceSequence = 0, int respawnIndex = 0)
        {
            Info = info;
            RespawnIndex = respawnIndex;

            if (instance != null)
            {
                Instance = instance;
                InstanceSequence = instanceSequence;
                InstanceExpiryDateTime = instance.TimeLimitInMinutes > 0 ? SEnvir.Now.AddMinutes(instance.TimeLimitInMinutes) : DateTime.MaxValue;
            }
        }

        public void Load()
        {
            var path = Path.Combine(Config.MapPath, Info.FileName + ".map");

            if (!File.Exists(path))
            {
                SEnvir.Log($"Map: {path} not found.");
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(path);

            Width = fileBytes[23] << 8 | fileBytes[22];
            Height = fileBytes[25] << 8 | fileBytes[24];

            Cells = new Cell[Width, Height];

            int offSet = 28 + Width * Height / 4 * 3;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    byte flag = fileBytes[offSet + (x * Height + y) * 14];

                    if ((flag & 0x02) != 2 || (flag & 0x01) != 1) continue;

                    ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }

            OrderedObjects = new HashSet<MapObject>[Width];
            for (int i = 0; i < OrderedObjects.Length; i++)
                OrderedObjects[i] = new HashSet<MapObject>();
        }
        public void Setup()
        {
            CreateGuards();

            CreateCastleFlags();
            CreateCastleGates();
            CreateCastleGuards();

            LastPlayer = DateTime.UtcNow;
        }

        private void CreateGuards()
        {
            foreach (GuardInfo info in Info.Guards)
            {
                MonsterObject mob = MonsterObject.GetMonster(info.Monster);
                mob.Direction = info.Direction;

                if (!mob.Spawn(this, new Point(info.X, info.Y)))
                {
                    SEnvir.Log($"Failed to spawn Guard Map:{Info.Description}, Location: {info.X}, {info.Y}");
                    continue;
                }
            }
        }

        private void CreateCastleFlags()
        {
            foreach (var castle in Info.Castles)
            {
                foreach (var info in castle.Flags)
                {
                    CastleFlag mob = MonsterObject.GetMonster(info.Monster) as CastleFlag;

                    mob.Castle = castle;

                    if (!mob.Spawn(castle, info))
                    {
                        SEnvir.Log($"Failed to spawn Flag Map:{Info.Description}, Location: {info.X}, {info.Y}");
                        continue;
                    }
                }
            }
        }
        private void CreateCastleGates()
        {
            foreach (var castle in Info.Castles)
            {
                foreach (var gate in castle.Gates)
                {
                    var mob = CastleGates.FirstOrDefault(x => x.GateInfo == gate);

                    if (mob == null)
                    {
                        mob = MonsterObject.GetMonster(gate.Monster) as CastleGate;

                        mob.Spawn(castle, gate);
                    }
                    else
                    {
                        mob.RepairGate();
                    }
                }
            }
        }
        private void CreateCastleGuards()
        {
            foreach (var castle in Info.Castles)
            {
                foreach (var guard in castle.Guards)
                {
                    var mob = CastleGuards.FirstOrDefault(x => x.GuardInfo == guard);

                    if (mob == null)
                    {
                        mob = MonsterObject.GetMonster(guard.Monster) as CastleGuard;

                        mob.Spawn(castle, guard);
                    }
                    else
                    {
                        mob.RepairGuard();
                    }
                }
            }
        }

        public void RefreshFlags()
        {
            foreach (var ob in CastleFlags)
            {
                ob.Refresh();
            }
        }

        public void Process()
        {
            if (LastPlayer.AddMinutes(1) < DateTime.UtcNow && Players.Any())
            {
                LastPlayer = DateTime.UtcNow;
            }
        }

        public void AddObject(MapObject ob)
        {
            Objects.Add(ob);

            switch (ob.Race)
            {
                case ObjectType.Player:
                    Players.Add((PlayerObject)ob);
                    break;
                case ObjectType.Item:
                    break;
                case ObjectType.NPC:
                    NPCs.Add((NPCObject)ob);
                    break;
                case ObjectType.Spell:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss)
                        Bosses.Add(mob);
                    break;
            }
        }
        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            switch (ob.Race)
            {
                case ObjectType.Player:
                    Players.Remove((PlayerObject)ob);
                    break;
                case ObjectType.Item:
                    break;
                case ObjectType.NPC:
                    NPCs.Remove((NPCObject)ob);
                    break;
                case ObjectType.Spell:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss)
                        Bosses.Remove(mob);
                    break;
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return null;

            return Cells[x, y];
        }
        public Cell GetCell(Point location)
        {
            return GetCell(location.X, location.Y);
        }
        public List<Cell> GetCells(Point location, int minRadius, int maxRadius, bool randomOrder = false)
        {
            List<Cell> cells = new List<Cell>();

            for (int d = 0; d <= maxRadius; d++)
            {
                for (int y = location.Y - d; y <= location.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= Height) break;

                    for (int x = location.X - d; x <= location.X + d; x += Math.Abs(y - location.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= Width) break;

                        Cell cell = Cells[x, y]; //Direct Access we've checked the boudaries.

                        if (cell == null) continue;

                        cells.Add(cell);
                    }
                }
            }

            if (randomOrder)
            {
                return cells.OrderBy(item => SEnvir.Random.Next()).ToList();
            }

            return cells;
        }

        public Point GetRandomLocation()
        {
            return ValidCells.Count > 0 ? ValidCells[SEnvir.Random.Next(ValidCells.Count)].Location : Point.Empty;
        }

        public Point GetRandomLocation(Point location, int range, int attempts = 25)
        {
            int minX = Math.Max(0, location.X - range);
            int maxX = Math.Min(Width, location.X + range);
            int minY = Math.Max(0, location.Y - range);
            int maxY = Math.Min(Height, location.Y + range);

            for (int i = 0; i < attempts; i++)
            {
                Point test = new Point(SEnvir.Random.Next(minX, maxX), SEnvir.Random.Next(minY, maxY));

                if (GetCell(test) != null)
                    return test;
            }

            return Point.Empty;
        }

        public Point GetRandomLocation(int minX, int maxX, int minY, int maxY, int attempts = 25)
        {
            for (int i = 0; i < attempts; i++)
            {
                Point test = new Point(SEnvir.Random.Next(minX, maxX), SEnvir.Random.Next(minY, maxY));

                if (GetCell(test) != null)
                    return test;
            }

            return Point.Empty;
        }

        public void Broadcast(Point location, Packet p)
        {
            foreach (PlayerObject player in Players)
            {
                if (!Functions.InRange(location, player.CurrentLocation, Config.MaxViewRange)) continue;
                player.Enqueue(p);
            }
        }
        public void Broadcast(Packet p)
        {
            foreach (PlayerObject player in Players)
                player.Enqueue(p);
        }
    }

    public class SpawnInfo
    {
        public RespawnInfo Info;
        public Map CurrentMap;

        public DateTime NextSpawn;
        public int AliveCount;

        public DateTime LastCheck;

        public SpawnInfo(RespawnInfo info, InstanceInfo instance, byte index)
        {
            Info = info;
            CurrentMap = SEnvir.GetMap(info.Region.Map, instance, index);
            LastCheck = SEnvir.Now;
        }

        public void DoSpawn(bool eventSpawn)
        {
            if (CurrentMap.RespawnIndex != Info.RespawnIndex) return;

            if (!eventSpawn)
            {
                if (Info.EventSpawn || SEnvir.Now < NextSpawn) return;

                if (Info.Delay >= 1000000)
                {
                    TimeSpan timeofDay = TimeSpan.FromMinutes(Info.Delay - 1000000);

                    if (LastCheck.TimeOfDay >= timeofDay || SEnvir.Now.TimeOfDay < timeofDay)
                    {
                        LastCheck = SEnvir.Now;
                        return;
                    }

                    LastCheck = SEnvir.Now;
                }
                else
                {
                    if (Info.Announce)
                        NextSpawn = SEnvir.Now.AddSeconds(Info.Delay * 60);
                    else
                        NextSpawn = SEnvir.Now.AddSeconds(SEnvir.Random.Next(Info.Delay * 60) + Info.Delay * 30);

                }
            }

            for (int i = AliveCount; i < Info.Count; i++)
            {
                MonsterObject mob = MonsterObject.GetMonster(Info.Monster);

                if (!Info.Monster.IsBoss)
                {
                    if (SEnvir.Now > CurrentMap.HalloweenEventTime && SEnvir.Now <= Config.HalloweenEventEnd)
                    {
                        mob = new HalloweenMonster { MonsterInfo = Info.Monster, HalloweenEventMob = true };
                        CurrentMap.HalloweenEventTime = SEnvir.Now.AddHours(1);
                    }
                    else if (SEnvir.Now > CurrentMap.ChristmasEventTime && SEnvir.Now <= Config.ChristmasEventEnd)
                    {
                        mob = new ChristmasMonster { MonsterInfo = Info.Monster, ChristmasEventMob = true };
                        CurrentMap.ChristmasEventTime = SEnvir.Now.AddMinutes(20);
                    }
                }

                mob.SpawnInfo = this;

                if (!mob.Spawn(Info.Region, CurrentMap.Instance, CurrentMap.InstanceSequence))
                {
                    mob.SpawnInfo = null;
                    continue;
                }

                if (Info.Announce)
                {
                    if (Info.Delay >= 1000000)
                    {
                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat($"{mob.MonsterInfo.MonsterName} has appeared.", MessageType.System);
                    }
                    else
                    {
                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat(string.Format(con.Language.BossSpawn, CurrentMap.Info.Description), MessageType.System);
                    }
                }

                mob.DropSet = Info.DropSet;
                AliveCount++;
            }
        }
    }

    public class Cell
    {
        public Point Location;

        public Map Map;

        public List<MapObject> Objects;
        public SafeZoneInfo SafeZone;

        public List<MovementInfo> Movements;

        public List<QuestTask> QuestTasks;

        public Cell(Point location)
        {
            Location = location;
        }


        public void AddObject(MapObject ob)
        {
            if (Objects == null)
                Objects = new List<MapObject>();

            Objects.Add(ob);

            ob.CurrentMap = Map;
            ob.CurrentLocation = Location;

            Map.OrderedObjects[Location.X].Add(ob);
        }
        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            if (Objects.Count == 0)
                Objects = null;

            Map.OrderedObjects[Location.X].Remove(ob);
        }
        public bool IsBlocking(MapObject checker, bool cellTime)
        {
            if (Objects == null) return false;

            foreach (MapObject ob in Objects)
            {
                if (!ob.Blocking) continue;
                if (cellTime && SEnvir.Now < ob.CellTime) continue;

                if (ob.Stats == null) return true;

                if (ob.Buffs.Any(x => x.Type == BuffType.Cloak || x.Type == BuffType.Transparency) && ob.Level > checker.Level && !ob.InGroup(checker)) continue;


                return true;
            }

            return false;
        }

        public Cell GetMovement(MapObject ob)
        {
            if (QuestTasks != null && QuestTasks.Count > 0)
            {
                if (ob.Race == ObjectType.Player)
                {
                    PlayerObject player = (PlayerObject)ob;

                    foreach (var task in QuestTasks)
                    {
                        var userQuest = player.Quests.FirstOrDefault(x => x.QuestInfo == task.Quest && !x.Completed);

                        if (userQuest == null) continue;

                        UserQuestTask userTask = userQuest.Tasks.FirstOrDefault(x => x.Task == task);

                        if (userTask == null)
                        {
                            userTask = SEnvir.UserQuestTaskList.CreateNewObject();
                            userTask.Task = task;
                            userTask.Quest = userQuest;
                        }

                        if (userTask.Completed) continue;

                        userTask.Amount = 1;

                        player.Enqueue(new S.QuestChanged { Quest = userQuest.ToClientInfo() });
                    }
                }
            }

            if (Movements == null || Movements.Count == 0)
                return this;

            for (int i = 0; i < 5; i++) //20 Attempts to get movement;
            {
                MovementInfo movement = Movements[SEnvir.Random.Next(Movements.Count)];

                Map map = SEnvir.GetMap(movement.DestinationRegion.Map, Map.Instance, Map.InstanceSequence);

                if (movement.NeedInstance != null)
                {
                    if (ob.Race != ObjectType.Player) break;

                    if (Map.Instance != null) //Moving from instance
                    {
                        map = SEnvir.GetMap(movement.DestinationRegion.Map, null, 0);
                    }
                    else //Moving to instance
                    {
                        var (index, result) = ((PlayerObject)ob).GetInstance(movement.NeedInstance);

                        if (result != InstanceResult.Success)
                        {
                            ((PlayerObject)ob).SendInstanceMessage(movement.NeedInstance, result);
                            break;
                        }

                        map = SEnvir.GetMap(movement.DestinationRegion.Map, movement.NeedInstance, index.Value);
                    }
                }

                if (map == null) break;

                Cell cell = map.GetCell(movement.DestinationRegion.PointList[SEnvir.Random.Next(movement.DestinationRegion.PointList.Count)]);

                if (cell == null) continue;

                if (ob.Race == ObjectType.Player)
                {
                    bool allowed = true;
                    PlayerObject player = (PlayerObject)ob;

                    if (movement.DestinationRegion.Map.MinimumLevel > ob.Level && !player.Character.Account.TempAdmin)
                    {
                        player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.NeedLevel, movement.DestinationRegion.Map.MinimumLevel), MessageType.System);
                        break;
                    }
                    if (movement.DestinationRegion.Map.MaximumLevel > 0 && movement.DestinationRegion.Map.MaximumLevel < ob.Level && !player.Character.Account.TempAdmin)
                    {
                        player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.NeedMaxLevel, movement.DestinationRegion.Map.MaximumLevel), MessageType.System);
                        break;
                    }

                    if (movement.DestinationRegion.Map.RequiredClass != RequiredClass.None && movement.DestinationRegion.Map.RequiredClass != RequiredClass.All)
                    {
                        switch (player.Class)
                        {
                            case MirClass.Warrior:
                                if ((movement.DestinationRegion.Map.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                                    allowed = false;
                                break;
                            case MirClass.Wizard:
                                if ((movement.DestinationRegion.Map.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                                    allowed = false;
                                break;
                            case MirClass.Taoist:
                                if ((movement.DestinationRegion.Map.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                                    allowed = false;
                                break;
                            case MirClass.Assassin:
                                if ((movement.DestinationRegion.Map.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                                    allowed = false;
                                break;
                        }

                        if (!allowed)
                        {
                            player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.NeedClass, movement.DestinationRegion.Map.RequiredClass.ToString()), MessageType.System);

                            break;
                        }
                    }

                    if (movement.NeedSpawn != null)
                    {
                        SpawnInfo spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == movement.NeedSpawn);

                        if (spawn == null)
                            break;

                        if (spawn.AliveCount == 0)
                        {
                            player.Connection.ReceiveChatWithObservers(con => con.Language.NeedMonster, MessageType.System);

                            break;
                        }
                    }

                    if (movement.NeedItem != null)
                    {
                        if (player.GetItemCount(movement.NeedItem) == 0)
                        {
                            player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.NeedItem, movement.NeedItem.ItemName), MessageType.System);
                            break;
                        }

                        player.TakeItem(movement.NeedItem, 1);
                    }

                    switch (movement.Effect)
                    {
                        case MovementEffect.SpecialRepair:
                            player.SpecialRepair(EquipmentSlot.Weapon);
                            player.SpecialRepair(EquipmentSlot.Shield);
                            player.SpecialRepair(EquipmentSlot.Helmet);
                            player.SpecialRepair(EquipmentSlot.Armour);
                            player.SpecialRepair(EquipmentSlot.Necklace);
                            player.SpecialRepair(EquipmentSlot.BraceletL);
                            player.SpecialRepair(EquipmentSlot.BraceletR);
                            player.SpecialRepair(EquipmentSlot.RingL);
                            player.SpecialRepair(EquipmentSlot.RingR);
                            player.SpecialRepair(EquipmentSlot.Shoes);

                            player.RefreshStats();
                            break;
                    }
                }

                return cell.GetMovement(ob);
            }

            return this;
        }
    }
}

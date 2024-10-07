using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Magics;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public abstract class MapObject
    {
        public uint ObjectID { get; }
        public LinkedListNode<MapObject> Node;

        public DateTime SpawnTime { get; set; }
        public abstract ObjectType Race { get; }

        public virtual bool Blocking => !Dead;

        public virtual string Name { get; set; }

        public virtual string Caption { get; set; }
       
    
        public virtual int Level { get; set; }

        public Cell CurrentCell
        {
            get { return _CurrentCell; }
            set
            {
                if (_CurrentCell == value) return;

                var oldValue = _CurrentCell;
                _CurrentCell = value;

                LocationChanged(oldValue, value);
            }
        }
        private Cell _CurrentCell;

        public Map CurrentMap
        {
            get { return _CurrentMap; }
            set
            {
                if (_CurrentMap == value) return;

                var oldValue = _CurrentMap;
                _CurrentMap = value;

                MapChanged(oldValue, value);
            }
        }
        private Map _CurrentMap;

        public virtual Point CurrentLocation { get; set; }
        public virtual MirDirection Direction { get; set; }

        public bool DisplayCrit, DisplayMiss, DisplayResist, DisplayBlock;

        public int DisplayHP;
        public int DisplayMP;
        public int DisplayFP;

        public virtual int CurrentHP { get; set; }
        public virtual int CurrentMP { get; set; }
        public virtual int CurrentFP { get; set; }

        public bool Spawned, Dead, CoolEye, Activated;
        public bool InSafeZone;

        public DateTime FrostBiteImmunity;

        public DateTime ActionTime, MoveTime, RegenTime, AttackTime, MagicTime, CellTime, StruckTime, BuffTime, ShockTime, DisplayHPMPTime, ItemReviveTime;
        public List<DelayedAction> ActionList;

        public virtual bool CanMove => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= MoveTime && SEnvir.Now > ShockTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip && (Poison & PoisonType.Containment) != PoisonType.Containment && Buffs.All(x => x.Type != BuffType.DragonRepulse);
        public virtual bool CanAttack => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= AttackTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.Fear) != PoisonType.Fear && Buffs.All(x => x.Type != BuffType.DragonRepulse);
        public virtual bool CanCast => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= MagicTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.Fear) != PoisonType.Fear && (Poison & PoisonType.Silenced) != PoisonType.Silenced && Buffs.All(x => x.Type != BuffType.DragonRepulse);

        public List<SpellObject> SpellList = new List<SpellObject>();

        public List<BuffInfo> Buffs;

        public Stats Stats;
        public bool Visible, PreventSpellCheck;
        public decimal LifeSteal;

        public TimeSpan RegenDelay;

        public Color NameColour;

        public List<PlayerObject> NearByPlayers;
        public List<PlayerObject> SeenByPlayers;
        public List<PlayerObject> DataSeenByPlayers;

        public PoisonType Poison;
        public List<Poison> PoisonList;
        public List<PlayerObject> GroupMembers;

        protected MapObject()
        {
            ObjectID = SEnvir.ObjectID;

            NameColour = Color.White;
            Visible = true;
            RegenDelay = TimeSpan.FromSeconds(10);
            BuffTime = SEnvir.Now;

            NearByPlayers = new List<PlayerObject>();
            SeenByPlayers = new List<PlayerObject>();

            ActionList = new List<DelayedAction>();
            Buffs = new List<BuffInfo>();
            PoisonList = new List<Poison>();
            DataSeenByPlayers = new List<PlayerObject>();
        }

        public void StartProcess()
        {
            DeActivate();

            //Other things
            for (int i = ActionList.Count - 1; i >= 0; i--)
            {
                if (SEnvir.Now < ActionList[i].Time) continue;

                DelayedAction ac = ActionList[i];
                ActionList.RemoveAt(i);
                ProcessAction(ac);
            }

            ProcessBuff();
            ProcessPoison();
            Process();

            ProcessHPMP();

            Color oldColour = NameColour;
            ProcessNameColour();

            if (oldColour != NameColour)
                Broadcast(new S.ObjectNameColour { ObjectID = ObjectID, Colour = NameColour });

        }

        public virtual void Process()
        {

        }

        public virtual void ProcessHPMP()
        {
            if (SEnvir.Now < DisplayHPMPTime) return;

            DisplayHPMPTime = SEnvir.Now.AddMilliseconds(200);

            bool changed = false;
            if (DisplayHP != CurrentHP || DisplayBlock || DisplayCrit || DisplayMiss)
            {
                int change = CurrentHP - DisplayHP;

                Broadcast(new S.HealthChanged { ObjectID = ObjectID, Change = change, Critical = DisplayCrit, Miss = DisplayMiss, Block = DisplayBlock });

                DisplayHP = CurrentHP;

                DisplayMiss = false;
                DisplayBlock = false;
                DisplayCrit = false;

                changed = true;
            }

            if (DisplayMP != CurrentMP)
            {
                int change = CurrentMP - DisplayMP;
                Broadcast(new S.ManaChanged { ObjectID = ObjectID, Change = change });
                DisplayMP = CurrentMP;

                changed = true;
            }

            if (DisplayFP != CurrentFP)
            {
                int change = CurrentFP - DisplayFP;
                Broadcast(new S.FocusChanged { ObjectID = ObjectID, Change = change });
                DisplayFP = CurrentFP;

                changed = true;
            }

            if (!changed) return;

            S.DataObjectHealthMana p = new S.DataObjectHealthMana { ObjectID = ObjectID, Health = DisplayHP, Mana = DisplayMP, Dead = Dead };

            foreach (PlayerObject player in DataSeenByPlayers)
                player.Enqueue(p);
        }
        public virtual void ProcessPoison()
        {
            PoisonType current = PoisonType.None;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                Poison poison = PoisonList[i];
                if (poison.Owner?.Node == null || poison.Owner.Dead || poison.Owner.CurrentMap != CurrentMap || !Functions.InRange(poison.Owner.CurrentLocation, CurrentLocation, Config.MaxViewRange))
                {
                    PoisonList.Remove(poison);
                    continue;
                }

                current |= poison.Type;

                if (SEnvir.Now < poison.TickTime) continue;

                if (poison.TickCount-- <= 0) 
                    PoisonList.RemoveAt(i);

                poison.TickTime = SEnvir.Now + poison.TickFrequency;

                int damage = 0;
                bool explode = false;
                MonsterObject mob;
                switch (poison.Type)
                {
                    case PoisonType.Green:
                        damage += poison.Value;
                        break;
                    case PoisonType.WraithGrip:
                        ChangeMP(-poison.Value);

                        if (poison.Extra != null)
                            poison.Owner.ChangeMP(poison.Value * (((UserMagic)poison.Extra).Level + 1));
                        break;
                    case PoisonType.HellFire:
                        damage += poison.Value;
                        break;
                    case PoisonType.Parasite:
                        {
                            damage += poison.Value;

                            if (poison.TickCount < 0)
                            {
                                explode = true;
                            }
                            else
                            {
                                if ((poison.Owner is PlayerObject owner) && owner.GetMagic(MagicType.Infection, out Infection infection))
                                {
                                    infection.MagicComplete(new object[]
                                    {
                                        MagicType.Infection,
                                        this
                                    });
                                }
                            }
                        }
                        break;
                    case PoisonType.Burn:
                        damage += poison.Value;
                        break;
                    case PoisonType.Containment:
                        damage += poison.Value; 
                        break;
                    case PoisonType.Chain:
                        damage += Chain.PoisonTick(this);
                        break;
                    case PoisonType.Hemorrhage:
                        damage += poison.Value;
                        break;
                }

                if (Stats[Stat.Invincibility] > 0)
                    damage = 0;

                if (damage > 0)
                {
                    if (Race == ObjectType.Monster && ((MonsterObject)this).MonsterInfo.IsBoss)
                        damage = 0;
                    else
                    {
                        if (!poison.CanKill)
                            damage = Math.Min(CurrentHP - 1, damage);
                    }
                        
                    if (damage > 0)
                    {
                        #region Conquest Stats

                        UserConquestStats conquest;

                        switch (Race)
                        {
                            case ObjectType.Player:
                                conquest = SEnvir.GetConquestStats((PlayerObject)this);

                                if (conquest != null)
                                {
                                    switch (poison.Owner.Race)
                                    {
                                        case ObjectType.Player:
                                            conquest.PvPDamageTaken += damage;

                                            conquest = SEnvir.GetConquestStats((PlayerObject)poison.Owner);

                                            if (conquest != null)
                                                conquest.PvPDamageDealt += damage;
                                            break;
                                        case ObjectType.Monster:
                                            mob = (MonsterObject)poison.Owner;

                                            if (mob is CastleLord)
                                                conquest.BossDamageTaken += damage;
                                            else if (mob.PetOwner != null)
                                            {
                                                conquest.PvPDamageTaken += damage;

                                                conquest = SEnvir.GetConquestStats(mob.PetOwner);

                                                if (conquest != null)
                                                    conquest.PvPDamageDealt += damage;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case ObjectType.Monster:
                                mob = (MonsterObject)this;

                                if (mob is CastleLord)
                                {
                                    switch (poison.Owner.Race)
                                    {
                                        case ObjectType.Player:
                                            conquest = SEnvir.GetConquestStats((PlayerObject)poison.Owner);

                                            if (conquest != null)
                                                conquest.BossDamageDealt += damage;
                                            break;
                                        case ObjectType.Monster:

                                            mob = (MonsterObject)poison.Owner;
                                            if (mob.PetOwner != null)
                                            {
                                                conquest = SEnvir.GetConquestStats(mob.PetOwner);

                                                if (conquest != null)
                                                    conquest.BossDamageDealt += damage;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                        #endregion

                        ChangeHP(-damage);
                    }

                    if (Dead) break;

                    RegenTime = SEnvir.Now + RegenDelay;
                    ShockTime = DateTime.MinValue;
                }

                if (explode)
                {
                    switch (Race)
                    {
                        case ObjectType.Monster:
                            {
                                mob = (MonsterObject)this;

                                mob.EXPOwner ??= (PlayerObject)poison.Owner;
                            }
                            break;
                    }

                    switch (poison.Type)
                    {
                        case PoisonType.Parasite:
                            {
                                if ((poison.Owner is PlayerObject owner) && owner.GetMagic(MagicType.Parasite, out Parasite parasite))
                                {
                                    parasite.Explode(this);
                                }
                            }
                            break;
                    }
                }
            }

            if (current == Poison) return;

            Poison = current;
            Broadcast(new S.ObjectPoison { ObjectID = ObjectID, Poison = Poison });
        }
        public virtual void ProcessNameColour()
        { }
        public virtual void ProcessBuff()
        {
            TimeSpan ticks = SEnvir.Now - BuffTime;

            BuffTime = SEnvir.Now;
            List<BuffInfo> expiredBuffs = new List<BuffInfo>();

            foreach (BuffInfo buff in Buffs)
            {
                int amount;
                PlayerObject player;

                if (buff.Pause) continue;

                switch (buff.Type)
                {
                    case BuffType.Companion:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        player = (PlayerObject)this;

                        if (!player.InSafeZone || player.Companion.UserCompanion.Level < 15)
                            player.Companion.UserCompanion.Hunger--;

                        if (player.Companion.LevelInfo.MaxExperience > 0)
                        {
                            int highest = player.Character.Account.Companions.Max(x => x.Level);

                            if (highest <= player.Companion.UserCompanion.Level)
                                highest = 1;

                            player.Companion.UserCompanion.Experience += highest + Stats[Stat.CompanionRate];

                            if (player.Companion.UserCompanion.Experience >= player.Companion.LevelInfo.MaxExperience)
                            {
                                player.Companion.UserCompanion.Experience = 0;
                                player.Companion.UserCompanion.Level++;
                                player.Companion.CheckSkills();
                                player.Companion.RefreshStats();
                            }
                        }

                        player.Companion.AutoFeed();

                        player.Enqueue(new S.CompanionUpdate
                        {
                            Level = player.Companion.UserCompanion.Level,
                            Experience = player.Companion.UserCompanion.Experience,
                            Hunger = player.Companion.UserCompanion.Hunger,
                        });

                        if (player.Companion.UserCompanion.Hunger <= 0)
                            expiredBuffs.Add(buff);
                        break;
                    case BuffType.Heal:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        amount = Math.Min(buff.Stats[Stat.Healing], buff.Stats[Stat.HealingCap]);

                        ChangeHP(amount);
                        buff.Stats[Stat.Healing] -= amount;

                        if (CurrentHP < Stats[Stat.Health] && buff.Stats[Stat.Healing] > 0)
                        {
                            if (Race == ObjectType.Player)
                                ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                        }
                        else
                            expiredBuffs.Add(buff);

                        break;
                    case BuffType.Cloak:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        amount = buff.Stats[Stat.CloakDamage];

                        if (amount >= CurrentHP)
                        {
                            expiredBuffs.Add(buff);
                            break;
                        }

                        ChangeHP(-amount);
                        break;
                    case BuffType.DarkConversion:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        amount = buff.Stats[Stat.DarkConversion];

                        if (amount > CurrentMP)
                        {
                            expiredBuffs.Add(buff);
                            break;
                        }

                        ChangeMP(-amount);
                        ChangeHP(amount * 2);
                        if (Race != ObjectType.Player) break;

                        player = (PlayerObject)this;

                        if (player.GetMagic(MagicType.DarkConversion, out DarkConversion darkConversion))
                        {
                            player.LevelMagic(darkConversion.Magic);
                        }
                        break;
                    case BuffType.PKPoint:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickFrequency = Config.PKPointTickRate;

                        buff.TickTime += buff.TickFrequency;

                        buff.Stats[Stat.PKPoint]--;

                        RefreshStats();

                        if (buff.Stats[Stat.PKPoint] <= 0)
                            expiredBuffs.Add(buff);
                        else
                        {
                            if (Race == ObjectType.Player)
                                ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                        }
                        break;
                    case BuffType.HuntGold:
                        buff.TickTime -= ticks;

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        player = this as PlayerObject;


                        if (player != null)
                        {
                            if (SEnvir.ConquestWars.Any(war => war.Map == CurrentMap))
                            {
                                player.Character.Account.HuntGold.Amount += 1;

                                player.CurrencyChanged(player.Character.Account.HuntGold);

                                continue;
                            }
                        }


                        if (buff.Stats[Stat.AvailableHuntGold] >= buff.Stats[Stat.AvailableHuntGoldCap]) continue;

                        buff.Stats[Stat.AvailableHuntGold]++;

                        if (Race == ObjectType.Player)
                            ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                        break;
                    case BuffType.DragonRepulse:
                        buff.TickTime -= ticks;

                        if (buff.RemainingTime != TimeSpan.MaxValue)
                        {
                            buff.RemainingTime -= ticks;

                            if (buff.RemainingTime <= TimeSpan.Zero)
                                expiredBuffs.Add(buff);
                        }

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 5);

                        switch (Race)
                        {
                            case ObjectType.Player:
                                player = (PlayerObject)this;

                                if (!player.GetMagic(MagicType.DragonRepulse, out DragonRepulse dragonRepulse)) break;

                                player.LevelMagic(dragonRepulse.Magic);

                                for (int c = cells.Count - 1; c >= 0; c--)
                                {
                                    Cell cell = cells[c];
                                    if (cell.Objects == null) continue;

                                    if (Functions.Distance4Directions(CurrentLocation, cell.Location) > 5) continue;

                                    for (int o = cell.Objects.Count - 1; o >= 0; o--)
                                    {
                                        MapObject ob = cell.Objects[o];

                                        if (!CanAttackTarget(ob)) continue;

                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(SEnvir.Random.Next(200) + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 20),
                                            ActionType.DelayMagic,
                                            MagicType.DragonRepulse,
                                            ob));
                                    }
                                }
                                break;
                            case ObjectType.Monster:
                                for (int c = cells.Count - 1; c >= 0; c--)
                                {
                                    Cell cell = cells[c];
                                    if (cell.Objects == null) continue;

                                    if (Functions.Distance4Directions(CurrentLocation, cell.Location) > 5) continue;

                                    for (int o = cell.Objects.Count - 1; o >= 0; o--)
                                    {
                                        MapObject ob = cell.Objects[o];

                                        if (!CanAttackTarget(ob)) continue;

                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(400),
                                            ActionType.DelayMagic,
                                            MagicType.DragonRepulse,
                                            ob));
                                    }
                                }
                                break;

                        }
                        break;
                    case BuffType.FrostBite:
                        {
                            buff.RemainingTime -= ticks;
                            if (buff.RemainingTime > TimeSpan.Zero) continue;

                            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.FrostBiteEnd });

                            if (this is PlayerObject ob && ob.GetMagic(MagicType.FrostBite, out FrostBite frostBite))
                            {
                                frostBite.FrostBiteEnd(buff);
                            }

                            FrostBiteImmunity = SEnvir.Now.AddSeconds(1);

                            expiredBuffs.Add(buff);
                        }
                        break;
                    case BuffType.ElementalHurricane:
                        buff.TickTime -= ticks;

                        if (buff.RemainingTime != TimeSpan.MaxValue)
                        {
                            buff.RemainingTime -= ticks;

                            if (buff.RemainingTime <= TimeSpan.Zero)
                                expiredBuffs.Add(buff);
                        }

                        if (buff.TickTime > TimeSpan.Zero) continue;

                        buff.TickTime += buff.TickFrequency;

                        switch (Race)
                        {
                            case ObjectType.Player:
                                {
                                    player = (PlayerObject)this;

                                    if (!player.GetMagic(MagicType.ElementalHurricane, out ElementalHurricane elementalHurricane)) break;

                                    int cost = elementalHurricane.Magic.Cost;

                                    if (cost > CurrentMP)
                                    {
                                        expiredBuffs.Add(buff);
                                    }
                                    ChangeMP(-cost);

                                    for (int i = 1; i <= 8; i++)
                                    {
                                        Point location = Functions.Move(CurrentLocation, Direction, i);
                                        Cell cell = CurrentMap.GetCell(location);

                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now,
                                            ActionType.DelayMagic,
                                            MagicType.ElementalHurricane,
                                            cell,
                                            true));

                                        switch (Direction)
                                        {
                                            case MirDirection.Up:
                                            case MirDirection.Right:
                                            case MirDirection.Down:
                                            case MirDirection.Left:
                                                ActionList.Add(new DelayedAction(
                                                    SEnvir.Now,
                                                    ActionType.DelayMagic,
                                                    MagicType.ElementalHurricane,
                                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -2))),
                                                    false));
                                                ActionList.Add(new DelayedAction(
                                                    SEnvir.Now,
                                                    ActionType.DelayMagic,
                                                    MagicType.ElementalHurricane,
                                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 2))),
                                                    false));
                                                break;
                                            case MirDirection.UpRight:
                                            case MirDirection.DownRight:
                                            case MirDirection.DownLeft:
                                            case MirDirection.UpLeft:
                                                ActionList.Add(new DelayedAction(
                                                    SEnvir.Now,
                                                    ActionType.DelayMagic,
                                                    MagicType.ElementalHurricane,
                                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 1))),
                                                    false));
                                                ActionList.Add(new DelayedAction(
                                                    SEnvir.Now,
                                                    ActionType.DelayMagic,
                                                    MagicType.ElementalHurricane,
                                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -1))),
                                                    false));
                                                break;
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    default:
                        if (buff.RemainingTime == TimeSpan.MaxValue) continue;

                        buff.RemainingTime -= ticks;

                        if (buff.RemainingTime > TimeSpan.Zero) continue;

                        expiredBuffs.Add(buff);
                        break;
                }
            }

            foreach (BuffInfo buff in expiredBuffs)
                BuffRemove(buff);
        }


        public virtual void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.BroadCastPacket:
                    Broadcast((Packet)action.Data[0]);
                    break;
            }
        }

        public bool Spawn(MapRegion region, InstanceInfo instance, byte instanceSequence)
        {
            if (region == null) return false;

            Map map = SEnvir.GetMap(region.Map, instance, instanceSequence);

            if (map == null) return false;

            if (region.PointList.Count == 0) return false;

            for (int i = 0; i < 20; i++)
                if (Spawn(map, region.PointList[SEnvir.Random.Next(region.PointList.Count)])) break;

            return true;
        }

        public bool Spawn(MapInfo info, InstanceInfo instance, byte instanceSequence, Point location)
        {
            var map = SEnvir.GetMap(info, instance, instanceSequence);

            if (map == null)
            {
                return false;
            }

            return Spawn(map, location);
        }



        public bool Spawn(Map map, Point location)
        {
            if (Node != null)
                throw new InvalidOperationException("Node is not null, Object already spawned");

            if (map == null || map.Info == null) return false;

            if (Race == ObjectType.Player && map.Info.MinimumLevel > Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;
            if (Race == ObjectType.Player && map.Info.MaximumLevel > 0 && map.Info.MaximumLevel < Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;
            if (Race == ObjectType.Player && map.Info.RequiredClass != RequiredClass.None && map.Info.RequiredClass != RequiredClass.All && !((PlayerObject)this).Character.Account.TempAdmin)
            {
                switch (((PlayerObject)this).Class)
                {
                    case MirClass.Warrior:
                        if ((map.Info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                            return false;
                        break;
                    case MirClass.Wizard:
                        if ((map.Info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                            return false;
                        break;
                    case MirClass.Taoist:
                        if ((map.Info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                            return false;
                        break;
                    case MirClass.Assassin:
                        if ((map.Info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                            return false;
                        break;
                }
            }

            Cell cell = map.GetCell(location);

            if (cell == null) return false;

            CurrentCell = cell;

            Spawned = true;
            Node = SEnvir.Objects.AddLast(this);

            OnSpawned();

            return true;
        }
        protected void MapChanged(Map oMap, Map nMap)
        {
            oMap?.RemoveObject(this);
            nMap?.AddObject(this);

            OnMapChanged();
        }
        public void LocationChanged(Cell oCell, Cell nCell)
        {
            oCell?.RemoveObject(this);
            nCell?.AddObject(this);

            OnLocationChanged();
        }
        protected virtual void OnLocationChanged()
        {
            CellTime = SEnvir.Now.AddMilliseconds(300);

            BuffRemove(BuffType.PoisonousCloud);

            if (CurrentCell != null)
            {
                if (!PreventSpellCheck) CheckSpellObjects();

                S.DataObjectLocation p = new S.DataObjectLocation { ObjectID = ObjectID, MapIndex = CurrentCell.Map.Info.Index, CurrentLocation = CurrentLocation };

                foreach (PlayerObject player in DataSeenByPlayers)
                    player.Enqueue(p);
            }
        }
        public virtual void CheckSpellObjects()
        {
            Cell cell = CurrentCell;

            for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = CurrentCell.Objects[i];
                if (Dead) break;
                if (ob.Race != ObjectType.Spell) continue;

                ((SpellObject)ob).ProcessSpell(this);

                if (cell != CurrentCell) break; //Tempest could repel this 
            }
        }
        protected virtual void OnMapChanged() { }
        protected virtual void OnSpawned()
        {
            SpawnTime = SEnvir.Now;
        }

        public void TeleportNearby(int minDistance, int maxDistance)
        {
            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, minDistance, maxDistance);

            if (cells.Count == 0) return;

            Teleport(CurrentMap, cells[SEnvir.Random.Next(cells.Count)].Location);
        }
        public bool Teleport(MapRegion region, InstanceInfo instance, byte instanceSequence, bool leaveEffect = true)
        {
            Map map = SEnvir.GetMap(region.Map, instance, instanceSequence);

            if (map == null)
            {
                return false;
            }

            Point point = region.PointList.Count > 0 ? region.PointList[SEnvir.Random.Next(region.PointList.Count)] : map.GetRandomLocation();

            return Teleport(map, point, leaveEffect);
        }
        public virtual bool Teleport(Map map, Point location, bool leaveEffect = true, bool enterEffect = true)
        {
            if (Race == ObjectType.Player && map.Info.MinimumLevel > Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;
            if (Race == ObjectType.Player && map.Info.MaximumLevel > 0 && map.Info.MaximumLevel < Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;

            Cell cell = map?.GetCell(location);

            if (cell == null || cell.Movements != null) return false;

            if (leaveEffect)
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.TeleportOut });

            CurrentCell = cell.GetMovement(this);
            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            if (enterEffect)
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.TeleportIn });

            return true;
        }
        public virtual void AddAllObjects()
        {
            foreach (PlayerObject ob in CurrentMap.Players)
            {
                if (CanBeSeenBy(ob))
                    ob.AddObject(this);

                if (IsNearBy(ob))
                    ob.AddNearBy(this);
            }

            foreach (PlayerObject ob in SEnvir.Players)
            {
                if (CanDataBeSeenBy(ob))
                    ob.AddDataObject(this);
            }
        }
        public virtual void RemoveAllObjects()
        {
            for (int i = SeenByPlayers.Count - 1; i >= 0; i--)
            {
                if (CanBeSeenBy(SeenByPlayers[i])) continue;

                SeenByPlayers[i].RemoveObject(this);
            }

            for (int i = DataSeenByPlayers.Count - 1; i >= 0; i--)
            {
                if (CanDataBeSeenBy(DataSeenByPlayers[i])) continue;

                DataSeenByPlayers[i].RemoveDataObject(this);
            }

            for (int i = NearByPlayers.Count - 1; i >= 0; i--)
            {
                if (IsNearBy(NearByPlayers[i])) continue;

                NearByPlayers[i].RemoveNearBy(this);
            }
        }
        public virtual void Activate()
        {
            if (Activated) return;

            if (NearByPlayers.Count == 0) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public virtual void DeActivate()
        {
            if (!Activated) return;

            if (NearByPlayers.Count > 0 && ActionList.Count == 0) return;

            Activated = false;
            SEnvir.ActiveObjects.Remove(this);
        }

        public virtual bool CanDataBeSeenBy(PlayerObject ob)
        {
            if (ob == this) return true;

            if (CurrentMap == null || ob.CurrentMap == null) return false;

            switch (Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)this;
                    if (player.Observer) return false;

                    if (ob.CurrentMap.Instance != CurrentMap.Instance || ob.CurrentMap.InstanceSequence != CurrentMap.InstanceSequence) return false;

                    if (InGroup(ob)) return true;

                    if (player.InGuild(ob)) return true;

                    if (player.Character?.Partner?.Player == ob) return true;

                    if (ob.CurrentMap == CurrentMap && ob.Stats[Stat.PlayerTracker] > 0) return true;

                    break;
                case ObjectType.Monster:

                    MonsterObject mob = (MonsterObject)this;

                    if (ob.CurrentMap == CurrentMap && mob.MonsterInfo.IsBoss && ob.Stats[Stat.BossTracker] > 0) return true;
                    break;
            }

            return CanBeSeenBy(ob);
        }
        public virtual bool CanBeSeenBy(PlayerObject ob)
        {
            if (ob == this) return true;

            if (CurrentMap != ob.CurrentMap)
                return false;

            if (!Functions.InRange(CurrentLocation, ob.CurrentLocation, Config.MaxViewRange))
                return false;

            if (Race == ObjectType.Player && ((PlayerObject)this).Observer) return false;

            if (ob.Character.Account.TempAdmin)
                return true;

            if (Buffs.Any(x => x.Type == BuffType.Cloak || x.Type == BuffType.Transparency))
            {
                if (InGroup(ob))
                    return true;

                if (Race == ObjectType.Player)
                {
                    PlayerObject player = (PlayerObject)this;

                    if (player.Observer) return false;

                    if (player.InGuild(ob)) return true;

                    if (player.Character?.Partner?.Player == ob) return true;
                }

                if (ob.Level < Level || !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.CloakRange))
                    return false;
            }

            return true;
        }

        public virtual bool IsNearBy(PlayerObject ob)
        {
            if (ob == this) return true;

            return CurrentMap == ob.CurrentMap && Functions.InRange(CurrentLocation, ob.CurrentLocation, Config.MaxViewRange);
        }

        public void Despawn()
        {
            if (Node == null)
                throw new InvalidOperationException("Node is null, Object already Despawned");

            CurrentMap = null;
            CurrentCell = null;

            RemoveAllObjects();

            //Clear Lists

            Node.List.Remove(Node);
            Node = null;

            if (Activated)
            {
                Activated = false;
                SEnvir.ActiveObjects.Remove(this);
            }

            OnDespawned();

            CleanUp();
        }

        public void SafeDespawn()
        {
            CurrentMap = null;
            CurrentCell = null;

            RemoveAllObjects();

            if (Node != null)
            {
                Node.List.Remove(Node);
                Node = null;
            }

            OnSafeDespawn();

            CleanUp();
        }

        public virtual void CleanUp()
        {
            ActionList?.Clear();

            SpellList?.Clear();

            Buffs?.Clear();

            NearByPlayers?.Clear();

            SeenByPlayers?.Clear();

            DataSeenByPlayers?.Clear();

            PoisonList?.Clear();

            GroupMembers?.Clear();
        }
        public virtual void OnDespawned()
        {
            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
        }
        public virtual void OnSafeDespawn()
        {
            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
        }
        public virtual void RefreshStats() { }

        public virtual Cell GetDropLocation(int distance, PlayerObject player)
        {
            if (CurrentMap == null || CurrentCell == null) return null;

            Cell bestCell = null;
            int layers = 0;

            for (int d = 0; d <= distance; d++)
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

                        if (cell == null || cell.Movements != null) continue;

                        bool blocking = false;

                        int count = 0;

                        if (cell.Objects != null)
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (ob.Blocking)
                                {
                                    blocking = true;
                                    break;
                                }

                                if (ob.Race != ObjectType.Item) continue;

                                if (player != null && !ob.CanBeSeenBy(player)) continue;

                                count++;
                            }

                        if (blocking) continue;

                        if (count == 0) return cell;

                        if (bestCell != null && count >= layers) continue;

                        bestCell = cell;
                        layers = count;
                    }
                }
            }

            if (bestCell == null || layers >= Config.DropLayers) 
                return null;

            return bestCell;
        }

        public virtual void SetHP(int amount)
        {
            if (Dead) return;

            CurrentHP = Math.Min(amount, Stats[Stat.Health]);

            if (CurrentHP <= 0 && !Dead)
            {
                if (Buffs.Any(x => x.Type == BuffType.CelestialLight))
                {
                    CelestialLightActivate();
                    return;
                }
                if (Stats[Stat.ItemReviveTime] > 0 && SEnvir.Now >= ItemReviveTime)
                {
                    ItemRevive();
                    return;
                }

                Die();
            }
        }
        public virtual void ChangeHP(int amount)
        {
            if (Dead) return;

            if (amount < 0 && Stats[Stat.ProtectionRing] > 0)
            {
                if (CurrentMP >= Math.Abs(amount))
                {
                    ChangeMP(amount);
                    return;
                }

                if (CurrentMP > 0)
                {
                    amount += CurrentMP; //Amount is Negative so -100 + 15 Remaining mana = 85 Damage
                    SetMP(0);
                }
            }

            if (CurrentHP + amount > Stats[Stat.Health])
                amount = Stats[Stat.Health] - CurrentHP;

            CurrentHP += amount;

            if (CurrentHP <= 0 && !Dead)
            {
                if (Buffs.Any(x => x.Type == BuffType.CelestialLight))
                {
                    CelestialLightActivate();
                    return;
                }
                if (Stats[Stat.ItemReviveTime] > 0 && SEnvir.Now >= ItemReviveTime)
                {
                    ItemRevive();
                    return;
                }

                Die();
            }
        }

        public virtual void SetMP(int amount)
        {
            CurrentMP = Math.Min(amount, Stats[Stat.Mana]);
        }

        public void SetFP(int amount)
        {
            CurrentFP = Math.Min(amount, Stats[Stat.Focus]);
        }

        public virtual void ChangeMP(int amount)
        {
            if (CurrentMP + amount > Stats[Stat.Mana])
                amount = Stats[Stat.Mana] - CurrentMP;

            CurrentMP += amount;
        }

        public void ChangeFP(int amount)
        {
            if (CurrentFP + amount > Stats[Stat.Focus])
                amount = Stats[Stat.Focus] - CurrentFP;

            CurrentFP += amount;
        }

        public virtual void CelestialLightActivate()
        {
            CurrentHP = Stats[Stat.Health] * Stats[Stat.CelestialLight] / 100;
            BuffRemove(BuffType.CelestialLight);
        }

        public virtual void ItemRevive()
        {
            CurrentHP = Stats[Stat.Health];
            CurrentMP = Stats[Stat.Mana];
            ItemReviveTime = SEnvir.Now.AddSeconds(Stats[Stat.ItemReviveTime]);
        }

        public virtual int Purify(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead) return 0;

            int result = 0;

            List<BuffInfo> buffs = new List<BuffInfo>();

            if (CanHelpTarget(ob))
            {
                result += ob.PoisonList.Count;

                for (int i = ob.PoisonList.Count - 1; i >= 0; i--)
                {
                    if (ob.PoisonList[i].Type == PoisonType.Parasite) continue;

                    ob.PoisonList.RemoveAt(i);
                }

                foreach (BuffInfo buff in ob.Buffs)
                {
                    switch (buff.Type)
                    {
                        case BuffType.MagicWeakness:
                        case BuffType.DefensiveBlow:
                            buffs.Add(buff);
                            result++;
                            break;
                        default:
                            continue;
                    }
                }
            }

            if (CanAttackTarget(ob) && (ob.Level <= Level || SEnvir.Random.Next(100) < 42))
            {
                foreach (BuffInfo buff in ob.Buffs)
                {
                    switch (buff.Type)
                    {
                        case BuffType.Heal:
                        case BuffType.Invisibility:
                        case BuffType.MagicResistance:
                        case BuffType.Resilience:
                        case BuffType.MagicShield:
                        case BuffType.SuperiorMagicShield:
                        case BuffType.ElementalSuperiority:
                        case BuffType.BloodLust:
                        case BuffType.Defiance:
                        case BuffType.Might:
                        case BuffType.ReflectDamage:
                        case BuffType.JudgementOfHeaven:
                        case BuffType.StrengthOfFaith:
                        case BuffType.CelestialLight:
                        case BuffType.Transparency:
                        case BuffType.Renounce:
                        case BuffType.Cloak:
                        case BuffType.GhostWalk:
                        case BuffType.LifeSteal:
                        case BuffType.DarkConversion:
                        case BuffType.Evasion:
                        case BuffType.RagingWind:
                        case BuffType.Invincibility:
                            buffs.Add(buff);
                            result++;
                            break;
                        default:
                            continue;
                    }
                }
            }

            for (int i = buffs.Count - 1; i >= 0; i--)
                ob.BuffRemove(buffs[i]);

            return result;
        }

        public virtual BuffInfo BuffAdd(BuffType type, TimeSpan remainingTicks, Stats stats, bool visible, bool pause, TimeSpan tickRate)
        {
            BuffRemove(type);

            BuffInfo info;

            Buffs.Add(info = SEnvir.BuffInfoList.CreateNewObject());

            info.Type = type;
            info.Visible = visible;

            info.RemainingTime = remainingTicks;
            info.TickFrequency = tickRate;
            info.Pause = pause;
            info.Stats = stats;

            if (info.Stats != null && info.Stats.Count > 0)
                RefreshStats();

            switch (type)
            {
                case BuffType.PoisonousCloud:
                case BuffType.Observable:
                case BuffType.TheNewBeginning:
                case BuffType.Server:
                case BuffType.MapEffect:
                case BuffType.InstanceEffect:
                case BuffType.Castle:
                case BuffType.Guild:
                case BuffType.Veteran:
                case BuffType.Fame:
                    info.IsTemporary = true;
                    break;
                case BuffType.RagingWind:
                case BuffType.MagicWeakness:
                    RefreshStats();
                    break;
                case BuffType.Invisibility:
                    //Much faster than checking nearby cells?
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        if (mob.Target == this && !mob.CoolEye)
                        {
                            mob.Target = null;
                            mob.SearchTime = SEnvir.Now;
                        }
                    }
                    break;
                case BuffType.Cloak:
                case BuffType.Transparency:
                    //Much faster than checking nearby cells?
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        if (mob.Target == this)
                        {
                            mob.Target = null;
                            mob.SearchTime = SEnvir.Now;
                        }
                    }

                    RemoveAllObjects();
                    AddAllObjects();

                    if (Race == ObjectType.Player)
                    {
                        ((PlayerObject)this).Companion?.RemoveAllObjects();
                        ((PlayerObject)this).Companion?.AddAllObjects();
                    }

                    break;
            }


            if (!info.Visible) return info;

            Broadcast(new S.ObjectBuffAdd { ObjectID = ObjectID, Type = type });

            return info;
        }

        public virtual void DecreaseBuffCharge(BuffInfo info)
        {
            if (info.Type == BuffType.TheNewBeginning)
            {
                var buff = Buffs.SingleOrDefault(x => x.Type == info.Type);

                if (buff != null)
                {
                    buff.Stats[Stat.TheNewBeginning]--;
                    RefreshStats();
                }
            }
        }
        public virtual void BuffRemove(BuffInfo info)
        {
            if (info.Visible)
                Broadcast(new S.ObjectBuffRemove { ObjectID = ObjectID, Type = info.Type });

            Buffs.Remove(info);

            if (info.Stats != null && info.Stats.Count > 0)
                RefreshStats();

            info.Delete();
            switch (info.Type)
            {
                case BuffType.Cloak:
                case BuffType.Transparency:
                    BuffRemove(BuffType.GhostWalk);
                    RemoveAllObjects();
                    AddAllObjects();

                    if (Race == ObjectType.Player)
                    {
                        ((PlayerObject)this).Companion?.RemoveAllObjects();
                        ((PlayerObject)this).Companion?.AddAllObjects();
                    }
                    break;
                case BuffType.RagingWind:
                case BuffType.MagicWeakness:
                    RefreshStats();
                    break;
            }

            switch (info.Type)
            {
                case BuffType.Cloak:
                case BuffType.Transparency:
                case BuffType.Invisibility:
                    //Much faster than checking nearby cells?
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        mob.SearchTime = DateTime.MinValue;
                    }

                    break;
            }
        }

        public void BuffRemove(BuffType type)
        {
            BuffInfo info = Buffs.FirstOrDefault(x => x.Type == type);

            if (info != null)
                BuffRemove(info);
        }
        public virtual int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true) { return 0; }

        public List<MapObject> GetTargets(Map map, Point location, int radius)
        {
            List<MapObject> targets = new List<MapObject>();

            for (int y = location.Y - radius; y <= location.Y + radius; y++)
            {
                if (y < 0) continue;
                if (y >= map.Height) break;

                for (int x = location.X - radius; x <= location.X + radius; x++)
                {
                    if (x < 0) continue;
                    if (x >= map.Width) break;

                    Cell cell = map.Cells[x, y];

                    if (cell?.Objects == null) continue;

                    foreach (MapObject ob in cell.Objects)
                    {
                        if (!CanAttackTarget(ob)) continue;

                        targets.Add(ob);
                    }
                }
            }

            return targets;
        }
        public List<MapObject> GetAllObjects(Point location, int radius)
        {
            List<MapObject> obs = new List<MapObject>();

            for (int y = location.Y - radius; y <= location.Y + radius; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = location.X - radius; x <= location.X + radius; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;

                    Cell cell = CurrentMap.Cells[x, y];

                    if (cell?.Objects == null) continue;

                    obs.AddRange(cell.Objects);
                }
            }

            return obs;
        }

        public virtual bool CanHelpTarget(MapObject ob)
        {
            return false;
        }

        public virtual bool CanAttackTarget(MapObject ob)
        {
            return false;
        }

        public virtual void Dodged()
        {
            DisplayMiss = true;
        }
        public virtual void Blocked()
        {
            DisplayBlock = true;
        }
        public virtual void Critical()
        {
            DisplayCrit = true;
        }

        public virtual bool ApplyPoison(Poison p)
        {
            if (Dead) return false;

            if (SEnvir.Random.Next(100) < Stats[Stat.PoisonResistance]) return false;

            foreach (Poison poison in PoisonList)
            {
                if (poison.Type != p.Type) continue;

                if (poison.Value > p.Value) return false;

                PoisonList.Remove(poison);
                break;
            }

            //Check Pets target

            PoisonList.Add(p);

            return true;
        }

        public virtual void Die()
        {
            Dead = true;

            BuffRemove(BuffType.Heal);
            BuffRemove(BuffType.DragonRepulse);
            BuffRemove(BuffType.ElementalHurricane);
            BuffRemove(BuffType.DefensiveBlow);

            var p = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Chain);

            if (p != null && p.Owner is PlayerObject owner && owner.GetMagic(MagicType.Chain, out Chain chain))
            {
                chain.Explode(this);
            }

            PoisonList.Clear();

            Broadcast(new S.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public abstract Packet GetInfoPacket(PlayerObject ob);
        public abstract Packet GetDataPacket(PlayerObject ob);

        public void Broadcast(Packet p)
        {
            foreach (PlayerObject player in SeenByPlayers)
                player.Enqueue(p);
        }
        public virtual int Pushed(MirDirection direction, int distance)
        {
            int count = 0;
            if (Dead) return count;
            if (Buffs.Any(x => x.Type == BuffType.Endurance || x.Type == BuffType.DragonRepulse || x.Type == BuffType.ElementalHurricane)) return count;

            PreventSpellCheck = true;
            for (int i = 0; i < distance; i++)
            {
                Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction));

                if (cell == null || cell.IsBlocking(this, false)) break;
                if (Race == ObjectType.Monster && cell.Movements != null) break;

                Direction = Functions.ShiftDirection(direction, 4);

                ActionTime += Globals.TurnTime;

                CurrentCell = cell.GetMovement(this); //Repel same direction across map ?

                RemoveAllObjects();
                AddAllObjects();
                Broadcast(new S.ObjectPushed { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                count++;
            }
            PreventSpellCheck = false;

            return count;
        }

        public bool InGroup(MapObject ob)
        {
            //Ob can be Null
            return ob?.GroupMembers != null && ob.GroupMembers == GroupMembers;
        }

        public int GetDC()
        {
            int min = Stats[Stat.MinDC];
            int max = Stats[Stat.MaxDC];
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
        public int GetMC()
        {
            int min = Stats[Stat.MinMC];
            int max = Stats[Stat.MaxMC];
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
        public int GetSC()
        {
            int min = Stats[Stat.MinSC];
            int max = Stats[Stat.MaxSC];
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
        public int GetSP()
        {
            int min = Math.Min(Stats[Stat.MinMC], Stats[Stat.MinSC]);
            int max = Math.Min(Stats[Stat.MaxMC], Stats[Stat.MaxSC]);
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
        public int GetAC()
        {
            int min = Stats[Stat.MinAC];
            int max = Stats[Stat.MaxAC];
            int defensiveMastery = Stats[Stat.DefensiveMastery];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (defensiveMastery > 0)
            {
                if (defensiveMastery >= 10) return max;

                if (SEnvir.Random.Next(10) < defensiveMastery) return max;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        public int GetMR()
        {
            int min = Stats[Stat.MinMR];
            int max = Stats[Stat.MaxMR];

            if (min < 0) min = 0;
            if (min >= max) return max;

            return SEnvir.Random.Next(min, max + 1);
        }
    }

    public class Poison
    {
        public MapObject Owner;
        public PoisonType Type;
        public int Value;
        public TimeSpan TickFrequency;
        public int TickCount;
        public DateTime TickTime;
        public object Extra, Extra1;
        public bool CanKill;
    }
}

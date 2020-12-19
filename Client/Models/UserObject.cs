﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Client.Scenes.Views;
using Library;
using Library.Network.ServerPackets;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

namespace Client.Models
{
    public sealed class UserObject : PlayerObject
    {
        #region Stats
        public override Stats Stats
        {
            get { return _Stats; }
            set
            {
                _Stats = value;

                GameScene.Game.StatsChanged();
            }
        }
        private Stats _Stats = new Stats();
        #endregion

        #region Hermit Stats

        public Stats HermitStats
        {
            get { return _HermitStats; }
            set
            {
                if (_HermitStats == value) return;
                
                _HermitStats = value;

                GameScene.Game.StatsChanged();
            }
        }
        private Stats _HermitStats = new Stats();

        #endregion

        #region Level
        public override int Level
        {
            get { return _level; }
            set
            {
                _level = value;

                GameScene.Game.LevelChanged();
            }
        }
        private int _level;
        #endregion

        #region Class
        public override MirClass Class
        {
            get { return _Class; }
            set
            {
                _Class = value;

                GameScene.Game.ClassChanged();
            }
        }
        private MirClass _Class;
        #endregion

        #region Experience
        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                _Experience = value;

                GameScene.Game.ExperienceChanged();
            }
        }
        private decimal _Experience;
        #endregion
        
        #region MaxExperience
        public decimal MaxExperience
        { 
            get { return _MaxExperience; }
            set
            {
                _MaxExperience = value;

                GameScene.Game.ExperienceChanged();
            }
        }
        private decimal _MaxExperience;
        #endregion

        #region CurrentHP
        public override int CurrentHP
        {
            get { return _CurrentHP; }
            set
            {
                _CurrentHP = value;

                GameScene.Game.HealthChanged();
            }
        }
        private int _CurrentHP;
        #endregion

        #region CurrentMP
        public override int CurrentMP
        {
            get { return _CurrentMP; }
            set
            {
                _CurrentMP = value;

                GameScene.Game.ManaChanged();
            }
        }
        private int _CurrentMP;
        #endregion

        #region AttackMode
        public AttackMode AttackMode
        {
            get { return _AttackMode; }
            set
            {
                _AttackMode = value;

                GameScene.Game.AttackModeChanged();
            }
        }
        private AttackMode _AttackMode;
        #endregion

        #region AttackMode
        public PetMode PetMode
        {
            get { return _PetMode; }
            set
            {
                _PetMode = value;

                GameScene.Game.PetModeChanged();
            }
        }
        private PetMode _PetMode;
        #endregion

        #region Gold
        public long Gold
        {
            get { return _Gold; }
            set
            {
                _Gold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private long _Gold;
        #endregion

        #region Game Gold
        public int GameGold
        {
            get { return _GameGold; }
            set
            {
                _GameGold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private int _GameGold;
        #endregion

        #region Hunt Gold
        public int HuntGold
        {
            get { return _HuntGold; }
            set
            {
                _HuntGold = value;

                GameScene.Game.GoldChanged();
            }
        }
        private int _HuntGold;
        #endregion
        

        public int BagWeight, WearWeight, HandWeight;

        public bool InSafeZone
        {
            get { return _InSafeZone; }
            set
            {
                if (_InSafeZone == value) return;
                
                _InSafeZone = value;

                GameScene.Game.SafeZoneChanged();
            }
        }
        private bool _InSafeZone;

        public int HermitPoints;
        


        public List<ClientBuffInfo> Buffs = new List<ClientBuffInfo>();
        
        public Dictionary<MagicInfo, ClientUserMagic> Magics = new Dictionary<MagicInfo, ClientUserMagic>();

        public DateTime NextActionTime, ServerTime, AttackTime, NextRunTime, NextMagicTime, BuffTime = CEnvir.Now, LotusTime, CombatTime, MoveTime;
        public MagicType AttackMagic;

        public ObjectAction MagicAction;
        public bool CanPowerAttack;

        public bool CanThrusting
        {
            get { return _canThrusting; }
            set
            {
                if (_canThrusting == value) return;
                
                _canThrusting = value;

                GameScene.Game.ReceiveChat(CanThrusting ? "Use Thrusting." : "Do not use Thrusting.", MessageType.Hint);
            }
        }
        private bool _canThrusting;

        public bool CanHalfMoon
        {
            get { return _CanHalfMoon; }
            set
            {
                if (_CanHalfMoon == value) return;

                _CanHalfMoon = value;

                GameScene.Game.ReceiveChat(CanHalfMoon ? "Use Half Moon Strike." : "Do not use Half Moon Strike.", MessageType.Hint);
            }
        }
        private bool _CanHalfMoon;
        public bool CanDestructiveBlow
        {
            get { return _CanDestructiveBlow; }
            set
            {
                if (_CanDestructiveBlow == value) return;
                _CanDestructiveBlow = value;

                GameScene.Game.ReceiveChat(CanDestructiveBlow ? "Use Destructive Blow." : "Do not use Destructive Blow.", MessageType.Hint);
            }
        }
        private bool _CanDestructiveBlow;

        public bool CanFlamingSword, CanDragonRise, CanBladeStorm;

        public bool CanFlameSplash
        {
            get { return _CanFlameSplash; }
            set
            {
                if (_CanFlameSplash == value) return;
                _CanFlameSplash = value;

                GameScene.Game.ReceiveChat(CanFlameSplash ? "Use Flame Splash." : "Do not use Flame Splash.", MessageType.Hint);
            }
        }
        private bool _CanFlameSplash;


        public UserObject(StartInformation info)
        {
            CharacterIndex = info.Index;

            ObjectID = info.ObjectID;

            Name = info.Name;
            NameColour = info.NameColour;

            Class = info.Class;
            Gender = info.Gender;

            Title = info.GuildName;
            GuildRank = info.GuildRank;

            CurrentLocation = info.Location;
            Direction = info.Direction;

            CurrentHP = info.CurrentHP;
            CurrentMP = info.CurrentMP;

            Level = info.Level;
            Experience = info.Experience;

            HairType = info.HairType;
            HairColour = info.HairColour;

            ArmourShape = info.Armour;
            ArmourImage = info.ArmourImage;
            ArmourColour = info.ArmourColour;
            LibraryWeaponShape = info.Weapon;
            WingsShape = info.WingsShape;
            EmblemShape = info.EmblemShape;

            Poison = info.Poison;

            InSafeZone = info.InSafeZone;

            AttackMode = info.AttackMode;
            PetMode = info.PetMode;

            Horse = info.Horse;
            Dead = info.Dead;

            HorseShape = info.HorseShape;
            HelmetShape = info.HelmetShape;
            ShieldShape = info.Shield;

            Gold = info.Gold;
            GameScene.Game.DayTime = info.DayTime;
            GameScene.Game.GroupBox.AllowGroup = info.AllowGroup;

            HermitPoints = info.HermitPoints;

            foreach (ClientUserMagic magic in info.Magics)
                Magics[magic.Info] = magic;

            foreach (ClientBuffInfo buff in info.Buffs)
            {
                Buffs.Add(buff);
                VisibleBuffs.Add(buff.Type);
            }
            

            FiltersClass = info.FiltersClass;
            FiltersRarity = info.FiltersRarity;
            FiltersItemType = info.FiltersItemType;

            UpdateLibraries();

            SetFrame(new ObjectAction(!Dead ? MirAction.Standing : MirAction.Dead, Direction, CurrentLocation));
            
            GameScene.Game.FillItems(info.Items);

            foreach (ClientBeltLink link in info.BeltLinks)
            {
                if (link.Slot < 0 || link.Slot >= GameScene.Game.BeltBox.Links.Length) continue;

                GameScene.Game.BeltBox.Links[link.Slot].LinkInfoIndex = link.LinkInfoIndex;
                GameScene.Game.BeltBox.Links[link.Slot].LinkItemIndex = link.LinkItemIndex;
            }
            GameScene.Game.BeltBox.UpdateLinks();

            foreach (ClientAutoPotionLink link in info.AutoPotionLinks)
            {
                if (link.Slot < 0 || link.Slot >= GameScene.Game.AutoPotionBox.Links.Length) continue;

                GameScene.Game.AutoPotionBox.Links[link.Slot] = link;
            }
            GameScene.Game.AutoPotionBox.UpdateLinks();

            GameScene.Game.MapControl.AddObject(this);

        }
        public override void LocationChanged()
        {
            base.LocationChanged();

            GameScene.Game.MapControl.UpdateMapLocation();
            GameScene.Game.MapControl.FLayer.TextureValid = false;
        }

        public override void SetAction(ObjectAction action)
        {
            if (CEnvir.Now < ServerTime) return; //Next Server response Time.

            base.SetAction(action);

            switch (CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    TargetObject = null;
                    break;
                case MirAction.Standing:
                    if ((GameScene.Game.MapControl.MapButtons & MouseButtons.Right) != MouseButtons.Right)
                        GameScene.Game.CanRun = false;
                    break;
            }

            if (Interupt) return;

            NextActionTime = CEnvir.Now;

            foreach (TimeSpan delay in CurrentFrame.Delays)
                NextActionTime += delay;
        }
        public void AttemptAction(ObjectAction action)
        {
            if (CEnvir.Now < NextActionTime || ActionQueue.Count > 0) return;
            if (CEnvir.Now < ServerTime) return; //Next Server response Time.

            switch (action.Action)
            {
                case MirAction.Moving:
                    if (CEnvir.Now < MoveTime) return;
                    break;
                case MirAction.Attack:
                    action.Extra[2] = Functions.GetElement(Stats);
                    
                    if (GameScene.Game.Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
                    {
                        foreach (KeyValuePair<Stat, int> stats in GameScene.Game.Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                        {
                            switch (stats.Key)
                            {
                                case Stat.FireAffinity:
                                    action.Extra[2] = Element.Fire;
                                    break;
                                case Stat.IceAffinity:
                                    action.Extra[2] = Element.Ice;
                                    break;
                                case Stat.LightningAffinity:
                                    action.Extra[2] = Element.Lightning;
                                    break;
                                case Stat.WindAffinity:
                                    action.Extra[2] = Element.Wind;
                                    break;
                                case Stat.HolyAffinity:
                                    action.Extra[2] = Element.Holy;
                                    break;
                                case Stat.DarkAffinity:
                                    action.Extra[2] = Element.Dark;
                                    break;
                                case Stat.PhantomAffinity:
                                    action.Extra[2] = Element.Phantom;
                                    break;
                            }
                        }
                    }

                    MagicType attackMagic = MagicType.None;

                    if (AttackMagic != MagicType.None)
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != AttackMagic) continue;

                            if (CEnvir.Now < pair.Value.NextCast) break;

                            if (AttackMagic == MagicType.Karma)
                            {
                                if (Stats[Stat.Health] * pair.Value.Cost / 100 > CurrentHP || Buffs.All(x => x.Type != BuffType.Cloak))
                                    break;
                            }
                            else 
                                if (pair.Value.Cost > CurrentMP) break;


                            attackMagic = AttackMagic;
                            break;
                        }
                    }
                    
                    if (CanPowerAttack && TargetObject != null)
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.Slaying) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (CanThrusting && GameScene.Game.MapControl.CanEnergyBlast(action.Direction))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.Thrusting) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (CanHalfMoon && (TargetObject != null || (GameScene.Game.MapControl.CanHalfMoon(action.Direction) &&
                                                                 (GameScene.Game.MapControl.HasTarget(Functions.Move(CurrentLocation, action.Direction)) || attackMagic != MagicType.Thrusting))))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.HalfMoon) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }


                    if (CanDestructiveBlow && (TargetObject != null || (GameScene.Game.MapControl.CanDestructiveBlow(action.Direction) &&
                                                                        (GameScene.Game.MapControl.HasTarget(Functions.Move(CurrentLocation, action.Direction)) || attackMagic != MagicType.Thrusting))))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.DestructiveSurge) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }

                    if (attackMagic == MagicType.None && CanFlameSplash && (TargetObject != null || GameScene.Game.MapControl.CanDestructiveBlow(action.Direction)))
                    {
                        foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in Magics)
                        {
                            if (pair.Key.Magic != MagicType.FlameSplash) continue;

                            if (pair.Value.Cost > CurrentMP) break;

                            attackMagic = pair.Key.Magic;
                            break;
                        }
                    }


                    if (CanBladeStorm)
                        attackMagic = MagicType.BladeStorm;
                    else if (CanDragonRise)
                        attackMagic = MagicType.DragonRise;
                    else if (CanFlamingSword)
                        attackMagic = MagicType.FlamingSword;
                    

                    action.Extra[1] = attackMagic;
                    break;
                case MirAction.Mount:
                    return;
            }

            SetAction(action);

            int attackDelay;
            switch (action.Action)
            {
                case MirAction.Standing:
                    NextActionTime = CEnvir.Now + Globals.TurnTime;
                    CEnvir.Enqueue(new C.Turn { Direction = action.Direction });
                    if ((GameScene.Game.MapControl.MapButtons & MouseButtons.Right) != MouseButtons.Right)
                        GameScene.Game.CanRun = false;
                    break;
                case MirAction.Harvest:
                    NextActionTime = CEnvir.Now + Globals.HarvestTime;
                    CEnvir.Enqueue(new C.Harvest { Direction = action.Direction });
                    GameScene.Game.CanRun = false;
                    break;
                case MirAction.Moving:
                    MoveTime = CEnvir.Now + Globals.MoveTime;

                    CEnvir.Enqueue(new C.Move { Direction = action.Direction, Distance = MoveDistance });
                    GameScene.Game.CanRun = true;
                    break;
                case MirAction.Attack:
                    attackDelay = Globals.AttackDelay - Stats[Stat.AttackSpeed]*Globals.ASpeedRate;
                    attackDelay = Math.Max(800, attackDelay);
                    AttackTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay);

                    if (BagWeight > Stats[Stat.BagWeight])
                        AttackTime += TimeSpan.FromMilliseconds(attackDelay);

                    CEnvir.Enqueue(new C.Attack { Direction = action.Direction, Action = action.Action, AttackMagic = MagicType });
                    GameScene.Game.CanRun = false;
                    break;
                case MirAction.Spell:
                    NextMagicTime = CEnvir.Now + Globals.MagicDelay;
                    if (BagWeight > Stats[Stat.BagWeight])
                        NextMagicTime += Globals.MagicDelay;

                    CEnvir.Enqueue(new C.Magic { Direction = action.Direction, Action = action.Action, Type = MagicType, Target = AttackTargets?.Count > 0 ? AttackTargets[0].ObjectID : 0, Location = MagicLocations?.Count > 0 ? MagicLocations[0] : Point.Empty });
                    GameScene.Game.CanRun = false;
                    break;
                case MirAction.Mining:
                    attackDelay = Globals.AttackDelay - Stats[Stat.AttackSpeed] * Globals.ASpeedRate;
                    attackDelay = Math.Max(800, attackDelay);
                    AttackTime = CEnvir.Now + TimeSpan.FromMilliseconds(attackDelay);

                    if (BagWeight > Stats[Stat.BagWeight])
                        AttackTime += TimeSpan.FromMilliseconds(attackDelay);

                    CEnvir.Enqueue(new C.Mining { Direction = action.Direction });
                    GameScene.Game.CanRun = false;
                    break;
                default:
                    GameScene.Game.CanRun = false;
                    break;
            }
            ServerTime = CEnvir.Now.AddSeconds(5);

        }

        public override void Process()
        {
            base.Process();

            if (DrawColour == DefaultColour)
            {
                if (BagWeight > Stats[Stat.BagWeight] || WearWeight > Stats[Stat.WearWeight] || HandWeight > Stats[Stat.HandWeight])
                    DrawColour = Color.CornflowerBlue;
            }
            
            TimeSpan ticks = CEnvir.Now - BuffTime;
            BuffTime = CEnvir.Now;

            foreach (ClientBuffInfo buff in Buffs)
            {
                if (buff.Pause || buff.RemainingTime == TimeSpan.MaxValue) continue;
                buff.RemainingTime = Functions.Max(TimeSpan.Zero, buff.RemainingTime - ticks);
            }
        }

        public override void FrameIndexChanged()
        {
            base.FrameIndexChanged();

            switch (CurrentAction)
            {
                case MirAction.Moving:
                    switch (CurrentAnimation)
                    {
                        case MirAnimation.HorseWalking:
                            if (FrameIndex == 1)
                                DXSoundManager.Play(SoundIndex.HorseWalk1);
                            if (FrameIndex == 4)
                                DXSoundManager.Play(SoundIndex.HorseWalk2);
                            break;
                        case MirAnimation.HorseRunning:
                            if (FrameIndex != 1) return;
                            DXSoundManager.Play(SoundIndex.HorseRun);
                            break;
                        default:
                            if (FrameIndex != 1 && FrameIndex != 4) return;
                            DXSoundManager.Play((SoundIndex)((int)SoundIndex.Foot1 + CEnvir.Random.Next((int)SoundIndex.Foot4 - (int)SoundIndex.Foot1) + 1));
                            break;
                    }

                    break;
            }
        }
        public override void MovingOffSetChanged()
        {
            base.MovingOffSetChanged();
            GameScene.Game.MapControl.FLayer.TextureValid = false;
        }


        public override void NameChanged()
        {
            base.NameChanged();

            GameScene.Game.CharacterBox.GuildNameLabel.Text = Title;
            GameScene.Game.CharacterBox.GuildRankLabel.Text = GuildRank;

            GameScene.Game.CharacterBox.CharacterNameLabel.Text = Name;
            GameScene.Game.TradeBox.UserLabel.Text = Name;
        }
    }
}

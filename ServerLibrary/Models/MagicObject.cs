﻿using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models
{
    //TODO - Send list to client of which magics are toggled, casted, targetted etc (allows removing of lots of client switches)
    //TODO - Add MonsterObject methods
    //TODO - Add client side version of MagicObject

    public abstract class MagicObject
    {
        public PlayerObject Player { get; }
        public UserMagic Magic { get; }

        public MagicType Type => Magic.Info.Magic;
        public List<DelayedAction> ActionList => Player.ActionList;
        public Map CurrentMap => Player.CurrentMap;
        public Point CurrentLocation => Player.CurrentLocation;
        public MirDirection Direction => Player.Direction;


        protected abstract Element Element { get; }
        public virtual bool UpdateCombatTime => true;


        //Magic variables
        public virtual bool MagicSkill => false; //TODO
        public virtual bool AugmentedSkill => false;
        public virtual bool CanStruck => true;
        protected virtual int Slow => 0;
        protected virtual int SlowLevel => 0;
        protected virtual int Repel => 0;
        protected virtual int Silence => 0;
        protected virtual int Shock => 0;


        //Attack variables
        public virtual bool AttackSkill => false;
        public virtual bool IgnoreAccuracy => false;
        public virtual bool HasFlameSplash(bool primary)
        {
            return false;
        }
        public virtual bool HasLotus => false;
        public virtual bool HasDestructiveSurge(bool primary)
        {
            return false;
        }
        public virtual bool HasBladeStorm => false;
        public virtual bool HasDanceOfSallows => false;
        public virtual bool HasMassacre => false;
        public virtual bool HasSwiftBlade => false;
        public virtual bool HasSeismicSlam => false;


        public MagicObject(PlayerObject player, UserMagic magic) 
        {
            Player = player;
            Magic = magic;
        }

        public virtual Element GetElement(Element currentElement, Stats stats = null)
        {
            return Element;
        }

        public virtual bool CheckCost()
        {
            if (Magic.Cost > Player.CurrentMP)
            {
                return false;
            }

            return true;
        }

        public virtual MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            return new MagicCast();
        }

        public virtual void MagicComplete(params object[] data)
        {

        }

        public virtual void AttackLocations(List<MagicType> magics)
        {

        }

        public virtual void MagicConsume()
        {
            if (Magic.Info.School == MagicSchool.Discipline)
            {
                Player.ChangeFP(-Magic.Cost);
                return;
            }

            Player.ChangeMP(-Magic.Cost);
        }

        public virtual void MagicFinalise()
        {
            Player.BuffRemove(BuffType.Cloak);
            Player.BuffRemove(BuffType.Transparency);
        }

        public virtual void ResetCombatTime()
        {
            if (UpdateCombatTime)
            {
                Player.CombatTime = SEnvir.Now;
            }
        }

        public virtual void Process()
        {

        }

        public virtual void RefreshToggle()
        {
            
        }

        public virtual void Toggle(bool canUse)
        {

        }

        public virtual void Cooldown(int attackDelay)
        {

        }

        public virtual AttackCast AttackCast(MagicType attackType)
        {
            return new AttackCast();
        }

        protected DateTime GetDelayFromDistance(int start, MapObject target)
        {
            var delay = SEnvir.Now.AddMilliseconds(start + Functions.Distance(Player.CurrentLocation, target.CurrentLocation) * 48);

            return delay;
        }

        public virtual int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            return power;
        }

        public virtual int ModifyPowerMultiplier(bool primary, int power, Stats stats = null)
        {
            return power;
        }

        public virtual Stats GetPassiveStats()
        {
            return new Stats();
        }

        public virtual int GetSlow(int slow, Stats stats = null)
        {
            return Slow;
        }

        public virtual int GetSlowLevel(int slowLevel, Stats stats = null)
        {
            return SlowLevel;
        }

        public virtual int GetRepel(int repel, Stats stats = null)
        {
            return Repel;
        }

        public virtual int GetSilence(int silence, Stats stats = null)
        {
            return Silence;
        }

        public virtual int GetShock(int shock, Stats stats = null)
        {
            return Shock;
        }

        protected UserMagic GetAugmentedSkill(MagicType type)
        {
            if (Player.Magics.TryGetValue(type, out UserMagic augMagic))
            {
                return augMagic;
            }

            return null;
        }
    }

    public class MagicCast
    {
        /// <summary>
        /// List of locations spell has locked on to
        /// </summary>
        public List<Point> Locations = new ();

        /// <summary>
        /// List of targets spell has locked on to
        /// </summary>
        public List<uint> Targets = new ();

        /// <summary>
        /// Targetted object for the spell. Will be used to automatically update players direction to face target
        /// </summary>
        public MapObject Ob = null;

        /// <summary>
        /// has the spell been cast, sends cooldown to the client
        /// </summary>
        public bool Cast = true;

        /// <summary>
        /// Should the players direction be overwritten
        /// </summary>
        public MirDirection? Direction = null;

        /// <summary>
        /// Dont send a return packet to the client
        /// </summary>
        public bool Return = false;
    }

    public class AttackCast
    {
        public List<MagicType> Magics = new List<MagicType>();
        public bool Cast = false;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MagicTypeAttribute : Attribute
    {
        public readonly MagicType Type;
        public MagicTypeAttribute(MagicType type)
        {
            Type = type;
        }
    }
}

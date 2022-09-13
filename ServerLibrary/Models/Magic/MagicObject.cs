﻿using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models
{
    public abstract class MagicObject
    {
        public PlayerObject Player { get; }
        public UserMagic Magic { get; }
        public MagicType Type => Magic.Info.Magic;

        public abstract Element Element { get; }

        public virtual bool UpdateCombatTime => true;

        //Attack variables
        public virtual bool PhysicalSkill => false;
        public virtual bool HasAttackAnimation => true;

        //MagicAttack variables
        public virtual bool CanStruck => true;
        protected virtual int Slow => 0;
        protected virtual int SlowLevel => 0;
        protected virtual int Repel => 0;
        protected virtual int Silence => 0;
        protected virtual int Shock => 0;

        public MagicObject(PlayerObject player, UserMagic magic) 
        {
            Player = player;
            Magic = magic;
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

        public virtual bool CanAttack(MagicType attackType)
        {
            return false;
        }

        public virtual void Attack()
        {

        }

        protected DateTime GetDelayFromDistance(int start, MapObject target)
        {
            var delay = SEnvir.Now.AddMilliseconds(start + Functions.Distance(Player.CurrentLocation, target.CurrentLocation) * 48);

            return delay;
        }

        public virtual int ModifyPower1(bool primary, int power)
        {
            return power;
        }

        public virtual int ModifyPower2(bool primary, int power)
        {
            return power;
        }

        public virtual Stats GetPassiveStats()
        {
            return new Stats();
        }

        public virtual int GetSlow()
        {
            return Slow;
        }

        public virtual int GetSlowLevel()
        {
            return SlowLevel;
        }

        public virtual int GetRepel()
        {
            return Repel;
        }

        public virtual int GetSilence()
        {
            return Silence;
        }

        public virtual int GetShock()
        {
            return Shock;
        }

        //TODO - Send list to client of which magics are toggled, casted, targetted etc (allows removing of lots of switches)
        //TODO - Create new Attack method passing in MagicType instead
    }

    public class MagicCast
    {
        public List<Point> Locations = new List<Point>();
        public List<uint> Targets = new List<uint>();
        public MapObject Ob;
        public bool Cast = true;
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

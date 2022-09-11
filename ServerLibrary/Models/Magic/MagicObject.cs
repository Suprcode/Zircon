using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models
{
    public abstract class MagicObject
    {
        public PlayerObject Player { get; set; }
        public UserMagic Magic { get; set; }
        public virtual bool UpdateCombatTime => true;

        public abstract Element Element { get; }

        public MagicObject(PlayerObject player, UserMagic magic) 
        {
            Player = player;
            Magic = magic;
        }

        public virtual int GetPower()
        {
            return 0;
        }

        public virtual bool CheckCost()
        {
            if (Magic.Cost > Player.CurrentMP)
            {
                return false;
            }

            return true;
        }

        public abstract MagicCast Cast(MapObject target, Point location);

        public abstract void Complete(params object[] data);

        public virtual void Consume()
        {
            if (Magic.Info.School == MagicSchool.Discipline)
            {
                Player.ChangeFP(-Magic.Cost);
                return;
            }

            Player.ChangeMP(-Magic.Cost);
        }

        public virtual void Finalise()
        {
        }

        public virtual void ResetCombatTime()
        {
            if (UpdateCombatTime)
            {
                Player.CombatTime = SEnvir.Now;
            }
        }

        public virtual void Toggle()
        {

        }

        protected DateTime GetDelayFromDistance(int start, MapObject target)
        {
            var delay = SEnvir.Now.AddMilliseconds(start + Functions.Distance(Player.CurrentLocation, target.CurrentLocation) * 48);

            return delay;
        }

        //TODO - Send list to client of which magics are toggled, casted, targetted etc (allows removing of lots of switches)
        //TODO - Add GetPower method, GetSlow, GetElement Method etc call this within MagicAttack instead?
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

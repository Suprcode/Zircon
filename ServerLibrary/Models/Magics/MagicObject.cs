using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FireBall)]
    public class FireBall : MagicObject
    {
        public FireBall(MagicInfo info) : base(info)
        {
        }

        public override MagicCastResponse Cast(UserMagic magic, PlayerObject player, MapObject target, Point location)
        {
            var response = new MagicCastResponse();

            if (!player.CanAttackTarget(target))
            {
                response.locations.Add(location);
                return response;
            }

            response.targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(player.CurrentLocation, target.CurrentLocation) * 48);

            player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, new List<UserMagic> { magic }, target));

            return response;
        }

        public override void Complete(UserMagic magic, PlayerObject player)
        {
            //element = Element.Fire;
            //power += magic.GetPower() + GetMC();
        }
    }

    public abstract class MagicObject
    {
        public readonly MagicInfo Info;

        public MagicObject(MagicInfo info)
        {
            Info = info;
        }

        public virtual bool CheckCost(UserMagic magic, PlayerObject player)
        {
            if (magic.Cost > player.CurrentMP)
            {
                return false;
            }

            return true;
        }

        public abstract MagicCastResponse Cast(UserMagic magic, PlayerObject player, MapObject target, Point location);

        public abstract void Complete(UserMagic magic, PlayerObject player);

        public virtual void Consume(UserMagic magic, PlayerObject player)
        {
            if (magic.Info.School == MagicSchool.Discipline)
            {
                player.ChangeFP(-magic.Cost);
                return;
            }
            
            player.ChangeMP(-magic.Cost);
        }

        public virtual void Finalise(UserMagic magic, PlayerObject player)
        {

        }

        public virtual void Toggle(UserMagic magic, PlayerObject player)
        {

        }
    }

    public class MagicCastResponse
    {
        public List<Point> locations = new List<Point>();
        public List<uint> targets = new List<uint>();
        public MapObject ob;
        public bool cast;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class MagicTypeAttribute : Attribute
    {
        public readonly MagicType Type;
        public MagicTypeAttribute(MagicType type)
        {
            Type = type;
        }
    }
}

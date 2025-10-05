using System;

namespace Server.Models
{
    public sealed class DelayedAction
    {
        public DateTime Time;
        public ActionType Type;
        public object[] Data;


        public DelayedAction(DateTime time, ActionType type, params object[] data)
        {
            Time = time;
            Type = type;
            Data = data;
        }

    }

    public enum ActionType
    {
        Turn,
        Move,
        Mount,
        Harvest,
        Mining,
        Fishing,
        Attack,
        Magic,
        RangeAttack,
        DelayAttack,
        DelayMagic,
        BroadCastPacket,
        Function,
        DelayedAttackDamage,
        DelayedMagicDamage
    }
}

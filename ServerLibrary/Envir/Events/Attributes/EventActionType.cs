using Library.SystemModels;
using System;

namespace Server.Envir.Events
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventActionTypeAttribute : Attribute
    {
        public readonly EventActionType Type;
        public EventActionTypeAttribute(EventActionType type)
        {
            Type = type;
        }
    }
}

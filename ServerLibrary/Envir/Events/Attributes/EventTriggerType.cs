using System;

namespace Server.Envir.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EventTriggerTypeAttribute : Attribute
    {
        public string TriggerName { get; set; }
        public EventTriggerTypeAttribute(string name)
        {
            TriggerName = name;
        }
    }
}

using System;

namespace MirDB
{
    [Flags]
    public enum SessionMode
    {
        None = 0,
        System = 1,
        Users = 2,
        Both = System | Users,
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class UserObject : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreProperty : Attribute
    {

    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class Association : Attribute
    {
        public string Identity { get; }
        public bool Aggregate { get; }

        public Association(string identity)
        {
            Identity = identity;
        }
        public Association(bool aggregate)
        {
            Aggregate = aggregate;
        }
        public Association(string identity, bool aggregate)
        {
            Identity = identity;
            Aggregate = aggregate;
        }
    }
}

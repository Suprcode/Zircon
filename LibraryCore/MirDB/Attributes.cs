using System;

namespace MirDB
{
    [Flags]
    public enum SessionMode
    {
        None = 0,
        System = 1,
        Users = 2,
        Both = System | Users
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UserObjectAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {

    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AssociationAttribute : Attribute
    {
        public string Identity { get; }
        public bool Aggregate { get; }

        public AssociationAttribute(string identity)
        {
            Identity = identity;
        }
        public AssociationAttribute(bool aggregate)
        {
            Aggregate = aggregate;
        }
        public AssociationAttribute(string identity, bool aggregate)
        {
            Identity = identity;
            Aggregate = aggregate;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IsIdentityAttribute : Attribute
    {

    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Library.Network.ServerPackets;
using MirDB;

namespace Library.MirDB
{
    internal class DBRelationship
    {
        public Type Type { get; set; }
        public Dictionary<int, DBRelationshipTargets> LinkTargets = new Dictionary<int, DBRelationshipTargets>();

        public DBRelationship(Type type)
        {
            Type = type;
        }

        public void ConsumeKeys(Session session)
        {

            foreach (KeyValuePair<int, DBRelationshipTargets> pair in LinkTargets)
            {
                DBObject linkOb = session.GetObject(Type, pair.Key);

                foreach (KeyValuePair<PropertyInfo, ConcurrentQueue<DBObject>> target in pair.Value.PropertyTargets)
                {
                    while (!target.Value.IsEmpty)
                    {
                        if (!target.Value.TryDequeue(out DBObject targetOb)) continue;

                        target.Key.SetValue(targetOb, linkOb);
                    }
                }
                pair.Value.PropertyTargets.Clear();
            }
            LinkTargets.Clear();

        }
    }

    internal class DBRelationshipTargets
    {
        public Dictionary<PropertyInfo, ConcurrentQueue<DBObject>> PropertyTargets = new Dictionary<PropertyInfo, ConcurrentQueue<DBObject>>();
    }
}

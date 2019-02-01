using System;
using Library;
using Library.SystemModels;
using Server.DBModels;

namespace Server.Models
{
    public class ItemCheck
    {
        public UserItem Item { get; set; }
        public ItemInfo Info { get; set; }
        public long Count { get; set; }

        public UserItemFlags Flags { get; set; }
        public TimeSpan ExpireTime { get; set; }

        public Stats Stats { get; set; }

        public ItemCheck(UserItem item, long count, UserItemFlags flags, TimeSpan time)
        {
            Item = item;
            Info = item.Info;
            Count = count;
            Flags = flags;

            ExpireTime = time;
            Stats = new Stats(item.Stats);
        }

        public ItemCheck(ItemInfo info, long count, UserItemFlags flags, TimeSpan time)
        {
            Info = info;
            Count = count;
            Flags = flags;

            ExpireTime = time;

            Stats = new Stats();
        }
    }
}
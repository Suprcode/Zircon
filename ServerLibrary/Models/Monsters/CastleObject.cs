using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;

namespace Server.Models.Monsters
{
    public class CastleObject : MonsterObject
    {
        public CastleInfo Castle;

        public ConquestWar War;

        public GuildInfo Guild = null;

        protected override void OnSpawned()
        {
            base.OnSpawned();

            if (Castle == null)
                throw new InvalidOperationException("Cannot spawn objective without a castle.");

            Guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == Castle);
        }

        public override void Process()
        {
            base.Process();

            RefreshGuild();
        }

        public virtual void RefreshGuild()
        {
            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == Castle);

            if (ownerGuild != Guild)
            {
                Guild = ownerGuild;

                Refresh();
            }
        }

        public virtual void Refresh()
        {
        }
    }
}

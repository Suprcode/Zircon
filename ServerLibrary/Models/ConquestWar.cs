using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public sealed class ConquestWar
    {
        public DateTime StartTime, EndTime;
        public CastleInfo Castle;

        public List<GuildInfo> Participants;
        public Map Map;

        public CastleObject CastleTarget;
        public bool Ended;

        public Dictionary<CharacterInfo, UserConquestStats> Stats = new Dictionary<CharacterInfo, UserConquestStats>();

        public void StartWar()
        {
            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat(string.Format(con.Language.ConquestStarted, Castle.Name), MessageType.System);
            

            Map = SEnvir.GetMap(Castle.Map);

            for (int i = Map.NPCs.Count - 1; i >= 0; i--)
            {
                NPCObject npc = Map.NPCs[i];
             //   if (!Castle.CastleRegion.PointList.Contains(npc.CurrentLocation)) continue;
                
                npc.Visible = false;
                npc.RemoveAllObjects();
            }

            foreach (GuildInfo guild in Participants)
                guild.Conquest?.Delete();
            
            SEnvir.Broadcast(new S.GuildConquestStarted { Index = Castle.Index });

            PingPlayers();

            SpawnBoss();

            SEnvir.ConquestWars.Add(this);
        }

        public void Process()
        {
            if (SEnvir.Now < EndTime) return;
            
            EndWar();
        }

        
        public void EndWar()
        {
            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat(string.Format(con.Language.ConquestFinished, Castle.Name), MessageType.System);

            Ended = true;
            

            //for (int i = Map.NPCs.Count - 1; i >= 0; i--)
            //{
            //    NPCObject npc = Map.NPCs[i];
            //    if (!Castle.CastleRegion.PointList.Contains(npc.CurrentLocation)) continue;

            //    npc.Visible = true;
            //    npc.AddAllObjects();
            //}

            PingPlayers();

            DespawnBoss();

            SEnvir.ConquestWars.Remove(this);

            SEnvir.Broadcast(new S.GuildConquestFinished { Index = Castle.Index });

            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == Castle);


            if (ownerGuild != null)
            {
                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(string.Format(con.Language.ConquestOwner, ownerGuild.GuildName, Castle.Name), MessageType.System);

                UserConquest conquest = SEnvir.UserConquestList.Binding.FirstOrDefault(x => x.Castle == Castle && x.Castle == ownerGuild?.Castle);

                TimeSpan warTime = TimeSpan.MinValue;

                if (conquest != null)
                    warTime = (conquest.WarDate + conquest.Castle.StartTime) - SEnvir.Now;

                foreach (GuildMemberInfo member in ownerGuild.Members)
                {
                    if (member.Account.Connection?.Player == null) continue; //Offline
                    
                    member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = Castle.Index, WarTime = warTime, ObserverPacket = false });
                }
            }
            
            foreach (GuildInfo participant in Participants)
            {
                if (participant == ownerGuild) continue;

                foreach (GuildMemberInfo member in participant.Members)
                {
                    if (member.Account.Connection?.Player == null) continue; //Offline

                    member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = Castle.Index, WarTime = TimeSpan.MinValue, ObserverPacket = false });
                }
            }
        }

        public void PingPlayers()
        {
            foreach (PlayerObject player in Map.Players)
            {
                //if (!Castle.CastleRegion.PointList.Contains(player.CurrentLocation)) continue;

                if (player.Character.Account.GuildMember?.Guild?.Castle == Castle) continue;

                player.Teleport(Castle.AttackSpawnRegion, null, 0);
            }
        }

        public void DespawnBoss()
        {
            if (CastleTarget == null) return;

            CastleTarget.EXPOwner = null;
            CastleTarget.War = null;
            CastleTarget.Die();
            CastleTarget.Despawn();
            CastleTarget = null;
        }
        public void SpawnBoss()
        {
            if (Castle.Monster != null)
            {
                switch (Castle.Monster.AI)
                {
                    case 1000: //CastleLord
                        CastleTarget = new CastleLord
                        {
                            MonsterInfo = Castle.Monster,
                            War = this,
                        };

                        CastleTarget.Spawn(Castle.CastleRegion, null, 0);
                        break;
                    case 1001: //CastleFlag
                        CastleTarget = new CastleFlag
                        {
                            MonsterInfo = Castle.Monster,
                            War = this,
                        };

                        CastleTarget.Spawn(Castle.CastleRegion, null, 0);
                        break;
                }
            }
        }

        public UserConquestStats GetStat(CharacterInfo character)
        {
            UserConquestStats user;

            if (!Stats.TryGetValue(character, out user))
            {
                user = SEnvir.UserConquestStatsList.CreateNewObject();

                user.Character = character;

                user.WarStartDate = StartTime;
                user.CastleName = Castle.Name;

                user.GuildName = character.Account.GuildMember?.Guild.GuildName ?? "No Guild.";
                user.CharacterName = character.CharacterName;
                user.Class = character.Class;
                user.Level = character.Level;

                Stats[character] = user;
            }
            
            return user;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CastleLord : CastleObjective
    {
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker == null || attacker.Race != ObjectType.Player) return 0;

            PlayerObject player = (PlayerObject) attacker;

            if (War == null) return 0;

            if (player.Character.Account.GuildMember == null) return 0;

            if (player.Character.Account.GuildMember.Guild.Castle != null) return 0;

            if (War.Participants.Count > 0 && !War.Participants.Contains(player.Character.Account.GuildMember.Guild)) return 0;

            int result = base.Attacked(attacker, 1, element, canReflect, ignoreShield, canCrit);

            #region Conquest Stats

            switch (attacker.Race)
            {
                case ObjectType.Player:
                    UserConquestStats conquest = SEnvir.GetConquestStats((PlayerObject) attacker);

                    if (conquest != null)
                        conquest.BossDamageDealt += result;
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject) attacker;
                    if (mob.PetOwner != null)
                    {
                        conquest = SEnvir.GetConquestStats(mob.PetOwner);

                        if (conquest != null)
                            conquest.BossDamageDealt += result;
                    }
                    break;
            }

            #endregion


            EXPOwner = null;


            return result;
        }

        public override bool ApplyPoison(Poison p)
        {
            return false;
        }

        public override void ProcessRegen() { }
        public override bool ShouldAttackTarget(MapObject ob)
        {
            if (Passive || ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }

            if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility) && !CoolEye) return false;
            if (ob.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                if (!CoolEye) return false;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, 2)) return false;
                if (ob.Level >= Level) return false;
            }
            if (ob.Buffs.Any(x => x.Type == BuffType.Transparency)) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject) ob;
                    if (player.GameMaster) return false;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.Castle;
                default:
                    throw new NotImplementedException();
            }
        }
        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || War == null) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                case ObjectType.Monster:
                    return false;
            }
            
            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject) ob;

                    if (player.GameMaster) return false;

                    return player.Character.Account.GuildMember?.Guild.Castle != War.Castle;
                default:
                    throw new NotImplementedException();
            }

        }

        public override void Die()
        {
            if (War != null)
            {
                if (EXPOwner?.Node == null) return;

                if (EXPOwner.Character.Account.GuildMember == null) return;

                if (EXPOwner.Character.Account.GuildMember.Guild.Castle != null) return;

                if (War.Participants.Count > 0 && !War.Participants.Contains(EXPOwner.Character.Account.GuildMember.Guild)) return;

                #region Conquest Stats

                UserConquestStats conquest = SEnvir.GetConquestStats((PlayerObject) EXPOwner);

                if (conquest != null)
                    conquest.BossKillCount++;

                #endregion

                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == War.Castle);

                if (ownerGuild != null)
                    ownerGuild.Castle = null;

                EXPOwner.Character.Account.GuildMember.Guild.Castle = War.Castle;

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(string.Format(con.Language.ConquestCapture, EXPOwner.Character.Account.GuildMember.Guild.GuildName, War.Castle.Name), MessageType.System);

                SEnvir.Broadcast(new S.GuildCastleInfo { Index = War.Castle.Index, Owner = EXPOwner.Character.Account.GuildMember.Guild.GuildName });

                War.CastleBoss = null;

                War.PingPlayers();
                War.SpawnBoss();

                if (War.EndTime - SEnvir.Now < TimeSpan.FromMinutes(15))
                    War.EndTime = SEnvir.Now.AddMinutes(15);

                foreach (PlayerObject player in SEnvir.Players)
                    player.ApplyCastleBuff();


                War = null;
            }

            base.Die();
        }
    }
}

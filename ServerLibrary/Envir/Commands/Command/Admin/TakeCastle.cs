using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class TakeCastle : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "TAKECASTLE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => string.Compare(x.Name.Replace(" ", ""), vals[1], StringComparison.OrdinalIgnoreCase) == 0);
            if (castle == null)
                throw new UserCommandException(string.Format("Could not find castle {0}.", vals[1]));

            if (player.Character.Account.GuildMember?.Guild == null)
            {
                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);
                if (ownerGuild == null)
                    throw new UserCommandException(string.Format("No guild currently owns {0} castle.", castle.Name));
                ownerGuild.Castle = null;

                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            con.ReceiveChat(string.Format(con.Language.ConquestLost, ownerGuild.GuildName, castle.Name), MessageType.System);
                            break;
                        default:
                            continue;
                    }
                }

                SEnvir.Broadcast(new S.GuildCastleInfo { Index = castle.Index, Owner = string.Empty });

                foreach (PlayerObject user in SEnvir.Players)
                    user.ApplyCastleBuff();

                return;
            }
            else
            {
                player.Character.Account.GuildMember.Guild.Castle = castle;
                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            con.ReceiveChat(string.Format(con.Language.ConquestCapture, player.Character.Account.GuildMember.Guild.GuildName, castle.Name), MessageType.System);
                            break;
                        default:
                            continue;
                    }
                }

                SEnvir.Broadcast(new S.GuildCastleInfo { Index = castle.Index, Owner = player.Character.Account.GuildMember.Guild.GuildName });
                foreach (PlayerObject user in SEnvir.Players)
                    user.ApplyCastleBuff();
            }
        }
    }
}

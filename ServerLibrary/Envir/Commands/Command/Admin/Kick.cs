using Library;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using G = Library.Network.GeneralPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class Kick : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "KICK";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CharacterInfo character = SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}.", vals[1]));

            if (character.Account.Connection == null)
                throw new UserCommandException(string.Format("Player {0} is not online.", vals[1]));

            if (player.Character == character)
                throw new UserCommandException("You cannot kick yourself.");

            character.Account.Connection.SendDisconnect(new G.Disconnect { Reason = DisconnectReason.Kicked });
        }
    }
}

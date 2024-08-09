using Library;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using G = Library.Network.GeneralPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class Ban : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "BAN";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CharacterInfo character = SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}.", vals[1]));

            int count;
            if (vals.Length < 3 || !int.TryParse(vals[2], out count))
                count = 1440 * 365; //365days

            character.Account.Banned = true;
            character.Account.BanReason = $"{player.Name} banned by command.";
            character.Account.BanExpiry = SEnvir.Now.AddMinutes(count);

            player.Connection.ReceiveChat($"You have banned {character.CharacterName} for {count} minutes.", MessageType.System);

            if (character.Account.Connection != null)
            {
                character.Account.Connection.SendDisconnect(new G.Disconnect { Reason = DisconnectReason.Banned });
            }
        }
    }
}

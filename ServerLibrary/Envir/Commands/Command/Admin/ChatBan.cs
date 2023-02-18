using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class ChatBan : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "CHATBAN";
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

            character.Account.ChatBanExpiry = SEnvir.Now.AddMinutes(count);
        }
    }
}

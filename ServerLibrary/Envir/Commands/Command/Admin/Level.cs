using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class Level : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "LEVEL";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            int value;
            if (vals.Length >= PARAMS_LENGTH + 1)
            {
                player = SEnvir.GetPlayerByCharacter(vals[1]);
                if (player == null)
                    throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));
                if (!int.TryParse(vals[2], out value) || value < 0)
                    ThrowNewInvalidParametersException();
            }
            else
            {
                if (vals.Length < PARAMS_LENGTH)
                    ThrowNewInvalidParametersException();
                if (!int.TryParse(vals[1], out value) || value < 0)
                    ThrowNewInvalidParametersException();
            }
            player.Level = value;
            player.LevelUp();
        }
    }
}

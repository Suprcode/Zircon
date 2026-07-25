using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class PromoteFame : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "PROMOTEFAME";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length >= PARAMS_LENGTH)
            {
                player = SEnvir.GetPlayerByCharacter(vals[1]);

                if (player == null)
                    throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));
            }

            player.PromoteFame();
        }
    }
}

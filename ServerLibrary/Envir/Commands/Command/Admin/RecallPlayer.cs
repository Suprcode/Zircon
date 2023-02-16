using Library;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class RecallPlayer : AbstractParameterizedUserCommand
    {
        public override string VALUE => "RECALL";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            PlayerObject targetPlayer = SEnvir.GetPlayerByCharacter(vals[1]);
            targetPlayer?.Teleport(player.CurrentMap, Functions.Move(player.CurrentLocation, player.Direction));
        }
    }
}

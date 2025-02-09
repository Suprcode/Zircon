using Server.Models;
using System.Collections.Generic;

namespace Server.Envir.Commands.Command.Player
{
    class Event : AbstractParameterizedCommand<IPlayerCommand>
    {
        public override string VALUE => "EVENT";
        public override int PARAMS_LENGTH => 1;

        public static Dictionary<string, string> LastCommand { get; set; } = [];

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            if (string.IsNullOrEmpty(vals[1]))
                return;

            LastCommand[player.Name] = vals[1];

            SEnvir.EventHandler.Process(player, "PLAYERCOMMAND");
        }
    }
}

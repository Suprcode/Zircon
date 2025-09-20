using Library;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class SetHermitStat : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "SETHERMITSTAT";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException(); //SetHermitStat Health 50

            if (!Enum.TryParse(vals[1], true, out Stat tstat))
                ThrowNewInvalidParametersException();
            if (!int.TryParse(vals[2], out int tamount))
                ThrowNewInvalidParametersException();

            player.Character.HermitStats[tstat] = tamount;

            player.RefreshStats();
        }
    }
}

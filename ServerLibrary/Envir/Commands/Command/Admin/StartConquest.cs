using Library.SystemModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class StartConquest : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "STARTCONQUEST";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => string.Compare(x.Name.Replace(" ", ""), vals[1], StringComparison.OrdinalIgnoreCase) == 0);
            if (castle == null)
                throw new UserCommandException(string.Format("Could not find castle: {0}", vals[1]));

            if (SEnvir.ConquestWars.Any(x => x.Castle == castle))
                throw new UserCommandException(string.Format("{0} is already at war.", vals[1]));

            SEnvir.StartConquest(castle, true);
        }
    }
}

using Library.SystemModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class MapMove : AbstractParameterizedUserCommand
    {
        public override string VALUE => "MOVE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            //TODO: make this smarter.. allow us to enter co-ordinates and/or default to RandomLocation();
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            MapInfo info = SEnvir.MapInfoList.Binding.FirstOrDefault(x => string.Compare(x.FileName, vals[1], StringComparison.OrdinalIgnoreCase) == 0);
            Map map = SEnvir.GetMap(info);
            if (map == null)
                throw new UserCommandException(string.Format("Could not find map with index: {0}", vals[1]));

            player.Teleport(map, map.GetRandomLocation());
        }
    }
}

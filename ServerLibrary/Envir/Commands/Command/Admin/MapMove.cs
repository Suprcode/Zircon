using Library.SystemModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class MapMove : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "MOVE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            int x = 0, y = 0;

            if (vals.Length == 4)
            {
                if (!int.TryParse(vals[2], out x))
                    throw new UserCommandException(string.Format("Failed to parse X coord: {0}", vals[2]));

                if (!int.TryParse(vals[3], out y))
                    throw new UserCommandException(string.Format("Failed to parse Y coord: {0}", vals[3]));
            }

            MapInfo info = SEnvir.MapInfoList.Binding.FirstOrDefault(x => string.Compare(x.FileName, vals[1], StringComparison.OrdinalIgnoreCase) == 0);
            Map map = SEnvir.GetMap(info);
            if (map == null)
                throw new UserCommandException(string.Format("Could not find map with index: {0}", vals[1]));

            if (x > 0 && y > 0)
            {
                if (x > map.Width || y > map.Height)
                    throw new UserCommandException(string.Format("Coords {0}:{1} outside of map boundary", x, y));

                player.Teleport(map, new Point(x, y));
                return;
            }

            player.Teleport(map, map.GetRandomLocation());
        }
    }
}

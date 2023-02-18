using Library;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class RemovePKPoints : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "REMOVEPKPOINTS";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length > 1)
            {
                player = SEnvir.GetPlayerByCharacter(vals[1]);
                if (player == null)
                    throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));
            }

            BuffInfo buffInfo = player.Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);
            if (buffInfo != null)
                player.BuffRemove(buffInfo);
        }
    }
}

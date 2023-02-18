using Server.Envir.Commands.Exceptions;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class SetCompanionLevel : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "SETCOMPANIONLEVEL";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            int level;
            if (!int.TryParse(vals[1], out level) || level <= 0 || level >= 16)
                ThrowNewInvalidParametersException();

            if (player.Companion == null)
                throw new UserCommandException(string.Format("{0} does not have a companion active.", player.Name));

            player.Companion.UserCompanion.Level = level;
            player.Enqueue(new S.CompanionUpdate
            {
                Level = level,
                Experience = player.Companion.UserCompanion.Experience,
                Hunger = player.Companion.UserCompanion.Hunger
            });
            player.Companion.CheckSkills();
        }
    }
}

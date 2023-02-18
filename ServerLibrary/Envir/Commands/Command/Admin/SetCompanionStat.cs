using Library;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class SetCompanionStat : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "SETCOMPANIONSTAT";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < 3)
                ThrowNewInvalidParametersException();

            Stat stat;
            int level, value;
            if (!int.TryParse(vals[1], out level))
                ThrowNewInvalidParametersException();
            if (!Enum.TryParse(vals[2], out stat))
                ThrowNewInvalidParametersException();
            if (!int.TryParse(vals[3], out value))
                ThrowNewInvalidParametersException();

            if (player.Companion == null)
                throw new UserCommandException("You do not have a companion.");
            if (player.Companion.UserCompanion.Level <= level)
                throw new UserCommandException("Companion is not a high enough level.");

            switch (level)
            {
                case 3:
                    player.Companion.UserCompanion.Level3 = new Stats { [stat] = value };
                    break;
                case 5:
                    player.Companion.UserCompanion.Level5 = new Stats { [stat] = value };
                    break;
                case 7:
                    player.Companion.UserCompanion.Level7 = new Stats { [stat] = value };
                    break;
                case 10:
                    player.Companion.UserCompanion.Level10 = new Stats { [stat] = value };
                    break;
                case 11:
                    player.Companion.UserCompanion.Level11 = new Stats { [stat] = value };
                    break;
                case 13:
                    player.Companion.UserCompanion.Level13 = new Stats { [stat] = value };
                    break;
                case 15:
                    player.Companion.UserCompanion.Level15 = new Stats { [stat] = value };
                    break;
                default:
                    throw new UserCommandException(string.Format("Companion level {0} does not exist.", level));
            }

            player.CompanionRefreshBuff();
            player.Enqueue(new S.CompanionSkillUpdate
            {
                Level3 = player.Companion.UserCompanion.Level3,
                Level5 = player.Companion.UserCompanion.Level5,
                Level7 = player.Companion.UserCompanion.Level7,
                Level10 = player.Companion.UserCompanion.Level10,
                Level11 = player.Companion.UserCompanion.Level11,
                Level13 = player.Companion.UserCompanion.Level13,
                Level15 = player.Companion.UserCompanion.Level15
            });
        }
    }
}

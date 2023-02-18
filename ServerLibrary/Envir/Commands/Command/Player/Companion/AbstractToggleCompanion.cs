using Library;
using Server.Envir.Commands.Command;
using Server.Envir.Commands.Command.Player;
using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Player {
    abstract class AbstractToggleCompanion : AbstractCommand<IPlayerCommand> {
        public override string VALUE => "ENABLELEVEL{0}";

        public override void Action(PlayerObject player) {
            int level = int.Parse(VALUE.Substring(11));
            bool result = ToggleCompanionLockFor(player, level);

            player.Connection.ReceiveChat(string.Format(
                result ?
                    player.Connection.Language.CompanionSkillEnabled :
                    player.Connection.Language.CompanionSkillDisabled
                , level), MessageType.System);
        }

        public bool ToggleCompanionLockFor(PlayerObject p, int level) {
            switch (level) {
                case 3:
                    return p.CompanionLevelLock3 = !p.CompanionLevelLock3;
                case 5:
                    return p.CompanionLevelLock5 = !p.CompanionLevelLock5;
                case 7:
                    return p.CompanionLevelLock7 = !p.CompanionLevelLock7;
                case 10:
                    return p.CompanionLevelLock10 = !p.CompanionLevelLock10;
                case 11:
                    return p.CompanionLevelLock11 = !p.CompanionLevelLock11;
                case 13:
                    return p.CompanionLevelLock13 = !p.CompanionLevelLock13;
                case 15:
                    return p.CompanionLevelLock15 = !p.CompanionLevelLock15;
                default:
                    throw new UserCommandFatalException(string.Format("Trying to run non-existant command: {0}.", string.Format(VALUE, level)));
            }
        }
    }
}

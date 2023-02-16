using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;

namespace Server.Envir.Commands {
    public abstract class AbstractUserCommand : UserCommand {
        public abstract string VALUE { get; }
        public abstract void Action(PlayerObject player);
    }

    public abstract class AbstractParameterizedUserCommand : ParameterizedUserCommand {
        public abstract string VALUE { get; }
        public abstract int PARAMS_LENGTH { get; }

        public abstract void Action(PlayerObject player, String[] vals);

        public UserCommandException ThrowNewInvalidParametersException() {
            throw new UserCommandException(string.Format("Invalid Parameters for command @{0}", VALUE));
        }
    }

    public interface UserCommand {
        String VALUE { get; }
    }

    public interface ParameterizedUserCommand : UserCommand {
        int PARAMS_LENGTH { get; }
    }
}

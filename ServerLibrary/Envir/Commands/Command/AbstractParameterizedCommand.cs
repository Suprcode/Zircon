using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Command
{
    public abstract class AbstractParameterizedCommand<CommandType> : IParameterizedCommand 
        where CommandType : ICommand
    {
        public abstract string VALUE { get; }
        public abstract int PARAMS_LENGTH { get; }

        public abstract void Action(PlayerObject player, string[] vals);

        public UserCommandException ThrowNewInvalidParametersException()
        {
            throw new UserCommandException(string.Format("Invalid Parameters for command @{0}", VALUE));
        }
    }
}

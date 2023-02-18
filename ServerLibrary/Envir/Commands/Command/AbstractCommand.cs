using Server.Models;

namespace Server.Envir.Commands.Command
{
    public abstract class AbstractCommand<CommandType> : ICommand 
        where CommandType : ICommand
    {
        public abstract string VALUE { get; }
        public abstract void Action(PlayerObject player);
    }
}

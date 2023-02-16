using Server.Envir.Commands.Exceptions;
using Server.Models;
using System.Collections.Generic;

namespace Server.Envir.Commands.Handler
{
    public abstract class AbstractCommandHandler : ValidatingCommandHandler
    {

        public List<UserCommand> Commands = new List<UserCommand>();

        public void InitializeCommands(List<UserCommand> Commands)
        {
            this.Commands = Commands;
        }

        public virtual bool IsAllowedByPlayer(PlayerObject player)
        {
            return false;
        }

        public virtual bool CommandExists(string command)
        {
            return Commands.Exists(userCommand => userCommand.VALUE.Equals(command.ToUpper()));
        }

        public virtual void Handle(PlayerObject player, string[] input)
        {
            if (IsAllowedByPlayer(player))
            {
                string CommandInput = input[0].ToUpper();

                UserCommand command = Commands.Find(userCommand => userCommand.VALUE.Equals(CommandInput));
                if (command == null)
                    throw new UserCommandException(string.Format("Command @{0} does not exist.", CommandInput));

                if (command is AbstractUserCommand)
                    (command as AbstractUserCommand).Action(player);
                else if (command is AbstractParameterizedUserCommand)
                    (command as AbstractParameterizedUserCommand).Action(player, input);
            }
        }
    }
}

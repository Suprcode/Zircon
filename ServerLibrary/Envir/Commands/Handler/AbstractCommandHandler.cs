using Server.Envir.Commands.Command;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server.Envir.Commands.Handler
{
    public abstract class AbstractCommandHandler<CommandType> : IValidatingCommandHandler 
        where CommandType : ICommand
    {

        public readonly List<ICommand> Commands = new List<ICommand>();

        public AbstractCommandHandler()
        {
            this.Commands = Assembly.GetAssembly(typeof(CommandType)).GetTypes()
                .Where(type => type.IsClass)
                .Where(type => !type.IsAbstract)
                .Where(type => type.IsSubclassOf(typeof(AbstractCommand<CommandType>)) || type.IsSubclassOf(typeof(AbstractParameterizedCommand<CommandType>)))
                .Select(type => (ICommand)Activator.CreateInstance(type))
                .ToList();
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

                ICommand command = Commands.Find(userCommand => userCommand.VALUE.Equals(CommandInput));
                if (command == null)
                    throw new UserCommandException(string.Format("Command @{0} does not exist.", CommandInput));

                if (command is AbstractParameterizedCommand<CommandType>)
                    (command as AbstractParameterizedCommand<CommandType>).Action(player, input);
                else if (command is AbstractCommand<CommandType>)
                    (command as AbstractCommand<CommandType>).Action(player);

            }
        }
    }
}

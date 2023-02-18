using Library;
using Server.Envir.Commands.Exceptions;
using Server.Envir.Commands.Handler;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Commands
{
    public class ErrorHandlingCommandHandler : ICommandHandler
    {
        private readonly List<IValidatingCommandHandler> CommandHandlers;

        public ErrorHandlingCommandHandler(params IValidatingCommandHandler[] CommandHandlers)
        {
            this.CommandHandlers = CommandHandlers.ToList();
        }

        public void Handle(PlayerObject player, string[] commandParts)
        {
            try
            {
                string commandIdentifier = commandParts[0].ToUpper();
                IValidatingCommandHandler matchingHandler = CommandHandlers
                    .Where(handler => handler.IsAllowedByPlayer(player))
                    .Where(handler => handler.CommandExists(commandIdentifier))
                    .FirstOrDefault();

                if (matchingHandler != null)
                {
                    matchingHandler.Handle(player, commandParts);
                }
                else
                {
                    player.Connection.ReceiveChat(
                        string.Format("Command @{0} does not exist.", commandIdentifier),
                        MessageType.System
                    );
                }
            }
            catch (UserCommandException exception)
            {
                player.Connection.ReceiveChat(exception.Message, MessageType.System);
                if (!exception.userOnly)
                {
                    foreach (SConnection connection in player.Connection.Observers)
                        connection.ReceiveChat(exception.Message, MessageType.System);
                }
            }
            catch (Exception exception)
            {
                SEnvir.Log("FatalCommandError [" + player.Name + "]: " + exception.Message);
                player.Connection.ReceiveChat("FatalCommandError: The error has been logged. Contact an admin.", MessageType.System);
            }
        }
    }
}

using Library;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class GiveHorse : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "GIVEHORSE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            CharacterInfo character = SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}.", vals[1]));

            if (!Enum.TryParse(vals[2], true, out HorseType type))
                throw new UserCommandException(string.Format("Could not find horse: {0}.", vals[2]));

            character.Account.Horse = type;

            if (character.Player != null)
            {
                character.Player.RemoveMount();

                character.Player.RefreshStats();

                if (character.Player.Character.Account.Horse != HorseType.None) character.Player.Mount();
            }

            player.Connection.ReceiveChat(string.Format("[GIVE HORSE] {0} Type: {1}", character.CharacterName, type.ToString()), MessageType.System);
        }
    }
}

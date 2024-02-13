using Library;
using Server.DBModels;
using Server.Envir.Commands.Command;
using Server.Envir.Commands.Command.Admin;
using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Admin {
    class TakeGameGold : AbstractParameterizedCommand<IAdminCommand> 
    {
        public override string VALUE => "TAKEGAMEGOLD";
        public override int PARAMS_LENGTH => 3;

        public override void Action(PlayerObject player, string[] vals) 
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CharacterInfo character = SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));

            int count;
            if (!int.TryParse(vals[2], out count))
                ThrowNewInvalidParametersException();

            character.Account.GameGold.Amount -= count;
            character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.GameGoldLost, count), MessageType.System);
            character.Player?.GameGoldChanged();

            player.Connection.ReceiveChat(string.Format("[TAKE GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
        }
    }
}

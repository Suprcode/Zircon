using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    public class RemovePlayerCaption : AbstractParameterizedCommand<IAdminCommand>
    {

        public override string VALUE => "REMOVECAPTION";
        public override int PARAMS_LENGTH => 1;


        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            string characterName = vals[1];

            CharacterInfo? character = SEnvir.GetCharacter(characterName);

            if (character is null)
                throw new UserCommandException(string.Format("Could not find player: {0}.", characterName));

            character.Caption = null;
            SEnvir.CharacterInfoList.RaisePropertyChanges = true;

            PlayerObject? activePlayer = SEnvir.Players.FirstOrDefault(x => x.Name == characterName);

            if (activePlayer is not null)
            {
                activePlayer.SendChangeUpdate();
            }

        }
    }
}


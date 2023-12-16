using Library;
using Server.DBModels;
using Server.Envir.Commands.Command;
using Server.Envir.Commands.Command.Admin;
using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Admin {
    class GiveGameGold : AbstractParameterizedCommand<IAdminCommand> {
        public override string VALUE => "GIVEGAMEGOLD";
        public override int PARAMS_LENGTH => 3;

        public override void Action(PlayerObject player, string[] vals) {
            CharacterInfo character = SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}.", vals[1]));

            int count;
            if (!int.TryParse(vals[2], out count))
                ThrowNewInvalidParametersException();

            character.Account.GameGold.Amount += count;
            character.Account.Connection?.ReceiveChat(string.Format(character.Account.Connection.Language.PaymentComplete, count), MessageType.System);
            character.Player?.GameGoldChanged();

            if (character.Account.Referral != null) {
                character.Account.Referral.HuntGold.Amount += count / 10;

                if (character.Account.Referral.Connection != null) {
                    character.Account.Referral.Connection.ReceiveChat(string.Format(character.Account.Referral.Connection.Language.ReferralPaymentComplete, count / 10), MessageType.System, null, 0);

                    if (character.Account.Referral.Connection.Stage == GameStage.Game)
                        character.Account.Referral.Connection.Player.HuntGoldChanged();
                }
            }
            player.Connection.ReceiveChat(string.Format("[GIVE GAME GOLD] {0} Amount: {1}", character.CharacterName, count), MessageType.System);
        }
    }
}

using Library;
using Server.Models;

namespace Server.Envir.Commands.Command.Player
{
    class GroupRoll : AbstractCommand<IPlayerCommand>
    {
        public override string VALUE => "ROLL";

        public override void Action(PlayerObject player)
        {
            if (player.GroupMembers != null)
            {
                int result = SEnvir.Random.Next(6) + 1;
                foreach (PlayerObject member in player.GroupMembers)
                    member.Connection.ReceiveChat(string.Format(member.Connection.Language.DiceRoll, player.Name, result, 6), MessageType.Group);
            }
        }
    }
}

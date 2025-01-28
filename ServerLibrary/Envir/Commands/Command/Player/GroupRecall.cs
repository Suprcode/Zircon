using Library;
using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Command.Player
{
    class GroupRecall : AbstractCommand<IPlayerCommand>
    {
        public override string VALUE => "RECALLGROUP";

        public override void Action(PlayerObject player)
        {
            if (player.Stats[Stat.RecallSet] <= 0)
                throw new UserCommandException("You do not have the ability to recall.");

            if (player.GroupMembers == null)
                throw new UserCommandException(player.Connection.Language.GroupNoGroup);

            if (player.GroupMembers[0] != player)
                throw new UserCommandException(player.Connection.Language.GroupNotLeader);

            if (player.CurrentMap.Instance != null && !player.CurrentMap.Instance.AllowTeleport)
                throw new UserCommandException(player.Connection.Language.GroupRecallMap);

            if (!player.CurrentMap.Info.AllowTT || !player.CurrentMap.Info.AllowRT || player.CurrentMap.Info.SkillDelay > 0)
                throw new UserCommandException(player.Connection.Language.GroupRecallMap);

            if (SEnvir.Now < player.Character.GroupRecallTime)
                throw new UserCommandException(player.Connection.Language.GroupRecallDelay);

            foreach (PlayerObject member in player.GroupMembers)
            {
                if (member.Dead || member == player)
                    continue;

                if (!member.CurrentMap.Info.AllowTT)
                {
                    member.Connection.ReceiveChatWithObservers(con => con.Language.GroupRecallFromMap, MessageType.System);
                    player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.GroupRecallMemberFromMap, member.Name), MessageType.System);
                    continue;
                }

                if (!member.Character.Account.AllowGroupRecall)
                {
                    member.Connection.ReceiveChatWithObservers(con => con.Language.GroupRecallNotAllowed, MessageType.System);

                    player.Connection.ReceiveChatWithObservers(con => string.Format(con.Language.GroupRecallMemberNotAllowed, member.Name), MessageType.System);
                    continue;
                }
                member.Teleport(player.CurrentMap, player.CurrentMap.GetRandomLocation(player.CurrentLocation, 10));
            }
            player.Character.GroupRecallTime = SEnvir.Now.AddMinutes(3);
        }
    }
}

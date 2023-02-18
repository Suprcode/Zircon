using Library;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class CreateGuild : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "CREATEGUILD";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            CharacterInfo character = vals.Length < 3 ? player.Character : SEnvir.GetCharacter(vals[1]);
            if (character == null)
                throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));
            if (player.Character.Account.GuildMember != null)
                throw new UserCommandException(string.Format("{0} already belongs to a guild.", character.CharacterName));

            var guildName = vals.Length < 3 ? vals[1] : vals[2];
            if (!Globals.GuildNameRegex.IsMatch(guildName))
                throw new UserCommandException(string.Format("{0} is not a valid guild name.", guildName));
            if (SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0) != null)
                throw new UserCommandException(string.Format("Guild already exists with name: {0}.", guildName));

            GuildInfo newGuild = CreateGuildFrom(guildName);
            GuildMemberInfo leaderRole = CreateLeaderRole();
            leaderRole.Guild = newGuild;
            leaderRole.Account = character.Account;
            player.SendGuildInfo();
        }

        private GuildInfo CreateGuildFrom(string guildName)
        {
            GuildInfo guildInfo;
            guildInfo = SEnvir.GuildInfoList.CreateNewObject();
            guildInfo.GuildName = guildName;
            guildInfo.MemberLimit = 10;
            guildInfo.StorageSize = 10;
            guildInfo.GuildLevel = 1;
            return guildInfo;
        }

        public GuildMemberInfo CreateLeaderRole()
        {
            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();
            memberInfo.Rank = "Guild Leader";
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = GuildPermission.Leader;
            return memberInfo;
        }
    }
}

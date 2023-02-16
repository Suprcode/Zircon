using Server.Envir.Commands.Admin;
using Server.Envir.Commands.Command.Admin;
using Server.Envir.Commands.Handler;
using Server.Models;
using System.Collections.Generic;

namespace Server.Envir.Commands
{

    public class AdminCommandHandler : AbstractCommandHandler
    {

        public AdminCommandHandler()
        {
            InitializeCommands(new List<UserCommand>() {
                new ChatBan(),
                new GlobalShoutBan(),

                new SetCompanionStat(),
                new SetCompanionLevel(),

                new CreateGuild(),
                new StartConquest(),
                new EndConquest(),
                new TakeCastle(),

                new Goto(),
                new MapMove(),
                new RecallPlayer(),

                new GiveSkills(),
                new LevelSkill(),

                new ToggleObserver(),
                new ToggleGameMaster(),

                new AddStat(),
                new CreateItem(),
                new Level(),
                new SpawnMob(),

                new GiveGameGold(),
                new TakeGameGold(),

                new Reboot(),
                new ForceGarbageCollection(),
                new ClearIPBlocks()
            });
        }

        public override bool IsAllowedByPlayer(PlayerObject player)
        {
            return player.Character.Account.TempAdmin
                && player.Character.Account.Admin;
        }
    }
}

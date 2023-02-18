using Library;
using Server.Envir.Commands.Exceptions;
using Server.Models;

namespace Server.Envir.Commands.Command.Admin
{
    class SpawnMonster : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "MONSTER";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            var monsterInfo = SEnvir.GetMonsterInfo(vals[1]);
            if (monsterInfo == null)
                throw new UserCommandException(string.Format("Could not find monster: {0}", vals[1]));

            int value = 0;
            if (vals.Length < 3 || !int.TryParse(vals[2], out value) || value == 0)
                value = 1;

            while (value > 0)
            {
                var monster = MonsterObject.GetMonster(monsterInfo);
                monster.Spawn(player.CurrentMap, Functions.Move(player.CurrentLocation, player.Direction));
                value -= 1;
            }
        }
    }
}

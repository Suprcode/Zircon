using Library;
using Library.SystemModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class LevelSkill : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "LEVELSKILL";
        public override int PARAMS_LENGTH => 4;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException(); //@levelskill John healing 5

            player = SEnvir.GetPlayerByCharacter(vals[1]);
            if (player == null)
                throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));

            MagicInfo tinfo = SEnvir.MagicInfoList.Binding.FirstOrDefault(m => m.Name.Replace(" ", "").ToUpper().Equals(vals[2].ToUpper()));
            if (tinfo == null)
                throw new UserCommandException(string.Format("Could not find spell: {0}", vals[2]));

            if (int.TryParse(vals[3], out int tlevel))
            {
                if (player.GetMagic(tinfo.Magic, out MagicObject magicObject))
                {
                    magicObject.Magic.Level = tlevel;
                    magicObject.Magic.Experience = 0;

                    player.Enqueue(new S.MagicLeveled { InfoIndex = tinfo.Index, Level = tlevel, Experience = 0 });
                    player.RefreshStats();
                    player.Connection.ReceiveChat(string.Format("{0}'s {1} has been leveled to {2}", vals[1], vals[2], vals[3]), MessageType.System);
                }
            }
        }
    }
}

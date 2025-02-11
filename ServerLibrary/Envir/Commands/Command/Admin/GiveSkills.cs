using Library;
using Library.Network.ClientPackets;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System.Runtime;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class GiveSkills : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "GIVESKILLS";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            player = SEnvir.GetPlayerByCharacter(vals[1]);
            if (player == null)
                throw new UserCommandException(string.Format("Could not find player: {0}", vals[1]));

            UserMagic uMagic;
            foreach (MagicInfo mInfo in SEnvir.MagicInfoList.Binding)
            {
                if (mInfo.NeedLevel1 > player.Level || mInfo.Class != player.Class || mInfo.School == MagicSchool.None)
                    continue;

                if (!player.GetMagic(mInfo.Magic, out MagicObject magicObject))
                {
                    uMagic = SEnvir.UserMagicList.CreateNewObject();
                    uMagic.Character = player.Character;
                    uMagic.Info = mInfo;

                    player.SetupMagic(uMagic);

                    player.Enqueue(new S.NewMagic { Magic = uMagic.ToClientInfo() });
                }
                else
                {
                    uMagic = magicObject.Magic;

                    if (uMagic.ItemRequired)
                    {
                        uMagic.ItemRequired = false;
                        player.Enqueue(new S.NewMagic { Magic = uMagic.ToClientInfo() });
                    }
                }

                int level = player.Level >= mInfo.NeedLevel3 ? 3 : player.Level >= mInfo.NeedLevel2 ? 2 : 1;
                uMagic.Level = level;

                player.Enqueue(new S.MagicLeveled { InfoIndex = uMagic.Info.Index, Level = uMagic.Level, Experience = uMagic.Experience });
            }
            player.RefreshStats();
        }
    }
}

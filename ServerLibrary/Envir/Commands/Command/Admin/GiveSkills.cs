﻿using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
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

                if (!player.Magics.TryGetValue(mInfo.Magic, out uMagic))
                {
                    uMagic = SEnvir.UserMagicList.CreateNewObject();
                    uMagic.Character = player.Character;
                    uMagic.Info = mInfo;
                    player.Magics[mInfo.Magic] = uMagic;

                    player.Enqueue(new S.NewMagic { Magic = uMagic.ToClientInfo() });
                }

                int level = player.Level >= mInfo.NeedLevel3 ? 3 : player.Level >= mInfo.NeedLevel2 ? 2 : 1;
                uMagic.Level = level;

                player.Enqueue(new S.MagicLeveled { InfoIndex = uMagic.Info.Index, Level = uMagic.Level, Experience = uMagic.Experience });
            }
            player.RefreshStats();
        }
    }
}

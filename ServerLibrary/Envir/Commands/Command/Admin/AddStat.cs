using Library;
using Server.Models;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class AddStat : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "ADDSTAT";
        public override int PARAMS_LENGTH => 4;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException(); //AddStat Weapon MaxDC 50

            if (!Enum.TryParse(vals[1], true, out EquipmentSlot tslot))
                ThrowNewInvalidParametersException();
            if (!Enum.TryParse(vals[2], true, out Stat tstat))
                ThrowNewInvalidParametersException();
            if (!int.TryParse(vals[3], out int tamount))
                ThrowNewInvalidParametersException();

            if (player.Equipment[(int)tslot] != null)
            {
                player.Equipment[(int)tslot].AddStat(tstat, tamount, StatSource.Added);
                player.Equipment[(int)tslot].StatsChanged();

                player.SendShapeUpdate();
                player.RefreshStats();

                player.Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)tslot, NewStats = new Stats(player.Equipment[(int)tslot].Stats) });
                player.Connection.ReceiveChat(string.Format("{0} added {1} + {2}", player.Equipment[(int)tslot].Info.ItemName, tstat, tamount), MessageType.System);
            }
        }
    }
}

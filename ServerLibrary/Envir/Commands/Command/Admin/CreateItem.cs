using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir.Commands.Exceptions;
using Server.Models;
using System;

namespace Server.Envir.Commands.Command.Admin
{
    class CreateItem : AbstractParameterizedCommand<IAdminCommand>
    {
        public override string VALUE => "MAKE";
        public override int PARAMS_LENGTH => 2;

        public override void Action(PlayerObject player, string[] vals)
        {
            if (vals.Length < PARAMS_LENGTH)
                ThrowNewInvalidParametersException();

            ItemInfo item = SEnvir.GetItemInfo(vals[1]);
            if (item == null)
                throw new UserCommandException(string.Format("Could not find item: {0}", vals[1]));

            int value;
            if (vals.Length < 3 || !int.TryParse(vals[2], out value) || value == 0)
                value = 1;

            while (value > 0)
            {
                int count = Math.Min(value, item.StackSize);
                if (!player.CanGainItems(false, new ItemCheck(item, count, UserItemFlags.None, TimeSpan.Zero)))
                    throw new UserCommandException(string.Format("Can not hold anymore {0}.", vals[1]));

                UserItem userItem = SEnvir.CreateDropItem(item, 0);
                userItem.Count = count;
                userItem.Flags = UserItemFlags.GameMaster;

                value -= count;
                player.GainItem(userItem);
            }

        }
    }
}

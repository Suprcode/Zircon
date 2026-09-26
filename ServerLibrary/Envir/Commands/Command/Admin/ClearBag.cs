using Library;
using Server.DBModels;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Commands.Command.Admin
{
    class ClearBag : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "CLEARBAG";

        public override void Action(PlayerObject player)
        {
            int count = 0;

            for (int i = 0; i < player.Inventory.Length; i++)
            {
                UserItem item = player.Inventory[i];
                if (item == null) continue;

                player.RemoveItem(item);
                player.Inventory[i] = null;
                item.Delete();
                count++;

                player.Enqueue(new S.ItemChanged
                {
                    Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i },
                    Success = true,
                });
            }

            player.RefreshWeight();
            player.Connection.ReceiveChat($"[CLEAR BAG] Removed {count} item(s).", MessageType.System);
        }
    }
}

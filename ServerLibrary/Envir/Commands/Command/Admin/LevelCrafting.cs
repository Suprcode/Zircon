using Library;
using Server.Models;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    class LevelCrafting : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "LEVELCRAFTING";

        public override void Action(PlayerObject player)
        {
            if (!player.LevelCrafting())
            {
                player.Connection.ReceiveChat("Crafting is already at the max level.", MessageType.System);
                return;
            }

            bool maximum = SEnvir.CraftingLevelInfoList.Binding.All(x => x.Level != player.Character.CraftingLevel);
            string level = maximum ? "Max" : player.Character.CraftingLevel.ToString();
            player.Connection.ReceiveChat($"Crafting increased to level {level}.", MessageType.System);
        }
    }
}

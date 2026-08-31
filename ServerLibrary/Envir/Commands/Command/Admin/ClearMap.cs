using Library;
using Server.Models;
using Server.Models.Monsters;
using System.Linq;

namespace Server.Envir.Commands.Command.Admin
{
    /// <summary>
    /// Removes spawned monsters and ground items from the player's current map.
    /// Players, player-owned pets and companions, NPCs, and guards are preserved.
    /// </summary>
    class ClearMap : AbstractCommand<IAdminCommand>
    {
        public override string VALUE => "CLEARMAP";

        public override void Action(PlayerObject player)
        {
            int monsterCount = 0;
            int itemCount = 0;

            foreach (MapObject ob in player.CurrentMap.Objects.ToList())
            {
                if (ob is ItemObject item)
                {
                    item.Despawn();
                    itemCount++;
                    continue;
                }

                if (ob is not MonsterObject monster) continue;
                if (monster.PetOwner != null || monster is Companion or Guard or CastleGuard) continue;

                monster.Despawn();
                monsterCount++;
            }

            player.Connection.ReceiveChat($"[CLEAR MAP] Removed {monsterCount} monster(s) and {itemCount} item(s).", MessageType.System);
        }
    }
}

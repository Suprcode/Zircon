using Library.SystemModels;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.PlayerTeleport)]
    public class PlayerTeleport : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            TeleportPlayer(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, action.RegionParameter1);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            TeleportPlayer(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, action.RegionParameter1);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            if (action.InstanceParameter1 == null)
            {
                TeleportPlayer(action.MapParameter1, null, 0, action.RegionParameter1);
            }
            else
            {
                var mapInstance = SEnvir.Instances[action.InstanceParameter1];

                for (byte i = 0; i < mapInstance.Length; i++)
                {
                    TeleportPlayer(action.MapParameter1, action.InstanceParameter1, i, action.RegionParameter1);
                }
            }
        }

        private static void TeleportPlayer(MapInfo mapInfo, InstanceInfo instanceInfo, byte instanceSequence, MapRegion region)
        {
            var map = SEnvir.GetMap(mapInfo, instanceInfo, instanceSequence);
            if (map == null) return;

            for (int i = map.Players.Count - 1; i >= 0; i--)
            {
                PlayerObject mapPlayer = map.Players[i];
                mapPlayer.Teleport(region, instanceInfo, instanceSequence);
            }
        }
    }
}


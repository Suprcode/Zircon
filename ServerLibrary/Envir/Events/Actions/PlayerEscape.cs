using Library.SystemModels;
using Server.Models;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.PlayerEscape)]
    public class PlayerEscape : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            EscapePlayer(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            EscapePlayer(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            if (action.InstanceParameter1 == null)
            {
                EscapePlayer(action.MapParameter1, null, 0);
            }
            else
            {
                var mapInstance = SEnvir.Instances[action.InstanceParameter1];

                for (byte i = 0; i < mapInstance.Length; i++)
                {
                    EscapePlayer(action.MapParameter1, action.InstanceParameter1, i);
                }
            }
        }

        private static void EscapePlayer(MapInfo mapInfo, InstanceInfo instanceInfo, byte instanceSequence)
        {
            var map = SEnvir.GetMap(mapInfo, instanceInfo, instanceSequence);
            if (map == null) return;

            for (int i = map.Players.Count - 1; i >= 0; i--)
            {
                PlayerObject mapPlayer = map.Players[i];
                mapPlayer.Teleport(mapPlayer.Character.BindPoint.BindRegion, instanceInfo, instanceSequence);
            }
        }
    }
}


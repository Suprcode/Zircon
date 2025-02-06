using Library;
using Library.SystemModels;
using Server.Models;
using S = Library.Network.ServerPackets;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.MapMessage)]
    public class MapMessage : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            SendMapMessage(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, action.StringParameter1);
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            SendMapMessage(action.MapParameter1, player.CurrentMap.Instance, player.CurrentMap.InstanceSequence, action.StringParameter1);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            if (action.InstanceParameter1 == null)
            {
                SendMapMessage(action.MapParameter1, null, 0, action.StringParameter1);
            }
            else
            {
                var mapInstance = SEnvir.Instances[action.InstanceParameter1];

                for (byte i = 0; i < mapInstance.Length; i++)
                {
                    SendMapMessage(action.MapParameter1, action.InstanceParameter1, i, action.StringParameter1);
                }
            }
        }

        private static void SendMapMessage(MapInfo mapInfo, InstanceInfo instanceInfo, byte instanceSequence, string message)
        {
            var map = SEnvir.GetMap(mapInfo, instanceInfo, instanceSequence);
            if (map == null) return;

            map.Broadcast(new S.Chat { Text = message, Type = MessageType.System });
        }
    }
}

using Library.SystemModels;
using Server.Models;
using System.Drawing;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.MonsterPlayerSpawn)]
    public class MonsterPlayerSpawn : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            SpawnMonster(action.MonsterParameter1, player.CurrentMap, player.CurrentMap.GetRandomLocation(player.CurrentLocation, 10));
        }

        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            SpawnMonster(action.MonsterParameter1, player.CurrentMap, player.CurrentMap.GetRandomLocation(player.CurrentLocation, 10));
        }

        private static void SpawnMonster(MonsterInfo monsterInfo, Map map, Point location)
        {
            MonsterObject mob = MonsterObject.GetMonster(monsterInfo);
            mob.Spawn(map, location);
        }
    }
}


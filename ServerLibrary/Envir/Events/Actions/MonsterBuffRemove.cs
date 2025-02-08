using Library;
using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Envir.Events.Actions
{
    /// <summary>
    /// Removes a buff from monsters
    /// </summary>
    [EventActionType(EventActionType.MonsterBuffRemove)]
    public class MonsterBuffRemove : IWorldEventAction, IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action)
        {
            RemoveBuff(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence);
        }

        public void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action)
        {
            RemoveBuff(action, triggerPlayer.CurrentMap.Instance, triggerPlayer.CurrentMap.InstanceSequence);
        }

        public void Act(EventLog log, WorldEventAction action)
        {
            RemoveBuff(action, null, 0);
        }

        private static void RemoveBuff(BaseEventAction action, InstanceInfo instance, byte instanceSequence)
        {
            if (string.IsNullOrEmpty(action.StringParameter1) ||
                !Enum.TryParse(action.StringParameter1, true, out BuffType type)) return;

            if (action.InstanceParameter1 != instance) return;

            var map = GetTargetMap(action, instance, instanceSequence);
            if (map == null) return;

            var monsters = GetTargetMonsters(action, map);
            foreach (var monster in monsters)
            {
                if (action.RegionParameter1 == null || monster.CurrentCell.Regions.Contains(action.RegionParameter1))
                {
                    monster.BuffRemove(type);

                    if (action.Restrict) break;
                }
            }
        }

        private static Map GetTargetMap(BaseEventAction action, InstanceInfo playerInstance, byte playerInstanceSequence)
        {
            return action.MapParameter1 != null
                ? SEnvir.GetMap(action.MapParameter1, playerInstance, playerInstanceSequence)
                : action.RegionParameter1 != null
                    ? SEnvir.GetMap(action.RegionParameter1.Map, playerInstance, playerInstanceSequence)
                    : null;
        }

        private static List<MonsterObject> GetTargetMonsters(BaseEventAction action, Map map)
        {
            return action.MonsterParameter1 == null
                ? map.Objects.OfType<MonsterObject>().ToList()
                : map.Objects.OfType<MonsterObject>().Where(m => m.MonsterInfo == action.MonsterParameter1).ToList();
        }
    }
}

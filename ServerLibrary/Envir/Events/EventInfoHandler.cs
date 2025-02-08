using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server.Envir.Events
{
    public class EventInfoHandler
    {
        #region Properties

        private readonly Dictionary<string, WorldEventTriggerType[]> _worldTriggerMapping = [];
        private readonly Dictionary<string, PlayerEventTriggerType[]> _playerTriggerMapping = [];
        private readonly Dictionary<string, MonsterEventTriggerType[]> _monsterTriggerMapping = [];

        private readonly Dictionary<string, IEventTrigger> _triggers = [];

        private readonly Dictionary<EventActionType, IEventAction> _actions = [];

        #endregion

        #region Initialization

        public EventInfoHandler()
        {
            LoadTriggers();
            LoadActions();
        }

        private void LoadTriggers()
        {
            var eventTriggers = Assembly.GetAssembly(typeof(EventInfoHandler))?
                                .GetTypes()
                                .Where(type => typeof(IEventTrigger).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract) ?? [];

            foreach (var eventTrigger in eventTriggers)
            {
                var attribute = eventTrigger.GetCustomAttribute<EventTriggerTypeAttribute>();
                if (attribute != null && Activator.CreateInstance(eventTrigger) is IEventTrigger instance)
                {
                    _triggers[attribute.TriggerName] = instance;

                    if (instance is IWorldEventTrigger world)
                    {
                        _worldTriggerMapping[attribute.TriggerName] = world.WorldTypes;
                    }
                    else if (instance is IPlayerEventTrigger player)
                    {
                        _playerTriggerMapping[attribute.TriggerName] = player.PlayerTypes;
                    }
                    else if (instance is IMonsterEventTrigger monster)
                    {
                        _monsterTriggerMapping[attribute.TriggerName] = monster.MonsterTypes;
                    }
                }
            }
        }

        private void LoadActions()
        {
            var actionTypes = Assembly.GetAssembly(typeof(EventInfoHandler))?
                                .GetTypes()
                                .Where(type => typeof(IEventAction).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract) ?? [];

            foreach (var type in actionTypes)
            {
                var attribute = type.GetCustomAttribute<EventActionTypeAttribute>();
                if (attribute != null && Activator.CreateInstance(type) is IEventAction instance)
                {
                    _actions[attribute.Type] = instance;
                }
            }
        }

        #endregion

        #region Database Helpers

        public List<EventActionType> GetMonsterEventActions()
        {
            return _actions
                .Where(kvp => kvp.Value is IMonsterEventAction)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<EventActionType> GetWorldEventActions()
        {
            return _actions
                .Where(kvp => kvp.Value is IWorldEventAction)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public List<EventActionType> GetPlayerEventActions()
        {
            return _actions
                .Where(kvp => kvp.Value is IPlayerEventAction)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        #endregion

        #region Processes

        public void Process(string eventName)
        {
            eventName = eventName.ToUpper();

            if (!_worldTriggerMapping.TryGetValue(eventName, out var types)) return;

            var count = SEnvir.WorldEventInfoTriggerList.Binding.Count;

            for (var i = 0; i < count; i++)
            {
                var trigger = SEnvir.WorldEventInfoTriggerList.Binding[i];

                if (!types.Contains(trigger.Type))
                {
                    continue;
                }

                var eventLog = GetWorldEventLog(trigger.Event);

                if (eventLog == null) continue;

                if (_triggers.TryGetValue(eventName, out var eventTrigger) && eventTrigger is IWorldEventTrigger worldTrigger)
                {
                    if (!eventLog.WorldTriggerCount.TryGetValue(trigger, out int value))
                    {
                        eventLog.WorldTriggerCount.Add(trigger, 0);
                    }

                    if (trigger.MaxTriggers > 0 && value >= trigger.MaxTriggers)
                    {
                        continue;
                    }

                    if (worldTrigger.Check(trigger))
                    {
                        eventLog.WorldTriggerCount[trigger] = ++value;

                        int start = eventLog.CurrentValue;
                        int end = Math.Min(trigger.Event.MaxValue, Math.Max(0, start + trigger.Value));

                        eventLog.CurrentValue = end;

                        foreach (WorldEventAction action in trigger.Event.Actions)
                        {
                            if (start < action.TriggerValue && end >= action.TriggerValue)
                            {
                                if (!_actions.TryGetValue(action.Type, out IEventAction eventAction)) return;

                                if (eventAction is IWorldEventAction worldAction)
                                {
                                    worldAction.Act(eventLog, action);
                                }
                            }
                        }

                        if (trigger.Event.ResetWhenMax && eventLog.CurrentValue == trigger.Event.MaxValue)
                        {
                            eventLog.Reset();
                        }
                    }
                }
            }
        }

        public void Process(PlayerObject player, string eventName)
        {
            eventName = eventName.ToUpper();

            if (!_playerTriggerMapping.TryGetValue(eventName, out var types)) return;

            var count = SEnvir.PlayerEventInfoTriggerList.Binding.Count;

            for (var i = 0; i < count; i++)
            {
                var trigger = SEnvir.PlayerEventInfoTriggerList.Binding[i];

                if (!types.Contains(trigger.Type))
                {
                    continue;
                }

                var eventLog = GetPlayerEventLog(trigger.Event, player);

                if (eventLog == null) continue;

                if (_triggers.TryGetValue(eventName, out var eventTrigger) && eventTrigger is IPlayerEventTrigger playerTrigger)
                {
                    if (!eventLog.PlayerTriggerCount.TryGetValue(trigger, out int value))
                    {
                        eventLog.PlayerTriggerCount.Add(trigger, 0);
                    }

                    if (trigger.MaxTriggers > 0 && value >= trigger.MaxTriggers)
                    {
                        continue;
                    }

                    if (playerTrigger.Check(player, trigger))
                    {
                        eventLog.PlayerTriggerCount[trigger] = ++value;

                        int start = eventLog.CurrentValue;
                        int end = Math.Min(trigger.Event.MaxValue, Math.Max(0, start + trigger.Value));

                        eventLog.CurrentValue = end;

                        foreach (PlayerEventAction action in trigger.Event.Actions)
                        {
                            if (start < action.TriggerValue && end >= action.TriggerValue)
                            {
                                if (!_actions.TryGetValue(action.Type, out IEventAction eventAction)) return;

                                if (eventAction is IPlayerEventAction playerAction)
                                {
                                    playerAction.Act(player, eventLog, action);
                                }
                            }
                        }

                        if (trigger.Event.ResetWhenMax && eventLog.CurrentValue == trigger.Event.MaxValue)
                        {
                            eventLog.Reset();
                        }
                    }
                }
            }
        }

        public void Process(MonsterObject monster, string eventName)
        {
            eventName = eventName.ToUpper();

            if (!_triggers.ContainsKey(eventName)) return;

            if (!_monsterTriggerMapping.TryGetValue(eventName, out var types)) return;

            foreach (MonsterEventTrigger trigger in monster.MonsterInfo.Events)
            {
                if (!types.Contains(trigger.Type)) continue;

                var eventInfo = trigger.Event;

                var eventLog = GetMonsterEventLog(trigger.Event, monster.EXPOwner);

                if (eventLog == null) continue;

                if (!eventInfo.ResetWhenMax && eventLog.CurrentValue == eventInfo.MaxValue)
                {
                    continue;
                }

                if (_triggers.TryGetValue(eventName, out var eventTrigger) && eventTrigger is IMonsterEventTrigger monsterTrigger)
                {
                    if (!eventLog.MonsterTriggerCount.TryGetValue(trigger, out int value))
                    {
                        eventLog.MonsterTriggerCount.Add(trigger, 0);
                    }

                    if (trigger.MaxTriggers > 0 && value >= trigger.MaxTriggers)
                    {
                        continue;
                    }

                    if (monsterTrigger.Check(monster, trigger))
                    {
                        eventLog.MonsterTriggerCount[trigger] = ++value;

                        int start = eventLog.CurrentValue;
                        int end = Math.Min(eventInfo.MaxValue, Math.Max(0, start + trigger.Value));

                        eventLog.CurrentValue = end;

                        foreach (MonsterEventAction action in eventInfo.Actions)
                        {
                            if (start < action.TriggerValue && end >= action.TriggerValue)
                            {
                                if (!_actions.TryGetValue(action.Type, out IEventAction eventAction)) return;

                                if (eventAction is IMonsterEventAction monsterAction)
                                {
                                    monsterAction.Act(monster.EXPOwner, eventLog, action);
                                }
                            }
                        }

                        if (eventInfo.ResetWhenMax && eventLog.CurrentValue == eventInfo.MaxValue)
                        {
                            eventLog.Reset();
                        }
                    }
                }
            }
        }

        #endregion

        #region Event Logs

        private static EventLog GetWorldEventLog(WorldEventInfo info)
        {
            var match = SEnvir.EventLogs.FirstOrDefault(x => x.WorldEvent == info);

            if (match == null)
            {
                match = new EventLog
                {
                    WorldEvent = info,
                    Key = ""
                };
                SEnvir.EventLogs.Add(match);
            }

            return match;
        }

        private static EventLog GetPlayerEventLog(PlayerEventInfo info, PlayerObject player)
        {
            string key = GetEventKey(info.TrackingType, player);

            if (string.IsNullOrEmpty(key))
                return null;

            var match = SEnvir.EventLogs.FirstOrDefault(x => x.PlayerEvent == info && x.Key == key);

            if (match == null)
            {
                match = new EventLog
                {
                    PlayerEvent = info,
                    Key = key,
                    PlayerIndex = player.Character.Index,
                    InstanceInfo = player.CurrentMap.Instance,
                    InstanceSequence = player.CurrentMap.InstanceSequence
                };
                SEnvir.EventLogs.Add(match);
            }

            return match;
        }

        private static EventLog GetMonsterEventLog(MonsterEventInfo info, PlayerObject player)
        {
            string key = GetEventKey(info.TrackingType, player);

            if (string.IsNullOrEmpty(key))
                return null;

            var match = SEnvir.EventLogs.FirstOrDefault(x => x.MonsterEvent == info && x.Key == key);

            if (match == null)
            {
                match = new EventLog
                {
                    MonsterEvent = info,
                    Key = key,
                    PlayerIndex = player.Character.Index,
                    InstanceInfo = player.CurrentMap.Instance,
                    InstanceSequence = player.CurrentMap.InstanceSequence
                };
                SEnvir.EventLogs.Add(match);
            }

            return match;
        }

        private static string GetEventKey(EventTrackingType type, PlayerObject player)
        {
            if (player == null || player.Node == null)
                return null;

            switch (type)
            {
                case EventTrackingType.Global:
                    return "Global";

                case EventTrackingType.Player:
                    return $"Player:{player.Character.Index}";

                case EventTrackingType.Group:
                    bool hasGroupMembers = player.GroupMembers != null && player.GroupMembers.Count > 1;
                    if (!hasGroupMembers)
                        return null;
                    int groupLeaderIndex = player.GroupMembers[0].Character.Index;
                    return $"Group:{groupLeaderIndex}";

                case EventTrackingType.Guild:
                    var guildMember = player.Character.Account.GuildMember;
                    if (guildMember == null)
                        return null;
                    return $"Guild:{guildMember.Guild.GuildName}";

                case EventTrackingType.Instance:
                    var instance = player.CurrentMap.Instance;
                    if (instance == null)
                        return null;
                    return $"Instance:{instance.Index}:{player.CurrentMap.InstanceSequence}";

                default:
                    return null;
            }
        }

        #endregion
    }
}

using Library.SystemModels;
using Server.Models;
using System;
using System.Collections.Generic;

namespace Server.Envir.Events
{
    public interface IEventTrigger
    {

    }

    public interface IWorldEventTrigger
    {
        WorldEventTriggerType[] WorldTypes { get; }
        bool Check(WorldEventTrigger eventTrigger);
    }

    public interface IPlayerEventTrigger
    {
        PlayerEventTriggerType[] PlayerTypes { get; }
        bool Check(PlayerObject player, PlayerEventTrigger eventTrigger);
    }

    public interface IMonsterEventTrigger
    {
        MonsterEventTriggerType[] MonsterTypes { get; }
        bool Check(MonsterObject monster, MonsterEventTrigger eventTrigger);
    }
}

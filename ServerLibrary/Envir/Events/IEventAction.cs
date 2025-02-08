using Library.SystemModels;
using Server.Models;

namespace Server.Envir.Events
{
    public interface IEventAction
    {

    }

    public interface IWorldEventAction
    {
        void Act(EventLog log, WorldEventAction action);
    }

    public interface IPlayerEventAction
    {
        void Act(PlayerObject triggerPlayer, EventLog log, PlayerEventAction action);
    }

    public interface IMonsterEventAction
    {
        void Act(PlayerObject triggerPlayer, EventLog log, MonsterEventAction action);
    }
}

using Library;
using Library.SystemModels;
using Server.Models;
using System;

namespace Server.Envir.Events.Actions
{
    [EventActionType(EventActionType.ItemGive)]
    public class ItemGive : IPlayerEventAction, IMonsterEventAction, IEventAction
    {
        public void Act(PlayerObject player, EventLog log, PlayerEventAction action)
        {
            GiveItem(player, action.ItemParameter1);
        }

        public void Act(PlayerObject player, EventLog log, MonsterEventAction action)
        {
            GiveItem(player, action.ItemParameter1);
        }

        private static void GiveItem(PlayerObject player, ItemInfo item)
        {
            if (item == null) return;

            ItemCheck check = new(item, 1, UserItemFlags.None, TimeSpan.Zero);
            if (!player.CanGainItems(false, check)) return;

            player.GainItem(SEnvir.CreateFreshItem(check));
        }
    }
}

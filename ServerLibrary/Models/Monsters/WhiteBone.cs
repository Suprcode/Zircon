﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class WhiteBone : MonsterObject
    {
        public WhiteBone()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddSeconds(1), ActionType.Function));
            Direction = MirDirection.DownLeft;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.SummonSkeleton });

            ActionTime = SEnvir.Now.AddSeconds(1);
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Appear();
                    return;
            }

            base.ProcessAction(action);
        }
        
        public void Appear()
        {
            Visible = true;
            AddAllObjects();
        }
    }
}

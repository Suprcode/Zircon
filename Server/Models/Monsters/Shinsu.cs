using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.Network;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class Shinsu : SpittingSpider
    {
        public bool Mode;
        public DateTime ModeTime;

        public override bool CanAttack => base.CanAttack && Mode;

        public Shinsu()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddSeconds(1), ActionType.Function));
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.SummonShinsu, Direction = Direction });

            ActionTime = SEnvir.Now.AddSeconds(2);
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
        public override void Process()
        {
            if (!Dead && SEnvir.Now > ActionTime)
            {
                if (Target != null) ModeTime = SEnvir.Now.AddSeconds(10);

                if (!Mode && SEnvir.Now < ModeTime)
                {
                    Mode = true;
                    Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                    ActionTime = SEnvir.Now.AddSeconds(2);
                }
                else if (Mode && SEnvir.Now > ModeTime)
                {
                    Mode = false;
                    Broadcast(new S.ObjectHide() { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                    ActionTime = SEnvir.Now.AddSeconds(2);
                }
            }
            base.Process();
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            S.ObjectMonster packet = (S.ObjectMonster) base.GetInfoPacket(ob);

            packet.Extra = Mode;

            return packet;
        }
    }
}
using System;
using System.Collections.Generic;
using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class ZumaGuardian : MonsterObject
    {
        public override bool CanMove => base.CanMove && Visible;
        public override bool CanAttack => base.CanAttack && Visible;
        
        public int WakeRange = 7;

        public ZumaGuardian()
        {
            AvoidFireWall = true;
            Visible = false;
        }
        
        public virtual void Wake()
        {
            ActionTime = SEnvir.Now.AddSeconds(1);
            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Location = CurrentLocation, Direction = Direction });

            Visible = true;
        }
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (!Visible) return 0;

            return base.Attacked(attacker, power, element, ignoreShield, ignoreShield, canCrit);
        }
        public override void Process()
        {
            base.Process();

            if (Dead || Target == null || Visible) return;

            if (!Functions.InRange(CurrentLocation, Target.CurrentLocation, 3)) return;

            Wake();
            WakeAll(WakeRange);
        }

        public void WakeAll(int range)
        {
            List<MapObject> obs = GetAllObjects(CurrentLocation, range);

            foreach (MapObject ob in obs)
            {
                ZumaGuardian zuma = ob as ZumaGuardian;

                if (zuma == null || zuma.Visible) continue;

                zuma.Wake();
                zuma.Target = Target;
            }

        }
        

        public override bool ApplyPoison(Poison p)
        {
            if (!Visible) return false;

            return base.ApplyPoison(p);
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            S.ObjectMonster packet = (S.ObjectMonster) base.GetInfoPacket(ob);

            packet.Extra = Visible;

            return packet;
        }
    }
}

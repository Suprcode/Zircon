using System;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class VoraciousGhost : MonsterObject
    {
        public int DeathCount;
        public int ReviveCount;
        public DateTime ReviveTime;
        
        public override decimal Experience => base.Experience/(decimal) Math.Pow(2, ReviveCount);

        public VoraciousGhost()
        {
            ReviveCount = SEnvir.Random.Next(4);
        }


        public override void Process()
        {
            base.Process();

            if (!Dead || ReviveCount == 0 || SEnvir.Now < ReviveTime) return;

            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = SEnvir.Now.AddMilliseconds(1500);

            Dead = false;
            SetHP((int)(Stats[Stat.Health] / Math.Pow(2, DeathCount)));
            ReviveCount--;
        }

        public override void Drop(PlayerObject owner, int players, decimal rate)
        {
            if (ReviveCount != 0) return;

            base.Drop(owner, players, rate);
        }

        public override void Die()
        {
            base.Die();
            
            ReviveTime = SEnvir.Now.AddSeconds(SEnvir.Random.Next(5) + 3);
            DeadTime = ReviveTime.Add(Config.DeadDuration);

            DeathCount++;
        }
    }
}

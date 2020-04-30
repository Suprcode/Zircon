using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class FerociousIceTiger : MonsterObject
    {
        public bool Spinning;

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 3 || y > 3) return false;

            return true;
        }

        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();

            Spinning = false;
        }
        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            if (SEnvir.Random.Next(5) == 0 || Spinning)
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint>() });
                
                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, 3))
                {
                    int damage = GetDC();

                    if (ob.Race == ObjectType.Player)
                    {
                        switch (((PlayerObject)ob).Class)
                        {
                            case MirClass.Warrior:
                                damage -= damage * 2 / 10;
                                break;
                        }
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        ob,
                        damage,
                        AttackElement));

                }

                 ActionTime = SEnvir.Now.AddMilliseconds(600);
                 AttackTime = SEnvir.Now.AddMilliseconds(640);

                Spinning = true;
            }
            else
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation }); //Animation ?

                foreach (MapObject ob in GetTargets(CurrentMap, Functions.Move(CurrentLocation, Direction, 3), 3))
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayAttack,
                        ob,
                        GetDC() / 2,
                        AttackElement));
                }
                UpdateAttackTime();
            }

        }

    }
}

using System;
using System.Collections.Generic;
using Library;
using S = Library.Network.ServerPackets;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class WindfurySorcerer : MonsterObject
    {
        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400),
                               ActionType.DelayAttack,
                               Target,
                               GetDC(),
                               AttackElement));
        }
    }
}

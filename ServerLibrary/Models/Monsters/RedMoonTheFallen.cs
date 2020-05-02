using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class RedMoonTheFallen : MonsterObject
    {
        public override bool CanMove => false;

        public RedMoonTheFallen()
        {
            Direction = MirDirection.Up;
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap;
        }

        protected override void Attack()
        {
            List<uint> targetIDs = new List<uint>();

            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = targetIDs });

            UpdateAttackTime();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 40);

            foreach (MapObject ob in targets)
            {
                targetIDs.Add(ob.ObjectID);

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayAttack,
                    ob,
                    GetDC(),
                    AttackElement));
            }
        }

    }
}
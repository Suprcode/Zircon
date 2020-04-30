using System;
using System.Collections.Generic;
using Library;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class LordNiJae : CarnivorousPlant
    {

        public LordNiJae()
        {
            Direction = MirDirection.Up;
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Globals.MagicRange);
        }
        
        protected override void Attack()
        {
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, Globals.MagicRange);
            
            foreach (MapObject ob in targets)
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500 + Functions.Distance(ob.CurrentLocation, CurrentLocation)*48),
                    ActionType.DelayAttack,
                    ob,
                    GetDC(),
                    AttackElement));
            }
        }

        public override int Attack(MapObject ob, int power, Element element)
        {
            int result = base.Attack(ob, power, element);
            
            if (result <= 0) return result;

            if (SEnvir.Random.Next(5) == 0)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Green,
                    Value = GetSC(),
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 10,
                });
            }

            if (SEnvir.Random.Next(10) == 0)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }
            return result;
        }
    }
}

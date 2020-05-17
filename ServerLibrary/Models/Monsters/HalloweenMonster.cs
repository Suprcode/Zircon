using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.SystemModels;
using Server.Envir;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class HalloweenMonster : MonsterObject
    {
        public decimal BonusExperience = 0;
        public DateTime NextMonsterKill;
        public override decimal Experience => base.Experience + BonusExperience;

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject) action.Data[0]);
                    return;
            }

            base.ProcessAction(action);
        }

        public override void ProcessSearch()
        {
            ProperSearch();
        }

        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Globals.MagicRange);
        }

        public override void ProcessNameColour()
        {
            NameColour = Color.MediumPurple;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            MapExperienceRate *= 10;
            MapDropRate *= 10;
            MapGoldRate *= 30;
        }
        
        public override bool ShouldAttackTarget(MapObject ob)
        {
            return CanAttackTarget(ob);
        }

        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord || ob == this) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    return base.CanAttackTarget(ob);

                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject) ob;

                    return !mob.MonsterInfo.IsBoss ;
                default:
                    return false;
            }
        }

        protected override void Attack()
        {
            if (!CanAttackTarget(Target) || (SEnvir.Now <= NextMonsterKill && Target is MonsterObject))
            {
                Target = null;
                return;
            }

            Point targetBack = Functions.Move(Target.CurrentLocation, Target.Direction, -1);


            Broadcast(new S.ObjectAttack {ObjectID = ObjectID, Direction = Target.Direction, Location = targetBack});
            Broadcast(new S.ObjectTurn {ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation});


            UpdateAttackTime();


            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(300),
                ActionType.DelayAttack,
                Target));
        }

        private void Attack(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead) return;

            int power;
            MonsterObject mob = null;
            if (ob.Race == ObjectType.Monster)
            {
                mob = (MonsterObject) ob;
                if (mob.PetOwner == null)
                {
                    mob.EXPOwner = null;
                    power = ob.CurrentHP;
                    BonusExperience += mob.Experience;
                    NextMonsterKill = SEnvir.Now.AddMinutes(1);
                }
                else
                    power = GetDC();
            }
            else
                power = GetDC();

            ob.Attacked(this, power, Element.None);

            if (mob != null && mob.PetOwner == null && SEnvir.Random.Next(5) == 0)
                Teleport(CurrentMap, CurrentMap.GetRandomLocation());
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }

        public override void DeActivate()
        {
        }

        public override void Die()
        {
            base.Die();

            if (CurrentMap.HasSafeZone) return;

            if (SEnvir.Random.Next(2) == 0)
            {
                MonsterInfo boss;

                while (true)
                {
                    boss = SEnvir.BossList[SEnvir.Random.Next(SEnvir.BossList.Count)];

                    if (boss.Level >= 300) continue;

                    break;
                }

                MonsterObject mob = GetMonster(boss);

                mob.Spawn(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 2));
            }
            else
            {
                for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                {
                    MonsterObject mob = CurrentMap.Objects[i] as MonsterObject;

                    if (mob == null) continue;

                    if (mob.PetOwner != null) continue;

                    if (mob is Guard) continue;

                    if (mob.Dead || mob.MoveDelay == 0 || !mob.CanMove) continue;

                    if (mob.Target != null) continue;

                    if (mob.Level >= 300) continue;

                    mob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 15));
                }

            }
        }
    }
}

using Library;
using Server.Envir;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class Guard : MonsterObject
    {
        public override bool Blocking => true;
        public override bool CanMove => false;

        public Guard()
        {
            NameColour = Color.SkyBlue;
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0]);
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
            return Target.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange);
        }
        public override void ProcessNameColour()
        {
            NameColour = Color.SkyBlue;
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return 0;
        }
        
        public override bool ShouldAttackTarget(MapObject ob)
        {
            return CanAttackTarget(ob);
        }
        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;
            
            switch (ob.Race)
            {
                case ObjectType.Player:
                    return ob.Stats[Stat.PKPoint] >= Config.RedPoint && ob.Stats[Stat.Redemption] == 0;

                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == null)
                        return !mob.Passive;

                    if (mob.PetOwner.Stats[Stat.PKPoint] >= Config.RedPoint && mob.PetOwner.Stats[Stat.Redemption] == 0)
                        return true;
                    return false;

                default:
                    return false;
            }
        }

        protected override void Attack()
        {
            if (!CanAttackTarget(Target))
            {
                Target = null;
                return;
            }

            Point targetBack = Functions.Move(Target.CurrentLocation, Target.Direction, -1);

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Target.Direction, Location = targetBack });
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


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

            if (ob.Race == ObjectType.Monster)
            {
                MonsterObject mob = (MonsterObject) ob;
                mob.EXPOwner = null;
                power = ob.CurrentHP;
            }
            else
                power = GetDC();

            ob.Attacked(this, power, Element.None);
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

    }
}

using Library;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class DoomClaw : MonsterObject
    {
        protected override void OnSpawned()
        {
            base.OnSpawned();
            
            ActionTime = SEnvir.Now.AddSeconds(2);
            
            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public override int Pushed(MirDirection direction, int distance)
        {
            return 0;
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false,
            bool canCrit = true, bool canStruck = true)
        {

            if (!Functions.InRange(CurrentLocation, attacker.CurrentLocation, 10)) return 0;
               
            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit);
        }

        public override void ProcessAI()
        {
            if (Dead) return;

            ProcessRegen();

            if (!CanAttack) return;
            
            List<MapObject> rightTargets = GetTargets(CurrentMap, Functions.Move(CurrentLocation, MirDirection.Right, 5), 5);
            List<MapObject> leftTargets = GetTargets(CurrentMap, Functions.Move(CurrentLocation, MirDirection.Down, 5), 5);
            List<MapObject> middleTargets = GetTargets(CurrentMap, Functions.Move(CurrentLocation, MirDirection.DownRight, 5), Config.MaxViewRange);

            UpdateAttackTime();

            if (SEnvir.Random.Next(20) == 0)
            {

                List<MapObject> allTargerts = GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange);
                if (allTargerts.Count == 0) return;
                //Do Wave
                Wave(allTargerts);
                return;
            }
            
            int total = rightTargets.Count + leftTargets.Count + middleTargets.Count;
            
            int value = SEnvir.Random.Next(total);

            if ((value -= rightTargets.Count) < 0)
            {
                //Right Side - Left Claw
                if (SEnvir.Random.Next(10) > 0)
                    RightPinch(middleTargets);
                else
                    RightSwipe(middleTargets);
                return;
            }

            if ((value -= leftTargets.Count) < 0)
            {
                //Left Side - Right Claw
                if (SEnvir.Random.Next(10) > 0)
                    LeftPinch(middleTargets);
                else
                    LeftSwipe(middleTargets);
                return;
            }

            if ((value -= middleTargets.Count) < 0)
            {
                //Spit
                Spit(middleTargets);
                return;
            }

        }

        public void Wave(List<MapObject> targets)
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawWave });

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {
                int damage = GetDC();

                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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

                ob.Pushed(MirDirection.DownRight, 15);
            }




        }

        public void Spit(List<MapObject> targets)
        {
            List<Point> targetsIDs = new List<Point>();

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {
                int damage = GetDC();
                
                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawSpit, Locations = targetsIDs });

        }

        public void RightPinch(List<MapObject> targets)
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawRightPinch });

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {
                int damage = GetDC();

                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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
        }
        public void RightSwipe(List<MapObject> targets)
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawRightSwipe });

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {

                int damage = GetDC();

                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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

                ob.Pushed(MirDirection.DownLeft, 5);
            }


        }
        public void LeftPinch(List<MapObject> targets)
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawLeftPinch });

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {
                int damage = GetDC();

                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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


        }
        public void LeftSwipe(List<MapObject> targets)
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DoomClawLeftSwipe });

            UpdateAttackTime();

            foreach (MapObject ob in targets)
            {

                int damage = GetDC();

                if (ob.Race == ObjectType.Player)
                {
                    switch (((PlayerObject)ob).Class)
                    {
                        case MirClass.Warrior:
                            damage -= damage * 4 / 10;
                            break;
                        case MirClass.Wizard:
                            damage -= damage * 3 / 10;
                            break;
                        case MirClass.Taoist:
                            damage -= damage * 6 / 10;
                            break;
                        case MirClass.Assassin:
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

                ob.Pushed(MirDirection.UpRight, 5);
            }


        }

    }
}

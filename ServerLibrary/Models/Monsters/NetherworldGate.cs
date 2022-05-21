using System;
using System.Drawing;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class NetherworldGate : MonsterObject
    {
        public override bool CanMove => false;
        public override bool CanAttack => false;

        public DateTime DespawnTime;
        

        public NetherworldGate()
        {
            Direction = MirDirection.Up;
        }

        public override void ProcessNameColour()
        {
            NameColour = Color.Lime;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();


            DespawnTime = SEnvir.Now.AddMinutes(20);

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat(string.Format(con.Language.NetherGateOpen, CurrentMap.Info.Description, CurrentLocation), MessageType.System);
        }

        public override void Process()
        {
            base.Process();

            if (SEnvir.Now >= DespawnTime)
            {
                if (SpawnInfo != null)
                    SpawnInfo.AliveCount--;

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(con.Language.NetherGateClosed, MessageType.System);

                SpawnInfo = null;
                Despawn();
                return;
            }

            if (SEnvir.Now >= SearchTime && SEnvir.MysteryShipMapRegion != null && SEnvir.MysteryShipMapRegion.PointList.Count > 0)
            {
                SearchTime = SEnvir.Now.AddSeconds(3);
                Map map = SEnvir.GetMap(SEnvir.MysteryShipMapRegion.Map, CurrentMap.Instance, CurrentMap.InstanceSequence);

                if (map == null)
                {
                    SearchTime = SEnvir.Now.AddSeconds(60);
                    return;
                }

                for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject ob = CurrentMap.Objects[i];

                    if (ob == this) continue;

                    if (ob is Guard) continue;

                    switch (ob.Race)
                    {
                        case ObjectType.Player:
                        case ObjectType.Monster:
                            if (ob.InSafeZone) continue;

                            if (ob.Dead || !Functions.InRange(ob.CurrentLocation, CurrentLocation, MonsterInfo.ViewRange)) continue;


                            ob.Teleport(map, SEnvir.MysteryShipMapRegion.PointList[SEnvir.Random.Next(SEnvir.MysteryShipMapRegion.PointList.Count)]);
                            break;
                        default:
                            continue;
                    }

                }
            }

        }

        public override void ProcessSearch()
        {
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()
        {
            return;
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return 0;
        }
        public override bool ApplyPoison(Poison p)
        {
            return false;
        }
    }
}

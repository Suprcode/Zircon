using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class JinamStoneGate :MonsterObject
    {
        public override bool CanMove => false;
        public override bool CanAttack => false;

        public DateTime DespawnTime;


        public JinamStoneGate()
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
                con.ReceiveChat(string.Format(con.Language.LairGateOpen, CurrentMap.Info.Description, CurrentLocation), MessageType.System);

           }

        public override void Process()
        {
            base.Process();

            if (SEnvir.Now >= DespawnTime)
            {
                if (SpawnInfo != null)
                    SpawnInfo.AliveCount--;

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat(con.Language.LairGateClosed, MessageType.System);

                SpawnInfo = null;
                Despawn();
                return;
            }

            if (SEnvir.Now >= SearchTime && SEnvir.LairMapRegion != null && SEnvir.LairMapRegion.PointList.Count > 0)
            {
                SearchTime = SEnvir.Now.AddSeconds(3);
                Map map = SEnvir.GetMap(SEnvir.LairMapRegion.Map, CurrentMap.Instance, CurrentMap.InstanceSequence);

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
                            if (ob.InSafeZone) continue;

                            if (ob.Dead || !Functions.InRange(ob.CurrentLocation, CurrentLocation, MonsterInfo.ViewRange)) continue;

                            ob.Teleport(map, SEnvir.LairMapRegion.PointList[SEnvir.Random.Next(SEnvir.LairMapRegion.PointList.Count)]);
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

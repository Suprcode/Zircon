using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CastleGate : CastleObject
    {
        public CastleGateInfo GateInfo { get; set; }

        public bool Closed;
        private DateTime CloseTime = DateTime.MinValue;

        protected List<BlockingObject> BlockingObjects = new List<BlockingObject>();

        protected Point[] BlockArray;

        private bool AutoOpen = true;

        public override bool CanMove => false;

        public override bool CanAttack => false;

        public override bool Blocking => Closed && base.Blocking;

        public CastleGate()
        {
            Stats = new Stats();

            Closed = true;
            Direction = MirDirection.Up;
        }

        public bool Spawn(CastleInfo castle, CastleGateInfo gateInfo)
        {
            Castle = castle;
            GateInfo = gateInfo;

            if (castle == null ||  gateInfo == null)
            {
                return false;
            }

            var map = SEnvir.Maps.First(x => x.Key == castle.Map).Value;

            if (!base.Spawn(map, new Point(gateInfo.X, gateInfo.Y)))
            {
                return false;
            }

            map.CastleGates.Add(this);

            return true;
        }

        public override void ProcessRegen()
        {

        }

        protected override void OnSpawned()
        {
            switch (MonsterInfo.FaceImage)
            {
                case 1:// West Pointing Castle
                    BlockArray = new Point[]
                    {
                        new Point(1, 0),
                        new Point(0, 1),
                        new Point(-1, -1),
                        new Point(-1, 0),
                        new Point(0, -1),
                    };
                    break;
                case 2:// South Pointing Castle
                    BlockArray = new Point[]
                    {
                        new Point(1, -1),
                        new Point(-1, 0),
                        new Point(0, -1),
                        new Point(0, 1),
                        new Point(1, 0),
                    };
                    break;
                case 3:// East Pointing Castle
                    BlockArray = new Point[]
                    {
                        new Point(-1, -1),
                        new Point(-1, 0),
                        new Point(0, 1),
                        new Point(1, 0),
                        new Point(0, -1),
                    };
                    break;
            }

            base.OnSpawned();

            Direction = MirDirection.Up;

            SpawnBlockers();
        }

        private void SpawnBlockers()
        {
            if (BlockArray == null) return;

            var info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.Blocker);

            if (info == null) return;

            info.Stats[Stat.Health] = this.Stats[Stat.Health];

            foreach (var block in BlockArray)
            {
                BlockingObject b = new(this);
                BlockingObjects.Add(b);

                b.MonsterInfo = info;

                if (!b.Spawn(this.CurrentMap, new Point(this.CurrentLocation.X + block.X, this.CurrentLocation.Y + block.Y)))
                {
                    SEnvir.Log(string.Format("{3} blocking mob not spawned at {0} {1}:{2}", CurrentMap.Info.FileName, block.X, block.Y, MonsterInfo.MonsterName));
                }
            }
        }

        public override void ProcessAI()
        {
            base.ProcessAI();

            if (Dead) return;

            if (!Closed && CloseTime > DateTime.MinValue && CloseTime < SEnvir.Now)
            {
                foreach (PlayerObject player in CurrentMap.Players)
                {
                    int distance = Functions.Distance(player.CurrentLocation, CurrentLocation);

                    if (distance <= 4) return;
                }

                CloseDoor();
                CloseTime = DateTime.MinValue;
            }
        }

        public override void ProcessSearch()
        {
            if (SEnvir.Now < SearchTime || CurrentMap.Players.Count == 0) return;

            SearchTime = SEnvir.Now + SearchDelay;

            if (Closed && AutoOpen)
            {
                if (War == null)
                {
                    foreach (PlayerObject player in CurrentMap.Players)
                    {
                        int distance = Functions.Distance(player.CurrentLocation, CurrentLocation);

                        if (distance > 4) continue;

                        if (player.Connection?.Account.GuildMember?.Guild != Guild) continue;

                        OpenDoor();
                        CloseTime = SEnvir.Now.AddSeconds(10);
                        break;
                    }
                }
            }
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (!Closed) return 0;

            CheckDirection();

            //if (War == null) return 0;

            //TODO - Only the attacking guild

            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
        }

        public override void Turn(MirDirection dir) { }

        public override bool Walk(MirDirection dir) { return false; }

        public override void Die()
        {
            base.Die();

            ActiveDoorWall(false);

            DeadTime = SEnvir.Now.AddYears(1);
        }

        public void CloseDoor()
        {
            if (Closed) return;

            Direction = (MirDirection)(3 - GetDamageLevel());

            Closed = true;

            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActiveDoorWall(true);
        }

        public void OpenDoor()
        {
            if (!Closed) return;

            Direction = (MirDirection)7;
            Closed = false;

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActiveDoorWall(false);
        }

        public void RepairGate()
        {
            if (CurrentHP <= 0)
            {
                Dead = false;
                SetHP(Stats[Stat.Health]);
                Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = false });
            }
            else
                SetHP(Stats[Stat.Health]);

            CheckDirection();

            ActiveDoorWall(Closed);
        }

        protected int GetDamageLevel()
        {
            int level = (int)Math.Round((double)(3 * CurrentHP) / Stats[Stat.Health]);

            if (level < 1) level = 1;

            return level;
        }

        public void CheckDirection()
        {
            MirDirection newDirection = (MirDirection)(3 - GetDamageLevel());

            if (newDirection != Direction)
            {
                Direction = newDirection;
                Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            }
        }

        private void ActiveDoorWall(bool closed)
        {
            foreach (var obj in BlockingObjects)
            {
                if (obj == null) continue;
                if (closed)
                    obj.Show();
                else
                    obj.Hide();
            }
        }
    }
}

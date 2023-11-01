using Library;
using Library.Network;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class Doll : MonsterObject
    {
        public MapObject DollTarget;
        public PlayerObject Caster;
        public DateTime AliveTime;

        public static List<MapObject> CursedList = new List<MapObject>();

        public override bool CanMove => false;
        public override bool CanAttack => false;

        public Doll()
        {
            Visible = false;
            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(500), ActionType.Function));
            Direction = MirDirection.DownLeft;
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            CurrentMap.Broadcast(CurrentLocation, new S.MapEffect { Location = CurrentLocation, Effect = Effect.CursedDoll });

            ActionTime = SEnvir.Now.AddSeconds(1);

            CursedList.Add(DollTarget);
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.Function:
                    Appear();
                    return;
            }

            base.ProcessAction(action);
        }

        public void Appear()
        {
            Visible = true;
            AddAllObjects();
        }

        public override void Process()
        {
            base.Process();

            if (Caster?.Node == null || Caster.Dead)
                SetHP(0);

            if (DollTarget?.Node == null || DollTarget.Dead)
                SetHP(0);

            if (AliveTime < SEnvir.Now)
                SetHP(0);
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

        public override void Die()
        {
            base.Die();

            DeadTime = SEnvir.Now.AddSeconds(1);
        }

        public override void OnDespawned()
        {
            base.OnDespawned();

            CursedList.Remove(DollTarget);
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {        
            if (DollTarget != null)
            {
                DollTarget.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
            }

            return 0;
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            S.ObjectMonster packet = (S.ObjectMonster)base.GetInfoPacket(ob);

            switch (DollTarget.Race)
            {
                case ObjectType.Player:
                    {
                        var name = DollTarget?.Name;
                        packet.CustomName = $"{name} Doll";

                        return packet;
                    }
                case ObjectType.Monster:
                    {
                        var name = (DollTarget as MonsterObject)?.MonsterInfo.MonsterName;
                        packet.CustomName = $"{name} Doll";

                        return packet;
                    }
            }

            return null;
        }
    }
}

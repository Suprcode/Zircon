using Library;
using Library.Network;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public sealed class Puppet : MonsterObject
    {
        public PlayerObject Player;

        public override bool CanMove => false;
        public override bool CanAttack => false;
        
        public Stats DarkStoneStats;

        public DateTime ExplodeTime = SEnvir.Now.AddSeconds(5);

        public override void Process()
        {
            base.Process();
            
            if (Player?.Node == null)
            {
                Despawn();
                return;
            }

            if (Dead || SEnvir.Now < ExplodeTime || Player.Dead) return;

            SetHP(0);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            SetHP(0);
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            int value = base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);


            SetHP(0);

            return value;
        }

        public override void ProcessSearch()
        {
        }

        public override void OnDespawned()
        {
            base.OnDespawned();

            Player?.Pets.Remove(this);
        }
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            Player?.Pets.Remove(this);
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

            if (Player?.Node == null) return;

            List<MapObject> targets = Player.GetTargets(CurrentMap, CurrentLocation, 2);
            foreach (MapObject target in targets)
            {
                Player.ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(800),
                    ActionType.DelayedMagicDamage,
                    Magics.Select(x => x.Info.Magic).ToList(),
                    target,
                    Functions.InRange(target.CurrentLocation, CurrentLocation, 1),
                    DarkStoneStats,
                    0));
            }

            var element = Functions.GetAffinityElement(DarkStoneStats);

            Effect effect;

            switch (element)
            {
                default:
                    effect = Effect.Puppet;
                    break;
                case Element.Fire:
                    effect = Effect.PuppetFire;
                    break;
                case Element.Ice:
                    effect = Effect.PuppetIce;
                    break;
                case Element.Lightning:
                    effect = Effect.PuppetLightning;
                    break;
                case Element.Wind:
                    effect = Effect.PuppetWind;
                    break;
            }

            Broadcast(new S.ObjectEffect { Effect = effect, ObjectID = ObjectID });

            DeadTime = SEnvir.Now.AddSeconds(2);
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            if (Player?.Node == null) return null;

            S.ObjectPlayer packet = (S.ObjectPlayer) Player.GetInfoPacket(null);

            packet.ObjectID = ObjectID;
            packet.Location = CurrentLocation;
            packet.Direction = Direction;
            packet.Dead = Dead;
            packet.Buffs.Remove(BuffType.Cloak);
            packet.Buffs.Remove(BuffType.Transparency);

            return packet;
        }
    }
}

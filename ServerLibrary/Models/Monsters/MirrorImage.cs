using Library;
using Library.Network;
using Server.Envir;
using Server.Models.Magics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class MirrorImage : MonsterObject
    {
        public PlayerObject Player;
        public override bool CanMove => false;
        public override bool CanAttack => false;

        public int RangeChance = 5;

        public DateTime Cooldown;
        public Element Element { get; set; }

        public Stats DarkStoneStats;

        public DateTime ExplodeTime = SEnvir.Now.AddSeconds(15);

        public Point Location;

        public override void Process()
        {
            base.Process();

            if (Player?.Node == null)
            {
                Despawn();
                return;
            }

            if ((SEnvir.Now > ExplodeTime || Player.Dead) && !Dead)
            {
                SetHP(0);
                return;
            }

            if (Dead && Spawned)
            {
                SetHP(0);
                return;
            }

        }
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            RangeAttack();
        }

        public virtual void RangeAttack()
        {
            if (SEnvir.Now < Cooldown) return;

            switch (Element)
            {
                case Element.Fire:
                    AttackAoE(2, MagicType.FireStorm, Element.Ice, Player.GetMC());
                    break;
                case Element.Ice:
                    AttackAoE(2, MagicType.IceStorm, Element.Ice, Player.GetMC());
                    break;
                case Element.Lightning:
                    AttackAoE(2, MagicType.ChainLightning, Element.Ice, Player.GetMC());
                    break;
                case Element.Wind:
                    AttackAoE(2, MagicType.DragonTornado, Element.Ice, Player.GetMC());
                    break;
            }

            Cooldown = SEnvir.Now.AddSeconds(1.5);
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return 0;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            Player?.Pets.Remove(this);
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }

        public override void Die()
        {
            Dead = true;

            Master = null;

            Broadcast(new S.ObjectEffect { Effect = Effect.MirrorImage, ObjectID = ObjectID });

            DeadTime = SEnvir.Now;
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            if (Player?.Node == null) return null;

            S.ObjectPlayer packet = (S.ObjectPlayer)Player.GetInfoPacket(null);

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

using Library;
using Library.Network;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class BlockingObject : MonsterObject
    {
        public MonsterObject Parent;

        public override bool CanAttack => false;
        public override bool CanMove => false;
        public override bool Blocking => Visible && Parent.Blocking;

        public BlockingObject(MonsterObject parent) : base()
        {
            Parent = parent;
            Visible = true;
        }

        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }

        public override void Die()
        {
            Hide();
        }

        public override bool Walk(MirDirection dir) { return false; }

        public override void ProcessRoam() { }

        public override void ProcessSearch() { }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return Parent.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
        }

        public void Hide()
        {
            Visible = false;

            if (CurrentMap == null) return;

            Broadcast(new S.ObjectHide { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public void Show()
        {
            Visible = true;

            if (CurrentMap == null) return;

            AddAllObjects();
            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }

        public override Packet GetInfoPacket(PlayerObject ob)
        {
            S.ObjectMonster packet = (S.ObjectMonster)base.GetInfoPacket(ob);

            packet.CustomName = " ";

            return packet;
        }
    }
}

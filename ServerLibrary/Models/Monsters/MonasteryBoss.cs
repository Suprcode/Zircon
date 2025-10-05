using Library;

namespace Server.Models.Monsters
{
    class MonasteryBoss : PinkBat
    {
        private bool HasDied;

        public override void Die()
        {
            base.Die();

            if (HasDied) return;

            HasDied = true;

            SpawnMinions(1, 0, null); // 1 in 15 to spawn two.
        }

        public override void RangeAttack()
        {
            if (Functions.InRange(Target.CurrentLocation, CurrentLocation, Globals.MagicRange))
                AttackMagic(MagicType.GreenSludgeBall, Element.Wind, true);
        }
    }
}

using Library;
using Server.DBModels;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ChainOfFire)]
    public class ChainOfFire : MagicObject
    {
        protected override Element Element => Element.None;

        public ChainOfFire(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Level 0 - slows down all movement
            //Level 1 - siphons damage
            //Level 2 - Continuous fire damage to all connected
            //Level 3 - Center monster dies, inflicts damage to all connected
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            if (target?.Node == null || !target.Dead) return;

            var cells = CurrentMap.GetCells(target.CurrentLocation, 0, 2);

            foreach (var cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int j = cell.Objects.Count - 1; j >= 0; j--)
                {
                    if (j >= cell.Objects.Count) continue;
                    MapObject ob = cell.Objects[j];
                    if (!Player.CanAttackTarget(ob)) continue;

                    Player.MagicAttack(new List<MagicType> { Type }, ob, false);
                }
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetMC() * Magic.GetPower() / 100;

            return power;
        }
    }
}
using Library;
using Server.DBModels;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SoulResonance)]
    public class SoulResonance : MagicObject
    {
        protected override Element Element => Element.None;

        public SoulResonance(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO
            //Increase target max HP by depending on your own max HP. Only in groups. Cancelled when ungrouped or dead. Infinite buff.
            //If they die, you die

            //MagicEx7 - 500
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            var realTargets = new HashSet<MapObject>();

            if (Player.InGroup(target))
                realTargets.Add(target);
         
            if (target == null)
                response.Locations.Add(location);

            return response;
        }
    }
}

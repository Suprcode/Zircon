using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.MeteorShower)]
    public class MeteorShower : MagicObject
    {
        protected override Element Element => Element.Fire;

        public MeteorShower(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetBurn(int burn, Stats stats = null)
        {
            var burning = GetAugmentedSkill(MagicType.Burning);

            if (burning != null)
            {
                return burning.GetPower();
            }

            return base.GetBurn(burn, stats);
        }

        public override int GetBurnLevel(int burnLevel, Stats stats = null)
        {
            var burning = GetAugmentedSkill(MagicType.Burning);

            if (burning != null)
            {
                return burning.Level + 1;
            }

            return base.GetBurnLevel(burnLevel, stats);
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var realTargets = new HashSet<MapObject>();
            var possibleTargets = Player.GetTargets(Player.CurrentMap, location, 3);

            while (realTargets.Count < 6 + Magic.Level)
            {
                if (possibleTargets.Count == 0) break;

                MapObject ob = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                possibleTargets.Remove(ob);

                if (!Functions.InRange(Player.CurrentLocation, ob.CurrentLocation, Globals.MagicRange)) continue;

                realTargets.Add(ob);
            }

            foreach (MapObject ob in realTargets)
            {
                var delay = GetDelayFromDistance(500, ob);

                response.Targets.Add(ob.ObjectID);
                ActionList.Add(new DelayedAction( delay, ActionType.DelayMagic, Type, ob));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            var damage = Player.MagicAttack(new List<MagicType> { Type }, target);

            if (damage > 0)
            {
                var burning = GetAugmentedSkill(MagicType.Burning);

                if (burning != null)
                {
                    Player.LevelMagic(burning);
                }
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetMC();

            return power;
        }
    }
}

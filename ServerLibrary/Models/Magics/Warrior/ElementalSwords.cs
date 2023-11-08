using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalSwords)]
    public class ElementalSwords : MagicObject
    {
        protected override Element Element => Element.None;

        public ElementalSwords(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Redo anim
            //Magic Ex10 - 0
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var realTargets = new HashSet<MapObject>();

            if (Player.CanAttackTarget(target))
            {
                realTargets.Add(target);
            }

            var possibleTargets = Player.GetTargets(Player.CurrentMap, location, 3);

            while (realTargets.Count < 1 + Magic.Level)
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
                Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, ob));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            var damage = Player.MagicAttack(new List<MagicType> { Type }, target);

            if (damage > 0 && target.Dead && SEnvir.Random.Next(4) == 0)
            {
                var gain = (Player.Stats[Stat.Mana] - Player.CurrentMP) * (10 + Magic.Level * 10) / 100;

                Player.ChangeMP(gain);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}

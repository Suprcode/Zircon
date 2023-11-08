using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics.Taoist
{
    [MagicType(MagicType.CorpseExploder)]
    public class CorpseExploder : MagicObject
    {
        protected override Element Element => Element.None;

        public CorpseExploder(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (target == null || !(target.Race == ObjectType.Player || target.Race == ObjectType.Monster) || !target.Dead)
            {
                response.Ob = null;
                response.Locations.Add(location);
                return response;
            }

            if (!Player.UseAmulet(2, 0))
            {
                response.Cast = false;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var cells = CurrentMap.GetCells(location, 0, 1);

            var delay = SEnvir.Now.AddMilliseconds(500 + (Functions.Distance(CurrentLocation, target.CurrentLocation) * 48) + 500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target, cells));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var target = (MapObject)data[1];
            var cells = (List<Cell>)data[2];

            if (target?.Node == null || !target.Dead || Player.CurrentMap != target.CurrentMap) return;

            switch (target.Race)
            {
                case ObjectType.Player:
                    ((PlayerObject)target).TownRevive();
                    break;
                case ObjectType.Monster:
                    ((MonsterObject)target).DeadTime = SEnvir.Now.AddMilliseconds(500);
                    break;
            }

            foreach (var cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (i >= cell.Objects.Count) continue;
                    MapObject ob = cell.Objects[i];
                    if (!Player.CanAttackTarget(ob)) continue;

                    Player.MagicAttack(new List<MagicType> { Type }, ob);
                }
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC();

            return power;
        }
    }
}

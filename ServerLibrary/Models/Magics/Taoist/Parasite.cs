using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Parasite)]
    public class Parasite : MagicObject
    {
        protected override Element Element => Element.None;

        public Parasite(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target))
            {
                response.Locations.Add(location);
                response.Ob = null;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, location) * 48);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || (ob.Poison & PoisonType.Parasite) == PoisonType.Parasite) return;

            ob.ApplyPoison(new Poison
            {
                Value = Magic.GetPower(),
                Type = PoisonType.Parasite,
                Owner = Player,
                TickCount = 10 + Magic.Level * 5,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            Player.LevelMagic(Magic);
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower() + Player.GetSC() / 2;

            return power;
        }

        public void Explode(MapObject target)
        {
            var cells = CurrentMap.GetCells(CurrentLocation, 0, 1);

            foreach (var cell in cells)
            {
                if (cell?.Objects == null) continue;

                for (int j = cell.Objects.Count - 1; j >= 0; j--)
                {
                    if (j >= cell.Objects.Count) continue;
                    MapObject ob = cell.Objects[j];
                    if (!Player.CanAttackTarget(ob)) continue;

                    Player.MagicAttack(new List<MagicType> { MagicType.Parasite }, ob, true);
                }
            }

            target.Broadcast(new S.ObjectEffect { ObjectID = target.ObjectID, Effect = Effect.ParasiteExplode });
        }
    }
}

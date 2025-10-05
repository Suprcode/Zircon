using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DemonExplosion)]
    public class DemonExplosion : MagicObject
    {
        protected override Element Element => Element.Phantom;

        public DemonExplosion(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (Player.Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead) || !Player.UseAmulet(20, 0, out Stats stats))
            {
                response.Cast = false;
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, stats));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var stats = (Stats)data[1];

            MonsterObject pet = Player.Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;

            int damage = pet.Stats[Stat.Health];
            pet.Broadcast(new S.ObjectEffect { Effect = Effect.DemonExplosion, ObjectID = pet.ObjectID });

            List<MapObject> targets = Player.GetTargets(pet.CurrentMap, pet.CurrentLocation, 2);

            pet.ChangeHP(-damage * 75 / 100);

            int damagePvE = damage * Magic.GetPower() / 100 + Player.GetSC() * 3;
            int damagePvP = damage * Magic.GetPower() / 100 + Player.GetSC() * 3;

            if (stats != null && stats.GetAffinityValue(Element.Phantom) > 0)
            {
                damagePvE += Player.GetElementPower(ObjectType.Monster, Stat.PhantomAttack) * 8;
                damagePvP += Player.GetElementPower(ObjectType.Player, Stat.PhantomAttack) * 8;
            }

            var delay = SEnvir.Now.AddMilliseconds(800);

            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(delay, ActionType.DelayedMagicDamage, new List<MagicType> { Type }, target, true, null, target.Race == ObjectType.Player ? damagePvP : damagePvE));
            }

            List<Cell> cells = pet.CurrentMap.GetCells(pet.CurrentLocation, 0, 3, false, true);

            foreach (var cell in cells)
            {
                if (cell.Objects != null)
                {
                    for (int i = cell.Objects.Count - 1; i >= 0; i--)
                    {
                        if (cell.Objects[i].Race != ObjectType.Spell) continue;

                        SpellObject spell = (SpellObject)cell.Objects[i];

                        if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.Tempest) continue;

                        spell.Despawn();
                    }
                }

                SpellObject ob = new SpellObject
                {
                    DisplayLocation = cell.Location,
                    TickCount = (Magic.Level + 2),
                    TickFrequency = TimeSpan.FromSeconds(2),
                    Owner = Player,
                    Effect = SpellEffect.FireWall,
                    Magic = Magic,
                };

                ob.Spawn(cell.Map, cell.Location);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = extra;

            return power;
        }
    }
}

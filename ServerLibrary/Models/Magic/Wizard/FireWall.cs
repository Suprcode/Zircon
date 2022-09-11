using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Server.Models.Magic
{
    [MagicType(MagicType.FireWall)]
    public class FireWall : MagicObject
    {
        public override Element Element => Element.Fire;
        public override bool CanStuck => false;

        public FireWall(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override int GetPower()
        {
            var power = Magic.GetPower() + Player.GetMC();

            power = (int)(power * 0.60F);

            return power;
        }

        public override MagicCast Cast(MapObject target, Point location)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Functions.InRange(Player.CurrentLocation, location, Globals.MagicRange))
            {
                response.Cast = false;
                return response;
            }

            foreach (ConquestWar war in SEnvir.ConquestWars)
            {
                if (war.Map != Player.CurrentMap) continue;

                for (int i = Player.SpellList.Count - 1; i >= 0; i--)
                {
                    if (Player.SpellList[i].Effect != SpellEffect.FireWall) continue;

                    Player.SpellList[i].Despawn();
                }

                break;
            }

            int power = (Magic.Level + 2) * 5;

            var delay = SEnvir.Now.AddMilliseconds(500);

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, Player.CurrentMap.GetCell(Functions.Move(location, MirDirection.Up)), power));
            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, Player.CurrentMap.GetCell(Functions.Move(location, MirDirection.Down)), power));
            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, Player.CurrentMap.GetCell(location), power));
            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, Player.CurrentMap.GetCell(Functions.Move(location, MirDirection.Left)), power));
            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, Player.CurrentMap.GetCell(Functions.Move(location, MirDirection.Right)), power));

            return response;
        }

        public override void Complete(params object[] data)
        {
            Cell cell = (Cell)data[2];
            int power = (int)data[3];

            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = Player,
                Effect = SpellEffect.FireWall,
                Magic = Magic,
            };

            ob.Spawn(cell.Map, cell.Location);

            Player.LevelMagic(Magic);
        }
    }
}

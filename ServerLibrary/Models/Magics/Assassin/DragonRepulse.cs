using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DragonRepulse)]
    public class DragonRepulse : MagicObject
    {
        protected override Element Element => Element.Lightning;
        protected override int Repel => 5;

        public DragonRepulse(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var buff = Player.BuffAdd(BuffType.DragonRepulse, TimeSpan.FromSeconds(6), null, true, false, TimeSpan.FromSeconds(1));
            buff.TickTime = TimeSpan.FromMilliseconds(500);

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            Player.MagicAttack(new List<MagicType> { Type }, target, true);

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);
            if (target.Pushed(dir, 1) == 0)
            {
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int i = 1; i < 2; i++)
                {
                    if (target.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                    if (target.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                }
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power = Player.GetDC() * Magic.GetPower() / 100 + Player.Level;

            return power;
        }

        public override bool CheckCost()
        {
            if (Player.Stats[Stat.Health] * Magic.Cost / 1000 >= Player.CurrentHP || Player.CurrentHP < Player.Stats[Stat.Health] / 10)
            {
                return false;
            }

            if (Player.Stats[Stat.Mana] * Magic.Cost / 1000 >= Player.CurrentMP || Player.CurrentMP < Player.Stats[Stat.Mana] / 10)
            {
                return false;
            }

            return true;
        }

        public override void MagicConsume()
        {
            Player.ChangeHP(-(Player.Stats[Stat.Health] * Magic.Cost / 1000));
            Player.ChangeMP(-(Player.Stats[Stat.Mana] * Magic.Cost / 1000));
        }
    }
}

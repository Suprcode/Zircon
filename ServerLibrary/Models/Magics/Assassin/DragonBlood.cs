using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DragonBlood)]
    public class DragonBlood : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        private bool CanAttack;
        private readonly int MaxStack = 4;

        public DragonBlood(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (CanAttack && attackType == Type)
            {
                CanAttack = false;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });
                response.Cast = true;
                response.Magics.Add(Type);
            }

            if (!CanAttack && SEnvir.Random.Next(5) == 0)
            {
                UserItem poison = Player.Equipment[(int)EquipmentSlot.Poison];

                if (poison == null || poison.Info.ItemType != ItemType.Poison || poison.Count < 1 || poison.Info.Shape != 2)
                    return response;

                if (SEnvir.Random.Next(10) > 0)
                {
                    if (!Player.UsePoison(1, out _, 2))
                        return response;
                }

                CanAttack = true;
                Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = true });
            }

            return response;
        }

        public override void AttackComplete(MapObject target)
        {
            var p = target.PoisonList.FirstOrDefault(x => x.Type == PoisonType.Green);

            if (p != null && p.Extra is int stack && stack < MaxStack)
            {
                var value = p.Value / stack;

                p.Extra = ++stack;
                p.Value = value * stack;
            }
            else
            {
                target.ApplyPoison(new Poison
                {
                    Owner = Player,
                    Type = PoisonType.Green,
                    TickCount = 10,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    Value = Player.GetSP() * Magic.GetPower() / 100,
                    Extra = 1
                });
            }

            Player.LevelMagic(Magic);
        }
    }
}

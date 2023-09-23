using Library;
using Library.Network.ClientPackets;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Karma)]
    public class Karma : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool AttackSkill => true;

        public Karma(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override AttackCast AttackCast(MagicType attackType)
        {
            var response = new AttackCast();

            if (attackType != Type)
                return response;

            if (Player.Level < Magic.Info.NeedLevel1)
                return response;

            if (SEnvir.Now < Magic.Cooldown)
                return response;

            if (!Player.Buffs.Any(x => x.Type == BuffType.Cloak))
                return response;

            int cost = Player.Stats[Stat.Health] * Magic.Cost / 100;

            var release = GetAugmentedSkill(MagicType.Release);

            if (release != null && Player.Level >= release.Info.NeedLevel1)
            {
                cost -= cost * release.GetPower() / 100;
                response.Magics.Add(MagicType.Release);
            }

            if (cost < Player.CurrentHP)
            {
                Player.ChangeHP(-cost);

                response.Cast = true;
                response.Magics.Add(Type);

                return response;
            }


            return response;
        }

        public override void Cooldown(int attackDelay)
        {
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            Player.UseItemTime = SEnvir.Now.AddSeconds(10);

            if (Player.Magics.TryGetValue(MagicType.Karma, out UserMagic magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
            }

            if (Player.Magics.TryGetValue(MagicType.SummonPuppet, out magic))
            {
                magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
            }
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC();

            var karmaDamage = ob.CurrentHP * Magic.GetPower() / 100;

            if (ob.Race == ObjectType.Monster)
            {
                if (((MonsterObject)ob).MonsterInfo.IsBoss)
                    karmaDamage = Magic.GetPower() * 20;
                else
                    karmaDamage /= 4;
            }

            /*   buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);
               if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
               {
                   power += power * augMagic.GetPower() / 100;
                   magics.Add(augMagic);
                   BuffRemove(buff);
                   if (buff.Stats[Stat.TheNewBeginning] > 1)
                       BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
               }
               */
            ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.Karma });

            return power;
        }
    }
}

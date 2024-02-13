using Library;
using Server.DBModels;
using Server.Envir;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
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

            if (SEnvir.Now < Magic.Cooldown)
                return response;

            if (!Player.Buffs.Any(x => x.Type == BuffType.Cloak))
                return response;

            int cost = Player.Stats[Stat.Health] * Magic.Cost / 100;

            var release = GetAugmentedSkill(MagicType.Release);

            if (release != null)
            {
                cost -= cost * release.GetPower() / 100;
                response.Magics.Add(MagicType.Release);
            }

            var resolution = GetAugmentedSkill(MagicType.Resolution);

            if (resolution != null)
            {
                response.Magics.Add(MagicType.Resolution);
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

        public override void AttackLocationSuccess(int attackDelay)
        {
            Player.Enqueue(new S.MagicToggle { Magic = Type, CanUse = false });

            Player.UseItemTime = SEnvir.Now.AddSeconds(10);

            if (Player.GetMagic(MagicType.Karma, out Karma karma))
            {
                karma.Magic.Cooldown = SEnvir.Now.AddMilliseconds(karma.Magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = karma.Magic.Info.Index, Delay = karma.Magic.Info.Delay });
            }

            if (Player.GetMagic(MagicType.SummonPuppet, out SummonPuppet summonPuppet))
            {
                summonPuppet.Magic.Cooldown = SEnvir.Now.AddMilliseconds(summonPuppet.Magic.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = summonPuppet.Magic.Info.Index, Delay = summonPuppet.Magic.Info.Delay });
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC();

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

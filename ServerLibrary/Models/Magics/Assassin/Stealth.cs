using Library;
using Server.DBModels;
using Server.Envir;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Stealth)]
    public class Stealth : MagicObject
    {
        protected override Element Element => Element.None;

        public Stealth(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public bool CheckCloak()
        {
            BuffInfo buff = Player.Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);

            if (buff != null && Player.GetMagic(MagicType.TheNewBeginning, out TheNewBeginning theNewBeginning))
            {
                if (SEnvir.Random.Next(100) > Magic.GetPower())
                {
                    if (buff.Stats[Stat.TheNewBeginning] > 1)
                    {
                        Player.DecreaseBuffCharge(buff);
                        Player.Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                    }
                    else
                    {
                        Player.BuffRemove(buff);
                    }
                }
                else
                {
                    Player.LevelMagic(Magic);
                }

                Player.LevelMagic(theNewBeginning.Magic);

                return true;
            }

            return false;
        }
    }
}

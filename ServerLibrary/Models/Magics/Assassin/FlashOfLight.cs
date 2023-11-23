using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FlashOfLight)]
    public class FlashOfLight : MagicObject
    {
        protected override Element Element => Element.None;

        public FlashOfLight(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            /*   buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);

                        if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                        {
                            BuffRemove(buff);
                            magics.Add(augMagic);
                            if (buff.Stats[Stat.TheNewBeginning] > 1)
                                BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                        }*/

            var delay = SEnvir.Now.AddMilliseconds(400);

            for (int i = 1; i <= 2; i++)
            {
                var loc = Functions.Move(CurrentLocation, direction, i);
                Cell cell = CurrentMap.GetCell(loc);

                if (cell == null) continue;
                response.Locations.Add(cell.Location);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, cell));
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var cell = (Cell)data[1];

            if (cell?.Objects == null) return;
            if (cell.Objects.Count == 0) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!Player.CanAttackTarget(ob)) continue;

                Player.MagicAttack(new List<MagicType> { Type }, ob, true);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC() * Magic.GetPower() / 100;

            BuffInfo buff = Player.Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);

            if (buff != null && Player.GetMagic(MagicType.TheNewBeginning, out TheNewBeginning theNewBeginning))
            {
                for (int i = 0; i <= buff.Stats[Stat.TheNewBeginning]; i++)
                {
                    power += 80;
                }

                if (buff.Stats[Stat.TheNewBeginning] > 1)
                {
                    Player.DecreaseBuffCharge(buff);
                    Player.Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                }
                else
                {
                    Player.BuffRemove(buff);
                }

                Player.LevelMagic(theNewBeginning.Magic);
            }

            ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FlashOfLight });

            return power;
        }
    }
}

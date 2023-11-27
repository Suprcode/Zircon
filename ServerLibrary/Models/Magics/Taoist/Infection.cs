using Library;
using Server.DBModels;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Infection)]
    public class Infection : MagicObject
    {
        protected override Element Element => Element.None;
        
        public Infection(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];

            var targets = Player.GetTargets(ob.CurrentMap, ob.CurrentLocation, 1);

            foreach (MapObject target in targets)
            {
                if (target.Race != ObjectType.Monster) continue;

                if (((MonsterObject)target).MonsterInfo.IsBoss) continue;

                if (target.PoisonList.Any(x => x.Type == PoisonType.Parasite)) continue;

                foreach (var p in ob.PoisonList)
                {
                    if (target.PoisonList.Any(x => x.Type == p.Type)) continue;

                    target.ApplyPoison(new Poison
                    {
                        Value = (p.Value * Magic.Level + 1) / 10,
                        Owner = p.Owner,
                        TickCount = Magic.GetPower(),
                        TickFrequency = p.TickFrequency,
                        Type = p.Type
                    });
                }

                Player.LevelMagic(Magic);
                break;
            }
        }
    }
}

using Library;
using Server.DBModels;
using Server.Envir;
using System.Drawing;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ExpelUndead)]
    public class ExpelUndead : MagicObject
    {
        protected override Element Element => Element.None;

        public ExpelUndead(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanAttackTarget(target) || target.Race != ObjectType.Monster || !((MonsterObject)target).MonsterInfo.Undead)
            {
                response.Ob = null;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MonsterObject ob = (MonsterObject)data[1];

            if (ob?.Node == null || !Player.CanAttackTarget(ob) || ob.MonsterInfo.IsBoss || ob.Level >= 70) return;

            if (ob.Target == null && ob.CanAttackTarget(Player))
                ob.Target = Player;
            if (ob.Level >= Player.Level - 1 + SEnvir.Random.Next(4)) return;

            if (SEnvir.Random.Next(100) >= 35 + Magic.Level * 9 + (Player.Level - ob.Level) * 5 + Player.Stats[Stat.PhantomAttack] / 2) return;

            if (ob.EXPOwner == null && ob.Master == null)
                ob.EXPOwner = Player;

            ob.SetHP(0);

            Player.LevelMagic(Magic);
        }
    }
}

using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SummonDead)]
    public class SummonDead : MagicObject
    {
        protected override Element Element => Element.None;

        public SummonDead(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (target == null || !(target.Race == ObjectType.Player || target.Race == ObjectType.Monster) || !target.Dead || target is UndeadSoul)
            {
                response.Ob = null;
                response.Locations.Add(location);
                return response;
            }

            if (!Player.UseAmulet(10, 1))
            {
                response.Cast = false;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500);

            var info = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.UndeadSoul);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target, info));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var target = (MapObject)data[1];
            var info = (MonsterInfo)data[2];

            if (target?.Node == null || !target.Dead || Player.CurrentMap != target.CurrentMap) return;

            if (info == null) return;

            if (Player.Pets.Count >= 2) return;

            if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
            {
                if (SEnvir.Random.Next(2) == 0) Player.LevelMagic(Magic);
                return;
            }

            Cell cell = target.CurrentMap.GetCell(target.CurrentLocation);

            switch (target.Race)
            {
                case ObjectType.Player:
                    ((PlayerObject)target).TownRevive();
                    break;
                case ObjectType.Monster:
                    ((MonsterObject)target).DeadTime = SEnvir.Now.AddMilliseconds(500);
                    break;
            }

            MonsterObject ob = MonsterObject.GetMonster(info);

            if (ob == null) return;

            ob.PetOwner = Player;
            Player.Pets.Add(ob);

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;
            ob.Magics.Add(Magic);
            ob.SummonLevel = Magic.Level * 2;
            ob.TameTime = SEnvir.Now.AddDays(365);

            if (cell == null || cell.Movements != null || !ob.Spawn(cell.Map, cell.Location))
                ob.Spawn(CurrentMap, CurrentLocation);

            ob.SetHP(ob.Stats[Stat.Health]);

            Player.LevelMagic(Magic);
        }
    }
}

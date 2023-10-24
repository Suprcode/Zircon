using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElectricShock)]
    public class ElectricShock : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public ElectricShock(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (!Player.CanAttackTarget(target) || target.Race != ObjectType.Monster)
            {
                response.Ob = null;
                response.Locations.Add(location);
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

            if (ob?.Node == null || !Player.CanAttackTarget(ob)) return;

            if (ob.MonsterInfo.IsBoss) return;

            if (SEnvir.Random.Next(Globals.MagicMaxLevel + 1) > Magic.Level)
            {
                if (SEnvir.Random.Next(2) == 0) Player.LevelMagic(Magic);
                return;
            }

            Player.LevelMagic(Magic);

            if (ob.PetOwner == Player)
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(Magic.Level * 5 + 10);
                ob.Target = null;
                return;
            }

            if (SEnvir.Random.Next(2) > 0)
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(Magic.Level * 5 + 10);
                ob.Target = null;
                return;
            }

            if (ob.Level > Player.Level + 2 || !ob.MonsterInfo.CanTame) return;

            if (SEnvir.Random.Next(Player.Level + 20 + Magic.Level * 5) <= ob.Level + 10)
            {
                if (SEnvir.Random.Next(5) > 0 && ob.PetOwner == null)
                {
                    ob.RageTime = SEnvir.Now.AddSeconds(SEnvir.Random.Next(20) + 10);
                    ob.Target = null;
                }
                return;
            }

            if (Player.Pets.Count >= 3) return;

            if (SEnvir.Random.Next(4) > 0) return;

            if (SEnvir.Random.Next(20) == 0)
            {
                if (ob.EXPOwner == null && ob.PetOwner == null)
                    ob.EXPOwner = Player;

                ob.Die();
                return;
            }

            if (ob.PetOwner != null)
            {
                int hp = Math.Max(1, ob.Stats[Stat.Health] / 10);

                if (hp < ob.CurrentHP) ob.SetHP(hp);

                ob.PetOwner.Pets.Remove(ob);
                ob.PetOwner = null;
                ob.Magics.Clear();
            }
            else if (ob.SpawnInfo != null)
            {
                ob.SpawnInfo.AliveCount--;
                ob.SpawnInfo = null;
            }

            ob.PetOwner = Player;
            Player.Pets.Add(ob);

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;

            ob.TameTime = SEnvir.Now.AddHours(Magic.Level + 1);
            ob.Target = null;
            ob.RageTime = DateTime.MinValue;
            ob.ShockTime = DateTime.MinValue;
            ob.Magics.Add(Magic);
            ob.SummonLevel = Magic.Level;
            ob.RefreshStats();

            ob.Broadcast(new S.ObjectPetOwnerChanged { ObjectID = ob.ObjectID, PetOwner = Player.Name });
        }
    }
}

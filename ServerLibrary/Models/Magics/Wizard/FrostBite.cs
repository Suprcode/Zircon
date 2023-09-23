﻿using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.FrostBite)]
    public class FrostBite : MagicObject
    {
        protected override Element Element => Element.Ice;
        public override bool UpdateCombatTime => false;
        protected override int Slow => 5;
        protected override int SlowLevel => 5;

        public FrostBite(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            var delay = SEnvir.Now.AddMilliseconds(600);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.FrostBite)) return;

            Stats buffStats = new Stats
            {
                [Stat.FrostBiteDamage] = Player.GetMC() + Player.Stats[Stat.IceAttack] * 2 + Magic.GetPower(),
                [Stat.FrostBiteMaxDamage] = Player.Stats[Stat.MaxMC] * 50 + Player.Stats[Stat.IceAttack] * 70,
            };

            Player.BuffAdd(BuffType.FrostBite, TimeSpan.FromSeconds(3 + Magic.Level * 3), buffStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }

        public void FrostBiteEnd(BuffInfo buff)
        {
            Player.LevelMagic(Magic);

            foreach (MapObject ob in Player.GetTargets(CurrentMap, CurrentLocation, 3))
            {
                if (!Player.CanAttackTarget(ob)) continue;

                if (ob.Race != ObjectType.Monster) continue;

                MonsterObject mob = (MonsterObject)ob;

                if (mob.MonsterInfo.IsBoss) continue;

                var delay = SEnvir.Now.AddMilliseconds(500);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayedMagicDamage, Type, ob, true, buff.Stats));
            }
        }

        public override int ModifyPower1(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Math.Min(stats[Stat.FrostBiteDamage], stats[Stat.FrostBiteMaxDamage]) - Player.Stats[Stat.IceAttack] * 2;

            return power;
        }
    }
}

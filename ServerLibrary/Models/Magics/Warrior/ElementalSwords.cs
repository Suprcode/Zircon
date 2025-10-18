using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.ElementalSwords)]
    public class ElementalSwords : MagicObject
    {
        protected override Element Element => Element.None;

        private int SwordCount = 0;
        private DateTime SwordTime = DateTime.MaxValue;

        public ElementalSwords(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //TODO - Redo anim
            //Magic Ex10 - 0
            //Swords appear over head, throws at enemy if they attack
            //https://www.youtube.com/watch?v=l8m9JipWIaA&t=1836s
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (SwordCount > 0)
            {
                response.Cast = false;
                return response;
            }

            SwordCount = 5;
            SwordTime = SEnvir.Now.AddSeconds(5);

            Player.BuffAdd(BuffType.ElementalSwords, TimeSpan.MaxValue, new Stats(), true, false, TimeSpan.Zero, extra: SwordCount);

            return response;
        }

        public override void Process()
        {
            base.Process();

            if (SwordCount == 0 || SEnvir.Now < SwordTime) return;

            List<MapObject> targets = [];

            foreach (var ob in Player.GetTargets(Player.CurrentMap, Player.CurrentLocation, 5))
            {
                if (!Player.CanAttackTarget(ob)) continue;

                if (ob is MonsterObject monsterObject && monsterObject.Target != Player) continue;

                targets.Add(ob);
            }

            if (targets.Count == 0) return;

            var target = targets[SEnvir.Random.Next(targets.Count)];

            SwordCount--;
            SwordTime = SEnvir.Now.AddSeconds(5);

            Player.BuffAdd(BuffType.ElementalSwords, TimeSpan.MaxValue, new Stats(), true, false, TimeSpan.Zero, extra: SwordCount);

            var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48);

            var dir = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);

            Player.Broadcast(new S.ObjectProjectile
            {
                ObjectID = Player.ObjectID,
                Direction = dir,
                CurrentLocation = Player.CurrentLocation,
                Type = Type,
                Targets = [target.ObjectID]
            });

            Player.ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target, target.CurrentLocation));
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject target = (MapObject)data[1];

            var damage = Player.MagicAttack(new List<MagicType> { Type }, target);

            if (damage > 0 && target.Dead && SEnvir.Random.Next(4) == 0)
            {
                var gain = (Player.Stats[Stat.Mana] - Player.CurrentMP) * (10 + Magic.Level * 10) / 100;

                Player.ChangeMP(gain);
            }
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Magic.GetPower();

            return power;
        }
    }
}

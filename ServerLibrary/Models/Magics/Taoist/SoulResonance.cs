using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.SoulResonance)]
    public class SoulResonance : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public SoulResonance(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.InGroup(target) || target.Buffs.Any(x => x.Type == BuffType.SoulResonance))
            {
                response.Ob = null;
                return response;
            }

            response.Targets.Add(target.ObjectID);

            var delay = GetDelayFromDistance(500, target);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, target));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var target = (PlayerObject)data[1];

            if (target?.Node == null || target.Dead || !Player.InGroup(target))
            {
                return;
            }

            Stats ownerStats = new()
            {
                [Stat.SoulResonance] = target.Character.Index
            };

            Stats targetStats = new()
            {
                [Stat.HealthPercent] = Magic.GetPower(),
                [Stat.SoulResonance] = Player.Character.Index
            };

            Player.BuffAdd(BuffType.SoulResonance, TimeSpan.MaxValue, ownerStats, false, false, TimeSpan.Zero);
            target.BuffAdd(BuffType.SoulResonance, TimeSpan.MaxValue, targetStats, false, false, TimeSpan.Zero);

            Player.LevelMagic(Magic);
        }

        public static void Remove(PlayerObject ob)
        {
            if (ob?.Node == null) return;

            var buff = ob.Buffs.FirstOrDefault(x => x.Type == BuffType.SoulResonance);

            if (buff != null)
            {
                var characterIndex = buff.Stats[Stat.SoulResonance];

                ob.BuffRemove(BuffType.SoulResonance);

                var character = SEnvir.GetCharacter(characterIndex);

                if (character?.Player?.Node != null)
                    character.Player.BuffRemove(BuffType.SoulResonance);
            }
        }

        public static void Activate(PlayerObject ob)
        {
            if (ob?.Node == null) return;

            var buff = ob.Buffs.FirstOrDefault(x => x.Type == BuffType.SoulResonance);

            if (buff != null)
            {
                var characterIndex = buff.Stats[Stat.SoulResonance];

                ob.BuffRemove(BuffType.SoulResonance);

                if (!ob.Dead)
                    ob.Die();

                var character = SEnvir.GetCharacter(characterIndex);

                if (character?.Player?.Node != null)
                {
                    character.Player.BuffRemove(BuffType.SoulResonance);

                    if (!character.Player.Dead)
                        character.Player.Die();
                }
            }
        }
    }
}

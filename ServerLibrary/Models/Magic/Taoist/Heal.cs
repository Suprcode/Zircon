using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magic
{
    [MagicType(MagicType.Heal)]
    public class Heal : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Heal(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            if (!Player.CanHelpTarget(target))
            {
                response.Ob = Player;
            }

            response.Targets.Add(response.Ob.ObjectID);

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagicNew, Type, response.Ob));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            MapObject ob = (MapObject)data[1];

            if (ob?.Node == null || !Player.CanHelpTarget(ob) || ob.CurrentHP >= ob.Stats[Stat.Health] || ob.Buffs.Any(x => x.Type == BuffType.Heal)) return;
            UserMagic empowered;
            int bonus = 0;
            int cap = 30;

            if (Player.Magics.TryGetValue(MagicType.EmpoweredHealing, out empowered) && Player.Level >= empowered.Info.NeedLevel1)
            {
                bonus = empowered.GetPower();
                cap += (1 + empowered.Level) * 30;

                Player.LevelMagic(empowered);
            }

            Stats buffStats = new Stats
            {
                [Stat.Healing] = Magic.GetPower() + Player.GetSC() + Player.Stats[Stat.HolyAttack] * 2 + bonus,
                [Stat.HealingCap] = cap, // empowered healing
            };

            ob.BuffAdd(BuffType.Heal, TimeSpan.FromSeconds(buffStats[Stat.Healing] / buffStats[Stat.HealingCap]), buffStats, false, false, TimeSpan.FromSeconds(1));
            Player.LevelMagic(Magic);
        }
    }
}

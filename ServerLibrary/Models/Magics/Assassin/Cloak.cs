using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Cloak)]
    public class Cloak : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Cloak(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override bool CheckCost()
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.Cloak)) return true;

            if (SEnvir.Now < Player.CombatTime.AddSeconds(10))
            {
                Player.Connection.ReceiveChat(Player.Connection.Language.CloakCombat, MessageType.System);

                foreach (SConnection con in Player.Connection.Observers)
                    con.ReceiveChat(con.Language.CloakCombat, MessageType.System);

                return true;
            }

            if (Player.Stats[Stat.Health] * Magic.Cost / 1000 >= Player.CurrentHP || Player.CurrentHP < Player.Stats[Stat.Health] / 10)
            {
                return false;
            }

            return true;
        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null
            };

            if (Player.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                return response;
            }

            var delay = SEnvir.Now.AddMilliseconds(500);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type));

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var forceGhost = false;
            var ob = Player;

            if (ob?.Node == null || !Player.CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Cloak)) return;

            var pledgeofBlood = GetAugmentedSkill(MagicType.PledgeOfBlood);

            int value = 0;
            if (pledgeofBlood != null)
            {
                value = pledgeofBlood.GetPower();

                Player.LevelMagic(pledgeofBlood);
            }

            Stats buffStats = new Stats
            {
                [Stat.Cloak] = 1,
                [Stat.CloakDamage] = Player.Stats[Stat.Health] * (20 - Magic.Level - value) / 1000,
            };

            ob.BuffAdd(BuffType.Cloak, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.FromSeconds(2));

            Player.LevelMagic(Magic);

            if (!forceGhost)
            {
                var ghostWalk = GetAugmentedSkill(MagicType.GhostWalk);

                if (ghostWalk == null) return;

                int rate = (ghostWalk.Level + 1) * 3;

                if (SEnvir.Random.Next(2 + rate) >= rate) return;

                Player.LevelMagic(ghostWalk);
            }

            ob.BuffAdd(BuffType.GhostWalk, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
        }

        public override void MagicConsume()
        {
            if (Player.Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                Player.BuffRemove(BuffType.Cloak);
                return;
            }

            Player.ChangeHP(-(Player.Stats[Stat.Health] * Magic.Cost / 1000));
        }

        public override void MagicFinalise()
        {
            
        }
    }
}

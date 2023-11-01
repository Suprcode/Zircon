using Library;
using Server.DBModels;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Resurrection)]
    public class Resurrection : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool UpdateCombatTime => false;

        public Resurrection(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = target
            };

            var realTargets = new HashSet<MapObject>();

            if ((Player.InGroup(target as PlayerObject) || Player.InGuild(target as PlayerObject)) && target.Dead)
            {
                realTargets.Add(target);
            }
            else
            {
                response.Ob = null;
            }

            var augmentResurrection = GetAugmentedSkill(MagicType.AugmentResurrection);

            if (augmentResurrection != null && SEnvir.Now > augmentResurrection.Cooldown)
            {
                var power = augmentResurrection.GetPower() + 1;

                var possibleTargets = Player.GetAllObjects(location, 3);

                while (power >= realTargets.Count)
                {
                    if (possibleTargets.Count == 0) break;

                    MapObject possibleTarget = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                    possibleTargets.Remove(possibleTarget);

                    if (!Functions.InRange(CurrentLocation, possibleTarget.CurrentLocation, Globals.MagicRange)) continue;

                    if (possibleTarget is PlayerObject && (Player.InGroup(possibleTarget as PlayerObject) || Player.InGuild(possibleTarget as PlayerObject)) && possibleTarget.Dead)
                        realTargets.Add(possibleTarget);
                }
            }

            var count = -1;

            var hasAugmentResurrection = false;

            foreach (MapObject realTarget in realTargets)
            {
                if (!Player.UseAmulet(1, 1))
                    break;

                if (augmentResurrection != null)
                {
                    hasAugmentResurrection = true;
                    count++;
                }

                response.Targets.Add(realTarget.ObjectID);

                var delay = SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, realTarget.CurrentLocation) * 48);

                ActionList.Add(new DelayedAction(delay, ActionType.DelayMagic, Type, realTarget, realTarget == target, hasAugmentResurrection));
            }

            if (count > 0)
            {
                augmentResurrection.Cooldown = SEnvir.Now.AddMilliseconds(augmentResurrection.Info.Delay);
                Player.Enqueue(new S.MagicCooldown { InfoIndex = augmentResurrection.Info.Index, Delay = augmentResurrection.Info.Delay });
            }

            return response;
        }

        public override void MagicComplete(params object[] data)
        {
            var ob = (MapObject)data[1];
            var primary = (bool)data[2];
            var hasAugmentResurrection = (bool)data[3];

            if (ob?.Node == null || !ob.Dead) return;

            if (SEnvir.Random.Next(100) > 25 + Magic.Level * 25) return;

            int power = Magic.GetPower();

            ob.Dead = false;
            ob.SetHP(ob.Stats[Stat.Health] * power / 100);
            ob.SetMP(ob.Stats[Stat.Mana] * power / 100);

            Player.Broadcast(new S.ObjectRevive { ObjectID = ob.ObjectID, Location = ob.CurrentLocation, Effect = false });
            Player.LevelMagic(Magic);

            Magic.Cooldown = SEnvir.Now.AddSeconds(20);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = 20000 });

            var augmentResurrection = GetAugmentedSkill(MagicType.AugmentResurrection);

            if (!primary && hasAugmentResurrection && augmentResurrection != null)
            {
                Player.LevelMagic(augmentResurrection);
            }
        }
    }
}

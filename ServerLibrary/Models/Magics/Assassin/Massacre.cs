using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;

namespace Server.Models.Magics
{
    [MagicType(MagicType.Massacre)]
    public class Massacre : MagicObject
    {
        protected override Element Element => Element.None;
        public override bool HasMassacre => true;

        public Massacre(PlayerObject player, UserMagic magic) : base(player, magic)
        {
            //Custom Skill
        }

        public override void AttackCompletePassive(MapObject target, List<MagicType> types)
        {
            if (target?.Node != null && target.Dead && target.Race == ObjectType.Monster && target.CurrentHP < 0)
            {
                if (Player.Level >= Magic.Info.NeedLevel1)
                {
                    types.Add(MagicType.Massacre);

                    var power = Math.Abs(target.CurrentHP) * Magic.GetPower() / 100;

                    foreach (MapObject t in Player.GetTargets(CurrentMap, target.CurrentLocation, 1))
                    {
                        if (t.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)t;

                        if (mob.MonsterInfo.IsBoss) continue;

                        var delay = SEnvir.Now.AddMilliseconds(600);

                        ActionList.Add(new DelayedAction(delay, ActionType.DelayAttack, t, types, false, power));
                    }
                }
            }
        }
    }
}

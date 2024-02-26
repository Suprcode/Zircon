using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Magics
{
    [MagicType(MagicType.DanceOfSwallow)]
    public class DanceOfSwallow : MagicObject
    {
        protected override Element Element => Element.None;

        public DanceOfSwallow(PlayerObject player, UserMagic magic) : base(player, magic)
        {

        }

        public override MagicCast MagicCast(MapObject target, Point location, MirDirection direction)
        {
            var response = new MagicCast
            {
                Ob = null,
                Return = true
            };

            if (!Player.CanAttackTarget(target))
            {
                Player.Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return response;
            }

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);
            Cell cell = null;
            for (int i = 0; i < 8; i++)
            {
                cell = CurrentMap.GetCell(Functions.Move(target.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                if (cell == null || cell.IsBlocking(Player, false) || cell.Movements != null)
                {
                    cell = null;
                    continue;
                }
                break;
            }

            if (cell == null)
            {
                Player.Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return response;
            }

            Player.PreventSpellCheck = true;
            Player.CurrentCell = cell;
            Player.PreventSpellCheck = false;

            Player.Direction = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);
            Player.Broadcast(new S.ObjectTurn { ObjectID = Player.ObjectID, Direction = Direction, Location = CurrentLocation });

            Player.BuffRemove(BuffType.Transparency);
            Player.BuffRemove(BuffType.Cloak);

            Player.CombatTime = SEnvir.Now;

            if (Player.Stats[Stat.Comfort] < 15)
                Player.RegenTime = SEnvir.Now + Player.RegenDelay;
            Player.ActionTime = SEnvir.Now + Globals.AttackTime;

            int aspeed = Player.Stats[Stat.AttackSpeed];
            int attackDelay = Globals.AttackDelay - aspeed * Globals.ASpeedRate;
            attackDelay = Math.Max(800, attackDelay);
            Player.AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);


            Player.Broadcast(new S.ObjectAttack { ObjectID = Player.ObjectID, Direction = Direction, Location = CurrentLocation, AttackMagic = Magic.Info.Magic });

            var delay = SEnvir.Now.AddMilliseconds(400);

            ActionList.Add(new DelayedAction(delay, ActionType.DelayAttack, target, new List<MagicType> { Type }, true, 0));


            int magicDelay = Magic.Info.Delay;
            if (SEnvir.Now <= Player.PvPTime.AddSeconds(30))
                magicDelay *= 10;

            Magic.Cooldown = SEnvir.Now.AddMilliseconds(magicDelay);
            Player.Enqueue(new S.MagicCooldown { InfoIndex = Magic.Info.Index, Delay = magicDelay });

            Player.ChangeMP(-Magic.Cost);

            return response;
        }

        public override int ModifyPowerAdditionner(bool primary, int power, MapObject ob, Stats stats = null, int extra = 0)
        {
            power += Player.GetDC();
            ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.DanceOfSwallow });

            return power;
        }

        public override void AttackComplete(MapObject target)
        {
            if (target.Level < Player.Level)
            {
                target.ApplyPoison(new Poison
                {
                    Owner = Player,
                    Type = PoisonType.Silenced,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(Magic.GetPower() + 1)
                });

                target.ApplyPoison(new Poison
                {
                    Owner = Player,
                    Type = PoisonType.Paralysis,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(1)
                });
            }

            Player.LevelMagic(Magic);
        }
    }
}

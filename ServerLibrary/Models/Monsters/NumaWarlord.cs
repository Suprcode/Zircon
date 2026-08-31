using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NumaWarlord : MonsterObject
    {
        private const int SwingRange = 2;
        private const int PullRange = 5;
        private const int PullDistance = 3;

        private static readonly TimeSpan TeleportCooldown = TimeSpan.FromSeconds(6);
        private static readonly TimeSpan PullCooldown = TimeSpan.FromSeconds(10);

        private DateTime TeleportTime;
        private DateTime PullTime;

        private enum WarlordAction
        {
            Teleport,
            Pull,
        }

        protected override bool InAttackRange()
        {
            return Target?.CurrentMap == CurrentMap &&
                   Target.CurrentLocation != CurrentLocation &&
                   Functions.InRange(CurrentLocation, Target.CurrentLocation, SwingRange);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            int distance = Functions.Distance(CurrentLocation, Target.CurrentLocation);

            if (CanAttack && distance > 4 && SEnvir.Now >= TeleportTime && CanTeleportBeside(Target))
            {
                TeleportAttack();
                return;
            }

            if (CanAttack && distance > SwingRange && distance <= PullRange && SEnvir.Now >= PullTime && PullAttack())
                return;

            if (!InAttackRange())
            {
                if (CurrentLocation == Target.CurrentLocation)
                {
                    MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                    MoveTo(Target.CurrentLocation);

                return;
            }

            if (CanAttack)
                SwingAttack();
        }

        public override void ProcessAction(DelayedAction action)
        {
            if (action.Type != ActionType.Function || action.Data.Length == 0 || action.Data[0] is not WarlordAction warlordAction)
            {
                base.ProcessAction(action);
                return;
            }

            switch (warlordAction)
            {
                case WarlordAction.Teleport:
                    CompleteTeleport((MapObject)action.Data[1]);
                    break;
                case WarlordAction.Pull:
                    CompletePull((List<MapObject>)action.Data[1]);
                    break;
            }
        }

        private void TeleportAttack()
        {
            MapObject target = Target;

            Direction = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);
            Broadcast(new S.ObjectAttack
            {
                ObjectID = ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                TargetID = target.ObjectID,
            });

            UpdateAttackTime();
            TeleportTime = SEnvir.Now + TeleportCooldown;

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.Function,
                WarlordAction.Teleport,
                target));
        }

        private void CompleteTeleport(MapObject target)
        {
            if (Dead) return;
            if (!CanAttackTarget(target) || target.CurrentMap != CurrentMap) return;
            if (!TryGetTeleportCell(target, out Cell cell)) return;

            Direction = Functions.DirectionFromPoint(cell.Location, target.CurrentLocation);
            Teleport(CurrentMap, cell.Location, false, false);
        }

        private bool CanTeleportBeside(MapObject target)
        {
            return TryGetTeleportCell(target, out _);
        }

        private bool TryGetTeleportCell(MapObject target, out Cell result)
        {
            result = null;

            if (target?.Node == null || target.Dead || target.CurrentMap != CurrentMap) return false;

            MirDirection direction = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);
            int start = SEnvir.Random.Next(8);
            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

            for (int i = 0; i < 8; i++)
            {
                MirDirection candidateDirection = Functions.ShiftDirection(direction, (start + i * rotation) % 8);
                Cell cell = CurrentMap.GetCell(Functions.Move(target.CurrentLocation, candidateDirection));

                if (cell == null || cell.Movements != null) continue;

                result = cell;
                return true;
            }

            return false;
        }

        private void SwingAttack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Point centre = Functions.Move(CurrentLocation, Direction);
            List<MapObject> targets = GetTargets(CurrentMap, centre, 1);

            Broadcast(new S.ObjectRangeAttack
            {
                ObjectID = ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Targets = targets.Select(x => x.ObjectID).ToList(),
            });

            UpdateAttackTime();

            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(400),
                    ActionType.DelayAttack,
                    target,
                    GetDC(),
                    AttackElement));
            }
        }

        private bool PullAttack()
        {
            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, PullRange);

            if (targets.Count == 0) return false;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectMagic
            {
                ObjectID = ObjectID,
                Direction = Direction,
                CurrentLocation = CurrentLocation,
                Type = MagicType.None,
                Cast = true,
                Targets = targets.Select(x => x.ObjectID).ToList(),
            });

            UpdateAttackTime();
            PullTime = SEnvir.Now + PullCooldown;

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(400),
                ActionType.Function,
                WarlordAction.Pull,
                targets));

            return true;
        }

        private void CompletePull(List<MapObject> targets)
        {
            if (Dead) return;

            foreach (MapObject target in targets)
            {
                if (!CanAttackTarget(target) || target.CurrentMap != CurrentMap) continue;
                if (!Functions.InRange(CurrentLocation, target.CurrentLocation, PullRange)) continue;

                int power = Math.Max(1, GetDC() / 2);
                Attack(target, power, AttackElement);

                if (target.Dead || target.CurrentMap != CurrentMap) continue;

                int distance = Functions.Distance(target.CurrentLocation, CurrentLocation);
                int pullDistance = Math.Min(PullDistance, distance - 1);

                if (pullDistance <= 0) continue;

                MirDirection pullDirection = Functions.DirectionFromPoint(target.CurrentLocation, CurrentLocation);
                target.Pushed(pullDirection, pullDistance);
            }
        }
    }
}

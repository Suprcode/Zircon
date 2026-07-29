using Client.Envir;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;

namespace Client.Scenes
{
    public sealed partial class GameScene
    {
        private const int AutoPathProgressSearchRange = 12;

        public List<AutoPathRoute> AutoPathRoutes = new List<AutoPathRoute>();
        public AutoPathRoute AutoPathRoute;
        public int AutoPathRouteProgressMapIndex { get; private set; } = -1;
        public int AutoPathRouteProgressIndex { get; private set; } = -1;

        private bool _AutoPathCancelPending;
        private AutoPathMovement _AutoPathMove;

        private sealed class AutoPathMovement
        {
            public MirDirection Direction;
            public Point Location;
            public int Distance;
            public TimeSpan Slow;
        }

        public void SetAutoPathRoutes(List<AutoPathRoute> routes)
        {
            if (_AutoPathCancelPending)
            {
                if (routes?.Count > 0) return;

                _AutoPathCancelPending = false;
            }

            ApplyAutoPathRoutes(routes);
        }

        private void ApplyAutoPathRoutes(List<AutoPathRoute> routes)
        {
            AutoPathRoute route = routes?.FirstOrDefault();
            if (route != null)
                AutoRun = false;

            AutoPathRoutes = routes ?? new List<AutoPathRoute>();
            AutoPathRoute = route;
            AutoPathRouteProgressMapIndex = -1;
            AutoPathRouteProgressIndex = -1;
            if (route == null)
                _AutoPathMove = null;

            UpdateAutoPathProgress(false);
            BigMapBox?.RefreshAutoPathRoute();
            MiniMapBox?.RefreshAutoPathRoute();
        }

        public void UpdateAutoPathProgress(bool updateRouteControls = true)
        {
            if (AutoPathRoute == null || User == null || MapControl?.MapInfo == null)
                return;

            int mapIndex = MapControl.MapInfo.Index;
            AutoPathRouteLeg leg = AutoPathRoute.Legs.FirstOrDefault(x => x.MapIndex == mapIndex);

            if (leg?.Points == null || leg.Points.Count == 0) return;

            bool changed = AutoPathRouteProgressMapIndex != mapIndex;
            if (changed)
            {
                AutoPathRouteProgressMapIndex = mapIndex;
                AutoPathRouteProgressIndex = -1;
            }

            int startIndex = Math.Max(0, AutoPathRouteProgressIndex);
            int endIndex = Math.Min(leg.Points.Count - 1, startIndex + AutoPathProgressSearchRange);
            int closestIndex = startIndex;
            int closestDistance = int.MaxValue;

            for (int i = startIndex; i <= endIndex; i++)
            {
                int distance = Functions.Distance(User.CurrentLocation, leg.Points[i]);
                if (distance > closestDistance) continue;

                closestIndex = i;
                closestDistance = distance;
            }

            if (closestIndex > AutoPathRouteProgressIndex)
            {
                AutoPathRouteProgressIndex = closestIndex;
                changed = true;
            }

            if (!changed || !updateRouteControls) return;

            BigMapBox?.UpdateAutoPathRouteProgress();
            MiniMapBox?.UpdateAutoPathRouteProgress();
        }

        public void CancelAutoPath()
        {
            if (AutoPathRoute == null) return;

            _AutoPathCancelPending = true;
            ApplyAutoPathRoutes(null);
            CEnvir.Enqueue(new C.AutoPathCancel());
        }

        public bool TryQueueAutoPathMove(MirDirection direction, Point location, int distance, TimeSpan slow, bool mapChanged)
        {
            if ((AutoPathRoute == null && !_AutoPathCancelPending) || User == null) return false;

            if (mapChanged)
            {
                _AutoPathMove = null;
                User.ApplyAutoPathMapTransition(direction, location, slow);
                return true;
            }

            _AutoPathMove = new AutoPathMovement { Direction = direction, Location = location, Distance = distance, Slow = slow };
            return true;
        }

        private void ProcessAutoPathMove()
        {
            if (_AutoPathMove == null || !User.TryStartAutoPathMove(_AutoPathMove.Direction, _AutoPathMove.Location, _AutoPathMove.Distance, _AutoPathMove.Slow)) return;

            _AutoPathMove = null;
        }

        private void ClearAutoPath()
        {
            AutoPathRoute = null;
            AutoPathRoutes.Clear();
            AutoPathRouteProgressMapIndex = -1;
            AutoPathRouteProgressIndex = -1;
            _AutoPathMove = null;
        }

    }
}

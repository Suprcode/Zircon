using Library;
using Library.SystemModels;
using Server.Models.AutoPath;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models
{
    public partial class MonsterObject
    {
        private MonsterAutoPathState _AutoPath;

        public bool AutoPathing => _AutoPath != null;

        public bool StartAutoPath(Point destination, int arrivalDistance = 0, bool avoidLiveObjects = true, bool cancelOnTarget = true)
        {
            if (CurrentMap?.GetCell(destination) == null || MoveDelay <= 0) return false;
            if (!AutoPathService.Instance.TryBuildCurrentPath(this, destination, avoidLiveObjects, out List<Point> path)) return false;

            CancelAutoPath();

            _AutoPath = new MonsterAutoPathState
            {
                Map = CurrentMap,
                SourceMap = CurrentMap.Info,
                Source = CurrentLocation,
                Destination = destination,
                ArrivalDistance = arrivalDistance,
                AvoidLiveObjects = avoidLiveObjects,
                CancelOnTarget = cancelOnTarget,
                Path = path,
                PathIndex = 1,
            };

            AutoPathLogger.Started(this, _AutoPath.SourceMap, _AutoPath.Source, _AutoPath.Map.Info, _AutoPath.Destination);
            Activate();
            return true;
        }

        public void CancelAutoPath()
        {
            if (_AutoPath == null) return;

            AutoPathLogger.Ended(this, _AutoPath.SourceMap, _AutoPath.Source, _AutoPath.Map?.Info, _AutoPath.Destination);
            _AutoPath = null;
        }

        protected bool ProcessAutoPath()
        {
            MonsterAutoPathState state = _AutoPath;
            if (state == null) return false;

            if (Dead || CurrentMap != state.Map || state.CancelOnTarget && Target != null ||
                Functions.InRange(CurrentLocation, state.Destination, state.ArrivalDistance))
            {
                CancelAutoPath();
                return false;
            }

            if (!CanMove) return true;

            while (state.PathIndex < state.Path.Count && CurrentLocation == state.Path[state.PathIndex])
                state.PathIndex++;

            if (state.PathIndex >= state.Path.Count || !Functions.InRange(CurrentLocation, state.Path[state.PathIndex], 1))
            {
                if (!RebuildAutoPath(state))
                {
                    CancelAutoPath();
                    return false;
                }
            }

            if (state.PathIndex < state.Path.Count)
            {
                Point next = state.Path[state.PathIndex];
                Cell cell = CurrentMap.GetCell(next);

                if (state.AvoidLiveObjects && cell?.IsBlocking(this, false) == true)
                {
                    if (!RebuildAutoPath(state))
                    {
                        CancelAutoPath();
                        return false;
                    }

                    if (state.PathIndex >= state.Path.Count) return true;
                    next = state.Path[state.PathIndex];
                }

                MoveTo(next);
            }

            return true;
        }

        private bool RebuildAutoPath(MonsterAutoPathState state)
        {
            if (!AutoPathService.Instance.TryBuildCurrentPath(this, state.Destination, state.AvoidLiveObjects, out List<Point> path))
                return false;

            state.Path = path;
            state.PathIndex = 1;
            return true;
        }

        private sealed class MonsterAutoPathState
        {
            public Map Map;
            public MapInfo SourceMap;
            public Point Source;
            public Point Destination;
            public int ArrivalDistance;
            public bool AvoidLiveObjects;
            public bool CancelOnTarget;
            public List<Point> Path;
            public int PathIndex;
        }
    }
}

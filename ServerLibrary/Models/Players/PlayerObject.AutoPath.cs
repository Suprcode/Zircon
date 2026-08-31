using Library;
using Library.SystemModels;
using Server.Models.AutoPath;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models
{
    public partial class PlayerObject
    {
        internal AutoPathState AutoPath;

        public bool StartAutoPath(MapInfo map, Point point)
            => AutoPathService.Instance.TryStart(this, map, point);

        public bool StartAutoPath(MapRegion region)
            => AutoPathService.Instance.TryStart(this, region);

        public void CancelAutoPath()
            => AutoPathService.Instance.Cancel(this);
    }

    internal sealed class AutoPathState
    {
        public readonly List<AutoPathDestination> Destinations = new List<AutoPathDestination>();
        public readonly List<AutoPathRoute> Routes = new List<AutoPathRoute>();
        public int NextWaypointNumber = 1;

        public AutoPathDestination Destination => Destinations.Count == 0 ? null : Destinations[0];

        public AutoPathRoute Route;
        public Map RouteMap;
        public List<Point> Path;

        public bool AutoPathLogged;
        public MapInfo AutoPathSourceMap;
        public Point AutoPathSource;
        public MapInfo AutoPathDestinationMap;
        public Point AutoPathDestination;

        public bool FullStrideAvailable;
        public bool LiveObjectRepathRequired;
        public bool PathAvoidsLiveObjects;
        public bool MountRequested;
        public bool PathEndsAtMapTransition;
        public bool WaitingForMovementStart;
    }

}

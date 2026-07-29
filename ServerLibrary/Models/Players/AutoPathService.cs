using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Players
{
    public sealed class AutoPathService
    {
        public static AutoPathService Instance { get; } = new AutoPathService();

        private const int PathAdvanceSearchDistance = 4;

        private AutoPathService()
        {
        }

        public bool TryStart(PlayerObject player, MapInfo map, Point point)
        {
            if (player == null || map == null) return false;
            if (!Config.AllowWaypoints) return false;

            return Start(player, new AutoPathDestination
            {
                Map = map,
                Point = point,
            });
        }

        public bool TryStart(PlayerObject player, MapRegion region)
        {
            if (player == null || region?.Map == null) return false;
            if (!Config.AllowWaypoints) return false;

            return Start(player, new AutoPathDestination
            {
                Map = region.Map,
                Region = region,
            });
        }

        public bool TryStart(PlayerObject player, NPCInfo npc)
        {
            if (player == null || npc?.Region?.Map == null) return false;

            return Start(player, new AutoPathDestination
            {
                Map = npc.Region.Map,
                Region = npc.Region,
                NPC = npc,
            });
        }

        public bool TryAddWaypoint(PlayerObject player, MapInfo map, Point point)
        {
            if (player == null || map == null) return false;

            if (!Config.AllowWaypoints) return false;

            int maximum = Math.Max(0, Config.MaxWaypoints);
            if (maximum == 0 || player.AutoPath?.Destinations.Count >= maximum)
            {
                player.Connection.ReceiveChat(player.Connection.Language.AutoPathWaypointLimit, MessageType.System);
                return false;
            }

            AutoPathDestination destination = new AutoPathDestination
            {
                Map = map,
                Point = point,
            };

            if (player.AutoPath == null)
                return Start(player, destination);

            AutoPathState state = player.AutoPath;
            AutoPathRoute previous = state.Routes.LastOrDefault();
            MapInfo startMap = previous == null
                ? player.CurrentMap?.Info
                : SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == previous.DestinationMapIndex);
            Point start = previous?.Destination ?? player.CurrentLocation;

            if (!AutoPathRoutePlanner.TryBuild(player, startMap, start, destination, false, out AutoPathPlan plan))
            {
                player.Connection.ReceiveChat(player.Connection.Language.AutoPathNoRoute, MessageType.System);
                return false;
            }

            plan.Route.WaypointNumber = state.NextWaypointNumber++;
            state.Destinations.Add(destination);
            state.Routes.Add(plan.Route);
            SendRoutes(player, state);
            return true;
        }

        public bool TryBuildRoute(MapObject actor, MapInfo map, Point point, out AutoPathRoute route)
        {
            route = null;
            if (actor?.CurrentMap == null || map == null) return false;

            AutoPathDestination destination = new AutoPathDestination
            {
                Map = map,
                Point = point,
            };

            return TryBuildRoute(actor, destination, out route);
        }

        public bool TryBuildRoute(MapObject actor, MapRegion region, out AutoPathRoute route)
        {
            route = null;
            if (actor?.CurrentMap == null || region?.Map == null) return false;

            AutoPathDestination destination = new AutoPathDestination
            {
                Map = region.Map,
                Region = region,
            };

            return TryBuildRoute(actor, destination, out route);
        }

        private static bool TryBuildRoute(MapObject actor, AutoPathDestination destination, out AutoPathRoute route)
        {
            route = null;
            if (!AutoPathRoutePlanner.TryBuild(actor, destination, false, out AutoPathPlan plan))
                return false;

            route = plan.Route;
            return true;
        }

        public void Cancel(PlayerObject player, bool notifyClient = true)
        {
            if (player == null) return;

            player.AutoPath = null;

            if (notifyClient)
                player.Enqueue(new S.AutoPathChanged { Routes = new List<AutoPathRoute>() });
        }

        public void MoveStarted(PlayerObject player)
        {
            if (player?.AutoPath == null) return;

            player.AutoPath.WaitingForMovementStart = false;
        }

        public void Process(PlayerObject player)
        {
            AutoPathState state = player?.AutoPath;
            if (state == null) return;

            if (!Config.AllowWaypoints && state.Destination?.NPC == null)
            {
                Cancel(player);
                return;
            }

            if (player.Dead || player.CurrentMap == null || !player.CurrentMap.Info.CanAutoPath)
            {
                Fail(player);
                return;
            }

            if (state.WaitingForMovementStart) return;

            if (ReachedDestination(player, state.Destination))
            {
                if (!AdvanceWaypoint(player, state))
                    Cancel(player);
                return;
            }

            if (player.PacketWaiting) return;

            bool avoidLiveObjects = state.LiveObjectRepathRequired;
            if (state.LiveObjectRepathRequired ||
                state.RouteMap != player.CurrentMap ||
                state.Path == null ||
                state.Path.Count == 0 ||
                !AdvancePath(state.Path, player.CurrentLocation))
            {
                if (!RebuildCurrentPath(player, state, avoidLiveObjects) && !Rebuild(player, state, avoidLiveObjects))
                {
                    Fail(player);
                    return;
                }
            }

            if (ShouldMount(player))
            {
                if (!state.MountRequested)
                {
                    state.MountRequested = true;
                    player.Mount();
                }

                return;
            }

            if (!player.CanMove) return;

            if (state.Path.Count < 2)
            {
                Fail(player);
                return;
            }

            if (!TryGetExecutionStep(player, state, out MirDirection direction, out int distance))
            {
                if ((!RebuildCurrentPath(player, state, true) && !Rebuild(player, state, true)) ||
                    !TryGetExecutionStep(player, state, out direction, out distance))
                {
                    Fail(player);
                    return;
                }
            }

            Point destination = Functions.Move(player.CurrentLocation, direction, distance);
            if (!HasClearTravel(player, player.CurrentMap, player.CurrentLocation, destination, true))
            {
                RequireLiveObjectRepath(state);
                return;
            }

            player.Move(direction, distance);
            state.FullStrideAvailable = true;
            state.WaitingForMovementStart = true;
        }

        private static bool ShouldMount(PlayerObject player)
        {
            return player.Character.Account.Horse != HorseType.None &&
                   player.Horse == HorseType.None &&
                   player.CurrentMap.Info.CanHorse &&
                   !player.Dead;
        }

        private static void RequireLiveObjectRepath(AutoPathState state)
        {
            state.LiveObjectRepathRequired = true;
        }

        private static bool AdvancePath(List<Point> path, Point location)
        {
            if (path == null || path.Count == 0) return false;

            int searchCount = Math.Min(path.Count, PathAdvanceSearchDistance + 1);

            for (int i = 0; i < searchCount; i++)
            {
                if (path[i] != location) continue;

                if (i > 0)
                    path.RemoveRange(0, i);

                return true;
            }

            return false;
        }

        private static bool TryGetExecutionStep(PlayerObject player, AutoPathState state, out MirDirection direction, out int distance)
        {
            direction = MirDirection.Up;
            distance = 0;

            if (state.Path == null || state.Path.Count < 2) return false;

            int maximumDistance = GetMaximumDistance(player, state);
            int maximumAnchor = Math.Min(state.Path.Count - 1, maximumDistance);

            for (int i = maximumAnchor; i > 0; i--)
            {
                Point anchor = state.Path[i];
                if (!Functions.IsStraightEightDirection(player.CurrentLocation, anchor))
                    continue;

                if (!HasClearTravel(player, player.CurrentMap, player.CurrentLocation, anchor, state.PathAvoidsLiveObjects))
                    continue;

                direction = Functions.DirectionFromPoint(player.CurrentLocation, anchor);

                int xDistance = Math.Abs(anchor.X - player.CurrentLocation.X);
                int yDistance = Math.Abs(anchor.Y - player.CurrentLocation.Y);
                distance = Math.Max(xDistance, yDistance);

                if (IsEnteringMapTransition(player, state, direction, distance))
                    distance = 1;

                return distance > 0;
            }

            return false;
        }

        private static int GetMaximumDistance(PlayerObject player, AutoPathState state)
        {
            if (!state.FullStrideAvailable) return 1;

            return player.Horse == HorseType.None ? 2 : 3;
        }

        private static bool IsEnteringMapTransition(PlayerObject player, AutoPathState state, MirDirection direction, int distance)
        {
            return state.PathEndsAtMapTransition && Functions.Move(player.CurrentLocation, direction, distance) == state.Path[state.Path.Count - 1];
        }

        private static bool HasClearTravel(MapObject actor, Map map, Point start, Point destination, bool avoidLiveObjects)
        {
            Point current = start;

            while (current != destination)
            {
                MirDirection direction = Functions.DirectionFromPoint(current, destination);
                current = Functions.Move(current, direction);

                Cell cell = map.GetCell(current);
                if (cell == null || avoidLiveObjects && cell.IsBlocking(actor, false)) return false;
            }

            return true;
        }

        private bool Start(PlayerObject player, AutoPathDestination destination)
        {
            AutoPathState state = player.AutoPath;
            if (state == null)
            {
                state = new AutoPathState();
                player.AutoPath = state;
            }

            state.Destinations.Clear();
            state.Routes.Clear();
            state.NextWaypointNumber = 1;
            state.Destinations.Add(destination);
            state.Route = null;
            state.RouteMap = null;
            state.Path = null;
            state.LiveObjectRepathRequired = false;
            state.PathAvoidsLiveObjects = false;
            state.MountRequested = false;
            state.WaitingForMovementStart = false;
            state.FullStrideAvailable = false;

            if (Rebuild(player, state))
                return true;

            Fail(player);
            return false;
        }

        private bool Rebuild(PlayerObject player, AutoPathState state, bool avoidLiveObjects = false)
        {
            if (state?.Destination == null) return false;

            if (!AutoPathRoutePlanner.TryBuild(player, state.Destination, avoidLiveObjects, out AutoPathPlan plan))
                return false;

            bool mapChanged = state.RouteMap != null && state.RouteMap != player.CurrentMap;

            state.Route = plan.Route;
            if (state.Routes.Count == 0)
            {
                plan.Route.WaypointNumber = state.NextWaypointNumber++;
                state.Routes.Add(plan.Route);
            }
            else
            {
                plan.Route.WaypointNumber = state.Routes[0].WaypointNumber;
                state.Routes[0] = plan.Route;
            }
            state.RouteMap = player.CurrentMap;
            state.Path = plan.CurrentPath;
            state.PathEndsAtMapTransition = plan.ChangesMap;
            state.LiveObjectRepathRequired = false;
            state.PathAvoidsLiveObjects = avoidLiveObjects;

            if (mapChanged)
            {
                state.FullStrideAvailable = false;
                state.MountRequested = false;
            }

            SendRoutes(player, state);
            return true;
        }

        private bool RebuildCurrentPath(PlayerObject player, AutoPathState state, bool avoidLiveObjects)
        {
            AutoPathRouteLeg leg = state.Route?.Legs?.FirstOrDefault(x => x.MapIndex == player.CurrentMap.Info.Index);
            if (leg?.Points == null || leg.Points.Count == 0) return false;

            Point destination = leg.Points[leg.Points.Count - 1];
            if (!AutoPathRoutePlanner.TryBuildCurrentPath(player, destination, avoidLiveObjects, out List<Point> path))
                return false;

            bool mapChanged = state.RouteMap != player.CurrentMap;

            leg.Points = path;
            state.RouteMap = player.CurrentMap;
            state.Path = path;
            state.PathEndsAtMapTransition = player.CurrentMap.Info.Index != state.Route.DestinationMapIndex;
            state.LiveObjectRepathRequired = false;
            state.PathAvoidsLiveObjects = avoidLiveObjects;

            if (mapChanged)
            {
                state.FullStrideAvailable = false;
                state.MountRequested = false;
            }

            SendRoutes(player, state);
            return true;
        }

        private bool AdvanceWaypoint(PlayerObject player, AutoPathState state)
        {
            if (state.Destinations.Count == 0) return false;

            state.Destinations.RemoveAt(0);
            if (state.Routes.Count > 0)
                state.Routes.RemoveAt(0);

            if (state.Destinations.Count == 0) return false;

            state.Route = null;
            state.RouteMap = null;
            state.Path = null;
            state.FullStrideAvailable = false;
            state.LiveObjectRepathRequired = false;
            state.PathAvoidsLiveObjects = false;
            state.MountRequested = false;
            state.PathEndsAtMapTransition = false;
            state.WaitingForMovementStart = false;

            return Rebuild(player, state);
        }

        private static void SendRoutes(PlayerObject player, AutoPathState state)
        {
            player.Enqueue(new S.AutoPathChanged
            {
                Routes = state.Routes.ToList(),
            });
        }

        private static bool ReachedDestination(PlayerObject player, AutoPathDestination destination)
        {
            if (player.CurrentMap.Info != destination.Map) return false;

            if (destination.NPC != null)
            {
                NPCObject npc = player.CurrentMap.NPCs.FirstOrDefault(x => x.NPCInfo == destination.NPC && x.Visible);

                if (npc != null)
                    return Functions.InRange(player.CurrentLocation, npc.CurrentLocation, 1);

                return destination.Region?.PointList?.Any(x => Functions.InRange(player.CurrentLocation, x, 1)) == true;
            }

            if (destination.Region != null)
                return destination.Region.PointList?.Contains(player.CurrentLocation) == true;

            return player.CurrentLocation == destination.Point;
        }

        private void Fail(PlayerObject player)
        {
            Cancel(player);
            player.Connection.ReceiveChat(player.Connection.Language.AutoPathNoRoute, MessageType.System);
        }
    }
}

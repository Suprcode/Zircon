using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Server.Models.Players
{
    internal sealed class AutoPathPlan
    {
        public AutoPathRoute Route;
        public List<Point> CurrentPath;
        public bool ChangesMap;
    }

    internal static class AutoPathRoutePlanner
    {
        private const int PathGuideDistance = 48;
        private const int NormalPathCost = 8;

        private static readonly MirDirection[] Directions =
        {
            MirDirection.Up, MirDirection.UpRight, MirDirection.Right, MirDirection.DownRight,
            MirDirection.Down, MirDirection.DownLeft, MirDirection.Left, MirDirection.UpLeft
        };

        public static bool TryBuild(MapObject actor, AutoPathDestination destination, bool avoidLiveObjects, out AutoPathPlan plan)
            => TryBuild(actor, actor?.CurrentMap?.Info, actor?.CurrentLocation ?? Point.Empty, destination, avoidLiveObjects, out plan);

        public static bool TryBuild(MapObject actor, MapInfo startMap, Point startLocation, AutoPathDestination destination, bool avoidLiveObjects, out AutoPathPlan plan)
        {
            plan = null;
            if (actor?.CurrentMap == null ||
                startMap == null ||
                destination?.Map == null ||
                !startMap.CanAutoPath ||
                !destination.Map.CanAutoPath)
                return false;

            Dictionary<MapInfo, int> mapDistances = GetMapDistances(actor, destination.Map);
            if (!mapDistances.ContainsKey(startMap)) return false;

            SearchNode start = new SearchNode
            {
                Map = startMap,
                Location = startLocation,
            };

            PriorityQueue<SearchNode, int> open = new PriorityQueue<SearchNode, int>();
            open.Enqueue(start, 0);

            Dictionary<(MapInfo Map, Point Location), int> bestCosts =
                new Dictionary<(MapInfo Map, Point Location), int>
                {
                    [(start.Map, start.Location)] = 0
                };

            AutoPathPlan bestPlan = null;
            int bestRouteCost = int.MaxValue;

            while (open.TryDequeue(out SearchNode node, out _))
            {
                var nodeKey = (node.Map, node.Location);
                if (!bestCosts.TryGetValue(nodeKey, out int knownCost) || node.Cost != knownCost)
                    continue;
                if (node.Cost >= bestRouteCost) continue;

                Map map = SEnvir.GetMap(node.Map);
                if (map == null) continue;

                bool avoidObjectsOnMap = avoidLiveObjects && node.Map == actor.CurrentMap.Info;

                if (node.Map == destination.Map)
                {
                    NPCObject destinationNPC = GetDestinationNPC(actor, map, destination);
                    HashSet<Point> targets = GetDestinationTargets(map, destination, destinationNPC);
                    if (TryFindPreferredPath(actor, map, node.Location, targets,
                            avoidObjectsOnMap, out List<Point> finalPath))
                    {
                        int finalCost = node.Cost + Math.Max(0, finalPath.Count - 1);
                        if (finalCost < bestRouteCost)
                        {
                            List<AutoPathRouteLeg> legs = ReconstructLegs(node);
                            legs.Add(new AutoPathRouteLeg
                            {
                                MapIndex = node.Map.Index,
                                Points = finalPath
                            });
                            legs = MergeLegs(legs);

                            bestRouteCost = finalCost;
                            bestPlan = new AutoPathPlan
                            {
                                Route = new AutoPathRoute
                                {
                                    Legs = legs,
                                    DestinationMapIndex = node.Map.Index,
                                    Destination = finalPath[finalPath.Count - 1],
                                    DisplayDestination = GetDisplayDestination(destination, map, destinationNPC, finalPath[finalPath.Count - 1]),
                                },
                                CurrentPath = legs[0].Points,
                                ChangesMap = node.Previous != null,
                            };
                        }
                    }
                }

                if (!mapDistances.TryGetValue(node.Map, out int currentMapDistance))
                    continue;

                foreach (MovementInfo movement in GetOutgoingMovements(node.Map))
                {
                    if (!CanUseMovement(actor, movement, node.Map)) continue;

                    MapInfo destinationMap = movement.DestinationRegion?.Map;
                    if (destinationMap == null ||
                        !mapDistances.TryGetValue(destinationMap, out int destinationMapDistance) ||
                        destinationMapDistance != currentMapDistance - 1)
                        continue;

                    HashSet<Point> sourcePoints = GetValidPoints(
                        map, GetRegionPoints(movement.SourceRegion, map));
                    if (!TryFindPreferredPath(actor, map, node.Location, sourcePoints,
                            avoidObjectsOnMap, out List<Point> legPath))
                        continue;

                    if (movement.NeedHole && (node.Map != actor.CurrentMap.Info ||
                        !actor.CurrentMap.Objects.OfType<SpellObject>().Any(x =>
                            x.Effect == SpellEffect.ZombieHole &&
                            x.CurrentLocation == legPath[legPath.Count - 1])))
                        continue;

                    Map destinationMapInstance = SEnvir.GetMap(destinationMap);
                    if (destinationMapInstance == null) continue;

                    HashSet<Point> destinationPoints = GetValidPoints(
                        destinationMapInstance,
                        GetRegionPoints(movement.DestinationRegion, destinationMapInstance));
                    if (destinationPoints.Count == 0) continue;

                    int cost = node.Cost + Math.Max(1, legPath.Count - 1);
                    foreach (Point destinationPoint in destinationPoints)
                    {
                        var key = (destinationMap, destinationPoint);
                        if (bestCosts.TryGetValue(key, out int oldCost) && oldCost <= cost)
                            continue;

                        bestCosts[key] = cost;

                        SearchNode next = new SearchNode
                        {
                            Map = destinationMap,
                            Location = destinationPoint,
                            Cost = cost,
                            Previous = node,
                            IncomingLeg = new AutoPathRouteLeg
                            {
                                MapIndex = node.Map.Index,
                                Points = legPath
                            },
                        };

                        open.Enqueue(next, cost);
                    }
                }
            }

            plan = bestPlan;
            return plan != null;
        }

        public static bool TryBuildCurrentPath(MapObject actor, Point destination, bool avoidLiveObjects, out List<Point> path)
        {
            path = null;
            if (actor?.CurrentMap?.GetCell(destination) == null) return false;

            return TryFindPreferredPath(actor, actor.CurrentMap, actor.CurrentLocation, new HashSet<Point> { destination }, avoidLiveObjects, out path);
        }

        private static Dictionary<MapInfo, int> GetMapDistances(MapObject actor, MapInfo destination)
        {
            Dictionary<MapInfo, int> result = new Dictionary<MapInfo, int>
            {
                [destination] = 0,
            };
            Queue<MapInfo> pending = new Queue<MapInfo>();
            pending.Enqueue(destination);

            while (pending.Count > 0)
            {
                MapInfo current = pending.Dequeue();
                int distance = result[current] + 1;

                foreach (MovementInfo movement in GetIncomingMovements(current))
                {
                    MapInfo source = movement.SourceRegion?.Map;
                    if (source == null || result.ContainsKey(source) || !source.CanAutoPath ||
                        !CanUseMovement(actor, movement, source))
                        continue;

                    result[source] = distance;
                    pending.Enqueue(source);
                }
            }

            return result;
        }

        private static IEnumerable<MovementInfo> GetOutgoingMovements(MapInfo map)
        {
            if (map?.Regions == null) yield break;

            foreach (MapRegion region in map.Regions)
            {
                if (region.SourceMovements == null) continue;

                foreach (MovementInfo movement in region.SourceMovements)
                    yield return movement;
            }
        }

        private static IEnumerable<MovementInfo> GetIncomingMovements(MapInfo map)
        {
            if (map?.Regions == null) yield break;

            foreach (MapRegion region in map.Regions)
            {
                if (region.DestinationMovements == null) continue;

                foreach (MovementInfo movement in region.DestinationMovements)
                    yield return movement;
            }
        }

        private static List<AutoPathRouteLeg> ReconstructLegs(SearchNode node)
        {
            List<AutoPathRouteLeg> legs = new List<AutoPathRouteLeg>();

            while (node?.Previous != null)
            {
                legs.Add(node.IncomingLeg);
                node = node.Previous;
            }

            legs.Reverse();
            return legs;
        }

        private static List<AutoPathRouteLeg> MergeLegs(List<AutoPathRouteLeg> legs)
        {
            List<AutoPathRouteLeg> result = new List<AutoPathRouteLeg>();

            foreach (AutoPathRouteLeg leg in legs)
            {
                AutoPathRouteLeg existing = result.LastOrDefault();
                if (existing?.MapIndex != leg.MapIndex)
                {
                    result.Add(new AutoPathRouteLeg
                    {
                        MapIndex = leg.MapIndex,
                        Points = new List<Point>(leg.Points)
                    });
                    continue;
                }

                if (existing.Points.Count > 0 && leg.Points.Count > 0 &&
                    existing.Points[existing.Points.Count - 1] == leg.Points[0])
                    existing.Points.AddRange(leg.Points.Skip(1));
                else
                    existing.Points.AddRange(leg.Points);
            }

            return result;
        }

        private static bool CanUseMovement(MapObject actor, MovementInfo movement, MapInfo sourceMap)
        {
            MapInfo destination = movement.DestinationRegion?.Map;

            if (destination == null || !destination.CanAutoPath || movement.NeedInstance != null)
                return false;

            if (actor is not PlayerObject player)
                return true;

            if (destination.MinimumLevel > player.Level && !player.Character.Account.TempAdmin)
                return false;

            if (destination.MaximumLevel > 0 && destination.MaximumLevel < player.Level &&
                !player.Character.Account.TempAdmin)
                return false;

            if (movement.NeedItem != null && player.GetItemCount(movement.NeedItem) == 0)
                return false;

            if (movement.NeedSpawn != null)
            {
                SpawnInfo spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == movement.NeedSpawn);
                if (spawn?.AliveCount == 0 ||
                    (spawn == null && sourceMap == actor.CurrentMap.Info))
                    return false;
            }

            if (destination.RequiredClass == RequiredClass.None ||
                destination.RequiredClass == RequiredClass.All)
                return true;

            RequiredClass required = player.Class switch
            {
                MirClass.Warrior => RequiredClass.Warrior,
                MirClass.Wizard => RequiredClass.Wizard,
                MirClass.Taoist => RequiredClass.Taoist,
                MirClass.Assassin => RequiredClass.Assassin,
                _ => RequiredClass.None,
            };

            return (destination.RequiredClass & required) == required;
        }

        private static NPCObject GetDestinationNPC(MapObject actor, Map map, AutoPathDestination destination)
        {
            if (destination.NPC == null || actor.CurrentMap != map) return null;

            return map.NPCs.FirstOrDefault(x => x.NPCInfo == destination.NPC && x.Visible);
        }

        private static HashSet<Point> GetDestinationTargets(Map map, AutoPathDestination destination, NPCObject destinationNPC)
        {
            if (destinationNPC != null)
                return GetAdjacentPoints(map, destinationNPC.CurrentLocation);

            if (destination.Region != null)
            {
                if (destination.NPC == null)
                    return GetValidPoints(
                        map, GetRegionPoints(destination.Region, map));

                HashSet<Point> adjacent = new HashSet<Point>();
                foreach (Point point in GetRegionPoints(destination.Region, map))
                    adjacent.UnionWith(GetAdjacentPoints(map, point));
                if (adjacent.Count > 0) return adjacent;
            }

            return map.GetCell(destination.Point) == null
                ? new HashSet<Point>()
                : new HashSet<Point> { destination.Point };
        }

        private static IEnumerable<Point> GetRegionPoints(MapRegion region, Map map)
        {
            if (region == null) return Enumerable.Empty<Point>();

            return region.PointList ??
                   (IEnumerable<Point>)region.GetPoints(map.Width);
        }

        private static Point GetDisplayDestination(AutoPathDestination destination, Map map, NPCObject destinationNPC, Point routeDestination)
        {
            if (destinationNPC != null) return destinationNPC.CurrentLocation;
            if (destination.NPC == null || destination.Region == null) return routeDestination;

            List<Point> points = GetRegionPoints(destination.Region, map).ToList();
            if (points.Count == 0) return routeDestination;

            int minX = points.Min(x => x.X);
            int maxX = points.Max(x => x.X);
            int minY = points.Min(x => x.Y);
            int maxY = points.Max(x => x.Y);

            return new Point((minX + maxX) / 2, (minY + maxY) / 2);
        }

        private static HashSet<Point> GetAdjacentPoints(Map map, Point centre)
        {
            HashSet<Point> result = new HashSet<Point>();
            foreach (MirDirection direction in Directions)
            {
                Point point = Functions.Move(centre, direction);
                if (map.GetCell(point) != null)
                    result.Add(point);
            }

            return result;
        }

        private static HashSet<Point> GetValidPoints(Map map, IEnumerable<Point> points)
        {
            return points == null
                ? new HashSet<Point>()
                : new HashSet<Point>(points.Where(point => map.GetCell(point) != null));
        }

        private static bool TryFindPreferredPath(MapObject actor, Map map, Point start, HashSet<Point> targets, bool avoidLiveObjects, out List<Point> path)
        {
            path = null;
            if (targets.Count == 0) return false;

            if (!TryFindPath(actor, map, start, targets, avoidLiveObjects, null,
                    out List<Point> directPath))
                return false;

            HashSet<Point> guides = (map.Info.Regions ?? Enumerable.Empty<MapRegion>())
                .Where(x => x.RegionType == RegionType.Path)
                .SelectMany(x => GetRegionPoints(x, map))
                .Where(point => map.GetCell(point) != null)
                .ToHashSet();

            if (guides.Count == 0)
            {
                path = directPath;
                return true;
            }

            bool[,] directPathArea = CreatePathArea(map, directPath);
            guides.RemoveWhere(point => !directPathArea[point.X, point.Y]);

            if (guides.Count == 0 ||
                !TryFindPath(actor, map, start, targets, avoidLiveObjects, guides, out path))
                path = directPath;

            return true;
        }

        private static bool[,] CreatePathArea(Map map, IEnumerable<Point> path)
        {
            bool[,] result = new bool[map.Width, map.Height];
            Queue<(Point Point, int Distance)> pending =
                new Queue<(Point Point, int Distance)>();

            foreach (Point point in path)
            {
                if (result[point.X, point.Y]) continue;

                result[point.X, point.Y] = true;
                pending.Enqueue((point, 0));
            }

            while (pending.TryDequeue(out var current))
            {
                if (current.Distance >= PathGuideDistance) continue;

                foreach (MirDirection direction in Directions)
                {
                    Point next = Functions.Move(current.Point, direction);
                    if (next.X < 0 || next.X >= map.Width ||
                        next.Y < 0 || next.Y >= map.Height ||
                        result[next.X, next.Y])
                        continue;

                    result[next.X, next.Y] = true;
                    pending.Enqueue((next, current.Distance + 1));
                }
            }

            return result;
        }

        private static bool TryFindPath(MapObject actor, Map map,
            Point start, HashSet<Point> targets, bool avoidLiveObjects,
            HashSet<Point> preferredPoints, out List<Point> path)
        {
            path = null;
            if (targets.Contains(start))
            {
                path = new List<Point> { start };
                return true;
            }

            GetBounds(targets, out int minimumX, out int maximumX,
                out int minimumY, out int maximumY);

            PriorityQueue<Point, (int Cost, long Distance)> open = new PriorityQueue<Point, (int Cost, long Distance)>();
            Dictionary<Point, Point> previous = new Dictionary<Point, Point>();
            Dictionary<Point, int> costs = new Dictionary<Point, int> { [start] = 0 };
            HashSet<Point> closed = new HashSet<Point>();
            open.Enqueue(start, (0, DistanceSquaredToBounds(start, minimumX, maximumX, minimumY, maximumY)));

            while (open.TryDequeue(out Point current, out _))
            {
                if (!closed.Add(current)) continue;

                if (targets.Contains(current))
                {
                    path = Reconstruct(previous, current);
                    return true;
                }

                foreach (MirDirection direction in Directions)
                {
                    Point next = Functions.Move(current, direction);
                    if (closed.Contains(next) || map.GetCell(next) == null ||
                        avoidLiveObjects && IsBlockedByLiveObject(actor, map, next))
                        continue;

                    int stepCost = preferredPoints == null || preferredPoints.Contains(next)
                        ? 1
                        : NormalPathCost;
                    int newCost = costs[current] + stepCost;
                    if (costs.TryGetValue(next, out int oldCost) && oldCost <= newCost)
                        continue;

                    costs[next] = newCost;
                    previous[next] = current;

                    int heuristic = DistanceToBounds(next, minimumX, maximumX, minimumY, maximumY);
                    long distance = DistanceSquaredToBounds(next, minimumX, maximumX, minimumY, maximumY);
                    open.Enqueue(next, (newCost + heuristic, distance));
                }
            }

            return false;
        }

        private static void GetBounds(HashSet<Point> points,
            out int minimumX, out int maximumX, out int minimumY, out int maximumY)
        {
            minimumX = int.MaxValue;
            maximumX = int.MinValue;
            minimumY = int.MaxValue;
            maximumY = int.MinValue;

            foreach (Point point in points)
            {
                minimumX = Math.Min(minimumX, point.X);
                maximumX = Math.Max(maximumX, point.X);
                minimumY = Math.Min(minimumY, point.Y);
                maximumY = Math.Max(maximumY, point.Y);
            }
        }

        private static int DistanceToBounds(
            Point point, int minimumX, int maximumX, int minimumY, int maximumY)
        {
            int xDistance = point.X < minimumX
                ? minimumX - point.X
                : point.X > maximumX
                    ? point.X - maximumX
                    : 0;
            int yDistance = point.Y < minimumY
                ? minimumY - point.Y
                : point.Y > maximumY
                    ? point.Y - maximumY
                    : 0;

            return Math.Max(xDistance, yDistance);
        }

        private static long DistanceSquaredToBounds(Point point, int minimumX, int maximumX, int minimumY, int maximumY)
        {
            int xDistance = point.X < minimumX ? minimumX - point.X : point.X > maximumX ? point.X - maximumX : 0;
            int yDistance = point.Y < minimumY ? minimumY - point.Y : point.Y > maximumY ? point.Y - maximumY : 0;
            return (long)xDistance * xDistance + (long)yDistance * yDistance;
        }

        private static bool IsBlockedByLiveObject(MapObject actor, Map map, Point point)
        {
            if (actor.CurrentMap != map) return false;

            Cell cell = map.GetCell(point);
            return cell == null || cell.IsBlocking(actor, false);
        }

        private static List<Point> Reconstruct(
            Dictionary<Point, Point> previous, Point end)
        {
            List<Point> result = new List<Point> { end };
            while (previous.TryGetValue(end, out Point point))
            {
                end = point;
                result.Add(end);
            }

            result.Reverse();
            return result;
        }

        private sealed class SearchNode
        {
            public MapInfo Map;
            public Point Location;
            public int Cost;
            public SearchNode Previous;
            public AutoPathRouteLeg IncomingLeg;
        }
    }
}

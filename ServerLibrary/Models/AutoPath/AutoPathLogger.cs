using Library.SystemModels;
using Server.Envir;
using System.Drawing;

namespace Server.Models.AutoPath
{
    internal static class AutoPathLogger
    {
        public static void Started(MapObject actor, MapInfo sourceMap, Point source, MapInfo destinationMap, Point destination)
        {
            Log("STARTED", actor, sourceMap, source, destinationMap, destination);
        }

        public static void Ended(MapObject actor, MapInfo sourceMap, Point source, MapInfo destinationMap, Point destination)
        {
            Log("ENDED", actor, sourceMap, source, destinationMap, destination);
        }

        private static void Log(string status, MapObject actor, MapInfo sourceMap, Point source, MapInfo destinationMap, Point destination)
        {
            SEnvir.Log($"[AUTO PATH {status}] {GetActorDescription(actor)} from {GetMapDescription(sourceMap)} ({source.X}, {source.Y}) to {GetMapDescription(destinationMap)} ({destination.X}, {destination.Y}).");
        }

        private static string GetActorDescription(MapObject actor)
        {
            switch (actor)
            {
                case PlayerObject player:
                    return $"Player '{player.Name}'";
                case MonsterObject monster:
                    return $"Monster '{monster.MonsterInfo?.MonsterName ?? "Unknown"}'";
                default:
                    return $"{actor?.Race.ToString() ?? "Object"} '{actor?.Name ?? "Unknown"}'";
            }
        }

        private static string GetMapDescription(MapInfo map)
        {
            if (map == null) return "Unknown Map [Unknown File]";

            return $"{map.Description ?? "Unknown Map"} [{map.FileName ?? "Unknown File"}]";
        }
    }
}

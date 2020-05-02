using System;
using System.Diagnostics;

namespace Library
{
    public static class Time
    {
        private static readonly DateTime StartTime = DateTime.UtcNow;
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        public static DateTime Now => StartTime + Stopwatch.Elapsed;
    }
}

using Client.Envir;
using System;
using System.Collections.Generic;

namespace Client.Models
{
    internal static class ItemHighlights
    {
        private static string settings;
        private static readonly HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static bool Contains(string name)
        {
            if (settings != Config.HighlightedItems)
            {
                settings = Config.HighlightedItems;
                names.Clear();

                foreach (string entry in (settings ?? string.Empty).Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(entry))
                        names.Add(entry.Replace(" ", ""));
                }
            }

            return names.Count > 0 && name != null && names.Contains(name.Replace(" ", ""));
        }
    }
}

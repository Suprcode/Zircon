using Client.Controls;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models
{
    internal static class GroundItemLabels
    {
        private sealed class Entry
        {
            public DXLabel Label;
            public int References;
        }
        // Cache ownership is separate from DXControl parenting: the last item releases the label.
        private static readonly Dictionary<(string Text, Color Foreground, Color Background), Entry> entries = new();
        private static readonly Dictionary<DXLabel, (string Text, Color Foreground, Color Background)> keys = new();

        public static DXLabel Acquire(string text, Color foreground, Color background)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var key = (text, foreground, background);
            if (!entries.TryGetValue(key, out Entry entry))
            {
                entry = new Entry
                {
                    Label = new DXLabel
                    {
                        Text = text,
                        ForeColour = foreground,
                        BackColour = background,
                        Outline = true,
                        OutlineColour = Color.Black,
                        Border = background != Color.Empty,
                        BorderColour = Color.Black,
                        IsControl = false,
                        IsVisible = true,
                    },
                };
                entries.Add(key, entry);
                keys.Add(entry.Label, key);
            }
            entry.References++;
            return entry.Label;
        }

        public static void Release(DXLabel label)
        {
            if (label == null || !keys.TryGetValue(label, out var key)) return;
            Entry entry = entries[key];
            if (--entry.References != 0) return;
            entries.Remove(key);
            keys.Remove(label);
            if (!label.IsDisposed)
                label.Dispose();
        }
    }
}

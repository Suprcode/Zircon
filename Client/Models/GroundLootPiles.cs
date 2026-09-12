using System.Collections.Generic;
using System.Drawing;

namespace Client.Models
{
    internal sealed class GroundLootPiles
    {
        private readonly struct Candidate
        {
            public readonly uint ObjectID;
            public readonly int Priority;

            public Candidate(uint objectID, int priority)
            {
                ObjectID = objectID;
                Priority = priority;
            }
        }

        private readonly Dictionary<Point, Candidate> piles = new Dictionary<Point, Candidate>();

        public void Clear() => piles.Clear();

        public uint this[Point location] => piles[location].ObjectID;

        public void Add(Point location, uint objectID, int priority)
        {
            // Keep selection independent of insertion order and avoid writes for losing candidates.
            if (!piles.TryGetValue(location, out Candidate current) || priority > current.Priority ||
                (priority == current.Priority && objectID < current.ObjectID))
                piles[location] = new Candidate(objectID, priority);
        }
    }
}

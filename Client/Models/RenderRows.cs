using System.Collections.Generic;

namespace Client.Models
{
    // Stable buckets retain insertion order without scanning the complete scene per row.
    internal sealed class RenderRows<T>
    {
        private readonly List<List<T>> rows = new List<List<T>>();
        private int firstRow, count;

        public void Reset(int first, int last)
        {
            Clear();
            firstRow = first;
            count = System.Math.Max(0, last - first + 1);
            while (rows.Count < count)
                rows.Add(new List<T>());
        }

        public void Clear()
        {
            foreach (List<T> row in rows)
                row.Clear();

            count = 0;
        }

        public void Add(int row, T value)
        {
            int index = row - firstRow;
            if (index >= 0 && index < count)
                rows[index].Add(value);
        }

        public List<T> this[int row] => rows[row - firstRow];
    }
}

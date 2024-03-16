using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using MirDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Server
{
    public class JsonExporter
    {
        public static void Export<T>(GridView grid) where T : DBObject, new()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DBObjectArrayConverter<T>(SMain.Session) },
                WriteIndented = true
            };

            JsonExporter.Export<T>(grid, options);
        }

        public static void Export<T>(GridView gridView, JsonSerializerOptions options) where T : DBObject, new()
        {
            if (!Directory.Exists($"Exports/{typeof(T).Name}"))
            {
                Directory.CreateDirectory($"Exports/{typeof(T).Name}");
            }

            List<T> selectedItems = new();

            var rows = gridView.GetSelectedRows();

            if (rows.Length == 0)
            {
                selectedItems.AddRange(SMain.Session.GetCollection<T>().Binding);
            }
            else
            {
                foreach (var row in rows)
                {
                    T selRow = (T)gridView.GetRow(row);

                    selectedItems.Add(selRow);
                }
            }

            if (XtraMessageBox.Show($"You're about to export {selectedItems.Count} rows, are you sure?", "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            var json = JsonSerializer.Serialize<T[]>(selectedItems.ToArray(), options);

            var time = DateTime.UtcNow;
            var now = $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}-{time.Second:00}";

            var path = $"Exports/{typeof(T).Name}/{typeof(T).Name} - {selectedItems.Count} - {now}.json";

            using StreamWriter file = new(path);

            file.WriteLine(json);

            XtraMessageBox.Show($"All selected rows have been exported to '{path}'.", "Success", MessageBoxButtons.OK);
        }
    }
}

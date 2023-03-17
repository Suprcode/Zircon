using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using MirDB;
using Server.Envir;

namespace Server
{
    public class JsonImporter
    {
        public static void Import<T>() where T : DBObject, new()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DBObjectArrayConverter<T>(SMain.Session) },
                PropertyNameCaseInsensitive = true
            };

            JsonImporter.Import<T>(options);
        }

        public static void Import<T>(JsonSerializerOptions options)
        {
            if (!Directory.Exists($"Exports/{typeof(T).Name}"))
            {
                Directory.CreateDirectory($"Exports/{typeof(T).Name}");
            }

            SMain.Session.Save(true);

            XtraOpenFileDialog dlg = new()
            {
                InitialDirectory = $"{Directory.GetCurrentDirectory()}\\Exports\\{typeof(T).Name}\\",
                Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var file = File.ReadAllText(dlg.FileName);

                try
                {
                    var rows = JsonSerializer.Deserialize<T[]>(file, options);

                    XtraMessageBox.Show($"{rows.Length} rows have been successfully imported.", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.Message);

                    XtraMessageBox.Show($"Failed to import all rows.\r\n\r\n{ex.Message}", "Fail", MessageBoxButtons.OK);
                }
            }
        }
    }
}

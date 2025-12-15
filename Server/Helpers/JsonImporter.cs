using DevExpress.XtraEditors;
using MirDB;
using Server.Envir;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Server
{
    public class JsonImporter
    {
        public static void Import<T>(bool allowDeferredReferences = true) where T : DBObject, new()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DBObjectArrayConverter<T>(SMain.Session) },
                PropertyNameCaseInsensitive = true
            };

            JsonImporter.Import<T>(options, allowDeferredReferences);
        }

        public static void Import<T>(JsonSerializerOptions options, bool allowDeferredReferences = true)
        {
            if (!Directory.Exists($"Exports/{typeof(T).Name}"))
            {
                Directory.CreateDirectory($"Exports/{typeof(T).Name}");
            }

            ImportReferenceResolver.EnableDeferredResolution = allowDeferredReferences;

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

                    var (resolved, remaining) = ImportReferenceResolver.ResolvePendingReferences(SMain.Session);

                    string message = $"{rows.Length} rows have been successfully imported.";

                    if (allowDeferredReferences)
                    {
                        message += $"\n{resolved} deferred reference(s) were resolved.";

                        if (remaining > 0)
                        {
                            message += $"\n{remaining} reference(s) are still pending and will be retried on the next import.";
                        }
                    }

                    XtraMessageBox.Show(message, "Success", MessageBoxButtons.OK);
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

using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using MirDB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

            var tableName = typeof(T).Name;
            var time = DateTime.UtcNow;
            var now = $"{time.Year:0000}-{time.Month:00}-{time.Day:00} {time.Hour:00}-{time.Minute:00}-{time.Second:00}";

            using ExportConfirmationDialog dialog = new(tableName, selectedItems.Count, now);

            if (dialog.ShowDialog(gridView.GridControl.FindForm()) != DialogResult.OK)
            {
                return;
            }

            var json = JsonSerializer.Serialize<T[]>(selectedItems.ToArray(), options);
            var directory = Path.Combine("Exports", tableName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(directory, dialog.ExportFileName);

            using StreamWriter file = new(path);

            file.WriteLine(json);

            XtraMessageBox.Show($"All selected rows have been exported to '{path}'.", "Success", MessageBoxButtons.OK);
        }
    }

    public class ExportConfirmationDialog : XtraForm
    {
        private readonly string TableName;
        private readonly int RowCount;
        private readonly string Date;

        private readonly TextEdit ExportNameEdit;
        private readonly LabelControl FileNameLabel;

        public string ExportFileName => BuildFileName(ExportNameEdit.Text);

        public ExportConfirmationDialog(string tableName, int rowCount, string date)
        {
            TableName = tableName;
            RowCount = rowCount;
            Date = date;

            Text = "Export";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(460, 170);

            LabelControl confirmationLabel = new()
            {
                Text = $"You're about to export {RowCount} rows.",
                Location = new Point(18, 18),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(424, 20)
            };

            LabelControl exportNameLabel = new()
            {
                Text = "Export name (optional):",
                Location = new Point(18, 49),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(135, 20)
            };

            ExportNameEdit = new TextEdit
            {
                Location = new Point(155, 46),
                Size = new Size(287, 22)
            };
            ExportNameEdit.Properties.MaxLength = 80;
            ExportNameEdit.EditValueChanged += ExportNameEdit_EditValueChanged;

            FileNameLabel = new LabelControl
            {
                Location = new Point(18, 79),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new Size(424, 34),
                Appearance = { ForeColor = SystemColors.GrayText }
            };

            SimpleButton exportButton = new()
            {
                Text = "Export",
                DialogResult = DialogResult.OK,
                Location = new Point(286, 126),
                Size = new Size(75, 28)
            };

            SimpleButton cancelButton = new()
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(367, 126),
                Size = new Size(75, 28)
            };

            Controls.Add(confirmationLabel);
            Controls.Add(exportNameLabel);
            Controls.Add(ExportNameEdit);
            Controls.Add(FileNameLabel);
            Controls.Add(exportButton);
            Controls.Add(cancelButton);

            AcceptButton = exportButton;
            CancelButton = cancelButton;

            UpdateFileName();
        }

        private void ExportNameEdit_EditValueChanged(object sender, EventArgs e)
        {
            UpdateFileName();
        }

        private void UpdateFileName()
        {
            FileNameLabel.Text = $"File: {BuildFileName(ExportNameEdit.Text)}";
        }

        private string BuildFileName(string customName)
        {
            customName = SanitiseFileNamePart(customName);

            return string.IsNullOrWhiteSpace(customName)
                ? $"{TableName} - {RowCount} - {Date}.json"
                : $"{TableName} - {customName} - {RowCount} - {Date}.json";
        }

        private static string SanitiseFileNamePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();

            return new string(value.Trim()
                .Select(character => invalidCharacters.Contains(character) ? '_' : character)
                .ToArray())
                .Trim(' ', '.');
        }
    }
}

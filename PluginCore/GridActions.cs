using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PluginCore
{
    /// <summary>
    /// Optional plugin capability for contributing row actions to supported editor grids.
    /// Existing plugins do not need to implement this interface.
    /// </summary>
    public interface IPluginGridActionProvider
    {
        IEnumerable<PluginGridAction> GetGridActions(Type rowType);
    }

    public sealed class PluginGridAction
    {
        public string Key { get; init; }
        public string Caption { get; init; }
        public string ToolTip { get; init; }
        public Image Image { get; init; }
        public Func<object, bool> CanExecute { get; init; }
        public Action<PluginGridActionContext> Execute { get; init; }
    }

    public sealed class PluginGridActionContext
    {
        public object Row { get; }
        public IWin32Window Owner { get; }

        public PluginGridActionContext(object row, IWin32Window owner)
        {
            Row = row;
            Owner = owner;
        }
    }

    public static class PluginGridActionBinder
    {
        public static void Attach(GridControl gridControl, GridView view, Type rowType, IWin32Window owner)
        {
            foreach (IPluginStart plugin in PluginLoader.Instance.Plugins.ToArray())
            {
                if (plugin.Type is not IPluginGridActionProvider provider) continue;

                IEnumerable<PluginGridAction> actions;
                try
                {
                    actions = provider.GetGridActions(rowType) ?? Enumerable.Empty<PluginGridAction>();
                }
                catch (Exception ex)
                {
                    plugin.LogMessage($"Unable to load grid actions for {rowType.Name}: {ex}");
                    continue;
                }

                foreach (PluginGridAction action in actions)
                    AttachAction(gridControl, view, rowType, owner, plugin, action);
            }
        }

        private static void AttachAction(GridControl gridControl, GridView view, Type rowType, IWin32Window owner, IPluginStart plugin, PluginGridAction action)
        {
            if (action == null || string.IsNullOrWhiteSpace(action.Key) || action.Execute == null) return;

            string name = $"PluginAction.{plugin.Name}.{action.Key}";
            if (view.Columns.ColumnByName(name) != null) return;

            var editor = new RepositoryItemButtonEdit
            {
                Name = name + ".Editor",
                TextEditStyle = TextEditStyles.HideTextEditor
            };
            editor.Buttons.Clear();
            var button = new EditorButton(ButtonPredefines.Glyph) { Caption = action.Caption ?? action.Key };
            button.ToolTip = action.ToolTip ?? string.Empty;
            if (action.Image != null) button.ImageOptions.Image = action.Image;
            editor.Buttons.Add(button);
            gridControl.RepositoryItems.Add(editor);

            var column = new GridColumn
            {
                Name = name,
                Caption = action.Caption ?? action.Key,
                ColumnEdit = editor,
                UnboundType = DevExpress.Data.UnboundColumnType.Object,
                Visible = true,
                VisibleIndex = view.Columns.Count,
                Width = 90
            };
            column.OptionsColumn.AllowEdit = true;
            column.OptionsColumn.AllowFocus = true;
            column.OptionsColumn.ReadOnly = false;
            column.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            view.Columns.Add(column);

            view.ShowingEditor += (_, e) =>
            {
                if (view.FocusedColumn != column) return;
                object row = view.GetFocusedRow();
                if (row == null || !rowType.IsInstanceOfType(row) || action.CanExecute?.Invoke(row) == false)
                    e.Cancel = true;
            };

            editor.ButtonClick += (_, _) =>
            {
                try
                {
                    object row = view.GetFocusedRow();
                    if (row == null || !rowType.IsInstanceOfType(row) || action.CanExecute?.Invoke(row) == false) return;
                    action.Execute(new PluginGridActionContext(row, owner));
                }
                catch (Exception ex)
                {
                    plugin.LogMessage($"Grid action {action.Key} failed: {ex}");
                }
            };
        }
    }
}

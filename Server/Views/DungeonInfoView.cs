using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class DungeonInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DungeonInfoView()
        {
            InitializeComponent();

            DungeonInfoGridControl.DataSource = SMain.Session.GetCollection<DungeonInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            RoleImageComboBox.Items.AddEnum<DungeonMapRole>();

            DungeonMapGridView.CustomRowCellEditForEditing += DungeonMapGridView_CustomRowCellEditForEditing;
            DungeonMapGridView.ValidateRow += DungeonMapGridView_ValidateRow;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(DungeonInfoGridView);
            SMain.SetUpView(DungeonMapGridView);
        }

        private void DungeonMapGridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName != nameof(DungeonMapInfo.Map)) return;

            GridView view = sender as GridView;
            MapInfo currentMap = view?.GetRowCellValue(e.RowHandle, e.Column) as MapInfo;
            var availableMaps = SMain.Session.GetCollection<MapInfo>().Binding
                .Where(x => x.DungeonMap == null || x == currentMap)
                .ToList();

            e.RepositoryItem = CreateMapLookUpEdit(availableMaps);
        }

        private static RepositoryItemLookUpEdit CreateMapLookUpEdit(object dataSource)
        {
            RepositoryItemLookUpEdit lookUpEdit = new()
            {
                DataSource = dataSource,
                DisplayMember = nameof(MapInfo.ServerDescription),
                NullText = "[Map is null]",
                BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup
            };

            lookUpEdit.Columns.AddRange(
            [
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(MapInfo.Index), "Index"),
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(MapInfo.FileName), "File Name"),
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(MapInfo.ServerDescription), "Description")
            ]);

            return lookUpEdit;
        }

        private void DungeonMapGridView_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            if (e.Row is not DungeonMapInfo dungeonMap) return;

            if (dungeonMap.Map == null)
            {
                e.Valid = false;
                e.ErrorText = "A dungeon map must reference a map.";
                return;
            }

            DungeonMapInfo duplicate = SMain.Session.GetCollection<DungeonMapInfo>().Binding
                .FirstOrDefault(x => x != dungeonMap && x.Map == dungeonMap.Map);

            if (duplicate == null) return;

            e.Valid = false;
            e.ErrorText = $"{dungeonMap.Map.ServerDescription} already belongs to {duplicate.Dungeon?.Name}.";
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<DungeonInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<DungeonInfo>(DungeonInfoGridView);
        }
    }
}

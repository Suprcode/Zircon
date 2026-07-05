using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Views
{
    public partial class MovementInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private int _sourceRegionTopRowIndex;
        private int _destinationRegionTopRowIndex;

        public MovementInfoView()
        {
            InitializeComponent();

            MovementGridControl.DataSource = SMain.Session.GetCollection<MovementInfo>().Binding;

            UpdateRegionLookUps();
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            SpawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            InstanceLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;

            MapIconImageComboBox.Items.AddEnum<MapIcon>();
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MovementGridView);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MovementInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MovementInfo>(MovementGridView);
        }

        private void InsertRowButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.InsertRowAfterFocusedObject<MovementInfo>(MovementGridView);
        }

        private void UsedRegionFilter_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            UpdateRegionLookUps();
        }

        private void RegionLookUpEdit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (e.Value is MapRegion region)
                e.DisplayText = region.ToString();
        }

        private void MovementGridView_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MovementGridView.FocusedColumn == gridColumn1 || MovementGridView.FocusedColumn == gridColumn2)
                UpdateRegionLookUps();
        }

        private void RegionLookUpEdit_Popup(object sender, EventArgs e)
        {
            if (sender is not DevExpress.XtraEditors.LookUpEdit editor) return;

            int topRowIndex = MovementGridView.FocusedColumn == gridColumn1
                ? _sourceRegionTopRowIndex
                : _destinationRegionTopRowIndex;

            SetPopupTopRowIndex(editor, topRowIndex);
        }

        private void RegionLookUpEdit_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (sender is not DevExpress.XtraEditors.LookUpEdit editor) return;

            int topRowIndex = GetPopupTopRowIndex(editor);

            if (MovementGridView.FocusedColumn == gridColumn1)
                _sourceRegionTopRowIndex = topRowIndex;
            else
                _destinationRegionTopRowIndex = topRowIndex;
        }

        private static int GetPopupTopRowIndex(DevExpress.XtraEditors.LookUpEdit editor)
        {
            return editor.GetPopupEditForm()?.TopIndex ?? 0;
        }

        private static void SetPopupTopRowIndex(DevExpress.XtraEditors.LookUpEdit editor, int value)
        {
            DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupForm = editor.GetPopupEditForm();
            if (popupForm != null)
                popupForm.TopIndex = value;
        }

        private void UpdateRegionLookUps()
        {
            List<MapRegion> regions = SMain.Session.GetCollection<MapRegion>().Binding
                .Where(x => x.RegionType == RegionType.None || x.RegionType == RegionType.Connection || x.RegionType == RegionType.SpawnConnection)
                .ToList();

            List<RegionLookUpItem> regionItems = CreateRegionLookUpItems(regions);

            MapLookUpEdit.DataSource = regionItems;
            DestinationMapLookUpEdit.DataSource = regionItems.ToList();
        }

        private List<RegionLookUpItem> CreateRegionLookUpItems(IEnumerable<MapRegion> regions)
        {
            if (HideUsedSourcesButton.Checked)
                regions = regions.Where(x => x.SourceMovements == null || x.SourceMovements.Count == 0);

            if (HideUsedDestinationsButton.Checked)
                regions = regions.Where(x => x.DestinationMovements == null || x.DestinationMovements.Count == 0);

            return regions.Select(x => new RegionLookUpItem(x)).ToList();
        }

        private sealed class RegionLookUpItem
        {
            public MapRegion Region { get; }
            public string Text { get; }
            public int Size => Region.Size;

            public RegionLookUpItem(MapRegion region)
            {
                Region = region;
                Text = region.ToString();
            }
        }
    }
}

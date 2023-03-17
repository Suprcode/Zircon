using System;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library.SystemModels;

namespace Server.Views
{
    public partial class MapRegionView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapRegionView()
        {
            InitializeComponent();

            MapRegionGridControl.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            MapLookUpEdit.DataSource =  SMain.Session.GetCollection<MapInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MapRegionGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapRegionGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MapRegion>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MapRegion>(MapRegionGridView);
        }
    }
}
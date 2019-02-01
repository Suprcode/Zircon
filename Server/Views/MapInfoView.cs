using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using Server.DBModels;

namespace Server.Views
{
    public partial class MapInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapInfoView()
        {
            InitializeComponent();

            MapInfoGridControl.DataSource =  SMain.Session.GetCollection<MapInfo>().Binding;
            MonsterLookUpEdit.DataSource =  SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource =  SMain.Session.GetCollection<MapInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            LightComboBox.Items.AddEnum<LightSetting>();
            DirectionImageComboBox.Items.AddEnum<MirDirection>();
            MapIconImageComboBox.Items.AddEnum<MapIcon>();
            StartClassImageComboBox.Items.AddEnum<RequiredClass>();

        }
        
        

        private void MapInfoView_Load(object sender, EventArgs e)
        {
            SMain.SetUpView(MapInfoGridView);
            SMain.SetUpView(GuardsGridView);
            SMain.SetUpView(RegionGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ClearMapsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
           
        }

        private void RenameMiniMapButton_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapInfoGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }
    }
}
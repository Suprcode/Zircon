using System;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class InstanceInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public InstanceInfoView()
        {
            InitializeComponent();

            InstanceInfoGridControl.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            InstanceTypeImageComboBox.Items.AddEnum<InstanceType>();
            StatComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(InstanceInfoGridView);
            SMain.SetUpView(InstanceMapGridView);
            SMain.SetUpView(InstanceInfoStatsGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<InstanceInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<InstanceInfo>(InstanceInfoGridView);
        }
    }
}
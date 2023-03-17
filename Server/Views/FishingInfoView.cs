using System;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class FishingInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public FishingInfoView()
        {
            InitializeComponent();

            FishingInfoGridControl.DataSource = SMain.Session.GetCollection<FishingInfo>().Binding;

            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(FishingInfoGridView);
            SMain.SetUpView(FishingDropInfoGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<FishingInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<FishingInfo>(FishingInfoGridView);
        }
    }
}
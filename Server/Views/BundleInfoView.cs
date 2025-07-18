using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class BundleInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public BundleInfoView()
        {
            InitializeComponent();

            BundleInfoGridControl.DataSource = SMain.Session.GetCollection<BundleInfo>().Binding;

            BundleItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(BundleInfoGridView);
            SMain.SetUpView(BundleItemInfoGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<BundleInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<BundleInfo>(BundleInfoGridView);
        }
    }
}
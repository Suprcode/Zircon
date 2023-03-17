using System;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class StoreInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public StoreInfoView()
        {
            InitializeComponent();

            StoreInfoGridControl.DataSource = SMain.Session.GetCollection<StoreInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            SMain.SetUpView(StoreInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<StoreInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<StoreInfo>(StoreInfoGridView);
        }
    }
}
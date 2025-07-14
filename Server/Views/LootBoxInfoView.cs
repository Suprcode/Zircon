using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class LootBoxInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public LootBoxInfoView()
        {
            InitializeComponent();

            LootBoxInfoGridControl.DataSource = SMain.Session.GetCollection<LootBoxInfo>().Binding;

            LootBoxItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            CurrencyLookUpEdit.DataSource = SMain.Session.GetCollection<CurrencyInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(LootBoxInfoGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<LootBoxInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<LootBoxInfo>(LootBoxInfoGridView);
        }
    }
}
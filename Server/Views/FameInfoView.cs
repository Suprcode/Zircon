using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class FameInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public FameInfoView()
        {
            InitializeComponent();

            FameInfoGridControl.DataSource = SMain.Session.GetCollection<FameInfo>().Binding;

            StatComboBox.Items.AddEnum<Stat>();
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(FameInfoGridView);
            SMain.SetUpView(FameInfoStatGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<FameInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<FameInfo>(FameInfoGridView);
        }
    }
}
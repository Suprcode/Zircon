using System;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class DisciplineInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DisciplineInfoView()
        {
            InitializeComponent();

            DisciplineInfoGridControl.DataSource = SMain.Session.GetCollection<DisciplineInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(DisciplineInfoGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<DisciplineInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<DisciplineInfo>(DisciplineInfoGridView);
        }
    }
}
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Library;
using Library.SystemModels;
using System;

namespace Server.Views
{
    public partial class HelpInfoView : RibbonForm
    {
        public HelpInfoView()
        {
            InitializeComponent();

            HelpGridControl.DataSource = SMain.Session.GetCollection<HelpInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(HelpGridView);
            SMain.SetUpView(PageGridView);
            SMain.SetUpView(SectionGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<HelpInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<HelpInfo>(HelpGridView);
        }
    }
}
using System;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class CompanionInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CompanionInfoView()
        {
            InitializeComponent();

            CompanionInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionInfo>().Binding;
            CompanionLevelInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionLevelInfo>().Binding;
            CompanionSkillInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionSkillInfo>().Binding;

            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CompanionInfoGridView);
            SMain.SetUpView(CompanionLevelInfoGridView);
            SMain.SetUpView(CompanionSkillInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tabPane1.SelectedPage == tabNavigationPage1)
            {
                JsonImporter.Import<CompanionInfo>();
            }
            else if (tabPane1.SelectedPage == tabNavigationPage2)
            {
                JsonImporter.Import<CompanionLevelInfo>();
            }
            else if (tabPane1.SelectedPage == tabNavigationPage3)
            {
                JsonImporter.Import<CompanionSkillInfo>();
            }
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tabPane1.SelectedPage == tabNavigationPage1)
            {
                JsonExporter.Export<CompanionInfo>(CompanionInfoGridView);
            }
            else if (tabPane1.SelectedPage == tabNavigationPage2)
            {
                JsonExporter.Export<CompanionLevelInfo>(CompanionLevelInfoGridView);
            }
            else if (tabPane1.SelectedPage == tabNavigationPage3)
            {
                JsonExporter.Export<CompanionSkillInfo>(CompanionSkillInfoGridView);
            }
        }
    }
}
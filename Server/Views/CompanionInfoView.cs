using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Linq;

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
            CurrencyInfoLookUpEdit.DataSource = SMain.Session.GetCollection<CurrencyInfo>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            CompanionActionImageComboBox.Items.AddEnum<CompanionAction>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CompanionInfoGridView);
            SMain.SetUpView(CompanionSpeechGridView);
            SMain.SetUpView(CompanionLevelInfoGridView);
            SMain.SetUpView(CompanionSkillInfoGridView);

            var goldCurrency = SMain.Session.GetCollection<CurrencyInfo>().Binding.Where(x => x.Type == CurrencyType.Gold).FirstOrDefault();

            if (goldCurrency == null) return;

            var companions = SMain.Session.GetCollection<CompanionInfo>().Binding;

            foreach (var companion in companions)
                companion.Currency ??= goldCurrency;

            CompanionInfoGridView.RefreshData();
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
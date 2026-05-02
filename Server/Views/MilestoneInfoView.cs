using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class MilestoneInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MilestoneInfoView()
        {
            InitializeComponent();

            MilestoneInfoGridControl.DataSource = SMain.Session.GetCollection<MilestoneInfo>().Binding;

            TypeImageComboBox.Items.AddEnum<MilestoneType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            GradeImageComboBox.Items.AddEnum<MilestoneGrade>();
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding.Where(x => x.RegionType == RegionType.None || x.RegionType == RegionType.Area);
            CurrencyInfoLookUpEdit.DataSource = SMain.Session.GetCollection<CurrencyInfo>().Binding;
            InstanceInfoLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            QuestInfoLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            MagicInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MagicInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MilestoneInfoGridView);
            SMain.SetUpView(MilestoneInfoTaskGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MilestoneInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MilestoneInfo>(MilestoneInfoGridView);
        }
    }
}

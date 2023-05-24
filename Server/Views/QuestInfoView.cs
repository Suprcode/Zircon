using System;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class QuestInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public QuestInfoView()
        {
            InitializeComponent();

            RequirementImageComboBox.Items.AddEnum<QuestRequirementType>();
            TaskImageComboBox.Items.AddEnum<QuestTaskType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            TypeImageComboBox.Items.AddEnum<QuestType>();

            QuestInfoGridControl.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;

            QuestInfoLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            NPCLookUpEdit.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(QuestInfoGridView);
            SMain.SetUpView(RequirementsGridView);
            SMain.SetUpView(TaskGridView);
            SMain.SetUpView(MonsterDetailsGridView);
            SMain.SetUpView(RewardsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<QuestInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<QuestInfo>(QuestInfoGridView);
        }
    }
}
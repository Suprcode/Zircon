using System;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class NPCInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public NPCInfoView()
        {
            InitializeComponent();

            NPCInfoGridControl.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            PageLookUpEdit.DataSource = SMain.Session.GetCollection<NPCPage>().Binding;

            QuestInfoLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequirementImageComboBox.Items.AddEnum<NPCRequirementType>();

            DaysOfWeekImageComboBox.Items.AddEnum<DaysOfWeek>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCInfoGridView);
            SMain.SetUpView(RequirementGridView);
        }
        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<NPCInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<NPCInfo>(NPCInfoGridView);
        }
    }
}
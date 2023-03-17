using System;
using DevExpress.XtraBars;

using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class ItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemInfoView()
        {
            InitializeComponent();

            ItemInfoGridControl.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            SetLookUpEdit.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            ItemTypeImageComboBox.Items.AddEnum<ItemType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredGenderImageComboBox.Items.AddEnum<RequiredGender>();
            StatImageComboBox.Items.AddEnum<Stat>();
            RequiredTypeImageComboBox.Items.AddEnum<RequiredType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoGridView);
            SMain.SetUpView(ItemStatsGridView);
            SMain.SetUpView(DropsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_Click(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<ItemInfo>();
        }

        private void ExportButton_Click(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<ItemInfo>(ItemInfoGridView);
        }
    }
}
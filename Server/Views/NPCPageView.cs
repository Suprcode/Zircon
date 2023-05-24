using System;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class NPCPageView : RibbonForm
    {
        public NPCPageView()
        {
            InitializeComponent();

            NPCPageGridControl.DataSource = SMain.Session.GetCollection<NPCPage>().Binding; 

            PageLookUpEdit.DataSource = SMain.Session.GetCollection<NPCPage>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            InstanceLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            CurrencyInfoLookUpEdit.DataSource = SMain.Session.GetCollection<CurrencyInfo>().Binding;

            DialogTypeImageComboBox.Items.AddEnum<NPCDialogType>();
            CheckTypeImageComboBox.Items.AddEnum<NPCCheckType>();
            ActionTypeImageComboBox.Items.AddEnum<NPCActionType>();
            ItemTypeImageComboBox.Items.AddEnum<ItemType>();

            DataTypeImageComboBox.Items.AddEnum<NPCDataType>();
            ValueTypeImageComboBox.Items.AddEnum<NPCValueType>();
            FieldTypeImageComboBox.Items.AddEnum<NPCFieldType>();
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCPageGridView);
            SMain.SetUpView(ChecksGridView);
            SMain.SetUpView(ActionsGridView);
            SMain.SetUpView(ButtonsGridView);
            SMain.SetUpView(TypesGridView);
            SMain.SetUpView(GoodsGridView);
            SMain.SetUpView(ValuesGridView);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<NPCPage>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<NPCPage>(NPCPageGridView);
        }
    }
}
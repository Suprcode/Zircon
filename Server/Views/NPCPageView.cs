using System;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Library;
using Library.SystemModels;
using Server.DBModels;

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

            DialogTypeImageComboBox.Items.AddEnum<NPCDialogType>();
            CheckTypeImageComboBox.Items.AddEnum<NPCCheckType>();
            ActionTypeImageComboBox.Items.AddEnum<NPCActionType>();
            ItemTypeImageComboBox.Items.AddEnum<ItemType>();

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
        }
    }
}
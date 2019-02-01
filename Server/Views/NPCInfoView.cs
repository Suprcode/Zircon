using System;
using DevExpress.XtraBars;
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
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCInfoGridView);
        }
        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
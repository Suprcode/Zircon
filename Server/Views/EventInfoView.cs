using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class EventInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public EventInfoView()
        {
            InitializeComponent();


            EventInfoGridControl.DataSource = SMain.Session.GetCollection<EventInfo>().Binding;

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RespawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(EventInfoGridView);
            SMain.SetUpView(TargetsGridView);
            SMain.SetUpView(ActionsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
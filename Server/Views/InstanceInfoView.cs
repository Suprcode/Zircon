using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class InstanceInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public InstanceInfoView()
        {
            InitializeComponent();

            InstanceInfoGridControl.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(InstanceInfoGridView);
            SMain.SetUpView(InstanceMapGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
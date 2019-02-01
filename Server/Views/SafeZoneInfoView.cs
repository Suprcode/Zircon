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
    public partial class SafeZoneInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SafeZoneInfoView()
        {
            InitializeComponent();

            SafeZoneGridControl.DataSource = SMain.Session.GetCollection<SafeZoneInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(SafeZoneGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
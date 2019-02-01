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
    public partial class BaseStatView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public BaseStatView()
        {
            InitializeComponent();

            BaseStatGridControl.DataSource = SMain.Session.GetCollection<BaseStat>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(BaseStatGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
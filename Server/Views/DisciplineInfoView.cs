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
    public partial class DisciplineInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DisciplineInfoView()
        {
            InitializeComponent();

            DisciplineInfoGridControl.DataSource = SMain.Session.GetCollection<DisciplineInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(DisciplineInfoGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
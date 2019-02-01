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
    public partial class SetInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SetInfoView()
        {
            InitializeComponent();

            SetInfoGridControl.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            StatImageComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(SetInfoGridView);
            SMain.SetUpView(SetStatsGridView);
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
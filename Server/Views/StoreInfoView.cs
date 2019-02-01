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
    public partial class StoreInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public StoreInfoView()
        {
            InitializeComponent();

            StoreInfoGridControl.DataSource = SMain.Session.GetCollection<StoreInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            SMain.SetUpView(StoreInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
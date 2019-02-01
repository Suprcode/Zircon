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
    public partial class CastleInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CastleInfoView()
        {
            InitializeComponent();


            CastleInfoGridControl.DataSource = SMain.Session.GetCollection<CastleInfo>().Binding;
            
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CastleInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
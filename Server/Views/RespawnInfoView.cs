using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class RespawnInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public RespawnInfoView()
        {
            InitializeComponent();

            RespawnInfoGridControl.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding.Where(x => x.RegionType == RegionType.None || x.RegionType == RegionType.Spawn || x.RegionType == RegionType.SpawnConnection);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(RespawnInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<RespawnInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<RespawnInfo>(RespawnInfoGridView);
        }
    }
}
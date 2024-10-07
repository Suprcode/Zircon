using System;
using System.Linq;
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

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding.Where(x => x.Flag == Library.MonsterFlag.CastleObjective || x.Flag == Library.MonsterFlag.CastleDefense);
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

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<CastleInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<CastleInfo>(CastleInfoGridView);
        }
    }
}
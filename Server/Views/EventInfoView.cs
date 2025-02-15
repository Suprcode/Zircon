using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class EventInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public EventInfoView()
        {
            InitializeComponent();

            WorldEventInfoGridControl.DataSource = SMain.Session.GetCollection<WorldEventInfo>().Binding;
            PlayerEventInfoGridControl.DataSource = SMain.Session.GetCollection<PlayerEventInfo>().Binding;
            MonsterEventInfoGridControl.DataSource = SMain.Session.GetCollection<MonsterEventInfo>().Binding;

            WorldMonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            WorldRespawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            WorldRegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding.Where(x => x.RegionType == RegionType.Area);
            WorldMapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            WorldInstanceLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            WorldActionTypeLookUpEdit.DataSource = SEnvir.EventHandler.GetWorldEventActions();

            PlayerMonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            PlayerRespawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            PlayerRegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding.Where(x => x.RegionType == RegionType.Area);
            PlayerMapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            PlayerInstanceLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            PlayerItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            PlayerActionTypeLookUpEdit.DataSource = SEnvir.EventHandler.GetPlayerEventActions();

            MonsterMonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MonsterRespawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            MonsterRegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding.Where(x => x.RegionType == RegionType.Area);
            MonsterMapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MonsterInstanceLookUpEdit.DataSource = SMain.Session.GetCollection<InstanceInfo>().Binding;
            MonsterItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterActionTypeLookUpEdit.DataSource = SEnvir.EventHandler.GetMonsterEventActions();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(WorldEventInfoGridView);
            SMain.SetUpView(PlayerEventInfoGridView);
            SMain.SetUpView(MonsterEventInfoGridView);

            SMain.SetUpView(WorldTriggersInfoGridView);
            SMain.SetUpView(WorldActionsInfoGridView);

            SMain.SetUpView(PlayerTriggersInfoGridView);
            SMain.SetUpView(PlayerActionsInfoGridView);

            SMain.SetUpView(MonsterTriggersInfoGridView);
            SMain.SetUpView(MonsterActionsInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tabPane1.SelectedPage == tabNavigationPage1)
            {
                JsonImporter.Import<WorldEventInfo>();
            }
            else if (tabPane1.SelectedPage == tabNavigationPage2)
            {
                JsonImporter.Import<PlayerEventInfo>();
            }
            else if (tabPane1.SelectedPage == tabNavigationPage3)
            {
                JsonImporter.Import<MonsterEventInfo>();
            }
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tabPane1.SelectedPage == tabNavigationPage1)
            {
                JsonExporter.Export<WorldEventInfo>(WorldEventInfoGridView);
            }
            else if (tabPane1.SelectedPage == tabNavigationPage2)
            {
                JsonExporter.Export<PlayerEventInfo>(PlayerEventInfoGridView);
            }
            else if (tabPane1.SelectedPage == tabNavigationPage3)
            {
                JsonExporter.Export<MonsterEventInfo>(MonsterEventInfoGridView);
            }
        }
    }
}
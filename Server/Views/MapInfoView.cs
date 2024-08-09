using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Linq;

namespace Server.Views
{
    public partial class MapInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapInfoView()
        {
            InitializeComponent();

            MapInfoGridControl.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;

            LightComboBox.Items.AddEnum<LightSetting>();
            WeatherComboBox.Items.AddEnum<Weather>();
            DirectionImageComboBox.Items.AddEnum<MirDirection>();
            MapIconImageComboBox.Items.AddEnum<MapIcon>();
            StartClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            StatImageComboBox.Items.AddEnum<Stat>();

            MiningGridView.CustomRowCellEditForEditing += MiningGridView_CustomRowCellEditForEditing;
        }

        private void MiningGridView_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "Region")
            {
                var currentMapRow = MapInfoGridView.GetRow(MapInfoGridView.FocusedRowHandle) as MapInfo;

                var binding = SMain.Session.GetCollection<MapRegion>().Binding;

                var filteredDataSource = binding.Where(x => x.Map == currentMapRow).ToList();

                RepositoryItemLookUpEdit lookupEdit = new()
                {
                    DataSource = filteredDataSource,
                    DisplayMember = "Description",
                    NullText = "[Region is null]"
                };

                lookupEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] { new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description"), new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size") });
                
                e.RepositoryItem = lookupEdit;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MapInfoGridView);
            SMain.SetUpView(GuardsGridView);
            SMain.SetUpView(RegionGridView);
            SMain.SetUpView(MiningGridView);

            UpdateInfoStats();
        }

        private void UpdateInfoStats()
        {
            bool needSave = false;

            foreach (var map in SMain.Session.GetCollection<MapInfo>().Binding)
            {
                if (map.MonsterHealth != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MonsterHealth;
                    stat.Amount = map.MonsterHealth;
                    map.BuffStats.Add(stat);

                    map.MonsterHealth = 0;
                    needSave = true;
                }

                if (map.MonsterDamage != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MonsterDamage;
                    stat.Amount = map.MonsterDamage;
                    map.BuffStats.Add(stat);

                    map.MonsterDamage = 0;
                    needSave = true;
                }

                if (map.DropRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MonsterDrop;
                    stat.Amount = map.DropRate;
                    map.BuffStats.Add(stat);

                    map.DropRate = 0;
                    needSave = true;
                }

                if (map.ExperienceRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MonsterExperience;
                    stat.Amount = map.ExperienceRate;
                    map.BuffStats.Add(stat);

                    map.ExperienceRate = 0;
                    needSave = true;
                }

                if (map.GoldRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MonsterGold;
                    stat.Amount = map.GoldRate;
                    map.BuffStats.Add(stat);

                    map.GoldRate = 0;
                    needSave = true;
                }

                if (map.MaxMonsterHealth != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MaxMonsterHealth;
                    stat.Amount = map.MaxMonsterHealth;
                    map.BuffStats.Add(stat);

                    map.MaxMonsterHealth = 0;
                    needSave = true;
                }

                if (map.MaxMonsterDamage != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MaxMonsterDamage;
                    stat.Amount = map.MaxMonsterDamage;
                    map.BuffStats.Add(stat);

                    map.MaxMonsterDamage = 0;
                    needSave = true;
                }

                if (map.MaxDropRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MaxMonsterDrop;
                    stat.Amount = map.MaxDropRate;
                    map.BuffStats.Add(stat);

                    map.MaxDropRate = 0;
                    needSave = true;
                }

                if (map.MaxExperienceRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MaxMonsterExperience;
                    stat.Amount = map.MaxExperienceRate;
                    map.BuffStats.Add(stat);

                    map.MaxExperienceRate = 0;
                    needSave = true;
                }

                if (map.MaxGoldRate != 0)
                {
                    var stat = SMain.Session.GetCollection<MapInfoStat>().CreateNewObject();
                    stat.Stat = Stat.MaxMonsterGold;
                    stat.Amount = map.MaxGoldRate;
                    map.BuffStats.Add(stat);

                    map.MaxGoldRate = 0;
                    needSave = true;
                }
            }

            if (needSave)
            {
                SMain.Session.Save(true);
            }
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapInfoGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MapInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MapInfo>(MapInfoGridView);
        }
    }
}
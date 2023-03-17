using System;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class MonsterInfoStatView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonsterInfoStatView()
        {
            InitializeComponent();

            MonsterInfoStatGridControl.DataSource = SMain.Session.GetCollection<MonsterInfoStat>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            StatImageComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonsterInfoStatGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MonsterInfoStat>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MonsterInfoStat>(MonsterInfoStatGridView);
        }
    }
}
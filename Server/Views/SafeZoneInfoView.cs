using System;
using DevExpress.XtraBars;
using Library.SystemModels;

namespace Server.Views
{
    public partial class SafeZoneInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SafeZoneInfoView()
        {
            InitializeComponent();

            SafeZoneGridControl.DataSource = SMain.Session.GetCollection<SafeZoneInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(SafeZoneGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<SafeZoneInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<SafeZoneInfo>(SafeZoneGridView);
        }
    }
}
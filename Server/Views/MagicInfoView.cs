using System;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;

namespace Server.Views
{
    public partial class MagicInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MagicInfoView()
        {
            InitializeComponent();

            MagicInfoGridControl.DataSource = SMain.Session.GetCollection<MagicInfo>().Binding;

            MagicImageComboBox.Items.AddEnum<MagicType>();
            SchoolImageComboBox.Items.AddEnum<MagicSchool>();
            PropertyImageComboBox.Items.AddEnum<MagicProperty>();
            ClassImageComboBox.Items.AddEnum<MirClass>();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MagicInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void MagicInfoView_Load(object sender, EventArgs e)
        {

        }

        private void ImportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonImporter.Import<MagicInfo>();
        }

        private void ExportButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            JsonExporter.Export<MagicInfo>(MagicInfoGridView);
        }
    }
}
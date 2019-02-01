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
    public partial class CompanionInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CompanionInfoView()
        {
            InitializeComponent();

            CompanionInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionInfo>().Binding;
            CompanionLevelInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionLevelInfo>().Binding;
            CompanionSkillInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionSkillInfo>().Binding;

            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CompanionInfoGridView);
            SMain.SetUpView(CompanionLevelInfoGridView);
            SMain.SetUpView(CompanionSkillInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }
    }
}
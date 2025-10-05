using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;
using System;

namespace Server.Views
{
    public partial class NPCListView : DevExpress.XtraEditors.XtraForm
    {
        public NPCListView()
        {
            InitializeComponent();

            NPCListGridControl.DataSource = SEnvir.GameNPCList?.Binding;

            NPCListGridView.OptionsSelection.MultiSelect = true;
            NPCListGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCListGridView);
        }
    }
}
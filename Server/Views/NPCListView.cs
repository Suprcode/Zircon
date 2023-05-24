using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

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
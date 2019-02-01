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
    public partial class UserConquestStatsView : DevExpress.XtraEditors.XtraForm
    {
        public UserConquestStatsView()
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.UserConquestStatsList?.Binding;
            

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
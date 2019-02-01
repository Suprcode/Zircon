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
    public partial class UserDropView : DevExpress.XtraEditors.XtraForm
    {
        public UserDropView()
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.UserDropList?.Binding;

            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;
            ItemLookUpEdit.DataSource = SEnvir.ItemInfoList?.Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
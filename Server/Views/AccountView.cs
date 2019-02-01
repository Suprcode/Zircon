using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class AccountView : DevExpress.XtraEditors.XtraForm
    {
        public AccountView()
        {
            InitializeComponent();

            AccountGridControl.DataSource = SEnvir.AccountInfoList?.Binding;
            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;

            AccountGridView.OptionsSelection.MultiSelect = true;
            AccountGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
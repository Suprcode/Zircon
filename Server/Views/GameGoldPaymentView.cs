using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class GameGoldPaymentView : DevExpress.XtraEditors.XtraForm
    {
        public GameGoldPaymentView()
        {
            InitializeComponent();

            GameGoldPaymentGridControl.DataSource = SEnvir.GameGoldPaymentList?.Binding;
            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;

            GameGoldPaymentGridView.OptionsSelection.MultiSelect = true;
            GameGoldPaymentGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
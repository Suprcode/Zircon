using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class GameStoreSaleView : DevExpress.XtraEditors.XtraForm
    {
        public GameStoreSaleView()
        {
            InitializeComponent();

            GameStoreSaleGridControl.DataSource = SEnvir.GameStoreSaleList?.Binding;
            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;
            ItemLookUpEdit.DataSource = SEnvir.ItemInfoList?.Binding;

            GameStoreSaleGridView.OptionsSelection.MultiSelect = true;
            GameStoreSaleGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
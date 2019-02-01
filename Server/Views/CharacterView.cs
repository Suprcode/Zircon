using DevExpress.XtraEditors;
using Server.Envir;

namespace Server.Views
{
    public partial class CharacterView : XtraForm
    {
        public CharacterView()
        {
            InitializeComponent();

            CharacterGridControl.DataSource = SEnvir.CharacterInfoList?.Binding;
            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;

            CharacterGridView.OptionsSelection.MultiSelect = true;
            CharacterGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
        }
    }
}
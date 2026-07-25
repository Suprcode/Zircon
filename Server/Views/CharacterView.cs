using Server.Envir;

namespace Server.Views
{
    public partial class CharacterView : DevExpress.XtraBars.Ribbon.RibbonForm
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

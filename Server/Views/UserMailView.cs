using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class UserMailView : DevExpress.XtraEditors.XtraForm
    {
        public UserMailView()
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.MailInfoList?.Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}
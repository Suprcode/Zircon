using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Server.Diagnostics;
using Server.Envir;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class OrphanDiagnosticView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private readonly BindingList<OrphanDiagnostic.OrphanTypeResult> _results = new BindingList<OrphanDiagnostic.OrphanTypeResult>();

        public OrphanDiagnosticView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DiagnosticGridView.OptionsSelection.MultiSelect = true;
            DiagnosticGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            DiagnosticGridControl.DataSource = _results;
            DiagnosticGridView.OptionsBehavior.Editable = false;
            DiagnosticGridView.OptionsBehavior.ReadOnly = true;
        }

        private void ScanOrphansButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            RunScan(cleanRun: false);
        }

        private void CleanOrphansButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show(this,
                "This will mark all cleanable orphan aggregate-child rows as temporary so they are skipped on the next database save. Continue?",
                "Clean DB orphans",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            RunScan(cleanRun: true);
        }

        private void RunScan(bool cleanRun)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                OrphanDiagnostic.ScanResult result = cleanRun
                    ? OrphanDiagnostic.MarkTemporaryOnCleanableOrphans()
                    : OrphanDiagnostic.Scan();

                _results.Clear();
                foreach (OrphanDiagnostic.OrphanTypeResult row in result.Results)
                    _results.Add(row);

                string log = OrphanDiagnostic.FormatLog(result, cleanRun);
                memoEdit1.EditValue = log;

                DiagnosticGridView.BestFitColumns();
            }
            catch (Exception ex)
            {
                string message = cleanRun ? "DB orphan clean failed: " + ex.Message : "DB orphan scan failed: " + ex.Message;
                memoEdit1.EditValue = message;
                SEnvir.Log(message);
                XtraMessageBox.Show(this, message, "DB Orphans", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}

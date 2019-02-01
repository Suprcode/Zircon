using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.Network;
using Library.SystemModels;

namespace Server.Views
{
    public partial class DiagnosticView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private BindingList<DiagnosticValue> Results;

        public DiagnosticView()
        {
            InitializeComponent();

            Results = new BindingList<DiagnosticValue>();

            foreach (KeyValuePair<string, DiagnosticValue> pair in BaseConnection.Diagnostics)
                Results.Add(pair.Value);

            DiagnosticGridControl.DataSource = Results;

            DiagnosticButton.Down = BaseConnection.Monitor;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DiagnosticGridView.OptionsSelection.MultiSelect = true;
            DiagnosticGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
        

        private void DiagnosticButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            BaseConnection.Monitor = DiagnosticButton.Down;
        }

        private void ResetTimeButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (DiagnosticValue result in Results)
            {
                result.Count = 0;
                result.TotalSize = 0;
                result.LargestSize = 0;
                result.TotalTime = TimeSpan.Zero;
                result.LargestTime = TimeSpan.Zero;
            }
        }
    }

}
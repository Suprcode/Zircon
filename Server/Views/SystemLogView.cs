using System;
using System.ComponentModel;
using DevExpress.XtraBars;
using Server.Envir;

namespace Server.Views
{
    public partial class SystemLogView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static BindingList<string> Logs = new BindingList<string>();

        public SystemLogView()
        {
            InitializeComponent();

            LogListBoxControl.DataSource = Logs;
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            while (!SEnvir.DisplayLogs.IsEmpty)
            {
                string log;

                if (!SEnvir.DisplayLogs.TryDequeue(out log)) continue;

                Logs.Add(log);
            }

            if (Logs.Count > 0)
                ClearLogsButton.Enabled = true;
        }

        private void ClearLogsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Logs.Clear();
                ClearLogsButton.Enabled = false;
        }
    }
}
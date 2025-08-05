using System;
using System.ComponentModel;
using DevExpress.XtraBars;
using Server.Envir;

namespace Server.Views
{
    public partial class ChatLogView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static BindingList<string> Logs = new BindingList<string>();

        public ChatLogView()
        {
            InitializeComponent();

            LogListBoxControl.DataSource = Logs;
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            while (!SEnvir.ChatAppLogs.IsEmpty) //TODO: not sure if this was a bug or intentional? why was it using SEnvir.ChatLogs instead of ChatDisplayLogs
            {
                string log;

                if (!SEnvir.ChatAppLogs.TryDequeue(out log)) continue;

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
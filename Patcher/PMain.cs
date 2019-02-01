using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace Patcher
{
    public partial class PMain : WaitForm
    {
        public PMain()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion


        private async void PMain_Load(object sender, EventArgs e)
        {
            try
            {
                string patchFrom = Program.Arguments[0];
                string patchTo = Program.Arguments[1];

                SetDescription("Waiting 5 seconds Launcher to close...");

                await Task.Delay(TimeSpan.FromSeconds(5));

                if (File.Exists(patchTo))
                    File.Delete(patchTo);

                SetDescription("Updating Launcher...");

                await Task.Delay(TimeSpan.FromSeconds(3));

                File.Move(patchFrom, patchTo);
                Process.Start(patchTo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Failed.");
            }

            Close();
        }
    }
}
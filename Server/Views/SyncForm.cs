using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class SyncForm : Form
    {
        public SyncForm()
        {
            InitializeComponent();
            txtRemoteIP.Text = Config.SyncRemotePreffix;
            txtKey.Text = Config.SyncKey;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            //TODO!!

            WebClient webClient = new WebClient();
            using (var wc = webClient)
            {
                SMain.Session.Save(true);
                try
                {
                    var content = File.ReadAllBytes(SMain.Session.SystemPath);

                    //wc.UploadData(txtRemoteIP.Text + $"?Type={WebServer.SystemDBSyncCommand}&Key={HttpUtility.UrlEncode(txtKey.Text)}", content);

                    MessageBox.Show("Syncronization completed", "Sync", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}

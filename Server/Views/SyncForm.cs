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
using System.Net.Http;
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
            using var client = new HttpClient();

            var content = new ByteArrayContent(File.ReadAllBytes(SMain.Session.SystemPath));

            string key = Uri.EscapeDataString(txtKey.Text);

            var url = $"{txtRemoteIP.Text}?Type={WebServer.SystemDBSyncCommand}&Key={key}";
            try
            {
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();
                MessageBox.Show("Syncronization completed", "Sync", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using Server.Envir;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class DatabaseEncryptionForm : Form
    {
        public DatabaseEncryptionForm()
        {
            InitializeComponent();
        }

        private void DatabaseEncryptionForm_Load(object sender, EventArgs e)
        {
            chkEnabled.Checked = Config.EncryptionEnabled;
            txtEncryptionKey.Text = Config.EncryptionKey;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var key = Convert.FromBase64String(txtEncryptionKey.Text);
                if (key.Length != 32)
                {
                    MessageBox.Show("Encryption key is not a valid, expected 32 bytes", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Encryption key is not a valid base64", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Config.EncryptionEnabled = chkEnabled.Checked;
            Config.EncryptionKey = txtEncryptionKey.Text;
            Library.Encryption.SetKey(Config.EncryptionKey);
            SMain.Session.Save(true);

            MessageBox.Show("Changes saved OK!");

            this.Close();
        }

        private void btnGenerateRandomKey_Click(object sender, EventArgs e)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] random = new byte[32];
            rng.GetBytes(random);
            txtEncryptionKey.Text = Convert.ToBase64String(random);
        }
    }
}

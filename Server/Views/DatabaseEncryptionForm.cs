using Library.SystemModels;
using MirDB;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class DatabaseEncryptionForm : DevExpress.XtraEditors.XtraForm
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
                if (chkEnabled.Checked)
                {
                    var key = Convert.FromBase64String(txtEncryptionKey.Text);

                    if (key.Length != 32)
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                AlertControl.Show(this, "Validation", "Encryption key is not a valid base64", false);
                return;
            }

            Config.EncryptionEnabled = chkEnabled.Checked;
            Config.EncryptionKey = txtEncryptionKey.Text;

            var session = new Session(SessionMode.Both)
            {
                BackUpDelay = 60
            };

            session.Initialize(
                Assembly.GetAssembly(typeof(ItemInfo)),
                Assembly.GetAssembly(typeof(AccountInfo))
            );

            Library.Encryption.SetKey(Config.EncryptionEnabled ? Convert.FromBase64String(Config.EncryptionKey) : null);

            session.Save(true);

            this.Close();
        }

        private void btnGenerateRandomKey_Click(object sender, EventArgs e)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] random = new byte[32];
                rng.GetBytes(random);
                txtEncryptionKey.Text = Convert.ToBase64String(random);
            }
        }
    }
}

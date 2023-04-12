using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class CaptionDialog : DXWindow
    {
        #region Properties

        #region CaptionChanged

        public bool CaptionValid
        {
            get => _CaptionValid;
            set
            {
                if (_CaptionValid == value) return;

                bool oldValue = _CaptionValid;
                _CaptionValid = value;

                OnCaptionValidChanged(oldValue, value);
            }
        }
        private bool _CaptionValid;
        public event EventHandler<EventArgs> CaptionValidChanged;
        public void OnCaptionValidChanged(bool oValue, bool nValue)
        {
            CaptionValidChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion CaptionChanged

        public DXControl ClientPanel;
        public DXButton ChangeButton;
        public DXTextBox CaptionText;
        public DXLabel label, CaptionHelp;
        public override WindowType Type => WindowType.CaptionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public CaptionDialog()
        {
            TitleLabel.Text = @"Caption";

            HasFooter = true;
            Movable = true;
            Border = true;

            SetClientSize(new Size(325, 50));
            label = new DXLabel
            {
                Parent = this,
                Text = "Caption:",
            };

            CaptionText = new DXTextBox
            {
                Parent = this,
                Size = new Size(180, 20),
                MaxLength = 40,
            };
            CaptionText.TextBox.TextChanged += CaptionText_TextChanged;
            CaptionText.TextBox.KeyPress += TextBox_KeyPress;

            CaptionHelp = new DXLabel
            {
                Parent = this,
                Text = "[?]",
                Hint = $"Caption.\nAccepted characters: a-z A-Z 0-9.\nLength: between {Globals.MinCaptionLength} and {Globals.MaxCaptionLength} characters.\nAvoid harmful and racist words.",
            };

            ChangeButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Parent = this,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "Change" },
            };
            ChangeButton.MouseClick += (o, e) =>
            {
                if (CaptionValid && !String.IsNullOrWhiteSpace(CaptionText.TextBox.Text))
                {
                    CEnvir.Enqueue(new C.CaptionChange { Caption = CaptionText.TextBox.Text });
                    Visible = false;
                }
            };

            label.Location = new Point(ClientArea.Location.X, ClientArea.Y + 15);
            CaptionText.Location = new Point(ClientArea.Location.X + label.Size.Width + 10, ClientArea.Y + 15);
            CaptionHelp.Location = new Point(ClientArea.Location.X + CaptionText.Size.Width + CaptionHelp.Size.Width + 40, ClientArea.Y + 15 - 2);
            ChangeButton.Location = new Point(ClientArea.Location.X + CaptionText.Size.Width + ChangeButton.Size.Width + 24, ClientArea.Y + 15 - 2);

        }

        private void CaptionText_TextChanged(object sender, EventArgs e)
        {
            CaptionValid = Globals.CharacterReg.IsMatch(CaptionText.TextBox.Text);

            if (string.IsNullOrEmpty(CaptionText.TextBox.Text))
                CaptionText.BorderColour = Color.FromArgb(198, 166, 99);
            else
                CaptionText.BorderColour = CaptionValid ? Color.Green : Color.Red;

        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                if (label != null)
                {
                    if (!label.IsDisposed)
                        label.Dispose();

                    label = null;
                }

                if (ChangeButton != null)
                {
                    if (!ChangeButton.IsDisposed)
                        ChangeButton.Dispose();

                    ChangeButton = null;
                }

                if (CaptionText != null)
                {
                    if (!CaptionText.IsDisposed)
                        CaptionText.Dispose();

                    CaptionText = null;
                }
            }

        }

        #endregion
    }
}
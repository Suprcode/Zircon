using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Client.UserModels;

//Cleaned
namespace Client.Controls
{
    public sealed class DXInputWindow : DXWindow
    {
        #region Properites
        public DXLabel Label;

        public DXButton ConfirmButton, CancelButton;
        public DXTextBox ValueTextBox;

        public string Value; //Don't Dispose

        public override WindowType Type => WindowType.InputWindow;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public DXInputWindow( string message, string caption)
        {
            HasFooter = true;

            TitleLabel.Text = caption;

            Parent = ActiveScene;
            MessageBoxList.Add(this);


            Label = new DXLabel
            {
                AutoSize = false,
                Location = new Point(10, 35),
                Parent = this,
                Text = message,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak |TextFormatFlags.HorizontalCenter
            };
            Label.Size = new Size(300, DXLabel.GetHeight(Label, 300).Height);

            ValueTextBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(200, 20),
                Location = new Point(60, 45 + Label.Size.Height),
                KeepFocus = true,
            };
            ValueTextBox.SetFocus();
            ValueTextBox.TextBox.TextChanged += TextBox_TextChanged;
            ValueTextBox.TextBox.KeyPress += (o, e) => OnKeyPress(e);


            SetClientSize(new Size(300, Label.Size.Height + 30));
            Label.Location = ClientArea.Location;

            ConfirmButton = new DXButton
            {
                Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlConfirm }
            };
            ConfirmButton.MouseClick += (o, e) => Dispose();

            CancelButton = new DXButton
            {
                Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlCancel }
            };
            CancelButton.MouseClick += (o, e) => Dispose();

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);

        }

        #region Methods
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = true;
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (e.KeyChar)
            {
                case (char)Keys.Escape:
                    if (CancelButton != null && !CancelButton.IsDisposed)
                        CancelButton.InvokeMouseClick();
                    break;
                case (char)Keys.Enter:
                    if (ConfirmButton != null && !ConfirmButton.IsDisposed)
                        ConfirmButton.InvokeMouseClick();
                    break;
                default: return;
            }
            e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            Value = ValueTextBox.TextBox.Text;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();
                    ConfirmButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();
                    CancelButton = null;
                }

                if (ValueTextBox != null)
                {
                    if (!ValueTextBox.IsDisposed)
                        ValueTextBox.Dispose();
                    ValueTextBox = null;
                }
            }

        }

        #endregion
    }
}

using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Client.UserModels;

//Cleaned
namespace Client.Controls
{
    public sealed class DXMessageBox : DXWindow
    {
        #region Properties
        public DXLabel Label { get; private set; }
        public DXButton OKButton, CancelButton, NoButton, YesButton;
        public DXMessageBoxButtons Buttons;

        public DXTextBox HiddenBox;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);


            if (Parent != null)
                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);

            if (Label != null)
                Label.Location = ClientArea.Location;

            if (OKButton != null)
                OKButton.Location = new Point(OKButton.Location.X, Size.Height - 44);
            if (CancelButton != null)
                CancelButton.Location = new Point(CancelButton.Location.X, Size.Height - 44);
            if (NoButton != null)
                NoButton.Location = new Point(NoButton.Location.X, Size.Height - 44);
            if (YesButton != null)
                YesButton.Location = new Point(YesButton.Location.X, Size.Height - 44);

        }
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (Parent == null) return;

            Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                HiddenBox = DXTextBox.ActiveTextBox;
                DXTextBox.ActiveTextBox = null;
            }
            else if (HiddenBox != null)
                DXTextBox.ActiveTextBox = HiddenBox;

        }

        public override WindowType Type => WindowType.MessageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public DXMessageBox(string message, string caption, DXMessageBoxButtons buttons = DXMessageBoxButtons.OK)
        {
            Buttons = buttons;
            Modal = true;
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
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            Label.Size = new Size(380, DXLabel.GetSize(message, Label.Font, Label.Outline).Height);
            SetClientSize(Label.Size);
            Label.Location = ClientArea.Location;

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);


            switch (Buttons)
            {
                case DXMessageBoxButtons.OK:
                    OKButton = new DXButton
                    {
                        Location = new Point((Size.Width - 80) / 2, Size.Height - 43),
                        Size = new Size(80, DefaultHeight),
                        Parent = this,
                        Label = { Text = CEnvir.Language.CommonControlOk }
                    };
                    OKButton.MouseClick += (o, e) => Dispose();
                    break;
                case DXMessageBoxButtons.YesNo:
                    YesButton = new DXButton
                    {
                        Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                        Size = new Size(80, DefaultHeight),
                        Parent = this,
                        Label = { Text = CEnvir.Language.CommonControlYes }
                    };
                    YesButton.MouseClick += (o, e) => Dispose();
                    NoButton = new DXButton
                    {
                        Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                        Size = new Size(80, DefaultHeight),
                        Parent = this,
                        Label = { Text = CEnvir.Language.CommonControlNo }
                    };
                    NoButton.MouseClick += (o, e) => Dispose();
                    break;
                case DXMessageBoxButtons.Cancel:
                    CancelButton = new DXButton
                    {
                        Location = new Point((Size.Width - 80) / 2, Size.Height - 43),
                        Size = new Size(80, DefaultHeight),
                        Parent = this,
                        Label = { Text = CEnvir.Language.CommonControlCancel }
                    };
                    CancelButton.MouseClick += (o, e) => Dispose();
                    break;
            }

            BringToFront();
        }

        #region Methods

        public static DXMessageBox Show(string message, string caption, DialogAction action = DialogAction.None)
        {
            DXMessageBox box = new DXMessageBox(message, caption);

            switch (action)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Close:
                    box.OKButton.MouseClick += (o, e) => CEnvir.Target.Close();
                    box.CloseButton.MouseClick += (o, e) => CEnvir.Target.Close();
                    break;
                case DialogAction.ReturnToLogin:
                    box.OKButton.MouseClick += (o, e) => CEnvir.ReturnToLogin();
                    box.CloseButton.MouseClick += (o, e) => CEnvir.ReturnToLogin();
                    break;
            }

            return box;
        }
        
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)
            {
                switch (Buttons)
                {
                    case DXMessageBoxButtons.OK:
                        if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick();
                        break;
                    case DXMessageBoxButtons.YesNo:
                        if (NoButton != null && !NoButton.IsDisposed) NoButton.InvokeMouseClick();
                        break;
                }
                e.Handled = true;
            }

            else if (e.KeyChar == (char)Keys.Enter)
            {
                switch (Buttons)
                {
                    case DXMessageBoxButtons.OK:
                        if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick();
                        break;
                    case DXMessageBoxButtons.YesNo:
                        if (YesButton != null && !YesButton.IsDisposed) YesButton.InvokeMouseClick();
                        break;

                }
                e.Handled = true;
            }
        }
        public override void ResolutionChanged()
        {
            base.ResolutionChanged();

            if (Parent != null)
                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
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

                if (OKButton != null)
                {
                    if (!OKButton.IsDisposed)
                        OKButton.Dispose();
                    OKButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();
                    CancelButton = null;
                }

                if (NoButton != null)
                {
                    if (!NoButton.IsDisposed)
                        NoButton.Dispose();
                    NoButton = null;
                }

                if (YesButton != null)
                {
                    if (!YesButton.IsDisposed)
                        YesButton.Dispose();
                    YesButton = null;
                }

                if (HiddenBox != null)
                {
                    DXTextBox.ActiveTextBox = HiddenBox;
                    HiddenBox = null;
                }

                Buttons = DXMessageBoxButtons.None;
                MessageBoxList.Remove(this);
            }
        }
        #endregion
    }


    public enum DXMessageBoxButtons
    {
        None,
        OK,
        YesNo,
        Cancel
    }

    public enum DialogAction
    {
        None,
        Close,
        ReturnToLogin
    }
}

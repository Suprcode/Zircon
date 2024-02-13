using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Envir;
using Client.UserModels;

//Cleaned
namespace Client.Controls
{
    public sealed class DXColourControl : DXControl
    {
        #region Properies
        private DXColourPicker Window;

        #region AllowNoColour

        public bool AllowNoColour
        {
            get => _AllowNoColour;
            set
            {
                if (_AllowNoColour == value) return;

                bool oldValue = _AllowNoColour;
                _AllowNoColour = value;

                OnAllowNoColourChanged(oldValue, value);
            }
        }
        private bool _AllowNoColour;
        public event EventHandler<EventArgs> AllowNoColourChanged;
        public void OnAllowNoColourChanged(bool oValue, bool nValue)
        {
            AllowNoColourChanged?.Invoke(this, EventArgs.Empty);

            if (Window != null)
                Window.AllowNoColour = nValue;
        }

        #endregion

        #endregion

        public DXColourControl()
        {
            DrawTexture = true;
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);
            Size = new Size(40, 15);
            BackColour = Color.Black;

            MouseClick += DXColourControl_MouseClick;
        }

        #region Methods
        private void DXColourControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (Window != null)
            {
                if (!Window.IsDisposed)
                    Window.Dispose();
                Window = null;
            }

            Window = new DXColourPicker
            {
                Target = this,
                Parent = ActiveScene,
                PreviousColour = BackColour,
                SelectedColour = BackColour,
                AllowNoColour = AllowNoColour,
            };
            Window.Location = new Point((ActiveScene.Size.Width - Window.Size.Width) / 2, (ActiveScene.Size.Height - Window.Size.Height) / 2);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Window != null)
                {
                    if (!Window.IsDisposed)
                        Window.Dispose();

                    Window = null;
                }
            }
        }
        #endregion
    }

    public sealed class DXColourPicker : DXWindow
    {
        #region Properties
        
        #region SelectedColour

        public Color SelectedColour
        {
            get => _SelectedColour;
            set
            {
                if (_SelectedColour == value) return;

                Color oldValue = _SelectedColour;
                _SelectedColour = value;

                OnSelectedColourChanged(oldValue, value);
            }
        }
        private Color _SelectedColour;
        public event EventHandler<EventArgs> SelectedColourChanged;
        public void OnSelectedColourChanged(Color oValue, Color nValue)
        {
            SelectedColourChanged?.Invoke(this, EventArgs.Empty);

            if (ColourBox != null)
                ColourBox.BackColour = SelectedColour;

            if (SelectedColour == Color.FromArgb(0, 0, 0, 0))
            {
                ColourBox.Visible = false;
                NoColourLabel.Visible = true;
            }
            else
            {
                ColourBox.Visible = true;
                NoColourLabel.Visible = false;
            }

            if (Target != null)
                Target.BackColour = SelectedColour;

            Updating = true;
            RedBox.Value = SelectedColour.R;
            GreenBox.Value = SelectedColour.G;
            BlueBox.Value = SelectedColour.B;
            Updating = false;
        }

        #endregion

        #region AllowNoColour

        public bool AllowNoColour
        {
            get => _AllowNoColour;
            set
            {
                if (_AllowNoColour == value) return;

                bool oldValue = _AllowNoColour;
                _AllowNoColour = value;

                OnAllowNoColourChanged(oldValue, value);
            }
        }
        private bool _AllowNoColour;
        public event EventHandler<EventArgs> AllowNoColourChanged;
        public void OnAllowNoColourChanged(bool oValue, bool nValue)
        {
            AllowNoColourChanged?.Invoke(this, EventArgs.Empty);

            EmptyButton.Visible = nValue;
        }

        #endregion

        public Color PreviousColour;
        public bool Updating;

        public DXButton SelectButton, CancelButton, EmptyButton;
        public DXColourControl Target;
        public DXNumberBox RedBox, GreenBox, BlueBox;
        public DXControl ColourScaleBox;
        public DXControl ColourBox;

        public DXLabel NoColourLabel;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public DXColourPicker()
        {
            Size = new Size(380, 253);
            TitleLabel.Text = CEnvir.Language.CommonControlColourPickerTitle;
            Modal = true;
            HasFooter = true;

            CancelButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlCancel },
                Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
            };
            CancelButton.MouseClick += CancelButton_MouseClick;
            CloseButton.MouseClick += CancelButton_MouseClick;

            SelectButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlSelect },
                Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
            };
            SelectButton.MouseClick += (o, e) => Dispose();

            EmptyButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlColourPickerEmptyLabel },
                Location = new Point((Size.Width) / 2 - 80 - 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Visible = AllowNoColour
            };
            EmptyButton.Location = new Point(Size.Width - EmptyButton.Size.Width - 10, 115);
            EmptyButton.MouseClick += EmptyButton_MouseClick;

            ColourScaleBox = new DXControl
            {
                Location = new Point(20, 40),
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(200, 149)
            };
            AfterDraw += (o, e) =>
            {
                PresentTexture(DXManager.ColourPallete, ColourScaleBox, ColourScaleBox.DisplayArea, Color.White, this);
            };
            ColourScaleBox.MouseClick += ColourScaleBox_MouseClick;

            RedBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            RedBox.Location = new Point(Size.Width - RedBox.Size.Width - 10, 40);
            RedBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.CommonControlColourPickerRedLabel,
            };
            label.Location = new Point(RedBox.Location.X - label.Size.Width - 5, (RedBox.Size.Height - label.Size.Height) / 2 + RedBox.Location.Y);

            GreenBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            GreenBox.Location = new Point(Size.Width - GreenBox.Size.Width - 10, 65);
            GreenBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.CommonControlColourPickerGreenLabel,
            };
            label.Location = new Point(GreenBox.Location.X - label.Size.Width - 5, (GreenBox.Size.Height - label.Size.Height) / 2 + GreenBox.Location.Y);

            BlueBox = new DXNumberBox
            {
                Change = 5,
                MaxValue = 255,
                Parent = this,
            };
            BlueBox.Location = new Point(Size.Width - BlueBox.Size.Width - 10, 90);
            BlueBox.ValueTextBox.ValueChanged += ColourBox_ValueChanged;

            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.CommonControlColourPickerBlueLabel,
            };
            label.Location = new Point(BlueBox.Location.X - label.Size.Width - 5, (BlueBox.Size.Height - label.Size.Height) / 2 + BlueBox.Location.Y);

            ColourBox = new DXControl
            {
                Size = BlueBox.ValueTextBox.Size,
                Location = new Point(BlueBox.Location.X + BlueBox.ValueTextBox.Location.X, 172),
                BackColour = SelectedColour,
                Border = true,
                DrawTexture = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Parent = this,
                Visible = SelectedColour != Color.Empty
            };
            label = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.CommonControlColourPickerColourLabel,
            };
            label.Location = new Point(BlueBox.Location.X - label.Size.Width - 5, (ColourBox.Size.Height - label.Size.Height) / 2 + ColourBox.Location.Y);

            NoColourLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(BlueBox.Location.X + BlueBox.ValueTextBox.Location.X, 172),
                Text = CEnvir.Language.CommonControlColourPickerNoneLabel,
                Visible = SelectedColour == Color.Empty
            };
        }

        #region Methods
        private void ColourScaleBox_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X - ColourScaleBox.DisplayArea.X;
            int y = e.Y - ColourScaleBox.DisplayArea.Y;

            if (x < 0 || y < 0 || x >= 200 || y >= 149) return;

            SelectedColour = Color.FromArgb(DXManager.PalleteData[(y * 200 + x) * 4 + 2], DXManager.PalleteData[(y * 200 + x) * 4 + 1], DXManager.PalleteData[(y * 200 + x) * 4]);
        }
        private void CancelButton_MouseClick(object sender, MouseEventArgs e)
        {
            Target.BackColour = PreviousColour;
            Dispose();
        }
        private void ColourBox_ValueChanged(object sender, EventArgs e)
        {
            if (Updating) return;

            SelectedColour = Color.FromArgb((int) RedBox.Value, (int) GreenBox.Value, (int) BlueBox.Value);
        }

        private void EmptyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (Updating) return;

            SelectedColour = Color.FromArgb(0, 0, 0, 0);
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Updating = false;
                _SelectedColour = Color.Empty;
                SelectedColourChanged = null;
                PreviousColour = Color.Empty;

                if (SelectButton != null)
                {
                    if (!SelectButton.IsDisposed)
                        SelectButton.Dispose();

                    SelectButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                if (EmptyButton != null)
                {
                    if (!EmptyButton.IsDisposed)
                        EmptyButton.Dispose();

                    EmptyButton = null;
                }

                Target = null;

                if (RedBox != null)
                {
                    if (!RedBox.IsDisposed)
                        RedBox.Dispose();

                    RedBox = null;
                }

                if (GreenBox != null)
                {
                    if (!GreenBox.IsDisposed)
                        GreenBox.Dispose();

                    GreenBox = null;
                }

                if (BlueBox != null)
                {
                    if (!BlueBox.IsDisposed)
                        BlueBox.Dispose();

                    BlueBox = null;
                }

                if (ColourScaleBox != null)
                {
                    if (!ColourScaleBox.IsDisposed)
                        ColourScaleBox.Dispose();

                    ColourScaleBox = null;
                }

                if (ColourBox != null)
                {
                    if (!ColourBox.IsDisposed)
                        ColourBox.Dispose();

                    ColourBox = null;
                }

                if (NoColourLabel != null)
                {
                    if (!NoColourLabel.IsDisposed)
                        NoColourLabel.Dispose();

                    NoColourLabel = null;
                }
            }
        }
        #endregion
    }
}

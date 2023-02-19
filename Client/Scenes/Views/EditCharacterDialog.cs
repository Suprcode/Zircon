using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class EditCharacterDialog : DXWindow
    {
        #region Properties

        #region SelectedClass

        public MirClass SelectedClass
        {
            get => _SelectedClass;
            set
            {
                if (_SelectedClass == value) return;

                MirClass oldValue = _SelectedClass;
                _SelectedClass = value;

                OnSelectedClassChanged(oldValue, value);
            }
        }
        private MirClass _SelectedClass;
        public event EventHandler<EventArgs> SelectedClassChanged;
        public void OnSelectedClassChanged(MirClass oValue, MirClass nValue)
        {
            SelectedClassChanged?.Invoke(this, EventArgs.Empty);

            switch (SelectedClass)
            {
                case MirClass.Warrior:
                    WarriorButton.Index = 120;
                    WarriorButton.Pressed = true;
                    break;
                case MirClass.Wizard:
                    WizardButton.Index = 125;
                    WizardButton.Pressed = true;
                    break;
                case MirClass.Taoist:
                    TaoistButton.Index = 130;
                    TaoistButton.Pressed = true;
                    break;
                case MirClass.Assassin:
                    AssassinButton.Index = 135;
                    AssassinButton.Pressed = true;
                    ArmourColour.Enabled = false;
                    ArmourColourLabel.Enabled = false;
                    HairNumberBox.MaxValue = 5;
                    break;
            }

            switch (oValue)
            {
                case MirClass.Warrior:
                    WarriorButton.Index = 121;
                    WarriorButton.Pressed = false;
                    break;
                case MirClass.Wizard:
                    WizardButton.Index = 126;
                    WizardButton.Pressed = false;
                    break;
                case MirClass.Taoist:
                    TaoistButton.Index = 131;
                    TaoistButton.Pressed = false;
                    break;
                case MirClass.Assassin:
                    AssassinButton.Index = 136;
                    AssassinButton.Pressed = false;
                    ArmourColour.Enabled = true;
                    ArmourColourLabel.Enabled = true;
                    HairNumberBox.MaxValue = SelectedGender == MirGender.Male ? 10 : 11;
                    break;
            }

            SelectedClassLabel.Text = SelectedClass.ToString();
        }

        #endregion

        #region SelectedGender

        public MirGender SelectedGender
        {
            get => _SelectedGender;
            set
            {
                if (_SelectedGender == value) return;

                MirGender oldValue = _SelectedGender;
                _SelectedGender = value;

                OnSelectedGenderChanged(oldValue, value);
            }
        }
        private MirGender _SelectedGender;
        public event EventHandler<EventArgs> SelectedGenderChanged;
        public void OnSelectedGenderChanged(MirGender oValue, MirGender nValue)
        {
            SelectedGenderChanged?.Invoke(this, EventArgs.Empty);

            switch (SelectedGender)
            {
                case MirGender.Male:
                    MaleButton.Index = 115;
                    MaleButton.Pressed = true;
                    HairNumberBox.MaxValue = SelectedClass == MirClass.Assassin ? 5 : 10;
                    break;
                case MirGender.Female:
                    FemaleButton.Index = 110;
                    FemaleButton.Pressed = true;
                    HairNumberBox.MaxValue = SelectedClass == MirClass.Assassin ? 5 : 11;
                    break;
            }

            switch (oValue)
            {
                case MirGender.Male:
                    MaleButton.Index = 116;
                    MaleButton.Pressed = false;
                    break;
                case MirGender.Female:
                    FemaleButton.Index = 111;
                    FemaleButton.Pressed = false;
                    break;

            }
            SelectedGenderLabel.Text = SelectedGender.ToString();
        }

        #endregion

        #region CharacterNameValid

        public bool CharacterNameValid
        {
            get => _CharacterNameValid;
            set
            {
                if (_CharacterNameValid == value) return;

                bool oldValue = _CharacterNameValid;
                _CharacterNameValid = value;

                OnCharacterNameValidChanged(oldValue, value);
            }
        }
        private bool _CharacterNameValid;
        public event EventHandler<EventArgs> CharacterNameValidChanged;
        public void OnCharacterNameValidChanged(bool oValue, bool nValue)
        {
            CharacterNameValidChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region Change

        public ChangeType Change
        {
            get { return _Change; }
            set
            {
                if (_Change == value) return;

                ChangeType oldValue = _Change;
                _Change = value;

                OnChangeChanged(oldValue, value);
            }
        }
        private ChangeType _Change;
        public event EventHandler<EventArgs> ChangeChanged;
        public void OnChangeChanged(ChangeType oValue, ChangeType nValue)
        {
            ChangeChanged?.Invoke(this, EventArgs.Empty);

            MaleButton.Enabled = false;
            FemaleButton.Enabled = false;

            HairNumberBox.Enabled = false;
            HairTypeLabel.Enabled = false;

            HairColour.Enabled = false;
            HairColourLabel.Enabled = false;

            ArmourColour.Enabled = false;
            ArmourColourLabel.Enabled = false;

            CharacterNameTextBox.Enabled = false;
            CharacterNameTextBoxLabel.Enabled = false;

            switch (Change)
            {
                case ChangeType.GenderChange:
                    MaleButton.Enabled = true;
                    FemaleButton.Enabled = true;

                    HairNumberBox.Enabled = true;
                    HairTypeLabel.Enabled = true;

                    HairColourLabel.Enabled = HairNumberBox.Value > 0;
                    HairColour.Enabled = HairNumberBox.Value > 0;
                    break;
                case ChangeType.HairChange:
                    HairNumberBox.Enabled = true;
                    HairTypeLabel.Enabled = true;

                    HairColourLabel.Enabled = HairNumberBox.Value > 0;
                    HairColour.Enabled = HairNumberBox.Value > 0;
                    break;
                case ChangeType.ArmourDye:
                    ArmourColour.Enabled = true;
                    ArmourColourLabel.Enabled = true;
                    break;
                case ChangeType.NameChange:
                    CharacterNameTextBox.Enabled = true;
                    CharacterNameTextBoxLabel.Enabled = true;
                    break;
            }

        }

        #endregion
        
        
        public DXLabel SelectedClassLabel, SelectedGenderLabel, HairTypeLabel, HairColourLabel, ArmourColourLabel, CharacterNameHelpLabel, CharacterNameTextBoxLabel;
        public DXTextBox CharacterNameTextBox;
        public DXNumberBox HairNumberBox;
        public DXControl CharacterDisplay, GenderPanel;

        public DXColourControl HairColour, ArmourColour;

        public DXButton ChangeButton,
            WarriorButton,
            WizardButton,
            TaoistButton,
            AssassinButton,
            MaleButton,
            FemaleButton;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public EditCharacterDialog()
        {
            Size = new Size(260, 650 - 90);
            HasFooter = true;
            TitleLabel.Text = "Change";
            CloseButton.MouseClick += (o, e) => Close();

            ChangeButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.CommonControlConfirm },
                Location = new Point((Size.Width - 80) / 2, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
            };
            ChangeButton.MouseClick += (o, e) => Confirm();

            #region Select Class


            DXControl panel = new DXControl
            {
                Parent = this,
                BackColour = Color.FromArgb(72, 36, 36),
                Border = true,
                DrawTexture = true,
                Size = new Size(200, 85),
                Location = new Point(30, 40),
                BorderColour = Color.FromArgb(198, 166, 99),
                Visible = false,
            };

            DXLabel label = new DXLabel
            {
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "Select Class",
            };
            label.Location = new Point((panel.Size.Width - label.Size.Width) / 2, 0);


            WarriorButton = new DXButton
            {
                Index = 120,
                LibraryFile = LibraryFile.Interface1c,
                Pressed = true,
                Parent = panel,
            };
            WarriorButton.MouseClick += (o, e) => SelectedClass = MirClass.Warrior;
            int offset = (panel.Size.Width - WarriorButton.Size.Width * 4) / 5;
            WarriorButton.Location = new Point(offset, 22);

            WizardButton = new DXButton
            {
                Index = 126,
                LibraryFile = LibraryFile.Interface1c,
                Location = new Point(90, 50),
                Parent = panel,
            };
            WizardButton.Location = new Point(offset * 2 + WarriorButton.Size.Width, 22);
            WizardButton.MouseClick += (o, e) => SelectedClass = MirClass.Wizard;

            TaoistButton = new DXButton
            {
                Index = 131,
                LibraryFile = LibraryFile.Interface1c,
                Location = new Point(130, 50),
                Parent = panel,
            };
            TaoistButton.Location = new Point(offset * 3 + WarriorButton.Size.Width * 2, 22);
            TaoistButton.MouseClick += (o, e) => SelectedClass = MirClass.Taoist;

            AssassinButton = new DXButton
            {
                Index = 136,
                LibraryFile = LibraryFile.Interface1c,
                Location = new Point(170, 50),
                Parent = panel,
            };
            AssassinButton.Location = new Point(offset * 4 + WarriorButton.Size.Width * 3, 22);
            AssassinButton.MouseClick += (o, e) => SelectedClass = MirClass.Assassin;

            SelectedClassLabel = new DXLabel
            {
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(80, 15),
                Parent = panel,
                Text = "Warrior",
                BackColour = Color.FromArgb(16, 8, 8),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)

            };
            SelectedClassLabel.Location = new Point((panel.Size.Width - SelectedClassLabel.Size.Width) / 2, panel.Size.Height - SelectedClassLabel.Size.Height - 5);

            #endregion

            #region Select Gender

            panel = new DXControl
            {
                Parent = this,
                BackColour = Color.FromArgb(72, 36, 36),
                Border = true,
                DrawTexture = true,
                Size = new Size(200, 85),
                Location = new Point(30, 135 - 90),
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            label = new DXLabel
            {
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "Select Gender",
            };
            label.Location = new Point((panel.Size.Width - label.Size.Width) / 2, 0);

            MaleButton = new DXButton
            {
                Index = 115,
                LibraryFile = LibraryFile.Interface1c,
                Parent = panel,
                Pressed = true,
                Location = WizardButton.Location,
            };
            MaleButton.MouseClick += (o, e) => SelectedGender = MirGender.Male;


            FemaleButton = new DXButton
            {
                Index = 111,
                LibraryFile = LibraryFile.Interface1c,
                Location = TaoistButton.Location,
                Parent = panel,
            };
            FemaleButton.MouseClick += (o, e) => SelectedGender = MirGender.Female;


            SelectedGenderLabel = new DXLabel
            {
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(80, 15),
                Parent = panel,
                Text = "Male",
                BackColour = Color.FromArgb(16, 8, 8),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)

            };
            SelectedGenderLabel.Location = new Point((panel.Size.Width - SelectedGenderLabel.Size.Width) / 2, panel.Size.Height - SelectedGenderLabel.Size.Height - 5);

            #endregion

            #region Customization


            panel = new DXControl
            {
                Parent = this,
                BackColour = Color.FromArgb(72, 36, 36),
                Border = true,
                DrawTexture = true,
                Size = new Size(200, 330),
                Location = new Point(30, 230 - 90),
                BorderColour = Color.FromArgb(198, 166, 99),
            };
            label = new DXLabel
            {
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "Customization",
            };
            label.Location = new Point((panel.Size.Width - label.Size.Width) / 2, 0);

            HairNumberBox = new DXNumberBox
            {
                Change = 1,
                MaxValue = 10,
                Parent = panel,
                Location = new Point(90, 25),
            };
            HairNumberBox.ValueTextBox.ValueChanged += (o, e) =>
            {
                HairColourLabel.Enabled = HairNumberBox.Value > 0;
                HairColour.Enabled = HairNumberBox.Value > 0;
            };

            HairTypeLabel = new DXLabel
            {
                Parent = panel,
                Text = "Hair Type:",
            };
            HairTypeLabel.Location = new Point(HairNumberBox.Location.X - HairTypeLabel.Size.Width - 5, (HairNumberBox.Size.Height - HairTypeLabel.Size.Height) / 2 + HairNumberBox.Location.Y);

            HairColour = new DXColourControl
            {
                Parent = panel,
                Size = HairNumberBox.ValueTextBox.Size,
                Location = new Point(HairNumberBox.Location.X + HairNumberBox.ValueTextBox.Location.X, 50),
                BackColour = Color.FromArgb(CEnvir.Random.Next(256), CEnvir.Random.Next(256), CEnvir.Random.Next(256))
            };
            HairColourLabel = new DXLabel
            {
                Parent = panel,
                Text = "Hair Colour:",
            };
            HairColourLabel.Location = new Point(HairNumberBox.Location.X - HairColourLabel.Size.Width - 5, (HairColour.Size.Height - HairColourLabel.Size.Height) / 2 + HairColour.Location.Y);

            ArmourColour = new DXColourControl
            {
                Parent = panel,
                Size = HairNumberBox.ValueTextBox.Size,
                Location = new Point(HairNumberBox.Location.X + HairNumberBox.ValueTextBox.Location.X, 75),
                BackColour = Color.FromArgb(CEnvir.Random.Next(256), CEnvir.Random.Next(256), CEnvir.Random.Next(256))
            };
            ArmourColourLabel = new DXLabel
            {
                Parent = panel,
                Text = "Armour Colour:",
            };
            ArmourColourLabel.Location = new Point(HairNumberBox.Location.X - ArmourColourLabel.Size.Width - 5, (ArmourColour.Size.Height - ArmourColourLabel.Size.Height) / 2 + ArmourColour.Location.Y);


            DXControl previewPanel = new DXControl
            {
                Parent = panel,
                BackColour = Color.FromArgb(49, 40, 24),
                Border = true,
                DrawTexture = true,
                Size = new Size(190, panel.Size.Height - 5 - 100),
                Location = new Point(5, 100),
                BorderColour = Color.FromArgb(198, 166, 99),
            };
            previewPanel.AfterDraw += PreviewPanel_AfterDraw;

            label = new DXLabel
            {
                Parent = previewPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "Preview",
            };
            label.Location = new Point((panel.Size.Width - label.Size.Width) / 2, 0);

            #endregion

            CharacterNameTextBox = new DXTextBox
            {
                Location = new Point(75, 570 - 90),
                Parent = this,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(155, 20),
            };
            CharacterNameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
            CharacterNameTextBox.TextBox.GotFocus += (o, e) => CharacterNameHelpLabel.Visible = true;
            CharacterNameTextBox.TextBox.LostFocus += (o, e) => CharacterNameHelpLabel.Visible = false;
            CharacterNameTextBox.TextBox.KeyPress += TextBox_KeyPress;

            CharacterNameTextBoxLabel = new DXLabel
            {
                Parent = this,
                Text = "Name:",
            };
            CharacterNameTextBoxLabel.Location = new Point(CharacterNameTextBox.Location.X - CharacterNameTextBoxLabel.Size.Width - 5, (CharacterNameTextBox.Size.Height - CharacterNameTextBoxLabel.Size.Height) / 2 + CharacterNameTextBox.Location.Y);

            CharacterNameHelpLabel = new DXLabel
            {
                Visible = false,
                Parent = this,
                Text = "[?]",
                Hint = $"Character Name.\nAccepted characters: a-z A-Z 0-9.\nLength: between {Globals.MinCharacterNameLength} and {Globals.MaxCharacterNameLength} characters.\nCan use previous names on same account.",
            };
            CharacterNameHelpLabel.Location = new Point(CharacterNameTextBox.Location.X + CharacterNameTextBox.Size.Width + 2, (CharacterNameTextBox.Size.Height - CharacterNameHelpLabel.Size.Height) / 2 + CharacterNameTextBox.Location.Y);



            HairNumberBox.Value = 1;
        }

        #region Method

        public void Confirm()
        {
            switch (Change)
            {
                case ChangeType.GenderChange:
                    if (SelectedGender == GameScene.Game.User.Gender)
                    {
                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.CharacterSameGender, SelectedGender), MessageType.System);
                        return;
                    }

                    CEnvir.Enqueue(new C.GenderChange { Gender = SelectedGender, HairType = (int)HairNumberBox.Value, HairColour = HairColour.Enabled ? HairColour.BackColour : Color.Empty });
                    break;
                case ChangeType.HairChange:
                    CEnvir.Enqueue(new C.HairChange { HairType = (int)HairNumberBox.Value, HairColour = HairColour.Enabled ? HairColour.BackColour : Color.Empty });
                    break;
                case ChangeType.ArmourDye:
                    CEnvir.Enqueue(new C.ArmourDye { ArmourColour = ArmourColour.BackColour });
                    break;
                case ChangeType.NameChange:
                    CEnvir.Enqueue(new C.NameChange { Name = CharacterNameTextBox.TextBox.Text });
                    break;

                default:
                    return;
            }

            Visible = false;
        }
        public void Clear()
        {
            SelectedClass = MirClass.Warrior;
            SelectedGender = MirGender.Male;
            CharacterNameTextBox.TextBox.Text = string.Empty;
            HairNumberBox.Value = 1;

            Close();
        }
        private void Close()
        {
            SelectScene scene = ActiveScene as SelectScene;

            if (scene == null) return;

            Visible = false;
            scene.SelectBox.Visible = true;
            scene.CharacterAnimation.Visible = scene.SelectBox.CharacterList.Count > 0;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;
        }
        private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CharacterNameValid = Globals.CharacterReg.IsMatch(CharacterNameTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(CharacterNameTextBox.TextBox.Text))
                CharacterNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                CharacterNameTextBox.BorderColour = CharacterNameValid ? Color.Green : Color.Red;

        }
        private void PreviewPanel_AfterDraw(object sender, EventArgs e)
        {
            //scaling shit
            DXControl panel = (DXControl)sender;
            MirLibrary lib;

            float x = panel.DisplayArea.Location.X;
            float y = panel.DisplayArea.Location.Y;

            int armour, hair;

            DXItemCell[] Grid = GameScene.Game.CharacterBox.Grid;


            if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib))
            {
                switch (SelectedGender)
                {
                    case MirGender.Male:
                        lib.Draw(0, x + 70, y + 160, Color.White, true, 1f, ImageType.Image);
                        break;
                    case MirGender.Female:
                        lib.Draw(1, x + 70, y + 160, Color.White, true, 1F, ImageType.Image);
                        break;
                    default:
                        return;
                }
            }

            switch (SelectedClass)
            {
                case MirClass.Warrior:
                case MirClass.Wizard:
                case MirClass.Taoist:
                    armour = Grid[(int)EquipmentSlot.Armour].Item?.Info.Image ?? 0;
                    switch (SelectedGender)
                    {
                        case MirGender.Male:
                            hair = 60;
                            break;
                        case MirGender.Female:
                            hair = 80;
                            break;
                        default:
                            return;
                    }
                    break;
                case MirClass.Assassin:
                    armour = Grid[(int)EquipmentSlot.Armour].Item?.Info.Image ?? 0;
                    switch (SelectedGender)
                    {
                        case MirGender.Male:
                            hair = 1100;
                            break;
                        case MirGender.Female:
                            hair = 1120;
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }

            if (SelectedClass == MirClass.Assassin && SelectedGender == MirGender.Female && HairNumberBox.Value == 1)
            {

                if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib))
                    lib.Draw(1160, x + 70, y + 160, HairColour.BackColour, true, 1F, ImageType.Image);
            }

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out lib))
            {
                lib.Draw(armour, x + 70, y + 160, Color.White, true, 1F, ImageType.Image);
                lib.Draw(armour, x + 70, y + 160, ArmourColour.BackColour, true, 1F, ImageType.Overlay);
            }


            if (HairNumberBox.Value == 0) return;

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib))
                lib.Draw(hair + (int)HairNumberBox.Value - 1, x + 70, y + 160, HairColour.BackColour, true, 1F, ImageType.Overlay);

        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedClass = MirClass.Warrior;
                _SelectedGender = MirGender.Male;
                _CharacterNameValid = false;

                if (SelectedClassLabel != null)
                {
                    if (!SelectedClassLabel.IsDisposed)
                        SelectedClassLabel.Dispose();

                    SelectedClassLabel = null;
                }

                if (SelectedGenderLabel != null)
                {
                    if (!SelectedGenderLabel.IsDisposed)
                        SelectedGenderLabel.Dispose();

                    SelectedGenderLabel = null;
                }

                if (HairColourLabel != null)
                {
                    if (!HairColourLabel.IsDisposed)
                        HairColourLabel.Dispose();

                    HairColourLabel = null;
                }

                if (ArmourColourLabel != null)
                {
                    if (!ArmourColourLabel.IsDisposed)
                        ArmourColourLabel.Dispose();

                    ArmourColourLabel = null;
                }

                if (CharacterNameHelpLabel != null)
                {
                    if (!CharacterNameHelpLabel.IsDisposed)
                        CharacterNameHelpLabel.Dispose();

                    CharacterNameHelpLabel = null;
                }

                if (CharacterNameTextBox != null)
                {
                    if (!CharacterNameTextBox.IsDisposed)
                        CharacterNameTextBox.Dispose();

                    CharacterNameTextBox = null;
                }

                if (HairNumberBox != null)
                {
                    if (!HairNumberBox.IsDisposed)
                        HairNumberBox.Dispose();

                    HairNumberBox = null;
                }

                if (CharacterDisplay != null)
                {
                    if (!CharacterDisplay.IsDisposed)
                        CharacterDisplay.Dispose();

                    CharacterDisplay = null;
                }

                if (HairColour != null)
                {
                    if (!HairColour.IsDisposed)
                        HairColour.Dispose();

                    HairColour = null;
                }

                if (ArmourColour != null)
                {
                    if (!ArmourColour.IsDisposed)
                        ArmourColour.Dispose();

                    ArmourColour = null;
                }

                if (ChangeButton != null)
                {
                    if (!ChangeButton.IsDisposed)
                        ChangeButton.Dispose();

                    ChangeButton = null;
                }

                if (WarriorButton != null)
                {
                    if (!WarriorButton.IsDisposed)
                        WarriorButton.Dispose();

                    WarriorButton = null;
                }

                if (WizardButton != null)
                {
                    if (!WizardButton.IsDisposed)
                        WizardButton.Dispose();

                    WizardButton = null;
                }

                if (TaoistButton != null)
                {
                    if (!TaoistButton.IsDisposed)
                        TaoistButton.Dispose();

                    TaoistButton = null;
                }

                if (AssassinButton != null)
                {
                    if (!AssassinButton.IsDisposed)
                        AssassinButton.Dispose();

                    AssassinButton = null;
                }

                if (MaleButton != null)
                {
                    if (!MaleButton.IsDisposed)
                        MaleButton.Dispose();

                    MaleButton = null;
                }

                if (FemaleButton != null)
                {
                    if (!FemaleButton.IsDisposed)
                        FemaleButton.Dispose();

                    FemaleButton = null;
                }
            }
        }

        #endregion
    }

    public enum ChangeType
    {
        None = 0,
        GenderChange = 1,
        HairChange = 2,
        ArmourDye = 3,
        NameChange = 4,
    }
}

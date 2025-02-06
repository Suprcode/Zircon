using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes
{
    public sealed class SelectScene : DXScene
    {
        #region Properties
        
        public DXConfigWindow ConfigBox;
        public DXButton ConfigButton;

        public SelectDialog SelectBox;
        public NewCharacterDialog CharacterBox;

        public DXAnimatedControl CharacterAnimation;

        #endregion

        public SelectScene(Size size) : base(size)
        {
            DXImageControl background = new DXImageControl
            {
                Index = 50,
                LibraryFile = LibraryFile.Interface1c,
                Parent = this,
            };

            new DXAnimatedControl
            {
                BaseIndex = 2800,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(3),
                FrameCount = 17,
                Parent = background,
                Blend = true,
            };

            new DXAnimatedControl
            {
                BaseIndex = 2900,
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(3),
                FrameCount = 17,
                Parent = background,
                UseOffSet = true,
                Blend = true,
                Location = new Point(20, 25)
            };

            CharacterAnimation = new DXAnimatedControl
            {
                LibraryFile = LibraryFile.Interface1c,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(10),
                FrameCount = 100,
                Parent = background,
                UseOffSet = true,
                Visible = false,
                Location = new Point(450, 200)
            };
            CharacterAnimation.BeforeDraw += CharacterAnimation_BeforeDraw;
            CharacterAnimation.AfterDraw += CharacterAnimation_AfterDraw;


            ConfigButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 116,
                Parent = this,
            };
            ConfigButton.Location = new Point(Size.Width - ConfigButton.Size.Width - 10, 10);
            ConfigButton.MouseClick += (o, e) => ConfigBox.Visible = !ConfigBox.Visible;

            ConfigBox = new DXConfigWindow
            {
                Parent = this,
                Visible = false,
                NetworkTab = { Enabled = false, TabButton = { Visible = false } },
            };
            ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width)/2, (Size.Height - ConfigBox.Size.Height)/2);

            SelectBox = new SelectDialog
            {
                Parent = this,
            };
            SelectBox.Location = new Point((Size.Width/2 - SelectBox.Size.Width)/2, (Size.Height - SelectBox.Size.Height)/2);

            CharacterBox = new NewCharacterDialog
            {
                Parent = this,
            };
            CharacterBox.Location = new Point((Size.Width - CharacterBox.Size.Width)/2, (Size.Height - CharacterBox.Size.Height)/2);

            foreach (DXWindow window in DXWindow.Windows)
                window.LoadSettings();
        }

        #region Methods

        public void UpdateCharacterDisplay()
        {
            SelectInfo selectInfo = SelectBox.SelectedButton?.SelectInfo;
            if (selectInfo == null)
            {
                CharacterAnimation.Visible = false;
                return;
            }

            CharacterAnimation.Loop = false;
            CharacterAnimation.AnimationStart = DateTime.MinValue;
            CharacterAnimation.Animated = true;
            CharacterAnimation.Visible = true;

            switch (selectInfo.Class)
            {
                case MirClass.Warrior:
                    switch (selectInfo.Gender)
                    {
                        case MirGender.Male:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2200);
                            CharacterAnimation.BaseIndex = 240;
                            CharacterAnimation.FrameCount = 22;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(1900);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 300;
                                CharacterAnimation.FrameCount = 13;
                            };
                            break;
                        case MirGender.Female:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2800);
                            CharacterAnimation.BaseIndex = 440;
                            CharacterAnimation.FrameCount = 28;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(1900);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 500;
                                CharacterAnimation.FrameCount = 13;
                            };
                            break;
                    }
                    break;
                case MirClass.Wizard:
                    switch (selectInfo.Gender)
                    {
                        case MirGender.Male:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2000);
                            CharacterAnimation.BaseIndex = 740;
                            CharacterAnimation.FrameCount = 20;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(1500);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 800;
                                CharacterAnimation.FrameCount = 10;
                            };
                            break;
                        case MirGender.Female:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2600);
                            CharacterAnimation.BaseIndex = 940;
                            CharacterAnimation.FrameCount = 26;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2250);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 1000;
                                CharacterAnimation.FrameCount = 15;
                            };
                            break;
                    }
                    break;
                case MirClass.Taoist:
                    switch (selectInfo.Gender)
                    {
                        case MirGender.Male:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2700);
                            CharacterAnimation.BaseIndex = 1240;
                            CharacterAnimation.FrameCount = 27;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2250);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 1300;
                                CharacterAnimation.FrameCount = 15;
                            };
                            break;
                        case MirGender.Female:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2000);
                            CharacterAnimation.BaseIndex = 1440;
                            CharacterAnimation.FrameCount = 20;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(1500);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 1500;
                                CharacterAnimation.FrameCount = 10;
                            };
                            break;
                    }
                    break;
                case MirClass.Assassin:
                    switch (selectInfo.Gender)
                    {
                        case MirGender.Male:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2500);
                            CharacterAnimation.BaseIndex = 1740;
                            CharacterAnimation.FrameCount = 25;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2400);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 1800;
                                CharacterAnimation.FrameCount = 16;
                            };
                            break;
                        case MirGender.Female:
                            CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(2000);
                            CharacterAnimation.BaseIndex = 1940;
                            CharacterAnimation.FrameCount = 20;
                            CharacterAnimation.AfterAnimation += (o, e) =>
                            {
                                CharacterAnimation.AnimationDelay = TimeSpan.FromMilliseconds(1500);
                                CharacterAnimation.Loop = true;
                                CharacterAnimation.AnimationStart = DateTime.MinValue;
                                CharacterAnimation.Animated = true;
                                CharacterAnimation.BaseIndex = 2000;
                                CharacterAnimation.FrameCount = 10;
                            };
                            break;
                    }
                    break;
            }
        }

        private void CharacterAnimation_AfterDraw(object sender, EventArgs e)
        {
            SelectInfo selectInfo = SelectBox.SelectedButton?.SelectInfo;
            if (selectInfo == null)
            {
                CharacterAnimation.Visible = false;
                return;
            }

            if (!CharacterAnimation.Loop)
            {
                DXManager.SetBlend(true);

                MirImage image = CharacterAnimation.Library?.GetImage(CharacterAnimation.Index);

                if (image == null) return;

                int x = CharacterAnimation.DisplayArea.X - image.OffSetX;
                int y = CharacterAnimation.DisplayArea.Y - image.OffSetY;

                image = CharacterAnimation.Library?.CreateImage(CharacterAnimation.Index + 100, ImageType.Image);

                if (image != null)
                    PresentTexture(image.Image, CharacterAnimation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.White, this);

                image = CharacterAnimation.Library?.CreateImage(CharacterAnimation.Index + 130, ImageType.Image);
                if (image != null)
                    PresentTexture(image.Image, CharacterAnimation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.White, this);


                DXManager.SetBlend(false);
            }
        }
        private void CharacterAnimation_BeforeDraw(object sender, EventArgs e)
        {
            SelectInfo selectInfo = SelectBox.SelectedButton?.SelectInfo;
            if (selectInfo == null)
            {
                CharacterAnimation.Visible = false;
                return;
            }

            MirImage image = CharacterAnimation.Library?.GetImage(CharacterAnimation.Index);

            if (image == null) return;

            int x = CharacterAnimation.DisplayArea.X - image.OffSetX;
            int y = CharacterAnimation.DisplayArea.Y - image.OffSetY;

            image = CharacterAnimation.Library?.CreateImage(CharacterAnimation.Index + (CharacterAnimation.Loop ? 20 : 30), ImageType.Image);
            if (image?.Image != null)
                PresentTexture(image.Image, CharacterAnimation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.FromArgb(180, 255, 255, 255), this);

            /*   if ()
               { CharacterAnimation.BodyLibrary.Draw(CharacterAnimation.Index + 20, x, y, ScaleMatrix, Color.FromArgb(180, 255, 255, 255), true, 1f, ImageType.Image);}
               else
                   CharacterAnimation.BodyLibrary.Draw(CharacterAnimation.Index + 30, x, y, ScaleMatrix, Color.FromArgb(180, 255, 255, 255), true, 1f, ImageType.Image);*/
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled) return;

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.ConfigWindow:
                        ConfigBox.Visible = !ConfigBox.Visible;
                        break;
                    default:
                        continue;
                }

                e.Handled = true;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ConfigBox != null)
                {
                    if (!ConfigBox.IsDisposed)
                        ConfigBox.Dispose();
                    ConfigBox = null;
                }

                if (ConfigButton != null)
                {
                    if (!ConfigButton.IsDisposed)
                        ConfigButton.Dispose();
                    ConfigButton = null;
                }

                if (SelectBox != null)
                {
                    if (!SelectBox.IsDisposed)
                        SelectBox.Dispose();
                    SelectBox = null;
                }
                if (CharacterBox != null)
                {
                    if (!CharacterBox.IsDisposed)
                        CharacterBox.Dispose();
                    CharacterBox = null;
                }

                if (CharacterAnimation != null)
                {
                    if (!CharacterAnimation.IsDisposed)
                        CharacterAnimation.Dispose();
                    CharacterAnimation = null;
                }

            }
        }

        #endregion
        
        public sealed class SelectDialog : DXWindow
        {
            #region Properties

            #region SelectedButton

            public SelectButton SelectedButton
            {
                get => _SelectedButton;
                set
                {
                    if (_SelectedButton == value) return;

                    SelectButton oldValue = _SelectedButton;
                    _SelectedButton = value;

                    OnSelectedButtonChanged(oldValue, value);
                }
            }
            private SelectButton _SelectedButton;
            public event EventHandler<EventArgs> SelectedButtonChanged;
            public void OnSelectedButtonChanged(SelectButton oValue, SelectButton nValue)
            {
                SelectedButtonChanged?.Invoke(this, EventArgs.Empty);

                if (oValue != null)
                    oValue.Selected = false;

                if (SelectedButton != null)
                    SelectedButton.Selected = true;

                DeleteButton.Enabled = SelectedButton != null;
                StartButton.Enabled = CanStartGame;

                ((SelectScene) Parent).UpdateCharacterDisplay();
            }

            #endregion

            #region StartGameAttempted

            public bool StartGameAttempted
            {
                get => _StartGameAttempted;
                set
                {
                    if (_StartGameAttempted == value) return;

                    bool oldValue = _StartGameAttempted;
                    _StartGameAttempted = value;

                    OnStartGameAttemptedChanged(oldValue, value);
                }
            }
            private bool _StartGameAttempted;
            public event EventHandler<EventArgs> StartGameAttemptedChanged;
            public void OnStartGameAttemptedChanged(bool oValue, bool nValue)
            {
                StartGameAttemptedChanged?.Invoke(this, EventArgs.Empty);

                StartButton.Enabled = CanStartGame;
            }

            #endregion

            public bool CanStartGame => SelectedButton != null && !StartGameAttempted;

            private DXButton StartButton, CreateButton, DeleteButton;
            public List<SelectInfo> CharacterList = new List<SelectInfo>();
            public SelectButton[] SelectButtons = new SelectButton[4];

            public override WindowType Type => WindowType.None;
            public override bool CustomSize => false;
            public override bool AutomaticVisibility => false;
            #endregion

            public SelectDialog()
            {
                Size = new Size(320, 425);
                HasFooter = true;
                TitleLabel.Text = CEnvir.Language.SelectTitle;
                Movable = false;

                CloseButton.MouseClick += (o, e) => LogOut();

                StartButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.SelectStartButtonLabel },
                    Location = new Point((Size.Width - 260)/4 + 10, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                    Enabled = false,
                };
                StartButton.MouseClick += (o, e) => StartGame();

                CreateButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.SelectCreateButtonLabel },
                    Location = new Point((Size.Width - 260)/4*2 + 90, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CreateButton.MouseClick += CreateButton_MouseClick;

                DeleteButton = new DXButton
                {
                    Parent = this,
                    Label = { Text = CEnvir.Language.SelectDeleteButtonLabel },
                    Location = new Point((Size.Width - 260)/4*3 + 170, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                    Enabled = false,
                };
                DeleteButton.MouseClick += DeleteButton_MouseClick;

                for (int i = 0; i < 4; i++)
                {
                    SelectButton button;
                    SelectButtons[i] = button = new SelectButton
                    {
                        Parent = this,
                        Border = i > 0,
                        Visible = i == -1,
                        Sound = SoundIndex.ButtonA,
                    };
                    button.MouseClick += (o, e) => SelectedButton = (SelectButton) o;

                    button.Location = new Point(20, 45 + i*(button.Size.Height + 3));
                }

            }

            #region Methods

            public void StartGame()
            {
                StartGameAttempted = true;

                CEnvir.Enqueue(new C.StartGame
                {
                    CharacterIndex = SelectedButton.SelectInfo.CharacterIndex
                });

            }
            private void LogOut()
            {
                CEnvir.Enqueue(new C.Logout());
            }
            public void UpdateCharacters()
            {
                for (int i = 0; i < SelectButtons.Length; i++)
                {
                    SelectButton button = SelectButtons[i];
                    button.SelectInfo = i >= CharacterList.Count ? null : CharacterList[i];
                }


                SelectedButton = CharacterList.Count == 0 ? null : SelectButtons[0];
                CreateButton.Enabled = CharacterList.Count < 4;
            }

            private void CreateButton_MouseClick(object sender, MouseEventArgs e)
            {
                SelectScene scene = ActiveScene as SelectScene;

                if (scene == null) return;

                Visible = false;
                scene.CharacterBox.Visible = true;
                scene.CharacterBox.CharacterNameTextBox.SetFocus();
                scene.CharacterAnimation.Visible = false;
            }
            private void DeleteButton_MouseClick(object sender, MouseEventArgs e)
            {
                DateTime deleteTime = CEnvir.Now.AddSeconds(5);
                SelectInfo character = SelectedButton.SelectInfo;

                DXMessageBox box = new DXMessageBox($"Are you sure you want to delete the character {character.CharacterName}\n" +
                                                    $"Please wait {(deleteTime - CEnvir.Now).TotalSeconds:0.0} seconds before confirming.", "Delete Character", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o, e1) => CEnvir.Enqueue(new C.DeleteCharacter { CharacterIndex = character.CharacterIndex, CheckSum = CEnvir.C, });
                box.YesButton.Enabled = false;

                box.ProcessAction = () =>
                {
                    if (CEnvir.Now > deleteTime)
                    {
                        box.Label.Text = $"Are you sure you want to delete the character {character.CharacterName}.";
                        box.YesButton.Enabled = true;
                        box.ProcessAction = null;
                    }
                    else
                        box.Label.Text = $"Are you sure you want to delete the character {character.CharacterName}\n" +
                                         $"Please wait {(deleteTime - CEnvir.Now).TotalSeconds:0.0} seconds before confirming.";
                };
            }

            public override void OnKeyPress(KeyPressEventArgs e)
            {
                base.OnKeyPress(e);

                switch ((Keys) e.KeyChar)
                {
                    case Keys.Enter:
                        if (StartButton.Enabled)
                            StartGame();
                        break;
                    case Keys.Down:

                        bool select = false;

                        foreach (SelectButton button in SelectButtons)
                        {
                            if (select)
                            {
                                SelectedButton = button;
                                break;
                            }

                            if (button != SelectedButton) continue;

                            select = true;
                        }
                        break;
                    case Keys.Up:

                        SelectButton previous = null;

                        foreach (SelectButton button in SelectButtons)
                        {
                            if (button == SelectedButton)
                            {
                                if (previous != null)
                                    SelectedButton = previous;
                                break;
                            }

                            previous = button;
                        }

                        break;

                }
            }
            public override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                switch (e.KeyCode)
                {
                    case Keys.Down:

                        bool select = false;

                        foreach (SelectButton button in SelectButtons)
                        {
                            if (select)
                            {
                                if (button.Visible)
                                    SelectedButton = button;
                                break;
                            }

                            if (button != SelectedButton) continue;

                            select = true;
                        }
                        break;
                    case Keys.Up:

                        SelectButton previous = null;

                        foreach (SelectButton button in SelectButtons)
                        {
                            if (button == SelectedButton)
                            {
                                if (previous != null)
                                    SelectedButton = previous;
                                break;
                            }

                            previous = button;
                        }
                        break;
                }
            }

            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (StartButton != null)
                    {
                        if (!StartButton.IsDisposed)
                            StartButton.Dispose();

                        StartButton = null;
                    }
                    
                    if (CreateButton != null)
                    {
                        if (!CreateButton.IsDisposed)
                            CreateButton.Dispose();

                        CreateButton = null;
                    }

                    if (DeleteButton != null)
                    {
                        if (!DeleteButton.IsDisposed)
                            DeleteButton.Dispose();

                        DeleteButton = null;
                    }


                    for (int i = 0; i < SelectButtons.Length; i++)
                    {
                        if (SelectButtons[i] == null) continue;

                        if (!SelectButtons[i].IsDisposed)
                            SelectButtons[i].Dispose();

                        SelectButtons[i] = null;
                    }
                    SelectButtons = null;

                    if (_SelectedButton != null)
                    {
                        if (!_SelectedButton.IsDisposed)
                            _SelectedButton.Dispose();

                        _SelectedButton = null;
                    }

                    CharacterList.Clear();
                    CharacterList = null;
                    _StartGameAttempted = false;
                }
            }

            #endregion
        }

        public sealed class NewCharacterDialog : DXWindow
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
                CreateButton.Enabled = CanCreate;
            }

            #endregion
            
            #region CreateAttempted

            public bool CreateAttempted
            {
                get => _CreateAttempted;
                set
                {
                    if (_CreateAttempted == value) return;

                    bool oldValue = _CreateAttempted;
                    _CreateAttempted = value;

                    OnCreateAttemptedChanged(oldValue, value);
                }
            }
            private bool _CreateAttempted;
            public event EventHandler<EventArgs> CreateAttemptedChanged;
            public void OnCreateAttemptedChanged(bool oValue, bool nValue)
            {
                CreateAttemptedChanged?.Invoke(this, EventArgs.Empty);
                CreateButton.Enabled = CanCreate;
            }

            #endregion
            
            public bool CanCreate => !CreateAttempted && CharacterNameValid;
            
            public DXLabel SelectedClassLabel, SelectedGenderLabel, HairColourLabel, ArmourColourLabel, CharacterNameHelpLabel;
            public DXTextBox CharacterNameTextBox;
            public DXNumberBox HairNumberBox;
            public DXControl CharacterDisplay;

            public DXColourControl HairColour, ArmourColour;
            
            public DXButton CreateButton,
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

            public NewCharacterDialog()
            {
                Size = new Size(260, 650);
                HasFooter = true;
                TitleLabel.Text = CEnvir.Language.NewCharacterTitle;
                Movable = false;
                Visible = false;
                CloseButton.MouseClick += (o, e) => Close();

                CreateButton = new DXButton
                {
                    Parent = this,
                    Enabled = false,
                    Label = { Text = CEnvir.Language.NewCharacterCreateButtonLabel },
                    Location = new Point((Size.Width - 80)/2, Size.Height - 43),
                    Size = new Size(80, DefaultHeight),
                };
                CreateButton.MouseClick += (o, e) => Create();

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
                };

                DXLabel label = new DXLabel
                {
                    Parent = panel,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    Text = CEnvir.Language.NewCharacterSelectClassLabel,
                };
                label.Location = new Point((panel.Size.Width - label.Size.Width)/2, 0);


                WarriorButton = new DXButton
                {
                    Index = 120,
                    LibraryFile = LibraryFile.Interface1c,
                    Pressed = true,
                    Parent = panel,
                };
                WarriorButton.MouseClick += (o, e) => SelectedClass = MirClass.Warrior;
                int offset = (panel.Size.Width - WarriorButton.Size.Width*4)/5;
                WarriorButton.Location = new Point(offset, 22);

                WizardButton = new DXButton
                {
                    Index = 126,
                    LibraryFile = LibraryFile.Interface1c,
                    Location = new Point(90, 50),
                    Parent = panel,
                };
                WizardButton.Location = new Point(offset*2 + WarriorButton.Size.Width, 22);
                WizardButton.MouseClick += (o, e) => SelectedClass = MirClass.Wizard;

                TaoistButton = new DXButton
                {
                    Index = 131,
                    LibraryFile = LibraryFile.Interface1c,
                    Location = new Point(130, 50),
                    Parent = panel,
                };
                TaoistButton.Location = new Point(offset*3 + WarriorButton.Size.Width*2, 22);
                TaoistButton.MouseClick += (o, e) => SelectedClass = MirClass.Taoist;

                AssassinButton = new DXButton
                {
                    Index = 136,
                    LibraryFile = LibraryFile.Interface1c,
                    Location = new Point(170, 50),
                    Parent = panel,
                };
                AssassinButton.Location = new Point(offset*4 + WarriorButton.Size.Width*3, 22);
                AssassinButton.MouseClick += (o, e) => SelectedClass = MirClass.Assassin;

                SelectedClassLabel = new DXLabel
                {
                    AutoSize = false,
                    DrawFormat = TextFormatFlags.HorizontalCenter,
                    Size = new Size(80, 15),
                    Parent = panel,
                    Text = CEnvir.Language.NewCharacterSelectedClassLabel,
                    BackColour = Color.FromArgb(16, 8, 8),
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99)

                };
                SelectedClassLabel.Location = new Point((panel.Size.Width - SelectedClassLabel.Size.Width)/2, panel.Size.Height - SelectedClassLabel.Size.Height - 5);

                #endregion

                #region Select Gender

                panel = new DXControl
                {
                    Parent = this,
                    BackColour = Color.FromArgb(72, 36, 36),
                    Border = true,
                    DrawTexture = true,
                    Size = new Size(200, 85),
                    Location = new Point(30, 135),
                    BorderColour = Color.FromArgb(198, 166, 99),
                };

                label = new DXLabel
                {
                    Parent = panel,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    Text = CEnvir.Language.NewCharacterSelectGenderLabel,
                };
                label.Location = new Point((panel.Size.Width - label.Size.Width)/2, 0);

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
                    Text = CEnvir.Language.NewCharacterSelectedGenderLabel,
                    BackColour = Color.FromArgb(16, 8, 8),
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99)

                };
                SelectedGenderLabel.Location = new Point((panel.Size.Width - SelectedGenderLabel.Size.Width)/2, panel.Size.Height - SelectedGenderLabel.Size.Height - 5);

                #endregion

                #region Customization


                panel = new DXControl
                {
                    Parent = this,
                    BackColour = Color.FromArgb(72, 36, 36),
                    Border = true,
                    DrawTexture = true,
                    Size = new Size(200, 330),
                    Location = new Point(30, 230),
                    BorderColour = Color.FromArgb(198, 166, 99),
                };
                label = new DXLabel
                {
                    Parent = panel,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    Text = CEnvir.Language.NewCharacterCustomizationLabel,
                };
                label.Location = new Point((panel.Size.Width - label.Size.Width)/2, 0);

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

                label = new DXLabel
                {
                    Parent = panel,
                    Text = CEnvir.Language.NewCharacterHairTypeLabel,
                };
                label.Location = new Point(HairNumberBox.Location.X - label.Size.Width - 5, (HairNumberBox.Size.Height - label.Size.Height)/2 + HairNumberBox.Location.Y);

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
                    Text = CEnvir.Language.NewCharacterHairColorLabel
                };
                HairColourLabel.Location = new Point(HairNumberBox.Location.X - HairColourLabel.Size.Width - 5, (HairColour.Size.Height - HairColourLabel.Size.Height)/2 + HairColour.Location.Y);

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
                    Text = CEnvir.Language.NewCharacterArmourColorLabel,
                };
                ArmourColourLabel.Location = new Point(HairNumberBox.Location.X - ArmourColourLabel.Size.Width - 5, (ArmourColour.Size.Height - ArmourColourLabel.Size.Height)/2 + ArmourColour.Location.Y);


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
                    Text = CEnvir.Language.NewCharacterPreviewLabel,
                };
                label.Location = new Point((panel.Size.Width - label.Size.Width)/2, 0);

                #endregion

                CharacterNameTextBox = new DXTextBox
                {
                    Location = new Point(75, 570),
                    Parent = this,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Size = new Size(155, 20),
                };
                CharacterNameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
                CharacterNameTextBox.TextBox.GotFocus += (o, e) => CharacterNameHelpLabel.Visible = true;
                CharacterNameTextBox.TextBox.LostFocus += (o, e) => CharacterNameHelpLabel.Visible = false;
                CharacterNameTextBox.TextBox.KeyPress += TextBox_KeyPress;

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.NewCharacterCharacterNameLabel,
                };
                label.Location = new Point(CharacterNameTextBox.Location.X - label.Size.Width - 5, (CharacterNameTextBox.Size.Height - label.Size.Height)/2 + CharacterNameTextBox.Location.Y);

                CharacterNameHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = this,
                    Text = "[?]",
                    Hint = string.Format(CEnvir.Language.NewCharacterCharacterNameHelpHint, Globals.MinCharacterNameLength, Globals.MaxCharacterNameLength),
                };
                CharacterNameHelpLabel.Location = new Point(CharacterNameTextBox.Location.X + CharacterNameTextBox.Size.Width + 2, (CharacterNameTextBox.Size.Height - CharacterNameHelpLabel.Size.Height)/2 + CharacterNameTextBox.Location.Y);



                HairNumberBox.Value = 1;
            }

            #region Method

            public void Create()
            {
                CreateAttempted = true;

                C.NewCharacter p = new C.NewCharacter
                {
                    CharacterName = CharacterNameTextBox.TextBox.Text,
                    Class = SelectedClass,
                    Gender = SelectedGender,
                    HairType = (int)HairNumberBox.Value,
                    HairColour = HairColour.Enabled ? HairColour.BackColour : Color.Empty,
                    ArmourColour = ArmourColour.Enabled ? ArmourColour.BackColour : Color.Empty,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(p);
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
                if (e.KeyChar != (char) Keys.Enter) return;

                e.Handled = true;

                if (CreateButton.Enabled)
                    Create();
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
                DXControl panel = (DXControl) sender;
                MirLibrary lib;

                float x = panel.DisplayArea.Location.X;
                float y = panel.DisplayArea.Location.Y;

                int armour, weapon, hair;



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
                        weapon = 1042;
                        switch (SelectedGender)
                        {
                            case MirGender.Male:
                                armour = 941;
                                hair = 60;
                                break;
                            case MirGender.Female:
                                armour = 951;
                                hair = 80;
                                break;
                            default:
                                return;
                        }
                        break;
                    case MirClass.Assassin:
                        weapon = 2200;
                        switch (SelectedGender)
                        {
                            case MirGender.Male:
                                armour = 2000;
                                hair = 1100;
                                break;
                            case MirGender.Female:
                                armour = 2010;
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
                    lib.Draw(weapon, x + 70, y + 160, Color.White, true, 1F, ImageType.Image);
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
                    _CreateAttempted = false;

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

                    if (CreateButton != null)
                    {
                        if (!CreateButton.IsDisposed)
                            CreateButton.Dispose();

                        CreateButton = null;
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

        public sealed class SelectButton : DXControl
        {
            #region Properties

            #region Selected

            public bool Selected
            {
                get => _Selected;
                set
                {
                    if (_Selected == value) return;

                    bool oldValue = _Selected;
                    _Selected = value;

                    OnSelectedChanged(oldValue, value);
                }
            }
            private bool _Selected;
            public event EventHandler<EventArgs> SelectedChanged;
            public void OnSelectedChanged(bool oValue, bool nValue)
            {
                SelectedChanged?.Invoke(this, EventArgs.Empty);
                if (Selected)
                {
                    Border = false;
                    BackColour = Color.FromArgb(72, 36, 36);
                    if (SelectInfo != null)
                    {
                        switch (SelectInfo.Class)
                        {
                            case MirClass.Warrior:
                                DXSoundManager.Play(SelectInfo.Gender == MirGender.Male ? SoundIndex.SelectWarriorMale : SoundIndex.SelectWarriorFemale);
                                break;
                            case MirClass.Wizard:
                                DXSoundManager.Play(SelectInfo.Gender == MirGender.Male ? SoundIndex.SelectWizardMale : SoundIndex.SelectWizardFemale);
                                break;
                            case MirClass.Taoist:
                                DXSoundManager.Play(SelectInfo.Gender == MirGender.Male ? SoundIndex.SelectTaoistMale : SoundIndex.SelectTaoistFemale);
                                break;
                            case MirClass.Assassin:
                                DXSoundManager.Play(SelectInfo.Gender == MirGender.Male ? SoundIndex.SelectAssassinMale : SoundIndex.SelectAssassinFemale);
                                break;
                        }

                    }
                }
                else
                {
                    Border = true;
                    BackColour = Color.FromArgb(24, 12, 12);
                }
            }
            #endregion

            #region SelectInfo

            public SelectInfo SelectInfo
            {
                get => _SelectInfo;
                set
                {
                    if (_SelectInfo == value) return;

                    SelectInfo oldValue = _SelectInfo;
                    _SelectInfo = value;

                    OnSelectInfoChanged(oldValue, value);
                }
            }
            private SelectInfo _SelectInfo;
            public event EventHandler<EventArgs> SelectInfoChanged;
            public void OnSelectInfoChanged(SelectInfo oValue, SelectInfo nValue)
            {
                SelectInfoChanged?.Invoke(this, EventArgs.Empty);

                if (SelectInfo == null)
                {
                    Visible = false;
                    return;
                }
                Visible = true;
                ClassIcon.Index = 27 + (int) SelectInfo.Class;
                NameLabel.Text = SelectInfo.CharacterName;
                ClassLabel.Text = SelectInfo.Class.ToString();
                LevelLabel.Text = SelectInfo.Level.ToString();
                LocationLabel.Text = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == SelectInfo.Location)?.Description ?? "New Character";
            }

            #endregion

            public DXImageControl ClassIcon;
            public DXLabel NameLabel, ClassLabel, LevelLabel, LocationLabel;

            #endregion

            public SelectButton()
            {
                Border = true;
                BackColour = Color.FromArgb(24, 12, 12);
                DrawTexture = true;
                BorderColour = Color.FromArgb(198, 166, 99);

                Size = new Size(280, 75);

                ClassIcon = new DXImageControl
                {
                    LibraryFile = LibraryFile.Interface,
                    Location = new Point(6, 6),
                    Size = new Size(64, 64),
                    Border = true,
                    Parent = this,
                    IsControl = false,
                };

                NameLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(130, 15),
                    ForeColour = Color.White,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Parent = this,
                    Location = new Point(135, 8),
                    Border = true,
                    BackColour = Color.FromArgb(16, 8, 8),
                    IsControl = false,
                };

                DXLabel label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.SelectNameLabel,
                    IsControl = false,

                };
                label.Location = new Point(NameLabel.Location.X - label.Size.Width - 5, 8);

                ClassLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(53, 15),
                    ForeColour = Color.White,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Parent = this,
                    Location = new Point(135, 28),
                    Border = true,
                    BackColour = Color.FromArgb(16, 8, 8),
                    IsControl = false,
                };

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.SelectClassLabel,
                    IsControl = false,
                };
                label.Location = new Point(ClassLabel.Location.X - label.Size.Width - 5, 28);

                LevelLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(30, 15),
                    ForeColour = Color.White,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Parent = this,
                    Location = new Point(235, 28),
                    Border = true,
                    BackColour = Color.FromArgb(16, 8, 8),
                    IsControl = false,
                };

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.SelectLevelLabel,
                    IsControl = false,
                };
                label.Location = new Point(LevelLabel.Location.X - label.Size.Width - 5, 28);

                LocationLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(130, 15),
                    ForeColour = Color.White,
                    BorderColour = Color.FromArgb(198, 166, 99),
                    Parent = this,
                    Location = new Point(135, 48),
                    Border = true,
                    BackColour = Color.FromArgb(16, 8, 8),
                    IsControl = false,
                };

                label = new DXLabel
                {
                    Parent = this,
                    Text = CEnvir.Language.SelectLocationLabel,
                    IsControl = false,
                };
                label.Location = new Point(LocationLabel.Location.X - label.Size.Width - 5, 48);
            }

            #region Methods

            public override void Draw()
            {
                if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

                OnBeforeDraw();
                DrawControl();
                OnBeforeChildrenDraw();
                if (!Border)
                    DrawDetails();
                DrawChildControls();
                DrawBorder();
                OnAfterDraw();
            }
            public void DrawDetails()
            {
                Point location = DisplayArea.Location;

                InterfaceLibrary.Draw(25, location.X, location.Y, Color.White, false, 1F, ImageType.Image);

                Size s = InterfaceLibrary.GetSize(26);
                InterfaceLibrary.Draw(26, location.X + Size.Width - s.Width, location.Y, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(8);
                InterfaceLibrary.Draw(8, location.X, location.Y + Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(9);
                InterfaceLibrary.Draw(9, location.X + Size.Width - s.Width, location.Y + Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                int x = s.Width;
                int y = s.Height;

                s = InterfaceLibrary.GetSize(2);
                InterfaceLibrary.Draw(2, location.X + x, location.Y, Color.White, new Rectangle(0, 0, Size.Width - x*2, s.Height), 1f, ImageType.Image);
                InterfaceLibrary.Draw(2, location.X + x, location.Y + Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width - x*2, s.Height), 1f, ImageType.Image);

                s = InterfaceLibrary.GetSize(1);
                InterfaceLibrary.Draw(1, location.X, location.Y + y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y*2), 1f, ImageType.Image);
                InterfaceLibrary.Draw(1, location.X + Size.Width - s.Width, location.Y + y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y*2), 1f, ImageType.Image);

            }

            protected internal override void UpdateBorderInformation()
            {
                BorderInformation = null;
                if (!Border || Size.Width == 0 || Size.Height == 0) return;

                BorderInformation = new[]
                {
                    new Vector2(DisplayArea.Left, DisplayArea.Top),
                    new Vector2(DisplayArea.Right - 1, DisplayArea.Top),
                    new Vector2(DisplayArea.Right - 1, DisplayArea.Bottom - 1),
                    new Vector2(DisplayArea.Left, DisplayArea.Bottom - 1),
                    new Vector2(DisplayArea.Left, DisplayArea.Top)
                };
            }

            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (ClassIcon != null)
                    {
                        if (!ClassIcon.IsDisposed)
                            ClassIcon.Dispose();

                        ClassIcon = null;
                    }

                    if (NameLabel != null)
                    {
                        if (!NameLabel.IsDisposed)
                            NameLabel.Dispose();

                        NameLabel = null;
                    }

                    if (ClassLabel != null)
                    {
                        if (!ClassLabel.IsDisposed)
                            ClassLabel.Dispose();

                        ClassLabel = null;
                    }

                    if (LevelLabel != null)
                    {
                        if (!LevelLabel.IsDisposed)
                            LevelLabel.Dispose();

                        LevelLabel = null;
                    }

                    if (LocationLabel != null)
                    {
                        if (!LocationLabel.IsDisposed)
                            LocationLabel.Dispose();

                        LocationLabel = null;
                    }

                    _Selected = false;
                    _SelectInfo = null;
                }

            }

            #endregion
        }
    }
}
 
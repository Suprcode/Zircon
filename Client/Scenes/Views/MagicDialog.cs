using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class MagicDialog : DXControl
    {
        #region Properties

        private DXImageControl HeaderImage, BackgroundImage;

        private DXLabel TitleLabel;
        private DXButton CloseButton;

        private DXTabControl TabControl;
        public SortedDictionary<MagicSchool, MagicTab> SchoolTabs = new SortedDictionary<MagicSchool, MagicTab>();

        public Dictionary<MagicInfo, MagicCell> Magics = new Dictionary<MagicInfo, MagicCell>();

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            if (Settings != null)
                Settings.Visible = nValue;

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        #endregion

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.MagicBox;

        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;
        }

        #endregion

        public MagicDialog()
        {
            Size = new Size(420, 516);
            Movable = true;
            Sort = true;

            HeaderImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 0),
                IsControl = false
            };

            BackgroundImage = new DXImageControl
            {
                Parent = this,
                Index = 164,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 66),
                IsControl = false
            };

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.MagicDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            TabControl = new DXTabControl
            {
                Parent = this,
                Size = new Size(420, 448),
                Location = new Point(0, 40),
                MarginLeft = 56,
                Padding = 2,
                BackColour = Color.Empty
            };
        }

        #region Methods

        public void CreateTabs()
        {
            foreach (KeyValuePair<MagicSchool, MagicTab> pair in SchoolTabs)
                pair.Value.Dispose();

            SchoolTabs.Clear();

            List<MagicInfo> magics = Globals.MagicInfoList.Binding.ToList();
            magics.Sort((x1, x2) => x1.NeedLevel1.CompareTo(x2.NeedLevel1));

            switch (GameScene.Game.User.Class)
            {
                case MirClass.Warrior:
                    HeaderImage.Index = 160;
                    break;
                case MirClass.Wizard:
                    HeaderImage.Index = 161;
                    break;
                case MirClass.Taoist:
                    HeaderImage.Index = 162;
                    break;
                case MirClass.Assassin:
                    HeaderImage.Index = 163;
                    break;
            }

            foreach (MagicInfo magic in magics)
            {
                if (magic.Class != MapObject.User.Class || magic.School == MagicSchool.None || magic.School == MagicSchool.Discipline) continue;

                MagicTab tab;

                if (!SchoolTabs.TryGetValue(magic.School, out tab))
                {
                    SchoolTabs[magic.School] = tab = new MagicTab(magic.School);
                    tab.MouseWheel += tab.ScrollBar.DoMouseWheel;
                    tab.PassThrough = false;
                }                   

                MagicCell cell = new MagicCell
                {
                    Parent = tab,
                    Info = magic,
                    BackColour = Color.Empty
                };
                Magics[magic] = cell;
                cell.MouseWheel += tab.ScrollBar.DoMouseWheel;
            }

            foreach (KeyValuePair<MagicSchool, MagicTab> dxTab in SchoolTabs)
            {
                dxTab.Value.Parent = TabControl;
            }
        }

        public void RefreshMagic(MagicInfo info)
        {
            if (Magics.ContainsKey(info))
                Magics[info].Refresh();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (HeaderImage != null)
                {
                    if (!HeaderImage.IsDisposed)
                        HeaderImage.Dispose();

                    HeaderImage = null;
                }

                if (BackgroundImage != null)
                {
                    if (!BackgroundImage.IsDisposed)
                        BackgroundImage.Dispose();

                    BackgroundImage = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();

                    TitleLabel = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (SchoolTabs != null)
                {
                    foreach (KeyValuePair<MagicSchool, MagicTab> pair in SchoolTabs)
                    {
                        if (pair.Value == null) continue;
                        if (pair.Value.IsDisposed) continue;

                        pair.Value.Dispose();
                    }
                    SchoolTabs.Clear();
                    SchoolTabs = null;
                }
                
                if (Magics != null)
                {
                    foreach (KeyValuePair<MagicInfo, MagicCell> pair in Magics)
                    {
                        if (pair.Value == null) continue;
                        if (pair.Value.IsDisposed) continue;

                        pair.Value.Dispose();
                    }
                    Magics.Clear();
                    Magics = null;
                }

            }

        }

        #endregion
    }

    public sealed class MagicTab : DXTab
    {
        #region Properties
        public DXVScrollBar ScrollBar;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(20, Size.Height + 6);
            ScrollBar.Location = new Point(Size.Width - 30, -2);

            int height = 9;

            foreach (DXControl control in Controls)
            {
                if (control is not MagicCell) continue;

                height += control.Size.Height + 5;
            }

            ScrollBar.MaxValue = height;

            ScrollBar.VisibleSize = Size.Height;
            UpdateLocations();
        }
        
        #endregion

        public MagicTab(MagicSchool school)
        {
            TabButton.LibraryFile = LibraryFile.Interface;
            TabButton.Hint = school.ToString();

            BackColour = Color.Empty;
            Border = false;
            BorderColour = Color.Red;
            Location = new Point(10, 30);

            switch (school)
            {
                case MagicSchool.Active:
                    TabButton.Index = 166;
                    TabButton.HoverIndex = 167;
                    TabButton.PressedIndex = 167;
                    break;
                case MagicSchool.Passive:
                    TabButton.Index = 168;
                    TabButton.HoverIndex = 169;
                    TabButton.PressedIndex = 169;
                    break;
                case MagicSchool.Toggle:
                    TabButton.Index = 170;
                    TabButton.HoverIndex = 171;
                    TabButton.PressedIndex = 171;
                    break;
                case MagicSchool.Horse:
                    TabButton.Index = 172;
                    TabButton.HoverIndex = 173;
                    TabButton.PressedIndex = 173;
                    break;
                case MagicSchool.Fire:
                    TabButton.Index = 174;
                    TabButton.HoverIndex = 175;
                    TabButton.PressedIndex = 175;
                    break;
                case MagicSchool.Ice:
                    TabButton.Index = 176;
                    TabButton.HoverIndex = 177;
                    TabButton.PressedIndex = 177;
                    break;
                case MagicSchool.Lightning:
                    TabButton.Index = 178;
                    TabButton.HoverIndex = 179;
                    TabButton.PressedIndex = 179;
                    break;
                case MagicSchool.Wind:
                    TabButton.Index = 180;
                    TabButton.HoverIndex = 181;
                    TabButton.PressedIndex = 181;
                    break;
                case MagicSchool.Phantom:
                    TabButton.Index = 182;
                    TabButton.HoverIndex = 183;
                    TabButton.PressedIndex = 183;
                    break;
                case MagicSchool.Holy:
                    TabButton.Index = 184;
                    TabButton.HoverIndex = 185;
                    TabButton.PressedIndex = 185;
                    break;
                case MagicSchool.Dark:
                    TabButton.Index = 186;
                    TabButton.HoverIndex = 187;
                    TabButton.PressedIndex = 187;
                    break;
                case MagicSchool.Physical:
                    TabButton.Index = 188;
                    TabButton.HoverIndex = 189;
                    TabButton.PressedIndex = 189;
                    break;
                case MagicSchool.Atrocity:
                    TabButton.Index = 190;
                    TabButton.HoverIndex = 191;
                    TabButton.PressedIndex = 191;
                    break;
                case MagicSchool.Kill:
                    TabButton.Index = 192;
                    TabButton.HoverIndex = 193;
                    TabButton.PressedIndex = 193;
                    break;
                case MagicSchool.Assassination:
                    TabButton.Index = 194;
                    TabButton.HoverIndex = 195;
                    TabButton.PressedIndex = 195;
                    break;
                case MagicSchool.None:
                    TabButton.Index = 170;
                    TabButton.HoverIndex = 171;
                    TabButton.PressedIndex = 171;
                    break;
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface }
            };
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();
        }

        #region Methods
        public void UpdateLocations()
        {
            int y = -ScrollBar.Value + 7;

            foreach (DXControl control in Controls)
            {
                if (control is not MagicCell) continue;

                control.Location = new Point(5, y);
                y += control.Size.Height + 5;
            }
        }

        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            UpdateLocations();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }
        }

        #endregion
    }

    public sealed class MagicCell : DXControl
    {
        #region Properties

        #region Info
        public MagicInfo Info
        {
            get => _Info;
            set
            {
                if (_Info == value) return;

                MagicInfo oldValue = _Info;
                _Info = value;

                OnInfoChanged(oldValue, value);
            }
        }
        private MagicInfo _Info;
        public event EventHandler<EventArgs> InfoChanged;
        public void OnInfoChanged(MagicInfo oValue, MagicInfo nValue)
        {
            Image.Index = Info.Icon;
            NameLabel.Text = Info.Name;
            Refresh();
            InfoChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXImageControl Background, Image, Level4Border;
        public DXImageControl ExperienceBar;
        public DXLabel NameLabel, LevelLabel, ExperienceLabel, KeyLabel;

        #endregion

        public MagicCell()
        {
            Size = new Size(369, 54);
            DrawTexture = true;

            Background = new DXImageControl
            {
                Parent = this,
                Index = 165,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(0, 0),
                IsControl = false
            };

            Level4Border = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(4, 4),
                Visible = false
            };
            Level4Border.MouseEnter += (o, e) => OnMouseEnter();
            Level4Border.MouseLeave += (o, e) => OnMouseLeave();

            Image = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(9, 9)
            };
            Image.MouseClick += Image_MouseClick;
            Image.KeyDown += Image_KeyDown;
            Image.MouseEnter += (o, e) => OnMouseEnter();
            Image.MouseLeave += (o, e) => OnMouseLeave();

            ExperienceBar = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(110, 36),
                IsControl = false
            };
            ExperienceBar.Size = ExperienceBar.Library.GetSize(812);
            ExperienceBar.AfterDraw += ExperienceBarAfterDraw;

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(55, 1),
                ForeColour = Color.White,
                IsControl = false,
            };

            KeyLabel = new DXLabel
            {
                Parent = Image,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                IsControl = false,
                ForeColour = Color.Aquamarine,
                AutoSize =  false,
                Size = new Size(36,36),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            KeyLabel.SizeChanged += (o, e) => KeyLabel.Location = new Point(Image.Size.Width - KeyLabel.Size.Width, Image.Size.Height - KeyLabel.Size.Height);
            KeyLabel.MouseEnter += (o, e) => OnMouseEnter();
            KeyLabel.MouseLeave += (o, e) => OnMouseLeave();

            LevelLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(57, 30),
                IsControl = false
            };

            ExperienceLabel = new DXLabel
            {
                Parent = this,
                IsControl = false
            };
            ExperienceLabel.SizeChanged += (o, e) => ExperienceLabel.Location = new Point(Size.Width - ExperienceLabel.Size.Width - 6, 17);
        }

        #region Methods
        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            switch (GameScene.Game.MagicBarBox.SpellSet)
            {
                case 1:
                    magic.Set1Key = SpellKey.None;
                    break;
                case 2:
                    magic.Set2Key = SpellKey.None;
                    break;
                case 3:
                    magic.Set3Key = SpellKey.None;
                    break;
                case 4:
                    magic.Set4Key = SpellKey.None;
                    break;

            }

            CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
        }

        private void Image_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            if (e.Handled) return;
            if (MouseControl != Image) return;

            SpellKey key = SpellKey.None;

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.SpellUse01:
                        key = SpellKey.Spell01;
                        break;
                    case KeyBindAction.SpellUse02:
                        key = SpellKey.Spell02;
                        break;
                    case KeyBindAction.SpellUse03:
                        key = SpellKey.Spell03;
                        break;
                    case KeyBindAction.SpellUse04:
                        key = SpellKey.Spell04;
                        break;
                    case KeyBindAction.SpellUse05:
                        key = SpellKey.Spell05;
                        break;
                    case KeyBindAction.SpellUse06:
                        key = SpellKey.Spell06;
                        break;
                    case KeyBindAction.SpellUse07:
                        key = SpellKey.Spell07;
                        break;
                    case KeyBindAction.SpellUse08:
                        key = SpellKey.Spell08;
                        break;
                    case KeyBindAction.SpellUse09:
                        key = SpellKey.Spell09;
                        break;
                    case KeyBindAction.SpellUse10:
                        key = SpellKey.Spell10;
                        break;
                    case KeyBindAction.SpellUse11:
                        key = SpellKey.Spell11;
                        break;
                    case KeyBindAction.SpellUse12:
                        key = SpellKey.Spell12;
                        break;
                    case KeyBindAction.SpellUse13:
                        key = SpellKey.Spell13;
                        break;
                    case KeyBindAction.SpellUse14:
                        key = SpellKey.Spell14;
                        break;
                    case KeyBindAction.SpellUse15:
                        key = SpellKey.Spell15;
                        break;
                    case KeyBindAction.SpellUse16:
                        key = SpellKey.Spell16;
                        break;
                    case KeyBindAction.SpellUse17:
                        key = SpellKey.Spell17;
                        break;
                    case KeyBindAction.SpellUse18:
                        key = SpellKey.Spell18;
                        break;
                    case KeyBindAction.SpellUse19:
                        key = SpellKey.Spell19;
                        break;
                    case KeyBindAction.SpellUse20:
                        key = SpellKey.Spell20;
                        break;
                    case KeyBindAction.SpellUse21:
                        key = SpellKey.Spell21;
                        break;
                    case KeyBindAction.SpellUse22:
                        key = SpellKey.Spell22;
                        break;
                    case KeyBindAction.SpellUse23:
                        key = SpellKey.Spell23;
                        break;
                    case KeyBindAction.SpellUse24:
                        key = SpellKey.Spell24;
                        break;
                    default:
                        continue;
                }
                
                e.Handled = true;
            }

            if (key == SpellKey.None) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            switch (GameScene.Game.MagicBarBox.SpellSet)
            {
                case 1:
                    magic.Set1Key = key;
                    break;
                case 2:
                    magic.Set2Key = key;
                    break;
                case 3:
                    magic.Set3Key = key;
                    break;
                case 4:
                    magic.Set4Key = key;
                    break;

            }

            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in MapObject.User.Magics)
            {
                if (pair.Key == magic.Info) continue;

                if (pair.Value.Set1Key == magic.Set1Key && magic.Set1Key != SpellKey.None)
                {
                    pair.Value.Set1Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set2Key == magic.Set2Key && magic.Set2Key != SpellKey.None)
                {
                    pair.Value.Set2Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set3Key == magic.Set3Key && magic.Set3Key != SpellKey.None)
                {
                    pair.Value.Set3Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

                if (pair.Value.Set4Key == magic.Set4Key && magic.Set4Key != SpellKey.None)
                {
                    pair.Value.Set4Key = SpellKey.None;

                    GameScene.Game.MagicBox.RefreshMagic(pair.Key);
                    GameScene.Game.CharacterBox.RefreshDisciplineMagic(pair.Key);
                }

            }
            
            CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
        }

        public override void OnMouseEnter()
        {
            GameScene.Game.MouseMagic = Info;
        }
        public override void OnMouseLeave()
        {
            GameScene.Game.MouseMagic = null;
        }

        private void ExperienceBarAfterDraw(object sender, EventArgs e)
        {
            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            //Get percent.
            MirImage image = ExperienceBar.Library.CreateImage(812, ImageType.Image);

            if (image == null) return;

            int x = (ExperienceBar.Size.Width - image.Width) / 2;
            int y = (ExperienceBar.Size.Height - image.Height) / 2;

            float percent = 1F;

            if (magic.Level < Globals.MagicMaxLevel)
            {
                switch (magic.Level)
                {
                    case 0:
                        if (magic.Info.Experience1 == 0) return;
                        percent = (float)Math.Min(1, Math.Max(0, magic.Experience / (decimal)magic.Info.Experience1));
                        break;
                    case 1:
                        if (magic.Info.Experience2 == 0) return;
                        percent = (float)Math.Min(1, Math.Max(0, magic.Experience / (decimal)magic.Info.Experience2));
                        break;
                    case 2:
                        if (magic.Info.Experience3 == 0) return;
                        percent = (float)Math.Min(1, Math.Max(0, magic.Experience / (decimal)magic.Info.Experience3));
                        break;
                    default:
                        if (magic.Info.Experience3 == 0) return;
                        percent = (float)Math.Min(1, Math.Max(0, magic.Experience / (decimal)((magic.Level - 2) * 500)));
                        break;
                }
            }

            if (percent == 0) return;

            PresentTexture(image.Image, this, new Rectangle(ExperienceBar.DisplayArea.X + x, ExperienceBar.DisplayArea.Y + y, (int)(image.Width * percent), image.Height), Color.White, ExperienceBar);
        }
        
        public void Refresh()
        {
            if (MapObject.User == null) return;

            if (MapObject.User.Magics.TryGetValue(Info, out ClientUserMagic magic))
            {
                float opacity = 1F;

                Background.ImageOpacity = opacity;
                Image.ImageOpacity = opacity;
                Level4Border.ImageOpacity = opacity;
                NameLabel.Opacity = opacity;
                LevelLabel.Opacity = opacity;
                ExperienceLabel.Opacity = opacity;

                Image.IsEnabled = true;
                LevelLabel.Text = $"Level: {magic.Level}";
                LevelLabel.ForeColour = Color.FromArgb(198, 166, 99);
                LevelLabel.Location = new Point(57, 30);

                SpellKey key = SpellKey.None;
                switch (GameScene.Game.MagicBarBox.SpellSet)
                {
                    case 1:
                        key = magic.Set1Key;
                        break;
                    case 2:
                        key = magic.Set2Key;
                        break;
                    case 3:
                        key = magic.Set3Key;
                        break;
                    case 4:
                        key = magic.Set4Key;
                        break;
                }

                Type type = typeof(SpellKey);

                MemberInfo[] infos = type.GetMember(key.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                KeyLabel.Text = description?.Description;

                Level4Border.Visible = true;
                Level4Border.Index = UpdateBorder(magic.Info.School);

                if (Info.NeedLevel1 > MapObject.User.Level)
                {
                    ExperienceLabel.Text = $"Required Level: {Info.NeedLevel1}";
                    ExperienceLabel.ForeColour = Color.Red;
                }
                else
                {
                    if (magic.Level < Globals.MagicMaxLevel)
                    {
                        switch (magic.Level)
                        {
                            case 0:
                                ExperienceLabel.Text = $"Experience: {magic.Experience}/{magic.Info.Experience1}";
                                break;
                            case 1:
                                ExperienceLabel.Text = $"Experience: {magic.Experience}/{magic.Info.Experience2}";
                                break;
                            case 2:
                                ExperienceLabel.Text = $"Experience: {magic.Experience}/{magic.Info.Experience3}";
                                break;
                            default:
                                ExperienceLabel.Text = $"Experience: {magic.Experience}/{(magic.Level - 2) * 500}";
                                break;
                        }
                    }
                    else
                    {
                        ExperienceLabel.Text = $"Experience: Max Level";
                    }
                    ExperienceLabel.ForeColour = Color.FromArgb(198, 166, 99);
                }
            }
            else
            {
                float opacity = MapObject.User.Level >= Info.NeedLevel1 ? 1F : 0.3F;

                Background.ImageOpacity = opacity;
                Image.ImageOpacity = opacity;
                Level4Border.ImageOpacity = opacity;
                NameLabel.Opacity = opacity;
                LevelLabel.Opacity = opacity;
                ExperienceLabel.Opacity = opacity;

                Level4Border.Visible = false;
                Image.IsEnabled = false;
                LevelLabel.Text = "Not\r\nLearned";
                LevelLabel.ForeColour = Color.Red;
                LevelLabel.Location = new Point(57, 17);

                ExperienceLabel.Text = $"Required Level: {Info.NeedLevel1}";
                ExperienceLabel.ForeColour = MapObject.User.Level >= Info.NeedLevel1 ? Color.LimeGreen : Color.Red;
            }

            if (this == MouseControl)
            {
                GameScene.Game.MouseMagic = null;
                GameScene.Game.MouseMagic = Info;
            }
        }

        public int UpdateBorder(MagicSchool school)
        {
            int index = 0;

            switch (school)
            {
                case MagicSchool.Passive:
                    index = 860;
                    break;
                case MagicSchool.Active:
                    index = 861;
                    break;
                case MagicSchool.Toggle:
                    index = 862;
                    break;
                case MagicSchool.Fire:
                    index = 870;
                    break;
                case MagicSchool.Ice:
                    index = 871;
                    break;
                case MagicSchool.Lightning:
                    index = 872;
                    break;
                case MagicSchool.Wind:
                    index = 873;
                    break;
                case MagicSchool.Phantom:
                    index = 874;
                    break;
                case MagicSchool.Holy:
                    index = 880;
                    break;
                case MagicSchool.Dark:
                    index = 881;
                    break;
                case MagicSchool.Physical:
                    index = 883;
                    break;
                case MagicSchool.Atrocity:
                    index = 890;
                    break;
                case MagicSchool.Kill:
                    index = 891;
                    break;
                case MagicSchool.Assassination:
                    index = 892;
                    break;
                case MagicSchool.None:
                    break;
            }

            return index;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Info = null;
                InfoChanged = null;

                if (Image != null)
                {
                    if (!Image.IsDisposed)
                        Image.Dispose();

                    Image = null;
                }

                if (ExperienceBar != null)
                {
                    if (!ExperienceBar.IsDisposed)
                        ExperienceBar.Dispose();

                    ExperienceBar = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (KeyLabel != null)
                {
                    if (!KeyLabel.IsDisposed)
                        KeyLabel.Dispose();

                    KeyLabel = null;
                }
            }
        }

        #endregion
    }

}

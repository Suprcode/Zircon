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

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MagicDialog : DXWindow
    {
        #region Properties

        private DXTabControl TabControl;
        public SortedDictionary<MagicSchool, MagicTab> SchoolTabs = new SortedDictionary<MagicSchool, MagicTab>();

        public Dictionary<MagicInfo, MagicCell> Magics = new Dictionary<MagicInfo, MagicCell>();


        public override WindowType Type => WindowType.MagicBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public MagicDialog()
        {
            TitleLabel.Text = "Magic List";

            HasFooter = false;

            SetClientSize(new Size(311, 395));

            TabControl = new DXTabControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
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

            foreach (MagicInfo magic in magics)
            {
                if (magic.Class != MapObject.User.Class || magic.School == MagicSchool.None) continue;

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
                };
                Magics[magic] = cell;
                cell.MouseWheel += tab.ScrollBar.DoMouseWheel;
            }

            foreach (KeyValuePair<MagicSchool, MagicTab> dxTab in SchoolTabs)
            {
                dxTab.Value.Parent = TabControl;
            }
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

            ScrollBar.Size = new Size(14, Size.Height -2 );
            ScrollBar.Location = new Point(Size.Width - 16, 0);

            int height = 2;

            foreach (DXControl control in Controls)
            {
                if (!(control is MagicCell)) continue;

                height += control.Size.Height + 3;
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
            Border = true;

            switch (school)
            {
                case MagicSchool.Passive:
                    TabButton.Index = 54;
                    break;
                case MagicSchool.WeaponSkills:
                    TabButton.Index = 65;
                    break;
                case MagicSchool.Neutral:
                    TabButton.Index = 64;
                    break;
                case MagicSchool.Fire:
                    TabButton.Index = 56;
                    break;
                case MagicSchool.Ice:
                    TabButton.Index = 57;
                    break;
                case MagicSchool.Lightning:
                    TabButton.Index = 58;
                    break;
                case MagicSchool.Wind:
                    TabButton.Index = 59;
                    break;
                case MagicSchool.Holy:
                    TabButton.Index = 61;
                    break;
                case MagicSchool.Dark:
                    TabButton.Index = 62;
                    break;
                case MagicSchool.Phantom:
                    TabButton.Index = 60;
                    break;
                case MagicSchool.Combat:
                    TabButton.Index = 66;
                    break;
                case MagicSchool.Assassination:
                    TabButton.Index = 67;
                    break;
                case MagicSchool.None:
                    TabButton.Index = 55;
                    break;
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();
        }

        #region Methods
        public void UpdateLocations()
        {
            int y = -ScrollBar.Value + 5;

            foreach (DXControl control in Controls)
            {
                if (!(control is MagicCell)) continue;

                control.Location = new Point(5, y);
                y += control.Size.Height + 3;
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

        public DXImageControl Image;
        public DXImageControl ExperienceBar;
        public DXLabel NameLabel, LevelLabel, ExperienceLabel, KeyLabel;


        #endregion

        public MagicCell()
        {
            Size = new Size(286, 50);

            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            Image = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(8, 8),
            };
            Image.MouseClick += Image_MouseClick;
            Image.KeyDown += Image_KeyDown;

            ExperienceBar = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Location = new Point(52, 36),
                Index = 68,
                IsControl = false
            };
            ExperienceBar.AfterDraw += ExperienceBarAfterDraw;

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(48, 6),
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
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
            KeyLabel.SizeChanged += (o, e) => KeyLabel.Location = new Point(Image.Size.Width - KeyLabel.Size.Width, Image.Size.Height - KeyLabel.Size.Height);
            
            LevelLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(48, 20),
                IsControl = false
            };


            ExperienceLabel = new DXLabel
            {
                Parent = this,
                IsControl = false
            };
            ExperienceLabel.SizeChanged += (o, e) => ExperienceLabel.Location = new Point(282 - ExperienceLabel.Size.Width, 20);
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
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set2Key == magic.Set2Key && magic.Set2Key != SpellKey.None)
                {
                    pair.Value.Set2Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set3Key == magic.Set3Key && magic.Set3Key != SpellKey.None)
                {
                    pair.Value.Set3Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set4Key == magic.Set4Key && magic.Set4Key != SpellKey.None)
                {
                    pair.Value.Set4Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
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
            MirImage image = ExperienceBar.Library.CreateImage(69, ImageType.Image);

            if (image == null) return;

            int x = (ExperienceBar.Size.Width - image.Width) / 2;
            int y = (ExperienceBar.Size.Height - image.Height) / 2;

            float percent = 1F;
            switch (magic.Level)
            {
                case 0:
                    if (magic.Info.Experience1 == 0) return;
                    percent = (float) Math.Min(1, Math.Max(0, magic.Experience/(decimal) magic.Info.Experience1));
                    break;
                case 1:
                    if (magic.Info.Experience2 == 0) return;
                    percent = (float) Math.Min(1, Math.Max(0, magic.Experience/(decimal) magic.Info.Experience2));
                    break;
                case 2:
                    if (magic.Info.Experience3 == 0) return;
                    percent = (float) Math.Min(1, Math.Max(0, magic.Experience/(decimal) magic.Info.Experience3));
                    break;
                default:
                    if (magic.Info.Experience3 == 0) return;
                    percent = (float)Math.Min(1, Math.Max(0, magic.Experience / (decimal)((magic.Level - 2) * 500)));
                    break;
            }
            
            if (percent == 0) return;



            PresentTexture(image.Image, this, new Rectangle(ExperienceBar.DisplayArea.X + x, ExperienceBar.DisplayArea.Y + y, (int)(image.Width * percent), image.Height), Color.White, ExperienceBar);

        }
        
        public void Refresh()
        {
            if (MapObject.User == null) return;

            ClientUserMagic magic;

            if (MapObject.User.Magics.TryGetValue(Info, out magic))
            {
                Image.IsEnabled = true;
                LevelLabel.Text = $"Level: {magic.Level}";
                LevelLabel.ForeColour = Color.FromArgb(198, 166, 99);

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

                if (Info.NeedLevel1 > MapObject.User.Level)
                {

                    ExperienceLabel.Text = $"Required Level: {Info.NeedLevel1}";
                    ExperienceLabel.ForeColour = Color.Red;
                }
                else
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
                    ExperienceLabel.ForeColour = Color.FromArgb(198, 166, 99);
                }
            }
            else
            {
                Image.IsEnabled = false;
                LevelLabel.Text = "Not Learned";
                LevelLabel.ForeColour = Color.Red;

                ExperienceLabel.Text = $"Required Level: {Info.NeedLevel1}";
                ExperienceLabel.ForeColour = MapObject.User.Level >= Info.NeedLevel1 ? Color.Green : Color.Red;
            }

            if (this == MouseControl)
            {
                GameScene.Game.MouseMagic = null;
                GameScene.Game.MouseMagic = Info;
            }

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

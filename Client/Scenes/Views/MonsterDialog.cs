using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MonsterDialog : DXWindow
    {
        #region Properties

        #region Monster

        public MonsterObject Monster
        {
            get => _Monster;
            set
            {
                if (_Monster == value) return;

                MonsterObject oldValue = _Monster;
                _Monster = value;

                OnMonsterChanged(oldValue, value);
            }
        }
        private MonsterObject _Monster;
        public event EventHandler<EventArgs> MonsterChanged;
        public void OnMonsterChanged(MonsterObject oValue, MonsterObject nValue)
        {
            Visible = Monster != null && Config.MonsterBoxVisible;

            if (Monster == null) return;

            NameLabel.Text = Monster.MonsterInfo.MonsterName;
            LevelLabel.Text = Monster.MonsterInfo.Level.ToString();

            RefreshStats();

            MonsterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Expanded

        public bool Expanded
        {
            get => _Expanded;
            set
            {
                if (_Expanded == value) return;

                bool oldValue = _Expanded;
                _Expanded = value;

                OnExpandedChanged(oldValue, value);
            }
        }
        private bool _Expanded;
        public event EventHandler<EventArgs> ExpandedChanged;
        public void OnExpandedChanged(bool oValue, bool nValue)
        {
            ExpandButton.Index = Expanded ? 44 : 46;

            Size = Expanded ? new Size(Size.Width, 150) : new Size(Size.Width, 54);
            Config.MonsterBoxExpanded = Expanded;

            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public DXImageControl AttackIcon;

        public DXLabel LevelLabel, NameLabel, HealthLabel, ACLabel, MRLabel, DCLabel;
        public DXLabel FireResistLabel, IceResistLabel, LightningResistLabel, WindResistLabel, HolyResistLabel, DarkResistLabel, PhantomResistLabel, PhysicalResistLabel;
        public DXButton ExpandButton;


        public override WindowType Type => WindowType.MonsterBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => false;
        #endregion

        public MonsterDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.3F;

            Size = new Size(186, 54);
            Location = new Point(250, 50);
            DXControl panel = new DXControl
            {
                Size = new Size(31, 20),
                Location = new Point(5, 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                DrawTexture = true,
                IsControl = false
            };

            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(30,18),
                Location = new Point(0,0),
                Border = true,
                Parent = panel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            panel = new DXControl
            {
                Size = new Size(140, 20),
                Location = new Point(41, 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                DrawTexture = true,
                IsControl = false
            };

            NameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(139, 18),
                Location = new Point(0, 0),
                Border = true,
                Parent = panel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            panel = new DXControl
            {
                Size = new Size(121, 16),
                Location = new Point(41, 32),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                DrawTexture = true,
                IsControl = false
            };

            HealthLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(120, 18),
                Location = new Point(41, 30),
                Border = true,
                Parent = this,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            panel.AfterDraw += (o, e) =>
            {
                if (Monster == null) return;

                MirLibrary lib;

                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out lib)) return;


                ClientObjectData data;
                GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data);

                float percent = Monster.CompanionObject != null ? 1 : 0;
                //if (MapObject.User.Stats[Stat.Health] == 0) return;

                if (data != null && data.MaxHealth > 0)
                    percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));// MapObject.User.CurrentHP / (float)MapObject.User.Stats[Stat.Health]));

                if (percent == 0) return;

                MirImage image = lib.CreateImage(5430, ImageType.Image);

                if (image == null) return;
                
                PresentTexture(image.Image, this, new Rectangle(panel.DisplayArea.X, panel.DisplayArea.Y + 2, (int)(image.Width * percent), image.Height), Color.White, panel);
            };


            DXControl panel2 = new DXControl
            {
                Size = new Size(31, 20),
                Location = new Point(5, 30),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                DrawTexture = true,
                PassThrough =  true,
            };

            AttackIcon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Opacity = 0.7F,
                Location =  new Point(5,0),
                IsControl = false

            };

            ExpandButton = new DXButton
            {
                Parent = this,
                Location = new Point(167, 34),
                LibraryFile = LibraryFile.Interface,
                Index = 46
            };
            ExpandButton.MouseClick += (o, e) => Expanded = !Expanded;

            panel2 = new DXControl
            {
                Size = new Size(Size.Width - 10, 85),
                Location = new Point(5, 60),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                DrawTexture = true,
                PassThrough = true,
            };

            DXLabel label = new DXLabel
            {
                Parent = panel2,
                IsControl = false,
                Text = "AC:"
            };
            label.Location = new Point(36 - label.Size.Width, 5);

            ACLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(36, 5),
                ForeColour = Color.White,
            };

            label = new DXLabel
            {
                Parent = panel2,
                IsControl = false,
                Text = "MR:"
            };
            label.Location = new Point(125 - label.Size.Width, 5);

            MRLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(125, 5),
                ForeColour = Color.White,
            };
            label = new DXLabel
            {
                Parent = panel2,
                IsControl = false,
                Text = "DC:"
            };
            label.Location = new Point(36 - label.Size.Width, 22);

            DCLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(36, 22),
                ForeColour = Color.White,
            };

            DXImageControl icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1510,
                Location = new Point(5, 39),
                Hint = "Fire"
            };

            FireResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 41),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1511,
                Location = new Point(icon.Location.X + 43, 39),
                Hint = "Ice"
            };

            IceResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 41),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1512,
                Location = new Point(icon.Location.X + 43, 39),
                Hint = "Lightning"
            };

            LightningResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 41),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1513,
                Location = new Point(icon.Location.X + 43, 39),
                Hint = "Wind"
            };

            WindResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 41),
                Tag = icon,
            };


            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1514,
                Location = new Point(5, 63),
                Hint = "Holy",
            };

            HolyResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 65),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1515,
                Location = new Point(icon.Location.X + 43, 63),
                Hint = "Dark",
            };

            DarkResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 65),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1516,
                Location = new Point(icon.Location.X + 43, 63),
                Hint = "Phantom",
            };

            PhantomResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 65),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                Location = new Point(icon.Location.X + 43, 63),
                Hint = "Physical"
            };

            PhysicalResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 65),
                Tag = icon,
            };

            Expanded = Config.MonsterBoxExpanded;
        }

        #region Methods
        private void PopulateLabel(Stat stat, DXLabel label, Stats stats)
        {
            label.Text = $"x{Math.Abs(stats[stat]):0}";

            if (stats[stat] == 0)
                label.ForeColour = Color.White;
            else if (stats[stat] > 0)
                label.ForeColour = Color.Lime;
            else if (stats[stat] < 0)
                label.ForeColour = Color.IndianRed;
        }
        
        public void RefreshHealth()
        {
            ClientObjectData data;
            HealthLabel.Text = !GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data) ? string.Empty : $"{data.Health} / {data.MaxHealth}";
        }

        public void RefreshStats()
        {
            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data) || data.Stats == null)
            {
                HealthLabel.Text = string.Empty;

                ACLabel.Text = $"{Monster.MonsterInfo.Stats[Stat.MinAC]} - {Monster.MonsterInfo.Stats[Stat.MaxAC]}";
                MRLabel.Text = $"{Monster.MonsterInfo.Stats[Stat.MinMR]} - {Monster.MonsterInfo.Stats[Stat.MaxMR]}";
                DCLabel.Text = $"{Monster.MonsterInfo.Stats[Stat.MinDC]} - {Monster.MonsterInfo.Stats[Stat.MaxDC]}";

                PopulateLabel(Stat.FireResistance, FireResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.IceResistance, IceResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.LightningResistance, LightningResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.WindResistance, WindResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.HolyResistance, HolyResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.DarkResistance, DarkResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.PhantomResistance, PhantomResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.PhysicalResistance, PhysicalResistLabel, Monster.MonsterInfo.Stats);



                switch (Monster.Stats.GetAffinityElement())
                {
                    case Element.None:
                        AttackIcon.Index = 1517;
                        AttackIcon.Hint = "Physical";
                        break;
                    case Element.Fire:
                        AttackIcon.Index = 1510;
                        AttackIcon.Hint = "Fire";
                        break;
                    case Element.Ice:
                        AttackIcon.Index = 1511;
                        AttackIcon.Hint = "Ice";
                        break;
                    case Element.Lightning:
                        AttackIcon.Index = 1512;
                        AttackIcon.Hint = "Lightning";
                        break;
                    case Element.Wind:
                        AttackIcon.Index = 1513;
                        AttackIcon.Hint = "Wind";
                        break;
                    case Element.Holy:
                        AttackIcon.Index = 1514;
                        AttackIcon.Hint = "Holy";
                        break;
                    case Element.Dark:
                        AttackIcon.Index = 1515;
                        AttackIcon.Hint = "Dark";
                        break;
                    case Element.Phantom:
                        AttackIcon.Index = 1516;
                        AttackIcon.Hint = "Phantom";
                        break;
                }
            }
            else
            {
                HealthLabel.Text = $"{data.Health} / {data.MaxHealth}";


                ACLabel.Text = $"{data.Stats[Stat.MinAC]} - {data.Stats[Stat.MaxAC]}";
                MRLabel.Text = $"{data.Stats[Stat.MinMR]} - {data.Stats[Stat.MaxMR]}";
                DCLabel.Text = $"{data.Stats[Stat.MinDC]} - {data.Stats[Stat.MaxDC]}";


                PopulateLabel(Stat.FireResistance, FireResistLabel, data.Stats);
                PopulateLabel(Stat.IceResistance, IceResistLabel, data.Stats);
                PopulateLabel(Stat.LightningResistance, LightningResistLabel, data.Stats);
                PopulateLabel(Stat.WindResistance, WindResistLabel, data.Stats);
                PopulateLabel(Stat.HolyResistance, HolyResistLabel, data.Stats);
                PopulateLabel(Stat.DarkResistance, DarkResistLabel, data.Stats);
                PopulateLabel(Stat.PhantomResistance, PhantomResistLabel, data.Stats);
                PopulateLabel(Stat.PhysicalResistance, PhysicalResistLabel, data.Stats);


                switch (data.Stats.GetAffinityElement())
                {
                    case Element.None:
                        AttackIcon.Index = 1517;
                        AttackIcon.Hint = "Physical";
                        break;
                    case Element.Fire:
                        AttackIcon.Index = 1510;
                        AttackIcon.Hint = "Fire";
                        break;
                    case Element.Ice:
                        AttackIcon.Index = 1511;
                        AttackIcon.Hint = "Ice";
                        break;
                    case Element.Lightning:
                        AttackIcon.Index = 1512;
                        AttackIcon.Hint = "Lightning";
                        break;
                    case Element.Wind:
                        AttackIcon.Index = 1513;
                        AttackIcon.Hint = "Wind";
                        break;
                    case Element.Holy:
                        AttackIcon.Index = 1514;
                        AttackIcon.Hint = "Holy";
                        break;
                    case Element.Dark:
                        AttackIcon.Index = 1515;
                        AttackIcon.Hint = "Dark";
                        break;
                    case Element.Phantom:
                        AttackIcon.Index = 1516;
                        AttackIcon.Hint = "Phantom";
                        break;
                }
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Monster = null;
                MonsterChanged = null;

                _Expanded = false;
                ExpandedChanged = null;
                
                if (AttackIcon != null)
                {
                    if (!AttackIcon.IsDisposed)
                        AttackIcon.Dispose();

                    AttackIcon = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (ACLabel != null)
                {
                    if (!ACLabel.IsDisposed)
                        ACLabel.Dispose();

                    ACLabel = null;
                }

                if (MRLabel != null)
                {
                    if (!MRLabel.IsDisposed)
                        MRLabel.Dispose();

                    MRLabel = null;
                }

                if (DCLabel != null)
                {
                    if (!DCLabel.IsDisposed)
                        DCLabel.Dispose();

                    DCLabel = null;
                }

                if (FireResistLabel != null)
                {
                    if (!FireResistLabel.IsDisposed)
                        FireResistLabel.Dispose();

                    FireResistLabel = null;
                }

                if (IceResistLabel != null)
                {
                    if (!IceResistLabel.IsDisposed)
                        IceResistLabel.Dispose();

                    IceResistLabel = null;
                }

                if (LightningResistLabel != null)
                {
                    if (!LightningResistLabel.IsDisposed)
                        LightningResistLabel.Dispose();

                    LightningResistLabel = null;
                }

                if (WindResistLabel != null)
                {
                    if (!WindResistLabel.IsDisposed)
                        WindResistLabel.Dispose();

                    WindResistLabel = null;
                }

                if (HolyResistLabel != null)
                {
                    if (!HolyResistLabel.IsDisposed)
                        HolyResistLabel.Dispose();

                    HolyResistLabel = null;
                }

                if (DarkResistLabel != null)
                {
                    if (!DarkResistLabel.IsDisposed)
                        DarkResistLabel.Dispose();

                    DarkResistLabel = null;
                }

                if (PhantomResistLabel != null)
                {
                    if (!PhantomResistLabel.IsDisposed)
                        PhantomResistLabel.Dispose();

                    PhantomResistLabel = null;
                }

                if (ExpandButton != null)
                {
                    if (!ExpandButton.IsDisposed)
                        ExpandButton.Dispose();

                    ExpandButton = null;
                }

            }
        }

        #endregion
    }
}

using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public sealed class MagicBarDialog : DXWindow
    {
        #region SpellSet

        public int SpellSet
        {
            get => _SpellSet;
            set
            {
                if (_SpellSet == value) return;

                int oldValue = _SpellSet;
                _SpellSet = value;

                OnSpellSetChanged(oldValue, value);
            }
        }
        private int _SpellSet;
        public event EventHandler<EventArgs> SpellSetChanged;
        public void OnSpellSetChanged(int oValue, int nValue)
        {
            SpellSetChanged?.Invoke(this, EventArgs.Empty);

            UpdateIcons();

            foreach (KeyValuePair<MagicInfo, MagicCell> pair in GameScene.Game.MagicBox.Magics)
            {
                pair.Value.Refresh();
            }
        }

        #endregion

        public DXButton UpButton, DownButton;
        public DXLabel SetLabel;

        Dictionary<SpellKey, DXImageControl> IconBorders = new Dictionary<SpellKey, DXImageControl>();
        Dictionary<SpellKey, DXImageControl> Icons = new Dictionary<SpellKey, DXImageControl>();
        Dictionary<SpellKey, DXLabel> Cooldowns = new Dictionary<SpellKey, DXLabel>();

        public override WindowType Type => WindowType.MagicBarBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        public MagicBarDialog()
        {
            _SpellSet = 1;

            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.6F;

            int iconSpacing = Config.ShowMagicBarFrames ? 49 : 37;
            int rowSpacing = iconSpacing + 5;
            int groupSpacing = 5;

            SetClientSize(new Size(iconSpacing * 12 + 15 + 25, iconSpacing - 2));

            // Define how many icons per row
            const int IconsPerRow = 12;

            int startX = ClientArea.X;
            int startY = ClientArea.Y;

            for (int i = 0; i < 24; i++)
            {
                SpellKey key = (SpellKey)Enum.Parse(typeof(SpellKey), $"Spell{(i + 1):00}");

                int row = i / IconsPerRow;

                int col = i % IconsPerRow;

                int xOffset = col * iconSpacing + (col / 4) * groupSpacing;

                int yOffset = row * rowSpacing;

                bool isVisible = i < 12;

                IconBorders[key] = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.GameInter2,
                    Location = new Point(startX + xOffset, startY + yOffset),
                    Size = Config.ShowMagicBarFrames ? new Size(48, 46) : new Size(36, 36),
                    Visible = isVisible,
                    BackColour = Color.FromArgb(20, 20, 20),
                    Border = true,
                    BorderColour = Color.FromArgb(198, 166, 99),
                };

                Icons[key] = new DXImageControl
                {
                    Parent = IconBorders[key],
                    LibraryFile = LibraryFile.MagicIcon,
                    Location = Config.ShowMagicBarFrames ? new Point(6, 5) : new Point(0, 0),
                    DrawTexture = true,
                    Border = false,
                    Size = new Size(36, 36),
                    Opacity = 0.6F,
                    Visible = true
                };
            }
            int count = 1;
            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                pair.Value.MouseEnter += (o, e) => GameScene.Game.MouseMagic = ((DXImageControl)o).Tag as MagicInfo;
                pair.Value.MouseLeave += (o, e) => GameScene.Game.MouseMagic = null;

                DXLabel label = new DXLabel
                {
                    Parent = pair.Value,
                    Text = count.ToString(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Italic),
                    IsControl = false,
                };
                label.Location = new Point(34 - label.Size.Width, 34 - label.Size.Height);

                Cooldowns[pair.Key] = new DXLabel
                {
                    AutoSize = false,
                    BackColour = Color.FromArgb(125, 50, 50, 50),
                    Parent = pair.Value,
                    Location = new Point(1, 1),
                    IsControl = false,
                    Size = new Size(34, 34),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    ForeColour = Color.Gold,
                    Outline = true,
                    OutlineColour = Color.Black,
                };

                count++;
            }

            UpButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X + 461, ClientArea.Y),
                LibraryFile = LibraryFile.Interface,
                Index = 44
            };
            UpButton.MouseClick += (o, e) => SpellSet = Math.Max(1, SpellSet - 1);

            SetLabel = new DXLabel
            {
                Parent = this,
                Text = SpellSet.ToString(),
                IsControl = false,
                Location = new Point(ClientArea.X + 460, ClientArea.Y + UpButton.Size.Height - 1),
                ForeColour = Color.White,
            };

            DownButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X + 461, ClientArea.X + 37 - UpButton.Size.Height),
                LibraryFile = LibraryFile.Interface,
                Index = 46
            };
            DownButton.MouseClick += (o, e) => SpellSet = Math.Min(4, SpellSet + 1);
        }

        public void UpdateIcons()
        {
            SpellKey maxKey = SpellKey.None;
            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                ClientUserMagic magic = GameScene.Game?.User?.Magics.Values.FirstOrDefault(x =>
                {
                    switch (SpellSet)
                    {
                        case 1:
                            return x.Set1Key == pair.Key;
                        case 2:
                            return x.Set2Key == pair.Key;
                        case 3:
                            return x.Set3Key == pair.Key;
                        case 4:
                            return x.Set4Key == pair.Key;
                        default:
                            return false;
                    }
                });

                pair.Value.Tag = magic?.Info;

                if (magic != null)
                {
                    maxKey = pair.Key;
                    pair.Value.Index = magic.Info.Icon;
                    IconBorders[pair.Key].Index = Config.ShowMagicBarFrames ? UpdateBorder(magic.Info.School) : -1;
                }
                else
                {
                    pair.Value.Index = -1;
                    Cooldowns[pair.Key].Visible = false;
                    IconBorders[pair.Key].Index = Config.ShowMagicBarFrames ? UpdateBorder(MagicSchool.None) : -1;

                }

                pair.Value.Index = magic?.Info.Icon ?? -1;
            }

            SetLabel.Text = SpellSet.ToString();

            if (maxKey >= SpellKey.Spell13)
            {
                if (Config.ShowMagicBarFrames)
                {
                    SetClientSize(new Size(49 * 12 + 15 + 20, 46 * 2 + 5 + 3));
                }
                else
                {
                    SetClientSize(new Size(37 * 12 + 15 + 20, 37 * 2 + 5));
                }

                IconBorders[SpellKey.Spell13].Visible = true;
                IconBorders[SpellKey.Spell14].Visible = true;
                IconBorders[SpellKey.Spell15].Visible = true;
                IconBorders[SpellKey.Spell16].Visible = true;
                IconBorders[SpellKey.Spell17].Visible = true;
                IconBorders[SpellKey.Spell18].Visible = true;
                IconBorders[SpellKey.Spell19].Visible = true;
                IconBorders[SpellKey.Spell20].Visible = true;
                IconBorders[SpellKey.Spell21].Visible = true;
                IconBorders[SpellKey.Spell22].Visible = true;
                IconBorders[SpellKey.Spell23].Visible = true;
                IconBorders[SpellKey.Spell24].Visible = true;
            }
            else
            {
                if (Config.ShowMagicBarFrames)
                {
                    SetClientSize(new Size(49 * 12 + 15 + 20, 46));
                }
                else
                {
                    SetClientSize(new Size(37 * 12 + 15 + 20, 37));
                }

                IconBorders[SpellKey.Spell13].Visible = false;
                IconBorders[SpellKey.Spell14].Visible = false;
                IconBorders[SpellKey.Spell15].Visible = false;
                IconBorders[SpellKey.Spell16].Visible = false;
                IconBorders[SpellKey.Spell17].Visible = false;
                IconBorders[SpellKey.Spell18].Visible = false;
                IconBorders[SpellKey.Spell19].Visible = false;
                IconBorders[SpellKey.Spell20].Visible = false;
                IconBorders[SpellKey.Spell21].Visible = false;
                IconBorders[SpellKey.Spell22].Visible = false;
                IconBorders[SpellKey.Spell23].Visible = false;
                IconBorders[SpellKey.Spell24].Visible = false;
            }

            SetLabel.Location = new Point(ClientArea.Right - 16, ClientArea.Height / 2);
            UpButton.Location = new Point(ClientArea.Right - 15, SetLabel.Location.Y - 9);
            DownButton.Location = new Point(ClientArea.Right - 15, SetLabel.Location.Y + 15);
        }

        public override void Process()
        {
            base.Process();

            if (!Visible) return;

            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                MagicInfo info = pair.Value.Tag as MagicInfo;

                if (info == null)
                {
                    Cooldowns[pair.Key].Visible = false;
                    continue;
                }

                ClientUserMagic magic = GameScene.Game.User.Magics[info];

                bool toggleSkill = false;

                switch (magic.Info.Magic)
                {
                    case MagicType.Thrusting:
                    case MagicType.HalfMoon:
                    case MagicType.DestructiveSurge:
                    case MagicType.FlameSplash:
                    case MagicType.FullBloom:
                    case MagicType.WhiteLotus:
                    case MagicType.RedLotus:
                    case MagicType.SweetBrier:
                    case MagicType.Karma:
                        toggleSkill = true;
                        break;
                }

                if (CEnvir.Now >= magic.NextCast && (CEnvir.Now >= GameScene.Game.ToggleTime || !toggleSkill))
                {
                    Cooldowns[pair.Key].Visible = false;
                    continue;
                }

                var maxTime = (magic.NextCast > GameScene.Game.ToggleTime ? magic.NextCast : GameScene.Game.ToggleTime);

                Cooldowns[pair.Key].Visible = true;
                TimeSpan remaining = maxTime - CEnvir.Now;
                Cooldowns[pair.Key].Text = $"{Math.Ceiling(remaining.TotalSeconds)}s";

                if (remaining.TotalSeconds > 5)
                    Cooldowns[pair.Key].ForeColour = Color.Gold;
                else
                    Cooldowns[pair.Key].ForeColour = Color.Red;
            }

        }

        private static int UpdateBorder(MagicSchool school)
        {
            int index = -1;

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
                case MagicSchool.Discipline:
                    index = 815;
                    break;
                case MagicSchool.Horse:
                    index = 815;
                    break;
                case MagicSchool.None:
                    break;
            }

            return index;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            SpellSetChanged = null;

            if (UpButton != null)
            {
                if (!UpButton.IsDisposed)
                    UpButton.Dispose();

                UpButton = null;
            }

            if (DownButton != null)
            {
                if (!DownButton.IsDisposed)
                    DownButton.Dispose();

                DownButton = null;
            }

            if (SetLabel != null)
            {
                if (!SetLabel.IsDisposed)
                    SetLabel.Dispose();

                SetLabel = null;
            }

            if (IconBorders != null)
            {
                foreach (KeyValuePair<SpellKey, DXImageControl> pair in IconBorders)
                {
                    DXImageControl control = pair.Value;

                    if (control == null) continue;
                    if (control.IsDisposed) continue;

                    control.Dispose();
                }

                IconBorders.Clear();
                IconBorders = null;
            }

            if (Icons != null)
            {
                foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
                {
                    DXImageControl control = pair.Value;

                    if (control == null) continue;
                    if (control.IsDisposed) continue;

                    control.Dispose();
                }

                Icons.Clear();
                Icons = null;
            }

            if (Cooldowns != null)
            {
                foreach (KeyValuePair<SpellKey, DXLabel> pair in Cooldowns)
                {
                    DXLabel label = pair.Value;

                    if (label == null) continue;
                    if (label.IsDisposed) continue;

                    label.Dispose();
                }

                Cooldowns.Clear();
                Cooldowns = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


            SetClientSize(new Size(37 * 12 + 15 + 25, 37));

            int x = ClientArea.X;
            Icons[SpellKey.Spell01] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell02] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 37, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell03] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 74, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell04] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 111, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };

            x += 5;
            Icons[SpellKey.Spell05] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 148, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell06] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 185, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell07] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 222, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell08] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 259, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };

            x += 5;
            Icons[SpellKey.Spell09] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 296, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell10] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 333, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell11] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 370, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };
            Icons[SpellKey.Spell12] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 407, ClientArea.Y),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F
            };

            x = ClientArea.X;
            Icons[SpellKey.Spell13] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell14] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 37, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell15] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 74, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell16] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 111, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };

            x += 5;
            Icons[SpellKey.Spell17] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 148, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell18] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 185, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell19] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 222, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell20] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 259, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            x += 5;

            Icons[SpellKey.Spell21] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 296, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell22] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 333, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell23] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 370, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            Icons[SpellKey.Spell24] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon,
                Location = new Point(x + 407, ClientArea.Y + 37 + 5),
                DrawTexture = true,
                BackColour = Color.FromArgb(20, 20, 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                Opacity = 0.6F,
                Visible = false
            };
            
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
                label.Location = new Point(37 - label.Size.Width, 37 - label.Size.Height);

                Cooldowns[pair.Key] = new DXLabel
                {

                    AutoSize = false,
                    BackColour = Color.FromArgb(125, 50, 50, 50),
                    Parent = pair.Value,
                    Location = new Point(1, 1),
                    IsControl = false,
                    Size = new Size(36, 36),
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
                }
                else
                {
                    pair.Value.Index =  -1;
                    Cooldowns[pair.Key].Visible = false;

                }
                pair.Value.Index = magic?.Info.Icon ?? -1;

            }

            SetLabel.Text = SpellSet.ToString();

            if (maxKey >= SpellKey.Spell13)
            {
                SetClientSize(new Size(37 * 12 + 15 + 20, 37 * 2 + 5));

                Icons[SpellKey.Spell13].Visible = true;
                Icons[SpellKey.Spell14].Visible = true;
                Icons[SpellKey.Spell15].Visible = true;
                Icons[SpellKey.Spell16].Visible = true;
                Icons[SpellKey.Spell17].Visible = true;
                Icons[SpellKey.Spell18].Visible = true;
                Icons[SpellKey.Spell19].Visible = true;
                Icons[SpellKey.Spell20].Visible = true;
                Icons[SpellKey.Spell21].Visible = true;
                Icons[SpellKey.Spell22].Visible = true;
                Icons[SpellKey.Spell23].Visible = true;
                Icons[SpellKey.Spell24].Visible = true;

                UpButton.Location = new Point(ClientArea.X + 461, ClientArea.Y + 20);
                SetLabel.Location = new Point(ClientArea.X + 460, ClientArea.Y + UpButton.Size.Height - 1 + 20);
                DownButton.Location = new Point(ClientArea.X + 461, ClientArea.X + 37 - UpButton.Size.Height + 20);
            }
            else
            {
                SetClientSize(new Size(37 * 12 + 15 + 20, 37));

                Icons[SpellKey.Spell13].Visible = false;
                Icons[SpellKey.Spell14].Visible = false;
                Icons[SpellKey.Spell15].Visible = false;
                Icons[SpellKey.Spell16].Visible = false;
                Icons[SpellKey.Spell17].Visible = false;
                Icons[SpellKey.Spell18].Visible = false;
                Icons[SpellKey.Spell19].Visible = false;
                Icons[SpellKey.Spell20].Visible = false;
                Icons[SpellKey.Spell21].Visible = false;
                Icons[SpellKey.Spell22].Visible = false;
                Icons[SpellKey.Spell23].Visible = false;
                Icons[SpellKey.Spell24].Visible = false;

                UpButton.Location = new Point(ClientArea.X + 461, ClientArea.Y);
                SetLabel.Location = new Point(ClientArea.X + 460, ClientArea.Y + UpButton.Size.Height - 1);
                DownButton.Location = new Point(ClientArea.X + 461, ClientArea.X + 37 - UpButton.Size.Height);
            }
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
                    Cooldowns[pair.Key].ForeColour = Color.Red;}

        }
    }
}

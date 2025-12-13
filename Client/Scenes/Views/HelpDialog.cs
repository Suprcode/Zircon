using Client.Controls;
using Client.Envir;
using Client.Extensions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public sealed class HelpDialog : DXImageControl
    {
        #region Properties

        public DXLabel TitleLabel;
        public DXButton CloseButton;

        public HelpMenu Menu;

        #region Selected Page

        public HelpPage SelectedPage
        {
            get => _SelectedPage;
            private set
            {
                HelpPage oldValue = _SelectedPage;
                _SelectedPage = value;

                OnSelectedPageChanged(oldValue, value);
            }
        }
        private HelpPage _SelectedPage;
        public event EventHandler<EventArgs> SelectedPageChanged;
        public void OnSelectedPageChanged(HelpPage oValue, HelpPage nValue)
        {
            SelectedPageChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<HelpPage> Pages = [];

        public WindowSetting Settings;
        public WindowType Type => WindowType.HelpBox;

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
            Settings.Location = Location;
        }

        public void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            Visible = Settings.Visible;
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
                BringToFront();

            Settings.Visible = nValue;

            base.OnIsVisibleChanged(oValue, nValue);
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

        public HelpDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 9300;
            Movable = true;
            Sort = true;
            DropShadow = true;

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.HelpDialogTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 8);

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 1, 0);
            CloseButton.MouseClick += (o, e) => Visible = false;

            Menu = new HelpMenu
            {
                Parent = this,
                Location = new Point(13, 70)
            };
            Menu.SelectedChanged += (o, e) =>
            {
                SelectedPage?.Visible = false;

                SelectedPage = Menu.GetCurrentPage();
                SelectedPage.Update(SelectedPage.TabControl.SelectedTab);
                SelectedPage?.Visible = true;
            };

            AddPages();
        }

        #region Methods

        private void AddPages()
        {
            foreach (var helpInfo in Globals.HelpInfoList.Binding.OrderBy(x => x.Order))
            {
                AddPage(helpInfo);
            }
        }

        private void AddPage(HelpInfo info)
        {
            var page = new HelpPage(info)
            {
                Parent = this,
                Size = new Size(535, 312),
                Location = new Point(178, 68),
                Visible = false
            };

            Pages.Add(page);

            Menu.AddPage(page);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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

                if (Menu != null)
                {
                    if (!Menu.IsDisposed)
                        Menu.Dispose();

                    Menu = null;
                }

                _SelectedPage = null;
            }
        }

        #endregion
    }

    public sealed class HelpPage : DXControl
    {
        #region Properties

        public DXTabControl TabControl;
        public DXControl TextContainer;
        public DXVScrollBar TextContainerScrollBar;

        #endregion

        public Dictionary<HelpSectionInfo, HelpSection> Sections = new Dictionary<HelpSectionInfo, HelpSection>();

        public HelpInfo Info { get; private set; }

        public HelpPage(HelpInfo info) 
        {
            Info = info;

            TabControl = new DXTabControl
            {
                Parent = this,
                Size = new Size(512, 310),
                Location = new Point(0, 0),
                MarginLeft = 0,
                Padding = 0,
                BackColour = Color.Empty
            };

            TextContainerScrollBar = new DXVScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Location = new Point(514, 19),
                Size = new Size(20, 295),
                MinValue = 0,
                MaxValue = 500,
                VisibleSize = 310,
                Change = 10,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true
            };

            TextContainerScrollBar.ValueChanged += (o, e) => UpdateLocations(TabControl.SelectedTab);

            CreateTabs(info);
        }

        #region Methods

        public void CreateTabs(HelpInfo info)
        {
            List<DXTab> tabs = new List<DXTab>();

            int i = 0;

            foreach (var infoPage in info.Pages.OrderBy(x => x.Order))
            {
                var tab = new DXTab
                {
                    Parent = TabControl,
                    TabButton =
                    {
                        Label = { Text = infoPage.Title }
                    },
                    BackColour = Color.Empty,
                    MinimumTabWidth = 100,
                    Location = new Point(0, 22)
                };
                tab.SelectedChanged += (o, e) =>
                {
                    var selected = o as DXTab;
                    Update(selected);
                };

                tabs.Add(tab);

                int k = 0;

                foreach (var infoSection in infoPage.Sections.OrderBy(x => x.Order))
                {
                    HelpSection cell = new HelpSection
                    {
                        Parent = tab,
                        Info = infoSection,
                        BackColour = Color.Empty,
                        Location = new Point(5, k)
                    };
                    Sections[infoSection] = cell;
                    cell.MouseWheel += TextContainerScrollBar.DoMouseWheel;
                }

                i++;

                UpdateLocations(tab);
            }
        }

        public void Update(DXTab tab)
        {
            foreach (DXControl control in tab.Controls)
            {
                if (control is not HelpSection section) continue;

                section.Update();
            }

            UpdateLocations(tab);
        }

        public void UpdateLocations(DXTab tab)
        {
            int y = -TextContainerScrollBar.Value + 5;
            int h = 0;

            foreach (DXControl control in tab.Controls)
            {
                if (control is not HelpSection) continue;

                control.Location = new Point(5, y);
                h += control.Size.Height;
                y += control.Size.Height;
            }

            TextContainerScrollBar.MaxValue = h + 30;
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

                if (TextContainer != null)
                {
                    if (!TextContainer.IsDisposed)
                        TextContainer.Dispose();

                    TextContainer = null;
                }

                if (TextContainerScrollBar != null)
                {
                    if (!TextContainerScrollBar.IsDisposed)
                        TextContainerScrollBar.Dispose();

                    TextContainerScrollBar = null;
                }

                foreach (KeyValuePair<HelpSectionInfo, HelpSection> pair in Sections)
                {
                    if (pair.Value != null)
                    {
                        if (!pair.Value.IsDisposed)
                            pair.Value.Dispose();
                    }
                }

                Sections.Clear();
                Sections = null;

                Info = null;
            }
        }

        #endregion
    }

    public sealed class HelpMenu : DXControl
    {
        #region Properties

        public DXVScrollBar MenuScrollBar;

        #region SelectedEntry

        public DXButton Selected
        {
            get => _Selected;
            private set
            {
                DXButton oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private DXButton _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(DXButton oValue, DXButton nValue)
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
            {
                oValue.Index = 9310;
                oValue.HoverIndex = 9310;
            }

            if (nValue != null)
            {
                nValue.Index = 9311;
                nValue.HoverIndex = 9311;
            }
        }

        #endregion

        #endregion

        public Dictionary<DXButton, HelpPage> MenuItems = new Dictionary<DXButton, HelpPage>();

        private const int ButtonHeight = 23;

        public HelpMenu()
        {
            Size = new Size(156, 306);

            MenuScrollBar = new DXVScrollBar
            {
                Parent = this,
                BackColour = Color.Empty,
                Location = new Point(134, 0),
                Size = new Size(20, 310),
                MinValue = 0,
                VisibleSize = Size.Height,
                Change = ButtonHeight,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true
            };

            MenuScrollBar.ValueChanged += (o, e) => UpdateLocations(-MenuScrollBar.Value);
        }

        #region Methods

        public void UpdateLocations(int value)
        {
            int y = value;

            foreach (DXControl control in Controls)
            {
                if (control is DXButton)
                {
                    control.Location = new Point(0, y);
                    y += ButtonHeight;
                }
            }
        }

        public void AddPage(HelpPage page)
        {
            int y = MenuItems.Count * ButtonHeight;

            var button = new DXButton
            {
                Index = 9310,
                HoverIndex = 9310,
                PressedIndex = 9310,
                LibraryFile = LibraryFile.GameInter,
                Size = new Size(134, 21),
                Parent = this,
                Location = new Point(0, y),
                Label = { Text = page.Info.Title }
            };
            button.MouseClick += (o, e) => Selected = (DXButton)o;
            button.MouseWheel += MenuScrollBar.DoMouseWheel;

            MenuItems.Add(button, page);

            MenuScrollBar.MaxValue = MenuItems.Count * ButtonHeight;
        }

        public HelpPage GetCurrentPage()
        {
           if (MenuItems.TryGetValue(Selected, out HelpPage page))
                return page;

           return null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (MenuScrollBar != null)
                {
                    if (!MenuScrollBar.IsDisposed)
                        MenuScrollBar.Dispose();

                    MenuScrollBar = null;
                }

                _Selected = null;

                foreach (KeyValuePair<DXButton, HelpPage> pair in MenuItems)
                {
                    if (pair.Value != null)
                    {
                        if (!pair.Value.IsDisposed)
                            pair.Value.Dispose();
                    }

                    if (pair.Key != null)
                    {
                        if (!pair.Key.IsDisposed)
                            pair.Key.Dispose();
                    }
                }

                MenuItems.Clear();
                MenuItems = null;
            }
        }

        #endregion
    }

    public partial class HelpSection : DXControl
    {
        #region Properties

        public DXLabel Title;
        public DXLabel PageText;
        public DXImageControl Spacer;

        private List<DXLabel> Buttons = new();

        #endregion

        public HelpSectionInfo Info;

        private const int Width = 500;
        private const int TitleWidth = 120;
        private const int ContentWidth = 345;

        private const int SpacerLeft = 50;
        private const int TitleLeft = 20;
        private const int ContentLeft = 150;

        private bool Processed = false;

        private readonly Regex C = DrawTextExtensions.ColourRegex();

        public HelpSection()
        {
            Size = new Size(Width, 40);

            Title = new DXLabel
            {
                Parent = this,
                Text = "",
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.NoPrefix, 
                IsControl = false,
                Size = new Size(TitleWidth, 20),
                AutoSize = false
            };

            PageText = new DXLabel
            {
                Parent = this,
                Text = "",
                ForeColour = Color.White,
                Outline = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.NoPrefix,
                IsControl = false,
                Size = new Size(ContentWidth, 0),
                AutoSize = false,
            };

            Spacer = new DXImageControl
            {
                Index = 9315,
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                IsControl = false
            };
        }

        #region Methods

        public void Update()
        {
            if (Processed) return;

            Title.Text = Info.Title;

            var text = Info.Content;

            text = C.Replace(text, @"${Text}");
            PageText.Text = text;

            ProcessText(Info.Content);

            SetSize();

            Processed = true;
        }

        private void ProcessText(string page)
        {
            foreach (DXLabel label in Buttons)
                label.Dispose();

            Buttons.Clear();

            List<DXButtonIndex> buttonRanges = new();

            List<Match> matchList = new();
            matchList.AddRange(C.Matches(page).Cast<Match>());

            matchList = matchList.OrderBy(x => x.Groups["Text"].Index).ToList();

            int offset = 1;
            foreach (Match match in matchList)
            {
                DXButtonIndex index = new()
                {
                    Range = new CharacterRange(match.Groups["Text"].Index - offset, match.Groups["Text"].Length)
                };

                buttonRanges.Add(index);

                if (!string.IsNullOrEmpty(match.Groups["Colour"].Value))
                {
                    index.Type = DXButtonType.Label;
                    offset += 3 + match.Groups["Colour"].Length;
                }
            }

            for (int i = 0; i < buttonRanges.Count; i++)
            {
                var buttonIndex = buttonRanges[i];

                List<ButtonInfo> buttons = DrawTextExtensions.GetWordRegionsNew(PageText.Text, PageText.Font, PageText.DrawFormat, PageText.Size.Width, buttonIndex.Range.First, buttonIndex.Range.Length);

                List<DXLabel> labels = new();

                foreach (ButtonInfo info in buttons)
                {
                    labels.Add(new DXLabel
                    {
                        AutoSize = false,
                        Parent = PageText,
                        Location = info.Region.Location,
                        DrawFormat = PageText.DrawFormat,
                        Text = PageText.Text.Substring(info.Index, info.Length),
                        Font = PageText.Font,
                        Size = info.Region.Size,
                        Outline = false
                    });
                }

                int index = i;
                DateTime NextButtonTime = DateTime.MinValue;
                foreach (DXLabel label in labels)
                {
                    switch (buttonIndex.Type)
                    {
                        case DXButtonType.Label:
                            {
                                label.ForeColour = Color.FromName(matchList[index].Groups["Colour"].Value);
                            }
                            break;
                    }

                    Buttons.Add(label);
                }
            }
        }

        private void SetSize()
        {
            int bottomPadding = 5;
            int bottomMargin = 5;

            var titleHeight = Title.Size.Height;

            int pageTextHeight = DXLabel.GetHeight(PageText, PageText.Size.Width).Height;

            var biggestHeight = Math.Max(titleHeight, pageTextHeight);

            Title.Location = new Point(TitleLeft, 0);
            PageText.Location = new Point(ContentLeft, 0);

            Title.Size = DXLabel.GetHeight(Title, TitleWidth);
            PageText.Size = new Size(ContentWidth, pageTextHeight);

            Size = new Size(Width, biggestHeight + bottomPadding + 0 + bottomMargin);

            Spacer.Location = new Point(SpacerLeft, biggestHeight + bottomPadding + 2);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Title != null)
                {
                    if (!Title.IsDisposed)
                        Title.Dispose();

                    Title = null;
                }

                if (PageText != null)
                {
                    if (!PageText.IsDisposed)
                        PageText.Dispose();

                    PageText = null;
                }

                if (Spacer != null)
                {
                    if (!Spacer.IsDisposed)
                        Spacer.Dispose();

                    Spacer = null;
                }

                if (Buttons != null)
                {
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        if (Buttons[i] != null)
                        {
                            if (!Buttons[i].IsDisposed)
                                Buttons[i].Dispose();

                            Buttons[i] = null;
                        }
                    }

                    Buttons.Clear();
                    Buttons = null;
                }
            }
        }

        #endregion
    }
}

using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class ConsignmentDialog : DXImageControl
    {
        public const int VisibleRowCount = 6;

        public DXButton CloseButton, SearchButton, BuyButton, ConsignButton;
        public DXCheckBox BuyGuildBox, ConsignGuildBox;
        public DXLabel TitleLabel, ResultCountLabel, ConsignResultCountLabel;
        public DXLabel SortLabel, ItemTypesLabel, SearchNameLabel, SearchLevelLabel, SearchPriceLabel, SellerLabel;
        public DXLabel ConsignNameLabel, ConsignLevelLabel, ConsignPriceLabel, ConsignDateLabel;
        public DXTabControl TabControl;
        public DXTab SearchTab, ConsignTab;
        public DXImageControl TabImage;
        public DXTextBox SearchBox;
        public DXComboBox SortBox;
        public ConsignmentItemTypeMenu ItemTypeMenu;
        public DXVScrollBar SearchScrollBar;
        public ConsignmentSearchRow[] SearchRows;
        public ClientMarketPlaceInfo[] SearchResults;
        public DXVScrollBar ConsignScrollBar;
        public ConsignmentListRow[] ConsignRows;
        public List<ClientMarketPlaceInfo> ConsignItems = new List<ClientMarketPlaceInfo>();
        public ConsignItemDialog ConsignItemBox;

        public WindowSetting Settings;
        public WindowType Type => WindowType.ConsignmentBox;

        private ConsignmentSearchRow _SelectedRow;
        private ConsignmentListRow _SelectedConsignRow;
        public ConsignmentSearchRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                if (_SelectedRow != null)
                    _SelectedRow.Selected = false;

                _SelectedRow = value;

                if (_SelectedRow != null)
                    _SelectedRow.Selected = true;

                BuyButton.Enabled = !GameScene.Game.Observer && _SelectedRow?.MarketInfo?.Item != null;
            }
        }

        public ConsignmentListRow SelectedConsignRow
        {
            get => _SelectedConsignRow;
            set
            {
                if (_SelectedConsignRow == value) return;

                if (_SelectedConsignRow != null)
                    _SelectedConsignRow.Selected = false;

                _SelectedConsignRow = value;

                if (_SelectedConsignRow != null)
                    _SelectedConsignRow.Selected = true;
            }
        }

        public ConsignmentDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 300;
            Movable = true;
            Sort = true;
            DropShadow = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft,
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 3, 3);
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.ConsignmentDialogTitle,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 8);

            TabControl = new DXTabControl
            {
                Parent = this,
                Location = new Point(0, 37),
                Size = new Size(Size.Width, Size.Height - 42),
                MarginLeft = 10,
                Border = false,
            };

            TabImage = new DXImageControl
            {
                Parent = TabControl,
                LibraryFile = LibraryFile.Interface,
                Index = 301,
                Location = new Point(0, 23),
                IsControl = false,
            };

            SearchTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.ConsignmentDialogSearchTabLabel } },
                BackColour = Color.Empty,
                Border = false,
                Location = new Point(0, 23),
            };
            SearchTab.TabButton.MouseClick += (o, e) => SetActiveTab(true);

            ConsignTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.ConsignmentDialogConsignTabLabel } },
                BackColour = Color.Empty,
                Border = false,
                Location = new Point(0, 23),
            };
            ConsignTab.TabButton.MouseClick += (o, e) => SetActiveTab(false);

            SearchBox = new DXTextBox
            {
                Parent = SearchTab,
                Location = new Point(480, 8),
                Size = new Size(150, 18),
                Border = false,
            };
            SearchBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar != (char)Keys.Enter) return;
                e.Handled = true;
                Search();
            };

            SortBox = new DXComboBox
            {
                Parent = SearchTab,
                Location = new Point(62, 10),
                Size = new Size(105, DXComboBox.DefaultNormalHeight),
                Border = false
            };
            Type sortType = typeof(MarketPlaceSort);
            for (MarketPlaceSort sort = MarketPlaceSort.Newest; sort <= MarketPlaceSort.LowestPrice; sort++)
            {
                MemberInfo member = sortType.GetMember(sort.ToString())[0];
                DescriptionAttribute description = member.GetCustomAttribute<DescriptionAttribute>();
                new DXListBoxItem
                {
                    Parent = SortBox.ListBox,
                    Label = { Text = description?.Description ?? sort.ToString() },
                    Item = sort,
                };
            }
            SortBox.ListBox.SelectItem(MarketPlaceSort.Newest);

            SortLabel = CreateHeaderLabel(SearchTab, new Point(10, 6), new Size(50, 20), CEnvir.Language.ConsignmentDialogSortByLabel);
            ItemTypesLabel = CreateHeaderLabel(SearchTab, new Point(4, 32), new Size(160, 20), CEnvir.Language.ConsignmentDialogItemTypesLabel);
            SearchNameLabel = CreateHeaderLabel(SearchTab, new Point(180, 32), new Size(172, 20), CEnvir.Language.ConsignmentDialogNameLabel);
            SearchLevelLabel = CreateHeaderLabel(SearchTab, new Point(356, 32), new Size(55, 20), CEnvir.Language.ConsignmentDialogLevelLabel);
            SearchPriceLabel = CreateHeaderLabel(SearchTab, new Point(415, 32), new Size(110, 20), CEnvir.Language.ConsignmentDialogPriceLabel);
            SellerLabel = CreateHeaderLabel(SearchTab, new Point(525, 32), new Size(160, 20), CEnvir.Language.ConsignmentDialogSellerLabel);

            ConsignNameLabel = CreateHeaderLabel(ConsignTab, new Point(14, 32), new Size(250, 20), CEnvir.Language.ConsignmentDialogNameLabel);
            ConsignLevelLabel = CreateHeaderLabel(ConsignTab, new Point(260, 32), new Size(60, 20), CEnvir.Language.ConsignmentDialogLevelLabel);
            ConsignPriceLabel = CreateHeaderLabel(ConsignTab, new Point(325, 32), new Size(140, 20), CEnvir.Language.ConsignmentDialogPriceLabel);
            ConsignDateLabel = CreateHeaderLabel(ConsignTab, new Point(479, 32), new Size(200, 20), CEnvir.Language.ConsignmentDialogConsignDateLabel);

            SearchButton = new DXButton
            {
                Parent = SearchTab,
                Location = new Point(637, 6),
                Size = new Size(70, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.ConsignmentDialogSearchButtonLabel },
            };
            SearchButton.MouseClick += (o, e) => Search();

            ItemTypeMenu = new ConsignmentItemTypeMenu
            {
                Parent = SearchTab,
                Location = new Point(13, 50),
                Size = new Size(160, 268),
            };
            ItemTypeMenu.SelectedItemTypeChanged += (o, e) => Search();

            SearchRows = new ConsignmentSearchRow[VisibleRowCount];
            for (int i = 0; i < SearchRows.Length; i++)
            {
                int index = i;
                SearchRows[i] = new ConsignmentSearchRow
                {
                    Parent = SearchTab,
                    Location = new Point(180, 58 + i * 42),
                };
                SearchRows[i].MouseClick += (o, e) => SelectedRow = SearchRows[index];
                SearchRows[i].ItemCell.MouseClick += (o, e) => SelectedRow = SearchRows[index];
            }

            SearchScrollBar = new DXVScrollBar
            {
                Parent = SearchTab,
                Location = new Point(691, 52),
                Size = new Size(18, 268),
                VisibleSize = VisibleRowCount,
                Change = 1,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true,
            };
            SearchScrollBar.ValueChanged += (o, e) => RefreshList();
            foreach (ConsignmentSearchRow row in SearchRows)
                row.MouseWheel += SearchScrollBar.DoMouseWheel;

            ConsignRows = new ConsignmentListRow[VisibleRowCount];
            for (int i = 0; i < ConsignRows.Length; i++)
            {
                int index = i;
                ConsignRows[i] = new ConsignmentListRow
                {
                    Parent = ConsignTab,
                    Location = new Point(14, 58 + i * 42),
                };
                ConsignRows[i].MouseClick += (o, e) => CancelConsignment(index);
                ConsignRows[i].ItemCell.MouseClick += (o, e) => CancelConsignment(index);
            }

            ConsignScrollBar = new DXVScrollBar
            {
                Parent = ConsignTab,
                Location = new Point(691, 52),
                Size = new Size(18, 268),
                VisibleSize = VisibleRowCount,
                Change = 1,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true,
            };
            ConsignScrollBar.ValueChanged += (o, e) => RefreshConsignList();
            foreach (ConsignmentListRow row in ConsignRows)
                row.MouseWheel += ConsignScrollBar.DoMouseWheel;

            BuyButton = new DXButton
            {
                Parent = this,
                Location = new Point(Size.Width - 105, Size.Height - 42),
                Size = new Size(90, DefaultHeight),
                ButtonType = ButtonType.Default,
                Label = { Text = CEnvir.Language.ConsignmentDialogBuyButtonLabel },
                Enabled = false,
            };
            BuyButton.MouseClick += BuyButton_MouseClick;

            BuyGuildBox = new DXCheckBox
            {
                Parent = this,
                Location = new Point(Size.Width - 255, Size.Height - 38),
                Label = { Text = CEnvir.Language.ConsignmentDialogBuyGuildFundsLabel },
                Enabled = false,
            };

            ConsignButton = new DXButton
            {
                Parent = this,
                Location = BuyButton.Location,
                Size = BuyButton.Size,
                ButtonType = ButtonType.Default,
                Label = { Text = CEnvir.Language.ConsignmentDialogConsignButtonLabel },
                Visible = false,
            };
            ConsignButton.MouseClick += ConsignButton_MouseClick;

            ConsignGuildBox = new DXCheckBox
            {
                Parent = this,
                Location = BuyGuildBox.Location,
                Label = { Text = CEnvir.Language.ConsignmentDialogConsignGuildFundsLabel },
                Enabled = false,
                Visible = false,
            };

            ResultCountLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(38, Size.Height - 38),
                Size = new Size(106, 17),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.Yellow,
                Text = string.Format(CEnvir.Language.ConsignmentDialogResultCount, 1, 0),
                IsControl = false
            };

            ConsignResultCountLabel = new DXLabel
            {
                Parent = this,
                Location = ResultCountLabel.Location,
                Size = ResultCountLabel.Size,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.Yellow,
                Text = string.Format(CEnvir.Language.ConsignmentDialogResultCount, 1, 0),
                IsControl = false,
                Visible = false,
            };

            SearchTab.TabButton.InvokeMouseClick();
        }

        private static DXLabel CreateHeaderLabel(DXControl parent, Point location, Size size, string text)
        {
            return new DXLabel
            {
                Parent = parent,
                Location = location,
                Size = size,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = text,
                ForeColour = Color.FromArgb(198, 166, 99),
                IsControl = false,
            };
        }

        private void SetActiveTab(bool search)
        {
            TabImage.Index = search ? 301 : 302;
            BuyButton.Visible = search;
            ConsignButton.Visible = !search;
            BuyGuildBox.Visible = search;
            ConsignGuildBox.Visible = !search;
            ResultCountLabel.Visible = search;
            ConsignResultCountLabel.Visible = !search;

            if (search)
                SelectedConsignRow = null;
            else
            {
                SelectedRow = null;
                RefreshConsignList();
            }
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
            {
                BringToFront();
                if (SearchResults == null)
                    Search();
            }
            else if (ConsignItemBox != null && !ConsignItemBox.IsDisposed)
            {
                ConsignItemBox.Dispose();
            }

            if (Settings != null)
                Settings.Visible = false;

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

            if (e.KeyCode != Keys.Escape || !CloseButton.Visible) return;

            CloseButton.InvokeMouseClick();
            if (!Config.EscapeCloseAll)
                e.Handled = true;
        }

        public void LoadSettings()
        {
            if (!CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);
            if (Settings != null)
            {
                Location = Settings.Location;
                Visible = false;
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = false;
            Settings.Location = Location;
        }

        public void Search()
        {
            SelectedRow = null;
            SearchResults = null;
            SearchScrollBar.Value = 0;
            SearchScrollBar.MaxValue = 0;
            ResultCountLabel.Text = string.Format(CEnvir.Language.ConsignmentDialogResultCount, 1, 0);

            foreach (ConsignmentSearchRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            CEnvir.Enqueue(new C.MarketPlaceSearch
            {
                Name = SearchBox.TextBox.Text,
                ItemTypeFilter = ItemTypeMenu.SelectedItemType.HasValue,
                ItemType = ItemTypeMenu.SelectedItemType ?? ItemType.Nothing,
                Sort = (MarketPlaceSort?)SortBox.SelectedItem ?? MarketPlaceSort.Newest,
            });
        }

        public void ApplySearch(int count, IList<ClientMarketPlaceInfo> results)
        {
            SearchResults = new ClientMarketPlaceInfo[count];
            for (int i = 0; i < results.Count && i < SearchResults.Length; i++)
                SearchResults[i] = results[i];
            RefreshList();
        }

        public void ApplySearchCount(int count)
        {
            if (SearchResults == null)
                SearchResults = new ClientMarketPlaceInfo[count];
            else
                Array.Resize(ref SearchResults, count);
            RefreshList();
        }

        public void ApplySearchIndex(int index, ClientMarketPlaceInfo result)
        {
            if (SearchResults == null || index < 0 || index >= SearchResults.Length) return;
            SearchResults[index] = result;
            RefreshList();
        }

        public void ApplyBuy(int index, long count, bool success)
        {
            BuyButton.Enabled = !GameScene.Game.Observer && SelectedRow?.MarketInfo?.Item != null;
            if (!success || SearchResults == null) return;

            ClientMarketPlaceInfo info = SearchResults.FirstOrDefault(x => x != null && x.Index == index);
            if (info == null) return;

            if (count > 0)
                info.Item.Count = count;
            else
                info.Item = null;

            SelectedRow = null;
            RefreshList();
        }

        public void AddConsignments(IEnumerable<ClientMarketPlaceInfo> consignments)
        {
            if (consignments == null) return;

            foreach (ClientMarketPlaceInfo info in consignments)
            {
                int index = ConsignItems.FindIndex(x => x.Index == info.Index);
                if (index >= 0)
                    ConsignItems[index] = info;
                else
                    ConsignItems.Add(info);
            }

            RefreshConsignList();
        }

        public void ApplyConsignChanged(int index, long count)
        {
            ClientMarketPlaceInfo info = ConsignItems.FirstOrDefault(x => x.Index == index);
            if (info == null) return;

            if (count > 0)
                info.Item.Count = count;
            else
                ConsignItems.Remove(info);

            RefreshConsignList();
        }

        public void RefreshConsignList()
        {
            if (ConsignRows == null || ConsignScrollBar == null) return;

            SelectedConsignRow = null;
            ConsignScrollBar.MaxValue = ConsignItems.Count;
            ConsignResultCountLabel.Text = string.Format(CEnvir.Language.ConsignmentDialogResultCount, ConsignScrollBar.Value + 1, ConsignItems.Count);

            for (int i = 0; i < ConsignRows.Length; i++)
            {
                int resultIndex = i + ConsignScrollBar.Value;
                ConsignRows[i].MarketInfo = resultIndex < ConsignItems.Count ? ConsignItems[resultIndex] : null;
            }
        }

        public void RefreshList()
        {
            if (SearchResults == null) return;

            SelectedRow = null;
            SearchScrollBar.MaxValue = SearchResults.Length;
            ResultCountLabel.Text = string.Format(CEnvir.Language.ConsignmentDialogResultCount, SearchScrollBar.Value + 1, SearchResults.Length);

            for (int i = 0; i < SearchRows.Length; i++)
            {
                int resultIndex = i + SearchScrollBar.Value;
                ConsignmentSearchRow row = SearchRows[i];

                if (resultIndex >= SearchResults.Length)
                {
                    row.MarketInfo = null;
                    row.Loading = false;
                    row.Visible = false;
                    continue;
                }

                if (SearchResults[resultIndex] == null)
                {
                    row.Loading = true;
                    row.Visible = true;
                    SearchResults[resultIndex] = new ClientMarketPlaceInfo { Loading = true };
                    CEnvir.Enqueue(new C.MarketPlaceSearchIndex { Index = resultIndex });
                    continue;
                }

                if (SearchResults[resultIndex].Loading)
                {
                    row.Loading = true;
                    row.Visible = true;
                    continue;
                }

                row.Loading = false;
                row.MarketInfo = SearchResults[resultIndex];
                row.Visible = true;
            }
        }

        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            ClientMarketPlaceInfo info = SelectedRow?.MarketInfo;
            if (info?.Item == null || GameScene.Game.Observer) return;

            DXItemAmountWindow window = new DXItemAmountWindow(CEnvir.Language.ConsignmentDialogBuyItemCaption, info.Item);
            window.ConfirmButton.MouseClick += (o, args) => ConfirmPurchase(info, window.Amount);
        }

        private void ConfirmPurchase(ClientMarketPlaceInfo info, long count)
        {
            if (info?.Item == null || count <= 0 || count > info.Item.Count) return;

            long total = count * info.Price;
            string message = string.Format(CEnvir.Language.ConsignmentDialogBuyConfirmMessage, info.Item.Info.ItemName, count, info.Price, total);
            if (BuyGuildBox.Checked)
                message += CEnvir.Language.ConsignmentDialogUsingGuildFunds;

            DXMessageBox box = new DXMessageBox(message, CEnvir.Language.ConsignmentDialogBuyConfirmCaption, DXMessageBoxButtons.YesNo);
            box.YesButton.MouseClick += (o, args) =>
            {
                BuyButton.Enabled = false;
                CEnvir.Enqueue(new C.MarketPlaceBuy { Index = info.Index, Count = count, GuildFunds = BuyGuildBox.Checked });
                BuyGuildBox.Checked = false;
            };
        }

        private void CancelConsignment(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= ConsignRows.Length) return;

            ConsignmentListRow row = ConsignRows[rowIndex];
            ClientMarketPlaceInfo info = row.MarketInfo;
            if (info?.Item == null) return;

            SelectedConsignRow = row;

            DXItemAmountWindow window = new DXItemAmountWindow(CEnvir.Language.ConsignmentDialogCancelListingCaption, info.Item);
            window.ConfirmButton.MouseClick += (o, e) =>
            {
                if (window.Amount <= 0) return;

                CEnvir.Enqueue(new C.MarketPlaceCancelConsign { Index = info.Index, Count = window.Amount });
            };
        }

        private void ConsignButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (ConsignItemBox == null || ConsignItemBox.IsDisposed)
                ConsignItemBox = new ConsignItemDialog(this);
            else
                ConsignItemBox.BringToFront();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Settings = null;
            SearchResults = null;
            ConsignItems = null;
            _SelectedRow = null;
            _SelectedConsignRow = null;

            if (ConsignItemBox != null && !ConsignItemBox.IsDisposed)
                ConsignItemBox.Dispose();
            ConsignItemBox = null;

            if (CloseButton != null && !CloseButton.IsDisposed)
                CloseButton.Dispose();
            CloseButton = null;

            if (SearchButton != null && !SearchButton.IsDisposed)
                SearchButton.Dispose();
            SearchButton = null;

            if (BuyButton != null && !BuyButton.IsDisposed)
                BuyButton.Dispose();
            BuyButton = null;

            if (BuyGuildBox != null && !BuyGuildBox.IsDisposed)
                BuyGuildBox.Dispose();
            BuyGuildBox = null;

            if (ConsignButton != null && !ConsignButton.IsDisposed)
                ConsignButton.Dispose();
            ConsignButton = null;

            if (ConsignGuildBox != null && !ConsignGuildBox.IsDisposed)
                ConsignGuildBox.Dispose();
            ConsignGuildBox = null;

            if (ResultCountLabel != null && !ResultCountLabel.IsDisposed)
                ResultCountLabel.Dispose();
            ResultCountLabel = null;

            if (TitleLabel != null && !TitleLabel.IsDisposed)
                TitleLabel.Dispose();
            TitleLabel = null;

            if (ConsignResultCountLabel != null && !ConsignResultCountLabel.IsDisposed)
                ConsignResultCountLabel.Dispose();
            ConsignResultCountLabel = null;

            if (SortLabel != null && !SortLabel.IsDisposed)
                SortLabel.Dispose();
            SortLabel = null;

            if (ItemTypesLabel != null && !ItemTypesLabel.IsDisposed)
                ItemTypesLabel.Dispose();
            ItemTypesLabel = null;

            if (SearchNameLabel != null && !SearchNameLabel.IsDisposed)
                SearchNameLabel.Dispose();
            SearchNameLabel = null;

            if (SearchLevelLabel != null && !SearchLevelLabel.IsDisposed)
                SearchLevelLabel.Dispose();
            SearchLevelLabel = null;

            if (SearchPriceLabel != null && !SearchPriceLabel.IsDisposed)
                SearchPriceLabel.Dispose();
            SearchPriceLabel = null;

            if (SellerLabel != null && !SellerLabel.IsDisposed)
                SellerLabel.Dispose();
            SellerLabel = null;

            if (ConsignNameLabel != null && !ConsignNameLabel.IsDisposed)
                ConsignNameLabel.Dispose();
            ConsignNameLabel = null;

            if (ConsignLevelLabel != null && !ConsignLevelLabel.IsDisposed)
                ConsignLevelLabel.Dispose();
            ConsignLevelLabel = null;

            if (ConsignPriceLabel != null && !ConsignPriceLabel.IsDisposed)
                ConsignPriceLabel.Dispose();
            ConsignPriceLabel = null;

            if (ConsignDateLabel != null && !ConsignDateLabel.IsDisposed)
                ConsignDateLabel.Dispose();
            ConsignDateLabel = null;

            if (SearchBox != null && !SearchBox.IsDisposed)
                SearchBox.Dispose();
            SearchBox = null;

            if (SortBox != null && !SortBox.IsDisposed)
                SortBox.Dispose();
            SortBox = null;

            if (ItemTypeMenu != null && !ItemTypeMenu.IsDisposed)
                ItemTypeMenu.Dispose();
            ItemTypeMenu = null;

            if (SearchScrollBar != null && !SearchScrollBar.IsDisposed)
                SearchScrollBar.Dispose();
            SearchScrollBar = null;

            if (ConsignScrollBar != null && !ConsignScrollBar.IsDisposed)
                ConsignScrollBar.Dispose();
            ConsignScrollBar = null;

            if (SearchRows != null)
            {
                for (int i = 0; i < SearchRows.Length; i++)
                {
                    if (SearchRows[i] != null && !SearchRows[i].IsDisposed)
                        SearchRows[i].Dispose();
                    SearchRows[i] = null;
                }
                SearchRows = null;
            }

            if (ConsignRows != null)
            {
                for (int i = 0; i < ConsignRows.Length; i++)
                {
                    if (ConsignRows[i] != null && !ConsignRows[i].IsDisposed)
                        ConsignRows[i].Dispose();
                    ConsignRows[i] = null;
                }
                ConsignRows = null;
            }

            if (TabImage != null && !TabImage.IsDisposed)
                TabImage.Dispose();
            TabImage = null;

            if (SearchTab != null && !SearchTab.IsDisposed)
                SearchTab.Dispose();
            SearchTab = null;

            if (ConsignTab != null && !ConsignTab.IsDisposed)
                ConsignTab.Dispose();
            ConsignTab = null;

            if (TabControl != null && !TabControl.IsDisposed)
                TabControl.Dispose();
            TabControl = null;
        }
    }

    public sealed class ConsignmentItemTypeMenu : DXControl
    {
        private const int RowHeight = 21;
        private const int VisibleRows = 12;

        public DXControl Container;
        public DXVScrollBar ScrollBar;
        public List<DXButton> Buttons = new List<DXButton>();
        public ItemType? SelectedItemType { get; private set; }
        public event EventHandler<EventArgs> SelectedItemTypeChanged;

        public ConsignmentItemTypeMenu()
        {
            Container = new DXControl
            {
                Parent = this,
                Location = new Point(0, 5),
                Size = new Size(140, 260),
                PassThrough = true,
            };
            Container.MouseWheel += ScrollBar_DoMouseWheel;

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(136, 0),
                Size = new Size(18, 272),
                VisibleSize = VisibleRows,
                Change = 1,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            Add(CEnvir.Language.ConsignmentDialogAllLabel, null);
            Type enumType = typeof(ItemType);
            foreach (ItemType itemType in Enum.GetValues(enumType))
            {
                if (itemType == ItemType.Nothing) continue;

                MemberInfo member = enumType.GetMember(itemType.ToString())[0];
                DescriptionAttribute description = member.GetCustomAttribute<DescriptionAttribute>();
                Add(description?.Description ?? itemType.ToString(), itemType);
            }

            Select(null);
        }

        private void Add(string text, ItemType? itemType)
        {
            DXButton button = new DXButton
            {
                Parent = Container,
                LibraryFile = LibraryFile.GameInter,
                Index = 831,
                Size = new Size(136, 18),
                Label = { Text = text, ForeColour = Constants.InactiveTabColour },
                Tag = itemType,
            };
            button.MouseClick += (o, e) => Select((ItemType?)((DXButton)o).Tag);
            button.MouseWheel += ScrollBar.DoMouseWheel;
            Buttons.Add(button);
            ScrollBar.MaxValue = Buttons.Count;
            UpdateLocations();
        }

        private void Select(ItemType? itemType)
        {
            if (SelectedItemType == itemType && Buttons.Any(x => x.Label.ForeColour == Constants.ActiveTabColour)) return;

            SelectedItemType = itemType;
            foreach (DXButton button in Buttons)
            {
                bool selected = Equals(button.Tag, itemType);
                button.Index = selected ? 830 : 831;
                button.Label.ForeColour = selected ? Constants.ActiveTabColour : Constants.InactiveTabColour;
            }

            SelectedItemTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateLocations()
        {
            for (int i = 0; i < Buttons.Count; i++)
                Buttons[i].Location = new Point(0, (i - ScrollBar.Value) * RowHeight);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            SelectedItemTypeChanged = null;
            SelectedItemType = null;

            if (ScrollBar != null && !ScrollBar.IsDisposed)
                ScrollBar.Dispose();
            ScrollBar = null;

            if (Container != null && !Container.IsDisposed)
                Container.Dispose();
            Container = null;

            foreach (DXButton button in Buttons)
                if (button != null && !button.IsDisposed)
                    button.Dispose();
            Buttons.Clear();
            Buttons = null;
        }

        private void ScrollBar_DoMouseWheel(object sender, MouseEventArgs e)
        {
            ScrollBar?.DoMouseWheel(sender, e);
        }
    }

    public sealed class ConsignItemDialog : DXControl
    {
        private readonly ConsignmentDialog _Owner;
        private int _Price;

        public DXImageControl Header, Body, Footer, ItemContainer;
        public DXButton CloseButton, DownButton, UpButton, ConfirmButton, CancelButton;
        public DXItemGrid ItemGrid;
        public DXLabel ItemNameLabel, PriceLabel;

        public int Price
        {
            get => _Price;
            set
            {
                _Price = Math.Max(0, value);
                PriceLabel.Text = _Price.ToString("#,##0");
            }
        }

        public ConsignItemDialog(ConsignmentDialog owner)
        {
            _Owner = owner;
            Parent = GameScene.Game;
            Size = new Size(296, 228);
            Location = new Point((GameScene.Game.Size.Width - Size.Width) / 2, (GameScene.Game.Size.Height - Size.Height) / 2);
            Movable = true;
            Sort = true;

            Header = CreatePart(303, Point.Empty);
            Body = CreatePart(304, new Point(0, 60));
            Footer = CreatePart(305, new Point(0, 144));

            CloseButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 15,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft,
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 4, 4);
            CloseButton.MouseClick += (o, e) => Dispose();

            ItemContainer = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 306,
                Location = new Point(24, 88),
                IsControl = false,
            };

            ItemGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Location = new Point(36, 98),
                Parent = this,
                Border = false,
                Linked = true,
                GridType = GridType.Consign,
            };
            ItemGrid.Grid[0].LinkChanged += ItemCell_LinkChanged;

            ItemNameLabel = CreateLabel(new Point(89, 96), new Size(155, 20));
            PriceLabel = CreateLabel(new Point(89, 122), new Size(155, 20));

            UpButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 387,
                Location = new Point(246, 96),
            };
            UpButton.MouseClick += (o, e) => Price = (int)Math.Min(int.MaxValue, (long)Price + 5000);

            DownButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 385,
                Location = new Point(246, 122),
            };
            DownButton.MouseClick += (o, e) => Price = Math.Max(0, Price - 5000);

            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(43, 178),
                Size = new Size(80, DefaultHeight),
                ButtonType = ButtonType.Default,
                Label = { Text = CEnvir.Language.ConsignmentDialogConsignButtonLabel },
            };
            ConfirmButton.MouseClick += ConfirmButton_MouseClick;

            CancelButton = new DXButton
            {
                Parent = this,
                Location = new Point(173, 178),
                Size = new Size(80, DefaultHeight),
                ButtonType = ButtonType.Default,
                Label = { Text = CEnvir.Language.CommonControlCancel },
            };
            CancelButton.MouseClick += (o, e) => Dispose();

            Price = 0;
        }

        private DXImageControl CreatePart(int index, Point location)
        {
            return new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = index,
                Location = location,
                IsControl = false,
            };
        }

        private DXLabel CreateLabel(Point location, Size size)
        {
            return new DXLabel
            {
                Parent = this,
                Location = location,
                Size = size,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        private void ItemCell_LinkChanged(object sender, EventArgs e)
        {
            DXItemCell cell = ItemGrid.Grid[0];
            if (cell.Item == null)
            {
                ItemNameLabel.Text = string.Empty;
                Price = 0;
                return;
            }

            ItemInfo displayInfo = cell.Item.Info;
            if (displayInfo.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == cell.Item.AddedStats[Stat.ItemIndex]) ?? displayInfo;

            ItemNameLabel.Text = displayInfo.ItemName;

            DXInputWindow window = new DXInputWindow(CEnvir.Language.ConsignmentDialogPricePrompt, CEnvir.Language.ConsignmentDialogPriceLabel);
            window.ConfirmButton.MouseClick += (o, args) =>
            {
                if (int.TryParse(window.Value?.Replace(",", string.Empty), out int price) && price > 0)
                    Price = price;
                else
                    GameScene.Game.ReceiveChat(CEnvir.Language.MarketInvalidPrice, MessageType.System);
            };
        }

        private void ConfirmButton_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell cell = ItemGrid.Grid[0];
            if (cell.Item == null)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.MarketNoItemSelected, MessageType.System);
                return;
            }

            if (Price <= 0)
            {
                GameScene.Game.ReceiveChat(CEnvir.Language.MarketInvalidPrice, MessageType.System);
                return;
            }

            string message = string.Format(CEnvir.Language.ConsignmentDialogConsignConfirmMessage, ItemNameLabel.Text, cell.LinkedCount, Price, Globals.MarketPlaceFee);
            if (_Owner.ConsignGuildBox.Checked)
                message += CEnvir.Language.ConsignmentDialogUsingGuildFunds;
            DXMessageBox box = new DXMessageBox(message, CEnvir.Language.ConsignmentDialogConsignConfirmCaption, DXMessageBoxButtons.YesNo);
            box.YesButton.MouseClick += (o, args) =>
            {
                CEnvir.Enqueue(new C.MarketPlaceConsign
                {
                    Link = new CellLinkInfo { GridType = cell.Link.GridType, Count = cell.LinkedCount, Slot = cell.Link.Slot },
                    Price = Price,
                    Message = string.Empty,
                    GuildFunds = _Owner.ConsignGuildBox.Checked,
                });

                _Owner.ConsignGuildBox.Checked = false;
                cell.Link.Locked = true;
                cell.Link = null;
                Dispose();
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            if (_Owner != null && _Owner.ConsignItemBox == this)
                _Owner.ConsignItemBox = null;

            if (CloseButton != null && !CloseButton.IsDisposed)
                CloseButton.Dispose();
            CloseButton = null;

            if (DownButton != null && !DownButton.IsDisposed)
                DownButton.Dispose();
            DownButton = null;

            if (UpButton != null && !UpButton.IsDisposed)
                UpButton.Dispose();
            UpButton = null;

            if (ConfirmButton != null && !ConfirmButton.IsDisposed)
                ConfirmButton.Dispose();
            ConfirmButton = null;

            if (CancelButton != null && !CancelButton.IsDisposed)
                CancelButton.Dispose();
            CancelButton = null;

            if (ItemGrid != null && !ItemGrid.IsDisposed)
                ItemGrid.Dispose();
            ItemGrid = null;

            if (ItemNameLabel != null && !ItemNameLabel.IsDisposed)
                ItemNameLabel.Dispose();
            ItemNameLabel = null;

            if (PriceLabel != null && !PriceLabel.IsDisposed)
                PriceLabel.Dispose();
            PriceLabel = null;

            if (ItemContainer != null && !ItemContainer.IsDisposed)
                ItemContainer.Dispose();
            ItemContainer = null;


            if (Header != null && !Header.IsDisposed)
                Header.Dispose();
            Header = null;

            if (Body != null && !Body.IsDisposed)
                Body.Dispose();
            Body = null;

            if (Footer != null && !Footer.IsDisposed)
                Footer.Dispose();
            Footer = null;
        }
    }

    public sealed class ConsignmentListRow : DXControl
    {
        private ClientMarketPlaceInfo _MarketInfo;
        private bool _Selected;

        public DXImageControl SelectedImage;
        public DXItemCell ItemCell;
        public DXLabel NameLabel, LevelLabel, PriceLabel, DateLabel;

        public ClientMarketPlaceInfo MarketInfo
        {
            get => _MarketInfo;
            set
            {
                _MarketInfo = value;
                RefreshInfo();
            }
        }

        public bool Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                SelectedImage.Visible = value;
            }
        }

        public ConsignmentListRow()
        {
            Size = new Size(680, 42);
            Visible = false;

            SelectedImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 811,
                IsControl = false,
                Visible = false,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(23, 3),
                ReadOnly = true,
                Border = true,
                FixedBorder = false,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true,
            };

            NameLabel = CreateLabel(new Point(65, 10), new Size(180, 20), TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            LevelLabel = CreateLabel(new Point(250, 10), new Size(54, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            PriceLabel = CreateLabel(new Point(307, 10), new Size(145, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            DateLabel = CreateLabel(new Point(460, 10), new Size(210, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private DXLabel CreateLabel(Point location, Size size, TextFormatFlags format)
        {
            return new DXLabel
            {
                Parent = this,
                Location = location,
                Size = size,
                AutoSize = false,
                DrawFormat = format,
                ForeColour = Color.White,
                IsControl = false
            };
        }

        private void RefreshInfo()
        {
            Selected = false;
            Visible = MarketInfo?.Item != null;

            if (!Visible)
            {
                ItemCell.Item = null;
                NameLabel.Text = string.Empty;
                LevelLabel.Text = string.Empty;
                PriceLabel.Text = string.Empty;
                DateLabel.Text = string.Empty;
                return;
            }

            ItemCell.Item = MarketInfo.Item;
            ItemCell.RefreshItem();

            ItemInfo displayInfo = MarketInfo.Item.Info;
            if (displayInfo.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == MarketInfo.Item.AddedStats[Stat.ItemIndex]) ?? displayInfo;

            int requiredLevel = displayInfo.RequiredAmount;
            NameLabel.Text = displayInfo.ItemName;
            LevelLabel.Text = requiredLevel.ToString("#,##0");
            LevelLabel.ForeColour = MapObject.User.Level < requiredLevel ? Color.Red : Color.White;
            PriceLabel.Text = MarketInfo.Price.ToString("#,##0");
            DateLabel.Text = MarketInfo.ConsignDate == DateTime.MinValue ? string.Empty : MarketInfo.ConsignDate.ToString("yyyy-MM-dd");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            _MarketInfo = null;
            _Selected = false;

            if (SelectedImage != null && !SelectedImage.IsDisposed)
                SelectedImage.Dispose();
            SelectedImage = null;

            if (ItemCell != null && !ItemCell.IsDisposed)
                ItemCell.Dispose();
            ItemCell = null;

            if (NameLabel != null && !NameLabel.IsDisposed)
                NameLabel.Dispose();
            NameLabel = null;

            if (LevelLabel != null && !LevelLabel.IsDisposed)
                LevelLabel.Dispose();
            LevelLabel = null;

            if (PriceLabel != null && !PriceLabel.IsDisposed)
                PriceLabel.Dispose();
            PriceLabel = null;

            if (DateLabel != null && !DateLabel.IsDisposed)
                DateLabel.Dispose();
            DateLabel = null;
        }
    }

    public sealed class ConsignmentSearchRow : DXControl
    {
        private ClientMarketPlaceInfo _MarketInfo;
        private bool _Selected;
        private bool _Loading;

        public DXImageControl SelectedImage;
        public DXItemCell ItemCell;
        public DXLabel NameLabel, LevelLabel, PriceLabel, SellerLabel;

        public ClientMarketPlaceInfo MarketInfo
        {
            get => _MarketInfo;
            set
            {
                _MarketInfo = value;
                RefreshInfo();
            }
        }

        public bool Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                SelectedImage.Visible = value;
            }
        }

        public bool Loading
        {
            get => _Loading;
            set
            {
                _Loading = value;
                if (value)
                {
                    _MarketInfo = null;
                    NameLabel.Text = CEnvir.Language.ConsignmentDialogLoadingLabel;
                    ItemCell.Item = null;
                    LevelLabel.Text = string.Empty;
                    PriceLabel.Text = string.Empty;
                    SellerLabel.Text = string.Empty;
                }
            }
        }

        public ConsignmentSearchRow()
        {
            Size = new Size(512, 42);
            Visible = false;

            SelectedImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 810,
                IsControl = false,
                Visible = false,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(11, 3),
                ReadOnly = true,
                Border = false,
                FixedBorder = false,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                ShowCountLabel = true,
            };

            NameLabel = CreateLabel(new Point(52, 10), new Size(120, 20), TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            LevelLabel = CreateLabel(new Point(176, 10), new Size(55, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            PriceLabel = CreateLabel(new Point(235, 10), new Size(110, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            SellerLabel = CreateLabel(new Point(345, 10), new Size(160, 20), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
        }

        private DXLabel CreateLabel(Point location, Size size, TextFormatFlags format)
        {
            return new DXLabel
            {
                Parent = this,
                Location = location,
                Size = size,
                AutoSize = false,
                DrawFormat = format,
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        private void RefreshInfo()
        {
            Selected = false;
            Visible = MarketInfo != null;
            if (MarketInfo == null) return;

            ItemCell.Item = MarketInfo.Item;
            ItemCell.RefreshItem();

            ItemInfo displayInfo = MarketInfo.Item?.Info;
            if (MarketInfo.Item?.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == MarketInfo.Item.AddedStats[Stat.ItemIndex]);

            NameLabel.Text = displayInfo?.ItemName ?? CEnvir.Language.ConsignmentDialogItemSoldLabel;
            int requiredLevel = displayInfo?.RequiredAmount ?? 0;
            LevelLabel.Text = requiredLevel.ToString("#,##0");
            LevelLabel.ForeColour = MapObject.User.Level < requiredLevel ? Color.Red : Color.White;
            PriceLabel.Text = MarketInfo.Price.ToString("#,##0");
            SellerLabel.Text = MarketInfo.Seller;
            SellerLabel.ForeColour = MarketInfo.IsOwner ? Color.Yellow : Color.White;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            _MarketInfo = null;
            _Selected = false;
            _Loading = false;

            if (SelectedImage != null && !SelectedImage.IsDisposed)
                SelectedImage.Dispose();
            SelectedImage = null;

            if (ItemCell != null && !ItemCell.IsDisposed)
                ItemCell.Dispose();
            ItemCell = null;

            if (NameLabel != null && !NameLabel.IsDisposed)
                NameLabel.Dispose();
            NameLabel = null;

            if (LevelLabel != null && !LevelLabel.IsDisposed)
                LevelLabel.Dispose();
            LevelLabel = null;

            if (PriceLabel != null && !PriceLabel.IsDisposed)
                PriceLabel.Dispose();
            PriceLabel = null;

            if (SellerLabel != null && !SellerLabel.IsDisposed)
                SellerLabel.Dispose();
            SellerLabel = null;
        }
    }
}

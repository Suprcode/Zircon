using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class GameStoreTreeFilter
    {
        public GameStoreCategory Category { get; }
        public ItemType? ItemType { get; }
        public string StoreFilter { get; }
        public bool RequiresStoreFilter { get; }

        public GameStoreTreeFilter(GameStoreCategory category, ItemType? itemType = null, string storeFilter = null, bool requiresStoreFilter = false)
        {
            Category = category;
            ItemType = itemType;
            StoreFilter = storeFilter;
            RequiresStoreFilter = requiresStoreFilter;
        }
    }

    public sealed class GameStoreDialog : DXImageControl
    {
        public DXButton CloseButton;
        public DXTreeControl FolderTree;
        public GameStoreItemListControl ItemList;
        public GameStoreTopItemsControl TopItems;
        public DXComboBox SortBox;
        public DXTextBox SearchBox;
        public DXButton SearchButton, BuyGameGoldButton, CurrencyToggleButton;
        public DXLabel TitleLabel, SortLabel, CurrencyLabel, TopFiveLabel, GameGoldLabel;
        public bool UseHuntGold { get; private set; }
        public HashSet<int> Favourites { get; } = new HashSet<int>();

        public WindowSetting Settings;
        public WindowType Type => WindowType.GameStoreBox;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (IsVisible)
            {
                BringToFront();
                ItemList?.Search(SearchBox?.TextBox.Text, (MarketPlaceStoreSort?)SortBox?.SelectedItem ?? MarketPlaceStoreSort.Alphabetical);
                RefreshCurrency();
            }

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

            if (e.KeyCode != Keys.Escape || !CloseButton.Visible) return;

            CloseButton.InvokeMouseClick();
            if (!Config.EscapeCloseAll)
                e.Handled = true;
        }

        public GameStoreDialog()
        {
            LibraryFile = LibraryFile.Interface;
            Index = 310;
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
                Text = CEnvir.Language.GameStoreDialogTitle,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 8);

            FolderTree = new DXTreeControl
            {
                Parent = this,
                Location = new Point(10, 38),
                Size = new Size(170, 305),
                VisibleRows = 14,
            };
            FolderTree.SelectedNodeChanged += (o, e) => FilterItems(FolderTree.SelectedNode);

            SortLabel = new DXLabel
            {
                Parent = this,
                Text = CEnvir.Language.GameStoreDialogSortByLabel,
                ForeColour = Color.FromArgb(198, 166, 99),
                IsControl = false,
            };
            SortLabel.Location = new Point(260 - SortLabel.Size.Width, 42);

            SortBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(270, 41),
                Border = false,
                Size = new Size(108, DXComboBox.DefaultNormalHeight),
            };
            AddSortOption(MarketPlaceStoreSort.Alphabetical, CEnvir.Language.GameStoreDialogSortNameLabel);
            AddSortOption(MarketPlaceStoreSort.HighestPrice, CEnvir.Language.GameStoreDialogSortHighestPriceLabel);
            AddSortOption(MarketPlaceStoreSort.LowestPrice, CEnvir.Language.GameStoreDialogSortLowestPriceLabel);
            AddSortOption(MarketPlaceStoreSort.Favourite, CEnvir.Language.GameStoreDialogSortFavouritesLabel);
            SortBox.ListBox.SelectItem(MarketPlaceStoreSort.Alphabetical);
            SortBox.SelectedItemChanged += (o, e) => RefreshItems();

            SearchBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(385, 42),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                Border = false,
                Size = new Size(132, 16),
            };
            SearchBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar != (char)Keys.Enter) return;
                e.Handled = true;
                RefreshItems();
            };

            SearchButton = new DXButton
            {
                Parent = this,
                Location = new Point(530, 40),
                Size = new Size(68, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.GameStoreDialogSearchButtonLabel },
            };
            SearchButton.MouseClick += (o, e) => RefreshItems();

            ItemList = new GameStoreItemListControl
            {
                Parent = this,
                Location = new Point(199, 67),
                Size = new Size(409, 432),
                Favourites = Favourites
            };
            BuildFolderTree();

            GameGoldLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Location = new Point(14, 375),
                Size = new Size(164, 18),
            };

            CurrencyLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(10, 354),
                Size = new Size(172, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = CEnvir.Language.GameStoreDialogCurrencyLabel,
                ForeColour = Color.FromArgb(198, 166, 99),
                IsControl = false,
            };

            BuyGameGoldButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                Location = new Point(10, 410),
                Size = new Size(172, 27),
                Label = { Text = CEnvir.Language.GameStoreDialogRechargeButtonLabel },
            };
            BuyGameGoldButton.MouseClick += BuyGameGoldButton_MouseClick;

            CurrencyToggleButton = new DXButton
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                Location = new Point(10, 438),
                Size = new Size(172, 27),
            };
            CurrencyToggleButton.MouseClick += (o, e) =>
            {
                UseHuntGold = !UseHuntGold;
                ItemList.UseHuntGold = UseHuntGold;
                RefreshCurrency();
                BuildFolderTree();
                RefreshItems();
            };

            TopItems = new GameStoreTopItemsControl
            {
                Parent = this,
                Location = new Point(614, 65),
                Size = new Size(174, 425),
            };
            TopFiveLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(614, 37),
                Size = new Size(174, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = CEnvir.Language.GameStoreDialogTopFiveLabel,
                ForeColour = Color.FromArgb(198, 166, 99),
                IsControl = false,
            };
            TopItems.ItemSelected += info =>
            {
                SearchBox.TextBox.Text = info.Item.ItemName;
                ItemList.FilterTo(info);
            };
            RefreshItems();
            RefreshCurrency();
        }

        private void AddSortOption(MarketPlaceStoreSort sort, string text)
        {
            new DXListBoxItem
            {
                Parent = SortBox.ListBox,
                Label = { Text = text },
                Item = sort,
            };
        }

        private void RefreshItems()
        {
            if (ItemList == null || SearchBox == null || SortBox == null) return;

            ItemList.Search(SearchBox.TextBox.Text, (MarketPlaceStoreSort?)SortBox.SelectedItem ?? MarketPlaceStoreSort.Alphabetical);
        }

        public void RefreshCurrency()
        {
            if (GameGoldLabel == null) return;

            long amount = UseHuntGold
                ? GameScene.Game?.User?.HuntGold?.Amount ?? 0
                : GameScene.Game?.User?.GameGold?.Amount ?? 0;

            string currency = UseHuntGold
                ? CEnvir.Language.GameStoreDialogHuntGoldLabel
                : CEnvir.Language.GameStoreDialogGameGoldLabel;

            GameGoldLabel.Text = $"{currency}: {amount:#,##0}";
            CurrencyToggleButton.Label.Text = UseHuntGold
                ? CEnvir.Language.GameStoreDialogUseGameGoldLabel
                : CEnvir.Language.GameStoreDialogUseHuntGoldLabel;
        }

        private void BuyGameGoldButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer || CEnvir.TestServer) return;

            DXMessageBox box = new DXMessageBox(
                CEnvir.Language.GameStoreDialogRechargeConfirmMessage,
                CEnvir.Language.GameStoreDialogRechargeConfirmCaption,
                DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o, args) =>
            {
                if (string.IsNullOrEmpty(CEnvir.BuyAddress)) return;

                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = CEnvir.BuyAddress + MapObject.User.Name,
                    UseShellExecute = true,
                });
            };
        }

        public void LoadSettings()
        {
            if (!CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);
            if (Settings != null)
            {
                Location = Settings.Location;
                Visible = Settings.Visible;
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }

        private void BuildFolderTree()
        {
            List<StoreInfo> items = Globals.StoreInfoList?.Binding
                .Where(x => x.Item != null && GetEffectivePrice(x) > 0)
                .ToList() ?? new List<StoreInfo>();
            List<DXTreeNode> nodes = new List<DXTreeNode>();

            if (items.Any(x => Favourites.Contains(x.Index)))
                nodes.Add(new DXTreeNode(CEnvir.Language.GameStoreDialogFavouritesLabel, new GameStoreTreeFilter(GameStoreCategory.Favourites)));

            List<string> filters = items
                .SelectMany(x => (x.Filter ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (filters.Count > 0)
            {
                DXTreeNode filterNode = new DXTreeNode(CEnvir.Language.GameStoreDialogCategoriesLabel, new GameStoreTreeFilter(GameStoreCategory.All, requiresStoreFilter: true));
                foreach (string filter in filters)
                    filterNode.Children.Add(new DXTreeNode(filter, new GameStoreTreeFilter(GameStoreCategory.All, null, filter, true)));
                nodes.Add(filterNode);
            }

            if (items.Count > 0)
                nodes.Add(new DXTreeNode(CEnvir.Language.GameStoreDialogNewItemsLabel, new GameStoreTreeFilter(GameStoreCategory.NewItems)));

            DXTreeNode allItemsNode = new DXTreeNode(CEnvir.Language.GameStoreDialogAllItemsLabel, new GameStoreTreeFilter(GameStoreCategory.All));
            AddCategoryNode(allItemsNode.Children, items, CEnvir.Language.GameStoreDialogEquipmentLabel, GameStoreCategory.Equipment, GameStoreItemListControl.IsEquipment);
            AddCategoryNode(allItemsNode.Children, items, CEnvir.Language.GameStoreDialogConsumablesLabel, GameStoreCategory.Consumables, GameStoreItemListControl.IsConsumable);
            AddCategoryNode(allItemsNode.Children, items, CEnvir.Language.GameStoreDialogCosmeticsLabel, GameStoreCategory.Cosmetics, GameStoreItemListControl.IsCosmetic);

            if (items.Any(x => !GameStoreItemListControl.IsEquipment(x.Item.ItemType) &&
                               !GameStoreItemListControl.IsConsumable(x.Item.ItemType) &&
                               !GameStoreItemListControl.IsCosmetic(x.Item.ItemType)))
                allItemsNode.Children.Add(new DXTreeNode(CEnvir.Language.GameStoreDialogOtherLabel, new GameStoreTreeFilter(GameStoreCategory.Other)));

            if (allItemsNode.Children.Count > 0)
            {
                nodes.Add(allItemsNode);
            }

            FolderTree.SetNodes(nodes);
        }

        private static void AddCategoryNode(List<DXTreeNode> nodes, List<StoreInfo> items, string text, GameStoreCategory category, Func<ItemType, bool> matches)
        {
            DXTreeNode node = new DXTreeNode(text, new GameStoreTreeFilter(category));
            foreach (ItemType type in items.Where(x => matches(x.Item.ItemType)).Select(x => x.Item.ItemType).Distinct().OrderBy(x => x.ToString()))
                node.Children.Add(new DXTreeNode(type.ToString(), new GameStoreTreeFilter(category, type)));

            if (node.Children.Count > 0)
                nodes.Add(node);
        }

        private int GetEffectivePrice(StoreInfo info)
        {
            return UseHuntGold && info.HuntGoldPrice > 0 ? info.HuntGoldPrice : info.Price;
        }

        private void FilterItems(DXTreeNode node)
        {
            if (node == null) return;

            GameStoreTreeFilter filter = node.Tag as GameStoreTreeFilter;
            ItemList.Category = filter?.Category ?? GameStoreCategory.All;
            ItemList.ItemTypeFilter = filter?.ItemType;
            ItemList.StoreFilter = filter?.StoreFilter;
            ItemList.RequiresStoreFilter = filter?.RequiresStoreFilter ?? false;
            if (!string.IsNullOrEmpty(SearchBox.TextBox.Text))
                SearchBox.TextBox.Text = string.Empty;
            RefreshItems();
        }

        public void SetFavourites(IEnumerable<int> favourites)
        {
            Favourites.Clear();
            if (favourites != null)
                Favourites.UnionWith(favourites);
            RefreshFavouriteState();
        }

        public void SetFavourite(int index, bool favourited)
        {
            if (favourited)
                Favourites.Add(index);
            else
                Favourites.Remove(index);
            RefreshFavouriteState();
        }

        private void RefreshFavouriteState()
        {
            if (ItemList == null) return;
            foreach (GameStoreItem row in ItemList.Rows)
                row.RefreshFavourite();
            BuildFolderTree();
            if (ItemList.Category == GameStoreCategory.Favourites)
                RefreshItems();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            Settings = null;

            if (CloseButton != null && !CloseButton.IsDisposed)
                CloseButton.Dispose();
            CloseButton = null;

            if (FolderTree != null && !FolderTree.IsDisposed)
                FolderTree.Dispose();
            FolderTree = null;

            if (ItemList != null && !ItemList.IsDisposed)
                ItemList.Dispose();
            ItemList = null;

            if (TopItems != null && !TopItems.IsDisposed)
                TopItems.Dispose();
            TopItems = null;

            if (SortBox != null && !SortBox.IsDisposed)
                SortBox.Dispose();
            SortBox = null;

            if (SearchBox != null && !SearchBox.IsDisposed)
                SearchBox.Dispose();
            SearchBox = null;

            if (SearchButton != null && !SearchButton.IsDisposed)
                SearchButton.Dispose();
            SearchButton = null;

            if (BuyGameGoldButton != null && !BuyGameGoldButton.IsDisposed)
                BuyGameGoldButton.Dispose();
            BuyGameGoldButton = null;

            if (CurrencyToggleButton != null && !CurrencyToggleButton.IsDisposed)
                CurrencyToggleButton.Dispose();
            CurrencyToggleButton = null;

            if (GameGoldLabel != null && !GameGoldLabel.IsDisposed)
                GameGoldLabel.Dispose();
            GameGoldLabel = null;

            if (TitleLabel != null && !TitleLabel.IsDisposed)
                TitleLabel.Dispose();
            TitleLabel = null;

            if (SortLabel != null && !SortLabel.IsDisposed)
                SortLabel.Dispose();
            SortLabel = null;

            if (CurrencyLabel != null && !CurrencyLabel.IsDisposed)
                CurrencyLabel.Dispose();
            CurrencyLabel = null;

            if (TopFiveLabel != null && !TopFiveLabel.IsDisposed)
                TopFiveLabel.Dispose();
            TopFiveLabel = null;

            Favourites.Clear();
        }
    }

    public sealed class GameStoreItemListControl : DXControl
    {
        public const int ItemsPerPage = 10;

        private readonly List<StoreInfo> _Results = new List<StoreInfo>();
        private int _Page;
        private bool _UseHuntGold;
        public HashSet<int> Favourites { get; set; }
        public GameStoreCategory Category { get; set; }
        public ItemType? ItemTypeFilter { get; set; }
        public string StoreFilter { get; set; }
        public bool RequiresStoreFilter { get; set; }

        public bool UseHuntGold
        {
            get => _UseHuntGold;
            set
            {
                if (_UseHuntGold == value) return;
                _UseHuntGold = value;

                foreach (GameStoreItem row in Rows)
                    row.UseHuntGold = value;
            }
        }

        public GameStoreItem[] Rows { get; private set; }
        public DXButton PreviousButton { get; private set; }
        public DXButton NextButton { get; private set; }
        public DXLabel PageLabel { get; private set; }

        public GameStoreItemListControl()
        {
            Rows = new GameStoreItem[ItemsPerPage];

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new GameStoreItem
                {
                    Parent = this,
                    Location = new Point((i % 2) * 202, (i / 2) * 80),
                };
            }

            PreviousButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4840,
                Location = new Point(122, 410),
            };
            PreviousButton.MouseClick += (o, e) =>
            {
                if (_Page <= 0) return;
                _Page--;
                RefreshPage();
            };

            PageLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(150, 406),
                Size = new Size(106, 20),
            };

            NextButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4845,
                Location = new Point(265, 410),
            };
            NextButton.MouseClick += (o, e) =>
            {
                if (_Page >= PageCount - 1) return;
                _Page++;
                RefreshPage();
            };
        }

        private int PageCount => Math.Max(1, (_Results.Count + ItemsPerPage - 1) / ItemsPerPage);

        public void Search(string name, MarketPlaceStoreSort sort)
        {
            _Results.Clear();

            if (Globals.StoreInfoList != null)
            {
                foreach (StoreInfo info in Globals.StoreInfoList.Binding)
                {
                    if (info.Item == null) continue;
                    if (GetSortPrice(info) <= 0) continue;
                    if (!MatchesCategory(info)) continue;
                    if (!string.IsNullOrWhiteSpace(name) && info.Item.ItemName.IndexOf(name, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    _Results.Add(info);
                }
            }

            switch (sort)
            {
                case MarketPlaceStoreSort.HighestPrice:
                    _Results.Sort((x, y) => GetSortPrice(y).CompareTo(GetSortPrice(x)));
                    break;
                case MarketPlaceStoreSort.LowestPrice:
                    _Results.Sort((x, y) => GetSortPrice(x).CompareTo(GetSortPrice(y)));
                    break;
                case MarketPlaceStoreSort.Favourite:
                    _Results.Sort((x, y) =>
                    {
                        int favouriteOrder = (Favourites?.Contains(y.Index) == true).CompareTo(Favourites?.Contains(x.Index) == true);
                        return favouriteOrder != 0 ? favouriteOrder : string.Compare(x.Item.ItemName, y.Item.ItemName, StringComparison.OrdinalIgnoreCase);
                    });
                    break;
                default:
                    _Results.Sort((x, y) => string.Compare(x.Item.ItemName, y.Item.ItemName, StringComparison.OrdinalIgnoreCase));
                    break;
            }

            _Page = 0;
            RefreshPage();
        }

        private bool MatchesCategory(StoreInfo info)
        {
            if (ItemTypeFilter.HasValue)
                return info.Item.ItemType == ItemTypeFilter.Value;
            if (!string.IsNullOrEmpty(StoreFilter))
                return (info.Filter ?? string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(x => string.Equals(x.Trim(), StoreFilter, StringComparison.OrdinalIgnoreCase));
            if (RequiresStoreFilter)
                return !string.IsNullOrWhiteSpace(info.Filter) && info.Filter
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(x => !string.IsNullOrWhiteSpace(x));

            ItemType type = info.Item.ItemType;
            switch (Category)
            {
                case GameStoreCategory.Favourites:
                    return Favourites?.Contains(info.Index) == true;
                case GameStoreCategory.NewItems:
                    return Globals.StoreInfoList.Binding.Where(x => x.Item != null).OrderByDescending(x => x.Index).Take(10).Contains(info);
                case GameStoreCategory.Equipment:
                    return IsEquipment(type);
                case GameStoreCategory.Consumables:
                    return IsConsumable(type);
                case GameStoreCategory.Cosmetics:
                    return IsCosmetic(type);
                case GameStoreCategory.Other:
                    return !IsEquipment(type) && !IsConsumable(type) && !IsCosmetic(type);
                default:
                    return true;
            }
        }

        public static bool IsEquipment(ItemType type)
        {
            return type == ItemType.Weapon || type == ItemType.Armour || type == ItemType.Torch ||
                   type == ItemType.Helmet || type == ItemType.Necklace || type == ItemType.Bracelet ||
                   type == ItemType.Ring || type == ItemType.Shoes || type == ItemType.Amulet ||
                   type == ItemType.HorseArmour || type == ItemType.ItemPart || type == ItemType.Emblem ||
                   type == ItemType.Shield;
        }

        public static bool IsConsumable(ItemType type)
        {
            return type == ItemType.Consumable || type == ItemType.Poison || type == ItemType.Meat ||
                   type == ItemType.Book || type == ItemType.Scroll || type == ItemType.DarkStone ||
                   type == ItemType.RefineSpecial || type == ItemType.Flower || type == ItemType.CompanionFood ||
                   type == ItemType.Bait || type == ItemType.Currency || type == ItemType.Bundle || type == ItemType.LootBox;
        }

        public static bool IsCosmetic(ItemType type)
        {
            return type == ItemType.Costume || type == ItemType.CompanionHead || type == ItemType.CompanionBack;
        }

        public void FilterTo(StoreInfo info)
        {
            _Results.Clear();

            if (info?.Item != null && GetSortPrice(info) > 0)
                _Results.Add(info);

            _Page = 0;
            RefreshPage();
        }

        private int GetSortPrice(StoreInfo info)
        {
            return UseHuntGold && info.HuntGoldPrice > 0 ? info.HuntGoldPrice : info.Price;
        }

        private void RefreshPage()
        {
            _Page = Math.Max(0, Math.Min(PageCount - 1, _Page));

            for (int i = 0; i < Rows.Length; i++)
            {
                int index = _Page * ItemsPerPage + i;
                Rows[i].StoreInfo = index < _Results.Count ? _Results[index] : null;
            }

            PageLabel.Text = $"{_Page + 1} / {PageCount}";
            PreviousButton.Enabled = _Page > 0;
            NextButton.Enabled = _Page < PageCount - 1;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _Results.Clear();
            _Page = 0;
            _UseHuntGold = false;
            Favourites = null;
            Category = GameStoreCategory.All;
            ItemTypeFilter = null;
            StoreFilter = null;
            RequiresStoreFilter = false;

            if (Rows != null)
            {
                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i] != null && !Rows[i].IsDisposed)
                        Rows[i].Dispose();
                    Rows[i] = null;
                }
                Rows = null;
            }

            if (PreviousButton != null && !PreviousButton.IsDisposed)
                PreviousButton.Dispose();
            PreviousButton = null;

            if (NextButton != null && !NextButton.IsDisposed)
                NextButton.Dispose();
            NextButton = null;

            if (PageLabel != null && !PageLabel.IsDisposed)
                PageLabel.Dispose();
            PageLabel = null;
        }
    }

    public sealed class GameStoreItem : DXControl
    {
        private StoreInfo _StoreInfo;
        private bool _UseHuntGold;

        public bool UseHuntGold
        {
            get => _UseHuntGold;
            set
            {
                if (_UseHuntGold == value) return;
                _UseHuntGold = value;
                RefreshPrice();
            }
        }

        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;
                _StoreInfo = value;
                RefreshItem();
            }
        }

        public DXImageControl HoverImage { get; private set; }
        public DXItemCell ItemCell { get; private set; }
        public DXLabel NameLabel { get; private set; }
        public DXLabel PriceLabel { get; private set; }
        public DXComboBox QuantityBox { get; private set; }
        public DXButton BuyButton { get; private set; }
        public DXButton GiftButton { get; private set; }
        public DXButton FavouriteButton { get; private set; }

        public GameStoreItem()
        {
            Size = new Size(200, 78);

            HoverImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4872,
                IsControl = false,
                Visible = false,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(19, 18),
                Border = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorder = true,
                FixedBorderColour = true,
                BorderColour = Color.Empty,
                ShowCountLabel = false,
            };

            PriceLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                ForeColour = Color.FromArgb(255, 140, 0),
                Location = new Point(7, 59),
                Size = new Size(58, 16),
                IsControl = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordEllipsis,
                Location = new Point(65, 8),
                Size = new Size(128, 17),
                ForeColour = Color.White,
                IsControl = false,
            };

            QuantityBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(72, 30),
                Size = new Size(117, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 120,
                Border = false
            };
            for (int i = 1; i <= 10; i++)
            {
                new DXListBoxItem
                {
                    Parent = QuantityBox.ListBox,
                    Label = { Text = i.ToString() },
                    Item = i,
                };
            }
            QuantityBox.ListBox.SelectItem(1);

            BuyButton = CreateActionButton(4835, new Point(83, 51), CEnvir.Language.GameStoreDialogPurchaseHint);
            BuyButton.MouseClick += BuyButton_MouseClick;

            GiftButton = CreateActionButton(4830, new Point(116, 51), CEnvir.Language.GameStoreDialogGiftHint);
            GiftButton.MouseClick += GiftButton_MouseClick;

            FavouriteButton = CreateActionButton(4855, new Point(151, 51), CEnvir.Language.GameStoreDialogFavouriteHint);
            FavouriteButton.MouseClick += (o, e) =>
            {
                if (StoreInfo == null) return;
                CEnvir.Enqueue(new C.GameStoreFavouriteToggle { Index = StoreInfo.Index });
            };

            ForwardHover(ItemCell);
            ForwardHover(QuantityBox);
            ForwardHover(QuantityBox.DownArrow);
            ForwardHover(BuyButton);
            ForwardHover(GiftButton);
            ForwardHover(FavouriteButton);
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            HoverImage.Visible = StoreInfo != null;
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            HoverImage.Visible = false;
        }

        private void ForwardHover(DXControl control)
        {
            control.MouseEnter += (o, e) => HoverImage.Visible = StoreInfo != null;
            control.MouseLeave += (o, e) => HoverImage.Visible = false;
        }

        private DXButton CreateActionButton(int index, Point location, string hint)
        {
            DXButton button = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = index,
                Location = location,
                Hint = hint,
                HintPosition = HintPosition.TopLeft,
            };
            return button;
        }

        private void RefreshItem()
        {
            Visible = StoreInfo?.Item != null;
            HoverImage.Visible = false;

            if (!Visible)
            {
                ItemCell.Item = null;
                return;
            }

            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(StoreInfo.Duration);
            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;

            ItemCell.Item = new ClientUserItem(StoreInfo.Item, 1)
            {
                Flags = flags,
                ExpireTime = duration,
            };
            ItemCell.RefreshItem();
            NameLabel.Text = StoreInfo.Item.ItemName;
            RefreshPrice();
            BuyButton.Enabled = StoreInfo.Available;
            GiftButton.Enabled = StoreInfo.Available;
            FavouriteButton.Enabled = true;
            RefreshFavourite();
        }

        public void RefreshFavourite()
        {
            bool favourite = StoreInfo != null && GameScene.Game?.GameStoreBox?.Favourites.Contains(StoreInfo.Index) == true;
            FavouriteButton.Index = favourite ? 4857 : 4855;
            FavouriteButton.Hint = favourite
                ? CEnvir.Language.GameStoreDialogRemoveFavouriteHint
                : CEnvir.Language.GameStoreDialogAddFavouriteHint;
        }

        private void RefreshPrice()
        {
            if (StoreInfo?.Item == null)
            {
                PriceLabel.Text = string.Empty;
                return;
            }

            PriceLabel.Text = StoreInfo.Available ? GetPrice().ToString("#,##0") : CEnvir.Language.GameStoreDialogUnavailableLabel;
        }

        private int GetPrice()
        {
            return UseHuntGold && StoreInfo.HuntGoldPrice > 0 ? StoreInfo.HuntGoldPrice : StoreInfo.Price;
        }

        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (StoreInfo?.Item == null || !StoreInfo.Available) return;

            int count = (int?)QuantityBox.SelectedItem ?? 1;
            int price = GetPrice();
            string currency = UseHuntGold
                ? CEnvir.Language.GameStoreDialogHuntGoldLabel
                : CEnvir.Language.GameStoreDialogGameGoldLabel;
            long total = (long)price * count;
            string message = string.Format(CEnvir.Language.GameStoreDialogPurchaseConfirmMessage, StoreInfo.Item.ItemName, count, price, currency, total);

            DXMessageBox box = new DXMessageBox(message, CEnvir.Language.GameStoreDialogPurchaseConfirmCaption, DXMessageBoxButtons.YesNo);
            box.YesButton.MouseClick += (o, args) =>
            {
                CEnvir.Enqueue(new C.MarketPlaceStoreBuy
                {
                    Index = StoreInfo.Index,
                    Count = count,
                    UseHuntGold = UseHuntGold,
                });
            };
        }

        private void GiftButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (StoreInfo?.Item == null || !StoreInfo.Available || GameScene.Game.Observer) return;

            DXInputWindow window = new DXInputWindow(CEnvir.Language.GameStoreDialogGiftPrompt, CEnvir.Language.GameStoreDialogGiftCaption)
            {
                ConfirmButton = { Enabled = false },
                Modal = true,
            };
            window.ValueTextBox.TextBox.TextChanged += (o, args) =>
            {
                window.ConfirmButton.Enabled = Globals.CharacterReg.IsMatch(window.ValueTextBox.TextBox.Text);
            };
            window.ConfirmButton.MouseClick += (o, args) =>
            {
                CEnvir.Enqueue(new C.GameStoreGift
                {
                    Index = StoreInfo.Index,
                    Count = (int?)QuantityBox.SelectedItem ?? 1,
                    UseHuntGold = UseHuntGold,
                    Recipient = window.Value,
                });
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _StoreInfo = null;
            _UseHuntGold = false;

            if (HoverImage != null && !HoverImage.IsDisposed)
                HoverImage.Dispose();
            HoverImage = null;

            if (ItemCell != null && !ItemCell.IsDisposed)
                ItemCell.Dispose();
            ItemCell = null;

            if (NameLabel != null && !NameLabel.IsDisposed)
                NameLabel.Dispose();
            NameLabel = null;

            if (PriceLabel != null && !PriceLabel.IsDisposed)
                PriceLabel.Dispose();
            PriceLabel = null;

            if (QuantityBox != null && !QuantityBox.IsDisposed)
                QuantityBox.Dispose();
            QuantityBox = null;

            if (BuyButton != null && !BuyButton.IsDisposed)
                BuyButton.Dispose();
            BuyButton = null;

            if (GiftButton != null && !GiftButton.IsDisposed)
                GiftButton.Dispose();
            GiftButton = null;

            if (FavouriteButton != null && !FavouriteButton.IsDisposed)
                FavouriteButton.Dispose();
            FavouriteButton = null;
        }
    }

    public sealed class GameStoreTopItemsControl : DXControl
    {
        public GameStoreTopItemControl[] Rows { get; private set; }
        public event Action<StoreInfo> ItemSelected;

        public GameStoreTopItemsControl()
        {
            Rows = new GameStoreTopItemControl[5];
            string[] rankLabels =
            {
                CEnvir.Language.GameStoreDialogFirstPlaceLabel,
                CEnvir.Language.GameStoreDialogSecondPlaceLabel,
                CEnvir.Language.GameStoreDialogThirdPlaceLabel,
                CEnvir.Language.GameStoreDialogFourthPlaceLabel,
                CEnvir.Language.GameStoreDialogFifthPlaceLabel,
            };

            for (int i = 0; i < Rows.Length; i++)
            {
                GameStoreTopItemControl row = new GameStoreTopItemControl
                {
                    Parent = this,
                    Location = new Point(0, 5 + i * 87),
                    Size = new Size(174, i == Rows.Length - 1 ? 73 : 78)
                };
                row.RankLabel.Text = rankLabels[i];
                row.MouseClick += (o, e) => SelectItem(row);
                row.ItemCell.MouseClick += (o, e) => SelectItem(row);
                Rows[i] = row;
            }
        }

        public void SetItems(IEnumerable<int> indexes)
        {
            List<StoreInfo> items = indexes?
                .Select(index => Globals.StoreInfoList?.Binding.FirstOrDefault(x => x.Index == index))
                .Where(x => x?.Item != null)
                .Take(Rows.Length)
                .ToList() ?? new List<StoreInfo>();

            for (int i = 0; i < Rows.Length; i++)
                Rows[i].StoreInfo = i < items.Count ? items[i] : null;
        }

        private void SelectItem(GameStoreTopItemControl row)
        {
            if (row.StoreInfo?.Item == null) return;

            ItemSelected?.Invoke(row.StoreInfo);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            ItemSelected = null;

            if (Rows == null) return;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] != null && !Rows[i].IsDisposed)
                    Rows[i].Dispose();
                Rows[i] = null;
            }
            Rows = null;
        }
    }

    public sealed class GameStoreTopItemControl : DXControl
    {
        private StoreInfo _StoreInfo;

        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;

                _StoreInfo = value;
                RefreshItem();
            }
        }

        public DXItemCell ItemCell { get; private set; }
        public DXLabel RankLabel { get; private set; }
        public DXLabel NameLabel { get; private set; }

        public GameStoreTopItemControl()
        {
            RankLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(0, 1),
                Size = new Size(174, 20),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.CornflowerBlue,
                IsControl = false,
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(19, 26),
                FixedBorder = true,
                BorderColour = Color.Empty,
                Border = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis,
                Location = new Point(65, 30),
                Size = new Size(100, 20),
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        private void RefreshItem()
        {
            Visible = StoreInfo?.Item != null;

            if (!Visible)
            {
                ItemCell.Item = null;
                NameLabel.Text = string.Empty;
                return;
            }

            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(StoreInfo.Duration);
            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;

            ItemCell.Item = new ClientUserItem(StoreInfo.Item, 1)
            {
                Flags = flags,
                ExpireTime = duration,
            };
            ItemCell.RefreshItem();
            NameLabel.Text = StoreInfo.Item.ItemName;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _StoreInfo = null;

            if (ItemCell != null && !ItemCell.IsDisposed)
                ItemCell.Dispose();
            ItemCell = null;

            if (NameLabel != null && !NameLabel.IsDisposed)
                NameLabel.Dispose();
            NameLabel = null;

            if (RankLabel != null && !RankLabel.IsDisposed)
                RankLabel.Dispose();
            RankLabel = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
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
    public sealed class MarketPlaceDialog : DXWindow
    {
        #region Properites

        public DXTabControl TabControl;

        #region Search
        public DXTab SearchTab;
        public DXTextBox ItemNameBox, BuyTotalBox, SearchNumberSoldBox, SearchLastPriceBox, SearchAveragePriceBox;
        public DXNumberBox BuyCountBox, BuyPriceBox;
        public DXComboBox ItemTypeBox, SortBox;
        public DXControl MessagePanel, BuyPanel, HistoryPanel;
        public DXButton BuyButton, SearchButton;
        public DXCheckBox BuyGuildBox;
        public DXLabel MessageLabel;
        public DXVScrollBar SearchScrollBar;

        public MarketPlaceRow[] SearchRows;
        public ClientMarketPlaceInfo[] SearchResults;
        #endregion

        #region Consign
        public DXTab ConsignTab;

        public DXTextBox ConsignPriceBox, ConsignCostBox, NumberSoldBox, LastPriceBox, AveragePriceBox, ConsignMessageBox;
        public DXControl ConsignPanel, ConsignBuyPanel, ConsignConfirmPanel;
        public DXButton ConsignButton;
        public DXCheckBox ConsignGuildBox;
        public DXItemGrid ConsignGrid;
        public DXLabel ConsignPriceLabel;
        public DXVScrollBar ConsignScrollBar;

        public MarketPlaceRow[] ConsignRows;
        #endregion

        #region Store
        public DXTab StoreTab;

        public DXTextBox StoreItemNameBox, StoreBuyTotalBox;
        public DXNumberBox StoreBuyCountBox, StoreBuyPriceBox, GameGoldBox, HuntGoldBox;
        public DXComboBox StoreItemTypeBox, StoreSortBox;
        public DXControl StoreBuyPanel;
        public DXButton StoreBuyButton,  StoreSearchButton;
        public DXCheckBox UseHuntGoldBox;
        public DXVScrollBar StoreScrollBar;
        public DXLabel StoreBuyPriceLabel;

        public MarketPlaceStoreRow[] StoreRows;
        public List<StoreInfo> StoreSearchResults;
        #endregion

        #region SelectedRow

        public MarketPlaceRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                MarketPlaceRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private MarketPlaceRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(MarketPlaceRow oValue, MarketPlaceRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            if (nValue?.MarketInfo == null)
            {
                MessagePanel.Enabled = false;
                MessageLabel.Text = "";

                BuyPanel.Enabled = false;

                BuyCountBox.MinValue = 0;
                BuyCountBox.ValueTextBox.TextBox.Text = "";

                BuyPriceBox.MinValue = 0;
                BuyPriceBox.ValueTextBox.TextBox.Text = "";

                HistoryPanel.Enabled = false;

                SearchNumberSoldBox.TextBox.Text = "";
                SearchLastPriceBox.TextBox.Text = "";
                SearchAveragePriceBox.TextBox.Text = "";
            }
            else
            {
                MessagePanel.Enabled = true;
                MessageLabel.Text = nValue.MarketInfo.Message;

                BuyPanel.Enabled = !GameScene.Game.Observer;

                BuyCountBox.MinValue = 1;
                BuyCountBox.MaxValue = nValue.MarketInfo.Item.Count;
                BuyCountBox.Value = 1;

                BuyPriceBox.MinValue = nValue.MarketInfo.Price;
                BuyPriceBox.MaxValue = nValue.MarketInfo.Price;
                BuyPriceBox.Value = nValue.MarketInfo.Price;

                HistoryPanel.Enabled = true;

                SearchNumberSoldBox.TextBox.Text = "Searching...";
                SearchLastPriceBox.TextBox.Text = "Searching...";
                SearchAveragePriceBox.TextBox.Text = "Searching...";

                CEnvir.Enqueue(new C.MarketPlaceHistory { Index = nValue.MarketInfo.Item.Info.Index, PartIndex = nValue.MarketInfo.Item.AddedStats[Stat.ItemIndex], Display = 1 });
            }

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region SelectedStoreRow

        public MarketPlaceStoreRow SelectedStoreRow
        {
            get => _SelectedStoreRow;
            set
            {
                if (_SelectedStoreRow == value) return;

                MarketPlaceStoreRow oldValue = _SelectedStoreRow;
                _SelectedStoreRow = value;

                OnSelectedStoreRowChanged(oldValue, value);
            }
        }
        private MarketPlaceStoreRow _SelectedStoreRow;
        public event EventHandler<EventArgs> SelectedStoreRowChanged;
        public void OnSelectedStoreRowChanged(MarketPlaceStoreRow oValue, MarketPlaceStoreRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            if (nValue?.StoreInfo == null)
            {
                StoreBuyPanel.Enabled = false;

                StoreBuyCountBox.MinValue = 0;
                StoreBuyCountBox.ValueTextBox.TextBox.Text = "";

                StoreBuyPriceBox.MinValue = 0;
                StoreBuyPriceBox.ValueTextBox.TextBox.Text = "";
            }
            else
            {
                StoreBuyPanel.Enabled = !GameScene.Game.Observer;

                StoreBuyCountBox.MinValue = 1;
                StoreBuyCountBox.MaxValue = nValue.StoreInfo.Item.StackSize;
                StoreBuyCountBox.Value = 1;

                StoreBuyPriceBox.MinValue = nValue.StoreInfo.Price;
                StoreBuyPriceBox.MaxValue = nValue.StoreInfo.Price;
                StoreBuyPriceBox.Value = nValue.StoreInfo.Price;
            }

            SelectedStoreRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Price

        public int Price
        {
            get => _Price;
            set
            {
                if (_Price == value) return;

                int oldValue = _Price;
                _Price = value;

                OnPriceChanged(oldValue, value);
            }
        }
        private int _Price;
        public event EventHandler<EventArgs> PriceChanged;
        public void OnPriceChanged(int oValue, int nValue)
        {
            ConsignCostBox.TextBox.Text = Cost.ToString("#,##0");

            PriceChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public int Cost => 0;// (int)Math.Min(int.MaxValue, Price * Globals.MarketPlaceTax * ConsignGrid.Grid[0].LinkedCount + Globals.MarketPlaceFee);

        public List<ClientMarketPlaceInfo> ConsignItems = new List<ClientMarketPlaceInfo>();

        public DateTime NextSearchTime;
        
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (SearchRows == null) return; //Not Loaded

            if (!Visible)
            {
                ConsignGrid.ClearLinks();
                return;
            }


            if (SearchResults == null)
                Search();

            if (StoreSearchResults == null)
                StoreSearch();
        }

        public override WindowType Type => WindowType.MarketPlaceBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public MarketPlaceDialog()
        {
            //HasFooter = true;
            TitleLabel.Text = CEnvir.Language.MarketPlaceDialogTitle;
            SetClientSize(new Size(740, 461));


            TabControl = new DXTabControl
            {
                Parent = this,
                Size = ClientArea.Size,
                Location = ClientArea.Location,
            };



            #region Search

            SearchTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.MarketPlaceDialogSearchTabLabel } },
                Border = true,
            };


            DXControl filterPanel = new DXControl
            {
                Parent = SearchTab,
                Size = new Size(SearchTab.Size.Width - 20, 26),
                Location = new Point(10, 10),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = CEnvir.Language.MarketPlaceDialogSearchTabNameLabel,
            };

            ItemNameBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            ItemNameBox.TextBox.KeyPress += TextBox_KeyPress;



            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameBox.Location.X + ItemNameBox.Size.Width + 10, 5),
                Text = CEnvir.Language.MarketPlaceDialogSearchTabItemLabel,
            };



            ItemTypeBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };


            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"All" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = ItemTypeBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            ItemTypeBox.ListBox.SelectItem(null);


            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemTypeBox.Location.X + ItemTypeBox.Size.Width + 10, 5),
                Text = CEnvir.Language.MarketPlaceDialogSearchTabSortLabel,
            };

            SortBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(100, DXComboBox.DefaultNormalHeight)
            };

            itemType = typeof(MarketPlaceSort);

            for (MarketPlaceSort i = MarketPlaceSort.Newest; i <= MarketPlaceSort.LowestPrice; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                new DXListBoxItem
                {
                    Parent = SortBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            SortBox.ListBox.SelectItem(MarketPlaceSort.Newest);


            SearchButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(SortBox.Location.X + SortBox.Size.Width + 25, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.MarketPlaceDialogSearchTabSearchButtonLabel }
            };
            SearchButton.MouseClick += (o, e) => Search();

            DXButton ClearButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(SearchButton.Location.X + SearchButton.Size.Width + 40, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.MarketPlaceDialogSearchTabClearButtonLabel }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemNameBox.TextBox.Text = "";
                ItemTypeBox.ListBox.SelectItem(null);
                Search();
            };

            SearchRows = new MarketPlaceRow[9];

            SearchScrollBar = new DXVScrollBar
            {
                Parent = SearchTab,
                Location = new Point(533, 47),
                Size = new Size(14, SearchTab.Size.Height - 59),
                VisibleSize = SearchRows.Length,
                Change = 3,
            };
            SearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;


            for (int i = 0; i < SearchRows.Length; i++)
            {
                int index = i;
                SearchRows[index] = new MarketPlaceRow
                {
                    Parent = SearchTab,
                    Location = new Point(10, 46 + i*43),
                };
                SearchRows[index].MouseClick += (o, e) => { SelectedRow = SearchRows[index]; };
                SearchRows[index].MouseWheel += SearchScrollBar.DoMouseWheel;
            }

            HistoryPanel = new DXControl
            {
                Location = new Point(555, 47),
                Parent = SearchTab,
                Size = new Size(175, 95),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Enabled = false,
            };


            new DXLabel
            {
                Text = CEnvir.Language.MarketPlaceDialogSearchTabSaleHistoryLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15),
                Parent = HistoryPanel,
            };

            label = new DXLabel
            {
                Parent = HistoryPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabSaleHistoryNumberSoldLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 25);

            SearchNumberSoldBox = new DXTextBox
            {
                Location = new Point(80, 25),
                Size = new Size(85, 18),
                Parent = HistoryPanel,
                ReadOnly = true,
                Editable = false
            };

            label = new DXLabel
            {
                Parent = HistoryPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabSaleHistoryLastPriceLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 45);

            SearchLastPriceBox = new DXTextBox
            {
                Location = new Point(80, 45),
                Size = new Size(85, 18),
                Parent = HistoryPanel,
                ReadOnly = true,
                Editable = false
            };

            label = new DXLabel
            {
                Parent = HistoryPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabSaleHistoryAvgPriceLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 65);

            SearchAveragePriceBox = new DXTextBox
            {
                Location = new Point(80, 65),
                Size = new Size(85, 18),
                Parent = HistoryPanel,
                ReadOnly = true,
                Editable = false
            };


            MessagePanel = new DXControl
            {
                Location = new Point(555, 147),
                Parent = SearchTab,
                Size = new Size(175, 127),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Enabled = false,
            };

            new DXLabel
            {
                Parent = MessagePanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabMessageLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            MessageLabel = new DXLabel
            {
                Location = new Point(0, 20),
                Parent = MessagePanel,
                Size = new Size(175, 80),
                AutoSize = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis
            };

            BuyPanel = new DXControl
            {
                Location = new Point(555, 279),
                Parent = SearchTab,
                Size = new Size(175, 150),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Enabled = false,
            };

            new DXLabel
            {
                Parent = BuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = BuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingCountLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            BuyCountBox = new DXNumberBox
            {
                Parent = BuyPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18) },
                MaxValue = 5000,
                MinValue = 1,
                UpButton = { Location = new Point(108, 1) }
            };
            BuyCountBox.ValueTextBox.ValueChanged += UpdateBuyTotal;


            label = new DXLabel
            {
                Parent = BuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingPriceLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 40);

            BuyPriceBox = new DXNumberBox
            {
                Parent = BuyPanel,
                Location = new Point(50, 40),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = 0
            };
            BuyPriceBox.ValueTextBox.ValueChanged += UpdateBuyTotal;

            BuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = BuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };

            label = new DXLabel
            {
                Parent = BuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingTotalLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 60);

            BuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = BuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };


            BuyGuildBox = new DXCheckBox
            {
                Parent = BuyPanel,
                Label = { Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingGuildLabel },
                Enabled = false,
            };
            BuyGuildBox.Location = new Point(158 - BuyGuildBox.Size.Width, 101);

            BuyButton = new DXButton
            {
                Size = new Size(85, SmallButtonHeight),
                Location = new Point(69, 124),
                Label = { Text = CEnvir.Language.MarketPlaceDialogSearchTabBuyingBuyButtonLabel },
                ButtonType = ButtonType.SmallButton,
                Parent = BuyPanel,
            };
            BuyButton.MouseClick += BuyButton_MouseClick;

            #endregion

            #region Consign

            ConsignTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.MarketPlaceDialogConsignTabLabel } },
                Border = true,
            };
            DXControl consignPanel = new DXControl
            {
                Parent = ConsignTab,
                Size = new Size(175, 145),
                Location = new Point(10, 10),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };


            new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1Label,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1SelectItemLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 30);

            ConsignGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Location = new Point(79, 20),
                Parent = consignPanel,
                Border = true,
                Linked = true,
                GridType = GridType.Consign,
            };

            ConsignGrid.Grid[0].LinkChanged += (o, e) =>
            {
                if (ConsignGrid.Grid[0].Item == null)
                {
                    NumberSoldBox.TextBox.Text = "";
                    LastPriceBox.TextBox.Text = "";
                    AveragePriceBox.TextBox.Text = "";
                    ConsignGrid.Grid[0].LinkedCount = 0;
                }
                else
                {
                    NumberSoldBox.TextBox.Text = CEnvir.Language.MarketPlaceDialogConsignTabSearchingLabel;
                    LastPriceBox.TextBox.Text = CEnvir.Language.MarketPlaceDialogConsignTabSearchingLabel;
                    AveragePriceBox.TextBox.Text = CEnvir.Language.MarketPlaceDialogConsignTabSearchingLabel;

                    CEnvir.Enqueue(new C.MarketPlaceHistory { Index = ConsignGrid.Grid[0].Item.Info.Index, PartIndex = ConsignGrid.Grid[0].Item.AddedStats[Stat.ItemIndex], Display = 2 });
                }
                ConsignCostBox.TextBox.Text = Cost.ToString("#,##0");

            };

            new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1SaleHistoryLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15),
                Location = new Point(0, 60)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };


            label = new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1NumberSoldLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 80);

            NumberSoldBox = new DXTextBox
            {
                Location = new Point(80, 80),
                Size = new Size(85, 18),
                Parent = consignPanel,
                ReadOnly = true,
                Editable = false
            };

            label = new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1LastPriceLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 100);

            LastPriceBox = new DXTextBox
            {
                Location = new Point(80, 100),
                Size = new Size(85, 18),
                Parent = consignPanel,
                ReadOnly = true,
                Editable = false
            };

            label = new DXLabel
            {
                Parent = consignPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep1AvgPriceLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 120);

            AveragePriceBox = new DXTextBox
            {
                Location = new Point(80, 120),
                Size = new Size(85, 18),
                Parent = consignPanel,
                ReadOnly = true,
                Editable = false
            };

            ConsignBuyPanel = new DXControl
            {
                Parent = ConsignTab,
                Size = new Size(175, 50),
                Location = new Point(10, consignPanel.Location.Y + consignPanel.Size.Height + 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Parent = ConsignBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep2Label,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            ConsignPriceLabel = new DXLabel
            {
                Parent = ConsignBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep2PriceLabel,
            };
            ConsignPriceLabel.Location = new Point(80 - ConsignPriceLabel.Size.Width, 25);

            ConsignPriceBox = new DXTextBox
            {
                Location = new Point(80, 25),
                Size = new Size(85, 18),
                Parent = ConsignBuyPanel,
            };
            ConsignPriceBox.TextBox.TextChanged += (o, e) =>
            {
                int price;
                int.TryParse(ConsignPriceBox.TextBox.Text, out price);

                Price = price;
            };


            DXControl ConsignMesagePanel = new DXControl
            {
                Parent = ConsignTab,
                Size = new Size(175, 115),
                Location = new Point(10, ConsignBuyPanel.Location.Y + ConsignBuyPanel.Size.Height + 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Parent = ConsignMesagePanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep3Label,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };
            
            ConsignMessageBox = new DXTextBox
            {
                Location = new Point(10, 25),
                Parent = ConsignMesagePanel,
                TextBox = { Multiline = true, AcceptsReturn = true, },
                Size = new Size(ConsignMesagePanel.Size.Width - 20, 80),
                MaxLength = 150,
            };

            ConsignConfirmPanel = new DXControl
            {
                Parent = ConsignTab,
                Size = new Size(175, 90),
                Location = new Point(10, ConsignMesagePanel.Location.Y + ConsignMesagePanel.Size.Height + 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Parent = ConsignConfirmPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep4Label,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = ConsignConfirmPanel,
                Text = CEnvir.Language.MarketPlaceDialogConsignTabStep4ConsignCostLabel,
            };
            label.Location = new Point(80 - label.Size.Width, 25);

            ConsignCostBox = new DXTextBox
            {
                Location = new Point(80, 25),
                Size = new Size(85, 18),
                Parent = ConsignConfirmPanel,
                ReadOnly = true,
                Editable = false,
            };

            ConsignGuildBox = new DXCheckBox
            {
                Parent = ConsignConfirmPanel,
                Label = { Text = CEnvir.Language.MarketPlaceDialogConsignTabStep4ConsignGuildLabel },
                Enabled = false,
            };
            ConsignGuildBox.Location = new Point(169 - ConsignGuildBox.Size.Width, 45);

            ConsignButton = new DXButton
            {
                Size = new Size(85, SmallButtonHeight),
                Location = new Point(80, 65),
                Label = { Text = CEnvir.Language.MarketPlaceDialogConsignTabStep4ConsignButtonLabel },
                ButtonType = ButtonType.SmallButton,
                Parent = ConsignConfirmPanel,
            };
            ConsignButton.MouseClick += ConsignButton_MouseClick;

            ConsignRows = new MarketPlaceRow[10];
            for (int i = 0; i < ConsignRows.Length; i++)
            {
                int index = i;
                ConsignRows[index] = new MarketPlaceRow
                {
                    Parent = ConsignTab,
                    Location = new Point(190, 10 + index*42),
                };
                ConsignRows[index].MouseClick += (o, e) =>
                {
                    ClientMarketPlaceInfo info = ConsignRows[index].MarketInfo;
                    if (info == null) return;

                    DXItemAmountWindow window = new DXItemAmountWindow(CEnvir.Language.MarketPlaceDialogConsignTabCancelListingCaption, info.Item);

                    window.ConfirmButton.MouseClick += (o1, e1) =>
                    {
                        if (window.Amount <= 0) return;

                        CEnvir.Enqueue(new C.MarketPlaceCancelConsign { Index = info.Index, Count = window.Amount });
                    };

                };
            }

            ConsignScrollBar = new DXVScrollBar
            {
                Parent = ConsignTab,
                Location = new Point(713, 11),
                Size = new Size(14, SearchTab.Size.Height - 24),
                VisibleSize = ConsignRows.Length,
                Change = 3,
            };
            ConsignScrollBar.ValueChanged += ConsignScrollBar_ValueChanged;

            #endregion

            #region Store
            
            StoreTab = new DXTab
            {
                Parent = TabControl,
                TabButton = { Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabLabel }, Size = new Size(120, TabHeight), RightAligned = true },
                Border = true,
            };
            StoreTab.IsVisibleChanged += (o, e) => SelectedStoreRow = null;
            filterPanel = new DXControl
            {
                Parent = StoreTab,
                Size = new Size(SearchTab.Size.Width - 20, 26),
                Location = new Point(10, 10),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabNameLabel,
            };

            StoreItemNameBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            StoreItemNameBox.TextBox.KeyPress += StoreTextBox_KeyPress;



            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(StoreItemNameBox.Location.X + StoreItemNameBox.Size.Width + 10, 5),
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabItemLabel,
            };



            StoreItemTypeBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };


            new DXListBoxItem
            {
                Parent = StoreItemTypeBox.ListBox,
                Label = { Text = $"All" },
                Item = null
            };

            //TODO Store Filter

            HashSet<string> filters = new HashSet<string>();

            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (string.IsNullOrEmpty(info.Filter)) continue;

                string[] temp = info.Filter.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in temp)
                    filters.Add(s.Trim());
            }

            foreach (string filter in filters.OrderBy(x => x))
            {
                new DXListBoxItem
                {
                    Parent = StoreItemTypeBox.ListBox,
                    Label = { Text = filter },
                    Item = filter
                };
            }

            StoreItemTypeBox.ListBox.SelectItem(null);


            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(StoreItemTypeBox.Location.X + StoreItemTypeBox.Size.Width + 10, 5),
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabSortLabel,
            };

            StoreSortBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(100, DXComboBox.DefaultNormalHeight)
            };

            Type storeType = typeof(MarketPlaceStoreSort);

            for (MarketPlaceStoreSort i = MarketPlaceStoreSort.Alphabetical; i <= MarketPlaceStoreSort.Favourite; i++)
            {
                MemberInfo[] infos = storeType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
                new DXListBoxItem
                {
                    Parent = StoreSortBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            StoreSortBox.ListBox.SelectItem(MarketPlaceStoreSort.Alphabetical);


            StoreSearchButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(StoreSortBox.Location.X + StoreSortBox.Size.Width + 25, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabSearchButtonLabel }
            };
            StoreSearchButton.MouseClick += (o, e) => StoreSearch();

            ClearButton = new DXButton
            {
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(SearchButton.Location.X + SearchButton.Size.Width + 40, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabClearButtonLabel }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                StoreItemNameBox.TextBox.Text = "";
                StoreItemTypeBox.ListBox.SelectItem(null);
                StoreSearch();
            };

            StoreRows = new MarketPlaceStoreRow[9];

            StoreScrollBar = new DXVScrollBar
            {
                Parent = StoreTab,
                Location = new Point(533, 47),
                Size = new Size(14, SearchTab.Size.Height - 59),
                VisibleSize = StoreRows.Length,
                Change = 3,
            };
            StoreScrollBar.ValueChanged += StoreScrollBar_ValueChanged;


            for (int i = 0; i < StoreRows.Length; i++)
            {
                int index = i;
                StoreRows[index] = new MarketPlaceStoreRow
                {
                    Parent = StoreTab,
                    Location = new Point(10, 46 + i * 43),
                };
                StoreRows[index].MouseClick += (o, e) => { SelectedStoreRow = StoreRows[index]; };
                StoreRows[index].MouseWheel += StoreScrollBar.DoMouseWheel;
            }
            DXControl HuntGoldPanel = new DXControl
            {
                Location = new Point(555, 149),
                Parent = StoreTab,
                Size = new Size(175, 50),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            new DXLabel
            {
                Parent = HuntGoldPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabHuntGoldLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = HuntGoldPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabHuntGoldAmountLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            HuntGoldBox = new DXNumberBox
            {
                Parent = HuntGoldPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = -200000000
            };

            DXControl AddGameGoldPanel = new DXControl
            {
                Location = new Point(555, 204),
                Parent = StoreTab,
                Size = new Size(175, 70),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            new DXLabel
            {
                Parent = AddGameGoldPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabGameGoldLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = AddGameGoldPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabGameGoldAmountLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            GameGoldBox = new DXNumberBox
            {
                Parent = AddGameGoldPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = -200000000
            };
            DXButton RechargeButton = new DXButton
            {
                Size = new Size(85, SmallButtonHeight),
                Location = new Point(69, 45),
                Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabGameGoldRechargeButtonLabel },
                ButtonType = ButtonType.SmallButton,
                Parent = AddGameGoldPanel,
                Enabled = !CEnvir.TestServer
            };
            RechargeButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                DXMessageBox box = new DXMessageBox(CEnvir.Language.MarketPlaceDialogGameStoreTabGameGoldRechargeButtonConfirmMessage, CEnvir.Language.MarketPlaceDialogGameStoreTabGameGoldRechargeButtonConfirmCaption, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    if (string.IsNullOrEmpty(CEnvir.BuyAddress)) return;

                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = (CEnvir.BuyAddress + MapObject.User.Name),
                        UseShellExecute = true
                    });
                };
            };
            
            StoreBuyPanel = new DXControl
            {
                Location = new Point(555, 279),
                Parent = StoreTab,
                Size = new Size(175, 150),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Enabled = false,
            };

            new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingLabel,
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
                //   Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Underline)
            };

            label = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingCountLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            StoreBuyCountBox = new DXNumberBox
            {
                Parent = StoreBuyPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18) },
                MaxValue = 5000,
                MinValue = 1,
                UpButton = { Location = new Point(108, 1) }
            };
            StoreBuyCountBox.ValueTextBox.ValueChanged += UpdateStoreBuyTotal;


            StoreBuyPriceLabel = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingPriceLabel,
                ForeColour = Color.White,
            };
            StoreBuyPriceLabel.Location = new Point(50 - StoreBuyPriceLabel.Size.Width, 40);

            StoreBuyPriceBox = new DXNumberBox
            {
                Parent = StoreBuyPanel,
                Location = new Point(50, 40),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = 0
            };
            StoreBuyPriceBox.ValueTextBox.ValueChanged += UpdateStoreBuyTotal;

            StoreBuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = StoreBuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };

            label = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingTotalLabel,
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 60);

            StoreBuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = StoreBuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };

            UseHuntGoldBox = new DXCheckBox
            {
                Parent = StoreBuyPanel,
                Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingUseHuntGoldLabel },
            };
            UseHuntGoldBox.Location = new Point(158 - UseHuntGoldBox.Size.Width, 101);
            UseHuntGoldBox.CheckedChanged += UpdateStoreBuyTotal;


            StoreBuyButton = new DXButton
            {
                Size = new Size(85, SmallButtonHeight),
                Location = new Point(69, 124),
                Label = { Text = CEnvir.Language.MarketPlaceDialogGameStoreTabBuyingBuyButtonLabel },
                ButtonType = ButtonType.SmallButton,
                Parent = StoreBuyPanel,
            };
            StoreBuyButton.MouseClick += StoreBuyButton_MouseClick;

            #endregion
        }
        
        #region Methods


        public void Search()
        {
            SearchResults = null;

            SearchScrollBar.MaxValue = 0;


            foreach (MarketPlaceRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }


            CEnvir.Enqueue(new C.MarketPlaceSearch
            {
                Name = ItemNameBox.TextBox.Text,

                ItemTypeFilter = ItemTypeBox.SelectedItem != null,
                ItemType = (ItemType?)ItemTypeBox.SelectedItem ?? 0,

                Sort = (MarketPlaceSort)SortBox.SelectedItem,
            });
        }
        public void StoreSearch()
        {
            StoreSearchResults = new List<StoreInfo>();

            StoreScrollBar.MaxValue = 0;


            foreach (MarketPlaceStoreRow row in StoreRows)
                row.Visible = true;

            string filter = (string)StoreItemTypeBox.SelectedItem;

            MarketPlaceStoreSort sort = (MarketPlaceStoreSort)StoreSortBox.SelectedItem;

            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Item == null) continue;

                if (filter != null && !info.Filter.Contains(filter)) continue;

                if (!string.IsNullOrEmpty(StoreItemNameBox.TextBox.Text) && info.Item.ItemName.IndexOf(StoreItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;

                StoreSearchResults.Add(info);
            }

            switch (sort)
            {
                case MarketPlaceStoreSort.Alphabetical:
                    StoreSearchResults.Sort((x1, x2) => string.Compare(x1.Item.ItemName, x2.Item.ItemName, StringComparison.Ordinal));
                    break;
                case MarketPlaceStoreSort.HighestPrice:
                    StoreSearchResults.Sort((x1, x2) => x2.Price.CompareTo(x1.Price));
                    break;
                case MarketPlaceStoreSort.LowestPrice:
                    StoreSearchResults.Sort((x1, x2) => x1.Price.CompareTo(x2.Price));
                    break;
                case MarketPlaceStoreSort.Favourite:
                    // TODO StoreSearchResults.Sort((x1, x2) => x1.Price.CompareTo(x2.Price));
                    break;
            }

            RefreshStoreList();
        }

        public void RefreshList()
        {
            if (SearchResults == null) return;

            SearchScrollBar.MaxValue = SearchResults.Length;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                if (i + SearchScrollBar.Value >= SearchResults.Length)
                {
                    SearchRows[i].MarketInfo = null;
                    SearchRows[i].Loading = false;
                    SearchRows[i].Visible = false;
                    continue;
                }

                if (SearchResults[i + SearchScrollBar.Value] == null)
                {
                    SearchRows[i].Loading = true;
                    SearchRows[i].Visible = true;
                    SearchResults[i + SearchScrollBar.Value] = new ClientMarketPlaceInfo { Loading = true };
                    CEnvir.Enqueue(new C.MarketPlaceSearchIndex { Index = i + SearchScrollBar.Value });
                    continue;
                }

                if (SearchResults[i + SearchScrollBar.Value].Loading) continue;

                SearchRows[i].Loading = false;
                SearchRows[i].MarketInfo = SearchResults[i + SearchScrollBar.Value];
            }

        }
        public void RefreshConsignList()
        {
            ConsignScrollBar.MaxValue = ConsignItems.Count;

            for (int i = 0; i < ConsignRows.Length; i++)
            {
                if (i + ConsignScrollBar.Value >= ConsignItems.Count)
                {
                    ConsignRows[i].MarketInfo = null;
                    ConsignRows[i].Visible = false;
                    continue;
                }

                if (ConsignItems[i + ConsignScrollBar.Value].Loading) continue;

                ConsignRows[i].MarketInfo = ConsignItems[i + ConsignScrollBar.Value];
            }

        }
        public void RefreshStoreList()
        {
            if (StoreSearchResults == null) return;

            StoreScrollBar.MaxValue = StoreSearchResults.Count;

            for (int i = 0; i < StoreRows.Length; i++)
            {
                if (i + StoreScrollBar.Value >= StoreSearchResults.Count)
                {
                    StoreRows[i].StoreInfo = null;
                    StoreRows[i].Visible = false;
                    continue;
                }

                StoreRows[i].StoreInfo = StoreSearchResults[i + StoreScrollBar.Value];
            }

        }

        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedRow?.MarketInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = SelectedRow.MarketInfo.Item.Info;

            if (SelectedRow.MarketInfo.Item.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedRow.MarketInfo.Item.AddedStats[Stat.ItemIndex]);


            message.Append($"Item: {displayInfo.ItemName}");

            if (SelectedRow.MarketInfo.Item.Info.ItemEffect == ItemEffect.ItemPart)
                message.Append(" - [Part]");


            if (BuyCountBox.Value > 1)
                message.Append($" x{BuyCountBox.Value:#,##0}");

            message.Append("\n\n");

            message.Append($"Price: {BuyPriceBox.Value:#,##0}");

            if (BuyCountBox.Value > 1)
                message.Append(" (Each)");

            message.Append("\n\n");

            message.Append($"Total Cost: {BuyTotalBox.TextBox.Text}");

            if (BuyGuildBox.Checked)
                message.Append(" (Using Guild Funds)");


            DXMessageBox box = new DXMessageBox(message.ToString(), "Buy Confirmation", DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>
            {
                BuyButton.Enabled = false;

                CEnvir.Enqueue(new C.MarketPlaceBuy { Index = SelectedRow.MarketInfo.Index, Count = BuyCountBox.Value, GuildFunds = BuyGuildBox.Checked });
                BuyGuildBox.Checked = false;
            };
        }
        private void StoreBuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedStoreRow?.StoreInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();
            
            message.Append($"Item: {SelectedStoreRow.StoreInfo.Item.ItemName}");

            if (StoreBuyCountBox.Value > 1)
                message.Append($" x{StoreBuyCountBox.Value:#,##0}");

            message.Append("\n\n");

            message.Append($"Price: {StoreBuyPriceBox.Value:#,##0}");

            if (StoreBuyCountBox.Value > 1)
                message.Append(" (Each)");

            message.Append("\n\n");

            message.Append($"Total Cost: {StoreBuyTotalBox.TextBox.Text} ({(UseHuntGoldBox.Checked ? "Hunt" : "Game")} Gold)");

            DXMessageBox box = new DXMessageBox(message.ToString(), "Buy Confirmation", DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>
            {
                StoreBuyButton.Enabled = false;

                CEnvir.Enqueue(new C.MarketPlaceStoreBuy { Index = SelectedStoreRow.StoreInfo.Index, Count = StoreBuyCountBox.Value, UseHuntGold = UseHuntGoldBox.Checked});
            };
        }
        private void UpdateBuyTotal(object sender, EventArgs e)
        {
            BuyTotalBox.TextBox.Text = (BuyCountBox.Value * BuyPriceBox.Value).ToString("#,##0");
        }
        private void UpdateStoreBuyTotal(object sender, EventArgs e)
        {
            StoreInfo info = SelectedStoreRow?.StoreInfo;



            if (UseHuntGoldBox.Checked)
            {
                if (info != null)
                    StoreBuyPriceBox.Value = info.HuntGoldPrice == 0 ? info.Price : info.HuntGoldPrice;

                StoreBuyPriceLabel.Text = "H. Gold:";
            }
            else
            {
                if (info != null)
                    StoreBuyPriceBox.Value = info.Price;

                StoreBuyPriceLabel.Text = "G. Gold:";
            }

            StoreBuyTotalBox.TextBox.Text = (StoreBuyCountBox.Value * StoreBuyPriceBox.Value).ToString("#,##0");
        }
        private void ConsignScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshConsignList();
        }
        private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }
        private void ConsignButton_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell cell = ConsignGrid.Grid[0];

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
            
            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = cell.Item.Info;

            if (cell.Item.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == cell.Item.AddedStats[Stat.ItemIndex]);


            message.Append($"Item: {displayInfo.ItemName}");

            if (cell.Item.Info.ItemEffect == ItemEffect.ItemPart)
                message.Append(" - [Part]");


            if (cell.LinkedCount > 1)
                message.Append($" x{cell.LinkedCount:#,##0}");

            message.Append("\n\n");

            message.Append($"Price: {Price:#,##0}");

            if (cell.LinkedCount > 1)
                message.Append(" (Each)");

            message.Append("\n\n");

            message.Append($"Consign Cost: {Cost:#,##0}");

            if (ConsignGuildBox.Checked)
                message.Append(" (Using Guild Funds)");

            DXMessageBox box = new DXMessageBox(message.ToString(), "Consign Confirmation", DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>
            {
                CEnvir.Enqueue(new C.MarketPlaceConsign
                {
                    Link = new CellLinkInfo { GridType = cell.Link.GridType, Count = cell.LinkedCount, Slot = cell.Link.Slot },
                    Price = Price,
                    Message = ConsignMessageBox.TextBox.Text,
                    GuildFunds = ConsignGuildBox.Checked
                });

                cell.Link.Locked = true;
                cell.Link = null;
                ConsignPriceBox.TextBox.Text = "";
                ConsignGuildBox.Checked = false;
            };
        }
        private void StoreScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshStoreList();
        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (SearchButton.Enabled)
                Search();
        }
        private void StoreTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (StoreSearchButton.Enabled)
                StoreSearch();
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

                #region Search
                
                if (SearchTab != null)
                {
                    if (!SearchTab.IsDisposed)
                        SearchTab.Dispose();

                    SearchTab = null;
                }

                if (ItemNameBox != null)
                {
                    if (!ItemNameBox.IsDisposed)
                        ItemNameBox.Dispose();

                    ItemNameBox = null;
                }

                if (BuyTotalBox != null)
                {
                    if (!BuyTotalBox.IsDisposed)
                        BuyTotalBox.Dispose();

                    BuyTotalBox = null;
                }

                if (SearchNumberSoldBox != null)
                {
                    if (!SearchNumberSoldBox.IsDisposed)
                        SearchNumberSoldBox.Dispose();

                    SearchNumberSoldBox = null;
                }

                if (SearchLastPriceBox != null)
                {
                    if (!SearchLastPriceBox.IsDisposed)
                        SearchLastPriceBox.Dispose();

                    SearchLastPriceBox = null;
                }

                if (SearchAveragePriceBox != null)
                {
                    if (!SearchAveragePriceBox.IsDisposed)
                        SearchAveragePriceBox.Dispose();

                    SearchAveragePriceBox = null;
                }

                if (BuyCountBox != null)
                {
                    if (!BuyCountBox.IsDisposed)
                        BuyCountBox.Dispose();

                    BuyCountBox = null;
                }

                if (BuyPriceBox != null)
                {
                    if (!BuyPriceBox.IsDisposed)
                        BuyPriceBox.Dispose();

                    BuyPriceBox = null;
                }

                if (ItemTypeBox != null)
                {
                    if (!ItemTypeBox.IsDisposed)
                        ItemTypeBox.Dispose();

                    ItemTypeBox = null;
                }

                if (SortBox != null)
                {
                    if (!SortBox.IsDisposed)
                        SortBox.Dispose();

                    SortBox = null;
                }

                if (MessagePanel != null)
                {
                    if (!MessagePanel.IsDisposed)
                        MessagePanel.Dispose();

                    MessagePanel = null;
                }

                if (BuyPanel != null)
                {
                    if (!BuyPanel.IsDisposed)
                        BuyPanel.Dispose();

                    BuyPanel = null;
                }

                if (HistoryPanel != null)
                {
                    if (!HistoryPanel.IsDisposed)
                        HistoryPanel.Dispose();

                    HistoryPanel = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (SearchButton != null)
                {
                    if (!SearchButton.IsDisposed)
                        SearchButton.Dispose();

                    SearchButton = null;
                }

                if (BuyGuildBox != null)
                {
                    if (!BuyGuildBox.IsDisposed)
                        BuyGuildBox.Dispose();

                    BuyGuildBox = null;
                }

                if (MessageLabel != null)
                {
                    if (!MessageLabel.IsDisposed)
                        MessageLabel.Dispose();

                    MessageLabel = null;
                }

                if (SearchScrollBar != null)
                {
                    if (!SearchScrollBar.IsDisposed)
                        SearchScrollBar.Dispose();

                    SearchScrollBar = null;
                }
                
                if (SearchRows != null)
                {
                    for (int i = 0; i < SearchRows.Length; i++)
                    {
                        if (SearchRows[i] != null)
                        {
                            if (!SearchRows[i].IsDisposed)
                                SearchRows[i].Dispose();

                            SearchRows[i] = null;
                        }
                    }

                    SearchRows = null;
                }

                SearchResults = null;

                #endregion

                #region Consign

                if (ConsignTab != null)
                {
                    if (!ConsignTab.IsDisposed)
                        ConsignTab.Dispose();

                    ConsignTab = null;
                }

                if (ConsignPriceBox != null)
                {
                    if (!ConsignPriceBox.IsDisposed)
                        ConsignPriceBox.Dispose();

                    ConsignPriceBox = null;
                }

                if (ConsignCostBox != null)
                {
                    if (!ConsignCostBox.IsDisposed)
                        ConsignCostBox.Dispose();

                    ConsignCostBox = null;
                }

                if (NumberSoldBox != null)
                {
                    if (!NumberSoldBox.IsDisposed)
                        NumberSoldBox.Dispose();

                    NumberSoldBox = null;
                }

                if (LastPriceBox != null)
                {
                    if (!LastPriceBox.IsDisposed)
                        LastPriceBox.Dispose();

                    LastPriceBox = null;
                }

                if (AveragePriceBox != null)
                {
                    if (!AveragePriceBox.IsDisposed)
                        AveragePriceBox.Dispose();

                    AveragePriceBox = null;
                }

                if (ConsignMessageBox != null)
                {
                    if (!ConsignMessageBox.IsDisposed)
                        ConsignMessageBox.Dispose();

                    ConsignMessageBox = null;
                }

                if (ConsignPanel != null)
                {
                    if (!ConsignPanel.IsDisposed)
                        ConsignPanel.Dispose();

                    ConsignPanel = null;
                }

                if (ConsignBuyPanel != null)
                {
                    if (!ConsignBuyPanel.IsDisposed)
                        ConsignBuyPanel.Dispose();

                    ConsignBuyPanel = null;
                }

                if (ConsignConfirmPanel != null)
                {
                    if (!ConsignConfirmPanel.IsDisposed)
                        ConsignConfirmPanel.Dispose();

                    ConsignConfirmPanel = null;
                }

                if (ConsignButton != null)
                {
                    if (!ConsignButton.IsDisposed)
                        ConsignButton.Dispose();

                    ConsignButton = null;
                }

                if (ConsignGuildBox != null)
                {
                    if (!ConsignGuildBox.IsDisposed)
                        ConsignGuildBox.Dispose();

                    ConsignGuildBox = null;
                }

                if (ConsignGrid != null)
                {
                    if (!ConsignGrid.IsDisposed)
                        ConsignGrid.Dispose();

                    ConsignGrid = null;
                }

                if (ConsignPriceLabel != null)
                {
                    if (!ConsignPriceLabel.IsDisposed)
                        ConsignPriceLabel.Dispose();

                    ConsignPriceLabel = null;
                }

                if (ConsignScrollBar != null)
                {
                    if (!ConsignScrollBar.IsDisposed)
                        ConsignScrollBar.Dispose();

                    ConsignScrollBar = null;
                }
                
                if (ConsignRows != null)
                {
                    for (int i = 0; i < ConsignRows.Length; i++)
                    {
                        if (ConsignRows[i] != null)
                        {
                            if (!ConsignRows[i].IsDisposed)
                                ConsignRows[i].Dispose();

                            ConsignRows[i] = null;
                        }
                    }

                    ConsignRows = null;
                }

                #endregion

                #region Store

                if (StoreTab != null)
                {
                    if (!StoreTab.IsDisposed)
                        StoreTab.Dispose();

                    StoreTab = null;
                }

                if (StoreItemNameBox != null)
                {
                    if (!StoreItemNameBox.IsDisposed)
                        StoreItemNameBox.Dispose();

                    StoreItemNameBox = null;
                }

                if (StoreBuyTotalBox != null)
                {
                    if (!StoreBuyTotalBox.IsDisposed)
                        StoreBuyTotalBox.Dispose();

                    StoreBuyTotalBox = null;
                }

                if (StoreBuyCountBox != null)
                {
                    if (!StoreBuyCountBox.IsDisposed)
                        StoreBuyCountBox.Dispose();

                    StoreBuyCountBox = null;
                }

                if (StoreBuyPriceBox != null)
                {
                    if (!StoreBuyPriceBox.IsDisposed)
                        StoreBuyPriceBox.Dispose();

                    StoreBuyPriceBox = null;
                }

                if (GameGoldBox != null)
                {
                    if (!GameGoldBox.IsDisposed)
                        GameGoldBox.Dispose();

                    GameGoldBox = null;
                }

                if (HuntGoldBox != null)
                {
                    if (!HuntGoldBox.IsDisposed)
                        HuntGoldBox.Dispose();

                    HuntGoldBox = null;
                }

                if (StoreItemTypeBox != null)
                {
                    if (!StoreItemTypeBox.IsDisposed)
                        StoreItemTypeBox.Dispose();

                    StoreItemTypeBox = null;
                }

                if (StoreSortBox != null)
                {
                    if (!StoreSortBox.IsDisposed)
                        StoreSortBox.Dispose();

                    StoreSortBox = null;
                }

                if (StoreBuyPanel != null)
                {
                    if (!StoreBuyPanel.IsDisposed)
                        StoreBuyPanel.Dispose();

                    StoreBuyPanel = null;
                }

                if (StoreBuyButton != null)
                {
                    if (!StoreBuyButton.IsDisposed)
                        StoreBuyButton.Dispose();

                    StoreBuyButton = null;
                }

                if (StoreSearchButton != null)
                {
                    if (!StoreSearchButton.IsDisposed)
                        StoreSearchButton.Dispose();

                    StoreSearchButton = null;
                }

                if (UseHuntGoldBox != null)
                {
                    if (!UseHuntGoldBox.IsDisposed)
                        UseHuntGoldBox.Dispose();

                    UseHuntGoldBox = null;
                }

                if (StoreScrollBar != null)
                {
                    if (!StoreScrollBar.IsDisposed)
                        StoreScrollBar.Dispose();

                    StoreScrollBar = null;
                }

                if (StoreRows != null)
                {
                    for (int i = 0; i < StoreRows.Length; i++)
                    {
                        if (StoreRows[i] != null)
                        {
                            if (!StoreRows[i].IsDisposed)
                                StoreRows[i].Dispose();

                            StoreRows[i] = null;
                        }
                    }

                    StoreRows = null;
                }

                StoreSearchResults = null;

                #endregion

                _SelectedRow = null;
                SelectedRowChanged = null;

                _SelectedStoreRow = null;
                SelectedStoreRowChanged = null;

                _Price = 0;
                PriceChanged = null;

                ConsignItems = null;

                NextSearchTime = DateTime.MinValue;
            }

        }

        #endregion
    }

    public sealed class MarketPlaceRow : DXControl
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
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MarketInfo

        public ClientMarketPlaceInfo MarketInfo
        {
            get => _MarketInfo;
            set
            {
                ClientMarketPlaceInfo oldValue = _MarketInfo;
                _MarketInfo = value;

                OnMarketInfoChanged(oldValue, value);
            }
        }
        private ClientMarketPlaceInfo _MarketInfo;
        public event EventHandler<EventArgs> MarketInfoChanged;
        public void OnMarketInfoChanged(ClientMarketPlaceInfo oValue, ClientMarketPlaceInfo nValue)
        {
            Visible = MarketInfo != null;
            if (MarketInfo == null)
            {
                return;
            }

            ItemCell.Item = MarketInfo.Item;
            ItemCell.RefreshItem();

            ItemInfo displayInfo = MarketInfo.Item?.Info;

            if (MarketInfo.Item != null && MarketInfo.Item.Info.ItemEffect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MarketInfo.Item.AddedStats[Stat.ItemIndex]);

            string name = displayInfo?.ItemName ?? "Item has been Sold.";

            if (MarketInfo.Item != null && MarketInfo.Item.Info.ItemEffect == ItemEffect.ItemPart)
                name += " - [Part]";

            NameLabel.Text = name;

            if (MarketInfo.Item != null && MarketInfo.Item.AddedStats.Count > 0)
                NameLabel.ForeColour = Color.FromArgb(148, 255, 206);
            else
                NameLabel.ForeColour = Color.FromArgb(198, 166, 99);


            PriceLabel.Text = MarketInfo.Price.ToString("#,##0");
            
            SellerLabel.Text = MarketInfo.Seller;

            SellerLabel.ForeColour = MarketInfo.IsOwner ? Color.Yellow : Color.FromArgb(198, 166, 99);


            if (GameScene.Game.MarketPlaceBox.SelectedRow == this)
                GameScene.Game.MarketPlaceBox.SelectedRow = null;

            MarketInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Loading

        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading == value) return;

                bool oldValue = _Loading;
                _Loading = value;

                OnLoadingChanged(oldValue, value);
            }
        }
        private bool _Loading;
        public event EventHandler<EventArgs> LoadingChanged;
        public void OnLoadingChanged(bool oValue, bool nValue)
        {
            ItemCell.Visible = !Loading;
            PriceLabel.Visible = !Loading;
            PriceLabelLabel.Visible = !Loading;
            SellerLabel.Visible = !Loading;
            SellerLabelLabel.Visible = !Loading;

            if (Loading)
            {
                MarketInfo = null;
                NameLabel.Text = "Loading...";
            }
            else
                NameLabel.Text = "";


            if (GameScene.Game.MarketPlaceBox.SelectedRow == this)
                GameScene.Game.MarketPlaceBox.SelectedRow = null;

            LoadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public DXItemCell ItemCell;
        public DXLabel NameLabel, PriceLabel, PriceLabelLabel, SellerLabel, SellerLabelLabel;

        #endregion

        public MarketPlaceRow()
        {
            Size = new Size(515, 40);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            Visible = false;

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X + ItemCell.Size.Width, 12),
                IsControl = false,
            };
            
            PriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "Price:",
                ForeColour = Color.White,
                IsControl = false,

            };
            PriceLabelLabel.Location = new Point(290 - PriceLabelLabel.Size.Width, 12);

            PriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(290, 12),
                IsControl = false,
            };

            SellerLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "Seller:",
                ForeColour = Color.White,
                IsControl = false,

            };
            SellerLabelLabel.Location = new Point(425 - SellerLabelLabel.Size.Width, 12);

            SellerLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(425, 12),
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _MarketInfo = null;
                MarketInfoChanged = null;

                _Loading = false;
                LoadingChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }
                
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }
                
                if (PriceLabelLabel != null)
                {
                    if (!PriceLabelLabel.IsDisposed)
                        PriceLabelLabel.Dispose();

                    PriceLabelLabel = null;
                }
                
                if (SellerLabel != null)
                {
                    if (!SellerLabel.IsDisposed)
                        SellerLabel.Dispose();

                    SellerLabel = null;
                }
                
                if (SellerLabelLabel != null)
                {
                    if (!SellerLabelLabel.IsDisposed)
                        SellerLabelLabel.Dispose();

                    SellerLabelLabel = null;
                }
                
            }

        }

        #endregion
    }

    public sealed class MarketPlaceStoreRow : DXControl
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
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region StoreInfo

        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;

                StoreInfo oldValue = _StoreInfo;
                _StoreInfo = value;

                OnStoreInfoChanged(oldValue, value);
            }
        }
        private StoreInfo _StoreInfo;
        public event EventHandler<EventArgs> StoreInfoChanged;
        public void OnStoreInfoChanged(StoreInfo oValue, StoreInfo nValue)
        {
            Visible = StoreInfo?.Item != null;
            if (StoreInfo?.Item == null) return;

            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(StoreInfo.Duration);
            
            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;
            
            ItemCell.Item = new ClientUserItem(StoreInfo.Item, 1)
            {
                Flags = flags,
                ExpireTime = duration
            };
            

            ItemCell.RefreshItem();

            NameLabel.Text = StoreInfo.Item.ItemName;

            PriceLabel.Text = StoreInfo.Price.ToString("#,##0");

            if (!StoreInfo.Available)
                PriceLabel.Text = "(Not Available)";



            HuntPriceLabel.Visible = StoreInfo.HuntGoldPrice != 0;

            HuntPriceLabelLabel.Visible = StoreInfo.HuntGoldPrice != 0;
            
            HuntPriceLabel.Text = (StoreInfo.HuntGoldPrice == 0 ? StoreInfo.Price : StoreInfo.HuntGoldPrice).ToString("#,##0");

            if (!StoreInfo.Available)
                HuntPriceLabel.Text = "(Not Available)";


            if (GameScene.Game.MarketPlaceBox.SelectedStoreRow == this)
                GameScene.Game.MarketPlaceBox.SelectedStoreRow = null;

            //TODO If Favourite 
            
            StoreInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        
        public DXItemCell ItemCell;
        public DXLabel NameLabel, PriceLabel, HuntPriceLabel, PriceLabelLabel, HuntPriceLabelLabel;
        public DXButton FavouriteImage;

        #endregion

        public MarketPlaceStoreRow()
        {
            Size = new Size(515, 40);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

           // Visible = false;

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X + ItemCell.Size.Width, 12),
                IsControl = false,
            };


            PriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(290, 12),
                IsControl = false,
            };

            PriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "Game Gold:",
                ForeColour = Color.White,
                IsControl = false,

            };
            PriceLabelLabel.Location = new Point(290 - PriceLabelLabel.Size.Width, 12);
            

            HuntPriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(420, 12),
                IsControl = false,
            };

            HuntPriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "Hunt Gold:",
                ForeColour = Color.White,
                IsControl = false,

            };
            HuntPriceLabelLabel.Location = new Point(420 - HuntPriceLabelLabel.Size.Width, 12);


            FavouriteImage = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 6570,
                Parent = this,
                Hint = "Favourite (NOT YET ENABLED)",
                Enabled = false,
                Visible =  false,
            };
            FavouriteImage.Location = new Point(Size.Width - FavouriteImage.Size.Width - 10, (Size.Height - FavouriteImage.Size.Height)/2);

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _StoreInfo = null;
                StoreInfoChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (PriceLabelLabel != null)
                {
                    if (!PriceLabelLabel.IsDisposed)
                        PriceLabelLabel.Dispose();

                    PriceLabelLabel = null;
                }

                if (HuntPriceLabel != null)
                {
                    if (!HuntPriceLabel.IsDisposed)
                        HuntPriceLabel.Dispose();

                    HuntPriceLabel = null;
                }

                if (HuntPriceLabelLabel != null)
                {
                    if (!HuntPriceLabelLabel.IsDisposed)
                        HuntPriceLabelLabel.Dispose();

                    HuntPriceLabelLabel = null;
                }

                if (FavouriteImage != null)
                {
                    if (!FavouriteImage.IsDisposed)
                        FavouriteImage.Dispose();

                    FavouriteImage = null;
                }
            }

        }

        #endregion
    }

}

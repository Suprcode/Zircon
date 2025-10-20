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
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class LootBoxDialog : DXImageControl
    {
        #region Properties

        public DXItemGrid Grid;

        public DXLabel Message;

        public DXButton CloseButton, TakeItemsButton;
        public DXButton RerollCount, RerollButton, ConfirmChoiceButton;

        private ClientUserItem[] LootBoxArray;
        private DXItemCell SelectedLootBox;
        private int SelectedIndex = -1;

        private LootBoxInfo Info;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
        }

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.LootBoxBox;

        #endregion

        #endregion

        public LootBoxDialog()
        {
            LibraryFile = LibraryFile.GameInter2;
            Index = 2900;
            Movable = false;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
                Hint = CEnvir.Language.CommonControlClose,
                HintPosition = HintPosition.TopLeft
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width, 1);
            CloseButton.MouseClick += (o, e) => Close();

            LootBoxArray = new ClientUserItem[15];
            Grid = new DXItemGrid
            {
                GridSize = new Size(5, 3),
                Parent = this,
                ReadOnly = true,
                Location = new Point(18, 24),
                GridType = GridType.LootBox,
                GridPadding = 4.5F,
                BackColour = Color.Empty,
                ItemGrid = LootBoxArray,
                Border = false
            };

            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.MouseClick += Cell_MouseClick;
            }

            Message = new DXLabel
            {
                Parent = this,
                Size = new Size(235, 62),
                Location = new Point(14, 170),
                Text = "",
                ForeColour = Color.White,
                AutoSize = true
            };

            RerollCount = new DXButton
            {
                Parent = this,
                Location = new Point(15, 235),
                Label = { Text = "" },
                LibraryFile = LibraryFile.GameInter2,
                Index = 2920,
                PressedIndex = 2920,
                HoverIndex = 2920,
                Size = new Size(128, 20),
                CanBePressed = false
            };

            RerollButton = new DXButton
            {
                Parent = this,
                Location = new Point(15, 260),
                Label = { Text = "" },
                LibraryFile = LibraryFile.GameInter2,
                Index = 2926,
                PressedIndex = 2925,
                HoverIndex = 2927,
                Size = new Size(128, 20)
            };
            RerollButton.MouseClick += (o, e) =>
            {
                if (Info == null) return;

                var currency = Info.Currency ?? Globals.CurrencyInfoList.Binding.First(x => x.Type == CurrencyType.GameGold);

                DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.LootBoxRerollMessage, Globals.LootBoxRerollCost, currency.Name), CEnvir.Language.LootBoxRerollTitle, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    var userCurrency = GameScene.Game.User.GetCurrency(currency);

                    if (userCurrency.Amount < Globals.LootBoxRerollCost)
                    {
                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.NotEnoughCurrency, currency.Name), MessageType.System);
                        return;
                    }

                    RerollButton.Enabled = false;

                    CEnvir.Enqueue(new C.LootBoxReroll { Slot = SelectedLootBox.Slot });
                };
            };

            TakeItemsButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.LootBoxTakeItemsButtonLabel },
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Enabled = false
            };
            TakeItemsButton.Location = new Point((DisplayArea.Width - TakeItemsButton.Size.Width) / 2, 245);
            TakeItemsButton.MouseClick += (o, e) =>
            {
                if (Info == null) return;

                DXMessageBox box = new DXMessageBox(CEnvir.Language.LootBoxTakeItemsMessage, CEnvir.Language.LootBoxTakeItemsTitle, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    TakeItemsButton.Enabled = false;

                    CEnvir.Enqueue(new C.LootBoxTakeItems { Slot = SelectedLootBox.Slot, Choice = SelectedIndex });
                };
            };

            ConfirmChoiceButton = new DXButton
            {
                Parent = this,
                Label = { Text = CEnvir.Language.LootBoxConfirmChoiceButtonLabel },
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Enabled = false
            };
            ConfirmChoiceButton.Location = new Point(DisplayArea.Width - TakeItemsButton.Size.Width - 15, 245);
            ConfirmChoiceButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox(CEnvir.Language.LootBoxItemChoiceMessage, CEnvir.Language.LootBoxItemChoiceTitle, DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    ConfirmChoiceButton.Enabled = false;

                    CEnvir.Enqueue(new C.LootBoxConfirmSelection { Slot = SelectedLootBox.Slot });
                };
            };
        }

        private void ResetCells(bool cleanItems = true)
        {
            SelectedIndex = -1;

            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cleanItems)
                {
                    cell.Item = null;
                    cell.Tag = null;
                    cell.LootBoxLocked = false;
                }

                cell.FixedBorder = false;
                cell.Border = false;
                cell.FixedBorderColour = false;
                cell.BorderColour = Color.Lime;
            }
        }

        public void Open(DXItemCell itemCell, List<ClientLootBoxItemInfo> lootBoxItems)
        {
            var item = itemCell.Item;

            if (item == null || item.Info.ItemType != ItemType.LootBox) return;

            Info = Globals.LootBoxInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);

            if (Info == null || Info.Contents.Count == 0) return;

            var currency = Info.Currency ?? Globals.CurrencyInfoList.Binding.First(x => x.Type == CurrencyType.GameGold);

            SelectedLootBox = itemCell;

            ResetCells();

            var remainingRerolls = item.AddedStats[Stat.Counter1];
            var lootboxState = item.AddedStats[Stat.Counter2];

            RerollCount.Label.Text = string.Format(CEnvir.Language.LootBoxRerollCountLabel, remainingRerolls);
            RerollButton.Label.Text = string.Format(CEnvir.Language.LootBoxRerollButtonLabel, Globals.LootBoxRerollCost, currency.Abbreviation);

            RerollCount.Enabled = remainingRerolls > 0;
            RerollButton.Enabled = remainingRerolls > 0;

            if (lootboxState == 1)
            {
                Message.Text = CEnvir.Language.LootBoxShuffleMessage;

                RerollCount.Visible = true;
                RerollButton.Visible = true;
                ConfirmChoiceButton.Visible = true;
                ConfirmChoiceButton.Enabled = true;
                TakeItemsButton.Visible = false;
            }
            else
            {
                var unlockedCount = LootBoxCountUnlocked(SelectedLootBox.Item.CurrentDurability);

                var text = string.Format(CEnvir.Language.LootBoxOpenDescription, Globals.LootBoxRevealCost * unlockedCount, currency.Name);

                Message.Text = text;

                RerollCount.Visible = false;
                RerollButton.Visible = false;
                ConfirmChoiceButton.Visible = false;
                TakeItemsButton.Visible = true;
                TakeItemsButton.Enabled = true;
            }

            for (int i = 0; i < Grid.Grid.Length; i++)
            {
                if (i >= LootBoxInfo.SlotSize) break;

                var lootBoxContent = lootBoxItems.FirstOrDefault(x => x.Slot == i);

                if (lootBoxContent == null && lootboxState == 2)
                {
                    ClientUserItem lootBoxItem = new(item.Info, 1);

                    Grid.Grid[i].Item = lootBoxItem;
                    Grid.Grid[i].LootBoxLocked = true;
                }
                else
                {
                    if (lootBoxContent.ItemInfo == null)
                    {
                        Grid.Grid[i].Item = null;
                    }
                    else
                    {
                        ClientUserItem lootBoxItem = new(lootBoxContent.ItemInfo, lootBoxContent.Amount);
                        Grid.Grid[i].Item = lootBoxItem;
                    }
                }
            }

            Visible = true;
        }

        public void Close()
        {
            ResetCells();

            SelectedLootBox.Locked = false;

            Visible = false;
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (DXItemCell)sender;

            ResetCells(false);

            if (SelectedLootBox == null || Info == null) return;

            if (cell.Item == null) return;

            if (cell.LootBoxLocked)
            {
                var currency = Info.Currency;

                currency ??= Globals.CurrencyInfoList.Binding.First(x => x.Type == CurrencyType.GameGold);

                SelectedIndex = Array.IndexOf(Grid.Grid, cell);

                var unlockedCount = LootBoxCountUnlocked(SelectedLootBox.Item.CurrentDurability);

                if (unlockedCount > 0)
                {
                    DXMessageBox box = new DXMessageBox(string.Format(CEnvir.Language.LootBoxOpenMessage, Globals.LootBoxRevealCost * unlockedCount, currency.Name), CEnvir.Language.LootBoxOpenTitle, DXMessageBoxButtons.YesNo);

                    var userCurrency = GameScene.Game.User.GetCurrency(currency);

                    box.YesButton.MouseClick += (o1, e1) =>
                    {
                        if (userCurrency.Amount < (Globals.LootBoxRevealCost * unlockedCount))
                        {
                            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.NotEnoughCurrency, currency.Name), MessageType.System);
                            return;
                        }

                        CEnvir.Enqueue(new C.LootBoxReveal { Slot = SelectedLootBox.Slot, Choice = SelectedIndex });
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.LootBoxReveal { Slot = SelectedLootBox.Slot, Choice = SelectedIndex });
                }
            }
        }

        private static int LootBoxCountUnlocked(int state)
        {
            int count = 0;

            // Loop through all 16 bits
            for (int i = 0; i < 16; i++)
            {
                // Check if the i-th bit is set
                if ((state & (1 << i)) != 0)
                    count++;
            }

            return count;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (Message != null)
                {
                    if (!Message.IsDisposed)
                        Message.Dispose();

                    Message = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (TakeItemsButton != null)
                {
                    if (!TakeItemsButton.IsDisposed)
                        TakeItemsButton.Dispose();

                    TakeItemsButton = null;
                }

                if (RerollCount != null)
                {
                    if (!RerollCount.IsDisposed)
                        RerollCount.Dispose();

                    RerollCount = null;
                }

                if (RerollButton != null)
                {
                    if (!RerollButton.IsDisposed)
                        RerollButton.Dispose();

                    RerollButton = null;
                }

                if (ConfirmChoiceButton != null)
                {
                    if (!ConfirmChoiceButton.IsDisposed)
                        ConfirmChoiceButton.Dispose();

                    ConfirmChoiceButton = null;
                }

                if (SelectedLootBox != null)
                {
                    SelectedLootBox.Locked = false;
                    SelectedLootBox = null;
                }
            }
        }

        #endregion
    }
}

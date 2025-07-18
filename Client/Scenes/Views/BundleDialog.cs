using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class BundleDialog : DXImageControl
    {
        #region Properties

        public DXLabel TitleLabel;
        public DXItemGrid Grid;

        public DXButton CloseButton, ConfirmButton;

        private ClientUserItem[] BundleArray;
        private DXItemCell SelectedBundle;
        private int SelectedIndex = -1;

        private BundleInfo Info;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
        }

        #region Settings

        public WindowSetting Settings;
        public WindowType Type => WindowType.BundleBox;

        #endregion

        #endregion

        public BundleDialog()
        {
            LibraryFile = LibraryFile.GameInter;
            Index = 3350;
            Movable = true;

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

            BundleArray = new ClientUserItem[16];
            Grid = new DXItemGrid
            {
                GridSize = new Size(4, 4),
                Parent = this,
                ReadOnly = true,
                Location = new Point(15, 48),
                GridType = GridType.Bundle,
                GridPadding = 1,
                BackColour = Color.Empty,
                ItemGrid = BundleArray,
                Border = false
            };

            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.MouseClick += Cell_MouseClick;
            }

            TitleLabel = new DXLabel
            {
                Text = CEnvir.Language.BundleTitle,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 8);

            ConfirmButton = new DXButton
            {
                Parent = this,
                Label = { Text = "" },
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
                Enabled = false
            };
            ConfirmButton.Location = new Point((DisplayArea.Width - ConfirmButton.Size.Width) / 2, 225);
            ConfirmButton.MouseClick += (o, e) =>
            {
                if (Info == null) return;

                ConfirmButton.Enabled = false;

                CEnvir.Enqueue(new C.BundleConfirm { Slot = SelectedBundle.Slot, Choice = SelectedIndex });
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
                }

                cell.FixedBorder = false;
                cell.Border = false;
                cell.FixedBorderColour = false;
                cell.BorderColour = Color.Lime;
            }
        }

        public void Open(DXItemCell itemCell, List<ClientBundleItemInfo> bundleItems)
        {
            var item = itemCell.Item;

            if (item == null || item.Info.ItemType != ItemType.Bundle) return;

            Info = Globals.BundleInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);

            if (Info == null || Info.Contents.Count == 0) return;

            SelectedBundle = itemCell;

            ResetCells();

            switch (Info.Type)
            {
                case BundleType.AnyOf:
                    ConfirmButton.Label.Text = CEnvir.Language.BundleConfirmRandomButtonLabel;
                    ConfirmButton.Enabled = true;

                    if (Info.AutoOpen)
                    {
                        ConfirmButton.InvokeMouseClick();
                        return;
                    }

                    break;
                case BundleType.AllOf:
                    ConfirmButton.Label.Text = CEnvir.Language.BundleConfirmAllButtonLabel;
                    ConfirmButton.Enabled = true;

                    if (Info.AutoOpen)
                    {
                        ConfirmButton.InvokeMouseClick();
                        return;
                    }

                    break;
                case BundleType.OneOf:
                    ConfirmButton.Label.Text = CEnvir.Language.BundleConfirmOneButtonLabel;
                    ConfirmButton.Enabled = false;
                    break;
            }

            for (int i = 0; i < Grid.Grid.Length; i++)
            {
                if (i >= Info.SlotSize) break;

                var bundleContent = bundleItems.FirstOrDefault(x => x.Slot == i);

                if (bundleContent != null)
                {
                    ClientUserItem bundleItem = new(bundleContent.ItemInfo, bundleContent.Amount);
                    Grid.Grid[i].Item = bundleItem;
                }
            }

            Visible = true;
        }

        public void Close()
        {
            ResetCells();

            SelectedBundle.Locked = false;

            Visible = false;
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (DXItemCell)sender;

            ResetCells(false);

            if (SelectedBundle == null || Info == null || Info.Type != BundleType.OneOf) return;

            ConfirmButton.Enabled = false;

            if (cell.Item == null) return;

            for (int i = 0; i < Grid.Grid.Length; i++)
            {
                if (Grid.Grid[i] == cell)
                {
                    cell.Border = true;
                    cell.FixedBorder = true;
                    cell.FixedBorderColour = true;
                    cell.BorderColour = Color.Lime;

                    SelectedIndex = i;

                    ConfirmButton.Enabled = true;

                    break;
                }
            }
        }

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

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();

                    CloseButton = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();

                    ConfirmButton = null;
                }
            }
        }

        #endregion
    }
}

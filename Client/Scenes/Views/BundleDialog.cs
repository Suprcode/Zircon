using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using static Sentry.MeasurementUnit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class BundleDialog : DXImageControl
    {
        #region Properties

        public DXItemGrid Grid;
        public DXLabel Label, PlayerGoldLabel;
        public DXButton CloseButton, ConfirmButton;

        private ClientUserItem[] BundleArray;
        private int SelectedIndex = -1;

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
            Index = 4400;
            Movable = true;

            CloseButton = new DXButton
            {
                Parent = this,
                Index = 15,
                LibraryFile = LibraryFile.Interface,
            };
            CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width, 1);
            CloseButton.MouseClick += (o, e) => Visible = false;

            BundleArray = new ClientUserItem[15];
            Grid = new DXItemGrid
            {
                GridSize = new Size(5, 3),
                Parent = this,
                ReadOnly = true,
                Location = new Point(38, 40),
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

            Label = new DXLabel
            {
                AutoSize = false,
                Border = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Parent = this,
                Location = new Point(30, 170),
                Text = "Select one item...",
                Size = new Size(200, 15)
            };

            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(Grid.Location.X + Grid.Size.Width - 80, Label.Location.Y + 38),
                Label = { Text = "Take Items" },
                ButtonType = ButtonType.Default,
                Size = new Size(100, DefaultHeight),
            };
            ConfirmButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                ConfirmButton.Enabled = false;

                //CEnvir.Enqueue(new C.TradeConfirm());
            };
        }

        private void Reset(bool cleanItems = true)
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

        public void Show(ClientUserItem bundleItem)
        {
            if (bundleItem.Info.ItemType != ItemType.Bundle) return;

            BundleInfo info = Globals.BundleInfoList.Binding.FirstOrDefault(x => x.Index == bundleItem.Info.Shape);

            if (info == null) return;

            Reset();

            int choice = 0;

            foreach (var bundleContent in info.Contents)
            {
                if (choice >= Grid.Grid.Length) return;

                ClientUserItem item = new ClientUserItem(bundleContent.Item, bundleContent.Amount)
                {

                };

                Grid.Grid[choice].Item = item;

                choice++;
            }

            if (info == null) return;

            Visible = true;
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (DXItemCell)sender;

            Reset(false);

            for (int i = 0; i < Grid.Grid.Length; i++)
            {
                if (Grid.Grid[i] == cell)
                {
                    cell.Border = true;
                    cell.FixedBorder = true;
                    cell.FixedBorderColour = true;
                    cell.BorderColour = Color.Lime;

                    SelectedIndex = i;
                }
            }
        }

        #region Methods

        public void Clear()
        {
            Label.Text = "0";
            PlayerGoldLabel.Text = "0";
            ConfirmButton.Enabled = true;

            Reset();
        }

        #endregion


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

                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();

                    Label = null;
                }

                if (PlayerGoldLabel != null)
                {
                    if (!PlayerGoldLabel.IsDisposed)
                        PlayerGoldLabel.Dispose();

                    PlayerGoldLabel = null;
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

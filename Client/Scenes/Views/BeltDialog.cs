using System;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class BeltDialog : DXWindow
    {
        #region Properties

        public ClientBeltLink[] Links;

        public DXItemGrid Grid;
        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (Links == null || Grid == null) return;

            AdjustGrid();
            UpdateLinks();
        }

        private void AdjustGrid()
        {
            Grid?.Dispose();

            int cols = Math.Max(1, (ClientArea.Width) / (DXItemCell.CellWidth - 1));
            int rows = Math.Max(1, (ClientArea.Height) / (DXItemCell.CellHeight - 1));

            Grid = new DXItemGrid
            {
                Parent = this,
                Location = ClientArea.Location,
                GridSize = new Size(cols, rows),
                GridType = GridType.Belt,
                AllowLink = false,
            };

            for (int i = 0; i < Grid.Grid.Length; i++)
            {
                new DXLabel
                {
                    Parent = Grid.Grid[i],
                    Text = ((i + 1) % 10).ToString(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Italic),
                    IsControl = false,
                    Location = new Point(-2, -1)
                };
            }

            Grid.BringToFront();
            Grid.Visible = true;
        }

        public override WindowType Type => WindowType.BeltBox;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;

        #endregion

        public BeltDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            
            AllowResize = true;

            Links = new ClientBeltLink[Globals.MaxBeltCount];
            for (int i = 0; i < Globals.MaxBeltCount; i++)
                Links[i] = new ClientBeltLink { Slot = i };

            Size = GetAcceptableResize(Size.Empty);

            AdjustGrid();
        }

        #region Methods

        public void UpdateLinks()
        {
            foreach (ClientBeltLink link in Links)
            {
                if (link.Slot < 0 || link.Slot >= Grid.Grid.Length) continue;

                if (link.LinkInfoIndex > 0)
                    Grid.Grid[link.Slot].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == link.LinkInfoIndex);
                else if (link.LinkItemIndex > 0)
                    Grid.Grid[link.Slot].QuickItem = GameScene.Game.Inventory.FirstOrDefault(x => x?.Index == link.LinkItemIndex);
            }
        }

        public override Size GetAcceptableResize(Size size)
        {
            Rectangle area = GetClientArea(size);

            int x = (int)Math.Ceiling((area.Width - 10) / (float)DXItemCell.CellWidth);
            int y = (int)Math.Ceiling((area.Height - 10) / (float)DXItemCell.CellHeight);

            if (area.Height > area.Width)
                x = 0;
            else
                y = 0;

            x = Math.Max(1, Math.Min(Globals.MaxBeltCount, x))*(DXItemCell.CellWidth - 1) + 1;
            y = Math.Max(1, Math.Min(Globals.MaxBeltCount, y))*(DXItemCell.CellHeight - 1) + 1;

            if (x >= y)
                x += 10;
            else
                y += 10;

            return GetSize(new Size(x, y));
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Links != null)
                {
                    for (int i = 0; i < Links.Length; i++)
                        Links[i] = null;

                    Links = null;
                }

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }
            }

        }

        #endregion
    }
}

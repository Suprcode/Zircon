using System;
using System.Collections.Generic;
using System.Drawing;
using Client.Envir;
using Library;
using SlimDX;

//Cleaned
namespace Client.Controls
{
    public sealed class DXItemGrid : DXControl
    {
        #region Properies

        #region GridType

        public GridType GridType
        {
            get => _GridType;
            set
            {
                if (_GridType == value) return;

                GridType oldValue = _GridType;
                _GridType = value;

                OnGridTypeChanged(oldValue, value);
            }
        }
        private GridType _GridType;
        public event EventHandler<EventArgs> GridTypeChanged;
        public void OnGridTypeChanged(GridType oValue, GridType nValue)
        {
            foreach (DXItemCell cell in Grid)
                cell.GridType = GridType;

            GridTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region GridSize

        public Size GridSize
        {
            get => _GridSize;
            set
            {
                if (_GridSize == value) return;

                Size oldValue = _GridSize;
                _GridSize = value;

                OnGridSizeChanged(oldValue, value);
            }
        }
        private Size _GridSize;
        public event EventHandler<EventArgs> GridSizeChanged;
        public void OnGridSizeChanged(Size oValue, Size nValue)
        {
            Size = new Size(GridSize.Width * (DXItemCell.CellWidth - 1 + (GridPadding * 2)) + 1, Math.Min(GridSize.Height, VisibleHeight) * (DXItemCell.CellHeight - 1 + (GridPadding * 2)) + 1);
            CreateGrid();

            GridSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region GridPadding

        public int GridPadding
        {
            get => _GridPadding;
            set
            {
                if (_GridPadding == value) return;

                int oldValue = _GridPadding;
                _GridPadding = value;

                OnGridPaddingChanged(oldValue, value);
            }
        }
        private int _GridPadding;
        public event EventHandler<EventArgs> GridPaddingChanged;
        public void OnGridPaddingChanged(int oValue, int nValue)
        {
            Size = new Size(GridSize.Width * (DXItemCell.CellWidth - 1 + (GridPadding * 2)) + 1, Math.Min(GridSize.Height, VisibleHeight) * (DXItemCell.CellHeight - 1 + (GridPadding * 2)) + 1);
            CreateGrid();

            GridPaddingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ItemGrid

        public ClientUserItem[] ItemGrid
        {
            get => _ItemGrid;
            set
            {
                if (_ItemGrid == value) return;

                ClientUserItem[] oldValue = _ItemGrid;
                _ItemGrid = value;

                OnItemGridChanged(oldValue, value);
            }
        }
        private ClientUserItem[] _ItemGrid;
        public event EventHandler<EventArgs> ItemGridChanged;
        public void OnItemGridChanged(ClientUserItem[] oValue, ClientUserItem[] nValue)
        {
            ItemGridChanged?.Invoke(this, EventArgs.Empty);

            foreach (DXItemCell cell in Grid)
                cell.ItemGrid = ItemGrid;
        }

        #endregion

        #region Linked

        public bool Linked
        {
            get => _Linked;
            set
            {
                if (_Linked == value) return;

                bool oldValue = _Linked;
                _Linked = value;

                OnLinkedChanged(oldValue, value);
            }
        }
        private bool _Linked;
        public event EventHandler<EventArgs> LinkedChanged;
        public void OnLinkedChanged(bool oValue, bool nValue)
        {
            foreach (DXItemCell cell in Grid)
                cell.Linked = Linked;

            LinkedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AllowLink

        public bool AllowLink
        {
            get => _AllowLink;
            set
            {
                if (_AllowLink == value) return;

                bool oldValue = _AllowLink;
                _AllowLink = value;

                OnAllowLinkChanged(oldValue, value);
            }
        }
        private bool _AllowLink;
        public event EventHandler<EventArgs> AllowLinkChanged;
        public void OnAllowLinkChanged(bool oValue, bool nValue)
        {
            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.AllowLink = AllowLink;

            AllowLinkChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ReadOnly

        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == value) return;

                bool oldValue = _ReadOnly;
                _ReadOnly = value;

                OnReadOnlyChanged(oldValue, value);
            }
        }
        private bool _ReadOnly;
        public event EventHandler<EventArgs> ReadOnlyChanged;
        public void OnReadOnlyChanged(bool oValue, bool nValue)
        {
            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.ReadOnly = ReadOnly;

            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ScrollValue

        public int ScrollValue
        {
            get => _ScrollValue;
            set
            {
                if (_ScrollValue == value) return;

                int oldValue = _ScrollValue;
                _ScrollValue = value;

                OnScrollValueChanged(oldValue, value);
            }
        }
        private int _ScrollValue;
        public event EventHandler<EventArgs> ScrollValueChanged;
        public void OnScrollValueChanged(int oValue, int nValue)
        {
            UpdateGridDisplay();
            ScrollValueChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region VisibleHeight

        public int VisibleHeight
        {
            get => _VisibleHeight;
            set
            {
                if (_VisibleHeight == value) return;

                int oldValue = _VisibleHeight;
                _VisibleHeight = value;

                OnVisibleHeightChanged(oldValue, value);
            }
        }
        private int _VisibleHeight;
        public event EventHandler<EventArgs> VisibleHeightChanged;
        public void OnVisibleHeightChanged(int oValue, int nValue)
        {
            Size = new Size(GridSize.Width * (DXItemCell.CellWidth - 1) + 1, Math.Min(GridSize.Height, VisibleHeight) * (DXItemCell.CellHeight - 1) + 1);

            UpdateGridDisplay();
            VisibleHeightChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell[] Grid;

        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.Opacity = Opacity;
        }

        #endregion
        
        public DXItemGrid()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(24, 12, 12);
            Border = true;
            BorderColour = Color.FromArgb(99, 83, 50);
            Size = new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);

            AllowLink = true;
            VisibleHeight = 1000;
        }

        #region Methods

        private void CreateGrid()
        {
            if (Grid != null)
                foreach (DXItemCell cell in Grid)
                    cell.Dispose();

            Grid = new DXItemCell[GridSize.Width * GridSize.Height];

            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    Grid[y * GridSize.Width + x] = new DXItemCell
                    {
                        Parent = this,
                        Location = new Point((x * (DXItemCell.CellWidth - 1 + (GridPadding * 2))) + GridPadding, (y * (DXItemCell.CellHeight - 1 + (GridPadding * 2))) + GridPadding),
                        Slot = y * GridSize.Width + x,
                        HostGrid = this,
                        ItemGrid = ItemGrid,
                        GridType = GridType,
                        Linked = Linked,
                        AllowLink = AllowLink,
                        ReadOnly = ReadOnly,
                    };
                }

            UpdateGridDisplay();
        }

        public void UpdateGridDisplay()
        {
            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    DXItemCell cell = Grid[y*GridSize.Width + x];
                    
                    if (y < ScrollValue || y >= ScrollValue + VisibleHeight)
                    {
                        cell.Visible = false;
                        continue;
                    }

                    cell.Visible = true;

                    cell.Location = new Point((x * (DXItemCell.CellWidth - 1 + (GridPadding * 2))) + GridPadding, ((y - ScrollValue) * (DXItemCell.CellHeight - 1 + (GridPadding * 2))) + GridPadding);
                }
        }

        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (!Border || BorderInformation == null) return;

            DXManager.Line.Draw(BorderInformation, BorderColour);

            for (int i = 0; i <= GridSize.Width; i++)
            {
                DXManager.Line.Draw(new[] { 
                    new Vector2(((DXItemCell.CellWidth - 1 + (GridPadding * 2)) * i), 0), 
                    new Vector2(((DXItemCell.CellWidth - 1 + (GridPadding * 2)) * i), Size.Height) 
                }, BorderColour);
            }

            for (int i = 0; i <= Math.Min(GridSize.Height, VisibleHeight); i++)
            {
                DXManager.Line.Draw(new[] { 
                    new Vector2(0, ((DXItemCell.CellHeight - 1 + (GridPadding * 2)) * i)), 
                    new Vector2(Size.Width, ((DXItemCell.CellHeight - 1 + (GridPadding * 2)) * i))
                }, BorderColour);
            }
        }
        
        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || Size.Width == 0 || Size.Height == 0) return;

            List<Vector2> border = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(Size.Width - 1, 0),
                new Vector2(Size.Width - 1, Size.Height - 1),
                new Vector2(0, Size.Height - 1),
                new Vector2(0, 0)
            };


            BorderInformation = border.ToArray();
        }

        protected override void DrawBorder()
        {
        }

        public void ClearLinks()
        {
            if (Grid != null)
                foreach (DXItemCell cell in Grid)
                {
                    if (cell.Link == null) continue;

                    if (cell.Link.GridType == GridType.TradeUser) continue;

                    cell.Link = null;
                }
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _GridType = GridType.None;
                _GridSize = Size.Empty;
                _ItemGrid = null;
                _Linked = false;
                _AllowLink = false;
                _ReadOnly = false;
                _ScrollValue = 0;
                _VisibleHeight = 0;

                GridTypeChanged = null;
                GridSizeChanged = null;
                ItemGridChanged = null;
                LinkedChanged = null;
                AllowLinkChanged = null;
                ReadOnlyChanged = null;
                ScrollValueChanged = null;
                VisibleHeightChanged = null;


                for (int i = 0; i < Grid.Length; i++)
                {
                    if (Grid[i] == null) continue;

                    if (!Grid[i].IsDisposed)
                        Grid[i].Dispose();
                    Grid[i] = null;
                }

                Grid = null;
            }

        }
        #endregion
    }
}

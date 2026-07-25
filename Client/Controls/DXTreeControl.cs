using Library;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Controls
{
    public sealed class DXTreeNode
    {
        public string Text { get; set; }
        public object Tag { get; set; }
        public bool Expanded { get; set; }
        public List<DXTreeNode> Children { get; } = new List<DXTreeNode>();

        public DXTreeNode(string text, object tag = null)
        {
            Text = text;
            Tag = tag;
        }
    }

    public sealed class DXTreeControl : DXControl
    {
        private const int RowHeight = 21;
        private const int RowTopOffset = 2;
        private const int IndentWidth = 15;
        private const int ScrollBarWidth = 18;

        private readonly List<DXTreeRow> _Rows = new List<DXTreeRow>();
        private readonly List<DXTreeNode> _Nodes = new List<DXTreeNode>();
        private DXTreeNode _SelectedNode;
        private int _VisibleRows = 10;

        public IReadOnlyList<DXTreeNode> Nodes => _Nodes;

        public int VisibleRows
        {
            get => _VisibleRows;
            set
            {
                value = Math.Max(1, value);
                if (_VisibleRows == value) return;

                _VisibleRows = value;
                UpdateLayout();
            }
        }

        public DXTreeNode SelectedNode
        {
            get => _SelectedNode;
            set
            {
                if (_SelectedNode == value) return;

                _SelectedNode = value;
                UpdateSelection();
                SelectedNodeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> SelectedNodeChanged;

        public DXControl Container { get; private set; }
        public DXVScrollBar ScrollBar { get; private set; }

        public DXTreeControl()
        {
            Container = new DXControl
            {
                Parent = this,
                Location = Point.Empty,
            };

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Change = 1,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true
            };
            ScrollBar.ValueChanged += (o, e) => PositionRows();

            MouseWheel += ScrollBar.DoMouseWheel;
            UpdateLayout();
        }

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);
            UpdateLayout();
        }

        public void SetNodes(IEnumerable<DXTreeNode> nodes)
        {
            _Nodes.Clear();
            if (nodes != null)
                _Nodes.AddRange(nodes);

            if (_SelectedNode != null && !ContainsNode(_Nodes, _SelectedNode))
                _SelectedNode = null;

            RefreshRows();
        }

        public void RefreshRows()
        {
            foreach (DXTreeRow row in _Rows)
                row.Dispose();

            _Rows.Clear();

            foreach (DXTreeNode node in _Nodes)
                AddVisibleNode(node, 0);

            ScrollBar.MaxValue = _Rows.Count;
            ScrollBar.VisibleSize = VisibleRows;
            PositionRows();
            UpdateSelection();
            UpdateDisplayArea();
            UpdateClipAreaTree();
        }

        private void AddVisibleNode(DXTreeNode node, int depth)
        {
            DXTreeRow row = new DXTreeRow
            {
                Parent = Container,
                Node = node,
                Depth = depth,
                Size = new Size(Math.Max(0, Container.Size.Width - 2), RowHeight),
            };
            row.MouseWheel += ScrollBar.DoMouseWheel;
            row.MouseClick += (o, e) =>
            {
                SelectedNode = row.Node;

                if (row.Depth != 0 || row.Node.Children.Count == 0) return;

                row.Node.Expanded = !row.Node.Expanded;
                RefreshRows();
            };
            row.ExpandButton.MouseClick += (o, e) =>
            {
                if (row.Node.Children.Count == 0) return;
                row.Node.Expanded = !row.Node.Expanded;
                RefreshRows();
            };
            _Rows.Add(row);

            if (!node.Expanded) return;

            foreach (DXTreeNode child in node.Children)
                AddVisibleNode(child, depth + 1);
        }

        private void UpdateLayout()
        {
            if (Container == null || ScrollBar == null) return;

            int height = VisibleRows * RowHeight;
            Container.Size = new Size(Math.Max(0, Size.Width - ScrollBarWidth), height);
            ScrollBar.Location = new Point(Math.Max(0, Size.Width - ScrollBarWidth), 0);
            ScrollBar.Size = new Size(ScrollBarWidth, Size.Height);
            ScrollBar.VisibleSize = VisibleRows;

            foreach (DXTreeRow row in _Rows)
                row.Size = new Size(Math.Max(0, Container.Size.Width - 2), RowHeight);

            PositionRows();
        }

        private void PositionRows()
        {
            for (int i = 0; i < _Rows.Count; i++)
                _Rows[i].Location = new Point(0, RowTopOffset + (i - ScrollBar.Value) * RowHeight);
        }

        private void UpdateSelection()
        {
            foreach (DXTreeRow row in _Rows)
                row.Selected = row.Node == _SelectedNode;
        }

        private static bool ContainsNode(IEnumerable<DXTreeNode> nodes, DXTreeNode target)
        {
            foreach (DXTreeNode node in nodes)
            {
                if (node == target || ContainsNode(node.Children, target))
                    return true;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            SelectedNodeChanged = null;
            _SelectedNode = null;
            _Nodes.Clear();

            foreach (DXTreeRow row in _Rows)
                if (row != null && !row.IsDisposed)
                    row.Dispose();
            _Rows.Clear();

            if (Container != null && !Container.IsDisposed)
                Container.Dispose();
            Container = null;

            if (ScrollBar != null && !ScrollBar.IsDisposed)
                ScrollBar.Dispose();
            ScrollBar = null;
        }
    }

    internal sealed class DXTreeRow : DXControl
    {
        private const int IndentWidth = 15;

        private bool _Selected;
        private int _Depth;
        private DXTreeNode _Node;

        public DXTreeNode Node
        {
            get => _Node;
            set
            {
                _Node = value;
                RefreshNode();
            }
        }

        public int Depth
        {
            get => _Depth;
            set
            {
                _Depth = value;
                RefreshNode();
            }
        }

        public bool Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                DrawTexture = value;
                BackColour = value ? Color.FromArgb(80, 45, 100, 125) : Color.Empty;
            }
        }

        public DXButton ExpandButton { get; private set; }
        public DXLabel TextLabel { get; private set; }

        public DXTreeRow()
        {
            ExpandButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter
            };

            TextLabel = new DXLabel
            {
                Parent = this,
                IsControl = false,
                ForeColour = Color.FromArgb(210, 190, 140),
                Outline = true,
                OutlineColour = Color.Black,
            };
        }

        private void RefreshNode()
        {
            if (ExpandButton == null || TextLabel == null || Node == null) return;

            int indent = Depth * IndentWidth;
            bool hasChildren = Node.Children.Count > 0;
            bool root = Depth == 0;
            bool hasIcon = root || hasChildren;
            ExpandButton.Visible = hasIcon;
            ExpandButton.IsControl = !root && hasChildren;
            ExpandButton.CanBePressed = !root && hasChildren;
            ExpandButton.Index = root ? 4850 : Node.Expanded ? 4871 : 4870;
            ExpandButton.Location = new Point(indent + 2, 4);
            TextLabel.Location = new Point(indent + (hasIcon ? 22 : 7), 2);
            TextLabel.Text = Node.Text ?? string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _Node = null;

            if (ExpandButton != null && !ExpandButton.IsDisposed)
                ExpandButton.Dispose();
            ExpandButton = null;

            if (TextLabel != null && !TextLabel.IsDisposed)
                TextLabel.Dispose();
            TextLabel = null;
        }
    }
}

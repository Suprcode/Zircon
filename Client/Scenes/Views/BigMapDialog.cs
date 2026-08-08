using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class BigMapDialog : DXWindow
    {
        private const int SidePanelWidth = 220;
        private const int SidePanelGap = 6;
        private const int ListRowHeight = 22;
        private const int ListInset = 4;
        private const int ScrollBarWidth = 19;
        private const int ScrollBarGap = 2;
        private const int MaximumVisibleRows = 24;
        // Keeps the map, side panel, and window chrome within the native 1024px footer artwork.
        private const int MaximumMapWidth = 740;
        private const int MaximumMapHeight = 520;
        private const int SelectionSize = 15;

        #region Properties

        #region SelectedInfo

        public MapInfo SelectedInfo
        {
            get { return _SelectedInfo; }
            set
            {
                if (_SelectedInfo == value) return;

                MapInfo oldValue = _SelectedInfo;
                _SelectedInfo = value;

                OnSelectedInfoChanged(oldValue, value);
            }
        }
        private MapInfo _SelectedInfo;
        public event EventHandler<EventArgs> SelectedInfoChanged;
        public void OnSelectedInfoChanged(MapInfo oValue, MapInfo nValue)
        {
            SelectedInfoChanged?.Invoke(this, EventArgs.Empty);

            foreach (DXControl control in MapInfoObjects.Values)
                control.Dispose();

            MapInfoObjects.Clear();
            SelectedNPC = null;

            if (SelectedInfo == null) return;

            TitleLabel.Text = SelectedInfo.PlayerDescription;
            Image.Index = SelectedInfo.MiniMap;

            var minWidth = 320;
            var minHeight = 240;

            var maxWidth = Math.Min(MaximumMapWidth, Math.Max(minWidth, GameScene.Game.Size.Width - SidePanelWidth - SidePanelGap - 60));
            var maxHeight = MaximumMapHeight;

            _MapClientSize = new Size(Math.Min(Math.Max(Image.Size.Width, minWidth), maxWidth), Math.Min(Math.Max(Image.Size.Height, minHeight), maxHeight));
            UpdateWindowSize();

            var imageLargerThanPanel = (Image.Size.Width > maxWidth || Image.Size.Height > maxHeight);

            Image.Movable = imageLargerThanPanel;
            Image.IgnoreMoveBounds = imageLargerThanPanel;

            var locationX = (Image.Size.Width - Panel.Size.Width) / 2;
            var locationY = (Image.Size.Height - Panel.Size.Height) / 2;

            Point defaultImageLocation = new Point(-locationX, -locationY);

            Image.Location = new Point(-locationX, -locationY);

            RecenterButton.Enabled = SelectedInfo != GameScene.Game.MapControl.MapInfo;

            Location = new Point((GameScene.Game.Size.Width - Size.Width) / 2, (GameScene.Game.Size.Height - Size.Height) / 2);

            Size size = GetMapSize(SelectedInfo.FileName);
            ScaleX = Image.Size.Width / (float)size.Width;
            ScaleY = Image.Size.Height / (float)size.Height;

            Image.Location = SelectedInfo == GameScene.Game.MapControl.MapInfo
                ? GetUserCentredImageLocation(defaultImageLocation)
                : defaultImageLocation;

            foreach (NPCInfo ob in Globals.NPCInfoList.Binding)
                Update(ob);

            foreach (MovementInfo ob in Globals.MovementInfoList.Binding)
                Update(ob);

            foreach (ClientObjectData ob in GameScene.Game.DataDictionary.Values)
                Update(ob);

            RefreshSidePanel();
            RefreshAutoPathRoute();
        }

        private Point GetUserCentredImageLocation(Point fallbackLocation)
        {
            if (Image == null || Panel == null) return fallbackLocation;
            if (MapObject.User == null) return fallbackLocation;
            if (SelectedInfo != GameScene.Game.MapControl.MapInfo) return fallbackLocation;

            Point userLocation = MapObject.User.CurrentLocation;
            Size panelSize = Panel.Size;

            float userPixelX = ScaleX * userLocation.X;
            float userPixelY = ScaleY * userLocation.Y;

            int targetX = (int)Math.Round(panelSize.Width / 2f - userPixelX);
            int targetY = (int)Math.Round(panelSize.Height / 2f - userPixelY);

            int minX = Math.Min(0, panelSize.Width - Image.Size.Width);
            int maxX = Math.Max(0, panelSize.Width - Image.Size.Width);
            int minY = Math.Min(0, panelSize.Height - Image.Size.Height);
            int maxY = Math.Max(0, panelSize.Height - Image.Size.Height);

            targetX = Math.Max(minX, Math.Min(maxX, targetX));
            targetY = Math.Max(minY, Math.Min(maxY, targetY));

            return new Point(targetX, targetY);
        }

        public void ToggleOpen(bool open)
        {
            if (open)
            {
                if (!TryShowMap(GameScene.Game.MapControl.MapInfo))
                {
                    return;
                }

                Opacity = 1F;
                Visible = true;
            }
            else
            {
                Visible = false;
            }
        }

        public bool TryShowMap(MapInfo map, bool sendMessage = true)
        {
            if (map == null || map.MiniMap == 0)
            {
                if (sendMessage)
                    GameScene.Game.ReceiveChat("No map available.", MessageType.System);

                return false;
            }

            return true;
        }

        private Size GetMapSize(string fileName)
        {
            var path = Path.Combine(Config.MapPath, fileName + ".map");

            if (!File.Exists(path)) return Size.Empty;

            using (FileStream stream = File.OpenRead(path))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Seek(22, SeekOrigin.Begin);

                return new Size(reader.ReadInt16(), reader.ReadInt16());
            }
        }

        #endregion

        public Rectangle Area;
        public DXImageControl Image;
        public DXControl Panel;
        public DXControl SidePanel;
        public DXTabControl SideTabControl;
        public DXTab NPCTab, MonsterTab;
        public DXVScrollBar NPCScrollBar, MonsterScrollBar;
        public BigMapListRow[] NPCRows, MonsterRows;
        public BigMapSelectionControl NPCSelectionControl;

        public DXButton RecenterButton;

        public static float ScaleX, ScaleY;
        private Size _MapClientSize;
        private readonly List<BigMapNPCListEntry> _NPCEntries = new List<BigMapNPCListEntry>();
        private readonly List<MonsterInfo> _MonsterEntries = new List<MonsterInfo>();

        private NPCInfo SelectedNPC
        {
            get => _SelectedNPC;
            set
            {
                if (_SelectedNPC == value) return;

                _SelectedNPC = value;
                UpdateNPCSelection();
            }
        }
        private NPCInfo _SelectedNPC;

        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();
        public List<AutoPathRouteControl> AutoPathRoutes = new List<AutoPathRouteControl>();

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (Panel == null) return;

            Panel.Location = ClientArea.Location;
            Panel.Size = _MapClientSize;

            if (SidePanel != null)
            {
                SidePanel.Location = new Point(ClientArea.X + _MapClientSize.Width + SidePanelGap, ClientArea.Y);
                SidePanel.Size = new Size(SidePanelWidth, _MapClientSize.Height);
            }

            LayoutSidePanel();
            LayoutFooterButtons();
        }

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            SelectedInfo = IsVisible ? GameScene.Game.MapControl.MapInfo : null;

            BringToFront();
        }

        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            foreach (DXControl control in Controls)
                control.Opacity = Opacity;

            foreach (DXControl control in MapInfoObjects.Values)
                control.Opacity = Opacity;

            foreach (AutoPathRouteControl route in AutoPathRoutes)
                route.Opacity = Opacity;

            if (NPCSelectionControl != null)
                NPCSelectionControl.Opacity = Opacity;

            if (Image != null)
            {
                Image.Opacity = Opacity;
                Image.ImageOpacity = Opacity;
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public BigMapDialog()
        {
            BackColour = Color.Black;
            DropShadow = true;
            HasFooter = true;

            AllowResize = false;

            Panel = new DXControl
            {
                Parent = this,
                Location = Area.Location,
                Size = Area.Size
            };

            Image = new DXImageControl
            {
                Parent = Panel,
                LibraryFile = LibraryFile.MiniMap,
                Movable = true,
                IgnoreMoveBounds = true,
                Clip = true
            };
            Image.MouseClick += Image_MouseClick;
            Image.MouseDoubleClick += Image_MouseDoubleClick;

            RecenterButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = CEnvir.Language.BigMapRecenterLabel },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                LabelStyle = ButtonLabelStyle.Gold,
            };
            RecenterButton.MouseClick += RecenterButton_MouseClick;

            CreateSidePanel();
        }

        private void CreateSidePanel()
        {
            SidePanel = new DXControl
            {
                Parent = this,
                BackColour = Constants.WindowBackColour,
                Border = true,
                BorderColour = Constants.PrimaryColour,
                DrawTexture = true,
            };

            SideTabControl = new DXTabControl
            {
                Parent = SidePanel,
                MarginLeft = 0,
                Padding = 0,
                BackColour = Color.Empty,
            };

            NPCTab = new DXTab
            {
                Parent = SideTabControl,
                MinimumTabWidth = 104,
                BackColour = Color.Empty,
                TabButton = { Label = { Text = CEnvir.Language.BigMapNPCTabLabel } },
            };

            MonsterTab = new DXTab
            {
                Parent = SideTabControl,
                MinimumTabWidth = 104,
                BackColour = Color.Empty,
                TabButton = { Label = { Text = CEnvir.Language.BigMapMonsterTabLabel } },
            };

            NPCRows = CreateRows(NPCTab);
            MonsterRows = CreateRows(MonsterTab);

            NPCScrollBar = CreateScrollBar(NPCTab);
            MonsterScrollBar = CreateScrollBar(MonsterTab);
            NPCScrollBar.ValueChanged += NPCScrollBar_ValueChanged;
            MonsterScrollBar.ValueChanged += MonsterScrollBar_ValueChanged;

            foreach (BigMapListRow row in NPCRows)
            {
                row.MouseClick += NPCRow_MouseClick;
                row.MouseDoubleClick += NPCRow_MouseDoubleClick;
                row.MouseWheel += NPCScrollBar.DoMouseWheel;
            }

            foreach (BigMapListRow row in MonsterRows)
                row.MouseWheel += MonsterScrollBar.DoMouseWheel;

            SideTabControl.SelectedTab = NPCTab;
        }

        private static BigMapListRow[] CreateRows(DXTab tab)
        {
            BigMapListRow[] rows = new BigMapListRow[MaximumVisibleRows];

            for (int i = 0; i < rows.Length; i++)
            {
                BigMapListRow row = new BigMapListRow
                {
                    Parent = tab,
                    Visible = false,
                };
                rows[i] = row;
            }

            return rows;
        }

        private static DXVScrollBar CreateScrollBar(DXTab tab)
        {
            return new DXVScrollBar
            {
                Parent = tab,
                Change = 1,
                MinValue = 0,
                VisibleSize = 1,
                HideWhenNoScroll = true,
                BackColour = Color.Empty,
                Border = false,
                UpButton = { Index = 61, LibraryFile = LibraryFile.Interface },
                DownButton = { Index = 62, LibraryFile = LibraryFile.Interface },
                PositionBar = { Index = 60, LibraryFile = LibraryFile.Interface },
                ShowBackgroundSlider = true,
            };
        }

        private void UpdateWindowSize()
        {
            SetClientSize(new Size(_MapClientSize.Width + SidePanelWidth + SidePanelGap, _MapClientSize.Height));
        }

        private void LayoutFooterButtons()
        {
            if (RecenterButton == null) return;

            RecenterButton.Location = new Point(Size.Width - 30 - RecenterButton.Size.Width, Size.Height - 43);
        }

        private void LayoutSidePanel()
        {
            if (SidePanel == null || SideTabControl == null || NPCRows == null || MonsterRows == null) return;

            SideTabControl.Location = new Point(4, 4);
            SideTabControl.Size = new Size(SidePanel.Size.Width - 8, SidePanel.Size.Height - 8);

            LayoutList(NPCTab, NPCRows, NPCScrollBar, _NPCEntries.Count);
            LayoutList(MonsterTab, MonsterRows, MonsterScrollBar, _MonsterEntries.Count);
            RefreshNPCList();
            RefreshMonsterList();
        }

        private void LayoutList(DXTab tab, BigMapListRow[] rows, DXVScrollBar scrollBar, int itemCount)
        {
            if (tab == null || rows == null || scrollBar == null) return;

            int contentHeight = Math.Max(0, tab.Size.Height - ListInset * 2);
            int visibleRows = Math.Min(rows.Length, Math.Max(1, (contentHeight + ListRowHeight - 1) / ListRowHeight));
            bool needsScrollBar = itemCount > visibleRows;
            int rowWidth = tab.Size.Width - ListInset * 2 -
                           (needsScrollBar ? ScrollBarWidth + ScrollBarGap : 0);

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i].Location = new Point(ListInset, ListInset + i * ListRowHeight);
                int remainingHeight = contentHeight - i * ListRowHeight;
                rows[i].Size = new Size(rowWidth, Math.Min(ListRowHeight - 1, Math.Max(0, remainingHeight)));
                if (i >= visibleRows)
                    rows[i].Visible = false;
            }

            scrollBar.Location = new Point(tab.Size.Width - ListInset - ScrollBarWidth, ListInset);
            scrollBar.Size = new Size(ScrollBarWidth, contentHeight);
            scrollBar.VisibleSize = Math.Max(1, visibleRows);
        }

        private void RefreshSidePanel()
        {
            _NPCEntries.Clear();
            _MonsterEntries.Clear();

            if (SelectedInfo != null)
            {
                foreach (IGrouping<NPCCategory, NPCInfo> group in Globals.NPCInfoList.Binding
                             .Where(x => x.Region?.Map == SelectedInfo)
                             .GroupBy(x => x.Category)
                             .OrderBy(x => (int)x.Key))
                {
                    _NPCEntries.Add(new BigMapNPCListEntry(group.Key));

                    _NPCEntries.AddRange(group
                        .OrderBy(GetNPCDisplayName, StringComparer.CurrentCultureIgnoreCase)
                        .ThenBy(x => x.Index)
                        .Select(x => new BigMapNPCListEntry(x)));
                }

                _MonsterEntries.AddRange(SelectedInfo.Regions
                    .SelectMany(x => x.Respawns)
                    .Where(x => x.Monster != null)
                    .Select(x => x.Monster)
                    .Distinct()
                    .OrderBy(x => x.MonsterName, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(x => x.Index));
            }

            if (NPCScrollBar != null)
            {
                NPCScrollBar.Value = 0;
                NPCScrollBar.MaxValue = _NPCEntries.Count;
            }

            if (MonsterScrollBar != null)
            {
                MonsterScrollBar.Value = 0;
                MonsterScrollBar.MaxValue = _MonsterEntries.Count;
            }

            LayoutList(NPCTab, NPCRows, NPCScrollBar, _NPCEntries.Count);
            LayoutList(MonsterTab, MonsterRows, MonsterScrollBar, _MonsterEntries.Count);
            RefreshNPCList();
            RefreshMonsterList();
        }

        private void NPCScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshNPCList();
        }

        private void MonsterScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshMonsterList();
        }

        private void RefreshNPCList()
        {
            if (NPCRows == null || NPCScrollBar == null) return;

            int visibleRows = Math.Min(NPCRows.Length, NPCScrollBar.VisibleSize);
            for (int i = 0; i < NPCRows.Length; i++)
            {
                int index = NPCScrollBar.Value + i;
                bool hasEntry = index < _NPCEntries.Count;

                BigMapNPCListEntry entry = hasEntry ? _NPCEntries[index] : null;

                NPCRows[i].Entry = entry?.NPC;
                NPCRows[i].Heading = entry?.IsHeading == true;
                NPCRows[i].DisplayText = entry == null
                    ? string.Empty
                    : entry.IsHeading
                        ? entry.Category.ToString()
                        : GetNPCDisplayName(entry.NPC);
                NPCRows[i].Selected = entry?.NPC != null && entry.NPC == SelectedNPC;
                NPCRows[i].Visible = i < visibleRows;
            }
        }

        private void RefreshMonsterList()
        {
            if (MonsterRows == null || MonsterScrollBar == null) return;

            int visibleRows = Math.Min(MonsterRows.Length, MonsterScrollBar.VisibleSize);
            for (int i = 0; i < MonsterRows.Length; i++)
            {
                int index = MonsterScrollBar.Value + i;
                bool hasEntry = index < _MonsterEntries.Count;

                MonsterRows[i].Entry = hasEntry ? _MonsterEntries[index] : null;
                MonsterRows[i].DisplayText = hasEntry ? _MonsterEntries[index].MonsterName : string.Empty;
                MonsterRows[i].Selected = false;
                MonsterRows[i].Visible = i < visibleRows;
            }
        }

        private void NPCRow_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || sender is not BigMapListRow row || row.Entry is not NPCInfo npc) return;

            SelectedNPC = npc;
        }

        private void NPCRow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || sender is not BigMapListRow row || row.Entry is not NPCInfo npc) return;

            SelectedNPC = npc;
            CEnvir.Enqueue(new C.AutoPathStart { NPCIndex = npc.Index });
        }

        public void SelectNPC(int index)
        {
            BigMapNPCListEntry entry = _NPCEntries.FirstOrDefault(x => x.NPC?.Index == index);
            if (entry == null) return;

            SideTabControl.SelectedTab = NPCTab;

            int entryIndex = _NPCEntries.IndexOf(entry);
            int visibleSize = Math.Max(1, NPCScrollBar.VisibleSize);

            if (entryIndex < NPCScrollBar.Value)
                NPCScrollBar.Value = entryIndex;
            else if (entryIndex >= NPCScrollBar.Value + visibleSize)
                NPCScrollBar.Value = entryIndex - visibleSize + 1;

            SelectedNPC = entry.NPC;
        }

        private static string GetNPCDisplayName(NPCInfo npc)
        {
            return npc?.NPCName?.Replace('_', ' ') ?? string.Empty;
        }

        private void UpdateNPCSelection()
        {
            RefreshNPCList();

            if (NPCSelectionControl != null)
            {
                if (!NPCSelectionControl.IsDisposed)
                    NPCSelectionControl.Dispose();

                NPCSelectionControl = null;
            }

            if (SelectedNPC == null || Image == null ||
                !TryGetRegionPixelLocation(SelectedNPC.Region, out Point location)) return;

            NPCSelectionControl = new BigMapSelectionControl
            {
                Parent = Image,
                Size = new Size(SelectionSize, SelectionSize),
                Location = new Point(
                    location.X - SelectionSize / 2,
                    location.Y - SelectionSize / 2),
                Opacity = Opacity,
            };
            NPCSelectionControl.BringToFront();
        }

        private void RecenterButton_MouseClick(object sender, MouseEventArgs e)
        {
            GameScene.Game.BigMapBox.SelectedInfo = GameScene.Game.MapControl.MapInfo;
        }

        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                int x = (int)((e.Location.X - Image.DisplayArea.X) / ScaleX);
                int y = (int)((e.Location.Y - Image.DisplayArea.Y) / ScaleY);

                GameScene.Game.CancelAutoPath();
                CEnvir.Enqueue(new C.TeleportRing { Location = new Point(x, y), Index = SelectedInfo.Index });
            }
        }

        private void Image_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || SelectedInfo == null) return;

            int x = (int)((e.Location.X - Image.DisplayArea.X) / ScaleX);
            int y = (int)((e.Location.Y - Image.DisplayArea.Y) / ScaleY);
            CEnvir.Enqueue(new C.AutoPathWaypoint
            {
                MapIndex = SelectedInfo.Index,
                Location = new Point(x, y),
            });
        }

        #region Methods
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            OnBeforeDraw();
            DrawControl();
            DrawWindow();
            OnBeforeChildrenDraw();
            DrawChildControls();
            TitleLabel.Draw();
            DrawBorder();
            OnAfterDraw();
        }

        public void Update(NPCInfo ob)
        {
            if (SelectedInfo == null) return;

            if (!MapInfoObjects.TryGetValue(ob, out DXControl control))
            {
                if (ob.Region?.Map != SelectedInfo) return;

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = Image;
                control.Opacity = Opacity;
                AddAutoPathHandler(control, ob);
                MapInfoObjects[ob] = control;
            }
            else if ((CurrentQuest)control.Tag == ob.CurrentQuest) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != SelectedInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Parent = Image;
            control.Opacity = Opacity;
            AddAutoPathHandler(control, ob);
            MapInfoObjects[ob] = control;

            if (!TryGetRegionPixelLocation(ob.Region, out Point location)) return;

            control.Location = new Point(location.X - control.Size.Width / 2, location.Y - control.Size.Height / 2);
        }

        private bool TryGetRegionPixelLocation(MapRegion region, out Point location)
        {
            location = Point.Empty;
            if (SelectedInfo == null || region == null) return false;

            Size size = GetMapSize(SelectedInfo.FileName);

            if (region.PointList == null)
                region.CreatePoints(size.Width);

            if (region.PointList == null || region.PointList.Count == 0) return false;

            int minX = size.Width, maxX = 0, minY = size.Height, maxY = 0;

            foreach (Point point in region.PointList)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minY = Math.Min(minY, point.Y);
                maxY = Math.Max(maxY, point.Y);
            }

            location = new Point(
                (int)(ScaleX * ((minX + maxX) / 2)),
                (int)(ScaleY * ((minY + maxY) / 2)));
            return true;
        }

        private static void AddAutoPathHandler(DXControl control, NPCInfo npc)
        {
            control.MouseDoubleClick += (o, e) =>
            {
                if (e.Button != MouseButtons.Left) return;

                CEnvir.Enqueue(new C.AutoPathStart { NPCIndex = npc.Index });
            };
        }

        public void RefreshAutoPathRoute()
        {
            foreach (AutoPathRouteControl route in AutoPathRoutes)
            {
                if (!route.IsDisposed)
                    route.Dispose();
            }
            AutoPathRoutes.Clear();

            if (SelectedInfo == null || Image == null) return;

            for (int i = 0; i < GameScene.Game.AutoPathRoutes.Count; i++)
            {
                AutoPathRoute route = GameScene.Game.AutoPathRoutes[i];
                AutoPathRouteControl control = new AutoPathRouteControl
                {
                    Parent = Image,
                    Opacity = Opacity,
                };

                control.SetRoute(
                    route,
                    i == 0,
                    SelectedInfo,
                    GameScene.Game.MapControl.MapInfo,
                    ScaleX,
                    ScaleY);
                AutoPathRoutes.Add(control);
            }

            UpdateAutoPathRouteProgress();
        }

        public void UpdateAutoPathRouteProgress()
        {
            if (AutoPathRoutes.Count == 0) return;

            AutoPathRoutes[0].SetProgress(
                GameScene.Game.AutoPathRouteProgressMapIndex,
                GameScene.Game.AutoPathRouteProgressIndex);
        }

        public void Update(MovementInfo ob)
        {
            if (ob.SourceRegion == null || ob.SourceRegion.Map != SelectedInfo) return;
            if (ob.DestinationRegion?.Map == null || ob.Icon == MapIcon.None) return;

            if (GameScene.Game.MapControl.InstanceInfo != null)
            {
                if (!GameScene.Game.MapControl.InstanceInfo.Maps.Any(m => m.Map == ob.SourceRegion.Map) && ob.NeedInstance == null) return;
                if (!GameScene.Game.MapControl.InstanceInfo.Maps.Any(m => m.Map == ob.DestinationRegion?.Map) && ob.NeedInstance == null) return;
            }

            Size size = GetMapSize(SelectedInfo.FileName);

            if (ob.SourceRegion.PointList == null)
                ob.SourceRegion.CreatePoints(size.Width);

            int minX = size.Width, maxX = 0, minY = size.Height, maxY = 0;

            foreach (Point point in ob.SourceRegion.PointList)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int x = (minX + maxX) / 2;
            int y = (minY + maxY) / 2;

            DXImageControl control;
            MapInfoObjects[ob] = control = new DXImageControl
            {
                LibraryFile = LibraryFile.MiniMapIcon,
                Parent = Image,
                Opacity = Opacity,
                ImageOpacity = Opacity,
                Hint = ob.DestinationRegion.Map.PlayerDescription
            };
            control.OpacityChanged += (o, e) => control.ImageOpacity = control.Opacity;

            GameScene.Game.UpdateMapIcon(control, ob.Icon);

            control.MouseClick += (o, e) =>
            {
                if (!TryShowMap(ob.DestinationRegion.Map))
                {
                    return;
                }

                SelectedInfo = ob.DestinationRegion.Map;
            };
            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }

        public void Update(ClientObjectData ob)
        {
            if (SelectedInfo == null) return;

            if (!MapInfoObjects.TryGetValue(ob, out DXControl existing))
            {
                if (ob.MapIndex != SelectedInfo.Index) return;
                if (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common) return;
                if (ob.MonsterInfo != null && (ob.Dead || ob.MonsterInfo.Image == MonsterImage.None)) return;

                DXControl created = CreateMapInfoObject();
                MapInfoObjects[ob] = created;
                existing = created;
            }
            else if (ob.MapIndex != SelectedInfo.Index || (ob.MonsterInfo != null && ob.Dead) || (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common))
            {
                existing.Dispose();
                MapInfoObjects.Remove(ob);
                return;
            }

            DXControl control = existing as DXControl;
            if (control == null)
            {
                existing.Dispose();

                DXControl created = CreateMapInfoObject();
                MapInfoObjects[ob] = created;
                control = created;
            }

            Size size = new Size(3, 3);
            Color colour = Color.White;
            string name = ob.Name;
            bool isGroupMember = GameScene.Game.GroupBox.Members.Any(x => x.ObjectID == ob.ObjectID);
            if (control is DXMapInfoControl mapInfoControl)
                mapInfoControl.Hollow = false;

            control.Visible = true;

            if (ob.MonsterInfo != null)
            {
                name = $"{ob.MonsterInfo.MonsterName}";

                if (ob.MonsterInfo.AI < 0)
                {
                    colour = Color.Red;
                }
                else
                {
                    colour = Color.Red;

                    if (GameScene.Game.HasQuest(ob.MonsterInfo, GameScene.Game.MapControl.MapInfo))
                        colour = Color.Orange;
                }

                if (ob.MonsterInfo.Flag == MonsterFlag.CastleObjective || ob.MonsterInfo.Flag == MonsterFlag.CastleDefense)
                {
                    control.Visible = false;
                }

                if (ob.MonsterInfo.IsBoss)
                {
                    size = new Size(5, 5);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXMapInfoControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = colour,
                            Size = new Size(3, 3)
                        };
                    }
                    else
                        control.Controls[0].BackColour = colour;

                    colour = Color.White;

                }

                if (!string.IsNullOrEmpty(ob.PetOwner))
                {
                    name += $" ({ob.PetOwner})";

                    if (ob.PetOwner == GameScene.Game.User.Name)
                    {
                        colour = Color.Orange;
                        size = new Size(4, 4);
                    }
                    else
                    {
                        colour = Color.Red;
                    }
                }
            }
            else if (ob.ItemInfo != null)
            {
                colour = Color.DarkBlue;
            }
            else
            {
                if (MapObject.User.ObjectID == ob.ObjectID)
                {
                    size = new Size(5, 5);
                    control.Border = false;
                    control.BorderSize = 1f;
                    control.BorderColour = Color.Transparent;
                    colour = Color.Lime;
                    if (control is DXMapInfoControl playerControl)
                        playerControl.Hollow = true;

                    if (SelectedInfo == GameScene.Game.MapControl.MapInfo)
                    {
                        RecenterButton.Enabled = false;
                    }

                    var overlay = DXMapInfoControl.GetOverlay(control);

                    control.ProcessAction = () =>
                    {
                        if (overlay?.IsBorderAnimationActive == false)
                        {
                            bool isVisibleSecond = CEnvir.Now.Millisecond < 500;

                            control.Border = false;
                            control.BackColour = isVisibleSecond ? Color.Lime : Color.Transparent;
                            if (control is DXMapInfoControl playerControl)
                                playerControl.Hollow = true;
                        }
                        else
                        {
                            control.Border = false;
                            control.BackColour = Color.Lime;
                            if (control is DXMapInfoControl playerControl)
                                playerControl.Hollow = true;
                        }
                    };
                }
                else if (isGroupMember)
                {
                    colour = Color.Lime;
                    size = new Size(4, 4);
                }
                else if (GameScene.Game.Observer)
                {
                    control.Visible = false;
                }
                else if (GameScene.Game.Partner != null && GameScene.Game.Partner.ObjectID == ob.ObjectID)
                {
                    colour = Color.DeepPink;
                }
                else if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(x => x.ObjectID == ob.ObjectID))
                {
                    colour = Color.DeepSkyBlue;
                }
            }

            control.Hint = name;
            control.BackColour = colour;
            control.Size = size;
            control.Location = new Point((int)(ScaleX * ob.Location.X) - size.Width / 2, (int)(ScaleY * ob.Location.Y) - size.Height / 2);
        }

        public void PlayLocatorAnim(long id)
        {
            if (MapInfoObjects.Keys.OfType<ClientObjectData>().FirstOrDefault(x => x.ObjectID == id) is { } ob)
            {
                if (!MapInfoObjects.TryGetValue(ob, out var control))
                    return;

                DXMapInfoControl.GetOverlay(control)?.PlayBorderAnimation(Color.Lime);
            }
            else if (MapInfoObjects.Keys.OfType<NPCInfo>().FirstOrDefault(x => x.Index == id) is { } npcOb)
            {
                if (!MapInfoObjects.TryGetValue(npcOb, out var control))
                    return;

                DXMapInfoControl.GetOverlay(control)?.PlayBorderAnimation(Color.Yellow);
            }
        }

        private DXControl CreateMapInfoObject()
        {
            return new DXMapInfoControl
            {
                Parent = Image,
                Opacity = Opacity,
            };
        }

        public void Remove(object ob)
        {
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedInfo = null;
                _SelectedNPC = null;
                SelectedInfoChanged = null;

                Area = Rectangle.Empty;
                _MapClientSize = Size.Empty;
                ScaleX = 0;
                ScaleY = 0;

                _NPCEntries.Clear();
                _MonsterEntries.Clear();

                foreach (KeyValuePair<object, DXControl> pair in MapInfoObjects)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                MapInfoObjects.Clear();
                MapInfoObjects = null;

                if (AutoPathRoutes != null)
                {
                    foreach (AutoPathRouteControl route in AutoPathRoutes)
                    {
                        if (!route.IsDisposed)
                            route.Dispose();
                    }

                    AutoPathRoutes.Clear();
                    AutoPathRoutes = null;
                }

                if (Image != null)
                {
                    Image.MouseClick -= Image_MouseClick;
                    Image.MouseDoubleClick -= Image_MouseDoubleClick;

                    if (!Image.IsDisposed)
                        Image.Dispose();

                    Image = null;
                }

                if (Panel != null)
                {
                    if (!Panel.IsDisposed)
                        Panel.Dispose();

                    Panel = null;
                }

                if (RecenterButton != null)
                {
                    RecenterButton.MouseClick -= RecenterButton_MouseClick;

                    if (!RecenterButton.IsDisposed)
                        RecenterButton.Dispose();

                    RecenterButton = null;
                }

                if (NPCSelectionControl != null)
                {
                    if (!NPCSelectionControl.IsDisposed)
                        NPCSelectionControl.Dispose();

                    NPCSelectionControl = null;
                }

                if (NPCRows != null)
                {
                    foreach (BigMapListRow row in NPCRows)
                    {
                        if (row == null) continue;

                        row.MouseClick -= NPCRow_MouseClick;
                        row.MouseDoubleClick -= NPCRow_MouseDoubleClick;
                        if (NPCScrollBar != null)
                            row.MouseWheel -= NPCScrollBar.DoMouseWheel;

                        if (!row.IsDisposed)
                            row.Dispose();
                    }

                    NPCRows = null;
                }

                if (MonsterRows != null)
                {
                    foreach (BigMapListRow row in MonsterRows)
                    {
                        if (row == null) continue;

                        if (MonsterScrollBar != null)
                            row.MouseWheel -= MonsterScrollBar.DoMouseWheel;

                        if (!row.IsDisposed)
                            row.Dispose();
                    }

                    MonsterRows = null;
                }

                if (NPCScrollBar != null)
                {
                    NPCScrollBar.ValueChanged -= NPCScrollBar_ValueChanged;

                    if (!NPCScrollBar.IsDisposed)
                        NPCScrollBar.Dispose();

                    NPCScrollBar = null;
                }

                if (MonsterScrollBar != null)
                {
                    MonsterScrollBar.ValueChanged -= MonsterScrollBar_ValueChanged;

                    if (!MonsterScrollBar.IsDisposed)
                        MonsterScrollBar.Dispose();

                    MonsterScrollBar = null;
                }

                if (NPCTab != null)
                {
                    if (!NPCTab.IsDisposed)
                        NPCTab.Dispose();

                    NPCTab = null;
                }

                if (MonsterTab != null)
                {
                    if (!MonsterTab.IsDisposed)
                        MonsterTab.Dispose();

                    MonsterTab = null;
                }

                if (SideTabControl != null)
                {
                    if (!SideTabControl.IsDisposed)
                        SideTabControl.Dispose();

                    SideTabControl = null;
                }

                if (SidePanel != null)
                {
                    if (!SidePanel.IsDisposed)
                        SidePanel.Dispose();

                    SidePanel = null;
                }

            }
        }

        #endregion
    }

    internal sealed class BigMapNPCListEntry
    {
        public NPCCategory Category { get; }
        public NPCInfo NPC { get; }
        public bool IsHeading => NPC == null;

        public BigMapNPCListEntry(NPCCategory category)
        {
            Category = category;
        }

        public BigMapNPCListEntry(NPCInfo npc)
        {
            NPC = npc;
            Category = npc.Category;
        }
    }

    public sealed class BigMapListRow : DXControl
    {
        public object Entry;
        public DXLabel NameLabel;

        public bool Heading
        {
            get => _Heading;
            set
            {
                if (_Heading == value) return;

                _Heading = value;
                UpdateColours();
            }
        }
        private bool _Heading;

        public string DisplayText
        {
            get => NameLabel?.Text ?? string.Empty;
            set
            {
                if (NameLabel != null)
                    NameLabel.Text = value;
            }
        }

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                _Selected = value;
                UpdateColours();
            }
        }
        private bool _Selected;

        public BigMapListRow()
        {
            DrawTexture = true;
            BackColour = Constants.RowBackColour;

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(5, 3),
                IsControl = false,
            };

            UpdateColours();
        }

        private void UpdateColours()
        {
            BackColour = Heading
                ? Constants.WindowBackColour
                : Selected
                    ? Constants.SelectedRowBackColour
                    : Constants.RowBackColour;

            if (NameLabel == null) return;

            NameLabel.ForeColour = Heading ? Color.White : Constants.PrimaryColour;
            NameLabel.Location = new Point(Heading ? 5 : 10, 3);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            Entry = null;
            _Heading = false;
            _Selected = false;

            if (NameLabel != null)
            {
                if (!NameLabel.IsDisposed)
                    NameLabel.Dispose();

                NameLabel = null;
            }
        }
    }

    public sealed class BigMapSelectionControl : DXControl
    {
        private static readonly Rectangle[] Ring =
        {
            new Rectangle(4, 0, 7, 1),
            new Rectangle(2, 1, 3, 1),
            new Rectangle(10, 1, 3, 1),
            new Rectangle(1, 2, 1, 3),
            new Rectangle(13, 2, 1, 3),
            new Rectangle(0, 4, 1, 7),
            new Rectangle(14, 4, 1, 7),
            new Rectangle(1, 10, 1, 3),
            new Rectangle(13, 10, 1, 3),
            new Rectangle(2, 13, 3, 1),
            new Rectangle(10, 13, 3, 1),
            new Rectangle(4, 14, 7, 1),
        };

        public BigMapSelectionControl()
        {
            PassThrough = true;
            CacheInParent = false;
        }

        protected override void DrawControl()
        {
            if (Rectangle.Intersect(ClipArea, DisplayArea).IsEmpty) return;

            float oldOpacity = RenderingPipelineManager.GetOpacity();
            RenderingPipelineManager.SetOpacity(Opacity);

            foreach (Rectangle part in Ring)
            {
                Rectangle area = part;
                area.Offset(DisplayArea.Location);
                area = Rectangle.Intersect(area, ClipArea);
                if (area.Width <= 0 || area.Height <= 0) continue;

                RenderingPipelineManager.FillRectangle(area, Color.Lime);
            }

            RenderingPipelineManager.SetOpacity(oldOpacity);
        }
    }

}

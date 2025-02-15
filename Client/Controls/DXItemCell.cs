using Client.Envir;
using Client.Models;
using Client.Scenes;
using Client.Scenes.Views;
using Client.UserModels;
using Library;
using Library.SystemModels;
using SlimDX;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Controls
{
    public sealed class DXItemCell : DXControl
    {
        #region Static
        public static DXItemCell SelectedCell
        {
            get => GameScene.Game.SelectedCell;
            set => GameScene.Game.SelectedCell = value;
        }

        public const int CellWidth = 36;
        public const int CellHeight = 36;

        #endregion

        #region Properties

        #region FixedBorder

        public bool FixedBorder
        {
            get => _FixedBorder;
            set
            {
                if (_FixedBorder == value) return;

                bool oldValue = _FixedBorder;
                _FixedBorder = value;

                OnFixedBorderChanged(oldValue, value);
            }
        }
        private bool _FixedBorder;
        public event EventHandler<EventArgs> FixedBorderChanged;
        public void OnFixedBorderChanged(bool oValue, bool nValue)
        {
            FixedBorderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region FixedBorderColour

        public bool FixedBorderColour
        {
            get => _FixedBorderColour;
            set
            {
                if (_FixedBorderColour == value) return;

                bool oldValue = _FixedBorderColour;
                _FixedBorderColour = value;

                OnFixedBorderColourChanged(oldValue, value);
            }
        }
        private bool _FixedBorderColour;
        public event EventHandler<EventArgs> FixedBorderColourChanged;
        public void OnFixedBorderColourChanged(bool oValue, bool nValue)
        {
            FixedBorderColourChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
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
            GridTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region HostGrid

        public DXItemGrid HostGrid
        {
            get => _HostGrid;
            set
            {
                if (_HostGrid == value) return;

                DXItemGrid oldValue = _HostGrid;
                _HostGrid = value;

                OnHostGridChanged(oldValue, value);
            }
        }
        private DXItemGrid _HostGrid;
        public event EventHandler<EventArgs> HostGridChanged;
        public void OnHostGridChanged(DXItemGrid oValue, DXItemGrid nValue)
        {
            HostGridChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region Item

        public ClientUserItem Item
        {
            get
            {
                if (GridType == GridType.Belt || GridType == GridType.AutoPotion)
                {
                    if (QuickInfo != null)
                        return QuickInfoItem;

                    return QuickItem;
                }

                if (Linked)
                    return Link?.Item;

                if (ItemGrid == null || Slot >= ItemGrid.Length) return null;

                return ItemGrid[Slot];
            }
            set
            {
                if (ItemGrid[Slot] == value  || Linked || Slot >= ItemGrid.Length) return;

                ClientUserItem oldValue = ItemGrid[Slot];
                ItemGrid[Slot] = value;

                OnItemChanged(oldValue, value);
            }
        }
        public event EventHandler<EventArgs> ItemChanged;
        public void OnItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();
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
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();
        }

        #endregion

        #region Locked

        public bool Locked
        {
            get => _Locked;
            set
            {
                if (_Locked == value) return;

                bool oldValue = _Locked;
                _Locked = value;

                OnLockedChanged(oldValue, value);
            }
        }
        private bool _Locked;
        public event EventHandler<EventArgs> LockedChanged;
        public void OnLockedChanged(bool oValue, bool nValue)
        {
            LockedChanged?.Invoke(this, EventArgs.Empty);
            
            UpdateBorder();
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
            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

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
            SelectedChanged?.Invoke(this, EventArgs.Empty);
            
            UpdateBorder();
        }

        #endregion

        #region Slot

        public int Slot
        {
            get => _Slot;
            set
            {
                if (_Slot == value) return;

                int oldValue = _Slot;
                _Slot = value;

                OnSlotChanged(oldValue, value);
            }
        }
        private int _Slot;
        public event EventHandler<EventArgs> SlotChanged;
        public void OnSlotChanged(int oValue, int nValue)
        {
            SlotChanged?.Invoke(this, EventArgs.Empty);
            ItemChanged?.Invoke(this, EventArgs.Empty);
            RefreshItem();
        }

        #endregion

        #region ShowCountLabel

        public bool ShowCountLabel
        {
            get => _ShowCountLabel;
            set
            {
                if (_ShowCountLabel == value) return;

                bool oldValue = _ShowCountLabel;
                _ShowCountLabel = value;

                OnShowCountLabelChanged(oldValue, value);
            }
        }
        private bool _ShowCountLabel;
        public event EventHandler<EventArgs> ShowCountLabelChanged;
        public void OnShowCountLabelChanged(bool oValue, bool nValue)
        {
            ShowCountLabelChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public ClientUserItem QuickInfoItem { get; private set; }

        #region QuickInfo

        public ItemInfo QuickInfo
        {
            get => _QuickInfo;
            set
            {
                if (_QuickInfo == value) return;

                ItemInfo oldValue = _QuickInfo;
                _QuickInfo = value;

                OnLinkedInfoChanged(oldValue, value);
            }
        }
        private ItemInfo _QuickInfo;
        public event EventHandler<EventArgs> LinkedInfoChanged;
        public void OnLinkedInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            if (nValue != null)
            {
                QuickInfoItem = new ClientUserItem(nValue, 1);
                QuickItem = null;
                if (GridType == GridType.Belt)
                    GameScene.Game.BeltBox.Links[Slot].LinkInfoIndex = nValue.Index;
            }
            else
            {
                QuickInfoItem = null;
                if (GridType == GridType.Belt)
                    GameScene.Game.BeltBox.Links[Slot].LinkInfoIndex = -1;
            }
            

            RefreshItem();
            LinkedInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region QuickItem

        public ClientUserItem QuickItem
        {
            get => _QuickItem;
            set
            {
                if (_QuickItem == value) return;

                ClientUserItem oldValue = _QuickItem;
                _QuickItem = value;

                OnLinkedItemChanged(oldValue, value);
            }
        }
        private ClientUserItem _QuickItem;
        public event EventHandler<EventArgs> LinkedItemChanged;
        public void OnLinkedItemChanged(ClientUserItem oValue, ClientUserItem nValue)
        {

            if (nValue != null)
            {
                QuickInfo = null;
                GameScene.Game.BeltBox.Links[Slot].LinkItemIndex = nValue.Index;
            }
            else
            {
                GameScene.Game.BeltBox.Links[Slot].LinkItemIndex = -1;
            }
           
            RefreshItem();
            LinkedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Link

        public DXItemCell Link
        {
            get => _Link;
            set
            {
                if (_Link == value) return;

                DXItemCell oldValue = _Link;
                _Link = value;

                OnLinkChanged(oldValue, value);
            }
        }
        private DXItemCell _Link;
        public event EventHandler<EventArgs> LinkChanged;
        public void OnLinkChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue?.Link == this) oValue.Link = null;

            if (nValue != null && nValue.Link != this) nValue.Link = this;

            RefreshItem();

            UpdateBorder();

            LinkChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        #region LinkedCount

        public long LinkedCount
        {
            get => _LinkedCount;
            set
            {
                if (_LinkedCount == value) return;

                long oldValue = _LinkedCount;
                _LinkedCount = value;

                OnLinkedCountChanged(oldValue, value);
            }
        }
        private long _LinkedCount;
        public event EventHandler<EventArgs> LinkedCountChanged;
        public void OnLinkedCountChanged(long oValue, long nValue)
        {
            LinkedCountChanged?.Invoke(this, EventArgs.Empty);
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
            AllowLinkChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        public DXLabel CountLabel;
        public override void OnMouseWheel(MouseEventArgs e)
        {
            HandleMouseWheel(e);
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (HostGrid == null && !IsVisible)
                Link = null;
        }
        public override void OnBorderChanged(bool oValue, bool nValue)
        {
            base.OnBorderChanged(oValue, nValue);
            
            TextureValid = false;

            UpdateBorder();
        }
        public override void OnBorderColourChanged(Color oValue, Color nValue)
        {
            base.OnBorderColourChanged(oValue, nValue);

            TextureValid = false;

            UpdateBorder();
        }
        public override void OnEnabledChanged(bool oValue, bool nValue)
        {
            base.OnEnabledChanged(oValue, nValue);

            UpdateBorder();
        }
        #endregion

        #region Hidden

        public bool Hidden
        {
            get => _Hidden;
            set
            {
                if (_Hidden == value) return;

                bool oldValue = _Hidden;
                _Hidden = value;

                OnHiddenChanged(oldValue, value);
            }
        }
        private bool _Hidden;
        public event EventHandler<EventArgs> HiddenChanged;
        public void OnHiddenChanged(bool oValue, bool nValue)
        {
            HiddenChanged?.Invoke(this, EventArgs.Empty);

            UpdateBorder();
        }

        #endregion

        public DXItemCell()
        {
            BackColour = Color.Empty;
            DrawTexture = true;
            ShowCountLabel = true;
            AllowLink = true;


            BorderColour = Color.FromArgb(99, 83, 50);
            Size = new Size(CellWidth, CellHeight);

            CountLabel = new DXLabel
            {
                ForeColour = Color.Yellow,
                IsControl = false,
                Parent = this,
            };
            CountLabel.SizeChanged += CountLabel_SizeChanged;
        }

        #region Methods
        private void CountLabel_SizeChanged(object sender, EventArgs e)
        {
            CountLabel.Location = new Point(Size.Width - CountLabel.Size.Width, Size.Height - CountLabel.Size.Height);
        }

        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (!Border || BorderInformation == null) return;

            DXManager.Line.Draw(BorderInformation, BorderColour);
        }
        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || Size.Width == 0 || Size.Height == 0) return;

            BorderInformation = new[]
            {
                new Vector2(0, 0),
                new Vector2(Size.Width - 1, 0 ),
                new Vector2(Size.Width - 1, Size.Height - 1),
                new Vector2(0 , Size.Height - 1),
                new Vector2(0 , 0 )
            };
            TextureValid = false;
        }
        protected override void DrawBorder()
        {
        }
        protected override void DrawControl()
        {
            MirLibrary Library;

            CEnvir.LibraryList.TryGetValue(LibraryFile.StoreItems, out Library);

            if (Library != null && Item != null)
            {
                int drawIndex;

                if (CEnvir.IsCurrencyItem(Item.Info))
                {
                    drawIndex = CEnvir.CurrencyImage(Item.Info, Item.Count);
                }
                else
                {
                    ItemInfo info = Item.Info;

                    if (info.ItemEffect == ItemEffect.ItemPart && Item.AddedStats[Stat.ItemIndex] > 0)
                        info = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

                    drawIndex = info.Image;
                }

                if (!Hidden)
                {
                    MirImage image = Library.CreateImage(drawIndex, ImageType.Image);
                    if (image != null)
                    {
                        Rectangle area = new Rectangle(DisplayArea.X, DisplayArea.Y, image.Width, image.Height);
                        area.Offset((Size.Width - image.Width) / 2, (Size.Height - image.Height) / 2);
                        ItemInfo info = Item.Info;
                        if (info.ItemEffect == ItemEffect.ItemPart && Item.AddedStats[Stat.ItemIndex] > 0)
                        {
                            info = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);
                            PresentTexture(image.Image, this, area, Item.Count >= info.PartCount ? Color.White : Color.Gray, this);
                        }
                        else
                            PresentTexture(image.Image, this, area, Item.Count > 0 ? Color.White : Color.Gray, this);
                    }
                }
            }

            if (InterfaceLibrary != null)
            {
                MirImage image = InterfaceLibrary.CreateImage(47, ImageType.Image);

                if (Item != null && Item.New && image != null)
                    PresentTexture(image.Image, this, new Rectangle(DisplayArea.X + 1, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this);

                image = InterfaceLibrary.CreateImage(48, ImageType.Image);
                if (Item != null && (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked && image != null && !Hidden && GridType != GridType.Inspect)
                    PresentTexture(image.Image, this, new Rectangle(DisplayArea.X + 1, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this);


                image = InterfaceLibrary.CreateImage(49, ImageType.Image);
                if (Item != null && GameScene.Game != null && !GameScene.Game.CanUseItem(Item) && image != null && !Hidden && GridType != GridType.Inspect)
                    PresentTexture(image.Image, this, new Rectangle(DisplayArea.Right - 12, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this);

                image = InterfaceLibrary.CreateImage(103, ImageType.Image);
                if (Item != null && GameScene.Game != null && image != null && Item.Info.ItemEffect == ItemEffect.ItemPart)
                    PresentTexture(image.Image, this, new Rectangle(DisplayArea.Right - 16, DisplayArea.Y + 1, image.Width, image.Height), Item.Count > 0 ? Color.White : Color.Gray, this);
            }

            base.DrawControl();
        }

        public void UpdateBorder()
        {
            BackColour = Color.Empty;

            if (Hidden)
            {
                BackColour = Color.Empty;
                Border = false;
                return;
            }

            if (!Enabled)
                BackColour = Color.FromArgb(125,0,125,125);
            else if (Locked || Selected || (!Linked && Link != null))
                BackColour = Color.FromArgb(125, 255, 125, 125);

            DrawTexture = MouseControl == this || !Enabled || Locked || Selected || FixedBorder || (!Linked && Link != null);
            
            if (MouseControl == this || Locked || Selected || (!Linked && Link != null))
            {
                if (!FixedBorderColour)
                    BorderColour = Color.Lime;
                Border = true;
            }
            else
            {
                if (!FixedBorderColour)
                   BorderColour = Color.FromArgb(99, 83, 50);
                Border = FixedBorder;
            }
        }
        public void RefreshItem()
        {
            if ((GridType == GridType.Inventory || GridType == GridType.CompanionInventory) && GameScene.Game.BeltBox?.Grid != null)
                foreach (DXItemCell cell in GameScene.Game.BeltBox.Grid.Grid)
                    cell.RefreshItem();

            if ((GridType == GridType.Inventory || GridType == GridType.CompanionInventory) && GameScene.Game.AutoPotionBox?.Rows != null)
                foreach (AutoPotionRow row in GameScene.Game.AutoPotionBox.Rows)
                    row.ItemCell.RefreshItem();

            if ((GridType == GridType.Belt || GridType == GridType.AutoPotion) && QuickInfo != null)
                QuickInfoItem.Count = GameScene.Game.Inventory.Where(x => x?.Info == QuickInfo).Sum(x => x.Count) +( GameScene.Game.Companion?.InventoryArray.Where(x => x?.Info == QuickInfo).Sum(x => x.Count) ?? 0);
            
            if (MouseControl == this)
            {
                GameScene.Game.MouseItem = null;
                GameScene.Game.MouseItem = Item;
            }

            CountLabel.Visible = ShowCountLabel && !Hidden && Item != null && (!CEnvir.IsCurrencyItem(Item.Info) && Item.Info.ItemEffect != ItemEffect.Experience) && (Item.Info.StackSize > 1 || Item.Count > 1);
            CountLabel.Text = Linked ? LinkedCount.ToString() : Item?.Count.ToString();
        }
        public void MoveItem()
        {
            if (SelectedCell == null)
            {
                if (Item == null) return;

                if (Linked && Link != null)
                {
                    Link = null;
                    return;
                }

                SelectedCell = this;
                return;
            }

            //Put Item back where it came from
            if (SelectedCell == this || SelectedCell.Item == null)
            {
                SelectedCell = null;
                return;
            }

            switch (SelectedCell.GridType) //FROM Grid
            {
                case GridType.Equipment:
                    //If !CanRemoveItem return;

                    if (GridType == GridType.Equipment)
                    {
                        //Don't want to move items around the character body (no point)
                        return;
                    }

                    if (GameScene.Game.MapControl.FishingState != FishingState.None) return;

                    if (Item == null || (SelectedCell.Item.Info == Item.Info && SelectedCell.Item.Count < SelectedCell.Item.Info.StackSize))
                        SelectedCell.MoveItem(this);
                    else
                        SelectedCell.MoveItem(HostGrid);

                    SelectedCell = null;
                    return;
                case GridType.CompanionEquipment:
                    //If !CanRemoveItem return;

                    if (GridType == GridType.CompanionEquipment)
                    {
                        //Don't want to move items around the character body (no point)
                        return;
                    }
                             
                    if (Item == null || (SelectedCell.Item.Info == Item.Info && SelectedCell.Item.Count < SelectedCell.Item.Info.StackSize))
                        SelectedCell.MoveItem(this);
                    else
                        SelectedCell.MoveItem(HostGrid);

                    SelectedCell = null;
                    return;
            }

            switch (GridType) //To Grid
            {
                case GridType.Storage:
                    if (SelectedCell.Item.Info.ItemEffect == ItemEffect.ItemPart) return;
                    break;
                case GridType.PartsStorage:
                    if (SelectedCell.Item.Info.ItemEffect != ItemEffect.ItemPart) return;
                    break;
                case GridType.Equipment:
                    if (!Functions.CorrectSlot(SelectedCell.Item.Info.ItemType, (EquipmentSlot)Slot) || SelectedCell.GridType == GridType.Belt) return;

                    ToEquipment(SelectedCell);
                    return;
                case GridType.CompanionEquipment:
                    if (!Functions.CorrectSlot(SelectedCell.Item.Info.ItemType, (CompanionSlot)Slot) || SelectedCell.GridType == GridType.Belt) return;

                    ToCompanionEquipment(SelectedCell);
                    return;
            }

            //Gem sections, Refine box?

            SelectedCell.MoveItem(this);
        }
        public void ToEquipment(DXItemCell fromCell)
        {
            if (Locked || ReadOnly) return;

            if (Item != null && (Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (GameScene.Game.MapControl.FishingState != FishingState.None) return;

            if (fromCell == SelectedCell) SelectedCell = null;

            if (!GameScene.Game.CanWearItem(fromCell.Item, (EquipmentSlot) Slot)) return;

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = fromCell.GridType,
                ToGrid = GridType,
                FromSlot = fromCell.Slot,
                ToSlot = Slot
            };

            if (Item != null && Item.Info == fromCell.Item.Info && Item.Count < Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (fromCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (fromCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (fromCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (fromCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(fromCell.Item.AddedStats) &&
                Item.ExpireTime == fromCell.Item.ExpireTime)
                packet.MergeItem = true;

            Locked = true;
            fromCell.Locked = true;
            CEnvir.Enqueue(packet);
        }
        public void ToCompanionEquipment(DXItemCell fromCell)
        {
            if (Locked || ReadOnly) return;

            if (fromCell == SelectedCell) SelectedCell = null;

            if (!GameScene.Game.CanCompanionWearItem(fromCell.Item, (CompanionSlot)Slot)) return;

            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = fromCell.GridType,
                ToGrid = GridType,
                FromSlot = fromCell.Slot,
                ToSlot = Slot
            };

            if (Item != null && Item.Info == fromCell.Item.Info && Item.Count < Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (fromCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (fromCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (fromCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (fromCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(fromCell.Item.AddedStats) &&
                Item.ExpireTime == fromCell.Item.ExpireTime)
                packet.MergeItem = true;

            Locked = true;
            fromCell.Locked = true;
            CEnvir.Enqueue(packet);
        }
        public void MoveItem(DXItemCell toCell)
        {
            ClientBeltLink link;

            #region Belt

            if (toCell.GridType == GridType.Belt)
            {
                ItemInfo info = null;
                ClientUserItem item = null;

                if (GridType == toCell.GridType)
                {
                    info = toCell.QuickInfo;
                    item = toCell.QuickItem;
                }

                if (Item.Info.ShouldLinkInfo)
                    toCell.QuickInfo = Item.Info;
                else
                    toCell.QuickItem = Item;

                if (GridType == toCell.GridType)
                {
                    QuickInfo = info;
                    QuickItem = item;

                    link = GameScene.Game.BeltBox.Links[Slot];
                    CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });
                }

                if (Selected) SelectedCell = null;


                link = GameScene.Game.BeltBox.Links[toCell.Slot];
                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });


                return;
            }

            #endregion

            #region Auto Potion

            if (toCell.GridType == GridType.AutoPotion)
            {
                
                if (GridType == toCell.GridType) return;
                if (!Item.Info.CanAutoPot) return;

                if (Selected) SelectedCell = null;

                toCell.QuickInfo = Item.Info;
                
                GameScene.Game.AutoPotionBox.Rows[toCell.Slot].SendUpdate();
                return;
            }

            #endregion

            if (GridType == GridType.Belt)
            {
                QuickInfo = null;
                QuickItem = null;

                link = GameScene.Game.BeltBox.Links[Slot];
                CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex });

                if (Selected) SelectedCell = null;
                return;
            }

            if (GridType == GridType.AutoPotion)
            {
                QuickInfo = null;

                GameScene.Game.AutoPotionBox.Rows[toCell.Slot].SendUpdate();
                if (Selected) SelectedCell = null;
                return;
            }

            if (GridType == GridType.PartsStorage && toCell.Item != null && toCell.Item.Info.ItemEffect != ItemEffect.ItemPart) return;

            if (GridType == GridType.Storage && toCell.Item != null && toCell.Item.Info.ItemEffect == ItemEffect.ItemPart) return;

            if (toCell.Linked)
            {
                if (!CheckLink(toCell.HostGrid)) return;

                if (Selected) SelectedCell = null;

                if (Item?.Count > 1)
                {
                    DXItemAmountWindow window = new DXItemAmountWindow("Amount", Item);

                    window.ConfirmButton.MouseClick += (o, e) =>
                    {
                        toCell.LinkedCount = window.Amount;
                        toCell.Link = this;
                    };
                    
                    return;
                }

                toCell.LinkedCount = 1;
                toCell.Link = this;
                return;

            }
            
            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = GridType,
                ToGrid = toCell.GridType,
                FromSlot = Slot,
                ToSlot = toCell.Slot
            };

            if (toCell.Item != null && toCell.Item.Info == Item.Info && toCell.Item.Count < toCell.Item.Info.StackSize &&
                (Item.Flags & UserItemFlags.Bound) == (toCell.Item.Flags & UserItemFlags.Bound) &&
                (Item.Flags & UserItemFlags.Worthless) == (toCell.Item.Flags & UserItemFlags.Worthless) &&
                (Item.Flags & UserItemFlags.NonRefinable) == (toCell.Item.Flags & UserItemFlags.NonRefinable) &&
                (Item.Flags & UserItemFlags.Expirable) == (toCell.Item.Flags & UserItemFlags.Expirable) &&
                Item.AddedStats.Compare(toCell.Item.AddedStats) &&
                Item.ExpireTime == toCell.Item.ExpireTime)
                packet.MergeItem = true;

            if (Selected) SelectedCell = null;

            Locked = true;
            toCell.Locked = true;
            CEnvir.Enqueue(packet);
        }

        public bool SellMode
        {
            get { return (GameScene.Game.InventoryBox.InvMode == InventoryMode.Sell && GridType == GridType.Inventory); }
        }

        public bool MoveItem(DXItemGrid toGrid, bool skipCount = false)
        {
            if (toGrid.GridType == GridType.Belt || toGrid.GridType == GridType.AutoPotion) return false;
          
            C.ItemMove packet = new C.ItemMove
            {
                FromGrid = GridType,
                FromSlot = Slot,
            };

            DXItemCell toCell = null;
            foreach (DXItemCell cell in toGrid.Grid)
            {
                if (cell.Locked || !cell.Enabled) continue;

                ClientUserItem toItem = cell.Item;

                if (toItem == null)
                {
                    if (cell.Linked)
                    {
                        if (!CheckLink(toGrid)) return false;

                        if (Selected) SelectedCell = null;

                        switch (toGrid.GridType)
                        {
                            case GridType.RefineSpecial:
                            case GridType.RefinementStoneCrystal:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.MasterRefineFragment1:
                            case GridType.MasterRefineFragment2:
                                cell.LinkedCount = 10;
                                break;
                            case GridType.MasterRefineSpecial:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.MasterRefineStone:
                                cell.LinkedCount = 1;
                                break;
                            default:
                                if (Item.Count > 1 && !skipCount)
                                {
                                    DXItemAmountWindow window = new DXItemAmountWindow("Amount", Item);

                                    window.ConfirmButton.MouseClick += (o, e) =>
                                    {
                                        cell.LinkedCount = window.Amount;
                                        cell.Link = this;
                                    };

                                    return true;
                                }

                                cell.LinkedCount = Item.Count;
                                break;
                            case GridType.WeaponCraftTemplate:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftYellow:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftBlue:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftRed:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftPurple:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftGreen:
                                cell.LinkedCount = 1;
                                break;
                            case GridType.WeaponCraftGrey:
                                cell.LinkedCount = 1;
                                break;
                        }

                        cell.Link = this;
                        return true;
                    }

                    if (toCell == null) toCell = cell;
                    continue;
                }

                if (cell.Linked || toItem.Info != Item.Info || toItem.Count >= toItem.Info.StackSize) continue;
                if ((Item.Flags & UserItemFlags.Bound) != (toItem.Flags & UserItemFlags.Bound)) continue;
                if ((Item.Flags & UserItemFlags.Worthless) != (toItem.Flags & UserItemFlags.Worthless)) continue;
                if ((Item.Flags & UserItemFlags.NonRefinable) != (toItem.Flags & UserItemFlags.NonRefinable)) continue;
                if ((Item.Flags & UserItemFlags.Expirable) != (toItem.Flags & UserItemFlags.Expirable)) continue;
                if (!Item.AddedStats.Compare(toItem.AddedStats)) continue;
                if (Item.ExpireTime != toItem.ExpireTime) continue;


                toCell = cell;
                packet.MergeItem = true;
                break;
            }

            if (toCell == null) return false;

            if (toCell.Selected) SelectedCell = null;

            packet.ToSlot = toCell.Slot;
            packet.ToGrid = toCell.GridType;

            Locked = true;
            toCell.Locked = true;
            CEnvir.Enqueue(packet);

            return true;
        }

        public bool CheckLink(DXItemGrid grid)
        {
            if (!AllowLink || Item == null || (!Linked && Link != null) || grid == null) return false;

            switch (grid.GridType)
            {
                case GridType.Repair:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GameScene.Game.NPCBox.Page.Types.All(x => x.ItemType != Item.Info.ItemType) || !Item.Info.CanRepair || Item.CurrentDurability >= Item.MaxDurability || (GameScene.Game.NPCRepairBox.SpecialCheckBox.Checked && CEnvir.Now < Item.NextSpecialRepair))
                        return false;
                    break;

                case GridType.PartsStorage:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (!MapObject.User.InSafeZone) return false;
                    if (GridType != GridType.Inventory) return false;
                    if (Item.Info.ItemEffect != ItemEffect.ItemPart) return false;
                    break;

                case GridType.Storage:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (!MapObject.User.InSafeZone) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Equipment) return false;
                    if (!Item.Info.CanStore) return false;
                    if (Item.Info.ItemEffect == ItemEffect.ItemPart) return false;
                    break;

                case GridType.RefinementStoneIronOre:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.IronOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneSilverOre:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.SilverOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneDiamond:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Diamond || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneGoldOre:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.GoldOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefinementStoneCrystal:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Crystal || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefineBlackIronOre:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.BlackIronOre || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;

                case GridType.RefineAccessory:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }
                    break;
                case GridType.RefineSpecial:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.RefineSpecial:
                            if (Item.Info.Shape != 1) return false; //weapon refine ?
                            break;
                        default:
                            return false;
                    }
                    break;
                case GridType.ItemFragment:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if ((GridType != GridType.Inventory && GridType != GridType.CompanionInventory) || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || !Item.CanFragment())
                        return false;
                    break;
                case GridType.Consign:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.PartsStorage) return false;
                    if (GridType == GridType.Inventory && !MapObject.User.InSafeZone) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    break;
                case GridType.SendMail:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.PartsStorage) return false;
                    if (GridType == GridType.Inventory && !MapObject.User.InSafeZone) return false;
                    break;
                case GridType.TradeUser:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.PartsStorage && GridType != GridType.Equipment) return false;

                    break;
                case GridType.GuildStorage:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (!Item.Info.CanTrade) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return false;
                    if (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.PartsStorage && GridType != GridType.Equipment) return false;

                    break;
                case GridType.WeddingRing:
                    if (GridType != GridType.Inventory) return false;
                    if (Item.Info.ItemType != ItemType.Ring) return false;

                    if (!(GameScene.Game.CanWearItem(Item, EquipmentSlot.RingL) || GameScene.Game.CanWearItem(Item, EquipmentSlot.RingR))) return false;

                    break;
                case GridType.AccessoryRefineUpgradeTarget:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }
                    if ((Item.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return false;
                    break;
                case GridType.AccessoryRefineLevelTarget:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }

                    if ((Item.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return false;

                    if (Item.Level >= Globals.AccessoryExperienceList.Count) return false;

                    break;
                case GridType.AccessoryRefineLevelItems:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }

                    if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link?.Item?.Info != Item.Info) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link.Item.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) return false;

                    break;
                case GridType.AccessoryReset:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }

                    if (Item.Level >= Globals.AccessoryExperienceList.Count) return false;
                    break;
                case GridType.MasterRefineFragment1:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Fragment1 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
                case GridType.MasterRefineFragment2:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Fragment2 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
                case GridType.MasterRefineFragment3:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Fragment3 || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
                case GridType.MasterRefineStone:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.RefinementStone || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
                case GridType.MasterRefineSpecial:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.RefineSpecial:
                            if (Item.Info.Shape != 5) return false; //weapon refine ?
                            break;
                        default:
                            return false;
                    }
                    break;
                case GridType.WeaponCraftTemplate:
                    if (Item.Info.ItemType != ItemType.Weapon && Item.Info.ItemEffect != ItemEffect.WeaponTemplate) return false;
                    break;
                case GridType.WeaponCraftBlue:
                    if (Item.Info.ItemEffect != ItemEffect.BlueSlot) return false;
                    break;
                case GridType.WeaponCraftGreen:
                    if (Item.Info.ItemEffect != ItemEffect.GreenSlot) return false;
                    break;
                case GridType.WeaponCraftGrey:
                    if (Item.Info.ItemEffect != ItemEffect.GreySlot) return false;
                    break;
                case GridType.WeaponCraftPurple:
                    if (Item.Info.ItemEffect != ItemEffect.PurpleSlot) return false;
                    break;
                case GridType.WeaponCraftRed:
                    if (Item.Info.ItemEffect != ItemEffect.RedSlot) return false;
                    break;
                case GridType.WeaponCraftYellow:
                    if (Item.Info.ItemEffect != ItemEffect.YellowSlot) return false;
                    break;
                case GridType.RefineCorundumOre:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if (Item.Info.ItemEffect != ItemEffect.Corundum || (Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;
                    break;
                case GridType.AccessoryRefineCombTarget:
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.Equipment && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }

                    if ((Item.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return false;

                    if (Item.Level > 1) return false;

                    break;
                case GridType.AccessoryRefineCombItems:
                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                    if ((Item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return false;

                    if ((Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return false;
                    if (Item.Level > 1) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.CompanionInventory && GridType != GridType.Storage) return false;

                    switch (Item.Info.ItemType)
                    {
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            break;
                        default:
                            return false;
                    }

                    if (GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link?.Item?.Info != Item.Info) return false;
                    if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link.Item.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) return false;
                    if (GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link?.Item?.AddedStats.Count != Item.AddedStats.Count) return false; //if material has different amount of added stats to target dont refine

                    if (Item.AddedStats.Count >= 1) //if target has added stats loop through to check material has same stats
                    {
                        if (!Item.AddedStats.Compare(GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link?.Item?.AddedStats)) return false;
                    }
                    break;
            }

            return true;
        }

        public bool UseItem()
        {
            if (Item == null || Locked || ReadOnly || SelectedCell == this || (!Linked && Link != null) || !GameScene.Game.CanUseItem(Item) || GameScene.Game.Observer || GameScene.Game.MapControl.FishingState != FishingState.None) return false;

            if (GridType == GridType.Belt || GridType == GridType.Belt)
            {
                DXItemCell cell;

                if (QuickInfo != null)
                {
                    cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item?.Info == QuickInfo) ??
                           GameScene.Game.CompanionBox.InventoryGrid?.Grid.FirstOrDefault(x => x?.Item?.Info == QuickInfo);
                }
                else
                    cell = GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(x => x?.Item == QuickItem);

                return cell?.UseItem() == true;
            }

            switch (Item.Info.ItemType)
            {
                case ItemType.Weapon:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Weapon].ToEquipment(this);
                    break;
                case ItemType.Armour:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].ToEquipment(this);
                    break;
                case ItemType.Torch:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Torch].ToEquipment(this);
                    break;
                case ItemType.Helmet:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Helmet].ToEquipment(this);
                    break;
                case ItemType.Necklace:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Necklace].ToEquipment(this);
                    break;
                case ItemType.Bracelet:
                    if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletL].Item == null)
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletL].ToEquipment(this);
                    else
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.BraceletR].ToEquipment(this);
                    break;
                case ItemType.Ring:
                    if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].Item == null)
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingL].ToEquipment(this);
                    else
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.RingR].ToEquipment(this);
                    break;
                case ItemType.Shoes:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Shoes].ToEquipment(this);
                    break;
                case ItemType.Poison:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].ToEquipment(this);
                    break;
                case ItemType.Amulet:
                case ItemType.DarkStone:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Amulet].ToEquipment(this);
                    break;
                case ItemType.Flower:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Flower].ToEquipment(this);
                    break;
                case ItemType.Emblem:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Emblem].ToEquipment(this);
                    break;
                case ItemType.Shield:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Shield].ToEquipment(this);
                    break;
                case ItemType.Costume:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Costume].ToEquipment(this);
                    break;
                case ItemType.HorseArmour:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.HorseArmour].ToEquipment(this);
                    break;
                case ItemType.Hook:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Hook].ToEquipment(this);
                    break;
                case ItemType.Float:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Float].ToEquipment(this);
                    break;
                case ItemType.Bait:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Bait].ToEquipment(this);
                    break;
                case ItemType.Finder:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Finder].ToEquipment(this);
                    break;
                case ItemType.Reel:
                    GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Reel].ToEquipment(this);
                    break;
                case ItemType.CompanionBag:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Bag].ToEquipment(this);
                    break;
                case ItemType.CompanionHead:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Head].ToEquipment(this);
                    break;
                case ItemType.CompanionBack:
                    if (GameScene.Game.Companion != null)
                        GameScene.Game.CompanionBox.EquipmentGrid[(int)CompanionSlot.Back].ToEquipment(this);
                    break;
                case ItemType.Consumable:
                case ItemType.Scroll:
                case ItemType.CompanionFood:
                case ItemType.ItemPart:
                    if (!GameScene.Game.CanUseItem(Item)) return false;

                    if (GridType != GridType.Inventory && GridType != GridType.PartsStorage && GridType != GridType.CompanionEquipment && GridType != GridType.CompanionInventory) return false;
                        
                    if ((Item.Info.Shape == 19 || Item.Info.Shape == 20 || Item.Info.Shape == 21 || Item.Info.Shape == 22) && MapObject.User.Horse != HorseType.None) return false;

                    if ((CEnvir.Now < GameScene.Game.UseItemTime && Item.Info.ItemEffect != ItemEffect.ElixirOfPurification)) return false;

                    GameScene.Game.UseItemTime = CEnvir.Now.AddMilliseconds(Math.Max(250, Item.Info.Durability));
                    

                    Locked = true;

                    CEnvir.Enqueue(new C.ItemUse { Link = new CellLinkInfo { GridType = GridType, Slot = Slot, Count = 1 } });
                    PlayItemSound();
                    break;
                case ItemType.Book:
                    if (!GameScene.Game.CanUseItem(Item) || GridType != GridType.Inventory) return false;

                    if (CEnvir.Now < GameScene.Game.UseItemTime || MapObject.User.Horse != HorseType.None) return false;

                    
                    GameScene.Game.UseItemTime = CEnvir.Now.AddMilliseconds(250);
                    Locked = true;

                    CEnvir.Enqueue(new C.ItemUse { Link = new CellLinkInfo { GridType = GridType, Slot = Slot, Count = 1 } });
                    PlayItemSound();
                    break;
                case ItemType.System:
                    if (!GameScene.Game.CanUseItem(Item) || GridType != GridType.Inventory) return false;

                    switch (Item.Info.ItemEffect)
                    {
                        case ItemEffect.GenderChange:
                            if (GameScene.Game.CharacterBox.Grid[(int) EquipmentSlot.Armour].Item != null)
                            {
                                GameScene.Game.ReceiveChat(CEnvir.Language.CannotChangeGenderWhileWearingArmour, MessageType.System);
                                return false;
                            }

                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.GenderChange;
                            break;
                        case ItemEffect.HairChange:
                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.HairChange;

                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item != null)
                                GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;
                            break;
                        case ItemEffect.ArmourDye:
                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item == null)
                            {
                                GameScene.Game.ReceiveChat(CEnvir.Language.WearingArmourBeforeDye, MessageType.System);
                                return false;
                            }

                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;

                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.ArmourDye;

                            GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;
                            break;
                        case ItemEffect.NameChange:
                            GameScene.Game.EditCharacterBox.Visible = true;

                            GameScene.Game.EditCharacterBox.SelectedClass = GameScene.Game.User.Class;
                            GameScene.Game.EditCharacterBox.SelectedGender = GameScene.Game.User.Gender;
                            GameScene.Game.EditCharacterBox.HairColour.BackColour = GameScene.Game.User.HairColour;
                            GameScene.Game.EditCharacterBox.HairNumberBox.Value = GameScene.Game.User.HairType;

                            if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item != null)
                                GameScene.Game.EditCharacterBox.ArmourColour.BackColour = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Armour].Item.Colour;

                            GameScene.Game.EditCharacterBox.Change = ChangeType.NameChange;


                            GameScene.Game.EditCharacterBox.CharacterNameTextBox.TextBox.Text = GameScene.Game.User.Name;
                            break;
                        case ItemEffect.FortuneChecker:
                            GameScene.Game.FortuneCheckerBox.Visible = true;
                            break;
                        case ItemEffect.Caption:
                            GameScene.Game.CaptionBox.Visible = true;
                            break;
                    }

                    break;
            }

            return true;
        }

        private void PlayItemSound()
        {
            if (Item == null) return;

            switch (Item.Info.ItemType)
            {
                case ItemType.Weapon:
                    DXSoundManager.Play(SoundIndex.ItemWeapon);
                    break;
                case ItemType.Armour:
                    DXSoundManager.Play(SoundIndex.ItemArmour);
                    break;
                case ItemType.Helmet:
                    DXSoundManager.Play(SoundIndex.ItemHelmet);
                    break;
                case ItemType.Necklace:
                    DXSoundManager.Play(SoundIndex.ItemNecklace);
                    break;
                case ItemType.Bracelet:
                    DXSoundManager.Play(SoundIndex.ItemBracelet);
                    break;
                case ItemType.Ring:
                    DXSoundManager.Play(SoundIndex.ItemRing);
                    break;
                case ItemType.Shoes:
                    DXSoundManager.Play(SoundIndex.ItemShoes);
                    break;
                case ItemType.Consumable:
                    DXSoundManager.Play(Item.Info.Shape > 0 ? SoundIndex.ItemDefault : SoundIndex.ItemPotion);
                    break;
                default:
                    DXSoundManager.Play(SoundIndex.ItemDefault);
                    break;
            }
        }
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            GameScene.Game.MouseItem = Item;

            if (Item != null)
                Item.New = false;

            UpdateBorder();
        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            GameScene.Game.MouseItem = null;
            UpdateBorder();
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            //This needs to be here to stop chat box losing focus after you link an item
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (Locked || GameScene.Game.CurrencyPickedUp != null || (!Linked && Link != null) || GameScene.Game.Observer || GridType == GridType.Inspect) return;

            base.OnMouseClick(e);

            if (ReadOnly || !Enabled) return;

            if (Linked && Link != null)
            {
                Link = null;

                if (SelectedCell == null)
                    return;
            }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (CEnvir.Alt)
                    {
                        //Link Item
                        return;
                    }
                    if (CEnvir.Shift)
                    {
                        if (Item == null || (GridType != GridType.Inventory && GridType != GridType.Storage && GridType != GridType.PartsStorage && GridType != GridType.GuildStorage && GridType != GridType.CompanionInventory) || Item.Count <= 1) return;

                        DXItemAmountWindow window = new DXItemAmountWindow("Item Split", Item);

                        window.ConfirmButton.MouseClick += (o, e1) =>
                        {
                            Locked = true;
                            CEnvir.Enqueue(new C.ItemSplit { Grid = GridType, Slot = Slot, Count = window.Amount });
                        };

                        return;
                    }

                    if (Item != null && SelectedCell == null)
                        PlayItemSound();

                    MoveItem();
                    break;
                case MouseButtons.Middle:
                    if (Item != null)
                    {
                        if (CEnvir.Ctrl)
                        {
                            GameScene.Game.ChatTextBox.LinkItem(Item);
                            break;
                        }
                        CEnvir.Enqueue(new C.ItemLock { GridType = GridType, SlotIndex = Slot, Locked = (Item.Flags & UserItemFlags.Locked) != UserItemFlags.Locked });
                    }
                    break;
                case MouseButtons.Right:
                    switch (GridType)
                    {
                        case GridType.Belt:
                        case GridType.AutoPotion:
                            if (Item == null) return;

                            UseItem(); //Try Use Item
                            break;
                        case GridType.Inventory:
                            if (Item == null) return;

                            if (GameScene.Game.NPCRepairBox.IsVisible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.InventoryBox.IsVisible)
                            {
                                if (GameScene.Game.InventoryBox.InvMode == InventoryMode.Sell)
                                {
                                    if (!Item.Info.CanSell)
                                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToSellHereCannotSold, Item.Info.ItemName), MessageType.System);
                                    
                                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) 
                                        return;

                                    if ((GridType != GridType.Inventory/* && GridType != GridType.CompanionInventory*/) || (Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (Item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless || !Item.Info.CanSell)
                                        return;

                                    Selected = !Selected;

                                    return;
                                }
                            }

                            if (GameScene.Game.NPCMasterRefineBox.IsVisible)
                            {
                                switch (Item.Info.ItemEffect)
                                {
                                    case ItemEffect.Fragment1:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment2:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment3:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
                                            return;
                                        break;
                                    case ItemEffect.RefinementStone:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
                                            return;
                                        break;
                                }

                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.RefineSpecial:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
                                            return;
                                        break;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }
                            if (GameScene.Game.NPCRefinementStoneBox.IsVisible)
                            {
                                switch (Item.Info.ItemEffect)
                                {
                                    case ItemEffect.IronOre:
                                        MoveItem(GameScene.Game.NPCRefinementStoneBox.IronOreGrid);
                                        return;
                                    case ItemEffect.SilverOre:
                                        MoveItem(GameScene.Game.NPCRefinementStoneBox.SilverOreGrid);
                                        return;
                                    case ItemEffect.Diamond:
                                        MoveItem(GameScene.Game.NPCRefinementStoneBox.DiamondGrid);
                                        return;
                                    case ItemEffect.GoldOre:
                                        MoveItem(GameScene.Game.NPCRefinementStoneBox.GoldOreGrid);
                                        return;
                                    case ItemEffect.Crystal:
                                        MoveItem(GameScene.Game.NPCRefinementStoneBox.CrystalGrid);
                                        return;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }
                            if (GameScene.Game.NPCWeaponCraftBox.IsVisible)
                            {
                                switch (Item.Info.ItemEffect)
                                {
                                    case ItemEffect.WeaponTemplate:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.TemplateCell);
                                        return;
                                    case ItemEffect.YellowSlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.YellowCell);
                                        return;
                                    case ItemEffect.BlueSlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.BlueCell);
                                        return;
                                    case ItemEffect.RedSlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.RedCell);
                                        return;
                                    case ItemEffect.PurpleSlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.PurpleCell);
                                        return;
                                    case ItemEffect.GreenSlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.GreenCell);
                                        return;
                                    case ItemEffect.GreySlot:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.GreyCell);
                                        return;
                                }
                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.Weapon:
                                        MoveItem(GameScene.Game.NPCWeaponCraftBox.TemplateCell);
                                        return;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToCraft, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCItemFragmentBox.IsVisible)
                            {
                                if (!Item.CanFragment())
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToFragment, Item.Info.ItemName), MessageType.System);
                                else MoveItem(GameScene.Game.NPCItemFragmentBox.Grid);

                                return;
                            }

                            if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
                            {
                                if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
                                {
                                    if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
                                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToLevel, Item.Info.ItemName), MessageType.System);
                                }
                                else if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToUseToLevel, Item.Info.ItemName), MessageType.System);

                                return;
                            }

                            if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
                            {

                                if (!Item.CanAccessoryUpgrade())
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToUpgrade, Item.Info.ItemName), MessageType.System);
                                else
                                    MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

                                return;
                            }

                            if (GameScene.Game.NPCAccessoryResetBox.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.NPCAccessoryResetBox.AccessoryGrid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToReset, Item.Info.ItemName), MessageType.System);

                                return;
                            }

                            if (GameScene.Game.NPCRefineBox.IsVisible)
                            {
                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.Ore:
                                        if (Item.Info.ItemEffect != ItemEffect.BlackIronOre)
                                            GameScene.Game.ReceiveChat(CEnvir.Language.OnlyBlackIronOreCanBeUsed, MessageType.System);
                                        else
                                            MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
                                        return;
                                    case ItemType.Necklace:
                                    case ItemType.Bracelet:
                                    case ItemType.Ring:
                                        MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
                                        return;
                                    case ItemType.RefineSpecial:
                                        MoveItem(GameScene.Game.NPCRefineBox.SpecialGrid);
                                        return;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCAccessoryRefineBox.IsVisible)
                            {
                                if (Item.Level > 1)
                                {
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineHasBeenLevelled, Item.Info.ItemName), MessageType.System);
                                }
                                else
                                {
                                    if (Item.Info.ItemType == ItemType.Ore)
                                    {
                                        if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.OreTargetCell))
                                            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineNeedCorundumOre, Item.Info.ItemName), MessageType.System);
                                    }
                                    else
                                    {
                                        if (GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link == null)
                                        {

                                            if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.TargetCell))
                                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefine, Item.Info.ItemName), MessageType.System);
                                        }
                                        else if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.Grid))
                                            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.DoesntHaveSameStats, Item.Info.ItemName), MessageType.System);
                                    }
                                }
                                return;
                            }

                            if (GameScene.Game.MarketPlaceBox.ConsignTab.IsVisible)
                            {
                                MoveItem(GameScene.Game.MarketPlaceBox.ConsignGrid);
                                return;
                            }

                            if (GameScene.Game.CommunicationBox.IsVisible)
                            {
                                MoveItem(GameScene.Game.CommunicationBox.SendGrid);
                                return;
                            }

                            if (GameScene.Game.StorageBox.IsVisible)
                            {
                                if (Item.Info.ItemEffect == ItemEffect.ItemPart)
                                    MoveItem(GameScene.Game.StorageBox.PartGrid);
                                else if (!MoveItem(GameScene.Game.StorageBox.Grid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInStorage, MessageType.System);

                                return;
                            }

                            if (GameScene.Game.TradeBox.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.TradeBox.UserGrid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.UnableToTrade, MessageType.System);
                                return;
                            }

                            if (GameScene.Game.GuildBox.StorageTab.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.GuildBox.StorageGrid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.UnableToStoreInGuildStorage, MessageType.System);
                                return;
                            }

                            if (GameScene.Game.CompanionBox.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.CompanionBox.InventoryGrid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInCompanionInventory, MessageType.System);
                                return;
                            }

                            UseItem(); //Try Use Item
                            break;
                        case GridType.CompanionInventory:
                            if (Item == null) return;

                            if (GameScene.Game.NPCRepairBox.IsVisible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCRefineBox.IsVisible)
                            {
                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.Ore:
                                        if (Item.Info.ItemEffect != ItemEffect.BlackIronOre)
                                            GameScene.Game.ReceiveChat(CEnvir.Language.OnlyBlackIronOreCanBeUsed, MessageType.System);
                                        else
                                            MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
                                        return;
                                    case ItemType.Necklace:
                                    case ItemType.Bracelet:
                                    case ItemType.Ring:
                                        MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
                                        return;
                                    case ItemType.RefineSpecial:
                                        MoveItem(GameScene.Game.NPCRefineBox.SpecialGrid);
                                        return;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCMasterRefineBox.IsVisible)
                            {
                                switch (Item.Info.ItemEffect)
                                {
                                    case ItemEffect.Fragment1:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment2:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment3:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
                                            return;
                                        break;
                                    case ItemEffect.RefinementStone:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
                                            return;
                                        break;
                                }

                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.RefineSpecial:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
                                            return;
                                        break;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
                            {
                                if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
                                {
                                    if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
                                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToLevel, Item.Info.ItemName), MessageType.System);
                                }
                                else if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToUseToLevel, Item.Info.ItemName), MessageType.System);

                                return;
                            }

                            if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
                            {

                                if (!Item.CanAccessoryUpgrade())
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToUpgrade, Item.Info.ItemName), MessageType.System);
                                else
                                    MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

                                return;
                            }

                            if (GameScene.Game.NPCAccessoryResetBox.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.NPCAccessoryResetBox.AccessoryGrid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToReset, Item.Info.ItemName), MessageType.System);

                                return;
                            }

                            if (GameScene.Game.MarketPlaceBox.ConsignTab.IsVisible)
                            {
                                MoveItem(GameScene.Game.MarketPlaceBox.ConsignGrid);
                                return;
                            }

                            if (GameScene.Game.CommunicationBox.IsVisible)
                            {
                                MoveItem(GameScene.Game.CommunicationBox.SendGrid);
                                return;
                            }

                            if (GameScene.Game.StorageBox.IsVisible)
                            {
                                if (Item.Info.ItemEffect == ItemEffect.ItemPart)
                                    MoveItem(GameScene.Game.StorageBox.PartGrid);
                                else if (!MoveItem(GameScene.Game.StorageBox.Grid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInStorage, MessageType.System);
                            }

                            if (GameScene.Game.TradeBox.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.TradeBox.UserGrid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.UnableToTrade, MessageType.System);
                                return;
                            }

                            if (GameScene.Game.GuildBox.StorageTab.IsVisible)
                            {
                                if (!MoveItem(GameScene.Game.GuildBox.StorageGrid))
                                    GameScene.Game.ReceiveChat(CEnvir.Language.UnableToStoreInGuildStorage, MessageType.System);
                                return;
                            }

                            if (!MoveItem(GameScene.Game.InventoryBox.Grid))
                                GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInInventory, MessageType.System);

                            break;
                        case GridType.PartsStorage:
                            if (Item == null) return;

                            MoveItem(GameScene.Game.InventoryBox.Grid, true);
                            return;
                        case GridType.Storage:
                            if (Item == null) return;

                            if (GameScene.Game.NPCRepairBox.Visible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCMasterRefineBox.IsVisible)
                            {
                                switch (Item.Info.ItemEffect)
                                {
                                    case ItemEffect.Fragment1:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment1Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment2:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment2Grid))
                                            return;
                                        break;
                                    case ItemEffect.Fragment3:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.Fragment3Grid))
                                            return;
                                        break;
                                    case ItemEffect.RefinementStone:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.RefinementStoneGrid))
                                            return;
                                        break;
                                }

                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.RefineSpecial:
                                        if (MoveItem(GameScene.Game.NPCMasterRefineBox.SpecialGrid))
                                            return;
                                        break;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }
                            if (GameScene.Game.NPCRefineBox.Visible)
                            {
                                switch (Item.Info.ItemType)
                                {
                                    case ItemType.Ore:
                                        if (Item.Info.ItemEffect != ItemEffect.BlackIronOre)
                                            GameScene.Game.ReceiveChat(CEnvir.Language.OnlyBlackIronOreCanBeUsed, MessageType.System);
                                        else
                                            MoveItem(GameScene.Game.NPCRefineBox.BlackIronGrid);
                                        return;
                                    case ItemType.Necklace:
                                    case ItemType.Bracelet:
                                    case ItemType.Ring:
                                        MoveItem(GameScene.Game.NPCRefineBox.AccessoryGrid);
                                        return;
                                }
                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineIncorrectItem, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCAccessoryRefineBox.IsVisible)
                            {
                                if (Item.Level > 1)
                                {
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineHasBeenLevelled, Item.Info.ItemName), MessageType.System);
                                }
                                else
                                {
                                    if (Item.Info.ItemType == ItemType.Ore)
                                    {
                                        if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.OreTargetCell))
                                            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefineNeedCorundumOre, Item.Info.ItemName), MessageType.System);
                                    }
                                    else
                                    {
                                        if (GameScene.Game.NPCAccessoryRefineBox.TargetCell.Grid[0].Link == null)
                                        {

                                            if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.TargetCell))
                                                GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRefine, Item.Info.ItemName), MessageType.System);
                                        }
                                        else if (!MoveItem(GameScene.Game.NPCAccessoryRefineBox.Grid))
                                            GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.DoesntHaveSameStats, Item.Info.ItemName), MessageType.System);
                                    }
                                }
                                return;
                            }

                            if (GameScene.Game.MarketPlaceBox.ConsignTab.IsVisible)
                            {
                                MoveItem(GameScene.Game.MarketPlaceBox.ConsignGrid);
                                return;
                            }

                            MoveItem(GameScene.Game.InventoryBox.Grid, true);
                            return;
                        case GridType.GuildStorage:
                            if (Item == null) return;

                            if (GameScene.Game.NPCRepairBox.Visible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.MarketPlaceBox.ConsignTab.IsVisible)
                            {
                                MoveItem(GameScene.Game.MarketPlaceBox.ConsignGrid);
                                return;
                            }

                            MoveItem(GameScene.Game.InventoryBox.Grid, true);
                            return;
                        case GridType.Equipment:

                            if (Item == null) return;

                            if (GameScene.Game.MapControl.FishingState != FishingState.None) return;

                            if (GameScene.Game.NPCRepairBox.Visible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }

                            if (GameScene.Game.NPCAccessoryLevelBox.IsVisible)
                            {
                                if (GameScene.Game.NPCAccessoryLevelBox.TargetCell.Grid[0].Link == null)
                                {
                                    if (!MoveItem(GameScene.Game.NPCAccessoryLevelBox.TargetCell))
                                        GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToLevel, Item.Info.ItemName), MessageType.System);
                                }
                                return;
                            }

                            if (GameScene.Game.NPCAccessoryUpgradeBox.IsVisible)
                            {
                                if (!Item.CanAccessoryUpgrade())
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToUpgrade, Item.Info.ItemName), MessageType.System);
                                else
                                    MoveItem(GameScene.Game.NPCAccessoryUpgradeBox.TargetCell);

                                return;
                            }

                            if (Item != null && (Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
                            {
                                if (e.Button == MouseButtons.Right)
                                    CEnvir.Enqueue(new C.MarriageTeleport());
                                return;
                            }

                            if (!MoveItem(GameScene.Game.InventoryBox.Grid))
                                GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInInventory, MessageType.System);

                            break;
                        case GridType.CompanionEquipment:

                            if (Item == null) return;

                            if (GameScene.Game.NPCRepairBox.Visible)
                            {
                                if (Item.CurrentDurability >= Item.MaxDurability || !Item.Info.CanRepair)
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairFullyRepaired, Item.Info.ItemName), MessageType.System);
                                else if (!MoveItem(GameScene.Game.NPCRepairBox.Grid))
                                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.UnableToRepairHere, Item.Info.ItemName), MessageType.System);
                                return;
                            }



                            if (!MoveItem(GameScene.Game.InventoryBox.Grid))
                                GameScene.Game.ReceiveChat(CEnvir.Language.NoFreeSpaceInInventory, MessageType.System);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
            }

        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Locked || GameScene.Game.CurrencyPickedUp != null || (!Linked && Link != null) || GameScene.Game.Observer) return;

            base.OnMouseDoubleClick(e);

            if (ReadOnly || e.Button != MouseButtons.Left) return;
            
            switch (GridType)
            {
                case GridType.Belt:
                case GridType.AutoPotion:
                    UseItem();
                    break;
                case GridType.Inventory:
                case GridType.CompanionInventory:
                case GridType.CompanionEquipment:

                    UseItem();
                    return;
                    
                case GridType.Storage:
                case GridType.PartsStorage:
                    
                    UseItem();
                    return;
                case GridType.Equipment:
                    if (Item == null) return;

                    if ((Item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage)
                        CEnvir.Enqueue(new C.MarriageTeleport());

                    break;
            }
        }
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (MouseControl != this) return;

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.ToggleItemLock:
                        if (Locked || GameScene.Game.CurrencyPickedUp != null || (!Linked && Link != null) || GameScene.Game.Observer) return;
                        if (ReadOnly || !Enabled) return;


                        if (Item != null)
                        {
                            CEnvir.Enqueue(new C.ItemLock { GridType = GridType, SlotIndex = Slot, Locked = (Item.Flags & UserItemFlags.Locked) != UserItemFlags.Locked });
                            e.Handled = true;
                        }
                        break;
                    default: continue;
                }

            }
        }
        
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _FixedBorder = false;
                _FixedBorderColour = false;
                _GridType = GridType.None;
                _HostGrid = null;
                _ItemGrid = null;
                _Locked = false;
                _ReadOnly = false;
                _Selected = false;
                _Slot = 0;
                _ShowCountLabel = false;
                QuickInfoItem = null;
                _QuickInfo = null;
                _QuickItem = null;

                DXItemCell oldLink = _Link;

                _Link = null;
                if (oldLink != null)
                    oldLink.Link = null;

                _LinkedCount = 0;
                _Linked = false;
                _AllowLink = false;

                FixedBorderChanged = null;
                FixedBorderColourChanged = null;
                GridTypeChanged = null;
                HostGridChanged = null;
                ItemChanged = null;
                ItemGridChanged = null;
                LockedChanged = null;
                ReadOnlyChanged = null;
                SelectedChanged = null;
                SlotChanged = null;
                ShowCountLabelChanged = null;
                LinkChanged = null;
                LinkedCountChanged = null;
                LinkedChanged = null;
                AllowLinkChanged = null;

                if (CountLabel != null)
                {
                    if (!CountLabel.IsDisposed)
                        CountLabel.Dispose();

                    CountLabel = null;
                }
            }

            if (SelectedCell == this) SelectedCell = null;
        }
        #endregion
    }
}

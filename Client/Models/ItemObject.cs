using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;
using Library.SystemModels;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    public sealed class ItemObject : MapObject
    {
        public override ObjectType Race => ObjectType.Item;

        public DXLabel FocusLabel;

        public override bool Blocking => false;

        public ClientUserItem Item;
        public MirLibrary BodyLibrary;
        public Color LabelBackColour = Color.FromArgb(30, 0, 24, 48);
        private ItemInfo appearanceInfo;
        private ItemInfo sourceInfo;
        private int partIndex;
        private bool currency;
        private int drawIndex;
        private long appearanceCount = -1;
        private MirImage[] sizeImages;
        private Size imageSize;
        private string labelSettings;
        private bool labelsDirty = true;
        private bool nameLabelsReady;
        internal bool IsPileRepresentative = true;
        internal int VisualPriority { get; private set; }

        public ItemObject(S.ObjectItem info)
        {
            ObjectID = info.ObjectID;

            Item = info.Item;

            ItemInfo itemInfo = ResolveAppearanceInfo(Item.AddedStats[Stat.ItemIndex]);

            if (info.Item.Info.ItemEffect == ItemEffect.ItemPart)
            {
                Title = "[Part]";
            }

            Name = Item.Count > 1 ? $"{itemInfo.ItemName} ({Item.Count})" : itemInfo.ItemName;

            if ((Item.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem)
                Title = "(Quest)";

            switch (itemInfo.Rarity)
            {
                case Rarity.Common:
                    if (Item.AddedStats.Values.Count > 0 && Item.Info.ItemEffect != ItemEffect.ItemPart)
                    {
                        NameColour = Color.LightSkyBlue;

                        Effects.Add(new LootEffect(this, 110, Color.DeepSkyBlue));
                    }
                    else NameColour = Color.White;
                    break;
                case Rarity.Superior:
                    NameColour = Color.PaleGreen;
                    Effects.Add(new LootEffect(this, 100, Color.PaleGreen));
                    break;
                case Rarity.Elite:
                    NameColour = Color.MediumPurple;
                    Effects.Add(new LootEffect(this, 120, Color.MediumPurple));
                    break;
            }
            CurrentLocation = info.Location;
            VisualPriority = (int)itemInfo.Rarity * 2 + (Effects.Count > 0 ? 1 : 0);

            UpdateLibraries();

            SetFrame(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);
        }
        public void UpdateLibraries()
        {
            Frames = FrameSet.DefaultItem;

            CEnvir.LibraryList.TryGetValue(LibraryFile.Ground, out BodyLibrary);
            appearanceInfo = null;
            sizeImages = null;
            imageSize = Size.Empty;
        }

        public override void SetAnimation(ObjectAction action)
        {

            CurrentAnimation = MirAnimation.Standing;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }

        public override void Process()
        {
            DrawX = (CurrentLocation.X - User.CurrentLocation.X + OffSetX) * CellWidth + PixelOffsetX
                - User.MovingOffSet.X - User.ShakeScreenOffset.X;
            DrawY = (CurrentLocation.Y - User.CurrentLocation.Y + OffSetY) * CellHeight + PixelOffsetY
                - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;
            DrawColour = DefaultColour;
        }

        private void RefreshAppearance()
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile.Ground, out MirLibrary library);
            if (BodyLibrary != library)
            {
                BodyLibrary = library;
                sizeImages = null;
                imageSize = Size.Empty;
            }
            int currentPart = Item.Info.ItemEffect == ItemEffect.ItemPart ? Item.AddedStats[Stat.ItemIndex] : 0;
            if (appearanceInfo == null || sourceInfo != Item.Info || partIndex != currentPart || appearanceCount != Item.Count)
            {
                sourceInfo = Item.Info;
                partIndex = currentPart;
                appearanceInfo = ResolveAppearanceInfo(currentPart);
                currency = CEnvir.IsCurrencyItem(Item.Info);
                drawIndex = currency ? CEnvir.CurrencyImage(Item.Info, Item.Count) : appearanceInfo.Image;
                appearanceCount = Item.Count;
                imageSize = Size.Empty;
            }
            else if (!currency && drawIndex != appearanceInfo.Image)
            {
                drawIndex = appearanceInfo.Image;
                imageSize = Size.Empty;
            }
            if (BodyLibrary != null && (imageSize.IsEmpty || sizeImages != BodyLibrary.Images))
            {
                imageSize = BodyLibrary.GetSize(drawIndex);
                sizeImages = BodyLibrary.Images;
            }
        }

        private ItemInfo ResolveAppearanceInfo(int index)
        {
            if (Item.Info.ItemEffect != ItemEffect.ItemPart) return Item.Info;

            return Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == index) ?? Item.Info;
        }

        private bool SpriteVisible()
        {
            RefreshAppearance();
            return IntersectsViewport(new Rectangle(DrawX + (CellWidth - imageSize.Width) / 2,
                DrawY + (CellHeight - imageSize.Height) / 2, imageSize.Width, imageSize.Height));
        }

        internal static bool IntersectsViewport(Rectangle bounds)
        {
            return bounds.IntersectsWith(new Rectangle(Point.Empty, GameScene.Game.MapControl.Size));
        }

        public override void Draw()
        {
            if (!IsPileRepresentative || !SpriteVisible()) return;
            if (BodyLibrary == null) return;
            BodyLibrary.Draw(drawIndex, DrawX + (CellWidth - imageSize.Width) / 2, DrawY + (CellHeight - imageSize.Height) / 2, DrawColour, false, 1F, ImageType.Image);

        }

        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void NameChanged()
        {
            labelsDirty = true;
        }

        private void RefreshLabels()
        {
            if (!labelsDirty && labelSettings == Config.HighlightedItems) return;
            ReleaseLabels();
            labelSettings = Config.HighlightedItems;
            labelsDirty = false;
        }

        private void EnsureNameLabels()
        {
            RefreshLabels();
            if (nameLabelsReady) return;

            NameLabel = GroundItemLabels.Acquire(Name, ItemHighlights.Contains(Name) ? Color.OrangeRed : NameColour, Color.Empty);
            TitleNameLabel = GroundItemLabels.Acquire(Title, Color.Orange, Color.Empty);
            nameLabelsReady = true;
        }

        private void EnsureFocusLabel()
        {
            RefreshLabels();
            FocusLabel ??= GroundItemLabels.Acquire(Name, NameColour, LabelBackColour);
        }

        internal void ReleaseLabels()
        {
            GroundItemLabels.Release(NameLabel);
            GroundItemLabels.Release(TitleNameLabel);
            GroundItemLabels.Release(FocusLabel);
            NameLabel = TitleNameLabel = FocusLabel = null;
            labelsDirty = true;
            nameLabelsReady = false;
        }

        public override void DrawName()
        {
            if (!IsPileRepresentative) return;
            EnsureNameLabels();
            if (NameLabel == null) return;
            DrawLabel(NameLabel, GetNameBounds());
            if (TitleNameLabel != null)
                DrawLabel(TitleNameLabel, GetTitleBounds());
        }

        private Rectangle GetNameBounds()
        {
            int y = DrawY - (CellHeight - NameLabel.Size.Height) / 2 - 6 - (TitleNameLabel != null ? 13 : 0);
            return new Rectangle(DrawX + (CellWidth - NameLabel.Size.Width) / 2, y, NameLabel.Size.Width, NameLabel.Size.Height);
        }

        private Rectangle GetTitleBounds()
        {
            int y = DrawY - (CellHeight - TitleNameLabel.Size.Height) / 2 - 19;
            return new Rectangle(DrawX + (CellWidth - TitleNameLabel.Size.Width) / 2, y, TitleNameLabel.Size.Width, TitleNameLabel.Size.Height);
        }

        private void DrawLabel(DXLabel label, Rectangle bounds)
        {
            if (!LabelVisible(label, bounds)) return;
            label.Location = bounds.Location;
            label.Draw();
        }

        private static bool LabelVisible(DXLabel label, Rectangle bounds)
        {
            // Conservative margin accommodates UI scaling around the item origin.
            float scale = Math.Max(1F, GameScene.Game.UIScale);
            int margin = (int)(Math.Max(label.Size.Width, label.Size.Height) * scale);
            Rectangle viewport = new Rectangle(Point.Empty, GameScene.Game.MapControl.Size);
            viewport.Inflate(margin, margin);
            return viewport.IntersectsWith(bounds);
        }

        internal bool OverlayVisible()
        {
            if (!IsPileRepresentative) return false;
            if (!Config.ShowItemNames) return false;
            EnsureNameLabels();
            return (NameLabel != null && LabelVisible(NameLabel, GetNameBounds())) ||
                (TitleNameLabel != null && LabelVisible(TitleNameLabel, GetTitleBounds()));
        }

        public override void Remove()
        {
            base.Remove();
            ReleaseLabels();
        }

        internal void DrawFocus(int layer)
        {
            EnsureFocusLabel();
            if (FocusLabel == null) return;
            FocusLabel.Location = new Point(DrawX + (48 - FocusLabel.Size.Width) / 2, DrawY - (32 - FocusLabel.Size.Height / 2) + 8 - layer * 16);
            FocusLabel.Draw();
        }

        internal int FocusHeight
        {
            get
            {
                EnsureFocusLabel();
                return Math.Max(16, (FocusLabel?.Size.Height ?? 16) + 2);
            }
        }

        internal void DrawFocusAt(int y)
        {
            EnsureFocusLabel();
            if (FocusLabel == null) return;
            FocusLabel.Location = new Point(Math.Clamp(DrawX + (CellWidth - FocusLabel.Size.Width) / 2,
                0, Math.Max(0, GameScene.Game.MapControl.Size.Width - FocusLabel.Size.Width)), y);
            FocusLabel.Draw();
        }
    }

}

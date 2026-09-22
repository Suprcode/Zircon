using Client.Controls;
using Client.Envir;
using Client.Models;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed partial class MapControl
    {
        private int groundItemCount;
        public bool HasGroundItems => groundItemCount > 0;

        private const int MaxLootPageItems = 20;
        private const int LootFooterHeight = 24;
        private const int LootViewportPadding = 16;
        private readonly RenderRows<MapObject> objectRows = new RenderRows<MapObject>();
        private readonly RenderRows<MirEffect> effectRows = new RenderRows<MirEffect>();
        private readonly GroundLootPiles lootPiles = new GroundLootPiles();
        private readonly List<ItemObject> focusedLoot = new List<ItemObject>();
        private readonly List<MapObject> nameOverlayObjects = new List<MapObject>();
        private readonly List<MapObject> statusOverlayObjects = new List<MapObject>();
        private Point focusTile;
        private Cell focusCell;
        private int focusVersion = -1;
        private int focusOffset;
        private int selectedLootIndex;
        private bool denseLoot;
        private DXLabel lootPageLabel;
        private int lastPageOffset = -1, lastPageCount, lastPageTotal;
        private string lastPageFormat;

        public void PrepareLoot()
        {
            if (!HasGroundItems) return;
            lootPiles.Clear();
            nameOverlayObjects.Clear();
            statusOverlayObjects.Clear();
            if (denseLoot != Config.DenseLoot)
            {
                denseLoot = Config.DenseLoot;
                TextureValid = false;
                focusOffset = 0;
                selectedLootIndex = 0;
            }
            if (denseLoot)
                foreach (MapObject ob in Objects)
                    if (ob is ItemObject item) lootPiles.Add(item.CurrentLocation, item.ObjectID, item.VisualPriority);
            foreach (MapObject ob in Objects)
            {
                if (ob is not ItemObject item)
                {
                    nameOverlayObjects.Add(ob);
                    statusOverlayObjects.Add(ob);
                    continue;
                }
                item.IsPileRepresentative = true;
                if (denseLoot)
                {
                    item.IsPileRepresentative = lootPiles[item.CurrentLocation] == item.ObjectID;
                }
                if (item.OverlayVisible()) nameOverlayObjects.Add(item);
            }
        }

        private void CollectFocusedLoot(Cell cell)
        {
            if (focusTile == MapLocation && focusCell == cell && focusVersion == cell.ObjectVersion) return;
            focusedLoot.Clear();
            if (focusTile != MapLocation)
            {
                focusTile = MapLocation;
                focusOffset = 0;
                selectedLootIndex = 0;
            }
            focusCell = cell;
            focusVersion = cell.ObjectVersion;
            if (cell.Objects == null) return;
            for (int i = cell.Objects.Count - 1; i >= 0; i--)
                if (cell.Objects[i] is ItemObject item) focusedLoot.Add(item);
            selectedLootIndex = Math.Clamp(selectedLootIndex, 0, Math.Max(0, focusedLoot.Count - 1));
        }

        private int LootRowHeight => focusedLoot.Count > 0 ? focusedLoot[0].FocusHeight : 16;
        private int LootPageSize => Math.Clamp((Size.Height - LootFooterHeight - LootViewportPadding) / LootRowHeight, 1, MaxLootPageItems);

        public bool ScrollLoot(int delta)
        {
            if (!HasGroundItems || delta == 0 ||
                MapLocation.X < 0 || MapLocation.Y < 0 || MapLocation.X >= Width || MapLocation.Y >= Height) return false;
            CollectFocusedLoot(Cells[MapLocation.X, MapLocation.Y]);
            if (focusedLoot.Count <= 1) return false;

            if (CEnvir.Ctrl)
            {
                selectedLootIndex = Math.Clamp(selectedLootIndex - Math.Sign(delta), 0, focusedLoot.Count - 1);
                int pageSize = Math.Min(LootPageSize, focusedLoot.Count);
                if (selectedLootIndex < focusOffset)
                    focusOffset = selectedLootIndex;
                else if (selectedLootIndex >= focusOffset + pageSize)
                    focusOffset = selectedLootIndex - pageSize + 1;
            }
            else
            {
                if (focusedLoot.Count <= LootPageSize) return false;
                focusOffset = Math.Clamp(focusOffset - Math.Sign(delta) * LootPageSize, 0, focusedLoot.Count - LootPageSize);
                selectedLootIndex = focusOffset;
            }
            return true;
        }

        internal ItemObject GetSelectedLoot(ItemObject fallback)
        {
            if (!CEnvir.Ctrl || MapLocation.X < 0 || MapLocation.Y < 0 ||
                MapLocation.X >= Width || MapLocation.Y >= Height) return fallback;

            CollectFocusedLoot(Cells[MapLocation.X, MapLocation.Y]);
            return focusedLoot.Count == 0 ? fallback : focusedLoot[selectedLootIndex];
        }

        private bool TryPickUpSelectedLoot()
        {
            if (!CEnvir.Ctrl || MapObject.MouseObject is not ItemObject item ||
                !Functions.InRange(item.CurrentLocation, User.CurrentLocation, User.Stats[Stat.PickUpRadius])) return false;

            MapButtons &= ~MouseButtons.Left;
            if (CEnvir.Now <= GameScene.Game.PickUpTime) return true;

            CEnvir.Enqueue(new C.PickUp { ObjectID = item.ObjectID });
            GameScene.Game.PickUpTime = CEnvir.Now.AddMilliseconds(250);
            return true;
        }

        private void DrawLootFocus(Cell cell)
        {
            CollectFocusedLoot(cell);
            if (focusedLoot.Count == 0) return;

            int count = Math.Min(LootPageSize, focusedLoot.Count);
            focusOffset = Math.Clamp(focusOffset, 0, focusedLoot.Count - count);
            ItemObject first = focusedLoot[focusOffset];
            int rowHeight = LootRowHeight;
            int listHeight = count * rowHeight;
            int top = Math.Clamp(first.DrawY - listHeight - LootFooterHeight, 0,
                Math.Max(0, Size.Height - listHeight - LootFooterHeight));

            for (int i = 0; i < count; i++)
            {
                int itemIndex = focusOffset + i;
                focusedLoot[itemIndex].DrawFocusAt(top + i * rowHeight, CEnvir.Ctrl && itemIndex == selectedLootIndex);
            }

            DrawLootSummary(first.DrawX, top + listHeight, count);
        }

        private void DrawLootSummary(int drawX, int drawY, int count)
        {
            if (lootPageLabel == null)
            {
                lootPageLabel = new DXLabel
                {
                    IsControl = false,
                    IsVisible = true,
                    ForeColour = Color.White,
                    BackColour = Color.Black,
                    Outline = true,
                };
            }

            string format = focusedLoot.Count > count ? CEnvir.Language.GroundLootPage : CEnvir.Language.GroundLootCount;
            if (lastPageOffset != focusOffset || lastPageCount != count || lastPageTotal != focusedLoot.Count || lastPageFormat != format)
            {
                lastPageOffset = focusOffset;
                lastPageCount = count;
                lastPageTotal = focusedLoot.Count;
                lastPageFormat = format;
                lootPageLabel.Text = focusedLoot.Count > count
                    ? string.Format(format, focusOffset + 1, focusOffset + count, focusedLoot.Count)
                    : string.Format(format, focusedLoot.Count);
            }
            lootPageLabel.Location = new Point(
                Math.Clamp(drawX + (CellWidth - lootPageLabel.Size.Width) / 2, 0, Math.Max(0, Size.Width - lootPageLabel.Size.Width)),
                Math.Clamp(drawY, 0, Math.Max(0, Size.Height - lootPageLabel.Size.Height)));
            lootPageLabel.Draw();
        }

        private void ClearLootCaches()
        {
            ReleaseWorldNames();
            objectRows.Clear();
            effectRows.Clear();
            lootPiles.Clear();
            nameOverlayObjects.Clear();
            statusOverlayObjects.Clear();
            focusedLoot.Clear();
            focusCell = null;
            focusVersion = -1;
            focusOffset = 0;
            selectedLootIndex = 0;
        }

        private void DisposeLoot()
        {
            groundItemCount = 0;
            ClearLootCaches();

            if (lootPageLabel != null)
            {
                if (!lootPageLabel.IsDisposed)
                    lootPageLabel.Dispose();

                lootPageLabel = null;
            }
        }
    }
}

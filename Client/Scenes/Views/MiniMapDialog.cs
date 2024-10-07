using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MiniMapDialog : DXWindow
    { 
        #region Properties

        public Rectangle Area;
        private DXImageControl Image;
        public DXControl Panel;

        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();

        public static float ScaleX, ScaleY;

        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            foreach (DXControl control in Controls)
                control.Opacity = Opacity;

            foreach (DXControl control in MapInfoObjects.Values)
                control.Opacity = Opacity;

            if (Image != null)
            {
                Image.Opacity = Opacity;
                Image.ImageOpacity = Opacity;
            }
        }
        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            Area = ClientArea;
            Area.Inflate(6,6);

            if (Panel == null) return;

            Panel.Location = Area.Location;
            Panel.Size = Area.Size;

            UpdateMapPosition();
        }

        public override WindowType Type => WindowType.MiniMapBox;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;

        #endregion

        public MiniMapDialog()
        {
            BackColour = Color.Black;
            HasFooter = false;

            AllowResize = true;
            CloseButton.Visible = false;

            Size = new Size(200, 200);

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
                IgnoreMoveBounds =  true,
            };
            GameScene.Game.MapControl.MapInfoChanged += MapControl_MapInfoChanged;
            Image.Moving += Image_Moving;
        }

        #region Methods

        private void Image_Moving(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ClipMap();
        }

        private void MapControl_MapInfoChanged(object sender, EventArgs e)
        {
            foreach (DXControl temp in MapInfoObjects.Values)
                temp.Dispose();

            MapInfoObjects.Clear();

            if (GameScene.Game.MapControl.MapInfo == null) return;

            TitleLabel.Text = GameScene.Game.MapControl.MapInfo.Description;
            Image.Index = GameScene.Game.MapControl.MapInfo.MiniMap;

            ScaleX = Image.Size.Width/(float) GameScene.Game.MapControl.Width;
            ScaleY = Image.Size.Height/(float) GameScene.Game.MapControl.Height;
            
            foreach (NPCInfo ob in Globals.NPCInfoList.Binding)
                Update(ob);

            foreach (MovementInfo ob in Globals.MovementInfoList.Binding)
                Update(ob);

            foreach (ClientObjectData ob in GameScene.Game.DataDictionary.Values)
                Update(ob);

        }

        public void Update(NPCInfo ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = Image;
                control.Opacity = Opacity;
            }
            else if ((CurrentQuest)control.Tag == ob.CurrentQuest) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Parent = Image;
            control.Opacity = Opacity;
            MapInfoObjects[ob] = control;

            if (ob.Region.PointList == null)
                ob.Region.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

            foreach (Point point in ob.Region.PointList)
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


            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        public void Update(MovementInfo ob)
        {
            if (ob.SourceRegion == null || ob.SourceRegion.Map != GameScene.Game.MapControl.MapInfo) return;
            if (ob.DestinationRegion?.Map == null || ob.Icon == MapIcon.None) return;

            if (GameScene.Game.MapControl.InstanceInfo != null)
            {
                if (!GameScene.Game.MapControl.InstanceInfo.Maps.Any(m => m.Map == ob.SourceRegion.Map) && ob.NeedInstance == null) return;
                if (!GameScene.Game.MapControl.InstanceInfo.Maps.Any(m => m.Map == ob.DestinationRegion?.Map) && ob.NeedInstance == null) return;
            }

            if (ob.SourceRegion.PointList == null)
                ob.SourceRegion.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

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
                Hint = ob.DestinationRegion.Map.Description,
            };
            control.OpacityChanged += (o, e) => control.ImageOpacity = control.Opacity;

            GameScene.Game.UpdateMapIcon(control, ob.Icon);

            //control.MouseClick += (o, e) => SelectedInfo = ob.DestinationRegion.Map;
            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }

        public void Update(ClientObjectData ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index) return;
                if (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common) return;
                if (ob.MonsterInfo != null && (ob.Dead || ob.MonsterInfo.Image == MonsterImage.None)) return;

                MapInfoObjects[ob] = control = new DXControl
                {
                    DrawTexture = true,
                    Parent = Image,
                    Opacity = Opacity,
                    //MonsterInfo.AI < 0 ? Color.FromArgb(150, 200, 255) : Color.Red,
                };


            }
            else if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index || (ob.MonsterInfo != null && ob.Dead) || (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common))
            {
                control.Dispose();
                MapInfoObjects.Remove(ob);
                return;
            }

            Size size = new Size(3, 3);
            Color colour = Color.White;
            string name = ob.Name;

            if (ob.MonsterInfo != null)
            {
                name = $"{ob.MonsterInfo.MonsterName}";

                if (ob.MonsterInfo.AI < 0)
                {
                    colour =  Color.LightBlue;
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
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = colour,
                            DrawTexture = true,
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
                    control.DrawTexture = false;
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
                    colour = Color.Cyan;
                }
                else if (GameScene.Game.Observer)
                {
                    control.Visible = false;
                }
                else if (GameScene.Game.GroupBox.Members.Any(x => x.ObjectID == ob.ObjectID))
                {
                    colour = Color.Blue;
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

            if (MapObject.User.ObjectID != ob.ObjectID) return;

            Image.Location = new Point(-control.Location.X + Area.Width / 2, -control.Location.Y + Area.Height / 2);

            ClipMap();
        }

        public void UpdateMapPosition()
        {
            if (MapObject.User == null) return;

            ClientObjectData data;

            if (!GameScene.Game.DataDictionary.TryGetValue(MapObject.User.ObjectID, out data)) return;

            DXControl control;
            if (!MapInfoObjects.TryGetValue(data, out control)) return;

            Image.Location = new Point(-control.Location.X + Area.Width / 2, -control.Location.Y + Area.Height / 2);

            ClipMap();
        }

        private void ClipMap()
        {
            int x = Image.Location.X;

            if (x + Image.Size.Width < Panel.Size.Width)
                x = Panel.Size.Width - Image.Size.Width;

            if (x > 0)
                x = 0;

            int y = Image.Location.Y;

            if (y + Image.Size.Height < Panel.Size.Height)
                y = Panel.Size.Height - Image.Size.Height;

            if (y > 0)
                y = 0;

            Image.Location = new Point(x, y);
        }

        public void Remove(object ob)
        {
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }
        
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            OnBeforeDraw();
            DrawControl();
            OnBeforeChildrenDraw();
            DrawChildControls();
            DrawWindow();
            TitleLabel.Draw();
            DrawBorder();
            OnAfterDraw();
        }
        #endregion
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Area = Rectangle.Empty;
                ScaleX = 0;
                ScaleY = 0;

                foreach (KeyValuePair<object, DXControl> pair in MapInfoObjects)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                MapInfoObjects.Clear();
                MapInfoObjects = null;


                if (Image != null)
                {
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
            }
        }

        #endregion
    }
}

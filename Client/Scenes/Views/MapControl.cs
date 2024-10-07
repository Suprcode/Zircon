using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Models.Particles;
using Library;
using Library.SystemModels;
using SlimDX;
using SlimDX.Direct3D9;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MapControl : DXControl
    {
        #region Properties

        public static UserObject User => GameScene.Game.User;

        #region MapInformation

        public MapInfo MapInfo
        {
            get => _MapInfo;
            set
            {
                if (_MapInfo == value) return;

                MapInfo oldValue = _MapInfo;
                _MapInfo = value;

                OnMapInfoChanged(oldValue, value);
            }
        }
        private MapInfo _MapInfo;

        public InstanceInfo InstanceInfo
        {
            get => _InstanceInfo;
            set
            {
                if (_InstanceInfo == value) return;

                InstanceInfo oldValue = _InstanceInfo;
                _InstanceInfo = value;
                OnInstanceInfoChanged(oldValue, value);
            }
        }
        private InstanceInfo _InstanceInfo;
        public void OnInstanceInfoChanged(InstanceInfo oValue, InstanceInfo nValue)
        {
            MapInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> MapInfoChanged;
        public void OnMapInfoChanged(MapInfo oValue, MapInfo nValue)
        {
            TextureValid = false;
            LoadMap();

            if (oValue != null)
            {
                if (nValue == null || nValue.Music != oValue.Music)
                    DXSoundManager.Stop(oValue.Music);
            }

            if (nValue != null)
                DXSoundManager.Play(nValue.Music);

            UpdateWeather();
            LLayer.UpdateLights();
            MapInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Animation

        public int Animation
        {
            get => _Animation;
            set
            {
                if (_Animation == value) return;

                int oldValue = _Animation;
                _Animation = value;

                OnAnimationChanged(oldValue, value);
            }
        }
        private int _Animation;
        public event EventHandler<EventArgs> AnimationChanged;
        public void OnAnimationChanged(int oValue, int nValue)
        {
            AnimationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }



        #endregion
        

        #region MouseLocation

        public Point MouseLocation
        {
            get => _MouseLocation;
            set
            {
                if (_MouseLocation == value) return;

                Point oldValue = _MouseLocation;
                _MouseLocation = value;

                OnMouseLocationChanged(oldValue, value);
            }
        }
        private Point _MouseLocation;
        public event EventHandler<EventArgs> MouseLocationChanged;
        public void OnMouseLocationChanged(Point oValue, Point nValue)
        {
            MouseLocationChanged?.Invoke(this, EventArgs.Empty);
            UpdateMapLocation();
        }

        #endregion

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (FLayer != null)
                FLayer.Size = Size;

            if (LLayer != null)
                LLayer.Size = Size;


            OffSetX = Size.Width/2/CellWidth;
            OffSetY = Size.Height/2/CellHeight;
        }

        public MouseButtons MapButtons;
        public Point MapLocation;

        public bool Mining;
        public Point MiningPoint;
        public MirDirection MiningDirection;

        public FishingState FishingState;
        public Point FloatLocation;
        public MirDirection FishingDirection;
        public bool AutoCast;

        public Floor FLayer;
        public Light LLayer;

        public Cell[,] Cells;
        public int Width, Height;

        public List<DXControl> MapInfoObjects = new List<DXControl>();
        public List<MapObject> Objects = new List<MapObject>();
        public List<MirEffect> Effects = new List<MirEffect>();
        public List<Models.Particles.ParticleEmitter> ParticleEffects = new List<Models.Particles.ParticleEmitter>();

        public const int CellWidth = 48, CellHeight = 32;

        public int ViewRangeX = 12, ViewRangeY = 24;

        public static int OffSetX;
        public static int OffSetY;


        #endregion

        public MapControl()
        {
            DrawTexture = true;

            BackColour = Color.Empty;
            
            FLayer = new Floor { Parent = this, Size = Size };
            LLayer = new Light { Parent = this, Location = new Point(-GameScene.Game.Location.X, -GameScene.Game.Location.Y), Size = Size };
        }

        #region Methods

        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (!Visible) return;

            DrawBackground();

            if (FLayer.TextureValid)
                DXManager.Sprite.Draw(FLayer.ControlTexture, Color.White);

            if (Config.DrawEffects)
            {
                foreach (MirEffect ob in Effects)
                {
                    if (ob.DrawType != DrawType.Floor) continue;

                    ob.Draw();
                }
            }

            DrawObjects();

            if (MapObject.MouseObject != null) // && MapObject.MouseObject != MapObject.TargetObject)
                MapObject.MouseObject.DrawBlend();

            if (Config.DrawEffects)
            {
                foreach (MirEffect ob in Effects)
                {
                    if (ob.DrawType != DrawType.Final) continue;

                     ob.Draw();
                }
            }

            DXManager.Sprite.Flush();

            DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
            DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);

            DXManager.Sprite.Draw(LLayer.ControlTexture, Color.White);

            DXManager.Sprite.End();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);

            foreach (MapObject ob in Objects)
            {
                if (ob.Dead) continue;

                switch (ob.Race)
                {
                    case ObjectType.Player:
                        if (!Config.ShowPlayerNames) continue;
                        break;
                    case ObjectType.Item:
                        if (!Config.ShowItemNames || ob.CurrentLocation == MapLocation) continue;
                        break;
                    case ObjectType.NPC:
                        break;
                    case ObjectType.Spell:
                        break;
                    case ObjectType.Monster:
                        if (!Config.ShowMonsterNames) continue;
                        break;
                }

                ob.DrawName();
            }

            if (MapObject.MouseObject != null && MapObject.MouseObject.Race != ObjectType.Item)
                MapObject.MouseObject.DrawName();

            foreach (MapObject ob in Objects)
            {
                ob.DrawChat();
                ob.DrawPoison();
                ob.DrawHealth();
            }

            if (Config.ShowDamageNumbers)
                foreach (MapObject ob in Objects)
                    ob.DrawDamage();

            if (MapLocation.X >= 0 && MapLocation.X < Width && MapLocation.Y >= 0 && MapLocation.Y < Height)
            {
                Cell cell = Cells[MapLocation.X, MapLocation.Y];
                int layer = 0;
                if (cell.Objects != null)
                    for (int i = cell.Objects.Count - 1; i >= 0; i--)
                    {
                        ItemObject ob = cell.Objects[i] as ItemObject;

                        ob?.DrawFocus(layer++);
                    }
            }
        }

        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            FLayer.CheckTexture();
            LLayer.CheckTexture();
            
            //CreateTexture();
            OnBeforeDraw();

            DrawControl();
            
            DrawBorder();
            OnAfterDraw();
        }

        private void DrawBackground()
        {
            if (MapInfo.Background <= 0) return;

            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Background, out MirLibrary library)) return;

            MirImage image = library.CreateImage(MapInfo.Background, ImageType.Image);

            if (image?.Image == null) return;

            PresentTexture(image.Image, Parent,DisplayArea, Color.White, this, 0, 0, 1F);
        }

        private void DrawObjects()
        {
            int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 4), maxX = Math.Min(Width - 1, User.CurrentLocation.X + OffSetX + 4);
            int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 4), maxY = Math.Min(Height - 1, User.CurrentLocation.Y + OffSetY + 25);

            for (int y = minY; y <= maxY; y++)
            {
                int drawY = (y - User.CurrentLocation.Y + OffSetY + 1) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                for (int x = minX; x <= maxX; x++)
                {
                    int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                    Cell cell = Cells[x, y];

                    MirLibrary library;
                    LibraryFile file;

                    if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.MiddleImage - 1;

                        bool blend = false;
                        if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                        {
                            index += Animation%(cell.MiddleAnimationFrame & 0x4F);
                            blend = (cell.MiddleAnimationFrame & 0x50) > 0;
                        }

                        Size s = library.GetSize(index);

                        if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            if (!blend)
                                library.Draw(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                            else
                                library.DrawBlend(index, drawX, drawY - s.Height, Color.White, false, 0.5F, ImageType.Image);
                        }
                    }



                    if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                    {
                        int index = cell.FrontImage - 1;

                        bool blend = false;
                        if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                        {
                            index += Animation % (cell.FrontAnimationFrame & 0x7F);
                            blend = (cell.FrontAnimationFrame & 0x80) > 0;
                        }
                    
                        Size s = library.GetSize(index);


                        if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            if (!blend)
                                library.Draw(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                            else
                                library.DrawBlend(index, drawX, drawY - s.Height, Color.White, false, 0.5F, ImageType.Image);
                        }
                    }
                }

                foreach (MapObject ob in Objects)
                {
                    if (ob.RenderY == y)
                        ob.Draw();
                }

                if (Config.DrawEffects)
                {
                    foreach (MirEffect ob in Effects)
                    {
                        if (ob.DrawType != DrawType.Object) continue;

                        if (ob.MapTarget.IsEmpty && ob.Target != null)
                        {
                            if (ob.Target.RenderY == y && ob.Target != User)
                                ob.Draw();
                        }
                        else if (ob.MapTarget.Y == y)
                            ob.Draw();
                    }
                }

            }

            if (User.Opacity != 1f) return;
            float oldOpacity = MapObject.User.Opacity;
            MapObject.User.Opacity = 0.65F;

            MapObject.User.DrawPlayer(false);

            MapObject.User.Opacity = oldOpacity;

            if (Config.DrawEffects)
            {
                if (Config.DrawParticles)
                {
                    foreach (var ob in ParticleEffects)
                    {
                        ob.Draw();
                    }
                }

                foreach (MirEffect ob in Effects)
                {
                    if (ob.DrawType != DrawType.Object || !ob.MapTarget.IsEmpty || ob.Target != User) continue;

                    ob.Draw();
                }
            }
        }

        private void LoadMap()
        {
            try
            {
                var path = Path.Combine(Config.MapPath, MapInfo.FileName + ".map");

                if (!File.Exists(path)) return;

                using (MemoryStream mStream = new MemoryStream(File.ReadAllBytes(path)))
                using (BinaryReader reader = new BinaryReader(mStream))
                {
                    mStream.Seek(22, SeekOrigin.Begin);
                    Width = reader.ReadInt16();
                    Height = reader.ReadInt16();

                    mStream.Seek(28, SeekOrigin.Begin);

                    Cells = new Cell[Width, Height];
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            Cells[x, y] = new Cell();

                    for (int x = 0; x < Width/2; x++)
                        for (int y = 0; y < Height/2; y++)
                        {
                            Cells[(x*2), (y*2)].BackFile = reader.ReadByte();
                            Cells[(x*2), (y*2)].BackImage = reader.ReadUInt16();
                        }

                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                        {
                            byte flag = reader.ReadByte();
                            Cells[x, y].MiddleAnimationFrame = reader.ReadByte();

                            byte value = reader.ReadByte();
                            Cells[x, y].FrontAnimationFrame = value == 255 ? 0 : value;
                            Cells[x, y].FrontAnimationFrame &= 0x8F; //Probably a Blend Flag

                            Cells[x, y].FrontFile = reader.ReadByte();
                            Cells[x, y].MiddleFile = reader.ReadByte();

                            Cells[x, y].MiddleImage = reader.ReadUInt16() + 1;
                            Cells[x, y].FrontImage = reader.ReadUInt16() + 1;

                            mStream.Seek(3, SeekOrigin.Current);

                            Cells[x, y].Light = (byte) (reader.ReadByte() & 0x0F)*2;

                            mStream.Seek(1, SeekOrigin.Current);

                            Cells[x, y].Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);
                        }
                }
            }
            catch (Exception ex)
            {
                CEnvir.SaveException(ex);
            }

            foreach (MapObject ob in Objects)
                if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                    Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].AddObject(ob);
        }

        private void UpdateWeather()
        {
            GameScene.Game.MapControl.ParticleEffects.Clear();

            if (Config.DrawWeather)
            {
                var point = new Point((Size.Width) / 2, (Size.Height) / 2);

                if (GameScene.Game.MapControl.MapInfo.Weather.HasFlag(Weather.Rain))
                {
                    var emitter = (ParticleEmitter)Activator.CreateInstance(typeof(Rain), point);
                    GameScene.Game.MapControl.ParticleEffects.Add(emitter);
                }

                if (GameScene.Game.MapControl.MapInfo.Weather.HasFlag(Weather.Snow))
                {
                    var emitter = (ParticleEmitter)Activator.CreateInstance(typeof(Snow), point);
                    GameScene.Game.MapControl.ParticleEffects.Add(emitter);
                }

                if (GameScene.Game.MapControl.MapInfo.Weather.HasFlag(Weather.Fog))
                {
                    var emitter = (ParticleEmitter)Activator.CreateInstance(typeof(Fog), point);
                    GameScene.Game.MapControl.ParticleEffects.Add(emitter);
                }

                if (GameScene.Game.MapControl.MapInfo.Weather.HasFlag(Weather.Lightning))
                {
                    var emitter = (ParticleEmitter)Activator.CreateInstance(typeof(Lightning), point);
                    GameScene.Game.MapControl.ParticleEffects.Add(emitter);
                }
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MouseLocation = e.Location;
        }
        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (GameScene.Game.Observer) return;

            MapButtons |= e.Button;

            if (e.Button == MouseButtons.Right)
            {
                if (Config.RightClickDeTarget && MapObject.TargetObject?.Race == ObjectType.Monster)
                    MapObject.TargetObject = null;
            }
            
            if (e.Button != MouseButtons.Left) return;

            DXItemCell cell = DXItemCell.SelectedCell;
            if (cell != null)
            {
                MapButtons &= ~e.Button;

                if (cell.GridType == GridType.Belt)
                {
                    cell.QuickInfo = null;
                    cell.QuickItem = null;
                    DXItemCell.SelectedCell = null;

                    ClientBeltLink link = GameScene.Game.BeltBox.Links[cell.Slot];
                    CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update serve
                    return;
                }

                if (cell.GridType == GridType.AutoPotion)
                {
                    cell.QuickInfo = null;
                    cell.QuickItem = null;
                    DXItemCell.SelectedCell = null;

                    GameScene.Game.AutoPotionBox.Rows[cell.Slot].SendUpdate();
                    return;
                }

                if ((cell.Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (cell.GridType != GridType.Inventory && cell.GridType != GridType.CompanionInventory))
                {
                    DXItemCell.SelectedCell = null;
                    return;
                }
                
                DXItemAmountWindow window = new DXItemAmountWindow("Drop Item", cell.Item);

                window.ConfirmButton.MouseClick += (o, a) =>
                {
                    if (window.Amount <= 0) return;

                    CEnvir.Enqueue(new C.ItemDrop
                    {
                        Link = new CellLinkInfo { GridType = cell.GridType, Slot = cell.Slot, Count = window.Amount }
                    });

                    cell.Locked = true;
                };

                DXItemCell.SelectedCell = null;
                return;
            }

            if (GameScene.Game.CurrencyPickedUp != null)
            {
                MapButtons &= ~e.Button;

                int index = GameScene.Game.CurrencyPickedUp.Info.Index;

                DXItemAmountWindow window = new DXItemAmountWindow("Drop Item", new ClientUserItem(GameScene.Game.CurrencyPickedUp.Info.DropItem, GameScene.Game.CurrencyPickedUp.Amount));

                window.ConfirmButton.MouseClick += (o, a) =>
                {
                    if (window.Amount <= 0) return;

                    CEnvir.Enqueue(new C.CurrencyDrop
                    {
                        CurrencyIndex = index,
                        Amount = window.Amount
                    });
                };

                GameScene.Game.CurrencyPickedUp = null;
                return;
            }
            
            if (CanAttack(MapObject.MouseObject))
            {
                MapObject.TargetObject = MapObject.MouseObject;

                #region Shuriken
                if (!Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, Globals.MagicRange) && User.LibraryWeaponShape == Globals.ShurikenLibraryWeaponShape)
                {

                    GameScene.Game.OutputTime = CEnvir.Now.AddSeconds(1);
                    GameScene.Game.ReceiveChat(string.Format(CEnvir.Language.GameSceneThrowTooFar, "Shuriken"), MessageType.Hint);

                    Stop();

                    return;
                }

                if (User.Horse == HorseType.None && User.LibraryWeaponShape == Globals.ShurikenLibraryWeaponShape && MapObject.TargetObject != null &&
                    (MapObject.TargetObject.Race == ObjectType.Monster || MapObject.TargetObject.Race == ObjectType.Player))
                {

                    if (CEnvir.Now < User.AttackTime)
                    {
                        Stop();

                        return;
                    }


                    if (CEnvir.Now >= User.AttackTime)
                    {
                        int delayTime = 500;

                        if (Functions.Distance(MapObject.TargetObject.CurrentLocation, MapObject.User.CurrentLocation) == 1)
                        {
                            delayTime = 100;
                        }
                        else
                        {
                            int x = MapObject.OffSetX * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
                            int y = MapObject.OffSetY * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

                            int x1 = (MapObject.TargetObject.CurrentLocation.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
                            int y1 = (MapObject.TargetObject.CurrentLocation.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

                            long duration = Functions.Distance(new Point(x, y / 32 * 48), new Point(x1, y1 / 32 * 48)) * TimeSpan.TicksPerMillisecond * 2;

                            delayTime = int.Parse(duration.ToString().Substring(0, 3));
                        }

                        MapObject.User.AttemptAction(
                            new ObjectAction
                            (
                                      MirAction.RangeAttack,
                                      Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation),
                                      MapObject.User.CurrentLocation,
                                      MapObject.TargetObject.ObjectID, //Ranged Attack Target ID;
                                      MagicType.Shuriken,
                                      delayTime
                            ));

                        Stop();

                        return;
                    }


                }

                #endregion Shukiran

                if (MapObject.MouseObject.Race == ObjectType.Monster && ((MonsterObject) MapObject.MouseObject).MonsterInfo.AI >= 0) //Check if AI is guard
                {
                    MapObject.MagicObject = MapObject.TargetObject;
                    GameScene.Game.FocusObject = MapObject.MouseObject;
                }
                return;
            }

            MapObject.TargetObject = null;
            GameScene.Game.FocusObject = null;
            //GameScene.Game.OldTargetObjectID = 0;
        }

        private void Stop()
        {
            Functions.Move(Location, MapObject.TargetObject.Direction, 0);
            GameScene.Game.FocusObject = null;
            MapObject.TargetObject = null;
        }
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (GameScene.Game.Observer) return;

                    GameScene.Game.AutoRun = false;
                    if (MapObject.MouseObject == null) return;
                    NPCObject npc = MapObject.MouseObject as NPCObject;
                    if (npc != null)
                    {
                        if (CEnvir.Now <= GameScene.Game.NPCTime) return;

                        GameScene.Game.NPCTime = CEnvir.Now.AddSeconds(1);

                        CEnvir.Enqueue(new C.NPCCall { ObjectID = npc.ObjectID });
                    }
                    break;
                case MouseButtons.Right:
                    GameScene.Game.AutoRun = false;

                    if (User.CurrentAction == MirAction.Standing)
                        GameScene.Game.CanRun = false;

                    if (!CEnvir.Ctrl) return;

                    PlayerObject player = MapObject.MouseObject as PlayerObject;

                    if (player == null || player == MapObject.User) return;
                    if (CEnvir.Now <= GameScene.Game.InspectTime && player.ObjectID == GameScene.Game.InspectID) return;

                    GameScene.Game.InspectTime = CEnvir.Now.AddMilliseconds(2500);
                    GameScene.Game.InspectID = player.ObjectID;
                    CEnvir.Enqueue(new C.Inspect { Index = player.CharacterIndex });
                    break;
            }
        }

        public void CheckCursor()
        {
            MapObject deadObject = null, itemObject = null;
            
            for (int d = 0; d < 4; d++)
            {
                for (int y = MapLocation.Y - d; y <= MapLocation.Y + d; y++)
                {
                    if (y >= Height) continue;
                    if (y < 0) break;
                    for (int x = MapLocation.X - d; x <= MapLocation.X + d; x++)
                    {
                        if (x >= Width) continue;
                        if (x < 0) break;

                        List<MapObject> list = Cells[x, y].Objects;
                        if (list == null) continue;

                        MapObject cellSelect = null;
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            MapObject ob = list[i];

                            if (ob == MapObject.User || ob.Race == ObjectType.Spell || ((x != MapLocation.X || y != MapLocation.Y) && !ob.MouseOver(MouseLocation))) continue;

                            if (ob.Dead || (ob.Race == ObjectType.Monster && ((MonsterObject)ob).CompanionObject != null))
                            {
                                if (deadObject == null)
                                    deadObject = ob;
                                continue;
                            }
                            if (ob.Race == ObjectType.Item)
                            {
                                if (itemObject == null)
                                    itemObject = ob;
                                continue;
                            }
                            if (x == MapLocation.X && y == MapLocation.Y && !ob.MouseOver(MouseLocation))
                            {
                                if (cellSelect == null)
                                    cellSelect = ob;
                            }
                            else
                            {
                                MapObject.MouseObject = ob;
                                return;
                            }
                        }

                        if (cellSelect != null)
                        {
                            MapObject.MouseObject = cellSelect;
                            return;
                        }
                    }
                }
            }

            MapObject mouseOb = deadObject ?? itemObject;

            if (mouseOb == null)
            {
                if ((User.CurrentLocation.X == MapLocation.X && User.CurrentLocation.Y == MapLocation.Y) || User.MouseOver(MouseLocation))
                    mouseOb = User;
            }


            MapObject.MouseObject = mouseOb;
        }

        public void ProcessInput()
        {
            if (GameScene.Game.Observer) return;

            if (User.Dead || (User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (User.Poison & PoisonType.Containment) == PoisonType.Containment || User.Buffs.Any(x => x.Type == BuffType.DragonRepulse)) return; //Para or Frozen??

            if (User.MagicAction != null)
            {
                if (CEnvir.Now < MapObject.User.NextActionTime || MapObject.User.ActionQueue.Count != 0) return;

                //  if (QueuedMagic.Action == MirAction.Magic && (Spell)QueuedMagic.Extra[1] == Magic.ShoulderDash && !GameScene.Game.MoveFrame) return;

                MapObject.User.AttemptAction(User.MagicAction);
                User.MagicAction = null;
                Mining = false;
            }

            bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);

            if (!haselementalhurricane && MapObject.TargetObject != null && !MapObject.TargetObject.Dead && ((MapObject.TargetObject.Race == ObjectType.Monster && string.IsNullOrEmpty(MapObject.TargetObject.PetOwner)) || CEnvir.Shift))
            {
                if (Functions.Distance(MapObject.TargetObject.CurrentLocation, MapObject.User.CurrentLocation) ==  1 && CEnvir.Now > User.AttackTime && User.Horse == HorseType.None)
                {
                    MapObject.User.AttemptAction(new ObjectAction(
                        MirAction.Attack,
                        Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation),
                        MapObject.User.CurrentLocation,
                        0, //Ranged Attack Target ID
                        MagicType.None,
                        Element.None));
                    return;
                }
            }

            //If  auto run

            MirDirection direction = MouseDirection(), best;

            if (GameScene.Game.AutoRun && !haselementalhurricane)
            {
                if (!GameScene.Game.MoveFrame || (User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) return;
                Run(direction);
                return;
            }

            if (MouseControl == this)
            {
                switch (MapButtons)
                {
                    case MouseButtons.Left:
                        Mining = false;

                        if (CEnvir.Shift && MapObject.TargetObject == null)
                        {
                            if (FishingState != FishingState.None) return;

                            if (CEnvir.Now > User.AttackTime && User.Horse == HorseType.None && !haselementalhurricane)
                                MapObject.User.AttemptAction(new ObjectAction(MirAction.Attack, direction, MapObject.User.CurrentLocation, 0, MagicType.None, Element.None));

                            return;
                        }

                        ClientUserItem weap = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];
                        ClientUserItem armour = GameScene.Game.Equipment[(int)EquipmentSlot.Armour];

                        if (CEnvir.Alt)
                        {
                            if (haselementalhurricane) return;
                            if (User.Horse != HorseType.None) return;
                            if (FishingState != FishingState.None) return;

                            if (weap?.Info.ItemEffect == ItemEffect.FishingRod && armour?.Info.ItemEffect == ItemEffect.FishingRobe)
                            {
                                FloatLocation = MapLocation;
                                FishingDirection = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, FloatLocation);

                                var distance = Functions.Distance(MapObject.User.CurrentLocation, FloatLocation);

                                if (Functions.FishingZone(Globals.FishingInfoList, MapInfo, Width, Height, FloatLocation) != null && Functions.ValidFishingDistance(distance, User.Stats[Stat.ThrowDistance]))
                                {
                                    FishingState = FishingState.Cast;
                                    break;
                                }
                            }
                            else
                            {
                                MapObject.User.AttemptAction(new ObjectAction(MirAction.Harvest, direction, MapObject.User.CurrentLocation));
                                return;
                            }
                        }

                        if (FishingState != FishingState.None)
                        {
                            FishingState = FishingState.Cancel;
                            break;
                        }

                        if (MapLocation == MapObject.User.CurrentLocation)
                        {
                            if (CEnvir.Now <= GameScene.Game.PickUpTime) return;

                            CEnvir.Enqueue(new C.PickUp());
                            GameScene.Game.PickUpTime = CEnvir.Now.AddMilliseconds(250);

                            return;
                        }

                        if (MapObject.MouseObject != null && MapObject.MouseObject.Race != ObjectType.Item && !MapObject.MouseObject.Dead) break;

                        if (!haselementalhurricane && MapInfo.CanMine && weap != null && weap.Info.ItemEffect == ItemEffect.PickAxe)
                        {
                            MiningPoint = Functions.Move(User.CurrentLocation, direction);

                            if (MiningPoint.X >= 0 && MiningPoint.Y >= 0 && MiningPoint.X < Width && MiningPoint.Y < Height && Cells[MiningPoint.X, MiningPoint.Y].Flag)
                            {
                                Mining = true;
                                break;
                            }
                        }

                        if (!CanMove(direction, 1) || haselementalhurricane)
                        {
                            best = MouseDirectionBest(direction, 1);

                            if (best == direction)
                            {
                                if (direction != User.Direction)
                                    MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                                return;
                            }

                            direction = best;
                        }

                        if (!haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                            MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
                        return;
                    case MouseButtons.Right:
                        Mining = false;

                        if (FishingState != FishingState.None)
                        {
                            FishingState = FishingState.Cancel;
                            break;
                        }

                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != MapObject.User && CEnvir.Ctrl) break;

                        if (!GameScene.Game.MoveFrame || (User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) break;
                        
                        if (Functions.InRange(MapLocation, MapObject.User.CurrentLocation, 2) || haselementalhurricane)
                        {
                            if (direction != User.Direction)
                                MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                            return;
                        }

                        Run(direction);

                        return;
                }
            }

            if (Mining)
            {
                ClientUserItem weap = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

                if (MapInfo.CanMine && weap != null && (weap.CurrentDurability > 0 || weap.Info.Durability == 0) && weap.Info.ItemEffect == ItemEffect.PickAxe &&
                    MiningPoint.X >= 0 && MiningPoint.Y >= 0 && MiningPoint.X < Width && MiningPoint.Y < Height && Cells[MiningPoint.X, MiningPoint.Y].Flag &&
                    Functions.Distance(MiningPoint, MapObject.User.CurrentLocation) == 1 && User.Horse == HorseType.None)
                {
                    if (CEnvir.Now > User.AttackTime)
                    {
                        MapObject.User.AttemptAction(new ObjectAction(
                            MirAction.Mining,
                            Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MiningPoint),
                            MapObject.User.CurrentLocation,
                            false));
                    }
                }
                else
                {
                    Mining = false;
                }
            }

            if (FishingState != FishingState.None || AutoCast)
            {
                if (CEnvir.Now > User.AttackTime)
                {
                    if (FishingState == FishingState.Reel)
                    {
                        FishingState = FishingState.None;
                    }
                    else
                    {
                        if (AutoCast)
                        {
                            FishingState = FishingState.Cast;
                            AutoCast = false;
                        }

                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Fishing, FishingDirection, MapObject.User.CurrentLocation, FishingState, FloatLocation, MapObject.User.FishFound));
                    }
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if ((MapObject.TargetObject.Race == ObjectType.Player || !string.IsNullOrEmpty(MapObject.TargetObject.PetOwner)) && !CEnvir.Shift) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, MapObject.User.CurrentLocation, 1)) return;

            direction = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanMove(direction, 1) || haselementalhurricane)
            {
                best = DirectionBest(direction, 1, MapObject.TargetObject.CurrentLocation);

                if (best == direction)
                {
                    if (direction != User.Direction)
                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                    return;
                }
                direction = best;
            }

            if (!haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
        }

        public void Run(MirDirection direction)
        {
            int steps = 1;

            if (GameScene.Game.CanRun && CEnvir.Now >= User.NextRunTime && User.BagWeight <= User.Stats[Stat.BagWeight] && User.WearWeight <= User.Stats[Stat.WearWeight])
            {
                steps++;
                if (User.Horse != HorseType.None)
                    steps++;
            }

            for (int i = 1; i <= steps; i++)
            {
                if (CanMove(direction, i)) continue;

                MirDirection best = MouseDirectionBest(direction, 1);

                if (best == direction)
                {
                    if (i == 1)
                    {
                        if (direction != User.Direction)
                            MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                        return;
                    }

                    steps = i - 1;
                }
                else
                {
                    steps = 1;
                }
                direction = best;
                break;
            }

            MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction, steps), steps, MagicType.None));
        }

        public MirDirection MouseDirectionBest(MirDirection dir, int distance) //22.5 = 16
        {

            Point loc = Functions.Move(MapObject.User.CurrentLocation, dir, distance);

            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height && !Cells[loc.X, loc.Y].Blocking()) return dir;
            

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            double ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;

            MirDirection best = (MirDirection)(angle / 45F);

            if (best == dir)
                best = Functions.ShiftDirection(dir, 1);

            MirDirection next = Functions.ShiftDirection(dir, -((int)best - (int)dir));

            if (CanMove(best, distance))
                return best;

            if (CanMove(next, distance))
                return next;

            return dir;
        }
        public MirDirection DirectionBest(MirDirection dir, int distance, Point targetLocation) //22.5 = 16
        {
            Point loc = Functions.Move(MapObject.User.CurrentLocation, dir, distance);

            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height && !Cells[loc.X, loc.Y].Blocking()) return dir;
            

            PointF c = new PointF(MapObject.OffSetX * MapObject.CellWidth + MapObject.CellWidth / 2F, MapObject.OffSetY * MapObject.CellHeight + MapObject.CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF((targetLocation.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth + MapObject.CellWidth / 2F,
                (targetLocation.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight + MapObject.CellHeight / 2F);
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            double ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (b.X < c.X) angle = 360 - angle;

            MirDirection best = (MirDirection)(angle / 45F);

            if (best == dir)
                best = Functions.ShiftDirection(dir, 1);

            MirDirection next = Functions.ShiftDirection(dir, -((int)best - (int)dir));

            if (CanMove(best, distance))
                return best;

            return CanMove(next, distance) ? next : dir;
        }

        private bool CanMove(MirDirection dir, int distance)
        {
            for (int i = 1; i <= distance; i++)
            {
                Point loc = Functions.Move(User.CurrentLocation, dir, i);

                if (loc.X < 0 || loc.Y < 0 || loc.X >= Width || loc.Y > Height) return false;

                if (Cells[loc.X, loc.Y].Blocking())
                    return false;
            }
            return true;
        }

        public MirDirection MouseDirection() //22.5 = 16
        {
            PointF p = new PointF(MouseLocation.X  / CellWidth, MouseLocation.Y / CellHeight);

            //If close proximity then co by co ords 
            if (Functions.InRange(new Point(OffSetX, OffSetY), Point.Truncate(p), 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), Point.Truncate(p));

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(MouseLocation.X, MouseLocation.Y);
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += 22.5F;
            if (angle > 360) angle -= 360;


            return (MirDirection)(angle / 45F);
        }

        public void AddObject(MapObject ob)
        {
            Objects.Add(ob);

            if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].AddObject(ob);
        }

        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].RemoveObject(ob);
        }

        public bool CanAttack(MapObject ob)
        {
            if (ob == null || ob == User) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject) ob;

                    if (mob.MonsterInfo.AI < 0) return false;

                    break;
                default:
                    return false;
            }

            return !ob.Dead;
        }

        public void UpdateMapLocation()
        {
            if (User == null) return;

            GameScene.Game.MapControl.MapLocation = new Point((GameScene.Game.MapControl.MouseLocation.X - GameScene.Game.Location.X) / CellWidth - OffSetX + User.CurrentLocation.X,
                                                              (GameScene.Game.MapControl.MouseLocation.Y - GameScene.Game.Location.Y) / CellHeight - OffSetY + User.CurrentLocation.Y);
        }

        public bool HasTarget(Point loc)
        {
            if (loc.X < 0 || loc.Y < 0 || loc.X >= Width || loc.Y > Height) return false;

            Cell cell = Cells[loc.X, loc.Y];

            if (cell.Objects == null) return false;

            foreach (MapObject ob in cell.Objects)
                if (ob.Blocking) return true;

            return false;
        }

        public bool CanEnergyBlast(MirDirection direction)
        {
            return HasTarget(Functions.Move(MapObject.User.CurrentLocation, direction, 2));
        }

        public bool CanHalfMoon(MirDirection direction)
        {
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, -1)))) return true;
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, 1)))) return true;
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, 2)))) return true;

            return false;
        }

        public bool CanDestructiveSurge(MirDirection direction)
        {
            for (int i = 1; i < 8; i++)
                if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, i)))) return true;

            return false;
        }

        public bool ValidCell(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height) return false;

            return !Cells[location.X, location.Y].Flag;
        }
        #endregion
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _MapInfo = null;
                MapInfoChanged = null;

                _InstanceInfo = null;

                _Animation = 0;
                AnimationChanged = null;

                MapButtons = 0;
                MapLocation = Point.Empty;
                Mining = false;
                MiningPoint = Point.Empty;
                MiningDirection = 0;

                FishingState = FishingState.None;
                FloatLocation = Point.Empty;
                FishingDirection = 0;
                AutoCast = false;

                if (FLayer != null)
                {
                    if (!FLayer.IsDisposed)
                        FLayer.Dispose();

                    FLayer = null;
                }

                if (LLayer != null)
                {
                    if (!LLayer.IsDisposed)
                        LLayer.Dispose();

                    LLayer = null;
                }

                Cells = null;

                Width = 0;
                Height = 0;

                MapInfoObjects.Clear();
                MapInfoObjects = null;

                Objects.Clear();
                Objects = null;

                Effects.Clear();
                Effects = null;
                ViewRangeX = 0;
                ViewRangeY = 0;
                OffSetX = 0;
                OffSetY = 0;

                if (ParticleEffects != null)
                {
                    foreach (var effect in ParticleEffects)
                    {
                        effect.Dispose();
                    }

                    ParticleEffects.Clear();
                    ParticleEffects = null;
                }
            }
        }

        #endregion

        public sealed class Floor : DXControl
        {
            public Floor()
            {
                IsControl = false;
            }

            #region Methods
            public void CheckTexture()
            {
                if (!TextureValid)
                    CreateTexture();
            }
            
            protected override void OnClearTexture()
            {
                base.OnClearTexture();

                int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 4), maxX = Math.Min(GameScene.Game.MapControl.Width - 1, User.CurrentLocation.X + OffSetX + 4);
                int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 4), maxY = Math.Min(GameScene.Game.MapControl.Height - 1, User.CurrentLocation.Y + OffSetY + 4);

                for (int y = minY; y <= maxY; y++)
                {
                    if (y < 0) continue;
                    if (y >= GameScene.Game.MapControl.Height) break;

                    int drawY = (y - User.CurrentLocation.Y + OffSetY) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x < 0) continue;
                        if (x >= GameScene.Game.MapControl.Width) break;

                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                        Cell tile = GameScene.Game.MapControl.Cells[x, y];

                        if (y % 2 == 0 && x % 2 == 0)
                        {
                            MirLibrary library;
                            LibraryFile file;

                            if (!Libraries.KROrder.TryGetValue(tile.BackFile, out file)) continue;

                            if (!CEnvir.LibraryList.TryGetValue(file, out library)) continue;

                            library.Draw(tile.BackImage, drawX, drawY, Color.White, false, 1F, ImageType.Image);
                        }
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    int drawY = (y - User.CurrentLocation.Y + OffSetY + 1) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                    for (int x = minX; x <= maxX; x++)
                    {
                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                        Cell cell = GameScene.Game.MapControl.Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.MiddleImage - 1;

                            if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                                continue;//   index += GameScene.Game.MapControl.Animation % cell.MiddleAnimationFrame;

                            Size s = library.GetSize(index);

                            if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth*2 && s.Height == CellHeight*2))
                                library.Draw(index, drawX, drawY - CellHeight, Color.White, false, 1F, ImageType.Image);
                        }

                        
                        if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.FrontImage - 1;

                            if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                                continue;//  index += GameScene.Game.MapControl.Animation % cell.FrontAnimationFrame;

                                Size s = library.GetSize(index);

                            if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth*2 && s.Height == CellHeight*2))
                                library.Draw(index, drawX, drawY - CellHeight, Color.White, false, 1F, ImageType.Image);
                        }
                    }
                }
            }

            public override void Draw()
            {
            }
            protected override void DrawControl()
            {
            }

            #endregion
        }

        public sealed class Light : DXControl
        {
            public Light()
            {
                IsControl = false;
                BackColour = Color.FromArgb(15, 15, 15);
            }

            #region Methods

            public void CheckTexture()
            {
                CreateTexture();
            }

            protected override void OnClearTexture()
            {
                base.OnClearTexture();

                if (MapObject.User.Dead)
                {
                    DXManager.Device.Clear(ClearFlags.Target, Color.IndianRed, 0, 0);
                    return;
                }

                DXManager.SetBlend(true);
                DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);


                const float lightScale = 0.02F; //Players/Monsters
                const float baseSize = 0.1F;
                
                float fX;
                float fY;

                if ((MapObject.User.Poison & PoisonType.Abyss) == PoisonType.Abyss)
                {
                    DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);

                    float scale = baseSize + 4 * lightScale;

                    fX = (OffSetX + MapObject.User.CurrentLocation.X - User.CurrentLocation.X) * CellWidth  + CellWidth / 2;
                    fY = (OffSetY + MapObject.User.CurrentLocation.Y - User.CurrentLocation.Y) * CellHeight;

                    fX -= (DXManager.LightWidth * scale) / 2;
                    fY -= (DXManager.LightHeight * scale) / 2;

                    fX /= scale;
                    fY /= scale;

                    DXManager.Sprite.Transform = Matrix.Scaling(scale, scale, 1);

                    DXManager.Sprite.Draw(DXManager.LightTexture, Vector3.Zero, new Vector3(fX, fY, 0), Color.White);

                    DXManager.Sprite.Transform = Matrix.Identity;

                    DXManager.SetBlend(false);

                    var abyssEffects = MapObject.User.CreateMagicEffect(MagicEffect.Abyss);

                    foreach (var effect in abyssEffects)
                    {
                        effect.Draw();
                    }
                    return;
                }

                foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                {
                    if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                    {
                        float scale = baseSize + ob.Light * 2 * lightScale;

                        fX = (OffSetX + ob.CurrentLocation.X - User.CurrentLocation.X) * CellWidth + ob.MovingOffSet.X - User.MovingOffSet.X + CellWidth / 2;
                        fY = (OffSetY + ob.CurrentLocation.Y - User.CurrentLocation.Y) * CellHeight + ob.MovingOffSet.Y - User.MovingOffSet.Y;

                        fX -= (DXManager.LightWidth * scale) / 2;
                        fY -= (DXManager.LightHeight * scale) / 2;

                        fX /= scale;
                        fY /= scale;

                        DXManager.Sprite.Transform = Matrix.Scaling(scale, scale, 1);

                        DXManager.Sprite.Draw(DXManager.LightTexture, Vector3.Zero, new Vector3(fX, fY, 0), ob.LightColour);

                        DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }

                foreach (MirEffect ob in GameScene.Game.MapControl.Effects)
                {
                    float frameLight = ob.FrameLight;

                    if (frameLight > 0)
                    {
                        float scale = baseSize + frameLight * 2 * lightScale / 5;

                        fX = ob.DrawX + CellWidth / 2;
                        fY = ob.DrawY + CellHeight / 2;

                        fX -= (DXManager.LightWidth * scale) / 2;
                        fY -= (DXManager.LightHeight * scale) / 2;

                        fX /= scale;
                        fY /= scale;

                        DXManager.Sprite.Transform = Matrix.Scaling(scale, scale, 1);

                        DXManager.Sprite.Draw(DXManager.LightTexture, Vector3.Zero, new Vector3(fX, fY, 0), ob.FrameLightColour);

                        DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }

                int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 15), maxX = Math.Min(GameScene.Game.MapControl.Width - 1, User.CurrentLocation.X + OffSetX + 15);
                int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 15), maxY = Math.Min(GameScene.Game.MapControl.Height - 1, User.CurrentLocation.Y + OffSetY + 15);

                for (int y = minY; y <= maxY; y++)
                {
                    if (y < 0) continue;
                    if (y >= GameScene.Game.MapControl.Height) break;

                    int drawY = (y - User.CurrentLocation.Y + OffSetY) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x < 0) continue;
                        if (x >= GameScene.Game.MapControl.Width) break;

                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                        Cell tile = GameScene.Game.MapControl.Cells[x, y];

                        if (tile.Light == 0) continue;

                        float scale = baseSize + tile.Light * 30 * lightScale;

                        fX = drawX + CellWidth / 2;
                        fY = drawY  + CellHeight / 2;

                        fX -= DXManager.LightWidth * scale / 2;
                        fY -= DXManager.LightHeight * scale / 2;

                        fX /= scale;
                        fY /= scale;

                        DXManager.Sprite.Transform = Matrix.Scaling(scale, scale, 1);

                        DXManager.Sprite.Draw(DXManager.LightTexture, Vector3.Zero, new Vector3(fX, fY, 0), Color.White);

                        DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }

                DXManager.SetBlend(false);
            }

            public void UpdateLights()
            {
                switch (GameScene.Game.MapControl.MapInfo.Light)
                {
                    case LightSetting.Default:
                        byte shading = (byte) (255 * GameScene.Game.DayTime);
                        BackColour = Color.FromArgb(shading, shading, shading);
                        Visible = true;
                        break;
                    case LightSetting.Night:
                        BackColour = Color.FromArgb(15, 15, 15);
                        Visible = true;
                        break;
                    case LightSetting.Twilight:
                        BackColour = Color.FromArgb(100, 100, 100);
                        Visible = true;
                        break;
                    case LightSetting.Light:
                        BackColour = Color.FromArgb(255, 255, 255);
                        Visible = true;
                        break;
                }

                if (MapObject.User != null && (MapObject.User.Poison & PoisonType.Abyss) == PoisonType.Abyss)
                {
                    BackColour = Color.FromArgb(15, 15, 15);
                    Visible = false;
                }
            }
            protected override void DrawControl()
            {
            }

            public override void Draw()
            {
            }
            #endregion
        }
    }

    public sealed class Cell
    {
        public int BackFile;
        public int BackImage;

        public int MiddleFile;
        public int MiddleImage;

        public int FrontFile;
        public int FrontImage;

        public int FrontAnimationFrame;
        public int FrontAnimationTick;

        public int MiddleAnimationFrame;
        public int MiddleAnimationTick;

        public int Light;

        public bool Flag;

        public List<MapObject> Objects;

        public bool Blocking()
        {
            if (Objects != null)
            {
                foreach (MapObject ob in Objects)
                    if (ob.Blocking) return true;
            }

            return Flag;
        }

        public void AddObject(MapObject ob)
        {
            if (Objects == null)
                Objects = new List<MapObject>();

            if (ob.Race == ObjectType.Spell)
                Objects.Insert(0, ob);
            else
                Objects.Add(ob);

            ob.CurrentCell = this;
        }

        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            if (Objects.Count == 0)
                Objects = null;

            ob.CurrentCell = null;
        }
    }

}

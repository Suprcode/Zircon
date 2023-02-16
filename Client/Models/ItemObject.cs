using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Library;
using Library.SystemModels;
using  S = Library.Network.ServerPackets;

namespace Client.Models
{
    public sealed class ItemObject : MapObject
    {
        public override ObjectType Race  => ObjectType.Item;

        public DXLabel FocusLabel;

        public override bool Blocking => false;

        public ClientUserItem Item;
        public MirLibrary BodyLibrary;
        public Color LabelBackColour = Color.FromArgb(30, 0, 24, 48);

        public ItemObject(S.ObjectItem info)
        {
            ObjectID = info.ObjectID;

            Item = info.Item;

            ItemInfo itemInfo = info.Item.Info;

            if (info.Item.Info.ItemEffect == ItemEffect.ItemPart)
            {
                itemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

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

                        Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.DeepSkyBlue)
                        {
                            Target = this,
                            Loop = true,
                            Blend = true,
                            BlendRate = 0.5F,
                        });
                    }
                    else NameColour = Color.White;
                    break;
                case Rarity.Superior:
                    NameColour = Color.PaleGreen;
                    Effects.Add(new MirEffect(100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.PaleGreen)
                    {
                        Target = this,
                        Loop = true,
                        Blend = true,
                        BlendRate = 0.5F,
                    });
                    break;
                case Rarity.Elite:
                    NameColour = Color.MediumPurple;
                    Effects.Add(new MirEffect(120, 10, TimeSpan.FromMilliseconds(100), LibraryFile.ProgUse, 60, 60, Color.MediumPurple)
                    {
                        Target = this,
                        Loop = true,
                        Blend = true,
                        BlendRate = 0.5F,
                    });
                    break;
            }
            CurrentLocation = info.Location;


            UpdateLibraries();

            SetFrame(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);
        }
        public void UpdateLibraries()
        {
            Frames = FrameSet.DefaultItem;

            CEnvir.LibraryList.TryGetValue(LibraryFile.Ground, out BodyLibrary);
        }

        public override void SetAnimation(ObjectAction action)
        {

            CurrentAnimation = MirAnimation.Standing;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }

        public override void Draw()
        {
            if (BodyLibrary == null) return;

            int drawIndex;

            if (CEnvir.IsCurrencyItem(Item.Info))
            {
                drawIndex = CEnvir.CurrencyImage(Item.Info, Item.Count);
            }
            else
            {
                ItemInfo info = Item.Info;

                if (info.ItemEffect == ItemEffect.ItemPart)
                    info = Globals.ItemInfoList.Binding.First(x => x.Index == Item.AddedStats[Stat.ItemIndex]);

                drawIndex = info.Image;
            }

            Size size = BodyLibrary.GetSize(drawIndex);

            BodyLibrary.Draw(drawIndex, DrawX + (CellWidth - size.Width)/2, DrawY + (CellHeight - size.Height)/2, DrawColour, false, 1F, ImageType.Image);

        }

        public override bool MouseOver(Point p)
        {
            return false;
        }

        public override void NameChanged()
        {
            base.NameChanged();

            if (string.IsNullOrEmpty(Name))
            {
                FocusLabel = null;
            }
            else
            {
                if (!NameLabels.TryGetValue(Name, out List<DXLabel> focused))
                    NameLabels[Name] = focused = new List<DXLabel>();

                FocusLabel = focused.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == LabelBackColour);

                if (FocusLabel != null) return;

                FocusLabel = new DXLabel
                {
                    BackColour = LabelBackColour,
                    ForeColour = NameColour,
                    Outline = true,
                    OutlineColour = Color.Black,
                    Text = Name,
                    Border = true,
                    BorderColour = Color.Black,
                    IsVisible = true,
                };

                FocusLabel.Disposing += (o, e) => focused.Remove(FocusLabel);
                focused.Add(FocusLabel);
            }
        }

        public void DrawFocus(int layer)
        {
            FocusLabel.Location = new Point(DrawX + (48 - FocusLabel.Size.Width) / 2, DrawY - (32 - FocusLabel.Size.Height / 2) + 8 - layer * 16);
            FocusLabel.Draw();
        }
    }

}

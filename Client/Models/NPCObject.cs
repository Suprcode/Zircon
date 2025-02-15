using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Client.Scenes.Views;
using Library;
using Library.SystemModels;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    public sealed class NPCObject : MapObject
    {
        public static Dictionary<NPCInfo, NPCObject> NPCs = new Dictionary<NPCInfo, NPCObject>();

        public override ObjectType Race => ObjectType.NPC;

        public CurrentQuest CurrentQuest;
        public MirEffect QuestEffect;

        public NPCInfo NPCInfo;

        public MirLibrary BodyLibrary;
        public int BodyOffSet = 100;
        public int BodyShape;
        public int BodyFrame => DrawFrame + BodyShape * BodyOffSet;


        public NPCObject(S.ObjectNPC info)
        {
            ObjectID = info.ObjectID;

            NPCInfo = Globals.NPCInfoList.Binding.First(x => x.Index == info.NPCIndex);

            Light = 10;

            var splitName = NPCInfo.NPCName.Split('_');

            if (splitName.Length > 1)
            {
                Title = splitName[0];
                Name = splitName[1];
            }
            else
            {
                Name = splitName[0];
            }

            NameColour = Color.Lime;
            BodyShape = NPCInfo.Image;

            CurrentLocation = info.CurrentLocation;

            Direction = info.Direction;

            NPCs[NPCInfo] = this;

            CEnvir.LibraryList.TryGetValue(LibraryFile.NPC, out BodyLibrary);
            Frames = NPCInfo.Image switch
            {
                64 or 65 or 91 or 92 or 93 or 157 or 158 or 160 or 165 or 166 or 168 or 208 or 209 or 210 or 211 or 212 or 213 or 214 or 231 or 234 => new Dictionary<MirAnimation, Frame>
                {
                    [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromHours(1))
                },
                56 or 57 => new Dictionary<MirAnimation, Frame>
                {
                    [MirAnimation.Standing] = new Frame(0, 12, 0, TimeSpan.FromMilliseconds(200))
                },
                156 => new Dictionary<MirAnimation, Frame>
                {
                    [MirAnimation.Standing] = new Frame(0, 16, 0, TimeSpan.FromMilliseconds(200))
                },
                _ => FrameSet.DefaultNPC,
            };
            SetFrame(new ObjectAction(MirAction.Standing, MirDirection.Up, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);

            UpdateQuests();
        }
        
        public override void SetAnimation(ObjectAction action)
        {
            CurrentAnimation = MirAnimation.Standing;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }

        public override void NameChanged()
        {
            if (string.IsNullOrEmpty(Name))
            {
                NameLabel = null;
            }
            else
            {
                if (!NameLabels.TryGetValue(Name, out List<DXLabel> names))
                    NameLabels[Name] = names = new List<DXLabel>();

                NameLabel = names.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == Color.Empty);

                if (NameLabel == null)
                {
                    NameLabel = new DXLabel
                    {
                        BackColour = Color.Empty,
                        Outline = true,
                        OutlineColour = Color.Black,
                        Text = Name,
                        IsControl = false,
                        IsVisible = true,
                    };

                    NameLabel.Disposing += (o, e) => names.Remove(NameLabel);
                    names.Add(NameLabel);
                }

                NameLabel.ForeColour = string.IsNullOrEmpty(Title) ? NameColour : Color.White;
            }

            if (string.IsNullOrEmpty(Title))
            {
                TitleNameLabel = null;
            }
            else
            {
                string title = Title;

                if (!NameLabels.TryGetValue(title, out List<DXLabel> titles))
                    NameLabels[title] = titles = new List<DXLabel>();

                TitleNameLabel = titles.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == Color.Empty);

                if (TitleNameLabel == null)
                {
                    TitleNameLabel = new DXLabel
                    {
                        BackColour = Color.Empty,
                        Outline = true,
                        OutlineColour = Color.Black,
                        Text = title,
                        IsControl = false,
                        IsVisible = true,
                    };

                    TitleNameLabel.Disposing += (o, e) => titles.Remove(TitleNameLabel);
                    titles.Add(TitleNameLabel);
                }

                TitleNameLabel.ForeColour = NameColour;
            }
        }

        public override void Draw()
        {
            if (BodyLibrary == null) return;

            DrawShadow();

            DrawBody();
        }

        private void DrawShadow()
        {
            BodyLibrary.Draw(BodyFrame, DrawX, DrawY, Color.White, true, 0.5f, ImageType.Shadow);
        }
        private void DrawBody()
        {
            BodyLibrary.Draw(BodyFrame, DrawX, DrawY, DrawColour, true, 1F, ImageType.Image);
        }

        public override void DrawBlend()
        {
            if (BodyLibrary == null) return;
            
            DXManager.SetBlend(true, 0.20F, BlendMode.HIGHLIGHT);//0.60F
            DrawBody();
            DXManager.SetBlend(false);
        }

        public override bool MouseOver(Point p)
        {
            return BodyLibrary != null && BodyLibrary.VisiblePixel(BodyFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true);
        }


        public override void UpdateQuests()
        {
            if (NPCInfo.CurrentQuest == null)
            {
                RemoveQuestEffect();
                return;
            }

            if (CurrentQuest == NPCInfo.CurrentQuest) return;

            RemoveQuestEffect();

            CurrentQuest = NPCInfo.CurrentQuest;

            int startIndex = 0;

            switch (CurrentQuest.Type)
            {
                case QuestType.General:
                    startIndex = 10;
                    break;
                case QuestType.Daily:
                    startIndex = 70;
                    break;
                case QuestType.Weekly:
                    startIndex = 70;
                    break;
                case QuestType.Repeatable:
                    startIndex = 10;
                    break;
                case QuestType.Story:
                    startIndex = 50;
                    break;
                case QuestType.Account:
                    startIndex = 30;
                    break;
            }

            switch (CurrentQuest.Icon)
            {
                case QuestIcon.New:
                    startIndex += 0;
                    break;
                case QuestIcon.Incomplete:
                    startIndex = 0;
                    break;
                case QuestIcon.Complete:
                    startIndex += 2;
                    break;
            }

            QuestEffect = new MirEffect(startIndex, 2, TimeSpan.FromMilliseconds(500), LibraryFile.QuestIcon, 0, 0, Color.Empty)
            {
                Loop = true,
                MapTarget = CurrentLocation,
                Blend = false,
                DrawType = DrawType.Final,
                AdditionalOffSet = new Point(0, -80)
            };
            QuestEffect.Process();

        }

        public void RemoveQuestEffect()
        {
            CurrentQuest = null;
            QuestEffect?.Remove();
            QuestEffect = null;
        }

        public override void Remove()
        {
            base.Remove();

            RemoveQuestEffect();
            NPCs.Remove(NPCInfo);
        }
    }
}


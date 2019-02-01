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

        public QuestIcon CurrentIcon;
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

            Name = NPCInfo.NPCName;
            NameColour = Color.Lime;
            BodyShape = NPCInfo.Image;

            CurrentLocation = info.CurrentLocation;

            Direction = info.Direction;

            NPCs[NPCInfo] = this;

            CEnvir.LibraryList.TryGetValue(LibraryFile.NPC, out BodyLibrary);
            switch (NPCInfo.Image)
            {
                case 93:
                    Frames = new Dictionary<MirAnimation, Frame>
                    {
                        [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromHours(1))
                    };
                    break;
                case 56:
                case 57:
                    Frames = new Dictionary<MirAnimation, Frame>
                    {
                        [MirAnimation.Standing] = new Frame(0, 12, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                case 156:
                    Frames = new Dictionary<MirAnimation, Frame>
                    {
                        [MirAnimation.Standing] = new Frame(0, 16, 0, TimeSpan.FromMilliseconds(200))
                    };
                    break;
                default:
                    Frames = FrameSet.DefaultNPC;
                    break;

            }


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
            
            DXManager.SetBlend(true, 0.60F);
            DrawBody();
            DXManager.SetBlend(false);
        }

        public override bool MouseOver(Point p)
        {
            return BodyLibrary != null && BodyLibrary.VisiblePixel(BodyFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true);
        }


        public override void UpdateQuests()
        {
            if (NPCInfo.CurrentIcon == QuestIcon.None)
            {
                RemoveQuestEffect();
                return;
            }

            if (CurrentIcon == NPCInfo.CurrentIcon) return;

            RemoveQuestEffect();

            CurrentIcon = NPCInfo.CurrentIcon;

            int startIndex = 0;

            if ((CurrentIcon & QuestIcon.QuestComplete) == QuestIcon.QuestComplete)
                startIndex = 1130;
            else if ((CurrentIcon & QuestIcon.NewQuest) == QuestIcon.NewQuest)
                startIndex = 1080;
            else if ((CurrentIcon & QuestIcon.QuestIncomplete) == QuestIcon.QuestIncomplete)
                startIndex = 1090;
            
            QuestEffect = new MirEffect(startIndex, 2, TimeSpan.FromMilliseconds(500), LibraryFile.GameInter, 0, 0, Color.Empty)
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
            CurrentIcon = QuestIcon.None;
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


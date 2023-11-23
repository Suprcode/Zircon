using Client.Envir;
using Client.Scenes;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using Frame = Library.Frame;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    public sealed class SpellObject : MapObject
    {
        public override ObjectType Race => ObjectType.Spell;

        public override bool Blocking => false;

        public SpellEffect Effect;

        public MirLibrary BodyLibrary;
        public bool Blended;
        public float BlendRate = 1f;

        public int Power;

        public SpellObject(S.ObjectSpell info)
        {
            ObjectID = info.ObjectID;
            Direction = info.Direction;

            CurrentLocation = info.Location;

            Effect = info.Effect;
            Power = info.Power;

            UpdateLibraries();
            
            SetFrame(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));
            switch (Effect)
            {
                case SpellEffect.FireWall:
                case SpellEffect.MonsterDeathCloud:
                    FrameStart -= TimeSpan.FromMilliseconds(CEnvir.Random.Next(300));
                    break;
                case SpellEffect.Tempest:
                    FrameStart -= TimeSpan.FromMilliseconds(CEnvir.Random.Next(1350));
                    break;

            }


            GameScene.Game.MapControl.AddObject(this);
        }

        public void UpdateLibraries()
        {
            Frames = new Dictionary<MirAnimation, Frame>();
            switch (Effect)
            {
                case SpellEffect.SafeZone:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(649, 1, 0, TimeSpan.FromDays(365));
                    Blended = true;
                    BlendRate =0.3f;
                    break;
                case SpellEffect.FireWall:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(920, 3, 0, TimeSpan.FromMilliseconds(150));
                    Blended = true;
                    LightColour = Globals.FireColour;
                    BlendRate = 0.55f;
                    Light = 15;
                    break;
                case SpellEffect.Tempest:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx2, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(920, 10, 0, TimeSpan.FromMilliseconds(150));
                    Blended = true;
                    LightColour = Globals.WindColour;
                    BlendRate = 0.55f;
                    Light = 15;
                    break;
                case SpellEffect.TrapOctagon:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.Magic, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(640, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    BlendRate = 0.8f;
                    break;
                case SpellEffect.PoisonousCloud:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx4, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(400, 15, 0, TimeSpan.FromMilliseconds(100));
                    DefaultColour = Color.SaddleBrown;
                    Blended = true;
                    Light = 0;
                    DXSoundManager.Play(SoundIndex.PoisonousCloudStart);
                    break;
                case SpellEffect.DarkSoulPrison:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx6, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(700, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    Light = 0;
                    DXSoundManager.Play(SoundIndex.DarkSoulPrison);
                    break;
                case SpellEffect.BurningFire:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MagicEx6, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(1000, 8, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    LightColour = Globals.FireColour;
                    BlendRate = 1F;
                    Light = 15;
                    break;
                case SpellEffect.MonsterDeathCloud:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.MonMagicEx2, out BodyLibrary);
                    Frames[MirAnimation.Standing] = new Frame(850, 10, 0, TimeSpan.FromMilliseconds(100));
                    Blended = true;
                    Light = 0;
                    BlendRate = 1F;
                    DXSoundManager.Play(SoundIndex.JinchonDevilAttack3);
                    break;
                case SpellEffect.Rubble:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out BodyLibrary);
                    int index ;

                    if (Power > 20)
                        index = 234;
                    else if (Power > 15)
                        index = 233;
                    else if (Power > 10)
                        index = 232;
                    else if (Power > 5)
                        index = 231;
                    else 
                        index = 230;

                    Frames[MirAnimation.Standing] = new Frame(index, 1, 0, TimeSpan.FromMilliseconds(100));

                    Light = 0;
                    break;
            }

        }

        public override void SetAnimation(ObjectAction action)
        {
            CurrentAnimation = MirAnimation.Standing;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
        }

        public override void Draw()
        {
            if (Blended)
                BodyLibrary.DrawBlend(DrawFrame, DrawX, DrawY, DrawColour, true, BlendRate, ImageType.Image);
            else 
                BodyLibrary.Draw(DrawFrame, DrawX, DrawY, DrawColour, true, 1F, ImageType.Image);
        }

        public override bool MouseOver(Point p)
        {
            return false;
        }
    }
}

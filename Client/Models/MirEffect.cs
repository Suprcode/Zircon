using Client.Envir;
using Client.Models.Particles;
using Client.Scenes;
using Library;
using System;
using System.Drawing;

namespace Client.Models
{
    public class MirEffect
    {
        public MapObject Target;
        public Point MapTarget;

        public MirLibrary Library;

        public DateTime StartTime;
        public int StartIndex;
        public int FrameCount;
        public TimeSpan[] Delays;

        public int FrameIndex
        {
            get { return _FrameIndex; }
            set
            {
                if (_FrameIndex == value) return;

                _FrameIndex = value;
                FrameIndexAction?.Invoke();
            }
        }
        private int _FrameIndex;

        public Color DrawColour = Color.White;
        public bool Blend;
        public bool Reversed;
        public float Opacity = 1F;
        public float BlendRate = 0.7F;
        public bool UseOffSet = true;
        public bool Loop = false;

        public ParticleEmitter _particleEmitter;

        public int DrawX
        {
            get { return _DrawX; }
            set
            {
                if (_DrawX == value) return;

                _DrawX = value;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        private int _DrawX;

        public int DrawY
        {
            get { return _DrawY; }
            set
            {
                if (_DrawY == value) return;
                
                _DrawY = value;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        private int _DrawY;

        public int DrawFrame
        {
            get { return _DrawFrame; }
            set
            {
                if (_DrawFrame == value) return;
                
                _DrawFrame = value;
                GameScene.Game.MapControl.TextureValid = false;
                FrameAction?.Invoke();
            }
        }
        private int _DrawFrame;

        public DrawType DrawType = DrawType.Object;



        public int Skip { get; set; }
        public MirDirection Direction { get; set; }
        
        public Color[] LightColours;
        public int StartLight, EndLight;

        public float FrameLight 
        {
            get
            {
                if (CEnvir.Now < StartTime) return 0;

                TimeSpan elapsed = CEnvir.Now - StartTime;

                if (Loop)
                    elapsed = TimeSpan.FromTicks(elapsed.Ticks % TotalDuration.Ticks);

                return StartLight + (EndLight - StartLight) * elapsed.Ticks / TotalDuration.Ticks;
            }
        }
        public Color FrameLightColour => LightColours[FrameIndex];
        public Point CurrentLocation => Target?.CurrentLocation ?? MapTarget;
        public Point MovingOffSet => Target?.MovingOffSet ?? Point.Empty;

        public Action CompleteAction;
        public Action FrameAction;
        public Action FrameIndexAction;

        public Point AdditionalOffSet;

        public TimeSpan TotalDuration
        {
            get
            {
                TimeSpan temp = TimeSpan.Zero;

                foreach (TimeSpan delay in Delays)
                    temp += delay;

                return temp;
            }
        }
        
        public MirEffect(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startLight, int endLight, Color lightColour)
        {
            StartIndex = startIndex;
            FrameCount = frameCount;
            Skip = 10;

            StartTime = CEnvir.Now;
            StartLight = startLight;
            EndLight = endLight;

            Delays = new TimeSpan[FrameCount];
            LightColours = new Color[FrameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Delays[i] = frameDelay;
                //Light[i] = startLight + (endLight - startLight)/frameCount*i;
                LightColours[i] = lightColour;
            }

            CEnvir.LibraryList.TryGetValue(file, out Library);

            GameScene.Game.MapControl.Effects.Add(this);
        }

        public virtual void Process()
        {
            if (CEnvir.Now < StartTime) return;
            
            
            if (Target != null)
            {
                DrawX = Target.DrawX + AdditionalOffSet.X;
                DrawY = Target.DrawY + AdditionalOffSet.Y;
            }
            else
            {
                DrawX = (MapTarget.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X + AdditionalOffSet.X;
                DrawY = (MapTarget.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y + AdditionalOffSet.Y;
            }

            int frame = GetFrame();

            if (frame == FrameCount)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            if (Reversed)
                frame = FrameCount - frame - 1;

            FrameIndex = frame;
            DrawFrame = FrameIndex + StartIndex + (int)Direction * Skip;
        }
    
        protected virtual int GetFrame()
        {
            TimeSpan elapsed = CEnvir.Now - StartTime;

            if (Loop)
                elapsed = TimeSpan.FromTicks(elapsed.Ticks % TotalDuration.Ticks);

            if (Reversed)
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    elapsed -= Delays[Delays.Length - 1 - i];
                    if (elapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }
            else
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    elapsed -= Delays[i];
                    if (elapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }

            return FrameCount;
        }

        public virtual void Draw()
        {
            if (CEnvir.Now < StartTime || Library == null) return;
            
            if (Blend)
                Library.DrawBlend(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, BlendRate, ImageType.Image);
            else
                Library.Draw(DrawFrame, DrawX, DrawY, DrawColour, UseOffSet, Opacity, ImageType.Image);
        }

        public virtual void Remove()
        {
            CompleteAction = null;
            FrameAction = null;
            FrameIndexAction = null;
            GameScene.Game.MapControl.Effects.Remove(this);
            Target?.Effects.Remove(this);
        }
    }

    public enum DrawType
    {
        Floor,
        Object,
        Final
    }
}

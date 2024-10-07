using Client.Envir;
using Client.Models.Particles;
using Client.Scenes;
using Library;
using System;
using System.Drawing;

namespace Client.Models
{
    public  class MirProjectile : MirEffect
   {
        public Point Origin { get; set; }
        public int Speed { get; set; }
        public bool Explode { get; set; }
        public int Delay { get; set; }
        public int Direction16 { get; set; }
        public bool Has16Directions { get; set; }

        public MirProjectile(int startIndex, int frameCount, TimeSpan frameDelay, LibraryFile file, int startlight, int endlight, Color lightColour, Point origin, Type particleEmitter = null) : base(startIndex, frameCount, frameDelay, file, startlight, endlight, lightColour)
        {
            Has16Directions = true;

            Origin = origin;
            Speed = 50;
            Explode = false;

            if (Config.DrawParticles && particleEmitter != null)
            {
                _particleEmitter = (ParticleEmitter)Activator.CreateInstance(particleEmitter, this);

                GameScene.Game.MapControl.ParticleEffects.Add(_particleEmitter);
            }
        }

        public override void Process()
        {
            Point location = Target?.CurrentLocation ?? MapTarget;

            if (location == Origin)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            int x = (Origin.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
            int y = (Origin.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

            int x1 = (location.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth - MapObject.User.MovingOffSet.X;
            int y1 = (location.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight - MapObject.User.MovingOffSet.Y;

            Direction16 = Functions.Direction16(new Point(x, y / 32 * 48), new Point(x1, y1 / 32 * 48));
            long duration = Functions.Distance(new Point(x, y / 32 * 48), new Point(x1, y1 / 32 * 48)) * TimeSpan.TicksPerMillisecond;

            if (Delay > 0)
                duration *= Delay;

            if (!Has16Directions)
                Direction16 /= 2;

            if (duration == 0)
            {
                CompleteAction?.Invoke();
                Remove();
                return;
            }

            int x2 = x1 - x;
            int y2 = y1 - y;

            if (x2 == 0) x2 = 1;
            if (y2 == 0) y2 = 1;

            TimeSpan time = CEnvir.Now - StartTime;

            int frame = GetFrame();

            if (Reversed)
                frame = FrameCount - frame - 1;

            DrawFrame = frame + StartIndex + Direction16 * Skip;

            DrawX = x + (int)(time.Ticks / (duration / x2)) + AdditionalOffSet.X;
            DrawY = y + (int)(time.Ticks / (duration / y2)) + AdditionalOffSet.Y;

            if (_particleEmitter != null)
            {
                Point offset = Library.GetOffSet(DrawFrame);

                _particleEmitter.SetLocation(Direction16, DrawX + offset.X, DrawY + offset.Y);
            }

            if ((CEnvir.Now - StartTime).Ticks > duration)
            {
                if (Target == null && !Explode)
                {
                    Size s = Library.GetSize(FrameIndex);

                    if (DrawX + s.Width > 0 && DrawX < GameScene.Game.Size.Width &&
                        DrawY + s.Height > 0 && DrawY < GameScene.Game.Size.Height) return;
                }

                CompleteAction?.Invoke();
                Remove();
                return;
            }
        }

        protected override int GetFrame()
        {
            TimeSpan enlapsed = CEnvir.Now - StartTime;

            enlapsed = TimeSpan.FromTicks(enlapsed.Ticks%TotalDuration.Ticks);

            for (int i = 0; i < Delays.Length; i++)
            {
                enlapsed -= Delays[i];
                if (enlapsed >= TimeSpan.Zero) continue;

                return i;
            }

            return FrameCount;
        }

        public override void Remove()
        {
            _particleEmitter?.StopGeneration();
            base.Remove();
        }
    }
}

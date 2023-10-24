using Client.Envir;
using Client.Scenes;
using Library;
using SlimDX;
using SlimDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.Models.Particles.Rain;

namespace Client.Models.Particles
{
    public abstract class ParticleEmitter : IDisposable
    {
        protected Point[] CenterPoint;

        public virtual TimeSpan StartDelay { get => TimeSpan.FromMilliseconds(0); }
        private readonly DateTime _startTime;

        public Vector2 Location { get; set; }
        public float Angle { get; set; }
        public int Direction16 { get; set; }

        public List<ParticleType> ParticleTypes;

        private MirEffect _owner;

        private bool _stopGeneration;

        public bool NoParticles
        {
            get { return ParticleTypes.All(x => x.Particles.Count == 0); }
        }

        public ParticleEmitter(MirEffect owner)
        {
            _owner = owner;
            _startTime = CEnvir.Now.Add(StartDelay);
        }

        public ParticleEmitter(Point location)
        {
            _startTime = CEnvir.Now.Add(StartDelay);

            Location = new Vector2(location.X, location.Y);
        }

        public void SetLocation(int direction16, int x, int y)
        {
            var center = CenterPoint[direction16];

            Angle = (float)Functions.DegreesToRadians(direction16 * 22.5);

            Direction16 = direction16;

            Location = new Vector2(center.X + x, center.Y + y);
        }

        public void Process()
        {
            if (Location.X == 0 && Location.Y == 0) return;

            if (_startTime > CEnvir.Now) return;

            if (!_stopGeneration)
            {
                foreach (var particle in ParticleTypes)
                {
                    if (particle.Particles.Count < particle.MaxCount && particle.NextSpawn < CEnvir.Now)
                    {
                        particle.Particles.Add(particle.CreateParticle(Location, Direction16, Angle));
                        particle.NextSpawn = CEnvir.Now.Add(particle.SpawnFrequency);
                    }
                }
            }

            foreach (var type in ParticleTypes)
            {
                for (int i = 0; i < type.Particles.Count; i++)
                {
                    var particle = type.Particles[i];

                    if (particle.Update())
                    {
                        type.Updated(this, particle);
                    }

                    if (particle.Remove)
                    {
                        type.Completed(this, particle);

                        if (particle.Remove)
                        {
                            if (!particle.IsDisposed)
                                particle.Dispose();

                            type.Particles.RemoveAt(i);
                            i--;
                        }
                    }
                }

                if (NoParticles && _stopGeneration)
                {
                    Remove();
                }
            }

        }

        public void Draw()
        {
            if (_startTime > CEnvir.Now) return;

            foreach (var types in ParticleTypes)
            {
                for (int i = types.Particles.Count - 1; i >= 0; i--)
                {
                    var p = types.Particles[i];

                    Size size = p.Library.GetSize(p.TextureIndex);

                    var centerX = ((size.Width) / 2) * p.Scale;
                    var centerY = ((size.Height) / 2) * p.Scale;

                    if (p.Position.X - centerX > Config.GameSize.Width || p.Position.Y - centerY > Config.GameSize.Height)
                    {
                        types.Particles.Remove(p);
                        continue;
                    }

                    p.Draw();
                }
            }
        }

        public void StopGeneration()
        {
            _stopGeneration = true;
        }

        public void Remove()
        {
            GameScene.Game.MapControl.ParticleEffects.Remove(this);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                CenterPoint = null;
                _owner = null;

                if (ParticleTypes != null)
                {
                    foreach (var particle in ParticleTypes)
                    {
                        if (!particle.IsDisposed)
                            particle.Dispose();
                    }

                    ParticleTypes = null;
                }

                IsDisposed = true;
            }
        }

        #endregion
    }
}

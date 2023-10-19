using Client.Envir;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models.Particles
{
    public abstract class ParticleType : IDisposable
    {
        public MirLibrary Library;

        public List<Particle> Particles = new();

        public int MaxCount { get; set; }
        public virtual TimeSpan SpawnFrequency { get; set; }
        public DateTime NextSpawn { get; set; }

        public Color Color;

        public List<int> Textures;
        public Random random = new();

        public abstract Particle CreateParticle(Vector2 emitterLocation, int direction, float angle);

        public virtual void Updated(ParticleEmitter emitter, Particle updateParticle)
        {
        }

        public virtual void Completed(ParticleEmitter emitter, Particle completeParticle)
        {
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
                IsDisposed = true;

                Library = null;

                foreach (var particle in Particles)
                {
                    if (!particle.IsDisposed)
                        particle.Dispose();
                }

                Particles = null;
                Textures = null;

                IsDisposed = true;
            }
        }

        #endregion
    }
}

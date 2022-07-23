using Client.Envir;
using Client.Scenes;
using Library;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.Particles
{
    public abstract class ParticleEmitter
    {
        protected Point[] CenterPoint;

        public virtual TimeSpan StartDelay { get => TimeSpan.FromMilliseconds(0); }
        private readonly DateTime _startTime;

        public Vector2 Location { get; set; }
        public float Angle { get; set; }
        public int Direction { get; set; }

        public List<ParticleType> ParticleTypes;

        private readonly MirEffect _owner;

        private bool _stopGeneration;

        public bool NoParticles
        {
            get { return ParticleTypes.All(x => x.Particles.Count == 0); }
        }

        public ParticleEmitter(MirEffect owner)
        {
            _owner = owner;
            _startTime = CEnvir.Now.Add(StartDelay);

            _owner.CompleteAction += () =>
            {
                _stopGeneration = true;
            };
        }

        public ParticleEmitter(Point location)
        {
            _startTime = CEnvir.Now.Add(StartDelay);

            Location = new Vector2(location.X, location.Y);
        }

        public void SetLocation(int dir, int x, int y)
        {
            var center = CenterPoint[dir];

            Angle = (float)Functions.DegreesToRadians(dir * 22.5);

            Direction = dir;

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
                    if (particle.Particles.Count <= particle.MaxCount && particle.NextSpawn < CEnvir.Now)
                    {
                        particle.Particles.Add(particle.CreateParticle(Location, Direction, Angle));
                        particle.NextSpawn = CEnvir.Now.Add(particle.SpawnFrequency);
                    }
                }
            }

            foreach (var particle in ParticleTypes)
            {
                for (int i = 0; i < particle.Particles.Count; i++)
                {
                    particle.Particles[i].Update();

                    if (particle.Particles[i].Opacity <= 0F)
                    {
                        particle.Particles.RemoveAt(i);
                        i--;
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

            foreach (var particle in ParticleTypes)
            {
                for (int i = 0; i < particle.Particles.Count; i++)
                {
                    particle.Particles[i].Draw();
                }
            }
        }

        public void Remove()
        {
            GameScene.Game.MapControl.ParticleEffects.Remove(this);
        }
    }

}

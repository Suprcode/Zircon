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
    public class Fog : ParticleEmitter
    {
        public class FogParticle : ParticleType
        {
            private Vector2 _velocity;

            public FogParticle()
            {
                _velocity = new(1f, 0f);

                MaxCount = 4;
                SpawnFrequency = TimeSpan.FromMilliseconds(5000);
                Textures = new List<int> { 550 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                Vector2 position = emitterLocation;

                float opacity = 0.3f;

                float startingAngle = angle;
                float angularVelocity = 0f;

                float scale = 3f;
                float scaleRate = 0f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(15000, 15000));

                bool fade = false;
                float fadeRate = 0.2F;

                return new Particle(Library, texture, opacity, position, _velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }
        }

        public Fog(Point location) : base(location)
        {
            ParticleTypes = new List<ParticleType>
            {
                new FogParticle()
            };
        }
    }
}

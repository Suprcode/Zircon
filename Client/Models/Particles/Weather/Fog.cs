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
                _velocity = new Vector2(1F, 0f);

                MaxCount = 2;
                SpawnFrequency = TimeSpan.FromMilliseconds(0);
                Textures = new List<int> { 550 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];

            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                float scale = 4F;
                float scaleRate = 0F;

                Vector2 position = emitterLocation;

                if (Particles.Count > 0)
                {
                    var particle = Particles.Last();

                    var size = particle.Library.GetSize(particle.TextureIndex);

                    position = new Vector2(particle.Position.X - ((size.Width) * scale), particle.Position.Y);

                    //if (position.X + (size.Width * scale) < particle.Position.X)
                    //{
                    //    position.X -= 1F;
                    //}
                    //else
                    //{
                    //    position.X += 1F;
                    //}
                }

                float opacity = 0.1F;

                float startingAngle = angle;
                float angularVelocity = 0F;

                TimeSpan ttl = TimeSpan.FromHours(1);

                bool fade = false;
                float fadeRate = 0F;

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

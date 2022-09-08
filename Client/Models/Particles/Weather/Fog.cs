using Client.Envir;
using Client.Scenes;
using Library;
using SlimDX;
using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Client.Models.Particles
{
    public class Fog : ParticleEmitter
    {
        public class FogParticle : ParticleType
        {
            public FogParticle()
            {
                MaxCount = 4;
                SpawnFrequency = TimeSpan.FromMilliseconds(0);
                Textures = new List<int> { 550 };
                Color = Color.DarkGray;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                var size = Library.GetSize(texture);

                float scale = 4F;
                float scaleRate = 0F;

                Vector2 position = emitterLocation;
                Vector2 velocity = new (1F, 0f);

                //Position of next particle is left of the current one to create a repeating image
                if (Particles.Count > 0)
                {
                    var particle = Particles.Last();

                    position = new Vector2(particle.Position.X - (size.Width * scale), particle.Position.Y);
                }

                float opacity = 1F;

                float startingAngle = angle;
                float angularVelocity = 0F;

                TimeSpan ttl = TimeSpan.FromHours(1);

                bool fade = false;
                float fadeRate = 0F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }

            public override void Updated(ParticleEmitter emitter, Particle updateParticle)
            {
                //The next particle might be spawned slightly out of sync, so we need to update it to the correct position on first update
                if (updateParticle.UpdateCount == 1)
                {
                    foreach (var type in emitter.ParticleTypes)
                    {
                        var particles = type.Particles;

                        if (particles.Count < 2)
                            continue;

                        var position = particles[0].Position;

                        for (int i = 1; i < particles.Count; i++)
                        {
                            var p = particles[i];

                            var size = Library.GetSize(p.TextureIndex);
                            var scale = p.Scale;

                            p.Position = new Vector2(position.X - (size.Width * scale), position.Y);

                            position = p.Position;
                        }
                    }
                }
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

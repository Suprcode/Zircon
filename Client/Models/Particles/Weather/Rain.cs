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
using static Client.Models.Particles.Fog;

namespace Client.Models.Particles
{
    public class Rain : ParticleEmitter
    {
        public class RainParticle : ParticleType
        {
            public RainParticle()
            {
                MaxCount = 1000;
                SpawnFrequency = TimeSpan.FromMilliseconds(20);
                Textures = new List<int> { 509 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                float scale = 2F;
                float scaleRate = 0F;

                Vector2 position = new (random.Next(GameScene.Game.Size.Width), 0);
                Vector2 velocity = new (-1, 5);

                float opacity = 1F;

                float startingAngle = 0.4F;
                float angularVelocity = 0F;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(500, 2000));

                bool fade = false;
                float fadeRate = 0F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }
            public override void Update(ParticleEmitter emitter, Particle updateParticle)
            {
                base.Update(emitter, updateParticle);

                if (updateParticle.Velocity == Vector2.Zero)
                {
                    updateParticle.UpdateSpeed = TimeSpan.FromMilliseconds(100);
                    updateParticle.Remove = false;

                    updateParticle.TextureIndex++;

                    if (updateParticle.TextureIndex > 514)
                    {
                        updateParticle.Remove = true;
                    }
                }
            }

            public override void Complete(ParticleEmitter emitter, Particle completeParticle)
            {
                base.Complete(emitter, completeParticle);

                if (completeParticle.Velocity != Vector2.Zero)
                {
                    completeParticle.Velocity = Vector2.Zero;
                    completeParticle.Remove = false;
                }
            }
        }

        public Rain(Point location) : base(location)
        {
            ParticleTypes = new List<ParticleType>
            {
                new RainParticle()
            };
        }
    }
}

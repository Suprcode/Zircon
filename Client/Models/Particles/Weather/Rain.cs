using Client.Envir;
using Client.Scenes;
using Library;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models.Particles
{
    public class Rain : ParticleEmitter
    {
        public class RainParticle : ParticleType
        {
            public RainParticle()
            {
                MaxCount = int.MaxValue;
                SpawnFrequency = TimeSpan.FromMilliseconds(10);
                Textures = new List<int> { 509 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                float scale = random.Next(1, 3);
                float scaleRate = 0F;

                bool spawnTop = random.Next(10) < 8;

                Vector2 top = new(random.Next(GameScene.Game.Size.Width), 1);
                Vector2 right = new(GameScene.Game.Size.Width, random.Next(GameScene.Game.Size.Height));

                Vector2 position = spawnTop ? top : right;
                Vector2 velocity = new (-1, 5);

                float opacity = 1F;

                float startingAngle = 0.4F;
                float angularVelocity = 0F;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(500, 2000));

                bool fade = false;
                float fadeRate = 0F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate, useMovingOffset: false);
            }

            public override void Updated(ParticleEmitter emitter, Particle updateParticle)
            {
                base.Updated(emitter, updateParticle);

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

            public override void Completed(ParticleEmitter emitter, Particle completeParticle)
            {
                base.Completed(emitter, completeParticle);

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

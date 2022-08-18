using Client.Envir;
using Client.Scenes;
using Library;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models.Particles
{
    public class Lightning : ParticleEmitter
    {
        public class LightningParticle : ParticleType
        {
            public override TimeSpan SpawnFrequency 
            { 
                get => TimeSpan.FromMilliseconds(random.Next(1000, 5000));
            }

            public LightningParticle()
            {
                MaxCount = 3;
                Textures = new List<int> { 540 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                float scale = random.Next(1, 4);
                float scaleRate = 0F;

                Vector2 position = new(random.Next(GameScene.Game.Size.Width), 0);
                Vector2 velocity = new(0, 0);

                float opacity = 1F;

                float startingAngle = 0F;
                float angularVelocity = 0F;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(100, 200));

                bool fade = true;
                float fadeRate = 0.1F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate, useMovingOffset: true);
            }
        }

        public Lightning(Point location) : base(location)
        {
            ParticleTypes = new List<ParticleType>
            {
                new LightningParticle()
            };
        }
    }
}

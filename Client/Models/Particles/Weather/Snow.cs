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
using static Client.Models.Particles.Rain;

namespace Client.Models.Particles
{
    public class Snow : ParticleEmitter
    {
        public class SnowParticle : ParticleType
        {
            public SnowParticle()
            {
                MaxCount = 500;
                SpawnFrequency = TimeSpan.FromMilliseconds(20);
                Textures = new List<int> { 500 };
                Color = Color.White;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 emitterLocation, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                float scale = (float)(random.NextDouble() * 1.5F);
                float scaleRate = 0F;

                Vector2 position = new(random.Next(GameScene.Game.Size.Width), 0);
                Vector2 velocity = new(random.Next(-1, 1), 1F);

                float opacity = 1F;

                float startingAngle = 0F;
                float angularVelocity = 0.1F;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(4000, 10000));

                bool fade = false;
                float fadeRate = 0F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }

            public override void Complete(ParticleEmitter emitter, Particle completeParticle)
            {
                base.Complete(emitter, completeParticle);

                if (completeParticle.Velocity != Vector2.Zero)
                {
                    completeParticle.Velocity = Vector2.Zero;
                    completeParticle.Fade = true;
                    completeParticle.FadeRate = 0.01F;
                    completeParticle.AngularVelocity = 0F;
                    completeParticle.Remove = false;
                    completeParticle.ScaleRate = -0.01F;
                }
            }
        }

        public Snow(Point location) : base(location)
        {
            ParticleTypes = new List<ParticleType>
            {
                new SnowParticle()
            };
        }
    }
}

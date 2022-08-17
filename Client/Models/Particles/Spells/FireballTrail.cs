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
    public class FireballTrail : ParticleEmitter
    {
        public class SmokeParticle : ParticleType
        {
            public SmokeParticle()
            {
                MaxCount = 5;
                SpawnFrequency = TimeSpan.FromMilliseconds(15);
                Textures = new List<int> { 530 };
                Color = Color.DimGray;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                Vector2 position = location;
                Vector2 velocity = Vector2.Zero;// new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));

                float opacity = 0.05f;// 0.5f;

                float startingAngle = angle;
                float angularVelocity = 0f;// 1f * (float)(random.NextDouble() * 2 - 1);

                float scale = 0.3f;// 0.2f;
                float scaleRate = 0f;
                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(100, 200));

                bool fade = false;
                float fadeRate = 0.5F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }

        }

        public class EmberParticle : ParticleType
        {
            public EmberParticle()
            {
                MaxCount = 120;
                SpawnFrequency = TimeSpan.FromMilliseconds(5);
                Textures = new List<int> { 520, 521, 522, 523 };
                Color = Globals.FireColour;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                Vector2 position = location;
                Vector2 velocity = new(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));

                float opacity = 0.5f;

                float startingAngle = angle;
                float angularVelocity = 0f;// 4f * (float)(random.NextDouble() * 2 - 1);

                float scale = 0.3f;
                float scaleRate = 0f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(200, 300));

                bool fade = true;
                float fadeRate = 0.01F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }
        }

        public FireballTrail(MirEffect owner) : base(owner)
        {
            CenterPoint = new Point[]
            {
                new Point(34, 31),
                new Point(51, 32),
                new Point(65, 44),
                new Point(76, 39),

                new Point(81, 34),
                new Point(77, 44),
                new Point(59, 54),
                new Point(52, 49),

                new Point(34, 54),
                new Point(44, 49),
                new Point(50, 50),
                new Point(51, 44),

                new Point(42, 34),
                new Point(40, 39),
                new Point(40, 44),
                new Point(46, 32)
            };

            ParticleTypes = new List<ParticleType>
            {
                new EmberParticle(),
                new SmokeParticle()
            };
        }
    }
}

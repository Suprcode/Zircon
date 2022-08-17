using Client.Envir;
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
    public class IceBoltTrail : ParticleEmitter
    {
        public class SmokeParticle : ParticleType
        {
            public SmokeParticle()
            {
                MaxCount = 20;
                SpawnFrequency = TimeSpan.FromMilliseconds(15);
                Textures = new List<int> { 530 };
                Color = Color.LightGray;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = 530;

                Vector2 position = location;
                Vector2 velocity = new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));

                float opacity = 0.5f;

                float startingAngle = angle;
                float angularVelocity = 1f * (float)(random.NextDouble() * 2 - 1);

                float scale = 0.1f;
                float scaleRate = 0f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(50, 100));

                bool fade = true;
                float fadeRate = 0.5F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }

        }

        public class IceParticle : ParticleType
        {
            public IceParticle()
            {
                MaxCount = 120;
                SpawnFrequency = TimeSpan.FromMilliseconds(5);
                Textures = new List<int> { 520, 521, 522, 523 };
                Color = Globals.IceColour;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                Vector2 position = location;
                Vector2 velocity = new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));

                float opacity = 0.5f;

                float startAngle = angle;
                float angularVelocity = 4f * (float)(random.NextDouble() * 2 - 1);

                float scale = 0.15f;
                float scaleRate = 0f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(200, 500));

                bool fade = true;
                float fadeRate = 0.01F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }
        }

        public IceBoltTrail(MirEffect owner) : base(owner)
        {
            CenterPoint = new Point[]
            {
                new Point(9, 13),
                new Point(22, 12),
                new Point(36, 11),
                new Point(49, 9),

                new Point(54, 8),
                new Point(48, 15),
                new Point(36, 25),
                new Point(24, 34),

                new Point(10, 32),
                new Point(11, 36),
                new Point(15, 28),
                new Point(20, 18),

                new Point(21, 10),
                new Point(17, 7),
                new Point(18, 12),
                new Point(12, 17)
            };

            ParticleTypes = new List<ParticleType>
            {
                new IceParticle(),
                new SmokeParticle()
            };
        }
    }
}

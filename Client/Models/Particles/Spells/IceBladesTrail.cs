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
    public class IceBladesTrail : ParticleEmitter
    {
        public class SmokeParticle : ParticleType
        {
            public SmokeParticle()
            {
                MaxCount = 10; //20
                SpawnFrequency = TimeSpan.FromMilliseconds(70);
                Textures = new List<int> { 530 };
                Color = Color.CornflowerBlue;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                //CODE TO ALTERNATE DIRECTION
                //count++;

                //if (count == 2)
                //{
                //    count = 0;
                //    left = !left;
                //}
                //var dir = Functions.ShiftDirection((MirDirection)(direction / 2), 2 * (left ? 1 : -1));
                //var vel = Functions.Move(new Point(0, 0), dir, 1);
                //Vector2 position = location;
                //Vector2 velocity = new Vector2(vel.X, vel.Y);

                Vector2 position = location;
                Vector2 velocity = Vector2.Zero;

                float opacity = 0.7f;

                float startingAngle = angle;
                float angularVelocity = (float)(random.NextDouble()) / 50;

                float scale = 0.2f;
                float scaleRate = 0.09f;
                float maxScale = 1.7f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(300, 400));

                bool fade = true;
                float fadeRate = 0.1F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate, maxScale);
            }
        }

        public class EmberParticle : ParticleType
        {
            public EmberParticle()
            {
                MaxCount = 150;
                SpawnFrequency = TimeSpan.FromMilliseconds(10);
                Textures = new List<int> { 520, 521, 522, 523 };
                Color = Color.RoyalBlue;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = Textures[random.Next(Textures.Count)];

                Vector2 position = location;
                Vector2 velocity = new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));

                float opacity = 1f;

                float startingAngle = angle;
                float angularVelocity = 0f;// 4f * (float)(random.NextDouble() * 2 - 1);

                float scale = 0.2f;
                float scaleRate = 0f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(500, 700));

                bool fade = true;
                float fadeRate = 0.5F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }
        }

        public IceBladesTrail(MirEffect owner) : base(owner)
        {
            CenterPoint = new Point[]
            {
                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),

                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),

                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),

                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),
                new Point(22, 15),
            };

            ParticleTypes = new List<ParticleType>
            {
                new EmberParticle(),
                new SmokeParticle()
            };
        }
    }
}

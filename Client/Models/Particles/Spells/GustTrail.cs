using Client.Envir;
using Library;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models.Particles
{
    public class GustTrail : ParticleEmitter
    {
        public class SmokeParticle : ParticleType
        {
            public SmokeParticle()
            {
                MaxCount = 10;
                SpawnFrequency = TimeSpan.FromMilliseconds(15);
                Textures = new List<int> { 530 };
                Color = Globals.WindColour;

                Library = CEnvir.LibraryList[LibraryFile.ProgUse];
            }

            public override Particle CreateParticle(Vector2 location, int direction16, float angle)
            {
                int texture = 530;

                Vector2 position = location;
                Vector2 velocity = Vector2.Zero;

                float opacity = 0.5f;

                float startingAngle = 0f;
                float angularVelocity = 0f;

                float scale = 0.1f;
                float scaleRate = 0.06f;

                TimeSpan ttl = TimeSpan.FromMilliseconds(random.Next(50, 150));

                bool fade = true;
                float fadeRate = 0.5F;

                return new Particle(Library, texture, opacity, position, direction16, velocity, startingAngle, angularVelocity, Color, scale, scaleRate, ttl, fade, fadeRate);
            }

        }

        public GustTrail(MirEffect owner) : base(owner)
        {
            CenterPoint = new Point[]
            {
                 new Point(26, 14),
                 new Point(40, 19),
                 new Point(60, 11),
                 new Point(58, 18),

                 new Point(62, 26),
                 new Point(60, 44),
                 new Point(50, 44),
                 new Point(39, 49),

                 new Point(31, 61),
                 new Point(29, 58),
                 new Point(33, 48),
                 new Point(31, 34),

                 new Point(30, 27),
                 new Point(37, 19),
                 new Point(27, 13),
                 new Point(27, 15)
            };

            ParticleTypes = new List<ParticleType>
            {
                new SmokeParticle()
            };
        }
    }
}

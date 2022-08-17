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
    public class Particle : IDisposable
    {
        /// <summary>
        /// The texture that will be drawn to represent the particle
        /// </summary>
        public int TextureIndex { get; set; }

        /// <summary>
        /// Library of particle images
        /// </summary>
        public MirLibrary Library { get; set; }

        /// <summary>
        /// The current position of the particle 
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Direction16 that the particle is facing. 22.5 degrees each direction
        /// </summary>
        public int Direction16 { get; set; }

        /// <summary>
        /// The speed of the particle at the current instance
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// The current angle of rotation of the particle
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// The speed that the angle is changing
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// The color of the particle
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Starting size of the particle
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The rate at which the particle grows in size
        /// </summary>
        public float ScaleRate { get; set; }

        /// <summary>
        /// Maximum scale particle will grow to
        /// </summary>
        public float MaxScale { get; set; }

        /// <summary>
        /// The 'time to live' of the particle
        /// </summary>
        public TimeSpan TTL { get; set; }

        /// <summary>
        /// Time until next update
        /// </summary>
        public TimeSpan UpdateSpeed { get; set; }

        /// <summary>
        /// Fade out when expired
        /// </summary>
        public bool Fade { get; set; }

        /// <summary>
        /// Rate of fade out when expired
        /// </summary>
        public float FadeRate { get; set; }

        /// <summary>
        /// Starting opacity of particle
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Real expiry time
        /// </summary>
        public DateTime Expires;

        /// <summary>
        /// Real next update time
        /// </summary>
        public DateTime NextUpdate;

        public bool Remove { get; set; }

        public Particle(MirLibrary lib, int textureIndex, float opacity, Vector2 position, int direction16, Vector2 velocity,
            float angle, float angularVelocity, Color color, float scale, float scaleRate, TimeSpan ttl, bool fade, float fadeRate, float maxScale = -1f)
        {
            Library = lib;
            TextureIndex = textureIndex;
            Position = position;
            Direction16 = direction16;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Scale = scale;
            ScaleRate = scaleRate;
            MaxScale = maxScale;
            TTL = ttl;
            UpdateSpeed = TimeSpan.FromMilliseconds(10);

            Opacity = opacity;
            Fade = fade;
            FadeRate = fadeRate;

            Expires = CEnvir.Now.Add(TTL);
            NextUpdate = CEnvir.Now.Add(UpdateSpeed);
        }

        public bool Update()
        {
            if (NextUpdate <= CEnvir.Now)
            {
                Position += Velocity;
                Angle += AngularVelocity;

                if (MaxScale >= 0f)
                {
                    Scale = Math.Min(Scale + ScaleRate, MaxScale);
                }
                else
                {
                    Scale += ScaleRate;
                }

                if (Scale < 0F)
                    Scale = 0F;

                NextUpdate = CEnvir.Now.Add(UpdateSpeed);

                if (Expires <= CEnvir.Now)
                {
                    if (Fade)
                    {
                        Opacity -= FadeRate;

                        if (Opacity > 0F)
                        {
                            return true;
                        }
                        else
                        {
                            Remove = true;
                        }
                    } 
                    else
                    {
                        Remove = true;
                    }
                }

                return true;
            }

            return false;
        }

        public void Draw()
        {
            if (Library == null) return;

            Size size = Library.GetSize(TextureIndex);

            var centerX = (size.Width) / 2;
            var centerY = (size.Height) / 2;

            Library.DrawBlend(TextureIndex, Scale, Color, Position.X - centerX, Position.Y - centerY, Angle, Opacity, ImageType.Image, false, 0);
        }

        #region IDisposable

        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Library = null;

                IsDisposed = true;
            }
        }

        #endregion
    }
}

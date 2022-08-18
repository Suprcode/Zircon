using Client.Envir;
using Client.Scenes;
using Library;
using Newtonsoft.Json.Linq;
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
        /// Offset the position of the particle based on the original location.
        /// Used when the emitter has no target to focus on
        /// </summary>
        public bool UseMovingOffset { get; set; }

        /// <summary>
        /// Location of user at the time of particle creation
        /// </summary>
        public Point UserLocationOnCreation { get; set; }


        /// <summary>
        /// Real expiry time
        /// </summary>
        public DateTime Expires;

        /// <summary>
        /// Real next update time
        /// </summary>
        public DateTime NextUpdate;

        /// <summary>
        /// Removes the particle from process
        /// </summary>
        public bool Remove;

        /// <summary>
        /// Indicates how many times the particle has been updated
        /// </summary>
        public int UpdateCount { get; private set; }

        public Particle(MirLibrary lib, int textureIndex, float opacity, Vector2 position, int direction16, Vector2 velocity,
            float angle, float angularVelocity, Color color, float scale, float scaleRate, TimeSpan ttl, bool fade, float fadeRate, float maxScale = -1f, bool useMovingOffset = false, int updateSpeedMS = 10)
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
            UpdateSpeed = TimeSpan.FromMilliseconds(updateSpeedMS);

            Opacity = opacity;
            Fade = fade;
            FadeRate = fadeRate;

            UseMovingOffset = useMovingOffset;
            UserLocationOnCreation = MapObject.User.CurrentLocation;

            Expires = CEnvir.Now.Add(TTL);
            NextUpdate = CEnvir.Now.Add(UpdateSpeed);
            Remove = false;
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

                UpdateCount++;

                return true;
            }

            return false;
        }

        public void Draw()
        {
            if (Library == null) return;

            Size size = Library.GetSize(TextureIndex);

            var drawX = Position.X - ((size.Width) / 2);
            var drawY = Position.Y - ((size.Height) / 2);

            if (UseMovingOffset)
            {
                drawX -= (UseMovingOffset ? MapObject.User.MovingOffSet.X : 0) + ((MapObject.User.CurrentLocation.X - UserLocationOnCreation.X) * MapObject.CellWidth);
                drawY -= (UseMovingOffset ? MapObject.User.MovingOffSet.Y : 0) + ((MapObject.User.CurrentLocation.Y - UserLocationOnCreation.Y) * MapObject.CellHeight);
            }

            Library.DrawBlend(TextureIndex, Scale, Color, drawX, drawY, Angle, Opacity, ImageType.Image, false, 0);
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

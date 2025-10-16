using Client.Envir;
using Client.Scenes;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models
{
    public class MirLineEffect : MirEffect
    {
        // Simulation parameters
        private const float LinkLength = 30f;       // Desired length per chain link
        private const float Gravity = 0.05f;        // Downward force applied per tick
        private const float SpringStrength = 0.15f;  // Pulling force between links
        private const float Damping = 0.9f;         // Velocity damping to stabilize motion

        private readonly MapObject _source;
        private readonly MapObject _target;
        private MirEffect _owner;

        private readonly List<Vector2> _positions = [];
        private readonly List<Vector2> _velocities = [];

        private int _linkCount = 6;     // Starts with a default value; adjusts dynamically
        private bool _initialized;

        public MirLineEffect(MapObject source, MapObject target, LibraryFile library, int startIndex)
            : base(startIndex, 1, TimeSpan.FromMilliseconds(100), library, 0, 0, Color.White)
        {
            _source = source;
            _target = target;

            // Initialize chain links between source and target
            Point startLoc = _source.CurrentLocation;
            Point endLoc = _target.CurrentLocation;

            for (int i = 0; i < _linkCount; i++)
            {
                float t = i / (float)(_linkCount - 1);
                _positions.Add(new Vector2(
                    Lerp(startLoc.X, endLoc.X, t),
                    Lerp(startLoc.Y, endLoc.Y, t)
                ));
                _velocities.Add(Vector2.Zero);
            }
        }

        public void SetOwner(MirEffect owner) => _owner = owner;

        public override void Process()
        {
            // Remove if owner effect no longer exists
            if (_owner != null && !GameScene.Game.MapControl.Effects.Contains(_owner))
            {
                Remove();
                return;
            }

            Vector2 startPos = ToWorld(_source);
            Vector2 endPos = ToWorld(_target);

            // Rebuild chain if distance requires more or fewer links
            EnsureLinkCount(startPos, endPos);

            // Initialize straight chain on first update
            if (!_initialized)
            {
                for (int i = 0; i < _linkCount; i++)
                {
                    float t = i / (float)(_linkCount - 1);
                    _positions[i] = new Vector2(
                        Lerp(startPos.X, endPos.X, t),
                        Lerp(startPos.Y, endPos.Y, t)
                    );
                    _velocities[i] = Vector2.Zero;
                }
                _initialized = true;
            }

            // Anchor endpoints
            _positions[0] = startPos;
            _positions[^1] = endPos;

            // Simulate spring and gravity for intermediate links
            for (int i = 1; i < _linkCount - 1; i++)
            {
                // Apply gravity
                var vel = _velocities[i]; 
                vel.Y += Gravity; 
                _velocities[i] = vel;

                // Pull toward the midpoint between neighbors
                Vector2 avg = (_positions[i - 1] + _positions[i + 1]) * 0.5f;
                Vector2 force = (avg - _positions[i]) * SpringStrength;
                _velocities[i] += force;

                // Update position and apply damping
                _positions[i] += _velocities[i];
                _velocities[i] *= Damping;
            }
        }

        public override void Draw()
        {
            if (CEnvir.Now < StartTime || Library == null) return;

            // Draw chain segments between each pair of consecutive points
            for (int i = 0; i < _linkCount - 1; i++)
            {
                Vector2 p1 = _positions[i];
                Vector2 p2 = _positions[i + 1];
                Vector2 mid = (p1 + p2) * 0.5f;

                float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) + MathF.PI / 2;

                if (Blend)
                {
                    Library.DrawBlend(
                                        StartIndex,    // Texture index
                                        1f,            // Scale
                                        DrawColour,    // Tint color
                                        mid.X,         // X position
                                        mid.Y,         // Y position
                                        angle,         // Rotation
                                        Opacity,       // Transparency
                                        ImageType.Image,
                                        false,
                                        0
                                    );
                }
                else
                {
                    Library.Draw(
                                        StartIndex,    // Texture index
                                        1f,            // Scale
                                        DrawColour,    // Tint color
                                        mid.X,         // X position
                                        mid.Y,         // Y position
                                        angle,         // Rotation
                                        Opacity,       // Transparency
                                        ImageType.Image,
                                        false,
                                        0
                                    );
                }
                
            }
        }

        /// <summary>
        /// Ensures the number of chain links matches the current distance.
        /// Rebuilds internal arrays when necessary.
        /// </summary>
        private void EnsureLinkCount(Vector2 start, Vector2 end)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            int desiredLinks = Math.Max(2, (int)MathF.Ceiling(distance / LinkLength));
            if (desiredLinks == _linkCount) return;

            _linkCount = desiredLinks;
            _positions.Clear();
            _velocities.Clear();

            for (int i = 0; i < _linkCount; i++)
            {
                float t = i / (float)(_linkCount - 1);
                _positions.Add(new Vector2(
                    Lerp(start.X, end.X, t),
                    Lerp(start.Y, end.Y, t)
                ));
                _velocities.Add(Vector2.Zero);
            }
        }

        /// <summary>
        /// Converts a MapObject's position to world-space coordinates relative to the player.
        /// </summary>
        private static Vector2 ToWorld(MapObject obj)
        {
            float x = (obj.CurrentLocation.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth
                    - MapObject.User.MovingOffSet.X + obj.MovingOffSet.X;

            float y = (obj.CurrentLocation.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight
                    - MapObject.User.MovingOffSet.Y + obj.MovingOffSet.Y - 25;

            return new Vector2(x, y);
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;

        private struct Vector2(float x, float y)
        {
            public float X = x, Y = y;

            public static Vector2 Zero => new(0, 0);

            public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
            public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
            public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
        }
    }

    public class MirChainEffect : MirLineEffect
    {
        public MirChainEffect(MapObject source, MapObject target) : base(source, target, LibraryFile.MagicEx7, 80)
        {
        }
    }
}

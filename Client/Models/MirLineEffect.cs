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
        private const float AnchorOffsetY = 50f;    // Attach near the top of the target effect

        protected readonly MapObject _source;
        protected readonly MapObject _target;
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

            Size imageSize = Library.GetSize(StartIndex);
            float originX = imageSize.Width / 2f;
            float originY = imageSize.Height / 2f;

            // Draw chain segments between each pair of consecutive points
            for (int i = 0; i < _linkCount - 1; i++)
            {
                Vector2 p1 = _positions[i];
                Vector2 p2 = _positions[i + 1];
                Vector2 mid = (p1 + p2) * 0.5f;

                float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) + MathF.PI / 2;

                // Use actual distance to compute scale
                float dist = MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2) + MathF.Pow(p2.Y - p1.Y, 2));
                float stretchY = dist / LinkLength; // vertical stretch
                float stretchX = 1f;                // no horizontal stretch

                // DrawBlendScaled positions an image by its unscaled top-left corner.
                // Convert the desired segment centre to that coordinate space.
                float drawX = mid.X - originX;
                float drawY = mid.Y - originY;

                if (Blend)
                {
                    Library.DrawBlendScaled(StartIndex, stretchX, stretchY, DrawColour, drawX, drawY, angle, Opacity, ImageType.Image, false, 0);
                }
                else
                {
                    Library.DrawBlendScaled(StartIndex, stretchX, stretchY, DrawColour, drawX, drawY, angle, Opacity, ImageType.Image, false, 0);
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
        protected Vector2 ToWorld(MapObject obj)
        {
            var offset = obj == _source ? SourceOffset() : TargetOffset();

            float x = (obj.CurrentLocation.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth
                    - MapObject.User.MovingOffSet.X + obj.MovingOffSet.X + offset.X;

            float y = (obj.CurrentLocation.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight
                    - MapObject.User.MovingOffSet.Y + obj.MovingOffSet.Y + offset.Y - AnchorOffsetY;

            return new Vector2(x, y);
        }

        protected virtual Point SourceOffset()
        {
            return new Point(0, -25);
        }

        protected virtual Point TargetOffset()
        {
            return new Point(0, -25);
        }

        protected static float Lerp(float a, float b, float t) => a + (b - a) * t;

        protected struct Vector2(float x, float y)
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

    public class MirRopeEffect : MirLineEffect
    {
        public uint TargetObjectID { get; set; }

        // Time for the throw animation (ms)
        private const float LaunchDuration = 600f;

        // Maximum height above the line between source and target
        private const float ThrowArcHeight = 120f;

        // Overshoot factor for final snap
        private const float OvershootFactor = 1.15f;

        private float _launchProgress;
        private long _launchStartTime;
        private bool _launchComplete;

        public MirRopeEffect(MapObject source, MapObject target)
            : base(source, target, LibraryFile.MagicEx7, 81)
        {
            // Start all rope points collapsed at source
            var startPos = ToWorld(source);

            for (int i = 0; i < 6; i++)
                SetInitialPosition(startPos);

            _launchStartTime = CEnvir.Now.Ticks;
            _launchProgress = 0f;
            _launchComplete = false;
        }

        public override void Process()
        {
            if (Target == null || Target.TamingState != TamingState.Cast)
            {
                Remove();
                return;
            }

            if (!_launchComplete)
            {
                float elapsed = (CEnvir.Now.Ticks - _launchStartTime) / 10000f; // ms
                _launchProgress = MathF.Min(1.2f, elapsed / LaunchDuration);

                Vector2 startPos = ToWorld(_source);
                Vector2 endPos = ToWorld(_target);

                // Compute the current "in-flight" target end position (the rope tip)
                Vector2 flyingTarget = ComputeThrownTarget(startPos, endPos, _launchProgress);

                // Stretch rope links between source and flying target
                for (int i = 0; i < 6; i++)
                {
                    float segment = i / 5f;
                    Vector2 pos = new(
                        Lerp(startPos.X, flyingTarget.X, segment),
                        Lerp(startPos.Y, flyingTarget.Y, segment)
                    );
                    SetPosition(i, pos);
                }

                if (_launchProgress >= 1f)
                    _launchComplete = true;
            }
            else
            {
                // Once rope lands, run the normal rope physics
                base.Process();
            }
        }

        /// <summary>
        /// Computes the flying target's position based on a parabolic throw arc.
        /// </summary>
        private static Vector2 ComputeThrownTarget(Vector2 start, Vector2 end, float t)
        {
            // Horizontal and vertical interpolation base
            float tx = EaseOutCubic(t);  // smooth horizontal movement
            float x = Lerp(start.X, end.X, tx);

            // Vertical parabolic arc
            float ty = EaseOutQuad(t);
            float y = Lerp(start.Y, end.Y, ty);

            // Add upward arc (apex near mid-throw)
            float heightFactor = MathF.Sin(MathF.Min(t, 1f) * MathF.PI);
            y -= heightFactor * ThrowArcHeight;

            // Overshoot slight forward motion beyond target for realism
            if (t > 1f)
            {
                float overshoot = (t - 1f) * OvershootFactor * 0.5f;
                x += (end.X - start.X) * overshoot;
                y += (end.Y - start.Y) * overshoot * 0.2f;
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Smooth ease-out cubic curve.
        /// </summary>
        private static float EaseOutCubic(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return 1f - MathF.Pow(1f - t, 3);
        }

        /// <summary>
        /// Smooth ease-out quadratic for vertical descent.
        /// </summary>
        private static float EaseOutQuad(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return 1f - (1f - t) * (1f - t);
        }

        private void SetInitialPosition(Vector2 pos)
        {
            var positionsField = typeof(MirLineEffect)
                .GetField("_positions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var velocitiesField = typeof(MirLineEffect)
                .GetField("_velocities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var positions = (List<Vector2>)positionsField.GetValue(this);
            var velocities = (List<Vector2>)velocitiesField.GetValue(this);

            positions.Add(pos);
            velocities.Add(Vector2.Zero);
        }

        private void SetPosition(int index, Vector2 pos)
        {
            var positionsField = typeof(MirLineEffect)
                .GetField("_positions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positions = (List<Vector2>)positionsField.GetValue(this);

            if (index >= 0 && index < positions.Count)
                positions[index] = pos;
        }

        protected override Point SourceOffset()
        {
            switch (_source.Direction)
            {
                case MirDirection.Up:
                    return new Point(0, -50);
                case MirDirection.UpRight:
                    return new Point(40, -35);
                case MirDirection.Right:
                    return new Point(35, -15);
                case MirDirection.DownRight:
                    return new Point(27, -7);
                case MirDirection.Down:
                    return new Point(0, 0);
                case MirDirection.DownLeft:
                    return new Point(-17, -10);
                case MirDirection.Left:
                    return new Point(-25, -20);
                case MirDirection.UpLeft:
                    return new Point(-15, -40);
                default:
                    break;
            }

            return new Point(0, 0);
        }

        protected override Point TargetOffset()
        {
            // Neutral “neck center” relative to the target’s sprite center
            var baseOffset = new Point(8, -25);

            // Optional quick global scaling for all deltas (e.g., if sprite size changes).
            const float deltaScale = 1f;

            // Fetch the per-direction delta; fall back to (0,0) if unmapped.
            var delta = NeckDirDelta.TryGetValue(_target.Direction, out var d) ? d : Point.Empty;

            // Apply base + scaled delta.
            int x = baseOffset.X + (int)(delta.X * deltaScale);
            int y = baseOffset.Y + (int)(delta.Y * deltaScale);

            return new Point(x, y);
        }

        private static readonly Dictionary<MirDirection, Point> NeckDirDelta = new()
        {
            // Up directions
            [MirDirection.Up] = new Point(0, -50),
            [MirDirection.UpRight] = new Point(25, -45),
            [MirDirection.UpLeft] = new Point(-25, -45),

            // Horizontal
            [MirDirection.Right] = new Point(40, -30),
            [MirDirection.Left] = new Point(-40, -30),

            // Down directions
            [MirDirection.DownRight] = new Point(25, -10),
            [MirDirection.Down] = new Point(0, 10),
            [MirDirection.DownLeft] = new Point(-25, -10),
        };
    }
}

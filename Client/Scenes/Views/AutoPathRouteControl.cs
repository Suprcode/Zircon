using Client.Controls;
using Client.Envir;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public sealed class AutoPathRouteControl : DXControl
    {
        private const float DotSpacing = 6F;
        private static readonly Size DotSize = new Size(4, 4);

        private static readonly Rectangle[] WaypointBackground =
        {
            new Rectangle(5, 1, 5, 1),
            new Rectangle(2, 2, 11, 2),
            new Rectangle(1, 4, 13, 7),
            new Rectangle(2, 11, 11, 2),
            new Rectangle(5, 13, 5, 1),
        };

        private static readonly Rectangle[] WaypointBorder =
        {
            new Rectangle(4, 0, 7, 1),
            new Rectangle(2, 1, 3, 1),
            new Rectangle(10, 1, 3, 1),
            new Rectangle(1, 2, 1, 3),
            new Rectangle(13, 2, 1, 3),
            new Rectangle(0, 4, 1, 7),
            new Rectangle(14, 4, 1, 7),
            new Rectangle(1, 10, 1, 3),
            new Rectangle(13, 10, 1, 3),
            new Rectangle(2, 13, 3, 1),
            new Rectangle(10, 13, 3, 1),
            new Rectangle(4, 14, 7, 1),
        };

        private AutoPathRouteLeg _Leg;
        private AutoPathRoute _Route;
        private MapInfo _DisplayedMap;
        private Color _RouteColour;
        private float _ScaleX;
        private float _ScaleY;
        private int _ProgressMapIndex = -1;
        private int _ProgressIndex = -1;
        private readonly List<RouteDot> _RouteDots = new List<RouteDot>();

        public DXLabel WaypointLabel;

        public AutoPathRouteControl()
        {
            DrawTexture = false;
            PassThrough = true;
            CacheInParent = false;

            WaypointLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Size = new Size(15, 15),
                ForeColour = Color.Black,
                Outline = false,
                DrawFormat = TextFormatFlags.HorizontalCenter |
                             TextFormatFlags.VerticalCenter |
                             TextFormatFlags.NoPrefix,
                PassThrough = true,
                CacheInParent = false,
                Visible = false,
            };
        }

        public void SetRoute(AutoPathRoute route, bool active, MapInfo displayedMap, MapInfo currentMap, float scaleX, float scaleY)
        {
            _Route = route;
            _DisplayedMap = displayedMap;
            _ScaleX = scaleX;
            _ScaleY = scaleY;
            _RouteColour = displayedMap?.Index != currentMap?.Index
                ? Color.Gray
                : active
                    ? Color.White
                    : Color.Silver;
            _Leg = route?.Legs?.FirstOrDefault(x =>
                x.MapIndex == displayedMap?.Index);

            Size = Parent?.Size ?? Size.Empty;
            UpdateRouteDots();
            UpdateWaypoint();
        }

        public void SetProgress(int mapIndex, int pointIndex)
        {
            _ProgressMapIndex = mapIndex;
            _ProgressIndex = pointIndex;
        }

        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            if (WaypointLabel != null)
                WaypointLabel.Opacity = nValue;
        }

        protected override void DrawControl()
        {
            if (_Leg?.Points == null || _Leg.Points.Count == 0) return;

            float oldOpacity = RenderingPipelineManager.GetOpacity();
            RenderingPipelineManager.SetOpacity(Opacity);

            bool currentLeg = _DisplayedMap.Index == _ProgressMapIndex;
            foreach (RouteDot dot in _RouteDots)
            {
                if (currentLeg && dot.PointIndex <= _ProgressIndex) continue;

                Point location = dot.Location;
                Fill(new Rectangle(location, DotSize), Color.Black);
                Fill(new Rectangle(location.X + 1, location.Y + 1, 2, 2), _RouteColour);
            }

            DrawWaypoint();
            RenderingPipelineManager.SetOpacity(oldOpacity);
        }

        private void UpdateRouteDots()
        {
            _RouteDots.Clear();
            if (_Leg?.Points == null || _Leg.Points.Count < 2) return;

            PointF segmentStart = Project(_Leg.Points[0]);
            float distanceToNextDot = DotSpacing;

            for (int i = 1; i < _Leg.Points.Count; i++)
            {
                PointF segmentEnd = Project(_Leg.Points[i]);
                float segmentLength = Distance(segmentStart, segmentEnd);

                while (segmentLength >= distanceToNextDot)
                {
                    float ratio = distanceToNextDot / segmentLength;
                    segmentStart = new PointF(
                        segmentStart.X + (segmentEnd.X - segmentStart.X) * ratio,
                        segmentStart.Y + (segmentEnd.Y - segmentStart.Y) * ratio);

                    _RouteDots.Add(new RouteDot(i, new Point(
                        (int)Math.Round(segmentStart.X) - DotSize.Width / 2,
                        (int)Math.Round(segmentStart.Y) - DotSize.Height / 2)));

                    segmentLength = Distance(segmentStart, segmentEnd);
                    distanceToNextDot = DotSpacing;
                }

                distanceToNextDot -= segmentLength;
                segmentStart = segmentEnd;
            }
        }

        private static float Distance(PointF first, PointF second)
        {
            float x = second.X - first.X;
            float y = second.Y - first.Y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        private void UpdateWaypoint()
        {
            if (WaypointLabel == null) return;

            bool visible = _Route != null &&
                           _DisplayedMap != null &&
                           _Leg?.Points != null &&
                           _Leg.Points.Count > 0;
            WaypointLabel.Visible = visible;
            if (!visible) return;

            WaypointLabel.Text = _Route.WaypointNumber.ToString();
            WaypointLabel.Location = Project(GetWaypoint(), WaypointLabel.Size);
            WaypointLabel.Opacity = Opacity;
        }

        private void DrawWaypoint()
        {
            Point location = Project(GetWaypoint(), new Size(15, 15));

            foreach (Rectangle area in WaypointBackground)
                FillOffset(location, area, Color.Lime);

            foreach (Rectangle area in WaypointBorder)
                FillOffset(location, area, Color.Black);
        }

        private Point GetWaypoint()
        {
            return _Route.DestinationMapIndex == _DisplayedMap.Index
                ? _Route.DisplayDestination
                : _Leg.Points[_Leg.Points.Count - 1];
        }

        private PointF Project(Point point)
        {
            return new PointF(_ScaleX * point.X, _ScaleY * point.Y);
        }

        private Point Project(Point point, Size size)
        {
            return new Point(
                (int)(_ScaleX * point.X) - size.Width / 2,
                (int)(_ScaleY * point.Y) - size.Height / 2);
        }

        private void FillOffset(Point location, Rectangle area, Color colour)
        {
            Fill(new Rectangle(
                location.X + area.X,
                location.Y + area.Y,
                area.Width,
                area.Height), colour);
        }

        private void Fill(Rectangle area, Color colour)
        {
            area.Offset(DisplayArea.Location);
            area = Rectangle.Intersect(area, ClipArea);
            if (area.Width <= 0 || area.Height <= 0) return;

            RenderingPipelineManager.FillRectangle(area, colour);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            _Leg = null;
            _Route = null;
            _DisplayedMap = null;
            _RouteDots.Clear();

            if (WaypointLabel != null)
            {
                if (!WaypointLabel.IsDisposed)
                    WaypointLabel.Dispose();

                WaypointLabel = null;
            }
        }

        private readonly struct RouteDot
        {
            public int PointIndex { get; }
            public Point Location { get; }

            public RouteDot(int pointIndex, Point location)
            {
                PointIndex = pointIndex;
                Location = location;
            }
        }
    }
}

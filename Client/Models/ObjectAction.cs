using Library;
using System.Drawing;

namespace Client.Models
{
    public sealed class ObjectAction
    {
        public MirAction Action;
        public MirDirection Direction;
        public Point Location;
        public object[] Extra;

        public ObjectAction(MirAction action, MirDirection direction, Point location, params object[] extra)
        {
            Action = action;
            Direction = direction;
            Location = location;
            Extra = extra;
        }
    }
}

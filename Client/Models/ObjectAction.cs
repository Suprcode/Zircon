using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

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

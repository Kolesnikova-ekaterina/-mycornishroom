using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cornishroom
{
    enum typelight { point, directed, ambient  }
    class light
    {
        public typelight type { get;}
        public double intensity;
        public PointD position;
        public Vector direction;
        public light(double i)
        {
            intensity = i;
            type = typelight.ambient;
        }
        public light(double i, PointD pos)
        {
            intensity = i;
            type = typelight.point;
            position = pos;

        }
        public light(double i, Vector dir)
        {
            intensity = i;
            type = typelight.directed;
            direction = dir;

        }

    }
}

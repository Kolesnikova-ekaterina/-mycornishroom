using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cornishroom
{
    public class Vector
    {
        public double x;
        public double y;
        public double z;

        public Vector(double xx, double yy, double zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }
        public Vector(PointD a, PointD b)
        {
            x = b.x - a.x;
            y = b.y - a.y;
            z = b.z - a.z;
        }
        public void reverse()
        {
            x *= -1;
            y *= -1;
            z *= -1;
        }
        public static Vector operator -(Vector v)
        {
            return new Vector(-v.x, -v.y, -v.z);
        }
        public Vector cross(Vector other)
        {
            return new Vector(
              this.y * other.z - this.z * other.y,
              this.z * other.x - this.x * other.z,
              this.x * other.y - this.y * other.x
            );
        }
        public double dot(Vector other)
        {
            return this.x * other.x + this.y * other.y + this.z * other.z;
        }
        public void normalize()
        {
            double l = Math.Sqrt(x * x + y * y + z * z);
            if (l == 0)
                return ;
            x /= l;
            y /= l;
            z /= l;
        }
        public static bool operator ==(Vector v1, Vector v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y -v2.y, v1.z - v2.z);
        }
        public static bool operator !=(Vector v1, Vector v2)
        {
            return !(v1 == v2);
        }
        public double length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
        public static Vector operator *(float t, Vector p1)
        {
            return new Vector(p1.x * t, p1.y * t, p1.z * t);
        }
        public static Vector operator *(double t, Vector p1)
        {
            return new Vector(p1.x * t, p1.y * t, p1.z * t);
        }

    }
}

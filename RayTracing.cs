using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cornishroom
{
    class RayTracing
    {
        Vector O;
        int Vw, Vh, d, Cw, Ch;
        List<Polyhedra> scene;
        Color background;
        public RayTracing(int cw,int ch, List<Polyhedra> ps, Color back)
        {
            O = new Vector(0, 0, 0);
            Vw = Vh = d = 1;
            Cw = cw;
            Ch = ch;
            scene = ps;
            background = back;
        }
        public Bitmap ApplyRayTracing()
        {
            Bitmap res = new Bitmap(Cw, Ch);
            for (int cx = -Cw / 2; cx < Cw / 2; cx++)
            {
                for (int cy = -Ch / 2; cy < Ch / 2; cy++)
                {
                    int Sx = Cw / 2 + cx;
                    int Sy = Ch / 2 - cy;
                    Color color = RayTrace(cx, cy, 1.0, double.MaxValue);
                    res.SetPixel(Sx, Sy, color);
                }
            }
            return res;
        } 
        public Color RayTrace(int Cx, int Cy, double t_min, double t_max)
        {
            Vector D = new Vector(
                1.0 * Cx * Vw / Cw, 
                1.0 * Cy * Vh / Ch, 
                d);
            D.normalize();
            double closest_t = double.MaxValue;
            Polygon closest_poly = null;
            foreach (var plhdr in scene)
            {
                foreach(var plgn in plhdr.polygons)
                {
                    double t = IntesectRayPolygon(O, D, plgn, plhdr);
                    if (t >= t_min && t <= t_max && t < closest_t)
                    {
                        closest_t = t;
                        closest_poly = plgn;
                    }
                }
            }
            if (closest_poly != null)
                return closest_poly.color;
            return background;
        }
        private double eps = 0.1;
       
        public double IntesectRayPolygon(Vector Ro, Vector Rd, Polygon p, Polyhedra plhdr)
        {
            /*Vector P = O + t(V - O) = O + t * D*/
            double D = p.D;
            Vector Pn = p.normal;
            double Vd = Pn.dot(Rd);
            if (Math.Abs(Vd) < eps)
                return double.MinValue;
            double V0 = -Pn.dot(Ro) - D;
            double t = V0 / Vd;
            if (BelongsTo(
                new PointD(Ro.x + t * Rd.x, Ro.y + t * Rd.y, Ro.z + t * Rd.z),
                p, 
                plhdr))
                return t;
            return double.MinValue;
        }
        public bool BelongsTo(PointD point, Polygon triangle, Polyhedra plhdr)
        {

            PointD rA = plhdr.points[triangle.lines[0].a];
            PointD rB = plhdr.points[triangle.lines[1].a];
            PointD rC = plhdr.points[triangle.lines[2].a];
            double test = triangle.normal.x * point.x + triangle.normal.y * point.y + triangle.normal.z * point.z + triangle.D;
            double a = rB.dist(rC);
            double b = rA.dist(rC);
            double c = rA.dist(rB);

            double xA = point.dist(rA);
            double xB = point.dist(rB);
            double xC = point.dist(rA);
            if (!(xA < b && xA < c))
                return false;
            if (!(xB < a && xB < c))
                return false;
            if (!(xC < b && xC < a))
                return false;
            return true;
            /*
            PointD rA = plhdr.points[triangle.lines[0].a];
            PointD rB = plhdr.points[triangle.lines[1].a];
            PointD rC = plhdr.points[triangle.lines[2].a];

            Vector u = new Vector(rB.x - rA.x, rB.y - rA.y, rB.z - rA.z);
            Vector v = new Vector(rC.x - rA.x, rC.y - rA.y, rC.z - rA.z);
            Vector q = new Vector(rC.x - rB.x, rC.y - rB.y, rC.z - rB.z);
            Vector w = u.cross(v);

            Vector x = new Vector(point.x - rA.x, point.y - rA.y, point.z - rA.z);
            Vector y = new Vector(point.x - rB.x, point.y - rB.y, point.z - rB.z);

            Vector w1 = x.cross(u);
            Vector w2 = v.cross(x);
            Vector w3 = q.cross(y);

            double delta1 = 0.5 * w.length();
            double delta2 = 0.5 * (w1.length() + w2.length() + w3.length());
            if (Math.Abs(delta1 - delta2) < eps)
                return true;
            return false;*/

        }
    }
}

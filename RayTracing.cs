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
        List<Sphere> scenespheres;
        Color background;
        List<light> lights;
        public RayTracing(int cw,int ch, List<Polyhedra> ps, Color back, List<Sphere> ssphr, List<light> l)
        {
            O = new Vector(0, 0.4, 0);
            Vw = Vh = d = 1;
            Cw = cw;
            Ch = ch;
            scene = ps;
            background = back;
            scenespheres = ssphr;
            lights = l;
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
        public double ClosestIntersect(Vector O, Vector D, 
                                    out Sphere closest_sphere,  
                                    out Polygon closest_poly, 
                                    double t_min, double t_max)
        {
            double closest_t = double.MaxValue;
            closest_poly = null;
            closest_sphere = null;
            foreach (var plhdr in scene)
            {
                foreach (var plgn in plhdr.polygons)
                {
                    double t = IntersectTriangle(plgn, plhdr, O, D);
                    if (t >= t_min && t <= t_max && t < closest_t)
                    {
                        closest_t = t;
                        closest_poly = plgn;
                    }
                }
            }
            foreach (var sphere in scenespheres)
            {
                List<double> t = IntersectRaySphere(O, D, sphere);
                if (t[0] >= t_min && t[0] <= t_max && t[0] < closest_t)
                {
                    closest_t = t[0];
                    closest_sphere = sphere;
                    closest_poly = null;
                }
                if (t[1] >= t_min && t[1] <= t_max && t[1] < closest_t)
                {
                    closest_t = t[1];
                    closest_sphere = sphere;
                    closest_poly = null;
                }
            }
            return closest_t;
        }
        public Color RayTrace(int Cx, int Cy, double t_min, double t_max)
        {
            Vector D = new Vector(
                1.0 * Cx * Vw / Cw, 
                1.0 * Cy * Vh / Ch, 
                d -0.39);
            D.normalize();
            return RecursiveRayTrace(O, D, t_min, t_max, 3);
        }
        public double EvalLightning(Vector P, Vector N, Vector V, double s )
        {
            double res = 0.0;
            Vector L;
            foreach(var l in lights)
            {
                L = new Vector(0, 0, 0);
                switch (l.type)
                {
                    case typelight.ambient:
                        res += l.intensity;
                        break;
                    case typelight.point:
                        L = new Vector(
                            l.position.x -P.x,
                            l.position.y - P.y,
                            l.position.z - P.z);
                        break;
                    case typelight.directed:
                        L = l.direction;
                        break;
                }
                if (l.type == typelight.ambient)
                    continue;

                //shadow

                Polygon closest_poly = null;
                Sphere closest_sphere = null;
                double shadow_t = ClosestIntersect(P, L, out closest_sphere, out closest_poly,  0.001, double.MaxValue);
                if ( closest_sphere != null || (closest_poly!=null && closest_poly.Objecttype != objecttype.walls ))
                    continue;
                //Диффузность
                double dotnl = N.dot(L);
                if (dotnl > 0)
                    res += l.intensity * dotnl / (N.length() * L.length());
                //Зеркальность
                if (s != -1)
                {
                    Vector R = new Vector(
                        2.0*N.x* dotnl - L.x,
                        2.0 * N.y * dotnl - L.y,
                        2.0 * N.z * dotnl - L.z);
                    double dotrv = R.dot(V);
                    if (dotrv > 0)
                        res += l.intensity * Math.Pow(dotrv / (R.length() * V.length()), s);
                }

            }
            return res;

        }
        public double IntersectTriangle(Polygon p, Polyhedra plhdr, Vector Ro, Vector Rd )
        {
            PointD rA = plhdr.points[p.lines[0].a];//0
            PointD rB = plhdr.points[p.lines[1].a];//1
            PointD rC = plhdr.points[p.lines[2].a];//2


            double intersect = -1;
            Vector edge1 = new Vector(rB.x - rA.x, rB.y - rA.y, rB.z - rA.z);
            Vector edge2 = new Vector(rC.x - rA.x, rC.y - rA.y, rC.z - rA.z);
            Vector n1 = Rd.cross(edge2);//векторное произведение
            double a = edge1.dot(n1);
            if ( Math.Abs( a ) < eps) 
                return double.MinValue;

            double f = 1.0f / a;
            Vector v1 = new Vector(Ro.x - rA.x, Ro.y - rA.y, Ro.z - rA.z); ;
            double u = f * v1.dot(n1);
            if (u < 0 || u > 1)
                return double.MinValue;

            Vector n2 = v1.cross(edge1);
            double v = f * Rd.dot(n2);
            if (v < 0 || u + v > 1)
                return double.MinValue;

            // где находится точка пересечения на линии.
            double t = f * edge2.dot( n2);
            if (t > eps)
            {
                intersect = t;
                return intersect;
            }
            else return double.MinValue;//есть пересечение линий, но не пересечение лучей.
        }

        private double eps = 0.1;
       List<double> IntersectRaySphere(Vector O, Vector D, Sphere sphere)
       {
            PointD C = sphere.center;
            double r = sphere.radius;
            Vector OC = new Vector(O.x - C.x, O.y - C.y, O.z - C.z);

            double k1 = D.dot(D);
            double k2 = 2.0 * OC.dot(D);
            double k3 = OC.dot(OC) - r * r;

            double discriminant = k2 * k2 - 4.0 * k1 * k3;
            if (discriminant < 0)
                return new List<double>() { double.MinValue, double.MinValue };

            double t1 = (-k2 + Math.Sqrt(discriminant)) / (2.0 * k1);
            double t2 = (-k2 - Math.Sqrt(discriminant)) / (2.0 * k1);
            return new List<double>() { t1, t2 };
       }

        public Color RecursiveRayTrace(Vector O, Vector D, double t_min, double t_max, int depth)
        {
            Polygon closest_poly = null;
            Sphere closest_sphere = null;
            double closest_t = ClosestIntersect(O, D, out closest_sphere, out closest_poly, t_min, t_max);

            if (closest_sphere == null && closest_poly == null)
                return background;
            Color local = background;
            double r = 0.0;
            Vector N = new Vector(0,0,0);
            Vector P = new Vector(
                O.x + closest_t * D.x,
                O.y + closest_t * D.y,
                O.z + closest_t * D.z);
            if (closest_poly != null)
            {
                r = closest_poly.reflective;
                N = closest_poly.normal;
                N.normalize();
                local =  Color.FromArgb(
                    Math.Min(255, Math.Max(0, (int)(closest_poly.color.R * EvalLightning(P, N, -D, closest_poly.specular)))),
                    Math.Min(255, Math.Max(0, (int)(closest_poly.color.G * EvalLightning(P, N, -D, closest_poly.specular)))),
                    Math.Min(255, Math.Max(0, (int)(closest_poly.color.B * EvalLightning(P, N, -D, closest_poly.specular)))));
            }
            if (closest_sphere != null)
            {
                r = closest_sphere.reflective;
                N = new Vector(
                    P.x - closest_sphere.center.x,
                    P.y - closest_sphere.center.y,
                    P.z - closest_sphere.center.z);
                N.normalize();
                local =  Color.FromArgb(
                    Math.Min(255, Math.Max(0, (int)(closest_sphere.color.R * EvalLightning(P, N, -D, closest_sphere.specular)))),
                    Math.Min(255, Math.Max(0, (int)(closest_sphere.color.G * EvalLightning(P, N, -D, closest_sphere.specular)))),
                    Math.Min(255, Math.Max(0, (int)(closest_sphere.color.B * EvalLightning(P, N, -D, closest_sphere.specular)))));
            }


            if (depth <= 0 || r <= 0)
                return local;
            Vector ReflectionRay = ReflectRay(-D, N);
            Color reflected_color = RecursiveRayTrace(P, ReflectionRay, 0.001, double.MaxValue, depth - 1);
            //return local_color * (1 - r) + reflected_color * r

            return Color.FromArgb(Math.Min(255, Math.Max(0, (int)(local.R * (1 - r) + reflected_color.R *r))),
                    Math.Min(255, Math.Max(0, (int)(local.G * (1 - r) + reflected_color.G * r))),
                    Math.Min(255, Math.Max(0, (int)(local.B * (1 - r) + reflected_color.B * r))));
        }

        public Vector ReflectRay(Vector R, Vector N)
        {
            return 2* N.dot(R) * N  - R;
        }
    }
}

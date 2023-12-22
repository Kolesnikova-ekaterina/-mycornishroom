using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace cornishroom
{
    public class Sphere
    {
        public PointD center;
        public double radius;
        public Color color;
        public double specular;
        public double reflective;
        public double transparency = 0.3;
        public Sphere(PointD p, double r, Color c, double s, double reflect)
        {
            center = p;
            radius = r;
            color = c;
            specular = s;
            reflective = reflect;
        }
    }
    public enum objecttype { walls, polyhedron }
    public class Polyhedra 
    {
        public Color color;
        public List<Polygon> polygons;
        public List<PointD> points;
        public PointD center;
        public bool is_specular;
        public bool is_transparent;
        public double specular;
        public objecttype Objecttype;
        public double reflective;
        public double transparency = 0;

        public Polyhedra(Color c, List<Polygon> polys, List<PointD> ps, bool isspecular, bool istransparent )
        {
            color = c;
            polygons = polys;
            points = ps;
            is_specular = isspecular;
            is_transparent = istransparent;
            find_center();
            find_normals();
        }

        public Polyhedra(string filename, Vector shift, Color c, double s, objecttype ot, double reflect)
        {
            polygons = new List<Polygon>();
            points = new List<PointD>();
            color = c;
            specular = s;
            Objecttype = ot;
            reflective = reflect;
            var sr = new StreamReader(filename);
            var text = sr.ReadToEnd();

            var lines = text.Split('\n');
            List<Vector> ns = new List<Vector>();
            Regex rv = new Regex(@"v\s*(?<first>[0-9.-]+(,[0-9.-]*)?) (?<second>[0-9.-]+(,[0-9.-]*)?) (?<third>[0-9.-]+(,[0-9.-]*)?)");
            Regex rf = new Regex(@"\s[0-9]+(\/[0-9]*)?(\/[0-9]+)?");
            for (int i = 0; i < lines.Length; i++)
            {
                if (rv.IsMatch(lines[i]))
                {
                    var m = rv.Match(lines[i]);

                    double x =  Convert.ToDouble(m.Groups["first"].ToString().Replace('.', ',')) + shift.x;
                    double y =  Convert.ToDouble(m.Groups["second"].ToString().Replace('.', ',')) + shift.y;
                    double z =  Convert.ToDouble(m.Groups["third"].ToString().Replace('.', ',')) + shift.z;
                    PointD p = new PointD(x, y, z);

                    points.Add(p);

                }

                if (lines[i].StartsWith("f "))
                {
                    var m = rf.Matches(lines[i]);

                    List<Line> ll = new List<Line>();

                    for (int j = 0; j < m.Count; j++)
                    {
                        int v1 = Convert.ToInt32(m[j % m.Count].ToString().Split('/')[0]) - 1;
                        int v2 = Convert.ToInt32(m[(j + 1) % m.Count].ToString().Split('/')[0]) - 1;
                        ll.Add(new Line(v1, v2));
                    }

                    polygons.Add(new Polygon(ll, color, specular, Objecttype, reflective, transparency)) ;
                }

            }
            /*for (int i = 0; i < ns.Count; i++)
            {
                polygons[i].normal = ns[i];
            }*/

            find_center();
            find_normals();
        }

        public void find_center()
        {
            double res_x = 0;
            double res_y = 0;
            double res_z = 0;

            points.ForEach(x =>
            {
                res_x += x.x;
                res_y += x.y;
                res_z += x.z;
            });

            center = new PointD(res_x / points.Count, res_y / points.Count, res_z / points.Count);

        }

        public void find_normals()
        {
            for (int i= 0; i< polygons.Count; i++)
            {
                polygons[i].find_normal(points, center);
                polygons[i].normal.normalize();
            }
        }

        public void scale(double kx, double ky, double kz)
        {
            double[,] matrixScale = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            matrixScale[0, 0] = kx;
            matrixScale[1, 1] = ky;
            matrixScale[2, 2] = kz;
            for (int i = 0; i < points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { points[i].x, points[i].y, points[i].z, 1.0 } };

                var res = Form1.multipleMatrix(matrixPoint, matrixScale);

                points[i] = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            }
        }

        public void SetReflective(double reflect)
        {
            reflective = reflect;
            for (int i = 0; i < polygons.Count; i++)
            {
                polygons[i].reflective = reflect;
            }
        }
        public void SetTransparency(double tr)
        {
            transparency = tr;
            for (int i = 0; i < polygons.Count; i++)
            {
                polygons[i].transparency = tr;
            }
        }
    }

    public class PointD
    {
        public double x;
        public double y;
        public double z;

        public PointD(double xx, double yy, double zz)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public double dist(PointD other)
        {
            return Math.Sqrt(Math.Pow(x - other.x, 2) + Math.Pow(y - other.y, 2) + Math.Pow(z - other.z, 2) );
        }

    }
    public class Line
    {
        public int a;
        public int b;

        public Line(int aa, int bb)
        {
            a = aa;
            b = bb;
        }

    }

    public class Polygon
    {
        public List<Line> lines;
        public Vector normal;
        public double D;
        public bool isVisible = true;
        public Color color;
        public double specular;
        public objecttype Objecttype;
        public double reflective;
        public double transparency;

        public Polygon(List<Line> l, Color c, double s, objecttype ot, double reflect, double tr)
        {
            lines = l;
            color = c;
            specular = s;
            Objecttype = ot;
            reflective = reflect;
            transparency = tr;
        }
        public void find_normal(List<PointD> points, PointD center)
        {
            Vector v1 = new Vector(points[lines[0].a], points[lines[0].b]);
            Vector v2 = new Vector(points[lines[1].a], points[lines[1].b]);
            normal =  v1.cross(v2);
            D = -(points[lines[0].b].x * normal.x + points[lines[0].b].y * normal.y + points[lines[0].b].z * normal.z);
            if (Form1.ononeside(normal, D, center, points[lines[0].b]))
            {
                normal.reverse();
            }
        }
    }

}

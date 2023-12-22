using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cornishroom
{
    public partial class Form1 : Form
    {

        Polyhedra Walls;
        Polyhedra Tetr;
        Polyhedra Cube;
        Sphere Sphere;
        Sphere Sphere1;

        light ambient;
        light directional;
        List<light> pointslight;
        List<light> pointslight_extra;

        List<light> lightsourses;
        public Form1()
        {
            InitializeComponent();
            pointslight_extra = new List<light>();
            Walls = new Polyhedra("../../wallsagain.obj", new Vector(0, 0.5, 3.5), Color.Aqua, -1, objecttype.walls, 0.0);
            List<Color> colors = new List<Color>()
            {
                Color.AntiqueWhite, Color.Aqua, Color.CornflowerBlue, Color.LightBlue, Color.Cornsilk, Color.Lavender
            };
            for (int i = 0; i< Walls.polygons.Count; i++)
            {
                Walls.polygons[i].normal.reverse();
                Walls.polygons[i].color = colors[i/2];

            }

            Tetr = new Polyhedra("../../tetr.obj", new Vector(1, 0.5, 5), Color.BlueViolet, 10, objecttype.polyhedron, 0.0);
            Cube = new Polyhedra("../../modelcube.obj", new Vector(-1, -2, 5), Color.DarkSalmon, 100, objecttype.polyhedron, 0.0);

            Sphere = new Sphere(new PointD(-1, -0.7, 5 ), 0.7 , Color.DeepPink, 500, 0.0);
            Sphere1 = new Sphere(new PointD(1, -2.0, 5), 0.5, Color.LightPink, 120, 0.0);

            ambient = new light(0.2);
            pointslight = new List<light>() { new light(0.15, new PointD(0.95,3,3) ), new light(0.15, new PointD(1, 3,  3)), new light(0.15, new PointD(1, 3,  2.95)), new light(0.15, new PointD(0.95, 3, 2.95)) };
            directional = new light(0.2, new Vector(0, 5, -1));
            lightsourses = new List<light>();
        }
        public static double[,] multipleMatrix(double[,] a, double[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;

        }
        public static bool ononeside(Vector n, double D, PointD center, PointD p)
        {
            return ((p.x + 2 * n.x) * n.x + (p.y + 2 * n.y) * n.y + (p.z + 2 * n.z) * n.z + D) * (n.x * center.x + n.y * center.y + n.z * center.z + D) > 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckTheMirrorable();
            lightsourses.Clear();
            lightsourses.AddRange(pointslight);
            lightsourses.AddRange(pointslight_extra);
            lightsourses.Add(ambient);
            //lightsourses.Add(directional);
            var rayTracing = new RayTracing(pictureBox1.Width, pictureBox1.Height, new List<Polyhedra> { Walls, Cube, Tetr }, Color.Gray, new List<Sphere>() {Sphere, Sphere1 }, lightsourses);
            Bitmap image = rayTracing.ApplyRayTracing();
            pictureBox1.Image = image;


        }
        public void CheckTheMirrorable()
        {
            if (checkBox1.Checked)
            {
                Walls.polygons[0].reflective = 1.0;
                Walls.polygons[1].reflective = 1.0;
            } else
            {
                Walls.polygons[0].reflective = 0.0;
                Walls.polygons[1].reflective = 0.0;
            }
            if (checkBox2.Checked)
            {
                Walls.polygons[2].reflective = 1.0;
                Walls.polygons[3].reflective = 1.0;
            }
            else
            {
                Walls.polygons[2].reflective = 0.0;
                Walls.polygons[3].reflective = 0.0;
            }
            if (checkBox3.Checked)
            {
                Walls.polygons[10].reflective = 1.0;
                Walls.polygons[11].reflective = 1.0;
            }
            else
            {
                Walls.polygons[10].reflective = 0.0;
                Walls.polygons[11].reflective = 0.0;
            }

            if (checkBox4.Checked)
            {
                Walls.polygons[4].reflective = 1.0;
                Walls.polygons[5].reflective = 1.0;
            }
            else
            {
                Walls.polygons[4].reflective = 0.0;
                Walls.polygons[5].reflective = 0.0;
            }

            if (checkBox5.Checked)
            {
                Walls.polygons[6].reflective = 1.0;
                Walls.polygons[7].reflective = 1.0;
            }
            else
            {
                Walls.polygons[6].reflective = 0.0;
                Walls.polygons[7].reflective = 0.0;
            }

            if (checkBox6.Checked)
            {
                Walls.polygons[8].reflective = 1.0;
                Walls.polygons[9].reflective = 1.0;
            }
            else
            {
                Walls.polygons[8].reflective = 0.0;
                Walls.polygons[9].reflective = 0.0;
            }
            if(checkBox7.Checked)
            {
                Sphere.reflective = 1.0;
            }
            else
            {
                Sphere.reflective = 0.0;
            }
            if (checkBox8.Checked)
            {
                Sphere1.reflective = 1.0;
            }
            else
            {
                Sphere1.reflective = 0.0;

            }
            if (checkBox9.Checked)
            {
                Tetr.SetReflective( 1.0);
            }
            else
            {
                Tetr.SetReflective(0.0);
            }

            if (checkBox10.Checked)
            {
                Cube.SetReflective(1.0);
            }
            else
            {
                Cube.SetReflective(0.0);
            }

            if (textBox1.Text !="" && textBox2.Text != "" && textBox3.Text != ""  )
            {
                double x1 = Convert.ToDouble(textBox1.Text.Replace('.', ','));
                double y1 = Convert.ToDouble(textBox2.Text.Replace('.', ','));
                double z1 = Convert.ToDouble(textBox3.Text.Replace('.', ','));

                pointslight_extra.Add(new light(0.6, new PointD(x1, y1, z1))); 
            }

        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            pointslight_extra.Clear();
        }
    }
}

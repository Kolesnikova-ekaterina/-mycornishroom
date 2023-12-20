using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cornishroom
{
    class Camera
    {
        public PointD position;
        public Vector view;
        public double[,] matrixPerspectieve = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, -1f / -200 }, { 0, 0, 0, 1 } };
        public double[,] matrixToCamera = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 0.8, -0.5, 0 }, { 0, 0.5, 0.86, 0 }, { 0, 0, 0, 1 } };
        static float teta = (float)(5 * Math.PI / 180.0);

        public Camera()
        {
            position = new PointD(-100, -136, -36);
            view = new Vector(0.99, 0.09, -0.04);
        }
        public void Perspective(Graphics g, Polyhedra p, Vector shift)
        {
            
            var newimage = new List<PointD>();
            double[,] helpMatrix = Form1.multipleMatrix(matrixToCamera, this.matrixPerspectieve);
            for (int i = 0; i < p.points.Count; i++)
            {
                double[,] matrixPoint = new double[1, 4] { { p.points[i].x, p.points[i].y, p.points[i].z, 1.0 } };
                var res = Form1.multipleMatrix(matrixPoint, helpMatrix);
                double c = 10.0;
                res[0, 0] /= 1.0 - res[0, 3] / c;
                res[0, 1] /= 1.0 - res[0, 3] / c;
                newimage.Add(new PointD(res[0, 0], res[0, 1], res[0, 2]));
            }
            foreach(var poly in p.polygons) {
                if (!poly.isVisible)
                    continue;
                for (int i = 0; i < poly.lines.Count(); i++)
                {
                    Point a = new Point((int)(newimage[poly.lines[i].a].x) + (int)shift.x / 3 , (int)(newimage[poly.lines[i].a].y) + (int)shift.y / 3);
                    Point b = new Point((int)(newimage[poly.lines[i].b].x) + (int)shift.x / 3 , (int)(newimage[poly.lines[i].b].y) + (int)shift.y / 3);
                    g.DrawLine(new Pen(Color.Black, 2.0f), a, b);
                }
            }
        }
        void RotateCamera(double[,] matrixRotate)
        {

            matrixToCamera = Form1.multipleMatrix(matrixToCamera, matrixRotate);
            double[,] matrixPoint = new double[1, 4] { { position.x, position.y, position.z, 1.0 } };

            var res = (Form1.multipleMatrix(matrixPoint, matrixRotate));
            position = new PointD(res[0, 0], res[0, 1], res[0, 2]);
            Console.WriteLine($"pos: {position.x} {position.y} {position.z}");

            double[,] matrixviev = new double[1, 4] { { view.x, view.y, view.z, 1.0 } };
            res = (Form1.multipleMatrix(matrixviev, matrixRotate));
            view = new Vector(res[0, 0], res[0, 1], res[0, 2]);
            view.reverse();
            view.normalize();
            Console.WriteLine($"view: {view.x} {view.y} {view.z}");

        }
        public void RotateDown()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 1 } };
            currentRotate[1, 1] = Math.Cos(teta);
            currentRotate[1, 2] = Math.Sin(teta);
            currentRotate[2, 1] = -Math.Sin(teta);
            currentRotate[2, 2] = Math.Cos(teta);
            RotateCamera(currentRotate);

        }
        public void RotateUp()
        {
            var currentRotate = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 1 } };
            currentRotate[1, 1] = Math.Cos(-teta);
            currentRotate[1, 2] = Math.Sin(-teta);
            currentRotate[2, 1] = -Math.Sin(-teta);
            currentRotate[2, 2] = Math.Cos(-teta);
            RotateCamera(currentRotate);
        }

    }
}

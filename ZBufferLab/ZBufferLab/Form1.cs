using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ZBufferLab
{ 
    partial class MainForm : Form
    {
        private Timer timer;
        private List<Object3D> objects;
        private float angleX = 0;
        private float angleY = 0;
        private float angleZ = 0;
        private float scale = 1.0f;
        private const int ZBufferWidth = 800;
        private const int ZBufferHeight = 600;
        private float[,] zBuffer;
        private float rotationSpeed = 1.0f;

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = ZBufferWidth;
            this.Height = ZBufferHeight;
            this.BackColor = Color.White;

            this.KeyPreview = true;

            this.KeyDown += MainForm_KeyDown;
            this.Paint += MainForm_Paint;

            InitializeObjects();
            zBuffer = new float[ZBufferWidth, ZBufferHeight];
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void InitializeObjects()
        {
            objects = new List<Object3D>();

            Object3D prism = new Object3D();
            prism.Vertices.Add(new Point3D(-100, -100, -100));
            prism.Vertices.Add(new Point3D(100, -100, -100));
            prism.Vertices.Add(new Point3D(100, 100, -100));
            prism.Vertices.Add(new Point3D(-100, 100, -100));
            prism.Vertices.Add(new Point3D(-100, -100, 100));
            prism.Vertices.Add(new Point3D(100, -100, 100));
            prism.Vertices.Add(new Point3D(100, 100, 100));
            prism.Vertices.Add(new Point3D(-100, 100, 100));

            prism.Faces.Add(new Face(new int[] { 0, 1, 2, 3 }, Color.Red));
            prism.Faces.Add(new Face(new int[] { 4, 5, 6, 7 }, Color.Green));
            prism.Faces.Add(new Face(new int[] { 0, 1, 5, 4 }, Color.Blue));
            prism.Faces.Add(new Face(new int[] { 1, 2, 6, 5 }, Color.Yellow));
            prism.Faces.Add(new Face(new int[] { 2, 3, 7, 6 }, Color.Orange));
            prism.Faces.Add(new Face(new int[] { 3, 0, 4, 7 }, Color.Purple));

            objects.Add(prism);

            Object3D parallelepiped = new Object3D();
            parallelepiped.Vertices.Add(new Point3D(-150, -50, -50));
            parallelepiped.Vertices.Add(new Point3D(150, -50, -50));
            parallelepiped.Vertices.Add(new Point3D(150, 50, -50));
            parallelepiped.Vertices.Add(new Point3D(-150, 50, -50));
            parallelepiped.Vertices.Add(new Point3D(-150, -50, 50));
            parallelepiped.Vertices.Add(new Point3D(150, -50, 50));
            parallelepiped.Vertices.Add(new Point3D(150, 50, 50));
            parallelepiped.Vertices.Add(new Point3D(-150, 50, 50));

            parallelepiped.Faces.Add(new Face(new int[] { 0, 1, 2, 3 }, Color.Cyan));
            parallelepiped.Faces.Add(new Face(new int[] { 4, 5, 6, 7 }, Color.Magenta));
            parallelepiped.Faces.Add(new Face(new int[] { 0, 1, 5, 4 }, Color.Lime));
            parallelepiped.Faces.Add(new Face(new int[] { 1, 2, 6, 5 }, Color.Teal));
            parallelepiped.Faces.Add(new Face(new int[] { 2, 3, 7, 6 }, Color.Navy));
            parallelepiped.Faces.Add(new Face(new int[] { 3, 0, 4, 7 }, Color.Maroon));

            objects.Add(parallelepiped);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            angleX += rotationSpeed;  // Используем скорость вращения
            angleY += rotationSpeed;
            angleZ += rotationSpeed;
            this.Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            DrawCoordinateAxes(g);
            InitializeBuffer();

            foreach (var obj in objects)
            {
                obj.Rotate(angleX, angleY, angleZ);
            }

            List<FaceProjection> projectedFaces = new List<FaceProjection>();

            foreach (var obj in objects)
            {
                foreach (var face in obj.Faces)
                {
                    List<Point3D> faceVertices = new List<Point3D>();
                    foreach (var index in face.FaceVertices)
                    {
                        faceVertices.Add(obj.Vertices[index]);
                    }
                    float avgZ = 0;
                    foreach (var vertex in faceVertices)
                    {
                        avgZ += vertex.Z;
                    }
                    avgZ /= faceVertices.Count;

                    List<PointF> projected = new List<PointF>();
                    foreach (var vertex in faceVertices)
                    {
                        float x = vertex.X * scale + ZBufferWidth / 2;
                        float y = -vertex.Y * scale + ZBufferHeight / 2;
                        projected.Add(new PointF(x, y));
                    }

                    projectedFaces.Add(new FaceProjection
                    {
                        Points = projected,
                        Depth = avgZ,
                        Color = face.Color
                    });
                }
            }

            
            projectedFaces.Sort((f1, f2) => f2.Depth.CompareTo(f1.Depth));

          
            foreach (var faceProj in projectedFaces)
            {
                Brush brush = new SolidBrush(Color.FromArgb(100, faceProj.Color));
                Pen pen = new Pen(faceProj.Color, 1);
                g.FillPolygon(brush, faceProj.Points.ToArray());
                g.DrawPolygon(pen, faceProj.Points.ToArray());
            }
        }

        private void InitializeBuffer()
        {
            for (int i = 0; i < ZBufferWidth; i++)
                for (int j = 0; j < ZBufferHeight; j++)
                    zBuffer[i, j] = float.MaxValue;
        }

        private void DrawCoordinateAxes(Graphics g)
        {
            Pen axisPen = new Pen(Color.Black, 2);
            // X-axis
            g.DrawLine(axisPen, ZBufferWidth / 2, ZBufferHeight / 2, ZBufferWidth, ZBufferHeight / 2);
            // Y-axis
            g.DrawLine(axisPen, ZBufferWidth / 2, ZBufferHeight / 2, ZBufferWidth / 2, 0);
            // Z-axis
            g.DrawLine(axisPen, ZBufferWidth / 2, ZBufferHeight / 2, ZBufferWidth / 2 + 100, ZBufferHeight / 2 + 100);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    angleX -= 5.0f;
                    break;
                case Keys.Down:
                    angleX += 5.0f;
                    break;
                case Keys.Left:
                    angleY -= 5.0f;
                    break;
                case Keys.Right:
                    angleY += 5.0f;
                    break;
                case Keys.PageUp:
                    angleZ -= 5.0f;
                    break;
                case Keys.PageDown:
                    angleZ += 5.0f;
                    break;
                case Keys.Oemplus:  // Для клавиши "+"
                    scale += 0.1f;
                    if (scale > 3.0f)
                        scale = 3.0f;
                    break;
                case Keys.OemMinus:  // Для клавиши "-"
                    scale -= 0.1f;
                    if (scale < 0.5f)
                        scale = 0.5f;
                    break;
                case Keys.S:  // Клавиша "S" для уменьшения скорости вращения
                    rotationSpeed = Math.Max(0.1f, rotationSpeed - 0.1f); // Уменьшаем скорость вращения, но не ниже 0.1
                    break;
            }
            this.Invalidate();
        }

    }

    public class Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Rotate(float angleX, float angleY, float angleZ)
        {
            // Rotation around X-axis
            float radX = angleX * (float)Math.PI / 180.0f;
            float cosa = (float)Math.Cos(radX);
            float sina = (float)Math.Sin(radX);
            float y1 = Y * cosa - Z * sina;
            float z1 = Y * sina + Z * cosa;
            Y = y1;
            Z = z1;

            // Rotation around Y-axis
            float radY = angleY * (float)Math.PI / 180.0f;
            cosa = (float)Math.Cos(radY);
            sina = (float)Math.Sin(radY);
            float x1 = X * cosa + Z * sina;
            float z2 = -X * sina + Z * cosa;
            X = x1;
            Z = z2;

            // Rotation around Z-axis
            float radZ = angleZ * (float)Math.PI / 180.0f;
            cosa = (float)Math.Cos(radZ);
            sina = (float)Math.Sin(radZ);
            float x2 = X * cosa - Y * sina;
            float y2 = X * sina + Y * cosa;
            X = x2;
            Y = y2;
        }
    }

    public class Face
    {
        public int[] FaceVertices { get; set; }
        public Color Color { get; set; }

        public Face(int[] vertices, Color color)
        {
            FaceVertices = vertices;
            Color = color;
        }
    }

    public class Object3D
    {
        public List<Point3D> Vertices { get; set; }
        public List<Face> Faces { get; set; }

        public Object3D()
        {
            Vertices = new List<Point3D>();
            Faces = new List<Face>();
        }

        public void Rotate(float angleX, float angleY, float angleZ)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Rotate(angleX, angleY, angleZ);
            }
        }

        public List<PointF> Project(float scale, int centerX, int centerY)
        {
            List<PointF> projections = new List<PointF>();
            foreach (var vertex in Vertices)
            {
                float x = vertex.X * scale + centerX;
                float y = -vertex.Y * scale + centerY;
                projections.Add(new PointF(x, y));
            }
            return projections;
        }
    }

    public class FaceProjection
    {
        public List<PointF> Points { get; set; }
        public float Depth { get; set; }
        public Color Color { get; set; }
    }
}

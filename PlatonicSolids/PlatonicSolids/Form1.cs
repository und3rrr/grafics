using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlatonicSolids
{

    public class PlatonicSolids : Form
    {
        private ComboBox projectionTypeComboBox;

        public PlatonicSolids()
        {
            this.Text = "Гексаэдр и Октаэдр";
            this.Size = new Size(800, 600);

            projectionTypeComboBox = new ComboBox();
            projectionTypeComboBox.Items.Add("Центральная проекция");
            projectionTypeComboBox.Items.Add("Ортогональная проекция");
            projectionTypeComboBox.SelectedIndex = 0;
            projectionTypeComboBox.Dock = DockStyle.Top;
            projectionTypeComboBox.SelectedIndexChanged += (s, e) => this.Invalidate();

            this.Controls.Add(projectionTypeComboBox);
            this.Paint += OnPaint;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            string projectionType = projectionTypeComboBox.SelectedItem.ToString();

            // Массивы вершин для гексаэдра и октаэдра
            Point3D[] hexahedronVertices = {
            new Point3D(-1, -1, -1), new Point3D(1, -1, -1),
            new Point3D(1, 1, -1), new Point3D(-1, 1, -1),
            new Point3D(-1, -1, 1), new Point3D(1, -1, 1),
            new Point3D(1, 1, 1), new Point3D(-1, 1, 1)
        };
            int[][] hexahedronEdges = {
            new int[] {0, 1}, new int[] {1, 2}, new int[] {2, 3}, new int[] {3, 0},
            new int[] {4, 5}, new int[] {5, 6}, new int[] {6, 7}, new int[] {7, 4},
            new int[] {0, 4}, new int[] {1, 5}, new int[] {2, 6}, new int[] {3, 7}
        };

            Point3D[] octahedronVertices = {
            new Point3D(0, 0, -1), new Point3D(1, 0, 0),
            new Point3D(0, 1, 0), new Point3D(-1, 0, 0),
            new Point3D(0, -1, 0), new Point3D(0, 0, 1)
        };
            int[][] octahedronEdges = {
            new int[] {0, 1}, new int[] {0, 2}, new int[] {0, 3}, new int[] {0, 4},
            new int[] {5, 1}, new int[] {5, 2}, new int[] {5, 3}, new int[] {5, 4},
            new int[] {1, 2}, new int[] {2, 3}, new int[] {3, 4}, new int[] {4, 1}
        };

            DrawSolid(g, hexahedronVertices, hexahedronEdges, projectionType);
            DrawSolid(g, octahedronVertices, octahedronEdges, projectionType);
        }

        private void DrawSolid(Graphics g, Point3D[] vertices, int[][] edges, string projectionType)
        {
            Point center = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            float scale = 100;

            foreach (var edge in edges)
            {
                Point p1 = Project(vertices[edge[0]], projectionType, center, scale);
                Point p2 = Project(vertices[edge[1]], projectionType, center, scale);
                g.DrawLine(Pens.Black, p1, p2);
            }
        }

        private Point Project(Point3D point, string projectionType, Point center, float scale)
        {
            float x = point.X * scale;
            float y = point.Y * scale;
            float z = point.Z * scale;

            if (projectionType == "Центральная проекция")
            {
                float distance = 5;
                x = x / (z / distance + 1);
                y = y / (z / distance + 1);
            }

            return new Point((int)(center.X + x), (int)(center.Y - y));
        }

        public class Point3D
        {
            public float X, Y, Z;

            public Point3D(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CurveSurfaceBuilder
{
    public class MainForm : Form
    {
        private enum CurveType
        {
            BezierQuadratic,
            BezierCubic,
            Chaikin,
            DoSabin,
            BezierSurface
        }

        private List<PointF> controlPoints = new List<PointF>();
        private List<PointF> refinedPoints = new List<PointF>();
        private CurveType currentCurve = CurveType.BezierQuadratic;
        private int subdivisions = 1;

        private Button btnBezierQuad;
        private Button btnBezierCubic;
        private Button btnChaikin;
        private Button btnDoSabin;
        private Button btnClear;
        private Label lblSubdivision;
        private NumericUpDown numSubdivision;

        public MainForm()
        {
            this.Text = "Curve and Surface Builder";
            this.Width = 800;
            this.Height = 600;
            this.DoubleBuffered = true;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            btnBezierQuad = new Button() { Text = "Bezier Quadratic", Left = 10, Top = 10, Width = 120 };
            btnBezierQuad.Click += (s, e) => { currentCurve = CurveType.BezierQuadratic; this.Invalidate(); };

            btnBezierCubic = new Button() { Text = "Bezier Cubic", Left = 140, Top = 10, Width = 120 };
            btnBezierCubic.Click += (s, e) => { currentCurve = CurveType.BezierCubic; this.Invalidate(); };

            btnChaikin = new Button() { Text = "Chaikin's Curve", Left = 270, Top = 10, Width = 120 };
            btnChaikin.Click += (s, e) => { currentCurve = CurveType.Chaikin; this.Invalidate(); };

            btnDoSabin = new Button() { Text = "Do-Sabin Surface", Left = 400, Top = 10, Width = 120 };
            btnDoSabin.Click += (s, e) => { currentCurve = CurveType.DoSabin; this.Invalidate(); };

            btnClear = new Button() { Text = "Clear", Left = 530, Top = 10, Width = 80 };
            btnClear.Click += (s, e) => { controlPoints.Clear(); refinedPoints.Clear(); this.Invalidate(); };

            btnDoSabin = new Button() { Text = "Do-Sabin", Left = 400, Top = 10, Width = 120 };
            btnDoSabin.Click += (s, e) => { currentCurve = CurveType.DoSabin; this.Invalidate(); }; this.Controls.Add(btnDoSabin);

            lblSubdivision = new Label() { Text = "Subdivisions:", Left = 620, Top = 15, Width = 80 };
            numSubdivision = new NumericUpDown() { Left = 700, Top = 12, Width = 60, Minimum = 1, Maximum = 10, Value = 1 };
            numSubdivision.ValueChanged += (s, e) => { subdivisions = (int)numSubdivision.Value; this.Invalidate(); };

            this.Controls.Add(btnBezierQuad);
            this.Controls.Add(btnBezierCubic);
            this.Controls.Add(btnChaikin);
            this.Controls.Add(btnDoSabin);
            this.Controls.Add(btnClear);
            this.Controls.Add(lblSubdivision);
            this.Controls.Add(numSubdivision);

            this.MouseClick += MainForm_MouseClick;
            this.Paint += MainForm_Paint;
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                controlPoints.Add(e.Location);
                this.Invalidate();
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Draw original control points
            foreach (var pt in controlPoints)
            {
                g.FillEllipse(Brushes.Red, pt.X - 3, pt.Y - 3, 6, 6);
            }

            // Draw control polygon
            if (controlPoints.Count > 1)
            {
                g.DrawLines(Pens.Gray, controlPoints.ToArray());
            }

            // Compute and draw refined points
            refinedPoints.Clear();
            if (controlPoints.Count >= 3)
            {
                switch (currentCurve)
                {
                    case CurveType.BezierQuadratic:
                        refinedPoints = GetBezierQuadratic(controlPoints, subdivisions);
                        break;
                    case CurveType.BezierCubic:
                        refinedPoints = GetBezierCubic(controlPoints, subdivisions);
                        break;
                    case CurveType.Chaikin:
                        refinedPoints = GetChaikin(controlPoints, subdivisions);
                        break;
                    case CurveType.DoSabin:
                        refinedPoints = GetDoSabin(controlPoints, subdivisions);
                        break;
                }

                foreach (var pt in refinedPoints)
                {
                    g.FillEllipse(Brushes.Blue, pt.X - 3, pt.Y - 3, 6, 6);
                }

                // Draw refined control polygon
                if (refinedPoints.Count > 1)
                {
                    g.DrawLines(Pens.Blue, refinedPoints.ToArray());
                }

                // Draw curve
                if (refinedPoints.Count > 1)
                {
                    g.DrawCurve(Pens.Green, refinedPoints.ToArray());
                }
            }
        }

        private List<PointF> GetBezierQuadratic(List<PointF> points, int subdiv)
        {
            List<PointF> curve = new List<PointF>();
            for (int i = 0; i < points.Count - 2; i += 2)
            {
                for (int j = 0; j <= subdiv; j++)
                {
                    float t = (float)j / subdiv;
                    PointF pt = CalculateQuadraticBezier(points[i], points[i + 1], points[i + 2], t);
                    curve.Add(pt);
                }
            }
            return curve;
        }

        private List<PointF> GetDoSabin(List<PointF> points, int subdiv)
        {
            List<PointF> result = new List<PointF>(points);
            for (int n = 0; n < subdiv; n++)
            {
                List<PointF> temp = new List<PointF>();
                for (int i = 0; i < result.Count - 1; i++)
                {
                    // Создание новых точек Q и R
                    PointF q = new PointF(0.75f * result[i].X + 0.25f * result[i + 1].X,
                                         0.75f * result[i].Y + 0.25f * result[i + 1].Y);
                    PointF r = new PointF(0.25f * result[i].X + 0.75f * result[i + 1].X,
                                         0.25f * result[i].Y + 0.75f * result[i + 1].Y);
                    temp.Add(q);
                    temp.Add(r);
                }
                result = temp;
            }
            return result;
        }

        private PointF CalculateQuadraticBezier(PointF p0, PointF p1, PointF p2, float t)
        {
            float x = (1 - t) * (1 - t) * p0.X + 2 * (1 - t) * t * p1.X + t * t * p2.X;
            float y = (1 - t) * (1 - t) * p0.Y + 2 * (1 - t) * t * p1.Y + t * t * p2.Y;
            return new PointF(x, y);
        }

        private List<PointF> GetBezierCubic(List<PointF> points, int subdiv)
        {
            List<PointF> curve = new List<PointF>();
            for (int i = 0; i < points.Count - 3; i += 3)
            {
                for (int j = 0; j <= subdiv; j++)
                {
                    float t = (float)j / subdiv;
                    PointF pt = CalculateCubicBezier(points[i], points[i + 1], points[i + 2], points[i + 3], t);
                    curve.Add(pt);
                }
            }
            return curve;
        }

        private PointF CalculateCubicBezier(PointF p0, PointF p1, PointF p2, PointF p3, float t)
        {
            float x = (float)(Math.Pow(1 - t, 3) * p0.X +
                              3 * Math.Pow(1 - t, 2) * t * p1.X +
                              3 * (1 - t) * Math.Pow(t, 2) * p2.X +
                              Math.Pow(t, 3) * p3.X);
            float y = (float)(Math.Pow(1 - t, 3) * p0.Y +
                              3 * Math.Pow(1 - t, 2) * t * p1.Y +
                              3 * (1 - t) * Math.Pow(t, 2) * p2.Y +
                              Math.Pow(t, 3) * p3.Y);
            return new PointF(x, y);
        }

        private List<PointF> GetChaikin(List<PointF> points, int subdiv)
        {
            List<PointF> result = new List<PointF>(points);
            for (int n = 0; n < subdiv; n++)
            {
                List<PointF> temp = new List<PointF>();
                for (int i = 0; i < result.Count - 1; i++)
                {
                    PointF q = new PointF(0.75f * result[i].X + 0.25f * result[i + 1].X,
                                         0.75f * result[i].Y + 0.25f * result[i + 1].Y);
                    PointF r = new PointF(0.25f * result[i].X + 0.75f * result[i + 1].X,
                                         0.25f * result[i].Y + 0.75f * result[i + 1].Y);
                    temp.Add(q);
                    temp.Add(r);
                }
                result = temp;
            }
            return result;
        }
    }
}
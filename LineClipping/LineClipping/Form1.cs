using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LineClipping
{
    public partial class MainForm : Form
    {
        private List<Line> lines = new List<Line>();
        private Rectangle clipWindow;

        public MainForm()
        {
            InitializeComponent();
            this.Paint += MainForm_Paint;
            this.MouseDown += MainForm_MouseDown;

            // Установим окно отсечения по умолчанию
            clipWindow = new Rectangle(100, 100, 200, 150);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем окно отсечения
            g.DrawRectangle(Pens.Red, clipWindow);

            // Рисуем отрезки
            foreach (var line in lines)
            {
                g.DrawLine(Pens.Blue, line.Start, line.End);

                // Отсечение
                if (ClipLine(line, clipWindow, out Point clippedStart, out Point clippedEnd))
                {
                    g.DrawLine(Pens.Green, clippedStart, clippedEnd);
                }
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Добавление новой линии по клику
            if (e.Button == MouseButtons.Left)
            {
                var random = new Random();
                var line = new Line
                {
                    Start = new Point(random.Next(0, this.ClientSize.Width), random.Next(0, this.ClientSize.Height)),
                    End = new Point(random.Next(0, this.ClientSize.Width), random.Next(0, this.ClientSize.Height))
                };
                lines.Add(line);
                this.Invalidate();
            }
        }

        private bool ClipLine(Line line, Rectangle rect, out Point clippedStart, out Point clippedEnd)
        {
            clippedStart = line.Start;
            clippedEnd = line.End;

            // Вычисление кодов конечных точек
            int code1 = ComputeOutCode(clippedStart, rect);
            int code2 = ComputeOutCode(clippedEnd, rect);

            while (true)
            {
                if ((code1 | code2) == 0)
                {
                    // Полностью внутри
                    return true;
                }
                if ((code1 & code2) != 0)
                {
                    // Полностью вне
                    return false;
                }

                // Частично внутри
                int outCode = code1 != 0 ? code1 : code2;

                Point intersection = new Point();
                if ((outCode & 8) != 0) // Сверху
                {
                    intersection.X = clippedStart.X + (clippedEnd.X - clippedStart.X) * (rect.Top - clippedStart.Y) / (clippedEnd.Y - clippedStart.Y);
                    intersection.Y = rect.Top;
                }
                else if ((outCode & 4) != 0) // Снизу
                {
                    intersection.X = clippedStart.X + (clippedEnd.X - clippedStart.X) * (rect.Bottom - clippedStart.Y) / (clippedEnd.Y - clippedStart.Y);
                    intersection.Y = rect.Bottom;
                }
                else if ((outCode & 2) != 0) // Справа
                {
                    intersection.Y = clippedStart.Y + (clippedEnd.Y - clippedStart.Y) * (rect.Right - clippedStart.X) / (clippedEnd.X - clippedStart.X);
                    intersection.X = rect.Right;
                }
                else if ((outCode & 1) != 0) // Слева
                {
                    intersection.Y = clippedStart.Y + (clippedEnd.Y - clippedStart.Y) * (rect.Left - clippedStart.X) / (clippedEnd.X - clippedStart.X);
                    intersection.X = rect.Left;
                }

                if (outCode == code1)
                {
                    clippedStart = intersection;
                    code1 = ComputeOutCode(clippedStart, rect);
                }
                else
                {
                    clippedEnd = intersection;
                    code2 = ComputeOutCode(clippedEnd, rect);
                }
            }
        }

        private int ComputeOutCode(Point point, Rectangle rect)
        {
            int code = 0;

            if (point.X < rect.Left) code |= 1; // Слева
            if (point.X > rect.Right) code |= 2; // Справа
            if (point.Y < rect.Top) code |= 4; // Сверху
            if (point.Y > rect.Bottom) code |= 8; // Снизу

            return code;
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(800, 600);
            this.Text = "Отсечение отрезков";
        }
    }

    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

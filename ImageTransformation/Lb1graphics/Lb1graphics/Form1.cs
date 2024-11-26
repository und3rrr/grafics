using System.Drawing;
using System.Windows.Forms;
using System;

namespace Lb1graphics
{
    public partial class Form1 : Form
    {
        const int pixWidth = 1;//Ширина увеличенного пикселя
        const int pixHeight = 1;//Высота увеличенного пикселя
        Point start;
        Point end;
        algs cur;

        struct Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x, y;
        }

        enum algs
        {
            CDA,
            IntBres,
            DoubleBres
        }

        public Form1()
        {
            InitializeComponent();
            ListBoxInit();
            NumInit();
        }

        void ListBoxInit()
        {
            listBox1.Items.Add("алгоритм ЦДА");
            listBox1.Items.Add("целочисленный алгоритм Брезенхема");
            listBox1.Items.Add("алгоритм Брезенхема");
            listBox1.SetSelected(0, true);
        }

        void NumInit()
        {
            numericUpDown1.Maximum = 480 / pixWidth;
            numericUpDown3.Maximum = 480 / pixWidth;
            numericUpDown2.Maximum = 480 / pixHeight;
            numericUpDown4.Maximum = 480 / pixHeight;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start = new Point((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            end = new Point((int)numericUpDown3.Value, (int)numericUpDown4.Value);
            cur = toAlgs(listBox1.SelectedIndex);
            Draw();
        }

        algs toAlgs(int num)
        {
            switch (num)
            {
                case 0: return algs.CDA;
                case 1: return algs.IntBres;
                default: return algs.DoubleBres;
            }
        }

        //Рисование линии
        void Draw()
        {
            Bitmap bmp = new Bitmap(480, 480);
            switch (cur)
            {
                case algs.CDA: CDA(bmp); break;
                case algs.IntBres: IntBres(bmp); break;
                case algs.DoubleBres: DoubleBres(bmp); break;
            }
            pictureBox1.Image = bmp;
        }

        void CDA(Bitmap bmp)
        {
            if (start.x == end.x && start.y == end.y)
            {
                bmp.SetPixel(start.x, start.y, Color.Black);
                return;
            }
            double l;
            if (Math.Abs(end.x - start.x) > Math.Abs(end.y - start.y))
                l = Math.Abs(end.x - start.x);
            else l = Math.Abs(end.y - start.y);
            double dx = (end.x - start.x) / l, dy = (end.y - start.y) / l;
            double x = start.x + Sign(dx) * .5f;
            double y = start.y + Sign(dy) * .5f;
            for (int i = 1; i <= l + 1; i++)
            {
                DrawPixel(bmp, (int)x, (int)y, Color.Black);
                x += dx;
                y += dy;
            }
        }

        void IntBres(Bitmap bmp)
        {
            if (start.x == end.x && start.y == end.y)
            {
                bmp.SetPixel(start.x, start.y, Color.Black);
                return;
            }
            double dx = end.x - start.x;
            double dy = end.y - start.y;
            int sx = Sign(dx);
            int sy = Sign(dy);
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            bool flag = false;
            if (dy > dx)
            {
                double temp = dx;
                dx = dy;
                dy = temp;
                flag = true;
            }
            double f = 2 * dy - dx;
            double x = start.x, y = start.y;
            while (x <= end.x)
            {
                DrawPixel(bmp, (int)x, (int)y, Color.Black);
                if (f >= 0)
                {
                    if (flag)
                        x += sx;
                    else y += sy;
                    f -= 2 * dx;
                }
                else
                {
                    if (flag) y += sy;
                    else x += sx;
                    f += 2 * dy;
                }
            }
        }

        void DoubleBres(Bitmap bmp)
        {
            if (start.x == end.x && start.y == end.y)
            {
                bmp.SetPixel(start.x, start.y, Color.Black);
                return;
            }
            double dx = end.x - start.x;
            double dy = end.y - start.y;
            int sx = Sign(dx);
            int sy = Sign(dy);
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            bool flag = false;
            if (dy > dx)
            {
                double temp = dx;
                dx = dy;
                dy = temp;
                flag = true;
            }
            double f = dy / dx - .5f;
            double x = start.x, y = start.y;
            while (x <= end.x)
            {
                DrawPixel(bmp, (int)x, (int)y, Color.Black);
                if (f >= 0)
                {
                    if (flag)
                        x += sx;
                    else y += sy;
                    f--;
                }
                else
                {
                    if (flag) y += sy;
                    else x += sx;
                    f += dy / dx;
                }
            }
        }

        int Sign(double n)
        {
            return n > 0 ? 1 : n < 0 ? -1 : 0;
        }

        //Рисование увеличенного пискеля для наглядности
        void DrawPixel(Bitmap bitmap, int x, int y, Color col)
        {
            for (int i = 0; i < pixWidth; i++)
                for (int j = 0; j < pixHeight; j++)
                {
                    bitmap.SetPixel(x + i, y + j, col);
                }
        }
    }
}
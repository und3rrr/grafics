using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Komp_graph
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Метод для рисования пикселей с возможностью указать цвет и толщину пера
        void DrawPixels(int x1, int x2, int y, Color color, float thickness)
        {
            using (Graphics g = CreateGraphics())
            {
                Pen pen = new Pen(color, thickness);
                g.DrawLine(pen, x1, y, x2, y);
            }
        }

        // Алгоритм Брезенхема для рисования окружности
        void BresenhamCircle(int cx, int cy, int radius, Color color, float thickness)
        {
            int x = radius, y = 0;
            int D = 2 * (1 - radius);

            while (x >= 0)
            {
                DrawPixels(cx - x, cx + x, cy + y, color, thickness);
                DrawPixels(cx - x, cx + x, cy - y, color, thickness);

                if (D < 0 && 2 * D + 2 * x - 1 <= 0)
                {
                    ++y;
                    D += 2 * y + 1;
                }
                else if (D > 0 && 2 * D - 2 * y - 1 >= 0)
                {
                    --x;
                    D -= 2 * x - 1;
                }
                else
                {
                    --x;
                    ++y;
                    D += 2 * y - 2 * x + 2;
                }
            }
        }

        // Алгоритм Миченера для рисования окружности
        void MichenerCircle(int cx, int cy, int radius, Color color, float thickness)
        {
            int x = radius, y = 0;
            int d = 3 - 2 * radius;

            while (y <= x)
            {
                DrawPixels(cx - x, cx + x, cy - y, color, thickness);
                DrawPixels(cx - x, cx + x, cy + y, color, thickness);
                DrawPixels(cx - y, cx + y, cy + x, color, thickness);
                DrawPixels(cx - y, cx + y, cy - x, color, thickness);

                if (d <= 0)
                {
                    d += 4 * y + 6;
                    ++y;
                }
                else
                {
                    d += 4 * (y - x) + 10;
                    --x;
                    ++y;
                }
            }
        }

        // Обработчик нажатия кнопки для алгоритма Брезенхема

        // Обработчик нажатия кнопки для алгоритма Миченера

        private void button1_Click_1(object sender, EventArgs e)
        {
            int x = Convert.ToInt32(textBox1.Text);
            int y = Convert.ToInt32(textBox2.Text);
            int r = Convert.ToInt32(textBox3.Text);
            Color color = Color.Red; // Можно заменить на любой цвет
            float thickness = 3f; // Можно задать толщину линии
            BresenhamCircle(x, y, r, color, thickness);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            int x = Convert.ToInt32(textBox1.Text);
            int y = Convert.ToInt32(textBox2.Text);
            int r = Convert.ToInt32(textBox3.Text);
            Color color = Color.Blue; // Можно заменить на любой цвет
            float thickness = 3f; // Можно задать толщину линии
            MichenerCircle(x, y, r, color, thickness);
        }
    }
}

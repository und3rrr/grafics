using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PolygonClipper
{
    public partial class MainForm : Form
    {
        private Pen polygonPen = new Pen(Color.Blue);    // Перо для рисования многоугольника
        private Pen clipPen = new Pen(Color.Red);        // Перо для рисования отсекателя
        private Brush fillBrush = new SolidBrush(Color.LightBlue);  // Кисть для заливки
        private List<Point> polygonPoints = new List<Point>(); // Список точек многоугольника
        private List<Point> clipperPoints = new List<Point>(); // Список точек отсекателя
        private List<Point> clippedPoints = new List<Point>(); // Список точек после отсечения

        private bool isPolygonMode = true; // Режим добавления точек (многоугольник или отсекатель)

        public MainForm()
        {
            InitializeComponent();
            this.MouseClick += new MouseEventHandler(MainForm_MouseClick);  // Обработчик кликов
            InitializeUI();
        }

        // Инициализация UI (кнопок)
        private void InitializeUI()
        {
            Button addPolygonButton = new Button();
            addPolygonButton.Text = "Добавить вершину многоугольника";
            addPolygonButton.Location = new Point(10, 10);
            addPolygonButton.Click += (sender, e) => isPolygonMode = true;
            this.Controls.Add(addPolygonButton);

            Button addClipperButton = new Button();
            addClipperButton.Text = "Добавить вершину отсекателя";
            addClipperButton.Location = new Point(10, 50);
            addClipperButton.Click += (sender, e) => isPolygonMode = false;
            this.Controls.Add(addClipperButton);

            Button clipButton = new Button();
            clipButton.Text = "Отсечь и закрасить";
            clipButton.Width = 150;
            clipButton.Location = new Point(10, 90);
            clipButton.Click += (sender, e) => ClipPolygon();
            this.Controls.Add(clipButton);

            // Кнопка для очистки
            Button clearButton = new Button();
            clearButton.Text = "Очистить";
            clearButton.Width = 150;
            clearButton.Location = new Point(10, 130);
            clearButton.Click += (sender, e) => Clear();
            this.Controls.Add(clearButton);
        }

        // Метод очистки
        private void Clear()
        {
            polygonPoints.Clear();  // Очищаем список точек многоугольника
            clipperPoints.Clear();  // Очищаем список точек отсекателя
            clippedPoints.Clear();  // Очищаем результат отсечения
            Invalidate();  // Перерисовываем форму
        }

        // Обработчик кликов мыши
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (isPolygonMode)
            {
                AddPolygonPoint(e.Location);  // Добавляем точку в многоугольник
            }
            else
            {
                AddClipperPoint(e.Location);  // Добавляем точку в отсекатель
            }
        }

        // Этот метод добавляет точку многоугольника
        private void AddPolygonPoint(Point point)
        {
            polygonPoints.Add(point);  // Добавляем точку в список многоугольника
            Invalidate();  // Перерисовываем форму
        }

        // Этот метод добавляет точку отсекателя
        private void AddClipperPoint(Point point)
        {
            clipperPoints.Add(point);  // Добавляем точку в список отсекателя
            Invalidate();  // Перерисовываем форму
        }

        // Обработка рисования на форме
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем многоугольник, если точки добавлены
            if (polygonPoints.Count > 1)
            {
                e.Graphics.DrawPolygon(polygonPen, polygonPoints.ToArray());
            }

            // Рисуем отсекатель, если точки добавлены
            if (clipperPoints.Count > 1)
            {
                e.Graphics.DrawPolygon(clipPen, clipperPoints.ToArray());
            }

            // Рисуем результат отсечения, если он есть
            if (clippedPoints.Count > 2)
            {
                e.Graphics.FillPolygon(fillBrush, clippedPoints.ToArray());  // Закрашиваем результат отсечения
                e.Graphics.DrawPolygon(polygonPen, clippedPoints.ToArray());  // Рисуем контур результата отсечения
            }
        }

        // Метод для отсечения многоугольника отсекателем
        private void ClipPolygon()
        {
            // Применяем алгоритм Sutherland-Hodgman для отсечения
            List<Point> clipped = SutherlandHodgman(polygonPoints, clipperPoints);
            clippedPoints = clipped;  // Сохраняем результат отсечения
            Invalidate();  // Перерисовываем форму
        }

        // Алгоритм отсечения Sutherland-Hodgman
        private List<Point> SutherlandHodgman(List<Point> subject, List<Point> clip)
        {
            List<Point> output = new List<Point>(subject);  // Начинаем с исходного многоугольника

            // Перебираем каждую сторону отсекателя
            for (int i = 0; i < clip.Count; i++)
            {
                List<Point> input = new List<Point>(output);  // Копия текущего многоугольника
                output.Clear();
                Point clipStart = clip[i];
                Point clipEnd = clip[(i + 1) % clip.Count];

                for (int j = 0; j < input.Count; j++)
                {
                    Point current = input[j];
                    Point prev = input[(j - 1 + input.Count) % input.Count];

                    // Проверка, если точка лежит внутри отсекателя
                    if (Inside(current, clipStart, clipEnd))
                    {
                        if (!Inside(prev, clipStart, clipEnd))
                        {
                            output.Add(Intersection(prev, current, clipStart, clipEnd));
                        }
                        output.Add(current);
                    }
                    else if (Inside(prev, clipStart, clipEnd))
                    {
                        output.Add(Intersection(prev, current, clipStart, clipEnd));
                    }
                }

                // Если многоугольник пуст, то нечего отсекать
                if (output.Count == 0) break;
            }

            return output;
        }

        // Проверка, лежит ли точка внутри отсекающего окна
        private bool Inside(Point p, Point clipStart, Point clipEnd)
        {
            return (clipEnd.X - clipStart.X) * (p.Y - clipStart.Y) > (clipEnd.Y - clipStart.Y) * (p.X - clipStart.X);
        }

        // Находим точку пересечения двух отрезков
        private Point Intersection(Point prev, Point current, Point clipStart, Point clipEnd)
        {
            float x1 = prev.X, y1 = prev.Y, x2 = current.X, y2 = current.Y;
            float x3 = clipStart.X, y3 = clipStart.Y, x4 = clipEnd.X, y4 = clipEnd.Y;

            float denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denom == 0) return current;  // Параллельные линии

            float intersectX = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom;
            float intersectY = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom;

            return new Point((int)intersectX, (int)intersectY);
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageProcessing
{
    public class MainForm : Form
    {
        private Bitmap sourceImage;
        private Bitmap resultImage;
        private PictureBox sourcePictureBox;
        private PictureBox resultPictureBox;
        private Button loadImageButton;
        private Button processButton;
        private Button saveImageButton;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Настройка формы
            this.Text = "Image Processing - Variant 13";
            this.Size = new Size(800, 600);

            // Исходное изображение
            sourcePictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(10, 10),
                Size = new Size(300, 300),
                SizeMode = PictureBoxSizeMode.StretchImage // Подстраивание изображения
            };
            this.Controls.Add(sourcePictureBox);

            // Результирующее изображение
            resultPictureBox = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(320, 10),
                Size = new Size(300, 300),
                SizeMode = PictureBoxSizeMode.StretchImage // Подстраивание изображения
            };
            this.Controls.Add(resultPictureBox);

            // Кнопка загрузки изображения
            loadImageButton = new Button
            {
                Text = "Load Image",
                Location = new Point(10, 320),
                Size = new Size(100, 30)
            };
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            // Кнопка обработки
            processButton = new Button
            {
                Text = "Process",
                Location = new Point(120, 320),
                Size = new Size(100, 30)
            };
            processButton.Click += ProcessButton_Click;
            this.Controls.Add(processButton);

            // Кнопка сохранения
            saveImageButton = new Button
            {
                Text = "Save Image",
                Location = new Point(230, 320),
                Size = new Size(100, 30)
            };
            saveImageButton.Click += SaveImageButton_Click;
            this.Controls.Add(saveImageButton);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                sourceImage = new Bitmap(openFileDialog.FileName);
                sourcePictureBox.Image = sourceImage;
            }
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            if (sourceImage == null)
            {
                MessageBox.Show("Please load an image first!");
                return;
            }

            int width = 300, height = 300;
            resultImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(resultImage))
            {
                // Заливка фона белым
                g.Clear(Color.White);

                // Копирование фрагмента из правого верхнего угла
                int fragmentSize = 100;
                int sourceX = sourceImage.Width - fragmentSize;
                int sourceY = 0;
                int destX = width - fragmentSize;
                int destY = 0;

                Rectangle sourceRect = new Rectangle(sourceX, sourceY, fragmentSize, fragmentSize);
                Rectangle destRect = new Rectangle(destX, destY, fragmentSize, fragmentSize);
                g.DrawImage(sourceImage, destRect, sourceRect, GraphicsUnit.Pixel);

                // Рисование осей координат
                Pen axisPen = new Pen(Color.Black, 2);
                g.DrawLine(axisPen, new Point(width / 2, 0), new Point(width / 2, height)); // OY
                g.DrawLine(axisPen, new Point(0, height / 2), new Point(width, height / 2)); // OX

                // Отметки на осях
                for (int i = 10; i < width; i += 10)
                {
                    g.DrawLine(axisPen, new Point(i, height / 2 - 5), new Point(i, height / 2 + 5)); // OX
                }
                for (int i = 10; i < height; i += 10)
                {
                    g.DrawLine(axisPen, new Point(width / 2 - 5, i), new Point(width / 2 + 5, i)); // OY
                }

                // Рисование графика функции y = 1 / (x - 1)^2
                Pen graphPen = new Pen(Color.Blue, 1);
                for (int x = 1; x < width - 1; x++)
                {
                    double xValue1 = (x - width / 2) / 10.0;
                    double xValue2 = (x + 1 - width / 2) / 10.0;

                    if (Math.Abs(xValue1 - 1) < 0.1 || Math.Abs(xValue2 - 1) < 0.1) continue; // Пропускаем разрыв

                    double yValue1 = 1 / Math.Pow(xValue1 - 1, 2);
                    double yValue2 = 1 / Math.Pow(xValue2 - 1, 2);

                    int y1 = height / 2 - (int)(yValue1 * 50); // Масштаб 1:50
                    int y2 = height / 2 - (int)(yValue2 * 50);

                    if (y1 >= 0 && y1 < height && y2 >= 0 && y2 < height)
                    {
                        g.DrawLine(graphPen, new Point(x, y1), new Point(x + 1, y2));
                    }
                }
            }

            resultPictureBox.Image = resultImage;
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (resultImage == null)
            {
                MessageBox.Show("Please process the image first!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                resultImage.Save(saveFileDialog.FileName);
            }
        }

       
    }
}

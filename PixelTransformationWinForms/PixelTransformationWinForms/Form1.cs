using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PixelTransformationWinForms
{
    public partial class Form1 : Form
    {
        Bitmap baseImage;
        Bitmap overlayImage;

        public Form1()
        {
            InitializeComponent();
        }

        // Загрузка основного изображения
        // Загрузка основного изображения
        private void buttonLoadBaseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                baseImage = new Bitmap(openFileDialog.FileName);
                pictureBoxBase.Image = baseImage; // отображение изображения
                pictureBoxBase.SizeMode = PictureBoxSizeMode.Zoom; // Установка масштаба
            }
        }

        // Загрузка накладываемого изображения
        private void buttonLoadOverlayImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                overlayImage = new Bitmap(openFileDialog.FileName);
                pictureBoxOverlay.Image = overlayImage; // отображение изображения
                pictureBoxOverlay.SizeMode = PictureBoxSizeMode.Zoom; // Установка масштаба
            }
        }

        // Обработка изображения
        // Обработка изображения
        private void buttonProcessImages_Click(object sender, EventArgs e)
        {
            if (baseImage == null || overlayImage == null)
            {
                MessageBox.Show("Пожалуйста, загрузите оба изображения.");
                return;
            }

            // Преобразование основного изображения с синусом тона
            Bitmap transformedImage = TransformImageWithSineTone(baseImage);

            // Наложение второго изображения
            Bitmap resultImage = ApplyOverlay(transformedImage, overlayImage);

            // Показ результата
            pictureBoxResult.Image = resultImage;
            pictureBoxResult.SizeMode = PictureBoxSizeMode.Zoom; // Установка масштаба
        }


        // Метод преобразования изображения с синусом от тона
        private Bitmap TransformImageWithSineTone(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // Преобразование RGB в HSV
                    float hue, saturation, value;
                    RgbToHsv(pixel, out hue, out saturation, out value);

                    // Применение синуса от тона к насыщенности
                    saturation = (float)Math.Abs(Math.Sin(hue * Math.PI / 180));

                    // Преобразование обратно в RGB
                    Color newPixel = HsvToRgb(hue, saturation, value);
                    newImage.SetPixel(x, y, newPixel);
                }
            }
            return newImage;
        }

        // Метод наложения одного изображения на другое
        private Bitmap ApplyOverlay(Bitmap baseImage, Bitmap overlayImage)
        {
            Bitmap resultImage = new Bitmap(baseImage.Width, baseImage.Height);

            for (int y = 0; y < baseImage.Height; y++)
            {
                for (int x = 0; x < baseImage.Width; x++)
                {
                    Color basePixel = baseImage.GetPixel(x, y);
                    Color overlayPixel = overlayImage.GetPixel(x % overlayImage.Width, y % overlayImage.Height); // повторение наложения

                    // Пример наложения - среднее арифметическое (можно изменить правило 13)
                    int r = (basePixel.R + overlayPixel.R) / 2;
                    int g = (basePixel.G + overlayPixel.G) / 2;
                    int b = (basePixel.B + overlayPixel.B) / 2;

                    resultImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return resultImage;
        }

        // Сохранение результата в файл
        private void buttonSaveResult_Click(object sender, EventArgs e)
        {
            if (pictureBoxResult.Image == null)
            {
                MessageBox.Show("Сначала выполните обработку изображений.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxResult.Image.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Изображение сохранено.");
            }
        }

        // Метод преобразования RGB в HSV
        public static void RgbToHsv(Color color, out float hue, out float saturation, out float value)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : (1f - (1f * min / max));
            value = max;
        }

        // Метод преобразования HSV в RGB
        public static Color HsvToRgb(float hue, float saturation, float value)
        {
            // Убедимся, что hue находится в диапазоне [0, 360)
            if (hue < 0) hue += 360;
            if (hue >= 360) hue -= 360;

            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (int)(hue / 60); // Явное преобразование в int

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return Color.FromArgb(255, v, t, p);
                case 1:
                    return Color.FromArgb(255, q, v, p);
                case 2:
                    return Color.FromArgb(255, p, v, t);
                case 3:
                    return Color.FromArgb(255, p, q, v);
                case 4:
                    return Color.FromArgb(255, t, p, v);
                default:
                    return Color.FromArgb(255, v, p, q);
            }
        }
    }
}
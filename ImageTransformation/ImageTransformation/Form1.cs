using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageTransformation
{
    public partial class Form1 : Form
    {
        Bitmap originalImage;
        Bitmap transformedImage;
        double shearX = 0.2;  // Значение скоса
        double scaleY = 1.2;  // Значение масштабирования
        double coshFactor = 0.01; // Фактор для нелинейного преобразования

        public Form1()
        {
            InitializeComponent();
        }

        // Загрузка изображения
        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBoxOriginal.Image = originalImage; // Отображение исходного изображения
                pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        // Асинхронные аффинные преобразования
        private async void buttonAffineTransform_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Пожалуйста, загрузите изображение.");
                return;
            }

            transformedImage = await Task.Run(() => AffineTransform(originalImage));
            pictureBoxTransformed.Image = transformedImage;
            pictureBoxTransformed.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private async void buttonInverseAffineTransform_Click(object sender, EventArgs e)
        {
            if (transformedImage == null)
            {
                MessageBox.Show("Сначала выполните аффинные преобразования.");
                return;
            }

            Bitmap resultImage = await Task.Run(() => InverseAffineTransform(transformedImage));
            pictureBoxResult2.Image = resultImage;
            pictureBoxResult2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Асинхронное функциональное преобразование
        private async void buttonFunctionalTransform_Click(object sender, EventArgs e)
        {
            if (transformedImage == null)
            {
                MessageBox.Show("Сначала выполните аффинные преобразования.");
                return;
            }

            Bitmap resultImage = await Task.Run(() => FunctionalTransform(transformedImage));
            pictureBoxResult.Image = resultImage;
            pictureBoxResult.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Сохранение результата в файл
        private void buttonSaveResult_Click(object sender, EventArgs e)
        {
            if (pictureBoxResult.Image == null)
            {
                MessageBox.Show("Сначала выполните преобразования изображения.");
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

        // Метод для прямого аффинного преобразования (скос и масштабирование)
        private Bitmap AffineTransform(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var imageData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            var newImageData = newImage.LockBits(rect, ImageLockMode.WriteOnly, newImage.PixelFormat);

            unsafe
            {
                byte* sourcePtr = (byte*)imageData.Scan0;
                byte* destPtr = (byte*)newImageData.Scan0;
                int bpp = Image.GetPixelFormatSize(image.PixelFormat) / 8;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int newX = (int)(x + shearX * y);
                        int newY = (int)(y * scaleY);

                        if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                        {
                            byte* pixel = sourcePtr + (y * imageData.Stride + x * bpp);
                            byte* newPixel = destPtr + (newY * newImageData.Stride + newX * bpp);

                            for (int i = 0; i < bpp; i++)
                                newPixel[i] = pixel[i];
                        }
                    }
                }
            }

            image.UnlockBits(imageData);
            newImage.UnlockBits(newImageData);
            return newImage;
        }

        // Обратное аффинное преобразование
        private Bitmap InverseAffineTransform(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var imageData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
            var newImageData = newImage.LockBits(rect, ImageLockMode.WriteOnly, newImage.PixelFormat);

            unsafe
            {
                byte* sourcePtr = (byte*)imageData.Scan0;
                byte* destPtr = (byte*)newImageData.Scan0;
                int bpp = Image.GetPixelFormatSize(image.PixelFormat) / 8;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int originalX = (int)(x - shearX * y);
                        int originalY = (int)(y / scaleY);

                        if (originalX >= 0 && originalX < image.Width && originalY >= 0 && originalY < image.Height)
                        {
                            byte* pixel = sourcePtr + (originalY * imageData.Stride + originalX * bpp);
                            byte* newPixel = destPtr + (y * newImageData.Stride + x * bpp);

                            for (int i = 0; i < bpp; i++)
                                newPixel[i] = pixel[i];
                        }
                    }
                }
            }

            image.UnlockBits(imageData);
            newImage.UnlockBits(newImageData);
            return newImage;
        }

        // Функциональное преобразование i = cosh(x') - 1, j = y'
        private Bitmap FunctionalTransform(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int newX = (int)(Math.Cosh(x * coshFactor) - 1); // Нелинейное преобразование X
                    int newY = y;  // Y остается неизменным

                    if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                    {
                        newImage.SetPixel(newX, newY, image.GetPixel(x, y));
                    }
                }
            }

            return newImage;
        }
    }
}
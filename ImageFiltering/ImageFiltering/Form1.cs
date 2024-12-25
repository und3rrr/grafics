using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageFiltering
{
    public class MainForm : Form
    {
        private PictureBox pictureBoxOriginal;
        private PictureBox pictureBoxProcessed;
        private TextBox thresholdTextBox;
        private Label thresholdLabel;
        private Button loadImageButton;
        private Button applyLowPassFilterButton;
        private Button applyHighPassFilterButton;
        private Button saveImageButton;

        private Bitmap originalImage;
        private Bitmap processedImage;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Свойства формы
            this.Text = "Фильтрация изображений - Вариант 13";
            this.ClientSize = new Size(1000, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Оригинальное изображение
            pictureBoxOriginal = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(20, 20),
                Size = new Size(450, 450),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBoxOriginal);

            // Обработанное изображение
            pictureBoxProcessed = new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(520, 20),
                Size = new Size(450, 450),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBoxProcessed);

            // Метка для ввода порога
            thresholdLabel = new Label
            {
                Text = "Порог яркости (T):",
                Location = new Point(20, 490),
                AutoSize = true
            };
            this.Controls.Add(thresholdLabel);

            // Поле для ввода порога
            thresholdTextBox = new TextBox
            {
                Location = new Point(150, 485),
                Width = 100
            };
            this.Controls.Add(thresholdTextBox);

            // Кнопка загрузки изображения
            loadImageButton = new Button
            {
                Text = "Загрузить изображение",
                Location = new Point(20, 530),
                Width = 200
            };
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            // Кнопка для применения ФНЧ
            applyLowPassFilterButton = new Button
            {
                Text = "Применить ФНЧ",
                Location = new Point(240, 530),
                Width = 150
            };
            applyLowPassFilterButton.Click += ApplyLowPassFilterButton_Click;
            this.Controls.Add(applyLowPassFilterButton);

            // Кнопка для применения ФВЧ
            applyHighPassFilterButton = new Button
            {
                Text = "Применить ФВЧ",
                Location = new Point(410, 530),
                Width = 150
            };
            applyHighPassFilterButton.Click += ApplyHighPassFilterButton_Click;
            this.Controls.Add(applyHighPassFilterButton);

            // Кнопка сохранения изображения
            saveImageButton = new Button
            {
                Text = "Сохранить изображение",
                Location = new Point(580, 530),
                Width = 200
            };
            saveImageButton.Click += SaveImageButton_Click;
            this.Controls.Add(saveImageButton);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Файлы изображений|*.bmp;*.jpg;*.png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    originalImage = new Bitmap(dialog.FileName);
                    pictureBoxOriginal.Image = originalImage;
                    processedImage = null;
                    pictureBoxProcessed.Image = null;
                }
            }
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (processedImage == null)
            {
                MessageBox.Show("Нет обработанного изображения для сохранения.");
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Файлы PNG|*.png|Файлы JPEG|*.jpg|Файлы BMP|*.bmp";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png;
                    string ext = System.IO.Path.GetExtension(dialog.FileName).ToLower();
                    switch (ext)
                    {
                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                    }
                    processedImage.Save(dialog.FileName, format);
                }
            }
        }

        private void ApplyLowPassFilterButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение.");
                return;
            }

            if (!int.TryParse(thresholdTextBox.Text, out int threshold))
            {
                MessageBox.Show("Введите допустимое числовое значение для порога яркости.");
                return;
            }

            processedImage = ApplyLowPassFilter(originalImage, threshold);
            pictureBoxProcessed.Image = processedImage;
        }

        private void ApplyHighPassFilterButton_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение.");
                return;
            }

            processedImage = ApplyHighPassFilter(originalImage);
            pictureBoxProcessed.Image = processedImage;
        }

        private Bitmap ApplyLowPassFilter(Bitmap image, int threshold)
        {
            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Используем LockBits для производительности
            BitmapData dataOriginal = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataResult = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataOriginal.Stride;
            int bytes = Math.Abs(stride) * image.Height;
            byte[] bufferOriginal = new byte[bytes];
            byte[] bufferResult = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataOriginal.Scan0, bufferOriginal, 0, bytes);

            int radius = 1;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int idx = y * stride + x * 3;
                    int b = bufferOriginal[idx];
                    int g = bufferOriginal[idx + 1];
                    int r = bufferOriginal[idx + 2];
                    int brightness = (int)(0.299 * r + 0.587 * g + 0.114 * b);

                    if (brightness > threshold)
                    {
                        // Применяем размытие
                        int sumR = 0, sumG = 0, sumB = 0, count = 0;
                        for (int dy = -radius; dy <= radius; dy++)
                        {
                            for (int dx = -radius; dx <= radius; dx++)
                            {
                                int nx = x + dx;
                                int ny = y + dy;
                                if (nx >= 0 && ny >= 0 && nx < image.Width && ny < image.Height)
                                {
                                    int nIdx = ny * stride + nx * 3;
                                    sumB += bufferOriginal[nIdx];
                                    sumG += bufferOriginal[nIdx + 1];
                                    sumR += bufferOriginal[nIdx + 2];
                                    count++;
                                }
                            }
                        }
                        bufferResult[idx] = (byte)(sumB / count);
                        bufferResult[idx + 1] = (byte)(sumG / count);
                        bufferResult[idx + 2] = (byte)(sumR / count);
                    }
                    else
                    {
                        // Копируем без изменений
                        bufferResult[idx] = bufferOriginal[idx];
                        bufferResult[idx + 1] = bufferOriginal[idx + 1];
                        bufferResult[idx + 2] = bufferOriginal[idx + 2];
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferResult, 0, dataResult.Scan0, bytes);

            image.UnlockBits(dataOriginal);
            result.UnlockBits(dataResult);

            return result;
        }

        private Bitmap ApplyHighPassFilter(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Конвертация оригинального изображения в HSL
            Bitmap hslImage = ConvertToHSL(image);

            // Применяем Лапласиан Гауссиана к нужным компонентам
            Bitmap filteredHsl = ApplyLaplacianGaussianFilter(hslImage);

            // Конвертация обратно в RGB
            result = ConvertToRGB(filteredHsl);

            hslImage.Dispose();
            filteredHsl.Dispose();

            return result;
        }

        private Bitmap ConvertToHSL(Bitmap image)
        {
            Bitmap hslImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Используем LockBits для производительности
            BitmapData dataOriginal = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataHSL = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataOriginal.Stride;
            int bytes = Math.Abs(stride) * image.Height;
            byte[] bufferOriginal = new byte[bytes];
            byte[] bufferHSL = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataOriginal.Scan0, bufferOriginal, 0, bytes);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int idx = y * stride + x * 3;
                    Color color = Color.FromArgb(bufferOriginal[idx + 2], bufferOriginal[idx + 1], bufferOriginal[idx]);

                    RGBToHSL(color, out double h, out double s, out double l);

                    // Х, S, L сохраняем в канал R, G, B соответственно
                    bufferHSL[idx] = (byte)(h / 360.0 * 255);
                    bufferHSL[idx + 1] = (byte)(s * 255);
                    bufferHSL[idx + 2] = (byte)(l * 255);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferHSL, 0, dataHSL.Scan0, bytes);

            image.UnlockBits(dataOriginal);
            hslImage.UnlockBits(dataHSL);

            return hslImage;
        }

        private Bitmap ConvertToRGB(Bitmap hslImage)
        {
            Bitmap rgbImage = new Bitmap(hslImage.Width, hslImage.Height, PixelFormat.Format24bppRgb);

            // Используем LockBits для производительности
            BitmapData dataHSL = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataRGB = rgbImage.LockBits(new Rectangle(0, 0, rgbImage.Width, rgbImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataHSL.Stride;
            int bytes = Math.Abs(stride) * hslImage.Height;
            byte[] bufferHSL = new byte[bytes];
            byte[] bufferRGB = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataHSL.Scan0, bufferHSL, 0, bytes);

            for (int y = 0; y < hslImage.Height; y++)
            {
                for (int x = 0; x < hslImage.Width; x++)
                {
                    int idx = y * stride + x * 3;

                    double h = bufferHSL[idx] / 255.0 * 360.0;
                    double s = bufferHSL[idx + 1] / 255.0;
                    double l = bufferHSL[idx + 2] / 255.0;

                    Color color = HSLToRGB(h, s, l);

                    bufferRGB[idx] = color.B;
                    bufferRGB[idx + 1] = color.G;
                    bufferRGB[idx + 2] = color.R;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferRGB, 0, dataRGB.Scan0, bytes);

            hslImage.UnlockBits(dataHSL);
            rgbImage.UnlockBits(dataRGB);

            return rgbImage;
        }

        private Bitmap ApplyLaplacianGaussianFilter(Bitmap hslImage)
        {
            Bitmap result = new Bitmap(hslImage.Width, hslImage.Height, PixelFormat.Format24bppRgb);

            // Ядро Лапласиана Гауссиана 5x5
            double[,] kernel = {
                { 0,   0, -1,  0,  0 },
                { 0,  -1, -2, -1,  0 },
                { -1, -2, 16, -2, -1 },
                { 0,  -1, -2, -1,  0 },
                { 0,   0, -1,  0,  0 }
            };

            int kernelSize = 5;
            int radius = kernelSize / 2;

            // Используем LockBits для производительности
            BitmapData dataOriginal = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataResult = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataOriginal.Stride;
            int bytes = Math.Abs(stride) * hslImage.Height;
            byte[] bufferOriginal = new byte[bytes];
            byte[] bufferResult = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataOriginal.Scan0, bufferOriginal, 0, bytes);

            for (int y = radius; y < hslImage.Height - radius; y++)
            {
                for (int x = radius; x < hslImage.Width - radius; x++)
                {
                    double sumH = 0.0;
                    double sumS = 0.0;
                    double sumL = 0.0;

                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int px = x + kx;
                            int py = y + ky;
                            int idx = py * stride + px * 3;

                            double valueH = bufferOriginal[idx] / 255.0 * 360.0;
                            double valueS = bufferOriginal[idx + 1] / 255.0;
                            double valueL = bufferOriginal[idx + 2] / 255.0;

                            sumH += valueH * kernel[ky + radius, kx + radius];
                            sumS += valueS * kernel[ky + radius, kx + radius];
                            sumL += valueL * kernel[ky + radius, kx + radius];
                        }
                    }

                    // Определяем, в какой половине изображения находится пиксель
                    bool isLeftHalf = x < hslImage.Width / 2;

                    double newH = isLeftHalf ? ClampDouble(sumH, 0, 360) : bufferOriginal[y * stride + x * 3] / 255.0 * 360.0;
                    double newS = isLeftHalf ? ClampDouble(sumS, 0, 1) : ClampDouble(sumS, 0, 1);
                    double newL = isLeftHalf ? ClampDouble(sumL, 0, 1) : bufferOriginal[y * stride + x * 3 + 2] / 255.0 * 1;

                    // Обработка превышений
                    if (isLeftHalf)
                    {
                        // Левый полусрез
                        newL = ClampDouble(newL, 0, 1);
                        newS = ClampDouble(newS, 0, 1);
                    }
                    else
                    {
                        // Правый полусрез
                        newS = ClampDouble(newS, 0, 1);
                    }

                    Color newColor = HSLToRGB(newH, newS, newL);

                    int resultIdx = y * stride + x * 3;
                    bufferResult[resultIdx] = newColor.B;
                    bufferResult[resultIdx + 1] = newColor.G;
                    bufferResult[resultIdx + 2] = newColor.R;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferResult, 0, dataResult.Scan0, bytes);

            hslImage.UnlockBits(dataOriginal);
            result.UnlockBits(dataResult);

            return result;
        }

        private Bitmap ConvertToHSLOptimized(Bitmap image)
        {
            Bitmap hslImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Используем LockBits для производительности
            BitmapData dataOriginal = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataHSL = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataOriginal.Stride;
            int bytes = Math.Abs(stride) * image.Height;
            byte[] bufferOriginal = new byte[bytes];
            byte[] bufferHSL = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataOriginal.Scan0, bufferOriginal, 0, bytes);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int idx = y * stride + x * 3;
                    Color color = Color.FromArgb(bufferOriginal[idx + 2], bufferOriginal[idx + 1], bufferOriginal[idx]);

                    RGBToHSL(color, out double h, out double s, out double l);

                    // Х, S, L сохраняем в канал R, G, B соответственно
                    bufferHSL[idx] = (byte)(h / 360.0 * 255);
                    bufferHSL[idx + 1] = (byte)(s * 255);
                    bufferHSL[idx + 2] = (byte)(l * 255);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferHSL, 0, dataHSL.Scan0, bytes);

            image.UnlockBits(dataOriginal);
            hslImage.UnlockBits(dataHSL);

            return hslImage;
        }

        private Bitmap ConvertToRGBOptimized(Bitmap hslImage)
        {
            Bitmap rgbImage = new Bitmap(hslImage.Width, hslImage.Height, PixelFormat.Format24bppRgb);

            // Используем LockBits для производительности
            BitmapData dataHSL = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataRGB = rgbImage.LockBits(new Rectangle(0, 0, rgbImage.Width, rgbImage.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataHSL.Stride;
            int bytes = Math.Abs(stride) * hslImage.Height;
            byte[] bufferHSL = new byte[bytes];
            byte[] bufferRGB = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataHSL.Scan0, bufferHSL, 0, bytes);

            for (int y = 0; y < hslImage.Height; y++)
            {
                for (int x = 0; x < hslImage.Width; x++)
                {
                    int idx = y * stride + x * 3;

                    double h = bufferHSL[idx] / 255.0 * 360.0;
                    double s = bufferHSL[idx + 1] / 255.0;
                    double l = bufferHSL[idx + 2] / 255.0;

                    Color color = HSLToRGB(h, s, l);

                    bufferRGB[idx] = color.B;
                    bufferRGB[idx + 1] = color.G;
                    bufferRGB[idx + 2] = color.R;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferRGB, 0, dataRGB.Scan0, bytes);

            hslImage.UnlockBits(dataHSL);
            rgbImage.UnlockBits(dataRGB);

            return rgbImage;
        }

        private Bitmap ConvertToHSL(Bitmap image, bool optimized)
        {
            if (optimized)
                return ConvertToHSLOptimized(image);
            else
                return ConvertToHSL(image);
        }

        private Bitmap ConvertToRGB(Bitmap hslImage, bool optimized)
        {
            if (optimized)
                return ConvertToRGBOptimized(hslImage);
            else
                return ConvertToRGB(hslImage);
        }

        private Bitmap ApplyLaplacianGaussianFilterOptimized(Bitmap hslImage)
        {
            Bitmap result = new Bitmap(hslImage.Width, hslImage.Height, PixelFormat.Format24bppRgb);

            // Ядро Лапласиана Гауссиана 5x5
            double[,] kernel = {
                { 0,   0, -1,  0,  0 },
                { 0,  -1, -2, -1,  0 },
                { -1, -2, 16, -2, -1 },
                { 0,  -1, -2, -1,  0 },
                { 0,   0, -1,  0,  0 }
            };

            int kernelSize = 5;
            int radius = kernelSize / 2;

            // Используем LockBits для производительности
            BitmapData dataOriginal = hslImage.LockBits(new Rectangle(0, 0, hslImage.Width, hslImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataResult = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int stride = dataOriginal.Stride;
            int bytes = Math.Abs(stride) * hslImage.Height;
            byte[] bufferOriginal = new byte[bytes];
            byte[] bufferResult = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(dataOriginal.Scan0, bufferOriginal, 0, bytes);

            for (int y = radius; y < hslImage.Height - radius; y++)
            {
                for (int x = radius; x < hslImage.Width - radius; x++)
                {
                    double sumH = 0.0;
                    double sumS = 0.0;
                    double sumL = 0.0;

                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int px = x + kx;
                            int py = y + ky;
                            int idx = py * stride + px * 3;

                            double valueH = bufferOriginal[idx] / 255.0 * 360.0;
                            double valueS = bufferOriginal[idx + 1] / 255.0;
                            double valueL = bufferOriginal[idx + 2] / 255.0;

                            sumH += valueH * kernel[ky + radius, kx + radius];
                            sumS += valueS * kernel[ky + radius, kx + radius];
                            sumL += valueL * kernel[ky + radius, kx + radius];
                        }
                    }

                    // Определяем, в какой половине изображения находится пиксель
                    bool isLeftHalf = x < hslImage.Width / 2;

                    double newH = isLeftHalf ? ClampDouble(sumH, 0, 360) : bufferOriginal[y * stride + x * 3] / 255.0 * 360.0;
                    double newS = isLeftHalf ? ClampDouble(sumS, 0, 1) : ClampDouble(sumS, 0, 1);
                    double newL = isLeftHalf ? ClampDouble(sumL, 0, 1) : bufferOriginal[y * stride + x * 3 + 2] / 255.0 * 1;

                    // Ограничиваем значения
                    if (isLeftHalf)
                    {
                        newL = ClampDouble(newL, 0, 1);
                        newS = ClampDouble(newS, 0, 1);
                    }
                    else
                    {
                        newS = ClampDouble(newS, 0, 1);
                    }

                    Color newColor = HSLToRGB(newH, newS, newL);

                    int resultIdx = y * stride + x * 3;
                    bufferResult[resultIdx] = newColor.B;
                    bufferResult[resultIdx + 1] = newColor.G;
                    bufferResult[resultIdx + 2] = newColor.R;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(bufferResult, 0, dataResult.Scan0, bytes);

            hslImage.UnlockBits(dataOriginal);
            result.UnlockBits(dataResult);

            return result;
        }

        private Bitmap ApplyHighPassFilterOptimized(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Конвертация оригинального изображения в HSL
            Bitmap hslImage = ConvertToHSLOptimized(image);

            // Применяем Лапласиан Гауссиана к нужным компонентам
            Bitmap filteredHsl = ApplyLaplacianGaussianFilterOptimized(hslImage);

            // Конвертация обратно в RGB
            result = ConvertToRGBOptimized(filteredHsl);

            hslImage.Dispose();
            filteredHsl.Dispose();

            return result;
        }

        private Bitmap ApplyHighPassFilter(Bitmap image, bool optimized = true)
        {
            if (optimized)
                return ApplyHighPassFilterOptimized(image);
            else
                return ApplyHighPassFilter(image);
        }

        private void RGBToHSL(Color color, out double h, out double s, out double l)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            h = 0.0;
            s = 0.0;
            l = (max + min) / 2.0;

            if (max == min)
            {
                h = s = 0.0; // Achromatic
            }
            else
            {
                double delta = max - min;
                s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

                if (max == r)
                {
                    h = ((g - b) / delta) % 6.0;
                }
                else if (max == g)
                {
                    h = ((b - r) / delta) + 2.0;
                }
                else
                {
                    h = ((r - g) / delta) + 4.0;
                }

                h *= 60.0;
                if (h < 0)
                    h += 360.0;
            }
        }

        private Color HSLToRGB(double h, double s, double l)
        {
            double c = (1.0 - Math.Abs(2.0 * l - 1.0)) * s;
            double x = c * (1.0 - Math.Abs((h / 60.0) % 2 - 1.0));
            double m = l - c / 2.0;

            double r1, g1, b1;

            if (h >= 0 && h < 60)
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            }
            else if (h >= 180 && h < 240)
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            }
            else if (h >= 240 && h < 300)
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            }
            else
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            }

            int r = Clamp((int)((r1 + m) * 255.0), 0, 255);
            int g = Clamp((int)((g1 + m) * 255.0), 0, 255);
            int b = Clamp((int)((b1 + m) * 255.0), 0, 255);

            return Color.FromArgb(r, g, b);
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private double ClampDouble(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

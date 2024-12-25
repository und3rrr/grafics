using ImageFiltering;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AffineTransformations
{
    public partial class MainForm : Form
    {
        private Bitmap originalImage;

        public MainForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Аффинные преобразования";
            this.Size = new Size(800, 600);

            // Кнопка для выбора изображения
            Button loadImageButton = new Button
            {
                Text = "Выбрать изображение",
                Location = new Point(10, 10),
                Size = new Size(150, 30)
            };
            loadImageButton.Click += LoadImageButton_Click;
            this.Controls.Add(loadImageButton);

            // Текстовые поля для параметров преобразования
            Label scaleXLabel = new Label { Text = "Масштаб X:", Location = new Point(10, 50), Size = new Size(100, 20) };
            this.Controls.Add(scaleXLabel);

            TextBox scaleXTextBox = new TextBox { Name = "scaleXTextBox", Location = new Point(120, 50), Size = new Size(100, 20), Text = "1" };
            this.Controls.Add(scaleXTextBox);

            Label scaleYLabel = new Label { Text = "Масштаб Y:", Location = new Point(10, 80), Size = new Size(100, 20) };
            this.Controls.Add(scaleYLabel);

            TextBox scaleYTextBox = new TextBox { Name = "scaleYTextBox", Location = new Point(120, 80), Size = new Size(100, 20), Text = "1" };
            this.Controls.Add(scaleYTextBox);

            Label rotationLabel = new Label { Text = "Поворот (градусы):", Location = new Point(10, 110), Size = new Size(150, 20) };
            this.Controls.Add(rotationLabel);

            TextBox rotationTextBox = new TextBox { Name = "rotationTextBox", Location = new Point(160, 110), Size = new Size(100, 20), Text = "0" };
            this.Controls.Add(rotationTextBox);

            Label shearXLabel = new Label { Text = "Скос X:", Location = new Point(10, 140), Size = new Size(100, 20) };
            this.Controls.Add(shearXLabel);

            TextBox shearXTextBox = new TextBox { Name = "shearXTextBox", Location = new Point(120, 140), Size = new Size(100, 20), Text = "0" };
            this.Controls.Add(shearXTextBox);

            Label shearYLabel = new Label { Text = "Скос Y:", Location = new Point(10, 170), Size = new Size(100, 20) };
            this.Controls.Add(shearYLabel);

            TextBox shearYTextBox = new TextBox { Name = "shearYTextBox", Location = new Point(120, 170), Size = new Size(100, 20), Text = "0" };
            this.Controls.Add(shearYTextBox);

            // Выбор метода интерполяции
            Label interpolationLabel = new Label { Text = "Метод интерполяции:", Location = new Point(10, 200), Size = new Size(150, 20) };
            this.Controls.Add(interpolationLabel);

            ComboBox interpolationComboBox = new ComboBox { Name = "interpolationComboBox", Location = new Point(160, 200), Size = new Size(150, 20) };
            interpolationComboBox.Items.AddRange(new string[] { "Ближайший сосед", "Билинейная", "Бикубическая" });
            interpolationComboBox.SelectedIndex = 0;
            this.Controls.Add(interpolationComboBox);

            // Кнопка для применения преобразований
            Button transformButton = new Button
            {
                Text = "Применить",
                Location = new Point(10, 240),
                Size = new Size(100, 30)
            };
            transformButton.Click += TransformButton_Click;
            this.Controls.Add(transformButton);

            // Элемент PictureBox для отображения изображения
            PictureBox pictureBox = new PictureBox
            {
                Name = "pictureBox",
                Location = new Point(320, 10),
                Size = new Size(450, 450),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBox);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения (*.jpg; *.png)|*.jpg;*.png|Все файлы (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    originalImage = new Bitmap(openFileDialog.FileName);
                    PictureBox pictureBox = (PictureBox)this.Controls["pictureBox"];
                    pictureBox.Image = originalImage;
                }
            }
        }

        private void TransformButton_Click(object sender, EventArgs e)
        {
            float scaleX = float.Parse(((TextBox)this.Controls["scaleXTextBox"]).Text);
            float scaleY = float.Parse(((TextBox)this.Controls["scaleYTextBox"]).Text);
            float angle = float.Parse(((TextBox)this.Controls["rotationTextBox"]).Text);
            float shearX = float.Parse(((TextBox)this.Controls["shearXTextBox"]).Text);
            float shearY = float.Parse(((TextBox)this.Controls["shearYTextBox"]).Text);
            string interpolationMethod = ((ComboBox)this.Controls["interpolationComboBox"]).SelectedItem.ToString();

            ApplyTransformations(scaleX, scaleY, angle, shearX, shearY, interpolationMethod);
        }

        private void ApplyTransformations(float scaleX, float scaleY, float angle, float shearX, float shearY, string interpolationMethod)
        {
            if (originalImage == null) return;

            // Вычисляем новые размеры изображения
            int newWidth = (int)(originalImage.Width * Math.Abs(scaleX));
            int newHeight = (int)(originalImage.Height * Math.Abs(scaleY));

            // Ограничиваем размеры выходного изображения
            if (newWidth > 10000 || newHeight > 10000)
            {
                MessageBox.Show("Результирующее изображение слишком велико. Уменьшите масштаб.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap transformedImage = null;
            try
            {
                transformedImage = new Bitmap(newWidth, newHeight);
                using (Graphics g = Graphics.FromImage(transformedImage))
                {
                    g.Clear(Color.White);

                    // Выбор метода интерполяции
                    switch (interpolationMethod)
                    {
                        case "Ближайший сосед":
                            g.InterpolationMode = InterpolationMode.NearestNeighbor;
                            break;
                        case "Билинейная":
                            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                            break;
                        case "Бикубическая":
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            break;
                        default:
                            g.InterpolationMode = InterpolationMode.Default;
                            break;
                    }

                    // Применение преобразований
                    Matrix transformationMatrix = new Matrix();
                    transformationMatrix.Scale(scaleX, scaleY);
                    transformationMatrix.Rotate(angle);
                    transformationMatrix.Shear(shearX, shearY);

                    g.Transform = transformationMatrix;
                    g.DrawImage(originalImage, new Point(0, 0));
                }

                // Отображаем преобразованное изображение
                PictureBox pictureBox = (PictureBox)this.Controls["pictureBox"];
                pictureBox.Image = transformedImage;
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Недостаточно памяти для выполнения преобразования. Попробуйте уменьшить параметры масштабирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                transformedImage?.Dispose();
            }
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

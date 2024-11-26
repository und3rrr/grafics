namespace PixelTransformationWinForms
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxBase = new System.Windows.Forms.PictureBox();
            this.buttonLoadBaseImage = new System.Windows.Forms.Button();
            this.buttonLoadOverlayImage = new System.Windows.Forms.Button();
            this.buttonProcessImages = new System.Windows.Forms.Button();
            this.pictureBoxOverlay = new System.Windows.Forms.PictureBox();
            this.pictureBoxResult = new System.Windows.Forms.PictureBox();
            this.buttonSaveResult = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOverlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxBase
            // 
            this.pictureBoxBase.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxBase.Name = "pictureBoxBase";
            this.pictureBoxBase.Size = new System.Drawing.Size(351, 252);
            this.pictureBoxBase.TabIndex = 0;
            this.pictureBoxBase.TabStop = false;
            // 
            // buttonLoadBaseImage
            // 
            this.buttonLoadBaseImage.Location = new System.Drawing.Point(406, 32);
            this.buttonLoadBaseImage.Name = "buttonLoadBaseImage";
            this.buttonLoadBaseImage.Size = new System.Drawing.Size(108, 23);
            this.buttonLoadBaseImage.TabIndex = 2;
            this.buttonLoadBaseImage.Text = "LoadBaseImage";
            this.buttonLoadBaseImage.UseVisualStyleBackColor = true;
            this.buttonLoadBaseImage.Click += new System.EventHandler(this.buttonLoadBaseImage_Click);
            // 
            // buttonLoadOverlayImage
            // 
            this.buttonLoadOverlayImage.Location = new System.Drawing.Point(406, 96);
            this.buttonLoadOverlayImage.Name = "buttonLoadOverlayImage";
            this.buttonLoadOverlayImage.Size = new System.Drawing.Size(108, 23);
            this.buttonLoadOverlayImage.TabIndex = 3;
            this.buttonLoadOverlayImage.Text = "LoadOverlayImage";
            this.buttonLoadOverlayImage.UseVisualStyleBackColor = true;
            this.buttonLoadOverlayImage.Click += new System.EventHandler(this.buttonLoadOverlayImage_Click);
            // 
            // buttonProcessImages
            // 
            this.buttonProcessImages.Location = new System.Drawing.Point(406, 158);
            this.buttonProcessImages.Name = "buttonProcessImages";
            this.buttonProcessImages.Size = new System.Drawing.Size(108, 23);
            this.buttonProcessImages.TabIndex = 4;
            this.buttonProcessImages.Text = "ProcessImages";
            this.buttonProcessImages.UseVisualStyleBackColor = true;
            this.buttonProcessImages.Click += new System.EventHandler(this.buttonProcessImages_Click);
            // 
            // pictureBoxOverlay
            // 
            this.pictureBoxOverlay.Location = new System.Drawing.Point(559, 13);
            this.pictureBoxOverlay.Name = "pictureBoxOverlay";
            this.pictureBoxOverlay.Size = new System.Drawing.Size(339, 251);
            this.pictureBoxOverlay.TabIndex = 5;
            this.pictureBoxOverlay.TabStop = false;
            // 
            // pictureBoxResult
            // 
            this.pictureBoxResult.Location = new System.Drawing.Point(279, 284);
            this.pictureBoxResult.Name = "pictureBoxResult";
            this.pictureBoxResult.Size = new System.Drawing.Size(351, 251);
            this.pictureBoxResult.TabIndex = 6;
            this.pictureBoxResult.TabStop = false;
            // 
            // buttonSaveResult
            // 
            this.buttonSaveResult.Location = new System.Drawing.Point(420, 209);
            this.buttonSaveResult.Name = "buttonSaveResult";
            this.buttonSaveResult.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveResult.TabIndex = 7;
            this.buttonSaveResult.Text = "SaveResult";
            this.buttonSaveResult.UseVisualStyleBackColor = true;
            this.buttonSaveResult.Click += new System.EventHandler(this.buttonSaveResult_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 556);
            this.Controls.Add(this.buttonSaveResult);
            this.Controls.Add(this.pictureBoxResult);
            this.Controls.Add(this.pictureBoxOverlay);
            this.Controls.Add(this.buttonProcessImages);
            this.Controls.Add(this.buttonLoadOverlayImage);
            this.Controls.Add(this.buttonLoadBaseImage);
            this.Controls.Add(this.pictureBoxBase);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOverlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxBase;
        private System.Windows.Forms.Button buttonLoadBaseImage;
        private System.Windows.Forms.Button buttonLoadOverlayImage;
        private System.Windows.Forms.Button buttonProcessImages;
        private System.Windows.Forms.PictureBox pictureBoxOverlay;
        private System.Windows.Forms.PictureBox pictureBoxResult;
        private System.Windows.Forms.Button buttonSaveResult;
    }
}


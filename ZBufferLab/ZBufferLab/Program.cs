using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ZBufferLab
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm form = new MainForm();
            form.Text = "Z-Buffer Algorithm Lab";
            form.Width = 800;
            form.Height = 600;
            Application.Run(form);
        }
    }
}

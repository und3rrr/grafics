﻿using System;
using System.Windows.Forms;

namespace CurveSurfaceBuilder
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}
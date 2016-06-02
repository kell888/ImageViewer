using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox1.Text = path;
                bool f = imageViewer1.SetImagePath(path);
                if (!f)
                {
                    MessageBox.Show("指定图片目录里没有任何图片，或者指定的目录不存在！");
                }
            }
        }

        private void imageViewer1_ImageChanged(object sender, KellImageViewer.ImageArgs e)
        {
            this.Text = "图片浏览器 - " + e.ImageFilepath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace KellImageViewer
{
    [DefaultProperty("Sort")]
    [DefaultEvent("ImageChanged")]
    public partial class ImageViewer : UserControl
    {
        public ImageViewer()
        {
            InitializeComponent();
            DisableAllButtons();
            files = new List<string>();
        }

        int index;
        int pageCount;
        string path;
        bool sort;
        List<string> files;

        [Browsable(false)]
        public List<string> Files
        {
            get { return files; }
            set
            {
                if (value != null)
                    files = value;
                else
                    files = new List<string>();
            }
        }

        [Description("是否对目录下的图片进行排序")]
        [DefaultValue(false)]
        public bool Sort
        {
            get { return sort; }
            set { sort = value; }
        }

        [Description("图片的显示模式")]
        [DefaultValue(PictureBoxSizeMode.Zoom)]
        public PictureBoxSizeMode SizeMode
        {
            get
            {
                return pictureBox1.SizeMode;
            }
            set
            {
                pictureBox1.SizeMode = value;
            }
        }

        public event EventHandler<ImageArgs> ImageChanged;

        private void OnImageChanged(ImageArgs e)
        {
            if (ImageChanged != null)
                ImageChanged(this, e);
        }

        public bool SetImagePath(string path)
        {
            if (Directory.Exists(path))
            {
                this.path = path;
                pageCount = 0;
                string searchPattern = "*.bmp|*.jpg|*.jpeg|*.gif|*.png|*.emf|*.exif|*.ico|*.tiff|*.wmf";
                string[] searchPatterns = searchPattern.Split('|');
                files.Clear();
                foreach (string sp in searchPatterns)
                {
                    files.AddRange(Directory.GetFiles(path, sp, SearchOption.TopDirectoryOnly));
                }
                pageCount = files.Count;
                if (pageCount > 0)
                {
                    if (sort)
                        files.Sort();
                    First();
                    return true;
                }
            }
            DisableAllButtons();
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
            label1.Text = "0/0";
            return false;
        }

        private void DisableAllButtons()
        {
            button1.Enabled = button2.Enabled = button4.Enabled = button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            First();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Prex();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Last();
        }

        private void First()
        {
            index = 0;
            if (LoadImage())
            {
                button1.Enabled = button2.Enabled = false;
                if (pageCount == 1)
                {
                    button3.Enabled = button4.Enabled = false;
                }
                else
                {
                    button3.Enabled = button4.Enabled = true;
                }
            }
        }

        private void Prex()
        {
            if (index > 0)
            {
                index--;
                if (LoadImage())
                {
                    button4.Enabled = true;
                    if (index == 0)
                    {
                        button1.Enabled = button2.Enabled = false;
                    }
                    if (index < pageCount - 1)
                    {
                        button3.Enabled = button4.Enabled = true;
                    }
                }
            }
        }

        private void Next()
        {
            if (index < pageCount - 1)
            {
                index++;
                if (LoadImage())
                {
                    button2.Enabled = true;
                    if (index == pageCount - 1)
                    {
                        button3.Enabled = button4.Enabled = false;
                    }
                    if (index > 0)
                    {
                        button1.Enabled = button2.Enabled = true;
                    }
                }
            }
        }

        private void Last()
        {
            index = pageCount - 1;
            if (LoadImage())
            {
                button3.Enabled = button4.Enabled = false;
                if (pageCount == 1)
                {
                    button1.Enabled = button2.Enabled = false;
                }
                else
                {
                    button1.Enabled = button2.Enabled = true;
                }
            }
        }

        private bool LoadImage()
        {
            if (files.Count > 0)
            {
                if (index > files.Count - 1)
                    index = files.Count - 1;
                string imageFile = files[index];
                pictureBox1.Image = (Image)Image.FromFile(imageFile).Clone();
                label1.Text = (index + 1) + "/" + pageCount;
                OnImageChanged(new ImageArgs(imageFile, index, pageCount));
                return true;
            }
            else
            {
                DisableAllButtons();
                if (pictureBox1.Image != null)
                    pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                label1.Text = "0/0";
                return false;
            }
        }

        private void 图片另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "图片另存为";
            saveFileDialog1.Filter = "jpg图片(*.jpg)|*.jpg|png图片(*.png)|*.png|bmp图片(*.bmp)|*.bmp|gif图片(*.gif)|*.gif";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                        break;
                    case 2:
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Png);
                        break;
                    case 3:
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                        break;
                    case 4:
                        pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Gif);
                        break;
                }
                if (MessageBox.Show("保存成功！需要立即打开查看吗？", "查看提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Process.Start(saveFileDialog1.FileName);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (pictureBox1.Image == null)
                contextMenuStrip1.Enabled = false;
        }
    }

    public class ImageArgs : EventArgs
    {
        string imageFilepath;

        public string ImageFilepath
        {
            get { return imageFilepath; }
        }
        int imageIndex;

        public int ImageIndex
        {
            get { return imageIndex; }
        }
        int imageCount;

        public int ImageCount
        {
            get { return imageCount; }
        }

        public ImageArgs(string imageFilepath, int imageIndex, int imageCount)
        {
            if (!File.Exists(imageFilepath))
                throw new ArgumentException("指定的图片文件不存在！", "imageFilepath");
            this.imageFilepath = imageFilepath;
            this.imageIndex = imageIndex;
            this.imageCount = imageCount;
        }

        public override string ToString()
        {
            return Path.GetFileName(imageFilepath);
        }
    }
}

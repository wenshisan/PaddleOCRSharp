using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using PaddleOCRSharp;
 
namespace PaddleOCRSharpDemo
{
    public partial class ParaForm : Form
    {
        public ParaForm()
        {
            InitializeComponent();
        }
        string imagefile = "";
        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imagefile = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            imagefile = ofd.FileName;
            var imagebyte = File.ReadAllBytes(imagefile);
            pictureBox1.BackgroundImage = new Bitmap(new MemoryStream(imagebyte));
            ParaChanged();
        }

        private void ParaChanged()
        {
            //OCR参数
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.MaxSideLen = trackBar2.Value;
            oCRParameter.BoxScoreThresh = Convert.ToSingle(Math.Round(trackBar3.Value * 1.0 / 100, 2));
            oCRParameter.BoxThresh = Convert.ToSingle(Math.Round(trackBar4.Value * 1.0 / 100, 2));
            oCRParameter.UnClipRatio = Convert.ToSingle(Math.Round(trackBar5.Value * 1.0 / 10, 2));
            oCRParameter.det_db_score_mode = Convert.ToByte(use_polygon_score.Checked);

            oCRParameter.use_angle_cls = Convert.ToByte(use_angle_cls.Checked);
            oCRParameter.cls_thresh = Convert.ToSingle(Math.Round(trackBar1.Value * 1.0 / 100, 2));

            var imagescalebyte = File.ReadAllBytes(imagefile);
            Bitmap bitmap = new Bitmap(new MemoryStream(imagescalebyte));

            double scale = Math.Round(trackBar6.Value * 1.0 / 10, 1);

            string tempfile = "";
            bitmap = new Bitmap(bitmap, new Size(Convert.ToInt32(bitmap.Width * scale), Convert.ToInt32(bitmap.Height * scale)));
            tempfile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".bmp";
            bitmap.Save(tempfile);
            bitmap.Dispose();
            GC.Collect();

            PaddleOCREngine.Detect(null, tempfile, oCRParameter);
            File.Delete(tempfile);
            string file = Environment.CurrentDirectory + "\\ocr_vis.png";
            var imagebyte = File.ReadAllBytes(file);
            pictureBox1.BackgroundImage = new Bitmap(new MemoryStream(imagebyte));
            oCRParameter = null;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = Math.Round(trackBar1.Value * 1.0 / 100, 2).ToString();

            ParaChanged();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar2.Value.ToString();

            ParaChanged();
        }

        private void ParaForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\ocr_vis.png"))

            {
                File.Delete(Environment.CurrentDirectory + "\\ocr_vis.png");
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label3.Text = Math.Round(trackBar3.Value * 1.0 / 100, 2).ToString();

            ParaChanged();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label4.Text = Math.Round(trackBar4.Value * 1.0 / 100, 2).ToString();

            ParaChanged();

        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            label5.Text = Math.Round(trackBar5.Value * 1.0 / 10, 2).ToString();

            ParaChanged();

        }

        private void use_polygon_score_CheckedChanged(object sender, EventArgs e)
        {
            ParaChanged();

        }

        private void use_angle_cls_CheckedChanged(object sender, EventArgs e)
        {
            ParaChanged();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            label12.Text = Math.Round(trackBar6.Value * 1.0 / 10, 1).ToString();

            ParaChanged();
        }
    }
}

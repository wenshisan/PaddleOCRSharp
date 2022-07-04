
using PaddleOCRSharp;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace PaddleOCRSharpDemo
{
    /// <summary>
    /// PaddleOCRSharp使用示例
    /// </summary>
    public partial class MainForm : Form
    {
        private PaddleOCREngine engine;
        Bitmap bmp;
        public MainForm()
        {
            InitializeComponent();
           

             //自带轻量版中英文模型V3模型
             // OCRModelConfig config = null;

             //服务器中英文模型
             OCRModelConfig config = new OCRModelConfig();
            string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            string modelPathroot = root + @"\inferenceserver";
            config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
            config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
            config.keys = modelPathroot + @"\ppocr_keys.txt";

            //英文和数字模型
            //OCRModelConfig config = new OCRModelConfig();
            //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            //string modelPathroot = root + @"\en";
            //config.det_infer = modelPathroot + @"\ch_PP-OCRv2_det_infer";
            //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            //config.rec_infer = modelPathroot + @"\en_number_mobile_v2.0_rec_infer";
            //config.keys = modelPathroot + @"\en_dict.txt";

            //英文和数字模型V3
            //OCRModelConfig config = new OCRModelConfig();
            //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            //string modelPathroot = root + @"\en_v3";
            //config.det_infer = modelPathroot + @"\en_PP-OCRv3_det_infer";
            //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
            //config.rec_infer = modelPathroot + @"\en_PP-OCRv3_rec_infer";
            //config.keys = modelPathroot + @"\en_dict.txt";


            //OCR参数
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.numThread = 6;//预测并发线程数
            oCRParameter.Enable_mkldnn = 1;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            oCRParameter.use_angle_cls =1;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.det_db_score_mode = 1;//是否使用多段线，即文字区域是用多段线还是用矩形，
                                               //  oCRParameter.rec_img_h = 32;
            oCRParameter.UnClipRatio = 1.6f;

            //初始化OCR引擎
            engine = new PaddleOCREngine(config, oCRParameter);

        }
       
        /// <summary>
        /// 打开本地图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripopenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var imagebyte = File.ReadAllBytes(ofd.FileName);
            bmp = new Bitmap(new MemoryStream(imagebyte));
            pictureBox1.BackgroundImage = bmp;

            richTextBox1.Clear();
            richTextBox1.Show();
            dataGridView1.Hide();
            if (bmp == null) return;

         
            OCRResult ocrResult = engine.DetectText(imagebyte);
            ShowOCRResult(ocrResult);

        }
      


        /// <summary>
        /// 识别截图文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            this.Hide();
            richTextBox1.Clear();
            richTextBox1.Show();
            dataGridView1.Hide();
            ScreenCapturer.ScreenCapturerTool screenCapturer = new ScreenCapturer.ScreenCapturerTool();
            if (screenCapturer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bmp = (Bitmap)screenCapturer.Image;
                pictureBox1.BackgroundImage = bmp;
                try
                {
                   ;
                    OCRResult ocrResult  = engine.DetectText(bmp);
                   ShowOCRResult(ocrResult);
                    
                }
                catch (Exception ex)
                {
                }

            }
            this.Show();
        }
        /// <summary>
        /// 识别截图表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            this.Hide();

            ScreenCapturer.ScreenCapturerTool screenCapturer = new ScreenCapturer.ScreenCapturerTool();
            if (screenCapturer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bmp = (Bitmap)screenCapturer.Image;
                pictureBox1.BackgroundImage = bmp;
                richTextBox1.Hide();

               OCRStructureResult ocrResult = engine.DetectStructure(bmp);
                
                int rowcount = ocrResult.RowCount;
                int colcount = ocrResult.ColCount;
                DataTable dataTable = new DataTable();
                for (int i = 0; i < colcount; i++)
                {
                    dataTable.Columns.Add("Column" + (i + 1).ToString(), typeof(string));
                }
                for (int i = 0; i < rowcount; i++)
                {
                    DataRow row = dataTable.Rows.Add();
                    for (int j = 0; j < colcount; j++)
                    {
                        row[j] = ocrResult.Cells.FirstOrDefault(x => x.Row == i && x.Col == j).Text;
                    }
                }
                dataGridView1.DataSource = dataTable;
                dataGridView1.Show();
            }
            this.Show();
        }

        /// <summary>
        /// 检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            //OCR参数
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.numThread = 2;
            oCRParameter.Enable_mkldnn = 1;
            oCRParameter.det_db_score_mode = 1;
            oCRParameter.show_img_vis = 0;
           
            PaddleOCREngine.Detect(null, ofd.FileName,oCRParameter);
            string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
            string file = root + "\\ocr_vis.png";
            var imagebyte = File.ReadAllBytes(file);
            bmp = new Bitmap(new MemoryStream(imagebyte));
            pictureBox1.BackgroundImage = bmp;
        }


        /// <summary>
        /// 显示结果
        /// </summary>
        private void ShowOCRResult(OCRResult ocrResult)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox1.BackgroundImage;
            foreach (var item in ocrResult.TextBlocks)
            {
               richTextBox1.AppendText(item.Text + "\n");
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawPolygon(new Pen(Brushes.Red,2), item.BoxPoints.Select(x=>new PointF() { X=x.X,Y=x.Y}).ToArray());
                }
            }
            pictureBox1.BackgroundImage = null;
            pictureBox1.BackgroundImage = bitmap;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            new ParaForm().Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}

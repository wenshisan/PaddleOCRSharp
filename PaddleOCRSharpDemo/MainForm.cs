

using PaddleOCRSharp;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PaddleOCRSharpDemo
{
    public partial class MainForm : Form
    {
        Bitmap bmp;
       
       
        public MainForm()
        {
            InitializeComponent();
        }

    

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var imagebyte = File.ReadAllBytes(ofd.FileName);
            bmp= new Bitmap(new MemoryStream(imagebyte));
            pictureBox1.BackgroundImage = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            if (bmp == null ) return; ;
 
           OCRResult ocrResult= PaddleOCRHelper.DetectText(bmp,null);

            foreach (var item in ocrResult.TextBlocks)
            {
                richTextBox1.AppendText(item.Text + "\n");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            richTextBox1.Clear();
            ScreenCapturer.ScreenCapturerTool screenCapturer = new ScreenCapturer.ScreenCapturerTool();
            if (screenCapturer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bmp =(Bitmap) screenCapturer.Image;
                pictureBox1.BackgroundImage = bmp;
                OCRResult ocrResult = PaddleOCRHelper.DetectText(bmp,null) ;
                foreach (var item in ocrResult.TextBlocks)
                {
                    richTextBox1.AppendText(item.Text + "\n");
                }

            }
            this.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            
            ScreenCapturer.ScreenCapturerTool screenCapturer = new ScreenCapturer.ScreenCapturerTool();
            if (screenCapturer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bmp = (Bitmap)screenCapturer.Image;
                pictureBox1.BackgroundImage = bmp;
                richTextBox1.Hide();
            
                var ocrResult = PaddleOCRHelper.DetectStructure(bmp,null);
               
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


      
    }

    
}

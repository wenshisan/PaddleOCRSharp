using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
namespace PaddleOCRSharp
{
    /// <summary>
    /// PaddleOCR NET帮助类
    /// </summary>
    public static class PaddleOCRHelper
    {
        [DllImport("PaddleOCR.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Detect(string modelPath_det_infer, string modelPath_cls_infer,
            string modelPath_rec_infer, string keys, string imagefile, OCRParameter parameter, out IntPtr pOCResult);

        
        [DllImport("PaddleOCR.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int FreeDetectMem(IntPtr intPtr);
      
        /// <summary>
        /// 计算表格分割
        /// </summary>
        /// <param name="pixellist"></param>
        /// <param name="thresholdtozero"></param>
        /// <returns></returns>
        private static List<int> getzeroindexs(int[] pixellist, int thresholdtozero = 20)
        {
            List<int> zerolist = new List<int>();
            zerolist.Add(0);
            for (int i = 0; i < pixellist.Length; i++)
            {
                if ((i < pixellist.Length - 1)
                    && (Math.Abs(pixellist[i + 1] - pixellist[i])) > thresholdtozero)
                {
                    //突增点
                    zerolist.Add(i + 1);
                }
            }
            return zerolist;
        }

        private static float scale = 1.0f;
        private static OCRResult DetectText(string imagefile, OCRParameter parameter = null)
        {
            if (!System.IO.File.Exists(imagefile)) throw new Exception($"文件{imagefile}不存在");
            if (parameter == null) parameter = new OCRParameter();
           
            //模型库文件夹
            string modelPathroot = Environment.CurrentDirectory + @"\inference";
            var modelPathdata = new
            {
                modelPath_det_infer = modelPathroot + @"\ch_PP-OCRv2_det_infer",
                modelPath_cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer",
                modelPath_rec_infer = modelPathroot + @"\ch_PP-OCRv2_rec_infer",
                keys = modelPathroot + @"\ppocr_keys.txt",
                imagefile = imagefile
            };

            IntPtr ptrResult;
            int textCount = Detect(modelPathdata.modelPath_det_infer, modelPathdata.modelPath_cls_infer,
                modelPathdata.modelPath_rec_infer, modelPathdata.keys, modelPathdata.imagefile, new OCRParameter(), out ptrResult);
            if (textCount <= 0)
                return null;

            if (ptrResult == IntPtr.Zero)
                return null;

            OCRResult oCRResult = new OCRResult();
            IntPtr ptrFree= ptrResult;
            try
            {
                int textBlockAmount = (int)Marshal.PtrToStructure(ptrResult, typeof(int));
                //总textBlock个数
                ptrResult = ptrResult + 4;
                IntPtr ptrtextBlock = (IntPtr)Marshal.PtrToStructure(ptrResult, typeof(IntPtr));
                ptrResult = ptrtextBlock;
                for (int i = 0; i < textBlockAmount; i++)
                {
                    //文本长度
                    int textBlockLen = (int)Marshal.PtrToStructure(ptrResult, typeof(int));
                    ptrResult = ptrResult + 4;

                    //文本指针
                    IntPtr textPointValue = (IntPtr)Marshal.PtrToStructure(ptrResult, typeof(IntPtr));

                    //文本
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = Marshal.PtrToStringUni(textPointValue);

                    //文本四个点
                    ptrResult = ptrResult + 8;
                    for (int p = 0; p < 4; p++)
                    {
                        OCRPoint oCRPoint = (OCRPoint)Marshal.PtrToStructure(ptrResult, typeof(OCRPoint));
                        textBlock.BoxPoints.Add(new Point(oCRPoint.x, oCRPoint.y));
                        ptrResult = ptrResult + Marshal.SizeOf(typeof(OCRPoint));
                    }
                    oCRResult.TextBlocks.Add(textBlock);
                }
            }
            catch (Exception)
            {
            }
            finally 
            {
                FreeDetectMem(ptrFree);
            }
            oCRResult.TextBlocks.Reverse();
            return oCRResult;
        }

        /// <summary>
        ///文本识别
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static OCRResult DetectText(Image image, OCRParameter parameter = null)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (!Environment.Is64BitProcess) throw new Exception("暂不支持32位程序使用本OCR");
            string imagefile = "";
            #region 小图执行放大，分段线性缩放
            if (image.Width <= 50 || image.Height <= 50)
            {
                scale = 3;
            }
            else if (image.Width <= 100 || image.Height <= 100)
            {
                scale = 2;
            }
            else if (image.Width <= 150 || image.Height <= 150)
            {
                scale = 1.5f;
            }

            Bitmap bitmap = new Bitmap(image, new Size(Convert.ToInt32(image.Width * scale), Convert.ToInt32(image.Height * scale)));
            imagefile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".bmp";
            bitmap.Save(imagefile);
            bitmap.Dispose();
            #endregion
            OCRResult result= DetectText(imagefile, parameter);
            #region 返回结果的区域需要还原比例
            int padding = 50;
            if (parameter != null) padding = parameter.Padding;
            foreach (var item in result.TextBlocks)
            {
                List<Point> boxPoints = new List<Point>();

                for (int i = 0; i < item.BoxPoints.Count; i++)
                {
                    var point = item.BoxPoints[i];
                    Point p = new Point();
                    p.X = Convert.ToInt32(((float)(point.X - 0)) / scale);
                    p.Y = Convert.ToInt32(((float)(point.Y - 0)) / scale);
                    if (p.X < 0) p.X = 0;
                    if (p.Y < 0) p.Y = 0;
                    boxPoints.Add(p);
                }
                item.BoxPoints = boxPoints;
            }
            #endregion
            return result;
        }

        /// <summary>
        ///结构化文本识别(临时解决方案，需要的请研究官方的结构化识别)
        /// </summary>
        /// <param name="image">图像</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static OCRStructureResult DetectStructure(Image image, OCRParameter parameter = null)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (!Environment.Is64BitProcess) throw new Exception("暂不支持32位程序使用本OCR");
            string imagefile = "";
            #region 小图执行放大，分段线性缩放
            if (image.Width <= 50 || image.Height <= 50)
            {
                scale = 3;
            }
            else if (image.Width <= 100 || image.Height <= 100)
            {
                scale = 2;
            }
            else if (image.Width <= 150 || image.Height <= 150)
            {
                scale = 1.5f;
            }

            Bitmap bitmap = new Bitmap(image, new Size(Convert.ToInt32(image.Width * scale), Convert.ToInt32(image.Height * scale)));
            imagefile = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".bmp";
            bitmap.Save(imagefile);
            bitmap.Dispose();
            #endregion
            OCRResult result = DetectText(imagefile, parameter);
            #region 返回结果的区域需要还原比例
            int padding = 0;
            //if (parameter != null) padding = parameter.Padding;
            foreach (var item in result.TextBlocks)
            {
                List<Point> boxPoints = new List<Point>();

                for (int i = 0; i < item.BoxPoints.Count; i++)
                {
                    var point = item.BoxPoints[i];
                    Point p = new Point();
                    p.X = Convert.ToInt32(((float)(point.X - padding)) / scale);
                    p.Y = Convert.ToInt32(((float)(point.Y - padding)) / scale);
                    if (p.X < 0) p.X = 0;
                    if (p.Y < 0) p.Y = 0;
                    boxPoints.Add(p);
                }
                item.BoxPoints = boxPoints;
            }
            #endregion

            List<TextBlock> blocks = result.TextBlocks;
            if (blocks == null || blocks.Count == 0) return new OCRStructureResult();
            var listys = getzeroindexs(blocks.OrderBy(x => x.BoxPoints[0].Y).Select(x => x.BoxPoints[0].Y).ToArray(), 10);
            var listxs = getzeroindexs(blocks.OrderBy(x => x.BoxPoints[0].X).Select(x => x.BoxPoints[0].X).ToArray(), 10);

            int rowcount = listys.Count;
            int colcount = listxs.Count;
            OCRStructureResult structureResult = new OCRStructureResult();
            structureResult.TextBlocks = blocks;
            structureResult.RowCount = rowcount;
            structureResult.ColCount = colcount;
            structureResult.Cells = new List<StructureCells>();
            for (int i = 0; i < rowcount; i++)
            {
                int y_min = blocks.OrderBy(x => x.BoxPoints[0].Y).OrderBy(x => x.BoxPoints[0].Y).ToList()[listys[i]].BoxPoints[0].Y;
                int y_max = 99999;
                if (i < rowcount - 1)
                {
                    y_max = blocks.OrderBy(x => x.BoxPoints[0].Y).ToList()[listys[i + 1]].BoxPoints[0].Y;
                }

                for (int j = 0; j < colcount; j++)
                {
                    int x_min = blocks.OrderBy(x => x.BoxPoints[0].X).ToList()[listxs[j]].BoxPoints[0].X;
                    int x_max = 99999;

                    if (j < colcount - 1)
                    {
                        x_max = blocks.OrderBy(x => x.BoxPoints[0].X).ToList()[listxs[j + 1]].BoxPoints[0].X;
                    }

                    var textBlocks = blocks.Where(x => x.BoxPoints[0].X < x_max && x.BoxPoints[0].X >= x_min && x.BoxPoints[0].Y < y_max && x.BoxPoints[0].Y >= y_min).OrderBy(u => u.BoxPoints[0].X);
                    var texts = textBlocks.Select(x => x.Text).ToArray();

                    StructureCells cell = new StructureCells();
                    cell.Row = i;
                    cell.Col = j;
                    cell.Text = string.Join<string>("", texts);
                    cell.TextBlocks = textBlocks.ToList();
                    structureResult.Cells.Add(cell);
                }
            }
            return structureResult;

           
        }
  
    }
}

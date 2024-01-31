using PaddleOCRSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrFromImage
{
    public class MainClass
    {

        private PaddleOCREngine engine;
        private PaddleStructureEngine structengine;
        Bitmap bmp;
        OCRResult lastocrResult;
        string outpath = Path.Combine(Environment.CurrentDirectory, "out");

        public MainClass()
        {

            if (!Directory.Exists(outpath))
            { Directory.CreateDirectory(outpath); }
            //自带轻量版中英文模型V3模型
            OCRModelConfig config = null;
            //OCR参数
            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.cpu_math_library_num_threads = 10;//预测并发线程数
            oCRParameter.enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            oCRParameter.cls = false; //是否执行文字方向分类；默认false
            oCRParameter.det = true;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.use_angle_cls = false;//是否开启方向检测，用于检测识别180旋转
            oCRParameter.det_db_score_mode = true;//是否使用多段线，即文字区域是用多段线还是用矩形，

            //初始化OCR引擎
            engine = new PaddleOCREngine(config, oCRParameter);

            //模型配置，使用默认值
            StructureModelConfig structureModelConfig = null;
            //表格识别参数配置，使用默认值
            StructureParameter structureParameter = new StructureParameter();
            structengine = new PaddleStructureEngine(structureModelConfig, structureParameter);
        }

        public string GetResult(string FilePath)
        {
            var imagebyte = File.ReadAllBytes(FilePath);

            OCRResult ocrResult = engine.DetectText(imagebyte);
            return ocrResult.Text;
        }
        public string GetResult(byte[] ImageByte)
        {


            OCRResult ocrResult = engine.DetectText(ImageByte);
            return ocrResult.Text;
        }
    }
}

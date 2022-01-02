using System.Runtime.InteropServices;
namespace PaddleOCRSharp
{
    /// <summary>
    /// OCR识别参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class OCRParameter
    {
        public OCRParameter()
        {
            numThread = 2;
            Padding = 50;
            MaxSideLen = 2048;
            BoxScoreThresh = 0.618f;
            BoxThresh = 0.3f;
            UnClipRatio = 2.0f;
            DoAngle = 1;
            MostAngle = 1;
            Enable_mkldnn = 1;
            use_gpu = 0;
        }
        /// <summary>
        /// 使用线程数，默认2
        /// </summary>
        public int numThread { get; set; }
       /// <summary>
       /// 补白边，默认50，暂时没有用
       /// </summary>
        public int Padding { get; set; }
        /// <summary>
        /// 最大边限制，超过该尺寸的图片将被缩放到尺寸大小，默认2048
        /// </summary>
        public int MaxSideLen { get; set; }
        /// <summary>
        /// BoxScoreThresh 默认0.618
        /// </summary>
        public float BoxScoreThresh { get; set; }
        /// <summary>
        /// BoxThresh 默认0.3
        /// </summary>
        public float BoxThresh { get; set; }
        /// <summary>
        /// UnClipRatio 默认2.0
        /// </summary>
        public float UnClipRatio { get; set; }
        /// <summary>
        /// DoAngle 默认1启用
        /// </summary>
        public byte DoAngle { get; set; }
        /// <summary>
        /// MostAngle 默认1启用
        /// </summary>
        public byte MostAngle { get; set; }
        /// <summary>
        /// 启用mkldnn加速，默认开启
        /// </summary>
        public byte Enable_mkldnn { get; set; }
        /// <summary>
        /// 启用GPU加速，默认关闭
        /// </summary>
        public byte use_gpu { get; set; }
    }
}



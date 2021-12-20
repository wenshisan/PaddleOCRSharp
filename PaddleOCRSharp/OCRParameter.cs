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
        }

        public int numThread { get; set; }
        public int Padding { get; set; }
        public int MaxSideLen { get; set; }
        public float BoxScoreThresh { get; set; }
        public float BoxThresh { get; set; }
        public float UnClipRatio { get; set; }
        public byte DoAngle { get; set; }
        public byte MostAngle { get; set; }
    }
}



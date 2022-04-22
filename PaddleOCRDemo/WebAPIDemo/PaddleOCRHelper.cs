
using PaddleOCRSharp;
namespace WebAPIDemo
{
    public static class PaddleOCRHelper
    {
        // 定义一个标识确保线程同步
        private static readonly object _Locker = new object();
        private static PaddleOCREngine _Instance;

        public static PaddleOCREngine Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Locker)
                    {
                        // 如果类的实例不存在则创建，否则直接返回
                        if (_Instance == null)
                        {
                            //OCR参数
                            OCRParameter oCRParameter = new OCRParameter();
                            oCRParameter.numThread = 2;
                            oCRParameter.Enable_mkldnn = 0;
                            oCRParameter.use_angle_cls = 1;
                            oCRParameter.use_polygon_score = 1;
                            OCRModelConfig config = null;
                            _Instance = new PaddleOCREngine(config, oCRParameter);
                        }
                    }
                }
                return _Instance;
            }
        }


    }
}


#### 1. 参数说明（OCRModelConfig模型配置对象）：

|参数名称|类型|默认值|说明|
|---|---|---|---|
|det_infer           |string  | |文本检测 det_infer模型路径|
|cls_infer            |string  | | 文本角度检测cls_infer模型路径|
|rec_infer           |string  |  |文本识别rec_infer模型路径|
|keys         |string   |   |  ppocr_keys.txt文件名全路径|



#### 3. 参数说明（oCRParameter参数对象）：

|参数名称|类型|默认值|说明|
|---|---|---|---|
|use_gpu           |byte  |0 | 是否使用GPU|
|gpu_id            |int   |0 | GPU id，使用GPU时有效|
|gpu_mem           |int   |4000  |申请的GPU内存|
|numThread         |int   |10    | CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快|
|Enable_mkldnn     |byte  |1 | 是否使用mkldnn库，即CPU加速|
|det               |byte  |1 | 是否执行文字检测|
|rec               |byte  |1 | 是否执行文字识别|
|cls               |byte  |0 | 是否执行文字方向分类|
|MaxSideLen        |int   |960 | 输入图像长宽大于960时，等比例缩放图像，使得图像最长边为960|
|BoxThresh         |float |0.3 | 用于过滤DB预测的二值化图像，设置为0.-0.3对结果影响不明显|
|BoxScoreThresh    |float |0.5 |  DB后处理过滤box的阈值，如果检测存在漏框情况，可酌情减小|
|UnClipRatio       |float |1.6 |  表示文本框的紧致程度，越小则文本框更靠近文本|
|use_dilation      |byte  |0  | 是否在输出映射上使用膨胀|
|det_db_score_mode |byte  |1 | 1:使用多边形框计算bbox score，0:使用矩形框计算。矩形框计算速度更快，多边形框对弯曲文本区域计算更准确|
|visualize         |byte  |0 | 是否对结果进行可视化，为1时，预测结果会保存在output字段指定的文件夹下和输入图像同名的图像上|
|use_angle_cls     |byte  |0 | 是否使用方向分类器|
|cls_thresh        |float |0.9 | 方向分类器的得分阈值|
|cls_batch_num     |int   |1 | 方向分类器batchsize|
|rec_batch_num     |int   |6 | 识别模型batchsize|
|rec_img_h         |int   |48 | 识别模型输入图像高度，v2模型需要设置位32|
|rec_img_w         |int   |320 | 识别模型输入图像宽度|
|show_img_vis      |byte  |0 | 是否显示预测结果|

#### 3. 使用示例
```
  OpenFileDialog ofd = new OpenFileDialog();
  ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
  if (ofd.ShowDialog() != DialogResult.OK) return;
//使用默认中英文V3模型
  OCRModelConfig config = null;
//使用默认参数
  OCRParameter oCRParameter = new  OCRParameter ();
//识别结果对象
  OCRResult ocrResult = new OCRResult();
  //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。     
  PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
   {
    ocrResult = engine.DetectText(ofd.FileName );
   }
 if (ocrResult != null) MessageBox.Show(ocrResult.Text,"识别结果");

```
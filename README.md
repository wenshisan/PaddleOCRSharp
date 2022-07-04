### 简体中文 | [English](https://github.com/raoyutian/paddleocrsharp/main/README_en.md)     |[更新记录](https://github.com/raoyutian/paddleocrsharp/main/doc/README_update.md)


### 介绍
------
#### 1. 介绍

本项目是一个基于百度飞桨[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改并封装的.NET的工具类库。包含文本识别、文本检测、基于文本检测结果的统计分析的表格识别功能，同时针对小图识别不准的情况下，做了优化，提高识别准确率。包含总模型仅8.6M的超轻量级中文OCR，单模型支持中英文数字组合识别、竖排文本识别、长文本识别。同时支持多种文本检测。
项目封装极其简化，实际调用仅几行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。Nuget包即装即用，可以离线部署，不需要网络就可以识别的高精度中英文OCR。  

本项目中PaddleOCR.dll文件是基于开源项目[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改而成的C++动态库，基于opencv的x64编译而成的。

 **本项目已经适配[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)最新版release2.5，并支持PP-OCRv3模型。** 
 **超轻量OCR系统PP-OCRv3：中英文、纯英文以及多语言场景精度再提升5% - 11%！** 

如果使用v3模型，请设置OCR识别参数OCRParameter对象的属性rec_img_h：

```
rec_img_h=48
```

本项目只能在X64的CPU上编译和使用，因此不支持32位，暂不支持Linux平台，只能在avx指令集上的CPU上使用。

本项目目前支持以下NET框架：

```
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;

```


### 使用与部署
------
#### 1. 参数说明（oCRParameter参数对象）：

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
|rec_img_h         |int   |32 | 识别模型输入图像高度|
|rec_img_w         |int   |320 | 识别模型输入图像宽度|
|show_img_vis      |byte  |0 | 是否显示预测结果|


#### 2. 使用示例
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

[C++示例代码](https://github.com/raoyutian/paddleocrsharp/main/PaddleOCRDemo/PaddleOCRCppDemo/PaddleOCRCppDemo.cpp)

#### 2. 模型

OCR识别模型库支持官方所有的模型，也支持自己训练的模型。完全按照飞桨OCR接口搭桥。
本项目部署自带的一种轻量版8.6M模型库、服务器版模型库（更准确，需要自行下载），可以自行更改模型库适用实际需求。

|模型名称|模型大小|下载地址|备注|
|---|---|---|---|
|ch_PP-OCRv2  |10M  |[中英文轻量v2](https://gitee.com/raoyutian/paddle-ocrsharp/raw/master/models/PP-OCRv2/inference.zip)  | |
|en_PP-OCRv2  |4M   |[英文数字v2](https://gitee.com/raoyutian/paddle-ocrsharp/raw/master/models/PP-OCRv2/en.zip)  |  |
|ch_PP-OCRv3  |12M  |[中英文轻量v3](https://gitee.com/raoyutian/paddle-ocrsharp/raw/master/models/PP-OCRv3/inference_v3.zip)|   |
|en_PP-OCRv3  |10M  |[英文数字v3](https://gitee.com/raoyutian/paddle-ocrsharp/raw/master/models/PP-OCRv3/en_v3.zip)|   |

[更多PaddleOCR模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/dygraph/doc/doc_ch/models_list.md)

如果需要修改成服务器版模型库，参考代码如下：(假设服务器版模型库在运行目录的文件夹inferenceserver下)

```

 //自带轻量版中英文模型PP-OCRv3
 // OCRModelConfig config = null;

 //服务器中英文模型
 //OCRModelConfig config = new OCRModelConfig();
 //string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
 //string modelPathroot = root + @"\inferenceserver";
 //config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
 //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
 //config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
 //config.keys = modelPathroot + @"\ppocr_keys.txt";

 //英文和数字模型
 OCRModelConfig config = new OCRModelConfig();
 string root = System.IO.Path.GetDirectoryName(typeof(OCRModelConfig).Assembly.Location);
 string modelPathroot = root + @"\en";
 config.det_infer = modelPathroot + @"\ch_PP-OCRv2_det_infer";
 config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
 config.rec_infer = modelPathroot + @"\en_number_mobile_v2.0_rec_infer";
 config.keys = modelPathroot + @"\en_dict.txt";

```

### 源码编译
------
#### 1.文件夹结构

```
PaddleOCR                    //PaddleOCR.dll文件的源代码文件夹
|--cpp                       //PaddleOCR.dll的Cpp文件
|--include                   //PaddleOCR.dll的.h文件

PaddleOCRLib                 //OCR运行需要的文件
|--inference                 //OCR的轻量中文简体模型库
|--libiomp5md.dll            //第三方引用库
|--mkldnn.dll                //第三方引用库
|--mklml.dll                 //第三方引用库
|--opencv_world411.dll       //第三方引用库
|--paddle_inference.dll      //飞桨库
|--PaddleOCR.dll             //基于开源项目PaddleOCR修改的C++动态库，源码见根目录下的PaddleOCR文件夹 
PaddleOCRSharp               //.NET封装库项目，即本项目
PaddleOCRDemo                //Demo文件夹
|--Cpp                       //C++示例项目引用的头文件和库
|--PaddleOCRCppDemo          //C++调用示例项目
|--PaddleOCRSharpDemo        //.NET调用示例项目
|--WebAPIDemo                //.NET的WebAPI示例项目

```

#### 1.编译

建议使用VS2022版本编译，如果遇到无法编译，请切换成release后再切换回debug即可。 
如果因框架编译问题无法编译，请修改PaddleOCRSharp\PaddleOCRSharp.csproj文件，删除当前电脑环境没有的框架，只保留你想要的.Net框架。
具体框架说明见微软文档[SDK 样式项目中的目标框架](https://docs.microsoft.com/zh-cn/dotnet/standard/frameworks)
```
 <TargetFrameworks>
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;
</TargetFrameworks>
```

编译 C++项目PaddleOCR,需要下载paddlepaddle预测库。

[windows下C++预测库下载地址](https://paddleinference.paddlepaddle.org.cn/user_guides/download_lib.html#windows)

### 常见问题与解决方案

[常见问题与解决方案](https://github.com/raoyutian/paddleocrsharp/main/doc/README_question.md)

### 技术交流方式
------
#### QQ技术交流群：318860399。
#### 微信公众号：明月心技术学堂。
#### [个人博客地址： https://www.cnblogs.com/raoyutian/]( https://www.cnblogs.com/raoyutian/)

------
#### 喜欢的话就给个星，如果对你有用，就点个赞。开源不容易，为了造福大众。谢谢！
------

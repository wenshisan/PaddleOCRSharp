## 简体中文 | [English](https://github.com/raoyutian/PaddleOCRSharp/blob/main/README_en.md)     |[更新记录](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/README_update.md)


## 一、介绍


本项目是一个基于[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改并封装的.NET的工具类库。包含文本识别、文本检测、基于文本检测结果的统计分析的表格识别功能，同时针对小图识别不准的情况下，做了优化，提高识别准确率。包含总模型仅8.6M的超轻量级中文OCR，单模型支持中英文数字组合识别、竖排文本识别、长文本识别。同时支持多种文本检测。
项目封装极其简化，实际调用仅几行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。Nuget包即装即用，可以离线部署，不需要网络就可以识别的高精度中英文OCR。  

本项目中PaddleOCR.dll文件是基于开源项目[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改而成的C++动态库，基于opencv的x64编译而成的。

本项目只能在X64的CPU上编译和使用，因此不支持32位。

暂不支持Linux平台，如果有跨平台需求，请把本项目有关Systen.Drawing.dll、Systen.Drawing.Common.dll的引用删除，重新编译。

本项目目前支持以下NET框架：

```
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;

```

OCR识别模型库支持官方所有的模型，也支持自己训练的模型。完全按照飞桨OCR接口搭桥。

本项目部署自带的一种轻量版8.6M模型库、服务器版模型库（更准确，需要自行下载），可以自行更改模型库适用实际需求。

[PaddleOCR模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/dygraph/doc/doc_ch/models_list.md)

如果需要修改成服务器版模型库，参考代码如下：(假设服务器版模型库在运行目录的文件夹inferenceserver下)

```
OpenFileDialog ofd = new OpenFileDialog();
ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
if (ofd.ShowDialog() != DialogResult.OK) return;
var imagebyte = File.ReadAllBytes(ofd.FileName);
 Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));

 //自带轻量版中英文模型
 // OCRModelConfig config = null;
 //服务器中英文模型
 //OCRModelConfig config = new OCRModelConfig();
 //string root = Environment.CurrentDirectory;
 //string modelPathroot = root + @"\inferenceserver";
 //config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
 //config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
 //config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
 //config.keys = modelPathroot + @"\ppocr_keys.txt";

 //英文和数字模型
 OCRModelConfig config = new OCRModelConfig();
 string root = Environment.CurrentDirectory;
 string modelPathroot = root + @"\en";
 config.det_infer = modelPathroot + @"\ch_PP-OCRv2_det_infer";
 config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
 config.rec_infer = modelPathroot + @"\en_number_mobile_v2.0_rec_infer";
 config.keys = modelPathroot + @"\en_dict.txt";


 OCRParameter oCRParameter = new  OCRParameter ();
 OCRResult ocrResult = new OCRResult();

//建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。  
 PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
  {
    ocrResult = engine.DetectText(bitmap );
  }
 if (ocrResult != null)
 {
    MessageBox.Show(ocrResult.Text,"识别结果");
 }

//不再用OCR时，请把PaddleOCREngine释放

```

 [windows下C++预测库下载地址](https://paddleinference.paddlepaddle.org.cn/user_guides/download_lib.html#windows)



[全部调用参数](https://github.com/raoyutian/PaddleOCRSharp/blob/main/PaddleOCRSharp/OCRParameter.cs)

[PaddleOCR官方参数](https://gitee.com/paddlepaddle/PaddleOCR/tree/release/2.4/deploy/cpp_infer)


## 二、文件夹结构

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
PaddleOCRSharp               //.NET封装库项目
PaddleOCRDemo                //Demo文件夹
|--Cpp                       //C++项目需要引用的PaddleOCR.dll的头文件和库文件
|--PaddleOCRCppDemo          //C++调用示例项目
|--PaddleOCRSharpDemo        //.NET调用示例项目
|--WebAPIDemo                //.NET的WebAPI示例项目

```

## 三、源码编译

关于源码编译，PaddleOCRSharp项目包含众多.Net框架，因此建议采用vs2022版本编译，如果遇到无法编译，请切换成release后再切换回debug即可。 
如果因框架编译问题无法编译，请修改PaddleOCRSharp\PaddleOCRSharp.csproj文件，删除当前电脑没有的框架，或者只保留你想要的.Net框架。
具体框架说明见微软文档[SDK 样式项目中的目标框架](https://docs.microsoft.com/zh-cn/dotnet/standard/frameworks)
```
 <TargetFrameworks>
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;
</TargetFrameworks>
```


## 四、.NET使用示例

```
  OpenFileDialog ofd = new OpenFileDialog();
  ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
  if (ofd.ShowDialog() != DialogResult.OK) return;
  var imagebyte = File.ReadAllBytes(ofd.FileName);
  Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));
  OCRModelConfig config = null;
  OCRParameter oCRParameter = new  OCRParameter ();

  OCRResult ocrResult = new OCRResult();

  //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。     
  PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
   {
    ocrResult = engine.DetectText(bitmap );
   }
 if (ocrResult != null)
 {
    MessageBox.Show(ocrResult.Text,"识别结果");
 }

//不再用OCR时，请把PaddleOCREngine释放

```

[C++示例代码](https://github.com/raoyutian/PaddleOCRSharp/blob/main/PaddleOCRDemo/PaddleOCRCppDemo/PaddleOCRCppDemo.cpp)


## 五、实时预览

在PaddleOCRDemo\PaddleOCRSharpDemo项目中，有一个实时预览参数窗口，在界面上调整不同的识别参数，可以直观的看到预测的结果。
编译并运行PaddleOCRSharpDemo项目，如图：
![输入图片说明](doc/demo%E4%B8%BB%E7%95%8C%E9%9D%A2.jpg)
然后点击【参数调整】菜单按钮，弹出一个实时调整参数预览界面，点击【打开文件】选择一个本地图片，然后调整参数，可以预览实时参数的预测结果。
![输入图片说明](doc/%E5%8F%82%E6%95%B0%E8%B0%83%E6%95%B4%E9%A2%84%E8%A7%88.png)



## 六、[常见问题与解决方案](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/README_question.md)
---------------------------------------------------------------------------------------------------------------------
### 喜欢的话就给个星，如果对你有用，就点个赞。开源不容易，为了造福大众。谢谢！

### QQ交流群：318860399，有问题可以加QQ群咨询。微信公众号：明月心技术学堂，可以通过微信公众号获取微信群二维码。


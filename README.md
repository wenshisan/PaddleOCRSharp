## PaddleOCRSharp       [版本更新记录](https://github.com/raoyutian/paddle-ocrsharp/blob/main/doc/README_update.md)

## 一、介绍


本项目是一个基于PaddleOCR的C++代码修改并封装的.NET的工具类库。包含文本识别、文本检测、基于文本检测结果的统计分析的表格识别功能，同时针对小图识别不准的情况下，做了优化，提高识别准确率。包含总模型仅8.6M的超轻量级中文OCR，单模型支持中英文数字组合识别、竖排文本识别、长文本识别。同时支持多种文本检测。
项目封装极其简化，实际调用仅几行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。Nuget包即装即用，可以离线部署，不需要网络就可以识别的高精度中英文OCR。  

本项目中PaddleOCR.dll文件是基于开源项目PaddleOCR的C++代码修改而成的C++动态库，基于opencv的x64编译而成的。

本项目目前支持以下NET框架：

```
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;

```

[PaddleOCR官方项目地址（码云）](https://gitee.com/paddlepaddle/PaddleOCR)

[PaddleOCR官方项目地址（GitHub）](https://github.com/paddlepaddle/PaddleOCR)

OCR识别模型库支持轻量版（本项目部署自带的一种）、服务器版模型库（更准确，需要自行下载），可以自行更改模型库适用实际需求。

[PaddleOCR模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/dev/doc/doc_ch/models_list.md)

如果需要修改成服务器版模型库，参考代码如下：(假设服务器版模型库在运行目录的文件夹inferenceserver下)

<details style='background-color:#f9f2f4'>
<summary><font color='#c7254e' size='3px'>view code</font></summary>

```
OpenFileDialog ofd = new OpenFileDialog();
ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
if (ofd.ShowDialog() != DialogResult.OK) return;
var imagebyte = File.ReadAllBytes(ofd.FileName);
 Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));

 //OCRModelConfig config = null;
 // 使用服务器模型
 OCRModelConfig config = new OCRModelConfig();
 string root =Environment.CurrentDirectory;
 config = new OCRModelConfig();
 string modelPathroot = root + @"\inferenceserver";
 config.det_infer = modelPathroot + @"\ch_ppocr_server_v2.0_det_infer";
 config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
 config.rec_infer = modelPathroot + @"\ch_ppocr_server_v2.0_rec_infer";
 config.keys = modelPathroot + @"\ppocr_keys.txt"; 

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
</details>


 [windows下C++预测库下载地址](https://paddleinference.paddlepaddle.org.cn/user_guides/download_lib.html#windows)


 **关于源码编译，建议采用vs2022版本编译，如果遇到无法编译，请切换成release后再切换回debug即可。** 

如果因框架编译问题无法编译，请修改PaddleOCRSharp\PaddleOCRSharp.csproj文件，删除当前电脑没有的框架，
具体框架说明见微软文档[SDK 样式项目中的目标框架](https://docs.microsoft.com/zh-cn/dotnet/standard/frameworks)
```
 <TargetFrameworks>
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;
</TargetFrameworks>
```

[全部调用参数](https://github.com/raoyutian/paddle-ocrsharp/blob/main/PaddleOCRSharp/OCRParameter.cs)

[PaddleOCR官方参数](https://gitee.com/paddlepaddle/PaddleOCR/tree/release/2.4/deploy/cpp_infer)


## 二、文件夹结构

 
```
PaddleOCR                    //PaddleOCR.dll文件的源代码文件夹
|--cpp                       //PaddleOCR.dll的Cpp文件
|--include                   //PaddleOCR.dll的.h文件

PaddleOCRLib                 //OCR运行需要的文件
|--inference                 //OCR的模型库文件夹
|--libiomp5md.dll            //第三方引用库
|--mkldnn.dll                //第三方引用库
|--mklml.dll                 //第三方引用库
|--opencv_world411.dll       //第三方引用库
|--paddle_inference.dll      //飞桨库
|--PaddleOCR.dll             //基于开源项目PaddleOCR修改的C++动态库
PaddleOCRSharp               //.NET封装库项目
PaddleOCRDemo                //Demo文件夹
|--Cpp                       //PaddleOCR.dll的头文件和库文件，方便C++调用PaddleOCR.dll
|--PaddleOCRCppDemo          //C++调用示例项目
|--PaddleOCRSharpDemo        //.NET调用示例项目
|--WebAPIDemo                //.NET的WebAPI示例项目

```

## 三、.NET使用示例

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

[C++示例代码](https://github.com/raoyutian/paddle-ocrsharp/blob/main/PaddleOCRDemo/PaddleOCRCppDemo/PaddleOCRCppDemo.cpp)


## 四、[常见问题与解决方案](https://github.com/raoyutian/paddle-ocrsharp/blob/main/doc/README_question.md)

### 喜欢的就给个星，谢谢！QQ交流群：318860399，有问题可以加QQ群咨询。

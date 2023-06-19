### 简体中文 | [English](https://github.com/raoyutian/PaddleOCRSharp/blob/main/README_en.md)     |[更新记录](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/README_update.md)

#### 如果对你有用或者喜欢，那就给颗星赞，点个赞。谢谢！

#### 最新代码在 https://gitee.com/raoyutian/paddle-ocrsharp

## 介绍
-----
  **PaddleOCRSharp** 是一个基于百度飞桨[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改并封装的.NET的OCR工具类库。包含文本识别、文本检测、表格识别功能。本项目针对小图识别不准的情况下做了优化，比飞桨原代码识别准确率有所提高。包含总模型仅8.6M的超轻量级中文OCR，单模型支持中英文数字组合识别、竖排文本识别、长文本识别。同时支持中英文、纯英文以及多种语言文本检测识别。

**PaddleOCRSharp**封装极其简化，实际调用仅几行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。Nuget包即装即用，可以离线部署，不需要网络就可以识别的高精度中英文OCR。  

 本项目核心组件PaddleOCR.dll,由C++编写，根据百度飞桨[PaddleOCR](https://github.com/paddlepaddle/PaddleOCR)的C++代码修改而成。目前已经支持C++、.NET、Python、Golang、Rust等开发语言的直接API接口调用。PaddleOCR.dll导出标准C函数接口，欢迎更多的开发者使用其他语言调用。

本项目支持官方所有公开的通用OCR模型，PPOCRV2,PPOCRV3。
如果使用v2模型，请设置OCR识别参数OCRParameter对象的属性rec_img_h=32，本项目默认使用V3模型，默认rec_img_h=48：

本项目只能在X64的CPU上编译和使用，只能在avx指令集上的CPU上使用。

本项目目前支持以下.NET框架：

```
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;net481;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;net7.0;

```

本项目提供了两个SDK，一个是C++版本，一个是.net版本，.net版本是对C++版本的二次封装，其他语言开发亦是调用C++版本。


本项目目前支持以下.NET框架：

```
net35;net40;net45;net451;net452;net46;net461;net462;net47;net471;net472;net48;net481;
netstandard2.0;netcoreapp3.1;
net5.0;net6.0;net7.0;

```

本项目提供了两个SDK，一个是C++版本，一个是.net版本，.net版本是桥接C++的封装，核心还是C++代码。

 

#### 1.文件夹结构

```
PaddleOCRSharp               //该文件夹包含.NET对PaddleOCR封装类库项目
PaddleOCRDemo                //该文件夹包含OCR示例Demo文件夹
|--PaddleOCRCppDemo          //C++调用示例项目
|--PaddleOCRSharpDemo        //.NET调用示例项目
|--python                    //python调用示例项目
|--Go                       //Golang调用示例项目

```
 

#### 2. .NET版编译

[.NET版编译](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/Csharp.md) 



## 使用与部署
------

#### 1. 在C++中使用PaddleOCR

[在C++中使用PaddleOCR](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/UseInCpp.md) 

#### 2. 在.NET中使用PaddleOCRSharp

[在.NET中使用PaddleOCRSharp](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/UseInCsharp.md) 

## 模型
------
OCR识别模型库支持官方所有的模型，也支持自己训练的模型。完全按照飞桨OCR接口搭桥。
本项目部署自带的一种轻量版8.6M模型库、服务器版模型库（更准确，需要自行下载），可以自行更改模型库适用实际需求。

|模型名称|模型大小|下载地址|备注|
|---|---|---|---|
|ch_PP-OCRv2  |10M  |[中英文轻量v2](https://github.com/raoyutian/PaddleOCRSharp/raw/main/models/PP-OCRv2/inference.zip)  | |
|en_PP-OCRv2  |4M   |[英文数字v2](https://github.com/raoyutian/PaddleOCRSharp/raw/main/models/PP-OCRv2/en.zip)  |  |
|ch_PP-OCRv3  |12M  |[中英文轻量v3](https://github.com/raoyutian/PaddleOCRSharp/raw/main/models/PP-OCRv3/inference_v3.zip)|   |
|en_PP-OCRv3  |10M  |[英文数字v3](https://github.com/raoyutian/PaddleOCRSharp/raw/main/models/PP-OCRv3/en_v3.zip)|   |

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



## 常见问题与解决方案

[常见问题与解决方案](https://github.com/raoyutian/PaddleOCRSharp/blob/main/doc/README_question.md)

##  技术交流方式
------
#### QQ技术交流群：318860399。
#### 微信公众号：明月心技术学堂。
![输入图片说明](doc/%E5%85%AC%E4%BC%97%E5%8F%B7%E4%BA%8C%E7%BB%B4%E7%A0%81.jpg)
![输入图片说明](doc/%E6%98%8E%E6%9C%88%E5%BF%83%E6%8A%80%E6%9C%AF%E5%AD%A6%E5%A0%82%E7%BE%A4%E8%81%8A%E4%BA%8C%E7%BB%B4%E7%A0%81.png)
#### [个人博客地址： https://www.cnblogs.com/raoyutian/]( https://www.cnblogs.com/raoyutian/)

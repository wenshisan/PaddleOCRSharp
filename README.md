# PaddleOCRSharp

# 介绍
 本项目是一个基于PaddleOCR的C++代码修改并封装的.NET的类库。包含文本识别、文本检测、基于文本检测结果的统计分析的表格识别功能，同时针对小图识别不准的情况下，做了优化，提高识别准确率。项目封装极其简化，实际调用仅一行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。

其中PaddleOCR.dll文件是基于开源项目PaddleOCR的C++代码修改而成的C++动态库，基于opencv的x64编译而成的。

[百度飞桨PaddleOCR项目地址（码云）](https://gitee.com/paddlepaddle/PaddleOCR)

[百度飞桨PaddleOCR项目地址（GitHub）](https://github.com/paddlepaddle/PaddleOCR)

模型库支持轻量版（本项目）、服务器版模型库（更准确），可以自行更改模型库适用实际需求。

[百度飞桨PaddleOCR模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/release/2.4/doc/doc_ch/models_list.md)
 
关于源码编译，建议采用vs2019及以上版本编译

# 版本更新
### v1.1.0

1.移除命令行日志输出；

2.修复PaddleOCRSharp.dll文本识别不到报错的问题


# 文件夹结构
Cpp //PaddleOCR.dll的头文件和库文件，方便C++调用PaddleOCR.dll

[C++示例代码](https://gitee.com/raoyutian/paddle-ocrsharp/blob/master/Cpp/PaddleOCRCppDemo.cpp)

```
#include <iostream>
#include <Windows.h>
#include "include/PaddleOCR.h"
#include "include/OCRResult.h"
#include <tchar.h>
#include "string"
#pragma comment (lib,"PaddleOCR.lib")
using namespace std;
int main()
{
	LpOCRResult lpocrreult;
	modeldata md;
	OCRParameter parameter;
	char path[MAX_PATH];
	GetCurrentDirectoryA(MAX_PATH, path);
	string cls_infer(path);
	cls_infer += "\\inference\\ch_ppocr_mobile_v2.0_cls_infer";
	string rec_infer(path);
	rec_infer += "\\inference\\ch_PP-OCRv2_rec_infer";
	string det_infer(path);
	det_infer += "\\inference\\ch_PP-OCRv2_det_infer";
	string ocrkeys(path);
	ocrkeys += "\\inference\\ppocr_keys.txt";
	string imagefile(path);
	imagefile += "\\test.png";
	md.cls_infer = const_cast<char*>(cls_infer.c_str());
	md.rec_infer = const_cast<char*>(rec_infer.c_str());
	md.det_infer = const_cast<char*>(det_infer.c_str());
	md.keys = const_cast<char*>(ocrkeys.c_str());
	md.imagefile = const_cast<char*>(imagefile.c_str());
	int  cout =Detect(md.det_infer, md.cls_infer, md.rec_infer, md.keys, md.imagefile, parameter, &lpocrreult);
	for (size_t i = 0; i < cout; i++)
	{
		wstring ss =(WCHAR*)(lpocrreult->pOCRText[i].ptext);
		std::wcout <<ss;
	}
	FreeDetectMem(lpocrreult);
}

```

运行需要用的库文件



```
PaddleOCRLib  //OCR运行需要的文件
|--inference     //OCR的模型库文件夹
|--libiomp5md.dll   //第三方引用库
|--mkldnn.dll   //第三方引用库
|--mklml.dll   //第三方引用库
|--opencv_world411.dll   //第三方引用库
|--paddle_inference.dll   //飞桨库
|--PaddleOCR.dll   //基于开源项目PaddleOCR修改的C++动态库
PaddleOCRSharp  //.NET封装库
```

#.net使用示例
```
 OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var imagebyte = File.ReadAllBytes(ofd.FileName);
            Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));
            OCRResult ocrResult = PaddleOCRSharp.PaddleOCRHelper.DetectText(bitmap,null);
            if (ocrResult != null)
            {
                MessageBox.Show(ocrResult.Text,"识别结果");
            }
```

# 喜欢的给个星，谢谢
# QQ交流群：318860399
 ![输入图片说明](PaddleOCRSharp/PaddleOCRSharp%E4%BA%A4%E6%B5%81%E7%BE%A4%E7%BE%A4%E8%81%8A%E4%BA%8C%E7%BB%B4%E7%A0%81.png)


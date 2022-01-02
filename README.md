# PaddleOCRSharp

# 介绍
 本项目是一个基于PaddleOCR的C++代码修改并封装的.NET的类库。包含文本识别、文本检测、基于文本检测结果的统计分析的表格识别功能，同时针对小图识别不准的情况下，做了优化，提高识别准确率。项目封装极其简化，实际调用仅几行代码，极大的方便了中下游开发者的使用和降低了PaddleOCR的使用入门级别，同时提供不同的.NET框架使用，方便各个行业应用开发与部署。

其中PaddleOCR.dll文件是基于开源项目PaddleOCR的C++代码修改而成的C++动态库，基于opencv的x64编译而成的。

[百度飞桨PaddleOCR项目地址（码云）](https://gitee.com/paddlepaddle/PaddleOCR)

[百度飞桨PaddleOCR项目地址（GitHub）](https://github.com/paddlepaddle/PaddleOCR)

模型库支持轻量版（本项目）、服务器版模型库（更准确），可以自行更改模型库适用实际需求。

[百度飞桨PaddleOCR模型下载地址](https://gitee.com/paddlepaddle/PaddleOCR/blob/release/2.4/doc/doc_ch/models_list.md)
 
 **关于源码编译，建议采用vs2019及以上版本编译，如果遇到无法编译，请切换成release后再切换回debug即可。** 

# 版本更新
### v1.1.0

1.移除命令行日志输出；

2.修复PaddleOCRSharp.dll文本识别不到报错的问题

### v1.2.0

1.调整模型加载模式，可以加载一次模型，多次识别；

2.增加参数类OCRParameter增加Enable_mkldnn参数，默认开启；

3.优化模型库路径可自定义修改位置；

4.增加C++demo示例项目

 **6.PaddleOCRHelper静态类的检测方法，将在下一版本移除，请使用PaddleOCREngine类代替。** 

# 文件夹结构

 
```

Cpp                          //PaddleOCR.dll的头文件和库文件，方便C++调用PaddleOCR.dll
PaddleOCRLib                 //OCR运行需要的文件
|--inference                 //OCR的模型库文件夹
|--libiomp5md.dll            //第三方引用库
|--mkldnn.dll                //第三方引用库
|--mklml.dll                 //第三方引用库
|--opencv_world411.dll       //第三方引用库
|--paddle_inference.dll      //飞桨库
|--PaddleOCR.dll             //基于开源项目PaddleOCR修改的C++动态库
PaddleOCRSharp               //.NET封装库项目
PaddleOCRCppDemo             //C++调用示例项目
PaddleOCRSharpDemo           //.NET调用示例项目

```
[C++示例代码](https://gitee.com/raoyutian/paddle-ocrsharp/blob/master/PaddleOCRCppDemo/PaddleOCRCppDemo.cpp)

```
﻿#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include "string"
#include <include/Parameter.h>
#include <string.h>
using namespace std;
#pragma comment (lib,"PaddleOCR.lib")
extern "C" {
	/// <summary>
	/// PaddleOCREngine引擎初始化
	/// </summary>
	/// <param name="det_infer"></param>
	/// <param name="cls_infer"></param>
	/// <param name="rec_infer"></param>
	/// <param name="keys"></param>
	/// <param name="parameter"></param>
	/// <returns></returns>
	__declspec(dllimport) int* Initialize(char* det_infer, char* cls_infer, char* rec_infer, char* keys, OCRParameter  parameter);
	/// <summary>
	/// 文本检测
	/// </summary>
	/// <param name="engine"></param>
	/// <param name="imagefile"></param>
	/// <param name="pOCRResult">返回结果</param>
	/// <returns></returns>
	__declspec(dllimport) int  Detect(int* engine, char* imagefile, LpOCRResult* pOCRResult);
	/// <summary>
	/// 释放引擎对象
	/// </summary>
	/// <param name="engine"></param>
	__declspec(dllimport) void FreeEngine(int* engine);
	/// <summary>
	/// 释放文本识别结果对象
	/// </summary>
	/// <param name="pOCRResult"></param>
	__declspec(dllimport) void FreeDetectResult(LpOCRResult pOCRResult);
};

std::wstring string2wstring(const std::string& s)
{
	int len;
	int slength = (int)s.length() + 1;
	len = MultiByteToWideChar(CP_ACP, 0, s.c_str(), slength, 0, 0);
	wchar_t* buf = new wchar_t[len];
	MultiByteToWideChar(CP_ACP, 0, s.c_str(), slength, buf, len);
	std::wstring r(buf);
	delete[] buf;
	return r;
}

int main()
{
	LpOCRResult lpocrreult;
	OCRParameter parameter;
	/*parameter.enable_mkldnn = false;*/
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
	imagefile += "\\test.jpg";
	
	int*  pEngine = Initialize(const_cast<char*>(det_infer.c_str()),
							 const_cast<char*>(cls_infer.c_str()), 
						     const_cast<char*>(rec_infer.c_str()),
							 const_cast<char*>(ocrkeys.c_str()),
		                     parameter);
	
	int  cout = Detect(pEngine, const_cast<char*>(imagefile.c_str()), &lpocrreult);
	std::wcout.imbue(std::locale("chs"));
	for (size_t i = 0; i < cout; i++)
	{
		wstring ss = (WCHAR*)(lpocrreult->pOCRText[i].ptext);
		std::wcout << ss; 
	}
	FreeDetectResult(lpocrreult);
	FreeEngine(pEngine);
	std::cin.get();
}

```



#.net使用示例
```
 OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var imagebyte = File.ReadAllBytes(ofd.FileName);
            Bitmap bitmap = new Bitmap(new MemoryStream(imagebyte));

            OCRModelConfig config = null;
            OCRParameter oCRParameter = null;
            OCRResult ocrResult = new OCRResult();
            using (PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter))
            {
                ocrResult = engine.DetectText(bmp);
            }
            if (ocrResult != null)
            {
                MessageBox.Show(ocrResult.Text,"识别结果");
            }
```
# 喜欢的给个星，谢谢
# QQ交流群：318860399
 


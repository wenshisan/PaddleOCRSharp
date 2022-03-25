#include <iostream>
#include <Windows.h>
#include <tchar.h>
#include "string"
#include <string.h>
#include <include/Parameter.h>
#include <io.h> 
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
	/// 文本检测识别-图片路径
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagefile">图片路径</param>
	/// <param name="pOCRResult">返回结果</param>
	/// <returns></returns>
	__declspec(dllimport) int  Detect(int* engine, char* imagefile, LpOCRResult* pOCRResult);
	
	/// <summary>
	///  文本检测识别-cv Mat
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="cvmat">opencv Mat</param>
	/// <param name="pOCRResult">返回结果</param>
	/// <returns></returns>
	// __declspec(dllimport) int  DetectMat(int* engine, cv::Mat& cvmat, LpOCRResult* pOCRResult);

	/// <summary>
	/// 文本检测识别-图像字节流
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagebytedata">图像字节流</param>
	/// <param name="size">图像字节流长度</param>
	/// <param name="OCRResult">返回结果</param>
	/// <returns></returns>
	__declspec(dllimport) int DetectByte(int* engine, char* imagebytedata, size_t* size, LpOCRResult* OCRResult);

	/// <summary>
	/// 文本检测识别-图像base64
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagebase64">图像base64</param>
	/// <param name="OCRResult">返回结果</param>
	/// <returns></returns>
	__declspec(dllimport) int DetectBase64(int* engine, char* imagebase64, LpOCRResult* OCRResult);

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
	
	/// <summary>
	/// PaddleOCR检测
	/// </summary>
	/// <param name="det_infer"></param>
	/// <param name="imagefile"></param>
	/// <param name="parameter"></param>
	/// <returns></returns>
	__declspec(dllimport) void DetectImage(char* modelPath_det_infer, char* imagefile, OCRParameter parameter);

};

void getFiles(string path, vector<string>& files)
{
	intptr_t   hFile = 0;//文件句柄，过会儿用来查找
	struct _finddata_t fileinfo;//文件信息
	string p;
	if ((hFile = _findfirst(p.assign(path).append("\\*").c_str(), &fileinfo)) != -1)
		//如果查找到第一个文件
	{
		do
		{
			if ((fileinfo.attrib & _A_SUBDIR))//如果是文件夹
			{
				if (strcmp(fileinfo.name, ".") != 0 && strcmp(fileinfo.name, "..") != 0)
					getFiles(p.assign(path).append("\\").append(fileinfo.name), files);
			}
			else//如果是文件
			{
				files.push_back(p.assign(path).append("\\").append(fileinfo.name));
			}
		} while (_findnext(hFile, &fileinfo) == 0);	//能寻找到其他文件

		_findclose(hFile);	//结束查找，关闭句柄
	}
}

int main()
{
	LpOCRResult lpocrreult;
	OCRParameter parameter;
	parameter.enable_mkldnn = true;
	parameter.numThread = 4;
	parameter.BoxScoreThresh = 0.5f;
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
	
	string imagepath(path);
	imagepath += "\\image"; 
	vector<string> images;
	getFiles(imagepath, images);

	int*  pEngine = Initialize(const_cast<char*>(det_infer.c_str()),
							 const_cast<char*>(cls_infer.c_str()), 
						     const_cast<char*>(rec_infer.c_str()),
							 const_cast<char*>(ocrkeys.c_str()),
		                     parameter);
	
	std::wcout.imbue(std::locale("chs"));
	if (images.size() > 0)
	{
		
	 
		for (size_t i = 0; i < images.size(); i++)
		{ 
			std::wcout << "----------------------------------------------------------------- "<< endl;
			int  cout = Detect(pEngine, const_cast<char*>(images[i].c_str()), &lpocrreult);
			if (cout > 0)
			{
				for (int j = cout-1; j >=0; j--)
				{
					wstring ss = (WCHAR*)(lpocrreult->pOCRText[j].ptext);
					std::wcout << ss << endl;

				}
				
			}
			FreeDetectResult(lpocrreult);
		}
	}
	try
	{
		FreeEngine(pEngine);
	}
	catch (const std::exception& e)
	{
		std::wcout << e.what();
	}
	
	std::cin.get();
}


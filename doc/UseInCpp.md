#### 1.1 PaddleOCR的公开接口

```
                 /// <summary>
	/// PaddleOCREngine引擎初始化
	/// </summary>
	/// <param name="det_infer"></param>
	/// <param name="cls_infer"></param>
	/// <param name="rec_infer"></param>
	/// <param name="keys"></param>
	/// <param name="parameter"></param>
	/// <returns></returns>
	  int* Initialize(char* det_infer, char* cls_infer, char* rec_infer, char* keys, OCRParameter  parameter);
	
	/// <summary>
	/// 文本检测识别-图片路径
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagefile">图片路径</param>
	/// <param name="pOCRResult">返回结果</param>
	/// <returns></returns>
	  int  Detect(int* engine, char* imagefile, LpOCRResult* pOCRResult);
	
	/// <summary>
	///  文本检测识别-cv Mat
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="cvmat">opencv Mat</param>
	/// <param name="pOCRResult">返回结果</param>
	/// <returns></returns>
	  int  DetectMat(int* engine, cv::Mat& cvmat, LpOCRResult* pOCRResult);

	/// <summary>
	/// 文本检测识别-图像字节流
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagebytedata">图像字节流</param>
	/// <param name="size">图像字节流长度</param>
	/// <param name="OCRResult">返回结果</param>
	/// <returns></returns>
	  int DetectByte(int* engine, char* imagebytedata, size_t* size, LpOCRResult* OCRResult);

	/// <summary>
	/// 文本检测识别-图像base64
	/// </summary>
	/// <param name="engine">由Initialize返回的引擎</param>
	/// <param name="imagebase64">图像base64</param>
	/// <param name="OCRResult">返回结果</param>
	/// <returns></returns>
	_  int DetectBase64(int* engine, char* imagebase64, LpOCRResult* OCRResult);

	/// <summary>
	/// 释放引擎对象
	/// </summary>
	/// <param name="engine"></param>
	  void FreeEngine(int* engine);
	
	/// <summary>
	/// 释放文本识别结果对象
	/// </summary>
	/// <param name="pOCRResult"></param>
	  void FreeDetectResult(LpOCRResult pOCRResult);

	/// <summary>
	/// CPU支持检测
	/// </summary>
	/// <returns></returns>
	  int IsCPUSupport();
```
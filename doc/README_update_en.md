### v1.1.0

1.移除命令行日志输出；

2.修复PaddleOCRSharp.dll文本识别不到报错的问题

### v1.2.0

1.调整模型加载模式，可以加载一次模型，多次识别；

2.增加参数类OCRParameter增加Enable_mkldnn参数，默认开启；

3.优化模型库路径可自定义修改位置；

4.增加C++demo示例项目

 **PaddleOCRHelper静态类的检测方法，将在下一版本移除，请使用PaddleOCREngine类代替。** 

### v1.2.2

1.增加net35;net48,net6框架支持

2.补齐参数类OCRParameter参数（对标官方PaddleOCR参数）[官方PaddleOCR参数](https://gitee.com/paddlepaddle/PaddleOCR/tree/release/2.4/deploy/cpp_infer)
 
3.增加纯检测接口DetectImage

4.增加检测得分结果输出

5.Demo增加显示识别结果的文字区域
 
### v1.3.0

1.PaddleOCR.dll增加DetectMat、DetectByte、DetectBase64接口

2.PaddleOCRSharp.dll增DetectTextBase64接口、DetectText重载（Byte数组参数）

3.优化.net传输图像数据不再落地生成文件。

4.小图缩放优化功能移到C++代码中。

5.优化PaddleOCRSharp.dll中的识别结果OCRResult类，满足可序列化需要。

6.添加更多框架支持。

7.默认关闭CPU加速开关，即Enable_mkldnn默认等于0.




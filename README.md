# PaddleOCRSharp

# 介绍
本项目是基于开源项目PaddleOCR的的.NET封装库，包含文本OCR功能。
PaddleOCR.dll项目是基于开源项目PaddleOCR修改的C++动态库，由于PaddleOCR.dll引用的opencv没有编译32位，因此PaddleOCR.dll仅能在x64上使用。

PaddleOCR.dll使用了openblas依赖编译，为了降低部署文件大小。
模型库支持轻量版（本项目）、服务器版模型库。

 

[模型下载地址 百度飞桨PaddleOCR地址](https://gitee.com/paddlepaddle/PaddleOCR)
 
# 文件夹结构
Cpp     //PaddleOCR.dll的头文件和库文件，方便C++调用PaddleOCR.dll

PaddleOCRLib  //OCR运行需要的文件

|--inference     //OCR的模型库文件夹

|--openblas.dll   //第三方引用库

|--opencv_world411.dll   //第三方引用库

|--paddle_inference.dll   //飞桨库

|--PaddleOCR.dll   //基于开源项目PaddleOCR修改的C++动态库

PaddleOCRSharp  //.NET封装库


#使用示例
Bitmap bmp= new Bitmap(“XXXXXX.JPG”);

 var ocrResult= PaddleOCRHelper.DetectText(bmp);

返回结果是一个对象，包含一个识别的文本，和文本块的列表





## 喜欢的给个星，谢谢


# QQ交流
 ![输入图片说明](PaddleOCRSharp/PaddleOCRSharp%E4%BA%A4%E6%B5%81%E7%BE%A4%E7%BE%A4%E8%81%8A%E4%BA%8C%E7%BB%B4%E7%A0%81.png)


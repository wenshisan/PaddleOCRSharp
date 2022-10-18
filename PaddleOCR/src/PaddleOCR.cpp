// Copyright (c) 2021 raoyutian Authors. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#include "include/PaddleOCR.h"
#include "glog/logging.h"
#include "omp.h"
#include "opencv2/core.hpp"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/imgproc.hpp"
#include <chrono>
#include <iomanip>
#include <iostream>
#include <ostream>
#include <vector> 
#include <cstring>
#include <fstream>
#include <numeric>
#include <glog/logging.h>
#include <include/ocr_det.h>
#include <include/ocr_cls.h>
#include <include/ocr_rec.h>
#include <include/utility.h>
#include <sys/stat.h>
#include <gflags/gflags.h>
#include <codecvt>
#ifdef _WIN64
#include <include/environment_check.h>
#endif 

DEFINE_bool(use_tensorrt, false, "Whether use tensorrt.");
DEFINE_string(precision, "fp32", "Precision be one of fp32/fp16/int8");

#ifdef _WIN64
DEFINE_bool(benchmark, true, "Whether use benchmark.");
#endif 

DEFINE_int32(rec_batch_num, 4, "rec_batch_num.");

using namespace std;
using namespace cv;
using namespace PaddleOCR;
 
OCRParameter _parameter;
std::mutex m_lock;
#pragma region 环境监测
/// <summary>
/// 监测环境CPU是否支持PaddleOCR.
/// </summary>
/// <returns>0:代表CPU不支持AVX指令集，1代表CPU支持AVX指令集,2代表CPU支持AVX2指令集</returns>
int IsCPUSupport()
{
#ifdef _WIN64
    return Environment::IsCPUSupport_AVX();
#endif 
    return 1;
  
}
#pragma endregion

#pragma region 其他

    /// <summary>
    /// 模型路径内存赋值
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="src"></param>
    void SetmodelPath(char** dst, char* src)
    {
        int len = strlen(src) + 1;
        *dst = new char[len];
        memset(*dst, 0, len);
        memcpy(*dst, src, len);
    }
    std::wstring to_wide_string(const std::string& input)
    {
        std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
        return converter.from_bytes(input);
    }
#pragma region base64有关

    static const std::string base64_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    static inline bool is_base64(uchar c) {
        return (isalnum(c) || (c == '+') || (c == '/'));
    }
    std::vector<uchar> base64_decode(std::string const& encoded_string) {
        int in_len = encoded_string.size();
        int i = 0;
        int j = 0;
        int in_ = 0;
        uchar char_array_4[4], char_array_3[3];
        std::vector<uchar> ret;

        while (in_len-- && (encoded_string[in_] != '=') && is_base64(encoded_string[in_])) {
            char_array_4[i++] = encoded_string[in_]; in_++;
            if (i == 4) {
                for (i = 0; i < 4; i++)
                    char_array_4[i] = base64_chars.find(char_array_4[i]);

                char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
                char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
                char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

                for (i = 0; (i < 3); i++)
                    ret.push_back(char_array_3[i]);
                i = 0;
            }
        }

        if (i) {
            for (j = i; j < 4; j++)
                char_array_4[j] = 0;

            for (j = 0; j < 4; j++)
                char_array_4[j] = base64_chars.find(char_array_4[j]);

            char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
            char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
            char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

            for (j = 0; (j < i - 1); j++) ret.push_back(char_array_3[j]);
        }

        return ret;
    }
#pragma endregion
#pragma endregion

#pragma region 初始化

    /// <summary>
    /// 初始化OCR引擎
    /// </summary>
    /// <param name="modelPath_det_infer"></param>
    /// <param name="modelPath_cls_infer"></param>
    /// <param name="modelPath_rec_infer"></param>
    /// <param name="keys"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    OCREngine* Initialize(char* modelPath_det_infer, char* modelPath_cls_infer, char* modelPath_rec_infer, char* keys, OCRParameter parameter)
    {
        OCREngine* engine = new OCREngine();
        SetmodelPath(&engine->det_infer, modelPath_det_infer);
        SetmodelPath(&engine->cls_infer, modelPath_cls_infer);
        SetmodelPath(&engine->rec_infer, modelPath_rec_infer);
        SetmodelPath(&engine->keys, keys);
       
        _parameter = parameter;
        if (parameter.det)
        {
            engine->det = new DBDetector(engine->det_infer, parameter.use_gpu, parameter.gpu_id,
                parameter.gpu_mem, parameter.cpu_math_library_num_threads,
                parameter.enable_mkldnn, parameter.max_side_len, parameter.det_db_thresh,
                parameter.det_db_box_thresh, parameter.det_db_unclip_ratio,
                parameter.det_db_score_mode, parameter.use_dilation,
                FLAGS_use_tensorrt, FLAGS_precision);

            if (parameter.visualize) 
            {
                Utility::CreateDir("./output/");
            }
        }
        if (parameter.rec)
        {
            engine->rec = new CRNNRecognizer(engine->rec_infer, parameter.use_gpu, parameter.gpu_id,
                parameter.gpu_mem, parameter.cpu_math_library_num_threads,
                parameter.enable_mkldnn, engine->keys,
                FLAGS_use_tensorrt, FLAGS_precision, parameter.rec_batch_num, parameter.rec_img_h, parameter.rec_img_w);
        }
        if (parameter.use_angle_cls && parameter.cls) {
            engine->cls = new Classifier(engine->cls_infer, parameter.use_gpu, parameter.gpu_id,
                parameter.gpu_mem, parameter.cpu_math_library_num_threads,
                parameter.enable_mkldnn, parameter.cls_thresh,
                FLAGS_use_tensorrt, FLAGS_precision, parameter.cls_batch_num);
        }
        return  engine;
    }
#pragma endregion

#pragma region 图像识别

    /// <summary>
    /// 检测文本
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="imagefile"></param>
    /// <param name="OCRResult"></param>
    /// <returns></returns>
    int Detect(OCREngine* engine, char* imagefile, LpOCRResult* OCRResult)
    {
        cv::Mat srcimg = cv::imread(imagefile, cv::IMREAD_COLOR);
        if (!srcimg.data) return 0;
        return DetectMat(engine, srcimg, OCRResult);
    }

    /// <summary>
    /// 检测文本cv::Mat
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="imagefile"></param>
    /// <param name="OCRResult"></param>
    /// <returns></returns>
    int DetectMat(OCREngine* engine, cv::Mat& mat, LpOCRResult* OCRResult)
    {
        cv::Mat srcimg;
        if (!mat.data) return 0;
        std::vector<std::vector<std::vector<int>>> boxes;//这里是每个文本区域的四个点坐标集合
        std::vector<Textblock> textblocks;//这里是每个文本区域文本
        std::vector<double> det_times;
        std::vector<double> rec_times;
        std::vector<float> rec_scores;
        std::vector<float> scores;
        std::vector<int> cls_labels;
        std::vector<double> cls_times;
        std::vector<float> cls_scores;
        std::vector<std::string> rec_texts;
        std::vector<Mat> img_list;

#pragma region 缩放
        int scale = 1;
        if (mat.rows <= 48 || mat.cols <= 48)
        {
            scale = 3;
        }
        else if (mat.rows <= 72 || mat.cols <= 72)
        {
            scale = 2;
        }
        cv::Size size = cv::Size();
        cv::resize(mat, srcimg, size, scale, scale);
        mat.release();
#pragma endregion
      
        
        m_lock.lock();

        #pragma region 异步锁作用范围

                if (engine->det != nullptr)
                {
                    engine->det->Run(srcimg, boxes, det_times);
                }

                std::vector<std::vector<OCRPredictResult>> ocr_results;
                std::wstring text;
                //预测列表
                for (int j = 0; j < boxes.size(); j++) {
                    img_list.push_back(Utility::GetRotateCropImage(srcimg, boxes[j]));
                }

                if (img_list.size() == 0)
                {
                    img_list.push_back(srcimg);
                    std::vector<std::vector<int>> box;
           
                    std::vector<int> point;
                    point.push_back(0);
                    point.push_back(0);
                    box.push_back(point);
           
                    point.clear();
                    point.push_back(srcimg.cols);
                    point.push_back(0);
                    box.push_back(point);
           
                    point.clear();
                    point.push_back(srcimg.cols);
                    point.push_back(srcimg.rows);
                    box.push_back(point);

                    point.clear();
                    point.push_back(0);
                    point.push_back(srcimg.rows);
                    box.push_back(point);
                    boxes.push_back(box);

                }
                //初始化集合
                for (int i = 0; i < img_list.size(); i++) {
           
                    cls_labels.push_back(-1);
                    cls_scores.push_back(0);
            
                    rec_texts.push_back("");
                    rec_scores.push_back(0);
                }

               if (engine->cls != nullptr && _parameter.cls) 
               {
                     engine->cls->Run(img_list, cls_labels, cls_scores,cls_times);
                     for (int i = 0; i < img_list.size(); i++) {
                         if (cls_labels[i] % 2 == 1 &&
                             cls_scores[i] > _parameter.cls_thresh) {
                             cv::rotate(img_list[i], img_list[i], 1);
                         }
                     }
               }
               engine->rec->Run(img_list, rec_texts, rec_scores,rec_times);

               std::vector<OCRPredictResult>  predictResults;
               for (int i = 0; i < img_list.size(); i++)
               {
                   std::wstring str = to_wide_string( rec_texts[i]);
                   textblocks.push_back(Textblock(str, boxes[i], rec_scores[i]));

                   OCRPredictResult  predictResult;
                   predictResult.box = boxes[i];
                   predictResult.text = rec_texts[i];
                   predictResult.score = rec_scores[i];
                   predictResult.cls_score = cls_scores[i];
                   predictResults.push_back(predictResult);
                   img_list[i].release();
               }
        #pragma endregion
 
       m_lock.unlock();


       if (textblocks.empty())
            return 0;

        if (_parameter.visualize && _parameter.det) {
           // Utility::VisualizeBboxes(srcimg, predictResults,"./output/ocr_vis.png");
            Utility::VisualizeBboxes(srcimg, predictResults, "ocr_vis.png");
        }

        LpOCRResult pOCRResult = new _OCRResult();
        *OCRResult = pOCRResult;
        pOCRResult->textCount = textblocks.size();
        pOCRResult->pOCRText = new _OCRText[pOCRResult->textCount];
        int idx = 0;
        for (vector<Textblock>::iterator it = textblocks.begin(); it != textblocks.end(); it++) {


            //文本长度
            pOCRResult->pOCRText[idx].textLen = it->textblock.length() * 2 + 1;
            //文本
            pOCRResult->pOCRText[idx].ptext = new char[pOCRResult->pOCRText[idx].textLen + 1];
            memset(pOCRResult->pOCRText[idx].ptext, 0, pOCRResult->pOCRText[idx].textLen + 1);
            memcpy(pOCRResult->pOCRText[idx].ptext, it->textblock.c_str(), pOCRResult->pOCRText[idx].textLen);

            //P1
            pOCRResult->pOCRText[idx].points[0].x = (int)(it->box[0][0] / scale);
            pOCRResult->pOCRText[idx].points[0].y = (int)(it->box[0][1] / scale);
            //P2
            pOCRResult->pOCRText[idx].points[1].x = (int)(it->box[1][0] / scale);
            pOCRResult->pOCRText[idx].points[1].y = (int)(it->box[1][1] / scale);
            //P3
            pOCRResult->pOCRText[idx].points[2].x = (int)(it->box[2][0] / scale);
            pOCRResult->pOCRText[idx].points[2].y = (int)(it->box[2][1] / scale);
            //P4
            pOCRResult->pOCRText[idx].points[3].x = (int)(it->box[3][0] / scale);
            pOCRResult->pOCRText[idx].points[3].y = (int)(it->box[3][1] / scale);

            //score得分
            pOCRResult->pOCRText[idx].score = it->score;
            idx++;
        }
        //释放内存
        srcimg.release();
        return textblocks.size();
    }

    /// <summary>
    /// 检测文本Byte字节
    /// </summary>
    /// <param name="engine">OCR引擎</param>
    /// <param name="imagedata">图像的内存byte数组</param>
    /// <param name="size">图像的内存byte数组大小</param>
    /// <param name="OCRResult">OCR识别结果</param>
    /// <returns></returns>
    int DetectByte(OCREngine* engine, char* imagedata, size_t* size, LpOCRResult* OCRResult)
    {
        long bytesize = (long)size;
        if (bytesize <= 0)
        {
            cout << "the size  must greater than 0";
            return 0;
        }
        vector<uchar> data;
        data.assign(imagedata, imagedata + bytesize);
        cv::Mat srcimg = cv::imdecode(data, cv::IMREAD_COLOR);
        data.clear();
        vector<uchar>().swap(data);
        return DetectMat(engine, srcimg, OCRResult);
    }

    /// <summary>
    /// 检测文本Byte字节Base64
    /// </summary>
    /// <param name="engine">OCR引擎</param>
    /// <param name="base64">图像的内存byte数组base64</param>>
    /// <param name="OCRResult">OCR识别结果</param>
    /// <returns></returns>
    int DetectBase64(OCREngine* engine, char* base64, LpOCRResult* OCRResult)
    {
        vector<uchar> data = base64_decode(base64);
        cv::Mat srcimg = cv::imdecode(data, cv::IMREAD_COLOR);
        data.clear();
        vector<uchar>().swap(data);
        return  DetectMat(engine, srcimg, OCRResult);
    }

#pragma endregion

#pragma region 释放内存

    /// <summary>
    /// 释放OCR引擎
    /// </summary>
    /// <param name="engine"></param>
    void FreeEngine(OCREngine* engine)
    {
        if (engine == nullptr) return;
        delete  engine->cls_infer;
        engine->cls_infer = NULL;
        delete  engine->det_infer;
        engine->det_infer = NULL;
        delete  engine->rec_infer;
        engine->rec_infer = NULL;
        delete  engine->keys;
        engine->keys = NULL;
        delete  engine->rec;
        engine->rec = NULL;
        delete  engine->det;
        engine->det = NULL;
        delete engine;
        engine = nullptr;
    }

    /// <summary>
    /// 释放OCR结果
    /// </summary>
    /// <param name="pOCRResult"></param>
    void FreeDetectResult(LpOCRResult pOCRResult) {
        if (pOCRResult == nullptr)  return;
        if (pOCRResult->textCount == 0) return;
        //文本快数量
        for (int i = 0; i < pOCRResult->textCount; i++)
        {
            delete[] pOCRResult->pOCRText[i].ptext;
            pOCRResult->pOCRText[i].ptext = NULL;
        }
        delete[] pOCRResult->pOCRText;
        pOCRResult->pOCRText = NULL;

        //整个OCR文本数组内存
        delete pOCRResult;
        pOCRResult = nullptr;
    }
#pragma endregion

#pragma region 图像预测
    /// <summary>
    /// 检测图像
    /// </summary>
    /// <param name="modelPath_det_infer"></param>
    /// <param name="imagefile"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    void DetectImage(char* modelPath_det_infer, char* imagefile, OCRParameter parameter)
    {
        cv::Mat srcimg = cv::imread(imagefile, cv::IMREAD_COLOR);
        if (!srcimg.data) return;
        parameter.visualize = true;
        parameter.det = true;

        DBDetector det(modelPath_det_infer, parameter.use_gpu, parameter.gpu_id,
            parameter.gpu_mem, parameter.cpu_math_library_num_threads,
            parameter.enable_mkldnn, parameter.max_side_len, parameter.det_db_thresh,
            parameter.det_db_box_thresh, parameter.det_db_unclip_ratio,
            parameter.det_db_score_mode, parameter.use_dilation,
            FLAGS_use_tensorrt, FLAGS_precision);
        std::vector<std::vector<std::vector<int>>> boxes;//这里是每个文本区域的四个点坐标集合
        std::vector<Textblock> textblocks;//这里是每个文本区域文本
        std::vector<double> det_times;

        det.Run(srcimg, boxes, det_times);
          std::vector<OCRPredictResult>  predictResults;
          for (size_t i = 0; i < boxes.size(); i++)
          {
              OCRPredictResult  predictResult;
              predictResult.box = boxes[i];
              predictResults.push_back(predictResult);
          }
        
        if (parameter.visualize && parameter.det) {
            // Utility::VisualizeBboxes(srcimg, predictResults,"./output/ocr_vis.png");
            Utility::VisualizeBboxes(srcimg, predictResults, "ocr_vis.png");
        }
    }
#pragma endregion



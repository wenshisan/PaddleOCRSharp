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

#pragma once
#pragma pack(push,1)
#include <vector>
using namespace std;


#pragma pack(push,1)
/// <summary>
/// OCR识别参数
/// </summary>
struct OCRParameter
{
	//通用参数
	bool use_gpu;
	int gpu_id;
	int gpu_mem;
	int numThread;
	bool enable_mkldnn;
	bool use_custom_model;
	//检测模型相关
	int    Padding;
	int    MaxSideLen;
	float  BoxScoreThresh;
	float   BoxThresh;
	float   UnClipRatio;
	bool use_polygon_score;
	bool visualize;
	bool    DoAngle;
	bool   MostAngle;
	
	//方向分类器相关
	bool use_angle_cls;
	float   cls_thresh;

	OCRParameter()
	{
		//通用参数
		use_gpu = false;
		gpu_id = 0;
		gpu_mem = 4000;
		numThread = 2;
		enable_mkldnn = false;
		use_custom_model = false;
		//检测模型相关
		Padding = 50;
		MaxSideLen = 2048;
		BoxThresh = 0.3f;
		BoxScoreThresh = 0.618f;
		UnClipRatio = 1.6f;
		use_polygon_score = false;
		visualize = false;

		DoAngle = true;
		MostAngle = true;
		
		//方向分类器相关
		use_angle_cls = false;
		cls_thresh = 0.9f;
	}
};

/// <summary>
/// 文本区域
/// </summary>
struct Textblock {

	std::wstring textblock;
	std::vector<std::vector<int>> box;
	float score;
	Textblock(wstring textblock, std::vector<std::vector<int>> box, float score) {
		this->textblock = textblock;
		this->box = box;
		this->score = score;
	}
};

/// <summary>
/// OCR文本区域四周的点
/// </summary>
struct _OCRTextPoint {
	int x;
	int y;
	_OCRTextPoint() :x(0), y(0) {
	}
};

/// <summary>
/// OCR文本
/// </summary>
struct _OCRText {
	//textblock文本
	int textLen;
	char* ptext;
	//一个textblock四个点
	_OCRTextPoint points[4];
	//得分
	float score;
	_OCRText() {
		textLen = 0;
		ptext = nullptr;
		score = 0.0f;
	}
};
/// <summary>
/// OCR识别结果
/// </summary>
typedef struct _OCRResult {
	//textblock文本个数
	int textCount;
	_OCRText* pOCRText;
}OCRResult, * LpOCRResult;

#pragma pack(pop) 
#pragma pack(pop) 

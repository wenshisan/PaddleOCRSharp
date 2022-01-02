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
struct OCRParameter
{
	int numThread;
	int    Padding;
	int    MaxSideLen;
	float  BoxScoreThresh;
	float   BoxThresh;
	float   UnClipRatio;
	bool    DoAngle;
	bool   MostAngle;
	bool enable_mkldnn;
	bool use_gpu;
	OCRParameter()
	{
		numThread = 2;
		Padding = 50;
		MaxSideLen = 2048;
		BoxScoreThresh = 0.618f;
		BoxThresh = 0.3f;
		UnClipRatio = 2.0f;
		DoAngle = true;
		MostAngle = true;
		enable_mkldnn = true;
		use_gpu = false;
	}
};

struct Textblock {
	std::wstring textblock;
	std::vector<std::vector<int>> box;
	Textblock(wstring textblock, std::vector<std::vector<int>> box) {
		this->textblock = textblock;
		this->box = box;
	}
};

//textblock文本四个角的点
struct _OCRTextPoint {
	int x;
	int y;
	_OCRTextPoint() :x(0), y(0) {
	}
};

struct _OCRText {
	//textblock文本
	int textLen;
	char* ptext;
	//一个textblock四个点
	_OCRTextPoint points[4];
	_OCRText() {
		textLen = 0;
		ptext = nullptr;
	}
};

typedef struct _OCRResult {
	//textblock文本个数
	int textCount;
	_OCRText* pOCRText;
}OCRResult, * LpOCRResult;
#pragma pack(pop) 

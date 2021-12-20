#pragma once

#pragma pack(push,1)
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
	}
};

struct modeldata
{
	char* det_infer;
	char* cls_infer;
	char* rec_infer; 
	char* keys;
	char* imagefile;
};
#pragma pack(pop) 

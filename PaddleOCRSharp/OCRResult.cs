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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
namespace PaddleOCRSharp
{
    /// <summary>
    /// OCR识别结果
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class OCRResult
    {
        /// <summary>
        /// 文本块列表
        /// </summary>
        public List<TextBlock> TextBlocks { get; set; } = new List<TextBlock>();
        /// <summary>
        /// 识别结果文本
        /// </summary>
        public string Text=>this.ToString();
        public override string ToString()=>  string.Join("", TextBlocks.Select(x => x.Text).ToArray());
       
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class OCRPoint
    {
        /// <summary>
        /// X坐标，单位像素
        /// </summary>
        public int X;
        /// <summary>
        /// Y坐标，单位像素
        /// </summary>
        public int Y;
        public OCRPoint()
        {
        }
        public OCRPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString() => $"({X},{Y})";
    }
}

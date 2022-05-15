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

namespace PaddleOCRSharp
{
    /// <summary>
    /// 识别的文本块
    /// </summary>
    public class TextBlock
    {
        public List<OCRPoint> BoxPoints { get; set; } = new List<OCRPoint>();
        public string Text { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        public float Score { get; set; }
        public override string ToString()
        {
            string str = string.Join(",", BoxPoints.Select(x => x.ToString()).ToArray());
            return $"{Text},{Score},[{str}]";
        }
    }
}

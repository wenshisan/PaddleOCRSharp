using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRTestConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OcrFromImage.MainClass mainClass = new OcrFromImage.MainClass();
            var result = mainClass.GetResult("C:\\Users\\wen\\Pictures\\屏幕截图 2024-01-22 162726.png");
            Console.WriteLine(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PaddleOCRSharp;
using System;
using System.Drawing;
namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaddleOCRController : ControllerBase
    {

        //[HttpGet]
        //public ViewResult Get()
        //{
        //    return new ViewResult();
        //}

        [HttpPost("DetectText")]
        public string DetectText([FromBody]string base64)
        {
            var imagebyte = Convert.FromBase64String(base64);
            Bitmap bitmap = new Bitmap(new System.IO.MemoryStream(imagebyte));
            var result = PaddleOCRHelper.Instance.DetectText(bitmap);
            Console.WriteLine(result.Text);
            return result.Text;
        }
    }
}

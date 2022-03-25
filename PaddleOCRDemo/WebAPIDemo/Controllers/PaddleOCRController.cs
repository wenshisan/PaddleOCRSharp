using Microsoft.AspNetCore.Mvc;
using System;
namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaddleOCRController : ControllerBase
    {
        [HttpPost("DetectText")]
        public string DetectText([FromBody]string base64)
        {
            var result = PaddleOCRHelper.Instance.DetectTextBase64(base64);
            Console.WriteLine(result.Text);
            return result.Text;
        }
        //[HttpPost("DetectImageByte")]
        //public string DetectImageByte([FromBody] byte[] imagebyte)
        //{
        //    var result = PaddleOCRHelper.Instance.DetectText(imagebyte);
        //    Console.WriteLine(result.Text);
        //    return result.Text;
        //}

        [HttpPost("DetectImageByte")]
        public string DetectImageByte([FromBody] byte[] imagebyte)
        {
            //var result = PaddleOCRHelper.Instance.DetectText(imagebyte);
            //Console.WriteLine(result.Text);
            //return result.Text;
            return "RRR";
        }

    }
}

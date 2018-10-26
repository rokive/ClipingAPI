using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ClipingAPI.Controllers
{
    public class FileController : ApiController
    {
        [HttpPost]
        public void UploadFile()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];

                if (httpPostedFile != null)
                {
                    // Validate the uploaded image(optional)

                    // Get the complete file path
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);

                    // Save the uploaded file to "UploadedFiles" folder
                    httpPostedFile.SaveAs(fileSavePath);
                }
            }
            //var httpRequest = HttpContext.Current.Request;
            //if (httpRequest.Files.Count < 1)
            //{
            //    return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}

            //foreach (string file in httpRequest.Files)
            //{
            //    var postedFile = httpRequest.Files[file];
            //    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
            //    postedFile.SaveAs(filePath);
            //    // NOTE: To store in memory use postedFile.InputStream
            //}

            //return Request.CreateResponse(HttpStatusCode.Created);
        }
        [HttpPost, Route("api/file/upload")]
        public async Task<IHttpActionResult> Upload(string userName)
        {

            string returnPath = "";
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var splitFilename = filename.Split('.');

                var extention = getExtention(filename); 
                var buffer = await file.ReadAsByteArrayAsync();
                //Do whatever you want with filename and its binaray data.
                Image img = System.Drawing.Image.FromStream(new MemoryStream(buffer));
                var path = System.AppDomain.CurrentDomain.BaseDirectory + "Image\\profile\\";
                if (extention.ToLower() == ".jpeg" || extention.ToLower() == ".jpg")
                {
                    img.Save(path +userName +".jpeg" , ImageFormat.Jpeg);
                }
                else if (extention.ToLower()==".png")
                {
                    img.Save(path + userName + extention, ImageFormat.Png);
                }
                else if (extention.ToLower()== ".gif")
                {
                    img.Save(path + userName + extention, ImageFormat.Gif);                    
                }
                returnPath =userName + extention;
            }
            return Ok(returnPath);
        }

        private string getExtention(string filename)
        {
            var split=filename.Split('.');
            return "."+split[split.Length - 1];
        }

        //Directory.CreateDirectory(Server.MapPath("~/Images"));
    }
}

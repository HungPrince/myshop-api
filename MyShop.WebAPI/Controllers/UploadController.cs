using MyShop.Service;
using MyShop.WebAPI.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MyShop.WebAPI.Controllers
{
    [RoutePrefix("api/upload")]
    public class UploadController : ApiControllerBase
    {
        public UploadController(IErrorService errorService) : base(errorService)
        {
        }

        [HttpPost]
        [Route("saveImage")]
        public HttpResponseMessage SaveImage(string type = "")
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    var postFile = httpRequest.Files[file];
                    if (postFile != null && postFile.ContentLength > 0)
                    {
                        int maxContentLength = int.Parse(ConfigurationManager.AppSettings["MaxSizeUpload"]);
                        IList<string> lstAllowedFileExtentions = new List<string> { ".jpg", ".gif", ".png", ".jpeg" };
                        string extention = postFile.FileName.Substring(postFile.FileName.LastIndexOf('.')).ToLower();
                        if (!lstAllowedFileExtentions.Contains(extention))
                        {
                            dict.Add("error", "Please Upload image of type .jpg,.gif,.png.");
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postFile.ContentLength > maxContentLength)
                        {
                            dict.Add("error", "Please upload a file have size max is 1 MB.");
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            string directory = string.Empty;
                            switch (type)
                            {
                                case "avatar":
                                    {
                                        directory = "/UploadedFiles/Avatars/";
                                        break;
                                    }
                                case "product":
                                    {
                                        directory = "/UploadedFiles/product/";
                                        break;
                                    }
                                case "news":
                                    {
                                        directory = "/UploadedFiles/news/";
                                        break;
                                    }
                                case "banner":
                                    {
                                        directory = "/UploadedFiles/banner/";
                                        break;
                                    }
                                default:
                                    {
                                        directory = "/UploadedFiles/";
                                        break;
                                    }
                            }
                            string directoryVitual = HttpContext.Current.Server.MapPath(directory);
                            if (!Directory.Exists(directoryVitual))
                            {
                                Directory.CreateDirectory(directoryVitual);
                            }
                            string fileName = Helpers.Helper.RandomString() + postFile.FileName;
                            string path = Path.Combine(directoryVitual, fileName);
                            postFile.SaveAs(path);
                            return Request.CreateResponse(HttpStatusCode.OK, Path.Combine(directory, fileName));
                        }
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Image Updated Successfully.");
                }
                dict.Add("error", "Please Upload a image.");
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}

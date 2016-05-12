using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using CP.Common;
using CP.Model.Files;
using CP.Service;

namespace CP.AdminWeb.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPic()
        {
            Stream InputStream = null;
            string FileOuthName = "";
            bool issuc = true;
            string mess = "";
            object uploadData = null;
            try
            {
                //得到数据
                HttpPostedFileBase uploadImage = Request.Files["FileData"];
                FileOuthName = uploadImage.FileName;
                if (uploadImage == null)
                    RedirectToAction("Index");

                InputStream = uploadImage.InputStream;

                //得到图片大小   图片上传最大只能6m
                if (InputStream.Length > 6291456)
                {
                    issuc = false;
                    mess = "上传的图片超过了6M尺寸";
                }

                UploadService upload = new UploadService();

                FilesInfo model = upload.Operate(uploadImage, (int) InputStream.Length);
                model.FileOuthName = FileOuthName;

                if (model != null)
                {
                    mess = "上传成功";
                    uploadData = model;
                }
                else
                {
                    issuc = false;
                    mess = "上传图片失败";
                }
 

            }
            catch (Exception e)
            {

            }
            finally
            {
                InputStream.Close();
            }
            var result = new
            {
                IsSuccess = issuc,
                Message = mess,
                data = uploadData
            };
            return Json(result);
        }
    }
}

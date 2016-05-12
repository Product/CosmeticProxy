using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP.Common;
using CP.DAL;
using CP.Model;
using CP.Model.Enum;
using CP.Model.Files;
using CP.Service;

namespace CP.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductBLL pBll = new ProductBLL();
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取商品列表（分页）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetProductList(int pageSize, int area = 0, string search = "", int pageIndex = 1)
        {
            int totalCount = 0;
            List<ProductInfo> eriList = pBll.GetProProductList(area, search, pageIndex, pageSize, out totalCount);
            var result = new
            {
                data = eriList,
                pagesize = pageSize,
                totalcount = totalCount
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #region  获取图片
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string GetFileType(FileType filetype)
        {
            string format = "";
            switch (filetype)
            {
                case FileType.jpg:
                case FileType.jpeg:
                    format = "image/jpeg";
                    break;
                case FileType.gif:
                    format = "image/gif";
                    break;
                case FileType.png:
                    format = "image/png";
                    break;
                case FileType.bmp:
                    format = "image/bmp";
                    break;
                case FileType.zip:
                    format = "application/zip";
                    break;
                case FileType.swf:
                    format = "application/x-shockwave-flash";
                    break;
            }
            return format;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="fileId">图片名存储数据</param>
        /// <param name="size">图片的尺寸</param>
        public void GetPic(string fileId, int size)
        {
            string filePath = "";
            string fileType = "";
            string filePathNew = "";
            string filePathNewS = "";
            FilesInfo file = new FilesInfo();
            if (fileId != "notexists")
            {
                file = FileService.Create().GetModel(fileId);
                if (file != null)
                {
                    filePath = FilesConfig.FileRootDir + file.FilePath;
                    string filePathS = filePath;
                    if (filePath.StartsWith("C:"))
                    {
                        filePathNewS = filePath.Replace("C:", "D:");
                    }
                    else
                    {
                        filePathNewS = filePath;
                    }
                    filePath = filePath.Replace("Source", "Thumbnail");
                    int index = filePath.LastIndexOf('.');
                    filePath = filePath.Insert(index, "_" + size);
                    filePathNew = filePath.Replace("C:", "D:");

                    //文件存在
                    if (System.IO.File.Exists(filePath))
                    {

                    }
                    else if (System.IO.File.Exists(filePathNew))
                    {
                        filePath = filePathNew;
                    }
                    else if (System.IO.File.Exists(filePathS))
                    {
                        filePath = filePathS;
                    }
                    else if (System.IO.File.Exists(filePathNewS))
                    {
                        filePath = filePathNewS;
                    }
                    else
                    {
                        fileId = "notexists";
                    }
                }
                else
                {
                    fileId = "notexists";
                }
            }
            fileType = GetFileType(file.FFormat);

            if (Request.Headers["If-Modified-Since"] != null && TimeSpan.FromTicks(DateTime.Now.Ticks - DateTime.Parse(Request.Headers["If-Modified-Since"]).Ticks).Hours <= 72)
            {
                Response.StatusCode = 304;
            }
            else
            {
                FileStream myFile = null;
                try
                {
                    myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryReader reader = new BinaryReader(myFile);
                    byte[] bt = reader.ReadBytes((int)myFile.Length);

                    DateTime lastMod = DateTime.Now;
                    if (file != null)
                    {
                        if (file.CreateTime < lastMod)
                        {
                            lastMod = file.CreateTime;
                        }
                    }
                    GetFile(bt, fileType, lastMod);
                }
                catch (Exception e)
                {

                }
                finally
                {
                    if (myFile != null)
                    {
                        myFile.Close();
                        myFile.Dispose();
                    }
                }
            }
        }
        #endregion

        #region 输出文件
        /// <summary>
        /// 输出文件
        /// </summary>
        /// <param name="resData"></param>
        /// <param name="fileType"></param>
        /// <param name="lastMod"></param>
        private void GetFile(byte[] resData, String fileType, DateTime lastMod)
        {
            try
            {
                int cacheTime = 3;
                DateTime now = DateTime.Now;

                Response.Cache.SetLastModified(lastMod);
                Response.Cache.SetMaxAge(TimeSpan.FromDays(cacheTime));
                Response.Cache.SetExpires(DateTime.Now.AddDays(cacheTime));
                Response.Cache.SetAllowResponseInBrowserHistory(true);
                //HttpCookie cookie = new HttpCookie("duping");
                //cookie.Value = "junkai";
                //Response.AppendCookie(cookie);
                Response.AppendHeader("Content-Length", resData.Length.ToString());
                Response.AppendHeader("Connection", "Keep-Alive");
                Response.AppendHeader("Content-Type", fileType);
                Response.BinaryWrite(resData);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CP.BLL;
using CP.Model.Enum;
using CP.Model.Files;
using CP.Common;

namespace CP.Service
{
    public class UploadService
    {
        #region 变量
        /// <summary>
        /// 图片格式
        /// </summary>
        public FileType FileType
        {
            get;
            set;
        }

        /// <summary>
        /// 物理保存路径
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }


        #endregion

        #region 从客户端上传图片

        #region 单个文件图片
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <param name="fileSize"></param>
        /// <param name="sourceFile"></param>
        /// <param name="copyFile"></param>
        /// <returns></returns>
        public FilesInfo Operate(HttpPostedFileBase uploadImage, int fileSize)
        {
            FilesInfo model = new FilesInfo();
            bool flag = CheckForm(uploadImage.FileName);

            if (flag)
            {
                string fileName;
                flag = SaveFile(uploadImage, out fileName);
                if (flag)
                {
                    model = SaveFileToDB(fileSize, fileName);
                }
                else
                {
                    return null;
                }
            }

            return model;
        }

        #endregion



        #region 验证传入数据是否合法

        /// <summary>
        /// 验证传入数据是否合法
        /// </summary>
        private bool CheckForm(string fileName)
        {
            //得到   图片类型
            int lastIndex = fileName.LastIndexOf('.');
            string format = fileName.Substring(lastIndex + 1, fileName.Length - lastIndex - 1).ToLower();
            switch (format)
            {
                case "jpg":
                    FileType = FileType.jpg;
                    break;
                case "jpeg":
                    FileType = FileType.jpeg;
                    break;
                case "gif":
                    FileType = FileType.gif;
                    break;
                case "png":
                    FileType = FileType.png;
                    break;
                case "zip":
                    FileType = FileType.zip;
                    break;
                case "swf":
                    FileType = FileType.swf;
                    break;
                case "bmp":
                    FileType = FileType.bmp;
                    break;
                default:
                    return false;
            }
            return true;
        }

        #endregion

        #region 保存到物理磁盘

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <param name="ResType"></param>
        /// <param name="fileName"></param>
        /// <param name="sourceFile"></param>
        /// <param name="copyFile"></param>
        /// <returns></returns>
        private bool SaveFile(HttpPostedFileBase uploadImage,out string fileName)
        {
            //得到物理存储文件夹
            string fileDir = FilesConfig.ImageDir;

            string path = "";
            //把原图和缩略图分目录

            string sourceDir = ConfigurationManager.AppSettings["SourceDir"];
            string thumDir = ConfigurationManager.AppSettings["ThumDir"];

            //原图存储
            path = string.Format(@"{0}\{1}", sourceDir, fileDir);

            //保存到物理磁盘
            FilePath = FilesHelper.GetSavePath(FileType.ToString(), path, out fileName);

            try
            {

                uploadImage.SaveAs(FilePath);

                //生成缩略图
                return DoThum(fileDir, sourceDir, thumDir);
            }
            catch (Exception e)
            {
                return false;
            }
        }



        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="ResType"></param>
        /// <param name="sourceFile"></param>
        /// <param name="copyFile"></param>
        /// <returns></returns>
        private bool DoThum(string fileDir, string sourceFile, string copyFile)
        {
            bool flag = false;

            //生成缩略图
            int[] thumSize = { 270};
            for (int i = 0; i < thumSize.Length; i++)
            {
                flag = DoThumbanil(fileDir, thumSize[i], sourceFile, copyFile);
            }

            return flag;
        }

        /// <summary>
        /// 生成缩略图地址
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="size"></param>
        /// <param name="resType"></param>
        /// <param name="sourceFile"></param>
        /// <param name="copyFile"></param>
        /// <returns></returns>
        private bool DoThumbanil(string fileDir, int size, string sourceFile, string copyFile)
        {
            int height;
            int width;
            string thumPath;
            //原图存储
            string filePath = FilePath;

            //缩略图存储
            thumPath = filePath.Replace(sourceFile, copyFile);
            int index = thumPath.LastIndexOf('\\');
            string dir = thumPath.Substring(0, index);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            thumPath = thumPath.Insert(thumPath.LastIndexOf('.'), string.Format("_{0}", size));

            string fileFormat = "";
            if (FileType == FileType.gif)
            {
                fileFormat = "gif";
            }

            ImageHelper.GetThumbnail(size, filePath, thumPath, fileFormat, out height, out width);
            return height > 0 ? true : false;
        }

        #endregion

        #region 保存到文件数据库

        /// <summary>
        /// 保存到文件数据库
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="resType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private FilesInfo SaveFileToDB(int fileSize, string fileName)
        {
            //原图
            string site = FilesConfig.FileRootDir + FilesConfig.SourceDir;

            FilesInfo model = GetFileModel(fileSize, fileName);

            //保存到文件数据库
            FilesBLL bll = new FilesBLL();
            int row = bll.Insert(model);
            if (row > 0)
            {
                return model;
            }
            return null;
        }
        /// <summary>
        /// 得到文件模型
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private FilesInfo GetFileModel(int fileSize, string fileName)
        {
            int width;
            int height;
            FilesInfo model = new FilesInfo();
            model.FFormat = FileType;
            model.FilePath = FilePath.Replace(FilesConfig.FileRootDir, "");
            model.FileSize = fileSize;
            model.CreateTime = DateTime.Now;
            ImageHelper.GetPicSize(model.FilePath, out width, out height);
            model.Width = width;
            model.Height = height;
            model.FileName = fileName;
            return model;
        }

        #endregion
        #endregion
    }
}

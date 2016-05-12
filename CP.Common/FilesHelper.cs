using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CP.Common
{
       public sealed class FilesHelper
       {
        public static String NewStoreName()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static String StoreDir
        {
            get
            {
                return GetStoreDir(null);
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String GetStoreDir(string fileDir)
        {
            string fullPath = string.Empty;
            DateTime n = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(fileDir))
                {
                    sb.Append(fileDir);
                }
                if (!fileDir.EndsWith("\\"))
                {
                    sb.Append("\\");
                }

                sb.Append(n.ToString("yyyy"))  //year
                  .Append('\\')
                  .Append(n.ToString("MM"))  //month
                  .Append('\\')
                  .Append(n.ToString("dd"))  //day
                  .Append('\\')
                  .Append(n.ToString("HH"))  //hour
                  .Append('\\');

                fullPath = FilesConfig.FileRootDir + sb.ToString();

                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
            }
            catch (Exception ex)
            {

            }

            return fullPath;
        }

        /// <summary>
        /// 得到保存文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetSavePath(string format, string fileDir, out string fileName)
        {
            fileName = NewStoreName();
            return string.Format("{0}{1}.{2}", GetStoreDir(fileDir), fileName, format);
        }

        /// <summary>
        /// 拷贝文件到目标文件路径，目标没有路径自动建立
        /// </summary>
        /// <param name="srcFilePath">源文件路径</param>
        /// <param name="desFilePath">目标文件路径</param>
        /// <param name="overwrite">如果目标路径文件存在，true 替换目标文件，false 不替换目标文件</param>
        /// <returns>1 成功 -1 失败</returns>
        public static int CopyFile(string srcFilePath, string desFilePath, bool overwrite)
        {
            try
            {
                //获取目标文件路径
                int si = desFilePath.LastIndexOf("\\");
                string desfilename = desFilePath.Substring(si, desFilePath.Length - si);
                string desPath = desFilePath.Substring(0, si);
                desfilename = desfilename.Trim('\\');
                if (!Directory.Exists(desPath))
                    Directory.CreateDirectory(desPath);
                if (File.Exists(srcFilePath))
                    File.Copy(srcFilePath, desFilePath, overwrite);
                return 1;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        public static String SaveFile(String oriPath, Boolean bCopy)
        {
            String desPath = StoreDir + NewStoreName();

            try
            {
                if (bCopy)
                    File.Copy(oriPath, desPath);
                else
                    File.Move(oriPath, desPath);
            }
            catch
            {
                desPath = String.Empty;
            }
            return desPath;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="serverPath">服务器路径</param>
        /// <param name="oriPath">源文件路径</param>
        /// <param name="bCopy">复制/移动</param>
        /// <returns></returns>
        public static String SaveFile(String oriPath, string format, Boolean bCopy)
        {
            String desPath = GetStoreDir(null) + NewStoreName() + "." + format;

            try
            {
                if (bCopy)
                    File.Copy(oriPath, desPath);
                else
                    File.Move(oriPath, desPath);
            }
            catch(Exception ex)
            {
                desPath = String.Empty;
            }
            return desPath;
        }


        public static String SaveFile(String oriPath)
        {
            return SaveFile(oriPath, false);
        }

        public static Boolean DelFile(String oriPath)
        {
            try
            {
                File.Delete(oriPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static Int64 GetFileSize(String filePath)
        {
            Int64 fs = 0L;

            try
            {
                FileInfo fi = new FileInfo(filePath);
                fs = fi.Length;
            }
            catch (Exception ex)
            {
            }

            return fs;
        }

        public static Size GetPicSize(String picPath)
        {
            Size _size = new Size(0, 0);
            Image myOriPic = null;

            try
            {
                myOriPic = Image.FromFile(picPath);
                _size = myOriPic.Size;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if(myOriPic != null)
                    myOriPic.Dispose();
            }
            return _size;
        }

        public static String GetFileMime(String fileCachePath)
        {
            String mimeType = String.Empty;
            try
            {
                if (FilesConfig.MimeMap.Count == 0)
                {
                    lock (typeof(FilesConfig))
                    {
                        if (FilesConfig.MimeMap.Count == 0)// double-check
                        {
                            FilesConfig.SetupMimeMap();
                        }
                    }
                }

                if (string.IsNullOrEmpty(fileCachePath))
                {
                    throw new Exception("参数fileCachePath不能为空");
                }
                //Log4Net.Info("参数fileCachePath:" + fileCachePath);
                String extName = Path.GetExtension(fileCachePath);
                mimeType = FilesConfig.MimeMap[extName.ToLower().Substring(1)].ToString();
            }

            catch (Exception ex)
            {
            }
            return mimeType;
        }
    }
}

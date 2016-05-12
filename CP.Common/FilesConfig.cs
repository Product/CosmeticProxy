using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

namespace CP.Common
{
    /// <summary>
    /// 文件目录配置
    /// </summary>
    public class FilesConfig
    {
        private static readonly string MimMapUrl = ConfigurationManager.AppSettings["MimMapUrl"];
        public static readonly string FileRootDir = ConfigurationManager.AppSettings["FileRootDir"];
        public static readonly string FileRootUrl = ConfigurationManager.AppSettings["FileRootUrl"];
        public static readonly string ImageDir = ConfigurationManager.AppSettings["ImageDir"];
        //图片文件夹  源文件   缩略图 
        public static readonly string SourceDir = ConfigurationManager.AppSettings["SourceDir"];
        public static readonly string ThumDir = ConfigurationManager.AppSettings["ThumDir"];

        //虚拟文件路径
        public static readonly string UploadDir = ConfigurationManager.AppSettings["UploadDir"];

        public static Hashtable MimeMap = new Hashtable();

        public static Boolean SetupMimeMap()
        {
            Boolean bRet = false;

            try
            {
                if (!File.Exists(MimMapUrl))
                {
                    throw new Exception("文件类型文件不存在:" + MimMapUrl);
                }
                using (XmlTextReader reader = new XmlTextReader(MimMapUrl))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name != "mimemap"))
                        {
                            MimeMap.Add(reader.GetAttribute(0), reader.GetAttribute(1));
                        }
                    }
                }
                bRet = true;
            }
            catch (Exception ex)
            {

            }

            return bRet;
        }
    }
}

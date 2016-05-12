using System;

using CP.Model.Enum;

namespace CP.Model.Files
{
    [Serializable]
    public class FilesInfo
    {
        public FilesInfo()
        {
            CreateTime = DateTime.Now;
        }
        /// <summary>
        /// 文件Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public FileType FFormat { get; set; }
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 物理文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 图片高
        /// </summary>
        public int Height { get; set; }
        public string FileOuthName { get; set; }
    }
}

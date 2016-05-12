
using CP.BLL;
using CP.Model.Files;

namespace CP.Service
{
    public class FileService
    {
        static FileService _instance;
        static object lock_obj = new object();
        private readonly FilesBLL filesBll = new FilesBLL();

        public static FileService Create()
        {
            lock (lock_obj)
            {
                if (_instance == null)
                {
                    _instance = new FileService();
                }
            }
            return _instance;
        }


        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Insert(FilesInfo filesInfo)
        {
            return filesBll.Insert(filesInfo);
        }


        /// <summary>
        /// 得到某个文件的保存信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FilesInfo GetModel(string fileName)
        {
            return filesBll.GetModel(fileName);
        }
    }
}

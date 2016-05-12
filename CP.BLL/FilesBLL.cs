using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.DAL;
using CP.Model.Enum;
using CP.Model.Files;

namespace CP.BLL
{
    public class FilesBLL
    {
        private readonly FileDAL fileDAL = new FileDAL();

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Insert(FilesInfo filesInfo)
        {
            return fileDAL.Insert(filesInfo);
        }


        /// <summary>
        /// 得到某个文件的保存信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FilesInfo GetModel(string fileName)
        {
            return fileDAL.GetModel(fileName);
        }
    }
}

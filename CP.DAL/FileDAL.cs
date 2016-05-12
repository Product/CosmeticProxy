using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Model.Enum;
using CP.Model.Files;
using Gyyx.Core.DBUtility;

namespace CP.DAL
{
    public class FileDAL
    {
        #region SQL语句

        private const string AddFileSql = @"INSERT INTO [dbo].[TFiles]
           (FileName,
			FilePath,
			FFormat,
			FileSize,
			CreateTime)
            VALUES
           (@FileName
           ,@FilePath
           ,@FFormat
           ,@FileSize
           ,@CreateTime
           );SELECT @@IDENTITY";

        private const string GetFileSql = @"select * from TFiles where FileName = @FileName";


        #endregion


        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Insert(FilesInfo filesInfo)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@FileName",SqlDbType.VarChar),
                                     new SqlParameter("@FilePath",SqlDbType.VarChar),
                                     new SqlParameter("@FFormat",SqlDbType.TinyInt),
                                     new SqlParameter("@FileSize",SqlDbType.Int),
                                     new SqlParameter("@CreateTime",SqlDbType.DateTime)
                                     };
            parameters[0].Value = filesInfo.FileName;
            parameters[1].Value = filesInfo.FilePath;
            parameters[2].Value = filesInfo.FFormat;
            parameters[3].Value = filesInfo.FileSize;
            parameters[4].Value = filesInfo.CreateTime;
            object obj = dbo.ExecuteScalar(CommandType.Text, AddFileSql, parameters);
            if (obj != null)
                return Convert.ToInt32(obj.ToString());
            return 0;
        }


        /// <summary>
        /// 得到某个文件的保存信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FilesInfo GetModel(string fileName)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@FileName",SqlDbType.VarChar)
                                     };
            parameters[0].Value = fileName;
            DataRow row = dbo.GetDataRow(CommandType.Text, GetFileSql, parameters);
            FilesInfo file = null;

            if (row != null && row["FileName"] != DBNull.Value)
            {
                file = new FilesInfo
                {
                    FileName = row["FileName"].ToString(),
                    FilePath = row["FilePath"].ToString(),
                    FFormat = (FileType)Convert.ToInt32(row["FFormat"].ToString()),
                    FileSize = Convert.ToInt32(row["FileSize"].ToString()),
                    CreateTime = Convert.ToDateTime(row["CreateTime"].ToString()),
                };
            }
            return file;
        }
    }
}

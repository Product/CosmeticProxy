using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Model;
using Gyyx.Core.DBUtility;

namespace CP.DAL
{
    public class ProductDAL
    {
        #region SQL语句

        private const string AddProductSql = @"INSERT INTO [dbo].[TProduct]
           ([name]
           ,[brank]
           ,[description]
           ,[price]
           ,[area]
           ,[pic]
           ,[status]
           ,[createtime])
            VALUES
           (@name
           ,@brank
           ,@description
           ,@price
           ,@area
           ,@pic
           ,@status
           ,@createtime
           );SELECT @@IDENTITY";

        private const string UpdateProductSql = @"UPDATE [dbo].[TProduct]
           SET [name] = @name
              ,[brank] = @brank
              ,[description] = @description
              ,[price] = @price
              ,[area] = @area
              ,[pic] = @pic
              ,[status] = @status
              ,[createtime] = @createtime
           WHERE code = @code";

        private const string GetProductModelSql = @"select * from TProduct where code = @code and status != 1";

        private const string DeleteProductSql = @"update TProduct set status = 1 where code = @code";

        private const string UpdateProductStatusSql = @"update TProduct set status = @status where code = @code";

        #endregion


        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool AddProduct(ProductInfo product)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@name",SqlDbType.VarChar,200),
                                     new SqlParameter("@brank",SqlDbType.VarChar,200),
                                     new SqlParameter("@description",SqlDbType.VarChar,200),
                                     new SqlParameter("@price",SqlDbType.Decimal),
                                     new SqlParameter("@area",SqlDbType.Int),
                                     new SqlParameter("@pic",SqlDbType.VarChar,200),
                                     new SqlParameter("@status",SqlDbType.Int),
                                     new SqlParameter("@createtime",SqlDbType.DateTime)
                                     };
            parameters[0].Value = product.Name;
            parameters[1].Value = product.Brank;
            parameters[2].Value = product.Description;
            parameters[3].Value = product.Price;
            parameters[4].Value = product.Area;
            parameters[5].Value = product.Pic;
            parameters[6].Value = product.Status;
            parameters[7].Value = DateTime.Now;

            var obj = dbo.ExecuteScalar(CommandType.Text, AddProductSql, parameters);
            return obj != null && Convert.ToInt32(obj.ToString()) > 0;
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool DeleteProduct(int code)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@code",SqlDbType.Int)
                                     };
            parameters[0].Value = code;

            int result = dbo.ExecuteNonQuery(CommandType.Text, DeleteProductSql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 更新产品状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool UpdateProductStatus(int status, int code)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@code",SqlDbType.Int),
                                     new SqlParameter("@status",SqlDbType.Int)
                                     };
            parameters[0].Value = code;
            parameters[1].Value = status;
            int result = dbo.ExecuteNonQuery(CommandType.Text, UpdateProductStatusSql, parameters);
            return result > 0;
        }


        /// <summary>
        /// 编辑产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool EditProduct(ProductInfo product)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@name",SqlDbType.VarChar,200),
                                     new SqlParameter("@brank",SqlDbType.VarChar,200),
                                     new SqlParameter("@description",SqlDbType.VarChar,200),
                                     new SqlParameter("@price",SqlDbType.Decimal),
                                     new SqlParameter("@area",SqlDbType.Int),
                                     new SqlParameter("@pic",SqlDbType.VarChar,200),
                                     new SqlParameter("@status",SqlDbType.Int),
                                     new SqlParameter("@createtime",SqlDbType.DateTime),
                                     new SqlParameter("@code",SqlDbType.Int)
                                     };
            parameters[0].Value = product.Name;
            parameters[1].Value = product.Brank;
            parameters[2].Value = product.Description;
            parameters[3].Value = product.Price;
            parameters[4].Value = product.Area;
            parameters[5].Value = product.Pic;
            parameters[6].Value = product.Status;
            parameters[7].Value = DateTime.Now;
            parameters[8].Value = product.Code;

            int result = dbo.ExecuteNonQuery(CommandType.Text, UpdateProductSql, parameters);
            return result > 0;
        }

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ProductInfo GetProductInfo(int code)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@code",SqlDbType.Int)
                                     };
            parameters[0].Value = code;

            DataRow row = dbo.GetDataRow(CommandType.Text, GetProductModelSql, parameters);
            ProductInfo info = null;

            if (row != null && row["code"] != DBNull.Value)
            {
                info = BindProductInfo(row);
            }

            return info;
        }

        /// <summary>
        /// 获取商品列表（后台）
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetBackProductList(int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", SqlDbType.Int),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@TotalCount", SqlDbType.Int)
            };
            parameters[0].Value = pageIndex;
            parameters[1].Value = pageSize;
            parameters[2].Direction = ParameterDirection.Output;

            DataTable datable = dbo.GetDataTable(CommandType.StoredProcedure, "sp_get_backproduct_list", parameters);

            if (parameters[2].Value != null && parameters[2].Value != System.DBNull.Value)
            {
                totalCount = Convert.ToInt32(parameters[2].Value);
            }

            if (datable == null || datable.Rows.Count <= 0)
            {
                return new List<ProductInfo>();
            }

            List<ProductInfo> productList = new List<ProductInfo>();
            productList.AddRange(from DataRow row in datable.Rows
                                 select BindProductInfo(row));
            return productList;
            
        }


        /// <summary>
        /// 获取商品列表（前台）
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetProProductList(int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = 0;
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxy");
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", SqlDbType.Int),
                new SqlParameter("@PageSize", SqlDbType.Int),
                new SqlParameter("@TotalCount", SqlDbType.Int)
            };
            parameters[0].Value = pageIndex;
            parameters[1].Value = pageSize;
            parameters[2].Direction = ParameterDirection.Output;

            DataTable datable = dbo.GetDataTable(CommandType.StoredProcedure, "sp_get_proproduct_list", parameters);

            if (parameters[2].Value != null && parameters[2].Value != System.DBNull.Value)
            {
                totalCount = Convert.ToInt32(parameters[2].Value);
            }

            if (datable == null || datable.Rows.Count <= 0)
            {
                return new List<ProductInfo>();
            }

            List<ProductInfo> productList = new List<ProductInfo>();
            productList.AddRange(from DataRow row in datable.Rows
                                 select BindProductInfo(row));
            return productList;
        }



        /// <summary>
        /// 绑定商品信息
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private ProductInfo BindProductInfo(DataRow row)
        {
            ProductInfo info = new ProductInfo()
            {
                Code = Convert.ToInt32(row["code"]),
                Name = row["name"].ToString(),
                Brank = row["brank"].ToString(),
                Description = row["description"].ToString(),
                Price = Convert.ToDecimal(Convert.ToInt32(row["price"])),
                Area = Convert.ToInt32(row["area"]),
                Pic = row["pic"].ToString(),
                Status = Convert.ToInt32(row["status"]),
                CreateTime = Convert.ToDateTime(row["createtime"])
            };
            return info;
        }
    }
}

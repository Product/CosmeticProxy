using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.Model;

namespace CP.DAL
{
    public class ProductBLL
    {
        private readonly ProductDAL productDAL = new ProductDAL();

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool AddProduct(ProductInfo product)
        {
            return productDAL.AddProduct(product);
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool DeleteProduct(int code)
        {
            return productDAL.DeleteProduct(code);
        }

        /// <summary>
        /// 更新产品状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool UpdateProductStatus(int status, int code)
        {
            return productDAL.UpdateProductStatus(status,code);
        }


        /// <summary>
        /// 编辑产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool EditProduct(ProductInfo product)
        {
            return productDAL.EditProduct(product);
        }

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ProductInfo GetProductInfo(int code)
        {
            return productDAL.GetProductInfo(code);
        }

        /// <summary>
        /// 获取商品列表（后台）
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetBackProductList(int pageIndex, int pageSize, out int totalCount)
        {
            return productDAL.GetBackProductList(pageIndex, pageSize, out totalCount);
        }


        /// <summary>
        /// 获取商品列表（前台）
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> GetProProductList(int area, string search, int pageIndex, int pageSize, out int totalCount)
        {
            return productDAL.GetProProductList(area,search,pageIndex, pageSize, out totalCount);
        }

        /// <summary>
        /// 获取商品列表数量（前台）
        /// </summary>
        /// <returns></returns>
        public int GetProProductCount()
        {
            return productDAL.GetProProductCount();
        }
        
    }
}

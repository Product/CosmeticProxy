using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.DAL;

namespace CP.Service
{
    public class ProductService
    {
        static ProductService _instance;
        static object lock_obj = new object();
        private readonly ProductBLL productBll = new ProductBLL();

        public static ProductService Create()
        {
            lock (lock_obj)
            {
                if (_instance == null)
                {
                    _instance = new ProductService();
                }
            }
            return _instance;
        }
    }
}

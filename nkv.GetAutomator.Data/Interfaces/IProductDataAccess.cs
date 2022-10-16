using nkv.GetAutomator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.Interfaces
{
    public interface IProductDataAccess
    {
        public Task<List<Categories>> GetAllCategory();
        public Task<List<DatabaseTypes>> GetAllDataTypes();
        public Task<List<Product>> GetAllProducts();
        public Task<List<Product>> SearchProducts(ProductSearchRequest request);
    }
}

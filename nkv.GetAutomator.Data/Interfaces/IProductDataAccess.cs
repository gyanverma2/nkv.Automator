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
        Task<List<Categories>> GetAllCategory();
        Task<List<DatabaseTypes>> GetAllDataTypes();
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> SearchProducts(ProductSearchRequest request);
        Task<List<ProductPrice>> GetAllProductPrice(List<int> priceIDList);
        Task<CartUserMapping> AddCartToDB(CartUserMapping cartUserMapping);
        Task<CartUserMapping?> GetCartByPublicID(string publicID);
    }
}

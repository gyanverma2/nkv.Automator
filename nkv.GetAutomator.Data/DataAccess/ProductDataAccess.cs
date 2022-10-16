using Microsoft.EntityFrameworkCore;
using nkv.GetAutomator.Data.DBContext;
using nkv.GetAutomator.Data.Interfaces;
using nkv.GetAutomator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nkv.GetAutomator.Data.DataAccess
{
    public class ProductDataAccess : IProductDataAccess
    {
        private readonly MySQLContext _context;
        public ProductDataAccess(IDbContextFactory<MySQLContext> mySQLContext)
        {
            _context = mySQLContext.CreateDbContext();
        }

        public Task<List<Categories>> GetAllCategory()
        {
            return _context.Categories.ToListAsync();
        }

        public Task<List<DatabaseTypes>> GetAllDataTypes()
        {
            return _context.DatabaseTypes.ToListAsync();
        }

        public Task<List<Product>> GetAllProducts()
        {
            return _context.Products.ToListAsync();
        }

        public Task<List<Product>> SearchProducts(ProductSearchRequest request)
        {
            var products = _context.Products.ToListAsync();
            return products;
        }
    }
}

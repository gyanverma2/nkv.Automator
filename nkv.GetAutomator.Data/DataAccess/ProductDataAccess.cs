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
        public Task<List<ProductPrice>> GetAllProductPrice(List<int> priceIDList)
        {
            var price = _context.ProductPrice.Join(_context.PriceType, a => a.PriceTypeID, b => b.PriceTypeID, (a, b) => new ProductPrice()
            {
                PriceID = a.PriceID,
                PriceTypeID = a.PriceTypeID,
                ProductID = a.ProductID,
                Price = a.Price,
                NumberOfClick = b.NumberOfClick,
                NumberOfDays = b.NumberOfDays,
                PriceTypeName = b.PriceTypeName
            }).Where(i => priceIDList.Contains(i.PriceTypeID)).ToListAsync();
            return price;
        }
        public async Task<CartUserMapping> AddCartToDB(CartUserMapping cartUserMapping)
        {
            var user = cartUserMapping.User;
            if (cartUserMapping.UserID == 0 || user == null)
            {
                var publicID = Guid.NewGuid().ToString();
                user = new Users()
                {
                    FullName = "",
                    PublicID = publicID,
                    UserEmail = publicID,
                    UserPassword = publicID,
                    UserIsActive = false
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
            cartUserMapping.Remarks = user.PublicID;
            cartUserMapping.UserID = user.UserID;
            cartUserMapping.User = user;
            cartUserMapping.CartID = 0;
            await _context.CartUserMapping.AddAsync(cartUserMapping);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            cartUserMapping.CartDetails.ForEach(x => x.CartDetailID = 0);
            cartUserMapping.CartDetails.ForEach(x => x.CartID = cartUserMapping.CartID) ;
            await _context.CartDetails.AddRangeAsync(cartUserMapping.CartDetails);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return cartUserMapping;
        }
        public async Task<CartUserMapping?> GetCartByPublicID(string publicID)
        {
            var cart = await _context.CartUserMapping.FirstOrDefaultAsync(i => i.Remarks == publicID);
            if (cart != null)
            {
                cart.CartDetails = await _context.CartDetails.Where(i => i.CartID == cart.CartID).ToListAsync();
                cart.User = await _context.Users.FirstOrDefaultAsync(i => i.UserID == cart.UserID);
            }
            return cart;
        }

    }
}

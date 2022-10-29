using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;
using nkv.GetAutomator.Data.DataAccess;
using nkv.GetAutomator.Data.Interfaces;
using nkv.GetAutomator.Data.Models;
using nkv.GetAutomator.Shared;
using nkv.GetAutomator.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using static MudBlazor.CategoryTypes;
using ComponentBase = Microsoft.AspNetCore.Components.ComponentBase;
using Product = nkv.GetAutomator.Data.Models.Product;

namespace nkv.GetAutomator.Pages.Website.Sections
{
    public partial class Products : ComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IProductDataAccess ProductDataAccess { get; set; }
        [CascadingParameter] StateProvider StateProvider { get; set; }
        private List<DatabaseTypes> DatabaseTypes { get; set; }
        private List<Categories> Categories { get; set; }
        private List<Product> ProductList { get; set; }
        private List<Product> OriginalProductList { get; set; }
        private Dictionary<int, ProductPrice> ProductPriceDic { get; set; }
        private Product? SelectedProduct { get; set; }
        private Categories? SelectedCategory { get; set; }
        private DatabaseTypes? SelectedDatabaseType { get; set; }

        private bool IsShowVideo;
        private void OpenProductDemoDialog(Product product)
        {
            SelectedProduct = product;
            IsShowVideo = true;
        }
        private DialogOptions dialogOptions = new() { FullWidth = true, CloseButton = true, NoHeader = true, Position = DialogPosition.Center };
        protected override async Task OnInitializedAsync()
        {
            ProductPriceDic = new Dictionary<int, ProductPrice>();
            DatabaseTypes = await ProductDataAccess.GetAllDataTypes();
            Categories = await ProductDataAccess.GetAllCategory();
            ProductList = await ProductDataAccess.GetAllProducts();
            OriginalProductList = await ProductDataAccess.GetAllProducts();
            var productPrice = await ProductDataAccess.GetAllProductPrice(new List<int>() { 3 });
            if (productPrice != null && productPrice.Any())
                ProductPriceDic = productPrice.ToDictionary(x => x.ProductID, x => x);
        }
        private Price CreatePriceObject(ProductPrice productPrice)
        {
            StripeConfiguration.ApiKey = "sk_test_51HGBJ7Hz1IPPrcVFdustOWeApUSooEUuU70U7fjXXNPbwG9FUHy93MQrl1te6eH39tvqGMfr1ewRrsd6M6al8oNB00NoQxKHH5";
            var options = new PriceCreateOptions
            {
                UnitAmount = ((long?)(productPrice.Price * 100)),
                Currency = "usd",
                Product = "prod_HuWdY9LsybNIEL",
            };
            var service = new PriceService();
            var price = service.Create(options);
            return price;
        }
        private async Task CreateUserCartAsync(Product product, ProductPrice productPrice)
        {
            var carts = StateProvider.ShoppingCart;
            if (carts == null)
            {
                carts = new CartUserMapping();
            }
            if(carts!=null && !carts.CartDetails.Any())
            {
                carts = new CartUserMapping()
                {
                    CartDetails = new List<CartDetails>(),
                    CartID = Helper.GetRandomInteger(),
                    CouponID = 0,
                    StatusID = 1,
                    TotalInvoice = 0,
                    TotalPaid = 0,
                    UserID = 0
                };
            }
           
            var existing = carts?.CartDetails.FirstOrDefault(i => i.ProductID == product.ProductID);
            if (existing != null && carts!=null)
            {
                carts.CartDetails.First(i => i.ProductID == product.ProductID).Quantity = existing.Quantity + 1;
            }
            else
            {
                carts?.CartDetails.Add(new CartDetails()
                {
                    CartDetailID = Helper.GetRandomInteger(),
                    CartID = carts.CartID,
                    PriceTypeID = productPrice.PriceTypeID,
                    ProductID = product.ProductID,
                    Product = product,
                    Quantity = 1,
                    Price = productPrice
                });
            }
            StateProvider.ShoppingCart = carts;
            await StateProvider.SaveChangesAsync();
            StateProvider.DisplayCheckOut();
        }
        private async void OnBuyNowClick(Product product)
        {
            await CreateUserCartAsync(product, ProductPriceDic[product.ProductID]);
          
        }
        private List<Product> FilterProduct()
        {
            var product = OriginalProductList;
            if (SelectedCategory != null)
            {
                product = product.Where(i => i.CategoryID == SelectedCategory.CategoryID).ToList();
            }
            if (SelectedDatabaseType != null)
            {
                product = product.Where(i => i.DatabaseTypeId == SelectedDatabaseType.DatabaseTypeId).ToList();
            }
            return product;
        }
        private void SelectDatabase(DatabaseTypes? databaseTypes)
        {
            if (SelectedDatabaseType != databaseTypes)
                SelectedDatabaseType = databaseTypes;
            else
                SelectedDatabaseType = null;
            ProductList = FilterProduct();
        }
        private void SelectCategory(Categories? categories)
        {
            if (SelectedCategory != categories)
                SelectedCategory = categories;
            else
                SelectedCategory = null;
            ProductList = FilterProduct();
        }
        private void AllDatabaseSelect()
        {
            SelectedDatabaseType = null;
            ProductList = FilterProduct();
        }
        private void AllCategorySelect()
        {
            SelectedCategory = null;
            ProductList = FilterProduct();
        }
        private async Task OpenCodeCanyonAsync(Product product)
        {
            if (product != null && !string.IsNullOrEmpty(product.CodecanyonLink))
                await JSRuntime.InvokeVoidAsync("open", product.CodecanyonLink, "_blank");

        }
    }
}

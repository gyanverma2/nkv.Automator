using Microsoft.AspNetCore.Components;
using nkv.GetAutomator.Data.DataAccess;
using nkv.GetAutomator.Data.Interfaces;
using nkv.GetAutomator.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nkv.GetAutomator.Pages.Website.Sections
{
    public partial class Products : ComponentBase
    {
        [Inject]
        private IProductDataAccess ProductDataAccess { get; set; }
        private List<DatabaseTypes> DatabaseTypes { get; set; }
        private List<Categories> Categories { get; set; }
        private List<Product> ProductList { get; set; }
        private List<Product> OriginalProductList { get; set; }
        private Categories? SelectedCategory { get; set; }
        private DatabaseTypes? SelectedDatabaseType { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DatabaseTypes = await ProductDataAccess.GetAllDataTypes();
            Categories = await ProductDataAccess.GetAllCategory();
            ProductList = await ProductDataAccess.GetAllProducts();
            OriginalProductList = await ProductDataAccess.GetAllProducts();
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
    }
}

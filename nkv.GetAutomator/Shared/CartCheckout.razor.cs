using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json.Linq;
using nkv.GetAutomator.Data.Interfaces;
using nkv.GetAutomator.Data.Models;
using nkv.GetAutomator.Shared;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using Product = nkv.GetAutomator.Data.Models.Product;

namespace nkv.GetAutomator.Shared
{
    public partial class CartCheckout : ComponentBase
    {
        [Inject]
        ISnackbar Snackbar { get; set; } = null!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        IJSRuntime JSRuntime { get; set; } = null!;
        [Inject]
        private IProductDataAccess ProductDataAccess { get; set; } = null!;
        [CascadingParameter] StateProvider StateProvider { get; set; } = null!;
        private Dictionary<int, ProductPrice> ProductPriceDic { get; set; } = null!;
        [Parameter] public bool IsVisible { get; set; }
        private CartUserMapping? UserCart { get; set; }
        private bool isLoading { get; set; }
        private DialogOptions dialogOptions = new() { FullWidth = true, DisableBackdropClick = true, CloseButton = true, NoHeader = false, Position = DialogPosition.Center };
        protected override async Task OnInitializedAsync()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            UserCart = await StateProvider.GetShoppingCartAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            UserCart = await StateProvider.GetShoppingCartAsync();
        }
        private void HideCheckoutDialog()
        {
            StateProvider.HideCheckOut();
        }
        private Stripe.Product GetProductObject(Product p)
        {
            StripeConfiguration.ApiKey = "sk_test_51HGBJ7Hz1IPPrcVFdustOWeApUSooEUuU70U7fjXXNPbwG9FUHy93MQrl1te6eH39tvqGMfr1ewRrsd6M6al8oNB00NoQxKHH5";

            var service = new ProductService();
            var product = service.Get(p.ProductNumber);
            return product;
        }
        private Price CreatePriceObject(double price, Product p)
        {
            var stripeProduct = GetProductObject(p);
            StripeConfiguration.ApiKey = "sk_test_51HGBJ7Hz1IPPrcVFdustOWeApUSooEUuU70U7fjXXNPbwG9FUHy93MQrl1te6eH39tvqGMfr1ewRrsd6M6al8oNB00NoQxKHH5";
            var options = new PriceCreateOptions
            {
                UnitAmount = ((long?)(price * 100)),
                Currency = "usd",
                Nickname = p.ProductName + " " + p.ProductNumber,

            };
            if (stripeProduct != null)
            {
                options.Product = stripeProduct.Id;
            }
            else
            {
                options.ProductData = new PriceProductDataOptions()
                {
                    Id = p.ProductNumber,
                    Name = p.ProductName,
                    StatementDescriptor = "GetAutomator.com",
                    Active = true
                };
            }
            var service = new PriceService();
            var priceObj = service.Create(options);
            return priceObj;
        }
        private void SetIsLoading(bool isShow)
        {
            isLoading = isShow;
            StateHasChanged();
        }
        private async void ConfirmCheckout()
        {
            try
            {
                SetIsLoading(true);
                var cart = StateProvider.ShoppingCart;
                if (cart != null && cart.CartDetails.Any())
                {
                    cart.CartID = 0;
                    cart.CartDetails.ForEach(x => x.CartDetailID = 0);
                    var cartSaved = await ProductDataAccess.AddCartToDB(cart);
                    StateProvider.ShoppingCart = cartSaved;
                    if (cartSaved != null && cartSaved.User != null)
                    {
                        var lineItems = new List<PaymentLinkLineItemOptions>();
                        foreach (var c in cart.CartDetails)
                        {
                            var priceObj = CreatePriceObject(c.Price.Price, c.Product);
                            lineItems.Add(new PaymentLinkLineItemOptions()
                            {
                                Price = priceObj.Id,
                                Quantity = 1,
                            });
                        }
                        var options = new PaymentLinkCreateOptions
                        {
                            LineItems = lineItems,
                            AfterCompletion = new PaymentLinkAfterCompletionOptions()
                            {
                                Redirect = new PaymentLinkAfterCompletionRedirectOptions()
                                {
                                    Url = NavigationManager.BaseUri + "/payment/confirmation/" + cartSaved.User.PublicID
                                },
                                Type = "redirect"
                            },
                        };
                        StripeConfiguration.ApiKey = "sk_test_51HGBJ7Hz1IPPrcVFdustOWeApUSooEUuU70U7fjXXNPbwG9FUHy93MQrl1te6eH39tvqGMfr1ewRrsd6M6al8oNB00NoQxKHH5";
                        var service = new PaymentLinkService();
                        PaymentLink paymentLink = service.Create(options);
                        if (paymentLink != null)
                        {
                            await StateProvider.SaveChangesAsync();
                            NavigationManager.NavigateTo(paymentLink.Url);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);

            }
            finally
            {
                SetIsLoading(false);
            }
        }
        private async Task OnVisibleChange()
        {
            if (!IsVisible)
            {
                StateProvider.ShoppingCart = UserCart;
                await StateProvider.SaveChangesAsync();
            }
        }
        private async Task RemoveRowClickAsync(CartDetails item)
        {
            UserCart!.CartDetails = UserCart.CartDetails.Where(i => i.CartDetailID != item.CartDetailID).ToList();
            StateProvider.ShoppingCart = UserCart;
            await StateProvider.SaveChangesAsync();
        }


    }
}

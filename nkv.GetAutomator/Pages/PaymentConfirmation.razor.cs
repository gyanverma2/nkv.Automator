using Microsoft.AspNetCore.Components;
using MudBlazor;
using nkv.GetAutomator.Data.DataAccess;
using nkv.GetAutomator.Data.Interfaces;
using nkv.GetAutomator.Data.Models;
using nkv.GetAutomator.Shared;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nkv.GetAutomator.Pages
{
    public partial class PaymentConfirmation : ComponentBase
    {
        [Parameter]
        public string PLink { get; set; }
        [Inject]
        private IProductDataAccess ProductDataAccess { get; set; }
        [CascadingParameter] StateProvider StateProvider { get; set; }
        private DialogOptions dialogOptions = new() { FullWidth = true, DisableBackdropClick = false, CloseButton = false, NoHeader = true, Position = DialogPosition.Center };

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(PLink))
            {
                StateProvider.ShoppingCart = new CartUserMapping();
                var cart = await ProductDataAccess.GetCartByPublicID(PLink);
                var x = cart;
                await StateProvider.SaveChangesAsync();
            }
        }
        
    }
}

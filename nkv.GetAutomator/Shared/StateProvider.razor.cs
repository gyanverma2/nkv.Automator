using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using nkv.GetAutomator.Data.Models;

namespace nkv.GetAutomator.Shared
{
    public partial class StateProvider : ComponentBase
    {
        [Inject]
        ProtectedLocalStorage ProtectedLocalStore { get; set; } = null!;
        [Parameter]
        public RenderFragment ChildContent { get; set; } = null!;

        public CartUserMapping? ShoppingCart { get; set; }
        private bool IsCheckoutVisible { get; set; }
        bool hasLoaded;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var result = await ProtectedLocalStore.GetAsync<CartUserMapping>("MyShoppingCart");
                if (result.Success) ShoppingCart = result.Value;
                if (ShoppingCart != null && ShoppingCart.CartDetails.Any())
                {
                    ShoppingCart.LastAccessed = DateTime.Now;
                }
                hasLoaded = true;
                StateHasChanged();
            }
        }
        public void DisplayCheckOut()
        {
            IsCheckoutVisible = true;
            StateHasChanged();
        }
        public void HideCheckOut()
        {
            IsCheckoutVisible = false;
            StateHasChanged();
        }
        public async Task<CartUserMapping?> GetShoppingCartAsync()
        {
            var result = await ProtectedLocalStore.GetAsync<CartUserMapping>("MyShoppingCart");
            if (result.Success) ShoppingCart = result.Value;
            return ShoppingCart;
        }

        public async Task SaveChangesAsync()
        {
            if (ShoppingCart != null)
            {
                ShoppingCart.LastAccessed = DateTime.Now;
                await ProtectedLocalStore.SetAsync("MyShoppingCart", ShoppingCart);
                StateHasChanged();
            }
        }
        public async Task ClearStateAsync()
        {
            await ProtectedLocalStore.DeleteAsync("MyShoppingCart");
            StateHasChanged();
        }
    }
}

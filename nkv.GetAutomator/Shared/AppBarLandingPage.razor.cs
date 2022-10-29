using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nkv.GetAutomator.Shared
{
    public partial class AppBarLandingPage : ComponentBase
    {
        [CascadingParameter] StateProvider StateProvider { get; set; }
        private void ShowCheckoutDialog()
        {
            StateProvider.DisplayCheckOut();
        }
    }
}

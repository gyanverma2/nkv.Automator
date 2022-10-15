using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nkv.GetAutomator.Pages.Website.Sections
{
    public partial class Home : ComponentBase
    {
        private bool IsShowVideo;
        private void OpenDialog() => IsShowVideo = true;
        private DialogOptions dialogOptions = new() { FullWidth = true, CloseButton = true, NoHeader=true, Position=DialogPosition.Center };
    }
}

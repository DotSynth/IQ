using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IQ.Views.BranchViews.Pages.TransferInwards.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTransferInwardOverlay : Page
    {

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddTransferInwardOverlay()
        {
            this.InitializeComponent();
        }

        // This method sets the visibility and raises the event
        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);

            Debug.WriteLine($"Visibility changed to {visibility}");
        }
    }
}

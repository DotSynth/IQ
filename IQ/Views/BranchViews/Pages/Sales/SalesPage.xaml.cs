using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.Sales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SalesPage : Page
    {
        public SalesPage()
        {
            this.InitializeComponent();
            BranchSalesDatePicker.SelectedDate = DateTime.Today;
            this.BranchSalesDatePicker.LayoutUpdated += DatePickerControl_LayoutUpdated;
        }

        private void DatePickerControl_LayoutUpdated(object? sender, object e)
        {
            Windows.UI.Color Color = CommunityToolkit.WinUI.Helpers.ColorHelper.ToColor("#020066");
            if (VisualTreeHelper
                .GetOpenPopupsForXamlRoot(this.BranchSalesDatePicker.XamlRoot)
                .FirstOrDefault() is Popup popup &&
                popup.Child.FindDescendant<Grid>(x => x.Name is "HighlightRect") is Grid highlightRect &&
                popup.Child.FindDescendants().OfType<LoopingSelector>() is IEnumerable<LoopingSelector> loopingSelectors)
            {
                highlightRect.Background = new SolidColorBrush(Color);

                foreach (LoopingSelector loopingSelector in loopingSelectors)
                {
                   
                }
            }
        }
    }
}

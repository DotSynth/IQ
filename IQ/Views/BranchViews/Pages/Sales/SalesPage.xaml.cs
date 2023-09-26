using CommunityToolkit.WinUI.UI;
using IQ.Helpers.DataTableOperations.ViewModels;
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
        public BranchSalesViewModel ViewModel { get; } = new BranchSalesViewModel();
        public static DateTime DateFilter = DateTime.Today;

        public SalesPage()
        {
            this.InitializeComponent();
            BranchSalesDatePicker.SelectedDate = DateFilter;
            DataContext = ViewModel;
        }

        private void BranchSalesDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {

        }
    }
}

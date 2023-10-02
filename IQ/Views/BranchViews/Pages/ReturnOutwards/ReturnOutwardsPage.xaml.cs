using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Views.BranchViews.Pages.Sales.SubPages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.ReturnOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReturnOutwardsPage : Page
    {
        public BranchSalesViewModel ViewModel { get; } = new BranchSalesViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddSaleOverlay OverlayInstance = new AddSaleOverlay();

        public ReturnOutwardsPage()
        {
            this.InitializeComponent();
            BranchReturnOutwardsDatePicker.SelectedDate = DateTime.Today;
        }
    }
}

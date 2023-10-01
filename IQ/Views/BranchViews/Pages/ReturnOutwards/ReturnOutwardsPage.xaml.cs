using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.ReturnOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReturnOutwardsPage : Page
    {
        public ReturnOutwardsPage()
        {
            this.InitializeComponent();
            BranchReturnOutwardsDatePicker.SelectedDate = DateTime.Today;
        }
    }
}

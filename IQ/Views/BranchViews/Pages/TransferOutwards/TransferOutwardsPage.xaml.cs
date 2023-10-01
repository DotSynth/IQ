﻿using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Views.BranchViews.Pages.Sales.SubPages;
using IQ.Views.BranchViews.Pages.TransferOutwards.SubPages;
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

namespace IQ.Views.BranchViews.Pages.TransferOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransferOutwardsPage : Page
    {
        public BranchTOutsViewModel ViewModel { get; } = new BranchTOutsViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddTOutsOverlay OverlayInstance = new AddTOutsOverlay();

        public TransferOutwardsPage()
        {
            this.InitializeComponent();
            BranchTOutsDatePicker.SelectedDate = DateTime.Today;
        }
    }
}

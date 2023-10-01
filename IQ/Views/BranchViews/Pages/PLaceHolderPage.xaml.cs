using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PLaceHolderPage : Page
    {
        public PLaceHolderPage()
        {
            this.InitializeComponent();

            // Set the source of the Image control to your GIF file
            BitmapImage bitmapImage = new(new Uri("ms-appx:///Assets/Images/LoadingAnimation.gif"));
            LoadingAnimation.Source = bitmapImage;
        }
    }
}

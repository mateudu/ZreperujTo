using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ZreperujTo.UWP.Views
{
    public sealed partial class ReportFailPage : Page
    {
        public ReportFailPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var im = sender as Image;
            var uri = im.Source;//ToDo make fullscreen
        }
    }
}


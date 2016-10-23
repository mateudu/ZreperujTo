using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ZreperujTo.UWP.Models.FailModels;
using ZreperujTo.UWP.ViewModels;

namespace ZreperujTo.UWP.Views
{
    public sealed partial class FailsByCategoriesPage : Page
    {
        public FailsByCategoriesPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var r = (Border)sender;
            var x = r.DataContext as FailMetaModel;
            var dc = DataContext as FailsByCategoriesViewModel;
            dc?.GotoDetailsPage(x);
        }
    }
}


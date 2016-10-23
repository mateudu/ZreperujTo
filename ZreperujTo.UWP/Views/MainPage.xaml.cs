using ZreperujTo.UWP.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using ZreperujTo.UWP.Models.FailModels;

namespace ZreperujTo.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var r = (Border) sender;
            var x = r.DataContext as FailMetaModel;
            var dc = DataContext as MainPageViewModel;
            dc?.GotoDetailsPage(x);
        }
    }
}

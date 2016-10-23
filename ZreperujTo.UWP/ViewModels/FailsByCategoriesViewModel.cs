using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.CategoryModels;
using ZreperujTo.UWP.Models.FailModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class FailsByCategoriesViewModel : ViewModelBase
    {
        public FailsByCategoriesViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                }
            }
        }

        private string _value = "Default";

        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        private ZreperujToHelper _zreperujToHelper;

        List<FailMetaModel> _failMetaModels;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
            if (parameter is CategoryReadModel) await LoadFailsByCategoriesAsync((CategoryReadModel)parameter);
            else if (parameter is SubcategoryReadModel) await LoadFailsByCategoriesAsync((SubcategoryReadModel)parameter);
            else await LoadFailsByCategoriesAsync();
        }

        private async Task LoadFailsByCategoriesAsync(CategoryReadModel categoryReadModel)
        {
            _zreperujToHelper = new ZreperujToHelper();

            FailMetaModels = await _zreperujToHelper.BrowseFailsAsync(categoryReadModel);
        }

        private async Task LoadFailsByCategoriesAsync(SubcategoryReadModel subcategoryReadModel)
        {
            _zreperujToHelper = new ZreperujToHelper();

            FailMetaModels = await _zreperujToHelper.BrowseFailsAsync(subcategoryReadModel);
        }

        private async Task LoadFailsByCategoriesAsync()
        {
            _zreperujToHelper = new ZreperujToHelper();

            FailMetaModels = await _zreperujToHelper.BrowseFailsAsync();

        }

        public void GoToCategoriesSelection() => NavigationService.Navigate(typeof(Views.CategoriesPage));
        public void GoToReportFail() => NavigationService.Navigate(typeof(Views.ReportFailPage));

        public List<FailMetaModel> FailMetaModels
        {
            get
            {
                return _failMetaModels;
            }
            set
            {
                _failMetaModels = value;
                RaisePropertyChanged();
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
        public void GotoDetailsPage(FailMetaModel failMetaModel) =>
    NavigationService.Navigate(typeof(Views.DetailPage), failMetaModel);
    }
}


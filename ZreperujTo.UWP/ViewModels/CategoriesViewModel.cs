using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.CategoryModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class CategoriesViewModel : ViewModelBase
    {
        public CategoriesViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                }
            }
        }


        private ZreperujToHelper _zreperujToHelper;

        private List<CategoryReadModel> _categoryReadModels;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            _zreperujToHelper = new ZreperujToHelper();
            
            var x = await _zreperujToHelper.GetCategoriesAsync();
            var allsubs = x.SelectMany(model => model.Subcategories).ToList();
            x.Insert(0, new CategoryReadModel {Name = "Wszystkie", Subcategories = allsubs , Id = "1"});
            CategoryReadModels = x;

        }

        public void GoToSubcategoriesPage()
        {
            NavigationService.Navigate(typeof(Views.SubcategoriesPage), SelectedCategory);
        }


        public List<CategoryReadModel> CategoryReadModels
        {
            get
            {
                return _categoryReadModels;
            }
            set
            {
                _categoryReadModels = value;
                RaisePropertyChanged();
            }
        }

        public CategoryReadModel SelectedCategory { get; }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }
    }
}


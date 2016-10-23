using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
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
            var y = x.ToList();
            var allsubs = new List<SubcategoryReadModel>();
            foreach (var vr in y)
            {
                if (vr.Subcategories.Count > 1) allsubs.AddRange(vr.Subcategories);
                if (vr.Subcategories.Count == 1) allsubs.Add(vr.Subcategories.First());
            }
           y.Insert(0, new CategoryReadModel {Name = "Wszystkie", Subcategories = allsubs , Id = "1"});
            CategoryReadModels = y;

        }

        public async void GoToSubcategoriesPage()
        {
            while (SelectedCategory == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
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

        public CategoryReadModel SelectedCategory { get; set; }

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


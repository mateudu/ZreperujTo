using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Models.CategoryModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class SubcategoriesViewModel : ViewModelBase
    {
        public SubcategoriesViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                }
            }
        }

        private List<SubcategoryReadModel> _subcategoryReadModels;
        private SubcategoryReadModel _selectedSubcategory;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
            LoadSubcategories((CategoryReadModel) parameter);
        }

        private void LoadSubcategories(CategoryReadModel categoryReadModel)
        {
            categoryReadModel.Subcategories.Insert(0, new SubcategoryReadModel {Name = "Wszystkie", Id = "1",});
            SubcategoryReadModels = categoryReadModel.Subcategories;
            SelectedCategory = categoryReadModel;
        }

        public List<SubcategoryReadModel> SubcategoryReadModels
        {
            get { return _subcategoryReadModels; }

            set
            {
                _subcategoryReadModels = value; 
                RaisePropertyChanged();
            }
        }

        private CategoryReadModel SelectedCategory { get; set; }

        public SubcategoryReadModel SelectedSubcategory
        {
            get { return _selectedSubcategory; }
            set
            {
                _selectedSubcategory = value;
                RaisePropertyChanged();
            }
        }

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

        public async void GoToFailsPage()
        {
            while (SelectedSubcategory == null) await Task.Delay(TimeSpan.FromMilliseconds(50));
            if (SelectedCategory.Id== "1" && SelectedSubcategory.Id == "1") NavigationService.Navigate(typeof(Views.FailsByCategoriesPage));
            else if(SelectedCategory.Id != "1" && SelectedSubcategory.Id == "1") NavigationService.Navigate(typeof(Views.FailsByCategoriesPage), SelectedCategory);
            else NavigationService.Navigate(typeof(Views.FailsByCategoriesPage), SelectedSubcategory);
        }
    }
}


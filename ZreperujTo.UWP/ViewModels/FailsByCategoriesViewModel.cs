using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
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

        private List<CategoryReadModel> _categoryReadModels;
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
            await LoadFailsByCategoriesAsync();
        }

        private async Task LoadFailsByCategoriesAsync()
        {
            _zreperujToHelper = new ZreperujToHelper();

            FailMetaModels = await _zreperujToHelper.BrowseFailsAsync();
            await _zreperujToHelper.GetCategoriesAsync();

        }


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

        public List<CategoryReadModel> CategoryReadModels
        {
            get
            {;
                return _categoryReadModels;
            }
            set
            {
                _categoryReadModels = value;
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
    }
}


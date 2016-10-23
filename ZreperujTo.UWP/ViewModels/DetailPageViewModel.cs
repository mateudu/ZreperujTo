using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.FailModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    DetailedFailModel = new FailReadModel
                    {
                        Budget = 254
                    };
                }
            }
        }

        private string _Value = "Default";

        public string Value
        {
            get { return _Value; }
            set { Set(ref _Value, value); }
        }

        private ZreperujToHelper _zreperujToHelper;
        private FailReadModel _detailedFailModel;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
            await LoadInfoAboutFailAsync( (FailMetaModel) parameter);
        }

        private async Task LoadInfoAboutFailAsync(FailMetaModel failMetaModel)
        {
            _zreperujToHelper = new ZreperujToHelper();
            DetailedFailModel = await _zreperujToHelper.GetFailDetailAsync(failMetaModel);

        }

        public FailReadModel DetailedFailModel
        {
            get { return _detailedFailModel; }
            set
            {
                _detailedFailModel = value;
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


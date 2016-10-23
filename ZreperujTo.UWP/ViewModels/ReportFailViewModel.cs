using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using Template10.Utils;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.CategoryModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class ReporFailViewModel : ViewModelBase
    {
        private StorageFile photo;
        private ZreperujToHelper helper = new ZreperujToHelper();
        public ObservableCollection<CategoryReadModel> Categories = new ObservableCollection<CategoryReadModel>();
        public ObservableCollection<SubcategoryReadModel> Subcategories = new ObservableCollection<SubcategoryReadModel>();
        private bool _sendEnabled = false;
        private object _imageSource;

        public bool SendEnabled
        {
            get
            {
                return _sendEnabled;
            }
            set { Set(ref _sendEnabled, value, "SendEnabled"); }
        }

        public ReporFailViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                }
            }
        }
        public void CategorySelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    var item = (CategoryReadModel)(sender as ComboBox)?.SelectedItem;
                    Subcategories.AddRange(item.Subcategories.ToList(), true);
                }
                catch (Exception)
                {

                }
            }
        }

        public void SubcategorySelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    var item = (SubcategoryReadModel)(sender as ComboBox)?.SelectedItem;
                    if (item != null)
                    {
                        SendEnabled = true;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public object ImageSource
        {
            get { return photo; }
            set
            {
                photo = value as StorageFile;
                RaisePropertyChanged();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            Size aspectRatio = new Size(16, 9);
            dialog.PhotoSettings.CroppedAspectRatio = aspectRatio;
            var categories = helper.GetCategoriesAsync();
            var file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);
            
            if (file == null)
            {
                NavigationService.GoBack();
            }
            if (!Categories.Any())
            {
                Categories.AddRange(await categories, true);
            }
            await Task.CompletedTask;
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
    }
}


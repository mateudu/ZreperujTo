using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.BidModels;
using ZreperujTo.UWP.Models.CommonModels;
using ZreperujTo.UWP.Models.FailModels;
using ZreperujTo.UWP.Models.FileInfoModels;

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
                        Active = true,
                        AuctionValidThrough = DateTime.Today.AddDays(1),
                        Budget = 100,
                        Description =
                            "Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! Kran mi siê urwa³! ",
                        Title = "Urwany kran",
                        Location = new LocationInfo
                        {
                            City = "Lublin",
                            District = "Œwit"
                        },
                        Requirements = new List<SpecialRequirement> {SpecialRequirement.BronzeOrMore},
                        Highlited = false,
                        
                        Pictures = new List<PictureInfoReadModel>
                        {
                            new PictureInfoReadModel
                            {
                                OriginalFileUri =
                                    "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri =
                                    "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            },
                            new PictureInfoReadModel
                            {
                                OriginalFileUri =
                                    "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri =
                                    "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            }
                        },
                        Bids=new List<BidReadModel>
                        {
                            new BidReadModel
                            {
                                Active = true,
                                Assigned = true,
                                Budget = 230,
                                Description = "Panie marianie ja to zrobiê"
                            },
                            new BidReadModel
                            {
                                Active = true,
                                Assigned = true,
                                Budget = 230,
                                Description = "Panie marianie ja to zrobiê"
                            }
                        }
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


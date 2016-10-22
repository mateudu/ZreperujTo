using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using ZreperujTo.UWP.Helpers;
using ZreperujTo.UWP.Models.CommonModels;
using ZreperujTo.UWP.Models.FailModels;
using ZreperujTo.UWP.Models.FileInfoModels;
using ZreperujTo.UWP.Models.UserInfoModels;

namespace ZreperujTo.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoggedProfile = new UserInfoReadModel
                {
                    Name = "Piotr",
                    Email = "arapio@gmail.com",
                    Company = true,
                    Id = "",
                    MobileNumber = "+48534959123",
                    Badges =
                        new List<Badge>
                        {
                            new Badge
                            {
                                Description = "Zgłosił 10 usterek samochodu",
                                Name = "Janusz Tuningu",
                                Type = "",
                                ImageUrl = "http://pngimg.com/upload/car_logo_PNG1641.png"
                            },
                            new Badge
                            {
                                Name = "Elektromajster",
                                Type = "",
                                Description = "Naprawił 365 usterek z kategorii elektronarzędzia",
                                ImageUrl = "http://img06.deviantart.net/1501/i/2014/298/f/a/energy_lightning_power_electric_electricity_logo_by_andrea_perry-d840ydr.png"
                            }
                        },
                    Ratings =
                        new List<Rating>
                        {
                            new Rating
                            {
                                Points = 5,
                                Description = "Solidna firma, mistrz świata w naprawianiu szliferki kątowej firmy Booble",
                                UserId = "",
                                BidId = ""
                            }
                        },
                    RatingCount = 50,
                    RatingSum = 224
                };

                LoggedProfileFailMetaModels = new List<FailMetaModel>
                {
                    new FailMetaModel
                    {
                        Active = true,
                        AuctionValidThrough = DateTime.Today.AddDays(1),
                        Budget = 100,
                        Description = "Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! Kran mi się urwał! ",
                        Title = "Urwany kran",
                        Location = new LocationInfo
                        {
                            City = "Lublin",
                            District = "Świt"
                        },
                        Requirements = new List<SpecialRequirement> {SpecialRequirement.BronzeOrMore},
                        Highlited = false,
                        Pictures = new List<PictureInfoReadModel>
                        {
                            new PictureInfoReadModel
                            {
                                OriginalFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            },
                             new PictureInfoReadModel
                            {
                                OriginalFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            }
                        }
                    },
                                        new FailMetaModel
                    {
                        Active = true,
                        AuctionValidThrough = DateTime.Today.AddDays(1),
                        Budget = 100,
                        Description = "Kran mi się urwał!",
                        Title = "Urwany kran",
                        Location = new LocationInfo
                        {
                            City = "Lublin",
                            District = "Świt"
                        },
                        Requirements = new List<SpecialRequirement> {SpecialRequirement.BronzeOrMore},
                        Highlited = false,
                        Pictures = new List<PictureInfoReadModel>
                        {
                            new PictureInfoReadModel
                            {
                                OriginalFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            },
                             new PictureInfoReadModel
                            {
                                OriginalFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg",
                                ThumbnailFileUri = "http://static.urzadzone.pl/gallery/9710/0_47890dbdf0eb5ed48cbcbb236f0a507af3c6b53f.jpg"
                            }
                        }
                    }
                };
            }
        }

        private readonly string _resourceName = Windows.ApplicationModel.Package.Current.DisplayName;
        private ZreperujToHelper _zreperujToHelper;
        private UserInfoReadModel _loggedProfile;
        private List<FailMetaModel> _loggedProfileFailMetaModels;

        public UserInfoReadModel LoggedProfile
        {
            get { return _loggedProfile; }
            set
            {
                _loggedProfile = value;
                RaisePropertyChanged();
            }
        }

        public IsCompany IsCompany => LoggedProfile.Company ? new IsCompany {SymbolIconText = "Shop", TextBlockText = "Firmowe"} : new IsCompany { SymbolIconText = "Contact", TextBlockText = "Prywatny" };

        public List<FailMetaModel> LoggedProfileFailMetaModels
        {
            get { return _loggedProfileFailMetaModels; }
            set
            {
                _loggedProfileFailMetaModels = value;
                RaisePropertyChanged();
            }
        }
        public string RatingAverage
        {
            get
            {
                if (LoggedProfile?.RatingSum != null)
                    return
                        ((double) LoggedProfile.RatingSum/LoggedProfile.RatingCount).ToString(
                            CultureInfo.InvariantCulture) + "/5";
                return "5/5";
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                //Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
            LogInAsync();
        }

        private async void LogInAsync()
        {
            if (ZreperujToHelper.IsLogged == false) await RetrieveTokenAsync();
            await LoadProfile();
        }



        private async Task LoadProfile()
        {
            _zreperujToHelper = new ZreperujToHelper();
            LoggedProfile = await _zreperujToHelper.GetProfileInfoAsync();
            LoggedProfileFailMetaModels = await _zreperujToHelper.GetProfileFailsAsync();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(RatingAverage));
        }

        private async Task RetrieveTokenAsync()
        {
            var token = GetCredentialFromLocker();

            if (token == null || Convert.ToDateTime(token.UserName) < DateTime.Now)
            {
                await AuthorizeAndGetTokenAsync();
            }
            token = GetCredentialFromLocker();
            token?.RetrievePassword();
            ZreperujToHelper.Token = token?.Password;
            ZreperujToHelper.IsLogged = true;
        }

        public void LogOut()
        {
            FlushLocker();
            ZreperujToHelper.Token = null;
            ZreperujToHelper.IsLogged = false;
            LogInAsync();
        }

        private void FlushLocker()
        {
            try
            {
                var vault = new Windows.Security.Credentials.PasswordVault();
                var credentialList = vault.FindAllByResource(_resourceName);

                // When there are multiple tokens
                while (credentialList.Count > 0) vault.Remove(credentialList[0]);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private Windows.Security.Credentials.PasswordCredential GetCredentialFromLocker()
        {
            try
            {
                Windows.Security.Credentials.PasswordCredential credential;

                var vault = new Windows.Security.Credentials.PasswordVault();
                var credentialList = vault.FindAllByResource(_resourceName);
                if (credentialList.Count <= 0) return null;
                if (credentialList.Count == 1)
                {
                    credential = credentialList[0];
                }
                else
                {
                    // When there are multiple tokens
                    while (credentialList.Count > 1)
                    {
                        vault.Remove(Convert.ToDateTime(credentialList[0].UserName) >
                                     Convert.ToDateTime(credentialList[1].UserName)
                            ? credentialList[1]
                            : credentialList[0]);
                    }

                    credential = credentialList[0];
                }

                return credential;
            }
            catch (Exception)
            {
                return null;
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

        public async Task AuthorizeAndGetTokenAsync()
        {

            var callback = new Uri(@"https://zreperujto.azurewebsites.net/api/Profile");
            var startUri = new Uri(@"https://zreperujto.azurewebsites.net/connect/authorize?client_id=mobile&redirect_uri=https://zreperujto.azurewebsites.net/api/Profile&response_type=code id_token token&scope=openid zreperuj_to_api offline_access&nonce=a18800e2-78cc-4757-a79c-542e4d01517f");

            try
            {
                var webAuthenticationResult =
                    await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, callback);

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var payload = webAuthenticationResult.ResponseData.Substring(webAuthenticationResult.ResponseData.IndexOf('#') + 1);

                    var parameters = (from parameter in payload.Split('&')
                                      let pair = parameter.Split('=')
                                      select new { Name = pair[0], Value = pair[1] })
                          .ToDictionary(element => element.Name, element => element.Value);

                    var vault = new Windows.Security.Credentials.PasswordVault();
                    if (parameters.ContainsKey("access_token") && parameters.ContainsKey("expires_in"))
                    {
                        var date =
                            DateTime.Now.AddSeconds(Convert.ToDouble(parameters["expires_in"]))
                                .ToString(CultureInfo.InvariantCulture);

                        vault.Add(new Windows.Security.Credentials.PasswordCredential(_resourceName, date,
                            parameters["access_token"]));
                    }

                    //Claims.Clear();
                    //foreach (var par in parameters)
                    //{
                    //    Claims.Add(new Claim
                    //    {
                    //        Type = par.Key,
                    //        Value = par.Value
                    //    });
                    //}

                    string error;
                    // If an "error" parameter has been added by the authorization server, return an exception.
                    // Note: the optional "error_description" can be used to determine why the process failed.
                    if (parameters.TryGetValue("error", out error))
                    {
                        //throw new InvalidOperationException("An error occurred during the authorization process.");
                    }

                    string token;
                    // Ensure an access token has been returned by the authorization server.
                    if (!parameters.TryGetValue("access_token", out token))
                    {
                        //throw new InvalidOperationException("The access token was missing from the OAuth2 response.");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void GotoDetailsPage(FailMetaModel failMetaModel) =>
            NavigationService.Navigate(typeof(Views.DetailPage), failMetaModel);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
    public class IsCompany
    {
        public string SymbolIconText { get; set; }
        public string TextBlockText { get; set; }
    }
}


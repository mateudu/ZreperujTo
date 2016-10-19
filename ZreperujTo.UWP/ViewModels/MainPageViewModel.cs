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

namespace ZreperujTo.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
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

        public async void Authorization()
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
                    if (parameters.ContainsKey("access_token")) vault.Add(new Windows.Security.Credentials.PasswordCredential("ZreperujTo", "access_token", parameters["access_token"]));
                    if (parameters.ContainsKey("expires_in"))
                    {
                        var date = DateTime.Now.AddSeconds(Convert.ToDouble(parameters["expires_in"])).ToString(CultureInfo.InvariantCulture);
                        vault.Add(new Windows.Security.Credentials.PasswordCredential("ZreperujTo", "expiration_date", date)); //ToDo this name can not work
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

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Value);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}


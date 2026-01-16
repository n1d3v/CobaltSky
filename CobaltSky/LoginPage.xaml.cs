using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using CobaltSky.Classes;

namespace CobaltSky
{
    public partial class LoginPage : PhoneApplicationPage
    {
        private string token = SettingsMgr.AccessJwt;
        private string did = SettingsMgr.BskyDid;
        private string pwString;

        public LoginPage()
        {
            InitializeComponent();
            visiblePW.FontSize = passwordBox.FontSize;
        }

        private void GoBackButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/WelcomePage.xaml", UriKind.Relative));
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            var api = new API();
            var login = new LoginRequest
            {
                identifier = handleBox.Text,
                password = passwordBox.Password
            };

            ShowProgressIndicator(true, "Attempting to log in...");

            await api.SendAPI("/com.atproto.server.createSession", "POST", login, response =>
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<LoginRoot>(response);

                    if (!string.IsNullOrEmpty(result?.AccessJwt))
                    {
                        SettingsMgr.AccessJwt = result.AccessJwt;
                        SettingsMgr.RefreshJwt = result.RefreshJwt;
                        SettingsMgr.BskyDid = result.Did;
                        
                        NavigationService.Navigate(new Uri("/FeedPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show(
                        "Login has failed, please double-check your handle and password before continuing.",
                        "login unsuccessful",
                        MessageBoxButton.OK);
                }
            });
            ShowProgressIndicator(false, null);
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (handleBox.Text.Length > 0 && passwordBox.Password.Length > 0)
            {
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true;
            }
        }

        private void pwShowCheck_Checked(object sender, RoutedEventArgs e)
        {
            pwString = passwordBox.Password;
            passwordBox.Password = string.Empty;
            passwordBox.IsHitTestVisible = false;
            visiblePW.Text = pwString;
        }

        private void pwShowCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox.Password = pwString;
            passwordBox.IsHitTestVisible = true;
            visiblePW.Text = string.Empty;
        }

        private void ShowProgressIndicator(bool isVisible, string text = "")
        {
            ProgressIndicator progressIndicator = new ProgressIndicator
            {
                IsIndeterminate = true,
                IsVisible = isVisible,
                Text = text
            };

            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        public class LoginRoot
        {
            [JsonProperty("did")]
            public string Did { get; set; }
            [JsonProperty("accessJwt")]
            public string AccessJwt { get; set; }
            [JsonProperty("refreshJwt")]
            public string RefreshJwt { get; set; }
        }

        public class LoginRequest
        {
            public string identifier { get; set; }
            public string password { get; set; }
        }
    }
}
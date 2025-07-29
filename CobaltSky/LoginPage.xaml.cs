using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using CobaltSky.Classes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CobaltSky
{
    public partial class LoginPage : PhoneApplicationPage
    {
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
            var api = new CobaltSky.Classes.API();
            var login = new LoginRequest
            {
                identifier = handleBox.Text,
                password = passwordBox.Password
            };

            await api.SendAPI("/com.atproto.server.createSession", "POST", login, (response) =>
            {
                Debug.WriteLine($"Response from Bluesky's servers: {response}");
                if (response.Contains("accessJwt"))
                {
                    Debug.WriteLine("Login is successful, saving the necessary details to settings!");
                    string json = response.ToString();
                    var serializer = new DataContractJsonSerializer(typeof(LoginRoot));
                    using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                    {
                        var result = (LoginRoot)serializer.ReadObject(ms);

                        // Read the results
                        string accessJwt = result.bskyJwt;
                        string refreshJwt = result.bskyRefJwt;
                        string bskyDid = result.bskyDid;

                        // Save the JWT and DID to settings
                        SettingsMgr.AccessJwt = accessJwt;
                        SettingsMgr.RefreshJwt = refreshJwt;
                        SettingsMgr.BskyDid = bskyDid;

                        // Show the values to confirm
                        Debug.WriteLine($"Saved accessJwt to settings: {accessJwt}");
                        Debug.WriteLine($"Saved refreshJwt to settings: {refreshJwt}");
                        Debug.WriteLine($"Saved bskyDid to settings: {bskyDid}");
                    }
                    NavigationService.Navigate(new Uri("/FeedPage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Login has failed, please double-check your handle and password before continuing.", "login unsuccessful", MessageBoxButton.OK);
                }
            });
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

        [DataContract]
        public class LoginRoot
        {
            [DataMember (Name = "did")]
            public string bskyDid { get; set; }
            [DataMember (Name = "accessJwt")]
            public string bskyJwt { get; set; }
            [DataMember (Name = "refreshJwt")]
            public string bskyRefJwt { get; set; }
        }

        public class LoginRequest
        {
            public string identifier { get; set; }
            public string password { get; set; }
        }
    }
}
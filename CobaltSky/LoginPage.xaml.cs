using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Microsoft.Phone.Controls;
using CobaltSky.Classes;

namespace CobaltSky
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void goBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Sending request to Bluesky's servers...");
            var api = new CobaltSky.Classes.API();
            var login = new LoginRequest
            {
                identifier = usernameTB.Text,
                password = passwordTB.Password
            };

            api.APISend(login, response => {
                Debug.WriteLine("Response from Bluesky's servers: " + response.ToString());
                if (response.ToString().Contains("accessJwt"))
                {
                    string json = response.ToString();
                    var serializer = new DataContractJsonSerializer(typeof(LoginResponse));
                    using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                    {
                        var result = (LoginResponse)serializer.ReadObject(ms);

                        string accessJwt = result.AccessJwt;
                        string bskyDid = result.Did;

                        SettingsManager.AccessJwt = accessJwt;
                        SettingsManager.BSkyDid = bskyDid;

                        Debug.WriteLine($"Saved accessJwt to settings: {accessJwt}");
                        Debug.WriteLine($"Saved bskyDid to settings: {bskyDid}");
                    }

                    MessageBox.Show("CobaltSky will now redirect you to the home page to load posts.", "login successful", MessageBoxButton.OK);
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else if (response.ToString().Contains("Unauthorized"))
                {
                    MessageBox.Show("Please double check your password and username are correct.", "login failed", MessageBoxButton.OK);
                }
            }, "/com.atproto.server.createSession", null, "POST");
        }

        [DataContract]
        public class LoginResponse
        {
            [DataMember(Name = "accessJwt")]
            public string AccessJwt { get; set; }

            [DataMember(Name = "did")]
            public string Did { get; set; }
        }

        public class LoginRequest
        {
            public string identifier { get; set; }
            public string password { get; set; }
        }
    }
}
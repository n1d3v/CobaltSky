using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using CobaltSky.Classes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CobaltSky
{
    public partial class HomePage : PhoneApplicationPage
    {
        private string token = SettingsMgr.AccessJwt;
        private string tokenRef = SettingsMgr.RefreshJwt;
        private string did = SettingsMgr.BskyDid;
        private bool _refreshRunning = false; // Needed for the refresh

        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStringsAndValues();
            await RefreshJWTTimer(5); // Refreshes it every 5 minutes
        }

        // We need this so the user doesn't get unexpectedly logged out when requesting from the servers
        private async Task RefreshJWTTimer(int timerMin)
        {
            if (_refreshRunning) return;
            _refreshRunning = true;

            var api = new CobaltSky.Classes.API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {tokenRef}" }
            };

            while (true)
            {
                await api.SendAPI("/com.atproto.server.refreshSession", "POST", null, (response) =>
                {
                    Debug.WriteLine("Refreshing the Bluesky token!");
                    Debug.WriteLine($"Response from Bluesky's servers: {response}");

                    // Reused from LoginPage.xaml.cs (with a few minor changes...)
                    var serializer = new DataContractJsonSerializer(typeof(LoginRoot));
                    using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response)))
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
                }, headers);

                await Task.Delay(TimeSpan.FromMinutes(timerMin));
            }
        }

        private void LoadStringsAndValues()
        {
            // Again, kinda a mess not gonna lie...
            if (SettingsMgr.FeedSelection == "Following")
            {
                ModifiablePage.Header = "topics";
            }
            if (SettingsMgr.FeedSelection == "Topics")
            {
                ModifiablePage.Header = "following";
            }
            if (SettingsMgr.FeedSelection == "Both")
            {
                ModifiablePage.Visibility = Visibility.Collapsed;
            }
        }

        [DataContract]
        public class LoginRoot
        {
            [DataMember(Name = "did")]
            public string bskyDid { get; set; }
            [DataMember(Name = "accessJwt")]
            public string bskyJwt { get; set; }
            [DataMember(Name = "refreshJwt")]
            public string bskyRefJwt { get; set; }
        }
    }
}
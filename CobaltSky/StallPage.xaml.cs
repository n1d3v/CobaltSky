using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using CobaltSky.Classes;
using Newtonsoft.Json;

namespace CobaltSky
{
    public partial class StallPage : PhoneApplicationPage
    {
        private string token = SettingsMgr.RefreshJwt;
        private string did = SettingsMgr.BskyDid;

        public StallPage()
        {
            InitializeComponent();
            Loaded += StallPage_Loaded;
        }

        private async void StallPage_Loaded(object sender, RoutedEventArgs e)
        {
            // This delay is to make the transition not look ugly, my bad if it makes the experience annoying.
            await Task.Delay(1000);
            Debug.WriteLine($"FinishedWelcome's state is {SettingsMgr.FinishedWelcome}");

            if (SettingsMgr.FinishedWelcome)
            {
                // Lets refresh the token while we are at it...
                var api = new CobaltSky.Classes.API();
                var headers = new Dictionary<string, string>
                {
                    { "Accept", "*/*" },
                    { "Accept-Language", "en" },
                    { "atproto-accept-labelers", did },
                    { "authorization", $"Bearer {token}" }
                };

                await api.SendAPI("/com.atproto.server.refreshSession", "POST", null, (response) =>
                {
                    Debug.WriteLine("Refreshing the Bluesky token!");
                    Debug.WriteLine($"Response from Bluesky's servers: {response}");

                    var result = JsonConvert.DeserializeObject<LoginRoot>(response);

                    // Save the JWT and DID to settings
                    SettingsMgr.AccessJwt = result.bskyJwt;
                    SettingsMgr.RefreshJwt = result.bskyRefJwt;
                    SettingsMgr.BskyDid = result.bskyDid;

                    // Show the values to confirm
                    Debug.WriteLine($"Saved accessJwt to settings: {result.bskyJwt}");
                    Debug.WriteLine($"Saved refreshJwt to settings: {result.bskyRefJwt}");
                    Debug.WriteLine($"Saved bskyDid to settings: {result.bskyDid}");
                }, headers);

                NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/WelcomePage.xaml", UriKind.Relative));
            }
        }

        public class LoginRoot
        {
            [JsonProperty("did")]
            public string bskyDid { get; set; }
            [JsonProperty("accessJwt")]
            public string bskyJwt { get; set; }
            [JsonProperty("refreshJwt")]
            public string bskyRefJwt { get; set; }
        }
    }
}
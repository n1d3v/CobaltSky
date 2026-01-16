using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using CobaltSky.Classes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CobaltSky
{
    public partial class HomePage : PhoneApplicationPage
    {
        private string token = SettingsMgr.AccessJwt;
        private string tokenRef = SettingsMgr.RefreshJwt;
        private string userHandle = SettingsMgr.BskyHandle;
        private string did = SettingsMgr.BskyDid;
        private string selectedFeed = SettingsMgr.FeedSelection;
        private string userTopics = SettingsMgr.SelectedTopics.ToLower().Replace(" ", ""); // Make it compatible with how Bluesky does it
        private bool _refreshRunning = false; // Needed for the refresh

        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStringsAndValues();
            // Kinda messy but alright, gotta do what you gotta do.
            await RefreshJWT();
            await Task.Factory.StartNew(() => RefreshJWTTimer(5));

            // Actual API shit now...
            await LoadPosts();
        }

        // We need this so the user doesn't get unexpectedly logged out when requesting from the servers
        private async Task RefreshJWTTimer(int timerMin)
        {
            if (_refreshRunning) return;
            _refreshRunning = true;

            while (true)
            {
                await RefreshJWT();
                await Task.Delay(TimeSpan.FromMinutes(timerMin));
            }
        }

        private async Task RefreshJWT()
        {
            var api = new CobaltSky.Classes.API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {tokenRef}" }
            };

            await api.SendAPI("/com.atproto.server.refreshSession", "POST", null, (response) =>
            {
                Debug.WriteLine("Refreshing the Bluesky token!");
                Debug.WriteLine($"Response from Bluesky's servers: {response}");

                var result = JsonConvert.DeserializeObject<LoginRoot>(response);

                SettingsMgr.AccessJwt = result.bskyJwt;
                SettingsMgr.RefreshJwt = result.bskyRefJwt;
                SettingsMgr.BskyDid = result.bskyDid;
            }, headers);
        }

        private async Task LoadPosts()
        {
            var api = new CobaltSky.Classes.API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {token}" },
                { "x-bsky-topics", userTopics }
            };

            // Here is where we generate a URL for what we are gonna get
            string urlNeeded = null;
            string encFeed = null;

            if (selectedFeed == "Following")
            {
                urlNeeded = "/app.bsky.feed.getTimeline";
            }
            if (selectedFeed == "Topics")
            {
                Debug.WriteLine($"The value of BskyDidPref is {SettingsMgr.BskyDidPref}");
                encFeed = Uri.EscapeDataString(SettingsMgr.BskyDidPref);
                urlNeeded = $"/app.bsky.feed.getFeed?feed={encFeed}&limit=30";
            }

            Debug.WriteLine($"Generated endpoint: {urlNeeded}");

            await api.SendAPI(urlNeeded, "GET", null, (response) =>
            {
                // Keep this here for later!
                Debug.WriteLine($"Response from Bluesky's servers (urlNeeded): {response}");
            }, headers);
        }

        private void LoadStringsAndValues()
        {
            // Again, kinda a mess not gonna lie...
            if (SettingsMgr.FeedSelection == "Following")
                ModifiablePage.Header = "topics";

            if (SettingsMgr.FeedSelection == "Topics")
                ModifiablePage.Header = "following";

            if (SettingsMgr.FeedSelection == "Both")
                ModifiablePage.Visibility = Visibility.Collapsed;
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
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
                // Extremely rare case if this happens, but it's worth it to put it here.
                if (SettingsMgr.BskyDidPref == null)
                {
                    MessageBox.Show("The value of your Bluesky ID is invalid, you will need to redo the setup once more. Sorry about that!", "uhh... something is not right", MessageBoxButton.OK);

                    // This is sort of a mess, I'll try to figure out something better in the future.
                    SettingsMgr.BskyDid = null;
                    SettingsMgr.BskyDidPref = null;
                    SettingsMgr.BskyHandle = null;
                    SettingsMgr.BskyAvatar = null;
                    SettingsMgr.AccessJwt = null;
                    SettingsMgr.RefreshJwt = null;
                    SettingsMgr.FeedSelection = null;
                    SettingsMgr.SelectedTopics = null;
                    SettingsMgr.FinishedWelcome = false;

                    Application.Current.Terminate();
                    return;
                }
                encFeed = Uri.EscapeDataString(SettingsMgr.BskyDidPref);
                urlNeeded = $"/app.bsky.feed.getFeed?feed={encFeed}&limit=30";
            }

            await api.SendAPI(urlNeeded, "GET", null, (response) =>
            {
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

        private void PostButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PostPage.xaml", UriKind.Relative));
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void CobaltWM_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to clear all of your user data and log out of the app?",
                "dangerous action!",
                MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
                return;

            SettingsMgr.BskyDid = null;
            SettingsMgr.BskyDidPref = null;
            SettingsMgr.BskyHandle = null;
            SettingsMgr.BskyAvatar = null;
            SettingsMgr.AccessJwt = null;
            SettingsMgr.RefreshJwt = null;
            SettingsMgr.FeedSelection = null;
            SettingsMgr.SelectedTopics = null;
            SettingsMgr.FinishedWelcome = false;

            Application.Current.Terminate();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using CobaltSky.Classes;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace CobaltSky
{
    public partial class FeedPage : PhoneApplicationPage
    {
        private string _displayName;
        private string token = SettingsMgr.AccessJwt;
        private string did = SettingsMgr.BskyDid;

        public FeedPage()
        {
            InitializeComponent();
            FetchProfile();

            FeedOptions.ItemsSource = new List<FeedOptionData>
            {
                new FeedOptionData { Title = "People I follow" },
                new FeedOptionData { Title = "Topics I'm interested in" },
                new FeedOptionData { Title = "Combine both to one page" }
            };

            Loaded += FeedPage_Loaded;
        }

        private async void FeedPage_Loaded(object sender, RoutedEventArgs e)
        {
            await FetchDID();
        }

        public void ChangeCustomizeDesc()
        {
            CustomizeDescriptionBlock.Text =
                $"Welcome {_displayName}! Lets get your feed setup so you can see the content you want.";
        }

        public async void FetchProfile()
        {
            var api = new API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {token}" }
            };

            await api.SendAPI($"/app.bsky.actor.getProfile?actor={did}", "GET", null, response =>
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<UserRoot>(response);

                    _displayName = result.DisplayName;
                    SettingsMgr.BskyHandle = result.Handle;

                    ChangeCustomizeDesc();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Profile parse failed: {ex}");
                }
            }, headers);
        }

        public async Task FetchDID()
        {
            var api = new API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {token}" }
            };

            await api.SendAPI("/app.bsky.actor.getPreferences", "GET", null, res =>
            {
                Debug.WriteLine($"Response from Bluesky's server (getPreferences): {res}");

                try
                {
                    var root = JsonConvert.DeserializeObject<PreferencesRoot>(res);

                    var feed = root.Preferences?
                        .FirstOrDefault(p => p.Type == "app.bsky.actor.defs#savedFeedsPrefV2")?
                        .Items?
                        .FirstOrDefault(i => i.Type == "feed");

                    if (feed != null)
                    {
                        SettingsMgr.BskyDidPref = feed.Value;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Preferences parse failed: {ex}");
                }
            }, headers);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SettingsMgr.SelectedTopics = TopicsBox.Text;
            NavigationService.Navigate(new Uri("/FinishPage.xaml", UriKind.Relative));
        }

        private void FeedOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as ListPicker;
            var data = picker?.SelectedItem as FeedOptionData;
            if (data == null) return;

            switch (data.Title)
            {
                case "People I follow":
                    SelectionExplanation.Text =
                        "This selection will make the home page your following and there will be a page for topics.";
                    SettingsMgr.FeedSelection = "Following";
                    break;

                case "Topics I'm interested in":
                    SelectionExplanation.Text =
                        "This selection will make the home page your topics and there will be a page for following.";
                    SettingsMgr.FeedSelection = "Topics";
                    break;

                case "Combine both to one page":
                    SelectionExplanation.Text =
                        "This selection will make the home page both your topics and your following combined into one feed.";
                    SettingsMgr.FeedSelection = "Both";
                    break;
            }
        }

        public class UserRoot
        {
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }

            [JsonProperty("handle")]
            public string Handle { get; set; }
        }

        class PreferencesRoot
        {
            [JsonProperty("preferences")]
            public List<PreferenceItem> Preferences { get; set; }
        }

        class PreferenceItem
        {
            [JsonProperty("$type")]
            public string Type { get; set; }

            [JsonProperty("items")]
            public List<FeedItem> Items { get; set; }
        }

        class FeedItem
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }

            [JsonProperty("pinned")]
            public bool Pinned { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public class FeedOptionData
        {
            public string Title { get; set; }
        }
    }
}
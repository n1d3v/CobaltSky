using System;
using System.Collections.Generic;
using System.Windows;
using CobaltSky.Classes;
using Microsoft.Phone.Controls;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;
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

        // Needed for pagination
        private ScrollViewer _scrollViewer;
        private const int MinItemsBeforeScroll = 5; // 5 is needed because if you scroll too fast it will not loading anything further.
        private ObservableCollection<Post> _posts = new ObservableCollection<Post>();
        private string BskyCursor; // This is needed to prevent posts that were already seen from loading.

        // Prevents spamming the Bluesky API
        private bool _isLoading;

        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
            HomePostList.ItemsSource = _posts;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStringsAndValues();

            // Extremely rare case if this happens, but it's worth it to put it here.
            if (SettingsMgr.BskyDidPref == null)
            {
                MessageBox.Show("The value of your Bluesky ID is invalid, you will need to redo the setup once more. Sorry about that!", "uhh... something is not right", MessageBoxButton.OK);

                ClearUserData();

                Application.Current.Terminate();
                return;
            }

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
            if (_isLoading)
                return;

            // To prevent spamming the API once you've reached the bottom of the list
            _isLoading = true;

            try
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

                string urlNeeded = null;
                string encFeed = null;

                if (selectedFeed == "Following")
                {
                    urlNeeded = "/app.bsky.feed.getTimeline";
                }
                else if (selectedFeed == "Topics")
                {
                    encFeed = Uri.EscapeDataString(SettingsMgr.BskyDidPref);
                    urlNeeded = $"/app.bsky.feed.getFeed?feed={encFeed}&limit=15";

                    // Needed for pagination
                    if (!string.IsNullOrEmpty(BskyCursor))
                    {
                        urlNeeded += $"&cursor={Uri.EscapeDataString(BskyCursor)}";
                    }
                }

                await api.SendAPI(urlNeeded, "GET", null, (response) =>
                {
                    var feedResponse = JsonConvert.DeserializeObject<FeedResponse>(response);
                    BskyCursor = feedResponse.cursor;

                    foreach (var item in feedResponse.feed)
                    {
                        var post = item.post;
                        if (post != null)
                        {
                            post.record.createdAt =
                                GlobalHelper.GetRelativeTime(post.record.createdAt);

                            _posts.Add(post);
                        }
                    }
                }, headers);
            }
            finally
            {
                _isLoading = false;
            }
        }

        // Navigation functions
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

            ClearUserData();

            Application.Current.Terminate();
        }

        // Helper methods
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

        public static ScrollViewer GetScrollViewer(DependencyObject root)
        {
            if (root is ScrollViewer)
                return (ScrollViewer)root;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void HomePostList_Loaded(object sender, RoutedEventArgs e)
        {
            // This is necessary to make it not load immediately and see that there's no posts so we've reached the bottom!
            HomePostList.LayoutUpdated += HomePostList_LayoutUpdated;
        }

        private void HomePostList_LayoutUpdated(object sender, EventArgs e)
        {
            if (HomePostList.Items.Count < MinItemsBeforeScroll)
                return;

            _scrollViewer = GetScrollViewer(HomePostList);

            if (_scrollViewer != null)
            {
                HomePostList.LayoutUpdated -= HomePostList_LayoutUpdated;
                _scrollViewer.LayoutUpdated += ScrollViewer_LayoutUpdated;
            }
        }

        private async void ScrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            if (_scrollViewer == null)
                return;
            if (_scrollViewer.ScrollableHeight <= 0)
                return;

            if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 25)
            {
                // This is the not the best in general, however it's what I have right now.
                // Please let me know if there is a better way to do pagination that doesn't do this.
                Debug.WriteLine("Reached the place to load posts at, loading more posts");
                await LoadPosts();
            }
        }

        private void ClearUserData()
        {
            SettingsMgr.BskyDid = null;
            SettingsMgr.BskyDidPref = null;
            SettingsMgr.BskyHandle = null;
            SettingsMgr.BskyAvatar = null;
            SettingsMgr.AccessJwt = null;
            SettingsMgr.RefreshJwt = null;
            SettingsMgr.FeedSelection = null;
            SettingsMgr.SelectedTopics = null;
            SettingsMgr.FinishedWelcome = false;
        }

        // JSON for refreshing token
        public class LoginRoot
        {
            [JsonProperty("did")]
            public string bskyDid { get; set; }
            [JsonProperty("accessJwt")]
            public string bskyJwt { get; set; }
            [JsonProperty("refreshJwt")]
            public string bskyRefJwt { get; set; }
        }

        // JSON for post handling
        public class FeedResponse
        {
            public List<FeedItem> feed { get; set; }

            // Needed for pagination
            public string cursor { get; set; }
        }

        public class FeedItem
        {
            public Post post { get; set; }
        }

        public class Post
        {
            public string uri { get; set; }
            public Author author { get; set; }
            public Record record { get; set; }
            public Embed embed { get; set; }

            public Visibility PostTextVisibility
            {
                get { return record != null && record?.text != null ? Visibility.Visible : Visibility.Collapsed; }
            }

            public Visibility EmbedVisibility
            {
                get { return embed != null && embed.external != null ? Visibility.Visible : Visibility.Collapsed; }
            }

            public Visibility RecordVisibility
            {
                get { return embed != null && embed.record != null ? Visibility.Visible : Visibility.Collapsed; }
            }

            public int replyCount { get; set; }
            public int repostCount { get; set; }
            public int likeCount { get; set; }
            public string indexedAt { get; set; }
        }

        public class Author
        {
            public string did { get; set; }
            public string handle { get; set; }
            public string displayName { get; set; }
            public string avatar { get; set; }
        }

        public class Record
        {
            [JsonProperty("$type")]
            public string Type { get; set; }

            public string createdAt { get; set; }
            public string text { get; set; }
        }

        public class Embed
        {
            [JsonProperty("$type")]
            public string Type { get; set; }

            public EmbedRecord record { get; set; }
            public EmbedExternal external { get; set; }
            public List<EmbedImage> images { get; set; }
        }

        public class EmbedRecord
        {
            public Author author { get; set; }
            public RecordValue value { get; set; }
            public List<EmbedRecord> embeds { get; set; }
        }

        public class RecordValue
        {
            [JsonProperty("$type")]
            public string Type { get; set; }

            public string createdAt { get; set; }
            public string text { get; set; }
            public Embed embed { get; set; }
        }

        public class EmbedExternal
        {
            public string uri { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string thumb { get; set; }
        }

        public class EmbedImage
        {
            public string thumb { get; set; }
            public string fullsize { get; set; }
            public string alt { get; set; }
        }
    }
}
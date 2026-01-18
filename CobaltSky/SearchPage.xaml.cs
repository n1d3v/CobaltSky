using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using CobaltSky.Classes;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Threading;
using System;

namespace CobaltSky
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private string token = SettingsMgr.AccessJwt;
        private string did = SettingsMgr.BskyDid;
        private List<Trend> _cachedTrends; // Cache for trends to prevent reloading them each time we clear the search box
        private DispatcherTimer _searchTimer; // Timer used to prevent old search data from loading, which makes the app lag out a lot.

        public SearchPage()
        {
            InitializeComponent();
            LoadTrends();
        }

        private async void LoadTrends()
        {
            if (_cachedTrends != null)
            {
                TrendListBox.ItemsSource = _cachedTrends;
                return;
            }

            var api = new API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "Authorization", $"Bearer {token}" }
            };

            await api.SendAPI("/app.bsky.unspecced.getTrends?limit=6", "GET", null, (response =>
            {
                var data = JsonConvert.DeserializeObject<TrendsResponse>(response);

                _cachedTrends = data.trends;
                TrendListBox.ItemsSource = _cachedTrends;
            }), headers);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_searchTimer != null)
            {
                _searchTimer.Stop();
                _searchTimer.Tick -= SearchTimer_Tick;
            }

            TrendListBox.ItemsSource = null;
            TopicsDesc.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchListBox.ItemsSource = null;

                TopicsDesc.Visibility = Visibility.Visible;

                TrendListBox.ItemsSource = _cachedTrends ?? null;
                if (_cachedTrends == null) LoadTrends();

                return;
            }

            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromSeconds(0.5);
            _searchTimer.Tick += SearchTimer_Tick;
            _searchTimer.Start();
        }

        private async void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Tick -= SearchTimer_Tick;

            var api = new API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "Authorization", $"Bearer {SettingsMgr.AccessJwt}" }
            };

            await api.SendAPI($"/app.bsky.actor.searchActorsTypeahead?q={SearchBox.Text}&limit=6", "GET", null, (response =>
            {
                var data = JsonConvert.DeserializeObject<SearchPage.ActorResponse>(response);

                foreach (var actor in data.actors)
                {
                    if (string.IsNullOrWhiteSpace(actor.displayName))
                        actor.displayName = "No username";
                }

                SearchListBox.ItemsSource = data.actors;
            }), headers);
        }


        // JSON for search results
        public class ActorSearch
        {
            public string did { get; set; }
            public string handle { get; set; }
            public string displayName { get; set; }
            public string avatar { get; set; }
            public Associated associated { get; set; }
            public string createdAt { get; set; }
        }

        public class Associated
        {
            public Chat chat { get; set; }
            public ActivitySubscription activitySubscription { get; set; }
        }

        public class Chat
        {
            public string allowIncoming { get; set; }
        }

        public class ActivitySubscription
        {
            public string allowSubscriptions { get; set; }
        }

        public class ActorResponse
        {
            public List<ActorSearch> actors { get; set; }
        }

        // JSON for trends
        public class TrendsResponse
        {
            public List<Trend> trends { get; set; }
        }

        public class Trend
        {
            public string topic { get; set; }
            public string displayName { get; set; }
            public string link { get; set; }
            public string startedAt { get; set; }
            public int postCount { get; set; }
            public string status { get; set; }
            public string category { get; set; }
            public List<Actor> actors { get; set; }
        }

        public class Actor
        {
            public string did { get; set; }
            public string handle { get; set; }
            public string displayName { get; set; }
            public string avatar { get; set; }
            public List<object> labels { get; set; }
            public string createdAt { get; set; }
        }
    }
}
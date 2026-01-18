using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using CobaltSky.Classes;

namespace CobaltSky
{
    public partial class PostPage : PhoneApplicationPage
    {
        private string token = SettingsMgr.AccessJwt;
        private string did = SettingsMgr.BskyDid;

        public PostPage()
        {
            InitializeComponent();
            GlobalHelper.SetImageFromUrl(ProfilePic, SettingsMgr.BskyAvatar);
        }

        private async void PostButton_Click(object sender, EventArgs e)
        {
            var api = new API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "Authorization", $"Bearer {token}" }
            };
            var post = new PostRequest
            {
                repo = did,
                collection = "app.bsky.feed.post",
                record = new JObject
                {
                    ["$type"] = "app.bsky.feed.post",
                    ["text"] = PostBox.Text,
                    ["createdAt"] = GetBlueskyDateTime()
                }
            };
            await api.SendAPI("/com.atproto.repo.createRecord", "POST", post, null, headers);

            // Go back to where the user was originally
            NavigationService.GoBack();
        }

        // Gets the precise time used for a Bluesky post, borrowed from Cerulean
        public static string GetBlueskyDateTime()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public class PostRequest
        {
            public string repo { get; set; }
            public string collection { get; set; }
            public JObject record { get; set; }
        }

        private void PostBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int count = PostBox.Text.Length;
            TextCount.Text = $"{count}/300";
        }
    }
}
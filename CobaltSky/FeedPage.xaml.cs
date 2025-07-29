using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using CobaltSky.Classes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CobaltSky
{
    public partial class FeedPage : PhoneApplicationPage
    {
        // I'm using a _ here to suppress the warning that it isn't being used
        // Even though it's being used in ChangeCustomizeDesc() and in other places
        private string _displayName;
        private string token = SettingsMgr.AccessJwt;
        private string did = SettingsMgr.BskyDid;

        public FeedPage()
        {
            InitializeComponent();
            FetchProfile();

            // Fuck M$ for making this such a piece of shit just to get one piece of data.
            FeedOptions.ItemsSource = new List<FeedOptionData>
            {
                new FeedOptionData { Title = "People I follow" },
                new FeedOptionData { Title = "Topics I'm interested in" },
                new FeedOptionData { Title = "Combine both to one page" }
            };
        }

        public void ChangeCustomizeDesc()
        {
            CustomizeDescriptionBlock.Text = $"Welcome {_displayName}! Lets get your feed setup so you can see the content you want.";
        }

        public async void FetchProfile()
        {
            var api = new CobaltSky.Classes.API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {token}" }
            };

            await api.SendAPI($"/app.bsky.actor.getProfile?actor={did}", "GET", null, response =>
            {
                Debug.WriteLine($"Response from Bluesky's servers: {response}");
                string json = response.ToString();
                var serializer = new DataContractJsonSerializer(typeof(UserRoot));
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                {
                    var result = (UserRoot)serializer.ReadObject(ms);

                    // Read the results
                    _displayName = result.bskyDName;

                    // Show the values to confirm
                    Debug.WriteLine($"Saved displayName to variable: {_displayName}");

                    // Add the customize feed text
                    ChangeCustomizeDesc();
                }
            }, headers);
        }

        [DataContract]
        public class UserRoot
        {
            [DataMember (Name = "displayName")]
            public string bskyDName { get; set; }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SettingsMgr.SelectedTopics = TopicsBox.Text;
            NavigationService.Navigate(new Uri("/FinishPage.xaml", UriKind.Relative));
        }

        private void FeedOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as ListPicker;
            if (picker != null)
            {
                var data = picker.SelectedItem as FeedOptionData;
                if (data != null)
                {
                    string title = data.Title;

                    // Kinda a mess, but whatever...
                    if (title == "People I follow")
                    {
                        SelectionExplanation.Text = "This selection will make the home page your following and there will be a page for topics.";
                        SettingsMgr.FeedSelection = "Following";
                    }
                    if (title == "Topics I'm interested in")
                    {
                        SelectionExplanation.Text = "This selection will make the home page your topics and there will be a page for following.";
                        SettingsMgr.FeedSelection = "Topics";
                    }
                    if (title == "Combine both to one page")
                    {
                        SelectionExplanation.Text = "This selection will make the home page both your topics and your following combined into one feed.";
                        SettingsMgr.FeedSelection = "Both";
                    }

                    var selectedItem = picker.ItemContainerGenerator.ContainerFromItem(data) as ListPickerItem;
                }
            }
        }

        public class FeedOptionData
        {
            public string Title { get; set; }
        }
    }
}
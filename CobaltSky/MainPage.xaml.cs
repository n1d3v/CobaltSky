using System;
using System.IO;
using System.Text;
using System.Windows.Navigation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Controls;
using CobaltSky.Classes;

namespace CobaltSky
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<PostControl> postItems = new List<PostControl>();
        private string token;
        private string did;

        public MainPage()
        {
            InitializeComponent();

            // Check if the token exists, if it does, let the debugger know and start loading data
            if (SettingsManager.AccessJwt == null && SettingsManager.BSkyDid == null)
            {
                Debug.WriteLine("Token and bskyDid is null!");
            }
            else
            {
                Debug.WriteLine("Token and bskyDid is not null!");
                token = SettingsManager.AccessJwt;
                did = SettingsManager.BSkyDid;
                LoadPosts();
            }
        }

        public static string TrimText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "(no text)";
            if (text.Length <= 40) return text;
            return text.Substring(0, 36) + "...)";
        }

        private async void LoadPosts()
        {
            var api = new CobaltSky.Classes.API();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "x-bsky-topics", "tech,science" },
                { "Accept-Language", "en" },
                { "atproto-accept-labelers", did },
                { "authorization", $"Bearer {token}" }
            };

            await api.APISend(null, async response =>
            {
                var json = response.ToString();
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(Root));
                    var root = (Root)serializer.ReadObject(ms);
                    if (root?.feed == null) return;

                    foreach (var feedItem in root.feed)
                    {
                        var post = feedItem?.post;
                        if (post == null) continue;

                        // Author information
                        string authorDid = post.author?.did ?? "(no did)";
                        string displayName = post.author?.displayName ?? "(no author)";
                        string handleName = post.author?.handle ?? "(no handle)";
                        string fullName = displayName == null
                            ? TrimText($"{post.author.handle}")
                            : TrimText($"{post.author.displayName} ({post.author.handle})");

                        // Post information
                        string text = post.record?.text ?? "(no text)";
                        string input = post.record?.createdAt;
                        string displayDate = "(no creation date)";

                        // Post image
                        string imageLink = post.record?.embed?.images?[0]?.image?.RefData?.imageLink;
                        string fullImgLink = $"https://cdn.bsky.app/img/feed_thumbnail/plain/{authorDid}/{imageLink}@jpeg";

                        // Author's avatar
                        string authorAvatarLink = post.author?.avatar;


                        BitmapImage postImage = null;
                        if (!string.IsNullOrEmpty(imageLink))
                        {
                            postImage = await DLImageToBitmap(
                                fullImgLink);
                        }

                        BitmapImage authorAvatar = new BitmapImage(new Uri("..\\Images\\user-pfp.png", UriKind.Relative));
                        if (!string.IsNullOrEmpty(imageLink))
                        {
                            authorAvatar = await DLImageToBitmap(
                                authorAvatarLink);
                        }

                        DateTime parsedDate;
                        if (DateTime.TryParse(input, null, System.Globalization.DateTimeStyles.RoundtripKind, out parsedDate))
                        {
                            TimeSpan span = DateTime.UtcNow - parsedDate.ToUniversalTime();

                            if (span.TotalDays >= 1)
                                displayDate = parsedDate.ToString("yyyy-MM-dd");
                            else if (span.TotalHours >= 1)
                                displayDate = $"{(int)span.TotalHours} hours ago";
                            else if (span.TotalMinutes >= 1)
                                displayDate = $"{(int)span.TotalMinutes} minutes ago";
                            else
                                displayDate = "just now";
                        }

                        AddPost(authorAvatar, fullName, displayDate, text, postImage,
                            post.likeCount.ToString(),
                            post.replyCount.ToString(),
                            post.repostCount.ToString());
                    }
                }
            },
            "/app.bsky.feed.getFeed?feed=at%3A%2F%2Fdid%3Aplc%3Az72i7hdynmk6r22z27h6tvur%2Fapp.bsky.feed.generator%2Fwhats-hot&limit=30",
            headers,
            "GET");
        }


        private void AddPost(BitmapImage authorPicture, string authorDisplay, string authorPostDate, string postText, BitmapImage postImage, string likeCount, string commentCount, string repostCount)
        {
            var postControl = new PostControl
            {
                AuthorPicture = authorPicture,
                AuthorName = $"{authorDisplay}",
                AuthorPostDate = authorPostDate,
                PostText = postText,
                PostImage = postImage,
                LikeCount = likeCount,
                CommentCount = commentCount,
                RepostCount = repostCount
            };

            postItems.Add(postControl);

            PostsView.ItemsSource = null;
            PostsView.ItemsSource = postItems;
        }


        public async Task<BitmapImage> DLImageToBitmap(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(imageUrl);
                var bitmap = new BitmapImage();
                using (var ms = new MemoryStream(imageBytes))
                {
                    bitmap.SetSource(ms);
                }
                return bitmap;
            }
        }

        public class PostControl
        {
            // Author related strings
            public BitmapImage AuthorPicture { get; set; }
            public string AuthorName { get; set; }
            public string AuthorPostDate { get; set; }

            // Post related strings
            public string PostText { get; set; }
            public BitmapImage PostImage { get; set; }

            // Metric related strings
            public string LikeCount { get; set; }
            public string CommentCount { get; set; }
            public string RepostCount { get; set; }
        }

        [DataContract]
        public class Root
        {
            [DataMember]
            public FeedItem[] feed { get; set; }
        }

        [DataContract]
        public class FeedItem
        {
            [DataMember]
            public Post post { get; set; }
        }

        [DataContract]
        public class Post
        {
            [DataMember]
            public Author author { get; set; }
            [DataMember]
            public Record record { get; set; }
            [DataMember]
            public int replyCount { get; set; }
            [DataMember]
            public int repostCount { get; set; }
            [DataMember]
            public int likeCount { get; set; }
            [DataMember]
            public int quoteCount { get; set; }
        }

        [DataContract]
        public class Author
        {
            [DataMember]
            public string did { get; set; }
            [DataMember]
            public string handle { get; set; }
            [DataMember]
            public string displayName { get; set; }
            [DataMember]
            public string avatar { get; set; }
        }

        [DataContract]
        public class Record
        {
            [DataMember]
            public string createdAt { get; set; }
            [DataMember]
            public string text { get; set; }
            [DataMember]
            public Embed embed { get; set; }
        }

        [DataContract]
        public class Embed
        {
            [DataMember]
            public ImageEntry[] images { get; set; }
        }

        [DataContract]
        public class ImageEntry
        {
            [DataMember]
            public ImageJson image { get; set; }
        }

        [DataContract]
        public class Images
        {
            [DataMember]
            public ImageJson image { get; set; }
        }

        [DataContract]
        public class ImageJson
        {
            [DataMember(Name = "ref")]
            public Reference RefData { get; set; }
            [DataMember]
            public string mimeType { get; set; }
        }

        [DataContract]
        public class Reference
        {
            [DataMember (Name = "$link")]
            public string imageLink { get; set; } 
        }

        private void SettingsBar_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ComposeBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ComposePost.xaml", UriKind.Relative));
        }
    }
}
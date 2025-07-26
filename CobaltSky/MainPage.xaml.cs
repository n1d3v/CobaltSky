using System.Collections.Generic;
using Microsoft.Phone.Controls;

namespace CobaltSky
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            var postItems = new List<PostControl>
            {
                new PostControl
                {
                    AuthorName = "patricktbp",
                    AuthorPostDate = "12:34pm | 7/26/2025",
                    PostText = "Test of how posts look.",
                    LikeCount = "6.7K",
                    CommentCount = "382",
                    RepostCount = "761"
                }
            };
            PostsView.ItemsSource = postItems;
        }

        public class PostControl
        {
            // Author related strings
            public string AuthorName { get; set; }
            public string AuthorPostDate { get; set; }

            // Post related strings
            public string PostText { get; set; }

            // Metric related strings
            public string LikeCount { get; set; }
            public string CommentCount { get; set; }
            public string RepostCount { get; set; }
        }
    }
}
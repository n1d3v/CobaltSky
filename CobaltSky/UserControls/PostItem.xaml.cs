using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using CobaltSky.Classes;
using System.Collections.Generic;

namespace CobaltSky.UserControls
{
    public partial class PostItem : UserControl
    {
        public PostItem()
        {
            InitializeComponent();
        }

        // Post options
        public string AuthorPicURL
        {
            get { return (string)GetValue(AuthorPicURLProperty); }
            set { SetValue(AuthorPicURLProperty, value); }
        }
        public static readonly DependencyProperty AuthorPicURLProperty =
            DependencyProperty.Register(
                nameof(AuthorPicURL),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata(string.Empty));

        public string AuthorName
        {
            get { return (string)GetValue(AuthorNameProperty); }
            set { SetValue(AuthorNameProperty, value); }
        }
        public static readonly DependencyProperty AuthorNameProperty =
            DependencyProperty.Register(
                nameof(AuthorName),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata(string.Empty));

        public string AuthorPostDate
        {
            get { return (string)GetValue(AuthorPostDateProperty); }
            set { SetValue(AuthorPostDateProperty, value); }
        }
        public static readonly DependencyProperty AuthorPostDateProperty =
            DependencyProperty.Register(
                nameof(AuthorPostDate),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata(string.Empty));

        public string PostText
        {
            get { return (string)GetValue(PostTextProperty); }
            set { SetValue(PostTextProperty, value); }
        }
        public static readonly DependencyProperty PostTextProperty =
            DependencyProperty.Register(
                nameof(PostText),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata(string.Empty, OnPostTextChanged));

        public string PostImage
        {
            get { return (string)GetValue(PostImageProperty); }
            set { SetValue(PostImageProperty, value); }
        }
        public static readonly DependencyProperty PostImageProperty =
            DependencyProperty.Register(
                nameof(PostImage),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata(string.Empty));

        public Uri PostVideo
        {
            get { return (Uri)GetValue(PostVideoProperty); }
            set { SetValue(PostVideoProperty, value); }
        }
        public static readonly DependencyProperty PostVideoProperty =
            DependencyProperty.Register(
                nameof(PostVideo),
                typeof(Uri),
                typeof(PostItem),
                new PropertyMetadata(null));

        public string LikeCount
        {
            get { return (string)GetValue(LikeCountProperty); }
            set { SetValue(LikeCountProperty, value); }
        }
        public static readonly DependencyProperty LikeCountProperty =
            DependencyProperty.Register(
                nameof(LikeCount),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata("0"));

        public string CommentCount
        {
            get { return (string)GetValue(CommentCountProperty); }
            set { SetValue(CommentCountProperty, value); }
        }
        public static readonly DependencyProperty CommentCountProperty =
            DependencyProperty.Register(
                nameof(CommentCount),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata("0"));

        public string RepostCount
        {
            get { return (string)GetValue(RepostCountProperty); }
            set { SetValue(RepostCountProperty, value); }
        }
        public static readonly DependencyProperty RepostCountProperty =
            DependencyProperty.Register(
                nameof(RepostCount),
                typeof(string),
                typeof(PostItem),
                new PropertyMetadata("0"));

        // Helper functions
        private static void OnPostPicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PostItem)d;
            var imageUrl = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                CobaltSky.Classes.GlobalHelper.SetImageFromUrl(control.PostImageCont, imageUrl);
            }
        }

        private static Regex BuildUrlRegex(HashSet<string> tlds)
        {
            var tldList = new System.Text.StringBuilder();
            bool first = true;
            foreach (var tld in tlds)
            {
                if (!first) tldList.Append("|");
                tldList.Append(Regex.Escape(tld));
                first = false;
            }

            string pattern = @"(@[a-zA-Z0-9._-]+|" +                                                                                    // Mentions like @patricktbp
                             @"#[a-zA-Z0-9_]+|" +                                                                                       // Hashtags like #cobaltsky
                             @"https?://[^\s]+|" +                                                                                      // Full URLs like https://example.com (supports http:// also)
                             @"(?:www\.)[a-zA-Z0-9.-]+\.(?:" + tldList + @")[^\s]*|" +                                                  // www. URLs like www.example.com
                             @"[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*(?:\.[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*)*\.(?:" + tldList + @")(?:/[^\s]*)?)"; // bare URLs like example.com and example.com/hello

            return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private static void OnPostTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tlds = GlobalHelper.Tlds ?? new HashSet<string>();
            var regex = BuildUrlRegex(tlds);

            var control = d as PostItem;
            if (control == null) return;

            string newText = e.NewValue as string ?? string.Empty;
            control.PostTextBlock.Inlines.Clear();

            int lastIndex = 0;

            foreach (Match m in regex.Matches(newText))
            {
                if (m.Index > lastIndex)
                {
                    control.PostTextBlock.Inlines.Add(
                        new Run { Text = newText.Substring(lastIndex, m.Index - lastIndex) });
                }

                control.PostTextBlock.Inlines.Add(new Run
                {
                    Text = m.Value,
                    Foreground = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]),
                    TextDecorations = TextDecorations.Underline
                });

                lastIndex = m.Index + m.Length;
            }

            if (lastIndex < newText.Length)
            {
                control.PostTextBlock.Inlines.Add(new Run { Text = newText.Substring(lastIndex) });
            }
        }
    }
}
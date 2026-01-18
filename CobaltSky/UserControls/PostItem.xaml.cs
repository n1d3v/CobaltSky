using System;
using System.Windows;
using System.Windows.Controls;

namespace CobaltSky.UserControls
{
    public partial class PostItem : UserControl
    {
        public PostItem()
        {
            InitializeComponent();
        }

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
                new PropertyMetadata(string.Empty));

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

        private static void OnUserAvatarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PostItem)d;
            var imageUrl = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                CobaltSky.Classes.GlobalHelper.SetImageFromUrl(control.AuthorPicture, imageUrl);
            }
        }

        private static void OnPostPicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PostItem)d;
            var imageUrl = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                CobaltSky.Classes.GlobalHelper.SetImageFromUrl(control.PostImageCont, imageUrl);
            }
        }
    }
}
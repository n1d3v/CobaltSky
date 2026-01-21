using System.Windows;
using System.Windows.Controls;

namespace CobaltSky.UserControls
{
    public partial class ReplyItem : UserControl
    {
        public ReplyItem()
        {
            InitializeComponent();
        }

        public string AuthorAvatar
        {
            get { return (string)GetValue(AuthorAvatarProperty); }
            set { SetValue(AuthorAvatarProperty, value); }
        }

        public static readonly DependencyProperty AuthorAvatarProperty =
            DependencyProperty.Register(
                nameof(AuthorAvatar),
                typeof(string),
                typeof(ReplyItem),
                new PropertyMetadata(string.Empty, OnAuthorAvatarChanged));

        private static void OnAuthorAvatarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ReplyItem;
            var url = e.NewValue as string;
            if (!string.IsNullOrEmpty(url))
            {
                Classes.GlobalHelper.SetImageFromUrl(control.ReplyAuthor, url);
            }
        }

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register(
                nameof(DisplayName),
                typeof(string),
                typeof(ReplyItem),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    ((ReplyItem)d).ReplyDispName.Text = e.NewValue as string;
                }));

        public string Info
        {
            get { return (string)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register(
                nameof(Info),
                typeof(string),
                typeof(ReplyItem),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    ((ReplyItem)d).ReplyInfo.Text = e.NewValue as string;
                }));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(ReplyItem),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    ((ReplyItem)d).ReplyText.Text = e.NewValue as string;
                }));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(string),
                typeof(ReplyItem),
                new PropertyMetadata(string.Empty, OnImageChanged));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ReplyItem;
            var url = e.NewValue as string;
            if (!string.IsNullOrEmpty(url))
            {
                Classes.GlobalHelper.SetImageFromUrl(control.ReplyImage, url);
            }
        }
    }
}
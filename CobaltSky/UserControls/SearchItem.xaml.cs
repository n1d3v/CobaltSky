using System.Windows;
using System.Windows.Controls;

namespace CobaltSky.UserControls
{
    public partial class SearchItem : UserControl
    {
        public SearchItem()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register(
                "UserName",
                typeof(string),
                typeof(SearchItem),
                new PropertyMetadata(""));

        public string UserHandle
        {
            get { return (string)GetValue(UserHandleProperty); }
            set { SetValue(UserHandleProperty, value); }
        }
        public static readonly DependencyProperty UserHandleProperty =
            DependencyProperty.Register(
                "UserHandle",
                typeof(string),
                typeof(SearchItem),
                new PropertyMetadata(""));

        public string UserAvatar
        {
            get { return (string)GetValue(UserAvatarProperty); }
            set { SetValue(UserAvatarProperty, value); }
        }
        public static readonly DependencyProperty UserAvatarProperty =
            DependencyProperty.Register(
                "UserAvatar",
                typeof(string),
                typeof(SearchItem),
                new PropertyMetadata("", OnUserAvatarChanged));

        private static void OnUserAvatarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchItem)d;
            var imageUrl = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                CobaltSky.Classes.GlobalHelper.SetImageFromUrl(control.AvatarImage, imageUrl);
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace CobaltSky.UserControls
{
    public partial class PostEmbed : UserControl
    {
        public PostEmbed()
        {
            InitializeComponent();
        }

        public string Thumb
        {
            get { return (string)GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        public static readonly DependencyProperty ThumbProperty =
            DependencyProperty.Register(
                nameof(Thumb),
                typeof(string),
                typeof(PostEmbed),
                new PropertyMetadata(string.Empty, OnThumbChanged));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(PostEmbed),
                new PropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(PostEmbed),
                new PropertyMetadata(string.Empty));

        private static void OnThumbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PostEmbed;
            var url = e.NewValue as string;

            if (!string.IsNullOrEmpty(url))
            {
                CobaltSky.Classes.GlobalHelper.SetImageFromUrl(control.EmbedImage, url);
            }
        }
    }
}

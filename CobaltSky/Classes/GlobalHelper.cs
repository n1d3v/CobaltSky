using System;
using System.Net;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CobaltSky.Classes
{
    public static class GlobalHelper
    {
        public static HashSet<string> Tlds { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static void SetImageFromUrl(Image imageControl, string imageUrl)
        {
            if (imageControl == null)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    var defaultBitmap = new BitmapImage(new Uri("/Images/Home/user-pfp.png", UriKind.Relative));
                    imageControl.Source = defaultBitmap;
                    return;
                }

                var webClient = new WebClient();
                webClient.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error == null && e.Result != null)
                    {
                        var bitmap = new BitmapImage();
                        bitmap.SetSource(e.Result);
                        imageControl.Source = bitmap;
                    }
                    else
                    {
                        var defaultBitmap = new BitmapImage(new Uri("/Images/Home/user-pfp.png", UriKind.Relative));
                        imageControl.Source = defaultBitmap;
                    }
                };
                webClient.OpenReadAsync(new Uri(imageUrl, UriKind.Absolute));
            }
            finally
            {
                var defaultBitmap = new BitmapImage(new Uri("/Images/Home/user-pfp.png", UriKind.Relative));
                imageControl.Source = defaultBitmap;
            }
        }

        public static string GetRelativeTime(string utcTimeString)
        {
            DateTime time;
            if (!DateTime.TryParse(utcTimeString, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out time))
                return utcTimeString;

            var span = DateTime.UtcNow - time;

            if (span.TotalSeconds < 60)
                return $"{(int)span.TotalSeconds} second{(span.TotalSeconds >= 2 ? "s" : "")} ago";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes} minute{(span.TotalMinutes >= 2 ? "s" : "")} ago";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours} hour{(span.TotalHours >= 2 ? "s" : "")} ago";
            if (span.TotalDays < 30)
                return $"{(int)span.TotalDays} day{(span.TotalDays >= 2 ? "s" : "")} ago";
            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)} month{(span.TotalDays / 30 >= 2 ? "s" : "")} ago";

            return $"{(int)(span.TotalDays / 365)} year{(span.TotalDays / 365 >= 2 ? "s" : "")} ago";
        }

        public static Task LoadTldsAsync()
        {
            return Task.Run(() =>
            {
                var uri = new Uri("tld-list.txt", UriKind.Relative);
                var streamResourceInfo = Application.GetResourceStream(uri);
                using (var reader = new StreamReader(streamResourceInfo.Stream))
                {
                    var txt = reader.ReadToEnd();

                    Tlds = new HashSet<string>(
                        txt.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.StartsWith(".") ? t.Substring(1) : t),
                        StringComparer.OrdinalIgnoreCase
                    );
                }
            });
        }

        public class BoolToVisConv : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
} 
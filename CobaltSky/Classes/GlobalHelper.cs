using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CobaltSky.Classes
{
    public static class GlobalHelper
    {
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
            catch (Exception ex)
            {
                var defaultBitmap = new BitmapImage(new Uri("/Images/Home/user-pfp.png", UriKind.Relative));
                imageControl.Source = defaultBitmap;
            }
        }
    }
}
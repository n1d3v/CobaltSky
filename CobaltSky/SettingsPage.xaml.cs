using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Phone.Controls;


namespace CobaltSky
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            var accountItems = new List<SettingsItem>
            {
                new SettingsItem
                {
                    SettingsIconSource = "..\\Images\\SettingsIcon\\user.png",
                    SettingsName = "Login to Bluesky!",
                    SettingsDescription = "You are currently not logged into Bluesky.",
                    ClickAction = () =>
                    {
                        NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    }
                }
            };

            accountView.ItemsSource = accountItems;
        }

        private void accountView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = accountView.SelectedItem as SettingsItem;
            if (item != null)
            {
                item.ClickAction?.Invoke();
                accountView.SelectedItem = null;
            }
        }

        public class SettingsItem
        {
            // Icon for the settings item (Should be 70x70 and a PNG)
            public string SettingsIconSource { get; set; }

            // The settings name and description
            public string SettingsName { get; set; }
            public string SettingsDescription { get; set; }

            // A per-item click handler
            public Action ClickAction { get; set; }
        }
    }
}
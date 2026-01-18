using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using CobaltSky.Classes;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CobaltSky
{
    public partial class FinishPage : PhoneApplicationPage
    {
        public FinishPage()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SettingsMgr.FinishedWelcome = true;
            NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
        }

        private void GoBackButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FeedPage.xaml", UriKind.Relative));
        }
    }
}
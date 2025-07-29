using System;
using System.Diagnostics;
using CobaltSky.Classes;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace CobaltSky
{
    public partial class WelcomePage : PhoneApplicationPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}
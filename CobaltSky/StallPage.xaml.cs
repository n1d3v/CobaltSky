using System;
using System.Windows;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using CobaltSky.Classes;

namespace CobaltSky
{
    public partial class StallPage : PhoneApplicationPage
    {
        private string token = SettingsMgr.RefreshJwt;
        private string did = SettingsMgr.BskyDid;

        public StallPage()
        {
            InitializeComponent();
            Loaded += StallPage_Loaded;
        }
    
        private async void StallPage_Loaded(object sender, RoutedEventArgs e)
        {
            // This delay is to make the transition not look ugly, my bad if it makes the experience annoying.
            await Task.Delay(1000);
            Debug.WriteLine($"FinishedWelcome's state is {SettingsMgr.FinishedWelcome.ToString()}");
            if (SettingsMgr.FinishedWelcome == true)
            {
                // Lets refresh the token while we are at it...
                var api = new CobaltSky.Classes.API();
                var headers = new Dictionary<string, string>
                {
                    { "Accept", "*/*" },
                    { "Accept-Language", "en" },
                    { "atproto-accept-labelers", did },
                    { "authorization", $"Bearer {token}" }
                };

                await api.SendAPI("/com.atproto.server.refreshSession", "POST", null, (response) =>
                {
                    Debug.WriteLine("Refreshing the Bluesky token!");
                    Debug.WriteLine($"Response from Bluesky's servers: {response}");

                    // Reused from LoginPage.xaml.cs
                    string json = response.ToString();
                    var serializer = new DataContractJsonSerializer(typeof(LoginRoot));
                    using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                    {
                        var result = (LoginRoot)serializer.ReadObject(ms);

                        // Read the results
                        string accessJwt = result.bskyJwt;
                        string refreshJwt = result.bskyRefJwt;
                        string bskyDid = result.bskyDid;

                        // Save the JWT and DID to settings
                        SettingsMgr.AccessJwt = accessJwt;
                        SettingsMgr.RefreshJwt = refreshJwt;
                        SettingsMgr.BskyDid = bskyDid;

                        // Show the values to confirm
                        Debug.WriteLine($"Saved accessJwt to settings: {accessJwt}");
                        Debug.WriteLine($"Saved refreshJwt to settings: {refreshJwt}");
                        Debug.WriteLine($"Saved bskyDid to settings: {bskyDid}");
                    }
                }, headers);
                NavigationService.Navigate(new Uri("/HomePage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/WelcomePage.xaml", UriKind.Relative));
            }
        }

        [DataContract]
        public class LoginRoot
        {
            [DataMember(Name = "did")]
            public string bskyDid { get; set; }
            [DataMember(Name = "accessJwt")]
            public string bskyJwt { get; set; }
            [DataMember(Name = "refreshJwt")]
            public string bskyRefJwt { get; set; }
        }
    }
}
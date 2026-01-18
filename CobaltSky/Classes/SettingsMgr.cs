using System.IO.IsolatedStorage;

namespace CobaltSky.Classes
{
    static class SettingsMgr
    {
        private static string GetString(string key)
        {
            object value;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value))
                return value as string;
            return null;
        }

        private static void SetString(string key, string value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private static bool GetBool(string key)
        {
            object value;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value) && value is bool)
                return (bool)value;
            return false;
        }

        private static void SetBool(string key, bool value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static string AccessJwt
        {
            get { return GetString("accessJwt"); }
            set { SetString("accessJwt", value); }
        }

        public static string RefreshJwt
        {
            get { return GetString("refreshJwt"); }
            set { SetString("refreshJwt", value); }
        }

        public static string BskyDid
        {
            get { return GetString("bskyDid"); }
            set { SetString("bskyDid", value); }
        }

        public static string BskyHandle
        {
            get { return GetString("bskyHandle"); }
            set { SetString("bskyHandle", value); }
        }

        public static string BskyAvatar
        {
            get { return GetString("bskyAvatar"); }
            set { SetString("bskyAvatar", value); }
        }

        public static string BskyDidPref
        {
            get { return GetString("bskyDidPref"); }
            set { SetString("bskyDidPref", value); }
        }

        public static string SelectedTopics
        {
            get { return GetString("selectedTopics"); }
            set { SetString("selectedTopics", value); }
        }

        public static string FeedSelection
        {
            get { return GetString("selectedFeed"); }
            set { SetString("selectedFeed", value); }
        }

        public static bool FinishedWelcome
        {
            get { return GetBool("finishedWel"); }
            set { SetBool("finishedWel", value); }
        }
    }
}
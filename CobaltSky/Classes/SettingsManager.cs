// This is just a simple manager for Settings.
// Makes it easier to fetch them and shit like that.

using System.IO.IsolatedStorage;

namespace CobaltSky.Classes
{
    class SettingsManager
    {
        public static string AccessJwt
        {
            get
            {
                object value;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("accessJwt", out value))
                    return value as string;
                return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["accessJwt"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public static string BSkyDid
        {
            get
            {
                object value;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("bskyDid", out value))
                    return value as string;
                return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["bskyDid"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}

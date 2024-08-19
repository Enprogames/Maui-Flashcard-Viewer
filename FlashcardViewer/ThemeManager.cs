using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlashcardViewer.Resources.Themes;

namespace FlashcardViewer
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string themeName)
        {
            ResourceDictionary themeDictionary = themeName switch
            {
                "Light" => new LightTheme(),
                "Dark" => new DarkTheme(),
                _ => new LightTheme(), // Default theme
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
            Preferences.Set("AppTheme", themeName);
        }

        public static void LoadSavedTheme()
        {
            string savedTheme = Preferences.Get("AppTheme", "Light");
            ApplyTheme(savedTheme);
        }
    }
}

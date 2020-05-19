using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MacosApp.Common.Helpers
{
    public static class Settings
    {
        private const string _labour = "Labour";
        private const string _token = "Token";
        private const string _employee = "Employee";
        private const string _isRemembered = "IsRemembered";
        private static readonly string _stringDefault = string.Empty;
        private static readonly bool _boolDefault = false;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string Labour
        {
            get => AppSettings.GetValueOrDefault(_labour, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_labour, value);
        }

        public static string Token
        {
            get => AppSettings.GetValueOrDefault(_token, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_token, value);
        }

        public static string Employee
        {
            get => AppSettings.GetValueOrDefault(_employee, _stringDefault);
            set => AppSettings.AddOrUpdateValue(_employee, value);
        }

        public static bool IsRemembered
        {
            get => AppSettings.GetValueOrDefault(_isRemembered, _boolDefault);
            set => AppSettings.AddOrUpdateValue(_isRemembered, value);
        }
    }
}

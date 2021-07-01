using UnityEditor;

namespace ProjectPlanner
{
    public static class Prefs
    {
        public static void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(FormatKey(key), value);
        }
        public static int GetInt(string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(FormatKey(key), defaultValue);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            return EditorPrefs.GetString(FormatKey(key), defaultValue);
        }
        public static void SetString(string key, string value)
        {
            EditorPrefs.SetString(FormatKey(key), value);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return EditorPrefs.GetBool(FormatKey(key), defaultValue);
        }
        public static void SetBool(string key, bool value)
        {
            EditorPrefs.SetBool(FormatKey(key), value);
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return EditorPrefs.GetFloat(FormatKey(key), defaultValue);
        }
        public static void SetFloat(string key, float value)
        {
            EditorPrefs.SetFloat(FormatKey(key), value);
        }

        public static void Delete(string key)
        {
            EditorPrefs.DeleteKey(FormatKey(key));
        }
        public static bool HasKey(string key)
        {
            return EditorPrefs.HasKey(FormatKey(key));
        }

        private static string FormatKey(string key)
        {
            return (Info.Prefix + key).ToLower();
        }
    }
}
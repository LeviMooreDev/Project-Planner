using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    sealed class Menu
    {
        private const string root = "Tools/Project Planner/";

        [MenuItem(root + "Board Window %&b", false, 0)]
        private static void OpenBoardWindow()
        {
            BoardWindow.Init();
        }
        [MenuItem(root + "Task Window %&t", false, 1)]
        private static void OpenTaskWindow()
        {
            TaskWindow.Init();
        }
        [MenuItem(root + "Tree Window", false, 2)]
        private static void OpenTreeViewWindow()
        {
            TreeWindow.Init();
        }
        [MenuItem(root + "Code Analyzer", false, 3)]
        private static void OpenCodeAnalyzerWindow()
        {
            CodeAnalyzerWindow.Init();
        }
        [MenuItem(root + "Quick Task %&q", false, 4)]
        private static void OpenQuickTaskWindow()
        {
            QuickTaskWindow.Init();
        }

        [MenuItem(root + "Settings", false, 98)]
        public static void OpenSettings()
        {
            PreferencesWindow.Init();
        }

        [MenuItem(root + "Import Demo Content", false, 100)]
        public static void DemoContent()
        {
            DemoImporter.Init();
        }

        [MenuItem(root + "Manual", false, 103)]
        public static void Manual()
        {
            Application.OpenURL(FileManager.Manual);
        }

        [MenuItem(root + "GitHub", false, 104)]
        public static void GitHub()
        {
            Application.OpenURL(Info.GitHub);
        }
    }
}

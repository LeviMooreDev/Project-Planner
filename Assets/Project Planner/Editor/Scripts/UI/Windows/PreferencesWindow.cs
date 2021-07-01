using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public class PreferencesWindow : EditorWindow
    {
        [SerializeField]
        private Vector2 scrollPosition;

        public static void Init()
        {
            PreferencesWindow window = GetWindow<PreferencesWindow>(true, Language.Settings);
            window.minSize = window.maxSize = new Vector2(450, 400);
            window.CenterPosition();
        }

        private void OnGUI()
        {
            scrollPosition = UI.BeginScrollView(scrollPosition);
            PreferencesWindowBuildIn.PreferencesGUI();
            UI.EndScrollView();
        }
    }
    public static class PreferencesWindowBuildIn
    {
        private static List<string> languageNames;
        private static Rect boardDefaultGroupsRect;
        private static Rect codeAnalyzerScanWordsRect;
        private static Rect hideFilesButtonRect;

        private static bool Foldout(string name)
        {
            Prefs.SetBool("settingsWindow_show_" + name, UI.Foldout(Prefs.GetBool("settingsWindow_show_" + name), name));
            return Prefs.GetBool("settingsWindow_show_" + name);
        }

        [PreferenceItem("Project Planner")]
        public static void PreferencesGUI()
        {
            EditorGUIUtility.labelWidth = 240;

            if (languageNames == null || languageNames.Count == 0)
                languageNames = FileManager.Language.LanguageNames();

            EditorGUI.BeginChangeCheck();

            if (Foldout(Language.BoardWindow))
            {
                Settings.BoardWindow.AllowMultipleWindows = EditorGUILayout.Toggle(Language.AllowMultipleWindows, Settings.BoardWindow.AllowMultipleWindows);
                Settings.BoardWindow.BoardNameInWindowTab = EditorGUILayout.Toggle(Language.BoardNameInWindowTab, Settings.BoardWindow.BoardNameInWindowTab);
                Settings.BoardWindow.MinimumHeight = EditorGUILayout.IntField(new GUIContent(Language.MinimumHeight, "[100-1000]"), Settings.BoardWindow.MinimumHeight);
                Settings.BoardWindow.MinimumHeight = Mathf.Clamp(Settings.BoardWindow.MinimumHeight, 100, 1000);
                Settings.BoardWindow.MinimumWidth = EditorGUILayout.IntField(new GUIContent(Language.MinimumWidth, "[100-1000]"), Settings.BoardWindow.MinimumWidth);
                Settings.BoardWindow.MinimumWidth = Mathf.Clamp(Settings.BoardWindow.MinimumWidth, 100, 1000);
                Settings.BoardWindow.Utility = EditorGUILayout.Toggle(Language.Utility, Settings.BoardWindow.Utility);

                UI.Label(Language.Group, UI.LabelStyle.Bold);
                Settings.BoardWindow.Group.DragThreshold = EditorGUILayout.IntSlider(Language.DragThreshold, Settings.BoardWindow.Group.DragThreshold, 0, 200);
                Settings.BoardWindow.Group.FixedWidth = EditorGUILayout.IntSlider(Language.FixedWidth, Settings.BoardWindow.Group.FixedWidth, 75, 1000);
                if (Settings.BoardWindow.Group.Height != Settings.GroupSize.Fixed)
                    UI.Utilities.Enabled = false;
                Settings.BoardWindow.Group.FixedHeight = EditorGUILayout.IntSlider(Language.FixedHeight, Settings.BoardWindow.Group.FixedHeight, 100, 1000);
                UI.Utilities.Enabled = true;
                Settings.BoardWindow.Group.Grid = EditorGUILayout.Toggle(Language.Grid, Settings.BoardWindow.Group.Grid);
                Settings.BoardWindow.Group.Height = (Settings.GroupSize)EditorGUILayout.EnumPopup(Language.Height, Settings.BoardWindow.Group.Height);
                Settings.BoardWindow.Group.HideEmptyWhenSearching = EditorGUILayout.Toggle(Language.HideEmptyWhenSearching, Settings.BoardWindow.Group.HideEmptyWhenSearching);
                Settings.BoardWindow.Group.PlaceNew = (Settings.PlaceNewGroup)EditorGUILayout.EnumPopup(Language.PlaceNew, Settings.BoardWindow.Group.PlaceNew);

                UI.Label(Language.Task, UI.LabelStyle.Bold);
                Settings.BoardWindow.Task.DescriptionIndicator = EditorGUILayout.TextField(Language.DescriptionIndicator, Settings.BoardWindow.Task.DescriptionIndicator);
                Settings.BoardWindow.Task.DragThreshold = EditorGUILayout.IntSlider(Language.DragThreshold, Settings.BoardWindow.Task.DragThreshold, 0, 50);
                Settings.BoardWindow.Task.MinHeight = EditorGUILayout.IntSlider(Language.MinHeight, Settings.BoardWindow.Task.MinHeight, 20, 200);
                Settings.BoardWindow.Task.ShowDescriptionIndicator = EditorGUILayout.Toggle(Language.ShowDescriptionIndicator, Settings.BoardWindow.Task.ShowDescriptionIndicator);

                UI.Space();
            }
            if (Foldout(Language.TaskWindow))
            {
                Settings.TaskWindow.AutoDockWithBoard = EditorGUILayout.Toggle(Language.AutoDockWithBoard, Settings.TaskWindow.AutoDockWithBoard);
                Settings.TaskWindow.DescriptionFixedHeight = EditorGUILayout.Toggle(Language.DescriptionFixedHeight, Settings.TaskWindow.DescriptionFixedHeight);
                if (!Settings.TaskWindow.DescriptionFixedHeight)
                    UI.Utilities.Enabled = false;
                Settings.TaskWindow.DescriptionHeight = EditorGUILayout.IntSlider(Language.DescriptionHeight, Settings.TaskWindow.DescriptionHeight, 1, 99);
                UI.Utilities.Enabled = true;
                Settings.TaskWindow.FoldoutAnimation = EditorGUILayout.Toggle(Language.FoldoutAnimation, Settings.TaskWindow.FoldoutAnimation);
                Settings.TaskWindow.MinimumHeight = EditorGUILayout.IntField(new GUIContent(Language.MinimumHeight, "[100-1000]"), Settings.TaskWindow.MinimumHeight);
                Settings.TaskWindow.MinimumHeight = Mathf.Clamp(Settings.TaskWindow.MinimumHeight, 100, 1000);
                Settings.TaskWindow.MinimumWidth = EditorGUILayout.IntField(new GUIContent(Language.MinimumWidth, "[100-1000]"), Settings.TaskWindow.MinimumWidth);
                Settings.TaskWindow.MinimumWidth = Mathf.Clamp(Settings.TaskWindow.MinimumWidth, 100, 1000);
                Settings.TaskWindow.ShowTimestamps = EditorGUILayout.Toggle(Language.ShowTimestamps, Settings.TaskWindow.ShowTimestamps);
                Settings.TaskWindow.Utility = EditorGUILayout.Toggle(Language.Utility, Settings.TaskWindow.Utility);
            }
            if (Foldout(Language.TreeWindow))
            {
                UI.Label(Language.Group, UI.LabelStyle.Bold);
                Settings.TreeWindow.Group.DragThreshold = EditorGUILayout.IntSlider(Language.DragThreshold, Settings.TreeWindow.Group.DragThreshold, 0, 50);
                Settings.TreeWindow.Group.HideEmptyWhenSearching = EditorGUILayout.Toggle(Language.HideEmptyWhenSearching, Settings.TreeWindow.Group.HideEmptyWhenSearching);

                UI.Label(Language.Task, UI.LabelStyle.Bold);
                Settings.TreeWindow.Task.DescriptionIndicator = EditorGUILayout.TextField(Language.DescriptionIndicator, Settings.TreeWindow.Task.DescriptionIndicator);
                Settings.TreeWindow.Task.DragThreshold = EditorGUILayout.IntSlider(Language.DragThreshold, Settings.TreeWindow.Task.DragThreshold, 0, 50);
                Settings.TreeWindow.Task.ShowDescriptionIndicator = EditorGUILayout.Toggle(Language.ShowDescriptionIndicator, Settings.TreeWindow.Task.ShowDescriptionIndicator);

                UI.Label(Language.Coloring, UI.LabelStyle.Bold);
                Settings.TreeWindow.RowColoring.Type = (Settings.TreeRowColoring)EditorGUILayout.EnumPopup(Language.RowColoring, Settings.TreeWindow.RowColoring.Type);
                if (Settings.TreeWindow.RowColoring.Type == Settings.TreeRowColoring.Alternating)
                {
                    Settings.TreeWindow.RowColoring.Alternating.Even = EditorGUILayout.ColorField(Language.Even.Prefix(" -"), Settings.TreeWindow.RowColoring.Alternating.Even);
                    Settings.TreeWindow.RowColoring.Alternating.Odd = EditorGUILayout.ColorField(Language.Odd.Prefix(" -"), Settings.TreeWindow.RowColoring.Alternating.Odd);
                }
                else if (Settings.TreeWindow.RowColoring.Type == Settings.TreeRowColoring.ByType)
                {
                    Settings.TreeWindow.RowColoring.ByType.Board = EditorGUILayout.ColorField(Language.Board.Prefix(" -"), Settings.TreeWindow.RowColoring.ByType.Board);
                    Settings.TreeWindow.RowColoring.ByType.Group = EditorGUILayout.ColorField(Language.Group.Prefix(" -"), Settings.TreeWindow.RowColoring.ByType.Group);
                    Settings.TreeWindow.RowColoring.ByType.Task = EditorGUILayout.ColorField(Language.Task.Prefix(" -"), Settings.TreeWindow.RowColoring.ByType.Task);
                    Settings.TreeWindow.RowColoring.ByType.Subtask = EditorGUILayout.ColorField(Language.Subtask.Prefix(" -"), Settings.TreeWindow.RowColoring.ByType.Subtask);
                }
                else if (Settings.TreeWindow.RowColoring.Type == Settings.TreeRowColoring.Same)
                {
                    Settings.TreeWindow.RowColoring.Same = EditorGUILayout.ColorField(Language.Color.Prefix(" -"), Settings.TreeWindow.RowColoring.Same);
                }
                Settings.TreeWindow.RowColoring.RowStyle = (Settings.TreeRowStyle)EditorGUILayout.EnumPopup(Language.RowStyle, Settings.TreeWindow.RowColoring.RowStyle);

            }
            if (Foldout(Language.BoardGeneral))
            {
                Settings.BoardGeneral.DefaultName = EditorGUILayout.TextField(Language.DefaultName, Settings.BoardGeneral.DefaultName);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Language.DefaultGroups);
                if (UI.Button(Language.Edit, UI.ButtonStyle.Normal, false))
                {
                    UI.Utilities.ShowStringListPopup(boardDefaultGroupsRect, Settings.BoardGeneral.DefaultGroups.ToList(),
                    (values) =>
                    {
                        Settings.BoardGeneral.DefaultGroups = values.ToArray();
                        Settings.Save();
                    });
                }
                boardDefaultGroupsRect = UI.Utilities.LastRectOnRepaintRelative(boardDefaultGroupsRect);
                GUILayout.EndHorizontal();
            }
            if (Foldout(Language.GroupGeneral))
            {
                Settings.GroupGeneral.DefaultName = EditorGUILayout.TextField(Language.DefaultName, Settings.GroupGeneral.DefaultName);
            }
            if (Foldout(Language.TaskGeneral))
            {
                Settings.TaskGeneral.AutoSelectNew = EditorGUILayout.Toggle(Language.AutoSelectNew, Settings.TaskGeneral.AutoSelectNew);
                Settings.TaskGeneral.DefaultName = EditorGUILayout.TextField(Language.DefaultName, Settings.TaskGeneral.DefaultName);
            }
            if (Foldout(Language.Tags))
            {
                UI.Utilities.BeginChangeCheck();
                Settings.Tags.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Tags.Enabled);
                if (UI.Utilities.EndChangeCheck())
                {
                    BoardWindow boardWindow = BoardWindow.I;
                    if (boardWindow != null)
                    {
                        boardWindow.Search();
                        boardWindow.Repaint();
                    }

                    TreeWindow treeWindow = TreeWindow.I;
                    if (treeWindow != null)
                    {
                        treeWindow.Search();
                        treeWindow.Repaint();
                    }
                }
                if (Settings.Tags.Enabled)
                {
                    Settings.Tags.ConfirmRemoval = EditorGUILayout.Toggle(Language.ConfirmRemoval, Settings.Tags.ConfirmRemoval);
                    Settings.Tags.ShowByDefault = EditorGUILayout.Toggle(Language.ShowByDefault, Settings.Tags.ShowByDefault);
                }
            }
            if (Foldout(Language.Colors))
            {
                UI.Utilities.BeginChangeCheck();
                Settings.Colors.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Colors.Enabled);
                if (UI.Utilities.EndChangeCheck())
                {
                    BoardWindow boardWindow = BoardWindow.I;
                    if (boardWindow != null)
                    {
                        boardWindow.Search();
                        boardWindow.Repaint();
                    }

                    TreeWindow treeWindow = TreeWindow.I;
                    if (treeWindow != null)
                    {
                        treeWindow.Search();
                        treeWindow.Repaint();
                    }
                }
                if (Settings.Colors.Enabled)
                {
                    Settings.Colors.ConfirmRemoval = EditorGUILayout.Toggle(Language.ConfirmRemoval, Settings.Colors.ConfirmRemoval);
                    Settings.Colors.IconType = (Settings.ColorIcon)EditorGUILayout.EnumPopup(Language.IconType, Settings.Colors.IconType);
                    Settings.Colors.ShowByDefault = EditorGUILayout.Toggle(Language.ShowByDefault, Settings.Colors.ShowByDefault);
                }
            }
            if (Foldout(Language.Assets))
            {
                Settings.Assets.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Assets.Enabled);
                if (Settings.Assets.Enabled)
                {
                    Settings.Assets.ConfirmRemoval = EditorGUILayout.Toggle(Language.ConfirmRemoval, Settings.Assets.ConfirmRemoval);
                    Settings.Assets.ShowByDefault = EditorGUILayout.Toggle(Language.ShowByDefault, Settings.Assets.ShowByDefault);
                }
            }
            if (Foldout(Language.Screenshots))
            {
                Settings.Screenshots.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Screenshots.Enabled);
                if (Settings.Screenshots.Enabled)
                {
                    Settings.Screenshots.AutoViewNew = EditorGUILayout.Toggle(Language.AutoViewNew, Settings.Screenshots.AutoViewNew);
                    Settings.Screenshots.BackgroundColor = EditorGUILayout.ColorField(Language.BackgroundColor, Settings.Screenshots.BackgroundColor);
                    Settings.Screenshots.ConfirmRemoval = EditorGUILayout.Toggle(Language.ConfirmRemoval, Settings.Screenshots.ConfirmRemoval);
                    Settings.Screenshots.DefaultName = EditorGUILayout.TextField(Language.DefaultName, Settings.Screenshots.DefaultName);
                    Settings.Screenshots.ShowByDefault = EditorGUILayout.Toggle(Language.ShowByDefault, Settings.Screenshots.ShowByDefault);
                    Settings.Screenshots.UtilityWindow = EditorGUILayout.Toggle(Language.UtilityWindow, Settings.Screenshots.UtilityWindow);
                }
            }
            if (Foldout(Language.Subtasks))
            {
                Settings.Subtasks.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Subtasks.Enabled);
                if (Settings.Subtasks.Enabled)
                {
                    Settings.Subtasks.ShowByDefault = EditorGUILayout.Toggle(Language.ShowByDefault, Settings.Subtasks.ShowByDefault);
                    Settings.Subtasks.ShowIndividualProgress = EditorGUILayout.Toggle(Language.ShowIndividualProgress, Settings.Subtasks.ShowIndividualProgress);
                    Settings.Subtasks.ShowProgressBar = EditorGUILayout.Toggle(Language.ShowProgressBar, Settings.Subtasks.ShowProgressBar);
                    Settings.Subtasks.ShowProgressInBoardWindow = EditorGUILayout.Toggle(Language.ShowProgressInBoardWindow, Settings.Subtasks.ShowProgressInBoardWindow);
                }
            }
            if (Foldout(Language.Locking))
            {
                Settings.Locking.Enabled = EditorGUILayout.Toggle(Language.Enabled, Settings.Locking.Enabled);
                if (Settings.Locking.Enabled)
                {
                    Settings.Locking.ShowIndicator = EditorGUILayout.Toggle(Language.ShowIndicator, Settings.Locking.ShowIndicator);
                    if (Settings.Locking.ShowIndicator)
                    {
                        Settings.Locking.Color = EditorGUILayout.ColorField(Language.Color.Prefix(" -"), Settings.Locking.Color);
                        Settings.Locking.SubtaskColor = EditorGUILayout.ColorField(Language.SubtaskColor.Prefix(" -"), Settings.Locking.SubtaskColor);
                    }
                }
            }
            if (Foldout(Language.CodeAnalyzer))
            {
                Settings.CodeAnalyzer.DoubleClickToFind = EditorGUILayout.Toggle(Language.DoubleClickToFind, Settings.CodeAnalyzer.DoubleClickToFind);
                Settings.CodeAnalyzer.FindNotImplementedException = EditorGUILayout.Toggle(Language.FindNotImplementedException, Settings.CodeAnalyzer.FindNotImplementedException);
                Settings.CodeAnalyzer.FoldoutOpenByDefault = EditorGUILayout.Toggle(Language.FoldoutOpenByDefault, Settings.CodeAnalyzer.FoldoutOpenByDefault);


                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Language.HideFiles);
                if (UI.Button(Language.HideFiles, UI.ButtonStyle.Normal, false))
                {
                    UI.Utilities.ShowStringListPopup(hideFilesButtonRect, Settings.CodeAnalyzer.HideFilesNames.ToList(),
                    (values) =>
                    {
                        Settings.CodeAnalyzer.HideFilesNames = values.ToArray();
                        Settings.Save();
                    });
                }
                hideFilesButtonRect = UI.Utilities.LastRectOnRepaintRelative(hideFilesButtonRect);
                GUILayout.EndHorizontal();



                Settings.CodeAnalyzer.ScanAfterCompiling = EditorGUILayout.Toggle(Language.ScanAfterCompiling, Settings.CodeAnalyzer.ScanAfterCompiling);
                Settings.CodeAnalyzer.ScanOnWindowOpen = EditorGUILayout.Toggle(Language.ScanOnWindowOpen, Settings.CodeAnalyzer.ScanOnWindowOpen);

                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(Language.ScanWords);
                if (UI.Button(Language.Edit, UI.ButtonStyle.Normal, false))
                {
                    UI.Utilities.ShowStringListPopup(codeAnalyzerScanWordsRect, Settings.CodeAnalyzer.ScanWords.ToList(),
                    (values) =>
                    {
                        Settings.CodeAnalyzer.ScanWords = values.ToArray();
                        Settings.Save();
                    });
                }
                codeAnalyzerScanWordsRect = UI.Utilities.LastRectOnRepaintRelative(codeAnalyzerScanWordsRect);
                GUILayout.EndHorizontal();

                Settings.CodeAnalyzer.ScanWordsAreCaseSensitive = EditorGUILayout.Toggle(Language.ScanWordsAreCaseSensitive, Settings.CodeAnalyzer.ScanWordsAreCaseSensitive);
                Settings.CodeAnalyzer.SelectableMessage = EditorGUILayout.Toggle(Language.SelectableMessage, Settings.CodeAnalyzer.SelectableMessage);
                Settings.CodeAnalyzer.ShowSelectFileButton = EditorGUILayout.Toggle(Language.ShowSelectFileButton, Settings.CodeAnalyzer.ShowSelectFileButton);
                Settings.CodeAnalyzer.ShowScanWordWithMessage = EditorGUILayout.Toggle(Language.ShowScanWordWithMessage, Settings.CodeAnalyzer.ShowScanWordWithMessage);

                bool tmp = Settings.CodeAnalyzer.UseThreads;
                Settings.CodeAnalyzer.UseThreads = EditorGUILayout.Toggle(Language.UseThreads, Settings.CodeAnalyzer.UseThreads);
                if (Settings.CodeAnalyzer.UseThreads != tmp)
                {
                    if (CodeAnalyzerWindow.I != null)
                    {
                        CodeAnalyzerWindow.I.Reset();
                    }
                }

                Settings.CodeAnalyzer.IncludeFileNameInSearch = EditorGUILayout.Toggle(Language.IncludeFileNameInSearch, Settings.CodeAnalyzer.IncludeFileNameInSearch);
                Settings.CodeAnalyzer.Utility = EditorGUILayout.Toggle(Language.UtilityWindow, Settings.CodeAnalyzer.Utility);
            }
            if (Foldout(Language.Others))
            {
                Settings.Other.AdaptiveMouseCursor = EditorGUILayout.Toggle(Language.AdaptiveMouseCursor, Settings.Other.AdaptiveMouseCursor);
                Settings.Other.HideUIWhenDragging = EditorGUILayout.Toggle(Language.HideUIWhenDragging, Settings.Other.HideUIWhenDragging);
                Settings.Other.Language = UI.StringDropDown(Language.Language_, Settings.Other.Language, languageNames, UI.EnumDropDownStyle.Normal, false);
                Settings.Other.ShowWindowTitleIcons = EditorGUILayout.Toggle(Language.ShowWindowTitleIcons, Settings.Other.ShowWindowTitleIcons);
                Settings.Other.SupportUndo = EditorGUILayout.Toggle(Language.SupportUndo, Settings.Other.SupportUndo);
            }

            UI.Line();
            UI.FixedSpace(5);
            if (UI.Button(new GUIContent(Language.Reset)))
            {
                if (UI.Utilities.Dialog(Language.ResetSettings, Language.AreYouSureYouWantToResetTheSettings, Language.Yes, Language.No))
                {
                    Settings.Reset();
                }
            }
            UI.Space();

            if (EditorGUI.EndChangeCheck())
                Settings.Save();
        }
    }
}
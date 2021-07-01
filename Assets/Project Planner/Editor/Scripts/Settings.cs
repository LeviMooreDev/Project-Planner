using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectPlanner
{
    public static class Settings
    {
        private static bool needLoading = true;
        public enum ColorIcon
        {
            Circle, Diamond, Box
        }
        public enum GroupFlow
        {
            Horizontal, Vertical
        }
        public enum GroupSize
        {
            Fit, Fixed, Fill
        }
        public enum PlaceNewGroup
        {
            First, Last
        }
        public enum TreeRowStyle
        {
            Box1, Box2, Flat
        }
        public enum TreeRowColoring
        {
            Same, Alternating, ByType
        }

        private static Dictionary<string, object> settings;
        private static readonly Dictionary<string, object> defaultSettings = new Dictionary<string, object>()
        {
            //board window
            {"boardWindow.allowMultipleWindows", false},
            {"boardWindow.boardNameInWindowTab", false},
            {"boardWindow.minimumHeight", 400},
            {"boardWindow.minimumWidth", 600},
            {"boardWindow.utility", false},

            {"boardWindow.group.dragThreshold", 25},
            {"boardWindow.group.fixedHeight", 300},
            {"boardWindow.group.fixedWidth", 200},
            {"boardWindow.group.grid", false},
            {"boardWindow.group.hideEmptyWhenSearching", false},
            {"boardWindow.group.placeNew", PlaceNewGroup.Last},
            {"boardWindow.group.height", GroupSize.Fit},

            {"boardWindow.task.descriptionIndicator", "..."},
            {"boardWindow.task.dragThreshold", 20},
            {"boardWindow.task.minHeight", 25},
            {"boardWindow.task.showDescriptionIndicator", true},

            //task window
            {"taskWindow.autoDockWithBoard", false},
            {"taskWindow.descriptionFixedHeightOn", true},
            {"taskWindow.descriptionFixedHeightValue", 15},
            {"taskWindow.foldoutAnimation", true},
            {"taskWindow.minimumHeight", 400},
            {"taskWindow.minimumWidth", 300},
            {"taskWindow.showTimestamps", false},
            {"taskWindow.utility", false},

            //tree window
            {"treeWindow.group.dragThreshold", 25},
            {"treeWindow.group.hideEmptyWhenSearching", true},

            {"treeWindow.task.descriptionIndicator", "..."},
            {"treeWindow.task.dragThreshold", 20},
            {"treeWindow.task.showDescriptionIndicator", true},

            {"treeWindow.rowColoring", TreeRowColoring.ByType},
            {"treeWindow.rowColoring.alternating.even", new Color(1, 1, 1, 1)},
            {"treeWindow.rowColoring.alternating.odd", new Color(.5f, .5f, .5f, 1)},
            {"treeWindow.rowColoring.byType.board", new Color(1, 1, 1, 1)},
            {"treeWindow.rowColoring.byType.group", new Color(.5f, .5f, .5f, 1)},
            {"treeWindow.rowColoring.byType.task", new Color(.2f, .2f, .2f, 1)},
            {"treeWindow.rowColoring.byType.subtask", new Color(.2f, .2f, .2f, 1)},
            {"treeWindow.rowColoring.same", new Color(1, 1, 1, 1)},
            {"treeWindow.rowStyle", TreeRowStyle.Box1},

            //board general
            {"boardGeneral.defaultName", "Board"},
            {"boardGeneral.defaultGroups", new string[] { "Planned", "In Progress", "Testing", "Completed" } },

            //group general
            {"group.defaultName", "Group"},

            //task general
            {"task.autoSelectNew", true},
            {"task.defaultName", "Task"},

            //tags
            {"tags.enabled", true},
            {"tags.confirmRemoval", false},
            {"tags.showByDefault", false},

            //colors
            {"colors.enabled", true},
            {"colors.confirmRemoval", false},
            {"colors.iconType", ColorIcon.Box},
            {"colors.showByDefault", false},

            //locking
            {"lock.enabled", true},
            {"lock.color", new Color(0, 0, 0, .5f)},
            {"lock.showIndicator", true},
            {"lock.subtaskColor", new Color(0.4f, 0.4f, 0.4f, .5f)},

            //subtasks
            {"subtasks.enabled", true},
            {"subtasks.showByDefault", true},
            {"subtasks.showIndividualProgress", true},
            {"subtasks.showProgressBar", true},
            {"subtasks.showProgressInBoardWindow", true},

            {"subtasks.descriptionIndicator", "..."},
            {"subtasks.dragThreshold", 25},
            {"subtasks.showDescriptionIndicator", true},

            //assets
            {"assets.enabled", true},
            {"assets.confirmRemoval", false},
            {"assets.showByDefault", false},

            //screenshots
            {"screenshots.enabled", true},
            {"screenshots.autoViewNew", false},
            {"screenshots.backgroundColor", new Color(0, 0, 0, 1)},
            {"screenshots.confirmRemoval", false},
            {"screenshots.defaultName", "Screenshot"},
            {"screenshots.showByDefault", false},
            {"screenshots.utilityWindow", false},

            //code analyzer
            {"codeAnalyzer.doubleClickToFind", true},
            {"codeAnalyzer.findNotImplementedException", true},
            {"codeAnalyzer.foldoutOpenByDefault", true},
            {"codeAnalyzer.hideFiles", true},
            {"codeAnalyzer.hideFilesNames", new string[] { "" } },
            {"codeAnalyzer.scanAfterCompiling", true},
            {"codeAnalyzer.scanOnWindowOpen", true},
            {"codeAnalyzer.scanWords", new string[] { "TODO" } },
            {"codeAnalyzer.scanWordsAreCaseSensitive", false},
            {"codeAnalyzer.selectableMessage", true},
            {"codeAnalyzer.showSelectFileButton", false},
            {"codeAnalyzer.showScanWordWithMessage", false},
            {"codeAnalyzer.useThreads", true},
            {"codeAnalyzer.includeFileNameInSearch", true},
            {"codeAnalyzer.utility", false},
            
            //other
            {"other.adaptiveMouseCursor", true},
            {"other.hideUIWhenDragging", true},
            {"other.language", "English"},
            {"other.showWindowTitleIcons", true},
            {"other.supportUndo", true},
        };

        public static class BoardWindow
        {
            public static bool AllowMultipleWindows
            {
                get
                {
                    return (bool)GetValue("boardWindow.allowMultipleWindows");
                }
                set
                {
                    settings["boardWindow.allowMultipleWindows"] = value;
                }
            }
            public static bool BoardNameInWindowTab
            {
                get
                {
                    return (bool)GetValue("boardWindow.boardNameInWindowTab");
                }
                set
                {
                    settings["boardWindow.boardNameInWindowTab"] = value;
                }
            }
            public static int MinimumHeight
            {
                get
                {
                    return (int)GetValue("boardWindow.minimumHeight");
                }
                set
                {
                    settings["boardWindow.minimumHeight"] = value;
                }
            }
            public static int MinimumWidth
            {
                get
                {
                    return (int)GetValue("boardWindow.minimumWidth");
                }
                set
                {
                    settings["boardWindow.minimumWidth"] = value;
                }
            }
            public static bool Utility
            {
                get
                {
                    return (bool)GetValue("boardWindow.utility");
                }
                set
                {
                    settings["boardWindow.utility"] = value;
                }
            }

            public static class Group
            {
                public static int DragThreshold
                {
                    get
                    {
                        return (int)GetValue("boardWindow.group.dragThreshold");
                    }
                    set
                    {
                        settings["boardWindow.group.dragThreshold"] = value;
                    }
                }
                public static int FixedHeight
                {
                    get
                    {
                        return (int)GetValue("boardWindow.group.fixedHeight");
                    }
                    set
                    {
                        settings["boardWindow.group.fixedHeight"] = value;
                    }
                }
                public static int FixedWidth
                {
                    get
                    {
                        return (int)GetValue("boardWindow.group.fixedWidth");
                    }
                    set
                    {
                        settings["boardWindow.group.fixedWidth"] = value;
                    }
                }
                public static bool Grid
                {
                    get
                    {
                        return (bool)GetValue("boardWindow.group.grid");
                    }
                    set
                    {
                        settings["boardWindow.group.grid"] = value;
                    }
                }
                public static bool HideEmptyWhenSearching
                {
                    get
                    {
                        return (bool)GetValue("boardWindow.group.hideEmptyWhenSearching");
                    }
                    set
                    {
                        settings["boardWindow.group.hideEmptyWhenSearching"] = value;
                    }
                }
                public static PlaceNewGroup PlaceNew
                {
                    get
                    {
                        return (PlaceNewGroup)GetValue("boardWindow.group.placeNew");
                    }
                    set
                    {
                        settings["boardWindow.group.placeNew"] = value;
                    }
                }
                public static GroupSize Height
                {
                    get
                    {
                        return (GroupSize)GetValue("boardWindow.group.height");
                    }
                    set
                    {
                        settings["boardWindow.group.height"] = value;
                    }
                }
            }

            public static class Task
            {
                public static string DescriptionIndicator
                {
                    get
                    {
                        return (string)GetValue("boardWindow.task.descriptionIndicator");
                    }
                    set
                    {
                        settings["boardWindow.task.descriptionIndicator"] = value;
                    }
                }
                public static int DragThreshold
                {
                    get
                    {
                        return (int)GetValue("boardWindow.task.dragThreshold");
                    }
                    set
                    {
                        settings["boardWindow.task.dragThreshold"] = value;
                    }
                }
                public static int MinHeight
                {
                    get
                    {
                        return (int)GetValue("boardWindow.task.minHeight");
                    }
                    set
                    {
                        settings["boardWindow.task.minHeight"] = value;
                    }
                }
                public static bool ShowDescriptionIndicator
                {
                    get
                    {
                        return (bool)GetValue("boardWindow.task.showDescriptionIndicator");
                    }
                    set
                    {
                        settings["boardWindow.task.showDescriptionIndicator"] = value;
                    }
                }
            }
        }
        public static class TaskWindow
        {
            public static bool AutoDockWithBoard
            {
                get
                {
                    return (bool)GetValue("taskWindow.autoDockWithBoard");
                }
                set
                {
                    settings["taskWindow.autoDockWithBoard"] = value;
                }
            }
            public static bool DescriptionFixedHeight
            {
                get
                {
                    return (bool)GetValue("taskWindow.descriptionFixedHeightOn");
                }
                set
                {
                    settings["taskWindow.descriptionFixedHeightOn"] = value;
                }
            }
            public static int DescriptionHeight
            {
                get
                {
                    return (int)GetValue("taskWindow.descriptionFixedHeightValue");
                }
                set
                {
                    settings["taskWindow.descriptionFixedHeightValue"] = value;
                }
            }
            public static bool FoldoutAnimation
            {
                get
                {
                    return (bool)GetValue("taskWindow.foldoutAnimation");
                }
                set
                {
                    settings["taskWindow.foldoutAnimation"] = value;
                }
            }
            public static int MinimumHeight
            {
                get
                {
                    return (int)GetValue("taskWindow.minimumHeight");
                }
                set
                {
                    settings["taskWindow.minimumHeight"] = value;
                }
            }
            public static int MinimumWidth
            {
                get
                {
                    return (int)GetValue("taskWindow.minimumWidth");
                }
                set
                {
                    settings["taskWindow.minimumWidth"] = value;
                }
            }
            public static bool ShowTimestamps
            {
                get
                {
                    return (bool)GetValue("taskWindow.showTimestamps");
                }
                set
                {
                    settings["taskWindow.showTimestamps"] = value;
                }
            }
            public static bool Utility
            {
                get
                {
                    return (bool)GetValue("taskWindow.utility");
                }
                set
                {
                    settings["taskWindow.utility"] = value;
                }
            }
        }
        public static class TreeWindow
        {
            public static class Group
            {
                public static int DragThreshold
                {
                    get
                    {
                        return (int)GetValue("treeWindow.group.dragThreshold");
                    }
                    set
                    {
                        settings["treeWindow.group.dragThreshold"] = value;
                    }
                }
                public static bool HideEmptyWhenSearching
                {
                    get
                    {
                        return (bool)GetValue("treeWindow.group.hideEmptyWhenSearching");
                    }
                    set
                    {
                        settings["treeWindow.group.hideEmptyWhenSearching"] = value;
                    }
                }
            }
            public static class Task
            {
                public static string DescriptionIndicator
                {
                    get
                    {
                        return (string)GetValue("treeWindow.task.descriptionIndicator");
                    }
                    set
                    {
                        settings["treeWindow.task.descriptionIndicator"] = value;
                    }
                }
                public static int DragThreshold
                {
                    get
                    {
                        return (int)GetValue("treeWindow.task.dragThreshold");
                    }
                    set
                    {
                        settings["treeWindow.task.dragThreshold"] = value;
                    }
                }
                public static bool ShowDescriptionIndicator
                {
                    get
                    {
                        return (bool)GetValue("treeWindow.task.showDescriptionIndicator");
                    }
                    set
                    {
                        settings["treeWindow.task.showDescriptionIndicator"] = value;
                    }
                }
            }
            public static class RowColoring
            {
                public static TreeRowColoring Type
                {
                    get
                    {
                        return (TreeRowColoring)GetValue("treeWindow.rowColoring");
                    }
                    set
                    {
                        settings["treeWindow.rowColoring"] = value;
                    }
                }

                public static class Alternating
                {
                    public static Color Even
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.alternating.even");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.alternating.even"] = value;
                        }
                    }
                    public static Color Odd
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.alternating.odd");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.alternating.odd"] = value;
                        }
                    }
                }
                public static class ByType
                {
                    public static Color Board
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.byType.board");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.byType.board"] = value;
                        }
                    }
                    public static Color Group
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.byType.group");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.byType.group"] = value;
                        }
                    }
                    public static Color Task
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.byType.task");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.byType.task"] = value;
                        }
                    }
                    public static Color Subtask
                    {
                        get
                        {
                            return (Color)GetValue("treeWindow.rowColoring.byType.subtask");
                        }
                        set
                        {
                            settings["treeWindow.rowColoring.byType.subtask"] = value;
                        }
                    }
                }
                public static Color Same
                {
                    get
                    {
                        return (Color)GetValue("treeWindow.rowColoring.same");
                    }
                    set
                    {
                        settings["treeWindow.rowColoring.same"] = value;
                    }
                }
                public static TreeRowStyle RowStyle
                {
                    get
                    {
                        return (TreeRowStyle)GetValue("treeWindow.rowStyle");
                    }
                    set
                    {
                        settings["treeWindow.rowStyle"] = value;
                    }
                }
            }
        }
        public static class BoardGeneral
        {
            public static string DefaultName
            {
                get
                {
                    return (string)GetValue("boardGeneral.defaultName");
                }
                set
                {
                    settings["boardGeneral.defaultName"] = value;
                }
            }
            public static string[] DefaultGroups
            {
                get
                {
                    return (string[])GetValue("boardGeneral.defaultGroups");
                }
                set
                {
                    settings["boardGeneral.defaultGroups"] = value;
                }
            }
        }
        public static class GroupGeneral
        {
            public static string DefaultName
            {
                get
                {
                    return (string)GetValue("group.defaultName");
                }
                set
                {
                    settings["group.defaultName"] = value;
                }
            }
        }
        public static class TaskGeneral
        {
            public static bool AutoSelectNew
            {
                get
                {
                    return (bool)GetValue("task.autoSelectNew");
                }
                set
                {
                    settings["task.autoSelectNew"] = value;
                }
            }
            public static string DefaultName
            {
                get
                {
                    return (string)GetValue("task.defaultName");
                }
                set
                {
                    settings["task.defaultName"] = value;
                }
            }
        }
        public static class Tags
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("tags.enabled");
                }
                set
                {
                    settings["tags.enabled"] = value;
                }
            }
            public static bool ConfirmRemoval
            {
                get
                {
                    return Enabled && (bool)GetValue("tags.confirmRemoval");
                }
                set
                {
                    settings["tags.confirmRemoval"] = value;
                }
            }
            public static bool ShowByDefault
            {
                get
                {
                    return Enabled && (bool)GetValue("tags.showByDefault");
                }
                set
                {
                    settings["tags.showByDefault"] = value;
                }
            }
        }
        public static class Colors
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("colors.enabled");
                }
                set
                {
                    settings["colors.enabled"] = value;
                }
            }
            public static bool ConfirmRemoval
            {
                get
                {
                    return Enabled && (bool)GetValue("colors.confirmRemoval");
                }
                set
                {
                    settings["colors.confirmRemoval"] = value;
                }
            }
            public static ColorIcon IconType
            {
                get
                {
                    return (ColorIcon)GetValue("colors.iconType");
                }
                set
                {
                    settings["colors.iconType"] = value;
                }
            }
            public static bool ShowByDefault
            {
                get
                {
                    return Enabled && (bool)GetValue("colors.showByDefault");
                }
                set
                {
                    settings["colors.showByDefault"] = value;
                }
            }
        }
        public static class Subtasks
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("subtasks.enabled");
                }
                set
                {
                    settings["subtasks.enabled"] = value;
                }
            }
            public static string DescriptionIndicator
            {
                get
                {
                    return (string)GetValue("subtasks.descriptionIndicator");
                }
                set
                {
                    settings["subtasks.descriptionIndicator"] = value;
                }
            }
            public static int DragThreshold
            {
                get
                {
                    return (int)GetValue("subtasks.dragThreshold");
                }
                set
                {
                    settings["subtasks.dragThreshold"] = value;
                }
            }
            public static bool ShowByDefault
            {
                get
                {
                    return Enabled && (bool)GetValue("subtasks.showByDefault");
                }
                set
                {
                    settings["subtasks.showByDefault"] = value;
                }
            }
            public static bool ShowDescriptionIndicator
            {
                get
                {
                    return (bool)GetValue("subtasks.showDescriptionIndicator");
                }
                set
                {
                    settings["subtasks.showDescriptionIndicator"] = value;
                }
            }
            public static bool ShowIndividualProgress
            {
                get
                {
                    return Enabled && (bool)GetValue("subtasks.showIndividualProgress");
                }
                set
                {
                    settings["subtasks.showIndividualProgress"] = value;
                }
            }
            public static bool ShowProgressBar
            {
                get
                {
                    return Enabled && (bool)GetValue("subtasks.showProgressBar");
                }
                set
                {
                    settings["subtasks.showProgressBar"] = value;
                }
            }
            public static bool ShowProgressInBoardWindow
            {
                get
                {
                    return Enabled && (bool)GetValue("subtasks.showProgressInBoardWindow");
                }
                set
                {
                    settings["subtasks.showProgressInBoardWindow"] = value;
                }
            }
        }
        public static class Assets
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("assets.enabled");
                }
                set
                {
                    settings["assets.enabled"] = value;
                }
            }
            public static bool ConfirmRemoval
            {
                get
                {
                    return Enabled && (bool)GetValue("assets.confirmRemoval");
                }
                set
                {
                    settings["assets.confirmRemoval"] = value;
                }
            }
            public static bool ShowByDefault
            {
                get
                {
                    return Enabled && (bool)GetValue("assets.showByDefault");
                }
                set
                {
                    settings["assets.showByDefault"] = value;
                }
            }
        }
        public static class Screenshots
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("screenshots.enabled");
                }
                set
                {
                    settings["screenshots.enabled"] = value;
                }
            }
            public static bool AutoViewNew
            {
                get
                {
                    return (bool)GetValue("screenshots.autoViewNew");
                }
                set
                {
                    settings["screenshots.autoViewNew"] = value;
                }
            }
            public static Color BackgroundColor
            {
                get
                {
                    return (Color)GetValue("screenshots.backgroundColor");
                }
                set
                {
                    settings["screenshots.backgroundColor"] = value;
                }
            }
            public static bool ConfirmRemoval
            {
                get
                {
                    return Enabled && (bool)GetValue("screenshots.confirmRemoval");
                }
                set
                {
                    settings["screenshots.confirmRemoval"] = value;
                }
            }
            public static string DefaultName
            {
                get
                {
                    return (string)GetValue("screenshots.defaultName");
                }
                set
                {
                    settings["screenshots.defaultName"] = value;
                }
            }
            public static bool ShowByDefault
            {
                get
                {
                    return Enabled && (bool)GetValue("screenshots.showByDefault");
                }
                set
                {
                    settings["screenshots.showByDefault"] = value;
                }
            }
            public static bool UtilityWindow
            {
                get
                {
                    return (bool)GetValue("screenshots.utilityWindow");
                }
                set
                {
                    settings["screenshots.utilityWindow"] = value;
                }
            }
        }
        public static class Locking
        {
            public static bool Enabled
            {
                get
                {
                    return (bool)GetValue("lock.enabled");
                }
                set
                {
                    settings["lock.enabled"] = value;
                }
            }
            public static Color Color
            {
                get
                {
                    return (Color)GetValue("lock.color");
                }
                set
                {
                    settings["lock.color"] = value;
                }
            }
            public static bool ShowIndicator
            {
                get
                {
                    return Enabled && (bool)GetValue("lock.showIndicator");
                }
                set
                {
                    settings["lock.showIndicator"] = value;
                }
            }

            public static Color SubtaskColor
            {
                get
                {
                    return (Color)GetValue("lock.subtaskColor");
                }
                set
                {
                    settings["lock.subtaskColor"] = value;
                }
            }
        }
        public static class CodeAnalyzer
        {
            public static bool DoubleClickToFind
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.doubleClickToFind");
                }
                set
                {
                    settings["codeAnalyzer.doubleClickToFind"] = value;
                }
            }
            public static bool FoldoutOpenByDefault
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.foldoutOpenByDefault");
                }
                set
                {
                    settings["codeAnalyzer.foldoutOpenByDefault"] = value;
                }
            }
            public static bool FindNotImplementedException
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.findNotImplementedException");
                }
                set
                {
                    settings["codeAnalyzer.findNotImplementedException"] = value;
                }
            }
            public static bool HideFiles
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.hideFiles");
                }
                set
                {
                    settings["codeAnalyzer.hideFiles"] = value;
                }
            }
            public static string[] ScanWords
            {
                get
                {
                    return (string[])GetValue("codeAnalyzer.scanWords");
                }
                set
                {
                    settings["codeAnalyzer.scanWords"] = value;
                }
            }
            public static string[] HideFilesNames
            {
                get
                {
                    return (string[])GetValue("codeAnalyzer.hideFilesNames");
                }
                set
                {
                    settings["codeAnalyzer.hideFilesNames"] = value;
                }
            }
            public static bool ScanWordsAreCaseSensitive
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.scanWordsAreCaseSensitive");
                }
                set
                {
                    settings["codeAnalyzer.scanWordsAreCaseSensitive"] = value;
                }
            }
            public static bool SelectableMessage
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.selectableMessage");
                }
                set
                {
                    settings["codeAnalyzer.selectableMessage"] = value;
                }
            }
            public static bool ShowSelectFileButton
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.showSelectFileButton");
                }
                set
                {
                    settings["codeAnalyzer.showSelectFileButton"] = value;
                }
            }
            public static bool ShowScanWordWithMessage
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.showScanWordWithMessage");
                }
                set
                {
                    settings["codeAnalyzer.showScanWordWithMessage"] = value;
                }
            }
            public static bool UseThreads
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.useThreads");
                }
                set
                {
                    settings["codeAnalyzer.useThreads"] = value;
                }
            }
            public static bool ScanAfterCompiling
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.scanAfterCompiling");
                }
                set
                {
                    settings["codeAnalyzer.scanAfterCompiling"] = value;
                }
            }
            public static bool ScanOnWindowOpen
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.scanOnWindowOpen");
                }
                set
                {
                    settings["codeAnalyzer.scanOnWindowOpen"] = value;
                }
            }
            public static bool IncludeFileNameInSearch
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.includeFileNameInSearch");
                }
                set
                {
                    settings["codeAnalyzer.includeFileNameInSearch"] = value;
                }
            }
            public static bool Utility
            {
                get
                {
                    return (bool)GetValue("codeAnalyzer.utility");
                }
                set
                {
                    settings["codeAnalyzer.utility"] = value;
                }
            }
        }
        public static class Other
        {
            public static bool AdaptiveMouseCursor
            {
                get
                {
                    return (bool)GetValue("other.adaptiveMouseCursor");
                }
                set
                {
                    settings["other.adaptiveMouseCursor"] = value;
                }
            }
            public static bool HideUIWhenDragging
            {
                get
                {
                    return (bool)GetValue("other.hideUIWhenDragging");
                }
                set
                {
                    settings["other.hideUIWhenDragging"] = value;
                }
            }
            public static string Language
            {
                get
                {
                    return (string)GetValue("other.language");
                }
                set
                {
                    settings["other.language"] = value;
                }
            }
            public static bool ShowWindowTitleIcons
            {
                get
                {
                    return (bool)GetValue("other.showWindowTitleIcons");
                }
                set
                {
                    settings["other.showWindowTitleIcons"] = value;
                }
            }
            public static bool SupportUndo
            {
                get
                {
                    return (bool)GetValue("other.supportUndo");
                }
                set
                {
                    settings["other.supportUndo"] = value;
                }
            }
        }

        private static object GetValue(string name)
        {
            if (needLoading)
            {
                needLoading = false;
                Load();
            }
            
            return settings[name];
        }
        private static void Load()
        {
            settings = new Dictionary<string, object>(defaultSettings);

            foreach (var item in defaultSettings)
            {
                if (Prefs.HasKey(item.Key))
                {
                    if (item.Value.GetType() == typeof(bool))
                    {
                        settings[item.Key] = Prefs.GetBool(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(int))
                    {
                        settings[item.Key] = Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(string))
                    {
                        settings[item.Key] = Prefs.GetString(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(Color))
                    {
                        Color color = Color.white;
                        string[] data = Prefs.GetString(item.Key).Split(',');
                        if (data.Length == 4)
                        {
                            color.r = float.Parse(data[0]);
                            color.g = float.Parse(data[1]);
                            color.b = float.Parse(data[2]);
                            color.a = float.Parse(data[3]);
                        }
                        settings[item.Key] = color;
                    }
                    else if (item.Value.GetType() == typeof(string[]))
                    {
                        string data = Prefs.GetString(item.Key);
                        if (!string.IsNullOrEmpty(data) && data != "")
                        {
                            settings[item.Key] = data.Split('~');
                        }
                        else
                        {
                            settings[item.Key] = new string[0];
                        }

                    }
                    else if (item.Value.GetType() == typeof(ColorIcon))
                    {
                        settings[item.Key] = (ColorIcon)Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(GroupFlow))
                    {
                        settings[item.Key] = (GroupFlow)Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(GroupSize))
                    {
                        settings[item.Key] = (GroupSize)Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(PlaceNewGroup))
                    {
                        settings[item.Key] = (PlaceNewGroup)Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(TreeRowStyle))
                    {
                        settings[item.Key] = (TreeRowStyle)Prefs.GetInt(item.Key);
                    }
                    else if (item.Value.GetType() == typeof(TreeRowColoring))
                    {
                        settings[item.Key] = (TreeRowColoring)Prefs.GetInt(item.Key);
                    }
                }
            }
        }
        public static void Save()
        {
            foreach (var item in settings)
            {
                if (item.Value.GetType() == typeof(bool))
                {
                    Prefs.SetBool(item.Key, (bool)item.Value);
                }
                else if (item.Value.GetType() == typeof(int))
                {
                    Prefs.SetInt(item.Key, (int)item.Value);
                }
                else if (item.Value.GetType() == typeof(string))
                {
                    Prefs.SetString(item.Key, (string)item.Value);
                }
                else if (item.Value.GetType() == typeof(Color))
                {
                    Color color = (Color)item.Value;
                    Prefs.SetString(item.Key, color.r + "," + color.g + "," + color.b + "," + color.a);
                }
                else if (item.Value.GetType() == typeof(string[]))
                {
                    Prefs.SetString(item.Key, System.String.Join("~", (string[])item.Value));
                }
                else if (item.Value.GetType().IsEnum)
                {
                    Prefs.SetInt(item.Key, (int)item.Value);
                }
            }
        }
        public static void Reset()
        {
            settings = new Dictionary<string, object>(defaultSettings);
            Save();
        }

        public static Dictionary<string, object> All()
        {
            return settings;
        }
    }
}
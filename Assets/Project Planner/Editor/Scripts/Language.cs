using System;
using UnityEngine;

namespace ProjectPlanner
{
    [CreateAssetMenu(fileName = "Language", menuName = "Project Planner/Language", order = 99)]
    public class Language : ScriptableObject
    {
        private static Language Current
        {
            get
            {
                Language language = FileManager.Language.Get(ProjectPlanner.Settings.Other.Language);
                if (language == null)
                {
                    ProjectPlanner.Settings.Other.Language = "English";
                    language = FileManager.Language.Default();
                }

                return language;
            }
        }

#pragma warning disable IDE0044 // Add readonly modifier
        [SerializeField]
        [TextArea(2, 2)]
        private string _about = "About";
        public static string About
        {
            get
            {
                return Current._about;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _adaptiveMouseCursor = "Adaptive mouse cursor";
        public static string AdaptiveMouseCursor
        {
            get
            {
                return Current._adaptiveMouseCursor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _add = "Add";
        public static string Add
        {
            get
            {
                return Current._add;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _addAndClose = "Add and close";
        public static string AddAndClose
        {
            get
            {
                return Current._addAndClose;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _all = "All";
        public static string All
        {
            get
            {
                return Current._all;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _allowMultipleWindows = "Allow multiple windows";
        public static string AllowMultipleWindows
        {
            get
            {
                return Current._allowMultipleWindows;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _alphabetically = "Alphabetically";
        public static string Alphabetically
        {
            get
            {
                return Current._alphabetically;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _archived = "Archived";
        public static string Archived
        {
            get
            {
                return Current._archived;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToDeleteTheBoard = "Are you sure you want to delete the board?";
        public static string AreYouSureYouWantToDeleteTheBoard
        {
            get
            {
                return Current._areYouSureYouWantToDeleteTheBoard;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToDeleteTheColor = "Are you sure you want to delete the color?";
        public static string AreYouSureYouWantToDeleteTheColor
        {
            get
            {
                return Current._areYouSureYouWantToDeleteTheColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToDeleteTheGroup = "Are you sure you want to delete the group?";
        public static string AreYouSureYouWantToDeleteTheGroup
        {
            get
            {
                return Current._areYouSureYouWantToDeleteTheGroup;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToRemoveTheScreenshot = "Are you sure you want to remove the screenshot?";
        public static string AreYouSureYouWantToRemoveTheScreenshot
        {
            get
            {
                return Current._areYouSureYouWantToRemoveTheScreenshot;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToDeleteTheTag = "Are you sure you want to delete the tag?";
        public static string AreYouSureYouWantToDeleteTheTag
        {
            get
            {
                return Current._areYouSureYouWantToDeleteTheTag;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToDeleteTheTask = "Are you sure you want to delete the task?";
        public static string AreYouSureYouWantToDeleteTheTask
        {
            get
            {
                return Current._areYouSureYouWantToDeleteTheTask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToImportTheDemoContentNoExistingDataWillBeOverwritten = "Are you sure you want to import the demo content? No existing data will be overwritten.";
        public static string AreYouSureYouWantToImportTheDemoContentNoExistingDataWillBeOverwritten
        {
            get
            {
                return Current._areYouSureYouWantToImportTheDemoContentNoExistingDataWillBeOverwritten;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToRemoveTheAsset = "Are you sure you want to remove the asset?";
        public static string AreYouSureYouWantToRemoveTheAsset
        {
            get
            {
                return Current._areYouSureYouWantToRemoveTheAsset;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _areYouSureYouWantToResetTheSettings = "Are you sure you want to reset the settings?";
        public static string AreYouSureYouWantToResetTheSettings
        {
            get
            {
                return Current._areYouSureYouWantToResetTheSettings;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _ASC = "ASC";
        public static string ASC
        {
            get
            {
                return Current._ASC;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _assets = "Assets";
        public static string Assets
        {
            get
            {
                return Current._assets;
            }
        }


        [SerializeField]
        [TextArea(2, 2)]
        private string _assigningAssets = "Assigning assets";
        public static string AssigningAssets
        {
            get
            {
                return Current._assigningAssets;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _autoDockToBoard = "Auto dock with board";
        public static string AutoDockWithBoard
        {
            get
            {
                return Current._autoDockToBoard;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _autoSelectNew = "Auto select new";
        public static string AutoSelectNew
        {
            get
            {
                return Current._autoSelectNew;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _autoViewNew = "Auto view new";
        public static string AutoViewNew
        {
            get
            {
                return Current._autoViewNew;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _back = "Back";
        public static string Back
        {
            get
            {
                return Current._back;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _backgroundColor = "Background color";
        public static string BackgroundColor
        {
            get
            {
                return Current._backgroundColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _board = "Board";
        public static string Board
        {
            get
            {
                return Current._board;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _boardGeneral = "Board general";
        public static string BoardGeneral
        {
            get
            {
                return Current._boardGeneral;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _boardNameInWindowTab = "Board name in window tab";
        public static string BoardNameInWindowTab
        {
            get
            {
                return Current._boardNameInWindowTab;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _boardWindow = "Board window";
        public static string BoardWindow
        {
            get
            {
                return Current._boardWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _cancel = "Cancel";
        public static string Cancel
        {
            get
            {
                return Current._cancel;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _codeAnalyzer = "Code analyzer";
        public static string CodeAnalyzer
        {
            get
            {
                return Current._codeAnalyzer;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _color = "Color";
        public static string Color
        {
            get
            {
                return Current._color;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _coloring = "Coloring";
        public static string Coloring
        {
            get
            {
                return Current._coloring;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _colors = "Colors";
        public static string Colors
        {
            get
            {
                return Current._colors;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _confirmRemoval = "Confirm removal";
        public static string ConfirmRemoval
        {
            get
            {
                return Current._confirmRemoval;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _connect = "Connect";
        public static string Connect
        {
            get
            {
                return Current._connect;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _contactMe = "Contact me";
        public static string ContactMe
        {
            get
            {
                return Current._contactMe;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _copy = "Copy";
        public static string Copy
        {
            get
            {
                return Current._copy;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _created = "Created";
        public static string Created
        {
            get
            {
                return Current._created;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _defaultName = "Default name";
        public static string DefaultName
        {
            get
            {
                return Current._defaultName;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _defaultGroups = "Default groups";
        public static string DefaultGroups
        {
            get
            {
                return Current._defaultGroups;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _delete = "Delete";
        public static string Delete
        {
            get
            {
                return Current._delete;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _deleteGroup = "Delete group";
        public static string DeleteGroup
        {
            get
            {
                return Current._deleteGroup;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _deleteTask = "Delete task";
        public static string DeleteTask
        {
            get
            {
                return Current._deleteTask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _demoContent = "Demo content";
        public static string DemoContent
        {
            get
            {
                return Current._demoContent;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _demoContentWasImportedSuccessfully = "Demo content was imported successfully";
        public static string DemoContentWasImportedSuccessfully
        {
            get
            {
                return Current._demoContentWasImportedSuccessfully;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _DESC = "DESC";
        public static string DESC
        {
            get
            {
                return Current._DESC;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _description = "Description";
        public static string Description
        {
            get
            {
                return Current._description;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _descriptionFixedHeight = "Description fixed height";
        public static string DescriptionFixedHeight
        {
            get
            {
                return Current._descriptionFixedHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _descriptionHeight = "Description height";
        public static string DescriptionHeight
        {
            get
            {
                return Current._descriptionHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _descriptionIndicator = "Description indicator";
        public static string DescriptionIndicator
        {
            get
            {
                return Current._descriptionIndicator;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _disconnect = "Disconnect";
        public static string Disconnect
        {
            get
            {
                return Current._disconnect;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _dontOpenAutomaticallyWhenUpdated = "Don't open automatically when updated";
        public static string DontOpenAutomaticallyWhenUpdated
        {
            get
            {
                return Current._dontOpenAutomaticallyWhenUpdated;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _doubleClickToFind = "Double click to find";
        public static string DoubleClickToFind
        {
            get
            {
                return Current._doubleClickToFind;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _download = "Download";
        public static string Download
        {
            get
            {
                return Current._download;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _DragThreshold = "Drag threshold";
        public static string DragThreshold
        {
            get
            {
                return Current._DragThreshold;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _duplicate = "Duplicate";
        public static string Duplicate
        {
            get
            {
                return Current._duplicate;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _edit = "Edit";
        public static string Edit
        {
            get
            {
                return Current._edit;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _empty = "Empty";
        public static string Empty
        {
            get
            {
                return Current._empty;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _enabled = "Enabled";
        public static string Enabled
        {
            get
            {
                return Current._enabled;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _export = "Export";
        public static string Export
        {
            get
            {
                return Current._export;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _exportToGloBoards = "Export to Glo Boards";
        public static string ExportToGloBoards
        {
            get
            {
                return Current._exportToGloBoards;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _fixedHeight = "Fixed height";
        public static string FixedHeight
        {
            get
            {
                return Current._fixedHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _fixedWidth = "Fixed width";
        public static string FixedWidth
        {
            get
            {
                return Current._fixedWidth;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _foldoutAnimation = "Foldout animation";
        public static string FoldoutAnimation
        {
            get
            {
                return Current._foldoutAnimation;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _foldoutOpenByDefault = "Foldout open by default";
        public static string FoldoutOpenByDefault
        {
            get
            {
                return Current._foldoutOpenByDefault;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _forceCloseProjectPlanner = "Force close project planner";
        public static string ForceCloseProjectPlanner
        {
            get
            {
                return Current._forceCloseProjectPlanner;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _goTo = "Go to";
        public static string GoTo
        {
            get
            {
                return Current._goTo;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _grid = "Grid";
        public static string Grid
        {
            get
            {
                return Current._grid;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _group = "Group";
        public static string Group
        {
            get
            {
                return Current._group;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _groupGeneral = "Group general";
        public static string GroupGeneral
        {
            get
            {
                return Current._groupGeneral;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _groupLayout = "Group layout";
        public static string GroupLayout
        {
            get
            {
                return Current._groupLayout;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _height = "Height";
        public static string Height
        {
            get
            {
                return Current._height;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _hideEmptyWhenSearching = "Hide empty when searching";
        public static string HideEmptyWhenSearching
        {
            get
            {
                return Current._hideEmptyWhenSearching;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _hideFiles = "Hide files";
        public static string HideFiles
        {
            get
            {
                return Current._hideFiles;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _hideUIWhenDragging = "Hide UI when dragging";
        public static string HideUIWhenDragging
        {
            get
            {
                return Current._hideUIWhenDragging;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _iconType = "Icon type";
        public static string IconType
        {
            get
            {
                return Current._iconType;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _identityCode = "Identity code";
        public static string IdentityCode
        {
            get
            {
                return Current._identityCode;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _import = "Import";
        public static string Import
        {
            get
            {
                return Current._import;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _includeFileNameInSearch = "Include file name in search";
        public static string IncludeFileNameInSearch
        {
            get
            {
                return Current._includeFileNameInSearch;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _importCompleted = "Import completed";
        public static string ImportCompleted
        {
            get
            {
                return Current._importCompleted;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _importDemoContent = "Import demo content";
        public static string ImportDemoContent
        {
            get
            {
                return Current._importDemoContent;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _language = "Language";
        public static string Language_
        {
            get
            {
                return Current._language;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _layout = "Layout";
        public static string Layout
        {
            get
            {
                return Current._layout;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _line = "Line";
        public static string Line
        {
            get
            {
                return Current._line;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lock = "Lock";
        public static string Lock
        {
            get
            {
                return Current._lock;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lockAll = "Lock all";
        public static string LockAll
        {
            get
            {
                return Current._lockAll;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lockColor = "Lock color";
        public static string LockColor
        {
            get
            {
                return Current._lockColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _locked = "Locked";
        public static string Locked
        {
            get
            {
                return Current._locked;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lockIncludingSubtasks = "Lock including subtasks";
        public static string LockIncludingSubtasks
        {
            get
            {
                return Current._lockIncludingSubtasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _locking = "Locking";
        public static string Locking
        {
            get
            {
                return Current._locking;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lockingTasks = "Locking tasks";
        public static string LockingTasks
        {
            get
            {
                return Current._lockingTasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _lockTasks = "Lock tasks";
        public static string LockTasks
        {
            get
            {
                return Current._lockTasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _makeTask = "Make task";
        public static string MakeTask
        {
            get
            {
                return Current._makeTask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _manual = "Manual";
        public static string Manual
        {
            get
            {
                return Current._manual;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _minHeight = "Min height";
        public static string MinHeight
        {
            get
            {
                return Current._minHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _minimumHeight = "Minimum height";
        public static string MinimumHeight
        {
            get
            {
                return Current._minimumHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _minimumWidth = "Minimum width";
        public static string MinimumWidth
        {
            get
            {
                return Current._minimumWidth;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveDown = "Move down";
        public static string MoveDown
        {
            get
            {
                return Current._moveDown;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveLeft = "Move left";
        public static string MoveLeft
        {
            get
            {
                return Current._moveLeft;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveRight = "Move right";
        public static string MoveRight
        {
            get
            {
                return Current._moveRight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveToBoard = "Move to board";
        public static string MoveToBoard
        {
            get
            {
                return Current._moveToBoard;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveToGroup = "Move to group";
        public static string MoveToGroup
        {
            get
            {
                return Current._moveToGroup;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _moveUp = "Move up";
        public static string MoveUp
        {
            get
            {
                return Current._moveUp;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _name = "Name";
        public static string Name
        {
            get
            {
                return Current._name;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _newBoard = "New board";
        public static string NewBoard
        {
            get
            {
                return Current._newBoard;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _newGroup = "New group";
        public static string NewGroup
        {
            get
            {
                return Current._newGroup;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _newSubtask = "New subtask";
        public static string NewSubtask
        {
            get
            {
                return Current._newSubtask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _newTask = "New task";
        public static string NewTask
        {
            get
            {
                return Current._newTask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _no = "No";
        public static string No
        {
            get
            {
                return Current._no;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _normal = "Normal";
        public static string Normal
        {
            get
            {
                return Current._normal;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _NONAME = "NO NAME";
        public static string NONAME
        {
            get
            {
                return Current._NONAME;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _none = "None";
        public static string None
        {
            get
            {
                return Current._none;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _noTaskSelected = "No task selected";
        public static string NoTaskSelected
        {
            get
            {
                return Current._noTaskSelected;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _others = "Others";
        public static string Others
        {
            get
            {
                return Current._others;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _ok = "OK";
        public static string OK
        {
            get
            {
                return Current._ok;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _open = "Open";
        public static string Open
        {
            get
            {
                return Current._open;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _openManual = "Open manual";
        public static string OpenManual
        {
            get
            {
                return Current._openManual;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _openSettings = "Open settings";
        public static string OpenSettings
        {
            get
            {
                return Current._openSettings;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _password = "Password";
        public static string Password
        {
            get
            {
                return Current._password;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _pasteAsSubtask = "Paste as subtask";
        public static string PasteAsSubtask
        {
            get
            {
                return Current._pasteAsSubtask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _pasteAsNew = "Paste as new";
        public static string PasteAsNew
        {
            get
            {
                return Current._pasteAsNew;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _pasteGroup = "Paste group";
        public static string PasteGroup
        {
            get
            {
                return Current._pasteGroup;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _pasteTask = "Paste task";
        public static string PasteTask
        {
            get
            {
                return Current._pasteTask;
            }
        }

        [TextArea(2, 2)]
        private string _pasteValues = "Paste values";
        public static string PasteValues
        {
            get
            {
                return Current._pasteValues;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _placeNew = "Place new";
        public static string PlaceNew
        {
            get
            {
                return Current._placeNew;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _prefix = "Prefix";
        public static string Prefix
        {
            get
            {
                return Current._prefix;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _progress = "Progress";
        public static string Progress
        {
            get
            {
                return Current._progress;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _projectName = "Project name";
        public static string ProjectName
        {
            get
            {
                return Current._projectName;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _quickTask = "Quick task";
        public static string QuickTask
        {
            get
            {
                return Current._quickTask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _refresh = "Refresh";
        public static string Refresh
        {
            get
            {
                return Current._refresh;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _rename = "Rename";
        public static string Rename
        {
            get
            {
                return Current._rename;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _reset = "Reset";
        public static string Reset
        {
            get
            {
                return Current._reset;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _resetSettings = "Reset rettings";
        public static string ResetSettings
        {
            get
            {
                return Current._resetSettings;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _search = "Search";
        public static string Search
        {
            get
            {
                return Current._search;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _searchByColor = "Search by color";
        public static string SearchByColor
        {
            get
            {
                return Current._searchByColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _searchByTag = "Search by tag";
        public static string SearchByTag
        {
            get
            {
                return Current._searchByTag;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _settings = "Settings";
        public static string Settings
        {
            get
            {
                return Current._settings;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _sort = "Sort";
        public static string Sort
        {
            get
            {
                return Current._sort;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showByDefault = "Show by default";
        public static string ShowByDefault
        {
            get
            {
                return Current._showByDefault;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showDescriptionIndicator = "Show description indicator";
        public static string ShowDescriptionIndicator
        {
            get
            {
                return Current._showDescriptionIndicator;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showIndicator = "Show indicator";
        public static string ShowIndicator
        {
            get
            {
                return Current._showIndicator;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showIndividualProgress = "Show individual progress";
        public static string ShowIndividualProgress
        {
            get
            {
                return Current._showIndividualProgress;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showLocks = "Show locks";
        public static string ShowLocks
        {
            get
            {
                return Current._showLocks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showSelectFileButton = "Show select file button";
        public static string ShowSelectFileButton
        {
            get
            {
                return Current._showSelectFileButton;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showScanWordWithMessage = "Show scan word with message";
        public static string ShowScanWordWithMessage
        {
            get
            {
                return Current._showScanWordWithMessage;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showProgress = "Show progress";
        public static string ShowProgress
        {
            get
            {
                return Current._showProgress;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showProgressBar = "Show progress bar";
        public static string ShowProgressBar
        {
            get
            {
                return Current._showProgressBar;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showProgressInBoardWindow = "Show progress in board window";
        public static string ShowProgressInBoardWindow
        {
            get
            {
                return Current._showProgressInBoardWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showTimestamps = "Show timestamps";
        public static string ShowTimestamps
        {
            get
            {
                return Current._showTimestamps;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _showWindowTitleIcons = "Show window title icons";
        public static string ShowWindowTitleIcons
        {
            get
            {
                return Current._showWindowTitleIcons;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _scan = "Scan";
        public static string Scan
        {
            get
            {
                return Current._scan;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _scanAfterCompiling = "Scan after compiling";
        public static string ScanAfterCompiling
        {
            get
            {
                return Current._scanAfterCompiling;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _scanOnWindowOpen = "Scan on window open";
        public static string ScanOnWindowOpen
        {
            get
            {
                return Current._scanOnWindowOpen;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _scanWords = "Scan words";
        public static string ScanWords
        {
            get
            {
                return Current._scanWords;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _scanWordsAreCaseSensitive = "Scan words are case sensitive";
        public static string ScanWordsAreCaseSensitive
        {
            get
            {
                return Current._scanWordsAreCaseSensitive;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _screenshot = "Screenshot";
        public static string Screenshot
        {
            get
            {
                return Current._screenshot;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _screenshots = "Screenshots";
        public static string Screenshots
        {
            get
            {
                return Current._screenshots;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _sublockColor = "Sublock color";
        public static string SublockColor
        {
            get
            {
                return Current._sublockColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _submit = "Submit";
        public static string Submit
        {
            get
            {
                return Current._submit;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _subtasks = "Subtasks";
        public static string Subtasks
        {
            get
            {
                return Current._subtasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _subtaskColor = "Subtask color";
        public static string SubtaskColor
        {
            get
            {
                return Current._subtaskColor;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _support = "Support";
        public static string Support
        {
            get
            {
                return Current._support;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _supportUndo = "Support undo";
        public static string SupportUndo
        {
            get
            {
                return Current._supportUndo;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _tag = "Tag";
        public static string Tag
        {
            get
            {
                return Current._tag;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _tags = "Tags";
        public static string Tags
        {
            get
            {
                return Current._tags;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _task = "Task";
        public static string Task
        {
            get
            {
                return Current._task;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _taskGeneral = "Task general";
        public static string TaskGeneral
        {
            get
            {
                return Current._taskGeneral;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _taskHasAllSelected = "Task has all selected";
        public static string TaskHasAllSelected
        {
            get
            {
                return Current._taskHasAllSelected;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _taskMinHeight = "Task min height";
        public static string TaskMinHeight
        {
            get
            {
                return Current._taskMinHeight;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _taskWindow = "Task window";
        public static string TaskWindow
        {
            get
            {
                return Current._taskWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _utility = "Utility";
        public static string Utility
        {
            get
            {
                return Current._utility;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _utilityWindow = "Utility window";
        public static string UtilityWindow
        {
            get
            {
                return Current._utilityWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _updated = "Updated";
        public static string Updated
        {
            get
            {
                return Current._updated;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _upload = "Upload";
        public static string Upload
        {
            get
            {
                return Current._upload;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _unlock = "Unlock";
        public static string Unlock
        {
            get
            {
                return Current._unlock;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _unlockAll = "Unlock all";
        public static string UnlockAll
        {
            get
            {
                return Current._unlockAll;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _unlockIncludingSubtasks = "Unlock including subtasks";
        public static string UnlockIncludingSubtasks
        {
            get
            {
                return Current._unlockIncludingSubtasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _unlockTasks = "Unlock tasks";
        public static string UnlockTasks
        {
            get
            {
                return Current._unlockTasks;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _username = "Username";
        public static string Username
        {
            get
            {
                return Current._username;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _useThreads = "Use threads";
        public static string UseThreads
        {
            get
            {
                return Current._useThreads;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _view = "View";
        public static string View
        {
            get
            {
                return Current._view;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _width = "Width";
        public static string Width
        {
            get
            {
                return Current._width;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _remove = "Remove";
        public static string Remove
        {
            get
            {
                return Current._remove;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _working = "Working";
        public static string Working
        {
            get
            {
                return Current._working;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _yes = "Yes";
        public static string Yes
        {
            get
            {
                return Current._yes;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _youHaveToCreateALeastOneBoardToUseTheQuickTaskWindow = "You have to create a least one board to use the quick task window.";
        public static string YouHaveToCreateALeastOneBoardToUseTheQuickTaskWindow
        {
            get
            {
                return Current._youHaveToCreateALeastOneBoardToUseTheQuickTaskWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _youHaveToSelectABoardWithAtLeastOneGroupInItToUseTheQuickTaskWindow = "You have to select a board with at least one group in it to use the quick task window.";
        public static string YouHaveToSelectABoardWithAtLeastOneGroupInItToUseTheQuickTaskWindow
        {
            get
            {
                return Current._youHaveToSelectABoardWithAtLeastOneGroupInItToUseTheQuickTaskWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _rowColoring = "Row coloring";
        public static string RowColoring
        {
            get
            {
                return Current._rowColoring;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _even = "Even";
        public static string Even
        {
            get
            {
                return Current._even;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _odd = "Odd";
        public static string Odd
        {
            get
            {
                return Current._odd;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _subtask = "Subtask";
        public static string Subtask
        {
            get
            {
                return Current._subtask;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _rowStyle = "Row style";
        public static string RowStyle
        {
            get
            {
                return Current._rowStyle;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _treeView = "Tree view";
        public static string TreeView
        {
            get
            {
                return Current._treeView;
            }
        }
        
        [SerializeField]
        [TextArea(2, 2)]
        private string _treeWindow = "Tree window";
        public static string TreeWindow
        {
            get
            {
                return Current._treeWindow;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _find = "Find";
        public static string Find
        {
            get
            {
                return Current._find;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _findNotImplementedException = "Find not implemented exception";
        public static string FindNotImplementedException
        {
            get
            {
                return Current._findNotImplementedException;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _selectableMessage = "Selectable message";
        public static string SelectableMessage
        {
            get
            {
                return Current._selectableMessage;
            }
        }

        [SerializeField]
        [TextArea(2, 2)]
        private string _selectFile = "Select file";
        public static string SelectFile
        {
            get
            {
                return Current._selectFile;
            }
        }
#pragma warning restore IDE0044 // Add readonly modifier
    }
}
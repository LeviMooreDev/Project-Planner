using ProjectPlanner.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace ProjectPlanner
{
    public class TaskWindow : WindowBase
    {
        #region Fields
        public static TaskWindow I
        {
            get
            {
                var windows = Resources.FindObjectsOfTypeAll<TaskWindow>();
                if (windows == null || windows.Length == 0)
                    return null;
                else
                    return windows[0];
            }
        }

        //task
        private Task currentTask;

        [SerializeField]
        private string taskID;
        public string TaskID
        {
            get
            {
                return taskID;
            }
            set
            {
                taskID = value;
            }
        }

        //lock
        private bool IsLocked
        {
            get
            {
                if (currentTask == null)
                    return false;
                return currentTask.Locked;
            }
        }

        //rename
        private RenameHandler renameObj;

        //input
        private Vector2 lastMousePositionDown;

        //tags
        private AnimBool tagsFoldoutAnimation;
        private string newTag = string.Empty;

        //colors
        private AnimBool colorsFoldoutAnimation;
        private Color newColor = Color.white;

        //assets
        private AnimBool assetsFoldoutAnimation;

        //subtasks
        private AnimBool subtasksFoldoutAnimation;
        private int draggingSubtaskStartIndex;
        private Task draggingSubtask;
        private Task draggingSubtaskHold;

        //screenshots
        private AnimBool screenshotsFoldoutAnimation;

        #endregion

        #region Methods
        public static TaskWindow Init()
        {
            TaskWindow window;
            if (Settings.TaskWindow.Utility)
            {
                window = GetWindow<TaskWindow>(true, Language.Task);
            }
            else
            {
                if (Settings.TaskWindow.AutoDockWithBoard)
                {
                    window = GetWindow<TaskWindow>(Language.Task, typeof(BoardWindow));
                }
                else
                {
                    window = GetWindow<TaskWindow>(false, Language.Task);
                }
            }

            window.minSize = new Vector2(Settings.TaskWindow.MinimumWidth, Settings.TaskWindow.MinimumHeight);

            if (Prefs.GetBool("task_window_first_time_open", true))
            {
                window.CenterPosition();
                Prefs.SetBool("task_window_first_time_open", false);
            }

            return window;
        }
        public static void CloseWindow()
        {
            GetWindow<TaskWindow>(Settings.TaskWindow.Utility, Language.Task).Close();
        }

        protected override void OnEditorReload()
        {
            if (Settings.Other.ShowWindowTitleIcons)
            {
                titleContent.image = UI.Icons.TaskWindow;
            }
            else
            {
                titleContent.image = null;
            }

            draggingSubtask = null;
            draggingSubtaskHold = null;

            renameObj = new RenameHandler(MakeUpdateUISafe, Repaint);

            ResetFoldoutAnimations();
        }
        protected override void Input(Event e)
        {
            //rename
            if (!e.isMouse)
            {
                if (e.keyCode == KeyCode.Return)
                {
                    renameObj.End();
                }
                if (e.keyCode == KeyCode.Escape)
                {
                    renameObj.End(false);
                }
            }

            //dragging
            if (e.type == EventType.MouseDown)
            {
                lastMousePositionDown = UI.Utilities.MousePositionAbsolute(Position);
            }

            if (e.type == EventType.MouseUp)
            {
                MakeUpdateUISafe(() =>
                {
                    if (draggingSubtask != null)
                    {
                        int newIndex = currentTask.TasksIds.IndexOf(draggingSubtask.ID);

                        if (newIndex != draggingSubtaskStartIndex)
                        {
                            currentTask.TasksIds.RemoveAt(newIndex);
                            currentTask.TasksIds.Insert(draggingSubtaskStartIndex, draggingSubtask.ID);
                            UndoHandler.SaveState("Move Task");
                            currentTask.TasksIds.RemoveAt(draggingSubtaskStartIndex);
                            currentTask.TasksIds.Insert(newIndex, draggingSubtask.ID);
                        }

                    }

                    draggingSubtask = null;
                    draggingSubtaskHold = null;
                });
            }
            if (e.type == EventType.MouseDrag)
            {
                if (draggingSubtaskHold != null)
                {
                    if (Vector2.Distance(UI.Utilities.MousePositionAbsolute(Position), lastMousePositionDown) > Settings.Subtasks.DragThreshold)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            draggingSubtask = draggingSubtaskHold;
                            draggingSubtaskHold = null;
                            draggingSubtaskStartIndex = currentTask.TasksIds.IndexOf(draggingSubtask.ID);
                        });
                    }
                }
            }

            if (draggingSubtask != null)
            {
                if (e.type == EventType.MouseDrag)
                {
                    draggingSubtask.WasDraggedLastFrame = true;

                    foreach (var otherTaskId in currentTask.TasksIds)
                    {
                        Task task = Tasks.I.Get(otherTaskId);

                        if (task.TaskWindow_ShowSubtask)
                        {
                            if (task.TaskWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                if (task != draggingSubtask)
                                {
                                    MakeUpdateUISafe(() =>
                                    {
                                        int newIndex = currentTask.TasksIds.IndexOf(task.ID);

                                        currentTask.TasksIds.Remove(draggingSubtask.ID);
                                        currentTask.TasksIds.Insert(newIndex, draggingSubtask.ID);
                                        Save();
                                    });
                                }
                            }
                        }
                    }

                    Repaint();
                }
            }
        }
        protected override void OnDraw()
        {
            //find task by id
            if (!string.IsNullOrEmpty(taskID))
            {
                string oldId = currentTask == null ? "" : currentTask.ID;
                currentTask = Tasks.I.Get(taskID);
                if (currentTask != null)
                {
                    if (oldId != currentTask.ID)
                    {
                        ResetFoldoutAnimations();
                    }
                }
            }
            else
            {
                currentTask = null;
                Repaint();
            }

            if (currentTask != null && currentTask.IsParentATask && !Settings.Subtasks.Enabled)
            {
                taskID = "";
                currentTask = null;
            }

            //empty
            if (currentTask == null)
            {
                UI.BeginVertical();
                UI.Space(true);
                UI.Label(Language.NoTaskSelected, UI.LabelStyle.CenteredGrey);
                UI.Space(true);
                UI.EndVertical();
            }
            //draw task
            else
            {
                UI.Utilities.BeginChangeCheck();

                DrawToolbar();

                currentTask.ScrollPosition = UI.BeginScrollView(currentTask.ScrollPosition);

                UI.Space();
                DrawName();
                DrawDescription();

                if (!currentTask.IsParentATask)
                    DrawTags();
                DrawColor();
                DrawAssets();
                DrawScreenshots();
                DrawSubtasks();
                UI.Space();
                DrawTimestamps();
                UI.EndScrollView();

                if (UI.Utilities.EndChangeCheck())
                {
                    currentTask.UpdateTimestamp();
                    Save();
                }
            }
        }
        private void OnLostFocus()
        {
            //rename
            if (renameObj != null)
                renameObj.End();

            draggingSubtask = null;
        }

        //toolbar
        private void DrawToolbar()
        {
            UI.BeginToolbar();

            DrawToolbarNavigate();
            UI.Space(true);

            DrawToolbarDelete();
            DrawToolbarLock();

            UI.EndToolbar();
        }
        private void DrawToolbarNavigate()
        {
            if (currentTask.IsParentATask)
            {
                if (UI.Button(Language.Back, UI.ButtonStyle.Toolbar))
                {
                    UndoHandler.SaveState("Task Go Back");

                    MakeUpdateUISafe(() =>
                    {
                        TaskID = currentTask.ParentID;
                        Repaint();
                    });
                }
                if (currentTask.IsParentATask)
                {
                    if (UI.Button(Language.GoTo, UI.ButtonStyle.Toolbar))
                    {
                        List<Task> breadcrumbs = new List<Task>();
                        Task c = currentTask;
                        while (c.IsParentATask)
                        {
                            Task taskParent = Tasks.I.Get(c.ParentID);

                            breadcrumbs.Add(taskParent);
                            c = breadcrumbs.Last();
                        }
                        breadcrumbs.Reverse();

                        GenericMenu menu = new GenericMenu();

                        foreach (var breadcrumb in breadcrumbs)
                        {
                            menu.AddItem(new GUIContent(breadcrumb.Name), false, () =>
                            {
                                UndoHandler.SaveState("Go To Task");

                                MakeUpdateUISafe(() =>
                                {
                                    TaskID = breadcrumb.ID;

                                    Repaint();
                                });
                            });
                        }

                        menu.ShowAsContext();
                    }
                }
            }
        }
        private void DrawToolbarDelete()
        {
            if (!IsLocked)
            {
                if (!currentTask.HasLockedTask())
                {
                    if (UI.Button(Language.Delete, UI.ButtonStyle.Toolbar))
                    {
                        if (UI.Utilities.Dialog(Language.DeleteTask, Language.AreYouSureYouWantToDeleteTheTask, Language.Yes, Language.No))
                        {
                            UndoHandler.SaveState("Delete Task");

                            MakeUpdateUISafe(() =>
                            {
                                if (currentTask.IsParentATask)
                                    taskID = currentTask.ParentID;

                                Tasks.I.Delete(currentTask);
                                currentTask = null;

                                Repaint();
                            });
                        }
                    }
                }
                else
                {
                    UI.Utilities.Enabled = false;
                    UI.Button(Language.Delete, UI.ButtonStyle.Toolbar);
                    UI.Utilities.Enabled = true;
                }
            }
        }
        private void DrawToolbarLock()
        {
            if (!Settings.Locking.Enabled)
                return;

            if (UI.Button(new GUIContent(currentTask.Locked ? UI.Icons.Lock : UI.Icons.LockOpen, currentTask.Locked ? Language.Unlock : Language.Lock), UI.ButtonStyle.Toolbar))
            {
                if (Event.current.button == 0)
                {
                    if (currentTask.Locked)
                    {
                        UndoHandler.SaveState("Unlock Task");

                        MakeUpdateUISafe(() =>
                        {
                            Tasks.I.Unlock(currentTask);

                            Repaint();
                        });
                    }
                    else
                    {
                        UndoHandler.SaveState("Lock Task");
                        MakeUpdateUISafe(() =>
                        {
                            Tasks.I.Lock(currentTask);

                            Repaint();
                        });
                    }
                }
                else if (Settings.Subtasks.Enabled)
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent(Language.LockIncludingSubtasks), false, () =>
                    {
                        UndoHandler.SaveState("Lock All Tasks");

                        MakeUpdateUISafe(() =>
                        {
                            Tasks.I.LockAndSubtask(currentTask);

                            Repaint();
                        });
                    });
                    if (currentTask.HasLockedTask())
                    {
                        menu.AddItem(new GUIContent(Language.UnlockIncludingSubtasks), false, () =>
                        {
                            UndoHandler.SaveState("Unlock All Tasks");
                            MakeUpdateUISafe(() =>
                            {
                                Tasks.I.UnlockAndSubtask(currentTask);

                                Repaint();
                            });
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Language.UnlockAll));
                    }

                    menu.ShowAsContext();
                }
            }
        }

        //panels
        private void DrawName()
        {
            UI.Label(Language.Name);

            if (IsLocked)
            {
                UI.SelectableLabelWrap(string.IsNullOrEmpty(currentTask.Name) ? Language.NONAME : currentTask.Name, Size.x);
            }
            else
            {
                UI.Utilities.BeginChangeCheck();

                UI.Utilities.Name("name_" + currentTask.ID);
                string newName = UI.TextField(currentTask.Name, Language.NONAME);

                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Task Name");

                    currentTask.Name = newName;

                    Save();
                    Repaint();
                }
            }

            UI.Space();
        }
        private void DrawDescription()
        {
            if (IsLocked && string.IsNullOrEmpty(currentTask.Description))
                return;

            if (IsLocked)
            {
                UI.Label(Language.Description);
                UI.SelectableLabelWrap(currentTask.Description, Size.x);
                UI.Space();
            }
            else
            {
                UI.Label(Language.Description);

                UI.Utilities.BeginChangeCheck();

                string newDescription = currentTask.Description;
                if (Settings.TaskWindow.DescriptionFixedHeight)
                {
                    newDescription = UI.TextArea(newDescription, Language.Empty, Settings.TaskWindow.DescriptionHeight);
                }
                else
                {
                    newDescription = UI.TextArea(newDescription, Language.Empty);
                }

                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Task Description");

                    currentTask.Description = newDescription;

                    Save();
                    Repaint();
                }

                UI.Space();
            }
        }
        private void DrawTags()
        {
            if (!Settings.Tags.Enabled)
                return;

            if (IsLocked && currentTask.TagIds.Count == 0)
                return;

            bool newShow = UI.Foldout(currentTask.ShowTags, Language.Tags, () => { Repaint(); });
            if (newShow != currentTask.ShowTags)
            {
                MakeUpdateUISafe(() =>
                {
                    UndoHandler.SaveState("Toogle Show Tags");
                    currentTask.ShowTags = newShow;
                    tagsFoldoutAnimation.target = newShow;
                    if (!Settings.TaskWindow.FoldoutAnimation)
                    {
                        tagsFoldoutAnimation.value = tagsFoldoutAnimation.target;
                    }
                    Save();
                    Repaint();
                });
            }

            if (EditorGUILayout.BeginFadeGroup(tagsFoldoutAnimation.faded))
            {
                DrawTagsList();

                UI.Space();
                UI.Line();
                UI.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }
        private void DrawTagsList()
        {
            UI.FixedSpace(5);

            if (Tags.I.Values.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
            else
            {
                foreach (var tag in Tags.I.Values.ToArray())
                {
                    UI.BeginHorizontal();

                    if (IsLocked)
                    {
                        UI.FixedSpace(26);
                        UI.Label(Tags.I.Values[tag.Key]);
                    }
                    else
                    {
                        bool on = currentTask.TagIds.Contains(tag.Key);
                        UI.Utilities.BeginChangeCheck();
                        on = UI.Toggle(on);

                        if (UI.Utilities.EndChangeCheck())
                        {
                            if (on && !currentTask.TagIds.Contains(tag.Key))
                            {
                                UndoHandler.SaveState("Select Tag");
                                currentTask.TagIds.Add(tag.Key);
                                Save();
                            }
                            else if (currentTask.TagIds.Contains(tag.Key))
                            {
                                UndoHandler.SaveState("Deselect Tag");
                                currentTask.TagIds.Remove(tag.Key);
                                Save();
                            }

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

                        UI.Utilities.BeginChangeCheck();
                        string newValue = UI.TextField(Tags.I.Values[tag.Key]);

                        if (UI.Utilities.EndChangeCheck())
                        {
                            newValue = newValue.Trim();
                            if (newValue != string.Empty)
                            {
                                if (!Tags.I.Values.ContainsValue(newValue.ToLower()))
                                {
                                    UndoHandler.SaveState("Change Tag");
                                    Tags.I.Values[tag.Key] = newValue;
                                    Save();
                                }
                            }
                        }

                        if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                        {
                            if (!Settings.Tags.ConfirmRemoval || UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheTag, Language.Yes, Language.No))
                            {
                                DeleteTag(tag.Key);
                            }
                        }
                    }
                    UI.EndHorizontal();
                }
            }

            if (!IsLocked)
            {
                UI.FixedSpace(7);
                UI.BeginHorizontal();
                UI.FixedSpace(26);

                newTag = UI.TextField(newTag, Language.Tag);
                if (UI.Button(Language.Add))
                {
                    UndoHandler.SaveState("New Tag");
                    Tags.I.Add(newTag, false);
                    newTag = string.Empty;
                    newTag = newTag.Trim();

                    UI.Utilities.LossFocus();
                }

                UI.EndHorizontal();
                UI.FixedSpace(3);
            }
        }
        private void DrawColor()
        {
            if (!Settings.Colors.Enabled)
                return;

            if (IsLocked && currentTask.ColorIds.Count == 0)
                return;

            bool newShow = UI.Foldout(currentTask.ShowColors, Language.Colors, () => { Repaint(); });
            if (newShow != currentTask.ShowColors)
            {
                MakeUpdateUISafe(() =>
                {
                    UndoHandler.SaveState("Toogle Show Colors");
                    currentTask.ShowColors = newShow;
                    colorsFoldoutAnimation.target = newShow;
                    if (!Settings.TaskWindow.FoldoutAnimation)
                    {
                        colorsFoldoutAnimation.value = colorsFoldoutAnimation.target;
                    }
                    Save();
                    Repaint();
                });
            }

            if (EditorGUILayout.BeginFadeGroup(colorsFoldoutAnimation.faded))
            {
                DrawColorsList();

                UI.Space();
                UI.Line();
                UI.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }
        private void DrawColorsList()
        {
            UI.FixedSpace(5);

            if (Colors.I.Values.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
            else
            {
                foreach (var color in Colors.I.Values.ToArray())
                {
                    if (IsLocked)
                    {
                        UI.BeginHorizontal();
                        UI.Space();
                        UI.ColorDisplay(color.Value);
                        UI.Space();
                        UI.EndHorizontal();
                    }
                    else
                    {
                        UI.BeginHorizontal();

                        bool on = currentTask.ColorIds.Contains(color.Key);
                        UI.Utilities.BeginChangeCheck();
                        on = UI.Toggle(on);
                        if (UI.Utilities.EndChangeCheck())
                        {
                            if (on && !currentTask.ColorIds.Contains(color.Key))
                            {
                                UndoHandler.SaveState("Select Color");
                                currentTask.ColorIds.Add(color.Key);
                                Save();
                            }
                            else if (currentTask.ColorIds.Contains(color.Key))
                            {
                                UndoHandler.SaveState("Deselect Color");
                                currentTask.ColorIds.Remove(color.Key);
                                Save();
                            }

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

                        UI.Utilities.BeginChangeCheck();
                        Color newColor = UI.ColorField(color.Value, false);
                        if (UI.Utilities.EndChangeCheck())
                        {
                            UndoHandler.SaveState("Change Color");
                            Colors.I.Values[color.Key] = newColor;
                            Save();
                        }

                        if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                        {
                            if (!Settings.Colors.ConfirmRemoval || UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheColor, Language.Yes, Language.No))
                            {
                                MakeUpdateUISafe(() =>
                                {
                                    DeleteColor(color.Key);
                                });
                            }
                        }

                        UI.EndHorizontal();
                    }
                }
            }

            if (!IsLocked)
            {
                UI.FixedSpace(7);
                UI.BeginHorizontal();
                UI.FixedSpace(26);

                newColor = UI.ColorField(newColor, false);
                if (UI.Button(Language.Add))
                {
                    MakeUpdateUISafe(() =>
                    {
                        UndoHandler.SaveState("New Color");
                        Colors.I.Add(newColor, false);
                        newColor = Color.white;
                        UI.Utilities.LossFocus();
                    });
                }

                UI.EndHorizontal();
                UI.FixedSpace(3);
            }
        }
        private void DrawAssets()
        {
            if (!Settings.Assets.Enabled)
                return;
            if (IsLocked && currentTask.Assets.Count == 0)
                return;

            bool newShow = UI.Foldout(currentTask.ShowAssets, Language.Assets, () => { Repaint(); });
            if (newShow != currentTask.ShowAssets)
            {
                MakeUpdateUISafe(() =>
                {
                    UndoHandler.SaveState("Toogle Show Assets");
                    currentTask.ShowAssets = newShow;
                    assetsFoldoutAnimation.target = newShow;
                    if (!Settings.TaskWindow.FoldoutAnimation)
                    {
                        assetsFoldoutAnimation.value = assetsFoldoutAnimation.target;
                    }
                    Save();
                    Repaint();
                });
            }

            if (EditorGUILayout.BeginFadeGroup(assetsFoldoutAnimation.faded))
            {
                DrawAssetsList();

                UI.Space();
                UI.Line();
                UI.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }
        private void DrawAssetsList()
        {
            UI.FixedSpace(5);
            if (IsLocked)
            {
                UI.ObjectDisplayList(currentTask.Assets);
            }
            else
            {
                for (int i = 0; i < currentTask.Assets.Count; i++)
                {
                    UI.BeginHorizontal();
                    UnityEngine.Object newObject = UI.ObjectField(currentTask.Assets[i]);
                    if (currentTask.Assets[i] != newObject)
                    {
                        if (newObject == null)
                        {
                            if (!Settings.Assets.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheAsset, Language.Yes, Language.No))
                            {
                                UndoHandler.SaveState("Remove Task Asset");
                            }
                            else
                            {
                                newObject = currentTask.Assets[i];
                            }
                        }
                        else
                        {
                            UndoHandler.SaveState("Change Task Asset");
                        }

                        currentTask.Assets[i] = newObject;
                    }

                    if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                    {
                        if (!Settings.Assets.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheAsset, Language.Yes, Language.No))
                        {
                            UndoHandler.SaveState("Remove Task Asset");
                            currentTask.Assets[i] = null;
                        }
                    }
                    UI.EndHorizontal();
                }

                //new
                UI.BeginHorizontal();
                UnityEngine.Object _new = UI.ObjectField(null);
                UI.FixedSpace(22);
                UI.EndHorizontal();
                if (_new != null)
                {
                    UndoHandler.SaveState("Add Task Asset");
                    currentTask.Assets.Insert(currentTask.Assets.Count, _new);
                }

                //remove null
                currentTask.Assets.RemoveAll(delegate (UnityEngine.Object o) { return o == null; });
            }
        }

        private void DrawScreenshots()
        {
            if (!Settings.Screenshots.Enabled)
                return;
            if (IsLocked && currentTask.Screenshots.Count == 0)
                return;

            UI.BeginHorizontal();
            bool newShow = UI.Foldout(currentTask.ShowScreenshots, Language.Screenshots, () => { Repaint(); });
            if (newShow != currentTask.ShowScreenshots)
            {
                MakeUpdateUISafe(() =>
                {
                    UndoHandler.SaveState("Toogle Show Screenshots");
                    currentTask.ShowScreenshots = newShow;
                    screenshotsFoldoutAnimation.target = newShow;
                    if (!Settings.TaskWindow.FoldoutAnimation)
                    {
                        screenshotsFoldoutAnimation.value = screenshotsFoldoutAnimation.target;
                    }
                    Save();
                    Repaint();
                });
            }
            UI.Space(true);
            if (currentTask.ShowScreenshots)
            {
                if (!IsLocked)
                {
                    DrawScreenshotsNew();
                }
            }
            UI.EndHorizontal();

            if (EditorGUILayout.BeginFadeGroup(screenshotsFoldoutAnimation.faded))
            {
                DrawScreenshotsList();

                UI.Space();
                UI.Line();
                UI.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }
        private void DrawScreenshotsNew()
        {
            if (UI.Button(UI.Icons.Plus, UI.ButtonStyle.None))
            {
                UndoHandler.SaveState("Add Screenshot");

                Screenshot screenshot = Screenshots.I.New(currentTask);

                string baseName = screenshot.Name;
                string newName = baseName;
                int i = 1;
                while (Screenshots.I.All.Count(o => o.TaskID == currentTask.ID && o.Name == newName && o != screenshot) > 0)
                {
                    newName = baseName + " " + i.ToString();
                    i++;
                }
                screenshot.Name = newName;

                if (Settings.Screenshots.AutoViewNew)
                {
                    ScreenshotWindow.Init(screenshot.ID);
                }
            }
        }
        private void DrawScreenshotsList()
        {
            if (currentTask.Screenshots.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.Centered);
            }
            else
            {
                UI.FixedSpace(5);

                foreach (var screenshotId in currentTask.Screenshots)
                {
                    Screenshot screenshot = Screenshots.I.Get(screenshotId);
                    if (screenshot != null)
                    {
                        UI.BeginHorizontal();
                        UI.Utilities.BeginChangeCheck();
                        string newValue = UI.TextField(screenshot.Name, Language.Name);

                        if (UI.Utilities.EndChangeCheck())
                        {
                            if (!string.IsNullOrEmpty(newValue))
                            {
                                UndoHandler.SaveState("Change Screenshot Name");
                                screenshot.Name = newValue;
                                FileManager.SaveAll();
                            }
                        }

                        if (UI.Button(Language.View))
                        {
                            ScreenshotWindow.Init(screenshot.ID);
                        }
                        if (!IsLocked)
                        {
                            if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                            {
                                if (!Settings.Screenshots.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheScreenshot, Language.Yes, Language.No))
                                {
                                    MakeUpdateUISafe(() =>
                                    {
                                        UndoHandler.SaveState("Delete Screenshot");
                                        Screenshots.I.Delete(screenshot);
                                    });
                                }
                            }
                        }
                        UI.EndHorizontal();
                    }
                }
            }
        }

        private void DrawSubtasks()
        {
            if (!Settings.Subtasks.Enabled)
                return;
            if (IsLocked && currentTask.TasksIds.Count == 0)
                return;

            UI.BeginHorizontal();
            bool newShow = UI.Foldout(currentTask.ShowSubtasks, Language.Subtasks, () => { Repaint(); });
            if (newShow != currentTask.ShowSubtasks)
            {
                MakeUpdateUISafe(() =>
                {
                    UndoHandler.SaveState("Toogle Show Subtasks");
                    currentTask.ShowSubtasks = newShow;
                    subtasksFoldoutAnimation.target = newShow;
                    if (!Settings.TaskWindow.FoldoutAnimation)
                    {
                        subtasksFoldoutAnimation.value = subtasksFoldoutAnimation.target;
                    }
                    Save();
                    Repaint();
                });
            }
            UI.Space(true);
            if (currentTask.ShowSubtasks)
            {
                if (!IsLocked)
                {
                    DrawSubtasksEye();
                    DrawSubtasksNew();
                }
                UI.EndHorizontal();
            }
            else
            {
                UI.EndHorizontal();
            }

            if (EditorGUILayout.BeginFadeGroup(subtasksFoldoutAnimation.faded))
            {
                DrawSubtasksProgress();
                DrawSubtasksList();
            }
            EditorGUILayout.EndFadeGroup();
        }
        private void DrawSubtasksEye()
        {
            if (currentTask.TasksIds.Count != 0)
            {
                if (currentTask.HideCompletedChecklistItems)
                {
                    UI.Utilities.SetColor(Color.green);
                }
                else
                {
                    if (!UI.Utilities.ProSkin)
                    {
                        UI.Utilities.SetColor(Color.black);
                    }
                    else
                    {
                        UI.Utilities.SetColor(Color.white);
                    }
                }
                if (UI.Button(UI.Icons.Eye, UI.ButtonStyle.None))
                {
                    UndoHandler.SaveState("Toggle Hide Subtasks");
                    MakeUpdateUISafe(() =>
                    {
                        if (currentTask.HideCompletedChecklistItems)
                        {
                            ShowCompletedSubtask(currentTask);
                        }
                        else
                        {
                            HideCompletedSubtask(currentTask);
                        }
                    });
                }
                UI.Utilities.ResetColor();
            }
        }
        private void DrawSubtasksNew()
        {
            if (UI.Button(new GUIContent(UI.Icons.Plus, Language.NewSubtask), UI.ButtonStyle.None))
            {
                if (Event.current.button == 0)
                {
                    Tasks.I.New(currentTask, false);
                }
                else
                {
                    GenericMenu menu = new GenericMenu();

                    //paste
                    if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Task)
                    {
                        menu.AddItem(new GUIContent(Language.PasteTask), false, () =>
                        {
                            UndoHandler.SaveState("Paste Subtask");
                            MakeUpdateUISafe(() =>
                            {
                                CopyHandler.PasteTask(currentTask, 0);
                                Repaint();
                            });
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Language.PasteAsNew));
                    }
                    menu.ShowAsContext();
                }
            }
        }
        private void DrawSubtasksProgress()
        {
            if (Settings.Subtasks.ShowProgressBar)
            {
                GUILayout.Label("", GUIStyle.none, GUILayout.Height(20));
                EditorGUI.ProgressBar(UI.Utilities.LastRectRelative(), currentTask.Progress, currentTask.Progress100);
            }
        }
        private void DrawSubtasksList()
        {
            bool anyTaskShown = false;
            foreach (var taskId in currentTask.TasksIds)
            {
                Task subtask = Tasks.I.Get(taskId);

                //check if subtask is hidden
                if (currentTask.HideCompletedChecklistItems && subtask.Done)
                {
                    MakeUpdateUISafe(() =>
                    {
                        subtask.TaskWindow_ShowSubtask = false;
                    });
                }
                else
                {
                    subtask.TaskWindow_ShowSubtask = true;
                }
                if (!subtask.TaskWindow_ShowSubtask)
                {
                    continue;
                }
                anyTaskShown = true;

                //input update
                SubtaskInputUpdate(subtask);

                //drag margin
                UI.BeginHorizontal();
                bool dragging = draggingSubtask != null && draggingSubtask == subtask;
                if (dragging)
                    UI.FixedSpace(10);

                //begin box
                UI.BeginBox(UI.BoxStyle.Round, GUILayout.MinHeight(20));
                UI.BeginHorizontal();

                DrawSubtasksListCompletedToggle(subtask);
                DrawSubtasksListName(subtask);
                DrawSubtasksListProgress(subtask);
                DrawSubtasksListDelete(subtask);
                DrawSubtasksListLock(subtask);

                UI.EndHorizontal();

                DrawSubtasksListColors(subtask);

                //end box
                UI.EndBox();

                //drag margin
                if (dragging)
                    UI.FixedSpace(10);

                UI.EndHorizontal();

                //get rects
                subtask.TaskWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(subtask.TaskWindow_RectAbsolute, Position);
                subtask.TaskWindow_RectRelative = UI.Utilities.LastRectOnRepaintRelative(subtask.TaskWindow_RectRelative);

                //check last frame drag
                if (draggingSubtask != subtask)
                    subtask.WasDraggedLastFrame = false;
            }

            if (!anyTaskShown)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
        }
        private void DrawSubtasksListCompletedToggle(Task subtask)
        {
            if (subtask.TasksIds.Count == 0)
            {
                if (!IsLocked && !subtask.Locked)
                {
                    UI.Utilities.BeginChangeCheck();
                    bool newDone = UI.Toggle(subtask.Done);
                    if (UI.Utilities.EndChangeCheck())
                    {
                        UndoHandler.SaveState("Toggle Subtask Completed");

                        Tasks.I.SetDone(subtask, newDone);

                        Repaint();
                    }
                }
                else
                {
                    UI.Utilities.Enabled = false;
                    UI.Toggle(subtask.Done);
                    UI.Utilities.Enabled = true;
                }
            }
            else
            {
                UI.Utilities.Enabled = false;
                UI.Toggle(subtask.Progress100 == "100%" ? true : false);
                UI.Utilities.Enabled = true;
            }
        }
        private void DrawSubtasksListName(Task subtask)
        {
            if (renameObj.Is(subtask))
            {
                renameObj.Draw();

                subtask.TaskWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(subtask.TaskWindow_RectNameAbsolute, Position);
            }
            else
            {
                string finalName = (string.IsNullOrEmpty(subtask.Name) ? Language.NONAME : subtask.Name) + (Settings.Subtasks.ShowDescriptionIndicator && !string.IsNullOrEmpty(subtask.Description) ? Settings.Subtasks.DescriptionIndicator : "");
                UI.LabelWrap(finalName, subtask.TaskWindow_RectNameAbsolute.width);
                subtask.TaskWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(subtask.TaskWindow_RectNameAbsolute, Position);
                UI.Utilities.CursorIcon();
            }
        }
        private void DrawSubtasksListProgress(Task subtask)
        {
            if (Settings.Subtasks.ShowIndividualProgress && subtask.TasksIds.Count != 0)
            {
                UI.Label(subtask.Progress100, UI.LabelStyle.Mini, true);
            }
        }
        private void DrawSubtasksListDelete(Task subtask)
        {
            if (!IsLocked && !subtask.Locked && !subtask.HasLockedTask())
            {
                if (UI.Button(new GUIContent(UI.Icons.XBox, Language.DeleteTask), UI.ButtonStyle.None))
                {
                    if (UI.Utilities.Dialog(Language.DeleteTask, Language.AreYouSureYouWantToDeleteTheTask, Language.Yes, Language.No))
                    {
                        UndoHandler.SaveState("Delete Subtask");

                        MakeUpdateUISafe(() =>
                        {
                            Tasks.I.Delete(subtask);
                            Repaint();
                        });
                    }
                }
            }
        }
        private void DrawSubtasksListLock(Task subtask)
        {
            if (subtask.Locked || subtask.HasLockedTask())
            {
                if (Settings.Locking.ShowIndicator)
                {
                    if (subtask.Locked)
                        UI.Utilities.SetColor(Settings.Locking.Color);
                    else
                        UI.Utilities.SetColor(Settings.Locking.SubtaskColor);

                    GUI.DrawTexture(new Rect(subtask.TaskWindow_RectRelative.x, subtask.TaskWindow_RectRelative.y, 3, subtask.TaskWindow_RectRelative.height), UI.Icons.WhitePixel);
                    UI.Utilities.ResetColor();
                }
            }
        }
        private void DrawSubtasksListColors(Task subtask)
        {
            if (!Settings.Colors.Enabled)
                return;

            if (subtask.ColorIds.Count != 0)
            {
                if (Colors.I.RemoveInvalidIDs(subtask.ColorIds))
                {
                    Save();
                }

                Texture colorIcon = UI.Icons.ColorIconCircle;
                switch (Settings.Colors.IconType)
                {
                    case Settings.ColorIcon.Diamond:
                        colorIcon = UI.Icons.ColorIconDiamond;
                        break;
                    case Settings.ColorIcon.Box:
                        colorIcon = UI.Icons.ColorIconBox;
                        break;
                }

                float iconWidth = UI.Styles.None.CalcSize(new GUIContent(colorIcon)).x + 2;
                float startWidth = UI.Styles.BoxRound.padding.left + UI.Styles.BoxRound.padding.right;
                float width = startWidth;
                float maxWidth = Size.x - UI.Styles.Box.margin.left - UI.Styles.Box.margin.right - UI.Styles.BoxRound.padding.left - UI.Styles.BoxRound.padding.right;

                UI.BeginVertical();
                UI.BeginHorizontal();
                foreach (var colorId in subtask.ColorIds.ToArray())
                {
                    width += iconWidth;

                    if (width > maxWidth)
                    {
                        width = startWidth;
                        width += iconWidth;

                        UI.EndHorizontal();
                        UI.FixedSpace(2);
                        UI.BeginHorizontal();
                    }

                    UI.Utilities.SetColor(Colors.I.Values[colorId]);
                    UI.Image(colorIcon);
                    UI.Utilities.ResetColor();
                    UI.FixedSpace(2);
                }
                UI.EndHorizontal();
                UI.EndVertical();
            }
        }
        private void DrawTimestamps()
        {
            if (Settings.TaskWindow.ShowTimestamps)
            {
                UI.Space();
                UI.BeginVertical();
                UI.Label(Language.Created.ToLower() + ": " + currentTask.CreatedFormatted, UI.LabelStyle.Mini);
                UI.Label(Language.Updated.ToLower() + ": " + currentTask.UpdatedFormatted, UI.LabelStyle.Mini);
                UI.EndVertical();
            }
        }

        private void SubtaskInputUpdate(Task subtask)
        {
            Event e = Event.current;

            if (e.isMouse)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.button == 0 && e.clickCount == 1)
                    {
                        if (subtask.TaskWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            if (!IsLocked && !subtask.Locked && !renameObj.Is(subtask))
                            {
                                draggingSubtaskHold = subtask;
                            }
                        }
                        else
                        {
                            if (renameObj.Is(subtask))
                            {
                                renameObj.End();
                            }
                        }
                    }
                    else if (e.button == 1 && !renameObj.Is(subtask))
                    {
                        if (subtask.TaskWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            Tasks.I.ShowContextMenu(subtask, renameObj.Begin, Repaint, MakeUpdateUISafe);
                        }
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    if (!subtask.WasDraggedLastFrame && !renameObj.Is(subtask) && e.button == 0)
                    {
                        if (subtask.TaskWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            UndoHandler.SaveState("Open Subtask");

                            MakeUpdateUISafe(() =>
                            {
                                UI.Utilities.LossFocus();
                                subtask.Open();

                                Repaint();
                            });
                        }
                    }
                }
            }
        }
        private void HideCompletedSubtask(Task currentTask)
        {
            currentTask.HideCompletedChecklistItems = true;
            Save();
            Repaint();
        }
        private void ShowCompletedSubtask(Task currentTask)
        {
            currentTask.HideCompletedChecklistItems = false;
            Save();
            Repaint();
        }
        private void DeleteColor(string key)
        {
            UndoHandler.SaveState("Delete Color");

            Colors.I.Remove(key);

            if (currentTask.ColorIds.Contains(key))
                currentTask.ColorIds.Remove(key);

            Save();
        }
        private void DeleteTag(string key)
        {
            UndoHandler.SaveState("Remove Tag");

            Tags.I.Remove(key);

            if (currentTask.TagIds.Contains(key))
                currentTask.TagIds.Remove(key);

            Save();
        }

        //helpers
        private void Save()
        {
            FileManager.SaveAll();
        }
        private void ResetFoldoutAnimations()
        {
            if (currentTask != null)
            {
                tagsFoldoutAnimation = new AnimBool(currentTask.ShowTags);
                tagsFoldoutAnimation.valueChanged.AddListener(Repaint);

                colorsFoldoutAnimation = new AnimBool(currentTask.ShowColors);
                colorsFoldoutAnimation.valueChanged.AddListener(Repaint);

                assetsFoldoutAnimation = new AnimBool(currentTask.ShowAssets);
                assetsFoldoutAnimation.valueChanged.AddListener(Repaint);

                subtasksFoldoutAnimation = new AnimBool(currentTask.ShowSubtasks);
                subtasksFoldoutAnimation.valueChanged.AddListener(Repaint);

                screenshotsFoldoutAnimation = new AnimBool(currentTask.ShowScreenshots);
                screenshotsFoldoutAnimation.valueChanged.AddListener(Repaint);
            }
        }
        #endregion
    }
}
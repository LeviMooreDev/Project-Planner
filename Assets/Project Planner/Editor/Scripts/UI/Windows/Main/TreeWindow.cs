using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ProjectPlanner.Popups;
using System;

namespace ProjectPlanner
{
    public class TreeWindow : WindowBase
    {
        public static TreeWindow I
        {
            get
            {
                var windows = Resources.FindObjectsOfTypeAll<TreeWindow>();
                if (windows == null || windows.Length == 0)
                    return null;
                else
                    return windows[0];
            }
        }

        [SerializeField]
        private Vector2 scrollPosition;

        private enum RowType
        {
            Board,
            Group,
            Task,
            Subtask
        }
        private int rowCounter;

        //input
        private Vector2 lastMousePositionDown;

        //drag task
        private IHaveTasks draggingTaskStartParent;
        private int draggingTaskStartIndex;
        private Task draggingTask;
        private Task draggingTaskHold;

        //drag group
        private Board draggingGroupStartParent;
        private Group draggingGroup;
        private Group draggingGroupHold;
        private int draggingGroupStartIndex;

        //rename
        private RenameHandler renameObj;

        //search
        private bool Searching
        {
            get
            {
                return !string.IsNullOrEmpty(searchQuery) || searchByTagIds.Count != 0 || searchByColorIds.Count != 0;
            }
        }

        //search query
        [SerializeField]
        private string searchQuery = string.Empty;

        //search tags
        private Rect searchByTagButtonRect;
        [SerializeField]
        private List<string> searchByTagIds = new List<string>();
        [SerializeField]
        private bool searchByTagIncludeAll;

        //search colors
        private Rect searchByColorButtonRect;
        [SerializeField]
        private List<string> searchByColorIds = new List<string>();
        [SerializeField]
        private bool searchByColorIncludeAll;

        public static void Init()
        {
            TreeWindow window = GetWindow<TreeWindow>(false, Language.TreeWindow.ToTitleCase());
            window.minSize = new Vector2(300, 300);

            if (Prefs.GetBool("tree_view_window_first_time_open", true))
            {
                window.CenterPosition();
                Prefs.SetBool("tree_view_window_first_time_open", false);
            }
        }
        protected override void OnWindowOpen()
        {
            Search();
        }
        protected override void OnEditorReload()
        {
            draggingGroup = null;
            draggingGroupHold = null;
            draggingTask = null;
            draggingTaskHold = null;

            titleContent.image = UI.Icons.TreeWindow;
            renameObj = new RenameHandler(MakeUpdateUISafe, Repaint);

            Colors.I.OnDeleted += OnColorDeleted;
            Tags.I.OnDeleted += OnTagDeleted;

            Search();
        }
        protected override void OnUpdate()
        {
        }
        protected override void OnDraw()
        {
            DrawToolbar();

            scrollPosition = UI.BeginScrollView(scrollPosition);

            rowCounter = 0;
            foreach (var board in Boards.I.All.OrderBy(o => o.Name))
            {
                DrawBoard(board);
            }

            UI.EndScrollView();
            UI.Space();
        }
        protected override void Input(Event e)
        {
            //mouse position
            if (e.type == EventType.MouseDown)
            {
                lastMousePositionDown = UI.Utilities.MousePositionAbsolute(Position);
            }

            //drag
            if (e.type == EventType.MouseUp)
            {
                MakeUpdateUISafe(() =>
                {
                    if (draggingGroup != null)
                    {
                        Board newParent = Boards.I.Get(draggingGroup.BoardID);
                        int newIndex = newParent.GroupIds.IndexOf(draggingGroup.ID);

                        if (newParent != draggingGroupStartParent || draggingGroupStartIndex != newIndex)
                        {
                            newParent.GroupIds.RemoveAt(newIndex);
                            draggingGroupStartParent.GroupIds.Insert(draggingGroupStartIndex, draggingGroup.ID);

                            UndoHandler.SaveState("Move Group");

                            draggingGroupStartParent.GroupIds.RemoveAt(draggingGroupStartIndex);
                            newParent.GroupIds.Insert(newIndex, draggingGroup.ID);
                            draggingGroup.BoardID = newParent.ID;
                        }
                    }
                    draggingGroup = null;
                    draggingGroupHold = null;

                    if (draggingTask != null)
                    {
                        IHaveTasks newParent = draggingTask.IsParentATask ? Tasks.I.Get(draggingTask.ParentID) : (IHaveTasks)Groups.I.Get(draggingTask.ParentID);
                        int newIndex = newParent.TasksIds.IndexOf(draggingTask.ID);

                        if (newParent != draggingTaskStartParent || newIndex != draggingTaskStartIndex)
                        {
                            newParent.TasksIds.RemoveAt(newIndex);
                            draggingTaskStartParent.TasksIds.Insert(draggingTaskStartIndex, draggingTask.ID);
                            draggingTask.ParentID = draggingTaskStartParent.GetID();
                            draggingTask.IsParentATask = draggingTaskStartParent.IsATask();

                            UndoHandler.SaveState("Move Task");

                            draggingTaskStartParent.TasksIds.RemoveAt(draggingTaskStartIndex);
                            newParent.TasksIds.Insert(newIndex, draggingTask.ID);
                            draggingTask.ParentID = newParent.GetID();
                            draggingTask.IsParentATask = newParent.IsATask();
                        }

                    }
                    draggingTask = null;
                    draggingTaskHold = null;
                });
            }
            if (e.type == EventType.MouseDrag)
            {
                if (draggingGroup != null)
                {
                    GroupDragUpdate();
                }
                if (draggingTask != null)
                {
                    TaskDragUpdate();
                }

                if (draggingGroupHold != null)
                {
                    if (Vector2.Distance(UI.Utilities.MousePositionAbsolute(Position), lastMousePositionDown) > Settings.TreeWindow.Group.DragThreshold)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            draggingGroup = draggingGroupHold;
                            draggingGroupHold = null;
                            draggingGroupStartParent = Boards.I.Get(draggingGroup.BoardID);
                            draggingGroupStartIndex = draggingGroupStartParent.GroupIds.IndexOf(draggingGroup.ID);
                        });
                    }
                }
                else if (draggingTaskHold != null)
                {
                    if (Vector2.Distance(UI.Utilities.MousePositionAbsolute(Position), lastMousePositionDown) > Settings.TreeWindow.Task.DragThreshold)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            draggingTask = draggingTaskHold;
                            draggingTaskHold = null;

                            draggingTaskStartParent = draggingTask.IsParentATask ? Tasks.I.Get(draggingTask.ParentID) : (IHaveTasks)Groups.I.Get(draggingTask.ParentID);
                            draggingTaskStartIndex = draggingTaskStartParent.TasksIds.IndexOf(draggingTask.ID);
                        });
                    }
                }
            }

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
        }
        private void OnLostFocus()
        {
            //rename
            if (renameObj != null)
                renameObj.End();

            draggingGroup = null;
            draggingGroupHold = null;
            draggingTask = null;
            draggingTaskHold = null;
        }

        //toolbar
        private void DrawToolbar()
        {
            UI.BeginToolbar();
            DrawToolBarNewBoard();
            UI.Space(true);
            DrawToolBarSearchQuery();
            DrawToolBarSearchTag();
            DrawToolBarSearchColor();

            UI.EndToolbar();
        }
        private void DrawToolBarNewBoard()
        {
            if (UI.Button(new GUIContent(UI.Icons.Plus, Language.NewBoard), UI.ButtonStyle.Toolbar))
            {
                UndoHandler.SaveState("New Board");

                renameObj.End();
                MakeUpdateUISafe(() =>
                {
                    UI.Utilities.LossFocus();
                    Boards.I.New();
                    Repaint();
                });
            }
        }
        private void DrawToolBarSearchQuery()
        {
            UI.Utilities.BeginChangeCheck();
            UI.Utilities.Name("tree_search");
            string newSearchQuery = UI.SearchField(searchQuery);
            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Search Query");
                searchQuery = newSearchQuery;
                Search();
            }
        }
        private void DrawToolBarSearchTag()
        {
            if (!Settings.Tags.Enabled)
                return;

            if (UI.Button(new GUIContent(UI.Icons.Tag, Language.SearchByTag), searchByTagIds.Count == 0 ? UI.ButtonStyle.Toolbar : UI.ButtonStyle.ToolbarActive))
            {
                List<PopupOption> options = new List<PopupOption>
                {
                    new PopupOptionButton(Language.Reset, () =>
                    {
                        searchByTagIds.Clear();
                    }),
                    new PopupOptionToggle(Language.TaskHasAllSelected, searchByTagIncludeAll, () =>
                    {
                        searchByTagIncludeAll = !searchByTagIncludeAll;
                        Prefs.SetBool("tree_window_search_by_tag_include_all", searchByTagIncludeAll);
                    })
                };

                UI.Utilities.ShowTagsPopup(searchByTagButtonRect, searchByTagIds, TagsPopup.State.Select, null, () => { Search(); Repaint(); }, options.ToArray());
            }
            searchByTagButtonRect = UI.Utilities.LastRectOnRepaintRelative(searchByTagButtonRect);
            Tags.I.RemoveInvalidIDs(searchByTagIds);
        }
        private void DrawToolBarSearchColor()
        {
            if (!Settings.Colors.Enabled)
                return;

            if (UI.Button(new GUIContent(UI.Icons.Colors, Language.SearchByColor), searchByColorIds.Count == 0 ? UI.ButtonStyle.Toolbar : UI.ButtonStyle.ToolbarActive))
            {
                List<PopupOption> options = new List<PopupOption>
                {
                    new PopupOptionButton(Language.Reset, () =>
                    {
                        searchByColorIds.Clear();
                    }),
                    new PopupOptionToggle(Language.TaskHasAllSelected, searchByColorIncludeAll, () =>
                    {
                        searchByColorIncludeAll = !searchByColorIncludeAll;
                        Prefs.SetBool("tree_window_search_by_color_include_all", searchByColorIncludeAll);
                    })
                };

                UI.Utilities.ShowColorsPopup(searchByColorButtonRect, searchByColorIds, ColorsPopup.State.Select, null, () => { Search(); Repaint(); }, options.ToArray());
            }
            searchByColorButtonRect = UI.Utilities.LastRectOnRepaintRelative(searchByColorButtonRect);
            Colors.I.RemoveInvalidIDs(searchByColorIds);
        }

        //board
        private void DrawBoard(Board board)
        {
            //check for tasks visible in search
            if (Searching && Settings.TreeWindow.Group.HideEmptyWhenSearching)
            {
                bool hasTasksToShow = false;

                foreach (var groupId in board.GroupIds)
                {
                    foreach (var taskId in Groups.I.Get(groupId).TasksIds)
                    {
                        Task task = Tasks.I.Get(taskId);

                        if (task.TreeWindow_SearchShow)
                        {
                            hasTasksToShow = true;
                            break;
                        }
                    }
                }

                if (!hasTasksToShow)
                {
                    board.TreeWindow_RectAbsolute = Rect.zero;
                    return;
                }
            }


            BoardInputUpdate(board);
            SetupAnimation(ref board.treeWindowShowGroupsAnimation, board.TreeWindow_ShowGroups);

            BeginBox(RowType.Board);
            UI.BeginHorizontal();

            //foldout
            if (board.GroupIds.Count != 0)
            {
                UI.Utilities.BeginChangeCheck();
                bool newShow = UI.Foldout(board.TreeWindow_ShowGroups);
                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Toogle Board Foldout");
                    board.TreeWindow_ShowGroups = newShow;
                    board.treeWindowShowGroupsAnimation.target = newShow;
                    Save();
                    Repaint();
                }
            }

            //name
            if (renameObj.Is(board))
            {
                renameObj.Draw();

                board.TreeWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(board.TreeWindow_RectNameAbsolute, Position);
                board.TreeWindow_RectNameRelative = UI.Utilities.LastRectOnRepaintRelative(board.TreeWindow_RectNameRelative);
            }
            else
            {
                Rect rect = board.TreeWindow_RectNameAbsolute;
                Rect rectRelative = board.TreeWindow_RectNameRelative;
                DrawName(board.Name, board.TreeWindow_RectNameAbsolute.width, ref rect, ref rectRelative);
                board.TreeWindow_RectNameAbsolute = rect;
                board.TreeWindow_RectNameRelative = rectRelative;
            }

            UI.EndHorizontal();
            UI.EndBox();
            board.TreeWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(board.TreeWindow_RectAbsolute, Position);

            //groups
            if (Searching)
            {
                foreach (var group in board.GroupIds.Select(o => Groups.I.Get(o)))
                    DrawGroup(group);
            }
            else
            {
                if (EditorGUILayout.BeginFadeGroup(board.treeWindowShowGroupsAnimation.faded))
                {
                    foreach (var group in board.GroupIds.Select(o => Groups.I.Get(o)))
                        DrawGroup(group);

                    if (board.GroupIds.Count != 0)
                        UI.Space();
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
        private void BoardInputUpdate(Board board)
        {
            Event e = Event.current;

            ///rename
            if (e.isMouse && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (renameObj.Is(board))
                    {
                        if (e.clickCount == 1)
                        {
                            if (!board.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.End();
                            }
                        }
                    }
                    else if (e.clickCount == 2)
                    {
                        if (board.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            renameObj.Begin(board);
                        }
                    }
                }
                else if (e.button == 1)
                {
                    if (!renameObj.Is(board))
                    {
                        if (board.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            renameObj.End();

                            Boards.I.ShowContextMenu(board, renameObj.Begin, Repaint, MakeUpdateUISafe);
                        }
                    }
                }
            }
        }

        //group
        private void DrawGroup(Group group)
        {
            //check for tasks visible in search
            if (Searching && Settings.TreeWindow.Group.HideEmptyWhenSearching)
            {
                bool hasTasksToShow = false;
                foreach (var taskId in group.TasksIds)
                {
                    Task task = Tasks.I.Get(taskId);

                    if (task.TreeWindow_SearchShow)
                    {
                        hasTasksToShow = true;
                        break;
                    }
                }

                if (!hasTasksToShow)
                {
                    group.TreeWindow_RectAbsolute = Rect.zero;
                    return;
                }
            }

            GroupInputUpdate(group);
            SetupAnimation(ref group.TreeWindowShowAnimation, group.TreeWindow_ShowTasks);

            UI.BeginHorizontal();
            UI.FixedSpace(20);
            BeginBox(RowType.Group);
            UI.BeginHorizontal();

            //foldout
            if (group.TasksIds.Count != 0)
            {
                UI.Utilities.BeginChangeCheck();
                bool newShow = UI.Foldout(group.TreeWindow_ShowTasks);
                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Toogle Group Foldout");
                    group.TreeWindow_ShowTasks = newShow;
                    group.TreeWindowShowAnimation.target = newShow;
                    Save();
                    Repaint();
                }
            }

            //name
            if (renameObj.Is(group))
            {
                renameObj.Draw();

                group.TreeWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(group.TreeWindow_RectNameAbsolute, Position);
                group.TreeWindow_RectNameRelative = UI.Utilities.LastRectOnRepaintRelative(group.TreeWindow_RectNameRelative);
            }
            else
            {
                Rect rect = group.TreeWindow_RectNameAbsolute;
                Rect rectRelative = group.TreeWindow_RectNameRelative;
                DrawName(group.Name, group.TreeWindow_RectNameAbsolute.width, ref rect, ref rectRelative);
                group.TreeWindow_RectNameAbsolute = rect;
                group.TreeWindow_RectNameRelative = rectRelative;
            }

            UI.EndHorizontal();
            UI.EndBox();

            //dragging margin
            bool dragging = draggingGroup != null && draggingGroup == group;
            if (dragging)
                UI.FixedSpace(5);

            UI.EndHorizontal();
            group.TreeWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(group.TreeWindow_RectAbsolute, Position);
            group.TreeWindow_RectRelative = UI.Utilities.LastRectOnRepaintRelative(group.TreeWindow_RectRelative);

            //tasks
            if (Searching)
            {
                foreach (var task in group.TasksIds.Select(o => Tasks.I.Get(o)))
                    DrawTask(task, 1);
            }
            else
            {
                if (EditorGUILayout.BeginFadeGroup(group.TreeWindowShowAnimation.faded))
                {
                    foreach (var task in group.TasksIds.Select(o => Tasks.I.Get(o)))
                        DrawTask(task, 1);
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
        private void GroupInputUpdate(Group group)
        {
            Event e = Event.current;

            ///rename
            if (e.isMouse && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (e.clickCount == 1)
                    {
                        if (renameObj.Is(group))
                        {
                            if (!group.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.End();
                            }
                        }
                        else
                        { 
                            if (group.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                if (!Searching)
                                {
                                    draggingGroupHold = group;
                                }
                            }
                        }
                    }
                    else if (e.clickCount == 2)
                    {
                        if (!renameObj.Is(group))
                        {
                            if (group.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.Begin(group);
                            }
                        }
                    }
                }
                else if (e.button == 1)
                {
                    if (!renameObj.Is(group))
                    {
                        if (group.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            renameObj.End();

                            Groups.I.ShowContextMenu(group, renameObj.Begin, Repaint, MakeUpdateUISafe, Groups.FlowDirection.UpDown);
                        }
                    }
                }
            }
        }
        private void GroupDragUpdate()
        {
            if (draggingGroup != null)
            {
                foreach (var board in Boards.I.All)
                {
                    foreach (var groupID in board.GroupIds.ToArray())
                    {
                        Group group = Groups.I.Get(groupID);

                        if (group.TreeWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            if (group != draggingGroup)
                            {
                                MakeUpdateUISafe(() =>
                                {
                                    int newIndex = board.GroupIds.IndexOf(group.ID);
                                    Boards.I.Get(draggingGroup.BoardID).GroupIds.Remove(draggingGroup.ID);
                                    board.GroupIds.Insert(newIndex, draggingGroup.ID);
                                    draggingGroup.BoardID = board.ID;

                                    Save();
                                });
                                break;
                            }
                        }
                    }

                    if (board.TreeWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                    {
                        MakeUpdateUISafe(() =>
                        {
                            Boards.I.Get(draggingGroup.BoardID).GroupIds.Remove(draggingGroup.ID);
                            board.GroupIds.Insert(0, draggingGroup.ID);
                            draggingGroup.BoardID = board.ID;

                            Rect left = new Rect(board.TreeWindow_RectAbsolute.x, board.TreeWindow_RectAbsolute.y, board.TreeWindow_RectAbsolute.width / 2, board.TreeWindow_RectAbsolute.height);
                            Rect right = new Rect(left.x + left.width, left.y, left.width, left.height);

                            if (left.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                board.TreeWindow_ShowGroups = false;
                                board.treeWindowShowGroupsAnimation.target = false;
                                board.treeWindowShowGroupsAnimation.value = false;
                            }
                            else if (right.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                board.TreeWindow_ShowGroups = true;
                                board.treeWindowShowGroupsAnimation.target = true;
                                board.treeWindowShowGroupsAnimation.value = true;
                            }

                            Save();
                        });
                    }
                }

                Repaint();
            }
        }
         
        //task
        private void DrawTask(Task task, int insert)
        {
            if (!task.TreeWindow_SearchShow)
                return;

            TaskInputUpdate(task);
            SetupAnimation(ref task.treeWindowShowSubtasksAnimation, task.TreeWindow_ShowSubtasks);

            UI.BeginHorizontal();
            UI.FixedSpace(20 + insert * 20);
            BeginBox(insert == 1 ? RowType.Task : RowType.Subtask);
            UI.BeginHorizontal();
            DrawTaskFoldout(task);
            DrawTaskName(task);
            DrawTaskLock(task);
            UI.EndHorizontal();
            DrawTaskColors(task);
            DrawTaskProgress(task);

            UI.EndBox();
            task.TreeWindow_BoxRectRelative = UI.Utilities.LastRectOnRepaintRelative(task.TreeWindow_BoxRectRelative);
            UI.Utilities.CursorIcon();

            //dragging margin
            bool dragging = draggingTask != null && draggingTask == task;
            if (dragging)
                UI.FixedSpace(5);

            UI.EndHorizontal();
            task.TreeWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(task.TreeWindow_RectAbsolute, Position);
            task.TreeWindow_RectRelative = UI.Utilities.LastRectOnRepaintRelative(task.TreeWindow_RectRelative);

            //check last frame drag
            if (draggingTask != task)
                task.WasDraggedLastFrame = false;

            DrawTaskSubtasks(task, insert);
        }
        private void DrawTaskFoldout(Task task)
        {
            if (task.TasksIds.Count != 0)
            {
                UI.Utilities.BeginChangeCheck();
                bool newShow = UI.Foldout(task.TreeWindow_ShowSubtasks);
                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Toogle Task Foldout");
                    task.TreeWindow_ShowSubtasks = newShow;
                    task.treeWindowShowSubtasksAnimation.target = newShow;
                    Save();
                    Repaint();
                }
            }

        }
        private void DrawTaskName(Task task)
        {
            if (renameObj.Is(task))
            {
                renameObj.Draw();

                task.TreeWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(task.TreeWindow_RectNameAbsolute, Position);
                task.TreeWindow_RectNameRelative = UI.Utilities.LastRectOnRepaintRelative(task.TreeWindow_RectNameRelative);
            }
            else
            {
                string finalName = (string.IsNullOrEmpty(task.Name) ? Language.NONAME : task.Name) + (Settings.TreeWindow.Task.ShowDescriptionIndicator && !string.IsNullOrEmpty(task.Description) ? Settings.TreeWindow.Task.DescriptionIndicator : "");
                Rect rect = task.TreeWindow_RectNameAbsolute;
                Rect rectRelative = task.TreeWindow_RectNameRelative;
                DrawName(finalName, task.TreeWindow_RectNameAbsolute.width, ref rect, ref rectRelative);
                task.TreeWindow_RectNameAbsolute = rect;
                task.TreeWindow_RectNameRelative = rectRelative;
            }
        }
        private void DrawTaskLock(Task task)
        {
            if (task.Locked || task.HasLockedTask())
            {
                if (Settings.Locking.ShowIndicator)
                {
                    if (task.Locked)
                        UI.Utilities.SetColor(Settings.Locking.Color);
                    else
                        UI.Utilities.SetColor(Settings.Locking.SubtaskColor);

                    GUI.DrawTexture(new Rect(task.TreeWindow_BoxRectRelative.x - 4, task.TreeWindow_BoxRectRelative.y, 3, task.TreeWindow_BoxRectRelative.height), UI.Icons.WhitePixel);
                    UI.Utilities.ResetColor();
                }
            }
        }
        private void DrawTaskColors(Task task)
        {
            if (!Settings.Colors.Enabled)
                return;

            if (task.ColorIds.Count != 0)
            {
                if (Colors.I.RemoveInvalidIDs(task.ColorIds))
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
                float startWidth = UI.Styles.Box.padding.left + UI.Styles.Box.padding.right;
                float width = startWidth;
                float maxWidth = Settings.BoardWindow.Group.FixedWidth - UI.Styles.Box.padding.left - UI.Styles.Box.padding.right - UI.Styles.BoxRound.padding.left - UI.Styles.BoxRound.padding.right;

                UI.BeginVertical();
                UI.BeginHorizontal();
                foreach (var colorId in task.ColorIds.ToArray())
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
        private void DrawTaskProgress(Task task)
        {
            if (Settings.Subtasks.ShowProgressInBoardWindow && task.TasksIds.Count != 0)
            {
                UI.Label(task.Progress100, UI.LabelStyle.Mini, true);
            }
        }
        private void DrawTaskSubtasks(Task task, int insert)
        {
            if (task.TasksIds.Count != 0)
            {
                if (Searching)
                {
                    foreach (var subtask in task.TasksIds.Select(o => Tasks.I.Get(o)))
                        DrawTask(subtask, insert + 1);
                }
                else
                {
                    if (EditorGUILayout.BeginFadeGroup(task.treeWindowShowSubtasksAnimation.faded))
                    {
                        foreach (var subtask in task.TasksIds.Select(o => Tasks.I.Get(o)))
                            DrawTask(subtask, insert + 1);
                    }
                    EditorGUILayout.EndFadeGroup();
                }
            }
        }

        private void TaskInputUpdate(Task task)
        {
            Event e = Event.current;

            if (e.isMouse)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (e.button == 0)
                    {
                        if (renameObj.Is(task))
                        {
                            if (e.clickCount == 1)
                            {
                                if (!task.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    renameObj.End();
                                }
                            }
                        }
                        else
                        {
                            if (e.clickCount == 1)
                            {
                                if (task.TreeWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    if (!task.Locked && !Searching)
                                    {
                                        draggingTaskHold = task;
                                    }
                                }
                            }
                            else if (e.clickCount == 2)
                            {
                                if (task.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    renameObj.Begin(task);
                                }
                            }
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (!renameObj.Is(task))
                        {
                            if (task.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.End();
                                Tasks.I.ShowContextMenu(task, renameObj.Begin, Repaint, MakeUpdateUISafe);
                            }
                        }
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    if (!task.WasDraggedLastFrame && !renameObj.Is(task) && e.button == 0)
                    {
                        if (task.TreeWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            UndoHandler.SaveState("Open Task");
                            task.Open();
                        }
                    }
                }
            }
        }
        private void TaskDragUpdate()
        {
            if (draggingTask == null)
                return;

            draggingTask.WasDraggedLastFrame = true;
            bool moved = false;

            foreach (var board in Boards.I.All)
            {
                if (moved)
                    break;

                Rect left = new Rect(board.TreeWindow_RectAbsolute.x, board.TreeWindow_RectAbsolute.y, board.TreeWindow_RectAbsolute.width / 2, board.TreeWindow_RectAbsolute.height);
                Rect right = new Rect(left.x + left.width, left.y, left.width, left.height);

                if (left.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                {
                    MakeUpdateUISafe(() =>
                    {
                        board.TreeWindow_ShowGroups = false;
                        board.treeWindowShowGroupsAnimation.target = false;
                        board.treeWindowShowGroupsAnimation.value = false;

                        Save();
                    });
                }
                else if (right.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                {
                    MakeUpdateUISafe(() =>
                    {
                        board.TreeWindow_ShowGroups = true;
                        board.treeWindowShowGroupsAnimation.target = true;
                        board.treeWindowShowGroupsAnimation.value = true;

                        Save();
                    });
                }

                if (!board.TreeWindow_ShowGroups || !board.TreeWindow_SearchShow)
                    continue;

                foreach (var groupId in board.GroupIds)
                {
                    if (moved)
                        break;

                    Group group = Groups.I.Get(groupId);
                    if (group.TreeWindow_ShowTasks && group.TreeWindow_SearchShow)
                        moved = TaskDragUpdateIHaveTasks(group);

                    if (!moved)
                    {
                        if (group.TreeWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            MakeUpdateUISafe(() =>
                            {
                                IHaveTasks oldParentGroup = draggingTask.IsParentATask ? Tasks.I.Get(draggingTask.ParentID) : (IHaveTasks)Groups.I.Get(draggingTask.ParentID);
                                oldParentGroup.TasksIds.Remove(draggingTask.ID);

                                group.TasksIds.Insert(0, draggingTask.ID);
                                draggingTask.ParentID = group.ID;
                                draggingTask.IsParentATask = false;

                                left = new Rect(group.TreeWindow_RectAbsolute.x, group.TreeWindow_RectAbsolute.y, group.TreeWindow_RectAbsolute.width / 2, group.TreeWindow_RectAbsolute.height);
                                right = new Rect(left.x + left.width, left.y, left.width, left.height);
                                if (left.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    group.TreeWindow_ShowTasks = false;
                                    group.TreeWindowShowAnimation.target = false;
                                    group.TreeWindowShowAnimation.value = false;
                                }
                                else if (right.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    group.TreeWindow_ShowTasks = true;
                                    group.TreeWindowShowAnimation.target = true;
                                    group.TreeWindowShowAnimation.value = true;
                                }

                                Save();
                            });
                            moved = true;
                        }
                    }
                }
            }

            Repaint();
        }
        private bool TaskDragUpdateIHaveTasks(IHaveTasks newParent)
        {
            foreach (var taskId in newParent.TasksIds)
            {
                if (taskId == draggingTask.ID)
                    continue;

                Task task = Tasks.I.Get(taskId);
                if (!task.TreeWindow_SearchShow)
                    continue;

                Rect left = new Rect(task.TreeWindow_RectAbsolute.x, task.TreeWindow_RectAbsolute.y, task.TreeWindow_RectAbsolute.width / 2, task.TreeWindow_RectAbsolute.height);
                Rect right = new Rect(left.x + left.width, left.y, left.width, left.height);

                if (left.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                {
                    MakeUpdateUISafe(() =>
                    {
                        int newIndex = newParent.TasksIds.IndexOf(task.ID);

                        IHaveTasks oldParent = draggingTask.IsParentATask ? Tasks.I.Get(draggingTask.ParentID) : (IHaveTasks)Groups.I.Get(draggingTask.ParentID);

                        oldParent.TasksIds.Remove(draggingTask.ID);
                        newParent.TasksIds.Insert(newIndex, draggingTask.ID);
                        draggingTask.ParentID = newParent.GetID();
                        draggingTask.IsParentATask = newParent.IsATask();

                        task.TreeWindow_ShowSubtasks = false;
                        task.treeWindowShowSubtasksAnimation.target = false;
                        task.treeWindowShowSubtasksAnimation.value = false;

                        Save();
                    });
                    return true;
                }
                else if (right.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                {
                    MakeUpdateUISafe(() =>
                    {
                        IHaveTasks oldParentGroup = draggingTask.IsParentATask ? Tasks.I.Get(draggingTask.ParentID) : (IHaveTasks)Groups.I.Get(draggingTask.ParentID);
                        oldParentGroup.TasksIds.Remove(draggingTask.ID);

                        task.TasksIds.Insert(0, draggingTask.ID);
                        draggingTask.ParentID = task.ID;
                        draggingTask.IsParentATask = true;

                        task.TreeWindow_ShowSubtasks = true;
                        task.treeWindowShowSubtasksAnimation.target = true;
                        task.treeWindowShowSubtasksAnimation.value = true;

                        Save();
                    });
                }

                if (task.TreeWindow_ShowSubtasks && task.TreeWindow_SearchShow)
                    TaskDragUpdateIHaveTasks(task);
            }
            return false;
        }


        //others
        public void OnTagDeleted()
        {
            Tags.I.RemoveInvalidIDs(searchByTagIds);
            Search();
        }
        public void OnColorDeleted()
        {
            Colors.I.RemoveInvalidIDs(searchByColorIds);
            Search();
        }
        public void Search()
        {
            MakeUpdateUISafe(() =>
            {
                SearchTasks(Tasks.I.All);
                Repaint();
            });
        }
        public bool SearchTasks(List<Task> tasks)
        {
            string searchQueryFilter = string.Empty;
            string searchQuery = this.searchQuery;
            if (searchQuery != string.Empty)
            {
                string[] split = searchQuery.Split(':');
                if (split.Length > 1)
                {
                    searchQueryFilter = split[0];
                    searchQuery = split[1];
                }
            }

            bool showAny = false;
            foreach (var task in tasks)
            {
                task.TreeWindow_SearchShow = true;

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    task.TreeWindow_SearchShow = false;

                    if (searchQueryFilter == string.Empty || searchQueryFilter == "n")
                    {
                        if (task.Name.ToLower().Contains(searchQuery.ToLower()))
                            task.TreeWindow_SearchShow = true;
                    }
                    if (searchQueryFilter == string.Empty || searchQueryFilter == "d")
                    {
                        if (task.Description.ToLower().Contains(searchQuery.ToLower()))
                            task.TreeWindow_SearchShow = true;
                    }
                }

                if (Settings.Tags.Enabled)
                {
                    if (task.TreeWindow_SearchShow)
                    {
                        if (searchByTagIds.Count != 0)
                        {
                            foreach (var filterTag in searchByTagIds)
                            {
                                if (searchByTagIncludeAll)
                                {
                                    if (!task.TagIds.Contains(filterTag))
                                    {
                                        task.TreeWindow_SearchShow = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    task.TreeWindow_SearchShow = false;
                                    if (task.TagIds.Contains(filterTag))
                                    {
                                        task.TreeWindow_SearchShow = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (Settings.Colors.Enabled)
                {
                    if (task.TreeWindow_SearchShow)
                    {
                        if (searchByColorIds.Count != 0)
                        {
                            foreach (var filterColor in searchByColorIds)
                            {
                                if (searchByColorIncludeAll)
                                {
                                    if (!task.ColorIds.Contains(filterColor))
                                    {
                                        task.TreeWindow_SearchShow = false;
                                    }
                                }
                                else
                                {
                                    task.TreeWindow_SearchShow = false;
                                    if (task.ColorIds.Contains(filterColor))
                                    {
                                        task.TreeWindow_SearchShow = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!task.TreeWindow_SearchShow && SearchTasks(task.TasksIds.Select(o => Tasks.I.Get(o)).ToList()))
                {
                    task.TreeWindow_SearchShow = true;
                }
                
                if (task.TreeWindow_SearchShow)
                {
                    showAny = true;
                }
            }
            return showAny;
        }
        private void Save()
        {
            FileManager.SaveAll();
        }

        //helpers
        private void BeginBox(RowType type)
        {
            switch (Settings.TreeWindow.RowColoring.Type)
            {
                case Settings.TreeRowColoring.Same:
                    UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.Same);
                    break;
                case Settings.TreeRowColoring.Alternating:
                    if (rowCounter % 2 == 0)
                        UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.Alternating.Even);
                    else
                        UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.Alternating.Odd);
                    break;
                case Settings.TreeRowColoring.ByType:
                    switch (type)
                    {
                        case RowType.Board:
                            UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.ByType.Board);
                            break;
                        case RowType.Group:
                            UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.ByType.Group);
                            break;
                        case RowType.Task:
                            UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.ByType.Task);
                            break;
                        case RowType.Subtask:
                            UI.Utilities.SetColor(Settings.TreeWindow.RowColoring.ByType.Subtask);
                            break;
                        default:
                            break;
                    }
                    break;
            }

            switch (Settings.TreeWindow.RowColoring.RowStyle)
            {
                case Settings.TreeRowStyle.Box1:
                    UI.BeginBox(UI.BoxStyle.Round);
                    break;
                case Settings.TreeRowStyle.Box2:
                    UI.BeginBox();
                    break;
                case Settings.TreeRowStyle.Flat:
                    UI.BeginBox(UI.BoxStyle.Flat);
                    break;
            }

            UI.Utilities.ResetColor();

            rowCounter++;
        }
        private void DrawName(string name, float width, ref Rect rect, ref Rect rectRelative)
        {
            UI.LabelWrap(name, width);
            rect = UI.Utilities.LastRectOnRepaintAbsolute(rect, Position);
            rectRelative = UI.Utilities.LastRectOnRepaintRelative(rectRelative);
            UI.FixedSpace(5);
        }
        private void SetupAnimation(ref AnimBool animBool, bool value)
        {
            if (animBool == null)
            {
                animBool = new AnimBool(value);
            }
            animBool.valueChanged.RemoveListener(Repaint);
            animBool.valueChanged.AddListener(Repaint);
        }
    }
}
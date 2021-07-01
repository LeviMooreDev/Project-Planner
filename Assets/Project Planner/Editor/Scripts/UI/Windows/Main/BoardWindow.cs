using ProjectPlanner.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public class BoardWindow : WindowBase
    {
        #region Fields
        public static BoardWindow I
        {
            get
            {
                var windows = Resources.FindObjectsOfTypeAll<BoardWindow>();
                if (windows == null || windows.Length == 0)
                    return null;
                else
                    return windows[0];
            }
        }

        //board
        [SerializeField]
        public int currentBoardIndex;
        public Board CurrentBoard
        {
            get
            {
                if (Boards.I.All.Count == 0)
                    return null;

                if (currentBoardIndex < 0 || currentBoardIndex > Boards.I.All.Count - 1)
                    currentBoardIndex = 0;

                return Boards.I.All[currentBoardIndex];
            }
        }

        private readonly int boardPadding = 10;

        //group
        private Group draggingGroup;
        private Group draggingGroupHold;
        private int draggingGroupStartIndex;

        //task
        private Group draggingTaskStartParent;
        private int draggingTaskStartIndex;
        private Task draggingTask;
        private Task draggingTaskHold;

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

        //imput
        private Vector2 lastMousePositionDown;
        private bool HideUIWhenDragging
        {
            get
            {
                return Settings.Other.HideUIWhenDragging && (draggingGroup != null || draggingTask != null);
            }
        }
        #endregion

        #region Methods
        public static void Init()
        {
            if (Settings.BoardWindow.AllowMultipleWindows)
            {
                BoardWindow window = CreateInstance<BoardWindow>();

                window.titleContent = new GUIContent(Language.Board);
                window.minSize = new Vector2(Settings.BoardWindow.MinimumWidth, Settings.BoardWindow.MinimumHeight);
                if (Settings.BoardWindow.Utility)
                {
                    window.ShowUtility();
                }
                else
                {
                    window.Show();
                }
                window.CenterPosition();
            }
            else
            {
                BoardWindow window = GetWindow<BoardWindow>(Settings.BoardWindow.Utility, Language.Board);

                window.minSize = new Vector2(Settings.BoardWindow.MinimumWidth, Settings.BoardWindow.MinimumHeight);

                if (Prefs.GetBool("board_window_first_time_open", true))
                {
                    window.CenterPosition();
                    Prefs.SetBool("board_window_first_time_open", false);
                }
            }
        }
        public static void CloseWindow()
        {
            GetWindow<BoardWindow>(Settings.BoardWindow.Utility, Language.Board).Close();
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

            renameObj = new RenameHandler(MakeUpdateUISafe, Repaint);

            searchByTagIncludeAll = Prefs.GetBool("board_window_search_by_tag_include_all", true);
            searchByColorIncludeAll = Prefs.GetBool("board_window_search_by_color_include_all", true);

            Colors.I.OnDeleted += OnColorDeleted;
            Tags.I.OnDeleted += OnTagDeleted;

            Search();
        }
        protected override void OnUpdate()
        {
            if (Settings.Other.ShowWindowTitleIcons)
            {
                titleContent.image = UI.Icons.BoardWindow;
            }
            else
            {
                titleContent.image = null;
            }

            if (Settings.BoardWindow.BoardNameInWindowTab && CurrentBoard != null && !string.IsNullOrEmpty(CurrentBoard.Name))
            {
                titleContent.text = CurrentBoard.Name;
            }
            else
            {
                titleContent.text = Language.Board;
            }
        }
        protected override void OnDraw()
        {
            DrawToolbar();

            if (CurrentBoard != null)
                DrawBoard(CurrentBoard);
        }
        protected override void Input(Event e)
        {
            //mouse position
            if (e.type == EventType.MouseDown)
            {
                lastMousePositionDown = UI.Utilities.MousePositionAbsolute(Position);
            }

            //drag task
            if (e.type == EventType.MouseUp)
            {
                MakeUpdateUISafe(() =>
                {
                    if (draggingGroup != null)
                    {
                        int newIndex = CurrentBoard.GroupIds.IndexOf(draggingGroup.ID);
                        if (draggingGroupStartIndex != newIndex)
                        {
                            CurrentBoard.GroupIds.RemoveAt(newIndex);
                            CurrentBoard.GroupIds.Insert(draggingGroupStartIndex, draggingGroup.ID);

                            UndoHandler.SaveState("Move Group");

                            CurrentBoard.GroupIds.RemoveAt(draggingGroupStartIndex);
                            CurrentBoard.GroupIds.Insert(newIndex, draggingGroup.ID);
                        }

                    }
                    draggingGroup = null;
                    draggingGroupHold = null;

                    if (draggingTask != null)
                    {
                        Group newParent = Groups.I.Get(draggingTask.ParentID);
                        int newIndex = newParent.TasksIds.IndexOf(draggingTask.ID);

                        if (newParent != draggingTaskStartParent || newIndex != draggingTaskStartIndex)
                        {
                            newParent.TasksIds.RemoveAt(newIndex);
                            draggingTaskStartParent.TasksIds.Insert(draggingTaskStartIndex, draggingTask.ID);
                            draggingTask.ParentID = draggingTaskStartParent.ID;

                            UndoHandler.SaveState("Move Task");

                            draggingTaskStartParent.TasksIds.RemoveAt(draggingTaskStartIndex);
                            newParent.TasksIds.Insert(newIndex, draggingTask.ID);
                            draggingTask.ParentID = newParent.ID;
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
                    if (Vector2.Distance(UI.Utilities.MousePositionAbsolute(Position), lastMousePositionDown) > Settings.BoardWindow.Group.DragThreshold)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            draggingGroup = draggingGroupHold;
                            draggingGroupHold = null;
                            draggingGroupStartIndex = CurrentBoard.GroupIds.IndexOf(draggingGroup.ID);
                        });
                    }
                }
                else if (draggingTaskHold != null)
                {
                    if (Vector2.Distance(UI.Utilities.MousePositionAbsolute(Position), lastMousePositionDown) > Settings.BoardWindow.Task.DragThreshold)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            draggingTask = draggingTaskHold;
                            draggingTaskHold = null;
                            draggingTaskStartParent = Groups.I.Get(draggingTask.ParentID);
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
            if(renameObj != null)
                renameObj.End();

            draggingGroup = null;
            draggingTask = null;
        }

        //toolbar
        private void DrawToolbar()
        {
            UI.BeginToolbar();

            if (Boards.I.All.Count == 0)
            {
                DrawToolbarDisabled();
            }
            else
            {
                DrawToolBarSelectBoard();
                DrawToolBarNewBoard();
                DrawToolBarRenameBoard();
                DrawToolBarDeleteBoard();
                DrawToolBarNewGroup();
                DrawToolBarTreeView();

                UI.Space(true);

                DrawToolBarSearchQuery();
                DrawToolBarSearchTag();
                DrawToolBarSearchColor();
            }

            UI.EndToolbar();
        }
        private void DrawToolbarDisabled()
        {
            UI.Utilities.Enabled = false;
            UI.StringDropDown("", new List<string>() { "" }, UI.EnumDropDownStyle.Toolbar);
            UI.Utilities.Enabled = true;
            DrawToolBarNewBoard();
            UI.Utilities.Enabled = false;
            UI.Button(Language.Rename, UI.ButtonStyle.Toolbar);
            UI.Button(Language.Delete, UI.ButtonStyle.Toolbar);

            UI.Space(true);

            UI.SearchField(string.Empty);
            UI.Button(new GUIContent(UI.Icons.Tag, Language.SearchByTag), UI.ButtonStyle.Toolbar);
            UI.Button(new GUIContent(UI.Icons.Colors, Language.SearchByColor), UI.ButtonStyle.Toolbar);

            UI.Utilities.Enabled = true;
        }
        private void DrawToolBarSelectBoard()
        {
            if (renameObj.Is(CurrentBoard))
            {
                renameObj.Draw(true);

                CurrentBoard.BoardWindow_NameTextFieldRect = UI.Utilities.LastRectOnRepaintAbsolute(CurrentBoard.BoardWindow_NameTextFieldRect, Position);
            }
            else
            {
                UI.Utilities.BeginChangeCheck();
                List<string> names = Boards.I.All.Select(o => o.Name).ToList();
                names.Sort();

                string seleted = UI.StringDropDown(CurrentBoard.Name, names, UI.EnumDropDownStyle.Toolbar);
                if (UI.Utilities.EndChangeCheck())
                {
                    UndoHandler.SaveState("Select Board");
                    renameObj.End();
                    SelectBoard(Boards.I.All.Find(o => o.Name == seleted));
                    Search();
                }
            }
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
                    currentBoardIndex = Boards.I.All.IndexOf(Boards.I.New());

                    Repaint();
                });
            }
        }
        private void DrawToolBarRenameBoard()
        {
            if (renameObj.Is(CurrentBoard))
            {
                UI.Utilities.Enabled = false;
                UI.Button(Language.Rename, UI.ButtonStyle.Toolbar);
                UI.Utilities.Enabled = true;
            }
            else
            {
                if (UI.Button(Language.Rename, UI.ButtonStyle.Toolbar))
                {
                    renameObj.Begin(CurrentBoard);
                }
            }
        }
        private void DrawToolBarDeleteBoard()
        {
            if (!CurrentBoard.HasGroupsWithLockedTask())
            {
                if (UI.Button(Language.Delete, UI.ButtonStyle.Toolbar))
                {
                    if (UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheBoard, Language.Yes, Language.No))
                    {
                        UndoHandler.SaveState("Delete Board");

                        MakeUpdateUISafe(() =>
                        {
                            Boards.I.Delete(CurrentBoard);

                            currentBoardIndex = 0;
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
        private void DrawToolBarNewGroup()
        {
            if (UI.Button(Language.NewGroup.ToTitleCase(), UI.ButtonStyle.Toolbar))
            {
                if (Event.current.button == 0)
                {
                    UndoHandler.SaveState("New Group");

                    Groups.I.New(CurrentBoard);
                }
                else
                {
                    GenericMenu menu = new GenericMenu();

                    //paste
                    if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Group)
                    {
                        menu.AddItem(new GUIContent(Language.PasteGroup), false, () =>
                        {
                            MakeUpdateUISafe(() =>
                            {
                                UndoHandler.SaveState("Paste Group");

                                if (Settings.BoardWindow.Group.PlaceNew == Settings.PlaceNewGroup.First)
                                {
                                    Groups.I.PasteNew(0, CurrentBoard);
                                }
                                else
                                {
                                    Groups.I.PasteNew(CurrentBoard.GroupIds.Count, CurrentBoard);
                                }
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
        private void DrawToolBarTreeView()
        {
            if (UI.Button(UI.Icons.TreeWindow, UI.ButtonStyle.Toolbar))
            {
                if (Event.current.button == 0)
                {
                    TreeWindow.Init();
                }
            }
        }
        private void DrawToolBarSearchQuery()
        {
            UI.Utilities.BeginChangeCheck();
            UI.Utilities.Name("board_search");
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
                        Prefs.SetBool("board_window_search_by_tag_include_all", searchByTagIncludeAll);
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
                        Prefs.SetBool("board_window_search_by_color_include_all", searchByColorIncludeAll);
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
            BoardInputUpdate(board);
            board.ScrollPosition = UI.BeginScrollView(board.ScrollPosition);

            UI.FixedSpace(boardPadding);
            UI.BeginHorizontal();
            UI.FixedSpace(boardPadding);

            int count = 0;
            int cols = (int)(Size.x / (Settings.BoardWindow.Group.FixedWidth + UI.Styles.Box.margin.left + UI.Styles.Box.margin.right));

            foreach (var groupId in board.GroupIds.ToArray())
            {
                Group group = Groups.I.Get(groupId);

                if (DrawGroup(group))
                {
                    if (Settings.BoardWindow.Group.Grid)
                    {
                        count++;
                        if (count == cols || cols == 0)
                        {
                            count = 0;
                            UI.EndHorizontal();
                            UI.BeginHorizontal();
                            UI.FixedSpace(boardPadding);
                        }
                    }
                }
            }

            UI.EndHorizontal();
            board.BoardWindow_Rect = UI.Utilities.LastRectOnRepaintRelative(board.BoardWindow_Rect);
            UI.EndScrollView();
            board.BoardWindow_ScrollRect = UI.Utilities.LastRectOnRepaintRelative(board.BoardWindow_ScrollRect);
        }
        private void BoardInputUpdate(Board board)
        {
            Event e = Event.current;

            if (renameObj.Is(board))
            {
                if (e.isMouse)
                {
                    if (!board.BoardWindow_NameTextFieldRect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                    {
                        renameObj.End();
                    }
                }
            }
        }
        private void SelectBoard(Board board)
        {
            currentBoardIndex = Boards.I.All.IndexOf(board);
            Repaint();
        }

        //group
        private bool DrawGroup(Group group)
        {
            //check for tasks visible in search
            if (Searching && Settings.BoardWindow.Group.HideEmptyWhenSearching)
            {
                bool hasTasksToShow = false;
                foreach (var taskId in group.TasksIds)
                {
                    Task task = Tasks.I.Get(taskId);

                    if (task.BoardWindow_SearchShow)
                    {
                        hasTasksToShow = true;
                        break;
                    }
                }

                if (!hasTasksToShow)
                {
                    group.BoardWindow_RectAbsolute = Rect.zero;
                    return false;
                }
            }

            UI.BeginVertical(GUILayout.Width(Settings.BoardWindow.Group.FixedWidth), GUILayout.MaxWidth(Settings.BoardWindow.Group.FixedWidth));

            //drag margin
            bool dragging = draggingGroup != null && draggingGroup == group;
            if (dragging)
                UI.FixedSpace(5);

            //box start
            if (Settings.BoardWindow.Group.Height == Settings.GroupSize.Fit)
            {
                UI.BeginBox(UI.BoxStyle.Normal);
            }
            else if (Settings.BoardWindow.Group.Height == Settings.GroupSize.Fixed)
            {
                UI.BeginBox(UI.BoxStyle.Normal, GUILayout.MinHeight(Settings.BoardWindow.Group.FixedHeight), GUILayout.MaxHeight(Settings.BoardWindow.Group.FixedHeight), GUILayout.Height(Settings.BoardWindow.Group.FixedHeight));
            }
            else if (Settings.BoardWindow.Group.Height == Settings.GroupSize.Fill)
            {
                float height = Size.y - CurrentBoard.BoardWindow_ScrollRect.position.y - boardPadding * 2 - UI.Styles.Box.margin.top - UI.Styles.Box.margin.bottom - (CurrentBoard.BoardWindow_Rect.size.x > CurrentBoard.BoardWindow_ScrollRect.size.x ? 20 : 0);
                UI.BeginBox(UI.BoxStyle.Normal, GUILayout.MinHeight(height), GUILayout.MaxHeight(height), GUILayout.Height(height));
            }
            UI.Utilities.ResetColor();

            GroupInputUpdate(group);
            DrawGroupName(group);

            UI.Space();
            DrawGroupTasks(group);
            DrawGroupNewTask(group);

            //end start
            UI.EndBox();

            //get rects
            group.BoardWindow_RectRelative = UI.Utilities.LastRectRelative();
            group.BoardWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(group.BoardWindow_RectAbsolute, Position);

            //drag margin
            UI.EndVertical();

            DrawGroupDeleteButton(group);

            return true;
        }
        private void DrawGroupName(Group group)
        {
            if (renameObj.Is(group))
            {
                renameObj.Draw();
            }
            else
            {
                UI.LabelWrap(string.IsNullOrEmpty(group.Name) ? Language.NONAME : group.Name, Settings.BoardWindow.Group.FixedWidth, UI.LabelWrapStyle.Centered);
                UI.Utilities.CursorIcon(MouseCursor.Link);
            }
            group.BoardWindow_NameRect = UI.Utilities.LastRectOnRepaintAbsolute(group.BoardWindow_NameRect, Position);
        }
        private void DrawGroupTasks(Group group)
        {
            //begin scrollview if layout is fill
            if (Settings.BoardWindow.Group.Height != Settings.GroupSize.Fit)
                group.ScrollPosition = UI.BeginScrollViewOnlyVertical(group.ScrollPosition);

            foreach (var taskID in group.TasksIds)
            {
                Task task = Tasks.I.Get(taskID);

                DrawTask(task);
            }
            UI.FixedSpace(10);

            //end scrollview if layout is fill
            if (Settings.BoardWindow.Group.Height != Settings.GroupSize.Fit)
            {
                UI.EndScrollView();
                UI.Space(true);
            }
        }
        private void DrawGroupNewTask(Group group)
        {
            if (!HideUIWhenDragging)
            {
                if (UI.Button(new GUIContent(Language.Task, UI.Icons.Plus), UI.ButtonStyle.Normal, false))
                {
                    Tasks.I.New(group);
                }
            }
        }
        private void DrawGroupDeleteButton(Group group)
        {
            if (!HideUIWhenDragging && !group.HasLockedTask())
            {
                if (!renameObj.Is(group))
                {
                    Rect rect = new Rect(group.BoardWindow_RectRelative.x + group.BoardWindow_RectRelative.width - 15, group.BoardWindow_RectRelative.y + 2, 20, 20);
                    if (GUI.Button(rect, new GUIContent(UI.Icons.XBox, Language.DeleteGroup), UI.Styles.None))
                    {
                        Groups.I.Delete(group, true, true);
                    }
                    UI.Utilities.CursorIcon(rect, MouseCursor.Link);
                }
            }
        }

        private void GroupDragUpdate()
        {
            if (draggingGroup != null)
            {
                foreach (var groupID in CurrentBoard.GroupIds.ToArray())
                {
                    Group group = Groups.I.Get(groupID);

                    Rect rect = new Rect(group.BoardWindow_RectAbsolute.position, new Vector2(group.BoardWindow_RectAbsolute.width, Size.y));
                    if (Settings.BoardWindow.Group.Grid)
                        rect = group.BoardWindow_RectAbsolute;

                    if (rect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                    {
                        if (group != draggingGroup)
                        {
                            MakeUpdateUISafe(() =>
                            {
                                int newIndex = CurrentBoard.GroupIds.IndexOf(group.ID);

                                CurrentBoard.GroupIds.Remove(draggingGroup.ID);
                                CurrentBoard.GroupIds.Insert(newIndex, draggingGroup.ID);
                                Save();
                            });
                            break;
                        }
                    }
                }

                Repaint();
            }
        }
        private void GroupInputUpdate(Group group)
        {
            Event e = Event.current;

            if (e.isMouse && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (e.clickCount == 1)
                    {
                        if (renameObj.Is(group))
                        {
                            if (!group.BoardWindow_NameRect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.End();
                            }
                        }
                        else
                        {
                            if (group.BoardWindow_NameRect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                draggingGroupHold = group;
                            }
                        }
                    }
                    else if (e.clickCount == 2)
                    {
                        if (!renameObj.Is(group))
                        {
                            if (group.BoardWindow_NameRect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                renameObj.Begin(group);
                            }
                        }
                    }
                }
                else if (e.button == 1)
                {
                    if (group.BoardWindow_NameRect.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                    {
                        Groups.I.ShowContextMenu(group, renameObj.Begin, Repaint, MakeUpdateUISafe, Groups.FlowDirection.LeftRight);
                    }
                }
            }
        }

        //task
        private void DrawTask(Task task)
        {
            //dont show hidden
            if (!task.BoardWindow_SearchShow)
                return;

            //check for imput
            TaskInputUpdate(task);


            //draggin margin
            UI.BeginHorizontal();
            bool dragging = draggingTask != null && draggingTask == task;
            if (dragging)
                UI.FixedSpace(5);

            UI.BeginBox(UI.BoxStyle.Round, GUILayout.MinHeight(Settings.BoardWindow.Task.MinHeight));

            DrawTaskName(task);
            DrawTaskLock(task);
            DrawTaskColors(task);
            DrawTaskProgress(task);

            UI.EndBox();

            //draggin margin
            if (dragging)
                UI.FixedSpace(5);
            UI.EndHorizontal();

            //get rects
            task.BoardWindow_RectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(task.BoardWindow_RectAbsolute, Position);
            task.BoardWindow_RectRelative = UI.Utilities.LastRectOnRepaintRelative(task.BoardWindow_RectRelative);

            //cursor
            if (!renameObj.Is(task))
            {
                UI.Utilities.CursorIcon();
            }

            //check last frame drag
            if (draggingTask != task)
                task.WasDraggedLastFrame = false;
        }
        private void DrawTaskName(Task task)
        {
            if (renameObj.Is(task))
            {
                renameObj.Draw();

                task.BoardWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(task.BoardWindow_RectNameAbsolute, Position);
                task.BoardWindow_RectNameRelative = UI.Utilities.LastRectOnRepaintRelative(task.BoardWindow_RectNameRelative);
            }
            else
            {
                int rightNameOffset = 5;
                UI.BeginHorizontal();
                string finalName = (string.IsNullOrEmpty(task.Name) ? Language.NONAME : task.Name) + (Settings.BoardWindow.Task.ShowDescriptionIndicator && !string.IsNullOrEmpty(task.Description) ? Settings.BoardWindow.Task.DescriptionIndicator : "");
                UI.LabelWrap(finalName, task.BoardWindow_RectNameAbsolute.width);
                task.BoardWindow_RectNameAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(task.BoardWindow_RectNameAbsolute, Position);
                task.BoardWindow_RectNameRelative = UI.Utilities.LastRectOnRepaintRelative(task.BoardWindow_RectNameRelative);
                UI.FixedSpace(rightNameOffset);
                UI.EndHorizontal();
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

                    GUI.DrawTexture(new Rect(task.BoardWindow_RectRelative.x, task.BoardWindow_RectRelative.y, 3, task.BoardWindow_RectRelative.height), UI.Icons.WhitePixel);
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

        private void TaskDragUpdate()
        {
            if (draggingTask == null)
                return;

            draggingTask.WasDraggedLastFrame = true;
            bool moved = false;

            foreach (var groupId in CurrentBoard.GroupIds)
            {
                if (moved)
                    break;

                Group group = Groups.I.Get(groupId);

                foreach (var taskId in group.TasksIds)
                {
                    Task task = Tasks.I.Get(taskId);

                    if (task.BoardWindow_SearchShow)
                    {
                        if (task.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            MakeUpdateUISafe(() =>
                            {
                                int newIndex = group.TasksIds.IndexOf(task.ID);

                                Group parentGroup = Groups.I.Get(draggingTask.ParentID);

                                parentGroup.TasksIds.Remove(draggingTask.ID);
                                group.TasksIds.Insert(newIndex, draggingTask.ID);
                                draggingTask.ParentID = group.ID;
                                Save();
                            });
                            moved = true;
                        }
                    }
                }

                if (!moved)
                {
                    if (group.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                    {
                        MakeUpdateUISafe(() =>
                        {
                            Group parentGroup = Groups.I.Get(draggingTask.ParentID);
                            parentGroup.TasksIds.Remove(draggingTask.ID);

                            group.TasksIds.Add(draggingTask.ID);
                            draggingTask.ParentID = group.ID;

                            Save();
                        });
                        moved = true;
                    }
                }
            }

            Repaint();
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
                        if (e.clickCount == 1)
                        {
                            if (task.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                if (!task.Locked && !renameObj.Is(task))
                                    draggingTaskHold = task;
                            }

                            if (!task.BoardWindow_RectNameAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                if (renameObj.Is(task))
                                {
                                    renameObj.End();
                                }
                            }

                            if (!task.WasDraggedLastFrame && e.button == 0 && !renameObj.Is(task))
                            {
                                if (task.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    UndoHandler.SaveState("Open Task");
                                    task.Open();
                                }
                            }
                        }
                        else if (e.clickCount == 2)
                        {
                            if (!renameObj.Is(task))
                            {
                                if (task.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                                {
                                    renameObj.Begin(task);
                                }
                            }
                        }
                    }
                    else if (e.button == 1 && !renameObj.Is(task))
                    {
                        if (task.BoardWindow_RectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                        {
                            Tasks.I.ShowContextMenu(task, renameObj.Begin, Repaint, MakeUpdateUISafe);
                        }
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                }
            }
        }

        //other
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
            if (CurrentBoard == null)
                return;

            MakeUpdateUISafe(() =>
            {
                foreach (var groupID in CurrentBoard.GroupIds)
                {
                    Group group = Groups.I.Get(groupID);

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

                    foreach (var taskID in group.TasksIds)
                    {
                        Task task = Tasks.I.Get(taskID);

                        task.BoardWindow_SearchShow = true;

                        if (searchQuery != string.Empty)
                        {
                            task.BoardWindow_SearchShow = false;

                            if (searchQueryFilter == string.Empty || searchQueryFilter == "n")
                            {
                                if (task.Name.ToLower().Contains(searchQuery.ToLower()))
                                    task.BoardWindow_SearchShow = true;
                            }
                            if (searchQueryFilter == string.Empty || searchQueryFilter == "d")
                            {
                                if (task.Description.ToLower().Contains(searchQuery.ToLower()))
                                    task.BoardWindow_SearchShow = true;
                            }
                        }

                        if (Settings.Tags.Enabled)
                        {
                            if (task.BoardWindow_SearchShow)
                            {
                                if (searchByTagIds.Count != 0)
                                {
                                    foreach (var filterTag in searchByTagIds)
                                    {
                                        if (searchByTagIncludeAll)
                                        {
                                            if (!task.TagIds.Contains(filterTag))
                                            {
                                                task.BoardWindow_SearchShow = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            task.BoardWindow_SearchShow = false;
                                            if (task.TagIds.Contains(filterTag))
                                            {
                                                task.BoardWindow_SearchShow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Settings.Colors.Enabled)
                        {
                            if (task.BoardWindow_SearchShow)
                            {
                                if (searchByColorIds.Count != 0)
                                {
                                    foreach (var filterColor in searchByColorIds)
                                    {
                                        if (searchByColorIncludeAll)
                                        {
                                            if (!task.ColorIds.Contains(filterColor))
                                            {
                                                task.BoardWindow_SearchShow = false;
                                            }
                                        }
                                        else
                                        {
                                            task.BoardWindow_SearchShow = false;
                                            if (task.ColorIds.Contains(filterColor))
                                            {
                                                task.BoardWindow_SearchShow = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Repaint();
            });
        }
        private void Save()
        {
            FileManager.SaveAll();
        }
        #endregion
    }
}
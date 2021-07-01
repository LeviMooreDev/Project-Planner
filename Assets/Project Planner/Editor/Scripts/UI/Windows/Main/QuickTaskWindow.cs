using ProjectPlanner.Popups;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectPlanner
{
    public class QuickTaskWindow : WindowBase
    {
        #region Fields
        public static QuickTaskWindow I
        {
            get
            {
                var windows = Resources.FindObjectsOfTypeAll<QuickTaskWindow>();
                if (windows == null || windows.Length == 0)
                    return null;
                else
                    return windows[0];
            }
        }

        //board
        [SerializeField]
        private string selectedBoardID = string.Empty;
        private Board SelectedBoard
        {
            get
            {
                return Boards.I.Get(selectedBoardID);
            }
        }

        //group
        [SerializeField]
        private string selectedGroupID = string.Empty;
        private Group SelectedGroup
        {
            get
            {
                return Groups.I.Get(selectedGroupID);
            }
        }

        //task
        [SerializeField]
        private Task task;

        //other
        private bool giveNameFocus;
        #endregion

        #region Methods
        public static void Init()
        {
            QuickTaskWindow window = GetWindow<QuickTaskWindow>(true, Language.QuickTask.ToTitleCase());
            window.minSize = window.maxSize = new Vector2(400, 280);
            window.CenterPosition();
            window.giveNameFocus = true;
            LoseFocus();
        }
        public static void CloseWindow()
        {
            GetWindow<QuickTaskWindow>(true, Language.QuickTask).Close();
        }

        protected override void OnWindowOpen()
        {
            Reset();
        }
        protected override void OnEditorReload()
        {
            selectedBoardID = Prefs.GetString("quick_task_window_board_id");
            selectedGroupID = Prefs.GetString("quick_task_window_group_id");
        }

        protected override void Input(Event e)
        {
            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.Return && (e.control || e.command))
                {
                    if (e.alt)
                    {
                        MakeUpdateUISafe(() =>
                        {
                            Create();
                            Close();
                        });
                    }
                    else
                    {
                        MakeUpdateUISafe(() =>
                        {
                            Create();
                            Reset();
                        });
                    }
                }
            }
        }
        protected override void OnDraw()
        {
            if (Boards.I.All.Count == 0)
            {
                DrawNoBoardMessage();
            }
            else
            {
                if (SelectedBoard == null)
                    selectedBoardID = Boards.I.All[0].ID;

                if (SelectedBoard.GroupIds.Count() == 0)
                {
                    DrawNoGroupMessage();
                }
                else
                {
                    if (SelectedGroup == null)
                        selectedGroupID = SelectedBoard.GroupIds[0];

                    DrawToolbar();
                    DrawName();
                    DrawDescription();
                    UI.Space();
                    UI.BeginHorizontal();
                    DrawAssets();
                    UI.Space();
                    DrawTags();
                    UI.Space();
                    DrawColor();
                    UI.EndHorizontal();
                }
            }
        }

        private void DrawToolbar()
        {
            UI.BeginToolbar();
            DrawToolbarBoards();
            DrawToolbarGroups();

            UI.FixedSpace(50);
            UI.Space(true);

            if (UI.Button(new GUIContent(Language.Add, "Ctrl+Enter"), UI.ButtonStyle.Toolbar))
            {
                Create();
                Reset();
            }
            if (UI.Button(new GUIContent(Language.AddAndClose, "Ctrl+Alt+Enter"), UI.ButtonStyle.Toolbar))
            {
                Create();
                Close();
            }
            UI.EndToolbar();
        }
        private void DrawToolbarBoards()
        {
            UI.Utilities.BeginChangeCheck();

            string seletedBoardName = UI.StringDropDown(SelectedBoard.Name, Boards.I.All.OrderBy(s => s.Name).Select(o => o.Name).ToList(), UI.EnumDropDownStyle.Toolbar);

            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Select Board");
                MakeUpdateUISafe(() =>
                {
                    SelectBoard(Boards.I.All.First(o => o.Name == seletedBoardName));
                    ResetSelectedGroup();
                });
            }
        }
        private void DrawToolbarGroups()
        {
            UI.Utilities.BeginChangeCheck();

            List<string> groupNames = new List<string>();
            foreach (var groupId in SelectedBoard.GroupIds)
            {
                groupNames.Add(Groups.I.Get(groupId).Name);
            }

            string seletedGroupName = UI.StringDropDown(SelectedGroup.Name, groupNames, UI.EnumDropDownStyle.Toolbar);

            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Select Group");
                MakeUpdateUISafe(() =>
                {
                    SelectGroup(Groups.I.All.Where(o => o.BoardID == selectedBoardID).First(o => o.Name == seletedGroupName));
                });
            }
        }
        private void DrawNoBoardMessage()
        {
            UI.BeginToolbar();
            UI.Utilities.Enabled = false;
            UI.StringDropDown("", new List<string>() { "" }, UI.EnumDropDownStyle.Toolbar);
            UI.StringDropDown("", new List<string>() { "" }, UI.EnumDropDownStyle.Toolbar);
            UI.FixedSpace(50);
            UI.Space(true);
            UI.Button(Language.Add, UI.ButtonStyle.Toolbar);
            UI.Button(Language.AddAndClose, UI.ButtonStyle.Toolbar);
            UI.Utilities.Enabled = true;
            UI.EndToolbar();

            UI.BeginHorizontal();
            UI.FixedSpace(50);
            UI.BeginVertical();
            UI.Space(true);
            UI.LabelWrap(Language.YouHaveToCreateALeastOneBoardToUseTheQuickTaskWindow, position.width - 100, UI.LabelWrapStyle.Centered);
            UI.Space(true);
            UI.EndVertical();
            UI.FixedSpace(50);
            UI.EndHorizontal();
        }
        private void DrawNoGroupMessage()
        {
            UI.BeginToolbar();
            DrawToolbarBoards();
            UI.Utilities.Enabled = false;
            UI.StringDropDown("", new List<string>() { "" }, UI.EnumDropDownStyle.Toolbar);
            UI.FixedSpace(50);
            UI.Space(true);
            UI.Button(Language.Add, UI.ButtonStyle.Toolbar);
            UI.Button(Language.AddAndClose, UI.ButtonStyle.Toolbar);
            UI.Utilities.Enabled = true;
            UI.EndToolbar();

            UI.BeginHorizontal();
            UI.FixedSpace(50);
            UI.BeginVertical();
            UI.Space(true);
            UI.LabelWrap(Language.YouHaveToSelectABoardWithAtLeastOneGroupInItToUseTheQuickTaskWindow, position.width - 100, UI.LabelWrapStyle.Centered);
            UI.Space(true);
            UI.EndVertical();
            UI.FixedSpace(50);
            UI.EndHorizontal();
        }

        private void DrawName()
        {
            UI.Utilities.BeginChangeCheck();
            UI.Label(Language.Name);

            UI.Utilities.Name("quick_task_name");
            string newName = UI.TextField(task.Name, Language.NONAME);
            if (giveNameFocus)
            {
                if (!UI.Utilities.HasFocus("quick_task_name"))
                {
                    UI.Utilities.Focus("quick_task_name");
                }
                else
                {
                    giveNameFocus = false;
                }
            }

            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Quick Task Name");
                SetTaskName(newName);
            }
        }
        private void DrawDescription()
        {
            UI.Utilities.BeginChangeCheck();
            UI.Label(Language.Description);

            string newDescription = UI.TextArea(task.Description, Language.Empty, 10);

            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Quick Task Description");
                SetTaskDescription(newDescription);
            }
        }
        private void DrawAssets()
        {
            if (Settings.Assets.Enabled)
            {
                if (UI.Button(new GUIContent(Language.Assets, UI.Icons.Folder), UI.ButtonStyle.None))
                {
                    UI.Utilities.ShowAssetsPopup(task.QuickTaskWindow_AssetsButtonRect, task.Assets, true, null);
                }
                task.QuickTaskWindow_AssetsButtonRect = UI.Utilities.LastRectOnRepaintRelative(task.QuickTaskWindow_AssetsButtonRect);
            }
        }
        private void DrawTags()
        {
            if (UI.Button(new GUIContent(Language.Tags, UI.Icons.Tag), UI.ButtonStyle.None))
            {
                UI.Utilities.ShowTagsPopup(task.QuickTaskWindow_TagsButtonRect, task.TagIds, TagsPopup.State.Edit, null);
            }
            task.QuickTaskWindow_TagsButtonRect = UI.Utilities.LastRectOnRepaintRelative(task.QuickTaskWindow_TagsButtonRect);
        }
        private void DrawColor()
        {
            if (UI.Button(new GUIContent(Language.Color, UI.Icons.Colors), UI.ButtonStyle.None))
            {
                UI.Utilities.ShowColorsPopup(task.QuickTaskWindow_ColorsButtonRect, task.ColorIds, ColorsPopup.State.Edit, null);
            }
            UI.Utilities.ResetColor();
            task.QuickTaskWindow_ColorsButtonRect = UI.Utilities.LastRectOnRepaintRelative(task.QuickTaskWindow_ColorsButtonRect);
        }

        private void SelectBoard(Board board)
        {
            selectedBoardID = board.ID;
            Prefs.SetString("quick_task_window_board_id", selectedBoardID);
            Repaint();
        }
        private void SelectGroup(Group group)
        {
            selectedGroupID = group.ID;
            Prefs.SetString("quick_task_window_group_id", selectedGroupID);
            Repaint();
        }
        private void ResetSelectedGroup()
        {
            selectedGroupID = "";
        }
        private void SetTaskName(string name)
        {
            task.Name = name;
            Repaint();
        }
        private void SetTaskDescription(string description)
        {
            task.Description = description;
            Repaint();
        }

        private void Create()
        {
            task.UpdateTimestamp();
            UndoHandler.SaveState("Add Quick Task");
            task.ParentID = selectedGroupID;
            Tasks.I.Add_USED_BY_QUICK_WINDOW_ONLY(task, SelectedGroup);
            Repaint();
        }
        private void Reset()
        {
            task = new Task().Constructor(null);
            giveNameFocus = true;
            UI.Utilities.LossFocus();
            Repaint();
        }
        #endregion
    }
}
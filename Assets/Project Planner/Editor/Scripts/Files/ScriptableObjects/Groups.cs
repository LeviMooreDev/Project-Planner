using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Groups : FileBase, ISerializationCallbackReceiver
    {
        public enum FlowDirection
        {
            UpDown,
            LeftRight
        }

        public static Groups I
        {
            get
            {
                return FileManager.GetFile<Groups>("Groups");
            }
        }

        //fields
        [SerializeField]
        private List<Group> _all;
        public List<Group> All
        {
            get
            {
                return _all;
            }
            private set
            {
                _all = value;
            }
        }
        private Dictionary<string, Group> IDlinks;

        //data
        public void Add_USED_BY_COPY_ONLY(Group group)
        {
            Add(group);
        }
        private void Add(Group group)
        {
            if (All == null)
                All = new List<Group>();
            if(!All.Contains(group))
                All.Add(group);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Group>();
            if(!IDlinks.ContainsKey(group.ID))
                IDlinks.Add(group.ID, group);

            FileManager.SaveAll();
        }
        public Group Get(string id)
        {
            if (IDlinks.ContainsKey(id))
                return IDlinks[id];

            return null;
        }

        //editing
        public void ShowContextMenu(Group group, Action<Group> renameCallback, Action repaintCallback, Action<Action> safeUICallback, FlowDirection flowDirection)
        {
            Board board = Boards.I.Get(group.BoardID);

            GenericMenu menu = new GenericMenu();

            //rename
            menu.AddItem(new GUIContent(Language.Rename), false, () => {
                renameCallback(group);
            });

            //duplicate
            menu.AddItem(new GUIContent(Language.Duplicate), false, () => {
                safeUICallback(() =>
                {
                    UndoHandler.SaveState("Duplicate Group");

                    Duplicate(group);

                    InvokeCallback(repaintCallback);
                });
            });

            //lock
            if (Settings.Locking.Enabled)
            {
                if (group.TasksIds.Count != 0)
                {
                    menu.AddItem(new GUIContent(Language.Lock + "/" + Language.LockTasks), false, () =>
                    {
                        safeUICallback(() =>
                        {
                            UndoHandler.SaveState("Lock Tasks");

                            LockGroupTasks(group);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.Lock + "/" + Language.LockTasks));
                }
                if (group.HasLockedTask())
                {
                    menu.AddItem(new GUIContent(Language.Lock + "/" + Language.UnlockTasks), false, () =>
                    {
                        safeUICallback(() =>
                        {
                            UndoHandler.SaveState("Unlock Tasks");

                            UnlockGroupTasks(group);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.Lock + "/" + Language.UnlockTasks));
                }
            }

            //sort
            if (board.GroupIds.Count != 0)
            {
                //name
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Name + " " + Language.ASC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Name ASC");

                        SortTasksName(group, true);

                        InvokeCallback(repaintCallback);
                    });
                });
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Name + " " + Language.DESC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Name DESC");

                        SortTasksName(group, false);

                        InvokeCallback(repaintCallback);
                    });
                });

                menu.AddSeparator(Language.Sort + "/");

                //created
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Created + " " + Language.ASC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Created");

                        SortTasksCreated(group, true);

                        InvokeCallback(repaintCallback);
                    });
                });
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Created + " " + Language.DESC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Created");

                        SortTasksCreated(group, false);

                        InvokeCallback(repaintCallback);
                    });
                });

                menu.AddSeparator(Language.Sort + "/");

                //updated
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Updated + " " + Language.ASC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Updated");

                        SortTasksUpdated(group, true);

                        InvokeCallback(repaintCallback);
                    });
                });
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Updated + " " + Language.DESC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Updated");

                        SortTasksUpdated(group, false);

                        InvokeCallback(repaintCallback);
                    });
                });

                menu.AddSeparator(Language.Sort + "/");

                //progress
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Progress + " " + Language.ASC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Progress");

                        SortTasksProgress(group, true);

                        InvokeCallback(repaintCallback);
                    });
                });
                menu.AddItem(new GUIContent(Language.Sort + "/" + Language.Progress + " " + Language.DESC), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Sort Tasks Progress");

                        SortTasksProgress(group, false);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.Sort + "/" + Language.Alphabetically));
            }

            //delete
            if (!group.HasLockedTask())
            {
                menu.AddItem(new GUIContent(Language.Delete), false, () =>
                {
                    safeUICallback(() =>
                    {
                        Delete(group, true, true);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.Delete + " [" + Language.Locked + "]"));
            }

            menu.AddSeparator("");

            //copy
            menu.AddItem(new GUIContent(Language.Copy), false, () =>
            {
                Copy(group);
            });

            //paste
            if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Group)
            {
                menu.AddItem(new GUIContent(Language.PasteAsNew), false, () =>
                {
                    safeUICallback.Invoke(() =>
                    {
                        UndoHandler.SaveState("Paste Group as New");

                        PasteNew(board.GroupIds.IndexOf(group.ID) + 1, board);

                        InvokeCallback(repaintCallback);
                    });
                });
                if (!group.HasLockedTask())
                {
                    menu.AddItem(new GUIContent(Language.PasteValues), false, () =>
                    {
                        safeUICallback.Invoke(() =>
                        {
                            UndoHandler.SaveState("Paste Group values");

                            PasteValues(group, board);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.PasteValues + " [" + Language.Locked + "]"));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.PasteAsNew));
                menu.AddDisabledItem(new GUIContent(Language.PasteValues));
            }

            menu.AddSeparator("");

            //new task
            menu.AddItem(new GUIContent(Language.NewTask), false, () =>
            {
                safeUICallback(() =>
                {
                    UndoHandler.SaveState("New Task");

                    Tasks.I.New(group);

                    InvokeCallback(repaintCallback);
                });
            });

            //paste task
            if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Task)
            {
                menu.AddItem(new GUIContent(Language.PasteTask), false, () =>
                {
                    safeUICallback.Invoke(() =>
                    {
                        UndoHandler.SaveState("Paste Task");

                        Tasks.I.PasteNew(0, group);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.PasteTask));
            }

            menu.AddSeparator("");

            //move left
            if (board.GroupIds.First() != group.ID)
            {
                menu.AddItem(new GUIContent(flowDirection == FlowDirection.LeftRight ? Language.MoveLeft : Language.MoveUp), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Move Group Left");

                        MoveLeft(group, board);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(flowDirection == FlowDirection.LeftRight ? Language.MoveLeft : Language.MoveUp));
            }

            //move right
            if (board.GroupIds.Last() != group.ID)
            {
                menu.AddItem(new GUIContent(flowDirection == FlowDirection.LeftRight ? Language.MoveRight : Language.MoveDown), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Move Group Right");

                        MoveRight(group, board);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(flowDirection == FlowDirection.LeftRight ? Language.MoveRight : Language.MoveDown));
            }

            //move to
            if (Boards.I.All.Count > 1)
            {
                foreach (var itemB in Boards.I.All.OrderBy(o => o.Name))
                {
                    if (itemB == board)
                        continue;

                    menu.AddItem(new GUIContent(Language.MoveToBoard + "/" + itemB.Name), false, () =>
                    {
                        safeUICallback(() =>
                        {
                            UndoHandler.SaveState("Move Group To " + itemB.Name);

                            MoveToBoard(group, itemB);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.MoveToBoard));
            }

            menu.ShowAsContext();
        }
        public Group New(Board board)
        {
            Group group = new Group().Constructor(board.ID).Default();
            if (Settings.BoardWindow.Group.PlaceNew == Settings.PlaceNewGroup.First)
            {
                board.GroupIds.Insert(0, group.ID);
            }
            else
            {
                board.GroupIds.Add(group.ID);
            }
            Add(group);
            ValidateNames(board);

            FileManager.SaveAll();

            return group;
        }
        public void Duplicate(Group group)
        {
            Board board = Boards.I.Get(group.BoardID);

            CopyHandler.CopyGroup(group);
            CopyHandler.PasteGroup(board, board.GroupIds.IndexOf(group.ID) + 1);
            ValidateNames(board);

            FileManager.SaveAll();
        }
        public void Delete(Group group, bool undo, bool dialog)
        {
            if (!dialog || UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheGroup, Language.Yes, Language.No))
            {
                if (undo)
                    UndoHandler.SaveState("Delete Group");

                Board parent = Boards.I.Get(group.BoardID);
                if (parent != null)
                    parent.GroupIds.Remove(group.ID);

                foreach (var taskId in group.TasksIds.ToArray())
                    Tasks.I.Delete(Tasks.I.Get(taskId), group);

                if (All == null)
                    All = new List<Group>();
                if (All.Contains(group))
                    All.Remove(group);

                if (IDlinks == null)
                    IDlinks = new Dictionary<string, Group>();
                if (IDlinks.ContainsKey(group.ID))
                    IDlinks.Remove(group.ID);
            }


            FileManager.SaveAll();
        }
        public void MoveToBoard(Group group, Board newBoard)
        {
            Boards.I.Get(group.BoardID).GroupIds.Remove(group.ID);
            newBoard.GroupIds.Insert(0, group.ID);
            group.BoardID = newBoard.ID;

            FileManager.SaveAll();
        }
        public void PasteValues(Group group, Board board)
        {
            CopyHandler.PasteGroup(board, board.GroupIds.IndexOf(group.ID) + 1);
            Delete(group, false, false);
            ValidateNames(board);

            FileManager.SaveAll();
        }
        public void PasteNew(int index, Board board)
        {
            CopyHandler.PasteGroup(board, index);
            ValidateNames(board);

            FileManager.SaveAll();
        }
        public void Copy(Group group)
        {
            CopyHandler.CopyGroup(group);
        }
        public void MoveLeft(Group group, Board parent)
        {
            if (parent.GroupIds.Count == 1)
                return;
            if (parent.GroupIds.First() == group.ID)
                return;

            int index = parent.GroupIds.IndexOf(group.ID);
            parent.GroupIds.RemoveAt(index);
            index--;
            parent.GroupIds.Insert(index, group.ID);

            FileManager.SaveAll();
        }
        public void MoveRight(Group group, Board parent)
        {
            if (parent.GroupIds.Count == 1)
                return;
            if (parent.GroupIds.Last() == group.ID)
                return;

            int index = parent.GroupIds.IndexOf(group.ID);
            parent.GroupIds.RemoveAt(index);
            index++;
            parent.GroupIds.Insert(index, group.ID);

            FileManager.SaveAll();
        }
        public void UnlockGroupTasks(Group group)
        {
            group.Unlock();

            FileManager.SaveAll();
        }
        public void LockGroupTasks(Group group)
        {
            group.Lock();

            FileManager.SaveAll();
        }
        public void SortTasksName(Group group, bool ASC)
        {
            group.TasksIds = group.TasksIds.OrderBy(o => Tasks.I.Get(o).Name).ToList();
            if (!ASC)
                group.TasksIds.Reverse();

            FileManager.SaveAll();
        }
        public void SortTasksCreated(Group group, bool ASC)
        {
            group.TasksIds = group.TasksIds.OrderBy(o => Tasks.I.Get(o).CreatedTicks).ToList();
            if (ASC)
                group.TasksIds.Reverse();

            FileManager.SaveAll();
        }
        public void SortTasksUpdated(Group group, bool ASC)
        {
            group.TasksIds = group.TasksIds.OrderBy(o => Tasks.I.Get(o).UpdatedTicks).ToList();
            if (ASC)
                group.TasksIds.Reverse();

            FileManager.SaveAll();
        }
        public void SortTasksProgress(Group group, bool ASC)
        {
            group.TasksIds = group.TasksIds.OrderBy(o => Tasks.I.Get(o).Progress).ThenBy(o => Tasks.I.Get(o).TasksIds.Count).ToList();
            if (ASC)
                group.TasksIds.Reverse();

            FileManager.SaveAll();
        }
        public void ValidateNames(Board board)
        {
            List<string> names = new List<string>();
            foreach (var group in All.Where(o => o.BoardID == board.ID))
            {
                int postfix = 1;
                string originalName = group.Name;

                while (names.Contains(group.Name) || group.Name == "" || string.IsNullOrEmpty(group.Name))
                {
                    group.Name = originalName + postfix;
                    postfix++;
                }

                if (group.Name != originalName)
                {
                    FileManager.SaveAll();
                }

                names.Add(group.Name);
            }
        }

        //helpers
        private void InvokeCallback(Action callback)
        {
            if (callback != null)
                callback.Invoke();
        }
        public override void Setup()
        {
            All = new List<Group>();
            IDlinks = new Dictionary<string, Group>();
        }
        public void OnAfterDeserialize()
        {
            IDlinks = new Dictionary<string, Group>();

            All.RemoveAll(item => item == null);
            foreach (var group in All.ToArray())
            {
                IDlinks.Add(group.ID, group);
            }
        }
        public void OnBeforeSerialize() { }
    }

    [CustomEditor(typeof(Groups))]
    public class GroupFileEditor : Editor { public override void OnInspectorGUI() { } }
}
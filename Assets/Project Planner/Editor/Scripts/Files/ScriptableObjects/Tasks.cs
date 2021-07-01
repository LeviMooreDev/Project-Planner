using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Tasks : FileBase, ISerializationCallbackReceiver
    {
        public static Tasks I
        {
            get
            {
                return FileManager.GetFile<Tasks>("Tasks");
            }
        }

        //fields
        [SerializeField]
        private List<Task> _all;
        public List<Task> All
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
        private Dictionary<string, Task> IDlinks;

        //data
        public void Add_USED_BY_QUICK_WINDOW_ONLY(Task task, IHaveTasks parent)
        {
            Add(task, parent);
        }
        public void Add_USED_BY_COPY_ONLY(Task task, IHaveTasks parent)
        {
            Add(task, parent);
        }
        private void Add(Task task, IHaveTasks parent)
        {
            if (All == null)
                All = new List<Task>();
            if(!All.Contains(task))
                All.Add(task);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Task>();
            if(!IDlinks.ContainsKey(task.ID))
                IDlinks.Add(task.ID, task);

            if(parent != null)
            {
                if (!parent.TasksIds.Contains(task.ID))
                {
                    parent.TasksIds.Add(task.ID);
                }
            }

            FileManager.SaveAll();
        }
        public Task Get(string id)
        {
            if (IDlinks.ContainsKey(id))
                return IDlinks[id];

            return null;
        }

        //edit
        public void ShowContextMenu(Task task, Action<Task> renameCallback, Action repaintCallback, Action<Action> safeUICallback)
        {
            IHaveTasks parent = task.IsParentATask ? (IHaveTasks)Get(task.ParentID) : Groups.I.Get(task.ParentID);

            GenericMenu menu = new GenericMenu();

            //open
            menu.AddItem(new GUIContent(Language.Open), false, () =>
            {
                UndoHandler.SaveState("Open Task");

                task.Open();
            });

            //rename
            if (!task.HasLockedTask())
            {
                menu.AddItem(new GUIContent(Language.Rename), false, () => {
                    renameCallback(task);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.Rename + " [" + Language.Locked + "]"));
            }

            //duplicate
            menu.AddItem(new GUIContent(Language.Duplicate), false, () =>
            {
                UndoHandler.SaveState("Duplicate Task");

                safeUICallback(() =>
                {
                    Duplicate(task, parent);

                    InvokeCallback(repaintCallback);
                });
            });

            //lock
            if (Settings.Locking.Enabled)
            {
                //with subtasks
                if (Settings.Subtasks.Enabled)
                {
                    if (task.Locked)
                    {
                        menu.AddItem(new GUIContent(Language.Lock + "/" + Language.Unlock), false, () =>
                        {
                            UndoHandler.SaveState("Unlock Task");

                            safeUICallback(() =>
                            {
                                Unlock(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(Language.Lock + "/" + Language.Lock), false, () =>
                        {
                            UndoHandler.SaveState("Lock Task");

                            safeUICallback(() =>
                            {
                                Lock(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }

                    menu.AddSeparator(Language.Lock + "/");
                    if (task.TasksIds.Count != 0)
                    {
                        menu.AddItem(new GUIContent(Language.Lock + "/" + Language.LockIncludingSubtasks), false, () =>
                        {
                            UndoHandler.SaveState("Lock Including Subtasks");

                            safeUICallback(() =>
                            {
                                LockAndSubtask(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Language.Lock + "/" + Language.LockIncludingSubtasks));
                    }
                    if (task.HasLockedTask())
                    {
                        menu.AddItem(new GUIContent(Language.Lock + "/" + Language.UnlockIncludingSubtasks), false, () =>
                        {
                            UndoHandler.SaveState("Unlock Including Subtasks");

                            safeUICallback(() =>
                            {
                                UnlockAndSubtask(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Language.Lock + "/" + Language.UnlockIncludingSubtasks));
                    }
                }
                //without subtasks
                else
                {
                    if (task.Locked)
                    {
                        menu.AddItem(new GUIContent(Language.Unlock), false, () =>
                        {
                            UndoHandler.SaveState("Unlock Task");

                            safeUICallback(() =>
                            {
                                Unlock(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(Language.Lock), false, () =>
                        {
                            UndoHandler.SaveState("Lock Task");

                            safeUICallback(() =>
                            {
                                Lock(task);

                                InvokeCallback(repaintCallback);
                            });
                        });
                    }
                }
            }

            //delete
            if (!task.HasLockedTask())
            {
                menu.AddItem(new GUIContent(Language.Delete), false, () =>
                {
                    if (UI.Utilities.Dialog(Language.DeleteTask, Language.AreYouSureYouWantToDeleteTheTask, Language.Yes, Language.No))
                    {
                        UndoHandler.SaveState("Delete Task");

                        safeUICallback(() =>
                        {
                            Delete(task);

                            InvokeCallback(repaintCallback);
                        });
                    }
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
                Copy(task);
            });

            //paste
            if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Task)
            {
                menu.AddItem(new GUIContent(Language.PasteAsNew), false, () =>
                {
                    UndoHandler.SaveState("Paste Task as New");

                    safeUICallback(() =>
                    {
                        PasteNew(parent.TasksIds.IndexOf(task.ID) + 1, parent);

                        InvokeCallback(repaintCallback);
                    });
                });
                if (!task.HasLockedTask())
                {
                    menu.AddItem(new GUIContent(Language.PasteValues), false, () =>
                    {
                        UndoHandler.SaveState("Paste Task values");

                        safeUICallback(() =>
                        {
                            PasteValues(task, parent);

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

            //new subtask
            menu.AddItem(new GUIContent(Language.NewSubtask), false, () =>
            {
                safeUICallback(() =>
                {
                    UndoHandler.SaveState("New Subtask");

                    New(task);

                    InvokeCallback(repaintCallback);
                });
            });

            //paste subtask
            if (CopyHandler.GetCopiedType == CopyHandler.CopiedType.Task)
            {
                menu.AddItem(new GUIContent(Language.PasteAsSubtask), false, () =>
                {
                    safeUICallback.Invoke(() =>
                    {
                        UndoHandler.SaveState("Paste Subtask");

                        PasteNew(0, task);
                        task.Open();

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.PasteTask));
            }

            menu.AddSeparator("");

            //move up
            if (parent.TasksIds.First() != task.ID)
            {
                if (!task.Locked)
                {
                    menu.AddItem(new GUIContent(Language.MoveUp), false, () =>
                    {
                        UndoHandler.SaveState("Move Task Up");

                        safeUICallback(() =>
                        {
                            MoveUp(task, parent);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.MoveUp + " [" + Language.Locked + "]"));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.MoveUp));
            }

            //move down
            if (parent.TasksIds.Last() != task.ID)
            {
                if (!task.Locked)
                {
                    menu.AddItem(new GUIContent(Language.MoveDown), false, () =>
                    {
                        UndoHandler.SaveState("Move Task Down");

                        safeUICallback(() =>
                        {
                            MoveDown(task, parent);

                            InvokeCallback(repaintCallback);
                        });
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.MoveDown + " [" + Language.Locked + "]"));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.MoveDown));
            }

            //move to
            if (Groups.I.All.Count > 1)
            {
                if (!task.Locked)
                {
                    foreach (var toBoard in Boards.I.All)
                    {
                        foreach (var toGroupId in toBoard.GroupIds)
                        {
                            if (toGroupId == task.ParentID)
                                continue;

                            Group toGroup = Groups.I.Get(toGroupId);

                            menu.AddItem(new GUIContent(Language.MoveToGroup + "/" + toBoard.Name + "/" + toGroup.Name), false, () =>
                            {
                                UndoHandler.SaveState("Move Task To " + toBoard.Name + "->" + toGroup.Name);

                                safeUICallback(() =>
                                {
                                    MoveToGroup(task, toGroup);

                                    InvokeCallback(repaintCallback);
                                });
                            });
                        }
                    }
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(Language.MoveToGroup + " [" + Language.Locked + "]"));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(Language.MoveToBoard));
            }

            menu.ShowAsContext();
        }
        public Task New(IHaveTasks parent, bool autoSelect = true)
        {
            Task task = new Task().Constructor(parent).Default();
            Add(task, parent);

            if (autoSelect && Settings.TaskGeneral.AutoSelectNew)
                task.Open();

            FileManager.SaveAll();

            return task;
        }
        public void Delete(Task task)
        {
            if (task.IsParentATask)
            {
                Delete(task, Get(task.ParentID));
            }
            else
            {
                Delete(task, Groups.I.Get(task.ParentID));
            }
        }
        public void Delete(Task task, IHaveTasks parent)
        {
            if (parent != null)
                parent.TasksIds.Remove(task.ID);

            foreach (var subtaskId in task.TasksIds.ToArray())
                Delete(Get(subtaskId), task);

            foreach (var screenshotId in task.Screenshots.ToArray())
                Screenshots.I.Delete(screenshotId);

            if (All == null)
                All = new List<Task>();
            if (All.Contains(task))
                All.Remove(task);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Task>();
            if (IDlinks.ContainsKey(task.ID))
                IDlinks.Remove(task.ID);

            FileManager.SaveAll();
        }
        public void Duplicate(Task task, IHaveTasks parent)
        {
            CopyHandler.CopyTask(task);
            CopyHandler.PasteTask(parent, parent.TasksIds.IndexOf(task.ID) + 1);

            FileManager.SaveAll();
        }
        public void MoveToGroup(Task task, IHaveTasks parent)
        {
            IHaveTasks oldParent = task.IsParentATask ? Get(task.ParentID) : (IHaveTasks)Groups.I.Get(task.ParentID);
            oldParent.TasksIds.Remove(task.ID);

            parent.TasksIds.Insert(0, task.ID);
            task.ParentID = parent.GetID();

            FileManager.SaveAll();
        }
        public void Copy(Task task)
        {
            CopyHandler.CopyTask(task);
        }
        public void UnlockAndSubtask(Task task)
        {
            task.Unlock(true);

            FileManager.SaveAll();
        }
        public void LockAndSubtask(Task task)
        {
            task.Lock(true);

            FileManager.SaveAll();
        }
        public void Lock(Task task)
        {
            task.Lock(false);

            FileManager.SaveAll();
        }
        public void Unlock(Task task)
        {
            task.Unlock(false);

            FileManager.SaveAll();
        }
        public void PasteNew(int index, IHaveTasks parent)
        {
            CopyHandler.PasteTask(parent, index);

            FileManager.SaveAll();
        }
        public void PasteValues(Task task, IHaveTasks parent)
        {
            Task pastedTask = CopyHandler.PasteTask(parent, parent.TasksIds.IndexOf(task.ID) + 1);
            Delete(task);

            if (TaskWindow.I.TaskID == task.ID)
                pastedTask.Open();

            FileManager.SaveAll();
        }
        public void MoveUp(Task task, IHaveTasks parent)
        {
            if (parent.TasksIds.Count == 1)
                return;
            if (parent.TasksIds.First() == task.ID)
                return;

            int index = parent.TasksIds.IndexOf(task.ID);
            parent.TasksIds.RemoveAt(index);
            index--;
            parent.TasksIds.Insert(index, task.ID);

            FileManager.SaveAll();
        }
        public void MoveDown(Task task, IHaveTasks parent)
        {
            if (parent.TasksIds.Count == 1)
                return;
            if (parent.TasksIds.Last() == task.ID)
                return;

            int index = parent.TasksIds.IndexOf(task.ID);
            parent.TasksIds.RemoveAt(index);
            index++;
            parent.TasksIds.Insert(index, task.ID);

            FileManager.SaveAll();
        }
        public void SetDone(Task task, bool value)
        {
            task.Done = value;

            FileManager.SaveAll();
        }

        //helpers
        private void InvokeCallback(Action callback)
        {
            if (callback != null)
                callback.Invoke();
        }
        public override void Setup()
        {
            All = new List<Task>();
            IDlinks = new Dictionary<string, Task>();
        }
        public void OnAfterDeserialize()
        {
            IDlinks = new Dictionary<string, Task>();

            All.RemoveAll(item => item == null);
            foreach (var task in All.ToArray())
            {
                IDlinks.Add(task.ID, task);
            }
        }
        public void OnBeforeSerialize() { }
    }

    [CustomEditor(typeof(Tasks))]
    public class TaskFileEditor : Editor { public override void OnInspectorGUI() { } }
}
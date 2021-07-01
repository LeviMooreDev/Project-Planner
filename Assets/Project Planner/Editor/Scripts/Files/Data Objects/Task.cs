using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Serialization;

namespace ProjectPlanner
{
    [Serializable]
    public class Task : ISerializationCallbackReceiver, IHaveTasks, IRename
    {
        public string ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }
        [SerializeField]
        private string _id;

        public string ParentID
        {
            get
            {
                return _parentID;
            }
            set
            {
                _parentID = value;
            }
        }
        [SerializeField]
        private string _parentID;

        public bool Locked
        {
            get
            {
                if (!Settings.Locking.Enabled)
                    return false;

                return _lock;
            }
            set
            {
                _lock = value;
            }
        }
        [SerializeField]
        private bool _lock;

        public bool Done
        {
            get
            {
                return _done;
            }
            set
            {
                _done = value;
            }
        }
        [SerializeField]
        private bool _done;
        
        public bool IsParentATask
        {
            get
            {
                return _isParentSubtask;
            }
            set
            {
                _isParentSubtask = value;
            }
        }
        [SerializeField]
        private bool _isParentSubtask;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        [SerializeField]
        private string _name;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        [SerializeField]
        private string _description;

        public bool ShowSubtasks
        {
            get
            {
                return _showSubtasks;
            }
            set
            {
                _showSubtasks = value;
            }
        }
        [SerializeField]
        private bool _showSubtasks;

        public List<string> TasksIds
        {
            get
            {
                return _tasksIds;
            }
            set
            {
                _tasksIds = value;
            }
        }
        [SerializeField]
        private List<string> _tasksIds;

        public bool HideCompletedChecklistItems
        {
            get
            {
                return _hideCompletedChecklistItems;
            }
            set
            {
                _hideCompletedChecklistItems = value;
            }
        }
        [SerializeField]
        private bool _hideCompletedChecklistItems;

        //Tags
        public bool ShowTags
        {
            get
            {
                return _showTags;
            }
            set
            {
                _showTags = value;
            }
        }
        [SerializeField]
        private bool _showTags;

        public List<string> TagIds
        {
            get
            {
                return _tagIds;
            }
            set
            {
                _tagIds = value;
            }
        }
        [SerializeField]
        private List<string> _tagIds;

        //Assets
        public bool ShowAssets
        {
            get
            {
                return _showAssets;
            }
            set
            {
                _showAssets = value;
            }
        }
        [SerializeField]
        private bool _showAssets;

        public List<UnityEngine.Object> Assets
        {
            get
            {
                return _assets;
            }
            set
            {
                _assets = value;
            }
        }
        [SerializeField]
        private List<UnityEngine.Object> _assets;

        //Colors
        public bool ShowColors
        {
            get
            {
                return _showColors;
            }
            set
            {
                _showColors = value;
            }
        }
        [SerializeField]
        private bool _showColors;

        public List<string> ColorIds
        {
            get
            {
                return _colorIds;
            }
            set
            {
                _colorIds = value;
            }
        }
        [SerializeField]
        private List<string> _colorIds;

        //Screenshots
        public List<string> Screenshots
        {
            get
            {
                return _screenshot;
            }
            set
            {
                _screenshot = value;
            }
        }
        [SerializeField]
        private List<string> _screenshot;

        public bool ShowScreenshots
        {
            get
            {
                return _showScreenshots;
            }
            set
            {
                _showScreenshots = value;
            }
        }
        [SerializeField]
        private bool _showScreenshots;

        //created
        public long CreatedTicks
        {
            get
            {
                return _createdTicks;
            }
            set
            {
                _createdTicks = value;
            }
        }
        [SerializeField]
        private long _createdTicks;
        private DateTime createdDateTime;
        public string CreatedFormatted
        {
            get
            {
                return createdDateTime.ToString("g");
            }
        }

        //updated
        public long UpdatedTicks
        {
            get
            {
                return _updatedTicks;
            }
            set
            {
                _updatedTicks = value;
            }
        }
        [SerializeField]
        private long _updatedTicks;
        private DateTime updatedDateTime;
        public string UpdatedFormatted
        {
            get
            {
                return updatedDateTime.ToString("g");
            }
        }

        public Vector2 ScrollPosition
        {
            get
            {
                return _scrollPosition;
            }
            set
            {
                _scrollPosition = value;
            }
        }
        [SerializeField]
        private Vector2 _scrollPosition;

        public float Progress
        {
            get
            {
                int total = ProgressNestedTasksCount;

                if (total == 0)
                {
                    return Done ? 1 : 0;
                }
                else
                {
                    int done = ProgressNestedTaskDone;
                    return 1f / total * done;
                }
            }
        }
        public string Progress100
        {
            get
            {
                return (int)(Progress * 100) + "%";
            }
        }
        public int ProgressNestedTasksCount
        {
            get
            {
                int count = 0;
                foreach (var taskId in TasksIds.ToArray())
                {
                    Task subtask = Tasks.I.Get(taskId);

                    if (subtask.TasksIds.Count == 0)
                    {
                        count++;
                    }
                    else
                    {
                        count += subtask.ProgressNestedTasksCount;
                    }
                }
                return count;
            }
        }
        public int ProgressNestedTaskDone
        {
            get
            {
                int done = 0;
                foreach (var taskId in TasksIds)
                {
                    Task subtask = Tasks.I.Get(taskId);

                    if (subtask.TasksIds.Count == 0)
                    {
                        done += subtask.Done ? 1 : 0;
                    }
                    else
                    {
                        done += subtask.ProgressNestedTaskDone;
                    }
                }
                return done;
            }
        }
        
        public bool WasDraggedLastFrame
        {
            get;
            set;
        }

        //Board window
        public Rect BoardWindow_RectAbsolute
        {
            get;
            set;
        }
        public Rect BoardWindow_RectRelative
        {
            get;
            set;
        }
        public Rect BoardWindow_RectNameAbsolute
        {
            get;
            set;
        }
        public Rect BoardWindow_RectNameRelative
        {
            get;
            set;
        }

        public bool BoardWindow_SearchShow
        {
            get
            {
                return _boardWindow_SearchShow;
            }
            set
            {
                _boardWindow_SearchShow = value;
            }
        }
        [SerializeField]
        [FormerlySerializedAs("_show")]
        private bool _boardWindow_SearchShow;

        //Task window
        public Rect TaskWindow_RectAbsolute
        {
            get;
            set;
        }
        public Rect TaskWindow_RectRelative
        {
            get;
            set;
        }
        public Rect TaskWindow_RectNameAbsolute
        {
            get;
            set;
        }

        public bool TaskWindow_ShowSubtask
        {
            get
            {
                return _taskWindow_ShowSubtask;
            }
            set
            {
                _taskWindow_ShowSubtask = value;
            }
        }
        [SerializeField]
        private bool _taskWindow_ShowSubtask;

        //Quick task window
        public Rect QuickTaskWindow_AssetsButtonRect
        {
            get;
            set;
        }
        public Rect QuickTaskWindow_TagsButtonRect
        {
            get;
            set;
        }
        public Rect QuickTaskWindow_ColorsButtonRect
        {
            get;
            set;
        }

        //Tree view window
        public Rect TreeWindow_RectAbsolute
        {
            get;
            set;
        }
        public Rect TreeWindow_RectRelative
        {
            get;
            set;
        }
        public Rect TreeWindow_BoxRectRelative
        {
            get;
            set;
        }
        public Rect TreeWindow_RectNameAbsolute
        {
            get;
            set;
        }
        public Rect TreeWindow_RectNameRelative
        {
            get;
            set;
        }

        public bool TreeWindow_SearchShow
        {
            get
            {
                return _treeWindow_SearchShow;
            }
            set
            {
                _treeWindow_SearchShow = value;
            }
        }
        [SerializeField]
        private bool _treeWindow_SearchShow = true;

        public AnimBool treeWindowShowSubtasksAnimation;
        public bool TreeWindow_ShowSubtasks
        {
            get
            {
                return _treeWindow_ShowSubtasks;
            }
            set
            {
                _treeWindow_ShowSubtasks = value;
            }
        }
        [SerializeField]
        private bool _treeWindow_ShowSubtasks;


        public Task Constructor(IHaveTasks parent)
        {
            if (parent != null)
            {
                ParentID = parent.GetID();
                IsParentATask = parent.IsATask();
            }

            ID = Info.GetNewID();
            BoardWindow_SearchShow = true;
            Name = string.Empty;
            Description = string.Empty;
            TasksIds = new List<string>();
            TagIds = new List<string>();
            Assets = new List<UnityEngine.Object>();
            Screenshots = new List<string>();
            ColorIds = new List<string>();
            createdDateTime = new DateTime();
            createdDateTime = DateTime.Now;
            CreatedTicks = createdDateTime.Ticks;
            updatedDateTime = new DateTime();
            UpdatedTicks = updatedDateTime.Ticks;

            return this;
        }
        public Task Default()
        {
            Name = Settings.TaskGeneral.DefaultName;
            ShowSubtasks = Settings.Subtasks.ShowByDefault;
            ShowAssets = Settings.Assets.ShowByDefault;
            ShowTags = Settings.Tags.ShowByDefault;
            ShowColors = Settings.Colors.ShowByDefault;
            ShowScreenshots = Settings.Screenshots.ShowByDefault;
            updatedDateTime = DateTime.Now;
            UpdatedTicks = updatedDateTime.Ticks;

            return this;
        }

        public void Open()
        {
            TaskWindow.Init().TaskID = ID;
        }
        public void UpdateTimestamp()
        {
            updatedDateTime = DateTime.Now;
            UpdatedTicks = updatedDateTime.Ticks;

            FileManager.SaveAll();
        }

        public void OnBeforeSerialize()
        {
            CreatedTicks = createdDateTime.Ticks;
            UpdatedTicks = updatedDateTime.Ticks;
        }
        public void OnAfterDeserialize()
        {
            createdDateTime = new DateTime(CreatedTicks);
            updatedDateTime = new DateTime(UpdatedTicks);
        }

        public bool HasLockedTask(bool includeSubtasks = true)
        {
            if (Locked)
                return true;

            if (Settings.Subtasks.Enabled && includeSubtasks)
            {
                foreach (var taskId in TasksIds)
                {
                    if (Tasks.I.Get(taskId).HasLockedTask(includeSubtasks))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Task Clone(out List<string> subtasks)
        {
            Task copiedTask = new Task().Constructor(null);

            copiedTask.Locked = Locked;
            copiedTask.Done = Done;
            copiedTask.IsParentATask = IsParentATask;
            copiedTask.Name = Name;
            copiedTask.Description = Description;
            copiedTask.ShowSubtasks = ShowSubtasks;
            copiedTask.ShowAssets = ShowAssets;
            copiedTask.ShowTags = ShowTags;
            copiedTask.ShowScreenshots = ShowScreenshots;
            copiedTask.HideCompletedChecklistItems = HideCompletedChecklistItems;
            copiedTask.CreatedTicks = CreatedTicks;
            copiedTask.UpdatedTicks = UpdatedTicks;
            copiedTask.ScrollPosition = ScrollPosition;

            if (Settings.Tags.Enabled)
                copiedTask.TagIds = new List<string>(TagIds);
            else
                copiedTask.TagIds = new List<string>();

            if (Settings.Colors.Enabled)
                copiedTask.ColorIds = new List<string>(ColorIds);
            else
                copiedTask.ColorIds = new List<string>();

            if (Settings.Assets.Enabled)
                copiedTask.Assets = new List<UnityEngine.Object>(Assets);
            else
                copiedTask.Assets = new List<UnityEngine.Object>();

            if (Settings.Screenshots.Enabled)
                copiedTask.Screenshots = new List<string>(Screenshots);
            else
                copiedTask.Screenshots = new List<string>();

            copiedTask.OnAfterDeserialize();

            if (Settings.Subtasks.Enabled)
                subtasks = new List<string>(TasksIds);
            else
                subtasks = new List<string>();

            return copiedTask;
        }

        public void Lock(bool recursive = false)
        {
            Locked = true;

            if (Settings.Subtasks.Enabled && recursive)
            {
                foreach (var tasksId in TasksIds)
                    Tasks.I.Get(tasksId).Lock(recursive);
            }

            FileManager.SaveAll();
        }
        public void Unlock(bool recursive = false)
        {
            Locked = false;

            if (Settings.Subtasks.Enabled && recursive)
            {
                foreach (var tasksId in TasksIds)
                    Tasks.I.Get(tasksId).Unlock(recursive);
            }

            FileManager.SaveAll();
        }

        public string GetID()
        {
            return ID;
        }
        public bool IsATask()
        {
            return true;
        }

        string IRename.GetName()
        {
            return Name;
        }
        void IRename.SetName(string value)
        {
            Name = value;
        }
        void IRename.Validate()
        {

        }
    }
}
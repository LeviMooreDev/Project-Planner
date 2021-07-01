using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Group : IHaveTasks, IRename
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

        public string BoardID
        {
            get
            {
                return _boardID;
            }
            set
            {
                _boardID = value;
            }
        }
        [SerializeField]
        private string _boardID;

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

        public List<string> TasksIds
        {
            get
            {
                return _taskIds;
            }
            set
            {
                _taskIds = value;
            }
        }
        [SerializeField]
        private List<string> _taskIds;

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

        //board window
        public bool BoardWindow_AnyTaskShown
        {
            get;
            set;
        }
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
        public Rect BoardWindow_NameRect
        {
            get;
            set;
        }

        //tree view
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

        public AnimBool TreeWindowShowAnimation;
        public bool TreeWindow_ShowTasks
        {
            get
            {
                return _treeWindow_ShowTasks;
            }
            set
            {
                _treeWindow_ShowTasks = value;
            }
        }
        [SerializeField]
        private bool _treeWindow_ShowTasks;

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

        public Group Constructor(string parentID)
        {
            BoardID = parentID;
            ID = Info.GetNewID();
            TasksIds = new List<string>();

            return this;
        }
        public Group Default()
        {
            Name = Settings.GroupGeneral.DefaultName;

            return this;
        }

        public bool HasLockedTask(bool includeSubtasks = true)
        {
            foreach (var taskId in TasksIds)
            {
                if (Tasks.I.Get(taskId).HasLockedTask(includeSubtasks))
                {
                    return true;
                }
            }
            return false;
        }

        public Group Clone(out List<string> subtasks)
        {
            Group copiedGroup = new Group().Constructor("");

            copiedGroup.Name = Name;
            copiedGroup.ScrollPosition = ScrollPosition;
            copiedGroup.TreeWindow_ShowTasks = TreeWindow_ShowTasks;
            copiedGroup.TreeWindowShowAnimation = new AnimBool(TreeWindowShowAnimation.value);
            subtasks = new List<string>(TasksIds);

            return copiedGroup;
        }

        public void Lock(bool recursive = false)
        {
            foreach (var tasksId in TasksIds)
                Tasks.I.Get(tasksId).Lock(recursive);

            FileManager.SaveAll();
        }
        public void Unlock(bool recursive = false)
        {
            foreach (var tasksId in TasksIds)
                Tasks.I.Get(tasksId).Unlock(recursive);

            FileManager.SaveAll();
        }

        public string GetID()
        {
            return ID;
        }
        public bool IsATask()
        {
            return false;
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
            Groups.I.ValidateNames(Boards.I.Get(BoardID));
        }
    }
}
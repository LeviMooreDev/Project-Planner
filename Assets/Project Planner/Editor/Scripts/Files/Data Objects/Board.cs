using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Board : IRename
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

        public List<string> GroupIds
        {
            get
            {
                return _groupIds;
            }
            set
            {
                _groupIds = value;
            }
        }
        [SerializeField]
        private List<string> _groupIds;

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

        public bool WasDraggedLastFrame
        {
            get;
            set;
        }

        //board window
        public Rect BoardWindow_Rect
        {
            get;
            set;
        }
        public Rect BoardWindow_ScrollRect
        {
            get;
            set;
        }
        public Rect BoardWindow_NameTextFieldRect
        {
            get;
            set;
        }

        //tree window
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

        public AnimBool treeWindowShowGroupsAnimation;
        public bool TreeWindow_ShowGroups
        {
            get
            {
                return _treeWindow_ShowGroups;
            }
            set
            {
                _treeWindow_ShowGroups = value;
            }
        }
        [SerializeField]
        private bool _treeWindow_ShowGroups;

        public Rect TreeWindow_RectAbsolute
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

        public Board Constructor()
        {
            ID = Info.GetNewID();
            GroupIds = new List<string>();
            
            return this;
        }
        public Board Default()
        {
            Name = Settings.BoardGeneral.DefaultName;
            foreach (var defaultGroupName in Settings.BoardGeneral.DefaultGroups)
            {
                Group group = Groups.I.New(this);
                group.Name = defaultGroupName;
            }
            Groups.I.ValidateNames(this);

            return this;
        }

        public bool HasGroupsWithLockedTask(bool includeSubtasks = true)
        {
            foreach (var groupId in GroupIds)
            {
                if (Groups.I.Get(groupId).HasLockedTask(includeSubtasks))
                {
                    return true;
                }
            }
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
            Boards.I.ValidateBoardNames();
        }
    }
}
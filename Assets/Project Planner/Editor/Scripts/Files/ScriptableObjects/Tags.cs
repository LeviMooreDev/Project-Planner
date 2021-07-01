using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public class Tags : FileBase, ISerializationCallbackReceiver
    {
        public static Tags I
        {
            get
            {
                return FileManager.GetFile<Tags>("Tags");
            }
        }

        /// <summary>
        /// Key is ID. value is display text.
        /// </summary>
        public Dictionary<string, string> Values
        {
            get;
            set;
        }
        public Action OnDeleted { get; set; }

        [SerializeField]
        private List<string> _keys;

        [SerializeField]
        private List<string> _values;

        public string Add(string tag, bool undo = true)
        {
            tag = tag.Trim();

            if (tag != string.Empty)
            {
                foreach (var item in Values)
                {
                    if (item.Value.ToLower() == tag.ToLower())
                    {
                        return item.Key;
                    }
                }

                if (undo)
                    UndoHandler.SaveState("New Tag");

                string id = Info.GetNewID();
                Values.Add(id, tag);
                Sort();
                FileManager.SaveAll();
                return id;
            }

            return null;
        }
        public void Remove(string key)
        {
            if (Values.ContainsKey(key))
                Values.Remove(key);

            if (OnDeleted != null)
                OnDeleted.Invoke();
        }
        public void Sort()
        {
            var sorted = Values.OrderBy(o => o.Value);
            Values = new Dictionary<string, string>();
            foreach (var item in sorted)
            {
                Values.Add(item.Key, item.Value);
            }
        }
        public bool RemoveInvalidIDs(List<string> ids)
        {
            bool foundIDToDelete = false;

            foreach (var item in ids.ToList())
            {
                if (!Values.ContainsKey(item))
                {
                    foundIDToDelete = true;
                    ids.Remove(item);
                }
            }

            return foundIDToDelete;
        }

        public void OnBeforeSerialize()
        {
            if (Values == null)
                Values = new Dictionary<string, string>();

            _keys = new List<string>();
            _values = new List<string>();

            foreach (var kvp in Values)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
        public void OnAfterDeserialize()
        {
            Values = new Dictionary<string, string>();

            for (int i = 0; i < _keys.Count; i++)
            {
                Values.Add(_keys[i], _values[i]);
            }
        }
    }

    [CustomEditor(typeof(Tags))]
    public class TagsCustomEditor : Editor { public override void OnInspectorGUI() { } }
}
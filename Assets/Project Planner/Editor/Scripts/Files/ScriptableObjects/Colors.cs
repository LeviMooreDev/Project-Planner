using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public class Colors : FileBase, ISerializationCallbackReceiver
    {
        public static Colors I
        {
            get
            {
                return FileManager.GetFile<Colors>("Colors");
            }
        }

        public Dictionary<string, Color> Values
        {
            get;
            set;
        }
        public Action OnDeleted { get; set; }

        [SerializeField]
        private List<string> _keys;

        [SerializeField]
        private List<Color> _values;

        public string Add(Color color, bool undo = true)
        {
            foreach (var item in Values)
            {
                if (item.Value == color)
                {
                    return item.Key;
                }
            }

            if (undo)
                UndoHandler.SaveState("New Color");

            string id = Info.GetNewID();
            Values.Add(id, color);
            FileManager.SaveAll();

            return id;
        }
        public void Remove(string key)
        {
            if (Values.ContainsKey(key))
                Values.Remove(key);

            if(OnDeleted != null)
                OnDeleted.Invoke();
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
                Values = new Dictionary<string, Color>();

            _keys = new List<string>();
            _values = new List<Color>();

            foreach (var kvp in Values)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
        public void OnAfterDeserialize()
        {
            Values = new Dictionary<string, Color>();

            for (int i = 0; i < _keys.Count; i++)
            {
                Values.Add(_keys[i], _values[i]);
            }
        }
    }

    [CustomEditor(typeof(Colors))]
    public class ColorsCustomEditor : Editor { public override void OnInspectorGUI() { } }
}

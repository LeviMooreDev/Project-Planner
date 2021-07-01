using ProjectPlanner.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [InitializeOnLoad]
    static class UndoHandler
    {
        static UndoHandler()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private static UnityEngine.Object[] Objects
        {
            get
            {
                List<UnityEngine.Object> objectsToSave = new List<UnityEngine.Object>() { Boards.I, Groups.I, Tasks.I, Tags.I, Colors.I, Screenshots.I };

                foreach (var item in Resources.FindObjectsOfTypeAll<WindowBase>())
                {
                    objectsToSave.Add(item);
                }

                return objectsToSave.ToArray();
            }
        }

        private static void OnUndoRedo()
        {
            PopupBase.LoseFocus();
            WindowBase.LoseFocus();
            Screenshots.I.RecorverTmps();
            FileManager.SaveAll();
        }

        public static void SaveState(string actionName)
        {
            if (Settings.Other.SupportUndo)
            {
                Undo.FlushUndoRecordObjects();
                Undo.RegisterCompleteObjectUndo(Objects, "PP - " + actionName);
            }
        }

        public static void Clear(UnityEngine.Object window)
        {
            if (Settings.Other.SupportUndo)
            {
                Undo.ClearUndo(window);

                var allWindows = Resources.FindObjectsOfTypeAll<WindowBase>();
                if (allWindows == null || allWindows.Length <= 1)
                {
                    foreach (var _object in Objects)
                    {
                        Undo.ClearUndo(_object);
                    }
                }
            }
        }

        public static int Group(string name)
        {
            Undo.SetCurrentGroupName(name);
            return Undo.GetCurrentGroup();
        }
        public static void MergeGroup(int id)
        {
            Undo.CollapseUndoOperations(id);
        }
    }
}

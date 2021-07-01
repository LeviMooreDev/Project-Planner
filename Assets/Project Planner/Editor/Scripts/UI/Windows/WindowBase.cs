using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ProjectPlanner
{
    [Serializable]
    public abstract class WindowBase : EditorWindow, IHasCustomMenu
    {
        public Vector2 Position
        {
            get
            {
                return position.position;
            }
        }

        public Vector2 Size
        {
            get
            {
                return position.size;
            }
        }

        /// <summary>
        /// Used to check if somethking has happen that made the editor reload. E.g. "Enter and exit of play mode", "Script recompiling"
        /// Is reset to true in editor reload.
        /// </summary>
        [NonSerialized]
        private bool reloadSetupNeeded = true;
        
        /// <summary>
        /// Is false if the window has just been open.
        /// Used to check if we need to setup our serialized fields for the first time.
        /// </summary>
        [SerializeField]
        private bool initialized;

        /// <summary>
        /// Actions to call when it is safe to make changes to the UI.
        /// </summary>
        [SerializeField]
        private Action uiChanges;

        [NonSerialized]
        public bool canUpdate = false;

        private static int targetLossFocusId;
        protected int lossFocusId;

        protected virtual void OnWindowOpen() { }
        protected virtual void OnEditorReload() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnDraw() { }
        protected virtual void Input(Event e) { }

        /// <summary>
        /// Makes a action that changes the UI "safe".
        /// This is done by waiting for the EventType to be Layout before invoking the action.
        /// </summary>
        /// <param name="action"></param>
        public void MakeUpdateUISafe(Action action)
        {
            if (action != null)
                uiChanges += action;
        }

        public void OnInspectorUpdate()
        {
            if(canUpdate)
                OnUpdate();

            Repaint();
        }

        public void OnGUI()
        {
            if (targetLossFocusId != lossFocusId)
            {
                lossFocusId = targetLossFocusId;
                UI.Utilities.LossFocus();
            }

            if (reloadSetupNeeded)
            {
                reloadSetupNeeded = false;
                OnEditorReload();
            }
            
            if (!initialized)
            {
                initialized = true;
                OnWindowOpen();
            }
            
            if (Event.current.type == EventType.Layout)
            {
                if (uiChanges != null)
                {
                    uiChanges.Invoke();
                }

                uiChanges = null;
            }
            
            Input(Event.current);

            UI.BeginVertical();
            OnDraw();
            UI.EndVertical();

            canUpdate = true;
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public void OnDestroy()
        {
            UndoHandler.Clear(this);
        }

        public static void LoseFocus()
        {
            targetLossFocusId++;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent(Language.Settings), false, () => { PreferencesWindow.Init(); });
        }
    }

    public static class Extensions
    {
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();

            if (containerWinType == null)
                return Rect.zero;

            var showModeField = containerWinType.GetField("m_ShowMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (showModeField == null || positionProperty == null)
                return Rect.zero;

            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int)showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            return Rect.zero;
        }

        public static void CenterPosition(this UnityEditor.EditorWindow aWin)
        {
            try
            {
                var main = GetEditorMainWindowPos();
                if (main != Rect.zero)
                {
                    var pos = aWin.position;
                    float w = (main.width - pos.width) * 0.5f;
                    float h = (main.height - pos.height) * 0.5f;
                    pos.x = main.x + w;
                    pos.y = main.y + h;
                    aWin.position = pos;
                }
            }
            catch (Exception) { }
        }
    }
}
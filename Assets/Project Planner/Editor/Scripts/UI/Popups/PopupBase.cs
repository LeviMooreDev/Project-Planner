using System;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner.Popups
{
    public class PopupBase : PopupWindowContent
    {
        private readonly Action onClose;
        protected Action OnUpdate
        {
            get;
            private set;
        }
        private readonly PopupOption[] options;
        private readonly bool showSearch;
        protected string Search
        {
            get;
            private set;
        }
        private Vector2 scrollPosition;

        private static int targetLossFocusId;
        protected int lossFocusId;

        protected PopupBase(bool showSearch, Action onClose, Action onUpdate, PopupOption[] options)
        {
            this.showSearch = showSearch;
            this.onClose = onClose;
            this.OnUpdate = onUpdate;
            this.options = options;
            Search = string.Empty;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(175, 275);
        }

        public static void LoseFocus()
        {
            targetLossFocusId++;
        }

        public override void OnClose()
        {
            FileManager.SaveAll();

            if (onClose != null)
                onClose.Invoke();
        }
        protected virtual void DrawBeforeScroll() { }
        protected virtual void DrawScroll() { }
        protected virtual void DrawAfterScroll() { }

        public override void OnGUI(Rect rect)
        {
            if (targetLossFocusId != lossFocusId)
            {
                lossFocusId = targetLossFocusId;
                UI.Utilities.LossFocus();
            }

            DrawToolbar();

            DrawBeforeScroll();
            scrollPosition = UI.BeginScrollView(scrollPosition);
            DrawScroll();
            UI.EndScrollView();
            DrawAfterScroll();

            editorWindow.Repaint();
        }

        private void DrawToolbar()
        {
            UI.BeginToolbar();
            if (showSearch)
                Search = UI.SearchField(Search);
            else
                UI.Space(true);

            if (options != null)
            {
                if (UI.Button(UI.Icons.Options, UI.ButtonStyle.Toolbar))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var option in options)
                    {
                        if (option.GetType() == typeof(PopupOptionToggle))
                        {
                            menu.AddItem(new GUIContent(option.Name), ((PopupOptionToggle)option).On, () => {
                                ((PopupOptionToggle)option).On = !((PopupOptionToggle)option).On;

                                if (option.Action != null)
                                    option.Action.Invoke();
                                if (OnUpdate != null)
                                    OnUpdate.Invoke();
                            });
                        }
                        else if (option.GetType() == typeof(PopupOptionButton))
                        {
                            menu.AddItem(new GUIContent(option.Name), false, () => {
                                if (option.Action != null)
                                    option.Action.Invoke();
                                if (OnUpdate != null)
                                    OnUpdate.Invoke();
                            });
                        }
                    }
                    menu.ShowAsContext();
                }
            }
            UI.EndToolbar();
        }
    }
}
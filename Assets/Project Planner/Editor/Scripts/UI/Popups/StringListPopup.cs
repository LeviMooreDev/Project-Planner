using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner.Popups
{
    public class StringListPopup : PopupBase
    {
        private readonly List<string> values;
        private string newValue = string.Empty;
        private readonly Action<List<string>> onClose;

        public StringListPopup(List<string> values, Action<List<string>> onClose, PopupOption[] options) : base(true, null, null, options)
        {
            this.values = values;
            this.onClose = onClose;
        }

        protected override void DrawScroll()
        {
            if (values.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
            else
            {
                int remove = -1;
                bool empty = true;

                for (int i = 0; i < values.Count; i++)
                {
                    if (!values[i].Contains(Search))
                        continue;
                    else
                        empty = false;

                    UI.BeginHorizontal();

                    values[i] = UI.TextField(values[i]);

                    if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                    {
                        remove = i;
                    }

                    UI.EndHorizontal();
                }

                if (remove != -1)
                    values.RemoveAt(remove);

                if (empty)
                {
                    UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
                }
            }
        }
        protected override void DrawAfterScroll()
        {
            UI.Line();
            UI.BeginHorizontal();

            newValue = UI.TextField(newValue);

            if (UI.Button(Language.Add))
            {
                if (newValue != string.Empty)
                {
                    values.Add(newValue);
                    newValue = string.Empty;
                }

                UI.Utilities.LossFocus();
            }
            UI.EndHorizontal();
        }

        public override void OnClose()
        {
            if (onClose != null)
                onClose.Invoke(values);

            base.OnClose();
        }
    }
}
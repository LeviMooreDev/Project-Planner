using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner.Popups
{
    public class ColorsPopup : PopupBase
    {
        public enum State
        {
            Edit,
            Select,
            ViewSelected
        }

        private int undoGroupId = -1;
        private bool mergeUndo;
        private readonly List<string> selected;
        private readonly State state;
        private Color newColor;

        public ColorsPopup(List<string> selected, State state, Action onClose, Action onUpdate, PopupOption[] options = null) : base(false, onClose, onUpdate, options)
        {
            this.selected = selected;
            this.state = state;
            newColor = Color.white;
        }

        protected override void DrawScroll()
        {
            UI.FixedSpace(3);

            if (Colors.I.Values.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
            else
            {
                foreach (var item in new Dictionary<string, Color>(Colors.I.Values))
                {
                    if (state == State.ViewSelected)
                    {
                        if (!selected.Contains(item.Key))
                        {
                            continue;
                        }
                    }

                    UI.BeginHorizontal();

                    if (state != State.ViewSelected)
                    {
                        bool on = selected.Contains(item.Key);
                        UI.Utilities.BeginChangeCheck();
                        on = UI.Toggle(on);
                        if (UI.Utilities.EndChangeCheck())
                        {
                            if (on && !selected.Contains(item.Key))
                            {
                                SaveState("Select Color");
                                selected.Add(item.Key);
                                FileManager.SaveAll();
                            }
                            else if (selected.Contains(item.Key))
                            {
                                SaveState("Deselect Color");
                                selected.Remove(item.Key);
                                FileManager.SaveAll();
                            }

                            if (OnUpdate != null)
                                OnUpdate.Invoke();
                        }
                    }

                    if (state == State.Edit)
                    {
                        UI.Utilities.BeginChangeCheck();
                        Color newColor = UI.ColorField(item.Value, false);
                        if (UI.Utilities.EndChangeCheck())
                        {
                            SaveState("Change Color");
                            Colors.I.Values[item.Key] = newColor;
                            FileManager.SaveAll();
                        }

                        if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                        {
                            if (!Settings.Colors.ConfirmRemoval || UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheColor, Language.Yes, Language.No))
                            {
                                SaveState("Delete Color");
                                Colors.I.Values.Remove(item.Key);
                                FileManager.SaveAll();
                            }
                        }
                    }
                    else
                    {
                        UI.ColorDisplay(item.Value);
                        UI.Space();
                    }

                    UI.EndHorizontal();
                }
            }

            UI.Space();
        }
        protected override void DrawAfterScroll()
        {
            if (state == State.Edit)
            {
                UI.Line();

                UI.BeginHorizontal();

                newColor = UI.ColorField(newColor, false);

                if (UI.Button(Language.Add))
                {
                    SaveState("New Color");
                    Colors.I.Add(newColor, false);
                    newColor = Color.white;
                    UI.Utilities.LossFocus();
                }
                UI.EndHorizontal();
                UI.FixedSpace(3);
            }
        }

        private void SaveState(string name)
        {
            if(undoGroupId == -1)
                undoGroupId = UndoHandler.Group("Colors Popup");
            mergeUndo = true;
            UndoHandler.SaveState(name);
        }

        public override void OnClose()
        {
            if(mergeUndo)
                UndoHandler.MergeGroup(undoGroupId);

            base.OnClose();
        }
    }
}
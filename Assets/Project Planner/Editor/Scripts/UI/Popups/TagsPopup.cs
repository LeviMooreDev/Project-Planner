using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner.Popups
{
    public class TagsPopup : PopupBase
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
        private string newTag = string.Empty;

        public TagsPopup(List<string> selected, State state, Action onClose, Action onUpdate, PopupOption[] options) : base(true, onClose, onUpdate, options)
        {
            this.selected = selected;
            this.state = state;
        }

        protected override void DrawScroll()
        {
            if (Tags.I.Values.Count == 0)
            {
                UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
            }
            else
            {
                bool empty = true;
                foreach (var item in new Dictionary<string, string>(Tags.I.Values))
                {
                    if (state == State.ViewSelected)
                    {
                        if (!selected.Contains(item.Key))
                        {
                            continue;
                        }
                    }


                    if (!item.Value.ToLower().Contains(Search.ToLower()))
                        continue;
                    else
                        empty = false;

                    UI.BeginHorizontal();

                    if (state != State.ViewSelected)
                    {
                        //if the current tag is in the list of selected tags set on to true.
                        bool on = selected.Contains(item.Key);
                        UI.Utilities.BeginChangeCheck();
                        on = UI.Toggle(on);

                        //if the value of the toggle has changed, update the selected tags list
                        if (UI.Utilities.EndChangeCheck())
                        {
                            if (on && !selected.Contains(item.Key))
                            {
                                SaveState("Select Tag");
                                selected.Add(item.Key);
                                FileManager.SaveAll();
                            }
                            else if (selected.Contains(item.Key))
                            {
                                SaveState("Deselect Tag");
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
                        string newValue = UI.TextField(Tags.I.Values[item.Key]);

                        if (UI.Utilities.EndChangeCheck())
                        {
                            newValue = newValue.Trim();
                            if (newValue != string.Empty)
                            {
                                if (!Tags.I.Values.ContainsValue(newValue.ToLower()))
                                {
                                    SaveState("Change Tag");
                                    Tags.I.Values[item.Key] = newValue;
                                    FileManager.SaveAll();
                                }
                            }
                        }

                        if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                        {
                            if (!Settings.Tags.ConfirmRemoval || UI.Utilities.Dialog(Language.Delete, Language.AreYouSureYouWantToDeleteTheTag, Language.Yes, Language.No))
                            {
                                SaveState("Delete Tag");
                                Tags.I.Values.Remove(item.Key);
                                FileManager.SaveAll();
                            }
                        }
                    }
                    else
                    {
                        UI.Label(Tags.I.Values[item.Key]);
                    }
                    UI.EndHorizontal();
                }

                if (empty)
                {
                    UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
                }
            }
        }
        protected override void DrawAfterScroll()
        {
            if (state == State.Edit)
            {
                UI.Line();

                UI.BeginHorizontal();

                newTag = UI.TextField(newTag, Language.Tag);

                if (UI.Button(Language.Add))
                {
                    SaveState("New Tag");
                    Tags.I.Add(newTag, false);
                    newTag = string.Empty;
                    newTag = newTag.Trim();

                    UI.Utilities.LossFocus();
                }
                UI.EndHorizontal();
            }
        }

        private void SaveState(string name)
        {
            if (undoGroupId == -1)
                undoGroupId = UndoHandler.Group("Tags Popup");
            mergeUndo = true;
            UndoHandler.SaveState(name);
        }

        public override void OnOpen()
        {
            Tags.I.Sort();
        }
        public override void OnClose()
        {
            if(mergeUndo)
                UndoHandler.MergeGroup(undoGroupId);

            base.OnClose();
        }
    }
}
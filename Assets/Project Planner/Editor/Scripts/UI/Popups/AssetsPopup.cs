using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner.Popups
{
    public class AssetsPopup : PopupBase
    {
        private readonly List<UnityEngine.Object> assets;
        private readonly List<UnityEngine.Object> editAssets;

        private readonly bool canEdit;
        private bool needUndo;

        public AssetsPopup(List<UnityEngine.Object> assets, bool canEdit, Action onClose, PopupOption[] options) : base(true, onClose, null, options)
        {
            editAssets = assets.ToList();
            this.assets = assets;
            this.canEdit = canEdit;
        }

        protected override void DrawScroll()
        {
            if (canEdit)
            {
                //list
                UI.Utilities.BeginChangeCheck();
                for (int i = 0; i < editAssets.Count; i++)
                {
                    if (Search != string.Empty)
                    {
                        if (Search.Length >= 2 && Search.Substring(0, 2) == "t:")
                        {
                            if (!editAssets[i].GetType().Name.ToLower().Contains(Search.Remove(0, 2).ToLower()))
                                continue;
                        }
                        else
                        {
                            if (!editAssets[i].name.ToLower().Contains(Search.ToLower()))
                                continue;
                        }
                    }

                    UI.BeginHorizontal();
                    UnityEngine.Object newObject = UI.ObjectField(editAssets[i]);
                    if (editAssets[i] != newObject)
                    {
                        if (newObject == null)
                        {
                            if (!Settings.Assets.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheAsset, Language.Yes, Language.No))
                            {
                            }
                            else
                            {
                                newObject = editAssets[i];
                            }
                        }

                        editAssets[i] = newObject;
                    }

                    if (UI.Button(UI.Icons.XBox, UI.ButtonStyle.None))
                    {
                        if (!Settings.Assets.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheAsset, Language.Yes, Language.No))
                        {
                            editAssets[i] = null;
                        }
                    }
                    UI.EndHorizontal();
                }

                //new
                UnityEngine.Object _new = UI.ObjectField(null);
                if (_new != null)
                    editAssets.Insert(editAssets.Count, _new);

                //remove null
                editAssets.RemoveAll(delegate (UnityEngine.Object o) { return o == null; });

                if (UI.Utilities.EndChangeCheck())
                {
                    needUndo = true;
                }
            }
            else
            {
                if (assets.Count == 0)
                {
                    UI.Label(Language.Empty, UI.LabelStyle.CenteredGrey);
                }
                else
                {
                    UI.ObjectDisplayList(assets, Search);
                }
            }
        }
        
        public override void OnClose()
        {
            if (needUndo)
            {
                UndoHandler.SaveState("Assets Popup");
                assets.Clear();
                assets.AddRange(editAssets);
                FileManager.SaveAll();
            }

            base.OnClose();
        }
    }
}
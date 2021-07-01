using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ProjectPlanner
{
    public class ScreenshotWindow : WindowBase
    {
        [SerializeField]
        private string ScreenshotId
        {
            get
            {
                return Prefs.GetString("screenshot_window_screenshot_id", string.Empty);
            }
            set
            {
                Prefs.SetString("screenshot_window_screenshot_id", value);
            }
        }

        private Screenshot _screenshot;
        private Screenshot Screenshot
        {
            get
            {
                if (_screenshot == null)
                    _screenshot = Screenshots.I.Get(ScreenshotId);

                return _screenshot;
            }
        }

        private bool Empty
        {
            get
            {
                return Screenshot == null || Screenshot.Image == null;
            }
        }

        public static void Init(string screenshotId)
        {
            ScreenshotWindow window = (ScreenshotWindow)GetWindow(typeof(ScreenshotWindow), Settings.Screenshots.UtilityWindow);
            window.titleContent = new GUIContent(Language.Screenshot.ToTitleCase());
            window.minSize = new Vector2(300, 220);
            window._screenshot = null;
            window.ScreenshotId = screenshotId;
            window.Show();
        }

        protected override void OnDraw()
        {
            DrawToolbar();
            UI.Space();
            if (Empty)
            {
                DrawNoEmpty();
            }
            else
            {
                DrawBackground();
                DrawImage();
            }

            if (!Settings.Screenshots.Enabled)
            {
                Close();
            }
        }
        private void DrawToolbar()
        {
            UI.BeginToolbar();

            if (Empty)
            {
                UI.Utilities.Enabled = false;
                UI.TextField("", UI.TextFieldStyle.Toolbar);
                UI.Space(true);
                UI.Button("Show in explorer", UI.ButtonStyle.Toolbar);
                UI.Button("Delete", UI.ButtonStyle.Toolbar);
                UI.Utilities.Enabled = true;
            }
            else
            {
                if (Tasks.I.Get(Screenshot.TaskID).Locked)
                {
                    UI.Label(Screenshot.Name);
                    UI.Space(true);
                    UI.Button("Show in explorer", UI.ButtonStyle.Toolbar);
                    UI.Utilities.Enabled = false;
                    UI.Button("Delete", UI.ButtonStyle.Toolbar);
                    UI.Utilities.Enabled = true;
                }
                else
                {
                    UI.Utilities.BeginChangeCheck();
                    string newValue = UI.TextField(Screenshot.Name, Language.Name, UI.TextFieldStyle.Toolbar);

                    if (UI.Utilities.EndChangeCheck())
                    {
                        if (!string.IsNullOrEmpty(newValue))
                        {
                            UndoHandler.SaveState("Change Screenshot Name");
                            Screenshot.Name = newValue;
                            FileManager.SaveAll();
                        }
                    }

                    UI.Space(true);
                    if (UI.Button("Show in explorer", UI.ButtonStyle.Toolbar))
                    {
                        Application.OpenURL(FileManager.ScreenshotsFolder(Screenshot.TaskID));
                    }
                    if (UI.Button("Delete", UI.ButtonStyle.Toolbar))
                    {
                        if (!Settings.Screenshots.ConfirmRemoval || UI.Utilities.Dialog(Language.Remove, Language.AreYouSureYouWantToRemoveTheScreenshot, Language.Yes, Language.No))
                        {
                            MakeUpdateUISafe(() =>
                            {
                                UndoHandler.SaveState("Delete Screenshot");
                                Screenshots.I.Delete(Screenshot);
                            });
                        }
                    }
                }
            }

            UI.EndToolbar();
        }
        private void DrawNoEmpty()
        {
            UI.Space(true);
            UI.Label(Language.Empty, UI.LabelStyle.Centered);
            UI.Space(true);
        }
        private void DrawBackground()
        {
            float toolbarHeight = UI.Styles.Toolbar.fixedHeight;
            UI.Utilities.SetColor(Settings.Screenshots.BackgroundColor);
            GUI.DrawTexture(new Rect(0, toolbarHeight, position.width, position.height - toolbarHeight), UI.Utilities.WhiteTexture);
            UI.Utilities.ResetColor();
        }
        private void DrawImage()
        {
            float toolbarHeight = UI.Styles.Toolbar.fixedHeight;
            float areaHeight = position.height - toolbarHeight;
            float areaWidth = position.width;
            float scaleFactor = Mathf.Min(areaWidth / Screenshot.Image.width, areaHeight / Screenshot.Image.height);

            Vector2 size = new Vector2((int)(Screenshot.Image.width * scaleFactor), (int)(Screenshot.Image.height * scaleFactor));

            Rect previewRect = new Rect((areaWidth - size.x) / 2, toolbarHeight + (areaHeight - size.y) / 2, size.x, size.y);
            GUI.DrawTexture(previewRect, Screenshot.Image);
        }
    }
}
using ProjectPlanner.Popups;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    /// <summary>
    /// UI is a replacement for GUILayout.
    /// UI is designed to make all editor windows in Project Planner look the same.
    /// </summary>
    public static class UI
    {
        #region ScrollView
        public static Vector2 BeginScrollView(Vector2 scrollPosition)
        {
            return EditorGUILayout.BeginScrollView(scrollPosition);
        }
        public static Vector2 BeginScrollViewOnlyHorizontal(Vector2 scrollPosition)
        {
            return EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.horizontalScrollbar, GUIStyle.none);
        }
        public static Vector2 BeginScrollViewOnlyVertical(Vector2 scrollPosition)
        {
            return EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
        }
        public static void EndScrollView()
        {
            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Vertical and Horizontal Layout (layout flow)
        public static void BeginVertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
        }
        public static void EndVertical()
        {
            GUILayout.EndVertical();
        }

        public static void BeginHorizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
        }
        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Box
        public enum BoxStyle
        {
            Normal,
            Round,
            Flat
        }

        public static void BeginBox(BoxStyle style = BoxStyle.Normal, params GUILayoutOption[] options)
        {
            switch (style)
            {
                case BoxStyle.Normal:
                    GUILayout.BeginVertical(Styles.Box, options);
                    break;
                case BoxStyle.Round:
                    GUILayout.BeginVertical(Styles.BoxRound, options);
                    break;
                case BoxStyle.Flat:
                    GUILayout.BeginVertical(Styles.BoxFlat, options);
                    break;
            }
        }
        public static void EndBox()
        {
            GUILayout.EndVertical();
        }
        #endregion

        #region Toolbar
        public static void BeginToolbar()
        {
            GUILayout.BeginHorizontal(Styles.Toolbar);
        }
        public static void EndToolbar()
        {
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Button
        public enum ButtonStyle
        {
            Normal,
            Left,
            Mid,
            Right,
            Toolbar,
            ToolbarActive,
            None,
            Big
        }

        public static bool Button(string text, ButtonStyle style = ButtonStyle.Normal, bool fitContent = true)
        {
            return Button(new GUIContent(text), style, fitContent);
        }
        public static bool Button(Texture image, ButtonStyle style = ButtonStyle.Normal, bool fitContent = true)
        {
            return Button(new GUIContent(image), style, fitContent);
        }
        public static bool Button(GUIContent content, ButtonStyle style = ButtonStyle.Normal, bool fitContent = true)
        {
            GUIStyle guiStyle = Styles.Button;
            switch (style)
            {
                case ButtonStyle.Left:
                    guiStyle = Styles.ButtonLeft;
                    break;
                case ButtonStyle.Mid:
                    guiStyle = Styles.ButtonMid;
                    break;
                case ButtonStyle.Right:
                    guiStyle = Styles.ButtonRight;
                    break;
                case ButtonStyle.Toolbar:
                    guiStyle = Styles.ToolbarButton;
                    break;
                case ButtonStyle.ToolbarActive:
                    guiStyle = Styles.ToolbarButtonActive;
                    break;
                case ButtonStyle.None:
                    guiStyle = Styles.Label;
                    break;
                case ButtonStyle.Big:
                    guiStyle = Styles.ButtonBig;
                    break;
                default:
                    break;
            }

            if (fitContent)
            {
                if (GUILayout.Button(content, guiStyle, GUILayout.Width(guiStyle.CalcSize(content).x + 1)))
                {
                    return true;
                }
            }
            else
            {
                if (GUILayout.Button(content, guiStyle))
                {
                    return true;
                }
            }

            Utilities.CursorIcon();
            return false;
        }
        #endregion

        #region Label
        public enum LabelStyle
        {
            Normal,
            Mini,
            Centered,
            CenteredGrey,
            Bold,
            GettingStartedTitle,
            Headline,
        }

        public static void Label(int text, LabelStyle style = LabelStyle.Normal, bool fitContent = false)
        {
            Label(new GUIContent(text.ToString()), style, fitContent);
        }
        public static void Label(string text, LabelStyle style = LabelStyle.Normal, bool fitContent = false)
        {
            Label(new GUIContent(text), style, fitContent);
        }
        public static void Label(GUIContent content, LabelStyle style = LabelStyle.Normal, bool fitContent = false)
        {
            GUIStyle guiStyle = Styles.Label;
            switch (style)
            {
                case LabelStyle.Mini:
                    guiStyle = Styles.LabelMini;
                    break;
                case LabelStyle.Centered:
                    guiStyle = Styles.LabelCentered;
                    break;
                case LabelStyle.CenteredGrey:
                    guiStyle = Styles.LabelCenteredGrey;
                    break;
                case LabelStyle.Bold:
                    guiStyle = Styles.LabelBold;
                    break;
                case LabelStyle.GettingStartedTitle:
                    guiStyle = Styles.LabelGettingStartedTitle;
                    break;
                case LabelStyle.Headline:
                    guiStyle = Styles.LabelHeadline;
                    break;
            }

            if (fitContent)
            {
                GUILayout.Label(content, guiStyle, GUILayout.Width(Utilities.CalcWidth(guiStyle, content)));
            }
            else
            {
                GUILayout.Label(content, guiStyle);
            }
        }

        public enum LabelWrapStyle
        {
            Normal,
            Centered,
            Bold
        }
        public static void LabelWrap(string text, float width, LabelWrapStyle style = LabelWrapStyle.Normal)
        {
            LabelWrap(new GUIContent(text), width, style);
        }
        public static void LabelWrap(GUIContent content, float width, LabelWrapStyle style = LabelWrapStyle.Normal)
        {
            GUIStyle guiStyle = Styles.LabelWrap;
            switch (style)
            {
                case LabelWrapStyle.Centered:
                    guiStyle = Styles.LabelWrapCentered;
                    break;
                case LabelWrapStyle.Bold:
                    guiStyle = Styles.LabelWrapBold;
                    break;
            }

            GUILayout.Label(content, guiStyle, GUILayout.ExpandWidth(true), GUILayout.Height(guiStyle.CalcHeight(content, width)));
        }
        #endregion

        #region Toggle
        public static bool Toggle(bool value)
        {
            value = EditorGUILayout.Toggle(value, Styles.Toggle, GUILayout.Width(Utilities.CalcWidth(Styles.Toggle, "")));
            Utilities.CursorIcon();
            return value;
        }
        #endregion

        #region TextField
        public enum TextFieldStyle
        {
            Normal,
            Toolbar,
        }

        public static string TextField(string value, TextFieldStyle style = TextFieldStyle.Normal)
        {
            return TextField(value, "", style);
        }
        public static string TextField(string value, string placeholder, TextFieldStyle style = TextFieldStyle.Normal)
        {
            GUIStyle guiStyle = Styles.TextField;
            switch (style)
            {
                case TextFieldStyle.Toolbar:
                    guiStyle = Styles.TextFieldToolbar;
                    break;
            }

            value = EditorGUILayout.TextField(value, guiStyle);
            Placeholder(value, placeholder, GUILayoutUtility.GetLastRect());
            return value;
        }
        #endregion

        #region Password
        public static string Password(string value)
        {
            return Password(value, "");
        }
        public static string Password(string value, string placeholder)
        {
            GUIStyle guiStyle = Styles.TextField;
            value = EditorGUILayout.PasswordField(value, guiStyle);
            Placeholder(value, placeholder, GUILayoutUtility.GetLastRect());
            return value;
        }
        #endregion

        #region TextArea
        public static string TextArea(string value, int height = 0)
        {
            return TextArea(value, "", height);
        }
        public static string TextArea(string value, string placeholder, int height = 0)
        {
            if (height == 0)
            {
                value = EditorGUILayout.TextArea(value, Styles.TextAreaWrap, GUILayout.ExpandWidth(true));
            }
            else
            {
                value = EditorGUILayout.TextArea(value, Styles.TextAreaWrap, GUILayout.ExpandWidth(true), GUILayout.Height(Styles.TextAreaWrap.lineHeight * height));
            }
            Placeholder(value, placeholder, GUILayoutUtility.GetLastRect());
            return value;
        }
        #endregion

        #region Objects
        public static UnityEngine.Object ObjectField(UnityEngine.Object value)
        {
            return ObjectField<UnityEngine.Object>(value);
        }
        public static T ObjectField<T>(T value) where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(value, typeof(T), false);
        }


        public static void ObjectDisplay<T>(T o) where T : UnityEngine.Object
        {
            GUIStyle style = Styles.DisplayObjectStyle;
            style.imagePosition = o ? ImagePosition.ImageLeft : ImagePosition.TextOnly;

            Utilities.Enabled = false;
            EditorGUILayout.ObjectField(o, typeof(T), false);
            Utilities.Enabled = true;
            if (GUI.Button(GUILayoutUtility.GetLastRect(), EditorGUIUtility.ObjectContent(o, typeof(T)), style) && o)
            {
                EditorGUIUtility.PingObject(o);
                Selection.activeObject = o;
            }
        }
        public static void ObjectDisplayList<T>(List<T> list, string filter = "") where T : UnityEngine.Object
        {
            foreach (var item in list)
            {
                if (filter != string.Empty)
                {
                    if (filter.Length >= 2 && filter.Substring(0, 2) == "t:")
                    {
                        if (!item.GetType().Name.ToLower().Contains(filter.Remove(0, 2).ToLower()))
                            continue;
                    }
                    else
                    {
                        if (!item.name.ToLower().Contains(filter.ToLower()))
                            continue;
                    }
                }

                ObjectDisplay<T>(item);
            }
        }

        #endregion

        #region SelectableLabel
        public static void SelectableLabel(string text)
        {
            EditorGUILayout.SelectableLabel(text, Styles.Label);
        }
        public static void SelectableLabelWrap(string text, float width)
        {
            EditorGUILayout.SelectableLabel(text, Styles.LabelWrap, GUILayout.ExpandWidth(true), GUILayout.Height(EditorStyles.textArea.CalcHeight(new GUIContent(text), width)));
            //EditorGUILayout.SelectableLabel(text, Styles.LabelWrap, GUILayout.ExpandWidth(true), GUILayout.Height(EditorStyles.textArea.CalcHeight(new GUIContent(text), width) + Styles.LabelWrap.lineHeight));
        }
        #endregion

        #region Placeholder
        /// <summary>
        /// Placeholder text that disappears when input is not empty.
        /// </summary>
        /// <param name="input">Input of the element the placeholder is shown on top of</param>
        /// <param name="text">Placeholder text</param>
        /// <param name="rect">Rect of the UI element the placeholder is shown on top of</param>
        private static void Placeholder(string input, string text, Rect rect)
        {
            if (input == string.Empty)
            {
                GUI.Label(new Rect(rect.x + 5, rect.y, rect.width - 10, rect.height), text, Styles.PlaceholderStyle);
            }
        }
        #endregion

        #region Color
        public static void ColorDisplay(Color color)
        {
            BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight));
            Space(true);
            EndHorizontal();
            Rect rect = Utilities.LastRectRelative();
            int padding = 2;
            Color tmpColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.position.x + padding, rect.position.y + padding, rect.size.x - padding, rect.size.y - padding), UI.Utilities.WhiteTexture);
            GUI.color = tmpColor;
        }
        #endregion

        #region ColorField
        public static Color ColorField(Color color, bool alpha = false, float width = 0)
        {

            if (width == 0)
            {
                UI.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight));
                UI.Space(true);
                UI.EndHorizontal();
                Rect rect = Utilities.LastRectRelative();
                int padding = 2;

                //DLL
#if UNITY_2018_1_OR_NEWER
                color = EditorGUI.ColorField(new Rect(rect.position.x + padding, rect.position.y + padding, rect.size.x - padding, rect.size.y - padding), new GUIContent(""), color, false, alpha, false);
#else
                color = EditorGUI.ColorField(new Rect(rect.position.x + padding, rect.position.y + padding, rect.size.x - padding, rect.size.y - padding), new GUIContent(""), color, false, alpha, false, null);
#endif
            }
            else
            {
                //DLL
#if UNITY_2018_1_OR_NEWER
                color = EditorGUILayout.ColorField(new GUIContent(""), color, false, alpha, false, GUILayout.Width(width));
#else
                color = EditorGUILayout.ColorField(new GUIContent(""), color, false, alpha, false, null, GUILayout.Width(width));
#endif
            }

            if (!alpha)
                color.a = 1;

            Utilities.CursorIcon();
            return color;
        }
#endregion

#region Foldout
        public static bool Foldout(bool open, string text, Action onUpdate = null)
        {
            bool oldValue = open;
            Texture image = open ? Icons.ArrowDown : Icons.ArrowLeft;

            BeginHorizontal(GUILayout.ExpandWidth(false));
            Image(image, Vector3.one * Styles.Label.lineHeight);
            if (!string.IsNullOrEmpty(text))
            {
                Label(text, LabelStyle.Normal, true);
            }
            EndHorizontal();

            if (GUI.Button(Utilities.LastRectRelative(), "", GUIStyle.none))
            {
                open = !open;
            }

            if (oldValue != open && onUpdate != null)
                onUpdate.Invoke();

            return open;

            //return EditorGUILayout.Foldout(open, text, true, Styles.Foldout);
        }
        public static bool Foldout(bool open, Action onUpdate = null)
        {
            bool oldValue = open;
            Texture image = open ? Icons.ArrowDown : Icons.ArrowLeft;

            Image(image);
            if (GUI.Button(Utilities.LastRectRelative(), "", GUIStyle.none))
            {
                open = !open;
            }

            if (oldValue != open && onUpdate != null)
                onUpdate.Invoke();

            return open;

            //return EditorGUILayout.Foldout(open, text, true, Styles.Foldout);
        }
        #endregion

        #region Space
        /// <summary>
        /// Empty space with no content.
        /// </summary>
        /// <param name="expand">try to expand and fill as much space as possible</param>
        public static void Space(bool expand = false)
        {
            if (expand)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.Space(10);
            }
        }

        public static void FixedSpace(int pixels)
        {
            GUILayout.Space(pixels);
        }
#endregion

#region Line
        public static void Line()
        {
            Line(new Color(1, 1, 1, .5f), new Color(0, 0, 0, .5f));
        }
        public static void Line(Color darkSkin, Color lightSkin)
        {
            Color c = GUI.color;
            GUI.color = Utilities.ProSkin ? darkSkin : lightSkin;
            GUILayout.Box(GUIContent.none, Styles.LineStyle);
            GUI.color = c;
        }
#endregion

#region Image
        public static void Image(Texture image)
        {
            Image(image, Styles.None.CalcSize(new GUIContent(image)));
        }
        public static void Image(Texture image, Vector2 size)
        {
            GUILayout.Box(image, Styles.None, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }
#endregion

#region Search
        public static string SearchField(string text, string placeholder = "search")
        {
            if (placeholder == "search")
                placeholder = Language.Search;

            text = EditorGUILayout.TextField(text, Styles.ToolbarSearch);
            Placeholder(text, placeholder, new Rect(Utilities.LastRectRelative().x + 12, Utilities.LastRectRelative().y - 2, Utilities.LastRectRelative().width, Utilities.LastRectRelative().height));

            if (GUILayout.Button("", string.IsNullOrEmpty(text) ? Styles.ToolbarSearchButton : Styles.ToolbarSearchButtonActive))
            {
                Utilities.LossFocus();
                text = string.Empty;
            }
            if (!string.IsNullOrEmpty(text))
            {
                Utilities.CursorIcon();
            }

            return text;
        }
#endregion

#region TagList
        /// <summary>
        /// Display selected tags.
        /// </summary>
        /// <param name="tags">Tags the IDs are from</param>
        /// <param name="ids">The selected tags as IDs</param>
        public static void TagList(Tags tags, List<string> ids)
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool first = true;
            foreach (var id in ids)
            {
                if (!first)
                {
                    stringBuilder.Append(", ");
                }
                if (tags.Values.ContainsKey(id))
                {
                    stringBuilder.Append(tags.Values[id]);
                    first = false;
                }
            }
            GUILayout.Label(stringBuilder.ToString(), EditorStyles.wordWrappedLabel);
        }
#endregion

#region StringDropDown
        /// <summary>
        /// Display string list as dropdown.
        /// </summary>
        /// <param name="selected">Selected string</param>
        /// <param name="list">String list to display</param>
        /// <returns>Selected string</returns>
        public static string StringDropDown(string selected, List<string> list, EnumDropDownStyle style = EnumDropDownStyle.Normal, bool fitLabel = true)
        {
            return StringDropDown(string.Empty, selected, list, style, fitLabel);
        }

        /// <summary>
        /// Display string list as dropdown.
        /// </summary>
        /// <param name="preLabel">Label in front of dropdown</param>
        /// <param name="selected">Selected string</param>
        /// <param name="list">String list to display</param>
        /// <returns>new selected string</returns>
        public static string StringDropDown(string preLabel, string value, List<string> list, EnumDropDownStyle style = EnumDropDownStyle.Normal, bool fitLabel = true)
        {
            GUIStyle guiStyle = Styles.DropDown;
            switch (style)
            {
                case EnumDropDownStyle.Toolbar:
                    guiStyle = Styles.ToolbarDropDown;
                    break;
            }

            int index = 0;
            if (list.Contains(value))
            {
                index = list.IndexOf(value);
            }
            if (fitLabel)
            {
                BeginHorizontal();
                Label(preLabel, LabelStyle.Mini, true);
                index = EditorGUILayout.Popup(index, list.ToArray(), guiStyle);
                EndHorizontal();
            }
            else
            {
                if (preLabel == string.Empty)
                {
                    index = EditorGUILayout.Popup(index, list.ToArray(), guiStyle);
                }
                else
                {
                    index = EditorGUILayout.Popup(preLabel, index, list.ToArray(), guiStyle);
                }
            }
            Utilities.CursorIcon();

            return list[index];
        }
#endregion

#region EnumDropDown
        public enum EnumDropDownStyle
        {
            Normal,
            Toolbar,
        }

        /// <summary>
        /// Display an enum as a drop down.
        /// </summary>
        /// <param name="_enum">The enum to use</param>
        /// <param name="names">The names to show</param>
        /// <param name="buttonText">Button GUIContent</param>
        /// <param name="style">Style to use</param>
        /// <param name="fitContent">Limit width to content width</param>
        public static Enum EnumDropDown(Enum _enum, EnumDropDownStyle style = EnumDropDownStyle.Normal)
        {
            GUIStyle guiStyle = Styles.DropDown;
            switch (style)
            {
                case EnumDropDownStyle.Toolbar:
                    guiStyle = Styles.ToolbarDropDown;
                    break;
            }

            return EditorGUILayout.EnumPopup(_enum, guiStyle);
        }
#endregion

        public static class Utilities
        {
            public static void CursorIcon(MouseCursor icon = MouseCursor.Link)
            {
                CursorIcon(LastRectRelative(), icon);
            }
            public static void CursorIcon(Rect rect, MouseCursor icon = MouseCursor.Link)
            {
                if (Settings.Other.AdaptiveMouseCursor)
                    EditorGUIUtility.AddCursorRect(rect, icon);
            }

            public static void SetColor(Color color)
            {
                GUI.color = color;
            }
            public static void ResetColor()
            {
                GUI.color = Color.white;
            }

            public static Texture2D WhiteTexture
            {
                get
                {
                    return EditorGUIUtility.whiteTexture;
                }
            }
            public static Texture2D GreyTexture
            {
                get
                {
                    if (greyTexture == null)
                    {
                        greyTexture = new Texture2D(1, 1);
                        greyTexture.SetPixel(0, 0, Color.grey);
                        greyTexture.Apply();
                    }
                    return greyTexture;
                }
            }
            [SerializeField]
            private static Texture2D greyTexture;

            public static bool Enabled
            {
                get
                {
                    return GUI.enabled;
                }
                set
                {
                    GUI.enabled = value;
                }
            }

            public static bool IsRepaint
            {
                get
                {
                    return Event.current.type == EventType.Repaint;
                }
            }

            public static bool ProSkin
            {
                get
                {
                    return EditorGUIUtility.isProSkin;
                }
            }

            public static Rect LastRectRelative()
            {
                return GUILayoutUtility.GetLastRect();
            }
            public static Rect LastRectAbsolute(Vector2 windowPosition)
            {
                Rect rect = LastRectRelative();
                return new Rect(-windowPosition + GUIUtility.GUIToScreenPoint(rect.position), rect.size);
            }

            public static Rect LastRectOnRepaintRelative(Rect rect)
            {
                if (Event.current.type == EventType.Repaint)
                    return LastRectRelative();

                return rect;
            }
            public static Rect LastRectOnRepaintAbsolute(Rect rect, Vector2 windowPosition)
            {
                if (Event.current.type == EventType.Repaint)
                    return LastRectAbsolute(windowPosition);

                return rect;
            }

            public static Vector2 MousePositionAbsolute(Vector2 windowPosition)
            {
                return -windowPosition + GUIUtility.GUIToScreenPoint(MousePositionRelative());
            }
            public static Vector2 MousePositionRelative()
            {
                return Event.current.mousePosition;
            }

            public static void Name(string name)
            {
                GUI.SetNextControlName(Info.Prefix + name);
            }

            public static void LossFocus()
            {
                GUI.FocusControl(null);
            }
            public static void Focus(string name)
            {
                GUI.FocusControl(Info.Prefix + name);
                EditorGUI.FocusTextInControl(Info.Prefix + name);
            }
            public static bool HasFocus(string name)
            {
                return GUI.GetNameOfFocusedControl() == Info.Prefix + name;
            }

            public static float CalcWidth(GUIStyle style, string content)
            {
                return style.CalcSize(new GUIContent(content)).x;
            }
            public static float CalcWidth(GUIStyle style, GUIContent content)
            {
                return style.CalcSize(content).x;
            }

            public static void BeginChangeCheck()
            {
                EditorGUI.BeginChangeCheck();
            }
            public static bool EndChangeCheck()
            {
                return EditorGUI.EndChangeCheck();
            }

            public static bool Dialog(string title, string text, string yes)
            {
                return EditorUtility.DisplayDialog(title, text, yes);
            }
            public static bool Dialog(string title, string text, string yes, string no)
            {
                return EditorUtility.DisplayDialog(title, text, yes, no);
            }

            public static void ShowTagsPopup(Rect rect, List<string> ids, TagsPopup.State state = TagsPopup.State.Edit, Action onClose = null, Action onUpdate = null, PopupOption[] options = null)
            {
                PopupWindow.Show(rect, new TagsPopup(ids, state, onClose, onUpdate, options));
            }
            public static void ShowAssetsPopup(Rect rect, List<UnityEngine.Object> assets, bool canEdit = false, Action onClose = null, PopupOption[] options = null)
            {
                PopupWindow.Show(rect, new AssetsPopup(assets, canEdit, onClose, options));
            }
            public static void ShowStringListPopup(Rect rect, List<string> values, Action<List<string>> onClose = null, PopupOption[] options = null)
            {
                PopupWindow.Show(rect, new StringListPopup(values, onClose, options));
            }
            public static void ShowColorsPopup(Rect rect, List<string> selected, ColorsPopup.State state = ColorsPopup.State.Edit, Action onClose = null, Action onUpdate = null, PopupOption[] options = null)
            {
                PopupWindow.Show(rect, new ColorsPopup(selected, state, onClose, onUpdate, options));
            }

            /// <summary>
            /// Displays or updates a progress bar.
            /// </summary>
            /// <param name="progress">Between 0.0 and 1.0</param>
            public static void ProgressBar(string title, string message, float progress)
            {
                EditorUtility.DisplayProgressBar(title, message, progress);
            }
            public static void ClearProgressBar()
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static class Icons
        {
            public static Texture BoardWindow
            {
                get
                {
                    if (UI.Utilities.ProSkin)
                        return FileManager.GetIcon("d_board window");
                    else
                        return FileManager.GetIcon("board window");
                }
            }
            public static Texture TaskWindow
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_task window");
                    else
                        return FileManager.GetIcon("task window");
                }
            }
            public static Texture TreeWindow
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_tree window");
                    else
                        return FileManager.GetIcon("tree window");
                }
            }
            public static Texture ColorIconCircle
            {
                get
                {
                    return FileManager.GetIcon("color icon circle");
                }
            }
            public static Texture ColorIconDiamond
            {
                get
                {
                    return FileManager.GetIcon("color icon diamond");
                }
            }
            public static Texture ColorIconBox
            {
                get
                {
                    return FileManager.GetIcon("color icon box");
                }
            }
            public static Texture Lock
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_lock");
                    else
                        return FileManager.GetIcon("lock");
                }
            }
            public static Texture LockOpen
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_lock_open");
                    else
                        return FileManager.GetIcon("lock_open");
                }
            }
            public static Texture SubLock
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_sub_lock");
                    else
                        return FileManager.GetIcon("sub_lock");
                }
            }
            public static Texture DescriptionIndicator
            {
                get
                {
                    if (Utilities.ProSkin)
                        return FileManager.GetIcon("d_description_indicator");
                    else
                        return FileManager.GetIcon("description_indicator");
                }
            }
            public static Texture Options
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d__Popup").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("_Popup").image;
                    }
                }
            }
            public static Texture Tag
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_FilterByLabel").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("FilterByLabel").image;
                    }
                }
            }
            public static Texture Sorting
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_DefaultSorting").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("DefaultSorting").image;
                    }
                }
            }
            public static Texture Plus
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_Toolbar Plus").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("Toolbar Plus").image;
                    }
                }
            }
            public static Texture Minus
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_Toolbar Minus").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("Toolbar Minus").image;
                    }
                }
            }
            public static Texture Type
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("FilterByType").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("d_FilterByType").image;
                    }
                }
            }
            public static Texture Star
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("Favorite").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("d_Favorite").image;
                    }
                }
            }
            public static Texture XBox
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_winbtn_win_close_h").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("winbtn_win_close_h").image;
                    }
                }
            }
            public static Texture X
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("winbtn_win_close").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("d_winbtn_win_close").image;
                    }
                }
            }
            public static Texture Gear
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("_Popup").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("d__Popup").image;
                    }
                }
            }
            public static Texture ArrowLeft
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_arrow_right");
                    }
                    else
                    {
                        return FileManager.GetIcon("arrow_right");
                    }
                }
            }
            public static Texture ArrowDown
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_arrow_down");
                    }
                    else
                    {
                        return FileManager.GetIcon("arrow_down");
                    }
                }
            }
            public static Texture Folder
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_Project").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("Project").image;
                    }
                }
            }
            public static Texture Eye
            {
                get
                {
                    return EditorGUIUtility.IconContent("d_VisibilityOn").image;
                }
            }
            public static Texture Hierarchy
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_UnityEditor.SceneHierarchyWindow").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("UnityEditor.SceneHierarchyWindow").image;
                    }
                }
            }
            public static Texture Colors
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return EditorGUIUtility.IconContent("d_SceneViewRGB").image;
                    }
                    else
                    {
                        return EditorGUIUtility.IconContent("SceneViewRGB").image;
                    }
                }
            }
            public static Texture WhitePixel
            {
                get
                {
                    return FileManager.GetIcon("white_pixel");
                }
            }

            public static Texture Spinner0
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_0");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_0");
                    }
                }
            }
            public static Texture Spinner1
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_1");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_1");
                    }
                }
            }
            public static Texture Spinner2
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_2");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_2");
                    }
                }
            }
            public static Texture Spinner3
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_3");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_3");
                    }
                }
            }
            public static Texture Spinner4
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_4");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_4");
                    }
                }
            }
            public static Texture Spinner5
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_5");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_5");
                    }
                }
            }
            public static Texture Spinner6
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_6");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_6");
                    }
                }
            }
            public static Texture Spinner7
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_7");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_7");
                    }
                }
            }
            public static Texture Spinner8
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_8");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_8");
                    }
                }
            }
            public static Texture Spinner9
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_9");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_9");
                    }
                }
            }
            public static Texture Spinner10
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_10");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_10");
                    }
                }
            }
            public static Texture Spinner11
            {
                get
                {
                    if (Utilities.ProSkin)
                    {
                        return FileManager.GetIcon("d_spinner_11");
                    }
                    else
                    {
                        return FileManager.GetIcon("spinner_11");
                    }
                }
            }
        }

        public static class Styles
        {
            public static GUIStyle None
            {
                get
                {
                    return GUIStyle.none;
                }
            }

            public static GUIStyle Toggle
            {
                get
                {
                    return EditorStyles.toggle;
                }
            }

            [SerializeField]
            private static GUIStyle foldout;
            public static GUIStyle Foldout
            {
                get
                {
                    if (foldout == null)
                    {
                        foldout = new GUIStyle(EditorStyles.foldout)
                        {
                            fontStyle = FontStyle.Normal
                        };
                        foldout.padding = new RectOffset(foldout.padding.left, 0, foldout.padding.top, foldout.padding.bottom);
                    }

                    return foldout;
                }
            }

#region Label
            public static GUIStyle LabelBold
            {
                get
                {
                    return EditorStyles.boldLabel;
                }
            }

            public static GUIStyle Label
            {
                get
                {
                    return EditorStyles.label;
                }
            }
            public static GUIStyle LabelMini
            {
                get
                {
                    return EditorStyles.miniLabel;
                }
            }

            [SerializeField]
            private static GUIStyle labelWrap;
            public static GUIStyle LabelWrap
            {
                get
                {
                    if (labelWrap == null)
                    {
                        labelWrap = new GUIStyle(EditorStyles.label)
                        {
                            wordWrap = true
                        };
                    }

                    return labelWrap;
                }
            }

            [SerializeField]
            private static GUIStyle labelWrapBold;
            public static GUIStyle LabelWrapBold
            {
                get
                {
                    if (labelWrapBold == null)
                    {
                        labelWrapBold = new GUIStyle(EditorStyles.label)
                        {
                            wordWrap = true,
                            fontStyle = FontStyle.Bold
                        };
                    }

                    return labelWrapBold;
                }
            }
            [SerializeField]
            private static GUIStyle labelWrapCentered;
            public static GUIStyle LabelWrapCentered
            {
                get
                {
                    if (labelWrapCentered == null)
                    {
                        labelWrapCentered = new GUIStyle(EditorStyles.label)
                        {
                            wordWrap = true,
                            alignment = TextAnchor.UpperCenter
                        };
                    }

                    return labelWrapCentered;
                }
            }

            [SerializeField]
            private static GUIStyle labelMiniWrap;
            public static GUIStyle LabelMiniWrap
            {
                get
                {
                    if (labelMiniWrap == null)
                    {
                        labelMiniWrap = new GUIStyle(EditorStyles.miniLabel)
                        {
                            wordWrap = true
                        };
                    }

                    return labelMiniWrap;
                }
            }

            public static GUIStyle AssetLabel
            {
                get
                {
                    return GUI.skin.GetStyle("AssetLabel");
                }
            }

            public static GUIStyle LabelCenteredGrey
            {
                get
                {
                    return EditorStyles.centeredGreyMiniLabel;
                }
            }

            [SerializeField]
            private static GUIStyle labelGettingStartedTitle;
            public static GUIStyle LabelGettingStartedTitle
            {
                get
                {
                    if (labelGettingStartedTitle == null)
                    {
                        labelGettingStartedTitle = new GUIStyle(EditorStyles.label)
                        {
                            fontSize = 25,
                            alignment = TextAnchor.MiddleCenter
                        };
                    }

                    return labelGettingStartedTitle;
                }
            }

            [SerializeField]
            private static GUIStyle labelCentered;
            public static GUIStyle LabelCentered
            {
                get
                {
                    if (labelCentered == null)
                    {
                        labelCentered = new GUIStyle(EditorStyles.label)
                        {
                            alignment = TextAnchor.MiddleCenter
                        };
                    }

                    return labelCentered;
                }
            }

            [SerializeField]
            private static GUIStyle labelHeadline;
            public static GUIStyle LabelHeadline
            {
                get
                {
                    if (labelHeadline == null)
                    {
                        labelHeadline = new GUIStyle(EditorStyles.label)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 14,
                            fontStyle = FontStyle.Bold
                        };
                    }

                    return labelHeadline;
                }
            }
#endregion

#region Toolbar
            public static GUIStyle Toolbar
            {
                get
                {
                    return EditorStyles.toolbar;
                }
            }

            public static GUIStyle ToolbarButton
            {
                get
                {
                    return EditorStyles.toolbarButton;
                }
            }

            public static GUIStyle ToolbarSearch
            {
                get
                {
                    return GUI.skin.GetStyle("ToolbarSeachTextField");
                }
            }
            public static GUIStyle ToolbarSearchButton
            {
                get
                {
                    return GUI.skin.GetStyle("ToolbarSeachCancelButtonEmpty");
                }
            }
            public static GUIStyle ToolbarSearchButtonActive
            {
                get
                {
                    return GUI.skin.GetStyle("ToolbarSeachCancelButton");
                }
            }

            [SerializeField]
            private static GUIStyle toolbarButtonActive;
            public static GUIStyle ToolbarButtonActive
            {
                get
                {
                    if (toolbarButtonActive == null)
                    {
                        toolbarButtonActive = new GUIStyle(EditorStyles.toolbarButton);
                        toolbarButtonActive.normal = toolbarButtonActive.active;
                    }


                    return toolbarButtonActive;
                }
            }

            public static GUIStyle ToolbarDropDown
            {
                get
                {
                    return EditorStyles.toolbarDropDown;
                }
            }
#endregion

#region Button
            public static GUIStyle Button
            {
                get
                {
                    return EditorStyles.miniButton;
                }
            }
            public static GUIStyle ButtonLeft
            {
                get
                {
                    return EditorStyles.miniButtonLeft;
                }
            }
            public static GUIStyle ButtonMid
            {
                get
                {
                    return EditorStyles.miniButtonMid;
                }
            }
            public static GUIStyle ButtonRight
            {
                get
                {
                    return EditorStyles.miniButtonRight;
                }
            }


            [SerializeField]
            private static GUIStyle buttonBig;
            public static GUIStyle ButtonBig
            {
                get
                {
                    if (buttonBig == null)
                    {
                        buttonBig = new GUIStyle(GUI.skin.button)
                        {
                            fixedHeight = 25,
                            fontStyle = FontStyle.Bold
                        };
                    }

                    return buttonBig;
                }
            }
#endregion

#region Box
            public static GUIStyle BoxRound
            {
                get
                {
                    return EditorStyles.helpBox;
                }
            }

            [SerializeField]
            private static GUIStyle box;
            public static GUIStyle Box
            {
                get
                {
                    if (box == null)
                        box = new GUIStyle(GUI.skin.box);

                    return box;
                }
            }

            [SerializeField]
            private static GUIStyle boxFlat;
            public static GUIStyle BoxFlat
            {
                get
                {
                    if (boxFlat == null)
                    {
                        boxFlat = new GUIStyle(GUIStyle.none);
                        boxFlat.normal.background = FileManager.GetIcon("white_pixel");
                    }

                    return boxFlat;
                }
            }
#endregion

            [SerializeField]
            private static GUIStyle displayObjectStyle;
            public static GUIStyle DisplayObjectStyle
            {
                get
                {
                    if (displayObjectStyle == null)
                    {
                        displayObjectStyle = new GUIStyle(EditorStyles.textField)
                        {
                            fixedHeight = 16,
                            clipping = TextClipping.Clip
                        };
                    }

                    return displayObjectStyle;
                }
            }

            [SerializeField]
            private static GUIStyle lineStyle;
            public static GUIStyle LineStyle
            {
                get
                {
                    if (lineStyle == null)
                    {
                        lineStyle = new GUIStyle();
                        lineStyle.normal.background = Utilities.WhiteTexture;
                        lineStyle.fixedHeight = 1;
                    }

                    return lineStyle;
                }
            }

            [SerializeField]
            private static GUIStyle placeholderStyle;
            public static GUIStyle PlaceholderStyle
            {
                get
                {
                    if (placeholderStyle == null)
                    {
                        placeholderStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        {
                            alignment = TextAnchor.UpperLeft,
                            fontStyle = FontStyle.Italic
                        };
                    }

                    return placeholderStyle;
                }
            }

            public static GUIStyle TextField
            {
                get
                {
                    return EditorStyles.textField;
                }
            }
            public static GUIStyle TextFieldToolbar
            {
                get
                {
                    return EditorStyles.toolbarTextField;
                }
            }

            [SerializeField]
            private static GUIStyle textAreaWrap;
            public static GUIStyle TextAreaWrap
            {
                get
                {
                    if (textAreaWrap == null)
                    {
                        textAreaWrap = new GUIStyle(EditorStyles.textArea)
                        {
                            wordWrap = true
                        };
                    }

                    return textAreaWrap;
                }
            }

            public static GUIStyle DropDown
            {
                get
                {
                    return EditorStyles.popup;
                }
            }

            [SerializeField]
            private static GUIStyle white;
            public static GUIStyle White
            {
                get
                {
                    if (white == null)
                    {
                        white = new GUIStyle();
                        white.normal.background = Utilities.WhiteTexture;
                    }

                    return white;
                }
            }

            public static GUIStyle HorizontalScrollbar
            {
                get
                {
                    return GUI.skin.horizontalScrollbar;
                }
            }

            public static GUIStyle VerticalScrollbar
            {
                get
                {
                    return GUI.skin.verticalScrollbar;
                }
            }
        }
    }
}

/*
[SerializeField]
private static int stringClusterDeleteIndex = -1;
public static int StringCluster(int selectIndex, List<string> list)
{
    //the tag that is being eidted rect
    Rect selectedRect = new Rect(0, 0, 0, 0);

    //find the width of all the tags so we can calculate the height of the box they are in
    float finalWidth = 0;
    for (int i = 0; i < list.Count; i++)
    {
        if (i == selectIndex)
        {
            finalWidth += Utilitys.CalcWidth(EditorStyles.textField, list[i]) + EditorStyles.textField.margin.left + EditorStyles.textField.margin.right;
        }
        else
        {
            finalWidth += Utilitys.CalcWidth(EditorStyles.miniButton, list[i]) + EditorStyles.miniButton.margin.left + EditorStyles.miniButton.margin.right;
        }
    }
    finalWidth += Utilitys.CalcWidth(EditorStyles.miniButton, "+") + EditorStyles.miniButton.margin.left + EditorStyles.miniButton.margin.right;
    float finalHeight = EditorStyles.textField.CalcHeight(new GUIContent(""), 0) * Mathf.Max(1, Mathf.Ceil(finalWidth / (EditorGUIUtility.currentViewWidth)));

    //draw the box
    GUILayout.BeginVertical(EditorStyles.textField, GUILayout.Height(finalHeight));
    GUILayout.BeginHorizontal();

    //xPosition is the position of the next button that is gogin to be drawn.
    //we use this in order to check if we need to go to aother line
    float xPosition = 0;
    for (int i = 0; i < list.Count; i++)
    {
        //find the widht and margin of the button
        GUIStyle styleToUse = i == selectIndex ? EditorStyles.textField : EditorStyles.miniButton;
        float width = Utilitys.CalcWidth(styleToUse, list[i]);
        float margin = styleToUse.margin.left + styleToUse.margin.right;

        //cal the next x position
        float widthWithMargin = width + margin;
        xPosition += widthWithMargin;

        //do we need a new line
        if (xPosition > EditorGUIUtility.currentViewWidth - (i == list.Count - 1 ? Utilitys.CalcWidth(EditorStyles.miniButton, "x") : 0))
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            xPosition = widthWithMargin;
        }

        //editing the tag
        if (i == selectIndex)
        {
            list[i] = EditorGUILayout.TextField(list[i], EditorStyles.textField, GUILayout.Width(Utilitys.CalcWidth(EditorStyles.textField, list[i]) + 5));
            selectedRect = GUILayoutUtility.GetLastRect();
        }
        //not editing
        else
        {
            if (GUILayout.Button(list[i], EditorStyles.miniButton, GUILayout.Width(width)))
            {
                //left click
                if (Event.current.button == 0)
                {
                    //selct
                    selectIndex = i;
                }
                //right clikc
                else if (Event.current.button == 1)
                {
                    selectIndex = -1;

                    //save index for later
                    stringClusterDeleteIndex = i;

                    //show generic menu
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(Language.Delete), false, () =>
                    {
                        //make the tag empty so it will be removed when we deselct it.
                        list[stringClusterDeleteIndex] = string.Empty;
                        list.RemoveAll(delegate (string s) { return s == string.Empty; });
                    });
                    menu.ShowAsContext();
                }
            }
        }
    }

    //set right margin to 0//
    //new tag button
    if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(Utilitys.CalcWidth(EditorStyles.miniButton, "+"))))
    {
        //ad new tag and selct it
        list.Add(Language.Tag);
        selectIndex = list.Count - 1;
    }
    GUILayout.EndHorizontal();
    GUILayout.EndVertical();

    //if we are edting a tag
    if (selectIndex != -1)
    {
        //if enter or backspace is pressed deselct
        if (Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape)
            {
                selectIndex = -1;
            }
        }
        //if click outside of text field deselct
        if (Event.current.type == EventType.MouseDown)
        {
            if (!selectedRect.Contains(Event.current.mousePosition))
            {
                selectIndex = -1;
            }
        }
    }

    //if we are not edting remove all tag that are empty
    if (selectIndex == -1)
    {
        list.RemoveAll(delegate (string s) { return s == string.Empty; });
    }

    return selectIndex;
}
*/

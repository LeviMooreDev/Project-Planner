using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using ProjectPlanner.Popups;

namespace ProjectPlanner
{
    public class CodeAnalyzerWindow : WindowBase
    {
        public static CodeAnalyzerWindow I
        {
            get
            {
                var windows = Resources.FindObjectsOfTypeAll<CodeAnalyzerWindow>();
                if (windows == null || windows.Length == 0)
                    return null;
                else
                    return windows[0];
            }
        }

        private const int readBufferSize = 1024;
        [SerializeField]
        private Vector2 scrollPosition;
        [SerializeField]
        private List<CodeAnalyzerFile> files;
        [SerializeField]
        private int totalScanThreadCount;
        [SerializeField]
        private string searchQuery;
        [SerializeField]
        private List<Thread> scanThreads;
        private List<CodeAnalyzerFile> filesToAdd;

        private Spinner spinner;

        public static void Init()
        {
            CodeAnalyzerWindow window = CreateInstance<CodeAnalyzerWindow>();
            window.titleContent = new GUIContent(Language.CodeAnalyzer.ToTitleCase());
            window.minSize = new Vector2(300, 300);
            if (Settings.CodeAnalyzer.Utility)
            {
                window.ShowUtility();
            }
            else
            {
                window.Show();
            }

            if (Settings.CodeAnalyzer.ScanOnWindowOpen)
            {
                window.SafeScan();
            }
        }

        protected override void OnWindowOpen()
        {
            files = new List<CodeAnalyzerFile>();
            scrollPosition = new Vector2();
        }
        protected override void OnEditorReload()
        {
            spinner = new Spinner();
            Search();
        }
        protected override void Input(Event e)
        {

        }
        protected override void OnUpdate()
        {
            spinner.Update();
        }
        protected override void OnDraw()
        {
            DrawToolbar();
            scrollPosition = UI.BeginScrollView(scrollPosition);
            DrawWorking();
            DrawFile();
            UI.Space();
            UI.EndScrollView();
            UpdateFilesToAdd();
        }
        private void DrawToolbar()
        {
            UI.BeginToolbar();
            DrawToolBarScan();
            UI.Space(true);
            DrawToolBarSearch();
            UI.EndToolbar();
        }
        private void DrawToolBarSearch()
        {
            UI.Utilities.BeginChangeCheck();
            UI.Utilities.Name("codeAnalyzer_search");
            string newSearchQuery = UI.SearchField(searchQuery);
            if (UI.Utilities.EndChangeCheck())
            {
                UndoHandler.SaveState("Search Query");
                searchQuery = newSearchQuery;
                Search();
            }
        }
        private void DrawToolBarScan()
        {
            if (scanThreads != null && scanThreads.Count > 0)
            {
                UI.Utilities.Enabled = false;
                UI.Button(Language.Scan, UI.ButtonStyle.Toolbar);
                UI.Utilities.Enabled = true;
            }
            else
            {
                if (UI.Button(Language.Scan, UI.ButtonStyle.Toolbar))
                {
                    SafeScan();
                }
            }
        }

        private void DrawWorking()
        {
            if (scanThreads != null && scanThreads.Count > 0)
            {
                UI.Label(Language.Working, UI.LabelStyle.CenteredGrey);
                UI.Space();
                UI.BeginHorizontal();
                UI.Space(true);
                spinner.Draw();
                UI.Space(true);
                UI.EndHorizontal();
            }
        }
        private void DrawFile()
        {
            if (Settings.CodeAnalyzer.UseThreads && totalScanThreadCount != files.Count)
                return;

            foreach (var file in files)
            {
                if (file.messages.Count(o => o.searchShow) > 0)
                {
                    UI.BeginBox();
                    file.showMessages = UI.Foldout(file.showMessages, file.name);

                    if (file.showMessages)
                    {
                        UI.BeginHorizontal();
                        UI.Space();
                        UI.BeginVertical();
                        foreach (var message in file.messages)
                        {
                            if (message.searchShow)
                                DrawMessage(file, message);
                        }
                        UI.EndVertical();
                        UI.EndHorizontal();
                    }

                    UI.EndBox();
                }
            }
        }
        private void DrawMessage(CodeAnalyzerFile file, CodeAnalyzerMessage message)
        {
            MessageInputUpdate(file, message);

            UI.BeginBox();
            string messageValue = message.value;
            if (Settings.CodeAnalyzer.ShowScanWordWithMessage && !message.notImplementedException)
            {
                messageValue = message.scanWord + " - " + messageValue;
            }

            if (Settings.CodeAnalyzer.SelectableMessage)
            {
                UI.SelectableLabelWrap(messageValue, Size.x);
            }
            else
            {
                UI.LabelWrap(messageValue, Size.x);
            }
            UI.BeginHorizontal();
            UI.Label(Language.Line + ": " + message.number, UI.LabelStyle.Mini);
            UI.Space(true);
            if (UI.Button(Language.Find))
            {
                OpenAtLine(file, message);
            }
            if (Settings.CodeAnalyzer.ShowSelectFileButton)
            {
                if (UI.Button(Language.SelectFile))
                {
                    SelectFile(file);
                }
            }
            if (UI.Button(Language.MakeTask.ToTitleCase()))
            {
                MakeTask(file, message);
            }
            UI.EndHorizontal();
            UI.EndBox();
            message.rectAbsolute = UI.Utilities.LastRectOnRepaintAbsolute(message.rectAbsolute, Position);
        }

        private void MessageInputUpdate(CodeAnalyzerFile file, CodeAnalyzerMessage message)
        {
            Event e = Event.current;

            if (Settings.CodeAnalyzer.DoubleClickToFind)
            {
                if (e.isMouse)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (e.button == 0 && e.clickCount == 2)
                        {
                            if (message.rectAbsolute.Contains(UI.Utilities.MousePositionAbsolute(Position)))
                            {
                                OpenAtLine(file, message);
                            }
                        }
                    }
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScanAfterCompiling()
        {
            if (Settings.CodeAnalyzer.ScanAfterCompiling && I != null)
            {
                I.SafeScan();
            }
        }
        private void SafeScan()
        {
            MakeUpdateUISafe(() => { Scan(); });
        }
        private void Scan()
        {
            scrollPosition = Vector2.zero;
            searchQuery = string.Empty;
            scanThreads = new List<Thread>();
            files = new List<CodeAnalyzerFile>();
            totalScanThreadCount = 1;
            filesToAdd = new List<CodeAnalyzerFile>();

            string[] assets = AssetDatabase.GetAllAssetPaths();
            foreach (string asset in assets)
            {
                if (asset.StartsWith("Assets/") && asset.Contains(".cs"))
                {
                    bool ignore = false;
                    string fileName = Path.GetFileNameWithoutExtension(asset);
                    foreach (var hideFileName in Settings.CodeAnalyzer.HideFilesNames)
                    {
                        if (hideFileName.ToLower() == fileName.ToLower())
                        {
                            ignore = true;
                        }
                    }
                    if (fileName.ToLower() == "CodeAnalyzerWindow".ToLower())
                    {
                        ignore = true;
                    }

                    if (!ignore)
                    {
                        CodeAnalyzerFile file = new CodeAnalyzerFile(asset, fileName);

                        if (Settings.CodeAnalyzer.UseThreads)
                        {
                            lock (scanThreads)
                            {
                                scanThreads.Add(new Thread(() => ScanFile(file)));
                            }
                        }
                        else
                        {
                            ScanFile(file);
                        }
                    }
                }
            }

            if (Settings.CodeAnalyzer.UseThreads)
            {
                totalScanThreadCount = scanThreads.Count;
                foreach (var thread in scanThreads.ToArray())
                {
                    thread.Start();
                }
            }
        }
        private void ScanFile(CodeAnalyzerFile file)
        {
            using (FileStream fileStream = new FileStream(file.path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, readBufferSize))
                {
                    string line = string.Empty;
                    int lineNumber = 1;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        //scan words
                        foreach (var scanWord in Settings.CodeAnalyzer.ScanWords)
                        {
                            string fullScanWord = "//" + scanWord;
                            if (Settings.CodeAnalyzer.ScanWordsAreCaseSensitive)
                            {
                                fullScanWord = fullScanWord.ToLower();
                                line = line.ToLower();
                            }

                            if (line.Contains(fullScanWord))
                            {
                                line = line.Trim();
                                int index = line.IndexOf(fullScanWord);
                                if (index == 0)
                                {
                                    line = line.Substring(index + fullScanWord.Length).Trim();
                                    file.messages.Add(new CodeAnalyzerMessage(scanWord, line, lineNumber));
                                }
                            }
                        }

                        //not implemented exception
                        if (Settings.CodeAnalyzer.FindNotImplementedException)
                        {
                            if (line.Contains("new NotImplementedException("))
                            {
                                line = "Not Implemented Exception";
                                file.messages.Add(new CodeAnalyzerMessage("", line, lineNumber, true));
                            }
                        }

                        lineNumber++;
                    }
                }
            }

            lock (filesToAdd)
                filesToAdd.Add(file);

            if (Settings.CodeAnalyzer.UseThreads)
            {
                lock (scanThreads)
                {
                    scanThreads.Remove(Thread.CurrentThread);
                }
            }
        }
        private void Search()
        {
            MakeUpdateUISafe(() =>
            {
                foreach (var file in files)
                {
                    foreach (var message in file.messages)
                    {
                        message.searchShow = true;

                        if (searchQuery != string.Empty)
                        {
                            message.searchShow = false;

                            if (message.value.ToLower().Contains(searchQuery.ToLower()))
                                message.searchShow = true;

                            if (Settings.CodeAnalyzer.IncludeFileNameInSearch)
                            {
                                if (file.name.ToLower().Contains(searchQuery.ToLower()))
                                    message.searchShow = true;
                            }
                        }
                    }
                }
                Repaint();
            });
        }
        private void UpdateFilesToAdd()
        {
            if (filesToAdd != null)
            {
                lock (filesToAdd)
                {
                    if (filesToAdd.Count > 0)
                    {
                        List<CodeAnalyzerFile> tmp = new List<CodeAnalyzerFile>(filesToAdd);
                        MakeUpdateUISafe(() => { files.AddRange(tmp); });
                        filesToAdd.Clear();
                    }
                }
            }
        }
        private void SelectFile(CodeAnalyzerFile file)
        {
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(file.path));
        }
        private void OpenAtLine(CodeAnalyzerFile file, CodeAnalyzerMessage message)
        {
            MonoScript scriptObject = AssetDatabase.LoadAssetAtPath<MonoScript>(file.path);
            AssetDatabase.OpenAsset(scriptObject, message.number);
        }
        private void MakeTask(CodeAnalyzerFile file, CodeAnalyzerMessage message)
        {
            GenericMenu menu = new GenericMenu();
            bool show = false;
            if (Boards.I.All.Count > 0)
            {
                foreach (var board in Boards.I.All.OrderBy(o => o.Name))
                {
                    foreach (var group in Groups.I.All.Where(o => o.BoardID == board.ID))
                    {
                        show = true;
                        menu.AddItem(new GUIContent(board.Name + "/" + group.Name), false, () =>
                        {
                            MakeUpdateUISafe(() =>
                            {
                                UndoHandler.SaveState("Code Analyzer Make Task");

                                Task task = Tasks.I.New(group, false);
                                task.Name = message.value;
                                task.Description = "Script: " + file.name;
                                task.Description += "\n";
                                task.Description += "Line: " + message.number;
                                task.Assets.Add(AssetDatabase.LoadAssetAtPath<MonoScript>(file.path));
                                task.ShowAssets = true;
                                task.ShowColors = false;
                                task.ShowScreenshots = false;
                                task.ShowSubtasks = false;
                                task.ShowTags = false;

                                FileManager.SaveAll();
                            });
                        });
                    }
                }
            }
            if (!show)
            {
                menu.AddDisabledItem(new GUIContent(Language.MoveToBoard));
            }
            menu.ShowAsContext();
        }
        public void Reset()
        {
            MakeUpdateUISafe(() => { files.Clear(); });
        }
    }
}
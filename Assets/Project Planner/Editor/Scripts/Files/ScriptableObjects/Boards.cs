using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Boards : FileBase, ISerializationCallbackReceiver
    {
        public static Boards I
        {
            get
            {
                return FileManager.GetFile<Boards>("Boards");
            }
        }

        //fields
        [SerializeField]
        private List<Board> _all;
        public List<Board> All
        {
            get
            {
                return _all;
            }
            private set
            {
                _all = value;
            }
        }
        private Dictionary<string, Board> IDlinks;

        //data
        private void Add(Board board)
        {
            if (All == null)
                All = new List<Board>();
            if(!All.Contains(board))
                All.Add(board);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Board>();
            if(!IDlinks.ContainsKey(board.ID))
                IDlinks.Add(board.ID, board);

            FileManager.SaveAll();
        }
        public Board Get(string id)
        {
            if (IDlinks.ContainsKey(id))
                return IDlinks[id];

            return null;
        }

        //edit
        public void ShowContextMenu(Board board, Action<Board> renameCallback, Action repaintCallback, Action<Action> safeUICallback)
        {
            GenericMenu menu = new GenericMenu();

            //rename
            menu.AddItem(new GUIContent(Language.Rename), false, () =>
            {
                renameCallback(board);
            });

            //delete
            if (!board.HasGroupsWithLockedTask())
            {
                menu.AddItem(new GUIContent(Language.Delete), false, () =>
                {
                    safeUICallback(() =>
                    {
                        UndoHandler.SaveState("Delete Board");

                        Delete(board);

                        InvokeCallback(repaintCallback);
                    });
                });
            }
            else
            {
				//DLL 2017 remove false
                menu.AddDisabledItem(new GUIContent(Language.Delete + " [" + Language.Locked + "]"), false);
            }

            //new group
            menu.AddItem(new GUIContent(Language.NewGroup), false, () =>
            {
                safeUICallback(() =>
                {
                    UndoHandler.SaveState("New Group");

                    Groups.I.New(board);

                    InvokeCallback(repaintCallback);
                });
            });

            menu.ShowAsContext();
        }

        public void ValidateBoardNames()
        {
            List<string> names = new List<string>();
            foreach (var board in Boards.I.All)
            {
                int postfix = 1;
                string originalName = board.Name;

                while (names.Contains(board.Name) || board.Name == "")
                {
                    board.Name = originalName + postfix;
                    postfix++;
                }

                if (board.Name != originalName)
                {
                    FileManager.SaveAll();
                }

                names.Add(board.Name);
            }
        }
        public Board New(bool applyDefault = true)
        {
            Board board = new Board().Constructor();
            if (applyDefault)
                board.Default();

            Add(board);

            ValidateBoardNames();
            Groups.I.ValidateNames(board);

            FileManager.SaveAll();

            return board;
        }
        public void Delete(Board board)
        {
            //delete group
            foreach (var groupId in board.GroupIds.ToArray())
                Groups.I.Delete(Groups.I.Get(groupId), false, false);

            if (All == null)
                All = new List<Board>();
            if (All.Contains(board))
                All.Remove(board);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Board>();
            if (IDlinks.ContainsKey(board.ID))
                IDlinks.Remove(board.ID);

            FileManager.SaveAll();
        }

        //helpers
        private void InvokeCallback(Action callback)
        {
            if (callback != null)
                callback.Invoke();
        }
        public override void Setup()
        {
            All = new List<Board>();
            IDlinks = new Dictionary<string, Board>();
        }
        public void OnAfterDeserialize()
        {
            IDlinks = new Dictionary<string, Board>();

            All.RemoveAll(item => item == null);
            foreach (var board in All.ToArray())
            {
                IDlinks.Add(board.ID, board);
            }
        }
        public void OnBeforeSerialize() { }
    }

    [CustomEditor(typeof(Boards))]
    public class BoardFileEditor : Editor { public override void OnInspectorGUI() { } }
}
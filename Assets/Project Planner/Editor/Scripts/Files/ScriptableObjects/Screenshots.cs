using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public class Screenshots : FileBase, ISerializationCallbackReceiver
    {
        public static Screenshots I
        {
            get
            {
                return FileManager.GetFile<Screenshots>("Screenshots");
            }
        }

        //fields
        [SerializeField]
        private List<Screenshot> _all;
        public List<Screenshot> All
        {
            get
            {
                if (_all == null)
                    _all = new List<Screenshot>();

                return _all;
            }
            private set
            {
                _all = value;
            }
        }
        private Dictionary<string, Screenshot> IDlinks;
        
        public void Add_USED_BY_COPY_ONLY(Screenshot screenshot, Task task)
        {
            Add(screenshot, task);
        }
        private void Add(Screenshot screenshot, Task task)
        {
            if (All == null)
                All = new List<Screenshot>();
            if (!All.Contains(screenshot))
                All.Add(screenshot);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Screenshot>();
            if (!IDlinks.ContainsKey(screenshot.ID))
                IDlinks.Add(screenshot.ID, screenshot);

            task.Screenshots.Add(screenshot.ID);

            FileManager.SaveAll();
        }
        public Screenshot Get(string id)
        {
            if (IDlinks != null && IDlinks.ContainsKey(id))
                return IDlinks[id];

            return null;
        }

        public Screenshot New(Task task)
        {
            Screenshot screenshot = new Screenshot().Constructor(task).Default();
            screenshot.Capture();
            Add(screenshot, task);

            FileManager.SaveAll();

            return screenshot;
        }
        public void Delete(string screenshotId)
        {
            Screenshot screenshot = Get(screenshotId);
            if (screenshot != null)
            {
                Delete(screenshot);
            }
        }
        public void Delete(Screenshot screenshot)
        {
            if (screenshot == null)
                return;

            Task task = Tasks.I.Get(screenshot.TaskID);

            if (task != null)
                task.Screenshots.Remove(screenshot.ID);

            if (All == null)
                All = new List<Screenshot>();
            if (All.Contains(screenshot))
                All.Remove(screenshot);

            if (IDlinks == null)
                IDlinks = new Dictionary<string, Screenshot>();
            if (IDlinks.ContainsKey(screenshot.ID))
                IDlinks.Remove(screenshot.ID);

            FileManager.MoveSceenshotToTmp(screenshot);
            FileManager.SaveAll();
        }
        public void RecorverTmps()
        {
            foreach (var screenshot in All.ToArray())
            {
                string tmpFolder = FileManager.ScreenshotsFolder("tmp");
                string tmpFilePath = FileManager.ScreenshotsFolder("tmp") + screenshot.ID + ".png";
                if (Directory.Exists(tmpFolder) && File.Exists(tmpFilePath))
                {
                    string filePath = FileManager.ScreenshotsFolder(screenshot.TaskID) + screenshot.ID + ".png";
                    File.Move(tmpFilePath, filePath);
                }
            }

            FileManager.SaveAll();
        }

        public void OnAfterDeserialize()
        {
            IDlinks = new Dictionary<string, Screenshot>();

            All.RemoveAll(item => item == null);
            foreach (var screenshot in All.ToArray())
            {
                IDlinks.Add(screenshot.ID, screenshot);
            }
        }
        public void OnBeforeSerialize() { }
    }

    [CustomEditor(typeof(Screenshots))]
    public class ScreenshotsCustomEditor : Editor { public override void OnInspectorGUI() { } }
}
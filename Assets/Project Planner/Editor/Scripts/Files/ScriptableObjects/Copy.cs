using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Copy : FileBase, ISerializationCallbackReceiver
    {
        public static Copy I
        {
            get
            {
                return FileManager.GetFile<Copy>("Copy");
            }
        }

        //group
        [SerializeField]
        private List<Group> _groups;
        public List<Group> Groups
        {
            get
            {
                return _groups;
            }
            private set
            {
                _groups = value;
            }
        }
        private Dictionary<string, Group> groupIDlinks;

        public void AddGroup(Group group)
        {
            if (Groups == null)
                Groups = new List<Group>();
            if (!Groups.Contains(group))
                Groups.Add(group);

            if (groupIDlinks == null)
                groupIDlinks = new Dictionary<string, Group>();
            if (!groupIDlinks.ContainsKey(group.ID))
                groupIDlinks.Add(group.ID, group);

            FileManager.SaveAll();
        }
        public Group GetGroup(string id)
        {
            if (groupIDlinks.ContainsKey(id))
                return groupIDlinks[id];

            return null;
        }

        //task
        [SerializeField]
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get
            {
                return _tasks;
            }
            private set
            {
                _tasks = value;
            }
        }
        private Dictionary<string, Task> tasksIDlinks;

        public void AddTask(Task task)
        {
            if (Tasks == null)
                Tasks = new List<Task>();
            if (!Tasks.Contains(task))
                Tasks.Add(task);

            if (tasksIDlinks == null)
                tasksIDlinks = new Dictionary<string, Task>();
            if (!tasksIDlinks.ContainsKey(task.ID))
                tasksIDlinks.Add(task.ID, task);

            FileManager.SaveAll();
        }
        public Task GetTask(string id)
        {
            if (tasksIDlinks.ContainsKey(id))
                return tasksIDlinks[id];

            return null;
        }


        //screenshot
        [SerializeField]
        private List<Screenshot> _screenshot;
        public List<Screenshot> Screenshots
        {
            get
            {
                return _screenshot;
            }
            private set
            {
                _screenshot = value;
            }
        }
        private Dictionary<string, Screenshot> screenshotsIDlinks;

        public void AddScreenshot(Screenshot screenshot)
        {
            if (Screenshots == null)
                Screenshots = new List<Screenshot>();
            if (!Screenshots.Contains(screenshot))
                Screenshots.Add(screenshot);

            if (screenshotsIDlinks == null)
                screenshotsIDlinks = new Dictionary<string, Screenshot>();
            if (!screenshotsIDlinks.ContainsKey(screenshot.ID))
                screenshotsIDlinks.Add(screenshot.ID, screenshot);

            FileManager.SaveAll();
        }
        public Screenshot GetScreenshot(string id)
        {
            if (screenshotsIDlinks.ContainsKey(id))
                return screenshotsIDlinks[id];

            return null;
        }

        public override void Setup()
        {
            //groups
            Groups = new List<Group>();
            groupIDlinks = new Dictionary<string, Group>();

            //tasks
            Tasks = new List<Task>();
            tasksIDlinks = new Dictionary<string, Task>();

            //tasks
            Screenshots = new List<Screenshot>();
            screenshotsIDlinks = new Dictionary<string, Screenshot>();
        }
        public void OnAfterDeserialize()
        {
            //groups
            groupIDlinks = new Dictionary<string, Group>();
            Groups.RemoveAll(item => item == null);
            foreach (var group in Groups.ToArray())
            {
                groupIDlinks.Add(group.ID, group);
            }

            //tasks
            tasksIDlinks = new Dictionary<string, Task>();
            Tasks.RemoveAll(item => item == null);
            foreach (var task in Tasks.ToArray())
            {
                tasksIDlinks.Add(task.ID, task);
            }

            //screenshots
            screenshotsIDlinks = new Dictionary<string, Screenshot>();
            Screenshots.RemoveAll(item => item == null);
            foreach (var screenshot in Screenshots.ToArray())
            {
                screenshotsIDlinks.Add(screenshot.ID, screenshot);
            }
        }
        public void OnBeforeSerialize() { }
    }

    [CustomEditor(typeof(Copy))]
    public class CopyEditor : Editor { public override void OnInspectorGUI() { } }
}
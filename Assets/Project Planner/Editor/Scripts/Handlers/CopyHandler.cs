using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ProjectPlanner
{
    static class CopyHandler
    {
        private const string rootId = "root";

        public enum CopiedType
        {
            None,
            Group,
            Task
        }
        public static CopiedType GetCopiedType
        {
            get
            {
                if (Copy.I.Groups.Count != 0)
                {
                    return CopiedType.Group;
                }
                if (Copy.I.Tasks.Count != 0)
                {
                    return CopiedType.Task;
                }
                return CopiedType.None;
            }
        }

        //groups
        public static void CopyGroup(Group group)
        {
            Copy.I.Setup();

            List<Task> allTasks = new List<Task>();
            Group copy = MoveGroupToCopy(group, ref allTasks);
            copy.BoardID = rootId;
            Copy.I.AddGroup(copy);
            foreach (var item in allTasks)
            {
                Copy.I.AddTask(item);
            }

            FileManager.SaveAll();
        }
        public static Group PasteGroup(Board parent, int index)
        {
            if (index < 0)
                index = 0;
            if (index > parent.GroupIds.Count)
                index = parent.GroupIds.Count;

            List<Task> allTasks = new List<Task>();
            Group root = MoveGroupToData(Copy.I.Groups.Where(o => o.BoardID == rootId).First(), ref allTasks);
            Groups.I.Add_USED_BY_COPY_ONLY(root);

            foreach (var item in allTasks)
            {
                Tasks.I.Add_USED_BY_COPY_ONLY(item, null);
            }

            root.BoardID = parent.ID;
            parent.GroupIds.Insert(index, root.ID);

            FileManager.SaveAll();
            return root;
        }

        private static Group MoveGroupToCopy(Group group, ref List<Task> allTasks)
        {
            if (group == null)
                return null;

            List<string> subtasks;
            Group copy = group.Clone(out subtasks);

            foreach (var subtaskId in subtasks)
            {
                Task subtask = MoveTaskToCopy(Tasks.I.Get(subtaskId), ref allTasks);
                subtask.ParentID = copy.ID;
                copy.TasksIds.Add(subtask.ID);
            }

            return copy;
        }
        private static Group MoveGroupToData(Group group, ref List<Task> allTasks)
        {
            if (group == null)
                return null;

            List<string> subtasks;
            Group copy = group.Clone(out subtasks);

            foreach (var subtaskId in subtasks)
            {
                Task subtask = MoveTaskToData(Copy.I.GetTask(subtaskId), ref allTasks);
                subtask.ParentID = copy.ID;
                copy.TasksIds.Add(subtask.ID);
            }

            return copy;
        }

        //task
        public static void CopyTask(Task taskToCopy)
        {
            Copy.I.Setup();

            List<Task> allTasks = new List<Task>();
            MoveTaskToCopy(taskToCopy, ref allTasks).ParentID = rootId;
            foreach (var task in allTasks)
            {
                foreach (var screenshotId in task.Screenshots.ToArray())
                {
                    Screenshot oldScreenshot = Screenshots.I.Get(screenshotId);
                    Screenshot copyScreenshot = oldScreenshot.Clone();
                    copyScreenshot.TaskID = task.ID;

                    FileManager.MoveSceenshotToCopy(oldScreenshot, copyScreenshot.ID);
                    Copy.I.AddScreenshot(copyScreenshot);

                    task.Screenshots.Remove(oldScreenshot.ID);
                    task.Screenshots.Add(copyScreenshot.ID);
                }

                Copy.I.AddTask(task);
            }

            FileManager.SaveAll();
        }
        public static Task PasteTask(IHaveTasks parent, int index)
        {
            if (index < 0)
                index = 0;
            if (index > parent.TasksIds.Count)
                index = parent.TasksIds.Count;

            List<Task> allTasks = new List<Task>();
            Task root = MoveTaskToData(Copy.I.Tasks.Where(o => o.ParentID == rootId).First(), ref allTasks);
            foreach (var task in allTasks)
            {
                Tasks.I.Add_USED_BY_COPY_ONLY(task, null);

                foreach (var screenshotId in task.Screenshots.ToArray())
                {
                    Screenshot copyScreenshot = Copy.I.GetScreenshot(screenshotId);
                    Screenshot pasteScreenshot = copyScreenshot.Clone();
                    pasteScreenshot.TaskID = task.ID;

                    FileManager.MoveSceenshotFromCopy(copyScreenshot, pasteScreenshot);
                    task.Screenshots.Remove(copyScreenshot.ID);
                    Screenshots.I.Add_USED_BY_COPY_ONLY(pasteScreenshot, task);
                }
            }

            root.ParentID = parent.GetID();
            parent.TasksIds.Insert(index, root.ID);
            root.IsParentATask = parent.IsATask();

            FileManager.SaveAll();
            return root;
        }

        private static Task MoveTaskToCopy(Task task, ref List<Task> allTasks)
        {
            if (task == null)
                return null;

            List<string> subtasks;
            Task copy = task.Clone(out subtasks);

            foreach (var subtaskId in subtasks)
            {
                Task subtask = MoveTaskToCopy(Tasks.I.Get(subtaskId), ref allTasks);
                subtask.ParentID = copy.ID;
                copy.TasksIds.Add(subtask.ID);
            }

            allTasks.Add(copy);
            return copy;
        }
        private static Task MoveTaskToData(Task task, ref List<Task> allTasks)
        {
            if (task == null)
                return null;

            List<string> subtasks;
            Task copy = task.Clone(out subtasks);

            foreach (var subtaskId in subtasks)
            {
                Task subtask = MoveTaskToData(Copy.I.GetTask(subtaskId), ref allTasks);
                subtask.ParentID = copy.ID;
                copy.TasksIds.Add(subtask.ID);
            }

            allTasks.Add(copy);
            return copy;
        }
    }
}
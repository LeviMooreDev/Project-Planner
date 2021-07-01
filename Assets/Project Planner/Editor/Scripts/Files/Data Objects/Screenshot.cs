using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public class Screenshot : ISerializationCallbackReceiver
    {
        public string ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
            }
        }
        [SerializeField]
        private string _id;

        public string TaskID
        {
            get
            {
                return _taskID;
            }
            set
            {
                _taskID = value;
            }
        }
        [SerializeField]
        private string _taskID;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        [SerializeField]
        private string _name;

        //created
        public long CreatedTicks
        {
            get
            {
                return _createdTicks;
            }
            set
            {
                _createdTicks = value;
            }
        }
        [SerializeField]
        private long _createdTicks;
        private DateTime createdDateTime;
        public string CreatedFormatted
        {
            get
            {
                return createdDateTime.ToString("g");
            }
        }

        public Texture2D Image
        {
            get
            {
                return FileManager.GetScreenshotTexture2D(this);
            }
        }


        public Screenshot Constructor(Task task)
        {
            ID = Info.GetNewID();
            if(task != null)
                TaskID = task.ID;
            Name = string.Empty;
            createdDateTime = new DateTime();
            createdDateTime = DateTime.Now;
            CreatedTicks = createdDateTime.Ticks;

            return this;
        }
        public Screenshot Default()
        {
            Name = Settings.Screenshots.DefaultName;

            return this;
        }

        public void Capture()
        {
            //DLL
            #if UNITY_2018_2_OR_NEWER
                    EditorApplication.ExecuteMenuItem("Window/General/Game");
            #else
                    EditorApplication.ExecuteMenuItem("Window/Game");
            #endif
            ScreenCapture.CaptureScreenshot(FileManager.ScreenshotsFolder(TaskID) + ID + ".png");
        }

        public void OnBeforeSerialize()
        {
            CreatedTicks = createdDateTime.Ticks;
        }
        public void OnAfterDeserialize()
        {
            createdDateTime = new DateTime(CreatedTicks);
        }


        public Screenshot Clone()
        {
            Screenshot copiedScreenshot = new Screenshot().Constructor(Tasks.I.Get(TaskID));

            copiedScreenshot.Name = Name;
            copiedScreenshot.CreatedTicks = CreatedTicks;

            return copiedScreenshot;
        }

    }
}
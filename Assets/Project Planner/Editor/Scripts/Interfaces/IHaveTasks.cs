using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPlanner
{
    public interface IHaveTasks
    {
        List<string> TasksIds
        {
            get;
            set;
        }

        bool HasLockedTask(bool recursive = true);
        void Lock(bool recursive = false);
        void Unlock(bool recursive = false);

        string GetID();
        bool IsATask();
    }
}

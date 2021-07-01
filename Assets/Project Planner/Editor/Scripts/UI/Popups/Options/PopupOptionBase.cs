using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPlanner.Popups
{
    public class PopupOption
    {
        public string Name
        {
            get;
            private set;
        }
        public Action Action
        {
            get;
            private set;
        }

        public PopupOption(string name, Action action)
        {
            this.Name = name;
            this.Action = action;
        }
    }
}

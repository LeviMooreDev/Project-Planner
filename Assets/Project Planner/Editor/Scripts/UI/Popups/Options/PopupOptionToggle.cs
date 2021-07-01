using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPlanner.Popups
{
    public class PopupOptionToggle : PopupOption
    {
        public bool On
        {
            get;
            set;
        }

        public PopupOptionToggle(string name, bool on, Action action) : base(name, action)
        {
            this.On = on;
        }
    }
}

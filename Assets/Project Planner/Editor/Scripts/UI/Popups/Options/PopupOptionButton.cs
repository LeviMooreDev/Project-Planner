using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectPlanner.Popups
{
    public class PopupOptionButton : PopupOption
    {
        public PopupOptionButton(string name, Action action) : base(name, action)
        {

        }
    }
}

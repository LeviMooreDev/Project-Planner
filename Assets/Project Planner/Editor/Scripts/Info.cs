using System;
using UnityEngine;

namespace ProjectPlanner
{
    public static class Info
    {
        public readonly static string Name = "Project Planner";
        public readonly static string Version = "1.5";
        public readonly static string GitHub = "https://github.com/LeviMooreDev/Project-Planner";

        public static string Prefix
        {
            get
            {
                return Name.Replace(" ", string.Empty) + ".";
            }
        }


        public static string GetNewID()
        {
            return SystemInfo.deviceUniqueIdentifier + "-" + Guid.NewGuid();
        }
    }
}

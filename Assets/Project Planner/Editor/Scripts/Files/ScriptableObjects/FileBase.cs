using System;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [Serializable]
    public abstract class FileBase : ScriptableObject
    {
        public virtual void Setup() { }
    }
}
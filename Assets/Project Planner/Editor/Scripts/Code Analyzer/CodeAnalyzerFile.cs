using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    [System.Serializable]
    public class CodeAnalyzerFile
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string path;
        [SerializeField]
        public List<CodeAnalyzerMessage> messages;
        [SerializeField]
        public bool showMessages;

        public CodeAnalyzerFile(string path, string name)
        {
            this.name = name;
            this.path = path;
            messages = new List<CodeAnalyzerMessage>();
            showMessages = Settings.CodeAnalyzer.FoldoutOpenByDefault;
        }
    }
}
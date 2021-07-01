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
    public class CodeAnalyzerMessage
    {
        [SerializeField]
        public string scanWord;
        [SerializeField]
        public string value;
        [SerializeField]
        public int number;
        [SerializeField]
        public bool searchShow;
        [SerializeField]
        public bool notImplementedException;

        public Rect rectAbsolute;

        public CodeAnalyzerMessage(string scanWord, string value, int number, bool notImplementedException = false)
        {
            this.scanWord = scanWord;
            this.value = value;
            this.number = number;
            this.notImplementedException = notImplementedException;
            searchShow = true;
        }
    }
}
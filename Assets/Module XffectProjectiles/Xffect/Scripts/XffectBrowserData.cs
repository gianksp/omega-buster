using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//NOTE: THIS CLASS CAN'T BE PUT INTO THE EDITOR FOLDER, OR IT CAN'T BE LOADED WHEN RELAUNCH UNITY.
namespace Xft
{
    [System.Serializable]
    public class XffectPresetData
    {
        public string Category;
        public string Path;

        public XffectPresetData(string c, string p)
        {
            Category = c;
            Path = p;
        }
    }

    [System.Serializable]
    public class XffectBrowserData : ScriptableObject
    {
        public List<XffectPresetData> Presets = new List<XffectPresetData>();
    }
}

using System;
using UnityEngine;

namespace AlienUI.Models
{
    public class AmlAsset : ScriptableObject
    {
        public string Text;

#if UNITY_EDITOR
        public string Path => UnityEditor.AssetDatabase.GetAssetPath(this);

        public void SaveToDisk()
        {
            System.IO.File.WriteAllText(Path, Text);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }

        public static event Action<string> OnAssetReimported;
        public static void NotifyAmlReimported(string assetPath)
        {
            OnAssetReimported?.Invoke(assetPath);
        }
#endif
    }
}

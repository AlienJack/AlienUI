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
#endif
    }
}

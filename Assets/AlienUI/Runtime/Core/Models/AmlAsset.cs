using UnityEngine;

namespace AlienUI.Models
{
    public class AmlAsset : ScriptableObject
    {
        public string Text;
        public string Path;

#if UNITY_EDITOR
        public void SaveToDisk()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);

            System.IO.File.WriteAllText(Path, Text);
        }
#endif
    }
}

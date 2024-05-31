using System.IO;
using UnityEditor;

namespace AlienUI.Editors
{
    public class AmlAssetMidificationProcessor : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] assets)
        {
            foreach (string path in assets)
            {
                if (Path.GetExtension(path) != ".aml") continue;

                Settings.Get().CollectAsset();
            }

            return assets;
        }

        public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            if (Path.GetExtension(path) != ".aml") return AssetDeleteResult.DidNotDelete;

            Settings.Get().CollectAsset(path);

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
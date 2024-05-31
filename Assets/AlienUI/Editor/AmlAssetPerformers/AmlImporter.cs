using AlienUI.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace AlienUI.Editors
{

    [ScriptedImporter(1, "aml")]
    public class AmlImporter : ScriptedImporter
    {
        public Sprite Resource;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xmlContent = File.ReadAllText(ctx.assetPath);

            // 创建MyCustomXml实例并填充数据
            var amlAsset = ScriptableObject.CreateInstance<AmlAsset>();
            amlAsset.Text = xmlContent;
            // 添加到导入上下文
            ctx.AddObjectToAsset("main obj", amlAsset);
            ctx.SetMainObject(amlAsset);            

            EditorApplication.delayCall += handleCollectAsset;
        }

        private void handleCollectAsset()
        {
            Settings.Get().CollectAsset(null);
            EditorApplication.delayCall -= handleCollectAsset;
        }

        internal static bool OverrideAMLOpen = true;
        [OnOpenAsset(1, OnOpenAssetAttributeMode.Execute)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (!OverrideAMLOpen) return false;
            Object asset = EditorUtility.InstanceIDToObject(instanceID);

            // 检查资源是否是自定义资源类型
            if (asset is AmlAsset aa)
            {
                if (Settings.Get().DesignerLayout)
                    EditorUtility.LoadWindowLayout(AssetDatabase.GetAssetPath(Settings.Get().DesignerLayout));

                var prefab = Settings.Get().EditPrefab;
                var path = AssetDatabase.GetAssetPath(prefab);
                var stage = PrefabStageUtility.OpenPrefab(path);
                var engine = stage.prefabContentsRoot.GetComponent<Engine>();
                var canvas = engine.transform.parent.GetComponent<Canvas>();
                if (canvas) canvas.renderMode = RenderMode.WorldSpace;
                (canvas.transform as RectTransform).sizeDelta = Settings.Get().DesignSize;
                var ui = engine.CreateUI(aa, engine.transform, null);

                Designer.GetWindow<Designer>().StartDesginer(ui, aa);

                return !Settings.Get().OpenAmlFileWhenOpenDesigner; //return false to open file with os default open action
            }

            // 不是自定义资源，返回false让Unity处理
            return false;
        }
    }
}
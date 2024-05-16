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
    [CustomEditor(typeof(AmlImporter))]
    public class AmlImporterEditor : AssetImporterEditor
    {
        protected override bool needsApplyRevert => false;
        public override bool showImportedObject => true;

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {

        }
    }

    public class AmlAssetMidificationProcessor : AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] assets)
        {
            foreach (string path in assets)
            {
                if (Path.GetExtension(path) != "aml") continue;

                var aml = AssetDatabase.LoadAssetAtPath<AmlAsset>(path);
                Debug.Log(aml.Text);
            }

            return assets;
        }
    }

    [ScriptedImporter(1, "aml")]
    public class AmlImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xmlContent = File.ReadAllText(ctx.assetPath);

            // 创建MyCustomXml实例并填充数据
            var amlAsset = ScriptableObject.CreateInstance<AmlAsset>();
            amlAsset.Text = xmlContent;
            amlAsset.Path = ctx.assetPath;
            // 添加到导入上下文
            ctx.AddObjectToAsset("main obj", amlAsset);
            ctx.SetMainObject(amlAsset);
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

                Designer.GetWindow<Designer>().SetTarget(ui, aa);

                return false; //return false to open file with os default open action
            }

            // 不是自定义资源，返回false让Unity处理
            return false;
        }
    }
}
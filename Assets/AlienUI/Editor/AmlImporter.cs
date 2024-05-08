using AlienUI.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
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

    [ScriptedImporter(1, "aml")]
    public class AmlImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xmlContent = File.ReadAllText(ctx.assetPath);

            // 创建MyCustomXml实例并填充数据
            var amlAsset = ScriptableObject.CreateInstance<AmlAsset>();
            amlAsset.Text = xmlContent;

            // 添加到导入上下文
            ctx.AddObjectToAsset("main obj", amlAsset);
            ctx.SetMainObject(amlAsset);
        }
    }

}
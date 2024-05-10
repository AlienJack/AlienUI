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

    [ScriptedImporter(1, "aml")]
    public class AmlImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var xmlContent = File.ReadAllText(ctx.assetPath);

            // ����MyCustomXmlʵ�����������
            var amlAsset = ScriptableObject.CreateInstance<AmlAsset>();
            amlAsset.Text = xmlContent;

            // ��ӵ�����������
            ctx.AddObjectToAsset("main obj", amlAsset);
            ctx.SetMainObject(amlAsset);
        }

        [OnOpenAsset(1, OnOpenAssetAttributeMode.Execute)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);

            // �����Դ�Ƿ����Զ�����Դ����
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

                return true;
            }

            // �����Զ�����Դ������false��Unity����
            return false;
        }
    }
}
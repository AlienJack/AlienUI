using AlienUI.Models;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
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

            // ����MyCustomXmlʵ�����������
            var amlAsset = ScriptableObject.CreateInstance<AmlAsset>();
            amlAsset.Text = xmlContent;
            // ��ӵ�����������
            ctx.AddObjectToAsset("main obj", amlAsset);
            ctx.SetMainObject(amlAsset);

            EditorApplication.delayCall += handleCollectAsset;

            var path = ctx.assetPath;
            EditorApplication.CallbackFunction notify = null;
            notify = () =>
            {
                EditorApplication.delayCall -= notify;
                AmlAsset.NotifyAmlReimported(path);
            };
            EditorApplication.delayCall += notify;
        }

        private void handleAmlReimported()
        {
            EditorApplication.delayCall -= handleAmlReimported;
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

                Designer.GetWindow<Designer>().StartDesginer(ui, aa);

                return !Settings.Get().OpenAmlFileWhenOpenDesigner; //return false to open file with os default open action
            }

            // �����Զ�����Դ������false��Unity����
            return false;
        }
    }
}
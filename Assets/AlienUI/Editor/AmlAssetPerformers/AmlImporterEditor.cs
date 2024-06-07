using AlienUI.Models;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.ProjectWindowCallback;
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

        public override void OnInspectorGUI() { }

        [MenuItem("Assets/Create/AlienUI/CreateWindow AML File", priority = 0)]
        public static void CreateWindowAmlFile()
        {
            var select = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.TopLevel).FirstOrDefault();
            if (select == null) return;

            var createPath = $"{AssetDatabase.GetAssetPath(select)}/Window.aml";
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                CreateInstance<WindowAmlCreator>(),
                createPath,
                AlienEditorUtility.GetIcon("amlicon"),
                AssetDatabase.LoadAssetAtPath<TextAsset>($"{Settings.RootPATH}/Editor/AMLTextTemplate/Window_AT.txt").text
                );
        }

        class WindowAmlCreator : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var fileName = Path.GetFileNameWithoutExtension(pathName);
                resourceFile = resourceFile.Replace("[FILENAME]", fileName);

                File.WriteAllText(pathName, resourceFile);
                AssetDatabase.ImportAsset(pathName);
                AssetDatabase.Refresh(); 
            }
        }
    }
}
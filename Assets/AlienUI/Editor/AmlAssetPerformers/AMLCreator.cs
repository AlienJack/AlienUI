using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AMLCreator : EndNameEditAction
    {
        [MenuItem("Assets/Create/AlienUI/CreateWindow AML File", priority = 0)]
        public static void CreateWindowAmlFile()
        {
            var select = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.TopLevel).FirstOrDefault();
            if (select == null) return;

            var createPath = $"{AssetDatabase.GetAssetPath(select)}/Window.aml";
            var templatePath = $"{Settings.RootPATH}/Editor/AMLTextTemplate/Window_AT.txt";

            AlienUITypeSearchWindow.OpenWindow<Window>((type) =>
            {
                createAmlFile(type, createPath, templatePath);
            });

        }

        [MenuItem("Assets/Create/AlienUI/CreateTemplate AML File", priority = 0)]
        public static void CreateTemplateAmlFile()
        {
            var select = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.TopLevel).FirstOrDefault();
            if (select == null) return;

            var createPath = $"{AssetDatabase.GetAssetPath(select)}/Template.aml";
            var templatePath = $"{Settings.RootPATH}/Editor/AMLTextTemplate/Template_AT.txt";
            AlienUITypeSearchWindow.OpenWindow<UserControl>((type) =>
            {
                createAmlFile(type, createPath, templatePath);
            });
        }

        private static void createAmlFile(Type linkType, string createPath, string codeTemplatePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<AMLCreator>().SetReplaceType(linkType),
                createPath,
                AlienEditorUtility.GetIcon("amlicon"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(codeTemplatePath).text
                );
        }


        private Type type;

        public AMLCreator SetReplaceType(Type type)
        {
            this.type = type;
            return this;
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(pathName);
            resourceFile = resourceFile.Replace("[FILENAME]", fileName);

            resourceFile = resourceFile.Replace("[CLASSNAME]", type.FullName);

            File.WriteAllText(pathName, resourceFile);
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
        }
    }
}

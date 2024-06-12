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
    public class ScriptCreator : EndNameEditAction
    {
        [MenuItem("Assets/Create/AlienUI/Create UserControl Class File", priority = 0)]
        public static void CreateUserControlScriptFile()
        {
            var select = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.TopLevel).FirstOrDefault();
            if (select == null) return;

            var createPath = $"{AssetDatabase.GetAssetPath(select)}/MyUserControl.cs";
            var templatePath = $"{Settings.RootPATH}/Editor/AMLTextTemplate/UserControl_CT.txt";

            createScriptFile(typeof(UserControl), createPath, templatePath);
        }

        private static void createScriptFile(Type linkType, string createPath, string codeTemplatePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                CreateInstance<ScriptCreator>().SetReplaceType(linkType),
                createPath,
                AlienEditorUtility.GetIcon("script"),
                AssetDatabase.LoadAssetAtPath<TextAsset>(codeTemplatePath).text
                );
        }

        private Type type;

        public ScriptCreator SetReplaceType(Type type)
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

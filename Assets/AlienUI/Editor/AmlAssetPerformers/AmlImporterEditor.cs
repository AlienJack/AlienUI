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

        
    }
}
using AlienUI.Models;
using AlienUI.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AlienUI.Editors
{
    [CustomEditor(typeof(AmlAsset))]
    public class AmlInspector : Editor
    {
        public override bool HasPreviewGUI() => true;

        private PreviewRenderUtility previewRenderSetting;
        private Canvas canvas;
        private UIElement ui;


        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);

            if (Event.current.type == EventType.Repaint)
            {
                preparePreview();

                previewRenderSetting.BeginPreview(new Rect(r) { size = r.size * 3 }, background);
                previewRenderSetting.camera.cameraType = CameraType.SceneView;
                previewRenderSetting.camera.Render();

                var tex = previewRenderSetting.EndPreview();
                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill, false);
            }
            else if (Event.current.type == EventType.ScrollWheel)
            {
                var delta = Event.current.delta;
                if (ui != null)
                {
                    var delta3 = new Vector3(delta.y, delta.y, delta.y);
                    ui.m_rectTransform.localScale -= delta3 * 0.1f;
                    Event.current.Use();
                }
            }
            else
            {
                var evt = Event.current;
                var id = GUIUtility.GetControlID(FocusType.Passive);

                switch (evt.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        if (r.Contains(evt.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            EditorGUIUtility.SetWantsMouseJumping(1);

                            if (evt.button == 1)
                            {
                                ui.Rect.localRotation = Quaternion.identity;
                                ui.Rect.localPosition = Vector3.zero;
                            }
                            evt.Use();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == id)
                        {
                            if (evt.button == 0)
                            {
                                var qua = Quaternion.AngleAxis(-Event.current.delta.x, ui.Rect.InverseTransformDirection(Vector3.up));
                                ui.Rect.localRotation *= qua;
                                qua = Quaternion.AngleAxis(-Event.current.delta.y, ui.Rect.InverseTransformDirection(Vector3.right));
                                ui.Rect.localRotation *= qua;
                                evt.Use();
                            }
                            else if (evt.button == 2)
                            {
                                var direct = new Vector3(evt.delta.x, -evt.delta.y, 0);
                                var length = direct.magnitude;
                                direct = ui.Rect.InverseTransformDirection(direct).normalized;
                                ui.Rect.localPosition += direct.normalized * length;
                                evt.Use();
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == id)
                            GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        break;
                }
            }
        }



        private void preparePreview()
        {
            if (previewRenderSetting != null) return;
            previewRenderSetting = new PreviewRenderUtility();
            previewRenderSetting.camera.transform.position = new Vector3(0, 0, -10);
            previewRenderSetting.camera.transform.rotation = Quaternion.identity;

            var previewRoot = previewRenderSetting.InstantiatePrefabInScene(Settings.Get().PreviewPrefab);

            var engine = previewRoot.GetComponentInChildren<Engine>();
            canvas = previewRoot.GetComponentInChildren<Canvas>();
            (canvas.transform as RectTransform).sizeDelta = Settings.Get().DesignSize;
            canvas.worldCamera = previewRenderSetting.camera;
            previewRenderSetting.camera.cullingMask = -1;
            previewRenderSetting.camera.fieldOfView = 15;
            previewRenderSetting.camera.farClipPlane = 1000;
            previewRenderSetting.camera.allowMSAA = true;

            ui = engine.CreateUI(target as AmlAsset, canvas.transform, null);
            engine.ForceHanldeDirty();
        }

        private void OnDisable()
        {
            previewRenderSetting?.Cleanup();
            previewRenderSetting = null;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            var tar = target as AmlAsset;
            EditorGUILayout.TextArea(tar.Text, new GUIStyle(EditorStyles.textArea) { wordWrap = true });
            EditorGUILayout.EndVertical();
        }
    }
}

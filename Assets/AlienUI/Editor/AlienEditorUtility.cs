using AlienUI.Models.Attributes;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AlienUI.Editors
{
    public static class AlienEditorUtility
    {
        internal static Texture2D GetIcon(string iconName)
        {
            if (iconName == null) return null;

            var guids = AssetDatabase.FindAssets("t:texture2d", new string[] { Path.Combine(Settings.RootPATH, "Editor/Icons") });
            foreach (var id in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(id);
                if (Path.GetFileNameWithoutExtension(path).ToLower() == iconName.ToLower())
                {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }
            return null;
        }

        public static Texture2D GetIcon(this AmlNodeElement element)
        {
            var type = element.GetType();
            return type.GetIcon();
        }

        public static Texture2D GetIcon(this Type type)
        {
            var des = type.GetCustomAttribute<DescriptionAttribute>(true);
            if (des == null) return GetIcon("DefaultUI");

            var icon = GetIcon(des.Icon);
            if (icon == null) return GetIcon("DefaultUI");

            return icon;
        }

        public static string GetDescrib(this Type type)
        {
            var des = type.GetCustomAttribute<DescriptionAttribute>(true);
            if (des == null) return string.Empty;
            return des.Des;
        }

        public static List<Type> GetAllowChildTypes(this AmlNodeElement element)
        {
            List<Type> result = new List<Type>();

            var type = element.GetType();
            var collector = element.Engine.AttParser.Collector;

            do
            {
                var att = type.GetCustomAttribute<AllowChildAttribute>();
                if (att != null)
                {
                    result.AddRange(att.childTypes.SelectMany(t => collector.GetTypesOfAssignFrom(t)));
                }
                if (type.BaseType == typeof(AmlNodeElement)) break;
                type = type.BaseType;
            } while (true);

            return result;
        }

        public static void DrawBorder(Rect rect, Color color)
        {
            Handles.BeginGUI();
            // ����������ɫΪ��ɫ
            Handles.color = color;
            // ���ƾ��ο�
            Handles.DrawWireCube(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0), new Vector3(rect.width, rect.height, 0));
            Handles.EndGUI();
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color32[] pix = new Color32[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new(width, height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        public static void DrawSceneBorder(this RectTransform rectTrans, Vector2 leftTopPos, Vector2 size, Color drawColor, Action<Rect> OnGUI = null, Color flashColor = default)
        {
            var gridRect = rectTrans;
            size *= gridRect.localScale;

            Vector2 offset = (Vector3)gridRect.rect.size * new Vector2(rectTrans.pivot.x, 1 - rectTrans.pivot.y);
            offset.Scale(new Vector3(1, -1, 1));
            leftTopPos -= offset;

            var posLT = gridRect.TransformPoint(leftTopPos);
            var posRT = posLT + new Vector3(size.x, 0, 0);
            var posRB = posRT + new Vector3(0, -size.y, 0);
            var posLB = posRB + new Vector3(-size.x, 0, 0);

            var temp = Handles.color;
            Handles.color = drawColor;

            Handles.DrawLine(posLT, posRT, 2);
            Handles.DrawLine(posRT, posRB, 2);
            Handles.DrawLine(posRB, posLB, 2);
            Handles.DrawLine(posLB, posLT, 2);
            Handles.color = temp;



            Handles.BeginGUI();

            var screenPos = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(posLT);
            screenPos.y = SceneView.lastActiveSceneView.camera.pixelHeight - screenPos.y;

            var screenPosMax = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(posRB);
            screenPosMax.y = SceneView.lastActiveSceneView.camera.pixelHeight - screenPosMax.y;

            var rectSize = screenPosMax - screenPos;
            var rect = new Rect { position = screenPos, width = rectSize.x, height = rectSize.y };

            if (OnGUI != null)
            {
                GUILayout.BeginArea(rect);
                OnGUI(rect);
                GUILayout.EndArea();
            }

            using (new AlienEditorUtility.GUIColorScope(flashColor))
                GUI.DrawTexture(rect, Texture2D.whiteTexture);
            Handles.EndGUI();
        }

        private static List<Type> unityAssetsTypes;

        internal static List<Type> GetUnityAssetsTypes()
        {
            if (unityAssetsTypes != null) return unityAssetsTypes;

            unityAssetsTypes = new List<Type>();
            foreach (var type in AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.FullName.Contains("Editor"))
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(UnityEngine.Object))))
            {
                unityAssetsTypes.Add(type);
            }

            return unityAssetsTypes;
        }

        private static Dictionary<string, Type> unityAssetsTypesMap;
        internal static Dictionary<string, Type> GetUnityAssetsTypesNameMap()
        {
            if (unityAssetsTypesMap != null) return unityAssetsTypesMap;

            unityAssetsTypesMap = new Dictionary<string, Type>();
            var typeList = GetUnityAssetsTypes();
            foreach (var type in typeList)
            {
                unityAssetsTypesMap[type.FullName] = type;
            }

            return unityAssetsTypesMap;
        }

        public static GUIColorScope BeginGUIColorScope(Color color)
        {
            return new GUIColorScope(color);
        }

        public static HandlesColorScope BeginHandlesColorScope(Color color)
        {
            return new HandlesColorScope(color);
        }

        public class GUIColorScope : IDisposable
        {
            Color m_beforeColor;
            public GUIColorScope(Color guiColor)
            {
                m_beforeColor = GUI.color;
                GUI.color = guiColor;
            }

            public void Dispose()
            {
                GUI.color = m_beforeColor;
            }
        }

        public class HandlesColorScope : IDisposable
        {
            Color m_beforeColor;
            public HandlesColorScope(Color handlesColor)
            {
                m_beforeColor = Handles.color;
                Handles.color = handlesColor;
            }

            public void Dispose()
            {
                Handles.color = m_beforeColor;
            }
        }
    }
}

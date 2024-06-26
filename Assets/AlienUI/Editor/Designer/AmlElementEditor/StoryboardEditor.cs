using AlienUI.Core;
using AlienUI.Core.Resources;
using AlienUI.Editors.TimelineDrawer;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Animation = AlienUI.Core.Resources.Animation;

namespace AlienUI.Editors
{
    public class StoryboardEditor : ElementExtraEdit<Storyboard>
    {
        private Dictionary<Storyboard, float> m_scaleMap = new Dictionary<Storyboard, float>();
        private Dictionary<Animation, AnimationTimelineUI> m_animationDrawerMap = new Dictionary<Animation, AnimationTimelineUI>();
        protected override void OnDraw(UIElement host, Storyboard element)
        {
            var anis = element.GetChildren<Animation>();
            using (new EditorGUILayout.VerticalScope())
            {
                if (!m_scaleMap.TryGetValue(element, out var scale)) scale = 60f;
                m_scaleMap[element] = EditorGUILayout.Slider(scale, 60f, 500);

                foreach (var ani in anis)
                {
                    EditorGUILayout.LabelField(string.Empty, EditorStyles.helpBox, GUILayout.Height(100));
                    if (Event.current.type != EventType.Used && Event.current.type != EventType.Layout)
                    {
                        var rect = GUILayoutUtility.GetLastRect();
                        rect.xMin += 4;
                        rect.xMax -= 4;
                        rect.yMin += 4;
                        rect.yMax -= 4;
                        if (!m_animationDrawerMap.ContainsKey(ani)) m_animationDrawerMap[ani] = new AnimationTimelineUI(ani);
                        m_animationDrawerMap[ani].Scale = m_scaleMap[element];
                        m_animationDrawerMap[ani].Draw(rect);
                    }
                }
            }
        }
    }

    public class AnimationTimelineUI : TimelineDrawer<Animation>
    {
        public AnimationTimelineUI(Animation timelineObj) : base(timelineObj) { }

        protected override void OnDrawInfo(Rect infoDrawRect, Animation animation)
        {
            var targetFieldRect = new Rect(infoDrawRect);
            targetFieldRect.height = 20;
            targetFieldRect.width = 10 * 6;
            EditorGUI.LabelField(targetFieldRect, "Target");

            var targetValueRect = new Rect(targetFieldRect);
            targetValueRect.x = targetFieldRect.width + 2;
            targetValueRect.width = infoDrawRect.width - targetFieldRect.width - 2;

            if (animation.GetBinding(Animation.TargetProperty) is Binding targetbind)
            {
                using (new AlienEditorUtility.GUIColorScope(Color.yellow))
                    EditorGUI.LabelField(targetValueRect, targetbind.SourceCode);
            }
            else
            {
                var options = animation.Document.GetAllElements().Where(el => el is UIElement && !string.IsNullOrWhiteSpace(el.Name)).Select(el => el.Name).ToList();
                var index = options.IndexOf(animation.Target.GetUniqueTag());
                index = EditorGUI.Popup(targetValueRect, index, options.ToArray());
                if (index != -1)
                    animation.Target = new Models.DependencyObjectRef(options[index]);
            }

            var animationTarget = animation.Target.Get(animation);
            if (animationTarget != null)
            {
                var propertyFieldRect = new Rect(targetFieldRect);
                propertyFieldRect.y += targetFieldRect.height + 2;
                EditorGUI.LabelField(propertyFieldRect, "Property");

                var propertyValueRect = new Rect(propertyFieldRect);
                propertyValueRect.x = propertyFieldRect.width + 2;
                propertyValueRect.width = infoDrawRect.width - propertyFieldRect.width - 2;

                if (animation.GetBinding(Animation.PropertyNameProperty) is Binding propertyBind)
                {
                    using (new AlienEditorUtility.GUIColorScope(Color.yellow))
                        EditorGUI.LabelField(propertyValueRect, propertyBind.SourceCode);
                }
                else
                {
                    var properties = new List<DependencyProperty>();
                    properties.AddRange(animationTarget.GetAllDependencyProperties());
                    properties.AddRange(animationTarget.GetAttachedProperties());

                    var options = properties.Select(p => p.PropName).ToList();
                    var index = options.IndexOf(animation.PropertyName);
                    index = EditorGUI.Popup(propertyValueRect, index, options.ToArray());
                    if (index != -1)
                        animation.PropertyName = options[index];
                }

                if (animation.GetBinding(Animation.OffsetProperty) is Binding offsetBind)
                {
                    var offsetFieldRect = new Rect(propertyFieldRect);
                    offsetFieldRect.y += propertyFieldRect.height + 2;
                    EditorGUI.LabelField(offsetFieldRect, "Offset");

                    var offsetValueRect = new Rect(offsetFieldRect);
                    offsetValueRect.x = offsetFieldRect.width + 2;
                    offsetValueRect.width = infoDrawRect.width - offsetFieldRect.width - 2;

                    using (new AlienEditorUtility.GUIColorScope(Color.yellow))
                        EditorGUI.LabelField(offsetValueRect, offsetBind.SourceCode);
                }
                else
                {
                    var offsetFieldRect = new Rect(propertyFieldRect);
                    offsetFieldRect.y += propertyFieldRect.height + 2;
                    offsetFieldRect.width = infoDrawRect.width - 7;
                    var temp = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 60 - 7;
                    animation.Offset = EditorGUI.FloatField(offsetFieldRect, "Offset", animation.Offset);
                    EditorGUIUtility.labelWidth = temp;
                }

                var curveFieldRect = new Rect(propertyFieldRect);
                curveFieldRect.y += propertyFieldRect.height * 2 + 4;
                curveFieldRect.width = infoDrawRect.width - 7;
                var temp1 = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60 - 7;
                animation.Curve = EditorGUI.CurveField(curveFieldRect, "Curve", animation.Curve);
                EditorGUIUtility.labelWidth = temp1;
            }

            if (GUI.changed)
            {
                Designer.SaveToAml();
            }
        }

        protected override List<float> GetKeyTime(Animation target)
        {
            return target.GetChildren<AnimationKey>().Select(key => key.Time + target.Offset).ToList();
        }
    }
}

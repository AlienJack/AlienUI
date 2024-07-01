using AlienUI.Core;
using AlienUI.Core.Resources;
using AlienUI.Editors.TimelineDrawer;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using System.Linq;
using ToolKits;
using UnityEditor;
using UnityEngine;
using Animation = AlienUI.Core.Resources.Animation;

namespace AlienUI.Editors
{
    public class StoryboardEditor : ElementExtraEdit<Storyboard>
    {
        private Dictionary<Storyboard, float> m_scaleMap = new Dictionary<Storyboard, float>();
        private Dictionary<Storyboard, TimeControl> m_timeControls = new Dictionary<Storyboard, TimeControl>();
        private Dictionary<Storyboard, float> m_timeSample = new Dictionary<Storyboard, float>();
        private Dictionary<Animation, AnimationTimelineUI> m_animationDrawerMap = new Dictionary<Animation, AnimationTimelineUI>();

        protected override void OnDraw(UIElement host, Storyboard element)
        {
            var anis = element.GetChildren<Animation>();
            using (new EditorGUILayout.VerticalScope())
            {
                if (!m_scaleMap.TryGetValue(element, out var scale)) scale = 60f;
                m_scaleMap[element] = EditorGUILayout.Slider(scale, 60f, 500);

                DrawTimeControl(element, anis);

                foreach (var ani in anis)
                {
                    EditorGUILayout.LabelField(string.Empty, EditorStyles.helpBox, GUILayout.Height(125));
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

        private void DrawTimeControl(Storyboard sb, IReadOnlyList<Animation> anis)
        {
            if (!m_timeControls.ContainsKey(sb)) m_timeControls[sb] = new TimeControl();
            if (!m_timeSample.ContainsKey(sb)) m_timeSample[sb] = 0f;

            var sample = m_timeSample[sb];
            var tc = m_timeControls[sb];
            tc.startTime = 0;
            tc.stopTime = anis.Max(a => a.Duration);

            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(28));
            var totalTc = GUILayoutUtility.GetLastRect();
            totalTc.xMin += 4;
            totalTc.xMax -= 4;
            totalTc.yMin += 4;
            totalTc.yMax -= 4;
            tc.DoTimeControl(totalTc);
            if (Event.current.type == EventType.Repaint)
            {
                tc.Update();
            }

            if (tc.currentTime != m_timeSample[sb])
            {
                m_timeSample[sb] = tc.currentTime;
                foreach (var ani in anis)
                {
                    m_animationDrawerMap.TryGetValue(ani, out AnimationTimelineUI subTimeUI);
                    if (subTimeUI == null) return;

                    subTimeUI.SetTimeControlTime(tc.currentTime);
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
                    EditorGUI.LabelField(targetValueRect, targetbind.BindingCode);
            }
            else
            {
                var options = animation.Document.GetAllElements().Where(el => !string.IsNullOrWhiteSpace(el.Name)).Select(el => el.Name).ToList();
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
                        EditorGUI.LabelField(propertyValueRect, propertyBind.BindingCode);
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
                        EditorGUI.LabelField(offsetValueRect, offsetBind.BindingCode);
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
                animation.Curve = EditorGUI.CurveField(curveFieldRect, "Curve", new AnimationCurve(animation.Curve.keys));
                EditorGUIUtility.labelWidth = temp1;
            }

            if (GUI.changed)
            {
                Designer.SaveToAml();
            }
        }

        protected override void OnDragKey(int keyIndex, float time)
        {
            var keys = m_dataContext.GetChildren<AnimationKey>();
            var key = keys[keyIndex];
            key.Time = time;

            Designer.SaveToAml();
        }

        protected override List<float> GetKeyTime(Animation target)
        {
            return target.GetChildren<AnimationKey>().Select(key => key.Time + target.Offset).ToList();
        }

        protected override void OnKeyTipDraw(Vector2 position, int hoverKeyIndex)
        {
            var keys = m_dataContext.GetChildren<AnimationKey>();
            var key = keys[hoverKeyIndex];

            var content = new GUIContent($"time:{key.Time + m_dataContext.Offset}");
            var timeRect = new Rect(position, default);
            timeRect.width = content.text.Length * 7 + 10;
            timeRect.height = 18f;

            m_dataContext.PrepareDatas();
            var keyValue = $"value:{key.GetActualValue(m_dataContext.m_resolver).ToString()}";
            var valueRect = new Rect(timeRect);
            valueRect.width = keyValue.Length * 7 + 10;
            valueRect.height = 18f;
            valueRect.y += timeRect.height;

            var tipPosition = new Rect(position, default);
            tipPosition.width = Mathf.Max(timeRect.width, valueRect.width);
            tipPosition.height = timeRect.height + valueRect.height;

            using (new AlienEditorUtility.GUIColorScope(Color.black)) GUI.DrawTexture(tipPosition, Texture2D.whiteTexture);

            GUI.Label(timeRect, content);
            GUI.Label(valueRect, keyValue);
        }

        protected override void OnTimeChange(float time)
        {
            if (m_dataContext.m_defaultValue == null)
                m_dataContext.StageDefaultValue();

            if (m_dataContext.Evalution(time, out object value))
            {
                var target = m_dataContext.Target.Get(m_dataContext);
                target.SetValue(m_dataContext.PropertyName, value);
            }
        }

        protected override void OnKeyContextClicked(int keyIndex)
        {
            Vector2 mousePos = Event.current.mousePosition;
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("EditValue"), false, () =>
            {
                var keys = m_dataContext.GetChildren<AnimationKey>();
                var key = keys[keyIndex];
                var target = m_dataContext.Target.Get(m_dataContext);
                var properties = new List<DependencyProperty>();
                properties.AddRange(target.GetAllDependencyProperties());
                properties.AddRange(target.GetAttachedProperties());
                var targetProp = properties.FirstOrDefault(t => t.PropName == m_dataContext.PropertyName);
                if (targetProp == null) return;
                var drawer = Designer.FindDrawer(targetProp.PropType);
                if (drawer == null) return;
                var resolver = Settings.Get().m_collector.GetAttributeResolver(targetProp.PropType);
                if (resolver == null) return;

                FieldInputWinodw.ShowWindow("Edit Key", mousePos, () =>
                {
                    EditorGUI.BeginChangeCheck();
                    key.Time = EditorGUILayout.FloatField("Time", key.Time);

                    var resolvValue = resolver.Resolve(key.Value, targetProp.PropType);
                    var newValue = drawer.Draw(key, "Value", resolvValue);
                    var rawStr = resolver.ToOriString(newValue);
                    key.Value = rawStr;

                    if (EditorGUI.EndChangeCheck())
                        Designer.SaveToAml();

                    return 80;
                });
            });            
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Delelet"), false, () =>
            {
                var keys = m_dataContext.GetChildren<AnimationKey>();
                var key = keys[keyIndex];
                m_dataContext.RemoveChild(key);
                Designer.SaveToAml();
            });

            menu.ShowAsContext();
        }
    }
}

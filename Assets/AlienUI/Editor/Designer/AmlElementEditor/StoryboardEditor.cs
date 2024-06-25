using AlienUI.Core.Resources;
using AlienUI.UIElements;
using UnityEditor;

namespace AlienUI.Editors
{
    public class StoryboardEditor : ElementExtraEdit<Storyboard>
    {
        protected override void OnDraw(UIElement host, Storyboard element)
        {
            var anis = element.GetChildren<Core.Resources.Animation>();
            using (new EditorGUILayout.VerticalScope())
            {
                foreach (var ani in anis)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("these should be a AnimationEditor :(");
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

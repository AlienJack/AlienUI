using UnityEngine;

namespace AlienUI.Core
{
    public class UICoroutine
    {
        public Coroutine m_runtimeCoroutine;
        public UICoroutine(Coroutine runtimeCoroutine)
        {
            m_runtimeCoroutine = runtimeCoroutine;
        }

#if UNITY_EDITOR
        public Unity.EditorCoroutines.Editor.EditorCoroutine m_editorCoroutine;
        public UICoroutine(Unity.EditorCoroutines.Editor.EditorCoroutine editorCoroutine)
        {
            m_editorCoroutine = editorCoroutine;
        }
#endif
    }
}

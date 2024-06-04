using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlienUI
{
    [ExecuteInEditMode]
    public class NodeProxy : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler
    {
        public UIElement TargetObject { get; set; }
        internal float Alpha
        {
            get => m_canvasRenderer.alpha;
            set => m_canvasRenderer.alpha = value;
        }

        internal Color Color
        {
            get => m_graphElement.color;
            set => m_graphElement.color = value;
        }

        private Graphic m_graphElement;
        private CanvasGroup m_canvasRenderer;

        private void Awake()
        {
            m_graphElement = GetComponent<Graphic>();
            if (m_graphElement == null) m_graphElement = gameObject.AddComponent<RaycastLit>();
            m_canvasRenderer = gameObject.AddMissingComponemt<CanvasGroup>();
            SetRaycast(true);
        }

        public Canvas Canvas => m_graphElement.canvas;

        private void Start()
        {
            if (TargetObject == null) return;
        }

        internal void SetRaycast(bool value)
        {
            m_graphElement.raycastTarget = value;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TargetObject?.RaisePointerEnterEvent(TargetObject, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TargetObject?.RaisePointerExitEvent(TargetObject, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TargetObject?.RaisePointerDownEvent(TargetObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TargetObject?.RaisePointerUpEvent(TargetObject, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TargetObject?.RaisePointerClickEvent(TargetObject, eventData);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            TargetObject?.RaiseInitializePotentialDragEvent(TargetObject, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            TargetObject?.RaiseBeginDrag(TargetObject, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            TargetObject?.RaiseDrag(TargetObject, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            TargetObject?.RaiseEndDrag(TargetObject, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            TargetObject?.RaiseDrop(TargetObject, eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            TargetObject?.RaiseScroll(TargetObject, eventData);
        }
    }
}

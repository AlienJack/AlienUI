using AlienUI.Events;
using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlienUI
{
    internal class NodeProxy : MonoBehaviour,
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
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        public UIElement TargetObject { get; set; }

        private Graphic m_graphElement;
        private void Awake()
        {
            m_graphElement = GetComponent<Graphic>();
            if (m_graphElement == null)
            {
                m_graphElement = gameObject.AddComponent<RaycastLit>();
            }
            SetRaycast(true);
        }

        private void SetRaycast(bool value)
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

        public void OnUpdateSelected(BaseEventData eventData)
        {
            TargetObject?.RaiseUpdateSelected(TargetObject, eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            TargetObject?.RaiseSelected(TargetObject, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            TargetObject?.RaiseDeselect(TargetObject, eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            TargetObject?.RaiseMove(TargetObject, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            TargetObject?.RaiseSubmit(TargetObject, eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            TargetObject?.RaiseCancel(TargetObject, eventData);
        }
    }
}

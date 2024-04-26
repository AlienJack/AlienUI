using AlienUI.Events;
using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlienUI
{
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
            TargetObject?.RaiseMouseEnterEvent(TargetObject, new OnMouseEnterEvent(eventData));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TargetObject?.RaiseMouseExitEvent(TargetObject, new OnMouseExitEvent(eventData));
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {

        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {

        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnDrop(PointerEventData eventData)
        {

        }

        public void OnScroll(PointerEventData eventData)
        {

        }

        public void OnUpdateSelected(BaseEventData eventData)
        {

        }

        public void OnSelect(BaseEventData eventData)
        {

        }

        public void OnDeselect(BaseEventData eventData)
        {

        }

        public void OnMove(AxisEventData eventData)
        {

        }

        public void OnSubmit(BaseEventData eventData)
        {

        }

        public void OnCancel(BaseEventData eventData)
        {

        }
    }
}

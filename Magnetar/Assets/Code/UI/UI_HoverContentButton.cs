using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magnetar
{
    public class UI_HoverContentButton : Button
    {
        [field: SerializeField] public UI_HoverContentContainer HoverContentContainer { get; private set; }
        [field: SerializeField] public int HoverContentIndex { get; private set; } = 1;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            HoverContentContainer.OnHover(gameObject, HoverContentIndex);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            HoverContentContainer.OnUnhover(gameObject);
        }
    }
}

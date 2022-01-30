using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magnetar
{
    public class UI_HoverButton : Button
    {
        [field: SerializeField] public AudioClip OnHoverSound { get; private set; }
        [field: SerializeField] public AudioClip OnClickSound { get; private set; }

        [field: SerializeField] public UI_HoverContentContainer HoverContentContainer { get; private set; }
        [field: SerializeField] public int HoverContentIndex { get; private set; } = 1;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (HoverContentContainer != null)
            {
                HoverContentContainer.OnHover(gameObject, HoverContentIndex);
            }

            if (OnHoverSound != null)
            {
                float oldTimescale = Time.timeScale;
                Time.timeScale = 1.0f;
                UISoundPlayer.Instance.AudioSource.PlayOneShot(OnHoverSound);
                Time.timeScale = oldTimescale;
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (HoverContentContainer != null)
            {
                HoverContentContainer.OnUnhover(gameObject);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (OnClickSound != null)
            {
                float oldTimescale = Time.timeScale;
                Time.timeScale = 1.0f;
                UISoundPlayer.Instance.AudioSource.PlayOneShot(OnClickSound);
                Time.timeScale = oldTimescale;
            }

            base.OnPointerClick(eventData);
        }
    }
}

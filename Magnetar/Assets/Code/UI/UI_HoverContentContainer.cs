using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class UI_HoverContentContainer : MonoBehaviour
    {
        public List<RectTransform> hoverContent = new List<RectTransform>();

        private GameObject lastHoverer;

        private void Awake()
        {
            OnHover(null, 0);
        }

        public void OnHover(GameObject hoverer, int index)
        {
            for(int i = 0; i < hoverContent.Count; ++i)
            {
                hoverContent[i].gameObject.SetActive(i == index);
            }

            lastHoverer = hoverer;
        }

        public void OnUnhover(GameObject hoverer)
        {
            if(lastHoverer == hoverer)
            {
                OnHover(null, 0);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magnetar
{
    public class UI_MagnetState : MonoBehaviour
    {
        [field: SerializeField] public Image NegativeBar { get; private set; }
        [field: SerializeField] public Image PositiveBar { get; private set; }

        public Magnet AttachedMagnetComponent { get; private set; }

        float magneticBias = 0.0f;

        public void Setup(Magnet magnetComponent)
        {
            AttachedMagnetComponent = magnetComponent;
        }

        private void Update()
        {
            magneticBias = Mathf.Clamp(Mathf.Lerp(magneticBias, AttachedMagnetComponent.magnetismStrength >= 0 ? 1.0f : -1.0f, 0.1f), -1.0f, 1.0f);

            Vector2 negativeAnchor = NegativeBar.rectTransform.anchorMin;
            Vector2 positiveAnchor = PositiveBar.rectTransform.anchorMax;

            if(magneticBias < 0.0f)
            {
                positiveAnchor.x = 0.5f;
                negativeAnchor.x = Mathf.Lerp(0.0f, 0.5f, magneticBias + 1.0f);
            }
            else
            {
                negativeAnchor.x = 0.5f;
                positiveAnchor.x = Mathf.Lerp(0.5f, 1.0f, magneticBias);
            }

            NegativeBar.rectTransform.anchorMin = negativeAnchor;
            PositiveBar.rectTransform.anchorMax = positiveAnchor;
        }
    }
}

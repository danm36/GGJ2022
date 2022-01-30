using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Magnetar
{
    public class UI_HealthBar : MonoBehaviour
    {
        [field: SerializeField] public Image InnerBar { get; private set; }
        public HealthComponent AttachedHealthComponent { get; private set; }

        float hurtEffectTimer;

        // Update is called once per frame
        void Update()
        {
            if (AttachedHealthComponent == null)
            {
                return;
            }

            float healthPercentage = AttachedHealthComponent.CurrentHealth / AttachedHealthComponent.MaxHealth;
            Color barColour = Color.green;
            if(healthPercentage < 0.15f)
            {
                barColour = Color.Lerp(Color.green, Color.red, Mathf.Sin(Time.time * 6.0f) * 0.5f + 0.5f);
            }

            if (hurtEffectTimer > 0)
            {
                hurtEffectTimer -= Time.deltaTime;

                if (hurtEffectTimer <= 0)
                {
                    hurtEffectTimer = 0;
                }

                barColour = Color.Lerp(barColour, Color.red, hurtEffectTimer);
            }

            InnerBar.color = barColour;

            Vector2 anchorMax = InnerBar.rectTransform.anchorMax;
            anchorMax.x = Mathf.Lerp(anchorMax.x, healthPercentage, 0.1f);
            InnerBar.rectTransform.anchorMax = anchorMax;
        }

        public void Setup(HealthComponent healthComp)
        {
            AttachedHealthComponent = healthComp;
            AttachedHealthComponent.OnHurt += OnHurt;
        }

        private void OnHurt(Collision collision, float hurtAmount, float currentHealth)
        {
            hurtEffectTimer = 1.0f;
        }
    }
}

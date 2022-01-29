using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public delegate void OnHurt(Collision collision, float hurtAmount, float currentHealth);
    public delegate void OnDeath(Collision collision);

    public class HealthComponent : MonoBehaviour
    {
        [field: SerializeField] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField] public float CurrentHealth { get; private set; } = -1;

        public event OnHurt OnHurt;
        public event OnDeath OnDeath;

        // Start is called before the first frame update
        void Start()
        {
            if(CurrentHealth == -1)
            {
                CurrentHealth = MaxHealth;
            }
        }

        public void Hurt(Collision collision, float hurtAmount)
        {
            CurrentHealth -= hurtAmount;
            if(CurrentHealth <= 0)
            {
                OnDeath?.Invoke(collision);
            }
            else
            {
                OnHurt?.Invoke(collision, hurtAmount, CurrentHealth);
            }
        }
    }
}

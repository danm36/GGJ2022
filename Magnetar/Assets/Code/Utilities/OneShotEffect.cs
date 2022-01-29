using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class OneShotEffect : MonoBehaviour
    {
        [field: SerializeField] public float LifeTime { get; private set; } = 1.0f;
        private float elapsed;

        // Update is called once per frame
        void Update()
        {
            elapsed += Time.deltaTime;
            if(elapsed > LifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}

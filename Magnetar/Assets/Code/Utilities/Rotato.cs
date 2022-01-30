using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    /// <summary>
    /// Randomly rotates whatever it's attached to.
    /// </summary>
    public class Rotato : MonoBehaviour
    {
        [field: SerializeField] public float MinRotationSpeed { get; private set; } = 10.0f;
        [field: SerializeField] public float MaxRotationSpeed { get; private set; } = 20.0f;

        Vector3 rotationVector;

        private void Awake()
        {
            rotationVector = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * Random.Range(MinRotationSpeed, MaxRotationSpeed);
        }

        void Update()
        {
            transform.Rotate(rotationVector * Time.deltaTime);
        }
    }
}

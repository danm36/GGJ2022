using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Magnetar
{
    public enum EMagnetType
    {
        /// <summary>Positive strength values = Positive field, Negative strength = Negative field.</summary>
        DirectedMagnet = 0,
        /// <summary>Positive strength values = Attraction to anything magnetic, Negative strength values = Repulsion.</summary>
        Monopole = 1,
    }

    public class Magnet : MonoBehaviour
    {
        public const float MAX_VELOCITY_MAGNITUDE = 100.0f;

        public static List<Magnet> AllMagnets = new List<Magnet>();

        public EMagnetType magnetismType = EMagnetType.DirectedMagnet;
        public float magnetismStrength = 0.0f;
        public bool isStatic = false;

        public Vector3 DesiredWorldVelocity { get; private set; }

        private void OnEnable()
        {
            AllMagnets.Add(this);
        }

        private void OnDisable()
        {
            AllMagnets.Remove(this);
        }

        private void FixedUpdate()
        {
            if(isStatic)
            {
                return;
            }

            Vector3 desiredVector = Vector3.zero;

            foreach(Magnet other in AllMagnets)
            {
                if(other == this)
                {
                    continue;
                }

                Vector3 vecToOther = other.transform.position - transform.position;

                // Positive value = attraction, negative value = repulsion
                float othersForceOnMe = 1.0f / (vecToOther.sqrMagnitude + 0.01f);
                if (magnetismType == EMagnetType.Monopole || other.magnetismType == EMagnetType.Monopole)
                {
                    othersForceOnMe *= magnetismStrength + other.magnetismStrength;
                }
                else if (magnetismType == other.magnetismType)
                {
                    othersForceOnMe *= magnetismStrength * -other.magnetismStrength;
                }

                desiredVector += vecToOther.normalized * othersForceOnMe;
            }

            DesiredWorldVelocity = desiredVector.normalized * Mathf.Min(desiredVector.magnitude, MAX_VELOCITY_MAGNITUDE);
        }
    }
}

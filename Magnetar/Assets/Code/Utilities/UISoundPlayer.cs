using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    [RequireComponent(typeof(AudioSource))]
    public class UISoundPlayer : MonoBehaviour
    {
        public static UISoundPlayer Instance { get; private set; }
        public AudioSource AudioSource { get; private set; }

        private void Awake()
        {
            Instance = this;
            AudioSource = GetComponent<AudioSource>();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class UI_PlayerStatsContainer : MonoBehaviour
    {
        [field: SerializeField] public UI_HealthBar HealthBar { get; private set; }
        [field: SerializeField] public UI_MagnetState MagnetState { get; private set; }

        public void Setup(PlayerController player)
        {
            HealthBar.Setup(player.HealthComponent);
            MagnetState.Setup(player.MagnetComponent);
        }
    }
}

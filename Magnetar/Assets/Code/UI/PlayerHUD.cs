using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magnetar
{
    public class PlayerHUD : MonoBehaviour
    {
        [field: SerializeField] public UI_PlayerStatsContainer Player1StatsContainer { get; private set; }
        [field: SerializeField] public UI_PlayerStatsContainer Player2StatsContainer { get; private set; }

        [field: SerializeField] public TextMeshProUGUI GetReadyText { get; private set; }

        private void Awake()
        {
            GetReadyText.CrossFadeAlpha(0.0f, 0.0f, true);
        }

        public void Initialize(PlayerController player1, PlayerController player2 = null)
        {
            Player1StatsContainer.Setup(player1);

            if (player2 != null)
            {
                Player2StatsContainer.Setup(player2);
            }
            else
            {
                Player2StatsContainer.gameObject.SetActive(false);
            }

            GetReadyText.CrossFadeAlpha(1.0f, 1.0f, true);
        }

        public void BeginGameplay()
        {
            GetReadyText.CrossFadeAlpha(0.0f, 0.5f, true);
        }
    }
}

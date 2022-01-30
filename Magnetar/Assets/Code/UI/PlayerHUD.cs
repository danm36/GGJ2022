using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Magnetar
{
    public class PlayerHUD : MonoBehaviour
    {
        public bool IsPaused { get; private set; }

        [field: SerializeField] public UI_PlayerStatsContainer Player1StatsContainer { get; private set; }
        [field: SerializeField] public UI_PlayerStatsContainer Player2StatsContainer { get; private set; }

        [field: SerializeField] public CanvasGroup PauseMenuContainer { get; private set; }
        [field: SerializeField] public CanvasGroup GameOverContainer { get; private set; }
        [field: SerializeField] public CanvasGroup MissionCompleteContainer { get; private set; }

        [field: SerializeField] public TextMeshProUGUI GetReadyText { get; private set; }

        private void Awake()
        {
            GetReadyText.CrossFadeAlpha(0.0f, 0.0f, true);
            PauseMenuContainer.gameObject.SetActive(false);
            GameOverContainer.gameObject.SetActive(false);
            MissionCompleteContainer.gameObject.SetActive(false);
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

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            PauseMenuContainer.gameObject.SetActive(IsPaused);
            Time.timeScale = IsPaused ? 0.0f : 1.0f;
        }

        public void ShowGameOver()
        {
            StartCoroutine(DoShowGameOver());
        }

        private IEnumerator DoShowGameOver()
        {
            GameOverContainer.gameObject.SetActive(true);
            GameOverContainer.alpha = 0.0f;

            for(float i= 0; i < 1.0f; i += Time.unscaledDeltaTime)
            {
                Time.timeScale = 1.0f - i;
                GameOverContainer.alpha = Mathf.SmoothStep(0.0f, 1.0f, i);
                yield return null;
            }

            Time.timeScale = 0.0f;
            GameOverContainer.alpha = 1.0f;
        }

        public void ShowMissionComplete()
        {
            StartCoroutine(DoShowMissionComplete());
        }

        private IEnumerator DoShowMissionComplete()
        {
            MissionCompleteContainer.gameObject.SetActive(true);
            MissionCompleteContainer.alpha = 0.0f;

            for (float i = 0; i < 1.0f; i += Time.unscaledDeltaTime)
            {
                Time.timeScale = 1.0f - i;
                MissionCompleteContainer.alpha = Mathf.SmoothStep(0.0f, 1.0f, i);
                yield return null;
            }

            Time.timeScale = 0.0f;
            MissionCompleteContainer.alpha = 1.0f;
        }

        public void RestartLevel()
        {
            Time.timeScale = 1.0f;
            GamePersistance.Instance.GoToGameScene(PlayableZoneController.Instance.IsTwoShipMode);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1.0f;
            GamePersistance.Instance.GoToMainMenu();
        }
    }
}

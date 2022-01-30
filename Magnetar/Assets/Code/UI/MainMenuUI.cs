using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartSingleShipMode()
        {
            GamePersistance.Instance.GoToGameScene(false);
        }

        public void StartDualShipMode()
        {
            GamePersistance.Instance.GoToGameScene(true);
        }

        public void Quit()
        {
            Debug.Log("Main Menu Quit Button Clicked");
            Application.Quit();
        }
    }
}

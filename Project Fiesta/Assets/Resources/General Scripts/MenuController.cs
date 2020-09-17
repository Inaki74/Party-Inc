using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

namespace FiestaTime
{
    /// <summary>
    /// The personal pause menu where you can choose to go back to main menu (dc). Its also the game over screen for some games.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;

        [SerializeField] private Button menuButton;

        private bool menuOpen = false;

        public void BtnOpenMenu()
        {
            Debug.Log("AAAAA");
            if (!menuOpen)
            {
                // Open menu
                pauseMenu.SetActive(true);
                menuOpen = true;
            }
            else
            {
                // Close menu
                pauseMenu.SetActive(false);
                menuOpen = false;
            }
        }

        public void BtnQuitToMenu()
        {
            PhotonNetwork.LeaveRoom();

            SceneManager.LoadScene(1);
        }
    }
}

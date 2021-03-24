using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

namespace PartyInc
{
    /// <summary>
    /// The personal pause menu where you can choose to go back to main menu (dc). Its also the game over screen for some games.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject resetButton;

        [SerializeField] private Button menuButton;

        [SerializeField] private Text _techInfo;

        private float _startTime;
        private int _startFrames;
        private int _totalFrames;
        private float _timeElapsed;
        

        private bool menuOpen = false;

        private void Start()
        {
            _startTime = Time.time;
            _startFrames = Time.frameCount;
        }

        private void Update()
        {
            _techInfo.text = "FPS: " + (Time.frameCount - _startFrames) / (Time.time - _startTime);

        }

        public void BtnOpenMenu()
        {
            if (!menuOpen)
            {
                // Open menu
                pauseMenu.SetActive(true);
                menuOpen = true;
                if (PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length > 1) resetButton.SetActive(false);
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

            SceneManager.LoadScene(Stt_SceneIndexes.HUB);
        }

        public void BtnResetGame()
        {
            int reload = SceneManager.GetActiveScene().buildIndex;

            Debug.Log(reload);

            SceneManager.LoadScene(reload);
        }
    }
}

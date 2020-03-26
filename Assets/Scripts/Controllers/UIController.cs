using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class UIController : MonoBehaviour
    {
        public Button retryButton;
        public Button pauseButton;
        public Button skipButton;
        public Button undoButton;
        public Button turnMainButton;

        public Text levelInformation;

        void Awake()
        {
            InitProperties();
        }

        private void InitProperties()
        {
            InitEvents();
        }

        private void InitEvents()
        {
            Managers.EventManager.StartListening(Managers.EventManager.Listener.NextGeneretePuzzle.ToString(), NextGeneretePuzzle);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.StartGame.ToString(), StartGame);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.GameOver.ToString(), GameOver);
            retryButton.onClick.AddListener(() => Retry());
            turnMainButton.onClick.AddListener(() => TurnMain());
            pauseButton.onClick.AddListener(() => PauseGame());
            skipButton.onClick.AddListener(() => Skip());
            undoButton.onClick.AddListener(() => Undo());
            turnMainButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
        }

        void TurnMain()
        {
            turnMainButton.gameObject.SetActive(false);
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.TurnMain.ToString());
        }

        void StartGame(System.Object levelCount)
        {
            //start configuration
        }

        void UpdateLevelCount(System.Object levelCount)
        {
            levelInformation.text = "LEVEL " + levelCount;
        }

        void GameOver(System.Object arg = null)
        {
            turnMainButton.gameObject.SetActive(true);
            //game over configuration
        }

        void NextGeneretePuzzle(System.Object levelCount)
        {
            UpdateLevelCount(levelCount);
        }

        void Skip()
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.Skip.ToString());
        }

        void Undo()
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.Undo.ToString());
        }

        void PauseGame()
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.PauseGame.ToString());
        }

        void Retry()
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.RestartGame.ToString());
        }
    }
}

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

        public Text levelInformation;

        void Awake()
        {
            InitProperties();
        }

        private void InitProperties()
        {
          //  levelInformation.text = "Level " + _levelCount; 
            InitEvents();
        }

        private void InitEvents()
        {
            Managers.EventManager.StartListening(Managers.EventManager.Listener.NextGeneretePuzzle.ToString(), NextGeneretePuzzle);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.StartGame.ToString(), StartGame);
            retryButton.onClick.AddListener(() => Retry());
            pauseButton.onClick.AddListener(() => PauseGame());
            skipButton.onClick.AddListener(() => Skip());
            undoButton.onClick.AddListener(() => Undo());
        }

        void StartGame(System.Object levelCount)
        {
            //start configuration
        }

        void UpdateLevelCount(System.Object levelCount)
        {
            levelInformation.text = "LEVEL " + levelCount;
        }

        void GameOver(System.Object success)
        {

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

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
        public Button okButton;
        public Text informationText;
        public GameObject infoPopupContainer;

        public Text levelInformation;

        public enum Information
        {
            LevelsFinished,
            LevelSuccess,
            NoMoreMoves,
            NoMoreUndo
        }

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
            Managers.EventManager.StartListening(Managers.EventManager.Listener.SendMessage.ToString(), SendMessage);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.GameOver.ToString(), GameOver);
            retryButton.onClick.AddListener(() => Retry());
            okButton.onClick.AddListener(() => PopupContainerVisible(false));
            turnMainButton.onClick.AddListener(() => TurnMain());
            pauseButton.onClick.AddListener(() => PauseGame());
            skipButton.onClick.AddListener(() => Skip());
            undoButton.onClick.AddListener(() => Undo());
            turnMainButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            infoPopupContainer.SetActive(false);
        }

        void PopupContainerVisible(bool active)
        {
            infoPopupContainer.SetActive(active);
        }

        void SendMessage(System.Object message = null)
        {
            Information info = (Information)message;
            string m = "";
            switch (info)
            {
                case Information.LevelsFinished:
                    m = "Başka bölüm yok bitti";
                    break;
                case Information.LevelSuccess:
                    m = "Bir sonraki bölüme geçtin";
                    break;
                case Information.NoMoreMoves:
                    m = "ekmek sandwich oldu,hamle kalmadı";
                    break;
                case Information.NoMoreUndo:
                    m = "Undo yapacak hamle yok";
                    break;
            }
            informationText.text = m;
            PopupContainerVisible(true);
            //belki buraya animasyon vs eklenir.
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

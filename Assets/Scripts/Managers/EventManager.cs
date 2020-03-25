using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        //listener for game actions
        public enum Listener
        {
            StartGame,
            RestartGame,
            Undo,
            Skip,
            PauseGame,
            NextGeneretePuzzle,
            CheckNextMove,
            MoveDone,
            MoveStart
        }

        [System.Serializable]
        public class Event : UnityEvent<System.Object> { }

        private Dictionary<string, Event> _eventDictionary;
        private static EventManager eventManager;

        public static EventManager Instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                    if (eventManager)
                    {
                        eventManager.Init();
                    }
                }

                return eventManager;
            }
        }

        /// <summary>
        /// init properties
        /// </summary>
        void Init()
        {
            if (_eventDictionary == null)
            {
                _eventDictionary = new Dictionary<string, Event>();
            }
        }


        /// <summary>
        /// start listening according to listener type
        /// </summary>
        /// <param name="eventName">Listener type</param>
        /// <param name="listener">listener method</param>
        public static void StartListening(string eventName, UnityAction<System.Object> listener)
        {
            Event thisEvent = null;
            if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new Event();
                thisEvent.AddListener(listener);
                Instance._eventDictionary.Add(eventName, thisEvent);
            }
        }

        /// <summary>
        /// stop listening according to listener type
        /// </summary>
        /// <param name="eventName">Listener type</param>
        /// <param name="listener">listener method</param>
        public static void StopListening(string eventName, UnityAction<System.Object> listener)
        {
            if (Instance == null) return;
            Event thisEvent = null;
            if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        /// <summary>
        /// trigger event according to listener type
        /// </summary>
        /// <param name="eventName">Listener type</param>
        /// /// <param name="eventParam">from listener method's params</param>
        public static void TriggerEvent(string eventName, System.Object eventParam = null)
        {
            Event thisEvent = null;
            if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject eventSystem;

        private int _levelCount = 0;
        private List<FoodController> _foods;

        private Dictionary<Vector2, FoodController> _gridElements = new Dictionary<Vector2, FoodController>();
        private List<FoodController> _moveList;
        private List<FoodController> _bread;

        private bool _success;
        private bool _startGame;
        private bool _freezeGame;

        void Start()
        {
            InitProperties();
        }

        private void InitProperties()
        {
            _startGame = false;
            _freezeGame = false;
            StartGame();
            InitEvents();
        }

        private void InitEvents()
        {
            Managers.EventManager.StartListening(Managers.EventManager.Listener.Skip.ToString(), Skip);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.TurnMain.ToString(), TurnMain);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.MoveStart.ToString(), MoveStart);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.MoveDone.ToString(), MoveDone);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.Undo.ToString(), Undo);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.RestartGame.ToString(), RestartGame);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.CheckNextMove.ToString(), CheckNextMove);
        }

        void TurnMain(System.Object arg = null)
        {
            _levelCount = 0;
            StartGame();
        }

        void StartGame()
        {
            _startGame = true;
            GenereteNextPuzzle();
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.StartGame.ToString(), _levelCount);
        }

        private void Undo(System.Object arg = null)
        {
            if (_moveList.Count >= 1)
            {
                BlockGame(true);
                FoodController fC = _moveList[_moveList.Count - 1];
                fC.UndoMove();
                _freezeGame = false;
                Vector2 sP = fC.StartMatrix;
                _gridElements[sP].sandwiched = false;
                _gridElements[sP].CurrentMatrix = sP;
                _moveList.Remove(fC);
                CalculateNeighborhoods();
            }
            else
            {
                _startGame = true;
                _freezeGame = false;
                Debug.Log("Undo yapacak hamle yok");
            }
        }

        void BlockGame(bool value)
        {
            BlockUI(value);
            BlockElements(value);
        }

        void BlockUI(bool block)
        {
            eventSystem.SetActive(!block);
        }

        void BlockElements(bool block)
        {
            for (int i = 0; i < _foods.Count; i++)
            {
                _foods[i].gameObject.GetComponent<BoxCollider>().enabled = !block;
            }
        }

        void Skip(System.Object arg)
        {
            DestroyPreviousElements();
            GenereteNextPuzzle();
        }

        void RestartGame(System.Object arg = null)
        {
            _startGame = false;
            Undo();
        }

        private void MoveDone(System.Object arg = null)
        {
            if(!_freezeGame)
                BlockGame(false);
            if (_startGame == false)
            {
                RestartGame();
            }
        }

        private void MoveStart(System.Object arg = null)
        {
            BlockGame(true);
        }

        void GenereteNextPuzzle()
        {
            _moveList = new List<FoodController>();
            DestroyPreviousElements();
            CreateFoods();
            CalculateNeighborhoods();
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.NextGeneretePuzzle.ToString(), _levelCount);
        }

        void CreateFoods()
        {
            _gridElements = new Dictionary<Vector2, FoodController>();
            _bread = new List<FoodController>();
            _foods = new List<FoodController>();

            if (_levelCount < FakeMapConfig.mapCOnfig.Length)
            {
                IMapConfig mC = FakeMapConfig.mapCOnfig[_levelCount];

                for (int i = 0; i < mC.foodConfigs.Length; i++)
                {
                    GameObject food = Resources.Load<GameObject>("Prefabs/Food");
                    GameObject currentEntity = Instantiate(food, new Vector3(0, 0, 0), Quaternion.identity);
                    currentEntity.GetComponent<FoodController>().ChangeFoodType(mC.foodConfigs[i].name, mC.foodConfigs[i].startMatrix, mC.row, mC.coloumn);
                    FoodController fC = currentEntity.GetComponent<FoodController>();
                    fC.StartMatrix = mC.foodConfigs[i].startMatrix;
                    _gridElements[mC.foodConfigs[i].startMatrix] = fC;
                    if (fC.food.foodName == Food.FoodNames.Bread)
                    {
                        _bread.Add(fC);
                    }
                    _foods.Add(fC);
                }
                _levelCount++;
            }
            else
            {
                BlockElements(true);
                BlockUI(false);
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.GameOver.ToString());
                Debug.Log("Başka bölüm yok bitti");
            }
        }

        private void DestroyPreviousElements()
        {
            if (_foods != null)
            {
                for (int i = 0; i < _foods.Count; i++)
                {
                    FoodController fC = _foods[i];
                    _gridElements.Remove(fC.StartMatrix);
                    Destroy(fC.gameObject);
                }
            }
        }

        private void CheckNextMove(System.Object foodController)
        {
            FoodController fC = (FoodController)foodController;
            BlockGame(false);
            Vector2 sP = fC.StartMatrix;
            _moveList.Add(fC);
            _gridElements[sP].sandwiched = true;

            bool closedSandwich = IsClosedSandwich();
            if (closedSandwich)
            {
                bool isFinished = IsFinishedSandwich();
                if (isFinished)
                {
                    Debug.Log("Success");
                    GenereteNextPuzzle();
                    return;
                }
                _freezeGame = true;
                BlockUI(false);
                BlockElements(true);
                Debug.Log("ekmek sandwich oldu,hamle kalmadı");
                return;
            }

            CalculateNeighborhoods();
        }

        private bool IsFinishedSandwich()
        {
            int sandwich = 0;
            for(int i = 0; i<_foods.Count; i++)
            {
                if (_foods[i].sandwiched == false) 
                    sandwich++;
                if (sandwich > 1)
                    return false;//salt bi tane sandwich olmamış nesne kalması lazım
            }
            return true;
        }

        private bool IsClosedSandwich()
        {
            for(int i = 0; i<_bread.Count; i++)
            {
                if (_bread[i].sandwiched)
                    return true;
            }
            return false;
        }

        private void CalculateNeighborhoods()
        {
            for (int i = 0; i < _foods.Count; i++)
            {
                Vector2 sP = _foods[i].CurrentMatrix;
                _foods[i].Neighborhoods = new Dictionary<FoodController.Direction, FoodController>();
                if (_gridElements.ContainsKey(new Vector2(sP.x - 1, sP.y)) && _gridElements[new Vector2(sP.x - 1, sP.y)].sandwiched == false)
                    _foods[i].Neighborhoods.Add(FoodController.Direction.Left, _gridElements[new Vector2(sP.x - 1, sP.y)]);
                if (_gridElements.ContainsKey(new Vector2(sP.x + 1, sP.y)) && _gridElements[new Vector2(sP.x + 1, sP.y)].sandwiched == false)
                    _foods[i].Neighborhoods.Add(FoodController.Direction.Right, _gridElements[new Vector2(sP.x + 1, sP.y)]);
                if (_gridElements.ContainsKey(new Vector2(sP.x, sP.y + 1)) && _gridElements[new Vector2(sP.x, sP.y + 1)].sandwiched == false)
                    _foods[i].Neighborhoods.Add(FoodController.Direction.Down, _gridElements[new Vector2(sP.x, sP.y + 1)]);
                if (_gridElements.ContainsKey(new Vector2(sP.x, sP.y - 1)) && _gridElements[new Vector2(sP.x, sP.y - 1)].sandwiched == false)
                    _foods[i].Neighborhoods.Add(FoodController.Direction.Up, _gridElements[new Vector2(sP.x, sP.y - 1)]);
            }
        }
    }
}

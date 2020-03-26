using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers
{
    public class MoveObject
    {
        public FoodController food;
        //belki başka parametreler de eklenir,kalsın
    }

    public class GameController : MonoBehaviour
    {
        public GameObject eventSystem;

        private int _levelCount = 0;
        private List<FoodController> _foods;

        private Dictionary<Vector2, FoodController> _gridElements = new Dictionary<Vector2, FoodController>();
        private List<MoveObject> _moveList;
        private List<FoodController> _bread;

        private bool _success;
        private bool _waitGame;
        private bool _startGame;

        void Start()
        {
            InitProperties();
        }

        private void InitProperties()
        {
            _waitGame = false;
            _startGame = false;
            StartGame();
            InitEvents();
        }

        private void InitEvents()
        {
            Managers.EventManager.StartListening(Managers.EventManager.Listener.Skip.ToString(), Skip);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.MoveStart.ToString(), MoveStart);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.MoveDone.ToString(), MoveDone);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.Undo.ToString(), Undo);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.RestartGame.ToString(), RestartGame);
            Managers.EventManager.StartListening(Managers.EventManager.Listener.CheckNextMove.ToString(), CheckNextMove);
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
                MoveObject mO = _moveList[_moveList.Count - 1];
                mO.food.UndoMove();
                Vector2 sP = mO.food.StartMatrix;
                _gridElements[sP].sandwiched = false;
                _gridElements[sP].CurrentMatrix = sP;
                mO.food.CurrentMatrix = sP;
                mO.food.gameObject.SetActive(true);
                _moveList.Remove(mO);
                CalculateNeighborhoods();
            }
            else
            {
                _startGame = true;
                Debug.LogError("Undo yapacak hamle yok");
            }
        }

        void BlockGame(bool value)
        {
            _waitGame = value;
            eventSystem.SetActive(!value);
            for(int i = 0; i<_foods.Count; i++)
            {
                _foods[i].gameObject.GetComponent<BoxCollider>().enabled = !value;
            }
        }

        void Skip(System.Object arg)
        {
            DestroyPreviousElement();
            GenereteNextPuzzle();
        }

        void RestartGame(System.Object arg = null)
        {
            //some argumentler eklenebilir
            _startGame = false;
            Undo();
        }

        private void MoveDone(System.Object arg = null)
        {
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
            _moveList = new List<MoveObject>();
            DestroyPreviousElement();
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
                    currentEntity.GetComponent<FoodController>().ChangeFoodType(mC.foodConfigs[i].name, mC.foodConfigs[i].startMatrix);
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
                BlockGame(true);
                eventSystem.SetActive(true);
                Debug.LogError("Başka bölüm yok bitti");
            }
        }

        private void DestroyPreviousElement()
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

        private void CheckNextMove(System.Object moveListElement)
        {
            MoveObject mE = (MoveObject)moveListElement;
            BlockGame(false);
            Vector2 sP = mE.food.StartMatrix;
            _moveList.Add(mE);

            _gridElements[sP].sandwiched = true;

            bool closedSandwich = IsClosedSandwich();
            if (closedSandwich)
            {
                bool isFinished = IsFinishedSandwich();
                if (isFinished)
                {
                    Debug.LogError("Success");
                    GenereteNextPuzzle();
                    return;
                }
                BlockGame(true);
                eventSystem.SetActive(true);
                Debug.LogError("ekmek sandwich oldu,hamle kalmadı");
            }

            CalculateNeighborhoods();

            for (int i = 0; i < _foods.Count; i++)
            {
                bool empty = _foods[i].IsNeighborhoodsEmpty();
                if (empty)
                {
                    BlockGame(true);
                    eventSystem.SetActive(true);
                    Debug.LogError("hamle kalmadı");
                    return;
                }
            }
        }

        private bool IsFinishedSandwich()
        {
            int sandwich = 0;
            for(int i = 0; i<_foods.Count; i++)
            {
                if (_foods[i].sandwiched == false)
                    sandwich++;
                if (sandwich > 1)
                    return false;
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

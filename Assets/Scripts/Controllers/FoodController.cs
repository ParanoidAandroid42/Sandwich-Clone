using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class FoodController : MonoBehaviour
{
    public Food food;
    private Dictionary<Direction, FoodController> _neighborhoods = new Dictionary<Direction, FoodController>();
    private Vector2 _currentMatrix;
    private Vector2 _startMatrix;

    private Vector2 _moveStartPosition;
    private Vector2 _moveEndPosition;
    private Vector3 _scale = new Vector3(2, 2,.14f);

    private Direction _moveDirection;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    public bool sandwiched = false;

    public void Start()
    {
    }

    public void ChangeFoodType(Food.FoodNames name,Vector2 matrix)
    {
        Food f = Resources.Load<Food>("ScriptablesObject/Food/" + name);
        food = f;
        GetComponent<MeshRenderer>().material = food.metarial;
        gameObject.name = name.ToString();
        _startMatrix = matrix;
        _currentMatrix = _startMatrix;
        gameObject.transform.localScale = _scale;
        gameObject.transform.position = new Vector3(_scale.x * matrix.x -2, -_scale.y * matrix.y +2,0); // bu grid sistemine bağlansa iyi olur.pozisyon için
    }

    public void Move()
    {
        if (_neighborhoods.ContainsKey(_moveDirection))
        {
            FoodController nFC = _neighborhoods[_moveDirection];
            Vector3 position = _neighborhoods[_moveDirection].transform.position;
            Vector3 rotation = GetDirectionRotation(_moveDirection);

            Controllers.MoveObject mO = new Controllers.MoveObject();
            _currentMatrix = nFC.StartMatrix;
            GameObject targetObject = gameObject;
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveStart.ToString());
            float z = -_scale.z;
            while (targetObject.transform.parent != null)
            {
                targetObject = targetObject.transform.parent.gameObject;
                z -= _scale.z;
            }
            mO.food = targetObject.GetComponent<FoodController>();
            position = new Vector3(position.x, position.y, z);

            GameObject pO = nFC.gameObject;
            while (pO.transform.childCount != 0)
            {
                pO = pO.transform.GetChild(0).gameObject;
            }

            targetObject.transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd);
            targetObject.transform.DOMove(position, 1).OnComplete(() =>
            {
                targetObject.transform.parent = pO.transform;
                for (int i = 0; i < nFC.transform.childCount; i++)
                {
                    nFC.transform.GetChild(i).GetComponent<FoodController>().CurrentMatrix = nFC.StartMatrix;
                    nFC.transform.GetChild(i).GetComponent<FoodController>().sandwiched = true;
                }
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.CheckNextMove.ToString(), mO);
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
            });
        }
        else
        {
            Debug.LogError("O tarafta komşu yok");
        }
    }

    public Vector3 GetDirectionRotation(Direction moveDirection)
    {
        Vector3 rotation = new Vector3(0, 0, 0);
        switch (moveDirection)
        {
            case Direction.Left:
                rotation = new Vector3(0, 180, 0);
                break;
            case Direction.Down:
                rotation = new Vector3(-180, 0, 0);
                break;
            case Direction.Right:
                rotation = new Vector3(0, -180, 0);
                break;
            case Direction.Up:
                rotation = new Vector3(180, 0, 0);
                break;
            case Direction.None:
                break;
        }
        return rotation;
    }

    public void SetOrder()
    {

    }

    public void OnMouseUp()
    {
        _moveEndPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        bool horizontal = (Math.Abs(_moveEndPosition.x - _moveStartPosition.x) > Math.Abs(_moveEndPosition.y - _moveStartPosition.y));
        _moveDirection = Direction.None;
        if (horizontal)
        {
            if (_moveEndPosition.x > _moveStartPosition.x)
                _moveDirection = Direction.Right;
            if (_moveEndPosition.x < _moveStartPosition.x)
                _moveDirection = Direction.Left;
        }
        else
        {
            if (_moveEndPosition.y > _moveStartPosition.y)
                _moveDirection = Direction.Up;
            if (_moveEndPosition.y < _moveStartPosition.y)
                _moveDirection = Direction.Down;
        }
        Move();
    }

    public void UndoMove()
    {
        FoodController fC= gameObject.GetComponent<FoodController>();
        Vector2 startPosition = new Vector3(_scale.x * fC.StartMatrix.x - 2, -_scale.y * fC.StartMatrix.y + 2, 0);
        Vector3 rotation = GetDirectionRotation(fC.MoveDirection);
        gameObject.transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
        });
        GameObject targetObject = gameObject;

        while (targetObject.transform.parent != null)
        {
            targetObject = targetObject.transform.parent.gameObject;
        }
        gameObject.transform.parent = targetObject.transform.parent;
        gameObject.transform.DOMove(startPosition, 1);
        fC.CurrentMatrix = fC.StartMatrix;
    }

    public void OnMouseDown()
    {
        _moveStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    public bool IsNeighborhoodsEmpty()
    {
        if(_neighborhoods.Count == 0)
        {
            return true;
        }
        return false;
    }

    public Dictionary<Direction, FoodController> Neighborhoods
    {
        get { return _neighborhoods; }   // get method
        set { _neighborhoods = value; }  // set method
    }

    public Vector2 CurrentMatrix
    {
        set { _currentMatrix = value; }  // set method
        get { return _currentMatrix; }   // get method
    }

    public Direction MoveDirection
    {
        get { return _moveDirection; }   // get method
        set { _moveDirection = value; }// set method
    }

    public Vector2 StartMatrix
    {
        get { return _startMatrix; }   // get method
        set { _startMatrix = value; }// set method
    }
}

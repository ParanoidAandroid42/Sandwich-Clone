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
        gameObject.transform.position = new Vector3(_scale.x * matrix.x -2, -_scale.y * matrix.y +2,_scale.z); // bu grid sistemine bağlansa iyi olur.pozisyon için
    }

    public void Move()
    {
        Debug.LogError(gameObject);
        if (_neighborhoods.ContainsKey(_moveDirection))
        {
            FoodController nFC = _neighborhoods[_moveDirection];
            Vector3 position = _neighborhoods[_moveDirection].transform.position;
            Vector3 rotation = new Vector3(0, 0, 0);
            switch (_moveDirection)
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

            _currentMatrix = nFC.StartMatrix;
            GameObject targetObject = gameObject;
            position = new Vector3(position.x, position.y, position.z - _scale.z);
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveStart.ToString());

            targetObject = (transform.parent != null) ? transform.parent.gameObject : gameObject;
            targetObject.transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd);
            targetObject.transform.DOMove(position, 1).OnComplete(() =>
            {
                targetObject.transform.parent = nFC.transform;
                for (int i = 0; i < nFC.transform.childCount; i++)
                {
                    nFC.transform.GetChild(i).GetComponent<FoodController>().CurrentMatrix = nFC.StartMatrix;
                }
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.CheckNextMove.ToString(), this);
            });
        }
        else
        {
            Debug.LogError("O tarafta komşu yok");
        }
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
        Vector3 position = new Vector3(_scale.x * _startMatrix.x - 2, -_scale.y * _startMatrix.y + 2, _scale.z);
        Vector3 rotation = new Vector3(0, 0, 0);
        switch (_moveDirection)
        {
            case Direction.Left:
                rotation = new Vector3(0, -180, 0);
                break;
            case Direction.Down:
                rotation = new Vector3(180, 0, 0);
                break;
            case Direction.Right:
                rotation = new Vector3(0, 180, 0);
                break;
            case Direction.Up:
                rotation = new Vector3(-180, 0, 0);
                break;
            case Direction.None:
                break;
        }

        transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd).OnComplete(()=>
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
        }); // buradaki 1 sayısı dinamic olmalı
        transform.parent = transform.parent.parent;
        transform.DOMove(position, 1);
        _currentMatrix = _startMatrix;
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

    public Vector2 StartMatrix
    {
        get { return _startMatrix; }   // get method
        set { _startMatrix = value; }// set method
    }
}

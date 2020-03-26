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

    public void ChangeFoodType(Food.FoodNames name,Vector2 matrix,int row,int coloumn)
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
            int topCount = 0;
            float z = 0;
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveStart.ToString());

            Vector3 position = _neighborhoods[_moveDirection].transform.position;
            Vector3 rotation = GetDirectionRotation(_moveDirection,false);

            FoodController nFC = _neighborhoods[_moveDirection];
            _currentMatrix = nFC.StartMatrix;
            (int count, GameObject targetObject) = GetMultipleParentValue(gameObject);
            topCount += count;
            (int count2, GameObject parentObject) = GetMultipleChildValue(nFC.gameObject);
            topCount += count2;
            (int count3, GameObject childobject) = GetMultipleChildValue(gameObject);
            topCount += count3;
            
            FoodController fCList = targetObject.GetComponent<FoodController>();
            z = -((topCount +1) * _scale.z); 
            position = new Vector3(position.x, position.y, z);

            targetObject.transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd);
            targetObject.transform.DOMove(position, 1).OnComplete(() =>
            {
                targetObject.transform.parent = parentObject.transform;
                for (int i = 0; i < nFC.transform.childCount; i++)
                {
                    FoodController fC = nFC.transform.GetChild(i).GetComponent<FoodController>();
                    fC.CurrentMatrix = nFC.StartMatrix;
                    fC.sandwiched = true;
                }
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.CheckNextMove.ToString(), fCList);
                Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
            });
        }
        else
        {
            Debug.Log("O tarafta komşu yok");
        }
    }

    (int, GameObject) GetMultipleParentValue(GameObject targetObject)
    {
        int parentCount = 0;
        while (targetObject.transform.parent != null)
        {
            targetObject = targetObject.transform.parent.gameObject;
            parentCount++;
        }
        return (parentCount, targetObject);
    }

    (int, GameObject) GetMultipleChildValue(GameObject targetObject)
    {
        int childCount = 0;
        while (targetObject.transform.childCount != 0)
        {
            targetObject = targetObject.transform.GetChild(0).gameObject;
            childCount++;
        }
        return (childCount, targetObject);
    }

    public Vector3 GetDirectionRotation(Direction moveDirection,bool reverse)
    {
        Vector3 rotation = new Vector3(0, 0, 0);
        switch (moveDirection)
        {
            case Direction.Left:
                rotation = new Vector3(0, 180, 0);
                if (reverse)
                    rotation = new Vector3(0, -180, 0);
                break;
            case Direction.Down:
                rotation = new Vector3(-180, 0, 0);
                if (reverse)
                    rotation = new Vector3(180, 0, 0);
                break;
            case Direction.Right:
                rotation = new Vector3(0, -180, 0);
                if (reverse)
                    rotation = new Vector3(0, 180, 0);
                break;
            case Direction.Up:
                rotation = new Vector3(180, 0, 0); 
                if (reverse)
                    rotation = new Vector3(-180, 0, 0);
                break;
            case Direction.None:
                break;
        }
        return rotation;
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
        Vector2 startPosition = new Vector3(_scale.x * _startMatrix.x - 2, -_scale.y * _startMatrix.y + 2, 0);
        Vector3 rotation = GetDirectionRotation(_moveDirection, true);
        (int count,GameObject targetObject) = GetMultipleParentValue(gameObject);
        gameObject.transform.parent = targetObject.transform.parent;
        gameObject.transform.DOMove(startPosition, 1);
        _currentMatrix = _startMatrix;
        sandwiched = false;
        gameObject.transform.DORotate(rotation, 1, RotateMode.LocalAxisAdd).OnComplete(() =>
        {
            Managers.EventManager.TriggerEvent(Managers.EventManager.Listener.MoveDone.ToString());
        });
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Food/New Food", order = 1)]
public class Food : ScriptableObject
{
    public enum FoodNames 
    {
        Bread,
        Egg,
        Tomato,
        Lettuce,
        Cheese
    }

    public FoodNames foodName;
    public Material metarial;
}

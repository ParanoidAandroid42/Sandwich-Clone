using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeMapConfig
{
    public static IMapConfig[] mapCOnfig = new IMapConfig[] {
        new IMapConfig
        {
                row = 2,
                coloumn = 2,
                foodConfigs = new IFoodConfig[]{
                        new IFoodConfig {name= Food.FoodNames.Cheese, startMatrix = new Vector2(0, 0) },
                        new IFoodConfig{name= Food.FoodNames.Lettuce, startMatrix = new Vector2(1, 0) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(0, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(1, 1) },
                }
        },
        new IMapConfig
        {
                row = 3,
                coloumn = 3,
                foodConfigs = new IFoodConfig[]{
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(1, 0) },
                        new IFoodConfig{name= Food.FoodNames.Cheese, startMatrix = new Vector2(0, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(1, 1) },
                        new IFoodConfig{name= Food.FoodNames.Tomato, startMatrix = new Vector2(2, 1) },
                        new IFoodConfig{name= Food.FoodNames.Lettuce, startMatrix = new Vector2(1, 2) }
                }
        },
         new IMapConfig
        {
                row = 2,
                coloumn = 3,
                foodConfigs = new IFoodConfig[]{
                        new IFoodConfig{name= Food.FoodNames.Tomato, startMatrix = new Vector2(0, 0) },
                        new IFoodConfig{name= Food.FoodNames.Cheese, startMatrix = new Vector2(1, 0) },
                        new IFoodConfig{name= Food.FoodNames.Lettuce, startMatrix = new Vector2(2, 0) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(0, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(1, 1) },
                        new IFoodConfig{name= Food.FoodNames.Egg, startMatrix = new Vector2(2, 1) }
                }
        },
          new IMapConfig
        {
                row = 3,
                coloumn = 3,
                foodConfigs = new IFoodConfig[]{
                        new IFoodConfig{name= Food.FoodNames.Tomato, startMatrix = new Vector2(1, 0) },
                        new IFoodConfig{name= Food.FoodNames.Cheese, startMatrix = new Vector2(0, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(1, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(2, 1) },
                        new IFoodConfig{name= Food.FoodNames.Egg, startMatrix = new Vector2(1, 2) },
                        new IFoodConfig{name= Food.FoodNames.Lettuce, startMatrix = new Vector2(2, 2) }
                }
        },
            new IMapConfig
        {
                row = 4,
                coloumn = 2,
                foodConfigs = new IFoodConfig[]{
                        new IFoodConfig{name= Food.FoodNames.Cheese, startMatrix = new Vector2(0, 0) },
                        new IFoodConfig{name= Food.FoodNames.Egg, startMatrix = new Vector2(0, 1) },
                        new IFoodConfig{name= Food.FoodNames.Lettuce, startMatrix = new Vector2(1, 1) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(0, 2) },
                        new IFoodConfig{name= Food.FoodNames.Tomato, startMatrix = new Vector2(1, 2) },
                        new IFoodConfig{name= Food.FoodNames.Bread, startMatrix = new Vector2(0, 3) },
                        new IFoodConfig{name= Food.FoodNames.Egg, startMatrix = new Vector2(1, 3) }
                }
        }
    };
}

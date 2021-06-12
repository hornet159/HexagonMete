using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{

}

public enum HexagonType:int
{
    NORMAL,
    BOMB
}


public enum HexagonDirections:int
{
    Up = 0,
    UpRight = 1,
    DownRight = 2,
    Down = 3,
    DownLeft = 4,
    UpLeft = 5,
    Max = 6
}
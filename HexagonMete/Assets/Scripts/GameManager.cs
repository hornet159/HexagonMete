using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public HexagonGrid hexagonGrid;
    public TripleHexagonPointer tripleHexagonPointer;

    public GameOptions gameOptions;


    public void Awake()
    {
        hexagonGrid.Init(this);
        tripleHexagonPointer.Init(this);
    }

}

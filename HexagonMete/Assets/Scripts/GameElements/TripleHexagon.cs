using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleHexagon : CustomBehaviour
{
    public int index;
    public Vector2Int downCoor;
    public Vector2Int upCoor;
    public Vector2Int sideCoor;
    public bool isRight;

    public Hexagon downHexagon
    {
        get { return GameManager.hexagonGrid.GetHexagon(downCoor); }
    }

    public Hexagon upHexagon
    {
        get { return GameManager.hexagonGrid.GetHexagon(upCoor); }
    }

    public Hexagon sideHexagon
    {
        get { return GameManager.hexagonGrid.GetHexagon(sideCoor); }
    }

    public bool IsSameColor
    {
        get
        {
            return downHexagon.hexagonColor.index == upHexagon.hexagonColor.index && upHexagon.hexagonColor.index == sideHexagon.hexagonColor.index;
        }
    }

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
    }

    public void SetTripleHexagon(int _index, Hexagon _down, Hexagon _up, Hexagon _side, bool _isRight)
    {
        index = _index;
        downCoor = _down.coordinate;
        upCoor = _up.coordinate;
        sideCoor = _side.coordinate;
        isRight = _isRight;
        rectTransform.anchoredPosition = (_down.rectTransform.anchoredPosition + _up.rectTransform.anchoredPosition + _side.rectTransform.anchoredPosition) / 3;
    }

    public void SetHexagonsParent(Transform parent)
    {
        upHexagon.SetParent(parent);
        downHexagon.SetParent(parent);
        sideHexagon.SetParent(parent);
    }

    public void SwitchRotatedHexagons(bool isClockwise)
    {
        if (isRight && isClockwise || !isRight && !isClockwise)
        {
            GameManager.hexagonGrid.SwitchHexagons(downHexagon,sideHexagon);
            GameManager.hexagonGrid.SwitchHexagons(sideHexagon, upHexagon);
        }
        else 
        {
            GameManager.hexagonGrid.SwitchHexagons(downHexagon, upHexagon);
            GameManager.hexagonGrid.SwitchHexagons(upHexagon, sideHexagon);
        }
    }
}

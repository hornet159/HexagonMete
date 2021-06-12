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

    public List<TripleHexagon> mAllNeighborsTripleHexagon = new List<TripleHexagon>();

    public void Init(GameManager gameManager, int _index,Hexagon _down, Hexagon _up, Hexagon _side, bool _isRight)
    {
        Init(gameManager);
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

    public bool IsSameColor()
    {
        return downHexagon.hexagonColor.index == upHexagon.hexagonColor.index && upHexagon.hexagonColor.index == sideHexagon.hexagonColor.index;
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

    public void FindAllNeighbors()
    {
        mAllNeighborsTripleHexagon.Clear();
        List<TripleHexagon> upNeighbors = upHexagon.UsingTripleHexagons;
        List<TripleHexagon> sideNeighbors = sideHexagon.UsingTripleHexagons;
        List<TripleHexagon> downNeighbors = downHexagon.UsingTripleHexagons;

        foreach (var upItem in upNeighbors)
        {
            if (!mAllNeighborsTripleHexagon.Exists((x) => x.index == upItem.index))
                mAllNeighborsTripleHexagon.Add(upItem);
        }

        foreach (var sideItem in sideNeighbors)
        {
            if (!mAllNeighborsTripleHexagon.Exists((x) => x.index == sideItem.index))
                mAllNeighborsTripleHexagon.Add(sideItem);
        }

        foreach (var downItem in downNeighbors)
        {
            if (!mAllNeighborsTripleHexagon.Exists((x) => x.index == downItem.index))
                mAllNeighborsTripleHexagon.Add(downItem);
        }

        mAllNeighborsTripleHexagon.Remove(this);
    }



}

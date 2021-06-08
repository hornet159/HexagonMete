using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hexagon : CustomBehaviour,IPointerClickHandler
{
    public Image imageBase;
    public Vector2Int coordinate;
    public HexagonColor hexagonColor;
    public List<TripleHexagon> usingTripleHexagon;

    public bool isEvenHexagon
    {
        get
        {
            return coordinate.x % 2 == 0;
        }
    }

    public void Init(GameManager gameManager,Vector2Int _coordinate, Vector2 targetPosition)
    {
        Init(gameManager);

        rectTransform.anchoredPosition = targetPosition;
        coordinate = _coordinate;
        SetColor(GameManager.gameOptions.GetRandomColor());
    }

    public void SetColor(HexagonColor hexColor)
    {
        hexagonColor = hexColor;
        imageBase.color = hexColor.color;
    }

    public void AddTripleHexagon(TripleHexagon tripleHex)
    {
        usingTripleHexagon.Add(tripleHex);
    }
    public void ClearTripleHexagon()
    {
        usingTripleHexagon.Clear();
    }


    public Vector2Int GetNeighborCoordinate(HexagonDirections direction)
    {
        switch (direction)
        {
            case HexagonDirections.Up:
                return coordinate + Vector2Int.up;

            case HexagonDirections.UpRight:
                if (isEvenHexagon)
                    return coordinate + Vector2Int.right;
                else
                    return coordinate + Vector2Int.one;

            case HexagonDirections.DownRight:
                if (isEvenHexagon)
                    return coordinate + new Vector2Int(1, -1);
                else
                    return coordinate + Vector2Int.right;

            case HexagonDirections.Down:
                return coordinate + Vector2Int.down;

            case HexagonDirections.DownLeft:
                if (isEvenHexagon)
                    return coordinate - Vector2Int.one;
                else
                    return coordinate + Vector2Int.left;

            default: // HexagonDirections.UpLeft:
                if (isEvenHexagon)
                    return coordinate + Vector2Int.left;
                else
                    return coordinate + new Vector2Int(-1, 1);
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
 
        if (usingTripleHexagon != null)
        {
            TripleHexagon nearest = FindNearestTripleHexagon(eventData.pointerCurrentRaycast.worldPosition);
            GameManager.tripleHexagonPointer.SetPosition(nearest.rectTransform.anchoredPosition, nearest.isRight);
           

        }
    }

    private TripleHexagon FindNearestTripleHexagon(Vector2 clickPos)
    {
        float nearestSqrManetude = float.MaxValue;
        int index = 0;

        for (int i = 0; i < usingTripleHexagon.Count; i++)
        {
            float currentSqrMagnetude = (clickPos - (Vector2)usingTripleHexagon[i].transform.position).sqrMagnitude;

            if(nearestSqrManetude > currentSqrMagnetude)
            {
                nearestSqrManetude = currentSqrMagnetude;
                index = i;
            }
        }

        return usingTripleHexagon[index];
    }
}

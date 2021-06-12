using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Animations;

public class Hexagon : CustomBehaviour,IPointerClickHandler
{
    public Vector2Int coordinate;
    public HexagonColor hexagonColor;
    public RotationConstraint rotationConstraint;
    public ParticleSystem prefabExplodeEffect;
    public HexagonType hexagonType;

    [Header("Normal Hexagon Part")]
    public GameObject partNormal;
    public Image imageNormalBase;

    [Header("Bomb Hexagon Part")]
    public GameObject partBomb;
    public Image imageBombBase;
    public Text textBombCounter;
    private int mBombCounter;
    public int BombCounter
    {
        get { return mBombCounter; }
        set
        {
            mBombCounter = value;
            textBombCounter.text = value.ToString();
        }
    }

    [Header("Fall anim parameters")]
    public AnimationCurve fallAnimCurve;
    public float animTime = 0.35f;



    public List<TripleHexagon> UsingTripleHexagons
    {
        get { return GameManager.hexagonGrid.GetCoordinateTripleHexagons(coordinate); }
    }

    public bool isEvenHexagon
    {
        get { return coordinate.x % 2 == 0; }
    }

    public void Init(GameManager gameManager, Vector2Int coordinate, Vector2 startPositions, HexagonType type ,bool isStartColor)
    {
        Init(gameManager);
        SetHexagon(coordinate, startPositions, type, isStartColor);
    }

    public void SetHexagon(Vector2Int _coordinate, Vector2 startPositions, HexagonType type = HexagonType.NORMAL, bool isStartColor = false)
    {
        gameObject.SetActive(true);
        hexagonType = type;
        rectTransform.anchoredPosition = startPositions;
        coordinate = _coordinate;

        if (hexagonType == HexagonType.NORMAL)
        {
            partNormal.SetActive(true);
            partBomb.SetActive(false);
            GameManager.tripleHexagonPointer.ActionOnSuccesMove -= OnSuccesMove;
        }
        else if (hexagonType == HexagonType.BOMB)
        {
            partNormal.SetActive(false);
            partBomb.SetActive(true);
            GameManager.tripleHexagonPointer.ActionOnSuccesMove += OnSuccesMove;
            BombCounter = Random.Range(3,7);
            GameManager.BombCount++;
        }

        if (isStartColor)
            SetColor(GameManager.gameOptions.GetRandomColorWithoutColors(GetNeighborDoubleColorList()));
        else
            SetColor(GameManager.gameOptions.GetRandomColor());
    }

    public void SetColor(HexagonColor hexColor)
    {
        hexagonColor = hexColor;
       
        if(hexagonType == HexagonType.NORMAL)
        {
            imageNormalBase.color = hexColor.color;
        }
        else if(hexagonType == HexagonType.BOMB)
        {
            imageBombBase.color = hexColor.color;
        }
    }

 
    public List<HexagonColor> GetNeighborDoubleColorList()
    {
        List<HexagonColor> colors = new List<HexagonColor>();
        List<HexagonColor> doubleColors = new List<HexagonColor>();
        for (int i = 0; i < (int)HexagonDirections.Max; i++)
        {
            HexagonDirections direction = (HexagonDirections)i;
            Vector2Int coor = GetNeighborCoordinate(direction);
      
            if (GameManager.hexagonGrid.IsValidIndex(coor))
            {
                Hexagon hexagon = GameManager.hexagonGrid.GetHexagon(coor);

                if (hexagon == null)
                    continue;
                else
                    colors.Add(hexagon.hexagonColor);
            }
        }

       // return colors;

        for (int i = 0; i < colors.Count; i++)
        {
            if (!doubleColors.Exists((x)=>x.index == colors[i].index))
            {
                for (int j = 0; j < colors.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (colors[i].index == colors[j].index)
                        doubleColors.Add(colors[i]);
                }
            }
        }

        return doubleColors;
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
        if (UsingTripleHexagons != null)
        {
            TripleHexagon nearest = FindNearestTripleHexagon(eventData.pointerCurrentRaycast.worldPosition);
            GameManager.tripleHexagonPointer.SelectTripleHexagon(nearest);

        }
    }

    private TripleHexagon FindNearestTripleHexagon(Vector2 clickPos)
    {
        float nearestSqrManetude = float.MaxValue;
        int index = 0;

        List<TripleHexagon> tripleHexagons = UsingTripleHexagons;

        for (int i = 0; i < tripleHexagons.Count; i++)
        {
            float currentSqrMagnetude = (clickPos - (Vector2)tripleHexagons[i].transform.position).sqrMagnitude;

            if(nearestSqrManetude > currentSqrMagnetude)
            {
                nearestSqrManetude = currentSqrMagnetude;
                index = i;
            }
        }

        return tripleHexagons[index];
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        ConstraintSource cs = new ConstraintSource();
        cs.sourceTransform = parent;
        cs.weight = -1;
        rotationConstraint.SetSource(0,cs);
    }

    public void Explode()
    {
        var temp = prefabExplodeEffect.main;
        Color color = hexagonColor.color;
        color *= 0.8f;
        temp.startColor = color;
        Vector3 pos = transform.position;
        pos.z = 10;
        GameObject effect = Instantiate(prefabExplodeEffect, pos,Quaternion.identity).gameObject;
        Destroy(effect, temp.startLifetime.constant);
        gameObject.SetActive(false);
    }

    public IEnumerator IFallAnim()
    {
       // Debug.Log(GameManager.hexagonGrid.GetLowestEmptyArea(coordinate.x));
        Vector2Int newCoor = GameManager.hexagonGrid.GetLowestEmptyArea(coordinate.x);  //GetNeighborCoordinate(HexagonDirections.Down);
        Vector2 targetPosition = GameManager.hexagonGrid.GetHexagonPosition(newCoor);
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 moveDirection = targetPosition - startPosition;
       
        float timer = 0;
        float progress = 0;
        GameManager.hexagonGrid.SetHexagon(coordinate, null);
        GameManager.hexagonGrid.SetHexagon(newCoor, this);
        coordinate = newCoor;

        while (progress < 1)
        {
            progress = Mathf.Clamp01(timer / animTime);
            rectTransform.anchoredPosition = startPosition + (moveDirection * fallAnimCurve.Evaluate(progress));
            yield return null;
            timer += Time.deltaTime;
        }
    }

    #region Events
    public void OnSuccesMove()
    {
        BombCounter--;

        if(BombCounter == 0)
        {
            GameManager.GameOver();
        }
    }

    #endregion

}

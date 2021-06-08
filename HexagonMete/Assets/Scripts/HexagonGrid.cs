using UnityEngine;

public class HexagonGrid : CustomBehaviour
{
    public Hexagon prefabHexagon;
    public TripleHexagon prefabTripleHexagon;
    public RectTransform transformHexagonsParent;
    public RectTransform transformTripleHexagonsParent;

    [Tooltip("Grid dimention")]
    public Vector2Int gridSize = new Vector2Int(8,9);

    [Tooltip("Space between hexagons")]
    public int nodeSpace = 4;

    private Hexagon[,] mHexagons;
    private TripleHexagon[] mTripleHexagons;


    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        CreateGrid();
        FindTripleHexagons();
    }

    private void CreateGrid()
    {
        RectTransform prefabRectTransform = prefabHexagon.GetComponent<RectTransform>();
        Vector2 nodeSize = prefabRectTransform.sizeDelta + Vector2.one * nodeSpace;
        Vector2 GridLength = new Vector2(nodeSize.x * (0.25f + 0.75f * gridSize.x), nodeSize.y * ((gridSize.x > 1 ? 0.5f : 0) + gridSize.y));
        mHexagons = new Hexagon[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 nodePos = new Vector2((x * nodeSize.x * 0.75f), (((x % 2) * nodeSize.y * 0.5f) + (y * nodeSize.y)));
                Vector2 orginOfset = (GridLength - nodeSize) / 2;

                mHexagons[x, y] = Instantiate(prefabHexagon, transformHexagonsParent);
                mHexagons[x, y].Init(GameManager,new Vector2Int(x,y),nodePos - orginOfset);
            }
        }
    }

    private void FindTripleHexagons()
    {
        if (mHexagons == null)
            return;

        ClearAllTripleHexagons();

        mTripleHexagons = new TripleHexagon[(gridSize.x - 1) * (gridSize.y - 1) * 2];
        int index = 0;

        for (int y = 0; y < mHexagons.GetLength(1); y++)
        {
            for (int x = 0; x < mHexagons.GetLength(0); x++)
            {
                Hexagon currentHex = mHexagons[x, y];
                Vector2Int upHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.Up);
               

                if (IsThereHexagon(upHexCoor))
                {
                    Vector2Int UpLeftHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.UpLeft);
                    Vector2Int UpRightHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.UpRight);

                    if (IsThereHexagon(UpLeftHexCoor))
                    {
                        Hexagon upHex = GetHexagon(upHexCoor);
                        Hexagon upLeftHex = GetHexagon(UpLeftHexCoor);

                        mTripleHexagons[index] = Instantiate(prefabTripleHexagon, transformTripleHexagonsParent);
                        mTripleHexagons[index].Init(GameManager, currentHex, upHex, upLeftHex, false);


                        currentHex.AddTripleHexagon(mTripleHexagons[index]);
                        upHex.AddTripleHexagon(mTripleHexagons[index]);
                        upLeftHex.AddTripleHexagon(mTripleHexagons[index]);

                        index++;
                    }

                    if (IsThereHexagon(UpRightHexCoor))
                    {
                        Hexagon upHex = GetHexagon(upHexCoor);
                        Hexagon upRightHex = GetHexagon(UpRightHexCoor);

                        mTripleHexagons[index] = Instantiate(prefabTripleHexagon, transformTripleHexagonsParent);
                        mTripleHexagons[index].Init(GameManager,GetHexagon(mHexagons[x, y].coordinate), upHex, upRightHex, true);

                        currentHex.AddTripleHexagon(mTripleHexagons[index]);
                        upHex.AddTripleHexagon(mTripleHexagons[index]);
                        upRightHex.AddTripleHexagon(mTripleHexagons[index]);

                        index++;
                    }
                }
            }
        }

        Debug.Log("Index :"+index);
    }


    private void ClearAllTripleHexagons()
    {
        for (int y = 0; y < mHexagons.GetLength(1); y++)
        {
            for (int x = 0; x < mHexagons.GetLength(0); x++)
            {
                mHexagons[x, y].ClearTripleHexagon();
            }
        }
    }

    private bool IsThereHexagon(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.y >= 0 && coordinate.x < mHexagons.GetLength(0) && coordinate.y < mHexagons.GetLength(1);
    }

    private Hexagon GetHexagon(Vector2Int coordinate)
    {
        return mHexagons[coordinate.x, coordinate.y];
    }

}

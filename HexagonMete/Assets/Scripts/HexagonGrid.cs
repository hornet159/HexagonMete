using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class HexagonGrid : CustomBehaviour
{
    public Hexagon prefabHexagon;
    public MoveScore prefabMoveScore;
    public TripleHexagon prefabTripleHexagon;
    public RectTransform transformHexagonsParent;
    public RectTransform transformTripleHexagonsParent;
    public RectTransform transformStartLineParent;

    [Tooltip("Grid dimention")]
    public Vector2Int gridSize = new Vector2Int(8,9);

    [Tooltip("Space between hexagons")]
    public int nodeSpace = 4;

    private Hexagon[,] mHexagons;
    private Vector2[,] mHexagonsPositions;
    private TripleHexagon[] mTripleHexagons;
    private List<TripleHexagon>[,] mCoordinateToTripleHexagons;
    private Queue<Hexagon> mExplodedHexagons;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
        gridSize = GameManager.gameOptions.GridSize;
        nodeSpace = GameManager.gameOptions.HexagonSpace;
        CreateGrid();
        CreateTripleHexagons();
    }




    private void CreateGrid()
    {
        RectTransform prefabRectTransform = prefabHexagon.GetComponent<RectTransform>();
        Vector2 nodeSize = prefabRectTransform.sizeDelta + Vector2.one * nodeSpace;
        Vector2 GridLength = new Vector2(nodeSize.x * (0.25f + 0.75f * gridSize.x), nodeSize.y * ((gridSize.x > 1 ? 0.5f : 0) + gridSize.y));
        mHexagons = new Hexagon[gridSize.x, gridSize.y];
        mHexagonsPositions = new Vector2[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2 nodePos = new Vector2((x * nodeSize.x * 0.75f), (((x % 2) * nodeSize.y * 0.5f) + (y * nodeSize.y)));
                Vector2 orginOfset = (GridLength - nodeSize) / 2;
                Vector2 position = nodePos - orginOfset;
                mHexagons[x, y] = Instantiate(prefabHexagon, transformHexagonsParent);
                mHexagonsPositions[x, y] = position;
                mHexagons[x, y].Init(GameManager,new Vector2Int(x,y), position,HexagonType.NORMAL,true);

            }
        }

        if (!IsHaveMove())
            Debug.Log("GAME OVER");
    }


    private void CreateTripleHexagons()
    {
        if (mHexagons == null)
            return;

        mCoordinateToTripleHexagons = new List<TripleHexagon>[gridSize.x, gridSize.y];
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                mCoordinateToTripleHexagons[x, y] = new List<TripleHexagon>();
            }
        }

        mTripleHexagons = new TripleHexagon[(gridSize.x - 1) * (gridSize.y - 1) * 2];
        int index = 0;

        for (int y = 0; y < mHexagons.GetLength(1); y++)
        {
            for (int x = 0; x < mHexagons.GetLength(0); x++)
            {
                Hexagon currentHex = mHexagons[x, y];
                Vector2Int upHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.Up);
               

                if (IsValidIndex(upHexCoor))
                {
                    Vector2Int UpLeftHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.UpLeft);
                    Vector2Int UpRightHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.UpRight);

                    if (IsValidIndex(UpLeftHexCoor))
                    {
                        Hexagon upHex = GetHexagon(upHexCoor);
                        Hexagon upLeftHex = GetHexagon(UpLeftHexCoor);

                        mTripleHexagons[index] = Instantiate(prefabTripleHexagon, transformTripleHexagonsParent);
                        mTripleHexagons[index].Init(GameManager, index,currentHex, upHex, upLeftHex, false);


                        mCoordinateToTripleHexagons[x, y].Add(mTripleHexagons[index]);
                        mCoordinateToTripleHexagons[upHexCoor.x, upHexCoor.y].Add(mTripleHexagons[index]);
                        mCoordinateToTripleHexagons[UpLeftHexCoor.x, UpLeftHexCoor.y].Add(mTripleHexagons[index]);

                        index++;
                    }

                    if (IsValidIndex(UpRightHexCoor))
                    {
                        Hexagon upHex = GetHexagon(upHexCoor);
                        Hexagon upRightHex = GetHexagon(UpRightHexCoor);

                        mTripleHexagons[index] = Instantiate(prefabTripleHexagon, transformTripleHexagonsParent);
                        mTripleHexagons[index].Init(GameManager,index,GetHexagon(mHexagons[x, y].coordinate), upHex, upRightHex, true);
                        
                        mCoordinateToTripleHexagons[x, y].Add(mTripleHexagons[index]);
                        mCoordinateToTripleHexagons[upHexCoor.x, upHexCoor.y].Add(mTripleHexagons[index]);
                        mCoordinateToTripleHexagons[UpRightHexCoor.x, UpRightHexCoor.y].Add(mTripleHexagons[index]);

                        index++;
                    }
                }
            }
        }

        for (int i = 0; i < mTripleHexagons.Length; i++)
        {
            mTripleHexagons[i].FindAllNeighbors();
        }
    }

    public void SwitchHexagons(Hexagon a,Hexagon b)
    {
        mHexagons[b.coordinate.x, b.coordinate.y] = a;
        mHexagons[a.coordinate.x, a.coordinate.y] = b;

        Vector2Int tempCoor = a.coordinate;
        a.coordinate = b.coordinate;
        b.coordinate = tempCoor;

    }

    private void CreateGruopScorePopup(Vector3 pos, int groupScore)
    {
        Instantiate(prefabMoveScore,pos,Quaternion.identity,transformHexagonsParent).SetScoreText(groupScore.ToString());
    }


    public void RemoveExplodedHexagons(List<List<Hexagon>> explodedHexagons)
    {
        mExplodedHexagons = new Queue<Hexagon>();
        foreach (var hexGroup in explodedHexagons)
        {
            Vector3 scorePos = Vector3.zero;
            foreach (var item in hexGroup)
            {
                item.SetParent(transformHexagonsParent);
                mHexagons[item.coordinate.x, item.coordinate.y] = null;

                scorePos += item.rectTransform.position;

                mExplodedHexagons.Enqueue(item);

            }

            scorePos /= hexGroup.Count;
            scorePos.z = 10;
            GameManager.AddScore(hexGroup.Count);
            CreateGruopScorePopup(scorePos, hexGroup.Count*GameManager.gameOptions.HexagonPoint);
  
        }
    }

    public void ExplodeAll()
    {
        for (int y = 0; y < mHexagons.GetLength(1); y++)
        {
            for (int x = 0; x < mHexagons.GetLength(0); x++)
            {
                mHexagons[x, y].Explode();
            }
        }
    }

    public IEnumerator IAllHexagonFallAnim()
    {
        while (true)
        {
            Coroutine lastAnim = null;

            int max = mHexagons.GetLength(1) + mHexagons.GetLength(0) - 1;
            int hexCount = 0;
            for (int a = 0; a < max; a++)
            {
                int y = a;
                for (int x = 0; x < mHexagons.GetLength(0); x++, y = a - x)
                {
                    if (IsValidIndex(new Vector2Int(x, y)))
                    {
                        hexCount++;
                        Hexagon currentHex = mHexagons[x, y];

                        if (currentHex == null)
                            continue;

                        Vector2Int downHexCoor = currentHex.GetNeighborCoordinate(HexagonDirections.Down);

                        if (IsValidIndex(downHexCoor))
                        {
                            Hexagon downHex = GetHexagon(downHexCoor);

                            if (downHex == null)
                            {
                                lastAnim = StartCoroutine(currentHex.IFallAnim());
                                yield return new WaitForSeconds(0.05f);
                            }
                        }
                    }
                }
            }

            hexCount = 0;
            for (int a = 0; a < max; a++)
            {
                int y = a;
                for (int x = 0; x < mHexagons.GetLength(0); x++, y = a - x)
                {
                    if (IsValidIndex(new Vector2Int(x, y)))
                    {
                        hexCount++;
                        Hexagon currentHex = mHexagons[x, y];
                        // Debug.Log(x + "," + y + "currentHex is null: " + (currentHex == null) +" count> 0 :"+ (mExplodedHexagons.Count > 0));
                        if (currentHex == null && mExplodedHexagons.Count > 0)
                        {
                            Hexagon newHexagon = mExplodedHexagons.Dequeue();
                            Vector2 pos = mHexagonsPositions[x, y];
                            newHexagon.rectTransform.position = transformStartLineParent.position;
                            newHexagon.SetHexagon(new Vector2Int(x, y + 1), new Vector2(pos.x, newHexagon.rectTransform.anchoredPosition.y),GameManager.isBombComing? HexagonType.BOMB: HexagonType.NORMAL);

                            lastAnim = StartCoroutine(newHexagon.IFallAnim());
                            yield return new WaitForSeconds(0.05f);
                        }
                    }

                   
                }
            }

            yield return lastAnim;

            List<List<Hexagon>> newExplodedHexagons = GameManager.hexagonGrid.ControlAllTripleHexagonsList();

            if (newExplodedHexagons.Count > 0)
            {
                yield return new WaitForSeconds(0.2f);

                foreach (var hexGroupList in newExplodedHexagons)
                {
                    foreach (var item in hexGroupList)
                    {
                        item.Explode();
                    }
                }

                GameManager.hexagonGrid.RemoveExplodedHexagons(newExplodedHexagons);
            }
            else
                break;

        }

        if (IsHaveMove())
        {
            GameManager.tripleHexagonPointer.ReselectCurentTripleHexagon();
        }
        else
            GameManager.GameOver();
        
    }

    public List<List<Hexagon>> ControlAllTripleHexagonsList()
    {
        return ControlTripleHexagonsList(new List<TripleHexagon>(mTripleHexagons));
    }

    public bool IsHaveMove()
    {
        bool isHaveMove = false;
        int index = 0;

        for (int y = 0; y<mHexagons.GetLength(1); y++)
        {
            for (int x = 0; x < mHexagons.GetLength(0); x++)
            {
                index++;
                Hexagon currentHex = mHexagons[x, y];
                
                List<int> colorsIndex = new List<int>();

                colorsIndex.Add(currentHex.hexagonColor.index);

                for (int i = 0; i < (int)HexagonDirections.Max; i++)
                {
                    HexagonDirections direction = (HexagonDirections)i;
                    Vector2Int coor = currentHex.GetNeighborCoordinate(direction);

                    if (IsValidIndex(coor))
                    {
                        Hexagon hexagon = GetHexagon(coor);

                        if (hexagon == null)
                            continue;
                        else
                            colorsIndex.Add(hexagon.hexagonColor.index);
                    }
                }

                for (int i = 0; i < colorsIndex.Count; i++)
                {
                    int currentValue = colorsIndex[i];
                    if (colorsIndex.FindAll((x) => x == currentValue).Count >= 3)
                    {
                        if(colorsIndex.Count >= 6 && i+2<colorsIndex.Count && i+4<colorsIndex.Count )
                        {
                            bool isTrianglePos = currentValue == colorsIndex[i + 2] && currentValue == colorsIndex[i + 4];

                            if (!isTrianglePos)
                            {
                                isHaveMove = true;
                                Debug.Log("!THERE ARE MOVE LOOK AT AROUND : [" + x + "," + y + "]     Debug => count: " + colorsIndex.Count + " index: " + i + " array:" + string.Join(";", colorsIndex.Select(x => x.ToString()).ToArray()));
                            }

                        }
                        else
                        {
                            isHaveMove = true;
                            Debug.Log("!THERE ARE MOVE LOOK AT AROUND : [" + x + "," + y + "]     Debug => count: " + colorsIndex.Count + " index: " + i + " array:" + string.Join(";", colorsIndex.Select(x => x.ToString()).ToArray()));
                        }
                           

                        

                        break;
                    }
                }

                if (isHaveMove)
                    break;
            }

            if (isHaveMove)
                break;
        }
  
        return isHaveMove;
    }


    public List<List<Hexagon>> ControlTripleHexagonsList(List<TripleHexagon> tripleHexList)
    {
        List<List<Hexagon>> sameColorHexagonsList = new List<List<Hexagon>>();

        for (int i = 0; i < tripleHexList.Count; i++)
        {
            if (tripleHexList[i].IsSameColor())
            {
                Hexagon upHex = tripleHexList[i].upHexagon;
                Hexagon downHex = tripleHexList[i].downHexagon;
                Hexagon sideHex = tripleHexList[i].sideHexagon;

                bool isNewGroup = true;

                foreach (var item in sameColorHexagonsList)
                {
                    bool isExistUp = item.Exists((x) => x.coordinate == upHex.coordinate);
                    bool isExistDown = item.Exists((x) => x.coordinate == downHex.coordinate);
                    bool isExistSide = item.Exists((x) => x.coordinate == sideHex.coordinate);

                    if (isExistDown || isExistUp || isExistSide)
                    {
                        isNewGroup = false;

                        if (!isExistUp) 
                            item.Add(upHex);
 
                        if (!isExistDown)
                            item.Add(downHex);

                        if (!isExistSide)
                            item.Add(sideHex);

                        break;
                    }
                }

                if(isNewGroup) // New group
                    sameColorHexagonsList.Add(new List<Hexagon>() { upHex, downHex, sideHex });
                
            }
        }

        return sameColorHexagonsList;
    }


    public bool IsValidIndex(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.y >= 0 && coordinate.x < mHexagons.GetLength(0) && coordinate.y < mHexagons.GetLength(1);
    }

    public Hexagon GetHexagon(Vector2Int coordinate)
    {
        return mHexagons[coordinate.x, coordinate.y];
    }

    public void SetHexagon(Vector2Int coordinate, Hexagon hexagon)
    {
        if (IsValidIndex(coordinate))
            mHexagons[coordinate.x, coordinate.y] = hexagon;
    }

    public List<TripleHexagon> GetCoordinateTripleHexagons(Vector2Int coordinate)
    {
        return mCoordinateToTripleHexagons[coordinate.x,coordinate.y];
    }

    public Vector2 GetHexagonPosition(Vector2Int coordinate)
    {
        return mHexagonsPositions[coordinate.x, coordinate.y];
    }

    public Vector2Int GetLowestEmptyArea(int x)
    {
        for (int y = 0; y < mHexagons.GetLength(1); y++)
        {
            if (mHexagons[x, y] == null)
                return new Vector2Int(x, y);
        }

        return -Vector2Int.one;
    }




}

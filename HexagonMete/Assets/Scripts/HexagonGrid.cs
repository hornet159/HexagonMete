using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public GameObject PrefabHexagon;
    public RectTransform TransformGridParent;

    [Tooltip("Grid dimention")]
    public Vector2Int GridSize = new Vector2Int(8,9);

    [Tooltip("Space between hexagons")]
    public int NodeSpace = 4;

    // Start is called before the first frame update
    void Start()
    {
        CreateHexagonGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateHexagonGrid()
    {
        RectTransform prefabRectTransform = PrefabHexagon.GetComponent<RectTransform>();
        Vector2 nodeSize = prefabRectTransform.sizeDelta + Vector2.one * NodeSpace;
        Vector2 GridLength = new Vector2(nodeSize.x * (0.25f + 0.75f * GridSize.x), nodeSize.y * ((GridSize.x > 1 ? 0.5f : 0) + GridSize.y));

        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                Vector2 nodePos = new Vector2((x * nodeSize.x * 0.75f), (((x % 2) * nodeSize.y * 0.5f) + (y * nodeSize.y)));
                Vector2 orginOfset = (GridLength - nodeSize) / 2;  

                Instantiate(PrefabHexagon, TransformGridParent).GetComponent<RectTransform>().anchoredPosition = nodePos - orginOfset;
            }
        }
    }

}

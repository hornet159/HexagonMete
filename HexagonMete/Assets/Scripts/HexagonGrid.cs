using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public GameObject PrefabHexagon;
    public RectTransform TransformGridParent;

    public int GridX = 8;
    public int GridY = 9;
    public int NodeSpace = 4;

    public Vector2 NodeSize;



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

        float prefabWidth = prefabRectTransform.sizeDelta.x+ NodeSpace;
        float prefabHeight = prefabRectTransform.sizeDelta.y+ NodeSpace;

        Vector2 GridMagnetude = new Vector2(prefabWidth*(0.25f+0.75f*GridX), prefabHeight*(0.5f+GridY));

        Debug.Log("nodeSizeX : "+ GridMagnetude.x + "  nodeSizeY : "+ GridMagnetude.y);

        for (int y = 0; y < GridY; y++)
        {
            for (int x = 0; x < GridX; x++)
            {

                float posX = (x * prefabWidth * 0.75f) - ((GridMagnetude.x-prefabWidth)/2);
                float posY = (((x % 2) * prefabHeight * 0.5f) + (y * prefabHeight)) - ((GridMagnetude.y-prefabHeight)/2);

            
                Instantiate(PrefabHexagon, TransformGridParent).GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, posY);
            }
        }


    }

}

using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "Options", menuName = "Options")]
public class GameOptions : ScriptableObject
{
    public HexagonColor[] HexagonsColors;
    public int HexagonPoint;
    public int PointPerBomb;
    public Vector2Int GridSize = new Vector2Int(8,9);
    public int HexagonSpace = 4;


    public HexagonColor GetRandomColor()
    {
        if (HexagonsColors != null)
            return HexagonsColors[Random.Range(0, HexagonsColors.Length)];
        else
            return HexagonColor.white;
    }

    public HexagonColor GetRandomColorWithoutColors(List<HexagonColor> withoutColors)
    {
        if (HexagonsColors != null)
        {
            List<HexagonColor> newColorList = new List<HexagonColor>();

            for (int i = 0; i < HexagonsColors.Length; i++)
            {
                if (!withoutColors.Exists((x)=> x.index == HexagonsColors[i].index))
                    newColorList.Add(HexagonsColors[i]);
            }

            return newColorList[Random.Range(0, newColorList.Count)];
        }
        else
            return HexagonColor.white;
    }

}


[System.Serializable]
public class HexagonColor
{
    public int index;
    public Color color;

    public static HexagonColor white
    {
        get { return new HexagonColor { index = 0, color = Color.white }; }
    }
}

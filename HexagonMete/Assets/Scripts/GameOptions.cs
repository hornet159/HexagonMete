using UnityEngine;


[CreateAssetMenu(fileName = "Options", menuName = "Options")]
public class GameOptions : ScriptableObject
{
    public HexagonColor[] HexagonsColors; 

    public HexagonColor GetRandomColor()
    {
        if (HexagonsColors != null)
            return HexagonsColors[Random.Range(0, HexagonsColors.Length)];
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

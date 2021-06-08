using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TripleHexagon : CustomBehaviour
{
    public Hexagon down;
    public Hexagon up;
    public Hexagon side;
    public bool isRight;

    public void Init(GameManager gameManager, Hexagon _down, Hexagon _up, Hexagon _side, bool _isRight)
    {
        Init(gameManager);

        down = _down;
        up = _up;
        side = _side;
        isRight = _isRight;

        rectTransform.anchoredPosition = (down.rectTransform.anchoredPosition + up.rectTransform.anchoredPosition + side.rectTransform.anchoredPosition) / 3;
    }
}

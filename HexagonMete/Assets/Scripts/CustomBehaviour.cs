using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour
{
    public GameManager GameManager { get; set; }

    public virtual void  Init(GameManager gameManager)
    {
        GameManager = gameManager;
    }


    private RectTransform mRectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (mRectTransform == null)
                mRectTransform = GetComponent<RectTransform>();

            return mRectTransform;
        }
    }
}

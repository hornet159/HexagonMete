using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TripleHexagonPointer : CustomBehaviour
{
    public GameObject gameObjectPointer;


    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
        gameObjectPointer.SetActive(false);

    }

    public void SetPosition(Vector2 position, bool isRight)
    {
        gameObjectPointer.SetActive(true);
        rectTransform.anchoredPosition = position;
        rectTransform.eulerAngles = isRight ? Vector3.zero : Vector3.forward * 60;
    }





    // Update is called once per frame
    void Update()
    {



            //  Vector3 screenPoint = Input.mousePosition;
            //  screenPoint.z = 10.0f;
            //     transform.position = Camera.main.screet(screenPoint);

            // Debug.Log(Input.mousePosition + mouseOrginOfset);

            //   Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(screenPoint), ve)
        


        // Physics2D.RaycastAll()
    }
}

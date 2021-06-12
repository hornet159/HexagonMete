using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TripleHexagonPointer : CustomBehaviour
{
    public event Action ActionOnSuccesMove;

    public GameObject gameObjectPointer;
    public Transform selectedHexagonsParent;
    public Transform transfromBorder;
    public Touch touch;
    public float touchDistanceSqr = 4;

    [Header("RotateAnim Parameters")]
    [Tooltip("Animation max total time is three times that. Its for only 120 degre")]
    public float rotateStepTime = 0.15f;

    private TripleHexagon mCurrentTripleHexagon;
    private bool mIsRotateAnimActive;
    private bool mIsRotateAnimCanPlay;
    private bool mIsClockwise;
    private Vector2 mVectorOrginToTouch;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
        Input.simulateMouseWithTouches = true;
        touch.actionOnTouchDelta += OnTouchDelta;
        touch.actionOnTouchDown += OnTouchDown;
    }

    public void SetPointerVisibility(bool isVisibale)
    {
        gameObjectPointer.SetActive(isVisibale);
    }

    public void SelectTripleHexagon(TripleHexagon tripleHexagon)
    {
        if (mIsRotateAnimActive || !gameObjectPointer.activeSelf)
            return;

        if (mCurrentTripleHexagon != null)
            mCurrentTripleHexagon.SetHexagonsParent(GameManager.hexagonGrid.transformHexagonsParent);

        SetPointerVisibility(true);
        SetPosition(tripleHexagon.rectTransform.anchoredPosition, tripleHexagon.isRight);
        tripleHexagon.SetHexagonsParent(selectedHexagonsParent);
        mCurrentTripleHexagon = tripleHexagon;
    }

    /// <summary>
    /// Reseletct last selected TripleHexagon
    /// </summary>
    public void ReselectCurentTripleHexagon()
    {
        if (mCurrentTripleHexagon != null)
        {
            gameObjectPointer.SetActive(true);
            mCurrentTripleHexagon.SetHexagonsParent(GameManager.hexagonGrid.transformHexagonsParent);
            SetPointerVisibility(true);
            SetPosition(mCurrentTripleHexagon.rectTransform.anchoredPosition, mCurrentTripleHexagon.isRight);
            mCurrentTripleHexagon.SetHexagonsParent(selectedHexagonsParent);
        }
    }

    private void SetPosition(Vector3 anchoredPos, bool isRight)
    {
        rectTransform.anchoredPosition = anchoredPos;
        transfromBorder.eulerAngles = isRight ? Vector3.zero : Vector3.forward * 60;
    }

    public IEnumerator IRotateAnim(bool isClockwise)
    {
        mIsRotateAnimActive = true;
        float rotateAngle = 120 * (isClockwise ? -1 : 1);
        float timer = 0;
        float progress;
        List<List<Hexagon>> explodedHexGroups;
        Vector3 startAnlge;

        for (int i = 0; i < 3; i++)
        {
            timer = timer % rotateStepTime;
            progress = 0;
            startAnlge = transform.eulerAngles;
            while (progress < 1)
            {
                progress = Mathf.Clamp01(timer / rotateStepTime);
                transform.eulerAngles = new Vector3(0, 0, startAnlge.z + (rotateAngle * progress));
                yield return null;
                timer += Time.deltaTime;
            }

            mCurrentTripleHexagon.SwitchRotatedHexagons(isClockwise);
            explodedHexGroups = GameManager.hexagonGrid.ControlAllTripleHexagonsList();

            if (explodedHexGroups.Count > 0) // hexagons explode and rotate anim finish
            {
                SetPointerVisibility(false);
                ActionOnSuccesMove?.Invoke();

                if (GameManager.IsGameOver)
                {
                    yield break;
                }

                yield return new WaitForSeconds(0.1f);
       
                mCurrentTripleHexagon.SetHexagonsParent(GameManager.hexagonGrid.transformHexagonsParent);

                foreach (var hexGroupList in explodedHexGroups)
                {
                    foreach (var item in hexGroupList)
                    {
                        item.Explode();
                    }
                }

                GameManager.hexagonGrid.RemoveExplodedHexagons(explodedHexGroups);

                yield return StartCoroutine(GameManager.hexagonGrid.IAllHexagonFallAnim());

                break;
            }
              
        }
       

        mIsRotateAnimActive = false;
    }

    #region Events

    private void OnTouchDelta(Vector2 delta)
    {
        if (delta.sqrMagnitude > 1f && mIsRotateAnimCanPlay)
        {
            mIsClockwise = Vector3.Cross(mVectorOrginToTouch, delta).z < 0;
            StartCoroutine(IRotateAnim(mIsClockwise));
            mIsRotateAnimCanPlay = false;
        }
    }

    private void OnTouchDown(Vector2 position)
    {
        Vector2 touchWordPos = Camera.main.ScreenToWorldPoint(position);
        mVectorOrginToTouch = touchWordPos - (Vector2)rectTransform.position;
        bool isNearTouch = mVectorOrginToTouch.sqrMagnitude < touchDistanceSqr;
        mIsRotateAnimCanPlay = isNearTouch && !mIsRotateAnimActive;
    }

    #endregion
}

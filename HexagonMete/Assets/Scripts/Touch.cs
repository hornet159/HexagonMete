using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour,IPointerMoveHandler,IPointerDownHandler,IPointerUpHandler
{
    public event Action<Vector2> actionOnTouchDelta;
    public event Action<Vector2> actionOnTouchDown;
    public event Action<Vector2> actionOnTouchUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        actionOnTouchDown?.Invoke(eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.eligibleForClick)
        {
            actionOnTouchDelta?.Invoke(eventData.delta);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        actionOnTouchUp?.Invoke(eventData.position);
    }
}


